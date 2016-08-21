using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class ShipUpgrades
    {
        public enum UpgradeType
        {
            Theme,
            Paint,
            CannonMetal,

            Outfitting,
            Banner,
            Charm,            

            MinorAbility,
            MajorAbility,
            EpicAbility
        }

        public enum ThemeType
        {
            None,
            
            Pirate,
        }

        public enum PaintType
        {
            None,

            DarkGrey
        }

        public enum CannonMetalType
        {
            None,

            Bloodstone
        }

        public enum OutfittingType
        {
            None,

            Hunter,
        }

        public enum CharmType
        {
            None,

            BarrelOfLimes
        }

        public enum BannerType
        {
            None,

            Corsairs
        }

        public enum MinorAbilityType
        {
            None,

            ExpediteRepairs,
        }

        public enum MajorAbilityType
        {
            None,

            Smokescreen,
        }

        public enum EpicAbilityType
        {
            None,

            Hellfire
        }

        public static ShipUpgradeDetail GetThemeDetail(ThemeType themeUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Theme;

            switch (themeUpgrade)
            {
                case ThemeType.Pirate:
                    upgradeDetail.m_UpgradeName = "Pirate";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("Adds a variety of pirate-themed decoration to your ship", ShipUpgradeDetail.StatChangeHueType.Misc));
                    upgradeDetail.GumpCollectionId = "PirateShipThemeUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetPaintDetail(PaintType paintUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Paint;

            switch (paintUpgrade)
            {
                case PaintType.DarkGrey:
                    upgradeDetail.m_UpgradeName = "Dark Grey";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("Changes the color of the ship (Hue 1105)", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "DarkGreyShipPaintUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetCannonMetalDetail(CannonMetalType cannonMetalUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.CannonMetal;

            switch (cannonMetalUpgrade)
            {
                case CannonMetalType.Bloodstone:
                    upgradeDetail.m_UpgradeName = "Bloodstone";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("Changes the color of the ship's cannons (Hue 2117)", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "BloodstoneShipCannonMetalUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetOutfittingDetail(OutfittingType outfittingUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Outfitting;

            switch (outfittingUpgrade)
            {
                case OutfittingType.Hunter:
                    upgradeDetail.m_UpgradeName = "Hunter";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+5% Forward Speed", ShipUpgradeDetail.StatChangeHueType.Speed));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+5% Drifting Speed", ShipUpgradeDetail.StatChangeHueType.Speed));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+2% Cannon Accuracy", ShipUpgradeDetail.StatChangeHueType.Cannon));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("-5% Minor Ability Cooldown", ShipUpgradeDetail.StatChangeHueType.Abilities));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("-5% Major Ability Cooldown", ShipUpgradeDetail.StatChangeHueType.Abilities));
                    upgradeDetail.GumpCollectionId = "HunterShipOutfittingUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetBannerDetail(BannerType bannerUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Banner;

            switch (bannerUpgrade)
            {
                case BannerType.Corsairs:
                    upgradeDetail.m_UpgradeName = "Corsairs";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+25% Cannon Damage against Navy Ships", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+5% Boarding Chance", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+5% Doubloons Earned", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "CorsairsShipBannerUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetCharmDetail(CharmType charmUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Banner;

            switch (charmUpgrade)
            {
                case CharmType.BarrelOfLimes:
                    upgradeDetail.m_UpgradeName = "Barrel of Limes";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+25% Damage Dealt by Henchmen onboard", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+5% Doubloons Earned", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "BarrelOfLimesShipCharmUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetMinorAbilityDetail(MinorAbilityType minorAbilityUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.MinorAbility;

            switch (minorAbilityUpgrade)
            {
                case MinorAbilityType.ExpediteRepairs:
                    upgradeDetail.m_UpgradeName = "Expedite Repairs";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("+25% Repair Effectiveness for 30 seconds", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "ExpediteRepairsShipMinorAbilityUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetMajorAbilityDetail(MajorAbilityType majorAbilityUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.MajorAbility;

            switch (majorAbilityUpgrade)
            {
                case MajorAbilityType.Smokescreen:
                    upgradeDetail.m_UpgradeName = "Smokescreen";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("Pacifies creatures hit for 10 seconds", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "SmokescreenShipMajorAbilityUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        public static ShipUpgradeDetail GetEpicAbilityDetail(EpicAbilityType epicAbilityUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.MajorAbility;

            switch (epicAbilityUpgrade)
            {
                case EpicAbilityType.Hellfire:
                    upgradeDetail.m_UpgradeName = "Hellfire";
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("Unleashes a damaging barrage of fire that", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.m_Effects.Add(new KeyValuePair<string, ShipUpgradeDetail.StatChangeHueType>("continues to burn for 30 seconds, damaging targets", ShipUpgradeDetail.StatChangeHueType.Special));
                    upgradeDetail.GumpCollectionId = "HellfireShipEpicAbilityUpgrade";
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }
    }   

    public class ShipUpgradeDetail
    {
        public enum StatChangeHueType
        {           
            Stats,
            Speed,
            Cannon,
            Abilities,
            Special,
            Misc
        }

        public ShipUpgrades.UpgradeType m_UpgradeType = ShipUpgrades.UpgradeType.Theme;
        public string m_UpgradeName = "";
        public List<KeyValuePair<string, StatChangeHueType>> m_Effects = new List<KeyValuePair<string,StatChangeHueType>>();        
        public string GumpCollectionId = "";

        public ShipUpgradeDetail()
        {
        }
    }
}