using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBWeaponsmith: SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBWeaponsmith() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
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

                Add(new GenericBuyInfo("Bow", typeof(Bow), Bow.GetSBPurchaseValue(), 25, 0x13B2, 0));
                Add(new GenericBuyInfo("Crossbow", typeof(Crossbow), Crossbow.GetSBPurchaseValue(), 25, 0xF50, 0));
                Add(new GenericBuyInfo("Heavy Crossbow", typeof(HeavyCrossbow), HeavyCrossbow.GetSBPurchaseValue(), 25, 0x13FD, 0));
			}
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
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

                Add(typeof(Bow), Bow.GetSBSellValue());
                Add(typeof(Crossbow), Crossbow.GetSBSellValue());
                Add(typeof(HeavyCrossbow), HeavyCrossbow.GetSBSellValue());
			} 
		} 
	} 
}
