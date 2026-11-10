using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	// A green "data pad"-looking item that boots a small text adventure (a Zork-style
	// Great Underground Empire) inside a Commodore-64 flavoured terminal gump.
	public class ZorkPad : Item
	{
		// The shard's canonical "green" technology hue (see DemonPrison / GreenScales).
		public const int GreenHue = 0x85D;

		[Constructable]
		public ZorkPad() : base( 0x27FB )
		{
			Name = "Zork!";
			Weight = 1.0;
			Technology = true;
			Light = LightType.Circle150;

			// DataPad appearance, but always the green variant.
			ItemID = Utility.RandomList( 0x27FB, 0x27FC );
			Hue = GreenHue;
		}

		public ZorkPad( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 4 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}

			from.CloseGump( typeof( ZorkPadGump ) );
			from.SendGump( new ZorkPadGump( from ) );
			from.SendSound( 0x54D );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class ZorkPadGump : Gump
	{
		private const int KeyW = 56;
		private const int KeyH = 34;
		private const int GapX = 10;
		private const int GapY = 8;
		private const int KeyX0 = 35;
		private const int KeyY0 = 520;
		private const int RowW10 = 10 * KeyW + 9 * GapX;

		private static readonly string[][] KeyRows = new string[][]
		{
			new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
			new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" },
			new string[] { "A", "S", "D", "F", "G", "H", "J", "K", "L" },
			new string[] { "Z", "X", "C", "V", "B", "N", "M" }
		};

		// The in-progress input line, kept server-side so clicking keys can build it up
		// (the client can't be told to change its text entry, so we re-open with initialText).
		private static readonly Dictionary<Serial, string> m_Drafts = new Dictionary<Serial, string>();

		private static string DraftFor( Mobile m )
		{
			if ( m == null )
				return "";

			string d;
			return m_Drafts.TryGetValue( m.Serial, out d ) ? d : "";
		}

		private static void SetDraft( Mobile m, string d )
		{
			if ( m != null )
				m_Drafts[m.Serial] = d;
		}

		private static string KeyToken( int id )
		{
			if ( id == 50 ) return "SPC";
			if ( id == 51 ) return "BSP";
			if ( id == 52 ) return "CLR";

			if ( id >= 100 )
			{
				int idx = id - 100;
				int row = idx / 10;
				int col = idx % 10;

				if ( row >= 0 && row < KeyRows.Length && col < KeyRows[row].Length )
					return KeyRows[row][col];
			}

			return null;
		}

		public ZorkPadGump( Mobile from ) : this( from, DraftFor( from ) )
		{
		}

		public ZorkPadGump( Mobile from, string draft ) : base( 0, 0 )
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			ZorkGame.State st = ZorkGame.Get( from );

			AddPage( 0 );

			// Solid dark-gray board around the whole gump (tile 9750 = flat gray fill);
			// the black panel (2624) sits only behind the screen text area.
			AddImageTiled( 0, 0, 740, 780, 9750 );
			AddImageTiled( 16, 16, 708, 500, 2624 );

			AddHtml( 62, 62, 616, 40, ZorkGame.C64( "<B>ZORK I</B>   build 37" ), false, false, false );
			AddHtml( 520, 62, 158, 40, ZorkGame.C64( "<CENTER>" + ZorkGame.ScoreText( st ) + "</CENTER>" ), false, false, false );
			AddHtml( 62, 112, 616, 36, ZorkGame.C64( st.Status() ), false, false, false );
			AddHtml( 62, 156, 616, 300, ZorkGame.C64( st.Render() ), false, false, false );

			AddHtml( 82, 476, 20, 24, ZorkGame.C64( ">" ), false, false, false );
			AddTextEntry( 98, 476, 560, 24, 0x44, 1, draft );

			// Simulated QWERTY keyboard beneath the screen.
			for ( int r = 0; r < KeyRows.Length; r++ )
			{
				string[] row = KeyRows[r];
				int rowW = row.Length * KeyW + ( row.Length - 1 ) * GapX;
				int x0 = KeyX0 + ( RowW10 - rowW ) / 2;
				int y = KeyY0 + r * ( KeyH + GapY );

				for ( int c = 0; c < row.Length; c++ )
					AddKey( x0 + c * ( KeyW + GapX ), y, KeyW, 100 + r * 10 + c, row[c] );
			}

			int sy = KeyY0 + KeyRows.Length * ( KeyH + GapY );

			AddKey( 233, sy, 56, 50, "SPACE" );
			AddKey( 299, sy, 56, 51, "BKSP" );
			AddKey( 365, sy, 56, 52, "CLR" );
			AddKey( 431, sy, 56, 2, "ENTER" );

			SetDraft( from, draft );
		}

		private void AddKey( int x, int y, int w, int id, string label )
		{
			// Plain button art (no item overlay, so no "unused" placeholder graphic).
			AddButton( x, y, 0x918, 0x919, id, GumpButtonType.Reply, 0 );

			// Single characters use the bigger font; word keys stay normal so they fit one line.
			string open = ( label.Length <= 1 ) ? "<BIG>" : "";
			string close = ( label.Length <= 1 ) ? "</BIG>" : "";

			AddHtml( x, y + 6, w, KeyH, "<BODY><BASEFONT Color=#000000><CENTER>" + open + label + close + "</CENTER></BASEFONT></BODY>", false, false, false );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( from == null || from.Deleted )
				return;

			// Current draft: prefer what the client reports (real typing), else our stored draft.
			string cur = DraftFor( from );

			TextRelay entry = info.GetTextEntry( 1 );

			if ( entry != null && entry.Text != null && entry.Text.Length > 0 )
				cur = entry.Text;

			if ( info.ButtonID == 2 )
			{
				SetDraft( from, "" );
				Submit( from, cur.Trim() );
				return;
			}

			string token = KeyToken( info.ButtonID );

			if ( token == null )
				return;

			if ( token == "SPC" )
			{
				cur += " ";
			}
			else if ( token == "BSP" )
			{
				if ( cur.Length > 0 )
					cur = cur.Substring( 0, cur.Length - 1 );
			}
			else if ( token == "CLR" )
			{
				cur = "";
			}
			else
			{
				cur += token;
			}

			SetDraft( from, cur );

			from.CloseGump( typeof( ZorkPadGump ) );
			from.SendGump( new ZorkPadGump( from, cur ) );
		}

		private void Submit( Mobile from, string command )
		{
			ZorkGame.Process( ZorkGame.Get( from ), command );

			SetDraft( from, "" );

			from.CloseGump( typeof( ZorkPadGump ) );
			from.SendGump( new ZorkPadGump( from, "" ) );
			from.SendSound( 0x54D );
		}
	}
}
