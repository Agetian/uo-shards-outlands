using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMiner: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMiner()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Pickaxe", typeof(Pickaxe), Pickaxe.GetSBPurchaseValue(), 50, 0xE86, 0));
                Add(new GenericBuyInfo("Shovel", typeof(Shovel), Shovel.GetSBPurchaseValue(), 50, 0xF39, 0));

                Add(new AnimalBuyInfo(1, "Pack Horse", typeof(PackHorse), PackHorse.GetSBPurchaseValue(), 10, 0x123, 0));
                Add(new AnimalBuyInfo(1, "Pack Horse", typeof(PackLlama), PackLlama.GetSBPurchaseValue(), 10, 0x124, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Pickaxe), Pickaxe.GetSBSellValue());
                Add(typeof(Shovel), Shovel.GetSBSellValue());
			}
		}
	}
}