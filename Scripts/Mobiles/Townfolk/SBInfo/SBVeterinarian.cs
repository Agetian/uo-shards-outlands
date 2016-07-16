
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBVeterinarian : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBVeterinarian()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Bandage", typeof(Bandage), Bandage.GetSBPurchaseValue(), 250, 0xE21, 0));

                Add(new AnimalBuyInfo(1, "Rat", typeof(Rat), Rat.GetSBPurchaseValue(), 10, 0xEE, 0));
                Add(new AnimalBuyInfo(1, "Rabbit", typeof(Rabbit), Rabbit.GetSBPurchaseValue(), 10, 0xCD, 0));
                Add(new AnimalBuyInfo(1, "Cat", typeof(Cat), Cat.GetSBPurchaseValue(), 10, 0xC9, 0));
                Add(new AnimalBuyInfo(1, "Dog", typeof(Dog), Dog.GetSBPurchaseValue(), 10, 0xD9, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Bandage), Bandage.GetSBSellValue());
			}
		}
	}
}