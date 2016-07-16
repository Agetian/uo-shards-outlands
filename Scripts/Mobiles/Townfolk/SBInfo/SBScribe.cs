using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBScribe: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBScribe()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Recall Rune", typeof(RecallRune), RecallRune.GetSBPurchaseValue(), 50, 0x1f14, 0));
                Add(new GenericBuyInfo("Spellbook", typeof(Spellbook), Spellbook.GetSBPurchaseValue(), 25, 0xEFA, 0));
                Add(new GenericBuyInfo("Blank Scroll", typeof(BlankScroll), BlankScroll.GetSBPurchaseValue(), 500, 0x0E34, 0));
                Add(new GenericBuyInfo("Scribe's Pen", typeof(ScribesPen), ScribesPen.GetSBPurchaseValue(), 50, 0xFBF, 0));

                Add(new GenericBuyInfo("Book", typeof(RedBook), RedBook.GetSBPurchaseValue(), 50, 0xFF1, 0));
                Add(new GenericBuyInfo("Book", typeof(BrownBook), BrownBook.GetSBPurchaseValue(), 50, 0xFEF, 0));
                Add(new GenericBuyInfo("Book", typeof(TanBook), TanBook.GetSBPurchaseValue(), 50, 0xFF0, 0));
                Add(new GenericBuyInfo("Book", typeof(BlueBook), BlueBook.GetSBPurchaseValue(), 50, 0xFF2, 0));
                
                //TEST: DETERMINE SCROLLS

				//Add( new GenericBuyInfo( typeof( BladeSpiritsScroll ), 350, 5, 0x1F4D, 0 ) );
				Add( new GenericBuyInfo( typeof( IncognitoScroll ), 450, 5, 0x1F4F, 0 ) );
				Add( new GenericBuyInfo( typeof( MagicReflectScroll ), 500, 5, 0x1F50, 0 ) );
				Add( new GenericBuyInfo( typeof( MindBlastScroll ), 350, 5, 0x1F51, 0 ) );
				Add( new GenericBuyInfo( typeof( ParalyzeScroll ), 550, 5, 0x1F52, 0 ) );
				Add( new GenericBuyInfo( typeof( PoisonFieldScroll ), 450, 5, 0x1F53, 0 ) );
				Add( new GenericBuyInfo( typeof( SummonCreatureScroll ), 450, 5, 0x1F54, 0 ) );
				Add( new GenericBuyInfo( typeof( DispelScroll ), 800, 4, 0x1F55, 0 ) );
				Add( new GenericBuyInfo( typeof( EnergyBoltScroll ), 900, 4, 0x1F56, 0 ) );
				Add( new GenericBuyInfo( typeof( ExplosionScroll ), 1050, 4, 0x1F57, 0 ) );
				Add( new GenericBuyInfo( typeof( InvisibilityScroll ), 900, 4, 0x1F58, 0 ) );
				Add( new GenericBuyInfo( typeof( MarkScroll ), 600, 4, 0x1F59, 0 ) );
				Add( new GenericBuyInfo( typeof( MassCurseScroll ), 700, 4, 0x1F5A, 0 ) );
				Add( new GenericBuyInfo( typeof( ParalyzeFieldScroll ), 700, 4, 0x1F5B, 0 ) );
				Add( new GenericBuyInfo( typeof( RevealScroll ), 800, 4, 0x1F5C, 0 ) );
				//Add( new GenericBuyInfo( typeof( ChainLightningScroll ), 1500, 3, 0x1F5D, 0 ) );
				//Add( new GenericBuyInfo( typeof( EnergyFieldScroll ),1500, 3, 0x1F5E, 0 ) );
				//Add( new GenericBuyInfo( typeof( FlamestrikeScroll ), 1750, 3, 0x1F5F, 0 ) );
				//Add( new GenericBuyInfo( typeof( GateTravelScroll ), 1950, 3, 0x1F60, 0 ) );
				//Add( new GenericBuyInfo( typeof( ManaVampireScroll ), 1550, 3, 0x1F61, 0 ) );
				//Add( new GenericBuyInfo( typeof( MassDispelScroll ), 1500, 3, 0x1F62, 0 ) );
				//Add( new GenericBuyInfo( typeof( MeteorSwarmScroll ), 1050, 3, 0x1F63, 0 ) );
				//Add( new GenericBuyInfo( typeof( PolymorphScroll ), 1000, 3, 0x1F64, 0 ) );
				//Add( new GenericBuyInfo( typeof( EarthquakeScroll ), 5500, 2, 0x1F65, 0 ) );
				//Add( new GenericBuyInfo( typeof( EnergyVortexScroll ), 2500, 2, 0x1F66, 0 ) );
				//Add( new GenericBuyInfo( typeof( ResurrectionScroll ), 6575, 2, 0x1F67, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonAirElementalScroll ), 2000, 2, 0x1F68, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonDaemonScroll ), 2500, 2, 0x1F69, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonEarthElementalScroll ), 2000, 2, 0x1F6A, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonFireElementalScroll ), 2000, 2, 0x1F6B, 0 ) );
				//Add( new GenericBuyInfo( typeof( SummonWaterElementalScroll ), 2000, 2, 0x1F6C, 0 ) );
				//Add( new GenericBuyInfo( typeof( DispelFieldScroll ), 500, 5, 0x1F4E, 0 ) );				
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(RecallRune), RecallRune.GetSBSellValue());
                Add(typeof(BlankScroll), BlankScroll.GetSBSellValue());
                Add(typeof(Spellbook), Spellbook.GetSBSellValue());
                Add(typeof(ScribesPen), ScribesPen.GetSBSellValue());

                Add(typeof(BrownBook), BrownBook.GetSBSellValue());
                Add(typeof(TanBook), TanBook.GetSBSellValue());
                Add(typeof(BlueBook), BlueBook.GetSBSellValue());
                Add(typeof(RedBook), RedBook.GetSBSellValue());
			}
		}
	}
}