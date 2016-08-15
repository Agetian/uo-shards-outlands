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
                        
            ArenaGumpObject arenaGumpObject = new ArenaGumpObject(player, this);
            
            player.SendSound(0x055);

            player.CloseGump(typeof(ArenaGump));
            player.SendGump(new ArenaGump(player, arenaGumpObject));
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

        public List<ArenaMatch> GetArenaMatches(PlayerMobile player)
        {
            List<ArenaMatch> m_FilteredMatches = new List<ArenaMatch>();

            foreach (ArenaMatch arenaMatch in m_MatchListings)
            {
                if (arenaMatch == null) continue;
                if (arenaMatch.Deleted) continue;

                if (arenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed) continue;
                if (arenaMatch.m_Ruleset == null) continue;
                if (arenaMatch.m_Ruleset.Deleted) continue;
                if (arenaMatch.m_Creator == null) continue;

                if (arenaMatch.m_Creator == player)
                {
                    m_FilteredMatches.Add(arenaMatch);
                    continue;
                }

                if (arenaMatch.m_Ruleset.m_ListingMode == ArenaRuleset.ListingModeType.GuildOnly)
                {
                    if (arenaMatch.m_Creator.Party == null) continue;
                    if (arenaMatch.m_Creator.Party != player.Party) continue;
                }

                if (arenaMatch.m_Ruleset.m_ListingMode == ArenaRuleset.ListingModeType.PartyOnly)
                {
                    if (arenaMatch.m_Creator.Guild == null) continue;
                    if (arenaMatch.m_Creator.Guild != player.Guild) continue;
                }

                m_FilteredMatches.Add(arenaMatch);
            }

            return m_FilteredMatches;
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