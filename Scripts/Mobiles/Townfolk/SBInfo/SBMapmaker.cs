using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMapmaker : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMapmaker()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                //TEST: Determine Map Prices
				//for ( int i = 0; i < PresetMapEntry.Table.Length; ++i )
					//Add( new PresetMapBuyInfo( PresetMapEntry.Table[i], Utility.RandomMinMax( 7, 10 ), 20 ) );
              
                Add(new GenericBuyInfo("Blank Map", typeof(BlankMap), BlankMap.GetSBPurchaseValue(), 500, 0x14EC, 0));
                Add(new GenericBuyInfo("Mapmaker's Pen", typeof(MapmakersPen), MapmakersPen.GetSBPurchaseValue(), 50, 0x0FBF, 0));
                
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{               
                Add(typeof(BlankMap), BlankMap.GetSBSellValue());
                Add(typeof(MapmakersPen), MapmakersPen.GetSBSellValue());              

				//Add( typeof( LocalMap ), 6 );
				//Add( typeof( CityMap ), 8 );
				//Add( typeof( WorldMap ), 12 );				
			}
		}
	}
}