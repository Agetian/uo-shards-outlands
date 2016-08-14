using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public class ArenaTeam : Item
    {
        public DateTime m_LastEventTime = DateTime.UtcNow;

        public List<ArenaParticipant> m_Participants = new List<ArenaParticipant>();

        [Constructable]
        public ArenaTeam(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public ArenaTeam(Serial serial): base(serial)
        {
        }

        public bool IsReadyForEvent()
        {
            int requiredPlayers = 1;
            int readyPlayers = 0;

            foreach (ArenaParticipant arenaParticipant in m_Participants)
            {
                if (arenaParticipant == null) continue;
                if (arenaParticipant.Deleted) continue;

                if (arenaParticipant.m_EventStatus == ArenaParticipant.EventStatusType.Waiting)
                {
                    //TEST: Check for Other "Conditions" like being Nearby, Being Jail, etc

                    if (arenaParticipant.IsReadyForEvent())
                        readyPlayers++;
                }
            }

            if (readyPlayers >= requiredPlayers)
                return true;

            return false;
        }

        public ArenaParticipant GetPlayerParticipant(PlayerMobile player)
        {
            foreach (ArenaParticipant arenaParticipant in m_Participants)
            {
                if (arenaParticipant == null) continue;
                if (arenaParticipant.Deleted) continue;

                if (arenaParticipant.m_Player == player)
                    return arenaParticipant;
            }

            return null;
        }

        public void LeaveTeam(ArenaParticipant participant)
        {
            if (participant == null)
                return;

            if (m_Participants.Contains(participant))
                m_Participants.Remove(participant);

            participant.Delete();

            //TEST: BROADCAST TO REST OF TEAM THAT PLAYER HAS LEFT
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_LastEventTime);

            writer.Write(m_Participants.Count);
            for (int a = 0; a < m_Participants.Count; a++)
            {
                writer.Write(m_Participants[a]);
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

                int participantsCount = reader.ReadInt();
                for (int a = 0; a < participantsCount; a++)
                {
                    m_Participants.Add(reader.ReadItem() as ArenaParticipant);
                }
            }
        }
    }
}