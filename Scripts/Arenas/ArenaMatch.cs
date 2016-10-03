using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server
{
    public class ArenaMatch : Item
    {
        public enum MatchStatusType
        {
            Listed,
            Fighting
        }
        
        public ArenaGroupController m_ArenaGroupController;

        public ArenaTournament m_Tournament;

        public PlayerMobile m_Creator;
        public DateTime m_CreationDate;

        public bool m_CreatorReady;
        public DateTime m_NextReadyCheck = DateTime.UtcNow;

        public static TimeSpan ReadyCheckInterval = TimeSpan.FromSeconds(60);

        public MatchStatusType m_MatchStatus = MatchStatusType.Listed;        
        public ArenaRuleset m_Ruleset = new ArenaRuleset();        
        public ArenaFight m_ArenaFight;
        public List<ArenaTeam> m_Teams = new List<ArenaTeam>();

        [Constructable]
        public ArenaMatch(ArenaGroupController arenaGroupController, PlayerMobile player): base(0x0)
        {
            m_ArenaGroupController = arenaGroupController;
            m_Creator = player;
            m_CreationDate = DateTime.UtcNow;

            Visible = false;
            Movable = false;
        }

        public ArenaMatch(Serial serial): base(serial)
        {
        }

        public ArenaTeam GetTeam(int index)
        {
            for (int a = 0; a < m_Teams.Count; a++)
            {
                if (m_Teams[a] == null) continue;
                if (m_Teams[a].Deleted) continue;

                if (a == index)
                    return m_Teams[a];
            }

            return null;
        }

        public ArenaParticipant GetParticipant(PlayerMobile player)
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;

                    if (participant.m_Player == player)
                        return participant;
                }                
            }

            return null;
        }
        
        public void LeaveMatch(PlayerMobile player)
        {
            if (player == null)
                return;

            ArenaTeam playerTeam = null;
            ArenaParticipant playerParticipant = null;

            foreach (ArenaTeam team in m_Teams)
            {
                if (team == null) continue;
                if (team.Deleted) continue;

                playerTeam = team;
                playerParticipant = team.GetPlayerParticipant(player);
            }

            if (playerTeam != null && playerParticipant != null)
            {
                if (playerTeam.m_Participants.Contains(playerParticipant))
                    playerTeam.m_Participants.Remove(playerParticipant);

                playerParticipant.Delete();
            }            

            //TEST: BROADCAST TO REST OF MATCH PARTICIPANTS THAT PLAYER HAS LEFT
        }

        public bool CanPlayerJoinMatch(PlayerMobile player)
        {
            if (player == null) return false;
            if (m_Ruleset == null) return false;
            if (m_Ruleset.Deleted) return false;
            if (m_Creator == null) return false;

            if (player.AccessLevel > AccessLevel.Player)
                return true;

            if (m_Creator == player)
                return true;

            if (m_Creator.Guild != null &&  m_Ruleset.m_ListingMode == ArenaRuleset.ListingModeType.GuildOnly)
            {
                if (m_Creator.Guild == null) return false;
                if (m_Creator.Guild != player.Guild) return false;
            }

            if (m_Creator.Party != null &&  m_Ruleset.m_ListingMode == ArenaRuleset.ListingModeType.PartyOnly)
            {
                if (m_Creator.Party == null) return false;
                if (m_Creator.Party != player.Party) return false;
            }            

            return true;
        }

        public bool IsReadyToStart()
        {
            if (m_MatchStatus != MatchStatusType.Listed) return false;
            if (!m_CreatorReady) return false;
            if (m_Ruleset == null) return false;
            if (m_Ruleset.Deleted) return false;

            int fullTeams = 0;

            int playersNeededPerTeam = m_Ruleset.TeamSize;

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                int readyPlayers = 0;

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;
                    if (participant.m_Player.Deleted) continue;                                   

                    readyPlayers++;
                }

                if (readyPlayers >= playersNeededPerTeam)
                    fullTeams++;
            }

            if (fullTeams < ArenaGroupController.TeamsRequired)
                return false;

            return true;
        }

        public override void OnDelete()
        {
            if (m_ArenaGroupController != null)
            {
                if (m_ArenaGroupController.m_MatchListings.Contains(this))
                    m_ArenaGroupController.m_MatchListings.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaGroupController);

            writer.Write(m_Tournament);

            writer.Write(m_Creator);
            writer.Write(m_CreationDate);

            writer.Write(m_CreatorReady);
            writer.Write((int)m_MatchStatus);
            writer.Write(m_Ruleset);
            writer.Write(m_ArenaFight);

            writer.Write(m_Teams.Count);
            for (int a = 0; a < m_Teams.Count; a++)
            {
                writer.Write(m_Teams[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaGroupController = (ArenaGroupController)reader.ReadItem();

                m_Tournament = (ArenaTournament)reader.ReadItem();

                m_Creator = (PlayerMobile)reader.ReadMobile();
                m_CreationDate = reader.ReadDateTime();

                m_CreatorReady = reader.ReadBool();
                m_MatchStatus = (MatchStatusType)reader.ReadInt();
                m_Ruleset = (ArenaRuleset)reader.ReadItem();
                m_ArenaFight = (ArenaFight)reader.ReadItem();

                int m_TeamsCount = reader.ReadInt();
                for (int a = 0; a < m_TeamsCount; a++)
                {
                    m_Teams.Add(reader.ReadItem() as ArenaTeam);
                }
            }
        }
    }
}