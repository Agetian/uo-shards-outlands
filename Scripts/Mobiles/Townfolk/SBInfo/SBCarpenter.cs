using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBCarpenter: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBCarpenter()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Hatchet", typeof(Hatchet), Hatchet.GetSBPurchaseValue(), 50, 0xF43, 0));
                Add(new GenericBuyInfo("Saw", typeof(Saw), Saw.GetSBPurchaseValue(), 50, 0x1034, 0));                

                Add(new GenericBuyInfo("Club", typeof(Club), Club.GetSBPurchaseValue(), 25, 0x13B4, 0));
                Add(new GenericBuyInfo("Shepherd's Crook", typeof(ShepherdsCrook), ShepherdsCrook.GetSBPurchaseValue(), 25, 0xE81, 0));
                Add(new GenericBuyInfo("Quarter Staff", typeof(QuarterStaff), QuarterStaff.GetSBPurchaseValue(), 25, 0xE89, 0));
                Add(new GenericBuyInfo("Gnarled Staff", typeof(GnarledStaff), GnarledStaff.GetSBPurchaseValue(), 25, 0x13F8, 0));
                Add(new GenericBuyInfo("Black Staff", typeof(BlackStaff), BlackStaff.GetSBPurchaseValue(), 25, 0xDF0, 0));

                Add(new GenericBuyInfo("Wooden Shield", typeof(WoodenShield), WoodenShield.GetSBPurchaseValue(), 25, 7034, 0));
                Add(new GenericBuyInfo("Wooden Kite Shield", typeof(WoodenKiteShield), WoodenKiteShield.GetSBPurchaseValue(), 25, 7033, 0));

                Add(new GenericBuyInfo("Lute", typeof(Lute), Lute.GetSBPurchaseValue(), 50, 0x0EB3, 0));
                Add(new GenericBuyInfo("Drums", typeof(Drums), Drums.GetSBPurchaseValue(), 50, 0x0E9C, 0));
                Add(new GenericBuyInfo("Harp", typeof(Harp), Harp.GetSBPurchaseValue(), 50, 0x0EB1, 0));
                Add(new GenericBuyInfo("Tambourine", typeof(Tambourine), Tambourine.GetSBPurchaseValue(), 50, 0x0E9E, 0)); 

                Add(new GenericBuyInfo("Scorp", typeof(Scorp), Scorp.GetSBPurchaseValue(), 50, 0x10E7, 0));
                Add(new GenericBuyInfo("Smoothing Plane", typeof(SmoothingPlane), SmoothingPlane.GetSBPurchaseValue(), 50, 0x1032, 0));
                Add(new GenericBuyInfo("Drawing Knife", typeof(DrawKnife), DrawKnife.GetSBPurchaseValue(), 50, 0x10E4, 0));
                Add(new GenericBuyInfo("Froe", typeof(Froe), Froe.GetSBPurchaseValue(), 50, 0x10E5, 0));
                Add(new GenericBuyInfo("Hammer", typeof(Hammer), Hammer.GetSBPurchaseValue(), 50, 0x102A, 0));
                Add(new GenericBuyInfo("Inshave", typeof(Inshave), Inshave.GetSBPurchaseValue(), 50, 0x10E6, 0));
                Add(new GenericBuyInfo("Jointing Plane", typeof(JointingPlane), JointingPlane.GetSBPurchaseValue(), 50, 0x1030, 0));
                Add(new GenericBuyInfo("Wooden Plane", typeof(WoodenPlane), WoodenPlane.GetSBPurchaseValue(), 50, 0x102C, 0));
                Add(new GenericBuyInfo("Dovetail Saw", typeof(DovetailSaw), DovetailSaw.GetSBPurchaseValue(), 50, 0x1028, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Hatchet), Hatchet.GetSBSellValue());
                Add(typeof(Saw), Saw.GetSBSellValue());

                Add(typeof(Scorp), Scorp.GetSBSellValue());
                Add(typeof(SmoothingPlane), SmoothingPlane.GetSBSellValue());
                Add(typeof(DrawKnife), DrawKnife.GetSBSellValue());
                Add(typeof(Froe), Froe.GetSBSellValue());
                Add(typeof(Hammer), Hammer.GetSBSellValue());
                Add(typeof(Inshave), Inshave.GetSBSellValue());
                Add(typeof(JointingPlane), JointingPlane.GetSBSellValue());
                Add(typeof(WoodenPlane), WoodenPlane.GetSBSellValue());
                Add(typeof(DovetailSaw), DovetailSaw.GetSBSellValue());

                Add(typeof(Lute), Lute.GetSBSellValue());
                Add(typeof(Drums), Drums.GetSBSellValue());
                Add(typeof(Harp), Harp.GetSBSellValue());
                Add(typeof(Tambourine), Tambourine.GetSBSellValue()); 

                Add(typeof(Club), Club.GetSBSellValue());
                Add(typeof(ShepherdsCrook), ShepherdsCrook.GetSBSellValue());
                Add(typeof(QuarterStaff), QuarterStaff.GetSBSellValue());
                Add(typeof(GnarledStaff), GnarledStaff.GetSBSellValue());
                Add(typeof(BlackStaff), BlackStaff.GetSBSellValue());

                Add(typeof(WoodenShield), WoodenShield.GetSBSellValue());
                Add(typeof(WoodenKiteShield), WoodenKiteShield.GetSBSellValue());

                /*
				Add( typeof( WoodenBox ), 7 );
				Add( typeof( SmallCrate ), 5 );
				Add( typeof( MediumCrate ), 6 );
				Add( typeof( LargeCrate ), 7 );
				Add( typeof( WoodenChest ), 15 );
				
				Add( typeof( LargeTable ), 10 );
				Add( typeof( Nightstand ), 7 );
				Add( typeof( YewWoodTable ), 10 );
				Add( typeof( WritingTable ), 9 );

				Add( typeof( Throne ), 24 );
				Add( typeof( WoodenThrone ), 6 );
				Add( typeof( Stool ), 6 );
				Add( typeof( FootStool ), 6 );

				Add( typeof( FancyWoodenChairCushion ), 12 );
				Add( typeof( CushionedWoodenChair ), 10 );
				Add( typeof( WoodenChair ), 8 );
				Add( typeof( BambooChair ), 6 );
				Add( typeof( WoodenBench ), 6 );
                */				
			}
		}
	}
}
