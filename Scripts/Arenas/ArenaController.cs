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
    public class ArenaController : Item
    {
        public static List<ArenaController> m_Instances = new List<ArenaController>();

        //-----        

        public ArenaGroupController m_ArenaGroupController;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaGroupController ArenaGroupController
        {
            get { return m_ArenaGroupController; }
            set { m_ArenaGroupController = value; }
        }

        public bool m_Enabled;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public Rectangle2D m_ArenaBoundary;
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D ArenaBoundary
        {
            get { return m_ArenaBoundary; }
            set { m_ArenaBoundary = value; }
        }
        
        public ArenaFight m_ArenaFight;

        public List<ArenaTile> m_ArenaTiles
        {
            get
            {
                List<ArenaTile> arenaTiles = new List<ArenaTile>();

                foreach (ArenaTile arenaTile in ArenaTile.m_Instances)
                {
                    if (arenaTile == null) continue;
                    if (arenaTile.Deleted) continue;

                    if (arenaTile.m_ArenaController == this)
                        arenaTiles.Add(arenaTile);
                }

                return arenaTiles;
            }
        }

        public List<Item> m_Walls = new List<Item>();
        
        [Constructable]
        public ArenaController(): base(4479)
        {
            Name = "arena controller";

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

        public bool IsWithinArena(Point3D location)
        {
            Point2D sourcePoint = new Point2D(location.X, location.Y);

            if (m_ArenaBoundary.Contains(sourcePoint))
                return true;

            return false;
        }

        public ArenaTile GetRandomExitTile()
        {
            List<ArenaTile> m_ExitTiles = new List<ArenaTile>();

            foreach (ArenaTile arenaTile in m_ArenaTiles)
            {
                if (arenaTile == null) continue;
                if (arenaTile.Deleted) continue;

                if (arenaTile.m_TileType == ArenaTile.ArenaTileType.ExitLocation)
                    m_ExitTiles.Add(arenaTile);
            }

            if (m_ExitTiles.Count > 0)
                return m_ExitTiles[Utility.RandomMinMax(0, m_ExitTiles.Count - 1)];

            return null;
        }

        public ArenaTile GetPlayerStartingTile(int team, int playerNumber)
        {
            foreach (ArenaTile arenaTile in m_ArenaTiles)
            {
                if (arenaTile == null) continue;
                if (arenaTile.Deleted) continue;

                if (arenaTile.m_TileType == ArenaTile.ArenaTileType.StartLocation)
                {
                    if (arenaTile.m_TeamNumber == team && arenaTile.m_PlayerNumber == playerNumber)
                        return arenaTile;
                }
            }

            return null;
        }

        public ArenaTile GetWallTile(int position)
        {
            foreach (ArenaTile arenaTile in m_ArenaTiles)
            {
                if (arenaTile == null) continue;
                if (arenaTile.Deleted) continue;

                if (arenaTile.m_TileType == ArenaTile.ArenaTileType.WallLocation)
                {
                    if (arenaTile.m_PlayerNumber == position)
                        return arenaTile;
                }
            }

            return null;
        }

        public void ClearWalls()
        {
            Queue m_Queue = new Queue();

            foreach (Item item in m_Walls)
            {
                if (item == null) continue;
                if (item.Deleted) continue;

                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            m_Walls.Clear();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaGroupController);
            writer.Write(m_Enabled);
            writer.Write(m_ArenaBoundary);
            writer.Write(m_ArenaFight);

            writer.Write(m_Walls.Count);
            for (int a = 0; a < m_Walls.Count; a++)
            {
                writer.Write(m_Walls[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                ArenaGroupController = reader.ReadItem() as ArenaGroupController;
                Enabled = reader.ReadBool();
                ArenaBoundary = reader.ReadRect2D();
                m_ArenaFight = reader.ReadItem() as ArenaFight;

                int staticWallsCount = reader.ReadInt();
                for (int a = 0; a < staticWallsCount; a++)
                {
                    m_Walls.Add(reader.ReadItem());
                }
            }

            //-----

            m_Instances.Add(this);
        }
    }
}