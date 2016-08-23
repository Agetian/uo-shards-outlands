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

        public ShipUpgradeGump(PlayerMobile player, ShipUpgradeGumpObject shipUpgradeGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ShipUpgradeGumpObject = shipUpgradeGumpObject;            

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipUpgradeGumpObject == null) return;

            int startX = 0;
            int startY = 0;
            
            //Background
            AddImage(270, 4, 103);
            AddImage(270, 101, 103);
            AddImage(270, 144, 103);
            AddImage(140, 4, 103);
            AddImage(140, 101, 103);
            AddImage(141, 144, 103);
            AddImage(5, 143, 103);
            AddImage(5, 4, 103);
            AddImage(5, 101, 103);

            AddImage(15, 103, 3604, 2052);
            AddImage(140, 104, 3604, 2052);
            AddImage(15, 14, 3604, 2052);
            AddImage(135, 14, 3604, 2052);            
            AddImage(231, 13, 3604, 2052);
            AddImage(275, 13, 3604, 2052);
            AddImage(231, 104, 3604, 2052);
            AddImage(275, 104, 3604, 2052);

            AddImage(28, 63, 2328);
            AddItem(77, 168, 2539);
            AddItem(107, 208, 2539);

            ShipUpgradeDetail upgradeDetail = null;

            //Upgrade
            switch (m_ShipUpgradeGumpObject.m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme: upgradeDetail = ShipUpgrades.GetThemeDetail(m_ShipUpgradeGumpObject.m_Theme); break;
                case ShipUpgrades.UpgradeType.Paint: upgradeDetail = ShipUpgrades.GetPaintDetail(m_ShipUpgradeGumpObject.m_Paint); break;
                case ShipUpgrades.UpgradeType.CannonMetal: upgradeDetail = ShipUpgrades.GetCannonMetalDetail(m_ShipUpgradeGumpObject.m_CannonMetal); break;

                case ShipUpgrades.UpgradeType.Outfitting: upgradeDetail = ShipUpgrades.GetOutfittingDetail(m_ShipUpgradeGumpObject.m_Outfitting); break;
                case ShipUpgrades.UpgradeType.Banner: upgradeDetail = ShipUpgrades.GetBannerDetail(m_ShipUpgradeGumpObject.m_Banner); break;
                case ShipUpgrades.UpgradeType.Charm: upgradeDetail = ShipUpgrades.GetCharmDetail(m_ShipUpgradeGumpObject.m_Charm); break;

                case ShipUpgrades.UpgradeType.MinorAbility: upgradeDetail = ShipUpgrades.GetMinorAbilityDetail(m_ShipUpgradeGumpObject.m_MinorAbility); break;
                case ShipUpgrades.UpgradeType.MajorAbility: upgradeDetail = ShipUpgrades.GetMajorAbilityDetail(m_ShipUpgradeGumpObject.m_MajorAbility); break;
                case ShipUpgrades.UpgradeType.EpicAbility: upgradeDetail = ShipUpgrades.GetEpicAbilityDetail(m_ShipUpgradeGumpObject.m_EpicAbility); break;                
            }            

            int doubloonBaseCost = ShipUniqueness.GetShipUpgradeBaseDoubloonCost(upgradeDetail.m_UpgradeType);
           
            AddLabel(138, 15, 149, "Ship Outfitting Upgrade");
            AddLabel(Utility.CenteredTextOffset(192, upgradeDetail.m_UpgradeName), 35, 0, upgradeDetail.m_UpgradeName);
                        
            AddLabel(21, 165, 149, "Base Cost:");
            AddLabel(109, 165, 0, doubloonBaseCost.ToString());

            if (m_ShipUpgradeGumpObject.m_ShipDeed != null && m_ShipUpgradeGumpObject.m_UpgradeDisplayMode != UpgradeDisplayMode.DeedUse)
            {
                ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(m_ShipUpgradeGumpObject.m_ShipDeed.ShipType);

                if (shipStatsProfile == null)
                    return;

                double doubloonMultiplier = shipStatsProfile.UpgradeDoubloonMultiplier;

                AddLabel(21, 185, 149, "Ship Type Multiplier:");
                AddLabel(154, 186, 0, Utility.CreateDecimalString(doubloonMultiplier, 1) + "x");

                int doubloonAdjustedCost = (int)(Math.Round((double)doubloonBaseCost * (double)doubloonMultiplier));

                AddLabel(21, 205, 149, "Adjusted Cost:");
                AddLabel(138, 205, 63, doubloonAdjustedCost.ToString());
            }

            startY = 60;
            int rowSpacing = 20;

            for (int a = 0; a < upgradeDetail.m_Effects.Count; a++)
            {
                KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType> descriptionLine = upgradeDetail.m_Effects[a];

                AddLabel(120, startY, upgradeDetail.GetHue(descriptionLine.Value), descriptionLine.Key);
                startY += rowSpacing;
            }
            
            //Guide
            AddButton(1, 4, 2094, 2095, 0, GumpButtonType.Reply, 0);
            AddLabel(18, -3, 149, "Guide");

            switch (m_ShipUpgradeGumpObject.m_UpgradeDisplayMode)
            {
                case UpgradeDisplayMode.DeedUse: 
                    AddLabel(267, 204, 63, "Install Upgrade");
                    AddButton(367, 201, 2151, 2151, 2, GumpButtonType.Reply, 0);     
                break;

                case UpgradeDisplayMode.DeedAttemptInstall:
                    AddLabel(267, 204, 63, "Confirm Install");
                    AddButton(367, 201, 2151, 2151, 2, GumpButtonType.Reply, 0);     
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

            ShipUpgradeDetail upgradeDetail = null;

            //Upgrade
            switch (m_ShipUpgradeGumpObject.m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme: upgradeDetail = ShipUpgrades.GetThemeDetail(m_ShipUpgradeGumpObject.m_Theme); break;
                case ShipUpgrades.UpgradeType.Paint: upgradeDetail = ShipUpgrades.GetPaintDetail(m_ShipUpgradeGumpObject.m_Paint); break;
                case ShipUpgrades.UpgradeType.CannonMetal: upgradeDetail = ShipUpgrades.GetCannonMetalDetail(m_ShipUpgradeGumpObject.m_CannonMetal); break;

                case ShipUpgrades.UpgradeType.Outfitting: upgradeDetail = ShipUpgrades.GetOutfittingDetail(m_ShipUpgradeGumpObject.m_Outfitting); break;
                case ShipUpgrades.UpgradeType.Banner: upgradeDetail = ShipUpgrades.GetBannerDetail(m_ShipUpgradeGumpObject.m_Banner); break;
                case ShipUpgrades.UpgradeType.Charm: upgradeDetail = ShipUpgrades.GetCharmDetail(m_ShipUpgradeGumpObject.m_Charm); break;

                case ShipUpgrades.UpgradeType.MinorAbility: upgradeDetail = ShipUpgrades.GetMinorAbilityDetail(m_ShipUpgradeGumpObject.m_MinorAbility); break;
                case ShipUpgrades.UpgradeType.MajorAbility: upgradeDetail = ShipUpgrades.GetMajorAbilityDetail(m_ShipUpgradeGumpObject.m_MajorAbility); break;
                case ShipUpgrades.UpgradeType.EpicAbility: upgradeDetail = ShipUpgrades.GetEpicAbilityDetail(m_ShipUpgradeGumpObject.m_EpicAbility); break;
            }

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
                            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed == null)
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Deleted)
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else if (!m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else
                            {
                                m_Player.SendMessage("Which ship do you wish to install this upgrade into?");
                                m_Player.Target = new ShipUpgradeTarget(m_Player, m_ShipUpgradeGumpObject);

                                return;
                            }
                        break;

                        case UpgradeDisplayMode.DeedAttemptInstall:
                            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed == null)
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Deleted)
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (!m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipDeed == null)
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipDeed.Deleted)
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else if (!m_ShipUpgradeGumpObject.m_ShipDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else
                            {
                                int doubloonBaseCost = ShipUniqueness.GetShipUpgradeBaseDoubloonCost(upgradeDetail.m_UpgradeType);

                                ShipStatsProfile shipStatsProfile = ShipUniqueness.GetShipStatsProfile(m_ShipUpgradeGumpObject.m_ShipDeed.ShipType);

                                double doubloonMultiplier = shipStatsProfile.UpgradeDoubloonMultiplier;
                                int doubloonAdjustedCost = (int)(Math.Round((double)doubloonBaseCost * (double)doubloonMultiplier));

                                int doubloonBalance = Banker.GetUniqueCurrencyBalance(m_Player, typeof(Doubloon));

                                if (doubloonBalance >= doubloonAdjustedCost)
                                {
                                    //TEST: FINISH
                                }

                                else
                                {
                                    m_Player.SendMessage("You do not have the neccessary " + doubloonBalance.ToString() + " doubloons in your bank box to install this upgrade.");
                                    return;
                                }
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