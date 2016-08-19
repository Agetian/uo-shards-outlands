using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server.Items
{
    public static class ShipUniqueness
    {
        public static void GenerateShipUniqueness(BaseShip ship)
        {
            if (ship == null) return;
            if (ship.Deleted) return;

            string factionName = "";

            int shipLevel = 1;

            int BaseMaxHitPoints = 1000;
            int BaseMaxSailPoints = 500;
            int BaseMaxGunPoints = 500;

            double BaseFastInterval = 0.2;
            double BaseFastDriftInterval = 0.4;
            double BaseSlowInterval = 0.4;
            double BaseSlowDriftInterval = 0.8;

            double CannonAccuracyModifer = -1;
            double CannonRangeScalar = -1;
            double CannonDamageScalar = -1;
            double CannonReloadTimeScalar = -1;
            double DamageFromPlayerShipScalar = -1;

            if (ship is SmallShip || ship is SmallDragonShip)
            {
                shipLevel = 1;

                ship.BaseDoubloonValue = 15;              

                BaseMaxHitPoints = 1000;
                BaseMaxSailPoints = 500;
                BaseMaxGunPoints = 500;

                BaseFastInterval = 0.2;
                BaseFastDriftInterval = 0.4;
                BaseSlowInterval = 0.4;
                BaseSlowDriftInterval = 0.8;                             
            }

            if (ship is MediumShip || ship is MediumDragonShip)
            {
                shipLevel = 2;

                ship.BaseDoubloonValue = 25;

                BaseMaxHitPoints = 1250;
                BaseMaxSailPoints = 625;
                BaseMaxGunPoints = 625;

                BaseFastInterval = 0.225;
                BaseFastDriftInterval = 0.45;
                BaseSlowInterval = 0.45;
                BaseSlowDriftInterval = 0.9;                               
            }

            if (ship is LargeShip || ship is LargeDragonShip)
            {
                shipLevel = 3;

                ship.BaseDoubloonValue = 100;

                BaseMaxHitPoints = 1500;
                BaseMaxSailPoints = 750;
                BaseMaxGunPoints = 750;

                BaseFastInterval = 0.25;
                BaseFastDriftInterval = 0.5;
                BaseSlowInterval = 0.5;
                BaseSlowDriftInterval = 1.0;
            }

            if (ship is Carrack)
            {
                shipLevel = 4;

                ship.BaseDoubloonValue = 400;

                BaseMaxHitPoints = 2000;
                BaseMaxSailPoints = 1000;
                BaseMaxGunPoints = 1000;

                BaseFastInterval = 0.275;
                BaseFastDriftInterval = 0.55;
                BaseSlowInterval = 0.55;
                BaseSlowDriftInterval = 1.1;
            }

            if (ship is Galleon)
            {
                shipLevel = 5;
                
                ship.BaseDoubloonValue = 1000;

                BaseMaxHitPoints = 3000;
                BaseMaxSailPoints = 1500;
                BaseMaxGunPoints = 1500;

                BaseFastInterval = 0.3;
                BaseFastDriftInterval = 0.6;
                BaseSlowInterval = 0.6;
                BaseSlowDriftInterval = 1.2;
            }

            /*

            //NPC Ship Modifications
            if (ship is SmallFishingShip || ship is MediumFishingShip || ship is LargeFishingShip || ship is FishingCarrack || ship is GalleonFishingShip)
            {
                ship.MobileControlType = MobileControlType.Innocent;
                ship.MobileFactionType = MobileFactionType.Fishing;
                ship.Hue = 2076;

                factionName = "a fishing";

                DamageFromPlayerShipScalar = 2.0;                

                ship.DoubloonValue = 25 + (Utility.RandomMinMax(20 * (shipLevel - 1), 30 * (shipLevel - 1)));    
            }

            if (ship is SmallBritainNavyShip || ship is MediumBritainNavyShip || ship is LargeBritainNavyShip || ship is CarrackBritainNavyShip || ship is GalleonBritainNavyShip)
            {
                ship.MobileControlType = MobileControlType.Good;
                ship.MobileFactionType = MobileFactionType.Britain;
                ship.Hue = 1102;

                factionName = "a britain navy";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerShipScalar = 2.0;                

                ship.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (ship is SmallPirateShip || ship is MediumPirateShip || ship is LargePirateShip || ship is CarrackPirateShip || ship is GalleonPirateShip)
            {
                ship.MobileControlType = MobileControlType.Evil;
                ship.MobileFactionType = MobileFactionType.Pirate;
                ship.Hue = 1898;

                factionName = "a pirate";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerShipScalar = 2.0;

                ship.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (ship is SmallOrcShip || ship is MediumOrcShip || ship is LargeOrcShip || ship is CarrackOrcShip || ship is GalleonOrcShip)
            {
                ship.MobileControlType = MobileControlType.Evil;
                ship.MobileFactionType = MobileFactionType.Orc;
                ship.Hue = 1164;

                factionName = "an orc";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerShipScalar = 2.0;

                ship.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (ship is SmallOrghereimShip || ship is MediumOrghereimShip || ship is LargeOrghereimShip || ship is CarrackOrghereimShip || ship is GalleonOrghereimShip)
            {
                ship.MobileControlType = MobileControlType.Evil;
                ship.MobileFactionType = MobileFactionType.Orghereim;
                ship.Hue = 1154;

                factionName = "an orghereim";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerShipScalar = 2.0;

                ship.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (ship is SmallUndeadShip || ship is MediumUndeadShip || ship is LargeUndeadShip || ship is CarrackUndeadShip || ship is GalleonUndeadShip)
            {
                ship.MobileControlType = MobileControlType.Evil;
                ship.MobileFactionType = MobileFactionType.Undead;
                ship.Hue = 1072;
                ship.CannonHue = 1072;

                factionName = "an undead";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerShipScalar = 2.0;

                ship.DoubloonValue = 125 + (Utility.RandomMinMax(50 * (shipLevel - 1), 75 * (shipLevel - 1)));             
            }  
            */

            ship.BaseMaxHitPoints = BaseMaxHitPoints;
            ship.BaseMaxSailPoints = BaseMaxSailPoints;
            ship.BaseMaxGunPoints = BaseMaxGunPoints;

            ship.BaseFastInterval = BaseFastInterval;
            ship.BaseFastDriftInterval = BaseFastDriftInterval;
            ship.BaseSlowInterval = BaseSlowInterval;
            ship.BaseSlowDriftInterval = BaseSlowDriftInterval;

            if (CannonAccuracyModifer != -1)
                ship.CannonAccuracyModifer = CannonAccuracyModifer;

            if (CannonRangeScalar != -1)
                ship.CannonRangeScalar = CannonRangeScalar;

            if (CannonDamageScalar != -1)
                ship.CannonDamageScalar = CannonDamageScalar;

            if (CannonReloadTimeScalar != -1)
                ship.CannonReloadTimeScalar = CannonReloadTimeScalar;

            if (DamageFromPlayerShipScalar != -1)
                ship.DamageFromPlayerShipScalar = DamageFromPlayerShipScalar;
            
            if (ship.MobileControlType == MobileControlType.Player)
                return;

            ship.HitPoints = BaseMaxHitPoints;
            ship.SailPoints = BaseMaxSailPoints;
            ship.GunPoints = BaseMaxGunPoints;        

            string shipType = "";

            switch (shipLevel)
            {
                case 1: shipType = "small ship"; break;
                case 2: shipType = "medium ship"; break;
                case 3: shipType = "large ship"; break;
                case 4: shipType = "carrack"; break;
                case 5: shipType = "galleon"; break;
            }            

            ship.Name = factionName + " " + shipType;

            ship.TargetingMode = TargetingMode.Random;            
        }
    }
}
