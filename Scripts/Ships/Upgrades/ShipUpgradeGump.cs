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

namespace Server
{
    public class ShipUpgradeGump : Gump
    {
        public enum UpgradeDisplayMode
        {           
            DeedUse,
            DeedAttemptInstall,
            InstalledOnShip
        }

        public PlayerMobile m_Player;
        public ShipUpgradeGumpObject m_ShipUpgradeGumpObject;        

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public ShipUpgradeGump(PlayerMobile player, ShipUpgradeGumpObject shipUpgradeGumpObject): base(325, 150)
        {
            m_Player = player;
            m_ShipUpgradeGumpObject = shipUpgradeGumpObject;            

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipUpgradeGumpObject == null) return;           

            BaseShip ship = m_ShipUpgradeGumpObject.m_Ship;
            BaseShipDeed shipDeed = m_ShipUpgradeGumpObject.m_ShipDeed;

            ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(shipDeed, ship, true, true);                       

            int startX = 0;
            int startY = 0;

            #region Background

            AddImage(275, 9, 103);
            AddImage(275, 106, 103);
            AddImage(275, 149, 103);
            AddImage(145, 9, 103);
            AddImage(145, 106, 103);
            AddImage(146, 149, 103);
            AddImage(10, 148, 103);
            AddImage(10, 9, 103);
            AddImage(10, 106, 103);

            AddImage(20, 108, 3604, 2052);
            AddImage(145, 109, 3604, 2052);
            AddImage(20, 19, 3604, 2052);
            AddImage(140, 19, 3604, 2052);            
            AddImage(236, 18, 3604, 2052);
            AddImage(280, 18, 3604, 2052);
            AddImage(236, 109, 3604, 2052);
            AddImage(280, 109, 3604, 2052);

            AddImage(33, 68, 2328);
            AddItem(82, 173, 2539);            

            #endregion

            ShipUpgradeDetail upgradeDetail = null;

            bool replaceExistingUpgrade = false;

            #region Upgrade Type

            switch (m_ShipUpgradeGumpObject.m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme:
                    if (ship != null)
                    {
                        if (ship.m_ThemeUpgrade != ShipUpgrades.ThemeType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_ThemeUpgrade != ShipUpgrades.ThemeType.None)
                            replaceExistingUpgrade = true;
                    }                       

                    upgradeDetail = ShipUpgrades.GetThemeDetail(m_ShipUpgradeGumpObject.m_Theme); 
                break;

                case ShipUpgrades.UpgradeType.Paint: 
                    if (ship != null)
                    {
                        if (ship.m_PaintUpgrade != ShipUpgrades.PaintType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_PaintUpgrade != ShipUpgrades.PaintType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetPaintDetail(m_ShipUpgradeGumpObject.m_Paint);
                break;
                
                case ShipUpgrades.UpgradeType.CannonMetal:
                    if (ship != null)
                    {
                        if (ship.m_CannonMetalUpgrade != ShipUpgrades.CannonMetalType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_CannonMetalUpgrade != ShipUpgrades.CannonMetalType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetCannonMetalDetail(m_ShipUpgradeGumpObject.m_CannonMetal); 
                break;

                case ShipUpgrades.UpgradeType.Outfitting:
                    if (ship != null)
                    {
                        if (ship.m_OutfittingUpgrade != ShipUpgrades.OutfittingType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_OutfittingUpgrade != ShipUpgrades.OutfittingType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetOutfittingDetail(m_ShipUpgradeGumpObject.m_Outfitting); 
                break;

                case ShipUpgrades.UpgradeType.Banner: 
                    if (ship != null)
                    {
                        if (ship.m_BannerUpgrade != ShipUpgrades.BannerType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_BannerUpgrade != ShipUpgrades.BannerType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetBannerDetail(m_ShipUpgradeGumpObject.m_Banner);
                break;

                case ShipUpgrades.UpgradeType.Charm: 
                    if (ship != null)
                    {
                        if (ship.m_CharmUpgrade != ShipUpgrades.CharmType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_CharmUpgrade != ShipUpgrades.CharmType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetCharmDetail(m_ShipUpgradeGumpObject.m_Charm); 
                break;

                case ShipUpgrades.UpgradeType.MinorAbility: 
                    if (ship != null)
                    {
                        if (ship.m_MinorAbilityUpgrade != ShipUpgrades.MinorAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_MinorAbilityUpgrade != ShipUpgrades.MinorAbilityType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetMinorAbilityDetail(m_ShipUpgradeGumpObject.m_MinorAbility); 
                break;

                case ShipUpgrades.UpgradeType.MajorAbility:
                    if (ship != null)
                    {
                        if (ship.m_MajorAbilityUpgrade != ShipUpgrades.MajorAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_MajorAbilityUpgrade != ShipUpgrades.MajorAbilityType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetMajorAbilityDetail(m_ShipUpgradeGumpObject.m_MajorAbility);
                break;

                case ShipUpgrades.UpgradeType.EpicAbility: 
                    if (ship != null)
                    {
                        if (ship.m_EpicAbilityUpgrade != ShipUpgrades.EpicAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_EpicAbilityUpgrade != ShipUpgrades.EpicAbilityType.None)
                            replaceExistingUpgrade = true;
                    } 

                    upgradeDetail = ShipUpgrades.GetEpicAbilityDetail(m_ShipUpgradeGumpObject.m_EpicAbility);
                break;
            }

            #endregion

            if (upgradeDetail == null)
                return;

            int doubloonBaseCost = ShipUniqueness.GetShipUpgradeBaseDoubloonCost(upgradeDetail.m_UpgradeType);
           
            AddLabel(143, 20, 149, "Ship Outfitting Upgrade");
            AddLabel(Utility.CenteredTextOffset(210, upgradeDetail.m_UpgradeName), 40, WhiteTextHue, upgradeDetail.m_UpgradeName);
                        
            AddLabel(26, 170, 149, "Base Cost:");
            AddLabel(114, 170, WhiteTextHue, doubloonBaseCost.ToString());
            
            double doubloonMultiplier = shipStatsProfile.UpgradeDoubloonMultiplier;

            if (m_ShipUpgradeGumpObject.m_UpgradeDisplayMode == UpgradeDisplayMode.DeedAttemptInstall || m_ShipUpgradeGumpObject.m_UpgradeDisplayMode == UpgradeDisplayMode.InstalledOnShip)
            {
                if (replaceExistingUpgrade && m_ShipUpgradeGumpObject.m_UpgradeDisplayMode == UpgradeDisplayMode.DeedAttemptInstall)
                {
                    AddLabel(26, 190, 149, "Will Replace Existing Upgrade");
                    AddLabel(213, 190, 2550, "(at no cost)");   
                }

                else
                {
                    AddLabel(26, 190, 149, "Ship Type Multiplier:");
                    AddLabel(151, 190, WhiteTextHue, Utility.CreateDecimalString(doubloonMultiplier, 1) + "x");
                }

                int doubloonAdjustedCost = (int)(Math.Round((double)doubloonBaseCost * (double)doubloonMultiplier));

                if (replaceExistingUpgrade && m_ShipUpgradeGumpObject.m_UpgradeDisplayMode == UpgradeDisplayMode.DeedAttemptInstall)
                    doubloonAdjustedCost = 0;

                AddLabel(26, 210, 149, "Adjusted Cost:");
                AddItem(112, 213, 2539);
                AddLabel(143, 210, 63, doubloonAdjustedCost.ToString());
            }

            startY = 65;
            int rowSpacing = 20;

            for (int a = 0; a < upgradeDetail.m_Effects.Count; a++)
            {
                KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType> descriptionLine = upgradeDetail.m_Effects[a];

                AddLabel(125, startY, upgradeDetail.GetHue(descriptionLine.Value), descriptionLine.Key);
                startY += rowSpacing;
            }

            int offsetX = -32;
            int offsetY = -22;

            AddGumpCollection(GumpCollections.GetGumpCollection(upgradeDetail.GumpCollectionId, -1), offsetX + 33, offsetY + 68);
            
            //Guide
            AddButton(6, 7, 2094, 2095, 0, GumpButtonType.Reply, 0);
            AddLabel(1, -2, 149, "Guide");

            switch (m_ShipUpgradeGumpObject.m_UpgradeDisplayMode)
            {
                case UpgradeDisplayMode.DeedUse:
                    AddLabel(250, 209, 63, "Select Target Ship");
                    AddButton(372, 206, 2151, 2151, 2, GumpButtonType.Reply, 0);     
                break;

                case UpgradeDisplayMode.DeedAttemptInstall:
                    AddLabel(211, 209, 63, "Confirm Ship Installation");
                    AddButton(372, 206, 2151, 2151, 2, GumpButtonType.Reply, 0);     
                break;

                case UpgradeDisplayMode.InstalledOnShip:
                break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Player.Backpack == null) return;
            if (m_ShipUpgradeGumpObject == null) return;

            bool closeGump = true;

            BaseShip ship = m_ShipUpgradeGumpObject.m_Ship;
            BaseShipDeed shipDeed = m_ShipUpgradeGumpObject.m_ShipDeed;
            ShipUpgradeDeed shipUpgradeDeed = m_ShipUpgradeGumpObject.m_ShipUpgradeDeed;

            ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(shipDeed, ship, true, true);

            ShipUpgradeDetail upgradeDetail = null;

            bool replaceExistingUpgrade = false;

            #region Upgrade Type

            switch (m_ShipUpgradeGumpObject.m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme:
                    if (ship != null)
                    {
                        if (ship.m_ThemeUpgrade != ShipUpgrades.ThemeType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_ThemeUpgrade != ShipUpgrades.ThemeType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetThemeDetail(m_ShipUpgradeGumpObject.m_Theme);
                break;

                case ShipUpgrades.UpgradeType.Paint:
                    if (ship != null)
                    {
                        if (ship.m_PaintUpgrade != ShipUpgrades.PaintType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_PaintUpgrade != ShipUpgrades.PaintType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetPaintDetail(m_ShipUpgradeGumpObject.m_Paint);
                break;

                case ShipUpgrades.UpgradeType.CannonMetal:
                    if (ship != null)
                    {
                        if (ship.m_CannonMetalUpgrade != ShipUpgrades.CannonMetalType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_CannonMetalUpgrade != ShipUpgrades.CannonMetalType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetCannonMetalDetail(m_ShipUpgradeGumpObject.m_CannonMetal);
                break;

                case ShipUpgrades.UpgradeType.Outfitting:
                    if (ship != null)
                    {
                        if (ship.m_OutfittingUpgrade != ShipUpgrades.OutfittingType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_OutfittingUpgrade != ShipUpgrades.OutfittingType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetOutfittingDetail(m_ShipUpgradeGumpObject.m_Outfitting);
                break;

                case ShipUpgrades.UpgradeType.Banner:
                    if (ship != null)
                    {
                        if (ship.m_BannerUpgrade != ShipUpgrades.BannerType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_BannerUpgrade != ShipUpgrades.BannerType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetBannerDetail(m_ShipUpgradeGumpObject.m_Banner);
                break;

                case ShipUpgrades.UpgradeType.Charm:
                    if (ship != null)
                    {
                        if (ship.m_CharmUpgrade != ShipUpgrades.CharmType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_CharmUpgrade != ShipUpgrades.CharmType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetCharmDetail(m_ShipUpgradeGumpObject.m_Charm);
                break;

                case ShipUpgrades.UpgradeType.MinorAbility:
                    if (ship != null)
                    {
                        if (ship.m_MinorAbilityUpgrade != ShipUpgrades.MinorAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_MinorAbilityUpgrade != ShipUpgrades.MinorAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetMinorAbilityDetail(m_ShipUpgradeGumpObject.m_MinorAbility);
                break;

                case ShipUpgrades.UpgradeType.MajorAbility:
                    if (ship != null)
                    {
                        if (ship.m_MajorAbilityUpgrade != ShipUpgrades.MajorAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_MajorAbilityUpgrade != ShipUpgrades.MajorAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetMajorAbilityDetail(m_ShipUpgradeGumpObject.m_MajorAbility);
                break;

                case ShipUpgrades.UpgradeType.EpicAbility:
                    if (ship != null)
                    {
                        if (ship.m_EpicAbilityUpgrade != ShipUpgrades.EpicAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    if (shipDeed != null)
                    {
                        if (shipDeed.m_EpicAbilityUpgrade != ShipUpgrades.EpicAbilityType.None)
                            replaceExistingUpgrade = true;
                    }

                    upgradeDetail = ShipUpgrades.GetEpicAbilityDetail(m_ShipUpgradeGumpObject.m_EpicAbility);
                break;
            }

            #endregion

            if (upgradeDetail == null)
                return;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Install
                case 2:
                    switch(m_ShipUpgradeGumpObject.m_UpgradeDisplayMode)
                    {
                        case UpgradeDisplayMode.DeedUse:                            
                            if (shipUpgradeDeed == null)
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else if (shipUpgradeDeed.Deleted)
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else if (!shipUpgradeDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else
                            {
                                m_Player.SendMessage("Which ship do you wish to install this upgrade into?");
                                m_Player.Target = new ShipUpgradeTarget(m_Player, m_ShipUpgradeGumpObject);

                                return;                             
                            }                            
                        break;

                        case UpgradeDisplayMode.DeedAttemptInstall:                            
                            if (shipUpgradeDeed == null)
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (shipUpgradeDeed.Deleted)
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (!shipUpgradeDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (shipDeed == null)
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else if (shipDeed.Deleted)
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else if (!shipDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else
                            {
                                int doubloonBaseCost = ShipUniqueness.GetShipUpgradeBaseDoubloonCost(upgradeDetail.m_UpgradeType);
                                
                                double doubloonMultiplier = shipStatsProfile.UpgradeDoubloonMultiplier;
                                int doubloonAdjustedCost = (int)(Math.Round((double)doubloonBaseCost * (double)doubloonMultiplier));

                                if (replaceExistingUpgrade)
                                    doubloonAdjustedCost = 0;

                                int doubloonBalance = Banker.GetUniqueCurrencyBalance(m_Player, typeof(Doubloon));

                                if (doubloonBalance >= doubloonAdjustedCost)
                                {
                                    if (doubloonAdjustedCost > 0)
                                        Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), doubloonAdjustedCost, true);

                                    switch (upgradeDetail.m_UpgradeType)
                                    {
                                        case ShipUpgrades.UpgradeType.Theme: shipDeed.m_ThemeUpgrade = shipUpgradeDeed.m_ThemeUpgrade; break;
                                        case ShipUpgrades.UpgradeType.Paint: shipDeed.m_PaintUpgrade = shipUpgradeDeed.m_PaintUpgrade; break;
                                        case ShipUpgrades.UpgradeType.CannonMetal: shipDeed.m_CannonMetalUpgrade = shipUpgradeDeed.m_CannonMetalUpgrade; break;

                                        case ShipUpgrades.UpgradeType.Outfitting: shipDeed.m_OutfittingUpgrade = shipUpgradeDeed.m_OutfittingUpgrade; break;
                                        case ShipUpgrades.UpgradeType.Banner: shipDeed.m_BannerUpgrade = shipUpgradeDeed.m_BannerUpgrade; break;
                                        case ShipUpgrades.UpgradeType.Charm: shipDeed.m_CharmUpgrade = shipUpgradeDeed.m_CharmUpgrade; break;

                                        case ShipUpgrades.UpgradeType.MinorAbility: shipDeed.m_MinorAbilityUpgrade = shipUpgradeDeed.m_MinorAbilityUpgrade; break;
                                        case ShipUpgrades.UpgradeType.MajorAbility: shipDeed.m_MajorAbilityUpgrade = shipUpgradeDeed.m_MajorAbilityUpgrade; break;
                                        case ShipUpgrades.UpgradeType.EpicAbility: shipDeed.m_EpicAbilityUpgrade = shipUpgradeDeed.m_EpicAbilityUpgrade; break;
                                    }                                    
                                    
                                    if (m_Player.HasGump(typeof(ShipGump)))
                                        m_Player.CloseGump(typeof(ShipGump));

                                    ShipGumpObject shipGumpObject = new ShipGumpObject(m_Player, null, shipDeed);

                                    shipGumpObject.m_ShipPage = ShipGump.ShipPageType.Upgrades;

                                    m_Player.SendGump(new ShipGump(m_Player, shipGumpObject));

                                    if (replaceExistingUpgrade)
                                        m_Player.SendMessage("You place an upgrade onto your ship, overriding the existing one.");
                                    else
                                        m_Player.SendMessage("You place an upgrade onto your ship.");

                                    m_Player.SendSound(0x23D);

                                    m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Delete();

                                    return;
                                }

                                else                                
                                   m_Player.SendMessage("You do not have the neccessary " + doubloonAdjustedCost.ToString() + " doubloons in your bank box to install this upgrade.");
                            }  
                        break;

                        case UpgradeDisplayMode.InstalledOnShip:
                        break;
                    }

                    closeGump = false; 
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipUpgradeGump));
                m_Player.SendGump(new ShipUpgradeGump(m_Player, m_ShipUpgradeGumpObject));
            }
            else
                m_Player.SendSound(CloseGumpSound);
        }
    }

    public class ShipUpgradeGumpObject
    {
        public ShipUpgradeGump.UpgradeDisplayMode m_UpgradeDisplayMode;

        public Type m_ShipType = null;
        public BaseShip m_Ship;
        public BaseShipDeed m_ShipDeed;
        public ShipUpgradeDeed m_ShipUpgradeDeed;

        public ShipUpgrades.UpgradeType m_UpgradeType = ShipUpgrades.UpgradeType.Theme;

        public ShipUpgrades.ThemeType m_Theme = ShipUpgrades.ThemeType.None;
        public ShipUpgrades.PaintType m_Paint = ShipUpgrades.PaintType.None;
        public ShipUpgrades.CannonMetalType m_CannonMetal = ShipUpgrades.CannonMetalType.None;
        public ShipUpgrades.OutfittingType m_Outfitting = ShipUpgrades.OutfittingType.None;
        public ShipUpgrades.BannerType m_Banner = ShipUpgrades.BannerType.None;
        public ShipUpgrades.CharmType m_Charm = ShipUpgrades.CharmType.None;
        public ShipUpgrades.MinorAbilityType m_MinorAbility = ShipUpgrades.MinorAbilityType.None;
        public ShipUpgrades.MajorAbilityType m_MajorAbility = ShipUpgrades.MajorAbilityType.None;
        public ShipUpgrades.EpicAbilityType m_EpicAbility = ShipUpgrades.EpicAbilityType.None;

        public ShipUpgradeGumpObject()
        {            
        }
    }

    public class ShipUpgradeTarget : Target
    {
        public PlayerMobile m_Player;
        public ShipUpgradeGumpObject m_ShipUpgradeGumpObject;

        public ShipUpgradeTarget(PlayerMobile player, ShipUpgradeGumpObject shipUpgradeGumpObject): base(1, false, TargetFlags.None, false)
        {
            m_Player = player;
            m_ShipUpgradeGumpObject = shipUpgradeGumpObject;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Player == null) return;
            if (m_Player.Backpack == null) return;
            if (m_ShipUpgradeGumpObject == null) return;

            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed == null)
            {
                m_Player.SendMessage("That upgrade deed is no longer accessible.");
                return;
            }

            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Deleted)
            {
                m_Player.SendMessage("That upgrade deed is no longer accessible.");
                return;
            }

            if (!m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.IsChildOf(m_Player.Backpack))
            {
                m_Player.SendMessage("The upgrade deed you wish to use must be in your backpack.");
                return;
            }

            BaseShipDeed shipDeed = target as BaseShipDeed;

            if (shipDeed == null)
            {
                m_Player.SendMessage("You must target a ship token.");
                return;
            }

            if (shipDeed.Deleted)
            {
                m_Player.SendMessage("You must target a ship token.");
                return;
            }

            if (!shipDeed.IsChildOf(m_Player.Backpack))
            {
                m_Player.SendMessage("You must target a ship token in your backpack.");
                return;
            }

            m_ShipUpgradeGumpObject.m_ShipType = shipDeed.ShipType;

            m_ShipUpgradeGumpObject.m_ShipDeed = shipDeed;            
            m_ShipUpgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.DeedAttemptInstall;

            m_Player.CloseGump(typeof(ShipUpgradeGump));
            m_Player.SendGump(new ShipUpgradeGump(m_Player, m_ShipUpgradeGumpObject));
        }
    }
}