using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Misc
{
	/// <summary>
	/// Gives the gypsy lady a voice: questions are sent to the Command Code
	/// Provider API (an OpenAI-compatible chat endpoint) from a worker thread,
	/// and the answer is handed back to the game thread by a heartbeat timer.
	/// </summary>
	public static class GypsyOracle
	{
		private const string ConfigPath = "Data/System/CFG/GypsyOracle.cfg";

		private const string DefaultEndpoint = "https://api.commandcode.ai/provider/v1/chat/completions";
		private const string DefaultModel = "deepseek/deepseek-v4.1-flash-fast";

		private const string DefaultSystemPrompt =
			"You are the gypsy fortune teller of the shard Memento, a persistent Ultima Online world. " +
			"Speak in character at all times: cryptic, warm and theatrical, as if reading tarot cards and listening to spirits. " +
			"Answer the player's question directly but briefly, at most 90 words. " +
			"Never mention AI, APIs, language models, prompts, Command Code, or anything outside the fantasy world. " +
			"If you do not know something, say that the spirits are unclear. " +
			"Do not invent game commands, item names, or promises about the player's account; when asked about game mechanics, " +
			"answer in general terms and suggest asking a townsperson. Plain prose only: no markdown, no lists, no code.";

		private static bool m_Loaded;
		private static bool m_Enabled = true;
		private static string m_ApiKey = String.Empty;
		private static string m_Endpoint = DefaultEndpoint;
		private static string m_Model = DefaultModel;
		private static string m_SystemPrompt = DefaultSystemPrompt;
		private static int m_TimeoutSeconds = 30;
		private static int m_MaxQuestionLength = 300;
		private static int m_MaxAnswerLength = 900;
		private static int m_MaxTokens = 350;
		private static int m_CooldownSeconds = 8;
		private static int m_HistoryTurns = 4;
		private static double m_Temperature = 0.8;

		private static bool m_LoreEnabled = true;
		private static string m_LoreFile = "Data/System/CFG/GypsyLore.txt";
		private static int m_LoreMaxCharacters = 6000;
		private static readonly List<LoreSection> m_Lore = new List<LoreSection>();
		private static readonly Dictionary<string, Dictionary<int, int>> m_LoreIndex = new Dictionary<string, Dictionary<int, int>>();
		private static DateTime m_LoreStamp = DateTime.MinValue;
		private static bool m_LoreChecked;
		private static DateTime m_NextLoreCheck;

		private static readonly object m_ResultLock = new object();
		private static readonly Queue<Result> m_Results = new Queue<Result>();
		private static readonly Dictionary<Mobile, DateTime> m_Cooldown = new Dictionary<Mobile, DateTime>();
		private static readonly Dictionary<Mobile, List<Turn>> m_History = new Dictionary<Mobile, List<Turn>>();
		private static readonly Dictionary<Mobile, bool> m_Pending = new Dictionary<Mobile, bool>();
		private static Timer m_Heartbeat;

		private class Turn
		{
			public string Question;
			public string Answer;
		}

		private class LoreSection
		{
			public string Title;
			public string Text;
		}

		private class LoreMatch
		{
			public LoreSection Section;
			public int Score;
			public int Index;
		}

		private class Result
		{
			public Mobile From;
			public GypsyLady Gypsy;
			public string Question;
			public string Answer;
			public string Error;
		}

		private class Request
		{
			public Mobile From;
			public GypsyLady Gypsy;
			public string Question;
			public string Body;
		}

		/// <summary>True when the feature is switched on in the config file.</summary>
		public static bool Enabled
		{
			get { EnsureLoaded(); return m_Enabled; }
		}

		/// <summary>True when the feature is switched on and an API key is present.</summary>
		public static bool Available
		{
			get { EnsureLoaded(); return m_Enabled && m_ApiKey.Length > 0; }
		}

		/// <summary>The model id currently configured (for diagnostics).</summary>
		public static string Model
		{
			get { EnsureLoaded(); return m_Model; }
		}

		/// <summary>The maximum question length the gypsy will accept.</summary>
		public static int MaxQuestionLength
		{
			get { EnsureLoaded(); return m_MaxQuestionLength; }
		}

		public static void Initialize()
		{
			EnsureLoaded();
			ReloadLoreIfNeeded();
			m_NextLoreCheck = DateTime.Now + TimeSpan.FromSeconds( 5 );

			// Cloudflare (and most modern hosts) require TLS 1.2. Mono's default
			// security protocol is old enough that this matters.
			try
			{
				ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
			}
			catch ( Exception )
			{
			}

			if ( m_Heartbeat == null )
				m_Heartbeat = Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ), new TimerCallback( DrainResults ) );
		}

		/// <summary>
		/// Queues a question for the gypsy. Returns null when the request was
		/// started, or a message to show the player when it was refused.
		/// </summary>
		public static string Ask( Mobile from, GypsyLady gypsy, string question )
		{
			EnsureLoaded();

			if ( from == null || from.Deleted || !from.Alive || gypsy == null || gypsy.Deleted )
				return "The gypsy cannot hear you.";

			if ( !m_Enabled )
				return "The spirits are silent right now.";

			if ( m_ApiKey.Length == 0 )
			{
				if ( from.AccessLevel >= AccessLevel.GameMaster )
					return "The gypsy has no Command Code API key. Set ApiKey in Data/System/CFG/GypsyOracle.cfg or the COMMANDCODE_API_KEY environment variable.";

				return "The spirits are silent; the shard has not given the gypsy a voice.";
			}

			question = CleanQuestion( question );

			if ( question.Length == 0 )
				return "Ask a question first, and the spirits may answer.";

			if ( m_Pending.ContainsKey( from ) )
				return "The spirits are still considering your last question.";

			DateTime ready;

			if ( m_Cooldown.TryGetValue( from, out ready ) && ready > DateTime.Now )
			{
				int seconds = (int)Math.Ceiling( ( ready - DateTime.Now ).TotalSeconds );

				if ( seconds < 1 )
					seconds = 1;

				return String.Format( "The spirits need a moment to gather. Ask again in {0} second{1}.", seconds, seconds == 1 ? "" : "s" );
			}

			m_Pending[from] = true;
			m_Cooldown[from] = DateTime.Now + TimeSpan.FromSeconds( m_CooldownSeconds );

			Request request = new Request();
			request.From = from;
			request.Gypsy = gypsy;
			request.Question = question;
			request.Body = BuildRequestJson( question, SnapshotHistory( from ) );

			try
			{
				ThreadPool.QueueUserWorkItem( new WaitCallback( SendRequest ), request );
			}
			catch ( Exception e )
			{
				m_Pending.Remove( from );
				m_Cooldown.Remove( from );
				Console.WriteLine( "[GypsyOracle] Could not start a request: {0}", e.Message );
				return "The spirits are silent. Try again later.";
			}

			return null;
		}

		private static string CleanQuestion( string text )
		{
			if ( text == null )
				return String.Empty;

			StringBuilder sb = new StringBuilder( text.Length );
			bool lastSpace = false;

			foreach ( char raw in text )
			{
				char c = raw;

				if ( c == '\r' || c == '\n' || c == '\t' )
					c = ' ';

				if ( Char.IsControl( c ) )
					continue;

				if ( c == ' ' )
				{
					if ( lastSpace )
						continue;

					lastSpace = true;
				}
				else
				{
					lastSpace = false;
				}

				sb.Append( c );
			}

			string s = sb.ToString().Trim();

			if ( s.Length > m_MaxQuestionLength )
			{
				s = s.Substring( 0, m_MaxQuestionLength ).TrimEnd();

				if ( s.Length > 0 && Char.IsHighSurrogate( s[s.Length - 1] ) )
					s = s.Substring( 0, s.Length - 1 );
			}

			return s;
		}

		private static List<Turn> SnapshotHistory( Mobile from )
		{
			List<Turn> copy = new List<Turn>();
			List<Turn> stored;

			if ( !m_History.TryGetValue( from, out stored ) || stored == null )
				return copy;

			for ( int i = 0; i < stored.Count; ++i )
			{
				Turn turn = new Turn();
				turn.Question = stored[i].Question;
				turn.Answer = stored[i].Answer;
				copy.Add( turn );
			}

			return copy;
		}

		private static void SendRequest( object state )
		{
			Request request = (Request)state;

			Result result = new Result();
			result.From = request.From;
			result.Gypsy = request.Gypsy;
			result.Question = request.Question;

			try
			{
				HttpWebRequest http = (HttpWebRequest)WebRequest.Create( m_Endpoint );

				http.Method = "POST";
				http.ContentType = "application/json";
				http.Accept = "application/json";
				http.UserAgent = "Memento-Gypsy/1.0";
				http.Headers["Authorization"] = "Bearer " + m_ApiKey;
				http.Timeout = m_TimeoutSeconds * 1000;
				http.ReadWriteTimeout = m_TimeoutSeconds * 1000;
				http.KeepAlive = false;

				byte[] body = Encoding.UTF8.GetBytes( request.Body );
				http.ContentLength = body.Length;

				using ( Stream stream = http.GetRequestStream() )
					stream.Write( body, 0, body.Length );

				using ( HttpWebResponse response = (HttpWebResponse)http.GetResponse() )
				{
					using ( StreamReader reader = new StreamReader( response.GetResponseStream(), Encoding.UTF8 ) )
						result.Answer = ParseAnswer( reader.ReadToEnd() );
				}

				if ( result.Answer == null )
					result.Error = "The spirits answered in a tongue I cannot read.";
			}
			catch ( WebException we )
			{
				result.Error = DescribeWebError( we );
			}
			catch ( Exception e )
			{
				result.Error = "The spirits are silent. (" + e.Message + ")";
			}

			lock ( m_ResultLock )
				m_Results.Enqueue( result );
		}

		private static string DescribeWebError( WebException we )
		{
			if ( we.Response != null )
			{
				try
				{
					using ( Stream stream = we.Response.GetResponseStream() )
					{
						if ( stream != null )
						{
							using ( StreamReader reader = new StreamReader( stream, Encoding.UTF8 ) )
							{
								string body = reader.ReadToEnd();
								string message = ExtractStringValue( body, "message" );

								if ( !String.IsNullOrEmpty( message ) )
									return message;

								if ( !String.IsNullOrEmpty( body ) )
								{
									if ( body.Length > 200 )
										body = body.Substring( 0, 200 ) + "...";

									return body;
								}
							}
						}
					}
				}
				catch ( Exception )
				{
				}
			}

			return we.Message;
		}

		private static string ParseAnswer( string json )
		{
			if ( String.IsNullOrEmpty( json ) )
				return null;

			string content = null;
			int choices = json.IndexOf( "\"choices\"", StringComparison.Ordinal );

			if ( choices >= 0 )
				content = ExtractStringValue( json.Substring( choices ), "content" );

			if ( content == null )
				content = ExtractStringValue( json, "content" );

			// The answer is sanitised on the game thread by Apply, so the
			// truncation and scrolling limits are applied exactly once.
			return content;
		}

		private static string SanitizeAnswer( string text )
		{
			if ( String.IsNullOrEmpty( text ) )
				return null;

			StringBuilder sb = new StringBuilder( text.Length );
			bool lastSpace = false;

			foreach ( char raw in text )
			{
				char c = raw;

				if ( c == '\t' )
					c = ' ';

				if ( c == '\r' )
					continue;

				if ( c == '\n' )
				{
					if ( sb.Length > 0 && sb[sb.Length - 1] != '\n' )
						sb.Append( '\n' );

					lastSpace = false;
					continue;
				}

				if ( Char.IsControl( c ) )
					continue;

				if ( c == ' ' )
				{
					if ( lastSpace )
						continue;

					lastSpace = true;
				}
				else
				{
					lastSpace = false;
				}

				sb.Append( c );
			}

			string s = sb.ToString().Trim();

			if ( s.Length == 0 )
				return null;

			if ( s.Length > m_MaxAnswerLength )
			{
				s = s.Substring( 0, m_MaxAnswerLength ).TrimEnd();

				if ( s.Length > 0 && Char.IsHighSurrogate( s[s.Length - 1] ) )
					s = s.Substring( 0, s.Length - 1 );

				int cut = s.LastIndexOf( ' ' );

				if ( cut > m_MaxAnswerLength / 2 )
					s = s.Substring( 0, cut ).TrimEnd();

				s += "...";
			}

			return s;
		}

		/// <summary>Escapes an answer for safe display inside a gump's HTML pane.</summary>
		public static string ToGumpHtml( string text )
		{
			if ( String.IsNullOrEmpty( text ) )
				return String.Empty;

			StringBuilder sb = new StringBuilder( text.Length );

			foreach ( char c in text )
			{
				switch ( c )
				{
					case '&': sb.Append( "&amp;" ); break;
					case '<': sb.Append( "&lt;" ); break;
					case '>': sb.Append( "&gt;" ); break;
					case '"': sb.Append( "&quot;" ); break;
					case '\r': break;
					case '\n': sb.Append( "<BR>" ); break;
					default: sb.Append( c ); break;
				}
			}

			return sb.ToString();
		}

		private static string ToChatText( string text, int maxLength )
		{
			if ( String.IsNullOrEmpty( text ) )
				return String.Empty;

			StringBuilder sb = new StringBuilder( text.Length );
			bool lastSpace = false;

			foreach ( char c in text )
			{
				if ( c == '\r' || c == '\n' || c == '\t' || c == ' ' )
				{
					if ( sb.Length > 0 && !lastSpace )
					{
						sb.Append( ' ' );
						lastSpace = true;
					}

					continue;
				}

				if ( Char.IsControl( c ) )
					continue;

				sb.Append( c );
				lastSpace = false;
			}

			string s = sb.ToString().Trim();

			if ( s.Length > maxLength )
			{
				s = s.Substring( 0, maxLength ).TrimEnd();

				if ( s.Length > 0 && Char.IsHighSurrogate( s[s.Length - 1] ) )
					s = s.Substring( 0, s.Length - 1 );

				s += "...";
			}

			return s;
		}

		private static void DrainResults()
		{
			while ( true )
			{
				Result result = null;

				lock ( m_ResultLock )
				{
					if ( m_Results.Count > 0 )
						result = m_Results.Dequeue();
				}

				if ( result == null )
					break;

				Apply( result );
			}

			Prune();

			if ( DateTime.Now >= m_NextLoreCheck )
			{
				m_NextLoreCheck = DateTime.Now + TimeSpan.FromSeconds( 5 );
				ReloadLoreIfNeeded();
			}
		}

		private static void Apply( Result result )
		{
			Mobile from = result.From;
			GypsyLady gypsy = result.Gypsy;

			if ( from != null )
				m_Pending.Remove( from );

			if ( from == null || from.Deleted )
				return;

			if ( !String.IsNullOrEmpty( result.Error ) )
			{
				string message = result.Error;

				if ( from.AccessLevel < AccessLevel.GameMaster )
					message = "The spirits did not answer. Try again later.";

				Console.WriteLine( "[GypsyOracle] Request failed: {0}", result.Error );

				if ( gypsy != null && !gypsy.Deleted )
					gypsy.SayTo( from, "The spirits are restless... ask me again in a moment." );

				if ( gypsy != null && !gypsy.Deleted && from.InRange( gypsy, 12 ) )
				{
					from.CloseGump( typeof( GypsyAskGump ) );
					from.SendGump( new GypsyAskGump( from, gypsy, result.Question, message, false ) );
				}
				else
				{
					from.SendMessage( message );
				}

				return;
			}

			string answer = SanitizeAnswer( result.Answer );

			if ( String.IsNullOrEmpty( answer ) )
			{
				if ( gypsy != null && !gypsy.Deleted )
					gypsy.SayTo( from, "The spirits showed me nothing but mist. Ask me again later." );

				return;
			}

			Remember( from, result.Question, answer );

			if ( gypsy != null && !gypsy.Deleted )
			{
				gypsy.SayTo( from, ToChatText( answer, 220 ) );

				if ( from.InRange( gypsy, 12 ) )
				{
					from.CloseGump( typeof( GypsyAskGump ) );
					from.SendGump( new GypsyAskGump( from, gypsy, result.Question, answer, false ) );
				}
			}
			else
			{
				from.SendMessage( "The gypsy's voice fades, but her words remain: " + ToChatText( answer, 400 ) );
			}
		}

		private static void Remember( Mobile from, string question, string answer )
		{
			if ( m_HistoryTurns <= 0 )
			{
				m_History.Remove( from );
				return;
			}

			List<Turn> list;

			if ( !m_History.TryGetValue( from, out list ) || list == null )
			{
				list = new List<Turn>();
				m_History[from] = list;
			}

			Turn turn = new Turn();
			turn.Question = question;
			turn.Answer = answer;
			list.Add( turn );

			while ( list.Count > m_HistoryTurns )
				list.RemoveAt( 0 );
		}

		private static void Prune()
		{
			List<Mobile> stale = null;
			DateTime now = DateTime.Now;

			foreach ( Mobile m in m_Cooldown.Keys )
			{
				if ( m == null || m.Deleted || m.NetState == null || m_Cooldown[m] + TimeSpan.FromMinutes( 1 ) < now )
				{
					if ( stale == null )
						stale = new List<Mobile>();

					stale.Add( m );
				}
			}

			if ( stale != null )
			{
				for ( int i = 0; i < stale.Count; ++i )
					m_Cooldown.Remove( stale[i] );

				stale = null;
			}

			foreach ( Mobile m in m_History.Keys )
			{
				if ( m == null || m.Deleted || m.NetState == null )
				{
					if ( stale == null )
						stale = new List<Mobile>();

					stale.Add( m );
				}
			}

			if ( stale != null )
			{
				for ( int i = 0; i < stale.Count; ++i )
					m_History.Remove( stale[i] );

				stale = null;
			}

			foreach ( Mobile m in m_Pending.Keys )
			{
				if ( m == null || m.Deleted || m.NetState == null )
				{
					if ( stale == null )
						stale = new List<Mobile>();

					stale.Add( m );
				}
			}

			if ( stale != null )
			{
				for ( int i = 0; i < stale.Count; ++i )
					m_Pending.Remove( stale[i] );
			}
		}

		private static string BuildRequestJson( string question, List<Turn> history )
		{
			StringBuilder sb = new StringBuilder();
			bool first = true;

			sb.Append( "{\"model\":" );
			AppendJsonString( sb, m_Model );
			sb.Append( ",\"messages\":[" );

			AppendMessage( sb, ref first, "system", m_SystemPrompt );

			string lore = BuildLoreContext( question );

			if ( !String.IsNullOrEmpty( lore ) )
				AppendMessage( sb, ref first, "system", lore );

			if ( history != null )
			{
				for ( int i = 0; i < history.Count; ++i )
				{
					AppendMessage( sb, ref first, "user", history[i].Question );
					AppendMessage( sb, ref first, "assistant", history[i].Answer );
				}
			}

			AppendMessage( sb, ref first, "user", question );

			sb.Append( "],\"max_tokens\":" );
			sb.Append( m_MaxTokens.ToString( CultureInfo.InvariantCulture ) );
			sb.Append( ",\"temperature\":" );
			sb.Append( m_Temperature.ToString( CultureInfo.InvariantCulture ) );
			sb.Append( '}' );

			return sb.ToString();
		}

		private static void AppendMessage( StringBuilder sb, ref bool first, string role, string content )
		{
			if ( !first )
				sb.Append( ',' );

			first = false;

			sb.Append( "{\"role\":" );
			AppendJsonString( sb, role );
			sb.Append( ",\"content\":" );
			AppendJsonString( sb, content );
			sb.Append( '}' );
		}

		private static void AppendJsonString( StringBuilder sb, string value )
		{
			sb.Append( '"' );

			if ( value != null )
			{
				foreach ( char c in value )
				{
					switch ( c )
					{
						case '"': sb.Append( "\\\"" ); break;
						case '\\': sb.Append( "\\\\" ); break;
						case '\n': sb.Append( "\\n" ); break;
						case '\r': sb.Append( "\\r" ); break;
						case '\t': sb.Append( "\\t" ); break;
						case '\b': sb.Append( "\\b" ); break;
						case '\f': sb.Append( "\\f" ); break;
						default:
							if ( c < ' ' )
								sb.AppendFormat( "\\u{0:x4}", (int)c );
							else
								sb.Append( c );
							break;
					}
				}
			}

			sb.Append( '"' );
		}

		/// <summary>
		/// Finds the first string value stored under <paramref name="key"/> in a
		/// JSON document. This is deliberately small: the shard only needs to
		/// pull "content" and "message" out of the provider's replies, and this
		/// avoids adding another assembly reference to the script compiler.
		/// </summary>
		private static string ExtractStringValue( string json, string key )
		{
			if ( String.IsNullOrEmpty( json ) || String.IsNullOrEmpty( key ) )
				return null;

			int i = 0;

			while ( i < json.Length )
			{
				if ( json[i] != '"' )
				{
					++i;
					continue;
				}

				int start;
				string token = ReadJsonString( json, ref i, out start );

				if ( token != key )
					continue;

				int j = i;

				while ( j < json.Length && Char.IsWhiteSpace( json[j] ) )
					++j;

				if ( j >= json.Length || json[j] != ':' )
					continue;

				++j;

				while ( j < json.Length && Char.IsWhiteSpace( json[j] ) )
					++j;

				if ( j >= json.Length || json[j] != '"' )
					return null;

				int valueStart;
				return ReadJsonString( json, ref j, out valueStart );
			}

			return null;
		}

		/// <summary>Reads one JSON string token starting at <paramref name="i"/>.</summary>
		private static string ReadJsonString( string json, ref int i, out int contentStart )
		{
			contentStart = i + 1;

			StringBuilder sb = new StringBuilder();
			++i; // skip the opening quote

			while ( i < json.Length )
			{
				char c = json[i++];

				if ( c == '"' )
					return sb.ToString();

				if ( c != '\\' )
				{
					sb.Append( c );
					continue;
				}

				if ( i >= json.Length )
					break;

				char escape = json[i++];

				switch ( escape )
				{
					case '"': sb.Append( '"' ); break;
					case '\\': sb.Append( '\\' ); break;
					case '/': sb.Append( '/' ); break;
					case 'b': sb.Append( '\b' ); break;
					case 'f': sb.Append( '\f' ); break;
					case 'n': sb.Append( '\n' ); break;
					case 'r': sb.Append( '\r' ); break;
					case 't': sb.Append( '\t' ); break;
					case 'u':
						if ( i + 4 <= json.Length )
						{
							int code;

							if ( Int32.TryParse( json.Substring( i, 4 ), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code ) )
								sb.Append( (char)code );

							i += 4;
						}
						break;
					default:
						sb.Append( escape );
						break;
				}
			}

			return sb.ToString();
		}

		private static void ReloadLoreIfNeeded()
		{
			if ( !m_LoreEnabled )
			{
				m_Lore.Clear();
				m_LoreChecked = true;
				return;
			}

			string path = m_LoreFile;

			if ( !Path.IsPathRooted( path ) )
				path = Path.Combine( Core.BaseDirectory, path );

			DateTime stamp = DateTime.MinValue;

			try
			{
				if ( File.Exists( path ) )
					stamp = File.GetLastWriteTimeUtc( path );
			}
			catch ( Exception )
			{
			}

			if ( m_LoreChecked && stamp == m_LoreStamp )
				return;

			m_LoreChecked = true;
			m_LoreStamp = stamp;
			m_Lore.Clear();

			if ( stamp == DateTime.MinValue )
			{
				Console.WriteLine( "[GypsyOracle] Lore file not found: {0}", m_LoreFile );
				return;
			}

			try
			{
				LoreSection current = null;

				using ( StreamReader reader = new StreamReader( path ) )
				{
					string line;

					while ( ( line = reader.ReadLine() ) != null )
					{
						if ( line.Length > 0 && line[0] == ';' )
							continue;

						if ( line.StartsWith( "# " ) )
						{
							AddLoreSection( current );
							current = new LoreSection();
							current.Title = line.Substring( 2 ).Trim();
							current.Text = String.Empty;
							continue;
						}

						if ( current != null )
						{
							if ( current.Text.Length > 0 )
								current.Text += "\n";

							current.Text += line;
						}
					}
				}

				AddLoreSection( current );

				BuildLoreIndex();

				Console.WriteLine( "[GypsyOracle] Loaded {0} lore sections from {1}.", m_Lore.Count, m_LoreFile );
			}
			catch ( Exception e )
			{
				Console.WriteLine( "[GypsyOracle] Could not read lore file {0}: {1}", m_LoreFile, e.Message );
			}
		}

		private static void AddLoreSection( LoreSection section )
		{
			if ( section == null || String.IsNullOrEmpty( section.Text ) || String.IsNullOrEmpty( section.Title ) )
				return;

			section.Text = section.Text.Trim();

			if ( section.Text.Length > 0 )
				m_Lore.Add( section );
		}

		/// <summary>
		/// Picks the lore sections most relevant to the question and returns them
		/// as a system message. The primer (first section in the file) is always
		/// included; the rest are scored by keyword hits.
		/// </summary>
		private static string BuildLoreContext( string question )
		{
			if ( !m_LoreEnabled || m_Lore.Count == 0 || m_LoreMaxCharacters <= 0 )
				return null;

			string[] words = ExtractKeywords( question );
			List<LoreMatch> matches = new List<LoreMatch>();
			Dictionary<int, int> scores = new Dictionary<int, int>();

			for ( int i = 0; i < words.Length; ++i )
			{
				Dictionary<int, int> hits;

				if ( !m_LoreIndex.TryGetValue( words[i], out hits ) )
					continue;

				foreach ( KeyValuePair<int, int> hit in hits )
				{
					if ( hit.Key == 0 ) // the primer is always included separately
						continue;

					int current;
					scores.TryGetValue( hit.Key, out current );
					scores[hit.Key] = current + Math.Min( hit.Value, 12 );
				}
			}

			foreach ( KeyValuePair<int, int> score in scores )
			{
				LoreMatch match = new LoreMatch();
				match.Section = m_Lore[score.Key];
				match.Score = score.Value;
				match.Index = score.Key;
				matches.Add( match );
			}

			matches.Sort( delegate( LoreMatch a, LoreMatch b )
			{
				if ( a.Score != b.Score )
					return b.Score.CompareTo( a.Score );

				return a.Index.CompareTo( b.Index );
			} );

			StringBuilder sb = new StringBuilder();
			sb.Append( "The following excerpts are the recorded lore of this world. Treat them as canon, and present what they contain as what your cards and the spirits reveal. Never mention the excerpts or attribute the lore to any person, creature, book, note, object or other source.\n\n" );

			int budget = m_LoreMaxCharacters - sb.Length - 2;

			if ( budget <= 0 )
				return null;

			AppendLoreSection( sb, m_Lore[0], ref budget );

			for ( int i = 0; i < matches.Count && budget > 200; ++i )
				AppendLoreSection( sb, matches[i].Section, ref budget );

			if ( sb.Length < 260 )
				return null;

			return sb.ToString();
		}

		private static void AppendLoreSection( StringBuilder sb, LoreSection section, ref int budget )
		{
			if ( section == null || String.IsNullOrEmpty( section.Text ) )
				return;

			string header = "## " + section.Title + "\n";
			int room = budget - header.Length - 2;

			if ( room < 120 )
				return;

			string body = section.Text;

			if ( body.Length > room )
				body = TruncateAtWord( body, room );

			if ( body.Length == 0 )
				return;

			sb.Append( header );
			sb.Append( body );
			sb.Append( "\n\n" );

			budget -= header.Length + body.Length + 2;
		}

		private static readonly string[] m_StopWords = new string[]
		{
			"the", "and", "for", "are", "but", "not", "you", "your", "yours", "with", "that", "this",
			"these", "those", "from", "have", "has", "had", "was", "were", "been", "being", "will",
			"would", "could", "should", "can", "may", "might", "must", "shall", "does", "did", "doing",
			"what", "when", "where", "who", "whom", "whose", "why", "how", "which", "there", "here",
			"about", "into", "onto", "over", "under", "than", "then", "them", "they", "their", "she",
			"her", "his", "him", "its", "our", "ours", "me", "my", "mine", "we", "us", "tell", "say",
			"know", "think", "like", "just", "some", "any", "all", "one", "two", "also", "very", "much",
			"many", "more", "most", "such", "only", "own", "same", "other", "another", "please", "want",
			"need", "give", "get", "got", "make", "made", "take", "see", "look", "find", "found"
		};

		private static string[] ExtractKeywords( string text )
		{
			List<string> words = new List<string>();

			if ( String.IsNullOrEmpty( text ) )
				return words.ToArray();

			StringBuilder word = new StringBuilder();

			foreach ( char raw in text.ToLowerInvariant() )
			{
				char c = raw;

				if ( Char.IsLetterOrDigit( c ) || c == '\'' )
				{
					word.Append( c );
					continue;
				}

				AddKeyword( words, word );
			}

			AddKeyword( words, word );

			// A question about the castle mouse (or cheese) maps to the raw
			// lore it points at: the prophecies, rare relics and tribute relics.
			bool oldTale = words.Contains( "sherry" ) || words.Contains( "sherry's" ) ||
				words.Contains( "mouse" ) || words.Contains( "mouse's" ) || words.Contains( "cheese" );

			if ( oldTale )
			{
				if ( !words.Contains( "prophecies" ) )
					words.Add( "prophecies" );

				if ( !words.Contains( "rare" ) )
					words.Add( "rare" );

				if ( !words.Contains( "places" ) )
					words.Add( "places" );
			}

			return words.ToArray();
		}

		private static void AddKeyword( List<string> words, StringBuilder word )
		{
			if ( word.Length < 3 )
			{
				word.Length = 0;
				return;
			}

			string value = word.ToString();
			word.Length = 0;

			if ( IsStopWord( value ) || words.Contains( value ) )
				return;

			words.Add( value );
		}

		private static bool IsStopWord( string word )
		{
			for ( int i = 0; i < m_StopWords.Length; ++i )
			{
				if ( m_StopWords[i] == word )
					return true;
			}

			return false;
		}

		private static void BuildLoreIndex()
		{
			m_LoreIndex.Clear();

			for ( int i = 0; i < m_Lore.Count; ++i )
			{
				IndexWords( m_Lore[i].Title, i, 6 );
				IndexWords( m_Lore[i].Text, i, 1 );
			}
		}

		private static void IndexWords( string text, int section, int weight )
		{
			if ( String.IsNullOrEmpty( text ) )
				return;

			StringBuilder word = new StringBuilder();

			foreach ( char raw in text.ToLowerInvariant() )
			{
				char c = raw;

				if ( Char.IsLetterOrDigit( c ) || c == '\'' )
				{
					word.Append( c );
					continue;
				}

				IndexWord( word, section, weight );
			}

			IndexWord( word, section, weight );
		}

		private static void IndexWord( StringBuilder word, int section, int weight )
		{
			if ( word.Length < 3 )
			{
				word.Length = 0;
				return;
			}

			string value = word.ToString();
			word.Length = 0;

			if ( IsStopWord( value ) )
				return;

			Dictionary<int, int> hits;

			if ( !m_LoreIndex.TryGetValue( value, out hits ) )
			{
				hits = new Dictionary<int, int>();
				m_LoreIndex[value] = hits;
			}

			int count;
			hits.TryGetValue( section, out count );
			hits[section] = count + weight;
		}

		private static string TruncateAtWord( string text, int max )
		{
			if ( String.IsNullOrEmpty( text ) || max <= 0 )
				return String.Empty;

			if ( text.Length <= max )
				return text;

			int cut = text.LastIndexOf( ' ', Math.Min( max, text.Length - 1 ) );

			if ( cut < max / 2 )
				cut = max;

			string result = text.Substring( 0, cut ).TrimEnd();

			if ( result.Length > 0 && Char.IsHighSurrogate( result[result.Length - 1] ) )
				result = result.Substring( 0, result.Length - 1 );

			if ( result.Length > max - 3 )
				result = result.Substring( 0, Math.Max( 0, max - 3 ) ).TrimEnd();

			return result + "...";
		}

		private static void EnsureLoaded()
		{
			if ( m_Loaded )
				return;

			m_Loaded = true;
			LoadConfig();
		}

		private static void LoadConfig()
		{
			string path = Path.Combine( Core.BaseDirectory, ConfigPath );
			bool found = false;

			if ( File.Exists( path ) )
			{
				found = true;

				try
				{
					using ( StreamReader ip = new StreamReader( path ) )
					{
						string line;

						while ( ( line = ip.ReadLine() ) != null )
						{
							line = line.Trim();

							if ( line.Length == 0 || line[0] == '#' )
								continue;

							int eq = line.IndexOf( '=' );

							if ( eq <= 0 )
								continue;

							string key = line.Substring( 0, eq ).Trim().ToLowerInvariant();
							string value = line.Substring( eq + 1 ).Trim();

							ApplySetting( key, value );
						}
					}
				}
				catch ( Exception e )
				{
					Console.WriteLine( "[GypsyOracle] Could not read {0}: {1}", ConfigPath, e.Message );
				}
			}

			// The environment wins, so a key never has to live in a file.
			string environmentKey = Environment.GetEnvironmentVariable( "COMMANDCODE_API_KEY" );

			if ( !String.IsNullOrEmpty( environmentKey ) )
				m_ApiKey = environmentKey.Trim();

			if ( m_ApiKey.Length == 0 )
				Console.WriteLine( "[GypsyOracle] No API key configured. Copy {0}.example to {0} and set ApiKey, or set COMMANDCODE_API_KEY.", ConfigPath );
			else if ( !found )
				Console.WriteLine( "[GypsyOracle] No {0}; using defaults with the COMMANDCODE_API_KEY environment key.", ConfigPath );
			else
				Console.WriteLine( "[GypsyOracle] Ready; model {0}.", m_Model );
		}

		private static void ApplySetting( string key, string value )
		{
			switch ( key )
			{
				case "enabled":
					m_Enabled = ParseBool( value, m_Enabled );
					break;
				case "endpoint":
					if ( value.Length > 0 )
						m_Endpoint = value;
					break;
				case "apikey":
					m_ApiKey = value;
					break;
				case "model":
					if ( value.Length > 0 )
						m_Model = value;
					break;
				case "systemprompt":
					if ( value.Length > 0 )
						m_SystemPrompt = value;
					break;
				case "timeoutseconds":
					m_TimeoutSeconds = ParseInt( value, m_TimeoutSeconds, 5, 120 );
					break;
				case "maxquestionlength":
					m_MaxQuestionLength = ParseInt( value, m_MaxQuestionLength, 20, 600 );
					break;
				case "maxanswerlength":
					m_MaxAnswerLength = ParseInt( value, m_MaxAnswerLength, 120, 2000 );
					break;
				case "maxtokens":
					m_MaxTokens = ParseInt( value, m_MaxTokens, 50, 1000 );
					break;
				case "cooldownseconds":
					m_CooldownSeconds = ParseInt( value, m_CooldownSeconds, 0, 600 );
					break;
				case "historyturns":
					m_HistoryTurns = ParseInt( value, m_HistoryTurns, 0, 12 );
					break;
				case "temperature":
					m_Temperature = ParseDouble( value, m_Temperature, 0.0, 2.0 );
					break;
				case "loreenabled":
					m_LoreEnabled = ParseBool( value, m_LoreEnabled );
					break;
				case "lorefile":
					if ( value.Length > 0 )
						m_LoreFile = value;
					break;
				case "loremaxcharacters":
					m_LoreMaxCharacters = ParseInt( value, m_LoreMaxCharacters, 0, 20000 );
					break;
			}
		}

		private static bool ParseBool( string value, bool fallback )
		{
			bool result;

			if ( Boolean.TryParse( value, out result ) )
				return result;

			return fallback;
		}

		private static int ParseInt( string value, int fallback, int min, int max )
		{
			int result;

			if ( !Int32.TryParse( value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result ) )
				return fallback;

			if ( result < min )
				result = min;

			if ( result > max )
				result = max;

			return result;
		}

		private static double ParseDouble( string value, double fallback, double min, double max )
		{
			double result;

			if ( !Double.TryParse( value, NumberStyles.Float, CultureInfo.InvariantCulture, out result ) )
				return fallback;

			if ( result < min )
				result = min;

			if ( result > max )
				result = max;

			return result;
		}
	}
}
