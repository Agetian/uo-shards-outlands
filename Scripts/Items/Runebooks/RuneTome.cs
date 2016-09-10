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

        private string m_DisplayName = "Unnamed";
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

        public List<RecallRuneEntry> m_RecallRuneEntries = new List<RecallRuneEntry>();

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

            player.SendSound(0x055);

            player.CloseGump(typeof(RuneTomeGump));
            player.SendGump(new RuneTomeGump(player, new RuneTomeGumpObject(this)));
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

                    m_RecallRuneEntries.Add(new RecallRuneEntry(isDefaultRune, description, target, targetMap, house));
                }
            }
        }
    }

    public class RecallRuneEntry
    {
        public bool m_IsDefaultRune = false;

        public string m_Description;
        public Point3D m_Target;
        public Map m_TargetMap;
        public BaseHouse m_House;

        public RecallRuneEntry(bool isDefaultRune, string description, Point3D target, Map targetMap, BaseHouse house)
        {
            m_IsDefaultRune = isDefaultRune;
            m_Description = description;
            m_Target = target;
            m_TargetMap = targetMap;
            m_House = house;
        }
    }
}