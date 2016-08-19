using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
    public class ShipSpawner : Item
    {
        private bool m_Activated = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated
        {
            get { return m_Activated; }
            set { m_Activated = value; }
        }

        private string m_ShipTypes = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipTypes
        {
            get { return m_ShipTypes; }
            set { m_ShipTypes = value; }
        }

        private int m_ShipCount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShipCount
        {
            get { return m_ShipCount; }
            set { m_ShipCount = value; }
        }

        private bool m_SpawnAllAvailable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SpawnAllAvailable
        {
            get { return m_SpawnAllAvailable; }
            set { m_SpawnAllAvailable = value; }
        }

        private int m_HomeRange = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeRange
        {
            get { return m_HomeRange; }
            set { m_HomeRange = value; }
        }

        private int m_SpawnRange = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get { return m_SpawnRange; }
            set { m_SpawnRange = value; }
        }

        private int m_MinSpawnTime = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinSpawnTime
        {
            get { return m_MinSpawnTime; }
            set { m_MinSpawnTime = value; }
        }

        private int m_MaxSpawnTime = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSpawnTime
        {
            get { return m_MaxSpawnTime; }
            set { m_MaxSpawnTime = value; }
        }

        private Direction m_PreferredDirection = Direction.North;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction PreferredDirection
        {
            get { return m_PreferredDirection; }
            set { m_PreferredDirection = value; }
        }

        public List<BaseShip> m_Ships = new List<BaseShip>();
        public DateTime m_LastActivity = DateTime.UtcNow;
        public TimeSpan m_NextActivity;

        private Timer m_SpawnTimer;

        [Constructable]
        public ShipSpawner(): base(0x14F4)
        {
            Name = "Ship Spawner";

            Visible = false;
            Movable = false;

            m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_MinSpawnTime, m_MaxSpawnTime));

            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();
        }

        public ShipSpawner(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Ships Active: " + m_Ships.Count.ToString() + " / " + m_ShipCount.ToString());
            LabelTo(from, "[Double Click to Delete All Ships]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            int shipCount = m_Ships.Count;

            for (int a = 0; a < shipCount; a++)
            {
                if (m_Ships[0] != null)
                    m_Ships[0].Delete();
            }
        }

        public void StartSpawn(Mobile from)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //version

            //Version 0
            writer.Write(m_Activated);
            writer.Write(m_ShipTypes);
            writer.Write(m_ShipCount);
            writer.Write(m_SpawnAllAvailable);
            writer.Write(m_HomeRange);
            writer.Write(m_SpawnRange);
            writer.Write(m_MinSpawnTime);
            writer.Write(m_MaxSpawnTime);

            writer.Write(m_Ships.Count);

            foreach (BaseShip ship in m_Ships)
            {
                writer.Write(ship);
            }

            writer.Write(m_LastActivity);
            writer.Write(m_NextActivity);

            //Version 1
            writer.Write((int)PreferredDirection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_Ships = new List<BaseShip>();

            m_Activated = reader.ReadBool();
            m_ShipTypes = reader.ReadString();
            m_ShipCount = reader.ReadInt();
            m_SpawnAllAvailable = reader.ReadBool();
            m_HomeRange = reader.ReadInt();
            m_SpawnRange = reader.ReadInt();
            m_MinSpawnTime = reader.ReadInt();
            m_MaxSpawnTime = reader.ReadInt();

            int shipCount = reader.ReadInt();
            for (int a = 0; a < shipCount; a++)
            {
                BaseShip ship = (BaseShip)reader.ReadItem();
                m_Ships.Add(ship);
            }

            m_LastActivity = reader.ReadDateTime();
            m_NextActivity = reader.ReadTimeSpan();

            if (m_SpawnTimer == null)
            {
                m_SpawnTimer = new SpawnTimer(this);
                m_SpawnTimer.Start();
            }

            else if (!m_SpawnTimer.Running)
                m_SpawnTimer.Start();

            //Version 1
            if (version >= 1)
            {
                m_PreferredDirection = (Direction)reader.ReadInt();
            }
        }

        public static BaseShip RandomizeShip(Dictionary<Type, int> shipChances)
        {
            BaseShip ship = null;

            int TotalValues = 0;

            foreach (KeyValuePair<Type, int> pair in shipChances)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            foreach (KeyValuePair<Type, int> pair in shipChances)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    ship = (BaseShip)Activator.CreateInstance(pair.Key);
                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }  

            return ship;
        }

        public static BaseShip GetRandomShipType(string shipString)
        {
            BaseShip ship = null;

            int randomShip;

            shipString = shipString.Trim();
            shipString = shipString.ToLower();

            string[] shipTypes = shipString.Split(',');

            string newShip = shipTypes[Utility.RandomMinMax(0, shipTypes.Length - 1)];

            Dictionary<Type, int> DictShipOptions = new Dictionary<Type, int>();

            /*
            switch (newShip)
            {
                case "smallbritainnavyship": ship = new SmallBritainNavyShip(); break;
                case "mediumbritainnavyship": ship = new MediumBritainNavyShip(); break;
                case "largebritainnavyship": ship = new LargeBritainNavyShip(); break;
                case "carrackbritainnavyship": ship = new CarrackBritainNavyShip(); break;
                case "galleonbritainnavyship": ship = new GalleonBritainNavyShip(); break;

                case "smallfishingship": ship = new SmallFishingShip(); break;
                case "mediumfishingship": ship = new MediumFishingShip(); break;
                case "largefishingship": ship = new LargeFishingShip(); break;
                case "carrackfishingship": ship = new FishingCarrack(); break;
                case "galleonfishingship": ship = new GalleonFishingShip(); break;

                case "smallpirateship": ship = new SmallPirateShip(); break;
                case "mediumpirateship": ship = new MediumPirateShip(); break;
                case "largepirateship": ship = new LargePirateShip(); break;
                case "carrackpirateship": ship = new CarrackPirateShip(); break;
                case "galleonpirateship": ship = new GalleonPirateShip(); break;

                case "smallundeadship": ship = new SmallUndeadShip(); break;
                case "mediumundeadship": ship = new MediumUndeadShip(); break;
                case "largeundeadship": ship = new LargeUndeadShip(); break;
                case "carrackundeadship": ship = new CarrackUndeadShip(); break;
                case "galleonundeadship": ship = new GalleonUndeadShip(); break;

                case "smallorghereimship": ship = new SmallOrghereimShip(); break;
                case "mediumorghereimship": ship = new MediumOrghereimShip(); break;
                case "largeorghereimship": ship = new LargeOrghereimShip(); break;
                case "carrackorghereimship": ship = new CarrackOrghereimShip(); break;
                case "galleonorghereimship": ship = new GalleonOrghereimShip(); break;

                case "smallorcship": ship = new SmallOrcShip(); break;
                case "mediumorcship": ship = new MediumOrcShip(); break;
                case "largeorcship": ship = new LargeOrcShip(); break;
                case "carrackorcship": ship = new CarrackOrcShip(); break;
                case "galleonorcship": ship = new GalleonOrcShip(); break;

                case "anybritainship":

                    DictShipOptions.Add(typeof(SmallBritainNavyShip), 5);
                    DictShipOptions.Add(typeof(MediumBritainNavyShip), 4);
                    DictShipOptions.Add(typeof(LargeBritainNavyShip), 3);
                    DictShipOptions.Add(typeof(CarrackBritainNavyShip), 2);
                    DictShipOptions.Add(typeof(GalleonBritainNavyShip), 1);

                    ship = RandomizeShip(DictShipOptions);
                break;

                case "anyfishingship":

                    DictShipOptions.Add(typeof(SmallFishingShip), 5);
                    DictShipOptions.Add(typeof(MediumFishingShip), 4);
                    DictShipOptions.Add(typeof(LargeFishingShip), 3);
                    DictShipOptions.Add(typeof(FishingCarrack), 2);
                    DictShipOptions.Add(typeof(GalleonFishingShip), 1);

                    ship = RandomizeShip(DictShipOptions);
                break;

                case "anypirateship":

                    DictShipOptions.Add(typeof(SmallPirateShip), 5);
                    DictShipOptions.Add(typeof(MediumPirateShip), 4);
                    DictShipOptions.Add(typeof(LargePirateShip), 3);
                    DictShipOptions.Add(typeof(CarrackPirateShip), 2);
                    DictShipOptions.Add(typeof(GalleonPirateShip), 1);

                    ship = RandomizeShip(DictShipOptions);
                break;

                case "anyundeadship":

                    DictShipOptions.Add(typeof(SmallUndeadShip), 5);
                    DictShipOptions.Add(typeof(MediumUndeadShip), 4);
                    DictShipOptions.Add(typeof(LargeUndeadShip), 3);
                    DictShipOptions.Add(typeof(CarrackUndeadShip), 2);
                    DictShipOptions.Add(typeof(GalleonUndeadShip), 1);

                    ship = RandomizeShip(DictShipOptions);
                break;

                case "anyorghereimship":

                    DictShipOptions.Add(typeof(SmallOrghereimShip), 5);
                    DictShipOptions.Add(typeof(MediumOrghereimShip), 4);
                    DictShipOptions.Add(typeof(LargeOrghereimShip), 3);
                    DictShipOptions.Add(typeof(CarrackOrghereimShip), 2);
                    DictShipOptions.Add(typeof(GalleonOrghereimShip), 1);

                    ship = RandomizeShip(DictShipOptions);
                break;

                case "anyorcship":

                    DictShipOptions.Add(typeof(SmallOrcShip), 5);
                    DictShipOptions.Add(typeof(MediumOrcShip), 4);
                    DictShipOptions.Add(typeof(LargeOrcShip), 3);
                    DictShipOptions.Add(typeof(CarrackOrcShip), 2);
                    DictShipOptions.Add(typeof(GalleonOrcShip), 1);

                    ship = RandomizeShip(DictShipOptions);
                break;

                case "anyship":

                    DictShipOptions.Add(typeof(SmallBritainNavyShip), 5);
                    DictShipOptions.Add(typeof(MediumBritainNavyShip), 4);
                    DictShipOptions.Add(typeof(LargeBritainNavyShip), 3);
                    DictShipOptions.Add(typeof(CarrackBritainNavyShip), 2);
                    DictShipOptions.Add(typeof(GalleonBritainNavyShip), 1);

                    DictShipOptions.Add(typeof(SmallFishingShip), 5);
                    DictShipOptions.Add(typeof(MediumFishingShip), 4);
                    DictShipOptions.Add(typeof(LargeFishingShip), 3);
                    DictShipOptions.Add(typeof(FishingCarrack), 2);
                    DictShipOptions.Add(typeof(GalleonFishingShip), 1);

                    DictShipOptions.Add(typeof(SmallPirateShip), 5);
                    DictShipOptions.Add(typeof(MediumPirateShip), 4);
                    DictShipOptions.Add(typeof(LargePirateShip), 3);
                    DictShipOptions.Add(typeof(CarrackPirateShip), 2);
                    DictShipOptions.Add(typeof(GalleonPirateShip), 1);

                    DictShipOptions.Add(typeof(SmallUndeadShip), 5);
                    DictShipOptions.Add(typeof(MediumUndeadShip), 4);
                    DictShipOptions.Add(typeof(LargeUndeadShip), 3);
                    DictShipOptions.Add(typeof(CarrackUndeadShip), 2);
                    DictShipOptions.Add(typeof(GalleonUndeadShip), 1);

                    DictShipOptions.Add(typeof(SmallOrghereimShip), 5);
                    DictShipOptions.Add(typeof(MediumOrghereimShip), 4);
                    DictShipOptions.Add(typeof(LargeOrghereimShip), 3);
                    DictShipOptions.Add(typeof(CarrackOrghereimShip), 2);
                    DictShipOptions.Add(typeof(GalleonOrghereimShip), 1);

                    DictShipOptions.Add(typeof(SmallOrcShip), 5);
                    DictShipOptions.Add(typeof(MediumOrcShip), 4);
                    DictShipOptions.Add(typeof(LargeOrcShip), 3);
                    DictShipOptions.Add(typeof(CarrackOrcShip), 2);
                    DictShipOptions.Add(typeof(GalleonOrcShip), 1);

                    ship = RandomizeShip(DictShipOptions);

                break;
            }
            */

            return ship;
        }

        public override void OnAfterDelete()
        {           
            int shipCount = m_Ships.Count;

            for (int a = 0; a < shipCount; a++)
            {
                m_Ships[0].Delete();
            }

            base.OnAfterDelete();            
        }

        public void ShipSunk(BaseShip ship)
        {
            m_Ships.Remove(ship);

            foreach (BaseShip checkship in BaseShip.m_Instances)
            {
                if (checkship != null)
                {
                    if (checkship.ShipCombatant != null)
                    {
                        if (checkship.ShipCombatant == ship)
                            checkship.ShipCombatant = null;
                    }
                }
            }

            m_LastActivity = DateTime.UtcNow;
            m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_MinSpawnTime, m_MaxSpawnTime));
        }

        private class SpawnTimer : Timer
        {
            public ShipSpawner m_ShipSpawner;

            public SpawnTimer(ShipSpawner shipSpawner): base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                m_ShipSpawner = shipSpawner;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (!m_ShipSpawner.Activated)
                    return;

                DateTime nextSpawnTime = m_ShipSpawner.m_LastActivity + m_ShipSpawner.m_NextActivity;

                if (nextSpawnTime < DateTime.UtcNow)
                {
                    if (m_ShipSpawner.m_Ships.Count < m_ShipSpawner.ShipCount)
                    {
                        int shipsNeeded = m_ShipSpawner.ShipCount - m_ShipSpawner.m_Ships.Count;
                        int shipsSpawned = 0;

                        if (!m_ShipSpawner.SpawnAllAvailable)
                            shipsNeeded = 1;

                        m_ShipSpawner.m_LastActivity = DateTime.UtcNow;
                        m_ShipSpawner.m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_ShipSpawner.m_MinSpawnTime, m_ShipSpawner.m_MaxSpawnTime));

                        //Ships Needed
                        for (int a = 0; a < shipsNeeded; a++)
                        {
                            BaseShip ship = GetRandomShipType(m_ShipSpawner.m_ShipTypes);

                            if (ship == null)
                                continue;

                            bool shipSpawned = false;

                            //Make 50 Attempts to Find Randomized Location for Ship Spawn Point Before Aborting
                            for (int b = 0; b < 50; b++)
                            {
                                if (shipSpawned)                                
                                    break;                                

                                Point3D newLocation = new Point3D();

                                int x = m_ShipSpawner.X;

                                int xOffset = Utility.RandomMinMax(0, m_ShipSpawner.SpawnRange);
                                if (Utility.RandomDouble() >= .5)
                                    xOffset *= -1;

                                x += xOffset;

                                int y = m_ShipSpawner.Y;

                                int yOffset = Utility.RandomMinMax(0, m_ShipSpawner.SpawnRange);
                                if (Utility.RandomDouble() >= .5)
                                    yOffset *= -1;

                                y += yOffset;

                                newLocation.X = x;
                                newLocation.Y = y;
                                newLocation.Z = m_ShipSpawner.Z;

                                Direction newDirection = Direction.North;
                                int shipFacingItemID = -1;                                

                                //Try Preferred Direction first
                                switch (m_ShipSpawner.PreferredDirection)
                                {
                                    case Direction.North:
                                        newDirection = Direction.North;
                                        shipFacingItemID = ship.NorthID;
                                    break;

                                    case Direction.Up:
                                        newDirection = Direction.North;
                                        shipFacingItemID = ship.NorthID;
                                    break;

                                    case Direction.East:
                                        newDirection = Direction.East;
                                        shipFacingItemID = ship.EastID;
                                    break;

                                    case Direction.Right:
                                        newDirection = Direction.East;
                                        shipFacingItemID = ship.EastID;
                                    break;

                                    case Direction.South:
                                        newDirection = Direction.South;
                                        shipFacingItemID = ship.SouthID;
                                    break;

                                    case Direction.Down:
                                        newDirection = Direction.South;
                                        shipFacingItemID = ship.SouthID;
                                    break;

                                    case Direction.West:
                                        newDirection = Direction.West;
                                        shipFacingItemID = ship.WestID;
                                    break;

                                    case Direction.Left:
                                        newDirection = Direction.West;
                                        shipFacingItemID = ship.WestID;
                                    break;

                                    default:
                                        newDirection = Direction.North;
                                        shipFacingItemID = ship.NorthID;
                                    break; 
                                }

                                if (ship.CanFit(newLocation, m_ShipSpawner.Map, shipFacingItemID))
                                {
                                    m_ShipSpawner.m_LastActivity = DateTime.UtcNow;

                                    ship.MoveToWorld(newLocation, m_ShipSpawner.Map);
                                    m_ShipSpawner.m_Ships.Add(ship);
                                    ship.m_ShipSpawner = m_ShipSpawner;

                                    Timer.DelayCall(TimeSpan.FromSeconds(.200), delegate
                                    {
                                        if (ship != null)
                                        {
                                            if (!ship.Deleted && ship.CanFit(newLocation, m_ShipSpawner.Map, shipFacingItemID))                                            
                                                ship.SetFacing(newDirection);                                            
                                        }
                                    });

                                    shipSpawned = true;
                                }

                                //Try Random Direction
                                else
                                {
                                    int randomDirection = Utility.RandomList(1, 2, 3, 4);

                                    switch (randomDirection)
                                    {
                                        case 1:
                                            newDirection = Direction.North;
                                            shipFacingItemID = ship.NorthID;
                                            break;

                                        case 2:
                                            newDirection = Direction.East;
                                            shipFacingItemID = ship.EastID;
                                            break;

                                        case 3:
                                            newDirection = Direction.South;
                                            shipFacingItemID = ship.SouthID;
                                            break;

                                        case 4:
                                            newDirection = Direction.West;
                                            shipFacingItemID = ship.WestID;
                                            break;
                                    }

                                    if (ship.CanFit(newLocation, m_ShipSpawner.Map, shipFacingItemID))
                                    {
                                        m_ShipSpawner.m_LastActivity = DateTime.UtcNow;

                                        ship.MoveToWorld(newLocation, m_ShipSpawner.Map);
                                        m_ShipSpawner.m_Ships.Add(ship);
                                        ship.m_ShipSpawner = m_ShipSpawner;

                                        Timer.DelayCall(TimeSpan.FromSeconds(.200), delegate
                                        {
                                            ship.SetFacing(newDirection);
                                        });

                                        shipSpawned = true;
                                    }
                                }
                            }

                            if (!shipSpawned)
                                ship.Delete();
                        }
                    }
                }
            }
        }
    }
}