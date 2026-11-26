using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public abstract class HenchmanCustomizeGumpBase : Gump
	{
		protected Mobile m_From;
		protected BaseCreature m_Henchman;
		protected HenchmanItem m_Deed;

		protected HenchmanCustomizeGumpBase( Mobile from, BaseCreature henchman ) : base( 50, 50 )
		{
			m_From = from;
			m_Henchman = henchman;
			m_Deed = ( henchman == null ? null : HenchmanFunctions.FindDeed( henchman ) );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
		}

		protected bool IsValid()
		{
			return m_From != null && !m_From.Deleted && m_Henchman != null && !m_Henchman.Deleted
				&& m_Henchman.ControlMaster == m_From
				&& ( m_Henchman is HenchmanFighter || m_Henchman is HenchmanArcher || m_Henchman is HenchmanWizard );
		}

		protected void CloseAll()
		{
			HenchmanCustomizeGump.CloseFor( m_From );
		}

		protected void SendMain()
		{
			CloseAll();

			if ( IsValid() )
				m_From.SendGump( new HenchmanCustomizeGump( m_From, m_Henchman ) );
		}

		protected void RenderPanel( int width, int height, string title )
		{
			AddBackground( 0, 0, width, height, 0x1453 );
			AddImageTiled( 10, 10, width - 20, 20, 0xA40 );
			AddImageTiled( 10, 40, width - 20, height - 70, 0xA40 );
			AddImageTiled( 10, height - 30, width - 20, 20, 0xA40 );
			AddAlphaRegion( 10, 10, width - 20, height - 20 );

			AddHtml( 10, 12, width - 20, 18, "<CENTER><BASEFONT COLOR=#FFD700><BIG>" + title + "</BIG></BASEFONT></CENTER>", false, false );
		}

		protected void AddBackButton( int x, int y )
		{
			AddButton( x, y, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
			AddHtml( x + 35, y + 2, 100, 18, "<BASEFONT COLOR=#FFFFFF>BACK</BASEFONT>", false, false );
		}

		// The longest text these two fields have to carry is a pre-existing vendor
		// title from TavernPatrons.GetTitle/GetEvilTitle: the worst combinations are
		// "the Executioner of Minotaurs" (28), "of the Malevolent Society" (23) and
		// "of the Vampire's Tomb" (21), so 16 cut real titles in half. 32 keeps every
		// shipped title intact with a little room for typed text, and is also the
		// most that still displays in full: the text entries are 360px wide and the
		// headers print a name and a title on at most two lines of 32 characters.
		protected const int NameTitleMaxLength = 32;

		protected static string Sanitize( string text )
		{
			if ( string.IsNullOrEmpty( text ) )
				return "";

			StringBuilder builder = new StringBuilder( text.Length );

			for ( int i = 0; i < text.Length; ++i )
			{
				char c = text[i];

				if ( c == '<' || c == '>' || c == '&' || char.IsControl( c ) )
					continue;

				builder.Append( c );
			}

			string result = builder.ToString().Trim();

			if ( result.Length > NameTitleMaxLength )
				result = result.Substring( 0, NameTitleMaxLength ).Trim();

			return result;
		}

		protected static string DisplayName( Mobile m )
		{
			if ( m == null )
				return "";

			string name = Sanitize( m.Name );
			string title = Sanitize( m.Title );

			if ( !string.IsNullOrEmpty( title ) )
				return name + " " + title;

			return name;
		}

		protected void SaveAppearance()
		{
			if ( m_Deed == null || m_Deed.Deleted )
				return;

			m_Deed.HenchHair = m_Henchman.HairItemID;
			m_Deed.HenchHairHue = m_Henchman.HairHue;
			m_Deed.HenchBeard = m_Henchman.FacialHairItemID;
			m_Deed.HenchBeardHue = m_Henchman.FacialHairHue;
			m_Deed.HenchHue = m_Henchman.Hue;
			m_Deed.HenchBody = m_Henchman.Female ? 401 : 400;
			m_Deed.HenchName = m_Henchman.Name ?? "";
			m_Deed.HenchTitle = m_Henchman.Title ?? "";
			m_Deed.InvalidateProperties();
		}

		// TazUO and ClassicUO choose the paperdoll avatar from Mobile.IsFemale:
		// PaperDollInteractable.UpdateUI matches bodies 0x0191/0x0193 (and the elf,
		// gargoyle and shroud bodies) explicitly, and everything else - including the
		// male human body 0x0190 - falls through to "mobile.IsFemale ? 0x000D : 0x000C".
		// The client only recalculates that flag when it *creates* a mobile
		// (ObjectHelpers.UpdateGameObject calls Mobile.CheckGraphicChange for a new
		// mobile and never for one it already knows), so a plain 0x78 body update
		// leaves IsFemale on the old gender: the world sprite switches - it is drawn
		// from Graphic - while the paperdoll keeps the old body and the old armour
		// variants. Dropping and re-sending the mobile makes the client build it again
		// from the current body, which is the same remove/re-add the core already does
		// for a hue change (Mobile.ProcessDelta sends sendRemove for MobileDelta.Hue).
		protected void ResendBodyToClients()
		{
			if ( m_Henchman == null || m_Henchman.Deleted )
				return;

			m_Henchman.SendRemovePacket();
			m_Henchman.SendIncomingPacket();
		}

		// The paperdoll window belongs to the client, so the server can only ask for it
		// with the paperdoll packet (0x88), exactly as Server.Misc.Paperdoll does when a
		// player double-clicks a mobile: the client opens the gump when it has none (at
		// the position it saved when the old one closed) and refreshes the title and the
		// "can lift" flag when it already has one. Used to show the henchman while it is
		// being customized, and to bring the paperdoll back after a skin hue change
		// (Mobile.ProcessDelta removes the mobile for MobileDelta.Hue) or a gender change
		// (ResendBodyToClients) - both make the client destroy the mobile, which closes a
		// non-player's PaperDollGump.
		public static void ShowPaperdoll( Mobile beholder, Mobile henchman )
		{
			if ( beholder == null || beholder.Deleted || beholder.NetState == null )
				return;

			if ( henchman == null || henchman.Deleted )
				return;

			beholder.Send( new DisplayPaperdoll( henchman, Titles.ComputeTitle( beholder, henchman ), henchman.AllowEquipFrom( beholder ) ) );

			if ( ObjectPropertyList.Enabled )
			{
				List<Item> items = henchman.Items;

				for ( int i = 0; i < items.Count; ++i )
					beholder.Send( items[i].OPLPacket );
			}
		}

		protected void SaveNameTitle( string name, string title )
		{
			m_Henchman.Name = name;
			m_Henchman.Title = title;
			m_Henchman.InvalidateProperties();

			if ( m_Deed != null && !m_Deed.Deleted )
			{
				m_Deed.HenchName = name;
				m_Deed.HenchTitle = title;
				m_Deed.InvalidateProperties();
			}
		}

		protected void SetHair( int itemID, int hue )
		{
			if ( hue <= 0 )
				hue = 1102;

			m_Henchman.HairItemID = 0;
			m_Henchman.ProcessDelta();
			m_Henchman.HairItemID = itemID;
			m_Henchman.HairHue = hue;
			m_Henchman.ProcessDelta();

			SaveAppearance();
		}

		protected void SetBeard( int itemID, int hue )
		{
			if ( hue <= 0 )
				hue = ( m_Henchman.HairHue > 0 ? m_Henchman.HairHue : 1102 );

			m_Henchman.FacialHairItemID = 0;
			m_Henchman.ProcessDelta();
			m_Henchman.FacialHairItemID = itemID;
			m_Henchman.FacialHairHue = hue;
			m_Henchman.ProcessDelta();

			SaveAppearance();
		}

		protected bool HasFemaleOnlyGear()
		{
			if ( m_Henchman == null || m_Henchman.Deleted )
				return false;

			foreach ( Item item in m_Henchman.Items )
			{
				if ( !HenchmanFunctions.IsEquipment( item ) )
					continue;

				BaseArmor armor = item as BaseArmor;

				if ( armor != null && !armor.AllowMaleWearer )
					return true;

				BaseClothing clothing = item as BaseClothing;

				if ( clothing != null && !clothing.AllowMaleWearer )
					return true;
			}

			return false;
		}

		protected static readonly int[] SkinHues = CreateHues( 1002, 57 );
		protected static readonly int[] HairHues = CreateHues( 1102, 48 );

		private static int[] CreateHues( int start, int count )
		{
			int[] hues = new int[count];

			for ( int i = 0; i < count; ++i )
				hues[i] = start + i;

			return hues;
		}

		// A text entry is coloured with a hues.mul palette index, not with the
		// <BASEFONT COLOR=#RRGGBB> the rest of the gump uses. 0x7FFF is the AddHtml
		// "white", but it is out of range for a text entry, so the client drew the
		// name and title fields black. 2049 is hues.mul "Fade 30"/"Fade 31", whose
		// every ramp stop is white (247-255), so the typed text reads white.
		protected const int WhiteTextHue = 2049;

		// Colour-grid metrics. The 0xFA5 arrow is a 22x30 gump image, so a column has
		// to leave room for the button *and* the five asterisks that demonstrate the
		// hue, and the rows are a little taller than before so the swatches breathe.
		protected const int SwatchColumnWidth = 92;
		protected const int SwatchRowHeight = 24;
		protected const int SwatchLabelOffset = 24;

		protected void AddColorSwatch( int x, int y, int buttonID, int hue )
		{
			AddButton( x, y, 0xFA5, 0xFA7, buttonID, GumpButtonType.Reply, 0 );
			AddLabel( x + SwatchLabelOffset, y + 2, hue - 1, "*****" );
		}

		protected class StyleEntry
		{
			public int ItemID;
			public string Name;

			public StyleEntry( int itemID, string name )
			{
				ItemID = itemID;
				Name = name;
			}
		}

		protected static readonly StyleEntry[] HairStyles = new StyleEntry[]
		{
			new StyleEntry( 0x203B, "Short" ),
			new StyleEntry( 0x203C, "Long" ),
			new StyleEntry( 0x203D, "Ponytail" ),
			new StyleEntry( 0x2044, "Mohawk" ),
			new StyleEntry( 0x2045, "Pageboy" ),
			new StyleEntry( 0x204A, "Topknot" ),
			new StyleEntry( 0x2047, "Curly" ),
			new StyleEntry( 0x2048, "Receding" ),
			new StyleEntry( 0x2049, "2-Tails" ),
			new StyleEntry( 0, "Bald" )
		};

		protected static readonly StyleEntry[] BeardStyles = new StyleEntry[]
		{
			new StyleEntry( 0x2041, "Mustache" ),
			new StyleEntry( 0x203F, "Short Beard" ),
			new StyleEntry( 0x204B, "Short Beard & Mustache" ),
			new StyleEntry( 0x203E, "Long Beard" ),
			new StyleEntry( 0x204C, "Long Beard & Mustache" ),
			new StyleEntry( 0x2040, "Goatee" ),
			new StyleEntry( 0x204D, "Vandyke" ),
			new StyleEntry( 0, "None" )
		};
	}

	public class HenchmanCustomizeGump : HenchmanCustomizeGumpBase
	{
		public HenchmanCustomizeGump( Mobile from, BaseCreature henchman ) : base( from, henchman )
		{
			if ( !IsValid() )
				return;

			from.SendSound( 0x4A );

			RenderPanel( 370, 330, "HENCHMAN CUSTOMIZATION" );

			AddHtml( 20, 42, 340, 40, "<CENTER><BASEFONT COLOR=#E4E377>" + DisplayName( m_Henchman ) + "</BASEFONT></CENTER>", false, false );

			int y = 85;

			AddButton( 20, y, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtml( 55, y + 2, 280, 18, "<BASEFONT COLOR=#FFFFFF>Name/Title</BASEFONT>", false, false );
			y += 30;

			AddButton( 20, y, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
			AddHtml( 55, y + 2, 280, 18, "<BASEFONT COLOR=#FFFFFF>Hair</BASEFONT>", false, false );
			y += 30;

			if ( !m_Henchman.Female )
			{
				AddButton( 20, y, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
				AddHtml( 55, y + 2, 280, 18, "<BASEFONT COLOR=#FFFFFF>Beard</BASEFONT>", false, false );
				y += 30;
			}

			AddButton( 20, y, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
			AddHtml( 55, y + 2, 280, 18, "<BASEFONT COLOR=#FFFFFF>Gender</BASEFONT>", false, false );
			y += 30;

			AddButton( 20, y, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0 );
			AddHtml( 55, y + 2, 280, 18, "<BASEFONT COLOR=#FFFFFF>Skin Tone</BASEFONT>", false, false );

			AddButton( 20, 270, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0 );
			AddHtml( 55, 272, 280, 18, "<BASEFONT COLOR=#FFFFFF>CLOSE</BASEFONT>", false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from != m_From || !IsValid() )
			{
				CloseAll();
				return;
			}

			from.SendSound( 0x4A );

			switch ( info.ButtonID )
			{
				case 1:
					CloseAll();
					from.SendGump( new HenchmanCustomizeNameGump( from, m_Henchman ) );
					break;
				case 2:
					CloseAll();
					from.SendGump( new HenchmanCustomizeHairGump( from, m_Henchman ) );
					break;
				case 3:
					if ( !m_Henchman.Female )
					{
						CloseAll();
						from.SendGump( new HenchmanCustomizeBeardGump( from, m_Henchman ) );
					}
					else
					{
						SendMain();
					}
					break;
				case 4:
					CloseAll();
					from.SendGump( new HenchmanCustomizeGenderGump( from, m_Henchman ) );
					break;
				case 5:
					CloseAll();
					from.SendGump( new HenchmanCustomizeSkinGump( from, m_Henchman ) );
					break;
			}
		}

		public static void CloseFor( Mobile owner )
		{
			if ( owner == null || owner.Deleted )
				return;

			owner.CloseGump( typeof( HenchmanCustomizeGump ) );
			owner.CloseGump( typeof( HenchmanCustomizeNameGump ) );
			owner.CloseGump( typeof( HenchmanCustomizeHairGump ) );
			owner.CloseGump( typeof( HenchmanCustomizeBeardGump ) );
			owner.CloseGump( typeof( HenchmanCustomizeGenderGump ) );
			owner.CloseGump( typeof( HenchmanCustomizeSkinGump ) );
		}
	}

	public class HenchmanCustomizeNameGump : HenchmanCustomizeGumpBase
	{
		public HenchmanCustomizeNameGump( Mobile from, BaseCreature henchman ) : base( from, henchman )
		{
			if ( !IsValid() )
				return;

			from.SendSound( 0x4A );

			RenderPanel( 480, 240, "NAME/TITLE" );

			AddHtml( 20, 60, 100, 18, "<BASEFONT COLOR=#FFFFFF>Name</BASEFONT>", false, false );
			AddTextEntry( 90, 58, 360, 22, WhiteTextHue, 1, m_Henchman.Name ?? "", NameTitleMaxLength );

			AddHtml( 20, 100, 100, 18, "<BASEFONT COLOR=#FFFFFF>Title</BASEFONT>", false, false );
			AddTextEntry( 90, 98, 360, 22, WhiteTextHue, 2, m_Henchman.Title ?? "", NameTitleMaxLength );

			AddButton( 20, 150, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtml( 55, 152, 100, 18, "<BASEFONT COLOR=#FFFFFF>OK</BASEFONT>", false, false );

			AddButton( 300, 150, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0 );
			AddHtml( 335, 152, 100, 18, "<BASEFONT COLOR=#FFFFFF>BACK</BASEFONT>", false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from != m_From || !IsValid() )
			{
				CloseAll();
				return;
			}

			if ( info.ButtonID == 0 )
			{
				SendMain();
				return;
			}

			if ( info.ButtonID == 1 )
			{
				TextRelay nameRelay = info.GetTextEntry( 1 );
				TextRelay titleRelay = info.GetTextEntry( 2 );

				string name = Sanitize( nameRelay == null ? "" : nameRelay.Text );
				string title = Sanitize( titleRelay == null ? "" : titleRelay.Text );

				if ( string.IsNullOrEmpty( name ) )
				{
					from.SendMessage( "A henchman must have a name." );
					CloseAll();
					from.SendGump( new HenchmanCustomizeNameGump( from, m_Henchman ) );
					return;
				}

				SaveNameTitle( name, title );
				ShowPaperdoll( m_From, m_Henchman );
				from.SendSound( 0x248 );
			}

			SendMain();
		}
	}

	public class HenchmanCustomizeHairGump : HenchmanCustomizeGumpBase
	{
		public HenchmanCustomizeHairGump( Mobile from, BaseCreature henchman ) : base( from, henchman )
		{
			if ( !IsValid() )
				return;

			from.SendSound( 0x4A );

			RenderPanel( 580, 440, "HAIR" );

			AddHtml( 20, 55, 110, 18, "<BASEFONT COLOR=#FFD700><B>Style</B></BASEFONT>", false, false );

			for ( int i = 0; i < HairStyles.Length; ++i )
			{
				int y = 80 + ( i * 24 );

				AddButton( 20, y, 0xFA5, 0xFA7, 100 + i, GumpButtonType.Reply, 0 );
				AddHtml( 55, y + 2, 110, 18, "<BASEFONT COLOR=#FFFFFF>" + HairStyles[i].Name + "</BASEFONT>", false, false );
			}

			AddHtml( 180, 55, 360, 18, "<BASEFONT COLOR=#FFD700><B>Color</B></BASEFONT>", false, false );

			for ( int i = 0; i < HairHues.Length; ++i )
			{
				int x = 180 + ( ( i % 4 ) * SwatchColumnWidth );
				int y = 80 + ( ( i / 4 ) * SwatchRowHeight );

				AddColorSwatch( x, y, 1000 + i, HairHues[i] );
			}

			AddBackButton( 20, 375 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from != m_From || !IsValid() )
			{
				CloseAll();
				return;
			}

			if ( info.ButtonID == 0 )
			{
				SendMain();
				return;
			}

			if ( info.ButtonID >= 100 && info.ButtonID < 100 + HairStyles.Length )
			{
				int index = info.ButtonID - 100;

				SetHair( HairStyles[index].ItemID, m_Henchman.HairHue );
				from.SendSound( 0x248 );
			}
			else if ( info.ButtonID >= 1000 && info.ButtonID < 1000 + HairHues.Length )
			{
				int index = info.ButtonID - 1000;

				m_Henchman.HairHue = HairHues[index];
				m_Henchman.ProcessDelta();
				SaveAppearance();
				from.SendSound( 0x248 );
			}
			else
			{
				return;
			}

			SendMain();
		}
	}

	public class HenchmanCustomizeBeardGump : HenchmanCustomizeGumpBase
	{
		public HenchmanCustomizeBeardGump( Mobile from, BaseCreature henchman ) : base( from, henchman )
		{
			if ( !IsValid() || m_Henchman.Female )
				return;

			from.SendSound( 0x4A );

			RenderPanel( 680, 440, "BEARD" );

			AddHtml( 20, 55, 110, 18, "<BASEFONT COLOR=#FFD700><B>Style</B></BASEFONT>", false, false );

			for ( int i = 0; i < BeardStyles.Length; ++i )
			{
				int y = 80 + ( i * 24 );

				AddButton( 20, y, 0xFA5, 0xFA7, 100 + i, GumpButtonType.Reply, 0 );
				AddHtml( 55, y + 2, 240, 18, "<BASEFONT COLOR=#FFFFFF>" + BeardStyles[i].Name + "</BASEFONT>", false, false );
			}

			AddHtml( 300, 55, 360, 18, "<BASEFONT COLOR=#FFD700><B>Color</B></BASEFONT>", false, false );

			for ( int i = 0; i < HairHues.Length; ++i )
			{
				int x = 300 + ( ( i % 4 ) * SwatchColumnWidth );
				int y = 80 + ( ( i / 4 ) * SwatchRowHeight );

				AddColorSwatch( x, y, 1000 + i, HairHues[i] );
			}

			AddBackButton( 20, 375 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from != m_From || !IsValid() || m_Henchman.Female )
			{
				CloseAll();
				return;
			}

			if ( info.ButtonID == 0 )
			{
				SendMain();
				return;
			}

			if ( info.ButtonID >= 100 && info.ButtonID < 100 + BeardStyles.Length )
			{
				int index = info.ButtonID - 100;

				SetBeard( BeardStyles[index].ItemID, m_Henchman.FacialHairHue );
				from.SendSound( 0x248 );
			}
			else if ( info.ButtonID >= 1000 && info.ButtonID < 1000 + HairHues.Length )
			{
				int index = info.ButtonID - 1000;

				m_Henchman.FacialHairHue = HairHues[index];
				m_Henchman.ProcessDelta();
				SaveAppearance();
				from.SendSound( 0x248 );
			}
			else
			{
				return;
			}

			SendMain();
		}
	}

	public class HenchmanCustomizeGenderGump : HenchmanCustomizeGumpBase
	{
		public HenchmanCustomizeGenderGump( Mobile from, BaseCreature henchman ) : base( from, henchman )
		{
			if ( !IsValid() )
				return;

			from.SendSound( 0x4A );

			RenderPanel( 370, 240, "GENDER" );

			AddHtml( 20, 60, 330, 18, "<CENTER><BASEFONT COLOR=#E4E377>" + ( m_Henchman.Female ? "Female" : "Male" ) + "</BASEFONT></CENTER>", false, false );

			AddButton( 20, 100, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			AddHtml( 55, 102, 100, 18, "<BASEFONT COLOR=#FFFFFF>Male</BASEFONT>", false, false );

			AddButton( 200, 100, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
			AddHtml( 235, 102, 100, 18, "<BASEFONT COLOR=#FFFFFF>Female</BASEFONT>", false, false );

			AddBackButton( 20, 150 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from != m_From || !IsValid() )
			{
				CloseAll();
				return;
			}

			if ( info.ButtonID == 0 )
			{
				SendMain();
				return;
			}

			if ( info.ButtonID == 1 && m_Henchman.Female )
			{
				if ( HasFemaleOnlyGear() )
				{
					from.SendMessage( "Remove gender-specific gear before customizing gender." );
					CloseAll();
					from.SendGump( new HenchmanCustomizeGenderGump( from, m_Henchman ) );
					return;
				}

				m_Henchman.BodyValue = 400;
				m_Henchman.Female = false;
				m_Henchman.ProcessDelta();
				SaveAppearance();
				ResendBodyToClients();
				ShowPaperdoll( m_From, m_Henchman );
				from.SendSound( 0x248 );
			}
			else if ( info.ButtonID == 2 && !m_Henchman.Female )
			{
				m_Henchman.BodyValue = 401;
				m_Henchman.Female = true;
				m_Henchman.FacialHairItemID = 0;
				m_Henchman.FacialHairHue = 0;
				m_Henchman.ProcessDelta();
				SaveAppearance();
				ResendBodyToClients();
				ShowPaperdoll( m_From, m_Henchman );
				from.SendSound( 0x248 );
			}

			SendMain();
		}
	}

	public class HenchmanCustomizeSkinGump : HenchmanCustomizeGumpBase
	{
		public HenchmanCustomizeSkinGump( Mobile from, BaseCreature henchman ) : base( from, henchman )
		{
			if ( !IsValid() )
				return;

			from.SendSound( 0x4A );

			RenderPanel( 600, 380, "SKIN TONE" );

			for ( int i = 0; i < SkinHues.Length; ++i )
			{
				int x = 24 + ( ( i % 6 ) * SwatchColumnWidth );
				int y = 60 + ( ( i / 6 ) * SwatchRowHeight );

				AddColorSwatch( x, y, 1000 + i, SkinHues[i] );
			}

			AddBackButton( 20, 312 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from != m_From || !IsValid() )
			{
				CloseAll();
				return;
			}

			if ( info.ButtonID == 0 )
			{
				SendMain();
				return;
			}

			if ( info.ButtonID >= 1000 && info.ButtonID < 1000 + SkinHues.Length )
			{
				int index = info.ButtonID - 1000;

				m_Henchman.Hue = SkinHues[index];
				m_Henchman.ProcessDelta();
				SaveAppearance();
				ShowPaperdoll( m_From, m_Henchman );
				from.SendSound( 0x248 );
			}

			SendMain();
		}
	}
}
