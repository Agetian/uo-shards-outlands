using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBLeatherWorker: SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBLeatherWorker() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Skinning Knife", typeof(SkinningKnife), SkinningKnife.GetSBPurchaseValue(), 50, 0xEC4, 0));

                Add(new GenericBuyInfo("Leather Cap", typeof(LeatherCap), LeatherCap.GetSBPurchaseValue(), 25, 7610, 0));
                Add(new GenericBuyInfo("Leather Gorget", typeof(LeatherGorget), LeatherGorget.GetSBPurchaseValue(), 25, 5063, 0));
                Add(new GenericBuyInfo("Leather Arms", typeof(LeatherArms), LeatherArms.GetSBPurchaseValue(), 25, 5061, 0));
                Add(new GenericBuyInfo("Leather Gloves", typeof(LeatherGloves), LeatherGloves.GetSBPurchaseValue(), 25, 5070, 0));
                Add(new GenericBuyInfo("Leather Chest", typeof(LeatherChest), LeatherChest.GetSBPurchaseValue(), 25, 5075, 0));
                Add(new GenericBuyInfo("Female Leather Chest", typeof(FemaleLeatherChest), FemaleLeatherChest.GetSBPurchaseValue(), 25, 7175, 0));
                Add(new GenericBuyInfo("Leather Bustier", typeof(LeatherBustier), LeatherBustier.GetSBPurchaseValue(), 25, 7179, 0));
                Add(new GenericBuyInfo("Leather Legs", typeof(LeatherLegs), LeatherLegs.GetSBPurchaseValue(), 25, 5074, 0));
                Add(new GenericBuyInfo("Leather Skirt", typeof(LeatherSkirt), LeatherSkirt.GetSBPurchaseValue(), 25, 7177, 0));
                Add(new GenericBuyInfo("Leather Shorts", typeof(LeatherShorts), LeatherShorts.GetSBPurchaseValue(), 25, 7169, 0));

                Add(new GenericBuyInfo("Studded Cap", typeof(StuddedCap), StuddedCap.GetSBPurchaseValue(), 25, 7610, 1507));
                Add(new GenericBuyInfo("Studded Gorget", typeof(StuddedGorget), StuddedGorget.GetSBPurchaseValue(), 25, 5078, 0));
                Add(new GenericBuyInfo("Studded Arms", typeof(StuddedArms), StuddedArms.GetSBPurchaseValue(), 25, 5076, 0));
                Add(new GenericBuyInfo("Studded Gloves", typeof(StuddedGloves), StuddedGloves.GetSBPurchaseValue(), 25, 5085, 0));
                Add(new GenericBuyInfo("Studded Chest", typeof(StuddedChest), StuddedChest.GetSBPurchaseValue(), 25, 5090, 0));
                Add(new GenericBuyInfo("Female Studded Chest", typeof(FemaleStuddedChest), FemaleStuddedChest.GetSBPurchaseValue(), 25, 7170, 0));
                Add(new GenericBuyInfo("Studded Bustier", typeof(StuddedBustier), StuddedBustier.GetSBPurchaseValue(), 25, 7181, 0));
                Add(new GenericBuyInfo("Studded Legs", typeof(StuddedLegs), StuddedLegs.GetSBPurchaseValue(), 25, 5089, 0));

                Add(new GenericBuyInfo("Shoes", typeof(Shoes), Shoes.GetSBPurchaseValue(), 25, 0x170F, 0));
                Add(new GenericBuyInfo("Sandals", typeof(Sandals), Sandals.GetSBPurchaseValue(), 25, 0x170D, 0));
                Add(new GenericBuyInfo("Boots", typeof(Boots), Boots.GetSBPurchaseValue(), 25, 0x170B, 0));
                Add(new GenericBuyInfo("Thigh Boots", typeof(ThighBoots), ThighBoots.GetSBPurchaseValue(), 25, 0x1711, 0));
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(Hide), Hide.GetSBSellValue());

                Add(typeof(SkinningKnife), SkinningKnife.GetSBSellValue());

                Add(typeof(LeatherCap), LeatherCap.GetSBSellValue());
                Add(typeof(LeatherGorget), LeatherGorget.GetSBSellValue());
                Add(typeof(LeatherArms), LeatherArms.GetSBSellValue());
                Add(typeof(LeatherGloves), LeatherGloves.GetSBSellValue());
                Add(typeof(LeatherChest), LeatherChest.GetSBSellValue());
                Add(typeof(FemaleLeatherChest), FemaleLeatherChest.GetSBSellValue());
                Add(typeof(LeatherBustier), LeatherBustier.GetSBSellValue());
                Add(typeof(LeatherLegs), LeatherLegs.GetSBSellValue());
                Add(typeof(LeatherSkirt), LeatherSkirt.GetSBSellValue());
                Add(typeof(LeatherShorts), LeatherShorts.GetSBSellValue());

                Add(typeof(StuddedCap), StuddedCap.GetSBSellValue());
                Add(typeof(StuddedGorget), StuddedGorget.GetSBSellValue());
                Add(typeof(StuddedArms), StuddedArms.GetSBSellValue());
                Add(typeof(StuddedGloves), StuddedGloves.GetSBSellValue());
                Add(typeof(StuddedChest), StuddedChest.GetSBSellValue());
                Add(typeof(FemaleStuddedChest), FemaleStuddedChest.GetSBSellValue());
                Add(typeof(StuddedBustier), StuddedBustier.GetSBSellValue());
                Add(typeof(StuddedLegs), StuddedLegs.GetSBSellValue());

                Add(typeof(Shoes), Shoes.GetSBSellValue());
                Add(typeof(Sandals), Sandals.GetSBSellValue());
                Add(typeof(Boots), Boots.GetSBSellValue());
                Add(typeof(ThighBoots), ThighBoots.GetSBSellValue());
			} 
		} 
	} 
} 
