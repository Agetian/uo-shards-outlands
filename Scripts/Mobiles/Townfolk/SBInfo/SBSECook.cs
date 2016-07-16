using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSECook: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBSECook()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{                
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), Skillet.GetSBPurchaseValue(), 50, 0x1043, 0)); 
                Add(new GenericBuyInfo("Rolling Pin", typeof(RollingPin), RollingPin.GetSBPurchaseValue(), 50, 0x1043, 0));

                Add(new GenericBuyInfo("Sack of Flour", typeof(SackOfFlour), SackOfFlour.GetSBPurchaseValue(), 500, 0x1039, 0));			
                Add(new BeverageBuyInfo("Pitcher of Water", typeof(Pitcher), BeverageType.Water, Pitcher.GetSBPurchaseValue(), 25, 0x1F9D, 0));				
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{               
                Add(typeof(Skillet), Skillet.GetSBSellValue());				
                Add(typeof(RollingPin), RollingPin.GetSBSellValue());

                Add(typeof(SackOfFlour), SackOfFlour.GetSBSellValue());
                Add(typeof(Pitcher), Pitcher.GetSBSellValue());
			} 
		} 
	}
}