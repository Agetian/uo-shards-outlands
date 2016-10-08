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

        public DateTime m_NextReadyCheck = DateTime.UtcNow;

        public static TimeSpan ReadyCheckInterval = TimeSpan.FromSeconds(30);

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

        public List<ArenaParticipant> GetParticipants()
        {
            List<ArenaParticipant> m_Participants = new List<ArenaParticipant>();

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;

                    m_Participants.Add(participant);
                }
            }

            return m_Participants;
        }

        public void BroadcastMessage(string message, int hue)
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;

                    participant.m_Player.SendMessage(hue, message);
                }
            }
        }

        public void ParticipantsForceGumpUpdate()
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;
                    
                    if (participant.m_Player.HasGump(typeof(ArenaGump)) && participant.m_Player.m_ArenaGumpObject != null)
                    {
                        participant.m_Player.CloseGump(typeof(ArenaGump));
                        participant.m_Player.SendGump(new ArenaGump(participant.m_Player, participant.m_Player.m_ArenaGumpObject));
                    } 
                }
            }
        }
                
        public void LeaveMatch(PlayerMobile player, bool broadcast, bool gumpUpdateParticipants)
        {
            if (!IsValidArenaMatch(this, null, false))
                return;

            if (player == null)
                return;

            string playerName = player.RawName;
            
            ArenaTeam playerTeam = null;
            ArenaParticipant playerParticipant = null;
           
            foreach (ArenaTeam team in m_Teams)
            {
                if (team == null) continue;
                if (team.Deleted) continue;
               
                foreach(ArenaParticipant participant in team.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;

                    if (participant.m_Player == player)
                    {
                        playerTeam = team;
                        playerParticipant = participant;

                        break;
                    }
                }                
            }
            
            if (playerTeam != null && playerParticipant != null)
            {
                if (playerTeam.m_Participants.Contains(playerParticipant))
                    playerTeam.m_Participants.Remove(playerParticipant);

                playerParticipant.Delete();

                player.m_ArenaPlayerSettings.m_ArenaMatch = null;
            }

            if (player == m_Creator)            
                CancelMatch();            

            else
            {
                if (broadcast)
                    BroadcastMessage(playerName + " has left the match.", 0);
            }

            if (gumpUpdateParticipants)
                ParticipantsForceGumpUpdate();
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
            if (m_MatchStatus != MatchStatusType.Listed) 
                return false;            

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
                    
                    if (participant.m_ReadyToggled)
                        readyPlayers++;
                }

                if (readyPlayers >= playersNeededPerTeam)
                    fullTeams++;
            }

            if (fullTeams < ArenaGroupController.TeamsRequired)
                return false;

            return true;
        }

        public void CancelMatch()
        {
            BroadcastMessage("The current arena match you were in has been canceled.", 1256);

            List<PlayerMobile> m_Players = new List<PlayerMobile>();
                        
            Queue m_TeamsQueue = new Queue();
            Queue m_ParticipantQueue = new Queue();

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null)
                    continue;

                m_TeamsQueue.Enqueue(arenaTeam);

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) 
                        continue;                    

                    m_ParticipantQueue.Enqueue(participant);

                    if (participant.m_Player != null)
                        m_Players.Add(participant.m_Player);

                    if (participant.m_Player != null)
                        participant.m_Player.m_ArenaPlayerSettings.m_ArenaMatch = null;
                }
            }

            while (m_ParticipantQueue.Count > 0)
            {
                ArenaParticipant arenaParticipant = (ArenaParticipant)m_ParticipantQueue.Dequeue();
                
                arenaParticipant.Delete();
            }

            while (m_TeamsQueue.Count > 0)
            {
                ArenaTeam arenaTeam = (ArenaTeam)m_TeamsQueue.Dequeue();

                arenaTeam.Delete();
            }            

            Delete();

            foreach (PlayerMobile player in m_Players)
            {
                if (player.HasGump(typeof(ArenaGump)) && player.m_ArenaGumpObject != null)
                {
                    player.CloseGump(typeof(ArenaGump));
                    player.SendGump(new ArenaGump(player, player.m_ArenaGumpObject));
                }
            }
        }

        public static bool IsValidArenaMatch(ArenaMatch arenaMatch, PlayerMobile player, bool checkIfPlayerCanJoin)
        {
            if (arenaMatch == null) return false;
            if (arenaMatch.Deleted) return false;
            if (arenaMatch.m_ArenaGroupController == null) return false;
            if (arenaMatch.m_ArenaGroupController.Deleted) return false; 
            if (arenaMatch.m_Ruleset == null) return false;
            if (arenaMatch.m_Ruleset.Deleted) return false;

            if (checkIfPlayerCanJoin)
            {
                if (!arenaMatch.CanPlayerJoinMatch(player))
                    return false;
            }

            ArenaTeam arenaTeam1 = arenaMatch.GetTeam(0);
            ArenaTeam arenaTeam2 = arenaMatch.GetTeam(1);

            if (arenaTeam1 == null) return false;
            if (arenaTeam1.Deleted) return false;
            if (arenaTeam2 == null) return false;
            if (arenaTeam2.Deleted) return false;

            return true;
        }

        public override void OnDelete()
        {
            Queue m_TeamsQueue = new Queue();
            Queue m_ParticipantQueue = new Queue();

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null)
                    continue;

                m_TeamsQueue.Enqueue(arenaTeam);

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null)
                        continue;

                    m_ParticipantQueue.Enqueue(participant);

                    if (participant.m_Player != null)
                        participant.m_Player.m_ArenaPlayerSettings.m_ArenaMatch = null;
                }
            }

            while (m_ParticipantQueue.Count > 0)
            {
                ArenaParticipant arenaParticipant = (ArenaParticipant)m_ParticipantQueue.Dequeue();

                arenaParticipant.Delete();
            }

            while (m_TeamsQueue.Count > 0)
            {
                ArenaTeam arenaTeam = (ArenaTeam)m_TeamsQueue.Dequeue();

                arenaTeam.Delete();
            }

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