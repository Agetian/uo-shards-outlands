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
    public static class ShipCrew
    {
        public static void GenerateShipCrew(BaseShip ship)
        {
            if (ship == null) return;
            if (ship.Deleted) return;

            int shipLevel = 1;

            if (ship is MediumShip || ship is MediumDragonShip) shipLevel = 2;
            if (ship is LargeShip || ship is LargeDragonShip) shipLevel = 3;
            if (ship is Carrack) shipLevel = 4;
            if (ship is Galleon) shipLevel = 5;

            BaseCreature bc_Creature;
            Point3D creatureLocation;

            int crewNumber = 0;
            int ratChances = 0;

            Dictionary<Type, int> DictCrewOptions = new Dictionary<Type, int>();

            /*
            switch (ship.MobileFactionType)
            {
                case MobileFactionType.Fishing:
                    #region Fishing
                    crewNumber = 3 + shipLevel;

                    for (int a = 0; a < crewNumber; a++)
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.OceanFisherman();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);
                    }
                    #endregion
                break;

                case MobileFactionType.Pirate:
                    #region Pirate
                    crewNumber = 3 + shipLevel;
                    
                    for (int a = 0; a < crewNumber; a++)
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.OceanPirate();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);
                    }

                    if (Utility.RandomDouble() <= (.2 * shipLevel) )
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.PirateShipCarpenter();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);
                    }

                    if (Utility.RandomDouble() <= (.2 * shipLevel))
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.PirateSawbones();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);
                    }

                    creatureLocation = ship.GetRandomEmbarkLocation(true);

                    bc_Creature = new Custom.Pirates.OceanPirateCaptain(ship);
                    bc_Creature.BardImmune = true;
                    bc_Creature.MoveToWorld(creatureLocation, ship.Map);  

                    ship.Crew.Add(bc_Creature);
                    ship.AddEmbarkedMobile(bc_Creature);
                    
                    #endregion
                break;

                case MobileFactionType.Britain:
                    #region Britain
                    DictCrewOptions = new Dictionary<Type, int>();

                    DictCrewOptions.Add(typeof(Custom.Pirates.BritainSailor), 2);
                    DictCrewOptions.Add(typeof(Custom.Pirates.BritainMarine), 1);

                    crewNumber = 3 + shipLevel;

                    for (int a = 0; a < crewNumber; a++)
                    {
                        int TotalValues = 0;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            TotalValues += pair.Value;
                        }

                        double ActionCheck = Utility.RandomDouble();
                        double CumulativeAmount = 0.0;
                        double AdditionalAmount = 0.0;

                        bool foundDirection = true;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            AdditionalAmount = (double)pair.Value / (double)TotalValues;

                            if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                            {
                                bc_Creature = (BaseCreature)Activator.CreateInstance(pair.Key);

                                if (bc_Creature == null)
                                    continue;

                                creatureLocation = ship.GetRandomEmbarkLocation(true);

                                bc_Creature.BardImmune = true;
                                bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                                ship.Crew.Add(bc_Creature);
                                ship.AddEmbarkedMobile(bc_Creature);

                                break;
                            }

                            CumulativeAmount += AdditionalAmount;
                        }
                    }      
             
                    if (Utility.RandomDouble() <= (.2 * shipLevel ) )
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.BritainShipCarpenter();                        
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);

                    }

                    if (Utility.RandomDouble() <= (.2 * shipLevel))
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.BritainShipSurgeon();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);

                    }

                    creatureLocation = ship.GetRandomEmbarkLocation(true);

                    bc_Creature = new Custom.Pirates.BritainShipCaptain(ship);
                    bc_Creature.BardImmune = true;
                    bc_Creature.MoveToWorld(creatureLocation, ship.Map); 

                    ship.Crew.Add(bc_Creature);
                    ship.AddEmbarkedMobile(bc_Creature);
                    #endregion
                break;                

                case MobileFactionType.Orc:
                    #region Orc
                    
                    crewNumber = 3 + shipLevel;

                    DictCrewOptions = new Dictionary<Type, int>();

                    DictCrewOptions.Add(typeof(OrcishGrunt), 1);
                    DictCrewOptions.Add(typeof(OrcishScout), 1);
                    DictCrewOptions.Add(typeof(OrcMojoka), 1);

                    for (int a = 0; a < crewNumber; a++)
                    {
                        int TotalValues = 0;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            TotalValues += pair.Value;
                        }

                        double ActionCheck = Utility.RandomDouble();
                        double CumulativeAmount = 0.0;
                        double AdditionalAmount = 0.0;

                        bool foundDirection = true;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            AdditionalAmount = (double)pair.Value / (double)TotalValues;

                            if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                            {
                                bc_Creature = (BaseCreature)Activator.CreateInstance(pair.Key);

                                if (bc_Creature == null)
                                    continue;

                                if (bc_Creature is OrcishGrunt)                                                                    
                                    bc_Creature.PackItem(new Bow() { Movable = false, Hue = 0 });

                                creatureLocation = ship.GetRandomEmbarkLocation(true);

                                bc_Creature.BardImmune = true;
                                bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                                ship.Crew.Add(bc_Creature);
                                ship.AddEmbarkedMobile(bc_Creature);


                                break;
                            }

                            CumulativeAmount += AdditionalAmount;
                        }
                    }

                    crewNumber = (int)(Math.Ceiling((double)shipLevel / 2));

                    DictCrewOptions = new Dictionary<Type, int>();

                    DictCrewOptions.Add(typeof(OrcishExecutioner), 1);
                    DictCrewOptions.Add(typeof(OrcishSurjin), 1);
                    DictCrewOptions.Add(typeof(OrcishMaurk), 1);
                    DictCrewOptions.Add(typeof(ElderMojoka), 1);  

                    for (int a = 0; a < crewNumber; a++)
                    {
                        int TotalValues = 0;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            TotalValues += pair.Value;
                        }

                        double ActionCheck = Utility.RandomDouble();
                        double CumulativeAmount = 0.0;
                        double AdditionalAmount = 0.0;

                        bool foundDirection = true;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            AdditionalAmount = (double)pair.Value / (double)TotalValues;

                            if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                            {
                                bc_Creature = (BaseCreature)Activator.CreateInstance(pair.Key);

                                if (bc_Creature == null)
                                    continue;

                                if (bc_Creature is OrcishExecutioner || bc_Creature is OrcishSurjin)
                                    bc_Creature.PackItem(new Bow() { Movable = false, Hue = 0 });                                

                                creatureLocation = ship.GetRandomEmbarkLocation(true);

                                bc_Creature.BardImmune = true;
                                bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                                ship.Crew.Add(bc_Creature);
                                ship.AddEmbarkedMobile(bc_Creature);

                                break;
                            }

                            CumulativeAmount += AdditionalAmount;
                        }
                    }

                    #endregion
                break;

                case MobileFactionType.Orghereim:
                    #region Orghereim
                    crewNumber = 3 + shipLevel;

                    DictCrewOptions = new Dictionary<Type, int>();

                    DictCrewOptions.Add(typeof(OrghereimBowMaiden), 1);
                    DictCrewOptions.Add(typeof(OrghereimTracker), 1);
                    DictCrewOptions.Add(typeof(OrghereimSwordThane), 1);
                    DictCrewOptions.Add(typeof(OrghereimIceCarl), 1); 
                    DictCrewOptions.Add(typeof(OrghereimShieldMaiden), 1);

                    for (int a = 0; a < crewNumber; a++)
                    {
                        int TotalValues = 0;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            TotalValues += pair.Value;
                        }

                        double ActionCheck = Utility.RandomDouble();
                        double CumulativeAmount = 0.0;
                        double AdditionalAmount = 0.0;

                        bool foundDirection = true;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            AdditionalAmount = (double)pair.Value / (double)TotalValues;

                            if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                            {
                                bc_Creature = (BaseCreature)Activator.CreateInstance(pair.Key);

                                if (bc_Creature == null)
                                    continue;

                                if (bc_Creature is OrghereimSwordThane || bc_Creature is OrghereimShieldMaiden)
                                    bc_Creature.PackItem(new Bow() { Movable = false, Hue = 0 });                                

                                creatureLocation = ship.GetRandomEmbarkLocation(true);

                                bc_Creature.BardImmune = true;
                                bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                                ship.Crew.Add(bc_Creature);
                                ship.AddEmbarkedMobile(bc_Creature);

                                break;
                            }

                            CumulativeAmount += AdditionalAmount;
                        }
                    }

                    crewNumber = (int)(Math.Ceiling((double)shipLevel / 2));

                    DictCrewOptions = new Dictionary<Type, int>();

                    DictCrewOptions.Add(typeof(OrghereimBoneMender), 1);
                    DictCrewOptions.Add(typeof(OrghereimCrone), 1);
                    DictCrewOptions.Add(typeof(OrghereimSage), 1);
                    DictCrewOptions.Add(typeof(OrghereimShieldMother), 1); 

                    for (int a = 0; a < crewNumber; a++)
                    {
                        int TotalValues = 0;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            TotalValues += pair.Value;
                        }

                        double ActionCheck = Utility.RandomDouble();
                        double CumulativeAmount = 0.0;
                        double AdditionalAmount = 0.0;

                        bool foundDirection = true;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            AdditionalAmount = (double)pair.Value / (double)TotalValues;

                            if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                            {
                                bc_Creature = (BaseCreature)Activator.CreateInstance(pair.Key);

                                if (bc_Creature == null)
                                    continue;

                                if (bc_Creature is OrghereimShieldMother)
                                    bc_Creature.PackItem(new Bow() { Movable = false, Hue = 0 });                                

                                creatureLocation = ship.GetRandomEmbarkLocation(true);

                                bc_Creature.BardImmune = true;
                                bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                                ship.Crew.Add(bc_Creature);
                                ship.AddEmbarkedMobile(bc_Creature);

                                break;
                            }

                            CumulativeAmount += AdditionalAmount;
                        }
                    }
                    #endregion
                break;

                case MobileFactionType.Undead:
                    #region Undead
                    crewNumber = 3 + shipLevel;
                    
                    for (int a = 0; a < crewNumber; a++)
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.SkeletalCrewman();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);
                    }

                    crewNumber = 3 + shipLevel;

                    DictCrewOptions = new Dictionary<Type, int>();

                    DictCrewOptions.Add(typeof(Skeleton), 15);
                    DictCrewOptions.Add(typeof(ZombieMagi), 5);
                    DictCrewOptions.Add(typeof(SkeletalMage), 5);
                    DictCrewOptions.Add(typeof(Lich), 3);                 
                    DictCrewOptions.Add(typeof(SkeletalDrake), 2);
                    DictCrewOptions.Add(typeof(LichLord), 1);

                    for (int a = 0; a < crewNumber; a++)
                    {
                        int TotalValues = 0;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            TotalValues += pair.Value;
                        }

                        double ActionCheck = Utility.RandomDouble();
                        double CumulativeAmount = 0.0;
                        double AdditionalAmount = 0.0;

                        bool foundDirection = true;

                        foreach (KeyValuePair<Type, int> pair in DictCrewOptions)
                        {
                            AdditionalAmount = (double)pair.Value / (double)TotalValues;

                            if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                            {
                                bc_Creature = (BaseCreature)Activator.CreateInstance(pair.Key);

                                if (bc_Creature == null)
                                    continue;

                                creatureLocation = ship.GetRandomEmbarkLocation(true);

                                bc_Creature.BardImmune = true;
                                bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                                ship.Crew.Add(bc_Creature);
                                ship.AddEmbarkedMobile(bc_Creature);

                                break;
                            }

                            CumulativeAmount += AdditionalAmount;
                        }
                    }

                    int necromancers = (int)(Math.Ceiling((double)shipLevel / 2));
                    for (int a = 0; a < necromancers; a++)
                    {
                        creatureLocation = ship.GetRandomEmbarkLocation(true);

                        bc_Creature = new Custom.Pirates.GhostShipNecromancer();
                        bc_Creature.BardImmune = true;
                        bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                        ship.Crew.Add(bc_Creature);
                        ship.AddEmbarkedMobile(bc_Creature);
                    }

                    creatureLocation = ship.GetRandomEmbarkLocation(true);

                    bc_Creature = new Custom.Pirates.SkeletalCaptain(ship);
                    bc_Creature.BardImmune = true;
                    bc_Creature.MoveToWorld(creatureLocation, ship.Map);

                    ship.Crew.Add(bc_Creature);
                    ship.AddEmbarkedMobile(bc_Creature);

                    #endregion
                break;

             
            }
               */
        }
    }
}
