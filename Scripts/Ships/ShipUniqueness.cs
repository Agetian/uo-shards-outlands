using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server
{
    public static class ShipUniqueness
    {
        public static void GenerateShipUniqueness(BaseShip ship)
        {
            if (ship == null) return;
            if (ship.Deleted) return;            

            if (ship.MobileFactionType != MobileFactionType.None)
                ConfigureNPCShip(ship);

            else
            {
                ShipStatsProfile shipStatsProfile = GetShipStatsProfile(null, ship, true, true);
            }
        }

        public static ShipStatsProfile GetShipStatsProfile(BaseShipDeed shipDeed, BaseShip ship, bool applyCreationModifiers, bool applyUpgradeModifiers)
        {
            Type type = null;

            if (shipDeed != null)
                type = shipDeed.ShipType;

            if (ship != null)
                type = ship.GetType();           

            ShipStatsProfile shipStatsProfile = new ShipStatsProfile();

            if (type == null)
                return shipStatsProfile;

            shipStatsProfile.shipType = type;

            #region Small Ship

            if (type == typeof(SmallShip) || type == typeof(SmallDragonShip))
            {
                if (type == typeof(SmallShip))
                    shipStatsProfile.ShipTypeName = "small ship";

                if (type == typeof(SmallDragonShip))
                    shipStatsProfile.ShipTypeName = "small dragon ship";
            }

            #endregion

            #region Medium Ship

            if (type == typeof(MediumShip) || type == typeof(MediumDragonShip))
            {
                if (type == typeof(MediumShip))
                    shipStatsProfile.ShipTypeName = "medium ship";

                if (type == typeof(MediumDragonShip))
                    shipStatsProfile.ShipTypeName = "medium dragon ship";

                shipStatsProfile.RegistrationDoubloonCost = 500;
                shipStatsProfile.UpgradeDoubloonMultiplier = 1.5;

                shipStatsProfile.HoldSize = 75;

                shipStatsProfile.MaxHitPoints = 1250;
                shipStatsProfile.MaxSailPoints = 625;
                shipStatsProfile.MaxGunPoints = 625;

                shipStatsProfile.ForwardSpeed = 0.25;
                shipStatsProfile.DriftSpeed = 0.5;

                shipStatsProfile.CannonsPerSide = 4;
                shipStatsProfile.CannonReloadDuration = 8;                
            }

            #endregion

            #region Large Ship

            if (type == typeof(LargeShip) || type == typeof(LargeDragonShip))
            {
                if (type == typeof(LargeShip))
                    shipStatsProfile.ShipTypeName = "large ship";

                if (type == typeof(LargeDragonShip))
                    shipStatsProfile.ShipTypeName = "large dragon ship";

                shipStatsProfile.RegistrationDoubloonCost = 1500;
                shipStatsProfile.UpgradeDoubloonMultiplier = 2.0;

                shipStatsProfile.HoldSize = 100;

                shipStatsProfile.MaxHitPoints = 1500;
                shipStatsProfile.MaxSailPoints = 750;
                shipStatsProfile.MaxGunPoints = 750;

                shipStatsProfile.ForwardSpeed = 0.3;
                shipStatsProfile.DriftSpeed = 0.6;

                shipStatsProfile.CannonsPerSide = 5;
                shipStatsProfile.CannonReloadDuration = 10;  
            }

            #endregion

            #region Carrack

            if (type == typeof(Carrack))
            {                                  
                shipStatsProfile.ShipTypeName = "carrack";

                shipStatsProfile.RegistrationDoubloonCost = 5000;
                shipStatsProfile.UpgradeDoubloonMultiplier = 3.0;

                shipStatsProfile.HoldSize = 125;

                shipStatsProfile.MaxHitPoints = 2000;
                shipStatsProfile.MaxSailPoints = 1000;
                shipStatsProfile.MaxGunPoints = 1000;

                shipStatsProfile.ForwardSpeed = 0.35;
                shipStatsProfile.DriftSpeed = 0.7;

                shipStatsProfile.CannonsPerSide = 6;
                shipStatsProfile.CannonReloadDuration = 12;                
            }

            #endregion

            #region Galleon

            if (type == typeof(Galleon))
            {
                shipStatsProfile.ShipTypeName = "galleon";

                shipStatsProfile.RegistrationDoubloonCost = 20000;
                shipStatsProfile.UpgradeDoubloonMultiplier = 4.0;

                shipStatsProfile.HoldSize = 150;

                shipStatsProfile.MaxHitPoints = 3000;
                shipStatsProfile.MaxSailPoints = 1500;
                shipStatsProfile.MaxGunPoints = 1500;

                shipStatsProfile.ForwardSpeed = 0.4;
                shipStatsProfile.DriftSpeed = 0.8;

                shipStatsProfile.CannonsPerSide = 7;
                shipStatsProfile.CannonReloadDuration = 14;
            }

            #endregion

            #region Ship of the Line

            if (type == typeof(ShipOfTheLineShip))
            {
                shipStatsProfile.ShipTypeName = "ship of the line";

                shipStatsProfile.RegistrationDoubloonCost = 50000;
                shipStatsProfile.UpgradeDoubloonMultiplier = 5.0;

                shipStatsProfile.HoldSize = 200;

                shipStatsProfile.MaxHitPoints = 4000;
                shipStatsProfile.MaxSailPoints = 2000;
                shipStatsProfile.MaxGunPoints = 2000;

                shipStatsProfile.ForwardSpeed = 0.45;
                shipStatsProfile.DriftSpeed = 0.9;

                shipStatsProfile.CannonsPerSide = 8;
                shipStatsProfile.CannonReloadDuration = 16;
            }

            #endregion

            if (applyCreationModifiers)
                shipStatsProfile = ApplyCreationModifiers(shipDeed, ship, shipStatsProfile);

            if (applyUpgradeModifiers)
                shipStatsProfile = ApplyUpgradeModifiers(shipDeed, ship, shipStatsProfile);

            shipStatsProfile.CalculateAdjustedValues();

            return shipStatsProfile;
        }

        public static ShipStatsProfile ApplyCreationModifiers(BaseShipDeed shipDeed, BaseShip ship, ShipStatsProfile shipStatsProfile)
        {
            if (shipStatsProfile == null)
                return shipStatsProfile;

            #region Get Properties

            if (shipDeed != null)
            {
                shipStatsProfile.HoldSizeCreationModifier = shipDeed.HoldSizeCreationModifier;

                shipStatsProfile.MaxHitPointsCreationModifier = shipDeed.MaxHitPointsCreationModifier;
                shipStatsProfile.MaxSailPointsCreationModifier = shipDeed.MaxSailPointsCreationModifier;
                shipStatsProfile.MaxGunPointsCreationModifier = shipDeed.MaxGunPointsCreationModifier;

                shipStatsProfile.ForwardSpeedCreationModifier = shipDeed.ForwardSpeedCreationModifier;
                shipStatsProfile.DriftSpeedCreationModifier = shipDeed.DriftSpeedCreationModifier;
                shipStatsProfile.SlowdownModePenaltyCreationModifier = shipDeed.SlowdownModePenaltyCreationModifier;

                shipStatsProfile.CannonAccuracyCreationModifier = shipDeed.CannonAccuracyCreationModifier;
                shipStatsProfile.CannonDamageCreationModifier = shipDeed.CannonDamageCreationModifier;
                shipStatsProfile.CannonRangeCreationModifier = shipDeed.CannonRangeCreationModifier;
                shipStatsProfile.CannonReloadDurationCreationModifier = shipDeed.CannonReloadDurationCreationModifier;

                shipStatsProfile.MinorAbilityCooldownDurationCreationModifier = shipDeed.MinorAbilityCooldownDurationCreationModifier;
                shipStatsProfile.MajorAbilityCooldownDurationCreationModifier = shipDeed.MajorAbilityCooldownDurationCreationModifier;
                shipStatsProfile.EpicAbilityCooldownDurationCreationModifier = shipDeed.EpicAbilityCooldownDurationCreationModifier;

                shipStatsProfile.RepairCooldownDurationCreationModifier = shipDeed.RepairCooldownDurationCreationModifier;
                shipStatsProfile.BoardingChanceCreationModifier = shipDeed.BoardingChanceCreationModifier;
            }

            if (ship != null)
            {
                shipStatsProfile.HoldSizeCreationModifier = ship.HoldSizeCreationModifier;

                shipStatsProfile.MaxHitPointsCreationModifier = ship.MaxHitPointsCreationModifier;
                shipStatsProfile.MaxSailPointsCreationModifier = ship.MaxSailPointsCreationModifier;
                shipStatsProfile.MaxGunPointsCreationModifier = ship.MaxGunPointsCreationModifier;

                shipStatsProfile.ForwardSpeedCreationModifier = ship.ForwardSpeedCreationModifier;
                shipStatsProfile.DriftSpeedCreationModifier = ship.DriftSpeedCreationModifier;
                shipStatsProfile.SlowdownModePenaltyCreationModifier = ship.SlowdownModePenaltyCreationModifier;

                shipStatsProfile.CannonAccuracyCreationModifier = ship.CannonAccuracyCreationModifier;
                shipStatsProfile.CannonDamageCreationModifier = ship.CannonDamageCreationModifier;
                shipStatsProfile.CannonRangeCreationModifier = ship.CannonRangeCreationModifier;
                shipStatsProfile.CannonReloadDurationCreationModifier = ship.CannonReloadDurationCreationModifier;

                shipStatsProfile.MinorAbilityCooldownDurationCreationModifier = ship.MinorAbilityCooldownDurationCreationModifier;
                shipStatsProfile.MajorAbilityCooldownDurationCreationModifier = ship.MajorAbilityCooldownDurationCreationModifier;
                shipStatsProfile.EpicAbilityCooldownDurationCreationModifier = ship.EpicAbilityCooldownDurationCreationModifier;

                shipStatsProfile.RepairCooldownDurationCreationModifier = ship.RepairCooldownDurationCreationModifier;
                shipStatsProfile.BoardingChanceCreationModifier = ship.BoardingChanceCreationModifier;
            }

            #endregion

            return shipStatsProfile;
        }

        public static ShipStatsProfile ApplyUpgradeModifiers(BaseShipDeed shipDeed, BaseShip ship, ShipStatsProfile shipStatsProfile)
        {
            if (shipStatsProfile == null)
                return shipStatsProfile;

            ShipUpgrades.ThemeType m_ThemeUpgrade = ShipUpgrades.ThemeType.None;
            ShipUpgrades.PaintType m_PaintUpgrade = ShipUpgrades.PaintType.None;
            ShipUpgrades.CannonMetalType m_CannonMetalUpgrade = ShipUpgrades.CannonMetalType.None;

            ShipUpgrades.OutfittingType m_OutfittingUpgrade = ShipUpgrades.OutfittingType.None;
            ShipUpgrades.BannerType m_BannerUpgrade = ShipUpgrades.BannerType.None;
            ShipUpgrades.CharmType m_CharmUpgrade = ShipUpgrades.CharmType.None;

            ShipUpgrades.MinorAbilityType m_MinorAbilityUpgrade = ShipUpgrades.MinorAbilityType.None;
            ShipUpgrades.MajorAbilityType m_MajorAbilityUpgrade = ShipUpgrades.MajorAbilityType.None;
            ShipUpgrades.EpicAbilityType m_EpicAbilityUpgrade = ShipUpgrades.EpicAbilityType.None;

            #region Get Properties

            if (shipDeed != null)
            {
                m_ThemeUpgrade = shipDeed.m_ThemeUpgrade;
                m_PaintUpgrade = shipDeed.m_PaintUpgrade;
                m_CannonMetalUpgrade = shipDeed.m_CannonMetalUpgrade;

                m_OutfittingUpgrade = shipDeed.m_OutfittingUpgrade;
                m_BannerUpgrade = shipDeed.m_BannerUpgrade;
                m_CharmUpgrade = shipDeed.m_CharmUpgrade;

                m_MinorAbilityUpgrade = shipDeed.m_MinorAbilityUpgrade;
                m_MajorAbilityUpgrade = shipDeed.m_MajorAbilityUpgrade;
                m_EpicAbilityUpgrade = shipDeed.m_EpicAbilityUpgrade;
            }

            if (ship != null)
            {
                m_ThemeUpgrade = ship.m_ThemeUpgrade;
                m_PaintUpgrade = ship.m_PaintUpgrade;
                m_CannonMetalUpgrade = ship.m_CannonMetalUpgrade;

                m_OutfittingUpgrade = ship.m_OutfittingUpgrade;
                m_BannerUpgrade = ship.m_BannerUpgrade;
                m_CharmUpgrade = ship.m_CharmUpgrade;

                m_MinorAbilityUpgrade = ship.m_MinorAbilityUpgrade;
                m_MajorAbilityUpgrade = ship.m_MajorAbilityUpgrade;
                m_EpicAbilityUpgrade = ship.m_EpicAbilityUpgrade;
            }

            #endregion

            #region Apply Upgrades

            switch (m_OutfittingUpgrade)
            {
                case ShipUpgrades.OutfittingType.Hunter: 
                break;
            }

            switch (m_BannerUpgrade)
            {
                case ShipUpgrades.BannerType.Corsairs:
                break;
            }

            switch (m_CharmUpgrade)
            {
                case ShipUpgrades.CharmType.BarrelOfLimes:
                break;
            }

            #endregion

            return shipStatsProfile;
        }

        public static void ApplyShipStatsProfile(BaseShip ship, ShipStatsProfile shipStatsProfile)
        {
            if (ship == null) return;
            if (shipStatsProfile == null) return;

            if (ship.Hold != null)
                ship.Hold.MaxItems = shipStatsProfile.HoldSizeAdjusted;

            ship.MaxHitPoints = shipStatsProfile.MaxHitPointsAdjusted;
            ship.MaxSailPoints = shipStatsProfile.MaxSailPointsAdjusted;
            ship.MaxGunPoints = shipStatsProfile.MaxGunPointsAdjusted;

            ship.ForwardSpeed = shipStatsProfile.ForwardSpeedAdjusted;
            ship.DriftSpeed = shipStatsProfile.DriftSpeedAdjusted;
            ship.SlowdownModePenalty = shipStatsProfile.SlowdownModePenaltyAdjusted;

            ship.CannonAccuracy = shipStatsProfile.CannonAccuracyAdjusted;
            ship.CannonMinDamage = shipStatsProfile.CannonMinDamage;
            ship.CannonMaxDamage = shipStatsProfile.CannonMaxDamage;
            ship.CannonRange = shipStatsProfile.CannonRange;
            ship.CannonReloadDuration = shipStatsProfile.CannonReloadDuration;

            ship.MinorAbilityCooldownDuration = shipStatsProfile.MinorAbilityCooldownDuration;
            ship.MajorAbilityCooldownDuration = shipStatsProfile.MajorAbilityCooldownDuration;
            ship.EpicAbilityCooldownDuration = shipStatsProfile.EpicAbilityCooldownDuration;

            ship.RepairCooldownDuration = shipStatsProfile.RepairCooldownDuration;
            ship.BoardingChance = shipStatsProfile.BoardingChance;
        }

        public static void ConfigureNPCShip(BaseShip ship)
        {
            Type type = ship.GetType();

            if (type == null)
                return;

            ShipStatsProfile shipStatsProfile = new ShipStatsProfile();

            shipStatsProfile.shipType = type;
            
            #region Small Ship

            if (type == typeof(SmallShip) || type == typeof(SmallDragonShip))
            {
            }

            #endregion

            #region Medium Ship

            if (type == typeof(MediumShip) || type == typeof(MediumDragonShip))
            {
            }

            #endregion

            #region Large Ship

            if (type == typeof(LargeShip) || type == typeof(LargeDragonShip))
            {
            }

            #endregion

            #region Carrack

            if (type == typeof(Carrack))
            {
            }

            #endregion

            #region Galleon

            if (type == typeof(Galleon))
            {
            }

            #endregion

            #region Ship of the Line

            if (type == typeof(ShipOfTheLineShip))
            {
            }

            #endregion
        }

        public static int GetShipUpgradeBaseDoubloonCost(ShipUpgrades.UpgradeType upgradeType)
        {
            int doubloonCost = 500;

            switch (upgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.Paint: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.CannonMetal: doubloonCost = 1000; break;

                case ShipUpgrades.UpgradeType.Outfitting: doubloonCost = 2000; break;
                case ShipUpgrades.UpgradeType.Banner: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.Charm: doubloonCost = 1000; break;

                case ShipUpgrades.UpgradeType.MinorAbility: doubloonCost = 500; break;
                case ShipUpgrades.UpgradeType.MajorAbility: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.EpicAbility: doubloonCost = 2000; break;
            }

            return doubloonCost;
        }  
    }

    public class ShipStatsProfile
    {
        public Type shipType = null;
        public string ShipTypeName = "small ship";

        public int DoubloonSinkValue = 0;

        public int RegistrationDoubloonCost = 0;
        public double UpgradeDoubloonMultiplier = BaseShip.BaseUpgradeDoubloonMultiplier;      

        //Base Ship Values
        public int HoldSize = BaseShip.BaseHoldSize;

        public int MaxHitPoints = BaseShip.BaseHitPoints;
        public int MaxSailPoints = BaseShip.BaseSailPoints;
        public int MaxGunPoints = BaseShip.BaseGunPoints;

        public double ForwardSpeed = BaseShip.BaseForwardSpeed;
        public double DriftSpeed = BaseShip.BaseDriftSpeed;
        public double SlowdownModePenalty = BaseShip.BaseSlowdownPenalty;

        public int CannonsPerSide = BaseShip.BaseCannonsPerSide;
        public double CannonAccuracy = BaseShip.BaseCannonAccuracy;
        public double CannonMinDamage = BaseShip.BaseCannonDamageMin;
        public double CannonMaxDamage = BaseShip.BaseCannonDamageMax;
        public double CannonRange = BaseShip.BaseCannonRange;
        public double CannonReloadDuration = BaseShip.BaseCannonReloadDuration;

        public double MinorAbilityCooldownDuration = BaseShip.BaseMinorAbilityCooldown;
        public double MajorAbilityCooldownDuration = BaseShip.BaseMajorAbilityCooldown;
        public double EpicAbilityCooldownDuration = BaseShip.BaseEpicAbilityCooldown;

        public double RepairCooldownDuration = BaseShip.BaseRepairCooldown;
        public double BoardingChance = BaseShip.BaseBoardingChance;

        //Creation Modifiers
        public double HoldSizeCreationModifier = 0;

        public double MaxHitPointsCreationModifier = 0;
        public double MaxSailPointsCreationModifier = 0;
        public double MaxGunPointsCreationModifier = 0;

        public double ForwardSpeedCreationModifier = 0;
        public double DriftSpeedCreationModifier = 0;
        public double SlowdownModePenaltyCreationModifier = 0;

        public double CannonAccuracyCreationModifier = 0;
        public double CannonDamageCreationModifier = 0;
        public double CannonRangeCreationModifier = 0;
        public double CannonReloadDurationCreationModifier = 0;

        public double MinorAbilityCooldownDurationCreationModifier = 0;
        public double MajorAbilityCooldownDurationCreationModifier = 0;
        public double EpicAbilityCooldownDurationCreationModifier = 0;

        public double RepairCooldownDurationCreationModifier = 0;
        public double BoardingChanceCreationModifier = 0;

        //Upgrade Modifiers
        public double HoldSizeUpgradeModifier = 0;

        public double MaxHitPointsUpgradeModifier = 0;
        public double MaxSailPointsUpgradeModifier = 0;
        public double MaxGunPointsUpgradeModifier = 0;

        public double ForwardSpeedUpgradeModifier = 0;
        public double DriftSpeedUpgradeModifier = 0;
        public double SlowdownModePenaltyUpgradeModifier = 0;

        public double CannonAccuracyUpgradeModifier = 0;
        public double CannonDamageUpgradeModifier = 0;
        public double CannonRangeUpgradeModifier = 0;
        public double CannonReloadDurationUpgradeModifier = 0;

        public double MinorAbilityCooldownDurationUpgradeModifier = 0;
        public double MajorAbilityCooldownDurationUpgradeModifier = 0;
        public double EpicAbilityCooldownDurationUpgradeModifier = 0;

        public double RepairCooldownDurationUpgradeModifier = 0;
        public double BoardingChanceUpgradeModifier = 0;

        //Total Values
        public int HoldSizeAdjusted = 0;

        public int MaxHitPointsAdjusted = 0;
        public int MaxSailPointsAdjusted = 0;
        public int MaxGunPointsAdjusted = 0;

        public double ForwardSpeedAdjusted = 0;
        public double DriftSpeedAdjusted = 0;
        public double SlowdownModePenaltyAdjusted = 0;

        public double CannonAccuracyAdjusted = 0;
        public double CannonMinDamageAdjusted = 0;
        public double CannonMaxDamageAdjusted = 0;
        public double CannonRangeAdjusted = 0;
        public double CannonReloadDurationAdjusted = 0;

        public double MinorAbilityCooldownDurationAdjusted = 0;
        public double MajorAbilityCooldownDurationAdjusted = 0;
        public double EpicAbilityCooldownDurationAdjusted = 0;

        public double RepairCooldownDurationAdjusted = 0;
        public double BoardingChanceAdjusted = 0;

        public ShipStatsProfile()
        {
        }

        public void CalculateAdjustedValues()
        {
            HoldSizeAdjusted = (int)(Math.Round(HoldSize * (1 + HoldSizeCreationModifier + HoldSizeUpgradeModifier)));

            MaxHitPointsAdjusted = (int)(Math.Round((double)MaxHitPoints * (1 + MaxHitPointsCreationModifier + MaxHitPointsUpgradeModifier)));
            MaxSailPointsAdjusted = (int)(Math.Round((double)MaxSailPoints * (1 + MaxSailPointsCreationModifier + MaxSailPointsUpgradeModifier)));
            MaxGunPointsAdjusted = (int)(Math.Round((double)MaxGunPoints * (1 + MaxGunPointsCreationModifier + MaxGunPointsUpgradeModifier)));

            ForwardSpeedAdjusted = ForwardSpeed * (1 - ForwardSpeedCreationModifier - ForwardSpeedUpgradeModifier);
            DriftSpeedAdjusted = DriftSpeed * (1 - DriftSpeedCreationModifier - DriftSpeedUpgradeModifier);
            SlowdownModePenaltyAdjusted = SlowdownModePenalty * (1 - SlowdownModePenaltyCreationModifier - SlowdownModePenaltyUpgradeModifier);

            CannonAccuracyAdjusted = CannonAccuracy * (1 + CannonAccuracyCreationModifier + CannonAccuracyUpgradeModifier);
            CannonMinDamageAdjusted = CannonMinDamage * (1 + CannonDamageCreationModifier + CannonDamageUpgradeModifier);
            CannonMaxDamageAdjusted = CannonMaxDamage * (1 + CannonDamageCreationModifier + CannonDamageUpgradeModifier);
            CannonRangeAdjusted = CannonRange * (1 + CannonRangeCreationModifier + CannonRangeUpgradeModifier);
            CannonReloadDurationAdjusted = CannonReloadDuration * (1 - CannonReloadDurationCreationModifier - CannonReloadDurationUpgradeModifier);

            MinorAbilityCooldownDurationAdjusted = Math.Round(MinorAbilityCooldownDuration * (1 - MinorAbilityCooldownDurationCreationModifier - MinorAbilityCooldownDurationUpgradeModifier));
            MajorAbilityCooldownDurationAdjusted = Math.Round(MajorAbilityCooldownDuration * (1 - MajorAbilityCooldownDurationCreationModifier - MajorAbilityCooldownDurationUpgradeModifier));
            EpicAbilityCooldownDurationAdjusted = Math.Round(EpicAbilityCooldownDuration * (1 - EpicAbilityCooldownDurationCreationModifier - EpicAbilityCooldownDurationUpgradeModifier));

            RepairCooldownDurationAdjusted = Math.Round(RepairCooldownDuration * (1 - RepairCooldownDurationCreationModifier - RepairCooldownDurationUpgradeModifier));
            BoardingChanceAdjusted = BoardingChance * (1 + BoardingChanceCreationModifier + BoardingChanceUpgradeModifier);
        }
    }
}