using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Regions;

namespace Server
{
    public class ShipGump : Gump
    {
        public enum ShipPageType
        {
            Overview,
            Upgrades,
            Stats,
            History,
            Players
        }

        public enum PlayersPageType
        {
            Friends,
            CoOwners
        }

        public PlayerMobile m_Player;
        public ShipGumpObject m_ShipGumpObject;

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public static int PlayersPerPage = 10;

        public ShipGump(PlayerMobile player, ShipGumpObject shipGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ShipGumpObject = shipGumpObject;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipGumpObject == null) return;

            BaseShip ship = m_ShipGumpObject.m_Ship;

            if (ship == null) return;
            if (ship.Deleted) return;
            
            //TEST
            ship.ShipName = "The Rebellion";

            ship.ThemeUpgrade = ShipUpgrades.ThemeType.Pirate;
            ship.PaintUpgrade = ShipUpgrades.PaintType.DarkGrey;
            ship.CannonMetalUpgrade = ShipUpgrades.CannonMetalType.Bloodstone;

            ship.OutfittingUpgrade = ShipUpgrades.OutfittingType.Hunter;
            ship.BannerUpgrade = ShipUpgrades.BannerType.Corsairs;
            ship.CharmUpgrade = ShipUpgrades.CharmType.BarrelOfLimes;

            ship.MinorAbilityUpgrade = ShipUpgrades.MinorAbilityType.ExpediteRepairs;
            ship.MajorAbilityUpgrade = ShipUpgrades.MajorAbilityType.Smokescreen;
            ship.EpicAbilityUpgrade = ShipUpgrades.EpicAbilityType.Hellfire;

            #region Background Images

            AddImage(17, 356, 103);
            AddImage(303, 382, 103);
            AddImage(160, 416, 103);
            AddImage(17, 11, 103);
            AddImage(162, 11, 103);
            AddImage(303, 11, 103);
            AddImage(303, 99, 103);
            AddImage(303, 192, 103);
            AddImage(303, 287, 103);
            AddImage(17, 97, 103);
            AddImage(17, 191, 103);
            AddImage(17, 288, 103);
            AddImage(18, 417, 103);
            AddImage(303, 416, 103);
            AddBackground(31, 21, 403, 485, 3000);
            AddImage(32, 446, 96, 2308);
            AddImage(187, 446, 96, 2308);
            AddImage(254, 446, 96, 2308);
            AddImage(32, 445, 96, 2308);
            AddImage(187, 445, 96, 2308);
            AddImage(254, 445, 96, 2308);
            AddItem(29, 471, 5363);
            AddItem(194, 474, 4030);
            AddItem(350, 469, 2462);           
            AddItem(109, 467, 5362);           
            AddItem(277, 468, 7715);
            AddItem(273, 464, 4031);
            AddItem(201, 468, 4007);

            #endregion

            //Guide
            AddButton(14, 11, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(10, 0, 149, "Guide");

            //Header
            AddImage(94, 3, 1141);
            AddLabel(Utility.CenteredTextOffset(235, ship.ShipName), 5, 149, ship.ShipName);

            #region Footers

            //Footers
            AddLabel(49, 449, 149, "Overview");
            if (m_ShipGumpObject.m_ShipPage == ShipPageType.Overview)
                AddButton(62, 472, 9724, 9721, 2, GumpButtonType.Reply, 0);
            else
                AddButton(62, 472, 9721, 9724, 2, GumpButtonType.Reply, 0);

            AddLabel(134, 449, 2219, "Upgrades");
            if (m_ShipGumpObject.m_ShipPage == ShipPageType.Upgrades)
                AddButton(147, 472, 9724, 9721, 3, GumpButtonType.Reply, 0);
            else
                AddButton(147, 472, 9721, 9724, 3, GumpButtonType.Reply, 0);

            AddLabel(225, 449, 2608, "Stats");
            if (m_ShipGumpObject.m_ShipPage == ShipPageType.Stats)
                AddButton(228, 472, 9724, 9721, 4, GumpButtonType.Reply, 0);
            else
                AddButton(228, 472, 9721, 9724, 4, GumpButtonType.Reply, 0);

            AddLabel(300, 449, 2550, "History");
            if (m_ShipGumpObject.m_ShipPage == ShipPageType.History)
                AddButton(307, 472, 9724, 9721, 5, GumpButtonType.Reply, 0);
            else
                AddButton(307, 472, 9721, 9724, 5, GumpButtonType.Reply, 0);

            AddLabel(377, 449, 2599, "Players");
            if (m_ShipGumpObject.m_ShipPage == ShipPageType.Players)
                AddButton(387, 472, 9724, 9721, 6, GumpButtonType.Reply, 0);
            else
                AddButton(387, 472, 9721, 9724, 6, GumpButtonType.Reply, 0);

            #endregion

            switch (shipGumpObject.m_ShipPage)
            {
                #region Overview

                case ShipPageType.Overview:

                    #region Stats

                    int hullPoints = ship.HitPoints;
                    int maxHullPoints = ship.MaxHitPoints;
                    double hullPercent = (double)hullPoints / (double)maxHullPoints;

                    int sailPoints = ship.SailPoints;
                    int maxSailPoints = ship.MaxSailPoints;
                    double sailPercent = (double)sailPoints / (double)maxSailPoints;

                    int gunPoints = ship.GunPoints;
                    int maxGunPoints = ship.MaxGunPoints;
                    double gunPercent = (double)gunPoints / (double)maxGunPoints;

                    string shipType = "Carrack"; //TEST: FIX

                    AddImage(88, 41, 103);
			        AddImage(225, 41, 103);
			        AddImageTiled(102, 55, 257, 77, 2624);

                    AddLabel(Utility.CenteredTextOffset(232, shipType), 37, WhiteTextHue, shipType);

                    AddLabel(111, 60, 149, "Hull");
                    AddImage(142, 64, 2057);
                    AddImageTiled(142 + Utility.ProgressBarX(hullPercent), 67, Utility.ProgressBarWidth(hullPercent), 7, 2488);
                    AddLabel(260, 60, WhiteTextHue, hullPoints.ToString() + "/" + maxHullPoints.ToString());

                    AddLabel(106, 83, 187, "Sails");
                    AddImage(142, 88, 2054);
                    AddImageTiled(142 + Utility.ProgressBarX(sailPercent), 91, Utility.ProgressBarWidth(sailPercent), 7, 2488);
                    AddLabel(260, 83, WhiteTextHue, sailPoints.ToString() + "/" + maxSailPoints.ToString());

                    AddLabel(106, 106, WhiteTextHue, "Guns");
                    AddImage(142, 111, 2057, 2499);
                    AddImageTiled(142 + Utility.ProgressBarX(gunPercent), 114, Utility.ProgressBarWidth(gunPercent), 7, 2488);                  
                    AddLabel(260, 106, WhiteTextHue, gunPoints.ToString() + "/" + maxGunPoints.ToString());
                    
                    #endregion
			        
                    #region Abilities

                    ShipUpgradeDetail minorAbilityDetail = ShipUpgrades.GetMinorAbilityDetail(ship.MinorAbilityUpgrade);
                    ShipUpgradeDetail majorAbilityDetail = ShipUpgrades.GetMajorAbilityDetail(ship.MajorAbilityUpgrade);
                    ShipUpgradeDetail epicAbilityDetail = ShipUpgrades.GetEpicAbilityDetail(ship.EpicAbilityUpgrade);

                    string minorAbilityCooldownText = "1m 59s"; //TEST
                    string majorAbilityCooldownText = "4m 59s"; //TEST
                    string epicAbilityCooldownText = "9m 59s"; //TEST

                    int offsetX = -32;
                    int offsetY = -22;

                    //Minor Ability
                    AddLabel(60, 153, 2599, "Minor Ability");
                    AddImage(62, 197, 2328);
                    if (minorAbilityDetail != null)
                    {
                        AddLabel(Utility.CenteredTextOffset(105, minorAbilityDetail.m_UpgradeName), 173, WhiteTextHue, minorAbilityDetail.m_UpgradeName);
                        AddButton(57, 263, 2151, 2151, 16, GumpButtonType.Reply, 0);
                        AddLabel(94, 267, WhiteTextHue, minorAbilityCooldownText);
                        AddGumpCollection(GumpCollections.GetGumpCollection(minorAbilityDetail.GumpCollectionId, -1), offsetX + 62, offsetY + 197);
                    }

                    //Major Ability
                    AddLabel(190, 153, 2603, "Major Ability");
                    AddImage(192, 197, 2328);
                    if (majorAbilityDetail != null)
                    {
                        AddLabel(Utility.CenteredTextOffset(235, majorAbilityDetail.m_UpgradeName), 173, WhiteTextHue, majorAbilityDetail.m_UpgradeName);
                        AddButton(187, 263, 2151, 2151, 17, GumpButtonType.Reply, 0);
                        AddLabel(220, 267, WhiteTextHue, majorAbilityCooldownText);
                        AddGumpCollection(GumpCollections.GetGumpCollection(majorAbilityDetail.GumpCollectionId, -1), offsetX + 192, offsetY + 197);
                    }

                    //Epic Ability
                    AddLabel(324, 153, 2606, "Epic Ability");
                    AddImage(320, 197, 2328);
                    if (epicAbilityDetail != null)
                    {
                        AddLabel(Utility.CenteredTextOffset(360, epicAbilityDetail.m_UpgradeName), 173, WhiteTextHue, epicAbilityDetail.m_UpgradeName);
                        AddButton(315, 263, 2151, 2151, 18, GumpButtonType.Reply, 0);
                        AddLabel(349, 267, WhiteTextHue, epicAbilityCooldownText);
                        AddGumpCollection(GumpCollections.GetGumpCollection(epicAbilityDetail.GumpCollectionId, -1), offsetX + 320, offsetY + 197);
                    }

                    #endregion

                    #region Middle

                    string doubloonCount = "10,000"; //TEST

                    AddLabel(179, 304, 149, "Doubloons in Hold");
                    AddItem(182, 325, 2539);
                    AddLabel(220, 322, WhiteTextHue, doubloonCount);

                    #endregion
                    
                    #region Actions

                    //Actions - Left
                    AddButton(38, 344, 4011, 248, 19, GumpButtonType.Reply, 0);
                    AddLabel(75, 344, WhiteTextHue, "Open Ship Hotbar");

                    AddButton(38, 369, 4014, 248, 10, GumpButtonType.Reply, 0);
                    AddLabel(75, 369, WhiteTextHue, "Raise Anchor");

                    AddButton(38, 394, 4002, 248, 11, GumpButtonType.Reply, 0);
                    AddLabel(75, 394, WhiteTextHue, "Embark/Disembark");

			        AddButton(38, 419, 4008, 248, 12, GumpButtonType.Reply, 0);
                    AddLabel(75, 419, WhiteTextHue, "Embark/Disembark Followers");

                    //Actions - Right
                    AddButton(275, 344, 4026, 248, 20, GumpButtonType.Reply, 0);
                    AddLabel(313, 342, WhiteTextHue, "Rename Ship");
                   
			        AddButton(275, 369, 4029, 248, 14, GumpButtonType.Reply, 0);
                    AddLabel(313, 369, WhiteTextHue, "Throw Overboard");

                    AddButton(275, 394, 4020, 248, 13, GumpButtonType.Reply, 0);
                    AddLabel(313, 394, WhiteTextHue, "Clear The Decks");

			        AddButton(276, 419, 4017, 248, 15, GumpButtonType.Reply, 0);
                    AddLabel(313, 419, WhiteTextHue, "Dock The Ship");

                    #endregion
                break;

                #endregion

                #region Upgrades                

                case ShipPageType.Upgrades:
                    ShipUpgradeDetail themeDetail = ShipUpgrades.GetThemeDetail(ship.ThemeUpgrade);
                    ShipUpgradeDetail paintDetail = ShipUpgrades.GetPaintDetail(ship.PaintUpgrade);
                    ShipUpgradeDetail cannonMetalDetail = ShipUpgrades.GetCannonMetalDetail(ship.CannonMetalUpgrade);

                    ShipUpgradeDetail outfittingDetail = ShipUpgrades.GetOutfittingDetail(ship.OutfittingUpgrade);
                    ShipUpgradeDetail bannerDetail = ShipUpgrades.GetBannerDetail(ship.BannerUpgrade);
                    ShipUpgradeDetail charmDetail = ShipUpgrades.GetCharmDetail(ship.CharmUpgrade);

                    minorAbilityDetail = ShipUpgrades.GetMinorAbilityDetail(ship.MinorAbilityUpgrade);
                    majorAbilityDetail = ShipUpgrades.GetMajorAbilityDetail(ship.MajorAbilityUpgrade);
                    epicAbilityDetail = ShipUpgrades.GetEpicAbilityDetail(ship.EpicAbilityUpgrade);

                    offsetX = -32;
                    offsetY = -22;

                    AddLabel(78, 38, 145, "Theme");
                    AddImage(58, 80, 2328);
                    if (themeDetail != null)
                    {
                        AddButton(74, 143, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(94, 140, 2550, "Info");                       
                        AddLabel(Utility.CenteredTextOffset(100, themeDetail.m_UpgradeName), 58, WhiteTextHue, themeDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(themeDetail.GumpCollectionId, -1), offsetX + 58, offsetY + 80);
                    }

                    AddLabel(212, 38, 2578, "Paint");
                    AddImage(190, 80, 2328);
                    if (paintDetail != null)
                    {
                        AddButton(206, 143, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(226, 140, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(235, paintDetail.m_UpgradeName), 58, WhiteTextHue, paintDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(paintDetail.GumpCollectionId, -1), offsetX + 190, offsetY + 80);
                    }

                    AddLabel(316, 38, 2301, "Cannon Metal");
                    AddImage(318, 80, 2328);
                    if (cannonMetalDetail != null)
                    {
                        AddButton(334, 143, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(354, 140, 2550, "Info");                    
                        AddLabel(Utility.CenteredTextOffset(360, cannonMetalDetail.m_UpgradeName), 58, WhiteTextHue, cannonMetalDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(cannonMetalDetail.GumpCollectionId, -1), offsetX + 318, offsetY + 80);
                    }

                    AddLabel(64, 176, 2550, "Outfitting");
                    AddImage(58, 218, 2328);
                    if (outfittingDetail != null)
                    {
                        AddButton(74, 281, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(94, 278, 2550, "Info");                      
                        AddLabel(Utility.CenteredTextOffset(100, outfittingDetail.m_UpgradeName), 196, WhiteTextHue, outfittingDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(outfittingDetail.GumpCollectionId, -1), offsetX + 58, offsetY + 218);
                    }

                    AddLabel(208, 176, 2114, "Banner");
                    AddImage(190, 218, 2328);
                    if (bannerDetail != null)
                    {
                        AddButton(206, 281, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(226, 278, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(235, bannerDetail.m_UpgradeName), 196, WhiteTextHue, bannerDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(bannerDetail.GumpCollectionId, -1), offsetX + 190, offsetY + 218);
                    }

                    AddLabel(336, 176, 2650, "Charm");
                    AddImage(318, 218, 2328);
                    if (charmDetail != null)
                    {
                        AddButton(334, 281, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(354, 278, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(360, charmDetail.m_UpgradeName), 196, WhiteTextHue, charmDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(charmDetail.GumpCollectionId, -1), offsetX + 318, offsetY + 218);
                    }

                    AddLabel(58, 316, 2599, "Minor Ability");
                    AddImage(58, 358, 2328);
                    if (minorAbilityDetail != null)
                    {
                        AddButton(74, 421, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(94, 418, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(100, minorAbilityDetail.m_UpgradeName), 336, WhiteTextHue, minorAbilityDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(minorAbilityDetail.GumpCollectionId, -1), offsetX + 58, offsetY + 358);
                    }

                    AddLabel(189, 316, 2603, "Major Ability");
                    AddImage(190, 358, 2328);
                    if (majorAbilityDetail != null)
                    {
                        AddButton(206, 421, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(226, 418, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(235, majorAbilityDetail.m_UpgradeName), 336, WhiteTextHue, majorAbilityDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(majorAbilityDetail.GumpCollectionId, -1), offsetX + 190, offsetY + 358);
                    }

                    AddLabel(321, 316, 2606, "Epic Ability");
			        AddImage(318, 358, 2328);
                    if (epicAbilityDetail != null)
                    {
                        AddButton(334, 421, 2117, 248, 0, GumpButtonType.Reply, 0);
                        AddLabel(354, 418, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(360, epicAbilityDetail.m_UpgradeName), 336, WhiteTextHue, epicAbilityDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(epicAbilityDetail.GumpCollectionId, -1), offsetX + 318, offsetY + 358);
                    }
			        			        
                break;

                #endregion

                #region Stats

                case ShipPageType.Stats:
                    switch (m_ShipGumpObject.m_Page)
                    {
                        case 0:
                            //Type
                            AddLabel(145, 32, 2625, "Ship Type:");
                            AddLabel(319, 32, WhiteTextHue, "Small");

                            //Stats
                            AddLabel(145, 62, 149, "Hull Max Points:");
                            AddLabel(319, 62, WhiteTextHue, "1,000");

                            AddLabel(145, 82, 149, "Sail Max Points:");
                            AddLabel(319, 82, WhiteTextHue, "500");

                            AddLabel(145, 102, 149, "Gun Max Points:");
                            AddLabel(319, 102, WhiteTextHue, "500");

                            //Speed
                            AddLabel(145, 132, 2599, "Forward Speed:");
                            AddLabel(319, 132, WhiteTextHue, "5.0 tiles/sec");

                            AddLabel(145, 152, 2599, "Drifting Speed:");
                            AddLabel(319, 152, WhiteTextHue, "2.5 tiles/sec");

                             AddLabel(145, 172, 2599, "Slowdown Mode Penalty:");
                             AddLabel(319, 172, WhiteTextHue, "-50%");

                            //Cannons
                            AddLabel(145, 202, 2401, "Cannons Per Side:");
                            AddLabel(319, 202, WhiteTextHue, "5");

                            AddLabel(145, 222, 2401, "Cannon Accuracy:");
                            AddLabel(319, 222, WhiteTextHue, "80.0%");

                            AddLabel(145, 242, 2401, "Cannon Damage:");
                            AddLabel(319, 242, WhiteTextHue, "15.0 - 25.0");

                            AddLabel(145, 262, 2401, "Cannon Range:");
                            AddLabel(319, 262, WhiteTextHue, "12");

                            AddLabel(145, 282, 2401, "Cannon Reload Time:");
                            AddLabel(319, 282, WhiteTextHue, "10.0 sec");
			                
			                //Abilities
                            AddLabel(145, 312, 2603, "Repair Cooldown:");
                            AddLabel(319, 312, WhiteTextHue, "120 sec");

                            AddLabel(145, 332, 2603, "Minor Ability Cooldown:");
                            AddLabel(319, 332, WhiteTextHue, "120 sec");

                            AddLabel(145, 352, 2603, "Major Ability Cooldown:");
                            AddLabel(319, 352, WhiteTextHue, "300 sec");
			                
			                AddItem(61, 30, 5363);			               
			                AddItem(87, 44, 710);
			                AddItem(38, 70, 7860);
			                AddItem(74, 59, 3992);
			                AddItem(71, 140, 5367);
			                AddItem(97, 127, 7723);
			                AddItem(38, 136, 7839);
			                AddItem(30, 209, 710);
			                AddItem(71, 246, 3700);
			                AddItem(85, 239, 7726);
			                AddItem(76, 239, 3700);
			                AddItem(81, 247, 3700);
			                AddItem(49, 326, 7866);
			                AddItem(68, 328, 5742);
			                AddItem(90, 326, 6160);

                            AddButton(306, 416, 4005, 4007, 11, GumpButtonType.Reply, 0);
			                AddLabel(201, 416, 2599, "Page");
                            AddLabel(237, 416, WhiteTextHue, "1/2");
                        break;

                        case 1:
                            AddLabel(145, 32, 2550, "Hold Item Capacity");
                            AddLabel(319, 32, WhiteTextHue, "50");

                            AddLabel(145, 52, 2550, "Boarding Chance Bonus:");
                            AddLabel(319, 52, WhiteTextHue, "+0%");

                            AddLabel(145, 72, 2550, "Doubloons Earned Bonus:");
                            AddLabel(319, 72, WhiteTextHue, "+0%");

                            AddButton(136, 416, 4014, 4016, 10, GumpButtonType.Reply, 0);
			                AddLabel(201, 416, 2599, "Page");
			                AddLabel(237, 416, WhiteTextHue, "2/2");                           
			               
			                AddItem(37, 36, 4014);
			                AddItem(69, 48, 5368);
			                AddItem(99, 55, 2539);
                        break;
                    }                    
                break;

                #endregion

                #region History

                case ShipPageType.History:
                    switch (m_ShipGumpObject.m_Page)
                    {
                        case 0:
                            AddLabel(121, 40, 149, "Activity");
                            AddLabel(288, 40, 149, "Amount");
                            AddLabel(347, 40, 149, "Server Rank");

                            //-----

                            AddLabel(78, 60, 53, "Total Doubloons Earned");
                            AddLabel(289, 60, WhiteTextHue, "12,500");
                            AddLabel(377, 60, WhiteTextHue, "1st");	

                            //-----

                            AddLabel(97, 90, 2602, "Player Ships Sunk");
                            AddLabel(306, 90, WhiteTextHue, "3");
                            AddLabel(377, 90, WhiteTextHue, "1st");                           

                            AddLabel(41, 110, 2602, "Player Ships Sunk");                           
			                AddLabel(159, 110, 2550, "(doubloon value)");
                            AddLabel(294, 110, WhiteTextHue, "6,547");
                            AddLabel(377, 110, WhiteTextHue, "1st");

                            AddLabel(100, 130, 2602, "NPC Ships Sunk");
                            AddLabel(301, 130, WhiteTextHue, "75");
                            AddLabel(377, 130, WhiteTextHue, "1st");

                            AddLabel(57, 150, 2602, "NPC Ships Sunk");
			                AddLabel(160, 150, 2550, "(doubloon value)");
                            AddLabel(284, 150, WhiteTextHue, "27,553");
                            AddLabel(377, 150, WhiteTextHue, "1st");

                            //-----      
                       
			                AddLabel(110, 180, 2599, "Fish Caught");
                            AddLabel(277, 180, WhiteTextHue, "1,453,357");
                            AddLabel(377, 180, WhiteTextHue, "1st");

                            AddLabel(107, 200, 2599, "Nets Thrown");
                            AddLabel(298, 200, WhiteTextHue, "223");
                            AddLabel(377, 200, WhiteTextHue, "1st");
                           
			                AddLabel(55, 220, 2599, "Messages in Bottles Recovered");
                            AddLabel(300, 220, WhiteTextHue, "75");
                            AddLabel(377, 220, WhiteTextHue, "1st");
                             
			                AddLabel(83, 240, 2599, "Fishing Spots Fished");
                            AddLabel(300, 240, WhiteTextHue, "47");
                            AddLabel(377, 240, WhiteTextHue, "1st");
			         
                            //-----

                            AddLabel(79, 270, 2606, "Champions Defeated");
                            AddLabel(304, 270, WhiteTextHue, "4");
                            AddLabel(377, 270, WhiteTextHue, "1st");
                           
			                AddLabel(93, 290, 2606, "Bosses Defeated");
			                AddLabel(304, 290, WhiteTextHue, "2");
                            AddLabel(377, 290, WhiteTextHue, "1st");

                            //-----

                            //AddButton(136, 416, 4014, 248, 0, GumpButtonType.Reply, 0);
			                //AddLabel(201, 416, 2599, "Page");
			                //AddLabel(237, 416, 0, "1/2");			               
                            //AddButton(306, 416, 4005, 248, 0, GumpButtonType.Reply, 0);
                        break;

                        case 1:
                        break;
                    }
                break;

                #endregion

                #region Players                

                case ShipPageType.Players:
                    string shipOwner = "Luthius"; //TEST

                    AddLabel(Utility.CenteredTextOffset(235, "Owned by " + shipOwner), 34, WhiteTextHue, "Owned by " + shipOwner);
                    
                    switch (m_ShipGumpObject.m_PlayersPage)
                    {
                        case PlayersPageType.Friends:

                            AddButton(41, 83, 2472, 2474, 10, GumpButtonType.Reply, 0);
                            AddLabel(74, 87, 1256, "Clear Entire Friends List");

                            AddButton(40, 113, 2151, 2154, 11, GumpButtonType.Reply, 0);
			                AddLabel(75, 116, 149, "Set all on owner IP as");
                            AddLabel(221, 116, 2599, "Friends");

                            AddButton(40, 146, 2151, 2154, 12, GumpButtonType.Reply, 0);
			                AddLabel(75, 151, 149, "Set all in owner Guild as");
                            AddLabel(234, 151, 2599, "Friends"); 

                            AddButton(40, 177, 2151, 2151, 13, GumpButtonType.Reply, 0);
			                AddLabel(76, 182, 149, "Add");
                            AddLabel(105, 182, 2599, "Friend");

                            AddLabel(338, 87, 2599, "Friends");
                            AddButton(301, 121, 2223, 2223, 14, GumpButtonType.Reply, 0);
			                AddItem(336, 121, 2543);
                            AddButton(396, 121, 2224, 2224, 15, GumpButtonType.Reply, 0);

                            AddLabel(180, 226, 2550, "Remove Friend");

                        break;

                        case PlayersPageType.CoOwners:
                            AddButton(41, 83, 2472, 2474, 10, GumpButtonType.Reply, 0);
                            AddLabel(74, 87, 1256, "Clear Entire Co-Owners List");

                            AddButton(40, 113, 2151, 2154, 11, GumpButtonType.Reply, 0);
			                AddLabel(75, 116, 149, "Set all on owner IP as");
                            AddLabel(221, 116, 2603, "Co-Owner");

                            AddButton(40, 146, 2151, 2154, 12, GumpButtonType.Reply, 0);
			                AddLabel(75, 151, 149, "Set all in owner Guild as");
                            AddLabel(234, 151, 2603, "Co-Owner");

                            AddButton(40, 177, 2151, 2154, 13, GumpButtonType.Reply, 0);
			                AddLabel(76, 182, 149, "Add");
                            AddLabel(105, 182, 2603, "Co-Owner");

                            AddLabel(333, 87, 2603, "Co-Owners");
                            AddButton(301, 121, 2223, 2223, 14, GumpButtonType.Reply, 0);
			                AddItem(333, 113, 2462);
                            AddButton(396, 121, 2224, 2224, 15, GumpButtonType.Reply, 0);

                            AddLabel(180, 226, 2550, "Remove Co-Owner");
                        break;
                    }  

                    //Left Row
                    AddButton(40, 244, 9721, 9721, 0, GumpButtonType.Reply, 0);
                    AddLabel(75, 248, WhiteTextHue, "Merrill Calder");

                    //Right Row
                    AddButton(280, 244, 9721, 9721, 0, GumpButtonType.Reply, 0);
                    AddLabel(315, 248, WhiteTextHue, "Merrill Calder");

                    AddButton(136, 416, 4014, 4016, 0, GumpButtonType.Reply, 0);			       
			        AddLabel(201, 416, 2599, "Page");
			        AddLabel(237, 416, WhiteTextHue, "10/10");
                    AddButton(306, 416, 4005, 4007, 0, GumpButtonType.Reply, 0);
                break;

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipGumpObject == null) return;

            if (m_ShipGumpObject.m_Ship == null)
            {
                m_Player.SendMessage("That ship no longer is accessible.");
                return;
            }

            if (m_ShipGumpObject.m_Ship.Deleted)
            {
                m_Player.SendMessage("That ship no longer is accessible.");
                return;
            }

            BaseShip ship = m_ShipGumpObject.m_Ship;
            BaseShipDeed deed = m_ShipGumpObject.m_ShipDeed;

            bool closeGump = true;

            #region Footer Tabs

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Overview
                case 2:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Overview;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Upgrades
                case 3:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Upgrades;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Stats
                case 4:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Stats;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //History
                case 5:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.History;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Players
                case 6:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Players;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;
            }

            #endregion

            #region Page Content
            
            switch (m_ShipGumpObject.m_ShipPage)
            {
                #region Overview 

                case ShipPageType.Overview:
                    switch (info.ButtonID)
                    {
                        //Raise Anchor
                        case 10:
                            //Anchor Down
                            if (ship.Anchored)
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.RaiseAnchor(true);
                            }

                            //Anchor Raised
                            else
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.LowerAnchor(true);
                            }

                            closeGump = false;
                        break;

                        //Embark + Disembark
                        case 11:
                            //Disembark
                            if (ship.Contains(m_Player))
                            {
                                if (ship.CanUseCommand(m_Player, false, false, ShipAccessLevelType.None))
                                {
                                    ship.Disembark(m_Player);
                                    return;
                                }                               
                            }

                            //Embark
                            else
                            {
                                if (ship.CanUseCommand(m_Player, false, true, ShipAccessLevelType.Friend))
                                {
                                    ship.Embark(m_Player, false);
                                    return;
                                }                                
                            }

                            closeGump = false;
                        break;

                        //Embark + Disembark Followers
                        case 12:
                            bool followersOnBoard = false;

                            foreach (Mobile follower in m_Player.AllFollowers)
                            {
                                if (ship.Contains(follower))
                                    followersOnBoard = true;
                            }

                            //Disembark Followers
                            if (followersOnBoard)
                            {
                                if (ship.CanUseCommand(m_Player, false, false, ShipAccessLevelType.None))
                                {
                                    ship.DisembarkFollowers(m_Player);
                                    return;
                                }
                            }

                            //Embark Followers
                            else
                            {
                                if (ship.CanUseCommand(m_Player, false, true, ShipAccessLevelType.Friend))
                                {
                                    ship.EmbarkFollowers(m_Player);
                                    return;
                                }
                            }

                            closeGump = false;
                        break;

                        //Clear Deck
                        case 13:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                ship.ClearTheDeck(true);

                            closeGump = false;
                        break;

                        //Throw Overboard
                        case 14:
                            if (ship.CanUseCommand(m_Player, true, false, ShipAccessLevelType.None))
                            {
                                ship.ThrowOverboardCommand(m_Player);
                                return;
                            }

                            closeGump = false;
                        break;

                        //Dock + Place Ship
                        case 15:
                            //Dock Ship
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, false, true, ShipAccessLevelType.CoOwner))
                                {
                                    ship.BeginDryDock(m_Player);
                                    return;
                                }
                            }

                            //Place Ship
                            else if (deed != null)
                            {
                                if (m_Player.Backpack != null)
                                {
                                    if (!deed.IsChildOf(m_Player.Backpack))
                                        m_Player.SendMessage("This ship deed must be in your backpack if you wish to use it.");

                                    else if (!m_Player.Alive)
                                        m_Player.SendMessage("You are dead and cannot use that.");

                                    else
                                    {
                                        m_Player.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
                                        m_Player.Target = new InternalTarget(deed);

                                        return;
                                    }
                                }
                            }

                            closeGump = false;
                        break;

                        //Minor Ability
                        case 16:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))                    
                                ship.ActivateMinorAbility(m_Player);                            

                            closeGump = false;
                        break;

                        //Major Ability
                        case 17:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                ship.ActivateMajorAbility(m_Player);   

                            closeGump = false;
                        break;

                        //Epic Ability
                        case 18:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                ship.ActivateEpicAbility(m_Player);   

                            closeGump = false;
                        break;

                        //Launch Ship Hotbar
                        case 19:
                            ShipHotbarGumpObject shipHotbarGumpObject = new ShipHotbarGumpObject();

                            m_Player.CloseGump(typeof(ShipHotbarGump));
                            m_Player.SendGump(new ShipHotbarGump(m_Player, shipHotbarGumpObject));

                            closeGump = false;
                        break;

                        //Rename Ship
                        case 20:

                            closeGump = false;
                        break;
                    }
                break;

                #endregion

                #region Upgrades

                case ShipPageType.Upgrades:
                    switch (info.ButtonID)
                    {
                        //Theme
                        case 10:
                        break;

                        //Paint
                        case 11:
                        break;

                        //Cannon Metal
                        case 12:
                        break;

                        //Outfitting 
                        case 13:
                        break;

                        //Flag
                        case 14:
                        break;

                        //Charm
                        case 15:
                        break;

                        //Minor Ability
                        case 16:
                        break;

                        //Major Ability
                        case 17:
                        break;

                        //Epic Ability
                        case 18:
                        break;
                    }
                break;

                #endregion

                #region Stats

                case ShipPageType.Stats:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 10:
                            switch (m_ShipGumpObject.m_Page)
                            {
                                case 0: m_ShipGumpObject.m_Page = 1; break;
                                case 1: m_ShipGumpObject.m_Page = 0; break;
                            }

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 11:
                            switch (m_ShipGumpObject.m_Page)
                            {
                                case 0: m_ShipGumpObject.m_Page = 1; break;
                                case 1: m_ShipGumpObject.m_Page = 0; break;
                            }

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }
                break;

                #endregion

                #region History

                case ShipPageType.History:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 10:
                            switch (m_ShipGumpObject.m_Page)
                            {
                                //case 0: m_ShipGumpObject.m_Page = 1; break;
                                //case 1: m_ShipGumpObject.m_Page = 0; break;
                            }

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 11:
                            switch (m_ShipGumpObject.m_Page)
                            {
                                //case 0: m_ShipGumpObject.m_Page = 1; break;
                                //case 1: m_ShipGumpObject.m_Page = 0; break;
                            }

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }
                break;

                #endregion

                #region Players

                case ShipPageType.Players:
                    switch (info.ButtonID)
                    {
                        //Clear All
                        case 10:
                            closeGump = false;
                        break;

                        //Set All on IP
                        case 11:
                            closeGump = false;
                        break;

                        //Set All in Guild
                        case 12:
                            closeGump = false;
                        break;

                        //Add
                        case 13:
                            closeGump = false;
                        break;

                        //Previous Player Type
                        case 14:
                            switch (m_ShipGumpObject.m_PlayersPage)
                            {
                                case PlayersPageType.Friends: m_ShipGumpObject.m_PlayersPage = PlayersPageType.CoOwners; break;
                                case PlayersPageType.CoOwners: m_ShipGumpObject.m_PlayersPage = PlayersPageType.Friends; break;
                            }

                            closeGump = false;
                        break;

                        //Next Player Type
                        case 15:
                            switch (m_ShipGumpObject.m_PlayersPage)
                            {
                                case PlayersPageType.Friends: m_ShipGumpObject.m_PlayersPage = PlayersPageType.CoOwners; break;
                                case PlayersPageType.CoOwners: m_ShipGumpObject.m_PlayersPage = PlayersPageType.Friends; break;
                            }

                            closeGump = false;
                        break;
                        
                    }

                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                        int playerIndex = (info.ButtonID - 20) + (m_ShipGumpObject.m_Page * ShipGump.PlayersPerPage);
                    }

                break;

                #endregion
            }

            #endregion

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipGump));
                m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }

        #region Ship Placement

        private class InternalTarget : MultiTarget
        {
            private BaseShipDeed m_ShipDeed;

            public InternalTarget(BaseShipDeed deed): base(deed.MultiID, deed.Offset)
            {
                m_ShipDeed = deed;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null) return;
                if (player.Backpack == null) return;

                if (m_ShipDeed == null)
                {
                    player.SendMessage("That ship deed is no longer accessible.");
                    return;
                }

                if (m_ShipDeed.Deleted)
                {
                    player.SendMessage("That ship deed is no longer accessible.");
                    return;
                }

                if (m_ShipDeed.IsChildOf(player.Backpack))
                {
                    player.SendMessage("That ship deed is no longer accessible.");
                    return;
                }

                IPoint3D ip = o as IPoint3D;

                if (ip != null)
                {
                    if (ip is Item)
                        ip = ((Item)ip).GetWorldTop();

                    Point3D p = new Point3D(ip);
                   
                    m_ShipDeed.OnPlacement(player, p);
                }
            }
        }

        #endregion
    }

    public class ShipGumpObject
    {
        public PlayerMobile m_Player;
        public BaseShip m_Ship;
        public BaseShipDeed m_ShipDeed;

        public ShipGump.ShipPageType m_ShipPage = ShipGump.ShipPageType.Overview;
        public ShipGump.PlayersPageType m_PlayersPage = ShipGump.PlayersPageType.Friends;
        public int m_Page = 0;

        public ShipGumpObject(PlayerMobile player, BaseShip ship, BaseShipDeed deed)
        {
            m_Player = player;
            m_Ship = ship;
            m_ShipDeed = deed;
        }
    }
}