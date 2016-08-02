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
        public ArenaGroupController(): base(3823)
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
        public ArenaController(): base(3823)
        {
            Movable = false;

            //-----

            m_Instances.Add(this);
        }

        public ArenaController(Serial serial): base(serial)
        {
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
        }

        [Constructable]
        public ArenaTile(): base(0x0)
        {
            Movable = false;
        }

        public ArenaTile(Serial serial): base(serial)
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