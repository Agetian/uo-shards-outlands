using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public class ArenaParticipant : Item
    {
        public enum EventStatusType
        {
            Waiting,
            Playing,            
            PostBattle,
            Eliminated,
            Inactive
        }

        public enum FightStatusType
        {            
            Alive,
            Dead,
            Disqualified,
            PostBattle
        }

        public DateTime m_LastEventTime = DateTime.UtcNow;

        public PlayerMobile m_Player;
        public ArenaTeam m_ArenaTeam;
        public ArenaFight m_ArenaFight;
        public ArenaGroupController m_ArenaGroupController;

        public EventStatusType m_EventStatus = EventStatusType.Waiting;
        public FightStatusType m_FightStatus = FightStatusType.Alive;

        public int m_DamageDealt = 0;
        public int m_DamageReceived = 0;
        public int m_LowestHealth = 0;

        public List<ArenaItemUsage> m_ItemUsages = new List<ArenaItemUsage>();

        [Constructable]
        public ArenaParticipant(PlayerMobile player): base(0x0)
        {
            m_Player = player;

            Visible = false;
            Movable = false;
        }

        public bool IsReadyForEvent()
        {
            if (m_Player == null)
                return false;

            if (!m_Player.Alive)
                return false;

            if (m_Player.NetState == null)
                return false;

            return true;
        }

        public void ResetArenaFightValues()
        {
            m_DamageDealt = 0;
            m_DamageReceived = 0;
            m_LowestHealth = 0;

            foreach(ArenaItemUsage arenaItemUsage in m_ItemUsages)
            {
                if (arenaItemUsage == null) 
                    continue;
                
                arenaItemUsage.m_Uses = 0;
            }
        }

        public ArenaParticipant(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_LastEventTime);
            writer.Write(m_Player);
            writer.Write(m_ArenaTeam);
            writer.Write(m_ArenaFight);
            writer.Write(m_ArenaGroupController);
            writer.Write((int)m_EventStatus);
            writer.Write((int)m_FightStatus);
            writer.Write(m_DamageDealt);
            writer.Write(m_DamageReceived);
            writer.Write(m_LowestHealth);

            writer.Write(m_ItemUsages.Count);
            for (int a = 0; a < m_ItemUsages.Count; a++)
            {
                if (m_ItemUsages[a].m_ItemType == null)
                    writer.Write("null");
                else
                    writer.Write(m_ItemUsages[a].m_ItemType.ToString());
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_LastEventTime = reader.ReadDateTime();
                m_Player = reader.ReadMobile() as PlayerMobile;
                m_ArenaTeam = (ArenaTeam)reader.ReadItem();
                m_ArenaFight = (ArenaFight)reader.ReadItem();
                m_ArenaGroupController = (ArenaGroupController)reader.ReadItem();
                m_EventStatus = (EventStatusType)reader.ReadInt();
                m_FightStatus = (FightStatusType)reader.ReadInt();
                m_DamageDealt = reader.ReadInt();
                m_DamageReceived = reader.ReadInt();
                m_LowestHealth = reader.ReadInt();

                int itemUsages = reader.ReadInt();
                for (int a = 0; a < itemUsages; a++)
                {
                    string typeText = reader.ReadString();
                    Type itemType = null;

                    if (typeText != "null")
                        itemType = Type.GetType(typeText);

                    int usages = reader.ReadInt();

                    if (itemType != null)
                        m_ItemUsages.Add(new ArenaItemUsage(itemType, usages));
                }
            }
        }
    }

    public class ArenaItemUsage
    {
        public Type m_ItemType;
        public int m_Uses;

        public ArenaItemUsage(Type itemType, int uses)
        {
            m_ItemType = itemType;
            m_Uses = uses;
        }
    }
}