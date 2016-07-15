using System; 
using System.Collections.Generic; 
using Server.Items;
using Server.Custom;

namespace Server.Mobiles 
{ 
	public class SBFisherman : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBFisherman() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Fishing Pole", typeof(FishingPole), FishingPole.GetSBPurchaseValue(), 50, 0xDC0, 0));
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(FishingPole), FishingPole.GetSBSellValue());
                //Add(typeof(FishCommissionCompletedDeed), FishCommissionCompletedDeed.GetSBSellValue());
			} 
		} 
	} 
}