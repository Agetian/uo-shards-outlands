using System;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Mobiles
{
	public class SBMage : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMage()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                //TEST: FIX THIS!!!!


                /*
				Type[] types = Loot.RegularScrollTypes;

				//for ( int i = 0; i < types.Length; ++i )
				for ( int i = 0; i < 32; ++i ) 

				{
					int itemID = 0x1F2E + i;

					if ( i == 6 )
						itemID = 0x1F2D;

					else if ( i > 6 )
						--itemID;

                    //TEST: DETERMINE SCROLL PRICES
					Add( new GenericBuyInfo( types[i], 12 + ((i / 3) * 10), 20, itemID, 0 ) );
				}
                */

                Add(new GenericBuyInfo("Black Pearl", typeof(BlackPearl), BlackPearl.GetSBPurchaseValue(), 500, 0xF7A, 0));
                Add(new GenericBuyInfo("Bloodmoss", typeof(Bloodmoss), Bloodmoss.GetSBPurchaseValue(), 500, 0xF7B, 0));
                Add(new GenericBuyInfo("Mandrake Root", typeof(MandrakeRoot), MandrakeRoot.GetSBPurchaseValue(), 500, 0xF86, 0));
                Add(new GenericBuyInfo("Garlic", typeof(Garlic), Garlic.GetSBPurchaseValue(), 500, 0xF84, 0));
                Add(new GenericBuyInfo("Ginseng", typeof(Ginseng), Ginseng.GetSBPurchaseValue(), 500, 0xF85, 0));
                Add(new GenericBuyInfo("Nightshade", typeof(Nightshade), Nightshade.GetSBPurchaseValue(), 500, 0xF88, 0));
                Add(new GenericBuyInfo("Spider Silk", typeof(SpidersSilk), SpidersSilk.GetSBPurchaseValue(), 500, 0xF8D, 0));
                Add(new GenericBuyInfo("Sulfurous Ash", typeof(SulfurousAsh), SulfurousAsh.GetSBPurchaseValue(), 500, 0xF8C, 0));

                Add(new GenericBuyInfo("Recall Rune", typeof(RecallRune), RecallRune.GetSBPurchaseValue(), 50, 0x1f14, 0));
                Add(new GenericBuyInfo("Spellbook", typeof(Spellbook), Spellbook.GetSBPurchaseValue(), 25, 0xEFA, 0));
                Add(new GenericBuyInfo("Blank Scroll", typeof(BlankScroll), BlankScroll.GetSBPurchaseValue(), 500, 0x0E34, 0));               
                Add(new GenericBuyInfo("Scribe's Pen", typeof(ScribesPen), ScribesPen.GetSBPurchaseValue(), 50, 0xFBF, 0));
                Add(new GenericBuyInfo("Magical Wizard's Hat", typeof(MagicWizardsHat), MagicWizardsHat.GetSBPurchaseValue(), 25, 0x1718, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
                //TEST: FIX THIS !!!!!

                //TEST: DETERMINE SPELL SCROLL PRICES
                /*
                Type[] types = Loot.RegularScrollTypes;

                for (int i = 0; i < types.Length; ++i)
                {
                    Type type = types[i];

                    var ci = Server.Engines.Craft.DefInscription.CraftSystem.CraftItems.SearchFor(type);

                    if (ci == null)
                        continue;

                    var res = ci.Resources;

                    int cost = 0;

                    foreach (CraftRes c in ci.Resources)
                        cost += CostOfResource(c.ItemType);

                    int level = ((i + 1) / 8) / 2;

                    cost += LevelBonusFor(level);

                    cost *= 3;

                    int count = res.Count;

                    //TEST: DETERMINE SCROLL SELL PRICES
                    Add(type, Math.Min(6 + ((i / 8) * 5), cost));
                }
                */

                Add(typeof(BlackPearl), BlackPearl.GetSBSellValue());
                Add(typeof(Bloodmoss), Bloodmoss.GetSBSellValue());
                Add(typeof(MandrakeRoot), MandrakeRoot.GetSBSellValue());
                Add(typeof(Garlic), Garlic.GetSBSellValue());
                Add(typeof(Ginseng), Ginseng.GetSBSellValue());
                Add(typeof(Nightshade), Nightshade.GetSBSellValue());
                Add(typeof(SpidersSilk), SpidersSilk.GetSBSellValue());
                Add(typeof(SulfurousAsh), SulfurousAsh.GetSBSellValue());

                Add(typeof(RecallRune), RecallRune.GetSBSellValue());
                Add(typeof(BlankScroll), BlankScroll.GetSBSellValue());
                Add(typeof(Spellbook), Spellbook.GetSBSellValue());
                Add(typeof(ScribesPen), ScribesPen.GetSBSellValue());
                Add(typeof(MagicWizardsHat), MagicWizardsHat.GetSBSellValue());
			}

            public static int CostOfResource(Type type)
            {
                if (type == typeof(BlankScroll))
                    return 2;

                else if (type == typeof(MandrakeRoot))
                    return 2;

                else if (type == typeof(BlackPearl))
                    return 3;

                else if (type == typeof(Bloodmoss))
                    return 2;

                else
                    return 1;
            }

            public static int LevelBonusFor(int level)
            {
                if (level < 4)
                    return 0;

                else if (level < 7)
                    return 1;

                else
                    return 2;
            }
		}
	}
}