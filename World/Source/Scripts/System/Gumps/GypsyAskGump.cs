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
		private const string Accent = "#d5a496";

		private readonly GypsyLady m_Gypsy;

		public GypsyAskGump( Mobile from, GypsyLady gypsy, string question, string answer, bool pending ) : base( 75, 50 )
		{
			m_Gypsy = gypsy;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			AddImage( 0, 2, 9543, PlayerSettings.GetGumpHue( from ) );
			AddHtml( 12, 15, 341, 20, "<BODY><BASEFONT Color=" + Accent + "><B>Visions of the Truth</B></BASEFONT></BODY>", false, false );
			AddButton( 367, 12, 4017, 4017, 0, GumpButtonType.Reply, 0 );

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

			AddHtml( 12, 42, 380, 180, "<BODY><BASEFONT Color=" + Accent + ">" + body + "</BASEFONT></BODY>", false, true );

			if ( pending )
			{
				AddHtml( 12, 232, 380, 40, "<BODY><BASEFONT Color=" + Accent + "><I>Please wait for the spirits.</I></BASEFONT></BODY>", false, false );
				return;
			}

			AddHtml( 12, 232, 200, 20, "<BODY><BASEFONT Color=" + Accent + ">Your question:</BASEFONT></BODY>", false, false );
			AddTextEntry( 12, 252, 322, 20, 0x44, 1, question == null ? String.Empty : question );

			AddButton( 340, 250, 4005, 4006, 1, GumpButtonType.Reply, 0 );
			AddHtml( 340, 252, 44, 20, "<BODY><BASEFONT Color=" + Accent + "><B>Ask</B></BASEFONT></BODY>", false, false );
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
