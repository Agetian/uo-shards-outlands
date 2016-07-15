using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBHealer : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHealer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Bandage", typeof(Bandage), Bandage.GetSBPurchaseValue(), 250, 0xE21, 0));

                Add(new GenericBuyInfo("Lesser Heal Potion", typeof(LesserHealPotion), LesserHealPotion.GetSBPurchaseValue(), 50, 0xF0C, 0));
                Add(new GenericBuyInfo("Lesser Cure Potion", typeof(LesserCurePotion), LesserCurePotion.GetSBPurchaseValue(), 50, 0xF07, 0));
                Add(new GenericBuyInfo("Refresh Potion", typeof(RefreshPotion), RefreshPotion.GetSBPurchaseValue(), 50, 0xF0B, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Bandage), Bandage.GetSBSellValue());

                Add(typeof(LesserHealPotion), LesserHealPotion.GetSBSellValue());
                Add(typeof(LesserCurePotion), LesserCurePotion.GetSBSellValue());
                Add(typeof(RefreshPotion), RefreshPotion.GetSBSellValue());
			}
		}
	}
}