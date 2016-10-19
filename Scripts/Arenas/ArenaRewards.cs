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
    public static class ArenaRewards
    {
        public static List<ArenaRewardDetail> ArenaRewardList = new List<ArenaRewardDetail>()
        {    
        };

        public static void Initialize()
        {
            ArenaRewardDetail rewardDetail = new ArenaRewardDetail();
            rewardDetail.DisplayName = "Random Champion Reward Box";
            rewardDetail.DisplayNameHue = 149;
            rewardDetail.ItemRewardPoints = 2;
            rewardDetail.ItemBlackRewardPoints = 0;
            rewardDetail.ItemDescription = new List<string>()
            {
                "An decorative box containing a randomized arena reward item."
            };
            rewardDetail.IconItemID = 11764;
            rewardDetail.IconHue = 2587;
            rewardDetail.IconOffsetX = 48;
            rewardDetail.IconOffsetY = 24;
            rewardDetail.GumpCollectionId = "";

            ArenaRewardList.Add(rewardDetail);

            //-----

            rewardDetail = new ArenaRewardDetail();
            rewardDetail.DisplayName = "Champion's Statue";
            rewardDetail.DisplayNameHue = 149;
            rewardDetail.ItemRewardPoints = 10;
            rewardDetail.ItemBlackRewardPoints = 0;
            rewardDetail.ItemDescription = new List<string>()
            {
                "An large, decorative statue depicting an arena champion."
            };
            rewardDetail.IconItemID = 5416;
            rewardDetail.IconHue = 0;
            rewardDetail.IconOffsetX = 55;
            rewardDetail.IconOffsetY = 13;
            rewardDetail.GumpCollectionId = "";

            ArenaRewardList.Add(rewardDetail);

            //-----

            rewardDetail = new ArenaRewardDetail();
            rewardDetail.DisplayName = "True Black Clothing Dye";
            rewardDetail.DisplayNameHue = 149;
            rewardDetail.ItemRewardPoints = 0;
            rewardDetail.ItemBlackRewardPoints = 1;
            rewardDetail.ItemDescription = new List<string>()
            {
                "Dye any clothing true black color (1 use)."
            };
            rewardDetail.IconItemID = 3622;
            rewardDetail.IconHue = 1102;
            rewardDetail.IconOffsetX = 57;
            rewardDetail.IconOffsetY = 40;
            rewardDetail.GumpCollectionId = "";

            ArenaRewardList.Add(rewardDetail);

            //-----

            rewardDetail = new ArenaRewardDetail();
            rewardDetail.DisplayName = "True Black Champion's Vest";
            rewardDetail.DisplayNameHue = 149;
            rewardDetail.ItemRewardPoints = 0;
            rewardDetail.ItemBlackRewardPoints = 2;
            rewardDetail.ItemDescription = new List<string>()
            {
                "A unique-styled vest dyed in true black color (blessed)."
            };
            rewardDetail.IconItemID = 0;
            rewardDetail.IconHue = 0;
            rewardDetail.IconOffsetX = 57;
            rewardDetail.IconOffsetY = 40;
            rewardDetail.GumpCollectionId = "TrueBlackChampionsVestArenaReward";

            ArenaRewardList.Add(rewardDetail);

            //-----

            rewardDetail = new ArenaRewardDetail();
            rewardDetail.DisplayName = "True Black Champion's Vest";
            rewardDetail.DisplayNameHue = 149;
            rewardDetail.ItemRewardPoints = 0;
            rewardDetail.ItemBlackRewardPoints = 2;
            rewardDetail.ItemDescription = new List<string>()
            {
                "A unique-styled vest dyed in true black color (blessed)."
            };
            rewardDetail.IconItemID = 0;
            rewardDetail.IconHue = 0;
            rewardDetail.IconOffsetX = 57;
            rewardDetail.IconOffsetY = 40;
            rewardDetail.GumpCollectionId = "TrueBlackChampionsVestArenaReward";

            ArenaRewardList.Add(rewardDetail);
        }
    }

    public class ArenaRewardDetail
    {
        public string DisplayName = "Reward Display Name";
        public int DisplayNameHue = 149;

        public List<string> ItemDescription = new List<string>() {"Reward Description"};

        public int ItemRewardPoints = 0;
        public int ItemBlackRewardPoints = 0;

        public int IconItemID = 22326;
        public int IconHue = 0;
        public int IconOffsetX = 0;
        public int IconOffsetY = 0;
        public string GumpCollectionId = "";

        public ArenaRewardDetail()
        {
        }
    }
}
