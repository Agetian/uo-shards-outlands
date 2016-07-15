using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBArmorer : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBArmorer() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
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

                Add(new GenericBuyInfo("Ringmail Helm", typeof(RingmailHelm), RingmailHelm.GetSBPurchaseValue(), 25, 5131, 0));
                Add(new GenericBuyInfo("Ringmail Gorget", typeof(RingmailGorget), RingmailGorget.GetSBPurchaseValue(), 25, 5078, 1812));
                Add(new GenericBuyInfo("Ringmail Arms", typeof(RingmailArms), RingmailArms.GetSBPurchaseValue(), 25, 0x13EE, 0));
                Add(new GenericBuyInfo("Ringmail Gloves", typeof(RingmailGloves), RingmailGloves.GetSBPurchaseValue(), 25, 0x13eb, 0));
                Add(new GenericBuyInfo("Ringmail Chest", typeof(RingmailChest), RingmailChest.GetSBPurchaseValue(), 25, 0x13ec, 0));
                Add(new GenericBuyInfo("Ringmail Legs", typeof(RingmailLegs), RingmailLegs.GetSBPurchaseValue(), 25, 0x13F0, 0));

                Add(new GenericBuyInfo("Chainmail Coif", typeof(ChainmailCoif), ChainmailCoif.GetSBPurchaseValue(), 25, 0x13BB, 0));
                Add(new GenericBuyInfo("Chainmail Gorget", typeof(ChainmailGorget), ChainmailGorget.GetSBPurchaseValue(), 25, 5063, 2500));
                Add(new GenericBuyInfo("Chainmail Arms", typeof(ChainmailArms), ChainmailArms.GetSBPurchaseValue(), 25, 5103, 2500));
                Add(new GenericBuyInfo("Chainmail Gloves", typeof(ChainmailGloves), ChainmailGloves.GetSBPurchaseValue(), 25, 5106, 2500));
                Add(new GenericBuyInfo("Chainmail Chest", typeof(ChainmailChest), ChainmailChest.GetSBPurchaseValue(), 25, 0x13BF, 0));
                Add(new GenericBuyInfo("Chainmail Legs", typeof(ChainmailLegs), ChainmailLegs.GetSBPurchaseValue(), 25, 0x13BE, 0));

                Add(new GenericBuyInfo("Platemail Helm", typeof(PlateHelm), PlateHelm.GetSBPurchaseValue(), 25, 0x1412, 0));
                Add(new GenericBuyInfo("Platemail Gorget", typeof(PlateGorget), PlateGorget.GetSBPurchaseValue(), 25, 0x1413, 0));
                Add(new GenericBuyInfo("Platemail Arms", typeof(PlateArms), PlateArms.GetSBPurchaseValue(), 25, 0x1410, 0));
                Add(new GenericBuyInfo("Platemail Gloves", typeof(PlateGloves), PlateGloves.GetSBPurchaseValue(), 25, 0x1414, 0));
                Add(new GenericBuyInfo("Platemail PlateChest", typeof(PlateChest), PlateChest.GetSBPurchaseValue(), 25, 0x1415, 0));
                Add(new GenericBuyInfo("Female Plate Chest", typeof(FemalePlateChest), FemalePlateChest.GetSBPurchaseValue(), 25, 7173, 0));
                Add(new GenericBuyInfo("Platemail Legs", typeof(PlateLegs), PlateLegs.GetSBPurchaseValue(), 25, 0x1411, 0));

                Add(new GenericBuyInfo("Bascinet", typeof(Bascinet), Bascinet.GetSBPurchaseValue(), 25, 5132, 0));
                Add(new GenericBuyInfo("Close Helm", typeof(CloseHelm), CloseHelm.GetSBPurchaseValue(), 25, 5129, 0));
                Add(new GenericBuyInfo("Norse Helm", typeof(NorseHelm), NorseHelm.GetSBPurchaseValue(), 25, 5135, 0));

                Add(new GenericBuyInfo("Buckler", typeof(Buckler), Buckler.GetSBPurchaseValue(), 25, 0x1B73, 0));
                Add(new GenericBuyInfo("Metal Shield", typeof(MetalShield), MetalShield.GetSBPurchaseValue(), 25, 0x1B7B, 0));
                Add(new GenericBuyInfo("Bronze Shield", typeof(BronzeShield), BronzeShield.GetSBPurchaseValue(), 25, 0x1B72, 0));
                Add(new GenericBuyInfo("Metal Kite Shield", typeof(MetalKiteShield), MetalKiteShield.GetSBPurchaseValue(), 25, 0x1B74, 0));
                Add(new GenericBuyInfo("Heater Shield", typeof(HeaterShield), HeaterShield.GetSBPurchaseValue(), 25, 0x1B76, 0));                        
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
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

                Add(typeof(RingmailHelm), RingmailHelm.GetSBSellValue());
                Add(typeof(RingmailGorget), RingmailGorget.GetSBSellValue());
                Add(typeof(RingmailArms), RingmailArms.GetSBSellValue());
                Add(typeof(RingmailGloves), RingmailGloves.GetSBSellValue());
                Add(typeof(RingmailChest), RingmailChest.GetSBSellValue());
                Add(typeof(RingmailLegs), RingmailLegs.GetSBSellValue());

                Add(typeof(ChainmailCoif), ChainmailCoif.GetSBSellValue());
                Add(typeof(ChainmailGorget), ChainmailGorget.GetSBSellValue());
                Add(typeof(ChainmailArms), ChainmailArms.GetSBSellValue());
                Add(typeof(ChainmailGloves), ChainmailGloves.GetSBSellValue());
                Add(typeof(ChainmailChest), ChainmailChest.GetSBSellValue());
                Add(typeof(ChainmailLegs), ChainmailLegs.GetSBSellValue());

                Add(typeof(PlateHelm), PlateHelm.GetSBSellValue());
                Add(typeof(PlateGorget), PlateGorget.GetSBSellValue());
                Add(typeof(PlateArms), PlateArms.GetSBSellValue());
                Add(typeof(PlateGloves), PlateGloves.GetSBSellValue());
                Add(typeof(PlateChest), PlateChest.GetSBSellValue());
                Add(typeof(FemalePlateChest), FemalePlateChest.GetSBSellValue());
                Add(typeof(PlateLegs), PlateLegs.GetSBSellValue());

                Add(typeof(Bascinet), Bascinet.GetSBSellValue());
                Add(typeof(CloseHelm), CloseHelm.GetSBSellValue());
                Add(typeof(NorseHelm), NorseHelm.GetSBSellValue());

                Add(typeof(Buckler), Buckler.GetSBSellValue());
                Add(typeof(MetalShield), MetalShield.GetSBSellValue());
                Add(typeof(BronzeShield), BronzeShield.GetSBSellValue());
                Add(typeof(MetalKiteShield), MetalKiteShield.GetSBSellValue());
                Add(typeof(HeaterShield), HeaterShield.GetSBSellValue());
			} 
		} 
	} 
}