using System;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.ContextMenus
{
	public class HenchmanInfoEntry : ContextMenuEntry
	{
		private Mobile m_From;
		private Mobile m_Henchman;

		public HenchmanInfoEntry( Mobile from, Mobile henchman ) : base( 98, 3 )
		{
			m_From = from;
			m_Henchman = henchman;
		}

		public override void OnClick()
		{
			if ( m_From == null || m_Henchman == null || m_Henchman.Deleted )
				return;

			BaseCreature henchman = m_Henchman as BaseCreature;

			// Only the owning player may inspect a henchman.
			if ( henchman == null || henchman.ControlMaster != m_From )
				return;

			m_From.CloseGump( typeof( HenchmanInfoGump ) );
			m_From.SendGump( new HenchmanInfoGump( m_Henchman, m_From ) );
		}
	}
}
