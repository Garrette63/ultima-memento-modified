using System;
using Server.Items;
using Server.Mobiles;

namespace Server
{
	// Diablo-style rarity tiers. Artefacts are the top tier and keep their own
	// marker in addition to the tier line.
	public enum LootTier
	{
		None = 0,
		Common,
		Magic,
		Rare,
		Legendary,
		Ancient,
		Primal,
		Artefact
	}

	// Ranks equipment by a power estimate and adds a coloured tier line where the
	// artefact marker would appear (Item.AddNameProperties calls ItemRank.AppendTier
	// at that exact spot). Artefacts are always the top tier.
	//
	// Power = the shard's own weighted property budget, i.e. the same weights
	// ItemInformation.AddUpBenefits uses when pricing loot for vendors, with the
	// item's base price removed. A small material and quality adjustment is added
	// on top. Because the budget only counts what the item actually rolled, it is
	// a direct estimate of how much power the item carries.
	public static class ItemRank
	{
		private static readonly string[] m_Names = { "", "Common", "Magic", "Rare", "Legendary", "Ancient", "Primal", "Artefact" };
		private static readonly string[] m_Hues = { "", "C7C7C7", "6C8CFF", "FFFF99", "FF9C40", "FF5A5A", "D46AFF", "FFC000" };

		// Cumulative power floors; index matches LootTier. Power is the shard's own
		// weighted property budget (AddUpBenefits). Ancient/Primal stay at 500/700;
		// Magic deliberately covers a wide band (80-219), Rare starts at 220 and
		// Legendary at 400 so the name only applies to genuinely strong rolls.
		private static readonly int[] m_PowerFloor = { 0, 0, 80, 220, 400, 500, 700 };

		[ThreadStatic]
		private static Mobile m_Viewer;

		// The per-player OPL build sets the viewer, so the tier line and name tint
		// follow that player's preference. Cached shared property lists are built
		// with no viewer and stay neutral (normal name, no tier line).
		public static void BeginDisplay( Mobile viewer )
		{
			m_Viewer = viewer;
		}

		public static void EndDisplay()
		{
			m_Viewer = null;
		}

		public static bool IsDisplayEnabled( Mobile m )
		{
			PlayerMobile pm = m as PlayerMobile;

			return pm == null || pm.Preferences.ShowLootTiers;
		}

		private static bool IsDisplayVisible()
		{
			return m_Viewer != null && IsDisplayEnabled( m_Viewer );
		}

		public static void AppendTier( Item item, ObjectPropertyList list )
		{
			if ( !IsDisplayVisible() )
				return;

			// Artefacts already carry their own marker line; their tier only drives
			// the name/label colour, so do not print a second "Artefact" line.
			if ( IsArtefact( item ) )
				return;

			LootTier tier = GetTier( item );

			if ( tier == LootTier.None )
				return;

			list.Add( 1072172, "{0}\t{1}", m_Hues[(int)tier], m_Names[(int)tier] );
		}

		public static LootTier GetTier( Item item )
		{
			if ( item == null || item.Deleted )
				return LootTier.None;

			NotIdentified unknown = item as NotIdentified;

			if ( unknown != null )
				return GetTier( GetWrappedItem( unknown ) );

			if ( IsArtefact( item ) )
				return LootTier.Artefact;

			int power = GetPower( item );

			if ( power < 0 )
				return LootTier.None;

			LootTier tier = LootTier.Common;

			for ( int i = 2; i < m_PowerFloor.Length; ++i )
			{
				if ( power >= m_PowerFloor[i] )
					tier = (LootTier)i;
			}

			return tier;
		}

		// Power needed to enter a tier, and the power where the next tier begins.
		// Artefact is not power-based, so its floor is effectively unreachable.
		public static int GetFloor( LootTier tier )
		{
			if ( tier == LootTier.Artefact )
				return int.MaxValue;

			int index = (int)tier;

			if ( index < (int)LootTier.Common || index >= m_PowerFloor.Length )
				return 0;

			return m_PowerFloor[index];
		}

		public static int GetCeiling( LootTier tier )
		{
			int index = (int)tier + 1;

			if ( (int)tier < (int)LootTier.Common || index >= m_PowerFloor.Length )
				return int.MaxValue;

			return m_PowerFloor[index];
		}

		// Ground artefact names are drawn as single-click/overhead labels, which take
		// a real UO hue id. Artefact is the top loot tier, and this label follows the
		// tier display toggle.
		private const int ArtefactLabelHue = 0x0B54; // UO hue 2900

		public static bool IsArtefact( Item item )
		{
			if ( item == null || item.Deleted )
				return false;

			return item.ArtifactLevel != ArtifactLevel.None || HasArtifactRarity( item );
		}

		public static int GetLabelHue( Item item, Mobile viewer )
		{
			if ( item == null || item.Deleted || item.Parent != null || GetTier( item ) != LootTier.Artefact )
				return 0x3B2;

			PlayerMobile pm = viewer as PlayerMobile;

			if ( pm != null && !pm.Preferences.ShowLootTiers )
				return 0x3B2;

			return ArtefactLabelHue;
		}

		// Emits the item name as a rarity-coloured header line (cliloc 1072171 uses
		// the same <BASEFONT COLOR> markup the shard already renders for ColorText1).
		// Returns false when the item is not ranked, so the normal name path runs.
		public static bool TryAppendColoredName( Item item, ObjectPropertyList list )
		{
			if ( !IsDisplayVisible() )
				return false;

			if ( item == null || item.Amount > 1 )
				return false;

			LootTier tier = GetTier( item );

			if ( tier == LootTier.None )
				return false;

			return TryAppendName( item, list, m_Hues[(int)tier] );
		}

		private static bool TryAppendName( Item item, ObjectPropertyList list, string hue )
		{
			string name = item.Name;
			string material = null;
			bool exceptional = false;

			if ( item is BaseArmor && ( (BaseArmor)item ).Quality == ArmorQuality.Exceptional )
				exceptional = true;
			else if ( item is BaseTrinket && ( (BaseTrinket)item ).Quality == TrinketQuality.Exceptional )
				exceptional = true;

			if ( ( item is BaseWeapon || item is BaseArmor || item is BaseClothing || item is BaseTrinket || item is BaseInstrument )
				&& CraftResources.GetClilocLowerCaseName( item.Resource ) > 0 && item.SubResource == CraftResource.None )
			{
				material = CliLocTable.Lookup( CraftResources.GetClilocLowerCaseName( item.Resource ) );
			}

			if ( name == null )
				name = CliLocTable.Lookup( item.LabelNumber );

			// Without resolved text we cannot compose the name safely; fall back to
			// the normal naming path instead of showing raw cliloc tokens.
			if ( name == null )
				return false;

			if ( material != null )
				name = material + " " + name;

			if ( exceptional )
				name = "exceptional " + name;

			list.Add( 1072171, "{0}\t{1}", hue, name );

			return true;
		}

		// Tier hue used to tint header lines built from ColorText1/2 (quivers, etc.).
		// With Show Loot Tiers off, quivers switch to the default white name colour
		// so they match ordinary equipment; other authored ColorText hues are kept.
		public static string GetNameHue( Item item )
		{
			if ( m_Viewer == null )
				return null;

			if ( !IsDisplayEnabled( m_Viewer ) )
				return item is BaseQuiver ? "FFFFFF" : null;

			LootTier tier = GetTier( item );

			return tier == LootTier.None ? null : m_Hues[(int)tier];
		}

		// Returns -1 when the item is not ranked (not equipment, an artefact or a
		// CoinPrice oddity); artefacts get their tier directly in GetTier.
		public static int GetPower( Item item )
		{
			if ( item == null || item.Deleted )
				return -1;

			// Unidentified loot is a locked NotIdentified container around the real
			// item, so rank the wrapped equipment and let its tier line (and name
			// hue) show even though the wrapper itself is not equipment.
			NotIdentified unknown = item as NotIdentified;

			if ( unknown != null )
				return GetPower( GetWrappedItem( unknown ) );

			if ( item.ArtifactLevel != ArtifactLevel.None )
				return -1;

			if ( item.NotIdentified || item.CoinPrice > 0 )
				return -1;

			if ( !IsEquipment( item ) )
				return -1;

			if ( HasArtifactRarity( item ) )
				return -1;

			// Same weights the vendor price uses, with the base price removed.
			// AddUpBenefits floors the value at 1, so drop that floor as well.
			int power = ItemInformation.AddUpBenefits( item, 0, false, false );

			if ( power <= 1 )
				power = 0;
			else
				power -= 1;

			// Self repair is a convenience property, but its gold weight is 100 per
			// point (up to 5). Credit at most one point so a utility-only roll cannot
			// outrank a genuine combat roll.
			int repair = 0;

			if ( item is BaseWeapon )
				repair = ( (BaseWeapon)item ).WeaponAttributes.SelfRepair;
			else if ( item is BaseArmor )
				repair = ( (BaseArmor)item ).ArmorAttributes.SelfRepair;
			else if ( item is BaseClothing )
				repair = ( (BaseClothing)item ).ClothingAttributes.SelfRepair;

			if ( repair > 1 )
				power -= ( repair - 1 ) * 100;

			if ( power < 0 )
				power = 0;

			// Material: 1.00 for iron/base, rising with the rarer ores and woods.
			double material = CraftResources.GetGold( item.Resource );

			if ( material > 1.0 )
				power += (int)( ( material - 1.0 ) * 30.0 );

			return (int)( power * GetQualityFactor( item ) );
		}

		private static Item GetWrappedItem( NotIdentified unknown )
		{
			foreach ( Item item in unknown.Items )
				return item;

			return null;
		}

		private static bool IsEquipment( Item item )
		{
			return item is BaseWeapon
				|| item is BaseArmor
				|| item is BaseTrinket
				|| item is BaseQuiver
				|| item is BaseClothing
				|| item is BaseInstrument
				|| item is Spellbook;
		}

		private static bool HasArtifactRarity( Item item )
		{
			if ( item is BaseWeapon )
				return ( (BaseWeapon)item ).ArtifactRarity > 0;

			if ( item is BaseArmor )
				return ( (BaseArmor)item ).ArtifactRarity > 0;

			if ( item is BaseClothing )
				return ( (BaseClothing)item ).ArtifactRarity > 0;

			return false;
		}

		private static double GetQualityFactor( Item item )
		{
			if ( item is BaseArmor )
			{
				ArmorQuality quality = ( (BaseArmor)item ).Quality;

				if ( quality == ArmorQuality.Low )
					return 0.85;

				if ( quality == ArmorQuality.Exceptional )
					return 1.15;
			}
			else if ( item is BaseWeapon )
			{
				WeaponQuality quality = ( (BaseWeapon)item ).Quality;

				if ( quality == WeaponQuality.Low )
					return 0.85;

				if ( quality == WeaponQuality.Exceptional )
					return 1.15;
			}
			else if ( item is BaseClothing )
			{
				ClothingQuality quality = ( (BaseClothing)item ).Quality;

				if ( quality == ClothingQuality.Low )
					return 0.85;

				if ( quality == ClothingQuality.Exceptional )
					return 1.15;
			}
			else if ( item is BaseQuiver )
			{
				ClothingQuality quality = ( (BaseQuiver)item ).Quality;

				if ( quality == ClothingQuality.Low )
					return 0.85;

				if ( quality == ClothingQuality.Exceptional )
					return 1.15;
			}
			else if ( item is BaseInstrument )
			{
				InstrumentQuality quality = ( (BaseInstrument)item ).Quality;

				if ( quality == InstrumentQuality.Low )
					return 0.85;

				if ( quality == InstrumentQuality.Exceptional )
					return 1.15;
			}

			return 1.0;
		}
	}
}
