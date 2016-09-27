using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;

namespace Server
{
    public class ShipUpgrades
    {
        #region Enums

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
            
            Navy,
            Pirate,
            Orc,
            Undead,
            Treasure,
            Derelict,
            Merchant,
            Daemonic,
            Eldritch,
            Massacre
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

        #endregion

        #region Theme Upgrade

        public static ShipUpgradeDetail GetThemeDetail(ThemeType themeUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Theme;          

            switch (themeUpgrade)
            {
                case ThemeType.Navy:
                    upgradeDetail.m_UpgradeName = "Navy";
                    upgradeDetail.GumpCollectionId = "NavyShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of navy-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Pirate:
                    upgradeDetail.m_UpgradeName = "Pirate";
                    upgradeDetail.GumpCollectionId = "PirateShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of pirate-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Orc:
                    upgradeDetail.m_UpgradeName = "Orc";
                    upgradeDetail.GumpCollectionId = "OrcShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of orc-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Undead:
                    upgradeDetail.m_UpgradeName = "Undead";
                    upgradeDetail.GumpCollectionId = "UndeadShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of undead-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Treasure:
                    upgradeDetail.m_UpgradeName = "Treasure";
                    upgradeDetail.GumpCollectionId = "TreasureShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of treasure-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Derelict:
                    upgradeDetail.m_UpgradeName = "Derelict";
                    upgradeDetail.GumpCollectionId = "DerelictShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of derelict-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Merchant:
                    upgradeDetail.m_UpgradeName = "Merchant";
                    upgradeDetail.GumpCollectionId = "MerchantShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of merchant-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Daemonic:
                    upgradeDetail.m_UpgradeName = "Daemonic";
                    upgradeDetail.GumpCollectionId = "DaemonicShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of daemonic-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Eldritch:
                    upgradeDetail.m_UpgradeName = "Eldritch";
                    upgradeDetail.GumpCollectionId = "EldritchShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of eldritch-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;

                case ThemeType.Massacre:
                    upgradeDetail.m_UpgradeName = "Massacre";
                    upgradeDetail.GumpCollectionId = "MassacreShipThemeUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Adds a variety of massacre-themed decorations", ShipUpgradeDetail.UpgradeEffectType.Theme));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("to your ship.", ShipUpgradeDetail.UpgradeEffectType.Theme));
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Paint Upgrade

        public static ShipUpgradeDetail GetPaintDetail(PaintType paintUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Paint;

            switch (paintUpgrade)
            {
                case PaintType.DarkGrey:
                    upgradeDetail.m_UpgradeName = "Dark Grey";                    
                    upgradeDetail.GumpCollectionId = "DarkGreyShipPaintUpgrade";

                    upgradeDetail.PaintHue = 1105;
                    upgradeDetail.PaintTextHue = 1105;

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Changes the color of the ship (Hue 1105)", ShipUpgradeDetail.UpgradeEffectType.Paint));
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Cannon Metal Upgrade

        public static ShipUpgradeDetail GetCannonMetalDetail(CannonMetalType cannonMetalUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.CannonMetal;

            switch (cannonMetalUpgrade)
            {
                case CannonMetalType.Bloodstone:
                    upgradeDetail.m_UpgradeName = "Bloodstone";
                    upgradeDetail.GumpCollectionId = "BloodstoneShipCannonMetalUpgrade";

                    upgradeDetail.CannonMetalHue = 2117;
                    upgradeDetail.CannonMetalTextHue = 2116;

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Changes the color of the ship's cannons (Hue 2117)", ShipUpgradeDetail.UpgradeEffectType.CannonMetal));
                    
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Outfitting Upgrade

        public static ShipUpgradeDetail GetOutfittingDetail(OutfittingType outfittingUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Outfitting;

            switch (outfittingUpgrade)
            {
                case OutfittingType.Hunter:
                    upgradeDetail.m_UpgradeName = "Hunter";
                    upgradeDetail.GumpCollectionId = "HunterShipOutfittingUpgrade";

                    upgradeDetail.ForwardSpeed = .10;
                    upgradeDetail.DriftSpeed = .10;
                    upgradeDetail.MinorAbilityCooldown = .20;                  
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Banner Upgrade

        public static ShipUpgradeDetail GetBannerDetail(BannerType bannerUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Banner;

            switch (bannerUpgrade)
            {
                case BannerType.Corsairs:
                    upgradeDetail.m_UpgradeName = "Corsairs";
                    upgradeDetail.GumpCollectionId = "CorsairsShipBannerUpgrade";

                    upgradeDetail.BoardingChance = .10;
                    upgradeDetail.DoubloonsEarnedFromEnemyCrew = .10;                    
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Charm Upgrade

        public static ShipUpgradeDetail GetCharmDetail(CharmType charmUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.Banner;

            switch (charmUpgrade)
            {
                case CharmType.BarrelOfLimes:
                    upgradeDetail.m_UpgradeName = "Barrel of Limes";
                    upgradeDetail.GumpCollectionId = "BarrelOfLimesShipCharmUpgrade";

                    upgradeDetail.CrewDamageMeleeDamageDealt = .10;
                    upgradeDetail.CrewDamageMeleeDamageReceived = .10;
                    upgradeDetail.CrewHealingReceived = .25;
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Minor Ability Upgrade

        public static ShipUpgradeDetail GetMinorAbilityDetail(MinorAbilityType minorAbilityUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.MinorAbility;

            switch (minorAbilityUpgrade)
            {
                case MinorAbilityType.ExpediteRepairs:
                    upgradeDetail.m_UpgradeName = "Expedite Repairs";                    
                    upgradeDetail.GumpCollectionId = "ExpediteRepairsShipMinorAbilityUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("+25% Repair Effectiveness for 30 seconds", ShipUpgradeDetail.UpgradeEffectType.MinorAbility));
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Major Ability Upgrade

        public static ShipUpgradeDetail GetMajorAbilityDetail(MajorAbilityType majorAbilityUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.MajorAbility;

            switch (majorAbilityUpgrade)
            {
                case MajorAbilityType.Smokescreen:
                    upgradeDetail.m_UpgradeName = "Smokescreen";
                    upgradeDetail.GumpCollectionId = "SmokescreenShipMajorAbilityUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Pacifies creatures hit for 10 seconds", ShipUpgradeDetail.UpgradeEffectType.MajorAbility));                   
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion

        #region Epic Ability Upgrade

        public static ShipUpgradeDetail GetEpicAbilityDetail(EpicAbilityType epicAbilityUpgrade)
        {
            ShipUpgradeDetail upgradeDetail = new ShipUpgradeDetail();

            upgradeDetail.m_UpgradeType = UpgradeType.MajorAbility;

            switch (epicAbilityUpgrade)
            {
                case EpicAbilityType.Hellfire:
                    upgradeDetail.m_UpgradeName = "Hellfire";                    
                    upgradeDetail.GumpCollectionId = "HellfireShipEpicAbilityUpgrade";

                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("Unleashes a damaging barrage of fire that", ShipUpgradeDetail.UpgradeEffectType.EpicAbility));
                    upgradeDetail.m_SpecialEffects.Add(new KeyValuePair<string, ShipUpgradeDetail.UpgradeEffectType>("continues to burn for 30 seconds, damaging targets", ShipUpgradeDetail.UpgradeEffectType.EpicAbility));
                break;
            }

            if (upgradeDetail.m_UpgradeName == "")
                return null;

            else
                return upgradeDetail;
        }

        #endregion
    }   

    public class ShipUpgradeDetail
    {
        public enum UpgradeEffectType
        {           
            Theme,
            Paint,
            CannonMetal,
            MinorAbility,
            MajorAbility,
            EpicAbility
        }

        public ShipUpgrades.UpgradeType m_UpgradeType = ShipUpgrades.UpgradeType.Theme;
        public string m_UpgradeName = "";        
        public string GumpCollectionId = "";

        public List<KeyValuePair<string, UpgradeEffectType>> m_SpecialEffects = new List<KeyValuePair<string, UpgradeEffectType>>();

        public int PaintHue = 0;
        public int PaintTextHue = 0;

        public int CannonMetalHue = 0;
        public int CannonMetalTextHue = 0;

        //Core Stats
        public double HoldCapacity = 0.0;

        public double MaxHitPoints = 0.0;
        public double MaxSailPoints = 0.0;
        public double MaxGunPoints = 0.0;

        public double ForwardSpeed = 0.0;
        public double DriftSpeed = 0.0;
        public double SlowdownModePenalty = 0.0;

        public double CannonAccuracy = 0.0;
        public double CannonDamage = 0.0;       
        public double CannonRange = 0.0;
        public double CannonReloadTime = 0.0;

        public double MinorAbilityCooldown = 0.0;        
        public double MajorAbilityCooldown = 0.0;       
        public double EpicAbilityCooldown = 0.0; 

        public double RepairCooldown = 0.0;    
        public double BoardingChance = 0.0;

        //Non-Core Stats
        public double CannonEnemyCrewDamage = 0.0;
        public double CannonHitPointsDamage = 0.0;
        public double CannonSailPointsDamage = 0.0;
        public double CannonGunPointsDamage = 0.0;

        public double CrewDamageMeleeDamageDealt = 0.0;
        public double CrewDamageSpellDamageDealt = 0.0;

        public double CrewDamageMeleeDamageReceived = 0.0;
        public double CrewDamageSpellDamageReceived = 0.0;
        public double CrewDamageCannonDamageReceived = 0.0;

        public double RepairHitPointsBonus = 0.0;
        public double RepairSailPointsBonus = 0.0;
        public double RepairGunPointsBonus = 0.0;
        public double RepairMaterialsReduction = 0.0;

        public double MinorAbilityEffectiveness = 0.0;
        public double MajorAbilityEffectiveness = 0.0;       
        public double EpicAbilityEffectiveness = 0.0;

        public double CrewHealingReceived = 0.0;

        public double DoubloonsEarnedFromEnemyCrew = 0.0;
        public double DoubloonsEarnedFromEnemyShips = 0.0;
        
        public double FishingSuccess = 0.0;

        public double SpyglassAccuracy = 0.0;

        public ShipUpgradeDetail()
        {
        }

        public int GetHue(UpgradeEffectType statType)
        {
            switch (statType)
            {
                /*
                case UpgradeEffectType.Stats: return 149; break;
                case UpgradeEffectType.Speed: return 2599; break;
                case UpgradeEffectType.Cannon: return 2401; break;
                case UpgradeEffectType.Abilities: return 2603; break;
                case UpgradeEffectType.Special: return 2606; break;
                case UpgradeEffectType.Misc: return 53; break;
                */
            }

            return 2499;
        }
    }
}