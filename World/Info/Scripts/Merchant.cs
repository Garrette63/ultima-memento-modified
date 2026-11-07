using Server;
using System;
using System.Collections.Generic;
using System.Collections;
using Server.Items;
using Server.Multis;
using Server.Guilds;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Mobiles
{
	public class StoreMerchant : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public StoreMerchant()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			// Add or remomve items you want to sell here. Each line is in this format:
			// Add( new GenericBuyInfo( ITEM TYPE, PRICE, QUANTITY, ITEM ID, HUE ) );
			// Keep your individual quantities below 10,000.
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( BagOfSending ), 8000, 5, 0xE76, 0x89B ) );
				Add( new GenericBuyInfo( typeof( BallOfSummoning ), 6000, 5, 0xE2E, 0 ) );
				Add( new GenericBuyInfo( typeof( PowderOfTemperament ), 10000, 5, 4102, 2419 ) );
				Add( new GenericBuyInfo( typeof( PowderOfTranslocation ), 1000, 5, 0x26B8, 0 ) );
				Add( new GenericBuyInfo( typeof( ArcaneGem ), 1000, 5, 0x1EA7, 0 ) );
				Add( new GenericBuyInfo( typeof( LuckyHorseShoes ), 2500, 5, 0xFB6, 0 ) );
				Add( new GenericBuyInfo( typeof( Runebook ), 7000, 5, 0xF3D, 0xB61 ) );
				Add( new GenericBuyInfo( typeof( RecallScroll ), 100, 5, 0x1F4C, 0 ) );
				Add( new GenericBuyInfo( typeof( MarkScroll ), 100, 5, 0x1F59, 0 ) );
				Add( new GenericBuyInfo( typeof( BottleOfAcid ), 600, 5, 0x180F, 0x178 ) );
				Add( new GenericBuyInfo( typeof( AutoResPotion ), 2000, 5, 0x0E0F, 0x494 ) );
				Add( new GenericBuyInfo( typeof( InvulnerabilityPotion ), 8000, 5, 0x180F, 0x501 ) );
				Add( new GenericBuyInfo( typeof( ResurrectPotion ), 1000, 5, 0x180F, 0xB6D ) );
				Add( new GenericBuyInfo( typeof( DurabilityPotion ), 500, 5, 0x180F, 0xB7D ) );
				Add( new GenericBuyInfo( typeof( RepairPotion ), 2000, 5, 0x180F, 0xB7A ) );
				Add( new GenericBuyInfo( typeof( SuperPotion ), 500, 5, 0x180F, 0xBA4 ) );
				Add( new GenericBuyInfo( typeof( GreaterRejuvenatePotion ), 500, 5, 0x2406, 0x48E ) );
				Add( new GenericBuyInfo( typeof( GreaterInvisibilityPotion ), 500, 5, 0x2406, 0x490 ) );
				Add( new GenericBuyInfo( typeof( GreaterCurePotion ), 200, 5, 0x24EA, 0 ) );
				Add( new GenericBuyInfo( typeof( TransmutationPotion ), 4000, 5, 0x2827, 0x143 ) );
				Add( new GenericBuyInfo( typeof( GenderPotion ), 3000, 5, 0x1FDC, 0xB46 ) );
				Add( new GenericBuyInfo( typeof( HairDyePotion ), 2000, 5, 0x180F, 0 ) );
				Add( new GenericBuyInfo( typeof( HairOilPotion ), 2000, 5, 0x180F, 0xB50 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			// Add or remomve items this merchant will buy, along with the gold value.
			// Prices below will be divided by 2 to ensure mercantile mechanics.
			public InternalSellInfo()
			{
			}
		}
	}
}