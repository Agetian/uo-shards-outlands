using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBJewel: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBJewel()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Citrine", typeof(Citrine), Citrine.GetSBPurchaseValue(), 25, 0xF15, 0));
                Add(new GenericBuyInfo("Tourmaline", typeof(Tourmaline), Tourmaline.GetSBPurchaseValue(), 25, 0xF2D, 0));
                Add(new GenericBuyInfo("Amber", typeof(Amber), Amber.GetSBPurchaseValue(), 25, 0xF25, 0));
                Add(new GenericBuyInfo("Amethyst", typeof(Amethyst), Amethyst.GetSBPurchaseValue(), 25, 0xF16, 0));
                Add(new GenericBuyInfo("Ruby", typeof(Ruby), Ruby.GetSBPurchaseValue(), 25, 0xF13, 0));
                Add(new GenericBuyInfo("Sapphire", typeof(Sapphire), Sapphire.GetSBPurchaseValue(), 25, 0xF19, 0));
                Add(new GenericBuyInfo("Emerald", typeof(Emerald), Emerald.GetSBPurchaseValue(), 25, 0xF10, 0));
                Add(new GenericBuyInfo("Star Sapphire", typeof(StarSapphire), StarSapphire.GetSBPurchaseValue(), 25, 0xF21, 0));
                Add(new GenericBuyInfo("Diamond", typeof(Diamond), Diamond.GetSBPurchaseValue(), 25, 0xF26, 0));
               
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Amber), Amber.GetSBSellValue());
                Add(typeof(Amethyst), Amethyst.GetSBSellValue());
                Add(typeof(Citrine), Citrine.GetSBSellValue());
                Add(typeof(Diamond), Diamond.GetSBSellValue());
                Add(typeof(Emerald), Emerald.GetSBSellValue());
                Add(typeof(Ruby), Ruby.GetSBSellValue());
                Add(typeof(Sapphire), Sapphire.GetSBSellValue());
                Add(typeof(StarSapphire), StarSapphire.GetSBSellValue());
                Add(typeof(Tourmaline), Tourmaline.GetSBSellValue());
			}
		}
	}
}
