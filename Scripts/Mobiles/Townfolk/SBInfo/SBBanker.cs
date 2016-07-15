
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBBanker : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBanker()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Commodity Deed",  typeof(CommodityDeed), CommodityDeed.GetSBPurchaseValue(), 25, 0x14F0, 0x47));
                Add(new GenericBuyInfo("Vendor Rental Contract", typeof(VendorRentalContract), VendorRentalContract.GetSBPurchaseValue(), 25, 0x14F0, 0));             
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(CommodityDeed), CommodityDeed.GetSBSellValue());
                Add(typeof(VendorRentalContract), VendorRentalContract.GetSBSellValue());		
			}
		}
	}
}