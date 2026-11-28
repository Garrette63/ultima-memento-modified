using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Server.Accounting;
using Server.Commands;

namespace Server.Misc
{
	/// <summary>
	/// [UpdateServer - one-stop shard update:
	///   1. runs `git pull` in the repository base folder (the parent of the World folder),
	///   2. rebuilds the core with the toolchain that matches the *server's* operating system
	///      (mcs via Source/Tools/compile-world-linux.sh on Linux/Unix, csc via
	///      Source/Tools/compile-world-win.bat on Windows) so Source/System changes take effect,
	///   3. restarts the server so the pulled scripts are recompiled and loaded,
	///   4. runs [buildworld once the new server has finished starting up.
	///
	/// The *client's* operating system does not matter: a UO client never reports it, and the
	/// compiler always runs on the shard host.  The command therefore picks the toolchain from the
	/// host OS (Core.Unix / Environment.OSVersion) and reports which one it used in-game and in the
	/// log, so Windows-hosted and Linux-hosted shards both work unchanged.
	///
	/// The post-restart rebuild is driven by a marker file (RebuildWorldAfterRestart.flag) that is
	/// written before the restart and picked up by Initialize() on the next start.  Progress and
	/// command output are written to Data/Logs/UpdateServer.log and to the server console.
	///
	/// Optional arguments:
	///   nocompile - skip the core rebuild (script changes are still compiled on startup).
	///   nobuild   - restart without running [buildworld afterwards.
	/// </summary>
	public class UpdateServer
	{
		private const string MarkerFile = "Data/Logs/RebuildWorldAfterRestart.flag";
		private const string LogFile = "Data/Logs/UpdateServer.log";
		private const string RestartScriptFile = "Data/Logs/UpdateServer-restart.bat";

		// On Windows csc cannot overwrite the running World.exe, so the core is built to a staging
		// file and the restart wrapper swaps it in after this process has exited.
		private const string WindowsStagingFile = "World.exe.new";
		private const string WindowsStagingArgument = "..\\..\\World.exe.new";

		private const int ProcessTimedOut = -1;
		private const int ProcessStartFailed = -2;
		private const int BuildNotProduced = -3;

		private static readonly TimeSpan PullTimeout = TimeSpan.FromMinutes( 5.0 );
		private static readonly TimeSpan CompileTimeout = TimeSpan.FromMinutes( 15.0 );
		private static readonly TimeSpan RestartDelay = TimeSpan.FromSeconds( 5.0 );
		private static readonly TimeSpan ReplaceDelay = TimeSpan.FromSeconds( 2.0 );
		private static readonly TimeSpan ShutdownWatchdog = TimeSpan.FromSeconds( 20.0 );
		private const int ReplacementWaitSeconds = 90;
		private static readonly TimeSpan BuildDelay = TimeSpan.FromSeconds( 60.0 );
		private static readonly TimeSpan RecentBuildWindow = TimeSpan.FromMinutes( 5.0 );
		private static readonly TimeSpan StaleMarkerAge = TimeSpan.FromDays( 1.0 );

		private static bool m_Running;

		public static void Initialize()
		{
			CommandSystem.Register( "UpdateServer", AccessLevel.Administrator, new CommandEventHandler( UpdateServer_OnCommand ) );
			CommandSystem.Register( "PullAndRebuild", AccessLevel.Administrator, new CommandEventHandler( UpdateServer_OnCommand ) );

			ResumePendingBuild();
		}

		#region Command

		[Usage( "UpdateServer [ff|rebase] [nocompile] [nobuild]" )]
		[Description( "Pulls the latest code from git, rebuilds the core with the toolchain for this server's operating system (mcs on Linux/Unix, csc on Windows), restarts the server, then runs [buildworld once the shard is back up.  Upstream is merged by default; pass 'ff' for fast-forward-only or 'rebase' to rebase instead.  'nocompile' skips the core rebuild, 'nobuild' skips the world rebuild." )]
		public static void UpdateServer_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( m_Running )
			{
				if ( from != null )
					from.SendMessage( 0x22, "A server update is already running." );

				return;
			}

			bool compile = true;
			bool build = true;
			string strategy = "merge";

			for ( int i = 0; i < e.Arguments.Length; ++i )
			{
				string arg = ( e.Arguments[i] == null ? "" : e.Arguments[i].Trim().ToLower() );

				if ( arg.Length == 0 )
					continue;

				if ( arg == "compile" )
					compile = true;
				else if ( arg == "nocompile" || arg == "nocore" )
					compile = false;
				else if ( arg == "nobuild" || arg == "norebuild" )
					build = false;
				else if ( arg == "ff" || arg == "ff-only" || arg == "ffonly" )
					strategy = "ff";
				else if ( arg == "rebase" )
					strategy = "rebase";
				else if ( arg == "merge" || arg == "norebase" )
					strategy = "merge";
				else
				{
					if ( from != null )
						from.SendMessage( 0x22, "Format: [UpdateServer [ff|rebase] [nocompile] [nobuild]" );

					return;
				}
			}

			string requestedBy = ( from != null && from.Name != null && from.Name.Length > 0 ) ? from.Name : "the console";
			string platform = GetHostPlatform();

			m_Running = true;

			string start = string.Format( "{0} started a server update on {1} (pull [{2}]{3}{4}).  Pulling the latest code...",
				requestedBy,
				platform,
				strategy,
				compile ? " + core build" : "",
				build ? ", then restart and rebuild" : ", then restart" );

			Console.WriteLine( "[UpdateServer] {0}", start );
			Log( start );
			Log( string.Format( "Host OS: {0} ({1})", Environment.OSVersion, platform ) );

			string saveError;

			if ( !SaveWorld( out saveError ) )
			{
				m_Running = false;

				string message = "Server update aborted: the world could not be saved (" + saveError + ").";

				Console.WriteLine( "[UpdateServer] {0}", message );
				Log( message );

				if ( from != null && !from.Deleted )
					from.SendMessage( 0x22, message );

				return;
			}

			World.Broadcast( 0x35, true, "Server update started by {0}.  The shard will restart shortly.", requestedBy );

			UpdateJob job = new UpdateJob();
			job.From = from;
			job.RequestedBy = requestedBy;
			job.Compile = compile;
			job.Build = build;
			job.PullStrategy = strategy;

			ThreadPool.QueueUserWorkItem( new WaitCallback( PullWorker ), job );
			new UpdatePoller( job ).Start();
		}

		#endregion

		#region Update worker (background thread) + poller

		private class UpdateJob
		{
			public Mobile From;
			public string RequestedBy;
			public bool Compile;
			public bool Build;
			public string PullStrategy = "merge";

			public string Stage = "starting";
			public string Output = "";
			public string Failure = "";
			public bool Success;
			public volatile bool Done;
		}

		private class UpdatePoller : Timer
		{
			private UpdateJob m_Job;

			public UpdatePoller( UpdateJob job ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				Priority = TimerPriority.OneSecond;
				m_Job = job;
			}

			protected override void OnTick()
			{
				if ( !m_Job.Done )
					return;

				Stop();
				FinishJob( m_Job );
			}
		}

		private class ReplaceTimer : Timer
		{
			public ReplaceTimer() : base( RestartDelay )
			{
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				ReplaceServer();
			}
		}

		private class BuildTimer : Timer
		{
			public BuildTimer() : base( BuildDelay )
			{
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				ResumeBuild();
			}
		}

		private static void PullWorker( object state )
		{
			UpdateJob job = (UpdateJob)state;

			try
			{
				string repo = GetRepositoryRoot();

				if ( repo == null )
				{
					job.Failure = "this server is not inside a git repository (no .git folder above the World folder)";
					return;
				}

				job.Stage = "git pull";

				// Never let a staged binary left over from an earlier attempt be swapped in later.
				DeleteStagingBinary();

				string pullArgs = BuildPullArguments( job.PullStrategy );

				Console.WriteLine( "[UpdateServer] git pull [{0}] in {1}", job.PullStrategy, repo );

				// No `-C <path>` here on purpose: the working directory already is the repository,
				// and passing a path would drag OS-specific argument quoting into the pull (POSIX
				// single quotes are taken literally by git on Windows).
				string output;
				int code = RunProcess( "git", pullArgs, repo, PullTimeout, out output );

				// Some Unix images have git outside the server's PATH; Windows keeps its own error.
				if ( code == ProcessStartFailed && IsUnixHost() )
					code = RunProcess( "/usr/bin/git", pullArgs, repo, PullTimeout, out output );

				job.Output = output;

				if ( code != 0 )
				{
					job.Failure = ( code == ProcessTimedOut )
						? "git pull timed out"
						: string.Format( "git pull failed (exit code {0})", code );

					return;
				}

				if ( job.Compile )
				{
					job.Stage = "core build";

					string tools = Path.Combine( GetWorldDirectory(), "Source/Tools" );

					Console.WriteLine( "[UpdateServer] rebuilding the core with the {0} toolchain in {1}", GetHostPlatform(), tools );

					code = CompileCore( out output );

					job.Output = job.Output + output;

					if ( code != 0 )
					{
						job.Failure = "core build failed: " + DescribeCompileFailure( code ) + "; the server was NOT restarted";
						return;
					}
				}

				job.Success = true;
			}
			catch ( Exception ex )
			{
				job.Failure = ex.Message;
			}
			finally
			{
				job.Done = true;
			}
		}

		private static void FinishJob( UpdateJob job )
		{
			Log( job.Output );

			if ( !job.Success )
			{
				Abort( job, "Server update aborted during " + job.Stage + ": " + job.Failure );
				return;
			}

			// The marker has to be in place *before* the shard is told it is restarting: otherwise a
			// failed marker write would leave everyone waiting for a restart that never happens.
			if ( job.Build && !WriteMarker( job.RequestedBy ) )
			{
				Abort( job, "Server update aborted: the world rebuild marker could not be written." );
				return;
			}

			string saveError;

			if ( !SaveWorld( out saveError ) )
			{
				DeleteMarker();
				DeleteStagingBinary();

				Abort( job, "Server update aborted: the world could not be saved (" + saveError + ")." );
				return;
			}

			string summary = job.Compile ? "Pulled the latest code and rebuilt the core." : "Pulled the latest code.";
			summary += job.Build ? "  Restarting now, then the world will be rebuilt." : "  Restarting now.";

			Console.WriteLine( "[UpdateServer] {0}", summary );
			Log( summary );
			World.Broadcast( 0x35, true, summary );

			// m_Running stays true until the process is replaced.
			new ReplaceTimer().Start();
		}

		private static void Abort( UpdateJob job, string message )
		{
			m_Running = false;

			string hint = OutputHint( job.Output, 300 );
			string detail = ( hint.Length == 0 ? "" : " :: " + hint );

			Console.WriteLine( "[UpdateServer] {0}{1}", message, detail );
			Log( message + detail );

			if ( job.From == null || job.From.Deleted )
				return;

			job.From.SendMessage( 0x22, message );

			// The git/compiler complaint itself, so the admin does not have to open the log.
			if ( hint.Length > 0 )
				job.From.SendMessage( 0x22, hint );
		}

		/// <summary>
		/// Saves the world, reporting (instead of throwing) when it cannot be done - a restart on top
		/// of a failed save would silently lose everything since the last successful one.
		/// </summary>
		private static bool SaveWorld( out string error )
		{
			error = "";

			try
			{
				if ( AutoRestart.Restarting )
				{
					error = "the shard is already shutting down";
					return false;
				}

				AutoSave.Save();
				return true;
			}
			catch ( Exception ex )
			{
				error = ex.Message;
				return false;
			}
		}


		#endregion

		#region Restart

		private static void ReplaceServer()
		{
			try
			{
				StartReplacement();
			}
			catch ( Exception ex )
			{
				m_Running = false;

				string message = string.Format( "Could not start a replacement server, so this one was left running: {0}", ex.Message );

				Console.WriteLine( "[UpdateServer] {0}", message );
				Log( message );
				World.Broadcast( 0x22, true, "Server update aborted: the replacement server could not be started." );

				return;
			}

			Console.WriteLine( "[UpdateServer] Restarting the server..." );
			Log( "Restarting the server." );

			// Core.Kill() runs HandleClosed() inline, and HandleClosed() waits for any in-flight
			// world write.  If that wait ever wedges, the old process keeps its listen socket open but
			// stops servicing it: players hang at "authenticating" and the replacement prints
			// "Listener Failed: … (In Use)" and then runs without a listener.  Take the process down
			// regardless, so the socket is released.
			Thread watchdog = new Thread( new ThreadStart( ForceExitIfShutdownStalls ) );

			watchdog.IsBackground = true;
			watchdog.Start();

			try
			{
				Core.Kill( false );
			}
			catch ( Exception ex )
			{
				// Never let a failed kill throw out of the timer callback (that would crash the shard).
				Log( "Core.Kill failed: " + ex );
				Console.WriteLine( "[UpdateServer] Core.Kill failed: {0}", ex.Message );
			}

			// Core.Kill normally takes the process down immediately; this is only a fallback.
			Environment.Exit( 0 );
		}

		/// <summary>
		/// Last-resort exit used while shutting down for a restart: if HandleClosed() is still running
		/// after the watchdog period, terminate anyway so the listening socket is freed for the
		/// replacement process.  The world was already saved before the restart was scheduled.
		/// </summary>
		private static void ForceExitIfShutdownStalls()
		{
			try
			{
				Thread.Sleep( (int)ShutdownWatchdog.TotalMilliseconds );

				string message = string.Format(
					"Shutdown was still running after {0} seconds; forcing the process to exit so the replacement can bind.",
					(int)ShutdownWatchdog.TotalSeconds );

				Console.WriteLine( "[UpdateServer] {0}", message );
				Log( message );

				Environment.Exit( 0 );
			}
			catch
			{
			}
		}

		private static void StartReplacement()
		{
			if ( !IsUnixHost() )
			{
				StartWindowsReplacement();
				return;
			}

			// Unix: an mcs-built .exe is a Windows PE image, so it is normally started through Mono
			// (`mono WorldLinux.exe`).  Reproduce the exact command line this process was started
			// with (from /proc/self/cmdline) and let a detached shell start it once we are gone.
			List<string> command = GetUnixRestartCommand();

			StringBuilder sb = new StringBuilder();

			int pid = Process.GetCurrentProcess().Id;

			sb.Append( "sleep " );
			sb.Append( (int)ReplaceDelay.TotalSeconds );
			sb.Append( "; i=0; while kill -0 " );
			sb.Append( pid );
			sb.Append( " 2>/dev/null && [ $i -lt " );
			sb.Append( ReplacementWaitSeconds );
			sb.Append( " ]; do sleep 1; i=$((i+1)); done; cd " );
			sb.Append( Quote( GetWorldDirectory() ) );
			sb.Append( " && exec" );

			for ( int i = 0; i < command.Count; ++i )
			{
				sb.Append( ' ' );
				sb.Append( Quote( command[i] ) );
			}

			ProcessStartInfo psi = new ProcessStartInfo();

			psi.FileName = "/bin/sh";
			psi.Arguments = "-c " + Quote( sb.ToString() );
			psi.WorkingDirectory = GetWorldDirectory();
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;

			Process.Start( psi );
		}

		private static void StartWindowsReplacement()
		{
			string world = GetWorldDirectory();
			string script = Path.Combine( world, RestartScriptFile );

			string dir = Path.GetDirectoryName( script );

			if ( !string.IsNullOrEmpty( dir ) && !Directory.Exists( dir ) )
				Directory.CreateDirectory( dir );

			File.WriteAllText( script, BuildWindowsRestartScript( world, Core.Arguments, Process.GetCurrentProcess().Id ) );

			ProcessStartInfo psi = new ProcessStartInfo();

			psi.FileName = GetWindowsShell();
			psi.Arguments = "/c \"" + script + "\"";
			psi.WorkingDirectory = world;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;

			Process.Start( psi );
		}

		/// <summary>
		/// Builds the .bat that finishes a Windows update: wait for this server process to disappear
		/// (so the listen socket is free), swap in the freshly compiled World.exe if the core build
		/// produced one, then start the shard again.  Kept public and side-effect free so it can be
		/// exercised by a harness.
		/// </summary>
		public static string BuildWindowsRestartScript( string worldDirectory, string arguments, int pid )
		{
			string args = ( arguments == null ? "" : arguments.Trim() );

			StringBuilder sb = new StringBuilder();

			sb.AppendLine( "@echo off" );
			sb.AppendLine( "REM Written by [UpdateServer].  Waits for the old server to exit, swaps in a" );
			sb.AppendLine( "REM freshly compiled World.exe (when one was built) and starts the shard again." );
			sb.AppendLine( "ping -n 4 127.0.0.1 >NUL" );
			sb.AppendLine( "cd /d \"" + worldDirectory + "\"" );
			sb.AppendLine( "set TRIES=0" );
			sb.AppendLine( ":waitpid" );
			sb.AppendLine( "tasklist /FI \"PID eq " + pid + "\" 2>NUL | find /I \"World.exe\" >NUL" );
			sb.AppendLine( "if errorlevel 1 goto swap" );
			sb.AppendLine( "set /a TRIES+=1" );
			sb.AppendLine( "if %TRIES% GEQ " + ReplacementWaitSeconds + " goto swap" );
			sb.AppendLine( "ping -n 2 127.0.0.1 >NUL" );
			sb.AppendLine( "goto waitpid" );
			sb.AppendLine( ":swap" );
			sb.AppendLine( "set TRIES=0" );
			sb.AppendLine( "if not exist \"" + WindowsStagingFile + "\" goto launch" );
			sb.AppendLine( "move /y \"" + WindowsStagingFile + "\" \"World.exe\" >NUL 2>&1" );
			sb.AppendLine( "if not exist \"" + WindowsStagingFile + "\" goto launch" );
			sb.AppendLine( "set /a TRIES+=1" );
			sb.AppendLine( "if %TRIES% GEQ 10 goto launch" );
			sb.AppendLine( "ping -n 2 127.0.0.1 >NUL" );
			sb.AppendLine( "goto swap" );
			sb.AppendLine( ":launch" );

			if ( args.Length == 0 )
				sb.AppendLine( "start \"\" \"World.exe\"" );
			else
				sb.AppendLine( "start \"\" \"World.exe\" " + args );

			return sb.ToString();
		}

		private static string GetWindowsShell()
		{
			string comspec = null;

			try
			{
				comspec = Environment.GetEnvironmentVariable( "COMSPEC" );
			}
			catch
			{
			}

			if ( string.IsNullOrEmpty( comspec ) )
				comspec = "cmd.exe";

			return comspec;
		}

		private static List<string> GetUnixRestartCommand()
		{
			List<string> argv = new List<string>();

			try
			{
				string cmdline = File.ReadAllText( "/proc/self/cmdline" );
				string[] parts = cmdline.Split( new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries );

				for ( int i = 0; i < parts.Length; ++i )
					argv.Add( parts[i] );
			}
			catch
			{
			}

			// `mono WorldLinux.exe [args]` -> argv[1] is the shard executable.
			if ( argv.Count >= 2 && LooksLikeExecutable( argv[1] ) )
				return argv;

			// Otherwise fall back to the interpreter (if we can recognise it) plus Core.ExePath.
			string launcher = "mono";

			if ( argv.Count > 0 && argv[0] != null && argv[0].IndexOf( "mono", StringComparison.OrdinalIgnoreCase ) >= 0 )
				launcher = argv[0];

			List<string> fallback = new List<string>();

			fallback.Add( launcher );
			fallback.Add( Core.ExePath );

			string arguments = Core.Arguments;

			if ( !string.IsNullOrEmpty( arguments ) )
			{
				string[] split = arguments.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

				for ( int i = 0; i < split.Length; ++i )
					fallback.Add( split[i] );
			}

			return fallback;
		}

		private static bool LooksLikeExecutable( string path )
		{
			try
			{
				if ( string.IsNullOrEmpty( path ) )
					return false;

				return File.Exists( Path.GetFullPath( path ) );
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region Post-restart world rebuild

		private static void ResumePendingBuild()
		{
			if ( !File.Exists( MarkerFile ) )
				return;

			DateTime stamp = DateTime.MinValue;
			string who = "unknown";

			try
			{
				string[] lines = File.ReadAllLines( MarkerFile );

				if ( lines.Length > 0 )
					DateTime.TryParse( lines[0], out stamp );

				if ( lines.Length > 1 && lines[1] != null && lines[1].Length > 0 )
					who = lines[1];
			}
			catch
			{
			}

			if ( stamp != DateTime.MinValue && ( DateTime.Now - stamp ) > StaleMarkerAge )
			{
				string stale = string.Format( "Ignoring the world rebuild left by the update at {0}; it is more than {1} day(s) old.", stamp, (int)StaleMarkerAge.TotalDays );

				Console.WriteLine( "[UpdateServer] {0}", stale );
				Log( stale );
				DeleteMarker();

				return;
			}

			string message = string.Format( "World rebuild pending from the update at {0} (by {1}); running [buildworld in {2} seconds.",
				stamp, who, (int)BuildDelay.TotalSeconds );

			Console.WriteLine( "[UpdateServer] {0}", message );
			Log( message );

			new BuildTimer().Start();
		}

		private static void ResumeBuild()
		{
			DeleteMarker();

			DateTime last = Server.Scripts.Commands.BuildWorld.LastBuild;

			if ( last != DateTime.MinValue && ( DateTime.Now - last ) < RecentBuildWindow )
			{
				string skip = string.Format( "The world was already rebuilt at {0}, so the automatic [buildworld was skipped.", last );

				Console.WriteLine( "[UpdateServer] {0}", skip );
				Log( skip );

				return;
			}

			Mobile from = FindBuildMobile();
			bool temporary = false;

			if ( from == null )
			{
				// Nobody suitable is online (or saved): run the build against a throw-away mobile so the
				// console-driven rebuild does not require a staff member to be logged in.
				from = new Mobile();
				temporary = true;
			}

			try
			{
				Console.WriteLine( "[UpdateServer] Running [buildworld..." );
				Log( "Running [buildworld." );
				World.Broadcast( 0x35, true, "Server update complete.  Rebuilding the world; the shard may lag for a few minutes." );

				Server.Scripts.Commands.BuildWorld.BuildWorlds( new CommandEventArgs( from, "BuildWorld", "", new string[0] ) );

				Console.WriteLine( "[UpdateServer] [buildworld finished." );
				Log( "[buildworld finished." );
				World.Broadcast( 0x35, true, "The world has been rebuilt." );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( "[UpdateServer] [buildworld failed: {0}", ex );
				Log( "[buildworld failed: " + ex );
			}
			finally
			{
				if ( temporary && from != null && !from.Deleted )
					from.Delete();
			}
		}

		private static Mobile FindBuildMobile()
		{
			try
			{
				foreach ( Account a in Accounts.GetAccounts() )
				{
					if ( a == null )
						continue;

					for ( int i = 0; i < a.Length; ++i )
					{
						Mobile m = a[i];

						if ( m != null && !m.Deleted && m.AccessLevel >= AccessLevel.Administrator )
							return m;
					}
				}
			}
			catch ( Exception ex )
			{
				Log( "[UpdateServer] Could not scan accounts for a build mobile: " + ex.Message );
			}

			return null;
		}

		private static bool WriteMarker( string who )
		{
			try
			{
				string dir = Path.GetDirectoryName( MarkerFile );

				if ( !string.IsNullOrEmpty( dir ) && !Directory.Exists( dir ) )
					Directory.CreateDirectory( dir );

				using ( StreamWriter op = new StreamWriter( MarkerFile, false ) )
				{
					op.WriteLine( DateTime.Now.ToString( "o" ) );
					op.WriteLine( who == null ? "unknown" : who );
				}

				return true;
			}
			catch ( Exception ex )
			{
				Log( "[UpdateServer] Could not write the rebuild marker: " + ex );
				return false;
			}
		}

		private static void DeleteMarker()
		{
			try
			{
				if ( File.Exists( MarkerFile ) )
					File.Delete( MarkerFile );
			}
			catch ( Exception ex )
			{
				Log( "[UpdateServer] Could not delete the rebuild marker: " + ex.Message );
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// True when the shard host is Unix-like.  Mirrors Core.Main's own detection
		/// (PlatformID.Unix / MacOSX, plus Mono's 128) but works even before Core.Main has run,
		/// so the value is reliable in-game and in a test harness.
		/// </summary>
		private static bool IsUnixHost()
		{
			int platform = (int)Environment.OSVersion.Platform;

			if ( platform == 4 || platform == 6 || platform == 128 )
				return true;

			return Core.Unix;
		}

		/// <summary>
		/// Which OS the *shard host* is running, which is what decides the core-build toolchain.
		/// (A UO client never reports its own OS, and the compiler always runs server-side.)
		/// </summary>
		private static string GetHostPlatform()
		{
			if ( !IsUnixHost() )
				return "Windows";

			try
			{
				if ( Type.GetType( "Mono.Runtime" ) != null )
					return "Linux/Unix (Mono)";
			}
			catch
			{
			}

			return "Unix";
		}

		/// <summary>
		/// Rebuilds the core with the toolchain that belongs to this host: mcs through
		/// Source/Tools/compile-world-linux.sh on Linux/Unix, csc through
		/// Source/Tools/compile-world-win.bat on Windows.  On Windows the output goes to a staging
		/// file (csc cannot overwrite the running World.exe) and the restart wrapper swaps it in.
		/// </summary>
		private static int CompileCore( out string output )
		{
			string world = GetWorldDirectory();
			string tools = Path.Combine( world, "Source/Tools" );
			DateTime startedUtc = DateTime.UtcNow.AddSeconds( -2.0 ); // filesystem timestamp tolerance

			if ( IsUnixHost() )
			{
				if ( !File.Exists( Path.Combine( tools, "compile-world-linux.sh" ) ) )
				{
					output = "compile-world-linux.sh was not found in " + tools;
					return ProcessStartFailed;
				}

				// compile-world-linux.sh builds Source/Tools/WorldLinux.exe and then moves it over the
				// live binary.  Remove any leftover first so a failed/older build can never be moved
				// into place and reported as a success.
				DeleteFileQuietly( Path.Combine( tools, "WorldLinux.exe" ) );

				int code = RunProcess( "bash", "compile-world-linux.sh", tools, CompileTimeout, out output );

				if ( code == 0 && GetWriteTime( Path.Combine( world, "WorldLinux.exe" ) ) < startedUtc )
				{
					output = output + Environment.NewLine + "The compiler exited 0 but WorldLinux.exe was not rebuilt.";
					return BuildNotProduced;
				}

				return code;
			}

			if ( !File.Exists( Path.Combine( tools, "compile-world-win.bat" ) ) )
			{
				output = "compile-world-win.bat was not found in " + tools;
				return ProcessStartFailed;
			}

			// csc cannot overwrite the running World.exe, so the build is staged; clear any older
			// staging file and require the build to produce a fresh one.
			DeleteFileQuietly( Path.Combine( world, WindowsStagingFile ) );

			// Passing the output path also tells the script not to pause for a keypress.
			int winCode = RunProcess( GetWindowsShell(), "/c compile-world-win.bat " + WindowsStagingArgument, tools, CompileTimeout, out output );

			if ( winCode == 0 && GetWriteTime( Path.Combine( world, WindowsStagingFile ) ) < startedUtc )
			{
				output = output + Environment.NewLine + "The compiler exited 0 but " + WindowsStagingFile + " was not produced.";
				return BuildNotProduced;
			}

			return winCode;
		}

		/// <summary>
		/// Builds the `git pull` argument list.  The reconcile strategy is always explicit: since git
		/// 2.27 a divergent branch otherwise fails with exit 128 ("Need to specify how to reconcile
		/// divergent branches") instead of doing what a plain `git pull` used to do.
		///
		/// The repository is selected through ProcessStartInfo.WorkingDirectory, not `-C`, so no path
		/// ever has to survive shell/command-line quoting (that broke on Windows).
		/// </summary>
		private static string BuildPullArguments( string strategy )
		{
			string mode;

			if ( strategy == "ff" )
				mode = " --ff-only";
			else if ( strategy == "rebase" )
				mode = " --rebase";
			else
				mode = " --no-rebase";

			return "--no-pager pull --no-edit" + mode;
		}

		/// <summary>
		/// The last non-empty output lines, trimmed to something that fits comfortably in an in-game
		/// message (the full output is always in Data/Logs/UpdateServer.log).
		/// </summary>
		private static string OutputHint( string output, int maxCharacters )
		{
			if ( output == null || output.Length == 0 )
				return "";

			string[] lines = output.Replace( "\r\n", "\n" ).Replace( '\r', '\n' ).Split( '\n' );
			List<string> tail = new List<string>();

			for ( int i = lines.Length - 1; i >= 0 && tail.Count < 2; --i )
			{
				string line = lines[i].Trim();

				if ( line.Length > 0 )
					tail.Insert( 0, line );
			}

			if ( tail.Count == 0 )
				return "";

			string text = string.Join( " | ", tail.ToArray() );

			if ( text.Length > maxCharacters )
				text = "..." + text.Substring( text.Length - ( maxCharacters - 3 ) );

			return text;
		}

		private static DateTime GetWriteTime( string path )
		{
			try
			{
				return File.Exists( path ) ? File.GetLastWriteTimeUtc( path ) : DateTime.MinValue;
			}
			catch
			{
				return DateTime.MinValue;
			}
		}

		private static string DescribeCompileFailure( int code )
		{
			if ( code == ProcessStartFailed )
				return "the compiler could not be started";

			if ( code == BuildNotProduced )
				return "the compiler produced no new binary";

			if ( code == ProcessTimedOut )
				return "the compiler timed out";

			return string.Format( "the compiler failed (exit code {0})", code );
		}

		private static void DeleteStagingBinary()
		{
			DeleteFileQuietly( Path.Combine( GetWorldDirectory(), WindowsStagingFile ) );
		}

		private static void DeleteFileQuietly( string path )
		{
			try
			{
				if ( File.Exists( path ) )
					File.Delete( path );
			}
			catch ( Exception ex )
			{
				Log( "[UpdateServer] Could not remove " + path + ": " + ex.Message );
			}
		}

		private static string GetWorldDirectory()
		{
			string world = Core.BaseDirectory;

			if ( string.IsNullOrEmpty( world ) )
				world = Environment.CurrentDirectory;

			try
			{
				return Path.GetFullPath( world );
			}
			catch
			{
				return world;
			}
		}

		private static string GetRepositoryRoot()
		{
			try
			{
				string world = GetWorldDirectory();

				// The World folder itself may be the checkout (standalone deployments) …
				string dotGit = Path.Combine( world, ".git" );

				if ( Directory.Exists( dotGit ) || File.Exists( dotGit ) )
					return world;

				// … otherwise it is the folder above it (this repository's layout).
				DirectoryInfo parent = Directory.GetParent( world );

				if ( parent == null )
					return null;

				dotGit = Path.Combine( parent.FullName, ".git" );

				if ( Directory.Exists( dotGit ) || File.Exists( dotGit ) )
					return parent.FullName;
			}
			catch
			{
			}

			return null;
		}

		/// <summary>
		/// Quotes a single argument for the shell of the *host* OS: POSIX single quotes on Unix (used
		/// by the restart command line) and CRT-style double quotes on Windows, where single quotes
		/// are ordinary characters that the program would receive verbatim.
		/// </summary>
		private static string Quote( string value )
		{
			if ( value == null )
				value = "";

			if ( IsUnixHost() )
				return "'" + value.Replace( "'", "'\\''" ) + "'";

			return QuoteWindowsArgument( value );
		}

		/// <summary>
		/// Quotes one argument using the Windows command-line rules: wrap it in double quotes and
		/// double any run of backslashes that sits in front of a quote or of the closing quote.
		/// </summary>
		private static string QuoteWindowsArgument( string value )
		{
			if ( value == null )
				value = "";

			StringBuilder sb = new StringBuilder();

			sb.Append( '"' );

			int backslashes = 0;

			for ( int i = 0; i < value.Length; ++i )
			{
				char c = value[i];

				if ( c == '\\' )
				{
					++backslashes;
					continue;
				}

				if ( c == '"' )
				{
					sb.Append( '\\', backslashes * 2 + 1 );
					sb.Append( '"' );
					backslashes = 0;
					continue;
				}

				if ( backslashes > 0 )
				{
					sb.Append( '\\', backslashes );
					backslashes = 0;
				}

				sb.Append( c );
			}

			if ( backslashes > 0 )
				sb.Append( '\\', backslashes * 2 );

			sb.Append( '"' );

			return sb.ToString();
		}

		private static int RunProcess( string fileName, string arguments, string workingDirectory, TimeSpan timeout, out string output )
		{
			StringBuilder sb = new StringBuilder();

			ProcessStartInfo psi = new ProcessStartInfo();

			psi.FileName = fileName;
			psi.Arguments = arguments;
			psi.WorkingDirectory = workingDirectory;
			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.CreateNoWindow = true;

			try
			{
				psi.EnvironmentVariables["GIT_TERMINAL_PROMPT"] = "0";
			}
			catch
			{
			}

			try
			{
				using ( Process p = new Process() )
				{
					p.StartInfo = psi;
					p.OutputDataReceived += delegate( object sender, DataReceivedEventArgs e )
					{
						if ( e.Data != null )
						{
							lock ( sb )
								sb.AppendLine( e.Data );
						}
					};
					p.ErrorDataReceived += delegate( object sender, DataReceivedEventArgs e )
					{
						if ( e.Data != null )
						{
							lock ( sb )
								sb.AppendLine( e.Data );
						}
					};

					p.Start();
					p.BeginOutputReadLine();
					p.BeginErrorReadLine();

					int milliseconds = timeout.TotalMilliseconds > int.MaxValue ? int.MaxValue : (int)timeout.TotalMilliseconds;

					if ( !p.WaitForExit( milliseconds ) )
					{
						try
						{
							p.Kill();
						}
						catch
						{
						}

						output = sb.ToString() + Environment.NewLine + string.Format( "(timed out after {0} seconds)", (int)timeout.TotalSeconds );
						return -1;
					}

					// Flush the asynchronous readers before reading the accumulated output.
					p.WaitForExit();

					output = sb.ToString();
					return p.ExitCode;
				}
			}
			catch ( Exception ex )
			{
				output = string.Format( "Could not run '{0}': {1}", fileName, ex.Message );
				return ProcessStartFailed;
			}
		}

		private static void Log( string message )
		{
			if ( message == null || message.Length == 0 )
				return;

			try
			{
				string dir = Path.GetDirectoryName( LogFile );

				if ( !string.IsNullOrEmpty( dir ) && !Directory.Exists( dir ) )
					Directory.CreateDirectory( dir );

				using ( StreamWriter op = new StreamWriter( LogFile, true ) )
				{
					op.WriteLine( "[{0}] {1}", DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ), message );
				}
			}
			catch
			{
			}
		}

		#endregion
	}
}
