using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBButcher : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBButcher() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), Skillet.GetSBPurchaseValue(), 50, 0x97F, 0));

                Add(new GenericBuyInfo("Butcher's Knife", typeof(ButcherKnife), ButcherKnife.GetSBPurchaseValue(), 25, 0x13F6, 0));
                Add(new GenericBuyInfo("Cleaver", typeof(Cleaver), Cleaver.GetSBPurchaseValue(), 25, 0xEC3, 0));
                Add(new GenericBuyInfo("Skinning Knife", typeof(SkinningKnife), SkinningKnife.GetSBPurchaseValue(), 25, 0xEC4, 0)); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(Skillet), Skillet.GetSBSellValue());

                Add(typeof(ButcherKnife), ButcherKnife.GetSBSellValue());
                Add(typeof(Cleaver), Cleaver.GetSBSellValue());
                Add(typeof(SkinningKnife), SkinningKnife.GetSBSellValue());
			} 
		} 
	} 
}