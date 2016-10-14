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
        public string m_TeamName = "";
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_TeamName);

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
                m_TeamName = reader.ReadString();

                int participantsCount = reader.ReadInt();
                for (int a = 0; a < participantsCount; a++)
                {
                    m_Participants.Add(reader.ReadItem() as ArenaParticipant);
                }
            }
        }
    }
}