using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBAlchemist : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBAlchemist()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add(new GenericBuyInfo("Bottle", typeof(Bottle), Bottle.GetSBPurchaseValue(), 200, 0xF0E, 0));
                Add(new GenericBuyInfo("Mortar and Pestle", typeof(MortarPestle), MortarPestle.GetSBPurchaseValue(), 10, 0xE9B, 0));

                Add(new GenericBuyInfo("Black Pearl", typeof(BlackPearl), BlackPearl.GetSBPurchaseValue(), 500, 0xF7A, 0));
                Add(new GenericBuyInfo("Bloodmoss", typeof(Bloodmoss), Bloodmoss.GetSBPurchaseValue(), 500, 0xF7B, 0));
                Add(new GenericBuyInfo("Mandrake Root", typeof(MandrakeRoot), MandrakeRoot.GetSBPurchaseValue(), 500, 0xF86, 0));
                Add(new GenericBuyInfo("Garlic", typeof(Garlic), Garlic.GetSBPurchaseValue(), 500, 0xF84, 0));
                Add(new GenericBuyInfo("Ginseng", typeof(Ginseng), Ginseng.GetSBPurchaseValue(), 500, 0xF85, 0));
                Add(new GenericBuyInfo("Nightshade", typeof(Nightshade), Nightshade.GetSBPurchaseValue(), 500, 0xF88, 0));
                Add(new GenericBuyInfo("Spider Silk", typeof(SpidersSilk), SpidersSilk.GetSBPurchaseValue(), 500, 0xF8D, 0));
                Add(new GenericBuyInfo("Sulfurous Ash", typeof(SulfurousAsh), SulfurousAsh.GetSBPurchaseValue(), 500, 0xF8C, 0));

                Add(new GenericBuyInfo("Lesser Heal Potion", typeof(LesserHealPotion), LesserHealPotion.GetSBPurchaseValue(), 50, 0xF0C, 0));
                Add(new GenericBuyInfo("Lesser Cure Potion", typeof(LesserCurePotion), LesserCurePotion.GetSBPurchaseValue(), 50, 0xF07, 0));               
                Add(new GenericBuyInfo("Refresh Potion", typeof(RefreshPotion), RefreshPotion.GetSBPurchaseValue(), 50, 0xF0B, 0));
                Add(new GenericBuyInfo("Agility Potion", typeof(AgilityPotion), AgilityPotion.GetSBPurchaseValue(), 50, 0xF08, 0));
                Add(new GenericBuyInfo("Strength Potion", typeof(StrengthPotion), StrengthPotion.GetSBPurchaseValue(), 50, 0xF09, 0));
                Add(new GenericBuyInfo("Lesser Magic Resist Potion", typeof(LesserMagicResistPotion), LesserMagicResistPotion.GetSBPurchaseValue(), 10, 0xF06, 0));
                Add(new GenericBuyInfo("Lesser Poison Potion", typeof(LesserPoisonPotion), LesserPoisonPotion.GetSBPurchaseValue(), 50, 0xF0A, 0));
                Add(new GenericBuyInfo("Lesser Explosion Potion", typeof(LesserExplosionPotion), LesserExplosionPotion.GetSBPurchaseValue(), 50, 0xF0D, 0));                

                Add(new GenericBuyInfo("Hair Dye", typeof(HairDye), HairDye.GetSBPurchaseValue(), 10, 0xEFF, 0));               
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                Add(typeof(Bottle), Bottle.GetSBSellValue());
                Add(typeof(MortarPestle), MortarPestle.GetSBSellValue());

                Add(typeof(BlackPearl), BlackPearl.GetSBSellValue());
                Add(typeof(Bloodmoss), Bloodmoss.GetSBSellValue());
                Add(typeof(MandrakeRoot), MandrakeRoot.GetSBSellValue());
                Add(typeof(Garlic), Garlic.GetSBSellValue());
                Add(typeof(Ginseng), Ginseng.GetSBSellValue());
                Add(typeof(Nightshade), Nightshade.GetSBSellValue());
                Add(typeof(SpidersSilk), SpidersSilk.GetSBSellValue());
                Add(typeof(SulfurousAsh), SulfurousAsh.GetSBSellValue());               

                Add(typeof(LesserMagicResistPotion), LesserMagicResistPotion.GetSBSellValue());
                Add(typeof(AgilityPotion), AgilityPotion.GetSBSellValue());
                Add(typeof(StrengthPotion), StrengthPotion.GetSBSellValue());
                Add(typeof(RefreshPotion), RefreshPotion.GetSBSellValue());
                Add(typeof(LesserCurePotion), LesserCurePotion.GetSBSellValue());
                Add(typeof(LesserHealPotion), LesserHealPotion.GetSBSellValue());
                Add(typeof(LesserPoisonPotion), LesserPoisonPotion.GetSBSellValue());
                Add(typeof(LesserExplosionPotion), LesserExplosionPotion.GetSBSellValue());

                Add(typeof(HairDye), HairDye.GetSBSellValue()); 
			}
		}
	}
}
