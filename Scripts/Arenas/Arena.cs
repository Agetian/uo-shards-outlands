using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    #region Arena Group Controller

    public class ArenaGroupController : Item
    {
        public List<ArenaController> m_Arenas
        {
            get 
            {
                List<ArenaController> arenaControllers = new List<ArenaController>();

                foreach(ArenaController arenaController in ArenaController.m_Instances)
                {
                    if (arenaController == null) continue;
                    if (arenaController.Deleted) continue;
                    if (arenaController.m_ArenaGroupController == this)
                        arenaControllers.Add(arenaController);
                }

                return arenaControllers;
            } 
        }

        [Constructable]
        public ArenaGroupController(): base(3804)
        {
            Movable = false;
        }

        public ArenaGroupController(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        
            //Version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    #endregion

    #region Arena Controller

    public class ArenaController : Item
    {
        public ArenaGroupController m_ArenaGroupController;
        public Rectangle2D m_Boundary = new Rectangle2D();

        public ArenaFight m_ArenaFight;     
   
        //-----

        public static List<ArenaController> m_Instances = new List<ArenaController>();

        [Constructable]
        public ArenaController(): base(4479)
        {
            Movable = false;

            //-----

            m_Instances.Add(this);
        }

        public ArenaController(Serial serial): base(serial)
        {
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public bool IsWithin(Point3D location)
        {
            Point2D sourcePoint = new Point2D(location.X, location.Y);

            if (m_Boundary.Contains(sourcePoint))
                return true;

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaGroupController);
            writer.Write(m_Boundary);
            writer.Write(m_ArenaFight);           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaGroupController = reader.ReadItem() as ArenaGroupController;
                m_Boundary = reader.ReadRect2D();
                m_ArenaFight = reader.ReadItem() as ArenaFight;               
            }

            //-----

            m_Instances.Add(this);
        }
    }

    #endregion

    #region Arena Tile

    public class ArenaTile : Item
    {
        public enum ArenaTileType
        {
            StartLocation,           
            WallLocation,
            ExitLocation
        }

        public ArenaTileType m_TileType = ArenaTileType.StartLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaTileType TileType
        {
            get { return m_TileType; }
            set
            {
                m_TileType = value;

                switch (m_TileType)
                {
                    case ArenaTileType.StartLocation:
                        TeamNumber = 0; 
                        PlayerNumber = 0; 

                        ItemID = 6178;
                    break;
                    case ArenaTileType.WallLocation:
                        TeamNumber = 0;
                        PlayerNumber = 0; 

                        ItemID = 6182; 
                        Hue = 2267; 
                    break;
                    case ArenaTileType.ExitLocation:
                        TeamNumber = -1;
                        PlayerNumber = -1; 

                        ItemID = 6179; 
                        Hue = 2265; 
                    break;
                }
            }
        }

        public int m_TeamNumber = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TeamNumber
        {
            get { return m_TeamNumber; }
            set
            {
                m_TeamNumber = value;

                switch (m_TeamNumber)
                {
                    case -1: Hue = 0; break;

                    case 0: Hue = 201; break;
                    case 1: Hue = 239; break;
                    case 2: Hue = 2127; break;
                    case 3: Hue = 57; break;
                }
            }
        }

        public int m_PlayerNumber = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PlayerNumber
        {
            get { return m_PlayerNumber; }
            set { m_PlayerNumber = value; }
        }
        
        [Constructable]
        public ArenaTile(): base(6178)
        {
            Name = "arena tile";

            Movable = false;

            TeamNumber = 0;
        }

        public ArenaTile(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            switch (TileType)
            {
                case ArenaTileType.StartLocation:
                    LabelTo(from, "Arena Start Location");
                    LabelTo(from, "(Team " + m_TeamNumber.ToString() + ")");
                break;

                case ArenaTileType.WallLocation:
                    LabelTo(from, "Arena Wall Location" + m_PlayerNumber.ToString());
                    LabelTo(from, "(Player " + m_PlayerNumber.ToString() + ")");
                break;

                case ArenaTileType.ExitLocation:
                    LabelTo(from, "Arena Exit Location");
                break;
            }
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write((int)m_TileType);
            writer.Write(m_TeamNumber);
            writer.Write(m_PlayerNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                TileType = (ArenaTileType)reader.ReadInt();
                TeamNumber = reader.ReadInt();
                PlayerNumber = reader.ReadInt();
            }
        }
    }

    #endregion

    #region ArenaFight

    public class ArenaFight : Item
    {
        public CompetitionContext m_CompetitionContext;

        [Constructable]
        public ArenaFight(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public ArenaFight(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_CompetitionContext);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_CompetitionContext = reader.ReadItem() as CompetitionContext;
            }
        }
    }

    #endregion
}