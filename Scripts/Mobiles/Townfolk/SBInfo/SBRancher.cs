
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBRancher : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBRancher()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new AnimalBuyInfo(1, "Pack Horse", typeof(PackHorse), PackHorse.GetSBPurchaseValue(), 10, 0x123, 0));
                Add(new AnimalBuyInfo(1, "Pack Horse", typeof(PackLlama), PackLlama.GetSBPurchaseValue(), 10, 0x124, 0));

                Add(new AnimalBuyInfo(1, "Horse", typeof(Horse), Horse.GetSBPurchaseValue(), 10, 0xC8, 0));

                Add(new GenericBuyInfo("Skillet", typeof(Skillet), Skillet.GetSBPurchaseValue(), 50, 0x1043, 0));
                Add(new BeverageBuyInfo("Pitcher of Milk", typeof(Pitcher), BeverageType.Milk, Pitcher.GetSBPurchaseValue(), 25, 0x9AD, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Skillet), Skillet.GetSBSellValue());
                Add(typeof(Pitcher), Pitcher.GetSBSellValue());
			}
		}
	}
}