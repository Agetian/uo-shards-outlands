using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server
{
    public class ArenaTournament: Item
    {
        public enum TournamentStatusType
        {
            Configuration,
            Listed,
            Fighting
        }

        public ArenaGroupController m_ArenaGroupController;

        public TournamentStatusType m_TournamentStatus = TournamentStatusType.Configuration;
        public ArenaRuleset m_Ruleset = new ArenaRuleset();

        public DateTime m_StartTime = DateTime.UtcNow + TimeSpan.FromDays(30);

        [Constructable]
        public ArenaTournament(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public ArenaTournament(Serial serial): base(serial)
        {
        }

        public override void OnDelete()
        {
            if (m_ArenaGroupController != null)
            {
                if (m_ArenaGroupController.m_Tournaments.Contains(this))
                    m_ArenaGroupController.m_Tournaments.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0  
            writer.Write(m_ArenaGroupController);
            writer.Write((int)m_TournamentStatus);
            writer.Write(m_Ruleset);
            writer.Write(m_StartTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaGroupController = (ArenaGroupController)reader.ReadItem();
                m_TournamentStatus = (TournamentStatusType)reader.ReadInt();
                m_Ruleset = (ArenaRuleset)reader.ReadItem();
                m_StartTime = reader.ReadDateTime();
            }
        }
    }
}