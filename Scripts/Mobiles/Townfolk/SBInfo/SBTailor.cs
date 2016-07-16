using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBTailor: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBTailor()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Bolt of Cloth", typeof(BoltOfCloth), BoltOfCloth.GetSBPurchaseValue(), 50, 0xf95, 0));

                Add(new GenericBuyInfo("Scissors", typeof(Scissors), Scissors.GetSBPurchaseValue(), 25, 0xF9F, 0));
                Add(new GenericBuyInfo("Sewing Kit", typeof(SewingKit), SewingKit.GetSBPurchaseValue(), 50, 0xF9D, 0));
                Add(new GenericBuyInfo("Dyes", typeof(Dyes), Dyes.GetSBPurchaseValue(), 25, 0xFA9, 0));
                Add(new GenericBuyInfo("Dye Tub", typeof(DyeTub), DyeTub.GetSBPurchaseValue(), 25, 0xFAB, 0));

                //Hats
                Add(new GenericBuyInfo("Skull Cap", typeof(SkullCap), SkullCap.GetSBPurchaseValue(), 25, 0x1544, 0));
                Add(new GenericBuyInfo("Bandana", typeof(Bandana), Bandana.GetSBPurchaseValue(), 25, 0x1540, 0));
                Add(new GenericBuyInfo("Floppy Hat", typeof(FloppyHat), FloppyHat.GetSBPurchaseValue(), 25, 0x1713, 0));
                Add(new GenericBuyInfo("Cap", typeof(Cap), Cap.GetSBPurchaseValue(), 25, 0x1715, 0));
                Add(new GenericBuyInfo("Wide Brim Hat", typeof(WideBrimHat), WideBrimHat.GetSBPurchaseValue(), 25, 0x1714, 0));
                Add(new GenericBuyInfo("Tall Straw Hat", typeof(TallStrawHat), TallStrawHat.GetSBPurchaseValue(), 25, 0x1716, 0));
                Add(new GenericBuyInfo("Bonnet", typeof(Bonnet), Bonnet.GetSBPurchaseValue(), 25, 0x1719, 0));
                Add(new GenericBuyInfo("Feathered Hat", typeof(FeatheredHat), FeatheredHat.GetSBPurchaseValue(), 25, 0x171A, 0));
                Add(new GenericBuyInfo("Tricorne Hat", typeof(TricorneHat), TricorneHat.GetSBPurchaseValue(), 25, 0x171B, 0));
                Add(new GenericBuyInfo("Jester Hat", typeof(JesterHat), JesterHat.GetSBPurchaseValue(), 25, 0x171C, 0));
                Add(new GenericBuyInfo("Wizards Hat", typeof(WizardsHat), WizardsHat.GetSBPurchaseValue(), 25, 0x1718, 0));
                
                //Shirts
                Add(new GenericBuyInfo("Doublet", typeof(Doublet), Doublet.GetSBPurchaseValue(), 25, 0x1F7B, 0));
                Add(new GenericBuyInfo("Shirt", typeof(Shirt), Shirt.GetSBPurchaseValue(), 25, 0x1517, 0));
                Add(new GenericBuyInfo("Fancy Shirt", typeof(FancyShirt), FancyShirt.GetSBPurchaseValue(), 25, 0x1EFD, 0));
                Add(new GenericBuyInfo("Tunic", typeof(Tunic), Tunic.GetSBPurchaseValue(), 25, 0x1FA1, 0));
                Add(new GenericBuyInfo("Surcoat", typeof(Surcoat), Surcoat.GetSBPurchaseValue(), 25, 0x1FFD, 0));
                Add(new GenericBuyInfo("Jester Suit", typeof(JesterSuit), JesterSuit.GetSBPurchaseValue(), 25, 0x1F9F, 0));
                Add(new GenericBuyInfo("Plain Dress", typeof(PlainDress), PlainDress.GetSBPurchaseValue(), 25, 0x1F01, 0));
                Add(new GenericBuyInfo("Fancy Dress", typeof(FancyDress), FancyDress.GetSBPurchaseValue(), 25, 0x1EFF, 0));
                Add(new GenericBuyInfo("Robe", typeof(Robe), Robe.GetSBPurchaseValue(), 25, 0x1F03, 0));

                //Pants
                Add(new GenericBuyInfo("Short Pants", typeof(ShortPants), ShortPants.GetSBPurchaseValue(), 25, 0x152E, 0));
                Add(new GenericBuyInfo("Long Pants", typeof(LongPants), LongPants.GetSBPurchaseValue(), 25, 0x1539, 0));
                Add(new GenericBuyInfo("Kilt", typeof(Kilt), Kilt.GetSBPurchaseValue(), 25, 0x1537, 0));
                Add(new GenericBuyInfo("Skirt", typeof(Skirt), Skirt.GetSBPurchaseValue(), 25, 0x1516, 0));

                Add(new GenericBuyInfo("Cloak", typeof(Cloak), Cloak.GetSBPurchaseValue(), 25, 0x1515, 0));
                Add(new GenericBuyInfo("Body Sash", typeof(BodySash), BodySash.GetSBPurchaseValue(), 25, 0x1541, 0));
                Add(new GenericBuyInfo("Half Apron", typeof(HalfApron), HalfApron.GetSBPurchaseValue(), 25, 0x153b, 0));
                Add(new GenericBuyInfo("Full Apron", typeof(FullApron), FullApron.GetSBPurchaseValue(), 25, 0x153d, 0));

                //Shoes
                Add(new GenericBuyInfo("Sandals", typeof(Sandals), Sandals.GetSBPurchaseValue(), 25, 0x170D, 0));
                Add(new GenericBuyInfo("Shoes", typeof(Shoes), Shoes.GetSBPurchaseValue(), 25, 0x170F, 0));
                Add(new GenericBuyInfo("Boots", typeof(Boots), Boots.GetSBPurchaseValue(), 25, 0x170B, 0));
                Add(new GenericBuyInfo("Thigh Boots", typeof(ThighBoots), ThighBoots.GetSBPurchaseValue(), 25, 0x1711, 0));

                //TEST: FIX
				//Add( new GenericBuyInfo( typeof( SpoolOfThread ), 18, 20, 0xFA0, 0 ) );
				//Add( new GenericBuyInfo( typeof( Flax ), 156, 20, 0x1A9C, 0 ) );
				//Add( new GenericBuyInfo( typeof( Cotton ), 156, 20, 0xDF9, 0 ) );
				//Add( new GenericBuyInfo( typeof( Wool ), 78, 20, 0xDF8, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(BoltOfCloth), BoltOfCloth.GetSBSellValue());

                Add(typeof(Scissors), Scissors.GetSBSellValue());
                Add(typeof(SewingKit), SewingKit.GetSBSellValue());
                Add(typeof(Dyes), Dyes.GetSBSellValue());
                Add(typeof(DyeTub), DyeTub.GetSBSellValue());

                Add(typeof(SkullCap), SkullCap.GetSBSellValue());
                Add(typeof(Bandana), Bandana.GetSBSellValue());
                Add(typeof(FloppyHat), FloppyHat.GetSBSellValue());
                Add(typeof(Cap), Cap.GetSBSellValue());
                Add(typeof(WideBrimHat), WideBrimHat.GetSBSellValue());
                Add(typeof(TallStrawHat), TallStrawHat.GetSBSellValue());
                Add(typeof(Bonnet), Bonnet.GetSBSellValue());
                Add(typeof(FeatheredHat), FeatheredHat.GetSBSellValue());
                Add(typeof(TricorneHat), TricorneHat.GetSBSellValue());
                Add(typeof(JesterHat), JesterHat.GetSBSellValue());
                Add(typeof(WizardsHat), WizardsHat.GetSBSellValue());

                Add(typeof(Doublet), Doublet.GetSBSellValue());
                Add(typeof(Shirt), Shirt.GetSBSellValue());
                Add(typeof(FancyShirt), FancyShirt.GetSBSellValue());
                Add(typeof(Tunic), Tunic.GetSBSellValue());
                Add(typeof(Surcoat), Surcoat.GetSBSellValue());
                Add(typeof(JesterSuit), JesterSuit.GetSBSellValue());
                Add(typeof(PlainDress), PlainDress.GetSBSellValue());
                Add(typeof(FancyDress), FancyDress.GetSBSellValue());
                Add(typeof(Robe), Robe.GetSBSellValue());

                Add(typeof(ShortPants), ShortPants.GetSBSellValue());
                Add(typeof(LongPants), LongPants.GetSBSellValue());
                Add(typeof(Kilt), Kilt.GetSBSellValue());
                Add(typeof(Skirt), Skirt.GetSBSellValue());

                Add(typeof(Cloak), Cloak.GetSBSellValue());
                Add(typeof(BodySash), BodySash.GetSBSellValue());
                Add(typeof(HalfApron), HalfApron.GetSBSellValue());
                Add(typeof(FullApron), FullApron.GetSBSellValue());

                Add(typeof(Sandals), Sandals.GetSBSellValue());
                Add(typeof(Shoes), Shoes.GetSBSellValue());
                Add(typeof(Boots), Boots.GetSBSellValue());
                Add(typeof(ThighBoots), ThighBoots.GetSBSellValue());
			}
		}
	}
}
