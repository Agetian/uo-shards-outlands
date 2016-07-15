using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBBlacksmith : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBBlacksmith() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                //Tools
                Add(new GenericBuyInfo("Tongs", typeof(Tongs), Tongs.GetSBPurchaseValue(), 50, 0xFBB, 0));
                Add(new GenericBuyInfo("Smith's Hammer", typeof(SmithHammer), SmithHammer.GetSBPurchaseValue(), 50, 0x13E3, 0));

                //Armor
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
                
                //Weapons
                Add(new GenericBuyInfo("Dagger", typeof(Dagger), Dagger.GetSBPurchaseValue(), 25, 0xF52, 0));
                Add(new GenericBuyInfo("Kryss", typeof(Kryss), Kryss.GetSBPurchaseValue(), 25, 0x1401, 0));
                Add(new GenericBuyInfo("War Fork", typeof(WarFork), WarFork.GetSBPurchaseValue(), 25, 0x1405, 0));
                Add(new GenericBuyInfo("Short Spear", typeof(ShortSpear), ShortSpear.GetSBPurchaseValue(), 25, 0x1403, 0));
                Add(new GenericBuyInfo("Pitchfork", typeof(Pitchfork), Pitchfork.GetSBPurchaseValue(), 25, 0xE87, 0));
                Add(new GenericBuyInfo("Spear", typeof(Spear), Spear.GetSBPurchaseValue(), 25, 0xF62, 0));

                Add(new GenericBuyInfo("Hammer Pick", typeof(HammerPick), HammerPick.GetSBPurchaseValue(), 25, 0x143D, 0));
                Add(new GenericBuyInfo("War Axe", typeof(WarAxe), WarAxe.GetSBPurchaseValue(), 25, 0x13B0, 0));
                Add(new GenericBuyInfo("Mace", typeof(Mace), Mace.GetSBPurchaseValue(), 25, 0xF5C, 0));
                Add(new GenericBuyInfo("Maul", typeof(Maul), Maul.GetSBPurchaseValue(), 25, 0x143B, 0));
                Add(new GenericBuyInfo("WarHammer", typeof(WarHammer), WarHammer.GetSBPurchaseValue(), 25, 0x1439, 0));
                Add(new GenericBuyInfo("War Mace", typeof(WarMace), WarMace.GetSBPurchaseValue(), 25, 0x1407, 0));

                Add(new GenericBuyInfo("Butcher Knife", typeof(ButcherKnife), ButcherKnife.GetSBPurchaseValue(), 25, 0x13F6, 0));
                Add(new GenericBuyInfo("Skinning Knife", typeof(SkinningKnife), SkinningKnife.GetSBPurchaseValue(), 25, 0xEC4, 0));
                Add(new GenericBuyInfo("Cleaver", typeof(Cleaver), Cleaver.GetSBPurchaseValue(), 25, 0xEC3, 0));
                Add(new GenericBuyInfo("Cutlass", typeof(Cutlass), Cutlass.GetSBPurchaseValue(), 25, 0x1441, 0));
                Add(new GenericBuyInfo("Katana", typeof(Katana), Katana.GetSBPurchaseValue(), 25, 0x13FF, 0));
                Add(new GenericBuyInfo("Scimitar", typeof(Scimitar), Scimitar.GetSBPurchaseValue(), 25, 0x13B6, 0));
                Add(new GenericBuyInfo("Broadsword", typeof(Broadsword), Broadsword.GetSBPurchaseValue(), 25, 0xF5E, 0));
                Add(new GenericBuyInfo("Longsword", typeof(Longsword), Longsword.GetSBPurchaseValue(), 25, 0xF61, 0));
                Add(new GenericBuyInfo("Viking Sword", typeof(VikingSword), VikingSword.GetSBPurchaseValue(), 25, 0x13B9, 0));
                Add(new GenericBuyInfo("Axe", typeof(Axe), Axe.GetSBPurchaseValue(), 25, 0xF49, 0));
                Add(new GenericBuyInfo("Battle Axe", typeof(BattleAxe), BattleAxe.GetSBPurchaseValue(), 25, 0xF47, 0));
                Add(new GenericBuyInfo("Double Axe", typeof(DoubleAxe), DoubleAxe.GetSBPurchaseValue(), 25, 0xF4B, 0));
                Add(new GenericBuyInfo("Executioner's Axe", typeof(ExecutionersAxe), ExecutionersAxe.GetSBPurchaseValue(), 25, 0xF45, 0));
                Add(new GenericBuyInfo("Large Battle Axe", typeof(LargeBattleAxe), LargeBattleAxe.GetSBPurchaseValue(), 25, 0x13FB, 0));
                Add(new GenericBuyInfo("Two-Handed Axe", typeof(TwoHandedAxe), TwoHandedAxe.GetSBPurchaseValue(), 25, 0x1443, 0));
                Add(new GenericBuyInfo("Bardiche", typeof(Bardiche), Bardiche.GetSBPurchaseValue(), 25, 0xF4D, 0));
                Add(new GenericBuyInfo("Halberd", typeof(Halberd), Halberd.GetSBPurchaseValue(), 25, 0x143E, 0));               
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                //Tools
                Add(typeof(Tongs), Tongs.GetSBSellValue());
                Add(typeof(SmithHammer), SmithHammer.GetSBSellValue());

                //Armor
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

                //Weapons
                Add(typeof(Dagger), Dagger.GetSBSellValue());
                Add(typeof(Kryss), Kryss.GetSBSellValue());
                Add(typeof(WarFork), WarFork.GetSBSellValue());
                Add(typeof(ShortSpear), ShortSpear.GetSBSellValue());
                Add(typeof(Pitchfork), Pitchfork.GetSBSellValue());
                Add(typeof(Spear), Spear.GetSBSellValue());

                Add(typeof(HammerPick), HammerPick.GetSBSellValue());
                Add(typeof(WarAxe), WarAxe.GetSBSellValue());
                Add(typeof(Mace), Mace.GetSBSellValue());
                Add(typeof(Maul), Maul.GetSBSellValue());
                Add(typeof(WarHammer), WarHammer.GetSBSellValue());
                Add(typeof(WarMace), WarMace.GetSBSellValue());

                Add(typeof(ButcherKnife), ButcherKnife.GetSBSellValue());
                Add(typeof(SkinningKnife), SkinningKnife.GetSBSellValue());
                Add(typeof(Cleaver), Cleaver.GetSBSellValue());
                Add(typeof(Cutlass), Cutlass.GetSBSellValue());
                Add(typeof(Katana), Katana.GetSBSellValue());
                Add(typeof(Scimitar), Scimitar.GetSBSellValue());
                Add(typeof(Broadsword), Broadsword.GetSBSellValue());
                Add(typeof(Longsword), Longsword.GetSBSellValue());
                Add(typeof(VikingSword), VikingSword.GetSBSellValue());
                Add(typeof(Axe), Axe.GetSBSellValue());
                Add(typeof(BattleAxe), BattleAxe.GetSBSellValue());
                Add(typeof(DoubleAxe), DoubleAxe.GetSBSellValue());
                Add(typeof(ExecutionersAxe), ExecutionersAxe.GetSBSellValue());
                Add(typeof(LargeBattleAxe), LargeBattleAxe.GetSBSellValue());
                Add(typeof(TwoHandedAxe), TwoHandedAxe.GetSBSellValue());
                Add(typeof(Bardiche), Bardiche.GetSBSellValue());
                Add(typeof(Halberd), Halberd.GetSBSellValue());
			} 
		} 
	} 
}