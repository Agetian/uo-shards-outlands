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
            PlayerStartLocation,
            FollowerStartLocation,
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

        public ArenaTileType m_TileType = ArenaTileType.PlayerStartLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaTileType TileType
        {
            get { return m_TileType; }
            set
            {
                m_TileType = value;

                switch (m_TileType)
                {
                    case ArenaTileType.PlayerStartLocation:
                        TeamNumber = 0;
                        PlayerNumber = 0;

                        ItemID = 6178;
                        Hue = 201;
                    break;

                    case ArenaTileType.FollowerStartLocation:
                        TeamNumber = 0;
                        PlayerNumber = 0;
                    break;

                    case ArenaTileType.WallLocation:
                        TeamNumber = -1;
                        PlayerNumber = 0;
                    break;

                    case ArenaTileType.ExitLocation:
                        TeamNumber = -1;
                        PlayerNumber = -1;
                    break;
                }

                Configure();
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

                Configure();
            }
        }

        public int m_PlayerNumber = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PlayerNumber
        {
            get { return m_PlayerNumber; }
            set { m_PlayerNumber = value; }
        }

        public int m_FollowerNumber = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int FollowerNumber
        {
            get { return m_FollowerNumber; }
            set { m_FollowerNumber = value; }
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

        public void Configure()
        {
            switch (TileType)
            {
                case ArenaTileType.PlayerStartLocation:
                    ItemID = 6178;

                    switch (TeamNumber)
                    {
                        case 0: Hue = 200; break;
                        case 1: Hue = 238; break;
                    }
                break;

                case ArenaTileType.FollowerStartLocation:
                    ItemID = 6178;

                    switch (TeamNumber)
                    {
                        case 0: Hue = 201; break;
                        case 1: Hue = 239; break;
                    }
                break;

                case ArenaTileType.WallLocation:
                    ItemID = 6182;
                    Hue = 2267;
                break;

                case ArenaTileType.ExitLocation:
                    ItemID = 6179;
                    Hue = 2265;
                break;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            switch (TileType)
            {
                case ArenaTileType.PlayerStartLocation:
                    LabelTo(from, "Arena Start Location");
                    LabelTo(from, "(Team " + m_TeamNumber.ToString() + " / Player " + m_PlayerNumber.ToString() + ")");
                break;

                case ArenaTileType.FollowerStartLocation:
                    LabelTo(from, "Arena Start Location");
                    LabelTo(from, "(Team " + m_TeamNumber.ToString() + " / Player " + m_PlayerNumber.ToString() + " / Follower " + m_FollowerNumber.ToString() + ")");
                break;

                case ArenaTileType.WallLocation:
                    LabelTo(from, "Arena Wall Location");
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

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();           
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
            writer.Write(m_FollowerNumber);
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
                FollowerNumber = reader.ReadInt();
            }

            //-----

            m_Instances.Add(this);
        }
    }
}