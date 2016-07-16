using System;
using System.Collections.Generic;
using Server.Items;


namespace Server.Mobiles
{
	public class SBProvisioner : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBProvisioner()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Arrow", typeof(Arrow), Arrow.GetSBPurchaseValue(), 500, 0xF3F, 0));
                Add(new GenericBuyInfo("Bolt", typeof(Bolt), Bolt.GetSBPurchaseValue(), 500, 0x1BFB, 0));
                Add(new GenericBuyInfo("Bandage", typeof(Bandage), Bandage.GetSBPurchaseValue(), 250, 0xE21, 0));

                Add(new GenericBuyInfo("Scissors", typeof(Scissors), Scissors.GetSBPurchaseValue(), 25, 0xF9F, 0));
                Add(new GenericBuyInfo("Sewing Kit", typeof(SewingKit), SewingKit.GetSBPurchaseValue(), 50, 0xF9D, 0));
                Add(new GenericBuyInfo("Dyes", typeof(Dyes), Dyes.GetSBPurchaseValue(), 25, 0xFA9, 0));
                Add(new GenericBuyInfo("Dye Tub", typeof(DyeTub), DyeTub.GetSBPurchaseValue(), 25, 0xFAB, 0)); 

                Add(new GenericBuyInfo("Lockpick", typeof(Lockpick), Lockpick.GetSBPurchaseValue(), 100, 0x14FC, 0));
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), Skillet.GetSBPurchaseValue(), 50, 0x97F, 0));
                Add(new GenericBuyInfo("Dagger", typeof(Dagger), Dagger.GetSBPurchaseValue(), 25, 0xF52, 0));

                Add(new GenericBuyInfo("Lantern", typeof(Lantern), Lantern.GetSBPurchaseValue(), 25, 0xA25, 0));
                Add(new GenericBuyInfo("Torch", typeof(Torch), Torch.GetSBPurchaseValue(), 25, 0xF6B, 0));
                Add(new GenericBuyInfo("Candle", typeof(Candle), Candle.GetSBPurchaseValue(), 25, 0xA28, 0));
                Add(new GenericBuyInfo("Bedroll", typeof(Bedroll), Bedroll.GetSBPurchaseValue(), 25, 0xA57, 0));
                Add(new GenericBuyInfo("Backpack", typeof(Backpack), Backpack.GetSBPurchaseValue(), 25, 0x9B2, 0));
                Add(new GenericBuyInfo("Bag", typeof(Bag), Bag.GetSBPurchaseValue(), 25, 0xE76, 0));
                Add(new GenericBuyInfo("Pouch", typeof(Pouch), Pouch.GetSBPurchaseValue(), 25, 0xE79, 0));
                Add(new GenericBuyInfo("Wooden Box", typeof(WoodenBox), WoodenBox.GetSBPurchaseValue(), 25, 0xE7D, 0));                
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add( typeof(Arrow), Arrow.GetSBSellValue());
                Add( typeof(Bolt), Bolt.GetSBSellValue());
                Add( typeof(Bandage), Bandage.GetSBSellValue());

                Add(typeof(Scissors), Scissors.GetSBSellValue());
                Add(typeof(SewingKit), SewingKit.GetSBSellValue());
                Add(typeof(Dyes), Dyes.GetSBSellValue());
                Add(typeof(DyeTub), DyeTub.GetSBSellValue());

                Add(typeof(Lockpick), Lockpick.GetSBSellValue());
                Add(typeof(Skillet), Skillet.GetSBSellValue());
                Add(typeof(Dagger), Dagger.GetSBSellValue());

                Add(typeof(Lantern), Lantern.GetSBSellValue());
                Add(typeof(Torch), Torch.GetSBSellValue());
                Add(typeof(Candle), Candle.GetSBSellValue());
                Add(typeof(Bedroll), Bedroll.GetSBSellValue());
                Add(typeof(Backpack), Backpack.GetSBSellValue());
                Add(typeof(Bag), Bag.GetSBSellValue());
                Add(typeof(Pouch), Pouch.GetSBSellValue());
                Add(typeof(WoodenBox), WoodenBox.GetSBSellValue());                		
			}
		}
	}
}
