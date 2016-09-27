using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        public static int PlayersPerSide = 5;

        public ShipGump(PlayerMobile player, ShipGumpObject shipGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ShipGumpObject = shipGumpObject;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipGumpObject == null) return;

            ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(m_ShipGumpObject.m_ShipDeed, m_ShipGumpObject.m_Ship, true, true);

            BaseShip ship = m_ShipGumpObject.m_Ship;
            BaseShipDeed shipDeed = m_ShipGumpObject.m_ShipDeed;

            string shipName = "";    

            bool IsFriend = false;
            bool IsCoOwner = false;
            bool IsOwner = false;
            bool IsInBackpack = false;

            if (ship != null)
            {
                shipName = ship.ShipName;

                IsFriend = ship.IsFriend(m_Player);
                IsCoOwner = ship.IsCoOwner(m_Player);
                IsOwner = ship.IsOwner(m_Player);
            }

            if (shipDeed != null)
            {
                shipName = shipDeed.m_ShipName;

                IsFriend = shipDeed.IsFriend(m_Player);
                IsCoOwner = shipDeed.IsCoOwner(m_Player);
                IsOwner = shipDeed.IsOwner(m_Player);

                if (shipDeed.IsChildOf(m_Player.Backpack))
                    IsInBackpack = true;
            }

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
            AddLabel(Utility.CenteredTextOffset(235, shipName), 5, 149, shipName);

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

            if (IsFriend || IsCoOwner || IsOwner || IsInBackpack)
            {
                AddLabel(377, 449, 2599, "Players");
                AddItem(350, 469, 2462);           

                if (m_ShipGumpObject.m_ShipPage == ShipPageType.Players)
                    AddButton(387, 472, 9724, 9721, 6, GumpButtonType.Reply, 0);
                else
                    AddButton(387, 472, 9721, 9724, 6, GumpButtonType.Reply, 0);
            }

            #endregion

            int hitPoints = 100;
            int maxHullPoints = 100;

            int sailPoints = 100;
            int maxSailPoints = 100;

            int gunPoints = 100;
            int maxGunPoints = 100;

            Type shipType = null;

            ShipUpgradeDetail themeDetail = null;
            ShipUpgradeDetail paintDetail = null;
            ShipUpgradeDetail cannonMetalDetail = null;

            ShipUpgradeDetail outfittingDetail = null;
            ShipUpgradeDetail bannerDetail = null;
            ShipUpgradeDetail charmDetail = null;

            ShipUpgradeDetail minorAbilityDetail = null;
            ShipUpgradeDetail majorAbilityDetail = null;
            ShipUpgradeDetail epicAbilityDetail = null;
            
            if (ship != null)
            {
                shipType = ship.GetType();

                hitPoints = ship.HitPoints;
                maxHullPoints = ship.MaxHitPoints;

                sailPoints = ship.SailPoints;
                maxSailPoints = ship.MaxSailPoints;

                gunPoints = ship.GunPoints;
                maxGunPoints = ship.MaxGunPoints;
            }

            if (shipDeed != null)
            {                
                shipType = shipDeed.ShipType;

                hitPoints = shipDeed.HitPoints;
                maxHullPoints = shipStatsProfile.MaxHitPointsAdjusted;

                sailPoints = shipDeed.SailPoints;
                maxSailPoints = shipStatsProfile.MaxSailPointsAdjusted;

                gunPoints = shipDeed.GunPoints;
                maxGunPoints = shipStatsProfile.MaxGunPointsAdjusted;
            }

            double hullPercent = (double)hitPoints / (double)maxHullPoints;
            double sailPercent = (double)sailPoints / (double)maxSailPoints;
            double gunPercent = (double)gunPoints / (double)maxGunPoints;

            string shipTypeText = shipStatsProfile.ShipTypeName;

            switch (shipGumpObject.m_ShipPage)
            {
                #region Overview

                case ShipPageType.Overview:

                    #region Stats                    

                    AddImage(88, 41, 103);
			        AddImage(225, 41, 103);
			        AddImageTiled(102, 55, 257, 77, 2624);

                    AddLabel(Utility.CenteredTextOffset(232, Utility.Capitalize(shipTypeText)), 37, WhiteTextHue, Utility.Capitalize(shipTypeText));

                    AddLabel(111, 60, 149, "Hull");
                    AddImage(142, 64, 2057);
                    AddImageTiled(142 + Utility.ProgressBarX(hullPercent), 67, Utility.ProgressBarWidth(hullPercent), 7, 2488);
                    AddLabel(260, 60, WhiteTextHue, hitPoints.ToString() + "/" + maxHullPoints.ToString());

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

                    if (ship != null)
                    {
                        minorAbilityDetail = ShipUpgrades.GetMinorAbilityDetail(ship.m_MinorAbilityUpgrade);
                        majorAbilityDetail = ShipUpgrades.GetMajorAbilityDetail(ship.m_MajorAbilityUpgrade);
                        epicAbilityDetail = ShipUpgrades.GetEpicAbilityDetail(ship.m_EpicAbilityUpgrade);
                    }

                    if (shipDeed != null)
                    {
                        minorAbilityDetail = ShipUpgrades.GetMinorAbilityDetail(shipDeed.m_MinorAbilityUpgrade);
                        majorAbilityDetail = ShipUpgrades.GetMajorAbilityDetail(shipDeed.m_MajorAbilityUpgrade);
                        epicAbilityDetail = ShipUpgrades.GetEpicAbilityDetail(shipDeed.m_EpicAbilityUpgrade);
                    }

                    int offsetX = -32;
                    int offsetY = -22;

                    //Minor Ability
                    AddLabel(63, 153, 2599, "Minor Ability");
                    AddImage(62, 197, 2328);
                    if (minorAbilityDetail != null)
                    {
                        string minorAbilityCooldownText = "";

                        if (ship != null)
                        {
                            if (ship.m_MinorAbilityLastActivated + TimeSpan.FromSeconds(ship.m_ShipStatsProfile.MinorAbilityCooldownDuration) <= DateTime.UtcNow)
                                minorAbilityCooldownText = "Ready";

                            else
                                minorAbilityCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, ship.m_MinorAbilityLastActivated + TimeSpan.FromSeconds(ship.m_ShipStatsProfile.MinorAbilityCooldownDuration), true, false, false, true, true);   
                        }

                        if (shipDeed != null)                        
                            minorAbilityCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromSeconds(shipStatsProfile.MinorAbilityCooldownDurationAdjusted), true, false, false, true, true);   
                        
                        AddLabel(Utility.CenteredTextOffset(110, minorAbilityDetail.m_UpgradeName), 173, WhiteTextHue, minorAbilityDetail.m_UpgradeName);
                        AddButton(57, 263, 2151, 2151, 16, GumpButtonType.Reply, 0);
                        AddLabel(94, 267, WhiteTextHue, minorAbilityCooldownText);
                        AddGumpCollection(GumpCollections.GetGumpCollection(minorAbilityDetail.GumpCollectionId, -1), offsetX + 62, offsetY + 197);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(110, "Not Installed"), 173, 2401, "Not Installed");

                    //Major Ability
                    AddLabel(194, 153, 2603, "Major Ability");
                    AddImage(192, 197, 2328);
                    if (majorAbilityDetail != null)
                    {
                        string majorAbilityCooldownText = "";

                        if (ship != null)
                        {
                            if (ship.m_MajorAbilityLastActivated + TimeSpan.FromSeconds(ship.m_ShipStatsProfile.MajorAbilityCooldownDuration) <= DateTime.UtcNow)
                                majorAbilityCooldownText = "Ready";

                            else
                                majorAbilityCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, ship.m_MajorAbilityLastActivated + TimeSpan.FromSeconds(ship.m_ShipStatsProfile.MajorAbilityCooldownDuration), true, false, false, true, true);
                        }

                        if (shipDeed != null)
                            majorAbilityCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromSeconds(shipStatsProfile.MajorAbilityCooldownDurationAdjusted), true, false, false, true, true);   

                        AddLabel(Utility.CenteredTextOffset(239, majorAbilityDetail.m_UpgradeName), 173, WhiteTextHue, majorAbilityDetail.m_UpgradeName);
                        AddButton(187, 263, 2151, 2151, 17, GumpButtonType.Reply, 0);
                        AddLabel(220, 267, WhiteTextHue, majorAbilityCooldownText);
                        AddGumpCollection(GumpCollections.GetGumpCollection(majorAbilityDetail.GumpCollectionId, -1), offsetX + 192, offsetY + 197);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(240, "Not Installed"), 173, 2401, "Not Installed");

                    //Epic Ability
                    AddLabel(327, 153, 2606, "Epic Ability");
                    AddImage(320, 197, 2328);
                    if (epicAbilityDetail != null)
                    {
                        string epicAbilityCooldownText = "";

                        if (ship != null)
                        {
                            if (ship.m_EpicAbilityLastActivated + TimeSpan.FromSeconds(ship.m_ShipStatsProfile.EpicAbilityCooldownDuration) <= DateTime.UtcNow)
                                epicAbilityCooldownText = "Ready";

                            else
                                epicAbilityCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, ship.m_EpicAbilityLastActivated + TimeSpan.FromSeconds(ship.m_ShipStatsProfile.EpicAbilityCooldownDuration), true, false, false, true, true);
                        }

                        if (shipDeed != null)
                            epicAbilityCooldownText = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromSeconds(shipStatsProfile.EpicAbilityCooldownDurationAdjusted), true, false, false, true, true);   

                        AddLabel(Utility.CenteredTextOffset(370, epicAbilityDetail.m_UpgradeName), 173, WhiteTextHue, epicAbilityDetail.m_UpgradeName);
                        AddButton(315, 263, 2151, 2151, 18, GumpButtonType.Reply, 0);
                        AddLabel(349, 267, WhiteTextHue, epicAbilityCooldownText);
                        AddGumpCollection(GumpCollections.GetGumpCollection(epicAbilityDetail.GumpCollectionId, -1), offsetX + 320, offsetY + 197);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(370, "Not Installed"), 173, 2401, "Not Installed");

                    #endregion

                    #region Middle                   

                    if (ship != null)
                    {
                          string doubloonCount = "0"; //TEST

                        AddLabel(102, 298, 149, "Doubloons in Hold");
                        AddItem(104, 321, 2539);
                        AddLabel(142, 318, WhiteTextHue, doubloonCount);
                    }

                    if (ship != null)
                    {
                        if (ship.Hold != null)
                        {
                            string itemCount = ship.Hold.TotalItems.ToString() + "/" + ship.Hold.MaxItems.ToString();

                            AddLabel(259, 298, 149, "Items in Hold");
                            AddLabel(Utility.CenteredTextOffset(297, itemCount), 318, WhiteTextHue, itemCount);
                        }
                    }

                    if (shipDeed != null)
                    {
                        AddLabel(203, 298, 149, "Hold Size");
                        AddLabel(Utility.CenteredTextOffset(233, shipStatsProfile.HoldSize.ToString()), 318, WhiteTextHue, shipStatsProfile.HoldSize.ToString());
                    }

                    #endregion
                    
                    #region Actions

                    //Hotbar
                    AddButton(38, 344, 4011, 4013, 19, GumpButtonType.Reply, 0);
                    if (ship != null && (IsFriend || IsCoOwner || IsOwner))  
                        AddLabel(75, 344, WhiteTextHue, "Open Ship Hotbar");  
                    else
                        AddLabel(75, 344, 2401, "Open Ship Hotbar");  
                    
                    //Raise Anchor
                    AddButton(38, 369, 4014, 4016, 10, GumpButtonType.Reply, 0);

                    if (ship != null)
                    {
                        if (ship.Anchored)
                        {
                            if ((IsCoOwner || IsOwner))
                                AddLabel(75, 369, WhiteTextHue, "Raise Anchor");
                            else
                                AddLabel(75, 369, 2401, "Raise Anchor");
                        }

                        else
                        {
                            if ((IsCoOwner || IsOwner))
                                AddLabel(75, 369, WhiteTextHue, "Lower Anchor");
                            else
                                AddLabel(75, 369, 2401, "Lower Anchor");
                        }
                    }

                    else
                        AddLabel(75, 369, 2401, "Raise Anchor");                  
                    
                    //Embark + Disembark
                    AddButton(38, 394, 4002, 4004, 11, GumpButtonType.Reply, 0);
                    if (ship != null)
                        AddLabel(75, 394, WhiteTextHue, "Embark/Disembark");
                    else
                        AddLabel(75, 394, 2401, "Embark/Disembark");

                    //Embark + Disembark Followers
                    AddButton(38, 419, 4008, 4010, 12, GumpButtonType.Reply, 0);
                    if (ship != null)
                        AddLabel(75, 419, WhiteTextHue, "Embark/Disembark Followers");
                    else
                        AddLabel(75, 419, 2401, "Embark/Disembark Followers");

                    //Rename Ship
                    AddButton(275, 344, 4026, 4028, 20, GumpButtonType.Reply, 0);
                    if (IsOwner || IsInBackpack)                                               
                        AddLabel(313, 342, WhiteTextHue, "Rename Ship");
                    else
                        AddLabel(313, 342, 2401, "Rename Ship");                    

                    //Throw Overboard
                    AddButton(275, 369, 4029, 4031, 14, GumpButtonType.Reply, 0);
                    if (ship != null)
                        AddLabel(313, 369, WhiteTextHue, "Throw Overboard");
                    else
                        AddLabel(313, 369, 2401, "Throw Overboard");                    

                    //Clear the Decks                 
                    AddButton(275, 394, 4020, 4022, 13, GumpButtonType.Reply, 0);

                    if (ship != null && (IsCoOwner || IsOwner))                                                    
                        AddLabel(313, 394, WhiteTextHue, "Clear The Decks");  
                    else
                        AddLabel(313, 394, 2401, "Clear The Decks");                      

                    //Dock + Launch Ship
                    if (ship == null)
                    {
                        AddButton(276, 419, 4017, 4019, 15, GumpButtonType.Reply, 0);
                        if (IsInBackpack)     
                            AddLabel(313, 419, WhiteTextHue, "Launch The Ship");
                        else
                            AddLabel(313, 419, 2401, "Launch The Ship");
                    }

                    else
                    {
                        AddButton(276, 419, 4017, 4019, 15, GumpButtonType.Reply, 0);
                        if (IsOwner || IsCoOwner)    
                            AddLabel(313, 419, WhiteTextHue, "Dock The Ship");
                        else
                            AddLabel(313, 419, 2401, "Dock The Ship");
                    }

                    #endregion
                break;

                #endregion

                #region Upgrades                

                case ShipPageType.Upgrades:
                    if (ship != null)
                    {
                        themeDetail = ShipUpgrades.GetThemeDetail(ship.m_ThemeUpgrade);
                        paintDetail = ShipUpgrades.GetPaintDetail(ship.m_PaintUpgrade);
                        cannonMetalDetail = ShipUpgrades.GetCannonMetalDetail(ship.m_CannonMetalUpgrade);

                        outfittingDetail = ShipUpgrades.GetOutfittingDetail(ship.m_OutfittingUpgrade);
                        bannerDetail = ShipUpgrades.GetBannerDetail(ship.m_BannerUpgrade);
                        charmDetail = ShipUpgrades.GetCharmDetail(ship.m_CharmUpgrade);

                        minorAbilityDetail = ShipUpgrades.GetMinorAbilityDetail(ship.m_MinorAbilityUpgrade);
                        majorAbilityDetail = ShipUpgrades.GetMajorAbilityDetail(ship.m_MajorAbilityUpgrade);
                        epicAbilityDetail = ShipUpgrades.GetEpicAbilityDetail(ship.m_EpicAbilityUpgrade);
                    }

                    if (shipDeed != null)
                    {
                        themeDetail = ShipUpgrades.GetThemeDetail(shipDeed.m_ThemeUpgrade);
                        paintDetail = ShipUpgrades.GetPaintDetail(shipDeed.m_PaintUpgrade);
                        cannonMetalDetail = ShipUpgrades.GetCannonMetalDetail(shipDeed.m_CannonMetalUpgrade);

                        outfittingDetail = ShipUpgrades.GetOutfittingDetail(shipDeed.m_OutfittingUpgrade);
                        bannerDetail = ShipUpgrades.GetBannerDetail(shipDeed.m_BannerUpgrade);
                        charmDetail = ShipUpgrades.GetCharmDetail(shipDeed.m_CharmUpgrade);

                        minorAbilityDetail = ShipUpgrades.GetMinorAbilityDetail(shipDeed.m_MinorAbilityUpgrade);
                        majorAbilityDetail = ShipUpgrades.GetMajorAbilityDetail(shipDeed.m_MajorAbilityUpgrade);
                        epicAbilityDetail = ShipUpgrades.GetEpicAbilityDetail(shipDeed.m_EpicAbilityUpgrade);
                    }

                    offsetX = -32;
                    offsetY = -22;

                    AddLabel(82, 38, 145, "Theme");
                    AddImage(58, 80, 2328);
                    if (themeDetail != null)
                    {
                        AddButton(74, 143, 2117, 2118, 10, GumpButtonType.Reply, 0);
                        AddLabel(94, 140, 2550, "Info");                       
                        AddLabel(Utility.CenteredTextOffset(105, themeDetail.m_UpgradeName), 58, WhiteTextHue, themeDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(themeDetail.GumpCollectionId, -1), offsetX + 58, offsetY + 80);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(105, "Not Installed"), 58, 2401, "Not Installed");

                    AddLabel(215, 38, 2578, "Paint");
                    AddImage(190, 80, 2328);
                    if (paintDetail != null)
                    {
                        AddButton(206, 143, 2117, 2118, 11, GumpButtonType.Reply, 0);
                        AddLabel(226, 140, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(237, paintDetail.m_UpgradeName), 58, WhiteTextHue, paintDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(paintDetail.GumpCollectionId, -1), offsetX + 190, offsetY + 80);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(240, "Not Installed"), 58, 2401, "Not Installed");

                    AddLabel(319, 38, 2301, "Cannon Metal");
                    AddImage(318, 80, 2328);
                    if (cannonMetalDetail != null)
                    {
                        AddButton(334, 143, 2117, 2118, 12, GumpButtonType.Reply, 0);
                        AddLabel(354, 140, 2550, "Info");                    
                        AddLabel(Utility.CenteredTextOffset(368, cannonMetalDetail.m_UpgradeName), 58, WhiteTextHue, cannonMetalDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(cannonMetalDetail.GumpCollectionId, -1), offsetX + 318, offsetY + 80);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(370, "Not Installed"), 58, 2401, "Not Installed");

                    AddLabel(70, 176, 2550, "Outfitting");
                    AddImage(58, 218, 2328);
                    if (outfittingDetail != null)
                    {
                        AddButton(74, 281, 2117, 2117, 13, GumpButtonType.Reply, 0);
                        AddLabel(94, 278, 2550, "Info");                      
                        AddLabel(Utility.CenteredTextOffset(105, outfittingDetail.m_UpgradeName), 196, WhiteTextHue, outfittingDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(outfittingDetail.GumpCollectionId, -1), offsetX + 58, offsetY + 218);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(105, "Not Installed"), 196, 2401, "Not Installed");

                    AddLabel(211, 176, 2114, "Banner");
                    AddImage(190, 218, 2328);
                    if (bannerDetail != null)
                    {
                        AddButton(206, 281, 2117, 2117, 14, GumpButtonType.Reply, 0);
                        AddLabel(226, 278, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(237, bannerDetail.m_UpgradeName), 196, WhiteTextHue, bannerDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(bannerDetail.GumpCollectionId, -1), offsetX + 190, offsetY + 218);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(240, "Not Installed"), 196, 2401, "Not Installed");

                    AddLabel(339, 176, 2617, "Charm");
                    AddImage(318, 218, 2328);
                    if (charmDetail != null)
                    {
                        AddButton(334, 281, 2117, 2117, 15, GumpButtonType.Reply, 0);
                        AddLabel(354, 278, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(368, charmDetail.m_UpgradeName), 196, WhiteTextHue, charmDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(charmDetail.GumpCollectionId, -1), offsetX + 318, offsetY + 218);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(370, "Not Installed"), 196, 2401, "Not Installed");

                    AddLabel(59, 316, 2599, "Minor Ability");
                    AddImage(58, 358, 2328);
                    if (minorAbilityDetail != null)
                    {
                        AddButton(74, 421, 2117, 2117, 16, GumpButtonType.Reply, 0);
                        AddLabel(94, 418, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(105, minorAbilityDetail.m_UpgradeName), 336, WhiteTextHue, minorAbilityDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(minorAbilityDetail.GumpCollectionId, -1), offsetX + 58, offsetY + 358);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(105, "Not Installed"), 336, 2401, "Not Installed");

                    AddLabel(191, 316, 2603, "Major Ability");
                    AddImage(190, 358, 2328);
                    if (majorAbilityDetail != null)
                    {
                        AddButton(206, 421, 2117, 2117, 17, GumpButtonType.Reply, 0);
                        AddLabel(226, 418, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(237, majorAbilityDetail.m_UpgradeName), 336, WhiteTextHue, majorAbilityDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(majorAbilityDetail.GumpCollectionId, -1), offsetX + 190, offsetY + 358);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(240, "Not Installed"), 336, 2401, "Not Installed");

                    AddLabel(326, 316, 2606, "Epic Ability");
			        AddImage(318, 358, 2328);
                    if (epicAbilityDetail != null)
                    {
                        AddButton(334, 421, 2117, 2117, 18, GumpButtonType.Reply, 0);
                        AddLabel(354, 418, 2550, "Info");
                        AddLabel(Utility.CenteredTextOffset(368, epicAbilityDetail.m_UpgradeName), 336, WhiteTextHue, epicAbilityDetail.m_UpgradeName);
                        AddGumpCollection(GumpCollections.GetGumpCollection(epicAbilityDetail.GumpCollectionId, -1), offsetX + 318, offsetY + 358);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(370, "Not Installed"), 336, 2401, "Not Installed");
			        			        
                break;

                #endregion

                #region Stats

                case ShipPageType.Stats:
                    double UpgradeDoubloonMultiplier = shipStatsProfile.UpgradeDoubloonMultiplier;

                    int HoldSize = shipStatsProfile.HoldSizeAdjusted;

                    int MaxHitPoints = shipStatsProfile.MaxHitPointsAdjusted;
                    int MaxSailPoints = shipStatsProfile.MaxSailPointsAdjusted;
                    int MaxGunPoints = shipStatsProfile.MaxGunPointsAdjusted;

                    double ForwardSpeed = (1.0 / shipStatsProfile.ForwardSpeedAdjusted);
                    double DriftSpeed = (1.0 / shipStatsProfile.DriftSpeedAdjusted);
                    double SlowdownModePenalty = shipStatsProfile.SlowdownModePenaltyAdjusted;

                    double CannonAccuracy = shipStatsProfile.CannonAccuracyAdjusted;
                    double CannonMinDamage = shipStatsProfile.CannonMinDamageAdjusted;
                    double CannonMaxDamage = shipStatsProfile.CannonMaxDamageAdjusted;
                    double CannonRange = shipStatsProfile.CannonRangeAdjusted;
                    double CannonReloadTime = shipStatsProfile.CannonReloadDurationAdjusted;     

                    double MinorAbilityCooldown = shipStatsProfile.MinorAbilityCooldownDurationAdjusted;
                    double MajorAbilityCooldown = shipStatsProfile.MajorAbilityCooldownDurationAdjusted;
                    double EpicAbilityCooldown = shipStatsProfile.EpicAbilityCooldownDurationAdjusted;

                    double CoardingChance = shipStatsProfile.BoardingChanceAdjusted;
                    double RepairCooldownDuration = shipStatsProfile.RepairCooldownDurationAdjusted;

                    int textHue = WhiteTextHue;
                    int positiveTextHue = 63;
                    int negativeTextHue = 1256;

                    //TEST: REDO ALL THIS

                    switch (m_ShipGumpObject.m_Page)
                    {
                        case 0:
                            //Type
                            AddLabel(145, 32, 2625, "Ship Type:");
                            AddLabel(319, 32, WhiteTextHue, shipStatsProfile.ShipTypeName);

                            //Stats
                            if (shipStatsProfile.MaxHitPointsAdjusted > shipStatsProfile.MaxHitPoints) textHue = positiveTextHue;
                            else if (shipStatsProfile.MaxHitPointsAdjusted < shipStatsProfile.MaxHitPoints) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 62, 149, "Hull Max Points:");
                            AddLabel(319, 62, textHue, MaxHitPoints.ToString());

                            if (shipStatsProfile.MaxSailPointsAdjusted > shipStatsProfile.MaxSailPoints) textHue = positiveTextHue;
                            else if (shipStatsProfile.MaxSailPointsAdjusted < shipStatsProfile.MaxSailPoints) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 82, 149, "Sail Max Points:");
                            AddLabel(319, 82, textHue, MaxSailPoints.ToString());

                            if (shipStatsProfile.MaxGunPointsAdjusted > shipStatsProfile.MaxGunPoints) textHue = positiveTextHue;
                            else if (shipStatsProfile.MaxGunPointsAdjusted < shipStatsProfile.MaxGunPoints) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 102, 149, "Gun Max Points:");
                            AddLabel(319, 102, textHue, MaxGunPoints.ToString());

                            //Speed
                            if (shipStatsProfile.ForwardSpeedAdjusted > shipStatsProfile.ForwardSpeed) textHue = positiveTextHue;
                            else if (shipStatsProfile.ForwardSpeedAdjusted < shipStatsProfile.ForwardSpeed) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 132, 2599, "Forward Speed:");
                            AddLabel(319, 132, textHue, Utility.CreateDecimalString(ForwardSpeed, 1) + " tiles/sec");
                            
                            if (shipStatsProfile.DriftSpeedAdjusted > shipStatsProfile.DriftSpeed) textHue = positiveTextHue;
                            else if (shipStatsProfile.DriftSpeedAdjusted < shipStatsProfile.DriftSpeed) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 152, 2599, "Drift Speed:");
                            AddLabel(319, 152, textHue, Utility.CreateDecimalString(DriftSpeed, 1) + " tiles/sec");

                            if (shipStatsProfile.SlowdownModePenaltyAdjusted > shipStatsProfile.SlowdownModePenalty) textHue = positiveTextHue;
                            else if (shipStatsProfile.SlowdownModePenaltyAdjusted < shipStatsProfile.SlowdownModePenalty) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 172, 2599, "Slowdown Mode Penalty:");
                            AddLabel(319, 172, textHue, "-" + Utility.CreateDecimalPercentageString(SlowdownModePenalty, 1));

                            //Cannons
                            AddLabel(145, 202, 2401, "Cannons Per Side:");
                            AddLabel(319, 202, WhiteTextHue, shipStatsProfile.CannonsPerSide.ToString());

                            if (shipStatsProfile.CannonAccuracyAdjusted > shipStatsProfile.CannonAccuracy) textHue = positiveTextHue;
                            else if (shipStatsProfile.CannonAccuracyAdjusted < shipStatsProfile.CannonAccuracy) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 222, 2401, "Cannon Accuracy:");
                            AddLabel(319, 222, textHue, Utility.CreateDecimalPercentageString(CannonAccuracy, 1));

                            if (shipStatsProfile.CannonMaxDamageAdjusted > shipStatsProfile.CannonMaxDamage) textHue = positiveTextHue;
                            else if (shipStatsProfile.CannonMaxDamageAdjusted < shipStatsProfile.CannonMaxDamage) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 242, 2401, "Cannon Damage:");
                            AddLabel(319, 242, textHue, Utility.CreateDecimalString(CannonMinDamage, 1) + " - " + Utility.CreateDecimalString(CannonMaxDamage, 1));

                            if (shipStatsProfile.CannonRangeAdjusted > shipStatsProfile.CannonRange) textHue = positiveTextHue;
                            else if (shipStatsProfile.CannonRangeAdjusted < shipStatsProfile.CannonRange) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 262, 2401, "Cannon Range:");
                            AddLabel(319, 262, textHue, Utility.CreateDecimalPercentageString(CannonRange, 1));

                            if (shipStatsProfile.CannonReloadDurationAdjusted > shipStatsProfile.CannonReloadDuration) textHue = positiveTextHue;
                            else if (shipStatsProfile.CannonReloadDurationAdjusted < shipStatsProfile.CannonReloadDuration) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 282, 2401, "Cannon Reload Time:");
                            AddLabel(319, 282, textHue, Utility.CreateDecimalString(CannonReloadTime, 1) + " sec");
			                
			                //Abilities                 
                            if (shipStatsProfile.MinorAbilityCooldownDurationAdjusted > shipStatsProfile.MinorAbilityCooldownDuration) textHue = positiveTextHue;
                            else if (shipStatsProfile.MinorAbilityCooldownDurationAdjusted < shipStatsProfile.MinorAbilityCooldownDuration) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 332, 2603, "Minor Ability Cooldown:");
                            AddLabel(319, 332, textHue, Utility.CreateDecimalString(MinorAbilityCooldown, 1) + " sec");

                            if (shipStatsProfile.MajorAbilityCooldownDurationAdjusted > shipStatsProfile.MajorAbilityCooldownDuration) textHue = positiveTextHue;
                            else if (shipStatsProfile.MajorAbilityCooldownDurationAdjusted < shipStatsProfile.MajorAbilityCooldownDuration) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 352, 2603, "Major Ability Cooldown:");
                            AddLabel(319, 352, textHue, Utility.CreateDecimalString(MajorAbilityCooldown, 1) + " sec");

                            if (shipStatsProfile.EpicAbilityCooldownDurationAdjusted > shipStatsProfile.EpicAbilityCooldownDuration) textHue = positiveTextHue;
                            else if (shipStatsProfile.EpicAbilityCooldownDurationAdjusted < shipStatsProfile.EpicAbilityCooldownDuration) textHue = negativeTextHue;
                            else textHue = WhiteTextHue; 
                            AddLabel(145, 372, 2603, "Epic Ability Cooldown:");
                            AddLabel(319, 372, textHue, Utility.CreateDecimalString(EpicAbilityCooldown, 1) + " sec");

                            //AddLabel(145, 312, 2603, "Repair Cooldown:");
                            //AddLabel(319, 312, WhiteTextHue, "120 sec");

                            //AddLabel(145, 312, 2603, "Boarding Success:");
                            //AddLabel(319, 312, WhiteTextHue, "120 sec");

                            //Doubloons
                            //AddLabel(145, 402, 53, "Upgrade Cost Multiplier:");
                            //AddLabel(319, 402, WhiteTextHue, Utility.CreateDecimalString(UpgradeDoubloonMultiplier, 1) + "x");
			                
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
                            /*
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
                            */
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
                    string shipOwnerText = "Luthius"; //TEST

                    bool guildAsFriends = false;
                    bool ipAsFriends = false;
                    bool guildAsCoOwners = false;
                    bool ipAsCoOwners = false;
                    
                    if (ship != null)
                    {
                        if (ship.Owner != null)
                            shipOwnerText = ship.Owner.RawName;

                        guildAsFriends = ship.GuildAsFriends;
                        ipAsFriends = ship.IPAsFriends;
                        guildAsCoOwners = ship.GuildAsCoOwners;
                        ipAsCoOwners = ship.GuildAsCoOwners;

                        switch (m_ShipGumpObject.m_PlayersPage)
                        {
                            case PlayersPageType.Friends: m_ShipGumpObject.m_PlayerList = ship.Friends; break;
                            case PlayersPageType.CoOwners: m_ShipGumpObject.m_PlayerList = ship.CoOwners; break;
                        }
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_Owner != null)
                            shipOwnerText = shipDeed.m_Owner.RawName;

                        guildAsFriends = shipDeed.m_GuildAsFriends;
                        ipAsFriends = shipDeed.m_IPAsFriends;
                        guildAsCoOwners = shipDeed.m_GuildAsCoOwners;
                        ipAsCoOwners = shipDeed.m_IPAsCoOwners;

                        switch (m_ShipGumpObject.m_PlayersPage)
                        {
                            case PlayersPageType.Friends: m_ShipGumpObject.m_PlayerList = shipDeed.m_Friends; break;
                            case PlayersPageType.CoOwners: m_ShipGumpObject.m_PlayerList = shipDeed.m_CoOwners; break;
                        }
                    }

                    AddLabel(Utility.CenteredTextOffset(235, "Owned by " + shipOwnerText), 34, WhiteTextHue, "Owned by " + shipOwnerText);
                    
                    switch (m_ShipGumpObject.m_PlayersPage)
                    {
                        case PlayersPageType.Friends:
                            AddButton(41, 83, 2472, 2474, 10, GumpButtonType.Reply, 0);
                            AddLabel(74, 87, 1256, "Clear Entire Friends List");

                            if (ipAsFriends)
                                AddButton(40, 113, 2154, 2151, 11, GumpButtonType.Reply, 0);
                            else
                                AddButton(40, 113, 2151, 2154, 11, GumpButtonType.Reply, 0);
			                AddLabel(75, 116, 149, "Set all on owner IP as");
                            AddLabel(221, 116, 2599, "Friends");

                            if (guildAsFriends)
                                AddButton(40, 146, 2154, 2151, 12, GumpButtonType.Reply, 0);
                            else
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

                            if (m_ShipGumpObject.m_PlayerList.Count > 0)
                                AddLabel(180, 226, 2550, "Remove Friend");
                        break;

                        case PlayersPageType.CoOwners:
                            AddButton(41, 83, 2472, 2474, 10, GumpButtonType.Reply, 0);
                            AddLabel(74, 87, 1256, "Clear Entire Co-Owners List");

                            if (ipAsCoOwners)
                                AddButton(40, 113, 2154, 2151, 11, GumpButtonType.Reply, 0);
                            else
                                AddButton(40, 113, 2151, 2154, 11, GumpButtonType.Reply, 0);
			                AddLabel(75, 116, 149, "Set all on owner IP as");
                            AddLabel(221, 116, 2603, "Co-Owner");

                            if (guildAsCoOwners)
                                AddButton(40, 146, 2154, 2151, 12, GumpButtonType.Reply, 0);
                            else
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

                            if (m_ShipGumpObject.m_PlayerList.Count > 0)
                                AddLabel(180, 226, 2550, "Remove Co-Owner");
                        break;
                    }

                    int totalPlayers = m_ShipGumpObject.m_PlayerList.Count;
                    int totalPlayerPages = (int)(Math.Ceiling((double)totalPlayers / (double)PlayersPerPage));

                    if (m_ShipGumpObject.m_Page >= totalPlayerPages)
                        m_ShipGumpObject.m_Page = totalPlayerPages - 1;

                    if (m_ShipGumpObject.m_Page < 0)
                        m_ShipGumpObject.m_Page = 0;

                    int playerStartIndex = m_ShipGumpObject.m_Page * PlayersPerPage;
                    int playerEndIndex = (m_ShipGumpObject.m_Page * PlayersPerPage) + (PlayersPerPage - 1);

                    if (playerEndIndex >= totalPlayers)
                        playerEndIndex = totalPlayers - 1;

                    int playerDisplayCount = playerEndIndex - playerStartIndex;

                    int rowSpacing = 50;

                    int leftStartX = 75;
                    int leftStartY = 248;

                    int rightStartX = 315;
                    int rightStartY = 248;

                    for (int a = 0; a < playerDisplayCount + 1; a++)
                    {
                        if (totalPlayers == 0)
                            continue;

                        int playerIndex = playerStartIndex + a;

                        if (playerIndex >= totalPlayers)
                            continue;

                        PlayerMobile targetPlayer = m_ShipGumpObject.m_PlayerList[playerIndex] as PlayerMobile;

                        if (targetPlayer == null)
                            continue;

                        //Left Side
                        if (a < PlayersPerSide)
                        {
                            AddButton(leftStartX - 35, leftStartY - 4, 9721, 9721, 20 + a, GumpButtonType.Reply, 0);
                            AddLabel(leftStartX, leftStartY, WhiteTextHue, targetPlayer.RawName);

                            leftStartY += rowSpacing;
                        }

                        //Right Side
                        else
                        {
                            AddButton(rightStartX - 35, rightStartY - 4, 9721, 9721, 20 + a, GumpButtonType.Reply, 0);
                            AddLabel(rightStartX, rightStartY, WhiteTextHue, targetPlayer.RawName);

                            rightStartY += rowSpacing;
                        }
                    }

                    if (m_ShipGumpObject.m_Page > 0)
                        AddButton(136, 416, 4014, 4016, 16, GumpButtonType.Reply, 0);

                    if (m_ShipGumpObject.m_Page > 0 || m_ShipGumpObject.m_Page < totalPlayerPages - 1)
                    {
                        AddLabel(201, 416, 2599, "Page");
                        AddLabel(237, 416, WhiteTextHue, m_ShipGumpObject.m_Page.ToString() + "/" + totalPlayerPages.ToString());
                    }

                    if (m_ShipGumpObject.m_Page < totalPlayerPages - 1)
                        AddButton(306, 416, 4005, 4007, 17, GumpButtonType.Reply, 0);
                break;

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Player.Backpack == null) return;
            if (m_ShipGumpObject == null) return;
            
            BaseShip ship = m_ShipGumpObject.m_Ship;
            BaseShipDeed shipDeed = m_ShipGumpObject.m_ShipDeed;

            bool IsFriend = false;
            bool IsCoOwner = false;
            bool IsOwner = false;
            bool IsOnBoard = false;
            bool IsInBackpack = false;

            string shipNameText = "";

            if (ship != null)
            {
                shipNameText = ship.ShipName;

                IsFriend = ship.IsFriend(m_Player);
                IsCoOwner = ship.IsCoOwner(m_Player);
                IsOwner = ship.IsOwner(m_Player);
                IsOnBoard = ship.Contains(m_Player);
            }

            if (shipDeed != null)
            {
                shipNameText = shipDeed.m_ShipName;

                IsFriend = shipDeed.IsFriend(m_Player);
                IsCoOwner = shipDeed.IsCoOwner(m_Player);
                IsOwner = shipDeed.IsOwner(m_Player);

                if (shipDeed.IsChildOf(m_Player.Backpack))
                    IsInBackpack = true;
            }

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
                    m_ShipGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Upgrades
                case 3:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Upgrades;
                    m_ShipGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Stats
                case 4:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Stats;
                    m_ShipGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //History
                case 5:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.History;
                    m_ShipGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Players
                case 6:
                    if (IsFriend || IsCoOwner || IsOwner || IsInBackpack)
                    {
                        m_ShipGumpObject.m_ShipPage = ShipPageType.Players;
                        m_ShipGumpObject.m_Page = 0;

                        m_Player.SendSound(ChangePageSound);
                    }

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
                            if (ship != null)
                            {
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
                            }

                            closeGump = false;
                        break;

                        //Embark + Disembark
                        case 11:
                            if (ship != null)
                            {
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
                            }

                            closeGump = false;
                        break;

                        //Embark + Disembark Followers
                        case 12:
                            if (ship != null)
                            {
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
                            }

                            closeGump = false;
                        break;

                        //Clear Deck
                        case 13:
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.ClearTheDeck(true);
                            }

                            closeGump = false;
                        break;

                        //Throw Overboard
                        case 14:
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, true, false, ShipAccessLevelType.None))
                                {
                                    ship.ThrowOverboardCommand(m_Player);
                                    return;
                                }
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
                            else if (shipDeed != null)
                            {                                
                                if (!IsInBackpack)
                                    m_Player.SendMessage("This ship deed must be in your backpack if you wish to use it.");

                                else if (!m_Player.Alive)
                                    m_Player.SendMessage("You are dead and cannot use that.");

                                else
                                {
                                    m_Player.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
                                    m_Player.Target = new InternalTarget(shipDeed);

                                    return;
                                }                               
                            }

                            closeGump = false;
                        break;

                        //Minor Ability
                        case 16:
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.ActivateMinorAbility(m_Player);
                            }

                            closeGump = false;
                        break;

                        //Major Ability
                        case 17:
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.ActivateMajorAbility(m_Player);
                            }

                            closeGump = false;
                        break;

                        //Epic Ability
                        case 18:
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.ActivateEpicAbility(m_Player);
                            }

                            closeGump = false;
                        break;

                        //Launch Ship Hotbar
                        case 19:
                            if (ship != null)
                            {
                                ShipHotbarGumpObject shipHotbarGumpObject = new ShipHotbarGumpObject();

                                m_Player.CloseGump(typeof(ShipHotbarGump));
                                m_Player.SendGump(new ShipHotbarGump(m_Player, shipHotbarGumpObject));
                            }

                            closeGump = false;
                        break;

                        //Rename Ship
                        case 20:
                            if (IsOwner || IsInBackpack)
                            {
                                m_Player.CloseGump(typeof(ShipGump));
                                m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                                m_Player.CloseGump(typeof(ShipRenameGump));
                                m_Player.SendGump(new ShipRenameGump(m_Player, m_ShipGumpObject, shipNameText));
                            }

                            return;
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
                            ShipUpgradeGumpObject shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;

                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.Theme;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_Theme = ship.m_ThemeUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_Theme = shipDeed.m_ThemeUpgrade;
                            }

                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Paint
                        case 11:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.Paint;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_Paint = ship.m_PaintUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_Paint = shipDeed.m_PaintUpgrade;
                            }

                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Cannon Metal
                        case 12:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.CannonMetal;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_CannonMetal = ship.m_CannonMetalUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_CannonMetal = shipDeed.m_CannonMetalUpgrade;
                            }
                            
                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Outfitting 
                        case 13:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.Outfitting;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_Outfitting = ship.m_OutfittingUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_Outfitting = shipDeed.m_OutfittingUpgrade;
                            }
                            
                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Banner
                        case 14:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.Banner;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_Banner = ship.m_BannerUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_Banner = shipDeed.m_BannerUpgrade;
                            }
                            
                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Charm
                        case 15:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.Charm;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_Charm = ship.m_CharmUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_Charm = shipDeed.m_CharmUpgrade;
                            }
                            
                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Minor Ability
                        case 16:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.MinorAbility;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_MinorAbility = ship.m_MinorAbilityUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_MinorAbility = shipDeed.m_MinorAbilityUpgrade;
                            }
                            
                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Major Ability
                        case 17:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.MajorAbility;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_MajorAbility = ship.m_MajorAbilityUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_MajorAbility = shipDeed.m_MajorAbilityUpgrade;
                            }
                            
                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
                        break;

                        //Epic Ability
                        case 18:
                            shipUgradeGumpObject = new ShipUpgradeGumpObject();

                            shipUgradeGumpObject.m_Ship = ship;
                            shipUgradeGumpObject.m_ShipDeed = shipDeed;
                            
                            shipUgradeGumpObject.m_UpgradeType = ShipUpgrades.UpgradeType.EpicAbility;
                            shipUgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.InstalledOnShip;

                            if (ship != null)
                            {                                
                                shipUgradeGumpObject.m_ShipType = ship.GetType();
                                shipUgradeGumpObject.m_EpicAbility = ship.m_EpicAbilityUpgrade;
                            }

                            if (shipDeed != null)
                            {
                                shipUgradeGumpObject.m_ShipType = shipDeed.ShipType;
                                shipUgradeGumpObject.m_EpicAbility = shipDeed.m_EpicAbilityUpgrade;
                            }

                            m_Player.CloseGump(typeof(ShipGump));
                            m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                            m_Player.CloseGump(typeof(ShipUpgradeGump));
                            m_Player.SendGump(new ShipUpgradeGump(m_Player, shipUgradeGumpObject));

                            m_Player.SendSound(OpenGumpSound);

                            return;
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
                            if (IsOwner || IsInBackpack)
                            {
                                switch (m_ShipGumpObject.m_PlayersPage)
                                {
                                    case PlayersPageType.Friends:
                                        if (ship != null)                                        
                                            ship.Friends.Clear();

                                        if (shipDeed != null)
                                            shipDeed.m_Friends.Clear();

                                        m_Player.SendMessage("Ships friend list cleared.");
                                    break;

                                    case PlayersPageType.CoOwners:
                                        if (ship != null)                                        
                                            ship.CoOwners.Clear();

                                        if (shipDeed != null)
                                            shipDeed.m_CoOwners.Clear();

                                        m_Player.SendMessage("Ships co-owner list cleared.");
                                    break;
                                }
                            }

                            closeGump = false;
                        break;

                        //Set All on IP
                        case 11:
                            if (IsOwner || IsInBackpack)
                            {
                                switch (m_ShipGumpObject.m_PlayersPage)
                                {
                                    case PlayersPageType.Friends:
                                        if (ship != null)
                                        {
                                            ship.IPAsFriends = !ship.IPAsFriends;

                                            if (ship.IPAsFriends)
                                                m_Player.SendMessage("All characters on your IP address will be treated as friends to this ship.");

                                            else
                                                m_Player.SendMessage("Characters on your IP address will no longer be treated as friends to this ship.");
                                        }

                                        if (shipDeed != null)
                                        {
                                            shipDeed.m_IPAsFriends = !shipDeed.m_IPAsFriends;

                                            if (shipDeed.m_IPAsFriends)
                                                m_Player.SendMessage("All characters on your IP address will be treated as friends to this ship.");

                                            else
                                                m_Player.SendMessage("Characters on your IP address will no longer be treated as friends to this ship.");
                                        }                                        
                                    break;

                                    case PlayersPageType.CoOwners:
                                        if (ship != null)
                                        {
                                            ship.IPAsCoOwners = !ship.IPAsCoOwners;

                                            if (ship.IPAsFriends)
                                                m_Player.SendMessage("All characters on your IP will be treated as co-owners to this ship.");

                                            else
                                                m_Player.SendMessage("Characters on your IP will no longer be treated as co-owners to this ship.");
                                        }

                                        if (shipDeed != null)
                                        {
                                            shipDeed.m_IPAsCoOwners = !shipDeed.m_IPAsCoOwners;

                                            if (shipDeed.m_IPAsCoOwners)
                                                m_Player.SendMessage("All characters on your IP will be treated as co-owners to this ship.");

                                            else
                                                m_Player.SendMessage("Characters on your IP will no longer be treated as co-owners to this ship.");
                                        }
                                    break;
                                }
                            }

                            closeGump = false;
                        break;

                        //Set All in Guild
                        case 12:
                            if (IsOwner || IsInBackpack)
                            {
                                switch (m_ShipGumpObject.m_PlayersPage)
                                {
                                    case PlayersPageType.Friends:
                                        if (ship != null)
                                        {
                                            ship.GuildAsFriends = !ship.GuildAsFriends;

                                            if (ship.GuildAsFriends)
                                                m_Player.SendMessage("All characters in your guild will be treated as friends to this ship.");

                                            else
                                                m_Player.SendMessage("Characters in your guild will longer be treated as friends to this ship.");
                                        }

                                        if (shipDeed != null)
                                        {
                                            shipDeed.m_GuildAsFriends = !shipDeed.m_GuildAsFriends;

                                            if (shipDeed.m_GuildAsFriends)
                                                m_Player.SendMessage("All characters in your guild will be treated as friends to this ship.");

                                            else
                                                m_Player.SendMessage("Characters in your guild will longer be treated as friends to this ship.");
                                        }                                        
                                    break;

                                    case PlayersPageType.CoOwners:
                                        if (ship != null)
                                        {
                                            ship.GuildAsCoOwners = !ship.GuildAsCoOwners;

                                            if (ship.GuildAsCoOwners)
                                                m_Player.SendMessage("All characters in your guild will be treated as co-owners to this ship.");

                                            else
                                                m_Player.SendMessage("Characters in your guild will longer be treated as co-owners to this ship.");
                                        }

                                        if (shipDeed != null)
                                        {
                                            shipDeed.m_GuildAsCoOwners = !shipDeed.m_GuildAsCoOwners;

                                            if (shipDeed.m_GuildAsCoOwners)
                                                m_Player.SendMessage("All characters in your guild will be treated as co-owners to this ship.");

                                            else
                                                m_Player.SendMessage("Characters in your guild will longer be treated as co-owners to this ship.");
                                        }
                                    break;
                                }
                            }

                            closeGump = false;
                        break;

                        //Add
                        case 13:
                            switch (m_ShipGumpObject.m_PlayersPage)
                            {
                                case PlayersPageType.Friends:                                   
                                    if (IsOwner || IsInBackpack)
                                    {
                                        m_Player.SendMessage("Who do you wish to make a friend of this ship.");
                                        m_Player.Target = new AddFriendCoOwnerTarget(m_Player, m_ShipGumpObject, AddFriendCoOwnerTarget.DesiredShipAccessLevel.Friend);
                                    }

                                    else
                                        m_Player.SendMessage("You do not have a high enough access level to add friends to this ship.");
                                break;

                                case PlayersPageType.CoOwners:                                    
                                    if (IsOwner || IsCoOwner || IsInBackpack)
                                    {
                                        m_Player.SendMessage("Who do you wish to make a friend of this ship.");
                                        m_Player.Target = new AddFriendCoOwnerTarget(m_Player, m_ShipGumpObject, AddFriendCoOwnerTarget.DesiredShipAccessLevel.CoOwner);
                                    }

                                    else
                                        m_Player.SendMessage("You do not have a high enough access level to add co-owners to this ship.");
                                break;
                            }

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

                        //Previous Page
                        case 16:
                            m_ShipGumpObject.m_Page--;

                            closeGump = false;
                        break;

                        //Next Page
                        case 17:
                            m_ShipGumpObject.m_Page++;

                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                        int playerIndex = (info.ButtonID - 20) + (m_ShipGumpObject.m_Page * ShipGump.PlayersPerPage);

                        if (playerIndex < m_ShipGumpObject.m_PlayerList.Count)
                        {
                            PlayerMobile playerTarget = m_ShipGumpObject.m_PlayerList[playerIndex] as PlayerMobile;

                            if (playerTarget != null)
                            {
                                switch (m_ShipGumpObject.m_PlayersPage)
                                {
                                    case PlayersPageType.Friends:
                                        if (ship != null)
                                        {
                                            if (ship.Friends.Contains(playerTarget))
                                                ship.Friends.Remove(playerTarget);
                                            else
                                                m_Player.SendMessage("That player entry is no longer accessible.");
                                        }

                                        if (shipDeed != null)
                                        {
                                            if (shipDeed.m_Friends.Contains(playerTarget))
                                                shipDeed.m_Friends.Remove(playerTarget);
                                            else
                                                m_Player.SendMessage("That player entry is no longer accessible.");
                                        }                                       
                                    break;

                                    case PlayersPageType.CoOwners:
                                        if (ship != null)
                                        {
                                            if (ship.CoOwners.Contains(playerTarget))
                                                ship.CoOwners.Remove(playerTarget);
                                            else
                                                m_Player.SendMessage("That player entry is no longer accessible.");
                                        }

                                        if (shipDeed != null)
                                        {
                                            if (shipDeed.m_CoOwners.Contains(playerTarget))
                                                shipDeed.m_CoOwners.Remove(playerTarget);
                                            else
                                                m_Player.SendMessage("That player entry is no longer accessible.");
                                        } 
                                    break;
                                }
                            }

                            else
                                m_Player.SendMessage("That player entry is no longer accessible.");
                        }

                        else
                            m_Player.SendMessage("That player entry is no longer accessible.");

                        closeGump = false;
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

                if (!m_ShipDeed.IsChildOf(player.Backpack))
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

        #region Add Friend / Co-Owner

        private class AddFriendCoOwnerTarget : Target
        {
            public enum DesiredShipAccessLevel
            {
                Friend,
                CoOwner
            }

            private PlayerMobile m_Player;
            private ShipGumpObject m_ShipGumpObject;
            private DesiredShipAccessLevel m_DesiredShipAccessLevel;

            public AddFriendCoOwnerTarget(PlayerMobile player, ShipGumpObject shipGumpObject, DesiredShipAccessLevel desiredShipAccessLevel): base(25, false, TargetFlags.None, false)
            {
                m_Player = player;
                m_ShipGumpObject = shipGumpObject;
                m_DesiredShipAccessLevel = desiredShipAccessLevel;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Player == null) return;
                if (m_ShipGumpObject == null) return;

                PlayerMobile playerTarget = target as PlayerMobile;

                if (playerTarget == null)
                {
                    m_Player.SendMessage("You must target a player.");

                    LaunchGump();
                    return;
                }

                if (playerTarget == m_Player)
                {
                    m_Player.SendMessage("You cannot change your own access to this ship.");

                    LaunchGump();
                    return;
                }

                BaseShip ship = m_ShipGumpObject.m_Ship;
                BaseShipDeed shipDeed = m_ShipGumpObject.m_ShipDeed;

                bool IsFriend = false;
                bool IsCoOwner = false;
                bool IsOwner = false;
                bool IsInBackpack = false;

                if (ship != null)
                {
                    if (ship.IsFriend(m_Player))
                        IsFriend = true;

                    if (ship.IsCoOwner(m_Player))
                        IsCoOwner = true;

                    if (ship.IsOwner(m_Player))
                        IsOwner = true;

                    switch(m_DesiredShipAccessLevel)
                    {
                        case DesiredShipAccessLevel.Friend:
                            if (ship.Friends.Contains(playerTarget))
                            {
                                m_Player.SendMessage("That player is already a friend of this ship.");

                                LaunchGump();
                                return;
                            }

                            if (ship.m_CoOwners.Contains(playerTarget) || ship.Owner == playerTarget)
                            {
                                m_Player.SendMessage("That player already has a higher access level to this ship.");

                                LaunchGump();
                                return;
                            }

                            if (!(IsCoOwner || IsOwner || IsInBackpack))
                            {
                                m_Player.SendMessage("You do not have a high enough access level to this ship to do that.");

                                LaunchGump();
                                return;
                            }

                            ship.Friends.Add(playerTarget);

                            m_Player.SendMessage("You add a friend to the ship.");

                            LaunchGump();
                            return;
                        break;

                        case DesiredShipAccessLevel.CoOwner:
                            if (ship.Friends.Contains(playerTarget))
                            {
                                m_Player.SendMessage("That player is already a co-owner of this ship.");

                                LaunchGump();
                                return;
                            }

                            if (ship.Owner == playerTarget)
                            {
                                m_Player.SendMessage("That player is the owner of this ship.");

                                LaunchGump();
                                return;
                            }

                            if (!(IsOwner || IsInBackpack))
                            {
                                m_Player.SendMessage("You do not have a high enough access level to this ship to do that.");

                                LaunchGump();
                                return;
                            }

                            ship.CoOwners.Add(playerTarget);

                            m_Player.SendMessage("You add a co-owner to the ship.");

                            LaunchGump();
                            return;
                        break;
                    }                   
                }

                if (shipDeed != null)
                {
                    if (shipDeed.IsFriend(m_Player))
                        IsFriend = true;

                    if (shipDeed.IsCoOwner(m_Player))
                        IsCoOwner = true;

                    if (shipDeed.IsOwner(m_Player))
                        IsOwner = true;

                    switch (m_DesiredShipAccessLevel)
                    {
                        case DesiredShipAccessLevel.Friend:
                            if (shipDeed.m_Friends.Contains(playerTarget))
                            {
                                m_Player.SendMessage("That player is already a friend of this ship.");

                                LaunchGump();
                                return;
                            }

                            if (shipDeed.m_CoOwners.Contains(playerTarget) || shipDeed.m_Owner == playerTarget)
                            {
                                m_Player.SendMessage("That player already has a higher access level to this ship.");

                                LaunchGump();
                                return;
                            }

                            if (!(IsCoOwner || IsOwner || IsInBackpack))
                            {
                                m_Player.SendMessage("You do not have a high enough access level to this ship to do that.");

                                LaunchGump();
                                return;
                            }

                            shipDeed.m_Friends.Add(playerTarget);

                            m_Player.SendMessage("You add a friend to the ship.");

                            LaunchGump();
                            return;
                            break;

                        case DesiredShipAccessLevel.CoOwner:
                            if (shipDeed.m_Friends.Contains(playerTarget))
                            {
                                m_Player.SendMessage("That player is already a co-owner of this ship.");

                                LaunchGump();
                                return;
                            }

                            if (shipDeed.m_Owner == playerTarget)
                            {
                                m_Player.SendMessage("That player is the owner of this ship.");

                                LaunchGump();
                                return;
                            }

                            if (!(IsOwner || IsInBackpack))
                            {
                                m_Player.SendMessage("You do not have a high enough access level to this ship to do that.");

                                LaunchGump();
                                return;
                            }

                            shipDeed.m_CoOwners.Add(playerTarget);

                            m_Player.SendMessage("You add a co-owner to the ship.");

                            LaunchGump();
                            return;
                        break;
                    }
                }
            }

            protected void LaunchGump()
            {
                if (m_Player == null) return;
                if (m_ShipGumpObject == null) return;

                m_Player.CloseGump(typeof(ShipGump));
                m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));
            }
        }

        #endregion
    }  

    #region Ship Registration

    public class ShipRegistrationGump : Gump
    {
        public PlayerMobile m_Player;
        public BaseShipDeed m_ShipDeed;

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public ShipRegistrationGump(PlayerMobile player, BaseShipDeed shipDeed): base(10, 10)
        {
            m_Player = player;
            m_ShipDeed = shipDeed;

            if (m_Player == null) return;
            if (m_Player.BankBox == null) return;
            if (m_Player.Backpack == null) return;
            if (m_ShipDeed == null) return;
            if (m_ShipDeed.Deleted) return;

            ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(m_ShipDeed, null, true, true);

            if (shipStatsProfile == null)
                return;

            #region Images

            AddImage(17, 356, 103);
            AddImage(303, 382, 103);
            AddImage(164, 416, 103);
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

            AddItem(55, 25, 2473);
            AddItem(71, 140, 5367);
            AddItem(97, 127, 7723);
            AddItem(38, 136, 7839);
            AddItem(87, 44, 710);
            AddItem(38, 70, 7860);
            AddItem(74, 59, 3992);
            AddItem(30, 209, 710);
            AddItem(71, 246, 3700);
            AddItem(85, 239, 7726);
            AddItem(76, 239, 3700);
            AddItem(81, 247, 3700);
            AddItem(58, 385, 7866);
            AddItem(68, 328, 5742);
            AddItem(90, 326, 6160);
            AddItem(83, 386, 5370);
            AddItem(39, 328, 7192);
            AddItem(47, 328, 7192);
            AddItem(43, 332, 7192);
            AddItem(64, 425, 5362);

            #endregion            
            
            int RegistrationDoubloonCost = shipStatsProfile.RegistrationDoubloonCost;
            double UpgradeDoubloonMultiplier = shipStatsProfile.UpgradeDoubloonMultiplier;

            int HoldSize = shipStatsProfile.HoldSizeAdjusted;

            int MaxHitPoints = shipStatsProfile.MaxHitPointsAdjusted;
            int MaxSailPoints = shipStatsProfile.MaxSailPointsAdjusted;
            int MaxGunPoints = shipStatsProfile.MaxGunPointsAdjusted;

            double ForwardSpeed = (1.0 / shipStatsProfile.ForwardSpeedAdjusted);
            double DriftSpeed = (1.0 / shipStatsProfile.DriftSpeedAdjusted);
            double SlowdownModePenalty = shipStatsProfile.SlowdownModePenaltyAdjusted;

            double CannonAccuracy = shipStatsProfile.CannonAccuracyAdjusted;
            double CannonMinDamage = shipStatsProfile.CannonMinDamageAdjusted;
            double CannonMaxDamage = shipStatsProfile.CannonMaxDamageAdjusted;
            double CannonRange = shipStatsProfile.CannonRangeAdjusted;
            double CannonReloadTime = shipStatsProfile.CannonReloadDurationAdjusted;     

            double MinorAbilityCooldown = shipStatsProfile.MinorAbilityCooldownDurationAdjusted;
            double MajorAbilityCooldown = shipStatsProfile.MajorAbilityCooldownDurationAdjusted;
            double EpicAbilityCooldown = shipStatsProfile.EpicAbilityCooldownDurationAdjusted;

            double RepairCooldownDuration = shipStatsProfile.RepairCooldownDurationAdjusted;   
            double BoardingChance = shipStatsProfile.BoardingChanceAdjusted;  
         
            double modifierValue = 0;

            int textHue = WhiteTextHue;
            int modifierHue = WhiteTextHue;

            int positiveHue = 63;
            int negativeHue = 1256;

            //Header
            AddImage(94, 3, 1141);
            AddLabel(Utility.CenteredTextOffset(230, Utility.Capitalize(shipStatsProfile.ShipTypeName)), 5, WhiteTextHue, Utility.Capitalize(shipStatsProfile.ShipTypeName));

            int startX = 292;
            int modifierX = 377;

            //Misc
            AddLabel(145, 30, 2625, "Hold Capacity");
            AddLabel(startX, 30, textHue, HoldSize.ToString() + " Items");
            modifierValue = shipStatsProfile.HoldSizeCreationModifier + shipStatsProfile.HoldSizeUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 30, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 30, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
                      
            //Stats
            AddLabel(145, 60, 149, "Hull Max Points");
            AddLabel(startX, 60, textHue, MaxHitPoints.ToString());
            modifierValue = shipStatsProfile.MaxHitPointsCreationModifier + shipStatsProfile.MaxHitPointsUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 60, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 60, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 80, 149, "Sail Max Points");
            AddLabel(startX, 80, textHue, MaxSailPoints.ToString());
            modifierValue = shipStatsProfile.MaxSailPointsCreationModifier + shipStatsProfile.MaxSailPointsUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 80, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 80, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 100, 149, "Gun Max Points");
            AddLabel(startX, 100, textHue, MaxGunPoints.ToString());
            modifierValue = shipStatsProfile.MaxGunPointsCreationModifier + shipStatsProfile.MaxGunPointsUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 100, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 100, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
           
            //Speed
            AddLabel(145, 130, 2599, "Forward Speed");
            AddLabel(startX, 130, textHue, Utility.CreateDecimalString(ForwardSpeed, 1));
            modifierValue = shipStatsProfile.ForwardSpeedCreationModifier + shipStatsProfile.ForwardSpeedUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 130, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 130, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 150, 2599, "Drift Speed");
            AddLabel(startX, 150, textHue, Utility.CreateDecimalString(DriftSpeed, 1));
            modifierValue = shipStatsProfile.DriftSpeedCreationModifier + shipStatsProfile.DriftSpeedUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 150, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 150, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
 
            AddLabel(145, 170, 2599, "Slowdown Mode Penalty");
            AddLabel(startX, 170, textHue, "-" + Utility.CreateDecimalPercentageString(SlowdownModePenalty, 1));
            modifierValue = shipStatsProfile.SlowdownModePenaltyCreationModifier + shipStatsProfile.SlowdownModePenaltyUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 170, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 170, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
           
            //Cannons
            AddLabel(145, 200, 2401, "Cannons Per Side");
            AddLabel(startX, 200, WhiteTextHue, shipStatsProfile.CannonsPerSide.ToString());

            AddLabel(145, 220, 2401, "Cannon Accuracy");
            AddLabel(startX, 220, textHue, Utility.CreateDecimalPercentageString(CannonAccuracy, 1));
            modifierValue = shipStatsProfile.CannonAccuracyCreationModifier + shipStatsProfile.CannonAccuracyUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 220, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 220, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 240, 2401, "Cannon Damage");
            AddLabel(startX, 240, textHue, Utility.CreateDecimalString(CannonMinDamage, 1) + " - " + Utility.CreateDecimalString(CannonMaxDamage, 1));
            modifierValue = shipStatsProfile.CannonDamageCreationModifier + shipStatsProfile.CannonDamageUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 240, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 240, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 260, 2401, "Cannon Range");
            AddLabel(startX, 260, textHue, Utility.CreateDecimalString(CannonRange, 1));
            modifierValue = shipStatsProfile.CannonRangeCreationModifier + shipStatsProfile.CannonRangeUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 260, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 260, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 280, 2401, "Cannon Reload Time");
            AddLabel(startX, 280, textHue, Utility.CreateDecimalString(CannonReloadTime, 1) + " sec");
            modifierValue = shipStatsProfile.CannonReloadDurationCreationModifier + shipStatsProfile.CannonReloadDurationUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 280, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 280, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            //Abilities            
            AddLabel(145, 310, 2606, "Minor Ability Cooldown");
            AddLabel(startX, 310, textHue, Utility.CreateDecimalString(MinorAbilityCooldown, 0) + " sec");
            modifierValue = shipStatsProfile.MinorAbilityCooldownDurationCreationModifier + shipStatsProfile.MinorAbilityCooldownDurationUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 310, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 310, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 330, 2606, "Major Ability Cooldown");
            AddLabel(startX, 330, textHue, Utility.CreateDecimalString(MajorAbilityCooldown, 0) + " sec");
            modifierValue = shipStatsProfile.MajorAbilityCooldownDurationCreationModifier + shipStatsProfile.MajorAbilityCooldownDurationUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 330, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 330, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 350, 2606, "Epic Ability Cooldown");
            AddLabel(startX, 350, textHue, Utility.CreateDecimalString(EpicAbilityCooldown, 0) + " sec");
            modifierValue = shipStatsProfile.EpicAbilityCooldownDurationCreationModifier + shipStatsProfile.EpicAbilityCooldownDurationUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 350, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 350, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
 
            AddLabel(145, 380, 2603, "Repair Cooldown");
            AddLabel(startX, 380, textHue, Utility.CreateDecimalString(RepairCooldownDuration, 0) + " sec");
            modifierValue = shipStatsProfile.RepairCooldownDurationCreationModifier + shipStatsProfile.RepairCooldownDurationUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 380, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 380, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            AddLabel(145, 400, 2603, "Boarding Chance");
            AddLabel(startX, 400, textHue, Utility.CreateDecimalPercentageString(BoardingChance, 1));
            modifierValue = shipStatsProfile.BoardingChanceCreationModifier + shipStatsProfile.BoardingChanceUpgradeModifier;
            if (modifierValue > 0) AddLabel(modifierX, 400, positiveHue, "(+" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");
            if (modifierValue == 0) modifierHue = WhiteTextHue;
            if (modifierValue < 0) AddLabel(modifierX, 400, negativeHue, "(" + Utility.CreateDecimalPercentageString(modifierValue, 1) + ")");

            //Doubloons
            AddLabel(145, 430, 53, "Upgrade Cost Multiplier");
            AddLabel(startX, 430, WhiteTextHue, Utility.CreateDecimalString(UpgradeDoubloonMultiplier, 1) + "x");

            //Cost
            AddLabel(58, 475, 149, "Ship Registration Fee");
            AddItem(185, 479, 2539);
            AddLabel(218, 475, WhiteTextHue, RegistrationDoubloonCost.ToString());            
                                    
            //Register
            AddButton(303, 471, 2151, 2154, 2, GumpButtonType.Reply, 0);
            AddLabel(338, 475, 63, "Register Ship");          

            //Guide
            AddButton(14, 11, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(10, 0, 149, "Guide"); 
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_ShipDeed == null) return;
            if (m_ShipDeed.Deleted) return;

            ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(m_ShipDeed, null, true, true);

            if (shipStatsProfile == null)
                return;
            
            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                break;

                //Registration
                case 2:
                    if (m_ShipDeed.IsChildOf(m_Player.Backpack))
                    {
                        int doubloonCost = shipStatsProfile.RegistrationDoubloonCost;
                        int doubloonBankBalance = Banker.GetUniqueCurrencyBalance(m_Player, typeof(Doubloon));

                        if (doubloonCost <= doubloonBankBalance)
                        {
                            Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), doubloonCost, true);

                            m_ShipDeed.m_Registered = true;

                            if (m_Player.NetState != null)
                                m_Player.PrivateOverheadMessage(MessageType.Regular, 0, false, "You register the ship and it is now ready to make sail!", m_Player.NetState);

                            return;
                        }

                        else                        
                            m_Player.SendMessage("You must have " + doubloonCost.ToString() + " doubloons in your bank in order to register this ship.");
                        
                        closeGump = false;
                    }

                    else
                    {
                        m_Player.SendMessage("That must be in your backpack if you wish to register that ship.");

                        closeGump = false;
                    }
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipRegistrationGump));
                m_Player.SendGump(new ShipRegistrationGump(m_Player, m_ShipDeed));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }

    #endregion

    #region Ship Rename

    public class ShipRenameGump : Gump
    {
        public PlayerMobile m_Player;
        public ShipGumpObject m_ShipGumpObject;
        public string m_NameText = "New Ship Name";

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public static int MinShipNameLength = 4;
        public static int MaxShipNameLength = 35;

        public ShipRenameGump(PlayerMobile player, ShipGumpObject shipGumpObject, string nameText): base(400, 150)
        {
            m_Player = player;
            m_ShipGumpObject = shipGumpObject;
            m_NameText = nameText;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipGumpObject == null) return;

            AddImage(270, 4, 103);
            AddImage(270, 53, 103);
            AddImage(140, 4, 103);
            AddImage(140, 53, 103);
            AddImage(5, 4, 103);
            AddImage(5, 53, 103);
            AddImage(15, 14, 3604, 2052);
            AddImage(135, 14, 3604, 2052);
            AddImage(231, 13, 3604, 2052);
            AddImage(275, 13, 3604, 2052);            
               
            AddImage(68, 62, 1141);

            AddTextEntry(80, 65, 248, 20, WhiteTextHue, 1, nameText, MaxShipNameLength);
                
            AddLabel(170, 25, 149, "Rename Ship");

            AddLabel(166, 104, 63, "Confirm Name");
            AddButton(133, 102, 2151, 2151, 2, GumpButtonType.Reply, 0);                
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_ShipGumpObject == null) return;

            BaseShip ship = m_ShipGumpObject.m_Ship;
            BaseShipDeed shipDeed = m_ShipGumpObject.m_ShipDeed;

            string oldShipName = "";

            bool IsOwner = false;
            bool IsInBackpack = false;

            if (ship != null)
            {
                oldShipName = ship.ShipName;

                IsOwner = ship.IsOwner(m_Player);
            }

            if (shipDeed != null)
            {
                oldShipName = shipDeed.m_ShipName;

                IsOwner = shipDeed.IsOwner(m_Player);

                if (shipDeed.IsChildOf(m_Player.Backpack))
                    IsInBackpack = true;
            }

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Confirm Name
                case 2:
                    TextRelay shipNameTextRelay = info.GetTextEntry(1);

                    string newShipName = "";

                    if (shipNameTextRelay != null)
                        newShipName = shipNameTextRelay.Text;

                    newShipName.Trim();

                    if (newShipName == "")
                    {
                        m_Player.SendMessage("That is not a valid ship name.");

                        closeGump = false;
                    }

                    else if (newShipName == oldShipName)
                    {
                        m_Player.SendMessage("Your ship is already named that.");                       

                        closeGump = false;
                    }

                    else if (newShipName.Length < MinShipNameLength)
                    {
                        m_Player.SendMessage("Ships names must be at least " + MinShipNameLength.ToString() + " characters in length.");

                        closeGump = false;
                    }

                    else if (newShipName.Length > MaxShipNameLength)
                    {
                        m_Player.SendMessage("Ships names must be no more than " + MaxShipNameLength.ToString() + " characters in length.");

                        closeGump = false;
                    }

                    else if (!Guilds.CheckProfanity(newShipName))
                    {
                        m_Player.SendMessage("That ship name is not allowed.");

                        closeGump = false;
                    }

                    else
                    {
                        if (!(IsOwner || IsInBackpack))
                        {
                            m_Player.SendMessage("You do not have high enough access level to change this ship's name.");

                            closeGump = false;
                        }

                        else
                        {
                            if (ship != null)
                            {
                                ship.ShipName = newShipName;

                                m_Player.CloseGump(typeof(ShipGump));
                                m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                                m_Player.SendMessage("You change the name of the ship.");

                                return;
                            }

                            if (shipDeed != null)
                            {
                                shipDeed.m_ShipName = newShipName;

                                m_Player.CloseGump(typeof(ShipGump));
                                m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));

                                m_Player.SendMessage("You change the name of the ship.");

                                return;
                            }

                            m_Player.SendMessage("That ship is no longer accessible.");
                        }
                    }

                    //TEST: POSSIBLY ADD NAME UNIQUENESS REQUIREMENTS TO SHIP NAMES
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipRenameGump));
                m_Player.SendGump(new ShipRenameGump(m_Player, m_ShipGumpObject, oldShipName));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }

    #endregion

    #region Ship Gump Object

    public class ShipGumpObject
    {
        public PlayerMobile m_Player;
        public BaseShip m_Ship;
        public BaseShipDeed m_ShipDeed;

        public ShipGump.ShipPageType m_ShipPage = ShipGump.ShipPageType.Overview;
        public ShipGump.PlayersPageType m_PlayersPage = ShipGump.PlayersPageType.Friends;
        public int m_Page = 0;

        public List<Mobile> m_PlayerList = new List<Mobile>();

        public ShipGumpObject(PlayerMobile player, BaseShip ship, BaseShipDeed deed)
        {
            m_Player = player;
            m_Ship = ship;
            m_ShipDeed = deed;
        }
    }

    #endregion
}