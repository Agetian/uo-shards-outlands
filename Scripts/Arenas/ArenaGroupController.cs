using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Linq;

namespace Server
{
    public class ArenaGroupController : Item
    {
        public static List<ArenaGroupController> m_Instances = new List<ArenaGroupController>();

        //-----

        public List<ArenaController> m_Arenas
        {
            get
            {
                List<ArenaController> arenaControllers = new List<ArenaController>();

                foreach (ArenaController arenaController in ArenaController.m_Instances)
                {
                    if (arenaController == null) continue;
                    if (arenaController.Deleted) continue;

                    if (arenaController.m_ArenaGroupController == this)
                        arenaControllers.Add(arenaController);
                }

                return arenaControllers;
            }
        }

        public List<ArenaMatch> m_MatchListings = new List<ArenaMatch>();
        public List<ArenaTournament> m_Tournaments = new List<ArenaTournament>();        
        
        public DateTime m_NextListingAudit = DateTime.UtcNow;

        private Timer m_Timer;        

        public static int TeamsRequired = 2;        
        public static TimeSpan AuditInterval = TimeSpan.FromMinutes(5);
        public static TimeSpan LoggedOutPlayerTimeoutThreshold = TimeSpan.FromMinutes(10);

        [Constructable]
        public ArenaGroupController(): base(3804)
        {
            Name = "arena group controller";

            Movable = false;

            m_Timer = new ArenaGroupControllerTimer(this);
            m_Timer.Start();

            //-----

            m_Instances.Add(this);
        }

        public ArenaGroupController(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            //TEST: REPLACE WITH GUMP
            if (m_MatchListings.Count == 0)
            {
                ArenaMatch arenaMatch = new ArenaMatch(this, player);
                
                ArenaTeam team = new ArenaTeam();
                ArenaParticipant participant = new ArenaParticipant(player);

                team.m_Participants.Add(participant);
                arenaMatch.m_Teams.Add(team);
                
                arenaMatch.m_MatchStatus = ArenaMatch.MatchStatusType.Listed;

                m_MatchListings.Add(arenaMatch);
            }

            else
            {
                ArenaMatch arenaMatch = m_MatchListings[0];

                bool foundPlayer = false;

                foreach (ArenaTeam team in arenaMatch.m_Teams)
                {
                    foreach (ArenaParticipant participant in team.m_Participants)
                    {
                        if (participant.m_Player == player)
                            foundPlayer = true;
                    }
                }

                if (foundPlayer)
                    return;

                ArenaTeam newTeam = new ArenaTeam();
                ArenaParticipant newParticipant = new ArenaParticipant(player);

                newTeam.m_Participants.Add(newParticipant);
                arenaMatch.m_Teams.Add(newTeam);

                arenaMatch.m_CreatorReady = true;
            }
        }

        public void AuditListings()
        {
            m_NextListingAudit = DateTime.UtcNow + AuditInterval;
        }

        public ArenaController GetAvailableArena()
        {
            ArenaController arena = null;

            foreach (ArenaController arenaController in m_Arenas)
            {
                if (arenaController == null)
                    continue;

                if (!arenaController.Enabled)
                    continue;

                if (arenaController.m_ArenaFight == null)
                    return arenaController;

                if (arenaController.m_ArenaFight.Deleted)
                    return arenaController;
            }

            return arena;
        }

        public class ArenaGroupControllerTimer : Timer
        {
            public ArenaGroupController m_ArenaGroupController;

            public ArenaGroupControllerTimer(ArenaGroupController arenaGroupController): base(TimeSpan.Zero, TimeSpan.FromSeconds(3))
            {
                m_ArenaGroupController = arenaGroupController;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_ArenaGroupController == null)
                {
                    Stop();
                    return;
                }

                if (m_ArenaGroupController.Deleted)
                {
                    Stop();
                    return;        
                }
               
                if (DateTime.UtcNow >= m_ArenaGroupController.m_NextListingAudit)
                    m_ArenaGroupController.AuditListings();                

                ArenaController emptyArena = m_ArenaGroupController.GetAvailableArena();

                if (emptyArena != null)
                {
                    foreach (ArenaMatch arenaMatch in m_ArenaGroupController.m_MatchListings)
                    {
                        if (arenaMatch == null) continue;
                        if (arenaMatch.Deleted) continue;

                        if (DateTime.UtcNow < arenaMatch.m_NextReadyCheck) continue;
                        if (!arenaMatch.IsReadyToStart()) continue;

                        List<KeyValuePair<PlayerMobile, ArenaRuleset.ArenaRulesetFailureType>> m_RulesetViolations = new List<KeyValuePair<PlayerMobile, ArenaRuleset.ArenaRulesetFailureType>>();

                        bool rulesetViolationExists = false;

                        foreach (ArenaTeam arenaTeam in arenaMatch.m_Teams)
                        {
                            if (arenaTeam == null) continue;
                            if (arenaTeam.Deleted) continue;

                            foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                            {
                                if (arenaParticipant == null) continue;
                                if (arenaParticipant.Deleted) continue;
                                if (arenaParticipant.m_Player == null) continue;
                                if (arenaParticipant.Deleted == null) continue;

                                if (arenaMatch.m_Ruleset == null) continue;

                                ArenaRuleset.ArenaRulesetFailureType rulesetFailure = arenaMatch.m_Ruleset.CheckForRulesetViolations(arenaParticipant.m_Player);

                                m_RulesetViolations.Add(new KeyValuePair<PlayerMobile, ArenaRuleset.ArenaRulesetFailureType>(arenaParticipant.m_Player, rulesetFailure));

                                if (rulesetFailure != ArenaRuleset.ArenaRulesetFailureType.None)
                                    rulesetViolationExists = true;
                            }
                        }

                        if (rulesetViolationExists)
                        {
                            //TEST: SEND VIOLATION GUMP TO ALL TEAM MEMBERS OF EACH TIME                            

                            arenaMatch.m_NextReadyCheck = DateTime.UtcNow + ArenaMatch.ReadyCheckInterval;
                        }

                        else
                        {
                            arenaMatch.m_MatchStatus = ArenaMatch.MatchStatusType.Fighting;

                            ArenaFight arenaFight = new ArenaFight();                            

                            arenaFight.m_ArenaController = emptyArena;
                            arenaFight.m_ArenaMatch = arenaMatch;

                            for (int a = 0; a < arenaMatch.m_Teams.Count; a++)
                            {
                                arenaFight.m_Teams.Add(arenaMatch.m_Teams[a]);
                            }

                            emptyArena.m_ArenaFight = arenaFight;

                            arenaFight.Initialize();
                        }
                    }
                }                
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_MatchListings.Count);
            for (int a = 0; a < m_MatchListings.Count; a++)
            {
                writer.Write(m_MatchListings[a]);
            }

            writer.Write(m_Tournaments.Count);
            for (int a = 0; a < m_Tournaments.Count; a++)
            {
                writer.Write(m_Tournaments[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                int matchListingsCount = reader.ReadInt();
                for (int a = 0; a < matchListingsCount; a++)
                {
                    m_MatchListings.Add(reader.ReadItem() as ArenaMatch);
                }

                int tournamentsCount = reader.ReadInt();
                for (int a = 0; a < tournamentsCount; a++)
                {
                    m_Tournaments.Add(reader.ReadItem() as ArenaTournament);
                }
            }

            //-----

            m_Instances.Add(this);

            m_Timer = new ArenaGroupControllerTimer(this);
            m_Timer.Start();
        }
    }
}

#region OLD VERSION

/*
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;
            
            ArenaParticipant playerParticipant = GetPlayerParticipant(player);

            if (playerParticipant == null)
            {
                playerParticipant = new ArenaParticipant(player);

                if (player.m_CompetitionContext != null)
                    player.m_CompetitionContext.Delete();

                player.m_CompetitionContext = new CompetitionContext();
                player.m_CompetitionContext.m_ArenaParticipant = playerParticipant;

                ArenaTeam arenaTeam = new ArenaTeam();

                arenaTeam.m_Participants.Add(playerParticipant);
                m_Teams.Add(arenaTeam);

                playerParticipant.m_ArenaTeam = arenaTeam;
                playerParticipant.m_ArenaGroupController = this;

                //TEST
                player.Say("Joining the Arena Queue");
            }            
        }
        
        public ArenaParticipant GetPlayerParticipant(PlayerMobile player)
        {            
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                ArenaParticipant arenaParticipant = arenaTeam.GetPlayerParticipant(player);

                return arenaParticipant;
            }

            return null;             
        }

        public List<ArenaTeam> GetReadyTeams()
        {
            List<ArenaTeam> m_ReadyTeams = new List<ArenaTeam>();

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                if (arenaTeam.IsReadyForEvent())
                    m_ReadyTeams.Add(arenaTeam);
            }

            return m_ReadyTeams;
        }

        public ArenaController GetAvailableArena()
        {
            ArenaController arena = null;

            foreach (ArenaController arenaController in m_Arenas)
            {
                if (arenaController == null) 
                    continue;

                if (!arenaController.Enabled)
                    continue;

                if (arenaController.m_ArenaFight == null)
                    return arenaController;

                if (arenaController.m_ArenaFight.Deleted)
                    return arenaController;
            }

            return arena;
        }

        public void StartMatch()
        {
            ArenaController arenaController = GetAvailableArena();
            List<ArenaTeam> m_ReadyTeams = GetReadyTeams();

            if (arenaController == null) return;
            if (m_ReadyTeams.Count < TeamsRequired) return;

            Dictionary<ArenaTeam, double> dictReadyTeams = new Dictionary<ArenaTeam,double>();

            foreach(ArenaTeam arenaTeam in m_ReadyTeams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                dictReadyTeams.Add(arenaTeam, (DateTime.UtcNow - arenaTeam.m_LastEventTime).TotalMinutes);
            }

            List<KeyValuePair<ArenaTeam, double>> m_SortedTeams = new List<KeyValuePair<ArenaTeam, double>>();

            foreach (KeyValuePair<ArenaTeam, double> pair in dictReadyTeams.OrderByDescending(key => key.Value))
            {
                m_SortedTeams.Add(pair);
            }

            List<ArenaTeam> m_SelectedTeams = new List<ArenaTeam>();

            for (int a = 0; a < TeamsRequired; a++)
            {
                ArenaTeam arenaTeam = m_ReadyTeams[a];

                arenaTeam.m_LastEventTime = DateTime.UtcNow;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;

                    arenaParticipant.m_EventStatus = ArenaParticipant.EventStatusType.Playing;
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Alive;

                    arenaParticipant.ResetArenaFightValues();
                }

                m_SelectedTeams.Add(arenaTeam);
            }

            ArenaFight arenaFight = new ArenaFight(arenaController, m_SelectedTeams);

            arenaController.m_ArenaFight = arenaFight;
        }

        public void MatchComplete(ArenaFight arenaFight)
        {
            //Remove Player from System After Match
            if (RemoveFromQueueAfterMatch && arenaFight != null)
            {
                Queue m_Queue = new Queue();

                foreach (ArenaTeam arenaTeam in arenaFight.m_Teams)
                {
                    if (arenaTeam == null) continue;
                    if (arenaTeam.Deleted) continue;

                    foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                    {
                        if (arenaParticipant == null) continue;
                        if (arenaParticipant.Deleted) continue;

                        m_Queue.Enqueue(arenaParticipant);
                    }
                }

                while (m_Queue.Count > 0)
                {
                    ArenaParticipant arenaParticipant = (ArenaParticipant)m_Queue.Dequeue();

                    if (arenaParticipant.m_Player != null)
                        PlayerQuit(arenaParticipant.m_Player);
                }                
            }

            if (arenaFight != null)
                arenaFight.Delete();
        }

        public void AuditParticipants()
        {
            Queue m_Queue = new Queue();

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;

                    if (arenaParticipant.m_EventStatus == ArenaParticipant.EventStatusType.Inactive && DateTime.UtcNow >= arenaParticipant.m_LastEventTime + LoggedOutPlayerTimeoutThreshold)
                        m_Queue.Enqueue(arenaParticipant);
                }
            }

            while (m_Queue.Count > 0)
            {
                ArenaParticipant arenaParticipant = (ArenaParticipant)m_Queue.Dequeue();

                if (arenaParticipant.m_Player != null)
                    PlayerQuit(arenaParticipant.m_Player);
            }

            m_NextParticipantAudit = DateTime.UtcNow + ParticipantAuditInterval;
        }

        public void PlayerQuit(PlayerMobile player)
        {
            if (player == null) 
                return;

            ArenaParticipant arenaParticipant = GetPlayerParticipant(player);

            if (arenaParticipant == null)
                return;

            if (player.m_CompetitionContext != null)
            {
                player.m_CompetitionContext.Delete();
                player.m_CompetitionContext = null;
            }

            if (arenaParticipant.m_ArenaTeam != null)
            {
                if (arenaParticipant.m_ArenaTeam.m_Participants.Contains(arenaParticipant))
                    arenaParticipant.m_ArenaTeam.m_Participants.Remove(arenaParticipant);

                if (arenaParticipant.m_ArenaTeam.m_Participants.Count == 0)
                {
                    if (m_Teams.Contains(arenaParticipant.m_ArenaTeam))
                        m_Teams.Remove(arenaParticipant.m_ArenaTeam);

                    arenaParticipant.m_ArenaTeam.Delete();
                }                
            }

            arenaParticipant.Delete();
        }
        */

#endregion