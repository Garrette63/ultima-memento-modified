using System;
using Server;
using Server.Targeting;

namespace Server.Items
{
	// Shared behaviour for anything that opens the transmogrify picker - the GM test
	// scroll and the Paragon-dropped gem. Both are single use: TransmogrifyGump
	// spends the tool when an appearance is applied, not when the target is chosen,
	// so a bad target, or closing the picker, costs nothing.
	public abstract class TransmogrifyTool : Item
	{
		public TransmogrifyTool( int itemID ) : base( itemID )
		{
		}

		// Public, and with the serialisation pair declared, so the shard's startup
		// verifier (Main.VerifyType) stays quiet. Neither writes anything: subclasses
		// own their own version int.
		public TransmogrifyTool( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
		}

		// The item has to be on the player: worn, or carried inside their backpack
		// (nested bags inside the pack count too). Anything in the bank, in another
		// container, or lying in the world is refused.
		public static bool CanReach( Mobile from, Item item )
		{
			if ( from == null || item == null || item.Deleted )
				return false;

			if ( item.Parent == from )
				return true;

			return from.Backpack != null && item.IsChildOf( from.Backpack );
		}

		// Changes only the appearance. When GraphicID was simply mirroring ItemID
		// (the normal case for armour, shields and weapons), keep it in step too so
		// weapon animation and paperdoll art follow the new shape. A custom
		// GraphicID - modded art or a light's lit/unlit state - is left alone.
		public static void SetAppearance( Item item, int itemID )
		{
			if ( item == null || item.Deleted || itemID <= 0 || item.ItemID == itemID )
				return;

			bool syncGraphic = item.GraphicID == item.ItemID;

			if ( syncGraphic )
				item.GraphicID = itemID;

			item.ItemID = itemID;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Deleted || from == null || from.Deleted )
				return;

			if ( !IsChildOf( from.Backpack ) && !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}

			from.SendMessage( "Target the armor, clothing, or weapon whose appearance you wish to change." );
			from.Target = new ChooseTarget( this );
		}

		private class ChooseTarget : Target
		{
			private readonly Item m_Tool;

			public ChooseTarget( Item tool ) : base( 4, false, TargetFlags.None )
			{
				m_Tool = tool;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Tool == null || m_Tool.Deleted )
					return;

				Item item = targeted as Item;

				if ( item == null || item.Deleted || !TransmogrifyCatalog.CanReshape( item ) )
				{
					from.SendMessage( "That is not armor, clothing, or a weapon." );
					return;
				}

				if ( !TransmogrifyTool.CanReach( from, item ) )
				{
					from.SendMessage( "You can only change armor, clothing or weapons you are wearing or carrying." );
					return;
				}

				int count = TransmogrifyCatalog.CountFor( item );

				if ( count == 0 )
				{
					from.SendMessage( 0x22, "Nothing can be changed into that slot ({0}). Catalog: {1} appearances across {2} slots.",
						item.Layer, TransmogrifyCatalog.TotalCount, TransmogrifyCatalog.SlotCount );
					return;
				}

				// Send the picker first, then ask for the paperdoll: the window request
				// has to land after the gump or some clients show it and drop it again.
				TransmogrifyGump gump = new TransmogrifyGump( from, item, m_Tool );

				from.SendGump( gump );

				gump.OpenPaperdoll( from );
			}
		}
	}
}
