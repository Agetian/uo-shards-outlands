using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class RuneTome : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
        }

        private LockedDownAccessLevelType m_LockedDownAccessLevel = LockedDownAccessLevelType.Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public LockedDownAccessLevelType LockedDownAccessLevel
        {
            get { return m_LockedDownAccessLevel; }
            set { m_LockedDownAccessLevel = value; }
        }

        private string m_DisplayName = "Rune Tome";
        [CommandProperty(AccessLevel.GameMaster)]
        public string DisplayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }

        private int m_RecallCharges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RecallCharges
        {
            get { return m_RecallCharges; }
            set { m_RecallCharges = value; }
        }

        private int m_GateCharges;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GateCharges
        {
            get { return m_GateCharges; }
            set { m_GateCharges = value; }
        }

        public static int MaxRecallCharges = 100;
        public static int MaxGateCharges = 25;

        public static int RecallChargesPerScroll = 1;
        public static int GateChargesPerScroll = 5;

        public static int EntriesPerSide = 14;
        public static int MaxEntries = 28;

        public List<RuneTomeRuneEntry> m_RecallRuneEntries = new List<RuneTomeRuneEntry>();

        private List<Mobile> m_Openers = new List<Mobile>();
        public List<Mobile> Openers
        {
            get { return m_Openers; }
            set { m_Openers = value; }
        }

        [Constructable]
        public RuneTome(): base(8793)
        {
            Name = "a rune tome";

            Hue = 2418;
        }

        public RuneTome(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + m_DisplayName + ")");

            /*
            if (IsLockedDown)
            {
                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner: LabelTo(from, "[owner accessable]"); break;
                    case LockedDownAccessLevelType.CoOwner: LabelTo(from, "[co-owner accessable]"); break;
                    case LockedDownAccessLevelType.Friend: LabelTo(from, "[friend accessable]"); break;
                    case LockedDownAccessLevelType.Anyone: LabelTo(from, "[freely access]"); break;
                }
            }
            */
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null) return;
            if (player.Backpack == null) return;

            if (from.InRange(GetWorldLocation(), 2))
            {
                if (RootParent is BaseCreature)
                {
                    from.SendLocalizedMessage(502402); // That is inaccessible.
                    return;
                }

                if (IsChildOf(from.BankBox) && !from.BankBox.Opened)
                {
                    from.SendMessage("You cannot access that runebook from your closed bank box.");
                    return;
                }

                player.CloseGump(typeof(RuneTomeGump));
                player.SendGump(new RuneTomeGump(player, new RuneTomeGumpObject(this)));

                player.SendSound(0x055);

                //TEST: FIX THIS
                if (from.Player && from.BankBox.Items.Contains(this))
                    ((PlayerMobile)from).CloseRunebookGump = true;

                else if (from.Player)
                    ((PlayerMobile)from).CloseRunebookGump = false;

                m_Openers.Add(from);
            } 
        }       

        public bool IsOpen(Mobile toCheck)
        {
            NetState netstate = toCheck.NetState;

            if (netstate != null)
            {
                foreach (Gump gump in netstate.Gumps)
                {
                    RuneTomeGump runeTomeGump = gump as RuneTomeGump;

                    if (runeTomeGump != null)
                    {
                        if (runeTomeGump.m_RuneTomeGumpObject != null)
                        {
                            if (runeTomeGump.m_RuneTomeGumpObject.m_RuneTome == this)
                                return true;
                        }                       
                    }
                }
            }

            return false;
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from.HasGump(typeof(RuneTomeGump)))
            {
                from.SendLocalizedMessage(500169); // You cannot pick that up.
                return false;
            }

            foreach (Mobile m in m_Openers)
                if (IsOpen(m))
                    m.CloseGump(typeof(RuneTomeGump));

            m_Openers.Clear();

            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is RecallRune)
            {
                RecallRune recallRune = dropped as RecallRune;

                if (IsLockedDown && !HasChargeAccess(from) && from.AccessLevel == AccessLevel.Player)                
                    from.SendMessage("You do not have the neccessary access rights to do that.");                

                else if (IsOpen(from))
                    from.SendMessage("You cannot alter that runebook while viewing it's contents.");

                else if (m_RecallRuneEntries.Count >= RuneTome.MaxEntries)
                    from.SendMessage("This rune tome is at it's maximum capacity.");

                else
                {
                    if (recallRune.Marked && recallRune.TargetMap != null)
                    {
                        RuneTomeRuneEntry recallRuneEntry = new RuneTomeRuneEntry(false, recallRune.Description, recallRune.Target, recallRune.TargetMap, recallRune.House);

                        if (m_RecallRuneEntries.Count == 0)
                            recallRuneEntry.m_IsDefaultRune = true;

                        m_RecallRuneEntries.Add(recallRuneEntry);

                        recallRune.Delete();

                        from.SendSound(0x42);

                        from.SendMessage("You add the rune to the rune tome.");

                        return true;
                    }

                    else
                        from.SendMessage("That recall rune is not marked.");
                }
            }

            if (dropped is RecallScroll)
            {
                if (m_RecallCharges >= MaxRecallCharges)
                    from.SendMessage("That rune tome is at it's maximum number of stored recall charges.");

                else
                {
                    int availableCharges = MaxRecallCharges - RecallCharges;
                    int scrollChargeValue = dropped.Amount * RecallChargesPerScroll;

                    if (availableCharges >= scrollChargeValue)
                    {
                        RecallCharges += scrollChargeValue;
                        
                        from.SendMessage("You add " + scrollChargeValue.ToString() + " recall charges to the rune tome.");
                        from.SendSound(0x249);

                        dropped.Delete();
                    }

                    else
                    {
                        int scrollsUsed = (int)(Math.Ceiling((double)availableCharges / (double)RecallChargesPerScroll));
                        int chargesAdded = (scrollsUsed * RecallChargesPerScroll);

                        m_RecallCharges += chargesAdded;

                        if (RecallCharges > MaxRecallCharges)
                            RecallCharges = MaxRecallCharges;                       

                        from.SendMessage("You add " + chargesAdded.ToString() + " recall charges to the rune tome.");
                        from.SendSound(0x249);

                        dropped.Consume(scrollsUsed);
                    }   
                }
            }

            if (dropped is GateTravelScroll)
            {
                if (m_GateCharges >= MaxGateCharges)
                    from.SendMessage("That rune tome is at it's maximum number of stored gate charges.");

                else
                {
                    int availableCharges = MaxGateCharges - GateCharges;
                    int scrollChargeValue = dropped.Amount * GateChargesPerScroll;

                    if (availableCharges >= scrollChargeValue)
                    {
                        GateCharges += scrollChargeValue;

                        from.SendMessage("You add " + scrollChargeValue.ToString() + " gate charges to the rune tome.");
                        from.SendSound(0x249);

                        dropped.Delete();
                    }

                    else
                    {
                        int scrollsUsed = (int)(Math.Ceiling((double)availableCharges / (double)GateChargesPerScroll));
                        int chargesAdded = (scrollsUsed * GateChargesPerScroll);
                        
                        GateCharges += chargesAdded;

                        if (GateCharges > MaxGateCharges)
                            GateCharges = MaxGateCharges;  

                        from.SendMessage("You add " + chargesAdded.ToString() + " gate charges to the rune tome.");
                        from.SendSound(0x249);

                        dropped.Consume(scrollsUsed);
                    }
                }
            }

            return false;
        }

        public bool CanAccess(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            if (!from.Alive)
                return false;

            if (!(from.InRange(GetWorldLocation(), 2)))
                return false;

            if (IsChildOf(from.BankBox) && !from.BankBox.Opened)            
                return false;

            if (RootParent is BaseCreature)            
                return false;            

            return true;
        }

        public bool HasChargeAccess(Mobile from)
        {
            bool hasAccess = true;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if ((IsLockedDown || IsSecure) && house != null)
            {
                hasAccess = false;
                
                switch (LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Anyone:
                        hasAccess = true;
                    break;

                    case LockedDownAccessLevelType.Friend:
                        if (house.IsFriend(from) || house.IsCoOwner(from) || house.IsOwner(from))
                            hasAccess = true;
                    break;

                    case LockedDownAccessLevelType.CoOwner:
                        if (house.IsCoOwner(from) || house.IsOwner(from))
                            hasAccess = true;
                    break;

                    case LockedDownAccessLevelType.Owner:
                        if (house.IsOwner(from))
                            hasAccess = true;
                    break;
                }                
            }

            return hasAccess;
        }

        public override void OnAfterDuped(Item newItem)
        {
            //TEST: FINISH

            /*
            Runebook book = newItem as Runebook;

            if (book == null)
                return;

            book.m_Entries = new List<RunebookEntry>();

            for (int i = 0; i < m_Entries.Count; i++)
            {
                RunebookEntry entry = m_Entries[i];

                book.m_Entries.Add(new RunebookEntry(entry.Location, entry.Map, entry.Description, entry.House));
            }
            */
        }         

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  
       
            //Version 0
            writer.Write((int)m_LockedDownAccessLevel);
            writer.Write(m_DisplayName);
            writer.Write(m_RecallCharges);
            writer.Write(m_GateCharges);

            writer.Write(m_RecallRuneEntries.Count);
            for (int a = 0; a < m_RecallRuneEntries.Count; a++)
            {
                writer.Write(m_RecallRuneEntries[a].m_IsDefaultRune);
                writer.Write(m_RecallRuneEntries[a].m_Description);
                writer.Write(m_RecallRuneEntries[a].m_Target);
                writer.Write(m_RecallRuneEntries[a].m_TargetMap);
                writer.Write(m_RecallRuneEntries[a].m_House);
            }            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_LockedDownAccessLevel = (LockedDownAccessLevelType)reader.ReadInt();
                m_DisplayName = reader.ReadString();
                m_RecallCharges = reader.ReadInt();
                m_GateCharges = reader.ReadInt();

                int recallRuneEntryCount = reader.ReadInt();
                for (int a = 0; a < recallRuneEntryCount; a++)
                {
                    bool isDefaultRune = reader.ReadBool();
                    string description = reader.ReadString();
                    Point3D target = reader.ReadPoint3D();
                    Map targetMap = reader.ReadMap();
                    BaseHouse house = (BaseHouse)reader.ReadItem();

                    m_RecallRuneEntries.Add(new RuneTomeRuneEntry(isDefaultRune, description, target, targetMap, house));
                }
            }
        }
    }

    public class RuneTomeRuneEntry
    {
        public bool m_IsDefaultRune = false;

        public string m_Description;
        public Point3D m_Target;
        public Map m_TargetMap;
        public BaseHouse m_House;

        public RuneTomeRuneEntry(bool isDefaultRune, string description, Point3D target, Map targetMap, BaseHouse house)
        {
            m_IsDefaultRune = isDefaultRune;
            m_Description = description;
            m_Target = target;
            m_TargetMap = targetMap;
            m_House = house;
        }
    }
}