
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAnimalTrainer : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAnimalTrainer()
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

                Add(new AnimalBuyInfo(1, "Rat", typeof(Rat), Rat.GetSBPurchaseValue(), 10, 0xEE, 0));
                Add(new AnimalBuyInfo(1, "Rabbit", typeof(Rabbit), Rabbit.GetSBPurchaseValue(), 10, 0xCD, 0));
                Add(new AnimalBuyInfo(1, "Cat", typeof(Cat), Cat.GetSBPurchaseValue(), 10, 0xC9, 0));
                Add(new AnimalBuyInfo(1, "Dog", typeof(Dog), Dog.GetSBPurchaseValue(), 10, 0xD9, 0));
                Add(new AnimalBuyInfo(1, "Eagle", typeof(Eagle), Eagle.GetSBPurchaseValue(), 10, 0x5, 0));
                Add(new AnimalBuyInfo(1, "Horse", typeof(Horse), Horse.GetSBPurchaseValue(), 10, 0xC8, 0));
                Add(new AnimalBuyInfo(1, "Panther", typeof(Panther), Panther.GetSBPurchaseValue(), 10, 0xD6, 0));
                Add(new AnimalBuyInfo(1, "Timber Wolf", typeof(TimberWolf), TimberWolf.GetSBPurchaseValue(), 10, 0xE1, 0));
                Add(new AnimalBuyInfo(1, "Brown Bear", typeof(BrownBear), BrownBear.GetSBPurchaseValue(), 10, 0xA7, 0));
                Add(new AnimalBuyInfo(1, "Grizzly Bear", typeof(GrizzlyBear), GrizzlyBear.GetSBPurchaseValue(), 10, 0xD4, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			}
		}
	}
}
