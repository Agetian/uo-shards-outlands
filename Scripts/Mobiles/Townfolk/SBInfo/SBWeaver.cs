using System;
using System.Collections.Generic; 
using Server.Items;

namespace Server.Mobiles
{
	public class SBWeaver: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBWeaver()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Bolt of Cloth", typeof(BoltOfCloth), Bandage.GetSBPurchaseValue(), 50, 0xf95, 0));

                Add(new GenericBuyInfo("Scissors", typeof(Scissors), Scissors.GetSBPurchaseValue(), 25, 0xF9F, 0));
                Add(new GenericBuyInfo("Sewing Kit", typeof(SewingKit), SewingKit.GetSBPurchaseValue(), 50, 0xF9D, 0));
                Add(new GenericBuyInfo("Dyes", typeof(Dyes), Dyes.GetSBPurchaseValue(), 25, 0xFA9, 0));
                Add(new GenericBuyInfo("Dye Tub", typeof(DyeTub), DyeTub.GetSBPurchaseValue(), 25, 0xFAB, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(BoltOfCloth), BoltOfCloth.GetSBSellValue());

                Add(typeof(Scissors), Scissors.GetSBSellValue());
                Add(typeof(SewingKit), SewingKit.GetSBSellValue());
                Add(typeof(Dyes), Dyes.GetSBSellValue());
                Add(typeof(DyeTub), DyeTub.GetSBSellValue());
			}
		}
	}
}
