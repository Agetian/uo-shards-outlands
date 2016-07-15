using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBBard: SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBBard() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Lute", typeof(Lute), Lute.GetSBPurchaseValue(), 50, 0x0EB3, 0));
                Add(new GenericBuyInfo("Drums", typeof(Drums), Drums.GetSBPurchaseValue(), 50, 0x0E9C, 0));
                Add(new GenericBuyInfo("Harp", typeof(Harp), Harp.GetSBPurchaseValue(), 50, 0x0EB1, 0));
                Add(new GenericBuyInfo("Tambourine", typeof(Tambourine), Tambourine.GetSBPurchaseValue(), 50, 0x0E9E, 0)); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(Lute), Lute.GetSBSellValue());
                Add(typeof(Drums), Drums.GetSBSellValue());
                Add(typeof(Harp), Harp.GetSBSellValue());
                Add(typeof(Tambourine), Tambourine.GetSBSellValue()); 
			} 
		} 
	} 
}