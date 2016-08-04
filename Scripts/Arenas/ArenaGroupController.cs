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

        public List<ArenaTeam> m_Teams = new List<ArenaTeam>();

        private Timer m_Timer;

        public static int TeamsRequired = 2;        

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

            ArenaParticipant playerParticipant = GetPlayerParticipant(player);

            if (playerParticipant == null)
            {
                playerParticipant = new ArenaParticipant(player);

                ArenaTeam arenaTeam = new ArenaTeam();

                arenaTeam.m_Participants.Add(playerParticipant);
                m_Teams.Add(arenaTeam);

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
                    arenaParticipant.ResetArenaFightValues();
                }

                m_SelectedTeams.Add(arenaTeam);
            }

            ArenaFight arenaFight = new ArenaFight(arenaController, m_SelectedTeams);

            arenaController.m_ArenaFight = arenaFight;
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

                if (m_ArenaGroupController.GetReadyTeams().Count >= ArenaGroupController.TeamsRequired)
                {
                    ArenaController emptyArena = m_ArenaGroupController.GetAvailableArena();

                    if (emptyArena != null)                    
                        m_ArenaGroupController.StartMatch();                    
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
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
                int teamsCount = reader.ReadInt();
                for (int a = 0; a < teamsCount; a++)
                {
                    m_Teams.Add(reader.ReadItem() as ArenaTeam);
                }
            }

            //-----

            m_Instances.Add(this);

            m_Timer = new ArenaGroupControllerTimer(this);
            m_Timer.Start();
        }
    }
}