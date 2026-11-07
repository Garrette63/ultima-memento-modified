using System;
using Server.Network;

namespace Server.Items
{
	public class MementoCoin : Item
	{
		// Shared "Memento" theme hue used by the coins, the Tome of Wondrous Artifacts, and the
		// collector's cloak, sash and lantern.
		public const int MementoHue = 2141;

		public override double DefaultWeight
		{
			get { return 0.02; }
		}

		[Constructable]
		public MementoCoin() : this( 1 )
		{
		}

		[Constructable]
		public MementoCoin( int amount ) : base( 0xEED )
		{
			Name = "Memento Coins";
			Stackable = true;
			Amount = amount;
			Hue = MementoHue;
		}

		public MementoCoin( Serial serial ) : base( serial )
		{
		}

		public override int GetDropSound()
		{
			if ( Amount <= 1 )
				return 0x2E4;
			else if ( Amount <= 5 )
				return 0x2E5;
			else
				return 0x2E6;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( Amount > 1 )
				list.Add( 1050039, "{0}\t{1}", Amount, "Memento Coins" ); // ~1_NUMBER~ ~2_ITEMNAME~
			else
				list.Add( "Memento Coin" );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Deleted || !from.CanSee( this ) )
				return;

			if ( DisplayLootType )
				LabelLootTypeTo( from );

			NetState ns = from.NetState;

			if ( ns != null )
			{
				string label = ( Amount > 1 ) ? ( "Memento Coins : " + Amount ) : "Memento Coin";

				ns.Send( new UnicodeMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", label ) );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			Name = "Memento Coins";
			Stackable = true;
			Hue = MementoHue;
		}
	}
}
