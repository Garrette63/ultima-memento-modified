using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	/// <summary>
	/// The gypsy's single-click menu. It replaces the client context menu for
	/// her so the option labels are server-side text and never need a cliloc.
	/// </summary>
	public class GypsyMenuGump : Gump
	{
		private const string Accent = "#d5a496";

		private readonly GypsyLady m_Gypsy;

		public GypsyMenuGump( Mobile from, GypsyLady gypsy ) : base( 75, 50 )
		{
			m_Gypsy = gypsy;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			AddImage( 0, 2, 9543, Server.Misc.PlayerSettings.GetGumpHue( from ) );
			AddHtml( 12, 15, 341, 20, "<BODY><BASEFONT Color=" + Accent + "><B>Visions of the Truth</B></BASEFONT></BODY>", false, false );
			AddButton( 367, 12, 4017, 4017, 0, GumpButtonType.Reply, 0 );

			AddHtml( 12, 45, 380, 40, "<BODY><BASEFONT Color=" + Accent + ">What would you ask of me?</BASEFONT></BODY>", false, false );

			AddChoice( 20, 100, 1, "Hire - read a parchment or reshape armor" );
			AddChoice( 20, 140, 2, "Talk with the gypsy" );
			AddChoice( 20, 180, 3, "Spiritual Divination" );
		}

		private void AddChoice( int x, int y, int id, string label )
		{
			AddButton( x, y, 4005, 4006, id, GumpButtonType.Reply, 0 );
			AddHtml( x + 25, y + 2, 340, 20, "<BODY><BASEFONT Color=" + Accent + ">" + label + "</BASEFONT></BODY>", false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from.Deleted || info.ButtonID == 0 )
				return;

			if ( m_Gypsy == null || m_Gypsy.Deleted || !from.InRange( m_Gypsy, 12 ) )
			{
				from.SendMessage( "The gypsy is too far away to hear you." );
				return;
			}

			switch ( info.ButtonID )
			{
				case 1:
					m_Gypsy.FindTruth( from );
					break;
				case 2:
					PlayerMobile player = from as PlayerMobile;

					if ( player != null )
						m_Gypsy.TryTalk( player );
					break;
				case 3:
					from.CloseGump( typeof( GypsyAskGump ) );
					from.SendGump( new GypsyAskGump( from, m_Gypsy, String.Empty, null, false ) );
					break;
			}
		}
	}
}
