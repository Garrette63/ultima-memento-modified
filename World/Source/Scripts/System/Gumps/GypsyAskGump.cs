using System;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	/// <summary>
	/// The gypsy's question board. It shows the last question and answer, and
	/// accepts a new question which <see cref="GypsyOracle"/> sends to the
	/// Command Code Provider API.
	/// </summary>
	public class GypsyAskGump : Gump
	{
		private readonly GypsyLady m_Gypsy;

		public GypsyAskGump( Mobile from, GypsyLady gypsy, string question, string answer, bool pending ) : base( 50, 50 )
		{
			m_Gypsy = gypsy;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			// The same frame and portrait as the starting-area gypsy gump.
			AddImage( 0, 0, 2611, PlayerSettings.GetGumpHue( from ) );
			AddImage( 10, 8, 1102 );

			int header = 11473;

			if ( Server.MySettings.S_ServerName == "Adventurers of Akalabeth" )
				header = 11376;

			AddImage( 271, 13, header, 2813 );
			AddButton( 850, 14, 4017, 4017, 0, GumpButtonType.Reply, 0 );

			string body;

			if ( pending )
			{
				body = "<I>The spirits are listening... watch, and they will speak.</I>";
			}
			else if ( !String.IsNullOrEmpty( answer ) )
			{
				body = "<B>You ask:</B> " + GypsyOracle.ToGumpHtml( question ) +
					"<BR><BR><B>She answers:</B> " + GypsyOracle.ToGumpHtml( answer );
			}
			else
			{
				body = "Ask what you will of the past, the present or the days to come, and I shall read the cards for you.";
			}

			AddHtml( 278, 73, 604, 250, "<BODY><BASEFONT Color=#DEC6DE>" + body + "</BASEFONT></BODY>", false, true );

			if ( pending )
			{
				AddHtml( 278, 330, 604, 40, "<BODY><BASEFONT Color=#DEC6DE><I>Please wait for the spirits.</I></BASEFONT></BODY>", false, false );
				return;
			}

			AddHtml( 278, 330, 200, 20, "<BODY><BASEFONT Color=#DEC6DE>Your question:</BASEFONT></BODY>", false, false );
			AddTextEntry( 278, 352, 480, 20, 0x44, 1, question == null ? String.Empty : question );

			// The circular spread of tarot cards (0x12A5), clear of the Ask button.
			AddItem( 760, 345, 4773 );
			AddButton( 800, 350, 4005, 4006, 1, GumpButtonType.Reply, 0 );
			AddHtml( 828, 352, 44, 20, "<BODY><BASEFONT Color=#DEC6DE><B>Ask</B></BASEFONT></BODY>", false, false );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( from == null || from.Deleted || info.ButtonID != 1 )
				return;

			if ( m_Gypsy == null || m_Gypsy.Deleted || !from.InRange( m_Gypsy, 12 ) )
			{
				from.SendMessage( "The gypsy is too far away to hear you." );
				return;
			}

			TextRelay entry = info.GetTextEntry( 1 );
			string question = ( entry != null && entry.Text != null ) ? entry.Text.Trim() : String.Empty;

			string error = GypsyOracle.Ask( from, m_Gypsy, question );

			if ( error != null )
			{
				from.SendGump( new GypsyAskGump( from, m_Gypsy, question, error, false ) );
				return;
			}

			from.SendGump( new GypsyAskGump( from, m_Gypsy, question, null, true ) );
		}
	}
}
