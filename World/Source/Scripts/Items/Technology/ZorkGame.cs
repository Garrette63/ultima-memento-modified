using System;
using System.Collections.Generic;
using System.Text;
using Server;

namespace Server.Items
{
	// A compact but genuinely playable Zork-style text adventure engine.
	// Each mobile gets its own independent save, keyed by serial.
	public static partial class ZorkGame
	{
		public class State
		{
			public string Room;
			public List<string> Log = new List<string>();
			public Dictionary<string, bool> Flags = new Dictionary<string, bool>();
			public Dictionary<string, string> Where = new Dictionary<string, string>();
			public int Moves;
			public int Score;
			public bool Won;
			public List<string> Trophies = new List<string>();
			public int TrollHits;
			public int HP;
			public int ThiefHP;
			public int ThiefEngrossed;
			public int Base;      // Zork's BASE-SCORE: VALUE points, rooms, bonuses, death penalties
			public int LampStage; // which LAMP-TABLE warning comes next
			public int LampTicks; // turns of light left in the current stage
			public int Matches;   // matches left in the matchbook
			public int MatchTurns; // turns the lit match has left
			public int SwordGlow; // 0 calm, 1 something nearby, 2 something here
			public int CandleStage;
			public int CandleTicks;
			public int ThiefUnconsciousTurns;
			public int Mode;      // 0 superbrief, 1 brief, 2 verbose

			public bool Flag( string key )
			{
				bool v;
				return Flags.TryGetValue( key, out v ) && v;
			}

			public void Set( string key, bool value )
			{
				Flags[key] = value;
			}

			public string Loc( string id )
			{
				string v;
				if ( Where.TryGetValue( id, out v ) )
					return v;

				return null;
			}

			public string Status()
			{
				return ZorkGame.RoomTitle( Room ).ToUpper() + "   HP:" + HP + "/" + ZorkGame.MaxHealth + "   MOVES:" + Moves;
			}

			public string Render()
			{
				int max = 30;
				int start = Log.Count - max;

				if ( start < 0 )
					start = 0;

				StringBuilder sb = new StringBuilder();

				for ( int i = start; i < Log.Count; i++ )
				{
					if ( sb.Length > 0 )
						sb.Append( "<BR>" );

					sb.Append( Escape( Log[i] ) );
				}

				return sb.ToString();
			}
		}

		public const int MaxHealth = 10;

		private static readonly Dictionary<Serial, State> m_Saves = new Dictionary<Serial, State>();

		public static State Get( Mobile from )
		{
			State st;

			if ( from != null && m_Saves.TryGetValue( from.Serial, out st ) )
				return st;

			st = NewState();
			Boot( st );

			if ( from != null )
				m_Saves[from.Serial] = st;

			return st;
		}

		private static State NewState()
		{
			State st = new State();

			InitWorld( st );

			st.HP = MaxHealth;
			st.ThiefHP = 4;

			// The dumbwaiter starts with its basket at the top of the shaft.
			st.Set( "basketup", true );

			// Light sources burn down; the matchbook holds six matches.
			st.LampStage = 0;
			st.LampTicks = 100;
			st.Matches = 6;
			st.MatchTurns = 0;
			st.SwordGlow = 0;
			st.CandleStage = 0;
			st.CandleTicks = 20;
			st.Mode = 2;

			return st;
		}

		private static void Boot( State st )
		{
			st.Log.Add( "ZORK I: The Great Underground Empire" );
			st.Log.Add( "A small port for this terminal." );
			st.Log.Add( "Type HELP for a list of verbs." );
			Look( st );
		}

		public static string C64( string inner )
		{
			return "<BODY><BASEFONT Color=#00FF06>" + inner + "</BASEFONT></BODY>";
		}

		public static void Process( State st, string input )
		{
			ProcessCore( st, input );

			if ( !string.IsNullOrEmpty( input ) )
			{
				TurnTick( st );
				ThiefTick( st );
			}
		}

		private static void ProcessCore( State st, string input )
		{
			if ( st == null )
				return;

			if ( string.IsNullOrEmpty( input ) )
			{
				st.Log.Clear();
				Look( st );
				return;
			}

			Log( st, "> " + input );
			st.Moves++;

			string[] w = input.ToLower().Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if ( w.Length == 0 )
			{
				Look( st );
				return;
			}

			string verb = w[0];
			string rest = w.Length > 1 ? string.Join( " ", w, 1, w.Length - 1 ) : "";

			switch ( verb )
			{
				case "n": case "north": Go( st, "north" ); return;
				case "s": case "south": Go( st, "south" ); return;
				case "e": case "east": Go( st, "east" ); return;
				case "w": case "west": Go( st, "west" ); return;
				case "u": case "up": Go( st, "up" ); return;
				case "d": case "down": Go( st, "down" ); return;
				case "ne": case "northeast": case "north-east": Go( st, "ne" ); return;
				case "nw": case "northwest": case "north-west": Go( st, "nw" ); return;
				case "se": case "southeast": case "south-east": Go( st, "se" ); return;
				case "sw": case "southwest": case "south-west": Go( st, "sw" ); return;
				case "land": case "ashore": Go( st, "land" ); return;
				case "go": case "walk": case "run": case "head":
				{
					string dir = DirectionOf( rest );

					if ( dir != null )
					{
						Go( st, dir );
						return;
					}

					Log( st, "You can't go that way." );
					return;
				}
				case "in": case "enter":
				{
					ZRoom here;

					if ( _rooms.TryGetValue( st.Room, out here ) && here.Exits.ContainsKey( "in" ) )
					{
						Go( st, "in" );
						return;
					}

					Enter( st, rest );
					return;
				}
				case "out": Go( st, "out" ); return;
				case "l": case "look": st.Log.Clear(); Look( st ); SwordGlowCheck( st ); return;
				case "examine": case "x": case "inspect": Examine( st, rest ); return;
				case "i": case "inv": case "inventory": Inventory( st ); return;
				case "take": case "get": case "grab": Take( st, rest ); return;
				case "drop": case "discard": Drop( st, rest ); return;
				case "put": PutIn( st, rest ); return;
				case "attack": case "kill": case "hit": Attack( st, rest ); SwordGlowCheck( st ); return;
				case "open": Open( st, rest ); return;
				case "close": case "shut": Close( st, rest ); return;
				case "read": Read( st, rest ); return;
				case "move": case "push": case "pull": case "shove": MoveThing( st, rest ); return;
				case "turn": case "switch": Turn( st, rest ); return;
				case "climb": Climb( st, rest ); return;
				case "wave": Wave( st, rest ); return;
				case "ring": Ring( st, rest ); return;
				case "tie": Tie( st, rest ); return;
				case "inflate": Inflate( st, false ); return;
				case "deflate": Inflate( st, true ); return;
				case "dig": Dig( st ); return;
				case "launch": case "board": Launch( st ); return;
				case "give": Give( st, rest ); return;
				case "raise": RaiseLower( st, true ); return;
				case "lower": RaiseLower( st, false ); return;
				case "rub": Rub( st, rest ); return;
				case "eat": Eat( st, rest ); return;
				case "drink": Drink( st, rest ); return;
				case "count": Count( st, rest ); return;
				case "blow": BlowOut( st ); return;
				case "throw": case "toss": Throw( st, rest ); return;
				case "wind": Wind( st, rest ); return;
				case "pray": Pray( st ); return;
				case "yell": case "scream": Log( st, "Aaaarrrrgggghhhh!" ); return;
				case "zork": Log( st, "At your service!" ); return;
				case "hello": case "hi": Hello( st, rest ); return;
				case "swim": Swim( st ); return;
				case "leap": case "jump": Leap( st, rest ); return;
				case "verbose": st.Mode = 2; Log( st, "Maximum verbosity." ); return;
				case "brief": st.Mode = 1; Log( st, "Brief descriptions." ); return;
				case "superbrief": st.Mode = 0; Log( st, "Superbrief descriptions." ); return;
				case "diagnose": Diagnose( st ); return;
				case "unlock": Unlock( st, rest ); return;
				case "lock": LockGrate( st ); return;
				case "say": case "shout": Say( st, rest ); return;
				case "echo": Say( st, "echo" ); return;
				case "light": Light( st, rest ); return;
				case "wait": case "z": Log( st, "Time passes..." ); return;
				case "help": case "?": Help( st ); return;
				case "score": Log( st, ScoreLine( st ) ); return;
				case "restart": case "again": Restart( st ); return;
				case "save": case "restore": Log( st, "This terminal cannot save games." ); return;
				case "quit": case "q": Log( st, "The terminal powers down. Close the window to stop." ); return;
				default: Log( st, "I don't know the word \"" + verb + "\"." ); return;
			}
		}

		// Maps any accepted direction spelling to the key used by the room exits.
		private static string DirectionOf( string text )
		{
			if ( text == null )
				return null;

			switch ( text.Trim().ToLower() )
			{
				case "n": case "north": return "north";
				case "s": case "south": return "south";
				case "e": case "east": return "east";
				case "w": case "west": return "west";
				case "ne": case "northeast": case "north-east": return "ne";
				case "nw": case "northwest": case "north-west": return "nw";
				case "se": case "southeast": case "south-east": return "se";
				case "sw": case "southwest": case "south-west": return "sw";
				case "u": case "up": return "up";
				case "d": case "down": return "down";
				case "in": case "enter": return "in";
				case "out": return "out";
				case "land": case "ashore": return "land";
			}

			return null;
		}

		private static void Log( State st, string line )
		{
			st.Log.Add( line );

			if ( st.Log.Count > 240 )
				st.Log.RemoveRange( 0, 120 );
		}

		private static void Look( State st )
		{
			if ( IsDark( st ) )
			{
				Log( st, "It is pitch black. You are likely to be eaten by a grue." );
				return;
			}

			ZRoom r = _rooms[st.Room];

			Log( st, "-- " + r.Title + " --" );

			// VERBOSE always describes; BRIEF only the first time; SUPERBRIEF never.
			bool seen = st.Flag( "visited:" + st.Room );
			bool full = st.Mode == 2 || ( st.Mode == 1 && !seen );

			if ( full )
			{
				if ( st.Room == "livingroom" )
					Log( st, LivingRoomDesc( st ) );
				else if ( st.Room == "kitchen" )
					Log( st, KitchenDesc( st ) );
				else if ( !string.IsNullOrEmpty( r.Desc ) )
					Log( st, r.Desc );

				DynamicLines( st );
			}

			st.Set( "visited:" + st.Room, true );

			List<string> here = PresentThings( st );

			foreach ( string id in here )
			{
				if ( !_things[id].Listed )
					continue;

				Log( st, "There is " + _things[id].Name + " here." );
			}

			if ( r.Exits.Count > 0 )
			{
				string[] order = new string[] { "north", "south", "east", "west", "ne", "nw", "se", "sw", "up", "down", "in", "out", "land" };
				StringBuilder sb = new StringBuilder( "Obvious exits: " );
				bool first = true;

				foreach ( string dir in order )
				{
					if ( !r.Exits.ContainsKey( dir ) )
						continue;

					if ( !first )
						sb.Append( ", " );

					sb.Append( dir );
					first = false;
				}

				Log( st, sb.ToString() );
			}

			if ( st.Trophies.Count > 0 && st.Loc( "trophycase" ) == st.Room )
				Log( st, "The trophy case contains " + TrophyList( st ) + "." );
		}

		// The Living Room description changes as the rug, trap door and door do,
		// exactly as LIVING-ROOM-FCN rewrites it on every look.
		private static string LivingRoomDesc( State st )
		{
			StringBuilder sb = new StringBuilder( "You are in the living room. There is a doorway to the east" );

			if ( st.Flag( "magicflag" ) )
				sb.Append( ". To the west is a cyclops-shaped opening in an old wooden door, above which is some strange gothic lettering, " );
			else
				sb.Append( ", a wooden door with strange gothic lettering to the west, which appears to be nailed shut, " );

			sb.Append( "a trophy case, and " );

			bool rug = st.Flag( "rugmoved" );
			bool door = st.Flag( "trapdooropen" );

			if ( rug && door )
				sb.Append( "a rug lying beside an open trap door." );
			else if ( rug )
				sb.Append( "a closed trap door at your feet." );
			else if ( door )
				sb.Append( "an open trap door at your feet." );
			else
				sb.Append( "a large oriental rug in the center of the room." );

			return sb.ToString();
		}

		private static string KitchenDesc( State st )
		{
			return "You are in the kitchen of the white house. A table seems to have been used recently for the preparation of food. A passage leads to the west and a dark staircase can be seen leading upward. A dark chimney leads down and to the east is a small window which is "
				+ ( st.Flag( "windowopen" ) ? "open." : "slightly ajar." );
		}

		// The conditional second half of a room description (the ZIL's M-LOOK
		// routines append these depending on flags).
		private static void DynamicLines( State st )
		{
			switch ( st.Room )
			{
				case "loudroom":
					Log( st, st.Flag( "loudquiet" )
						? "The room is eerie in its quietness."
						: "The room is deafeningly loud with an undetermined rushing sound. The sound seems to reverberate from all directions." );
					break;

				case "torchroom":
					if ( st.Flag( "domeflag" ) )
						Log( st, "A piece of rope descends from the railing above, ending some five feet above your head." );
					break;

				case "domeroom":
					if ( st.Flag( "domeflag" ) )
						Log( st, "Hanging down from the railing is a rope which ends about ten feet from the floor below." );
					break;

				case "damroom":
					if ( st.Flag( "lowtide" ) )
						Log( st, "The water level behind the dam is low: The sluice gates have been opened. Water rushes through the dam and downstream." );
					else if ( st.Flag( "boltturned" ) )
						Log( st, "The sluice gates are open, and water rushes through the dam. The water level behind the dam is still high." );
					else
						Log( st, "The sluice gates are closed. The water level in the reservoir is quite low, but the level is rising quickly." );
					break;
			}
		}

		private static string TrophyList( State st )
		{
			StringBuilder sb = new StringBuilder();

			for ( int i = 0; i < st.Trophies.Count; i++ )
			{
				if ( i > 0 )
					sb.Append( i == st.Trophies.Count - 1 ? " and " : ", " );

				sb.Append( _things[st.Trophies[i]].Name );
			}

			return sb.ToString();
		}

		private static void Go( State st, string dir )
		{
			if ( IsDark( st ) && Utility.Random( 100 ) < 30 )
			{
				Grue( st );
				return;
			}

			ZRoom r = _rooms[st.Room];
			string dest;

			if ( !r.Exits.TryGetValue( dir, out dest ) )
			{
				string wall;

				if ( r.Walls.TryGetValue( dir, out wall ) )
					Log( st, wall );
				else
					Log( st, "You can't go that way." );

				return;
			}

			ZGate gate;
			string travelMsg = null;

			if ( _gates.TryGetValue( st.Room + "|" + dir, out gate ) )
			{
				switch ( gate.Key )
				{
					// The maze diodes: a one-way tunnel that ends blind in MAZE-2.
					case "MAZE-DIODES":
					{
						string next = MazeDiodeDest( st.Room );

						if ( next == null )
						{
							Log( st, "You won't be able to get back up to the tunnel you are going through when it gets to the next room." );
							return;
						}

						// Logged after the room description, since moving clears the scroll.
						travelMsg = "You won't be able to get back up to the tunnel you are going through when it gets to the next room.";
						dest = next;
						break;
					}

					// The Studio chimney: only the lantern is light enough.
					case "UP-CHIMNEY-FUNCTION":
					{
						int carried = Carried( st );

						if ( carried == 0 )
						{
							Log( st, "Going up empty-handed is a bad idea." );
							return;
						}

						if ( carried > 1 || !Carrying( st, "lamp" ) )
						{
							Log( st, "You can't get up there with what you're carrying." );
							return;
						}

						break;
					}

					case "TRAP-DOOR-EXIT":
					{
						if ( !st.Flag( "rugmoved" ) )
						{
							Log( st, "You can't go that way." );
							return;
						}

						if ( !st.Flag( "trapdooropen" ) )
						{
							Log( st, "The trap door is closed." );
							return;
						}

						break;
					}

					case "GRATING-EXIT":
					{
						if ( !st.Flag( "gratererevealed" ) )
						{
							Log( st, "You can't go that way." );
							return;
						}

						if ( !st.Flag( "grateopen" ) )
						{
							Log( st, "The grating is closed!" );
							return;
						}

						break;
					}

					default:
						if ( !GateOK( st, gate.Key ) )
						{
							Log( st, gate.Msg );
							return;
						}

						break;
				}
			}

			st.Room = dest;
			st.Moves++;

			ZRoom to = _rooms[dest];

			if ( to.Value > 0 && !st.Flag( "seen:" + dest ) )
			{
				st.Set( "seen:" + dest, true );
				Award( st, to.Value );
			}

			// Entering a new area clears the screen; ordinary actions just append.
			st.Log.Clear();
			Look( st );

			if ( travelMsg != null )
				Log( st, travelMsg );

			AfterEnter( st );
		}

		// Things that happen the moment you arrive somewhere: coal gas, the
		// vampire bat, and the thief making himself known.
		private static void AfterEnter( State st )
		{
			if ( BoomCheck( st ) )
				return;

			if ( st.Room == "batroom" && !GarlicHere( st ) )
			{
				FlyMe( st );
				return;
			}

			if ( !st.Flag( "thiefdead" ) && st.Loc( "thief" ) == st.Room )
				ThiefAppear( st );

			// The lit lower shaft is worth 13 points, once.
			if ( st.Room == "lowershaft" && !st.Flag( "lightshaft" ) && !IsDark( st ) )
			{
				st.Set( "lightshaft", true );
				Award( st, LightShaftBonus );
			}

			// A gust of wind in the Tiny Cave may blow the candles out.
			if ( st.Room == "tinycave" && Burning( st, "candles" ) && ( Carrying( st, "candles" ) || st.Loc( "candles" ) == st.Room ) && Utility.Random( 100 ) < 50 )
			{
				st.Set( "flameoff:candles", true );
				Log( st, "A gust of wind blows out your candles!" );
			}

			SwordGlowCheck( st );
		}

		// MAZE-DIODES: MAZE-7, MAZE-9 and MAZE-12 drop you elsewhere; MAZE-2 ends blind.
		private static string MazeDiodeDest( string room )
		{
			switch ( room )
			{
				case "maze7": return "deadend1";
				case "maze9": return "maze11";
				case "maze12": return "maze5";
			}

			return null;
		}

		private static bool GateOK( State st, string key )
		{
			switch ( key )
			{
				case "WON-FLAG": return st.Flag( "won" );
				case "KITCHEN-WINDOW-OPEN": return st.Flag( "windowopen" );
				case "TRAP-DOOR-EXIT":
				case "TRAP-DOOR-OPEN": return st.Flag( "trapdooropen" ) && st.Flag( "rugmoved" );
				case "GRATING-EXIT":
				case "GRATE-OPEN": return st.Flag( "grateopen" );
				case "UP-CHIMNEY-FUNCTION": return true;
				case "MAZE-DIODES": return true;
				case "MAGIC-FLAG": return st.Flag( "magicflag" );
				case "TROLL-FLAG": return st.Flag( "trolldead" );
				case "CYCLOPS-FLAG": return st.Flag( "cyclopsgone" );
				case "LLD-FLAG": return st.Flag( "lldflag" );
				case "DOME-FLAG": return st.Flag( "domeflag" );
				case "COFFIN-CURE": return st.Loc( "coffin" ) == "@inv";
				case "DEFLATE": return !st.Flag( "boatinflated" );
				case "RAINBOW-FLAG": return st.Flag( "rainbowflag" );
				case "EMPTY-HANDED":
				{
					// Zork's crawlways allow anything weighing 4 or less.
					foreach ( string id in _things.Keys )
					{
						if ( st.Loc( id ) == "@inv" && _things[id].Size > 4 )
							return false;
					}

					return true;
				}
				case "FALSE-FLAG": return false;
				case "LOW-TIDE": return st.Flag( "lowtide" );
			}

			return st.Flag( key.ToLower().Replace( "-", "" ) );
		}

		private static int Carried( State st )
		{
			int n = 0;

			foreach ( string id in _things.Keys )
			{
				if ( st.Loc( id ) == "@inv" )
					n++;
			}

			return n;
		}

		private static bool Carrying( State st, string id )
		{
			return st.Loc( id ) == "@inv";
		}

		private static void Enter( State st, string what )
		{
			if ( st.Room == "eastofhouse" )
			{
				if ( !st.Flag( "windowopen" ) )
				{
					Log( st, "The window is closed." );
					return;
				}

				st.Room = "kitchen";
				st.Moves++;
				st.Log.Clear();
				Look( st );
				return;
			}

			if ( st.Room == "kitchen" )
			{
				st.Room = "eastofhouse";
				st.Moves++;
				st.Log.Clear();
				Look( st );
				return;
			}

			Log( st, "You can't enter that." );
		}

		private static void Examine( State st, string what )
		{
			string id = Find( st, what );

			if ( id == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			Log( st, _things[id].Desc );

			if ( id == "lamp" )
			{
				string state = st.Flag( "lampburned" ) ? "has burned out." : ( st.Flag( "lampon" ) ? "is on." : "is turned off." );

				Log( st, "The lamp " + state );
			}
		}

		private static void Inventory( State st )
		{
			List<string> carried = new List<string>();

			foreach ( string id in _things.Keys )
			{
				if ( st.Loc( id ) == "@inv" )
					carried.Add( _things[id].Name );
			}

			if ( carried.Count == 0 )
				Log( st, "You are empty-handed." );
			else
				Log( st, "You are carrying: " + string.Join( ", ", carried.ToArray() ) );
		}

		private static void Take( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Take what?" );
				return;
			}

			string id = Find( st, what );

			if ( id == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			if ( id == "bat" )
			{
				if ( GarlicHere( st ) )
				{
					Log( st, "You can't reach him; he's on the ceiling." );
					return;
				}

				FlyMe( st );
				return;
			}

			if ( st.Loc( id ) == "@inv" )
			{
				Log( st, "You already have that." );
				return;
			}

			if ( !_things[id].Portable )
			{
				Log( st, "You can't take that." );
				return;
			}

			// Some things are held by whoever is guarding them.
			string host = st.Loc( id );

			if ( host == "troll" && !st.Flag( "trolldead" ) )
			{
				Log( st, "The troll snarls and keeps his axe." );
				return;
			}

			if ( host == "thief" && !st.Flag( "thiefdead" ) )
			{
				Log( st, "The thief slaps your hand away." );
				return;
			}

			if ( id == "chalice" && !st.Flag( "thiefdead" ) )
			{
				Log( st, "The chalice seems to be protected by the thief's magic." );
				return;
			}

			st.Where[id] = "@inv";
			Log( st, "Taken." );

			if ( IsTreasure( id ) && !st.Flag( "took:" + id ) )
			{
				st.Set( "took:" + id, true );
				Award( st, _things[id].Value );
			}

			// The candles only start burning down once you have handled them.
			if ( id == "candles" )
				st.Set( "candlestouched", true );
		}

		private static void Drop( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Drop what?" );
				return;
			}

			string id = FindCarried( st, what );

			if ( id == null )
			{
				Log( st, "You aren't carrying that." );
				return;
			}

			st.Where[id] = st.Room;
			Log( st, "Dropped." );
		}

		private static void PutIn( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Put what where?" );
				return;
			}

			string lower = what.ToLower();
			int pos = lower.IndexOf( " into " );
			int len = 6;

			if ( pos < 0 )
			{
				pos = lower.IndexOf( " in " );
				len = 4;
			}

			if ( pos < 0 )
			{
				// No container named: behave like a normal drop.
				Drop( st, what );
				return;
			}

			string itemText = what.Substring( 0, pos );
			string contText = what.Substring( pos + len );

			string cont = Find( st, contText );

			if ( cont == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			if ( cont != "trophycase" && !_things[cont].Container )
			{
				Log( st, "You can't put things in that." );
				return;
			}

			// The trophy case is open-topped (TRANSBIT, no OPENBIT) in the source.
			if ( cont != "trophycase" && _things[cont].Openable && !st.Flag( "open:" + cont ) )
			{
				Log( st, "The " + _things[cont].Name + " isn't open." );
				return;
			}

			string id = FindCarried( st, itemText );

			if ( id == null )
			{
				Log( st, "You aren't carrying that." );
				return;
			}

			if ( cont != "trophycase" )
			{
				if ( _things[cont].Capacity > 0 && ItemWeight( st, cont ) + _things[id].Size > _things[cont].Capacity )
				{
					Log( st, "There's no room." );
					return;
				}

				st.Where[id] = cont;
				Log( st, "Done." );
				return;
			}

			if ( !IsTreasure( id ) )
			{
				Log( st, "The trophy case is only for treasures." );
				return;
			}

			if ( st.Trophies.Contains( id ) )
			{
				Log( st, "That treasure is already in the trophy case." );
				return;
			}

			st.Where[id] = "@case";
			st.Trophies.Add( id );

			// Finding it counts too, even if it went straight into the case.
			if ( !st.Flag( "took:" + id ) )
			{
				st.Set( "took:" + id, true );
				st.Base += _things[id].Value;
			}

			Recalc( st );
			Log( st, "You place the " + _things[id].Name + " in the trophy case: +" + _things[id].TValue + " points." );

			if ( !st.Won && st.Trophies.Count >= TreasureCount() )
				Log( st, "Every treasure is in the case, but your score is not yet complete: some of the dungeon's points are still unearned." );
		}

		private static bool Hurt( State st, int amount )
		{
			st.HP -= amount;

			if ( st.HP > 0 )
				return false;

			st.HP = MaxHealth;
			st.Room = "westofhouse";
			st.Log.Clear();
			Log( st, "You have been killed! You awaken, dazed, west of the white house." );
			Award( st, -10 );
			Look( st );
			return true;
		}

		private static void Attack( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Attack what?" );
				return;
			}

			// --- Bat ---
			if ( st.Room == "batroom" && Matches( "bat", what ) )
			{
				if ( GarlicHere( st ) )
				{
					Log( st, "You can't reach him; he's on the ceiling." );
					return;
				}

				FlyMe( st );
				return;
			}

			// --- Mirror ---
			if ( ( st.Room == "mirrorroom1" || st.Room == "mirrorroom2" ) && what.ToLower().IndexOf( "mirror" ) >= 0 )
			{
				st.Set( "mirrormunged", true );
				Log( st, "The mirror breaks into a thousand pieces with a tremendous crash." );
				return;
			}

			// --- Egg ---
			if ( Matches( "egg", what ) && ( st.Loc( "egg" ) == st.Room || st.Loc( "egg" ) == "@inv" ) )
			{
				st.Where["egg"] = "@dead";
				st.Where["brokenegg"] = st.Room;
				st.Where["canary"] = "@dead";
				st.Where["brokencanary"] = "brokenegg";
				Log( st, "The egg smashes. Inside is a golden clockwork canary, its mechanism crushed." );
				return;
			}

			// --- Cyclops ---
			if ( st.Room == "cyclopsroom" && !st.Flag( "cyclopsgone" ) && Matches( "cyclops", what ) )
			{
				Log( st, "The cyclops picks you up and hurls you against the wall." );
				Hurt( st, MaxHealth );
				return;
			}

			// --- Troll ---
			if ( st.Room == "trollroom" && !st.Flag( "trolldead" ) && Matches( "troll", what ) )
			{
				if ( st.Loc( "sword" ) != "@inv" )
				{
					Log( st, "You have no weapon! You flail at the troll and it bats you aside." );
					return;
				}

				st.TrollHits++;

				if ( st.TrollHits >= 3 )
				{
					st.Set( "trolldead", true );
					st.Where["troll"] = "@dead";
					Log( st, "You drive the elvish sword home and the troll crashes to the floor, dead." );
					return;
				}

				Log( st, "You wound the troll. It swings its bloody axe; you leap clear." );
				return;
			}

			// --- Thief ---
			if ( !st.Flag( "thiefdead" ) && st.Loc( "thief" ) == st.Room && Matches( "thief", what ) )
			{
				if ( st.Loc( "sword" ) != "@inv" )
				{
					Log( st, "You have no weapon! The thief sidesteps and stabs you." );

					Hurt( st, 1 );
					return;
				}

				st.ThiefHP--;
				Log( st, "You slash the thief with the elvish sword." );

				if ( Hurt( st, 1 ) )
					return;

				if ( st.ThiefHP > 0 && Utility.Random( 100 ) < 25 )
				{
					st.Set( "thiefunconscious", true );
					st.ThiefUnconsciousTurns = 8;
					st.Where["stiletto"] = st.Room;
					Log( st, "The thief collapses, unconscious." );
					return;
				}

				if ( st.ThiefHP <= 0 )
				{
					ThiefDeath( st );
				}
				else
				{
					Log( st, "The thief snarls and cuts you with his stiletto." );
				}

				return;
			}

			Log( st, "There is nothing here to fight." );
		}

		private static List<string> m_Treasures;

		private static List<string> Treasures()
		{
			if ( m_Treasures != null )
				return m_Treasures;

			m_Treasures = new List<string>();
			State probe = NewState();

			// Items that only exist as debris once something has been broken are
			// not part of the score; everything else that carries a value is.
			string[] junk = new string[] { "brokenegg", "brokencanary", "brokenlamp", "hotbell", "gunk", "puncturedboat" };

			foreach ( string id in _things.Keys )
			{
				if ( !IsTreasure( id ) )
					continue;

				if ( Array.IndexOf( junk, id ) >= 0 )
					continue;

				string loc = probe.Loc( id );

				if ( loc != null && loc == "@dead" && id != "diamond" )
					continue;

				m_Treasures.Add( id );
			}

			return m_Treasures;
		}

		private static bool IsTreasure( string id )
		{
			ZThing t;

			if ( !_things.TryGetValue( id, out t ) )
				return false;

			return t.Value > 0 || t.TValue > 0;
		}

		private static int TreasureCount()
		{
			return Treasures().Count;
		}

		// Zork I scores BASE-SCORE (values found, rooms seen, bonuses, deaths)
		// plus the TVALUE of everything currently sitting in the trophy case.
		private static int MaxScore()
		{
			int total = LightShaftBonus;

			foreach ( string id in Treasures() )
				total += _things[id].Value + _things[id].TValue;

			foreach ( string id in _rooms.Keys )
				total += _rooms[id].Value;

			return total;
		}

		private const int LightShaftBonus = 13;

		private static void Award( State st, int points )
		{
			st.Base += points;

			if ( st.Base < 0 )
				st.Base = 0;

			Recalc( st );
		}

		private static void Recalc( State st )
		{
			int banked = 0;

			foreach ( string id in st.Trophies )
				banked += _things[id].TValue;

			st.Score = st.Base + banked;

			if ( !st.Won && st.Score >= MaxScore() )
			{
				st.Won = true;
				Log( st, "You have collected every treasure. YOU HAVE WON!" );
				Log( st, "The Great Underground Empire is yours. Rank: Master Adventurer." );
				Log( st, "Final score: " + st.Score + " of " + MaxScore() + " in " + st.Moves + " moves. Type RESTART to play again." );
			}
		}

		// The exact ladder from V-SCORE in 1actions.zil.
		private static string Rank( State st )
		{
			int s = st.Score;

			if ( s >= 350 ) return "Master Adventurer";
			if ( s > 330 ) return "Wizard";
			if ( s > 300 ) return "Master";
			if ( s > 200 ) return "Adventurer";
			if ( s > 100 ) return "Junior Adventurer";
			if ( s > 50 ) return "Novice Adventurer";
			if ( s > 25 ) return "Amateur Adventurer";

			return "Beginner";
		}

		private static string ScoreLine( State st )
		{
			return "Your score is " + st.Score + " (total of " + MaxScore() + " points), in " + st.Moves + " moves. Treasures: "
				+ st.Trophies.Count + "/" + TreasureCount() + ". This gives you the rank of " + Rank( st ) + ".";
		}

		public static string ScoreText( State st )
		{
			return "SCORE " + st.Score + "/" + MaxScore();
		}

		private static void Open( State st, string what )
		{
			string id = Find( st, what );

			if ( id == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			switch ( id )
			{
				case "kitchenwindow":
				{
					if ( st.Flag( "windowopen" ) )
					{
						Log( st, "It is already open." );
						break;
					}

					st.Set( "windowopen", true );
					Log( st, "With great effort, you open the window far enough to allow entry." );
					break;
				}

				case "trapdoor":
				{
					if ( !st.Flag( "rugmoved" ) )
					{
						Log( st, "You can't reach it." );
						break;
					}

					if ( st.Flag( "trapdooropen" ) )
					{
						Log( st, "It is already open." );
						break;
					}

					st.Set( "trapdooropen", true );
					Log( st, "The trap door opens, revealing a dusty stairway going down." );
					break;
				}

				case "grate":
				{
					if ( !st.Flag( "grunlock" ) )
					{
						Log( st, "The grating is locked." );
						break;
					}

					if ( st.Flag( "grateopen" ) )
					{
						Log( st, "It is already open." );
						break;
					}

					st.Set( "grateopen", true );
					Log( st, st.Room == "gratingclearing" ? "The grating opens." : "The grating opens to reveal trees above you." );
					break;
				}

				case "woodendoor":
					Log( st, st.Flag( "magicflag" ) ? "The door opens." : "The door is nailed shut." );
					break;

				case "frontdoor":
				case "boardedwindow":
					Log( st, "It is boarded up." );
					break;

				default:
				{
					if ( _things[id].Openable )
					{
						st.Set( "open:" + id, true );
						Log( st, "Opened." );
					}
					else if ( _things[id].Surface )
					{
						Log( st, "It is already open." );
					}
					else
					{
						Log( st, "You can't open that." );
					}

					break;
				}
			}
		}

		private static void Close( State st, string what )
		{
			string id = Find( st, what );

			if ( id == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			switch ( id )
			{
				case "kitchenwindow":
					st.Set( "windowopen", false );
					Log( st, "The window is closed." );
					break;

				case "trapdoor":
					st.Set( "trapdooropen", false );
					Log( st, "The trap door is closed." );
					break;

				case "grate":
					st.Set( "grateopen", false );
					Log( st, "The grating is closed." );
					break;

				default:
				{
					if ( _things[id].Openable )
					{
						st.Set( "open:" + id, false );
						Log( st, "Closed." );
					}
					else
					{
						Log( st, "You can't close that." );
					}

					break;
				}
			}
		}

		private static void Read( State st, string what )
		{
			string id = Find( st, what );

			if ( id == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			if ( string.IsNullOrEmpty( _things[id].ReadText ) )
			{
				Log( st, "There is nothing to read on that." );
				return;
			}

			Log( st, _things[id].ReadText );
		}

		private static void MoveThing( State st, string what )
		{
			string id = Find( st, what );

			if ( id == "rug" || id == "carpet" )
			{
				if ( st.Flag( "rugmoved" ) )
				{
					Log( st, "The rug is already moved aside." );
					return;
				}

				st.Set( "rugmoved", true );
				Log( st, "With a rumble, the rug slides aside, revealing a closed trap door." );
				return;
			}

			if ( id == "leaves" )
			{
				if ( st.Flag( "gratererevealed" ) )
				{
					Log( st, "The grating is already revealed." );
					return;
				}

				st.Set( "gratererevealed", true );
				Log( st, "In disturbing the pile of leaves, a grating is revealed." );
				return;
			}

			if ( id == "machineswitch" || id == "switch" )
			{
				Turn( st, "on switch" );
				return;
			}

			Log( st, "You can't move that." );
		}

		private static void Turn( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Turn on or off what?" );
				return;
			}

			string text = what.ToLower();
			bool off = false;

			if ( text.StartsWith( "off" ) )
			{
				off = true;
				text = text.Substring( 3 ).Trim();
			}
			else if ( text.StartsWith( "on" ) )
			{
				text = text.Substring( 2 ).Trim();
			}

			string id = Find( st, text );

			if ( id == null )
			{
				Log( st, "You don't see that here." );
				return;
			}

			switch ( id )
			{
				case "lamp":
				{
					if ( st.Flag( "lampburned" ) )
					{
						Log( st, off ? "The lamp has already burned out." : "A burned-out lamp won't light." );
						return;
					}

					if ( st.Loc( "lamp" ) != "@inv" )
					{
						Log( st, "You'll have to pick up the brass lantern first." );
						return;
					}

					st.Set( "lampon", !off );
					Log( st, off ? "The lamp is now off." : "The lamp is now on." );

					if ( !off )
						Look( st );

					return;
				}

				case "bolt":
				{
					if ( !Carrying( st, "wrench" ) )
					{
						Log( st, "The bolt is too stiff to turn by hand." );
						return;
					}

					st.Set( "lowtide", true );
					Log( st, "You turn the bolt with the wrench. Water pours from the reservoir and the level drops." );
					return;
				}

				case "machineswitch":
				{
					st.Set( "machineon", true );

					if ( Carrying( st, "coal" ) )
					{
						st.Where["coal"] = "@dead";
						st.Where["diamond"] = "@inv";
						Log( st, "The machine shudders and coughs. A huge diamond drops into the hopper." );
					}
					else
					{
						Log( st, "The machine rumbles, but nothing comes out. It seems to want coal." );
					}

					return;
				}
			}

			Log( st, "You can't turn that on or off." );
		}

		private static void Light( State st, string what )
		{
			string text = ( what == null ? "" : what.ToLower() );

			if ( text.IndexOf( "match" ) >= 0 )
			{
				LightMatch( st );
				return;
			}

			if ( st.Loc( "lamp" ) != "@inv" )
			{
				Log( st, "You don't have a lamp." );
				return;
			}

			if ( st.Flag( "lampburned" ) )
			{
				Log( st, "A burned-out lamp won't light." );
				return;
			}

			st.Set( "lampon", true );
			Log( st, "The lamp is now on." );
			Look( st );
		}

		private static void LightMatch( State st )
		{
			if ( st.Loc( "match" ) != "@inv" )
			{
				Log( st, "You don't have any matches." );
				return;
			}

			if ( st.Matches <= 0 )
			{
				Log( st, "I'm afraid that you have run out of matches." );
				return;
			}

			st.Matches--;

			// The shaft is drafty: Zork snuffs the match out immediately.
			if ( st.Room == "lowershaft" || st.Room == "timberroom" )
			{
				Log( st, "This room is drafty, and the match goes out instantly." );
				return;
			}

			bool wasDark = IsDark( st );

			st.Set( "flameon:match", true );
			st.MatchTurns = 2;
			Log( st, "One of the matches starts to burn." );

			if ( wasDark )
				Look( st );
		}

		private static void BlowOut( State st )
		{
			if ( !st.Flag( "flameon:match" ) )
			{
				Log( st, "Nothing to blow out." );
				return;
			}

			st.Set( "flameon:match", false );
			st.MatchTurns = 0;
			Log( st, "The match is out." );
		}

		private static void Count( State st, string what )
		{
			string text = ( what == null ? "" : what.ToLower() );

			if ( text.IndexOf( "match" ) >= 0 )
			{
				Log( st, "You have " + st.Matches + ( st.Matches == 1 ? " match." : " matches." ) );
				return;
			}

			Log( st, "Counting is not necessary." );
		}

		private static void Eat( State st, string what )
		{
			string id = FindCarried( st, what );

			if ( id == null )
			{
				if ( Find( st, what ) != null )
					Log( st, "You're not holding that." );
				else
					Log( st, "You don't see that here." );

				return;
			}

			if ( !_things[id].Food )
			{
				Log( st, "That is not something you can eat." );
				return;
			}

			st.Where[id] = "@dead";
			Log( st, "Thank you very much. It really hit the spot." );
		}

		private static void Drink( State st, string what )
		{
			string id = FindCarried( st, what );

			if ( id == null )
			{
				if ( Find( st, what ) != null )
				{
					Log( st, "You're not holding that." );
					return;
				}

				if ( RoomHasWater( st ) )
					Log( st, "Thank you very much. I was rather thirsty (from all this talking, probably)." );
				else
					Log( st, "There isn't any water here." );

				return;
			}

			if ( id != "water" )
			{
				Log( st, "How can you drink that?" );
				return;
			}

			if ( st.Loc( "bottle" ) != "@inv" )
			{
				Log( st, "You have to be holding the glass bottle first." );
				return;
			}

			if ( !st.Flag( "open:bottle" ) )
			{
				Log( st, "You'll have to open the glass bottle first." );
				return;
			}

			st.Where["water"] = "@dead";
			Log( st, "Thank you very much. I was rather thirsty (from all this talking, probably)." );
		}

		private static bool RoomHasWater( State st )
		{
			string[] wet = new string[] { "reservoir", "reservoirsouth", "reservoirnorth", "streamview", "instream", "river1", "river2", "river3", "river4", "river5", "shore", "aragainfalls" };

			return Array.IndexOf( wet, st.Room ) >= 0;
		}

		private static bool VillainHere( State st, string room )
		{
			if ( !st.Flag( "trolldead" ) && st.Loc( "troll" ) == room )
				return true;

			if ( !st.Flag( "thiefdead" ) && st.Loc( "thief" ) == room )
				return true;

			if ( !st.Flag( "cyclopsgone" ) && st.Loc( "cyclops" ) == room )
				return true;

			return false;
		}

		// The elvish sword warns of villains in the room or just beyond it.
		private static void SwordGlowCheck( State st )
		{
			if ( st.Loc( "sword" ) != "@inv" )
				return;

			int glow = 0;

			if ( VillainHere( st, st.Room ) )
			{
				glow = 2;
			}
			else
			{
				ZRoom r;

				if ( _rooms.TryGetValue( st.Room, out r ) )
				{
					foreach ( string dest in r.Exits.Values )
					{
						if ( dest != st.Room && VillainHere( st, dest ) )
						{
							glow = 1;
							break;
						}
					}
				}
			}

			if ( glow == st.SwordGlow )
				return;

			st.SwordGlow = glow;

			if ( glow == 2 )
				Log( st, "Your sword has begun to glow very brightly." );
			else if ( glow == 1 )
				Log( st, "Your sword is glowing with a faint blue glow." );
			else
				Log( st, "Your sword is no longer glowing." );
		}

		private static readonly int[] LampStageTicks = new int[] { 100, 70, 15, 0 };
		private static readonly string[] LampStageText = new string[]
		{
			"The lamp appears a bit dimmer.",
			"The lamp is definitely dimmer now.",
			"The lamp is nearly out.",
			"You'd better have more light than from the brass lantern."
		};

		// The lantern battery runs down (Zork's LAMP-TABLE), and a match burns
		// for a couple of turns.
		private static void TurnTick( State st )
		{
			if ( st.Flag( "lampon" ) && !st.Flag( "lampburned" ) )
			{
				st.LampTicks--;

				if ( st.LampTicks <= 0 )
				{
					int stage = st.LampStage < LampStageText.Length ? st.LampStage : LampStageText.Length - 1;

					Log( st, LampStageText[stage] );
					st.LampStage++;

					if ( st.LampStage >= LampStageTicks.Length )
					{
						st.Set( "lampon", false );
						st.Set( "lampburned", true );
					}
					else
					{
						st.LampTicks = LampStageTicks[st.LampStage];
					}
				}
			}

			if ( st.Flag( "flameon:match" ) )
			{
				st.MatchTurns--;

				if ( st.MatchTurns <= 0 )
				{
					st.Set( "flameon:match", false );
					Log( st, "The match has gone out." );
				}
			}

			// The forest chirps now and then (I-FOREST-ROOM, 15 %).
			if ( ( st.Room == "forest1" || st.Room == "forest2" || st.Room == "forest3" || st.Room == "path" || st.Room == "upatree" ) && Utility.Random( 100 ) < 15 )
				Log( st, "You hear in the distance the chirping of a song bird." );

			// The temple candles burn down too (CANDLE-TABLE: 20 / 10 / 5).
			if ( st.Flag( "candlestouched" ) && Burning( st, "candles" ) )
			{
				st.CandleTicks--;

				if ( st.CandleTicks <= 0 )
				{
					int stage = st.CandleStage < CandleStageText.Length ? st.CandleStage : CandleStageText.Length - 1;

					Log( st, CandleStageText[stage] );
					st.CandleStage++;

					if ( st.CandleStage >= CandleStageTicks.Length )
					{
						st.Set( "flameoff:candles", true );
					}
					else
					{
						st.CandleTicks = CandleStageTicks[st.CandleStage];
					}
				}
			}
		}

		private static readonly int[] CandleStageTicks = new int[] { 20, 10, 5, 0 };
		private static readonly string[] CandleStageText = new string[]
		{
			"The candles grow shorter.",
			"The candles are becoming quite short.",
			"The candles won't last long now.",
			"You'd better have more light than from the pair of candles."
		};

		private static void Climb( State st, string what )
		{
			string text = ( what == null ? "" : what.ToLower() );
			ZRoom here;

			// Zork's "tree"/"trees" ("forest") synonyms collide, so match text.
			if ( _rooms.TryGetValue( st.Room, out here ) && here.Exits.ContainsKey( "up" ) )
			{
				if ( text.Length == 0 || text.IndexOf( "tree" ) >= 0 || text.IndexOf( "ladder" ) >= 0 || text.IndexOf( "rope" ) >= 0 || text.IndexOf( "stair" ) >= 0 || text.IndexOf( "chimney" ) >= 0 || text.IndexOf( "cliff" ) >= 0 )
				{
					Go( st, "up" );
					return;
				}
			}

			Log( st, "You can't climb that." );
		}

		private static void Wave( State st, string what )
		{
			string id = Find( st, what );

			if ( id == "sceptre" && st.Room == "aragainfalls" )
			{
				st.Set( "rainbowflag", true );
				Log( st, "You wave the sceptre over the gorge. The rainbow hardens into a shimmering bridge!" );
				return;
			}

			Log( st, "Nothing happens." );
		}

		private static void Ring( State st, string what )
		{
			string id = Find( st, what );

			if ( id == "bell" )
			{
				if ( st.Room == "entrancetohades" && Carrying( st, "book" ) && Carrying( st, "candles" ) )
				{
					st.Set( "lldflag", true );
					Log( st, "You ring the bell and read from the book by candlelight. The ghosts howl and flee into the mist." );
				}
				else
				{
					Log( st, "The bell rings." );
				}

				return;
			}

			Log( st, "Nothing happens." );
		}

		private static void Tie( State st, string what )
		{
			string id = Find( st, what );

			if ( id == "rope" && st.Room == "domeroom" )
			{
				st.Set( "domeflag", true );
				Log( st, "You tie the rope to the railing. It dangles down into the darkness below." );
				return;
			}

			Log( st, "You can't tie that." );
		}

		private static void Inflate( State st, bool deflate )
		{
			if ( deflate )
			{
				if ( !st.Flag( "boatinflated" ) )
				{
					Log( st, "There is nothing to deflate." );
					return;
				}

				st.Set( "boatinflated", false );
				Log( st, "The boat deflates with a long sigh." );
				return;
			}

			if ( !Carrying( st, "inflatableboat" ) || !Carrying( st, "pump" ) )
			{
				Log( st, "You need the pile of plastic and the air pump." );
				return;
			}

			if ( st.Flag( "boatinflated" ) )
			{
				Log( st, "The boat is already inflated." );
				return;
			}

			st.Set( "boatinflated", true );
			st.Where["inflatableboat"] = "@dead";
			st.Where["inflatedboat"] = "@inv";
			Log( st, "You inflate the boat with the pump. It is now a seaworthy craft." );
		}

		private static void Dig( State st )
		{
			if ( !Carrying( st, "shovel" ) )
			{
				Log( st, "You have nothing to dig with." );
				return;
			}

			if ( st.Room == "sandycave" || st.Room == "sandybeach" )
			{
				if ( Carrying( st, "scarab" ) || st.Loc( "scarab" ) == "sandycave" )
				{
					Log( st, "You dig in the sand, but find nothing more." );
					return;
				}

				st.Where["scarab"] = "@inv";
				Log( st, "You dig in the sand and uncover a beautiful jeweled scarab!" );
				return;
			}

			Log( st, "The ground is too hard here." );
		}

		private static void Say( State st, string word )
		{
			string w = ( word == null ? "" : word.Trim().ToLower() );

			if ( w.Length == 0 )
			{
				Log( st, "Say what?" );
				return;
			}

			if ( w == "xyzzy" )
			{
				st.Room = "westofhouse";
				st.Log.Clear();
				Log( st, "A hollow voice says \"Fool.\"" );
				Look( st );
				return;
			}

			if ( ( w == "odysseus" || w == "ulysses" ) && st.Room == "cyclopsroom" )
			{
				st.Set( "cyclopsgone", true );
				st.Set( "magicflag", true );
				Log( st, "The cyclops yells \"ODYSSEUS!\" and flees in terror." );
				return;
			}

			if ( w == "echo" && st.Room == "loudroom" )
			{
				st.Set( "loudquiet", true );
				Log( st, "The roar abruptly stops. Silence reigns." );
				return;
			}

			Log( st, "Nothing happens." );
		}

		private static void Launch( State st )
		{
			if ( !st.Flag( "boatinflated" ) || !Carrying( st, "inflatedboat" ) )
			{
				Log( st, "You have no boat to launch." );
				return;
			}

			if ( st.Room == "dambase" || st.Room == "damroom" || st.Room == "reservoir" || st.Room == "streamview" || st.Room == "instream" )
			{
				st.Room = "river1";
				st.Moves++;
				st.Log.Clear();
				Log( st, "You launch the boat and are swept out into the river." );
				Look( st );
				return;
			}

			Log( st, "There is no water here to launch a boat on." );
		}

		// ------------------------------------------------------------------
		// Ported action routines. The thief wanders the dungeon, robs you and
		// stashes his loot; the bat throws you around; coal gas explodes; the
		// dumbwaiter moves; the mirror rooms swap their contents; and the egg
		// is a trap if you smash it instead of opening it.
		// ------------------------------------------------------------------

		private static readonly string[] ThiefHaunts = new string[]
		{
			"roundroom", "ewpassage", "nspassage", "deepcanyon", "loudroom", "dampcave",
			"gallery", "studio", "cellar", "trollroom", "coldpassage", "narrowpassage",
			"windingpassage", "twistingpassage", "atlantisroom", "smallcave", "engravingscave",
			"egyptroom", "domeroom", "damlobby", "maintenanceroom", "shaftroom", "squeekyroom",
			"smellyroom", "slideroom", "chasmroom", "mine1", "mine2", "mine3", "mine4"
		};

		private static void ThiefTick( State st )
		{
			if ( st.Flag( "thiefdead" ) || st.Won )
				return;

			string home = st.Loc( "thief" );

			if ( home == null || home == "@dead" )
				return;

			if ( st.ThiefEngrossed > 0 )
			{
				st.ThiefEngrossed--;
				return;
			}

			// Knocked out: he neither robs nor stumbles about until he revives.
			if ( st.Flag( "thiefunconscious" ) )
			{
				st.ThiefUnconsciousTurns--;

				if ( st.ThiefUnconsciousTurns <= 0 )
				{
					st.Set( "thiefunconscious", false );

					if ( st.Loc( "stiletto" ) == st.Room )
						st.Where["stiletto"] = "thief";

					if ( st.Loc( "thief" ) == st.Room )
					{
						Log( st, "The robber revives, briefly feigning continued unconsciousness, and, when he sees his moment, scrambles away from you." );
						st.Where["thief"] = ThiefHaunts[Utility.Random( ThiefHaunts.Length )];
					}
				}

				return;
			}

			// He is here with you: he may rob you -- or, if provoked, come at you.
			if ( home == st.Room )
			{
				ThiefAppear( st );

				if ( st.Flag( "thiefangry" ) )
				{
					if ( st.ThiefHP <= 1 )
					{
						Log( st, "Your opponent, determining discretion to be the better part of valor, decides to terminate this little contretemps. With a rueful nod of his head, he steps backward into the gloom and disappears." );
						st.Set( "thiefangry", false );

						List<string> away = new List<string>();

						foreach ( string room in ThiefHaunts )
						{
							if ( room != st.Room )
								away.Add( room );
						}

						if ( away.Count > 0 )
							st.Where["thief"] = away[Utility.Random( away.Count )];

						st.Set( "thiefhere", false );
					}
					else
					{
						Log( st, "The thief stabs at you with his stiletto." );
						Hurt( st, 1 );
					}

					return;
				}

				if ( Utility.Random( 100 ) < 30 )
					RobPlayer( st );

				return;
			}

			st.Set( "thiefhere", false );

			if ( Utility.Random( 100 ) >= 25 )
				return;

			string next = ThiefHaunts[Utility.Random( ThiefHaunts.Length )];

			st.Where["thief"] = next;

			if ( next == st.Room )
				ThiefAppear( st );
		}

		private static void ThiefAppear( State st )
		{
			if ( st.Flag( "thiefhere" ) )
				return;

			st.Set( "thiefhere", true );
			Log( st, "Someone carrying a large bag is casually leaning against one of the walls here. He does not speak, but it is clear from his aspect that the bag will be taken only over his dead body." );
			SwordGlowCheck( st );
		}

		private static bool RobPlayer( State st )
		{
			List<string> loot = new List<string>();

			foreach ( string id in Treasures() )
			{
				if ( st.Loc( id ) == "@inv" )
					loot.Add( id );
			}

			if ( loot.Count == 0 )
				return false;

			string pick = loot[Utility.Random( loot.Count )];

			st.Where[pick] = "thief";
			Log( st, "You suddenly notice that the " + _things[pick].Name + " vanished." );
			return true;
		}

		private static void ThiefDeath( State st )
		{
			st.Set( "thiefdead", true );
			st.Set( "thiefhere", false );
			st.Where["thief"] = "@dead";

			Log( st, "The thief staggers and falls, dead." );

			bool dropped = false;

			foreach ( string id in new List<string>( _things.Keys ) )
			{
				if ( id == "thief" )
					continue;

				if ( st.Loc( id ) == "thief" )
				{
					st.Where[id] = st.Room;
					dropped = true;
				}
			}

			if ( dropped )
				Log( st, "As the thief dies, the power of his magic decreases, and his treasures reappear. His booty remains." );
			else
				Log( st, "His booty remains." );
		}

		private static void Give( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Give what to whom?" );
				return;
			}

			string lower = what.ToLower();
			int pos = lower.IndexOf( " to " );

			if ( pos < 0 )
			{
				Log( st, "Give what to whom?" );
				return;
			}

			string itemText = what.Substring( 0, pos );
			string whoText = what.Substring( pos + 4 );

			string id = FindCarried( st, itemText );

			if ( id == null )
			{
				Log( st, "You aren't carrying that." );
				return;
			}

			if ( !Matches( "thief", whoText ) || st.Loc( "thief" ) != st.Room || st.Flag( "thiefdead" ) )
			{
				if ( Matches( "troll", whoText ) || Matches( "cyclops", whoText ) || Matches( "bat", whoText ) )
					Log( st, "It refuses it politely." );
				else
					Log( st, "You can't give it to that." );

				return;
			}

			st.Where[id] = "thief";

			if ( IsTreasure( id ) )
			{
				st.ThiefEngrossed = 6;
				Log( st, "The thief is taken aback by your unexpected generosity, but accepts the " + _things[id].Name + " and stops to admire its beauty." );
			}
			else
			{
				Log( st, "The thief places the " + _things[id].Name + " in his bag and thanks you politely." );
			}
		}

		private static bool GarlicHere( State st )
		{
			return Carrying( st, "garlic" ) || st.Loc( "garlic" ) == st.Room;
		}

		private static void FlyMe( State st )
		{
			string[] drops = new string[] { "mine1", "mine2", "mine3", "mine4", "laddertop", "ladderbottom", "squeekyroom", "mineentrance" };

			st.Room = drops[Utility.Random( drops.Length )];
			st.Moves++;
			st.Log.Clear();
			Log( st, "The bat grabs you by the scruff of your neck and lifts you away...." );
			Look( st );
			AfterEnter( st );
		}

		private static bool FlameOn( State st, string id )
		{
			return Burning( st, id );
		}

		private static bool BoomCheck( State st )
		{
			if ( st.Room != "gasroom" )
				return false;

			bool flame = ( Carrying( st, "torch" ) && FlameOn( st, "torch" ) )
				|| ( Carrying( st, "candles" ) && FlameOn( st, "candles" ) )
				|| ( Carrying( st, "match" ) && FlameOn( st, "match" ) );

			if ( !flame )
				return false;

			st.Log.Clear();
			Log( st, "Oh dear. It appears that the smell coming from this room was coal gas. I would have thought twice about carrying flaming objects in here." );
			Log( st, "      ** BOOOOOOOOOOOM **" );
			st.HP = MaxHealth;
			st.Room = "westofhouse";
			Log( st, "You have been blown to smithereens. You awaken, dazed, west of the white house." );
			Award( st, -10 );
			Look( st );
			return true;
		}

		private static void RaiseLower( State st, bool raise )
		{
			if ( st.Room != "shaftroom" && st.Room != "lowershaft" )
			{
				Log( st, "You can't do that here." );
				return;
			}

			bool top = st.Flag( "basketup" );

			if ( raise && top )
			{
				Log( st, "The basket is already at the top of the shaft." );
				return;
			}

			if ( !raise && !top )
			{
				Log( st, "The basket is already at the bottom of the shaft." );
				return;
			}

			st.Set( "basketup", raise );

			st.Where["raisedbasket"] = raise ? "shaftroom" : "lowershaft";
			st.Where["loweredbasket"] = raise ? "lowershaft" : "shaftroom";

			Log( st, raise ? "The basket is raised to the top of the shaft." : "The basket is lowered to the bottom of the shaft." );
		}

		private static void Rub( State st, string what )
		{
			string text = ( what == null ? "" : what.ToLower() );

			if ( text.IndexOf( "mirror" ) < 0 )
			{
				Log( st, "Nothing happens." );
				return;
			}

			if ( st.Room != "mirrorroom1" && st.Room != "mirrorroom2" )
			{
				Log( st, "Nothing happens." );
				return;
			}

			if ( st.Flag( "mirrormunged" ) )
			{
				Log( st, "The mirror is broken into many pieces." );
				return;
			}

			string here = st.Room;
			string other = ( here == "mirrorroom1" ) ? "mirrorroom2" : "mirrorroom1";

			// Everything standing in the two rooms changes places.
			foreach ( string id in new List<string>( _things.Keys ) )
			{
				string loc = st.Loc( id );

				if ( loc == here )
					st.Where[id] = other;
				else if ( loc == other )
					st.Where[id] = here;
			}

			st.Room = other;
			st.Moves++;
			st.Log.Clear();
			Log( st, "There is a rumble from deep within the earth and the room shakes." );
			Look( st );
			AfterEnter( st );
		}

		// V-THROW, plus the two special cases that matter: the lamp smashes and
		// a thrown knife can frighten the thief off his bag.
		private static void Throw( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Throw what?" );
				return;
			}

			string lower = what.ToLower();
			int pos = lower.IndexOf( " at " );

			string itemText = ( pos < 0 ) ? what : what.Substring( 0, pos );
			string targetText = ( pos < 0 ) ? null : what.Substring( pos + 4 ).Trim();

			string id = FindCarried( st, itemText );

			if ( id == null )
			{
				Log( st, "You aren't carrying that." );
				return;
			}

			if ( targetText != null && ( targetText == "me" || targetText == "myself" ) )
			{
				st.Where[id] = st.Room;
				st.Log.Clear();
				Log( st, "A terrific throw! The " + _things[id].Name + " hits you squarely in the head." );
				Log( st, "Normally, this wouldn't do much damage, but by incredible mischance you fall over backwards trying to duck, and break your neck." );
				st.HP = MaxHealth;
				st.Room = "westofhouse";
				Award( st, -10 );
				Look( st );
				return;
			}

			if ( id == "lamp" )
			{
				st.Where["lamp"] = "@dead";
				st.Where["brokenlamp"] = st.Room;
				st.Set( "lampon", false );
				st.Set( "lampburned", true );
				Log( st, "The lamp has smashed into the floor, and the light has gone out." );
				return;
			}

			st.Where[id] = st.Room;

			// The knife at the thief: a tenth of the time he bolts and drops his loot.
			if ( id == "knife" && targetText != null && Matches( "thief", targetText ) && !st.Flag( "thiefdead" ) && st.Loc( "thief" ) == st.Room )
			{
				st.Where[id] = st.Room;

				if ( Utility.Random( 100 ) < 10 )
				{
					ThiefFlees( st );
				}
				else
				{
					st.Set( "thiefangry", true );
					Log( st, "You missed. The thief makes no attempt to take the knife, though it would be a fine addition to the collection in his bag. He does seem angered by your attempt." );
				}

				return;
			}

			if ( targetText != null )
			{
				string who = null;

				if ( Matches( "troll", targetText ) && st.Loc( "troll" ) == st.Room )
					who = "troll";
				else if ( Matches( "thief", targetText ) && st.Loc( "thief" ) == st.Room )
					who = "thief";
				else if ( Matches( "cyclops", targetText ) && st.Loc( "cyclops" ) == st.Room )
					who = "cyclops";
				else if ( Matches( "bat", targetText ) && st.Loc( "bat" ) == st.Room )
					who = "bat";

				if ( who != null )
				{
					Log( st, "The " + _things[who].Name + " ducks as the " + _things[id].Name + " flies by and crashes to the ground." );
					return;
				}
			}

			Log( st, "Thrown." );
		}

		private static void ThiefFlees( State st )
		{
			Log( st, "You evidently frightened the robber, though you didn't hit him. He flees, but the contents of his bag fall on the floor." );

			foreach ( string thing in new List<string>( _things.Keys ) )
			{
				if ( thing != "thief" && st.Loc( thing ) == "thief" )
					st.Where[thing] = st.Room;
			}

			// He must actually go somewhere else.
			List<string> options = new List<string>();

			foreach ( string room in ThiefHaunts )
			{
				if ( room != st.Room )
					options.Add( room );
			}

			if ( options.Count > 0 )
				st.Where["thief"] = options[Utility.Random( options.Count )];

			st.Set( "thiefhere", false );
		}

		private static void Wind( State st, string what )
		{
			string id = FindCarried( st, what );

			if ( id == null )
			{
				Log( st, "You aren't carrying that." );
				return;
			}

			if ( id != "canary" )
			{
				Log( st, "There is an unpleasant grinding noise from inside the " + _things[id].Name + "." );
				return;
			}

			bool forest = st.Room == "forest1" || st.Room == "forest2" || st.Room == "forest3" || st.Room == "path" || st.Room == "upatree";

			if ( forest && !st.Flag( "singsong" ) )
			{
				st.Set( "singsong", true );
				st.Where["bauble"] = ( st.Room == "upatree" || st.Room == "path" ) ? "path" : st.Room;
				Log( st, "The canary chirps, slightly off-key, an aria from a forgotten opera. From out of the greenery flies a lovely songbird." );
				Log( st, "As it sings, a beautiful brass bauble drops from its mouth, bounces off the top of your head, and lands glimmering in the grass." );
				return;
			}

			Log( st, "The canary chirps blithely, if somewhat tinnily, for a short time." );
		}

		private static void Pray( State st )
		{
			// Zork I: praying in the South Temple whisks you back to the forest.
			if ( st.Room == "southtemple" )
			{
				st.Room = "forest1";
				st.Moves++;
				st.Log.Clear();
				Log( st, "If you pray enough, your prayers may be answered." );
				Look( st );
				AfterEnter( st );
				return;
			}

			Log( st, "If you pray enough, your prayers may be answered." );
		}

		private static void Hello( State st, string what )
		{
			if ( string.IsNullOrEmpty( what ) )
			{
				Log( st, "Hello." );
				return;
			}

			string id = Find( st, what );

			if ( id == "troll" || id == "thief" || id == "cyclops" || id == "bat" )
			{
				Log( st, "The " + _things[id].Name + " bows his head to you in greeting." );
				return;
			}

			Log( st, "It's a well known fact that only schizophrenics say \"Hello\" to a " + ( id == null ? what : _things[id].Name ) + "." );
		}

		private static void Swim( State st )
		{
			if ( RoomHasWater( st ) )
				Log( st, "Swimming isn't usually allowed in the " + _rooms[st.Room].Title.ToLower() + "." );
			else
				Log( st, "Swimming isn't usually allowed in the dungeon." );
		}

		private static readonly string[] Wheeee = new string[]
		{
			"Very good. Now you can go to the second grade.",
			"Are you enjoying yourself?",
			"Wheeeeeeeeee!!!!!"
		};

		private static readonly string[] JumpLoss = new string[]
		{
			"You should have looked before you leaped.",
			"In the movies, your life would be passing before your eyes.",
			"Geronimo..."
		};

		// V-LEAP / V-JUMP: the tree is a safe jump, most places are not.
		private static void Leap( State st, string what )
		{
			if ( !string.IsNullOrEmpty( what ) )
			{
				string text = what.ToLower().Replace( "over ", "" ).Trim();
				string id = Find( st, text );

				if ( id == null )
				{
					Log( st, "That would be a good trick." );
					return;
				}

				if ( id == "troll" || id == "thief" || id == "cyclops" )
				{
					Log( st, "The " + _things[id].Name + " is too big to jump over." );
					return;
				}

				Log( st, Wheeee[Utility.Random( Wheeee.Length )] );
				return;
			}

			if ( st.Room == "upatree" )
			{
				Go( st, "down" );
				Log( st, "In a feat of unaccustomed daring, you manage to land on your feet without killing yourself." );
				return;
			}

			ZRoom r;

			if ( !_rooms.TryGetValue( st.Room, out r ) || !r.Exits.ContainsKey( "down" ) )
			{
				st.Log.Clear();
				Log( st, "This was not a very safe place to try jumping." );
				Log( st, JumpLoss[Utility.Random( JumpLoss.Length )] );
				st.HP = MaxHealth;
				st.Room = "westofhouse";
				Award( st, -10 );
				Log( st, "You awaken, dazed, west of the white house." );
				Look( st );
				return;
			}

			Log( st, Wheeee[Utility.Random( Wheeee.Length )] );
		}

		// GRATE-FUNCTION: the grating is locked until the skeleton keys are used
		// on it from below; from the clearing the lock is out of reach.
		private static void Unlock( State st, string what )
		{
			string lower = ( what == null ? "" : what.ToLower() );
			int pos = lower.IndexOf( " with " );

			string itemText = ( pos < 0 ) ? lower : lower.Substring( 0, pos );
			string keyText = ( pos < 0 ) ? "" : lower.Substring( pos + 6 ).Trim();

			if ( itemText.IndexOf( "grate" ) < 0 && itemText.IndexOf( "grating" ) < 0 )
			{
				Log( st, "That doesn't seem to be locked." );
				return;
			}

			if ( st.Room == "gratingclearing" )
			{
				Log( st, "You can't reach the lock from here." );
				return;
			}

			if ( st.Room != "gratingroom" )
			{
				Log( st, "You don't see that here." );
				return;
			}

			if ( !Carrying( st, "keys" ) || keyText.IndexOf( "key" ) < 0 )
			{
				Log( st, "Can you unlock a grating with a " + ( keyText.Length > 0 ? keyText : "bare hand" ) + "?" );
				return;
			}

			st.Set( "grunlock", true );
			Log( st, "The grate is unlocked." );
		}

		private static void LockGrate( State st )
		{
			if ( st.Room == "gratingclearing" )
			{
				Log( st, "You can't lock it from this side." );
				return;
			}

			if ( st.Room != "gratingroom" )
			{
				Log( st, "There is nothing to lock here." );
				return;
			}

			st.Set( "grunlock", false );
			Log( st, "The grate is locked." );
		}

		private static void Diagnose( State st )
		{
			int lost = MaxHealth - st.HP;

			if ( lost <= 0 )
			{
				Log( st, "You are in perfect health." );
			}
			else
			{
				string wound = lost == 1 ? "a light wound," : lost == 2 ? "a serious wound," : lost == 3 ? "several wounds," : "serious wounds,";

				Log( st, "You have " + wound + " which will be cured with rest." );
			}

			if ( st.HP <= 0 )
				Log( st, "You can expect death soon." );
			else if ( st.HP <= 2 )
				Log( st, "You can be killed by one more light wound." );
			else if ( st.HP <= 5 )
				Log( st, "You can survive one serious wound." );
			else
				Log( st, "You can survive several wounds." );
		}

		private static int ItemWeight( State st, string id )
		{
			int weight = _things[id].Size;

			foreach ( string other in _things.Keys )
			{
				if ( other != id && st.Loc( other ) == id )
					weight += _things[other].Size;
			}

			return weight;
		}

		private static void Help( State st )
		{
			Log( st, "Movement: n/s/e/w/ne/nw/se/sw/up/down/in/out, look, inventory, wait, score, restart." );
			Log( st, "Items: examine <thing>, take <thing>, drop <thing>, read <thing>," );
			Log( st, "open/close <thing>, move <thing>, turn on/off lamp, enter window, climb <thing>." );
			Log( st, "Magic: say <word>, wave <thing>, ring <thing>, tie <thing>, inflate <thing>, dig." );
			Log( st, "Also: give <thing> to <creature>, raise/lower basket, rub mirror." );
			Log( st, "Survival: eat <food>, drink water, light match, count matches, blow out match." );
			Log( st, "Also: throw <thing> at <creature>." );
			Log( st, "Display: verbose, brief, superbrief, diagnose." );
			Log( st, "Combat: attack <creature> (needs a weapon). Treasures: put <treasure> in case." );
		}

		private static void Restart( State st )
		{
			State fresh = NewState();
			Boot( fresh );

			st.Room = fresh.Room;
			st.Log = fresh.Log;
			st.Flags = fresh.Flags;
			st.Where = fresh.Where;
			st.Moves = 0;
			st.Score = 0;
			st.Base = 0;
			st.ThiefEngrossed = 0;
			st.ThiefUnconsciousTurns = 0;
			st.Mode = 2;
			st.LampStage = 0;
			st.LampTicks = 100;
			st.Matches = 6;
			st.MatchTurns = 0;
			st.SwordGlow = 0;
			st.CandleStage = 0;
			st.CandleTicks = 20;
			st.Won = false;
			st.Trophies.Clear();
			st.TrollHits = 0;
			st.HP = MaxHealth;
			st.ThiefHP = 4;
		}

		private static void Grue( State st )
		{
			st.Room = "livingroom";
			st.Log.Clear();
			Log( st, "Oh, no! A grue has eaten you!" );
			Award( st, -10 );
			Look( st );
		}

		private static bool IsDark( State st )
		{
			ZRoom r = _rooms[st.Room];

			if ( !r.Dark )
				return false;

			return !RoomLit( st );
		}

		// Any burning light source you carry -- or that is standing in the room,
		// even inside an open container like the dumbwaiter basket -- lights it.
		private static bool RoomLit( State st )
		{
			foreach ( string id in _things.Keys )
			{
				if ( !( _things[id].Light || id == "match" ) || !Burning( st, id ) )
					continue;

				if ( st.Loc( id ) == "@inv" || IsPresent( st, id ) )
					return true;
			}

			return false;
		}

		private static bool Burning( State st, string id )
		{
			if ( id == "lamp" )
				return st.Flag( "lampon" );

			// The ivory torch and the temple candles arrive already alight.
			if ( id == "torch" || id == "candles" )
				return !st.Flag( "flameoff:" + id );

			return st.Flag( "flameon:" + id );
		}

		private static List<string> PresentThings( State st )
		{
			List<string> list = new List<string>();

			foreach ( string id in _things.Keys )
			{
				if ( !IsPresent( st, id ) )
					continue;

				if ( id == "trapdoor" && !st.Flag( "rugmoved" ) )
					continue;

				list.Add( id );
			}

			return list;
		}

		private static bool IsPresent( State st, string id )
		{
			ZThing t = _things[id];

			if ( t.Global )
				return true;

			string loc = st.Loc( id );

			if ( loc == null || loc == "@inv" || loc == "@dead" )
				return false;

			if ( _rooms.ContainsKey( loc ) )
				return loc == st.Room;

			if ( _things.ContainsKey( loc ) )
			{
				if ( !IsPresent( st, loc ) )
					return false;

				ZThing c = _things[loc];

				return c.Surface || st.Flag( "open:" + loc );
			}

			return false;
		}

		private static string Find( State st, string text )
		{
			if ( string.IsNullOrEmpty( text ) )
				return null;

			foreach ( string id in _things.Keys )
			{
				bool accessible = IsPresent( st, id ) || st.Loc( id ) == "@inv";

				if ( !accessible )
					continue;

				if ( id == "trapdoor" && !st.Flag( "rugmoved" ) )
					continue;

				if ( Matches( id, text ) )
					return id;
			}

			return null;
		}

		private static string FindCarried( State st, string text )
		{
			if ( string.IsNullOrEmpty( text ) )
				return null;

			foreach ( string id in _things.Keys )
			{
				if ( st.Loc( id ) != "@inv" )
					continue;

				if ( Matches( id, text ) )
					return id;
			}

			return null;
		}

		private static bool Matches( string id, string text )
		{
			ZThing t;

			if ( !_things.TryGetValue( id, out t ) )
				return false;

			string hay = ( t.Name + " " + t.Aliases ).ToLower();
			string[] tokens = hay.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
			string[] q = text.ToLower().Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			bool any = false;

			foreach ( string qw in q )
			{
				if ( qw.Length == 0 )
					continue;

				bool hit = false;

				foreach ( string tok in tokens )
				{
					if ( tok == qw || tok.StartsWith( qw ) )
					{
						hit = true;
						break;
					}
				}

				if ( !hit )
					return false;

				any = true;
			}

			return any;
		}

		private static string Escape( string s )
		{
			if ( string.IsNullOrEmpty( s ) )
				return "";

			// This gump client doesn't decode HTML entities (it prints "&gt;" literally),
			// so only strip '<' (which could start a tag); leave '>' and '&' as plain text.
			return s.Replace( "<", "" );
		}

		public static string RoomTitle( string id )
		{
			ZRoom r;

			if ( _rooms.TryGetValue( id, out r ) )
				return r.Title;

			return id;
		}

		private static readonly Dictionary<string, ZRoom> _rooms = BuildRooms();
		private static readonly Dictionary<string, ZThing> _things = BuildThings();
		private static readonly Dictionary<string, ZGate> _gates = BuildGates();

		private class ZRoom
		{
			public string Title;
			public string Desc;
			public bool Dark;
			public int Value;     // points for first visiting the room
			public Dictionary<string, string> Exits = new Dictionary<string, string>();
			public Dictionary<string, string> Walls = new Dictionary<string, string>();

			public ZRoom( string title, string desc, bool dark, int value )
			{
				Title = title;
				Desc = desc;
				Dark = dark;
				Value = value;
			}

			public ZRoom Exit( string dir, string to )
			{
				Exits[dir] = to;
				return this;
			}

			public ZRoom Wall( string dir, string msg )
			{
				Walls[dir] = msg;
				return this;
			}
		}

		private class ZGate
		{
			public string Key;
			public string Msg;

			public ZGate( string key, string msg )
			{
				Key = key;
				Msg = msg;
			}
		}

		private class ZThing
		{
			public string Name;
			public string Aliases;
			public string Desc;
			public bool Portable;
			public bool Container;
			public bool Surface;
			public bool Global;
			public bool Listed;
			public string ReadText;
			public int Value;     // points for finding it (Zork's VALUE)
			public int TValue;    // points for banking it (Zork's TVALUE)
			public int Size;      // weight, used by the mine's crawlways
			public bool Light;    // LIGHTBIT: lights the room while burning
			public bool Food;     // FOODBIT: can be eaten
			public int Capacity;  // how much can be put inside

			public bool Openable
			{
				get { return Container && !Surface; }
			}
		}

	}
}
