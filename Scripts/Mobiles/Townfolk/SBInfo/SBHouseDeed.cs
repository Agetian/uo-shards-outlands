using System;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Mobiles
{
	public class SBHouseDeed: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHouseDeed()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                /*
                Add(new GenericBuyInfo("deed to a stone-and-plaster house", typeof(StonePlasterHouseDeed), StonePlasterHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a field stone house", typeof(FieldStoneHouseDeed), FieldStoneHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a small brick house", typeof(SmallBrickHouseDeed), SmallBrickHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a wooden house", typeof(WoodHouseDeed), WoodHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a wood-and-plaster house", typeof(WoodPlasterHouseDeed), WoodPlasterHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a thatched-roof cottage", typeof(ThatchedRoofCottageDeed), ThatchedRoofCottageDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("deed to a small stone workshop", typeof(StoneWorkshopDeed), StoneWorkshopDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a small marble workshop", typeof(MarbleWorkshopDeed), MarbleWorkshopDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("deed to a small stone tower", typeof(SmallTowerDeed), SmallTowerDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a sandstone house with patio", typeof(SandstonePatioDeed), SandstonePatioDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("deed to a two story villa", typeof(VillaDeed), VillaDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a two story log cabin", typeof(LogCabinDeed), LogCabinDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("deed to a large house with patio", typeof(LargePatioDeed), LargePatioDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a brick house", typeof(BrickHouseDeed), BrickHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("deed to a two-story wood-and-plaster house", typeof(TwoStoryWoodPlasterHouseDeed), TwoStoryWoodPlasterHouseDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a marble house with patio", typeof(LargeMarbleDeed), LargeMarbleDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));

                Add(new GenericBuyInfo("deed to a tower", typeof(TowerDeed), TowerDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a small stone keep", typeof(KeepDeed), KeepDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
                Add(new GenericBuyInfo("deed to a castle", typeof(CastleDeed), CastleDeed.GetSBPurchaseValue(), 25, 0x14F0, 0));
			    */
            }
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                /*
                Add(typeof(StonePlasterHouseDeed), StonePlasterHouseDeed.GetSBSellValue());
                Add(typeof(FieldStoneHouseDeed), FieldStoneHouseDeed.GetSBSellValue());
                Add(typeof(SmallBrickHouseDeed), SmallBrickHouseDeed.GetSBSellValue());
                Add(typeof(WoodHouseDeed), WoodHouseDeed.GetSBSellValue());
                Add(typeof(WoodPlasterHouseDeed), WoodPlasterHouseDeed.GetSBSellValue());
                Add(typeof(ThatchedRoofCottageDeed), ThatchedRoofCottageDeed.GetSBSellValue());
                Add(typeof(BrickHouseDeed), BrickHouseDeed.GetSBSellValue());
                Add(typeof(TwoStoryWoodPlasterHouseDeed), TwoStoryWoodPlasterHouseDeed.GetSBSellValue());
                Add(typeof(TowerDeed), TowerDeed.GetSBSellValue());
                Add(typeof(KeepDeed), KeepDeed.GetSBSellValue());
                Add(typeof(CastleDeed), CastleDeed.GetSBSellValue());
                Add(typeof(LargePatioDeed), LargePatioDeed.GetSBSellValue());
                Add(typeof(LargeMarbleDeed), LargeMarbleDeed.GetSBSellValue());
                Add(typeof(SmallTowerDeed), SmallTowerDeed.GetSBSellValue());
                Add(typeof(LogCabinDeed), LogCabinDeed.GetSBSellValue());
                Add(typeof(SandstonePatioDeed), SandstonePatioDeed.GetSBSellValue());
                Add(typeof(VillaDeed), VillaDeed.GetSBSellValue());
                Add(typeof(StoneWorkshopDeed), StoneWorkshopDeed.GetSBSellValue());
                Add(typeof(MarbleWorkshopDeed), MarbleWorkshopDeed.GetSBSellValue());
                */
			}
		}
	}
}
