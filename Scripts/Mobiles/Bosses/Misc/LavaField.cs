using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
    public class LavaField : Item
    {
        public Timer m_Timer;

        public Mobile m_Owner;

        public double m_IntervalDuration = 2;
        public int m_MaxIntervals = 30;
        public int m_MinDamage = 3;
        public int m_MaxDamage = 6;

        public bool m_HitOwner = false;
        public bool m_HitMonsters = false;
        public bool m_HitPlayers = true;

        public int m_HitSound = 0x591;

        public bool m_ActivateOnMoveOver = true;

        [Constructable]
        public LavaField(Mobile owner, int hue, double intervalDuration, int maxIntervals, int minDamage, int maxDamage, bool hitOwner, bool hitMonsters, bool hitPlayers, int hitSound, bool activateOnMoveOver)
            : base(Utility.RandomList(4876, 4882, 13371, 13383, 13390, 13410, 6769, 4870, 4864, 4888, 4974, 13416, 4858, 4852, 4846))
        {
            Name = "lava field";
            Hue = hue;

            Movable = false;

            m_Owner = owner;

            m_IntervalDuration = 2;
            m_MaxIntervals = 30;
            m_MinDamage = 3;
            m_MaxDamage = 6;

            m_HitOwner = false;
            m_HitMonsters = false;
            m_HitPlayers = true;

            if (hitSound != -1)
                m_HitSound = hitSound;

            m_ActivateOnMoveOver = activateOnMoveOver;

            if (!OtherFireFieldOnTile())
            {
                Effects.PlaySound(Location, Map, m_HitSound);

                m_Timer = new InternalTimer(this, m_Owner, intervalDuration, maxIntervals, minDamage, maxDamage, hitOwner, hitMonsters, hitPlayers, hitSound);
                m_Timer.Start();
            }

            else
                Delete();
        }

        public LavaField(Serial serial): base(serial)
        {
        }

        public bool OtherFireFieldOnTile()
        {
            IPooledEnumerable itemsOnTile = GetItemsInRange(0);

            bool foundFireField = false;

            foreach (Item item in itemsOnTile)
            {
                if (item is LavaField)
                {
                    foundFireField = true;
                    break;
                }
            }

            itemsOnTile.Free();

            return foundFireField;
        }

        public override bool OnMoveOver(Mobile mobile)
        {
            if (mobile == null) return true;
            if (mobile.Deleted || !mobile.Alive) return true;

            if (!m_ActivateOnMoveOver)
                return true;

            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                return true;

            bool validTarget = false;

            PlayerMobile pm_Target = mobile as PlayerMobile;
            BaseCreature bc_Target = mobile as BaseCreature;

            if (mobile == m_Owner && m_Owner != null && m_HitOwner)
                validTarget = true;

            if (pm_Target != null)
                validTarget = m_HitPlayers;

            if (bc_Target != null)
            {
                validTarget = m_HitMonsters;
                
                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    validTarget = m_HitPlayers;
            }

            if (!validTarget)
                return true;

            double damage = Utility.RandomMinMax(m_MinDamage, m_MaxDamage);

            if (mobile is BaseCreature)
                damage *= 2;

            TimedStatic fire = new TimedStatic(6571, 1);           
            fire.Name = "fire";
            Point3D fireLocation = Location;
            fireLocation.Z += 3;
            fire.MoveToWorld(fireLocation, Map);

            Effects.PlaySound(mobile.Location, mobile.Map, 0x591);

            AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);            

            return true;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   

            writer.Write(m_Owner);

            writer.Write(m_IntervalDuration);
            writer.Write(m_MaxIntervals);
            writer.Write(m_MinDamage);
            writer.Write(m_MaxDamage);

            writer.Write(m_HitOwner);
            writer.Write(m_HitMonsters);
            writer.Write(m_HitPlayers);

            writer.Write(m_HitSound);
            writer.Write(m_ActivateOnMoveOver);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Owner = reader.ReadMobile();

                m_IntervalDuration = reader.ReadDouble();
                m_MaxIntervals = reader.ReadInt();
                m_MinDamage = reader.ReadInt();
                m_MaxDamage = reader.ReadInt();
                m_HitOwner = reader.ReadBool();
                m_HitMonsters = reader.ReadBool();
                m_HitPlayers = reader.ReadBool();

                m_HitSound = reader.ReadInt();
                m_ActivateOnMoveOver = reader.ReadBool();
            }

            m_Timer = new InternalTimer(this, m_Owner, m_IntervalDuration, m_MaxIntervals, m_MinDamage, m_MaxDamage, m_HitOwner, m_HitMonsters, m_HitPlayers, m_HitSound);
            m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            public LavaField m_LavaField;

            public Mobile m_Owner;

            public int m_IntervalCount = 0;
            public double m_IntervalDuration = 2;
            public int m_MaxIntervals = 30;
            public int m_MinDamage = 3;
            public int m_MaxDamage = 6;

            public bool m_HitOwner = false;
            public bool m_HitMonsters = false;
            public bool m_HitPlayers = true;

            public int m_HitSound = 0x591;

            public static Queue m_Queue = new Queue();

            public InternalTimer(LavaField LavaField, Mobile owner, double intervalDuration, int maxIntervals, int minDamage, int maxDamage, bool hitOwner, bool hitMonsters, bool hitPlayers, int hitSound)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(intervalDuration))
            {
                Priority = TimerPriority.TwoFiftyMS;

                m_LavaField = LavaField;

                m_Owner = owner;

                m_IntervalDuration = intervalDuration;
                m_MaxIntervals = maxIntervals;
                m_MinDamage = minDamage;
                m_MaxDamage = maxDamage;

                m_HitOwner = hitOwner;
                m_HitMonsters = hitMonsters;
                m_HitPlayers = hitPlayers;

                if (hitSound != -1)
                    m_HitSound = hitSound;
            }

            protected override void OnTick()
            {
                if (m_LavaField == null)
                {
                    Stop();
                    return;
                }

                if (m_LavaField.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_IntervalCount >= m_MaxIntervals)
                {
                    Stop();
                    m_LavaField.Delete();

                    return;
                }

                IPooledEnumerable mobilesOnTile = m_LavaField.GetMobilesInRange(0);

                foreach (Mobile mobile in mobilesOnTile)
                {
                    if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                        continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (mobile == m_Owner && m_Owner != null && m_HitOwner)
                        validTarget = true;

                    if (pm_Target != null)
                        validTarget = m_HitPlayers;

                    if (bc_Target != null)
                    {
                        validTarget = m_HitMonsters;

                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = m_HitPlayers;
                    }

                    if (validTarget)
                        m_Queue.Enqueue(mobile);
                }

                mobilesOnTile.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    double damage = Utility.RandomMinMax(m_MinDamage, m_MaxDamage);

                    if (mobile is BaseCreature)
                        damage *= 2;

                    TimedStatic fire = new TimedStatic(6571, 1);
                    fire.Name = "fire";
                    Point3D fireLocation = m_LavaField.Location;
                    fireLocation.Z += 3;
                    fire.MoveToWorld(fireLocation, m_LavaField.Map);

                    AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                    Effects.PlaySound(mobile.Location, mobile.Map, m_HitSound);
                }

                m_IntervalCount++;
            }
        }
    }
}