using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBCobbler : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBCobbler() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Sandals", typeof(Sandals), Sandals.GetSBPurchaseValue(), 25, 0x170d, 0));
                Add(new GenericBuyInfo("Shoes", typeof(Shoes), Shoes.GetSBPurchaseValue(), 25, 0x170f, 0));
                Add(new GenericBuyInfo("Boots", typeof(Boots), Boots.GetSBPurchaseValue(), 25, 0x170b, 0));
                Add(new GenericBuyInfo("Thigh Boots", typeof(ThighBoots), ThighBoots.GetSBPurchaseValue(), 25, 0x1711, 0));                
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(Shoes), Shoes.GetSBSellValue());
                Add(typeof(Boots), Boots.GetSBSellValue());
                Add(typeof(ThighBoots), ThighBoots.GetSBSellValue());
                Add(typeof(Sandals), Sandals.GetSBSellValue()); 
			} 
		} 
	} 
}