using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	// Dev helpers: spawn one random equipment item that ItemRank rates at a given
	// tier, so the coloured tooltip line can be checked side by side.
	public class LootTierCommands
	{
		private const int MaxAttempts = 300;

		public static void Initialize()
		{
			CommandSystem.Register( "TierCommon", AccessLevel.GameMaster, new CommandEventHandler( OnCommon ) );
			CommandSystem.Register( "TierMagic", AccessLevel.GameMaster, new CommandEventHandler( OnMagic ) );
			CommandSystem.Register( "TierRare", AccessLevel.GameMaster, new CommandEventHandler( OnRare ) );
			CommandSystem.Register( "TierLegendary", AccessLevel.GameMaster, new CommandEventHandler( OnLegendary ) );
			CommandSystem.Register( "TierAncient", AccessLevel.GameMaster, new CommandEventHandler( OnAncient ) );
			CommandSystem.Register( "TierPrimal", AccessLevel.GameMaster, new CommandEventHandler( OnPrimal ) );
			CommandSystem.Register( "TierAll", AccessLevel.GameMaster, new CommandEventHandler( OnAll ) );
			CommandSystem.Register( "TierItem", AccessLevel.GameMaster, new CommandEventHandler( OnItem ) );
			CommandSystem.Register( "LootTiers", AccessLevel.Player, new CommandEventHandler( OnToggleLootTiers ) );
		}

		[Usage( "TierCommon" )]
		[Description( "Spawns one random equipment item rated Common." )]
		private static void OnCommon( CommandEventArgs e ) { Spawn( e.Mobile, LootTier.Common ); }

		[Usage( "TierMagic" )]
		[Description( "Spawns one random equipment item rated Magic." )]
		private static void OnMagic( CommandEventArgs e ) { Spawn( e.Mobile, LootTier.Magic ); }

		[Usage( "TierRare" )]
		[Description( "Spawns one random equipment item rated Rare." )]
		private static void OnRare( CommandEventArgs e ) { Spawn( e.Mobile, LootTier.Rare ); }

		[Usage( "TierLegendary" )]
		[Description( "Spawns one random equipment item rated Legendary." )]
		private static void OnLegendary( CommandEventArgs e ) { Spawn( e.Mobile, LootTier.Legendary ); }

		[Usage( "TierAncient" )]
		[Description( "Spawns one random equipment item rated Ancient." )]
		private static void OnAncient( CommandEventArgs e ) { Spawn( e.Mobile, LootTier.Ancient ); }

		[Usage( "TierPrimal" )]
		[Description( "Spawns one random equipment item rated Primal." )]
		private static void OnPrimal( CommandEventArgs e ) { Spawn( e.Mobile, LootTier.Primal ); }

		[Usage( "TierAll" )]
		[Description( "Spawns one random equipment item of every tier." )]
		private static void OnAll( CommandEventArgs e )
		{
			for ( int i = (int)LootTier.Common; i <= (int)LootTier.Primal; ++i )
				Spawn( e.Mobile, (LootTier)i );
		}

		[Usage( "TierItem <common|magic|rare|legendary|ancient|primal>" )]
		[Description( "Spawns one random equipment item rated at the given tier." )]
		private static void OnItem( CommandEventArgs e )
		{
			LootTier tier;

			if ( !TryParse( e.ArgString, out tier ) )
			{
				e.Mobile.SendMessage( "Usage: [TierItem <common|magic|rare|legendary|ancient|primal>" );
				return;
			}

			Spawn( e.Mobile, tier );
		}

		[Usage( "LootTiers" )]
		[Description( "Toggles the cosmetic loot tier display on item tooltips." )]
		private static void OnToggleLootTiers( CommandEventArgs e )
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			if ( pm == null )
				return;

			pm.Preferences.ShowLootTiers = !pm.Preferences.ShowLootTiers;
			VendorValue.RefreshTooltips( pm );

			pm.SendMessage( pm.Preferences.ShowLootTiers
				? "Loot tier display enabled. Tooltips refreshed."
				: "Loot tier display disabled. Tooltips refreshed." );
		}

		public static void Spawn( Mobile from, LootTier tier )
		{
			Item item = Build( tier );

			if ( item == null )
			{
				from.SendMessage( "Could not build a {0} item after {1} attempts.", tier, MaxAttempts );
				return;
			}

			if ( from.Backpack != null )
				from.Backpack.DropItem( item );
			else
				item.MoveToWorld( from.Location, from.Map );

			from.SendMessage( "Spawned {0} [{1}] - power {2}.", item.Name ?? item.GetType().Name, tier, ItemRank.GetPower( item ) );
		}

		// Builds a plain random equipment item and rolls single random attributes
		// until ItemRank rates it at the requested tier.
		public static Item Build( LootTier tier )
		{
			int floor = ItemRank.GetFloor( tier );
			int ceiling = ItemRank.GetCeiling( tier );

			for ( int attempt = 0; attempt < MaxAttempts; ++attempt )
			{
				Item item = RandomEquipment();

				if ( item == null )
					continue;

				if ( ItemRank.GetPower( item ) < 0 )
				{
					item.Delete();
					continue;
				}

				if ( tier != LootTier.Common )
				{
					int guard = 0;

					while ( ItemRank.GetPower( item ) < floor && guard++ < 40 )
					{
						BaseRunicTool.ApplyAttributes( 0, item, 1, MinIntensity( tier ), MaxIntensity( tier ) );

						if ( ItemRank.GetPower( item ) >= ceiling )
							break;
					}
				}

				if ( ItemRank.GetTier( item ) == tier )
					return item;

				item.Delete();
			}

			return null;
		}

		private static Item RandomEquipment()
		{
			switch ( Utility.Random( 7 ) )
			{
				case 0: return global::Server.Loot.RandomWeapon( false );
				case 1: return global::Server.Loot.RandomArmorOrShield( false );
				case 2: return global::Server.Loot.RandomClothing( false );
				case 3: return global::Server.Loot.RandomHats( false );
				case 4: return global::Server.Loot.RandomJewelry();
				case 5: return global::Server.Loot.RandomInstrument();
				default: return global::Server.Loot.RandomQuiver();
			}
		}

		private static int MinIntensity( LootTier tier )
		{
			switch ( tier )
			{
				case LootTier.Magic: return 20;
				case LootTier.Rare: return 30;
				case LootTier.Legendary: return 50;
				case LootTier.Ancient: return 60;
				default: return 70;
			}
		}

		private static int MaxIntensity( LootTier tier )
		{
			switch ( tier )
			{
				case LootTier.Magic: return 50;
				case LootTier.Rare: return 70;
				case LootTier.Legendary: return 90;
				default: return 100;
			}
		}

		private static bool TryParse( string text, out LootTier tier )
		{
			switch ( ( text ?? "" ).Trim().ToLower() )
			{
				case "common": tier = LootTier.Common; return true;
				case "magic": tier = LootTier.Magic; return true;
				case "rare": tier = LootTier.Rare; return true;
				case "legendary": tier = LootTier.Legendary; return true;
				case "ancient": tier = LootTier.Ancient; return true;
				case "primal": tier = LootTier.Primal; return true;
			}

			tier = LootTier.None;
			return false;
		}
	}
}
