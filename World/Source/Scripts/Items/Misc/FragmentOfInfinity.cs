using System;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	// A single-use consumable that tops up the enchantment points carried by
	// artefact equipment. Gift artefacts store them through IGiftable; legendary
	// (leveling) artefacts store the same kind of spendable points through
	// ILevelable.
	public class FragmentOfInfinity : Item
	{
		// How many points one fragment is worth.
		public const int EnchantmentPoints = 10;

		[Constructable]
		public FragmentOfInfinity() : base( 7964 )
		{
			Name = "Fragment of Infinity";
			Hue = 1266;
			Weight = 1.0;

			// Rendered beneath the name, in green.
			ColorText3 = "Adds 10 points of enchantment to any artefact";
			ColorHue3 = "71e26c";
		}

		public FragmentOfInfinity( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Deleted || from == null || from.Deleted )
				return;

			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1060640 ); // The item must be in your backpack to use it.
				return;
			}

			from.SendMessage( "What artefact do you wish to enchant?" );
			from.Target = new EnchantTarget( this );
		}

		// The artefact has to be on the player: worn, or carried inside their
		// backpack (nested bags inside the pack count too). Anything in the bank,
		// in another container, or lying in the world is refused.
		private static bool CanReach( Mobile from, Item item )
		{
			if ( from == null || item == null || item.Deleted )
				return false;

			if ( item.Parent == from )
				return true;

			return from.Backpack != null && item.IsChildOf( from.Backpack );
		}

		private class EnchantTarget : Target
		{
			private readonly FragmentOfInfinity m_Fragment;

			public EnchantTarget( FragmentOfInfinity fragment ) : base( 4, false, TargetFlags.None )
			{
				m_Fragment = fragment;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Fragment == null || m_Fragment.Deleted )
					return;

				Item item = targeted as Item;

				// Two artefact families expose a spare-points pool: gift artefacts
				// through IGiftable and legendary leveling artefacts through
				// ILevelable. Anything else gets the refusal and keeps the fragment.
				if ( item == null || item.Deleted || ( !( item is IGiftable ) && !( item is ILevelable ) ) )
				{
					from.SendMessage( "This only works on artefacts." );
					return;
				}

				if ( !CanReach( from, item ) )
				{
					from.SendMessage( "You can only enchant artefacts you are wearing or carrying." );
					return;
				}

				if ( item is IGiftable )
				{
					( (IGiftable)item ).Points += EnchantmentPoints;
				}
				else if ( item is ILevelable )
				{
					( (ILevelable)item ).Points += EnchantmentPoints;
				}

				item.InvalidateProperties();

				from.PlaySound( 0x1F7 );
				from.SendMessage( "You add {0} enchantment points to the artefact.", EnchantmentPoints );

				m_Fragment.Consume();
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
		}
	}

	// Drop rule for the fragment: a flat 10% chance whenever one of the named
	// bosses, or any of the 25 Magical Prison bosses, is killed.
	public static class FragmentOfInfinityDrops
	{
		private const double DropChance = 0.10;

		// OnKilledBy fires once per looter, so remember the last corpse we rolled
		// for and never roll twice for the same kill.
		private static int m_HandledCorpse;

		public static void Initialize()
		{
			EventSink.OnKilledBy += new OnKilledByEventHandler( OnKilledBy );
		}

		// The boss registry lives in MobileUtilities so the "(boss)" name tag
		// and this drop rule can never drift apart.
		private static bool IsFragmentCarrier( BaseCreature bc )
		{
			return MobileUtilities.IsBoss( bc );
		}

		// Shared roll used by the corpse drops and by the bosses that replace
		// their corpse with a reward chest (BlackGateDemon, Mangar).
		public static void TryDrop( BaseCreature bc, Container destination )
		{
			if ( bc == null || !IsFragmentCarrier( bc ) )
				return;

			TryDropChance( destination );
		}

		// Rolls only the 10% chance, for callers that already know the container
		// belongs to one of the bosses (Mangar's vault is built when it is opened).
		public static void TryDropChance( Container destination )
		{
			if ( destination == null || destination.Deleted )
				return;

			if ( Utility.RandomDouble() >= DropChance )
				return;

			destination.DropItem( new FragmentOfInfinity() );
		}

		private static void OnKilledBy( OnKilledByArgs e )
		{
			BaseCreature bc = e.Killed as BaseCreature;

			if ( bc == null || !IsFragmentCarrier( bc ) )
				return;

			// These two leave a reward chest instead of a corpse; their own
			// OnBeforeDeath routes the roll into that chest.
			if ( bc.DeleteCorpseOnDeath )
				return;

			Container corpse = e.Corpse;

			if ( corpse == null || corpse.Deleted )
				return;

			int serial = corpse.Serial.Value;

			if ( serial == m_HandledCorpse )
				return;

			m_HandledCorpse = serial;

			TryDrop( bc, corpse );
		}
	}
}
