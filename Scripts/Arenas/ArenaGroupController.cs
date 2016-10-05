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

        public Rectangle2D m_ArenaGroupRegionBoundary;
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D ArenaGroupRegionBoundary
        {
            get { return m_ArenaGroupRegionBoundary; }
            set { m_ArenaGroupRegionBoundary = value; }
        }

        public bool m_Enabled;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;

                if (m_Enabled)
                {
                    if (m_Timer == null)
                    {
                        m_Timer = new ArenaGroupControllerTimer(this);
                        m_Timer.Start();
                    }

                    else
                        m_Timer.Start();
                }

                else
                {
                    if (m_Timer != null)                    
                        m_Timer.Stop();                    
                }
            }
        }

        [Constructable]
        public ArenaGroupController(): base(3804)
        {
            Name = "arena group controller";

            Movable = false;

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
                if (arenaController == null) continue;
                if (arenaController.Deleted) continue;
                if (!arenaController.Enabled) continue;

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
                    if (arenaMatch.m_Creator.Guild != null && arenaMatch.m_Creator.Guild != player.Guild)
                        continue;
                }

                if (arenaMatch.m_Ruleset.m_ListingMode == ArenaRuleset.ListingModeType.PartyOnly)
                {
                    if ( arenaMatch.m_Creator.Party != null && arenaMatch.m_Creator.Party != player.Party) 
                        continue;
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

                if (!m_ArenaGroupController.Enabled)
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
                        if (!ArenaMatch.IsValidArenaMatch(arenaMatch, null, false)) continue;                        
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

                                ArenaRuleset.ArenaRulesetFailureType rulesetFailure = arenaMatch.m_Ruleset.CheckForRulesetViolations(arenaMatch, arenaParticipant.m_Player);

                                m_RulesetViolations.Add(new KeyValuePair<PlayerMobile, ArenaRuleset.ArenaRulesetFailureType>(arenaParticipant.m_Player, rulesetFailure));

                                if (rulesetFailure != ArenaRuleset.ArenaRulesetFailureType.None)
                                    rulesetViolationExists = true;
                            }
                        }

                        if (rulesetViolationExists)
                        {
                            arenaMatch.BroadcastMessage("Unable to start match due to the following ruleset violations:", 0);

                            bool arenaError = false;

                            foreach (KeyValuePair<PlayerMobile, ArenaRuleset.ArenaRulesetFailureType> rulesetViolation in m_RulesetViolations)
                            {
                                string message = "";

                                PlayerMobile player = rulesetViolation.Key;
                                ArenaRuleset.ArenaRulesetFailureType rule = rulesetViolation.Value;

                                if (player == null)
                                    continue;

                                switch (rulesetViolation.Value)
                                {
                                    case ArenaRuleset.ArenaRulesetFailureType.ArenaInvalid: arenaError = true; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.Dead: message = player.RawName + ": Not Alive"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.EquipmentAllowed: message = player.RawName + ": Has Restricted Equipment (worn or in backpack)"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.Follower: message = player.RawName + ": Exceeds Follower Control Slots Allowed"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.Mount: message = player.RawName + ": Mounts Not Allowed"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.NotInArenaRegion: message = player.RawName + ": Outside of Arena Region"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.NotOnline: message = player.RawName + ": Not Online"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.PoisonedWeapon:message = player.RawName + ": Exceeds Allowed Number of Poisoned Weapons"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.Transformed: message = player.RawName + ": Transformed or Disguised"; break;
                                    case ArenaRuleset.ArenaRulesetFailureType.Young: message = player.RawName + ": Young Status"; break;
                                }

                                if (message != "")
                                    arenaMatch.BroadcastMessage(message, 1256);
                            }

                            if (arenaError)
                                arenaMatch.BroadcastMessage("Arena Configuration Error", 1256);

                            arenaMatch.m_NextReadyCheck = DateTime.UtcNow + ArenaMatch.ReadyCheckInterval;

                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, arenaMatch.m_NextReadyCheck, false, true, true, true, true);

                            arenaMatch.BroadcastMessage("Match will re-attempt to start in " + timeRemaining + ".", 0);                            
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

        public override void OnDelete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(Enabled);
            writer.Write(ArenaGroupRegionBoundary);           

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
                Enabled = reader.ReadBool();
                ArenaGroupRegionBoundary = reader.ReadRect2D();

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