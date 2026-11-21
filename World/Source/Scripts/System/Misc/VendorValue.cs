using System;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
	// Adds a "Worth" line to item tooltips. The amount is what a merchant
	// would pay for the item when the viewer sells it: it uses the same
	// GenericSellInfo.GetSellPriceFor formula the vendors themselves use, with the
	// viewer's Mercantile skill as the barter value. Because the stock
	// Item.PropertyList is cached per item (no viewer context), the two property
	// query handlers are replaced here so the list can be rebuilt per player.
	public static class VendorValue
	{
		private const string ValueHue = "E9C46A";

		private static GenericSellInfo m_SellInfo;

		public static void Initialize()
		{
			// 0xD6 is the batched tooltip query, 0xBF/0x10 the single item query.
			// Clients 6.0.1.7+ (ContainerGridLines) read the 6017 table for 0xD6, so
			// that table has to be replaced as well or those clients keep the stock
			// cached tooltip.
			PacketHandlers.Register( 0xD6, 0, true, new OnPacketReceive( OnBatchQueryProperties ) );
			PacketHandlers.Register6017( 0xD6, 0, true, new OnPacketReceive( OnBatchQueryProperties ) );
			PacketHandlers.RegisterExtended( 0x10, true, new OnPacketReceive( OnQueryProperties ) );

			CommandSystem.Register( "VendorValues", AccessLevel.Player, new CommandEventHandler( OnToggleVendorValues ) );
		}

		public static bool IsDisplayEnabled( Mobile m )
		{
			PlayerMobile pm = m as PlayerMobile;

			return pm == null || pm.Preferences.ShowVendorValues;
		}

		// Legacy relic loot (the DDRelic family, HighSeasRelic and the canopic jars)
		// writes its own "Worth N Gold" line into ColorText3 when it is identified.
		// That figure ignores Mercantile and duplicates the line below, so it is
		// suppressed at display time by the core Item hook. Saved items keep the
		// text (ColorText3 is serialized); it is simply never printed again.
		public static bool HideLegacyWorthText( Item item )
		{
			if ( item == null || item.CoinPrice <= 0 )
				return false;

			string text = item.ColorText3;

			return text != null && text.StartsWith( "Worth ", StringComparison.Ordinal ) && text.EndsWith( " Gold", StringComparison.Ordinal );
		}

		[Usage( "VendorValues" )]
		[Description( "Toggles the vendor value line on item tooltips." )]
		private static void OnToggleVendorValues( CommandEventArgs e )
		{
			PlayerMobile pm = e.Mobile as PlayerMobile;

			if ( pm == null )
				return;

			pm.Preferences.ShowVendorValues = !pm.Preferences.ShowVendorValues;
			RefreshTooltips( pm );

			pm.SendMessage( pm.Preferences.ShowVendorValues
				? "Worth display enabled. Tooltips refreshed."
				: "Worth display disabled. Tooltips refreshed." );
		}

		// Every ItemSalesInfo entry a merchant can buy, built the way the vendors
		// build their own sell tables. force: true skips the random spawn price
		// fluctuation so the displayed value is stable. Relic loot is in the table
		// too; AddUpBenefits returns CoinPrice for it, so the relic's authored worth
		// flows through the same Mercantile-scaled formula as everything else.
		private static GenericSellInfo SellInfo
		{
			get
			{
				if ( m_SellInfo == null )
				{
					GenericSellInfo info = new GenericSellInfo();

					ItemInformation.GetBuysList( null, info, ItemSalesInfo.Category.All, ItemSalesInfo.Material.All, ItemSalesInfo.Market.All, ItemSalesInfo.World.None, null, true );

					m_SellInfo = info;
				}

				return m_SellInfo;
			}
		}

		// Mirrors BaseVendor.TryGetSellState plus the vendor's own price formula.
		public static long GetValue( Item item, Mobile from )
		{
			if ( item == null || item.Deleted || from == null || from.Skills == null )
				return 0;

			LockableContainer parent = item.ParentEntity as LockableContainer;

			if ( parent != null && parent.Locked )
				return 0;

			// Unidentified loot is a locked NotIdentified container holding the real
			// item, so price that item from its base type and material instead of the
			// wrapper (which only carries a flat placeholder CoinPrice).
			NotIdentified unknown = item as NotIdentified;

			if ( unknown != null )
				return GetUnidentifiedValue( unknown, from );

			if ( item is Container && ((Container)item).Items.Count != 0 )
				return 0;

			if ( item is Runebook && ((Runebook)item).Entries.Count != 0 )
				return 0;

			if ( !item.Movable || !item.IsStandardLoot() )
				return 0;

			int barter = GetBarter( from );

			// Relics always show a worth: their CoinPrice once identified, and the
			// original unidentified estimate before that.
			if ( item is IRelic )
				return GetRelicValue( item, barter );

			GenericSellInfo info = SellInfo;

			if ( !info.IsSellable( item ) )
				return 0;

			int price = info.GetSellPriceFor( item, barter );

			if ( price < 1 )
				return 0;

			return (long)price * item.Amount;
		}

		private static int GetBarter( Mobile from )
		{
			int barter = (int)from.Skills[SkillName.Mercantile].Value;

			if ( barter > 100 )
				barter = 100;

			return barter;
		}

		private static long GetRelicValue( Item item, int barter )
		{
			int price = item.CoinPrice;

			// Vendors pay a single gold for anything bought from them.
			if ( item.Purchased )
				price = 1;
			else if ( item.NotIdentified )
				price = Utility.RandomMinMax( 5, 25 ); // the pre-identification estimate

			if ( price < 1 )
				return 0;

			if ( barter > 0 )
				price = (int)( price * ( 1 + barter * 0.01 ) );

			if ( price < 1 )
				price = 1;

			return (long)price * item.Amount;
		}

		// Unidentified loot shows what its base item is worth: the sell table price
		// for the wrapped item's type, scaled by its material, and nothing else. The
		// rolled properties are deliberately ignored so the tooltip cannot leak the
		// modifiers before the item is identified.
		private static long GetUnidentifiedValue( NotIdentified unknown, Mobile from )
		{
			Item inner = null;

			foreach ( Item item in unknown.Items )
			{
				inner = item;
				break;
			}

			if ( inner == null )
				return 0;

			int barter = GetBarter( from );
			GenericSellInfo info = SellInfo;
			int price = 0;

			if ( info.IsSellable( inner ) )
			{
				price = info.GetBasePriceFor( inner );

				if ( price > 0 )
				{
					double material = CraftResources.GetGold( inner.Resource );

					if ( material > 1.0 )
						price = (int)( price * material );

					// GetSellPriceFor halves the stored table price once more.
					price /= 2;

					if ( price < 1 )
						price = 1;
				}
			}

			// Types outside the sell table keep the old unidentified estimate.
			if ( price < 1 )
				price = Utility.RandomMinMax( 5, 25 );

			if ( barter > 0 )
				price = (int)( price * ( 1 + barter * 0.01 ) );

			if ( price < 1 )
				price = 1;

			return (long)price * inner.Amount;
		}

		// Re-sends the current per-player tooltips after a display preference
		// changes, so the client stops showing a cached colour immediately instead
		// of keeping the old tooltip until it is reopened or the client relogs.
		public static void RefreshTooltips( Mobile from )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null || pm.NetState == null )
				return;

			if ( pm.Backpack != null )
			{
				Refresh( pm, pm.Backpack );

				foreach ( Item item in pm.Backpack.Items )
					Refresh( pm, item );
			}

			foreach ( Item item in pm.Items )
				Refresh( pm, item );

			IPooledEnumerable eable = pm.GetItemsInRange( 18 );

			foreach ( Item item in eable )
				Refresh( pm, item );

			eable.Free();
		}

		private static void Refresh( Mobile from, Item item )
		{
			if ( item != null && !item.Deleted )
				SendPropertiesTo( from, item );
		}

		private static bool IsRelicLike( Item item )
		{
			return item is IRelic || item is EmptyCanopicJar;
		}

		private static void SendPropertiesTo( Mobile from, Item item )
		{
			long value = GetValue( item, from );
			bool ranked = ItemRank.GetTier( item ) != LootTier.None;

			// Relic worth is part of the relic itself, so it stays visible even when
			// the player has turned the generic worth line off.
			bool showValue = value > 0 && ( IsRelicLike( item ) || IsDisplayEnabled( from ) );

			// Ranked items are always built per player: the ColorText1/2 quiver tint
			// is a bug fix and independent of the preference, while the tier line and
			// the normal name tint are gated by ShowLootTiers inside ItemRank.
			if ( !ranked && !showValue )
			{
				item.SendPropertiesTo( from );
				return;
			}

			// Built per player: the tier line and name tint follow this viewer's
			// preference, and the vendor value is viewer-specific as well.
			ObjectPropertyList list = new ObjectPropertyList( item );

			ItemRank.BeginDisplay( from );

			try
			{
				item.GetProperties( list );
				item.AppendChildProperties( list );
			}
			finally
			{
				ItemRank.EndDisplay();
			}

			if ( showValue )
			{
				string text;

				if ( item.Amount > 1 )
					text = String.Format( "Worth: {0} gold ({1} each)", value.ToString( "N0" ), ( value / item.Amount ).ToString( "N0" ) );
				else
					text = String.Format( "Worth: {0} gold", value.ToString( "N0" ) );

				list.Add( 1072172, "{0}\t{1}", ValueHue, text );
			}

			list.Terminate();

			from.Send( list );
		}

		private static void OnBatchQueryProperties( NetState state, PacketReader pvSrc )
		{
			if ( !ObjectPropertyList.Enabled )
				return;

			Mobile from = state.Mobile;

			if ( from == null )
				return;

			int length = pvSrc.Size - 3;

			if ( length < 0 || ( length % 4 ) != 0 )
				return;

			int count = length / 4;

			for ( int i = 0; i < count; ++i )
			{
				Serial s = pvSrc.ReadInt32();

				if ( s.IsMobile )
				{
					Mobile m = World.FindMobile( s );

					if ( m != null && from.CanSee( m ) && Utility.InUpdateRange( from, m ) )
						m.SendPropertiesTo( from );
				}
				else if ( s.IsItem )
				{
					Item item = World.FindItem( s );

					if ( item != null && !item.Deleted && from.CanSee( item ) && Utility.InUpdateRange( from.Location, item.GetWorldLocation() ) )
						SendPropertiesTo( from, item );
				}
			}
		}

		private static void OnQueryProperties( NetState state, PacketReader pvSrc )
		{
			if ( !ObjectPropertyList.Enabled )
				return;

			Mobile from = state.Mobile;

			if ( from == null )
				return;

			Serial s = pvSrc.ReadInt32();

			if ( s.IsMobile )
			{
				Mobile m = World.FindMobile( s );

				if ( m != null && from.CanSee( m ) && Utility.InUpdateRange( from, m ) )
					m.SendPropertiesTo( from );
			}
			else if ( s.IsItem )
			{
				Item item = World.FindItem( s );

				if ( item != null && !item.Deleted && from.CanSee( item ) && Utility.InUpdateRange( from.Location, item.GetWorldLocation() ) )
					SendPropertiesTo( from, item );
			}
		}
	}
}
