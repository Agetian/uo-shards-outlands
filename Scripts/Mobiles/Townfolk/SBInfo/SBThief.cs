using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBThief : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBThief()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Lockpick", typeof(Lockpick), Lockpick.GetSBPurchaseValue(), 250, 0x14FC, 0));

                Add(new GenericBuyInfo("Lantern", typeof(Lantern), Lantern.GetSBPurchaseValue(), 25, 0xA25, 0));
                Add(new GenericBuyInfo("Torch", typeof(Torch), Torch.GetSBPurchaseValue(), 25, 0xF6B, 0));
                Add(new GenericBuyInfo("Candle", typeof(Candle), Candle.GetSBPurchaseValue(), 25, 0xA28, 0));
                Add(new GenericBuyInfo("Bedroll", typeof(Bedroll), Bedroll.GetSBPurchaseValue(), 25, 0xA57, 0));
                Add(new GenericBuyInfo("Backpack", typeof(Backpack), Backpack.GetSBPurchaseValue(), 25, 0x9B2, 0));
                Add(new GenericBuyInfo("Bag", typeof(Bag), Bag.GetSBPurchaseValue(), 25, 0xE76, 0));
                Add(new GenericBuyInfo("Pouch", typeof(Pouch), Pouch.GetSBPurchaseValue(), 25, 0xE79, 0));
                Add(new GenericBuyInfo("Wooden Box", typeof(WoodenBox), WoodenBox.GetSBPurchaseValue(), 25, 0xE7D, 0));

                Add(new GenericBuyInfo("Hair Dye", typeof(HairDye), HairDye.GetSBPurchaseValue(), 10, 0xEFF, 0)); 
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Lockpick), Lockpick.GetSBSellValue());

                Add(typeof(Lantern), Lantern.GetSBSellValue());
                Add(typeof(Torch), Torch.GetSBSellValue());
                Add(typeof(Candle), Candle.GetSBSellValue());
                Add(typeof(Bedroll), Bedroll.GetSBSellValue());
                Add(typeof(Backpack), Backpack.GetSBSellValue());
                Add(typeof(Bag), Bag.GetSBSellValue());
                Add(typeof(Pouch), Pouch.GetSBSellValue());
                Add(typeof(WoodenBox), WoodenBox.GetSBSellValue());

                Add(typeof(HairDye), HairDye.GetSBSellValue()); 
			}
		}
	}
}