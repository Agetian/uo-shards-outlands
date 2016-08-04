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

        public Rectangle2D m_Boundary;
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Boundary
        {
            get { return m_Boundary; }
            set { m_Boundary = value; }
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

        public bool IsWithin(Point3D location)
        {
            Point2D sourcePoint = new Point2D(location.X, location.Y);

            if (m_Boundary.Contains(sourcePoint))
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

        public void MatchComplete()
        {
            if (m_ArenaFight != null)
            {
                foreach (ArenaTeam arenaTeam in m_ArenaFight.m_Teams)
                {
                    if (arenaTeam == null) continue;
                    if (arenaTeam.Deleted) continue;

                    foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                    {
                        if (arenaParticipant == null) continue;
                        if (arenaParticipant.Deleted) continue;
                        if (arenaParticipant.m_Player == null) continue;
                        if (arenaParticipant.m_Player.Deleted) continue;

                        //Clear Players From Arena
                        if (IsWithin(arenaParticipant.m_Player.Location))
                        {
                            ArenaTile exitTile = GetRandomExitTile();

                            if (exitTile != null)                            
                                arenaParticipant.m_Player.Location = exitTile.Location;                            

                            else
                                arenaParticipant.m_Player.Location = Location;                          
                        }
                    }
                }
            }

            //TEST: Clear Items from Arena
            
            if (m_ArenaGroupController != null)
                m_ArenaGroupController.MatchComplete(m_ArenaFight);

            m_ArenaFight = null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaGroupController);
            writer.Write(m_Enabled);
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
                ArenaGroupController = reader.ReadItem() as ArenaGroupController;
                Enabled = reader.ReadBool();
                Boundary = reader.ReadRect2D();
                m_ArenaFight = reader.ReadItem() as ArenaFight;
            }

            //-----

            m_Instances.Add(this);
        }
    }
}