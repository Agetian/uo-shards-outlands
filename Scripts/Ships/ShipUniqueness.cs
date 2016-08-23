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

            ShipStatsProfile shipStatsProfile = GetShipStatsProfile(ship.GetType());
            shipStatsProfile = ApplyUniqueness(shipStatsProfile);
            shipStatsProfile = ApplyUpgrades(shipStatsProfile);

            ApplyShipStats(shipStatsProfile, ship);
        }

        public static ShipStatsProfile GetShipStatsProfile(Type type)
        {
            ShipStatsProfile shipStatsProfile = new ShipStatsProfile();

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

            return shipStatsProfile;
        }

        public static ShipStatsProfile ApplyUniqueness(ShipStatsProfile shipStatsProfile)
        {
            if (shipStatsProfile == null)
                return null;

            //TEST: APPLY NPC STATS

            return shipStatsProfile;
        }

        public static ShipStatsProfile ApplyUpgrades(ShipStatsProfile shipStatsProfile)
        {
            if (shipStatsProfile == null) 
                return null;

            //TEST: APPLY UPGRADES TO PROFILE

            return shipStatsProfile;
        }

        public static void ApplyShipStats(ShipStatsProfile shipStatsProfile, BaseShip ship)
        {
            if (shipStatsProfile == null) return;
            if (ship == null) return;

            //TEST: APPLY STATS TO SHIP
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
        public string ShipName = "small ship";

        public int RegistrationDoubloonCost = 0;
        public double UpgradeDoubloonMultiplier = 1.0;

        public int DoubloonSinkValue = 0;

        public int HoldItemCount = 50;

        public int MaxHitPoints = 1000;
        public int MaxSailPoints = 500;
        public int MaxGunPoints = 500;

        public double FastInterval = 0.2;
        public double FastDriftInterval = 0.4;

        public double SlowdownModePenalty = 0.5;

        public int CannonsPerSide = 3;
        public double CannonAccuracyScalar = 1.0;
        public double CannonDamageScalar = 1.0;
        public double CannonRangeScalar = 1.0;
        public double CannonReloadTimeScalar = 1.0;
        
        public double MinorAbilityCooldownScalar = 1.0;
        public double MajorAbilityCooldownScalar = 1.0;
        public double EpicAbilityCooldownScalar = 1.0;

        public double BoardingChanceScalar = 1.0;
        public double RepairCooldownScalar = 1.0;

        public ShipStatsProfile()
        {
        }
    }
}