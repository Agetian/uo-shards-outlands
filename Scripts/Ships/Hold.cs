using System;
using Server;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class Hold : Container
    {
        private BaseShip m_Ship;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseShip Ship { get { return m_Ship; } }

        public override int DefaultMaxWeight
		{
			get
			{
				return 0;
			}
		}

        public Hold(BaseShip ship): base(0x3EAE)
        {
            m_Ship = ship;

            Movable = false;                  
        }

        public Hold(Serial serial): base(serial)
        {
        }

        public void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.East: ItemID = 0x3E65; break;
                case Direction.West: ItemID = 0x3E93; break;
                case Direction.North: ItemID = 0x3EAE; break;
                case Direction.South: ItemID = 0x3EB9; break;
            }
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (m_Ship == null || !m_Ship.Contains(from))
                return false;

            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (m_Ship == null || !m_Ship.Contains(from))
                return false;

            return base.CheckLift(from, item, ref reject);
        }        

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (m_Ship == null || !m_Ship.Contains(from))
                return false;

            //Don't Have Normal Access
            if (!(m_Ship.IsOwner(from) || m_Ship.IsCoOwner(from)))
            {
                //Item is Coming From Outside Hold
                if (item.Parent != this)
                {
                    from.SendMessage("You do not have access to add items to the hold.");
                    return false;
                }
            }

            return base.OnDragDrop(from, item);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this && (m_Ship == null || !m_Ship.Contains(from)))
                return false;

            return base.CheckItemUse(from, item);
        }

        public override void OnAfterDelete()
        {
            if (m_Ship != null)
                m_Ship.Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Ship == null) return;
            if (m_Ship.Deleted) return;
            if (this == null) return;
            if (Deleted) return;

            if (from.AccessLevel > AccessLevel.Player)
            {
                from.SendMessage("You use your godly power's to access the hold.");
                base.OnDoubleClick(from);
                return;
            }

            if (m_Ship == null || !m_Ship.Contains(from))     
            {
                from.SendMessage("You must be onboard that ship to access it's hold.");
                return;
            }

            //NPC Ship
            if (m_Ship.MobileControlType != MobileControlType.Player)
            {
                if (m_Ship.HasCrewAlive())
                {
                    from.SendMessage("You cannot attempt to access that ship's hold while it's crew members still live.");
                    return;
                }
            }

            //Player Ship
            else
            {
                //Refresh Ship
                if (m_Ship.IsOwner(from) || m_Ship.IsCoOwner(from) || m_Ship.IsFriend(from))            
                    m_Ship.Refresh();            

                //Don't Have Access
                if (!(m_Ship.IsOwner(from) || m_Ship.IsCoOwner(from)))
                {
                    from.SendMessage("You must use a lockpick to attempt to access that.");
                    return;
                }
            }
               
            base.OnDoubleClick(from);            
        }

        public void LockpickHold(Mobile from, Lockpick lockpick)
        {
            if (from == null || lockpick == null || this.Deleted || this == null)
                return;

            //NPC Ship
            if (m_Ship.MobileControlType != MobileControlType.Player)
            {
                if (m_Ship.HasCrewAlive())
                {
                    from.SendMessage("You cannot access that ship's hold while it's crew members still live.");
                    return;
                }

                else
                {
                    from.SendMessage("With the ship's crew dead, you may access the hold freely.");
                    return;                
                }                
            }

            Effects.PlaySound(Location, Map, 0x241);
            from.BeginAction((typeof(Lockpick)));            

            Timer.DelayCall(TimeSpan.FromSeconds(3.0), delegate()
            {
                if (from != null)
                    from.EndAction(typeof(Lockpick));
                
                if (this == null) return;                
                if (this.Deleted) return;

                if (from.GetDistanceToSqrt(this) > 2)
                {
                    from.SendMessage("You are too far away from the hold to continue lockpicking.");
                    return;
                }

                if (!from.Alive)
                    return;
                
                double lockpickingSkill = from.Skills[SkillName.Lockpicking].Value;
                double successChance = (lockpickingSkill - 95) * .02; //10% At GM
                double chanceResult = Utility.RandomDouble();
                                
                //Succeed Lockpicking
                if (chanceResult < successChance)
                {
                    Effects.PlaySound(Location, Map, 0x4A);
                    from.SendMessage("You succeed in breaking into the ship's hold");

                    base.OnDoubleClick(from);

                    return;
                }

                //Fail Lockpicking
                else
                {
                    if (Utility.RandomDouble() > .5)
                    {
                        Effects.PlaySound(Location, Map, 0x3A4);
                        from.SendMessage("You fail to break into the ship's hold and break your lockpick in the process.");

                        lockpick.Consume();

                        return;
                    }

                    else
                    {                        
                        from.SendMessage("You fail to break into the ship's hold");

                        return;
                    }
                }
            });            
        }

        public override bool IsDecoContainer
        {
            get { return false; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(m_Ship);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    m_Ship = reader.ReadItem() as BaseShip;

                    if (m_Ship == null || Parent != null)
                        Delete();

                    Movable = false;

                    break;
                }
            }
        }
    }
}