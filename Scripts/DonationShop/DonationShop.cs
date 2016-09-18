using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public static class DonationShop
    {
        public static string DonationCurrencyName = "Platinum";
        public static Type DonationCurrencyType = typeof(DragonCoin);        

        public static List<DonationCategory> DonationCategories = new List<DonationCategory>();
        public static Dictionary<DonationCategory, List<DonationItem>> DonationShopList = new Dictionary<DonationCategory, List<DonationItem>>()
        {              
        };

        public static void Initialize()
        {
            CommandSystem.Register("Donation", AccessLevel.Player, new CommandEventHandler(DonationShop_OnCommand));
            CommandSystem.Register("DonationShop", AccessLevel.Player, new CommandEventHandler(DonationShop_OnCommand));
            
            //Monthly Specials
            DonationShopList.Add(new DonationCategory("Monthly Specials", 3103, 0, 10, -5),
            new List<DonationItem>()
            {
                new DonationItem(typeof(BearMask), "Bear Mask", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap."}, 500, 5445, 0, 48, 29),                
                //Launch Supporter Sash/Sandals/Hat
            });

            //Masks
            DonationShopList.Add(new DonationCategory("Masks", 5445, 0, -2, 0), 
            new List<DonationItem>()
            {
                new DonationItem(typeof(BearMask), "Bear Mask", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap.", "(Blessed)"}, 1000, 5445, 0, 48, 29),
                new DonationItem(typeof(DeerMask), "Deer Mask", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap.", "(Blessed)"}, 1000, 5447, 0, 44, 33),
                new DonationItem(typeof(OrcMask), "Orc Mask", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap.", "(Blessed)"}, 1000, 5147, 0, 55, 42),
                new DonationItem(typeof(OrcHelmMask), "Orc Helm (as Mask)", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap.", "(Blessed)"}, 1000, 7947, 0, 45, 38),
                new DonationItem(typeof(TribalMask), "Tribal Mask", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap.", "(Blessed)"}, 1000, 5451, 0, 55, 42),
                new DonationItem(typeof(AncestorMask), "Ancestor Mask", new List<string>{"A stylish piece of head protection.", "Equivalent to exceptional Leather Cap.", "(Blessed)"}, 1000, 5449, 0, 54, 37)
            
                //Mask Dye
                //Mask Dye
                //Mask Dye
            });

            //Clothing
            DonationShopList.Add(new DonationCategory("Clothing", 8098, 2576, -5, 0),
            new List<DonationItem>()
            {
                new DonationItem(typeof(ClothingBlessDeed), "Clothing Bless Deed", new List<string>{"Bless an article of clothing."}, 500, 5360, 2958, 48, 33),
                //Sash Layering Deed  
              
                //Special Dye
                //Special Dye
                //Special Dye
            });

            //Convenience Items
            DonationShopList.Add(new DonationCategory("Convenience", 8793, 2550, 0, 0),
            new List<DonationItem>()
            {
                new DonationItem(typeof(DiamondHatchet), "Diamond Hatchet", new List<string>{"Built to last and styled to impress!", "5000 Uses and Blessed."}, 500, 3908, 2500, 50, 38), 
                new DonationItem(typeof(DiamondPickaxe), "Diamond Pickaxe", new List<string>{"Built to last and styled to impress!", "5000 Uses and Blessed."}, 500, 3717, 2500, 57, 32), 
                new DonationItem(typeof(RunebookBlessDeed), "Runebook Bless Deed", new List<string>{"Bless a runebook or rune tome"}, 500, 5360, 1121, 48, 33),
                new DonationItem(typeof(PotionKegCombiningDeed), "Potion Keg Combining Deed", new List<string>{"Combine the capacities of two potion", "kegs."}, 500, 5360, 2500, 48, 33),
                new DonationItem(typeof(PotionBarrelConversionDeed), "Potion Barrel Conversion Deed", new List<string>{"Convert a potion keg with 500 capacity", "or more into a potion barrel."}, 500, 5360, 2515, 48, 33),                
                new DonationItem(typeof(SkillMasteryScrollLibrary), "Power Scroll Library", new List<string>{"Store and organize Power Scrolls", "in this handy tome."}, 500, 8793, 2657, 52, 31),
                new DonationItem(typeof(SpellScrollLibrary), "Spell Scroll Library", new List<string>{"Store and organize Spell Scrolls", "in this handy tome."}, 500, 8793, 2117, 52, 31),
                new DonationItem(typeof(TreasureMapLibrary), "Treasure Map Library", new List<string>{"Store and organize Treasure Maps", "in this handy tome."}, 500, 8793, 2550, 52, 31),            
            
                //Unlimited Use Crafting Tools (Blessed)
                //Unlimited Use Harvesting Tools (Blessed)
            });

            //Personalization Items
            DonationShopList.Add(new DonationCategory("Personalization", 5360, 0, 0, 0),
            new List<DonationItem>()
            {
                //Name Change Deed
                //Gender Change Deed                
                //Hair Restyle Deed
                //Beard Restyle Deed                
                //Guild Rename Deed
            });

            //Misc Items
            DonationShopList.Add(new DonationCategory("Misc", 5360, 0, 0, 0),
            new List<DonationItem>()
            {
                //Runebook Dye Tub
                //Bulk Order Dye Tub
                //Backpack Dye Tub

                //Trapped Pouch (Charges)
            }); 

            //Decorations

            //Add-Ons            
        }

        [Usage("DonationShop")]
        [Aliases("DonationStore", "Store", "Shop", "Donation")]
        [Description("Accesses the Donation Shop")]
        public static void DonationShop_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.SendSound(0x055);

            player.CloseGump(typeof(DonationShop));
            player.SendGump(new DonationShopGump(player, 0, 0, 0));
        }
    }        

    public class DonationCategory
    {
        public string CategoryName;
        public int CategoryIconItemId;
        public int CategoryIconHue;
        public int CategoryIconOffsetX;
        public int CategoryIconOffsetY;

        public DonationCategory(string categoryName, int categoryIconItemId, int categoryIconHue, int categoryIconOffsetX, int categoryOffsetY)
        {
            CategoryName = categoryName;
            CategoryIconItemId = categoryIconItemId;
            CategoryIconHue = categoryIconHue;
            CategoryIconOffsetX = categoryIconOffsetX;
            CategoryIconOffsetY = categoryOffsetY;

            DonationShop.DonationCategories.Add(this);
        }
    }

    public class DonationItem
    {
        public Type ItemType;
        public string ItemName;
        public List<string> ItemDescription = new List<string>() { };
        public int ItemCost;
        public int ItemIconItemId;
        public int ItemIconHue;
        public int ItemIconOffsetX;
        public int ItemIconOffsetY;

        public DonationItem(Type itemType, string itemName, List<string> itemDescription, int itemCost, int itemIconItemId, int itemIconHue, int itemIconOffsetX, int itemIconOffsetY)
        {
            ItemType = itemType;
            ItemName = itemName;
            ItemDescription = itemDescription;
            ItemCost = itemCost;
            ItemIconItemId = itemIconItemId;
            ItemIconHue = itemIconHue;
            ItemIconOffsetX = itemIconOffsetY;
            ItemIconOffsetY = itemIconOffsetY;
        }
    }
}
