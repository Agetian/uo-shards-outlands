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
        public static void GenerateCreationModifiers(BaseShipDeed shipDeed, BaseShip ship, Mobile from, Quality quality)
        {
            double lowRange = 0.0;
            double highRange = 0.2;

            double statMutationChance = .33;

            if (from != null)
            {
                if (from.Skills.Carpentry.Value > 100.0)
                    lowRange += .1 * ((from.Skills.Carpentry.Value - 100) / 20);
            }

            else
            {
                highRange = 0.1;
                statMutationChance = .66;
            }

            #region Determine and Set Creation Modifiers

            if (shipDeed != null)
            {
                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.HoldSizeCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.MaxHitPointsCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.MaxSailPointsCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.MaxGunPointsCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.ForwardSpeedCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.DriftSpeedCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.SlowdownModePenaltyCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.CannonAccuracyCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.CannonDamageCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.CannonRangeCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.CannonReloadDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.MinorAbilityCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.MajorAbilityCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.EpicAbilityCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.RepairCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    shipDeed.BoardingChanceCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));
            }

            if (ship != null)
            {
                if (Utility.RandomDouble() <= statMutationChance)
                    ship.HoldSizeCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.MaxHitPointsCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.MaxSailPointsCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.MaxGunPointsCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.ForwardSpeedCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.DriftSpeedCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.SlowdownModePenaltyCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.CannonAccuracyCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.CannonDamageCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.CannonRangeCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.CannonReloadDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.MinorAbilityCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.MajorAbilityCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.EpicAbilityCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.RepairCooldownDurationCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));

                if (Utility.RandomDouble() <= statMutationChance)
                    ship.BoardingChanceCreationModifier = -.1 + (lowRange + (Utility.RandomDouble() * (highRange - lowRange)));
            }

            #endregion
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

                if (ship != null)
                {
                    if (ship.MobileControlType == MobileControlType.Player)
                    {
                    }

                    else
                    {
                    }
                }
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

                shipStatsProfile.ForwardSpeed = 0.25;
                shipStatsProfile.DriftSpeed = 0.5;

                shipStatsProfile.CannonsPerSide = 4;
                shipStatsProfile.CannonReloadDuration = 8;

                if (ship != null)
                {
                    if (ship.MobileControlType == MobileControlType.Player)
                    {
                        shipStatsProfile.MaxHitPoints = 1250;
                        shipStatsProfile.MaxSailPoints = 625;
                        shipStatsProfile.MaxGunPoints = 625;
                    }

                    else
                    {
                        shipStatsProfile.MaxHitPoints = 1250;
                        shipStatsProfile.MaxSailPoints = 625;
                        shipStatsProfile.MaxGunPoints = 625;
                    }
                }
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

                shipStatsProfile.ForwardSpeed = 0.3;
                shipStatsProfile.DriftSpeed = 0.6;

                shipStatsProfile.CannonsPerSide = 5;
                shipStatsProfile.CannonReloadDuration = 10;

                if (ship.MobileControlType == MobileControlType.Player)
                {
                    shipStatsProfile.MaxHitPoints = 1500;
                    shipStatsProfile.MaxSailPoints = 750;
                    shipStatsProfile.MaxGunPoints = 750;
                }

                else
                {
                    shipStatsProfile.MaxHitPoints = 1500;
                    shipStatsProfile.MaxSailPoints = 750;
                    shipStatsProfile.MaxGunPoints = 750;
                }
            }

            #endregion

            #region Carrack

            if (type == typeof(Carrack))
            {                                  
                shipStatsProfile.ShipTypeName = "carrack";

                shipStatsProfile.RegistrationDoubloonCost = 5000;
                shipStatsProfile.UpgradeDoubloonMultiplier = 3.0;

                shipStatsProfile.HoldSize = 125;

                shipStatsProfile.ForwardSpeed = 0.35;
                shipStatsProfile.DriftSpeed = 0.7;

                shipStatsProfile.CannonsPerSide = 6;
                shipStatsProfile.CannonReloadDuration = 12;

                if (ship.MobileControlType == MobileControlType.Player)
                {
                    shipStatsProfile.MaxHitPoints = 2000;
                    shipStatsProfile.MaxSailPoints = 1000;
                    shipStatsProfile.MaxGunPoints = 1000;
                }

                else
                {
                    shipStatsProfile.MaxHitPoints = 2000;
                    shipStatsProfile.MaxSailPoints = 1000;
                    shipStatsProfile.MaxGunPoints = 1000;
                }
            }

            #endregion

            #region Galleon

            if (type == typeof(Galleon))
            {
                shipStatsProfile.ShipTypeName = "galleon";

                shipStatsProfile.RegistrationDoubloonCost = 20000;
                shipStatsProfile.UpgradeDoubloonMultiplier = 4.0;

                shipStatsProfile.HoldSize = 150;

                shipStatsProfile.ForwardSpeed = 0.4;
                shipStatsProfile.DriftSpeed = 0.8;

                shipStatsProfile.CannonsPerSide = 7;
                shipStatsProfile.CannonReloadDuration = 14;

                if (ship.MobileControlType == MobileControlType.Player)
                {
                    shipStatsProfile.MaxHitPoints = 3000;
                    shipStatsProfile.MaxSailPoints = 1500;
                    shipStatsProfile.MaxGunPoints = 1500;
                }

                else
                {
                    shipStatsProfile.MaxHitPoints = 3000;
                    shipStatsProfile.MaxSailPoints = 1500;
                    shipStatsProfile.MaxGunPoints = 1500;
                }
            }

            #endregion

            #region Ship of the Line

            if (type == typeof(ShipOfTheLineShip))
            {
                shipStatsProfile.ShipTypeName = "ship of the line";

                shipStatsProfile.RegistrationDoubloonCost = 50000;
                shipStatsProfile.UpgradeDoubloonMultiplier = 5.0;

                shipStatsProfile.HoldSize = 200;

                shipStatsProfile.ForwardSpeed = 0.45;
                shipStatsProfile.DriftSpeed = 0.9;

                shipStatsProfile.CannonsPerSide = 8;
                shipStatsProfile.CannonReloadDuration = 16;

                if (ship.MobileControlType == MobileControlType.Player)
                {
                    shipStatsProfile.MaxHitPoints = 4000;
                    shipStatsProfile.MaxSailPoints = 2000;
                    shipStatsProfile.MaxGunPoints = 2000;
                }

                else
                {
                    shipStatsProfile.MaxHitPoints = 4000;
                    shipStatsProfile.MaxSailPoints = 2000;
                    shipStatsProfile.MaxGunPoints = 2000;
                }
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

            List<ShipUpgradeDetail> m_UpgradeDetails = new List<ShipUpgradeDetail>();

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
           
            m_UpgradeDetails.Add(ShipUpgrades.GetThemeDetail(m_ThemeUpgrade));
            m_UpgradeDetails.Add(ShipUpgrades.GetPaintDetail(m_PaintUpgrade));
            m_UpgradeDetails.Add(ShipUpgrades.GetCannonMetalDetail(m_CannonMetalUpgrade));

            m_UpgradeDetails.Add(ShipUpgrades.GetOutfittingDetail(m_OutfittingUpgrade));
            m_UpgradeDetails.Add(ShipUpgrades.GetBannerDetail(m_BannerUpgrade));
            m_UpgradeDetails.Add(ShipUpgrades.GetCharmDetail(m_CharmUpgrade));

            m_UpgradeDetails.Add(ShipUpgrades.GetMinorAbilityDetail(m_MinorAbilityUpgrade));
            m_UpgradeDetails.Add(ShipUpgrades.GetMajorAbilityDetail(m_MajorAbilityUpgrade));
            m_UpgradeDetails.Add(ShipUpgrades.GetEpicAbilityDetail(m_EpicAbilityUpgrade));

            foreach (ShipUpgradeDetail shipUpgradeDetail in m_UpgradeDetails)
            {
                if (shipUpgradeDetail == null)
                    continue;

                shipStatsProfile.HoldSizeUpgradeModifier += shipUpgradeDetail.HoldCapacity;

                shipStatsProfile.MaxHitPointsUpgradeModifier += shipUpgradeDetail.MaxHitPoints;
                shipStatsProfile.MaxSailPointsUpgradeModifier += shipUpgradeDetail.MaxSailPoints;
                shipStatsProfile.MaxGunPointsUpgradeModifier += shipUpgradeDetail.MaxGunPoints;

                shipStatsProfile.ForwardSpeedUpgradeModifier += shipUpgradeDetail.ForwardSpeed;
                shipStatsProfile.DriftSpeedUpgradeModifier += shipUpgradeDetail.DriftSpeed;
                shipStatsProfile.SlowdownModePenaltyUpgradeModifier += shipUpgradeDetail.SlowdownModePenalty;

                shipStatsProfile.CannonAccuracyUpgradeModifier += shipUpgradeDetail.CannonAccuracy;
                shipStatsProfile.CannonDamageUpgradeModifier += shipUpgradeDetail.CannonDamage;
                shipStatsProfile.CannonRangeUpgradeModifier += shipUpgradeDetail.CannonRange;
                shipStatsProfile.CannonReloadDurationUpgradeModifier += shipUpgradeDetail.CannonReloadTime;

                shipStatsProfile.MinorAbilityCooldownDurationUpgradeModifier += shipUpgradeDetail.MinorAbilityCooldown;
                shipStatsProfile.MajorAbilityCooldownDurationUpgradeModifier += shipUpgradeDetail.MajorAbilityCooldown;
                shipStatsProfile.EpicAbilityCooldownDurationUpgradeModifier += shipUpgradeDetail.EpicAbilityCooldown;

                shipStatsProfile.RepairCooldownDurationUpgradeModifier += shipUpgradeDetail.RepairCooldown;
                shipStatsProfile.BoardingChanceUpgradeModifier += shipUpgradeDetail.BoardingChance;

                shipStatsProfile.CannonEnemyCrewDamageUpgradeModifier += shipUpgradeDetail.CannonEnemyCrewDamage;
                shipStatsProfile.CannonHitPointsDamageUpgradeModifier += shipUpgradeDetail.CannonHitPointsDamage;
                shipStatsProfile.CannonSailPointsDamageUpgradeModifier += shipUpgradeDetail.CannonSailPointsDamage;
                shipStatsProfile.CannonGunPointsDamageUpgradeModifier += shipUpgradeDetail.CannonGunPointsDamage;

                shipStatsProfile.CrewDamageMeleeDamageDealtUpgradeModifier += shipUpgradeDetail.CrewDamageMeleeDamageDealt;
                shipStatsProfile.CrewDamageSpellDamageDealtUpgradeModifier += shipUpgradeDetail.CrewDamageSpellDamageDealt;

                shipStatsProfile.CrewDamageMeleeDamageReceivedUpgradeModifier += shipUpgradeDetail.CrewDamageMeleeDamageReceived;
                shipStatsProfile.CrewDamageSpellDamageReceivedUpgradeModifier += shipUpgradeDetail.CrewDamageSpellDamageReceived;
                shipStatsProfile.CrewDamageCannonDamageReceivedUpgradeModifier += shipUpgradeDetail.CrewDamageCannonDamageReceived;

                shipStatsProfile.RepairHitPointsBonusUpgradeModifier += shipUpgradeDetail.RepairHitPointsBonus;
                shipStatsProfile.RepairSailPointsBonusUpgradeModifier += shipUpgradeDetail.RepairSailPointsBonus;
                shipStatsProfile.RepairGunPointsBonusUpgradeModifier += shipUpgradeDetail.RepairGunPointsBonus;
                shipStatsProfile.RepairMaterialsReductionUpgradeModifier += shipUpgradeDetail.RepairMaterialsReduction;

                shipStatsProfile.MinorAbilityEffectivenessUpgradeModifier += shipUpgradeDetail.MinorAbilityEffectiveness;
                shipStatsProfile.MajorAbilityEffectivenessUpgradeModifier += shipUpgradeDetail.MajorAbilityEffectiveness;
                shipStatsProfile.EpicAbilityEffectivenessUpgradeModifier += shipUpgradeDetail.EpicAbilityEffectiveness;

                shipStatsProfile.CrewHealingReceivedUpgradeModifier += shipUpgradeDetail.CrewHealingReceived;

                shipStatsProfile.DoubloonsEarnedFromEnemyCrewUpgradeModifier += shipUpgradeDetail.DoubloonsEarnedFromEnemyCrew;
                shipStatsProfile.DoubloonsEarnedFromEnemyShipsUpgradeModifier += shipUpgradeDetail.DoubloonsEarnedFromEnemyShips;

                shipStatsProfile.FishingSuccessUpgradeModifier += shipUpgradeDetail.FishingSuccess;

                shipStatsProfile.SpyglassAccuracyUpgradeModifier += shipUpgradeDetail.SpyglassAccuracy;
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

        public double CannonEnemyCrewDamageUpgradeModifier = 0.0;
        public double CannonHitPointsDamageUpgradeModifier = 0.0;
        public double CannonSailPointsDamageUpgradeModifier = 0.0;
        public double CannonGunPointsDamageUpgradeModifier = 0.0;

        public double CrewDamageMeleeDamageDealtUpgradeModifier = 0.0;
        public double CrewDamageSpellDamageDealtUpgradeModifier = 0.0;

        public double CrewDamageMeleeDamageReceivedUpgradeModifier = 0.0;
        public double CrewDamageSpellDamageReceivedUpgradeModifier = 0.0;
        public double CrewDamageCannonDamageReceivedUpgradeModifier = 0.0;

        public double RepairHitPointsBonusUpgradeModifier = 0.0;
        public double RepairSailPointsBonusUpgradeModifier = 0.0;
        public double RepairGunPointsBonusUpgradeModifier = 0.0;
        public double RepairMaterialsReductionUpgradeModifier = 0.0;

        public double MinorAbilityEffectivenessUpgradeModifier = 0.0;
        public double MajorAbilityEffectivenessUpgradeModifier = 0.0;       
        public double EpicAbilityEffectivenessUpgradeModifier = 0.0;

        public double CrewHealingReceivedUpgradeModifier = 0.0;

        public double DoubloonsEarnedFromEnemyCrewUpgradeModifier = 0.0;
        public double DoubloonsEarnedFromEnemyShipsUpgradeModifier = 0.0;

        public double FishingSuccessUpgradeModifier = 0.0;

        public double SpyglassAccuracyUpgradeModifier = 0.0;

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