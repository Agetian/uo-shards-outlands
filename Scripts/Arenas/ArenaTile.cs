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
    public class ArenaTile : Item
    {
        public enum ArenaTileType
        {
            StartLocation,
            WallLocation,
            ExitLocation
        }

        public static List<ArenaTile> m_Instances = new List<ArenaTile>();

        //-----

        public ArenaController m_ArenaController;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaController ArenaController
        {
            get { return m_ArenaController; }
            set { m_ArenaController = value; }
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
                        Hue = 201;
                    break;

                    case ArenaTileType.WallLocation:
                        TeamNumber = -1;
                        PlayerNumber = -1;

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

        public Direction m_Facing;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
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

            //-----

            m_Instances.Add(this);
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
                    LabelTo(from, "Arena Wall Location");                   
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
            writer.Write(m_ArenaController);
            writer.Write((int)m_TileType);
            writer.Write((int)m_Facing);
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
                ArenaController = (ArenaController)reader.ReadItem();
                TileType = (ArenaTileType)reader.ReadInt();
                Facing = (Direction)reader.ReadInt();
                TeamNumber = reader.ReadInt();
                PlayerNumber = reader.ReadInt();
            }

            //-----

            m_Instances.Add(this);
        }
    }
}