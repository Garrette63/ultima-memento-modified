using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	// The real transmogrify item: a gem that Paragon creatures drop.
	//
	// Same use as the test scroll - double click, target a piece of armour,
	// clothing or a weapon, pick an appearance, Apply - and it is spent on the Apply,
	// with the item's original look recorded in Saves/Transmogrify so Revert can put
	// it back. Weapons are limited to other weapons of the same combat skill.
	public class TransmogrifyGem : TransmogrifyTool
	{
		public const int GemHue = 0x48E;   // the shared transmogrify identity colour

		[Constructable]
		public TransmogrifyGem() : base( 0x0F26 ) // diamond
		{
			Name = "Transmogrification Gem";
			Hue = GemHue;
			Weight = 0.1;
			Light = LightType.Circle150;
		}

		public TransmogrifyGem( Serial serial ) : base( serial )
		{
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

			Light = LightType.Circle150;
		}
	}

	// Paragon drop: 12.5% of Paragons leave a Transmogrification Gem on the corpse,
	// with the chance rising on the killer's Luck - the same stat the shard's own
	// Paragon artifact roll uses - and capped so it never becomes a certainty.
	//
	// These are half the original rates (25% base, capped 90%) at the shard owner's
	// request: 12.5% at 0 luck, 25% at 1000, 37.5% at 2000, capped 45% from 2600 up.
	public static class TransmogrifyDrops
	{
		private const double BaseChance = 0.125;
		private const double LuckScale = 8000.0;   // +100% chance per 8000 luck
		private const double MaxChance = 0.45;

		// Corpse serial we have already rolled for, so one kill can only ever yield
		// one gem even though OnKilledBy fires once per looter.
		private static int m_Handled;

		public static void Initialize()
		{
			EventSink.OnKilledBy += new OnKilledByEventHandler( OnKilledBy );
		}

		// Chance for a given killer: 12.5% at 0 luck, 25% at 1000, 37.5% at 2000,
		// capped at 45% from 2600 luck upwards.
		public static double ChanceFor( Mobile m )
		{
			if ( m == null )
				return BaseChance;

			int luck = m.Luck;

			if ( luck < 0 )
				luck = 0;

			double chance = BaseChance + luck / LuckScale;

			return chance > MaxChance ? MaxChance : chance;
		}

		private static void OnKilledBy( OnKilledByArgs e )
		{
			BaseCreature bc = e.Killed as BaseCreature;

			if ( bc == null || !bc.IsParagon )
				return;

			Mobile killer = e.KilledBy;

			if ( killer == null || killer.Deleted )
				return;

			// Roll once per corpse: the first looter is the top damager, and its luck
			// is the one that counts.
			Container corpse = e.Corpse;
			int serial = corpse != null ? corpse.Serial.Value : 0;

			if ( serial != 0 )
			{
				if ( serial == m_Handled )
					return;

				m_Handled = serial;
			}

			if ( ChanceFor( killer ) <= Utility.RandomDouble() )
				return;

			TransmogrifyGem gem = new TransmogrifyGem();

			if ( corpse != null && !corpse.Deleted )
			{
				corpse.DropItem( gem );
			}
			else if ( !killer.AddToBackpack( gem ) )
			{
				gem.MoveToWorld( killer.Location, killer.Map );
			}
		}
	}
}
