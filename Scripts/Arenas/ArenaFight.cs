using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public class ArenaFight : Item
    {
        public enum FightPhaseType
        {
            StartCountdown,
            Fight,
            PostBattle,
            Completed
        }

        public ArenaController m_ArenaController;

        public FightPhaseType m_FightPhase = FightPhaseType.StartCountdown;
        public TimeSpan m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);

        public List<ArenaTeam> m_Teams = new List<ArenaTeam>();

        public static TimeSpan TimerTickDuration = TimeSpan.FromSeconds(1);

        public Timer m_Timer;

        //----

        public CompetitionContext m_CompetitionContext;

        [Constructable]
        public ArenaFight(ArenaController arenaController, List<ArenaTeam> teams): base(0x0)
        {
            m_ArenaController = arenaController;
            m_Teams = teams;

            Visible = false;
            Movable = false;

            m_Timer = new ArenaFightTimer(this);
            m_Timer.Start();
        }

        public ArenaFight(Serial serial) : base(serial)
        {
        }

        public ArenaParticipant GetParticipant(PlayerMobile player)
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant.m_Player == player)
                        return arenaParticipant;
                }
            }

            return null;
        }

        public void OnPlayerDeath(PlayerMobile player)
        {
            ArenaParticipant arenaParticipant = GetParticipant(player);

            if (arenaParticipant != null)            
                arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Dead;

            if (!CheckTeamsRemaining())
                StartPostBattle();
        }

        public void OnPlayerLogoutOrDisconnection(PlayerMobile player)
        {
            ArenaParticipant arenaParticipant = GetParticipant(player);

            if (arenaParticipant != null)
                arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Disqualified;

            if (!CheckTeamsRemaining())
                StartPostBattle();
        }

        public bool CheckTeamsRemaining()
        {
            int teamCount = m_Teams.Count;
            int teamsRemaining = teamCount;
            
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null)
                {
                    teamsRemaining--;
                    continue;
                }

                if (arenaTeam.Deleted)
                {
                    teamsRemaining--;
                    continue;
                }

                int participantCount = arenaTeam.m_Participants.Count;
                int playersEliminated = 0;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null)
                    {
                        playersEliminated++;
                        continue;
                    }

                    if (arenaParticipant.Deleted)
                    {
                        playersEliminated++;
                        continue;
                    }

                    if (arenaParticipant.m_Player == null)
                    {
                        playersEliminated++;
                        continue;
                    }

                    if (arenaParticipant.m_Player.Deleted)
                    {
                        playersEliminated++;
                        continue;
                    }

                    if (!arenaParticipant.m_Player.Alive)
                        playersEliminated++;

                    //TEST: USE THIS EVENTUALLY
                    //if (arenaParticipant.m_FightStatus != ArenaParticipant.FightStatusType.Alive)
                        //playersEliminated++;
                }

                if (playersEliminated >= participantCount)
                    teamsRemaining--;
            }

            if (teamsRemaining <= 1)
                return false;

            return true;
        }

        public void StartCountdown()
        {
            m_FightPhase = FightPhaseType.StartCountdown;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);
        }

        public void StartFight()
        {
            m_FightPhase = FightPhaseType.Fight;
            m_PhaseTimeRemaining = TimeSpan.FromDays(1);
        }

        public void StartPostBattle()
        {
            m_FightPhase = FightPhaseType.PostBattle;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(30);
        }

        public void FightCompleted()
        {
            m_FightPhase = FightPhaseType.Completed;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(5);
        }

        public void ClearFight()
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                arenaTeam.m_LastEventTime = DateTime.UtcNow;

                foreach(ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;

                    arenaParticipant.m_EventStatus = ArenaParticipant.EventStatusType.Waiting;

                    //TEST: Record Player Fight Data
                }
            }

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Delete();
        }

        public class ArenaFightTimer : Timer
        {
            public ArenaFight m_ArenaFight;

            public ArenaFightTimer(ArenaFight arenaFight): base(TimeSpan.Zero, ArenaFight.TimerTickDuration)
            {
                m_ArenaFight = arenaFight;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (m_ArenaFight == null)
                {
                    Stop();
                    return;
                }

                if (m_ArenaFight.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_ArenaFight.m_ArenaController == null)
                {
                    Stop();
                    return;
                }

                if (m_ArenaFight.m_ArenaController.Deleted)
                {
                    Stop();
                    return;
                }

                m_ArenaFight.m_PhaseTimeRemaining = m_ArenaFight.m_PhaseTimeRemaining.Subtract(ArenaFight.TimerTickDuration); 

                switch (m_ArenaFight.m_FightPhase)
                {
                    case FightPhaseType.StartCountdown:
                        m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Start Countdown");

                        if (m_ArenaFight.m_PhaseTimeRemaining.TotalSeconds <= 0)
                        {
                            m_ArenaFight.StartFight();
                            return;
                        }
                    break;

                    case FightPhaseType.Fight:
                        m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Fight");

                        if (!m_ArenaFight.CheckTeamsRemaining())
                        {
                            m_ArenaFight.StartPostBattle();
                            return;
                        }
                    break;

                    case FightPhaseType.PostBattle:
                        m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "PostBattle");

                        if (m_ArenaFight.m_PhaseTimeRemaining.TotalSeconds <= 0)
                        {
                            m_ArenaFight.FightCompleted();
                            return;
                        }
                    break;

                    case FightPhaseType.Completed:
                        m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Completed");

                        if (m_ArenaFight.m_PhaseTimeRemaining.TotalSeconds <= 0)
                        {
                            m_ArenaFight.ClearFight();
                            return;
                        }                       
                    break;
                }                
            }
        }

        public override void OnDelete()
        {
            if (m_CompetitionContext != null)
                m_CompetitionContext.Delete();

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaController);
            writer.Write((int)m_FightPhase);
            writer.Write(m_PhaseTimeRemaining);

            writer.Write(m_Teams.Count);
            for (int a = 0; a < m_Teams.Count; a++)
            {
                writer.Write(m_Teams[a]);
            }

            writer.Write(m_CompetitionContext);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaController = (ArenaController)reader.ReadItem();
                m_FightPhase = (FightPhaseType)reader.ReadInt();
                m_PhaseTimeRemaining = reader.ReadTimeSpan();

                int teamsCount = reader.ReadInt();
                for (int a = 0; a < teamsCount; a++)
                {
                    m_Teams.Add(reader.ReadItem() as ArenaTeam);
                }

                m_CompetitionContext = reader.ReadItem() as CompetitionContext;
            }

            //-----

            m_Timer = new ArenaFightTimer(this);
            m_Timer.Start();
        }
    }
}