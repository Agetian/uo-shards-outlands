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
using Server.Targeting;

namespace Server
{
    public class ArenaFight : Item
    {
        public enum FightPhaseType
        {
            StartCountdown,
            Fight,
            PostBattle
        }

        public enum VictoryType
        {
            None,
            PlayersEliminated,
            SuddenDeathHitPointsRemaining,
            SuddenDeathDamageDealt,
            SuddenDeathDamageReceived,
            Randomized
        }

        public ArenaController m_ArenaController;

        public ArenaMatch m_ArenaMatch;
        public FightPhaseType m_FightPhase = FightPhaseType.StartCountdown;
        public TimeSpan m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);
        public TimeSpan m_RoundTimeRemaining = TimeSpan.FromMinutes(3);

        public bool m_SuddenDeath = false;
        public int m_SuddenDeathTickCounter = 0;
        public TimeSpan m_SuddenDeathTimeRemaining = TimeSpan.FromMinutes(3);
        
        public static TimeSpan TimerTickDuration = TimeSpan.FromSeconds(1);

        public Timer m_Timer;
        
        //----
        
        [Constructable]
        public ArenaFight(): base(0x0)
        {
            m_Timer = new ArenaFightTimer(this);
            m_Timer.Start();

            Visible = false;
            Movable = false;
        }

        public ArenaFight(Serial serial) : base(serial)
        {
        }

        #region OnEvents

        public virtual void AnnouncerMessage(string message, int hue)
        {
            if (m_ArenaController == null) return;
            if (m_ArenaController.Deleted) return;

            foreach (ArenaAnnouncer arenaAnnouncer in ArenaAnnouncer.m_Instances)
            {
                if (arenaAnnouncer == null) continue;
                if (arenaAnnouncer.Deleted) continue;

                if (arenaAnnouncer.m_ArenaController != m_ArenaMatch.m_ArenaFight.m_ArenaController)
                    continue;

                arenaAnnouncer.PublicOverheadMessage(MessageType.Regular, hue, false, message);
            }
        }

        public virtual void OnMapChanged(PlayerMobile player)
        {
            OnLocationChanged(player);
        }

        public virtual void OnLocationChanged(PlayerMobile player)
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false)) return;
            if (player == null) return;

            ArenaParticipant arenaParticipant = m_ArenaMatch.GetParticipant(player);

            if (arenaParticipant != null && m_ArenaController != null)
            {
                if (arenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive && !IsWithinArena(player.Location, player.Map))
                {
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Eliminated;

                    ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                    if (player.Map == Map.Internal)
                    {
                        if (exitTile != null)
                        {
                            player.LogoutLocation = exitTile.Location;
                            player.LogoutMap = exitTile.Map;
                        }

                        else
                        {
                            player.LogoutLocation = m_ArenaController.Location;
                            player.LogoutMap = m_ArenaController.Map;
                        }
                    }

                    RestoreAndClearEffects(player);                    

                    foreach (Mobile mobile in player.AllFollowers)
                    {
                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature == null) continue;
                        if (bc_Creature.Deleted) continue;
                        if (!m_ArenaMatch.m_ArenaFight.IsWithinArena(bc_Creature.Location, bc_Creature.Map)) continue;

                        if ((!bc_Creature.Alive || bc_Creature.IsDeadFollower) && !bc_Creature.IsBonded)
                            continue;

                        if (exitTile != null)
                        {
                            bc_Creature.Location = exitTile.Location;
                            bc_Creature.Map = exitTile.Map;
                        }

                        else
                        {
                            bc_Creature.Location = m_ArenaController.Location;
                            bc_Creature.Map = m_ArenaController.Map;
                        }

                        if (bc_Creature.IsDeadBondedFollower)
                            bc_Creature.ResurrectPet();

                        RestoreAndClearEffects(bc_Creature);
                    }

                    ArenaTeam winningTeam = CheckForTeamVictory();

                    if (winningTeam != null)
                    {
                        StartPostBattle(winningTeam, VictoryType.PlayersEliminated);
                        return;
                    }

                    else
                        InvalidatePlayers();
                }                
            }
        }

        public virtual bool IsWithinArena(Point3D location, Map map)
        {
            if (m_ArenaController == null)
                return false;

            return m_ArenaController.IsWithinArena(location, map);
        }

        public virtual void FollowerOnDeath(BaseCreature creature, Container corpseContainer)
        {
            if (creature == null)
                return;

            ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

            if (IsWithinArena(creature.Location, creature.Map))
            {
                if (exitTile != null)
                {
                    creature.Location = exitTile.Location;
                    creature.Map = exitTile.Map;
                }

                else
                {
                    creature.Location = m_ArenaController.Location;
                    creature.Map = m_ArenaController.Map;
                }
            }

            creature.ControlTarget = null;
            creature.ControlOrder = OrderType.Stop;

            Timer.DelayCall(TimeSpan.FromSeconds(4.0), delegate
            {
                if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false)) return;
                if (creature == null) return;
                if (m_ArenaController == null) return;

                if (creature.IsDeadBondedFollower)
                    creature.ResurrectPet();

                RestoreAndClearEffects(creature);

                InvalidatePlayers();
            });
        }
        
        public virtual void OnDeath(PlayerMobile player, Container corpseContainer)
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false)) return;
            if (player == null)  return;

            ArenaParticipant arenaParticipant = m_ArenaMatch.GetParticipant(player);

            if (arenaParticipant != null)
            {
                if (arenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive)
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Eliminated;
            }

            Queue m_Queue = new Queue();

            foreach (Mobile mobile in player.AllFollowers)
            {
                BaseCreature bc_Creature = mobile as BaseCreature;

                if (bc_Creature == null) continue;
                if (!m_ArenaMatch.m_ArenaFight.IsWithinArena(bc_Creature.Location, bc_Creature.Map)) continue;

                if (bc_Creature.Alive)
                    m_Queue.Enqueue(bc_Creature);
            }

            while (m_Queue.Count > 0)
            {
                BaseCreature creature = (BaseCreature)m_Queue.Dequeue();

                creature.Kill();
            }
            
            Timer.DelayCall(TimeSpan.FromSeconds(4.0), delegate
            {
                if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false)) return;
                if (player == null) return;
                if (m_ArenaController == null) return;

                if (!player.Alive)
                    player.Resurrect(); 

                RestoreAndClearEffects(player);                               
                                
                ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                if (IsWithinArena(player.Location, player.Map))
                {
                    if (exitTile != null)
                    {
                        player.Location = exitTile.Location;
                        player.Map = exitTile.Map;
                    }

                    else
                    {
                        player.Location = m_ArenaController.Location;
                        player.Map = m_ArenaController.Map;
                    }
                }

                foreach (Mobile mobile in player.AllFollowers)
                {
                    BaseCreature bc_Creature = mobile as BaseCreature;

                    if (bc_Creature == null) continue;
                    if (bc_Creature.Deleted) continue;
                    if (!m_ArenaMatch.m_ArenaFight.IsWithinArena(bc_Creature.Location, bc_Creature.Map)) continue;

                    if ((!bc_Creature.Alive || bc_Creature.IsDeadFollower) && !bc_Creature.IsBonded)
                        continue;

                    if (exitTile != null)
                    {
                        bc_Creature.Location = exitTile.Location;
                        bc_Creature.Map = exitTile.Map;
                    }

                    else
                    {
                        bc_Creature.Location = m_ArenaController.Location;
                        bc_Creature.Map = m_ArenaController.Map;
                    }

                    if (bc_Creature.IsDeadBondedFollower)
                        bc_Creature.ResurrectPet();

                    RestoreAndClearEffects(bc_Creature);
                }

                InvalidatePlayers();
            });

            ArenaTeam winningTeam = CheckForTeamVictory();

            if (winningTeam != null)
            {
                StartPostBattle(winningTeam, VictoryType.PlayersEliminated);
                return;
            }

            else
                InvalidatePlayers();
        }

        public virtual bool AllowFreeConsume(PlayerMobile player)
        {
            return true;
        }

        public virtual bool AllowItemEquip(PlayerMobile player, Item item)
        {
            return true;
        }

        public virtual bool AllowItemRemove(PlayerMobile player, Item item)
        {
            return true;
        }

        public virtual bool AllowItemUse(PlayerMobile player, Item item)
        {
            return true;
        }

        public virtual bool AllowSkillUse(PlayerMobile player, SkillName skill)
        {
            return true;
        }

        public virtual bool AllowSpellCast(PlayerMobile player, Spell spell)
        {
            return true;
        }

        public virtual void CancelSpell(Mobile mobile)
        {
            if (mobile == null)
                return;

            if (mobile.Spell is Spell)
            {
                Spell spell = mobile.Spell as Spell;
                spell.Disturb(DisturbType.Kill);
            }
        }
                
        public virtual void RestoreAndClearEffects(Mobile mobile)
        {
            SpecialAbilities.ClearSpecialEffects(mobile);

            mobile.Warmode = false;

            mobile.RemoveStatModsBeginningWith("[Magic]");

            mobile.MagicDamageAbsorb = 0;
            mobile.MeleeDamageAbsorb = 0;
            mobile.VirtualArmorMod = 0;

            BuffInfo.RemoveBuff(mobile, BuffIcon.Agility);
            BuffInfo.RemoveBuff(mobile, BuffIcon.ArchProtection);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Bless);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Clumsy);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Incognito);
            BuffInfo.RemoveBuff(mobile, BuffIcon.MagicReflection);
            BuffInfo.RemoveBuff(mobile, BuffIcon.MassCurse);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Invisibility);
            BuffInfo.RemoveBuff(mobile, BuffIcon.HidingAndOrStealth);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Paralyze);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Poison);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Polymorph);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Protection);
            BuffInfo.RemoveBuff(mobile, BuffIcon.ReactiveArmor);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Strength);
            BuffInfo.RemoveBuff(mobile, BuffIcon.Weaken);
            BuffInfo.RemoveBuff(mobile, BuffIcon.FeebleMind);

            mobile.Paralyzed = false;
            mobile.RevealingAction();

            Spells.Second.ProtectionSpell.Registry.Remove(mobile);
            mobile.EndAction(typeof(DefensiveSpell));

            //TEST
            //TransformationSpellHelper.RemoveContext(mobile, true);

            BaseArmor.ValidateMobile(mobile);
            BaseClothing.ValidateMobile(mobile);

            mobile.Poison = null;

            mobile.ClearAllAggression();

            mobile.Hits = mobile.HitsMax;
            mobile.Stam = mobile.StamMax;
            mobile.Mana = mobile.ManaMax;

            mobile.DropHolding();

            if (mobile.BankBox != null)
                mobile.BankBox.Close();

            mobile.CloseAllGumps();

            if (mobile.NetState != null)
                mobile.NetState.CancelAllTrades();

            CancelSpell(mobile);

            Target.Cancel(mobile);

            BaseCreature bc_Creature = mobile as BaseCreature;

            if (bc_Creature != null)
            {
                if (bc_Creature.ControlMaster is PlayerMobile)
                {
                    bc_Creature.ControlTarget = null;
                    bc_Creature.ControlOrder = OrderType.Stop;
                }
            }        
        }

        #endregion
        
        public ArenaTeam CheckForTeamVictory()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return null;

            List<ArenaTeam> m_TeamsRemaining = new List<ArenaTeam>();

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                m_TeamsRemaining.Add(arenaTeam);
            }            

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                bool teamEliminated = false;

                if (arenaTeam == null)
                    teamEliminated = true;

                else if (arenaTeam.Deleted)
                    teamEliminated = true;

                int activePlayers = 0;

                foreach(ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;

                    if (participant.m_FightStatus == ArenaParticipant.FightStatusType.Alive)
                        activePlayers++;
                }

                if (activePlayers == 0)
                    teamEliminated = true;

                if (teamEliminated && m_TeamsRemaining.Contains(arenaTeam))
                    m_TeamsRemaining.Remove(arenaTeam);                   
            }

            if (m_TeamsRemaining.Count == 1)
                return m_TeamsRemaining[0];

            if (m_TeamsRemaining.Count == 0)
                return new ArenaTeam();

            return null;
        }

        public void ForcedSuddenDeathResolution()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            int team1HitsRemaining = 0;
            int team2HitsRemaining = 0;

            int team1TotalDamageDealt = 0;
            int team2TotalDamageDealt = 0;

            int team1TotalDamageReceived = 0;
            int team2TotalDamageReceived = 0;

            ArenaTeam team1 = m_ArenaMatch.GetTeam(0);
            ArenaTeam team2 = m_ArenaMatch.GetTeam(1);

            foreach (ArenaParticipant arenaParticipant in team1.m_Participants)
            {
                if (arenaParticipant == null) continue;
                if (arenaParticipant.Deleted) continue;
                if (arenaParticipant.m_FightStatus != ArenaParticipant.FightStatusType.Alive) continue;
                if (arenaParticipant.m_Player == null) continue;
                if (!arenaParticipant.m_Player.Alive) continue;

                team1HitsRemaining += arenaParticipant.m_Player.Hits;
                team1TotalDamageDealt += arenaParticipant.m_DamageDealt;
                team1TotalDamageReceived += arenaParticipant.m_DamageReceived;
            }

            foreach (ArenaParticipant arenaParticipant in team2.m_Participants)
            {
                if (arenaParticipant == null) continue;
                if (arenaParticipant.Deleted) continue;
                if (arenaParticipant.m_FightStatus != ArenaParticipant.FightStatusType.Alive) continue;
                if (arenaParticipant.m_Player == null) continue;
                if (!arenaParticipant.m_Player.Alive) continue;

                team2HitsRemaining += arenaParticipant.m_Player.Hits;
                team2TotalDamageDealt += arenaParticipant.m_DamageDealt;
                team2TotalDamageReceived += arenaParticipant.m_DamageReceived;
            }

            ArenaTeam winningTeam = null;
            VictoryType victoryType = VictoryType.None;

            if (team1HitsRemaining > team2HitsRemaining)
            {
                winningTeam = team1;
                victoryType = VictoryType.SuddenDeathHitPointsRemaining;
            }

            if (team2HitsRemaining > team1HitsRemaining)
            {
                winningTeam = team2;
                victoryType = VictoryType.SuddenDeathHitPointsRemaining;
            }

            if (winningTeam == null)
            {
                if (team1TotalDamageDealt > team2TotalDamageDealt)
                {
                    winningTeam = team1;
                    victoryType = VictoryType.SuddenDeathDamageDealt;
                }

                if (team2TotalDamageDealt > team1TotalDamageDealt)
                {
                    winningTeam = team2;
                    victoryType = VictoryType.SuddenDeathDamageDealt;
                }
            }

            if (winningTeam == null)
            {
                if (team1TotalDamageReceived > team2TotalDamageReceived)
                {
                    winningTeam = team1;
                    victoryType = VictoryType.SuddenDeathDamageDealt;
                }

                if (team2TotalDamageReceived > team1TotalDamageReceived)
                {
                    winningTeam = team2;
                    victoryType = VictoryType.SuddenDeathDamageDealt;
                }
            }

            if (winningTeam == null)
            {
                if (Utility.RandomDouble() <= .50)
                    winningTeam = team1;

                else
                    winningTeam = team2;

                victoryType = VictoryType.Randomized;
            }

            StartPostBattle(winningTeam, victoryType);
        }

        public void Initialize()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            //TEST: APPLY RULESET FEATURES

            ArenaRuleset ruleset = m_ArenaMatch.m_Ruleset;

            m_RoundTimeRemaining = ArenaRuleset.GetRoundDuration(ruleset.m_RoundDuration);
            m_SuddenDeathTimeRemaining = ArenaRuleset.GetSuddenDeathDuration(ruleset.m_RoundDuration);
            
            int teamSize = ruleset.TeamSize;
            
            for (int a = 0; a < teamSize; a++)
            {
                ArenaTile wallTile = m_ArenaController.GetWallTile(a);

                ArenaWall wall = new ArenaWall(); 

                if (wallTile != null)
                {
                    switch (wallTile.Facing)
                    {
                        case Server.Direction.West: wall.ItemID = 128; break;
                        case Server.Direction.East: wall.ItemID = 128; break;

                        case Server.Direction.North: wall.ItemID = 128; break;
                        case Server.Direction.South: wall.ItemID = 128; break;
                    }

                    wall.MoveToWorld(wallTile.Location, wallTile.Map);

                    m_ArenaController.m_Walls.Add(wall);
                }

                else
                {
                    //TEST: HANDLING FOR IF WALL TILE IS MISSING
                }
            }

            for (int a = 0; a < m_ArenaMatch.m_Teams.Count; a++)
            {
                ArenaTeam arenaTeam = m_ArenaMatch.m_Teams[a];

                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                for (int b = 0; b < arenaTeam.m_Participants.Count; b++)
                {
                    ArenaParticipant participant = arenaTeam.m_Participants[b];

                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;
                    if (participant.m_Player.Deleted) continue;

                    PlayerMobile player = participant.m_Player;

                    RestoreAndClearEffects(player);                    

                    ArenaTile playerStartingTile = m_ArenaController.GetPlayerStartingTile(a, b);

                    if (playerStartingTile != null)
                    {
                        player.Location = playerStartingTile.Location;
                        player.Map = playerStartingTile.Map;

                        player.Direction = playerStartingTile.Facing;
                    }

                    else
                    {
                        //TEST: SET DEFAULT PLAYER LOCATION (IF TILES MISSING)
                    }

                    for (int c = 0; c < player.AllFollowers.Count; c++)
                    {
                        BaseCreature bc_Creature = player.AllFollowers[c] as BaseCreature;

                        if (bc_Creature == null) continue;
                        if (!m_ArenaMatch.m_ArenaGroupController.ArenaGroupRegionBoundary.Contains(bc_Creature.Location) || m_ArenaMatch.m_ArenaGroupController.Map != bc_Creature.Map) continue;

                        if (bc_Creature.IsDeadBondedFollower)
                            bc_Creature.ResurrectPet();

                        RestoreAndClearEffects(bc_Creature);

                        ArenaTile followerStartingTile = m_ArenaController.GetFollowerStartingTile(a, b, c);

                        if (followerStartingTile != null)
                        {
                            bc_Creature.Location = followerStartingTile.Location;
                            bc_Creature.Map = followerStartingTile.Map;

                            bc_Creature.Direction = followerStartingTile.Facing;
                        }

                        else
                        {
                            //TEST: SET DEFAULT CREATURE LOCATION (IF TILES MISSING)
                        }                        
                    }                 

                    //TEST: FREEZE PLAYER UNTIL FIGHT START
                }
            }

            StartCountdown();
        }

        public void SendArenaParticipantsSound(int sound)
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;
                    if (arenaParticipant.m_Player == null) continue;
                    if (arenaParticipant.m_Player.Deleted) continue;

                    arenaParticipant.m_Player.SendSound(sound);
                }
            }
        }

        public void SendArenaParticipantsMessage(string text, int hue)
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;
                    if (arenaParticipant.m_Player == null) continue;
                    if (arenaParticipant.m_Player.Deleted) continue;

                    arenaParticipant.m_Player.SendMessage(hue, text);
                }
            }
        }

        public void InvalidateMobile(Mobile mobile)
        {
            mobile.InvalidateProperties();
            mobile.SendIncomingPacket();
            mobile.SendEverything();
            mobile.Delta(MobileDelta.Noto);
        }

        public void InvalidatePlayers()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            List<ArenaParticipant> m_Participants = m_ArenaMatch.GetParticipants();

            foreach (ArenaParticipant participant in m_Participants)
            {
                if (participant == null) continue;
                if (participant.Deleted) continue;
                if (participant.m_Player == null) continue;

                participant.m_Player.InvalidateProperties();
                participant.m_Player.SendIncomingPacket();
                participant.m_Player.SendEverything();
                participant.m_Player.Delta(MobileDelta.Noto);
            }
        }

        public void StartCountdown()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            m_FightPhase = FightPhaseType.StartCountdown;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);

            InvalidatePlayers();
        }

        public void StartFight()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            m_ArenaController.ClearWalls();

            //TEST: SEND SOUND TO PLAYERS

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;

                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Alive;

                    //TEST: UNFREEZE
                }
            }

            m_FightPhase = FightPhaseType.Fight;
            m_PhaseTimeRemaining = TimeSpan.FromDays(1);

            InvalidatePlayers();
        }

        public void StartSuddenDeath()
        {
            InvalidatePlayers();
        }
        
        public void StartPostBattle(ArenaTeam winningTeam, VictoryType victoryType)
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            string announcement = "";

            announcement = "Match Complete!";

            switch (victoryType)
            {
                case VictoryType.PlayersEliminated: announcement += " (team eliminated)"; break;
                case VictoryType.SuddenDeathHitPointsRemaining: announcement += " (hit points remaining)"; break;
                case VictoryType.SuddenDeathDamageDealt: announcement += " (most damage dealt)"; break;
                case VictoryType.SuddenDeathDamageReceived: announcement += " (least damage taken)"; break;
                case VictoryType.Randomized: announcement += " (random draw)"; break;
            }

            AnnouncerMessage(announcement, 63);
            
            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;
                    if (arenaParticipant.m_Player == null) continue;                   

                    PlayerMobile player = arenaParticipant.m_Player;

                    ArenaPlayerSettings.CheckCreateArenaPlayerSettings(player);

                    switch (m_ArenaMatch.m_Ruleset.m_MatchType)
                    {
                        case ArenaRuleset.MatchTypeType.Ranked1vs1:
                            if (arenaTeam == winningTeam)
                                player.m_ArenaPlayerSettings.Ranked1vs1Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked1vs1Losses++;

                        break;

                        case ArenaRuleset.MatchTypeType.Ranked2vs2:
                            if (arenaTeam == winningTeam)
                                player.m_ArenaPlayerSettings.Ranked2vs2Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked2vs2Losses++;
                        break;

                        case ArenaRuleset.MatchTypeType.Ranked3vs3:
                            if (arenaTeam == winningTeam)
                                player.m_ArenaPlayerSettings.Ranked3vs3Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked3vs3Losses++;
                        break;

                        case ArenaRuleset.MatchTypeType.Ranked4vs4:
                            if (arenaTeam == winningTeam)
                                player.m_ArenaPlayerSettings.Ranked4vs4Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked4vs4Losses++;
                        break;
                    }

                    //Send Match Summary Gump

                    if (IsWithinArena(player.Location, player.Map))                    
                        RestoreAndClearEffects(player);

                    foreach (Mobile mobile in player.AllFollowers)
                    {
                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature == null) continue;
                        if (!IsWithinArena(bc_Creature.Location, bc_Creature.Map)) continue;

                        RestoreAndClearEffects(bc_Creature);
                    }

                    if (arenaTeam == winningTeam)
                    {
                    }

                    else
                    {
                    }
                    
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.PostBattle;
                }
            }
            
            m_FightPhase = FightPhaseType.PostBattle;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);

            InvalidatePlayers();
        }

        public void FightCompleted()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            Queue m_ItemsToTrashQueue = new Queue();
            Queue m_ItemsToDeleteQueue = new Queue();

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null)
                    continue;

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null)
                        continue;
                    
                    PlayerMobile player = participant.m_Player;

                    if (IsWithinArena(player.Location, player.Map))
                    {
                        ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                        if (exitTile != null)
                        {
                            player.Location = exitTile.Location;
                            player.Map = exitTile.Map;
                        }

                        else
                        {
                            player.Location = m_ArenaController.Location;
                            player.Map = m_ArenaController.Map;
                        }
                    }

                    participant.m_FightStatus = ArenaParticipant.FightStatusType.Waiting;
                    participant.m_ReadyToggled = false;
                    participant.m_LastEventTime = DateTime.UtcNow;

                    participant.ResetArenaFightValues();                   
                }
            }

            m_ArenaMatch.m_ArenaFight = null;
            m_ArenaMatch.m_MatchStatus = ArenaMatch.MatchStatusType.Listed;   
         
            IPooledEnumerable arenaObjects = m_ArenaController.Map.GetObjectsInBounds(m_ArenaController.ArenaBoundary);

            foreach (Object targetObject in arenaObjects)
            {
                if (targetObject is Item)
                {
                    Item item = targetObject as Item;

                    if (item.Movable)
                        m_ItemsToTrashQueue.Enqueue(item);

                    if (item is Corpse)
                        m_ItemsToDeleteQueue.Enqueue(item);
                }

                if (targetObject is Mobile)
                {
                    Mobile targetMobile = targetObject as Mobile;

                    ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                    if (exitTile != null)
                    {
                        targetMobile.Location = exitTile.Location;
                        targetMobile.Map = exitTile.Map;
                    }

                    else
                    {
                        targetMobile.Location = m_ArenaController.Location;
                        targetMobile.Map = m_ArenaController.Map;
                    }
                }
            }

            arenaObjects.Free();

            while (m_ItemsToTrashQueue.Count > 0)
            {
                Item arenaItem = (Item)m_ItemsToTrashQueue.Dequeue();

                ArenaTrashBarrel arenaTrashBarrel = ArenaTrashBarrel.GetArenaTrashBarrel(m_ArenaController);

                if (arenaTrashBarrel != null)
                    arenaTrashBarrel.DropItem(arenaItem);
            }

            while (m_ItemsToDeleteQueue.Count > 0)
            {
                Item arenaItem = (Item)m_ItemsToDeleteQueue.Dequeue();

                arenaItem.Delete();
            }
            
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            InvalidatePlayers();

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
                            m_ArenaFight.SendArenaParticipantsSound(0x4B7); //0x0F5 //0x4D5 //0x485 //0x100
                            m_ArenaFight.SendArenaParticipantsMessage("Battle begins!", 63);

                            m_ArenaFight.StartFight();
                            return;
                        }

                        else
                        {
                            m_ArenaFight.SendArenaParticipantsSound(0x4D3); //0x0FA //0x49D
                            m_ArenaFight.SendArenaParticipantsMessage("Battle will begin in " + m_ArenaFight.m_PhaseTimeRemaining.TotalSeconds.ToString() + "...", 2599);
                        }
                    break;

                    case FightPhaseType.Fight:
                        ArenaTeam winningTeam = m_ArenaFight.CheckForTeamVictory();

                        if (winningTeam != null)
                        {
                            m_ArenaFight.StartPostBattle(winningTeam, VictoryType.PlayersEliminated);
                            return;
                        }

                        if (m_ArenaFight.m_SuddenDeath)
                        {
                            m_ArenaFight.m_SuddenDeathTickCounter++;
                            m_ArenaFight.m_SuddenDeathTimeRemaining = m_ArenaFight.m_SuddenDeathTimeRemaining.Subtract(ArenaFight.TimerTickDuration);

                            m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Sudden Death: " + m_ArenaFight.m_SuddenDeathTimeRemaining.TotalSeconds.ToString());

                            if (m_ArenaFight.m_SuddenDeathTimeRemaining.TotalSeconds <= 0)
                            {
                               m_ArenaFight.ForcedSuddenDeathResolution();
                                return;
                            }
                        }

                        else
                        {
                            m_ArenaFight.m_RoundTimeRemaining = m_ArenaFight.m_RoundTimeRemaining.Subtract(ArenaFight.TimerTickDuration);

                            m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Fight: " + m_ArenaFight.m_RoundTimeRemaining.TotalSeconds.ToString());
                        
                            if (m_ArenaFight.m_RoundTimeRemaining.TotalSeconds <= 0)
                            {
                                m_ArenaFight.m_SuddenDeath = true;

                                m_ArenaFight.SendArenaParticipantsSound(0x4D5);
                                m_ArenaFight.SendArenaParticipantsMessage("Sudden Death begins!", 2116);

                                m_ArenaFight.StartSuddenDeath();
                                return;
                            }
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
                }                
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaController);
            writer.Write(m_ArenaMatch);
            writer.Write((int)m_FightPhase);
            writer.Write(m_PhaseTimeRemaining);
            writer.Write(m_RoundTimeRemaining);

            writer.Write(m_SuddenDeath);
            writer.Write(m_SuddenDeathTickCounter);
            writer.Write(m_SuddenDeathTimeRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaController = (ArenaController)reader.ReadItem();
                m_ArenaMatch = (ArenaMatch)reader.ReadItem();
                m_FightPhase = (FightPhaseType)reader.ReadInt();
                m_PhaseTimeRemaining = reader.ReadTimeSpan();
                m_RoundTimeRemaining = reader.ReadTimeSpan();

                m_SuddenDeath = reader.ReadBool();
                m_SuddenDeathTickCounter = reader.ReadInt();
                m_SuddenDeathTimeRemaining = reader.ReadTimeSpan();
            }

            //-----

            m_Timer = new ArenaFightTimer(this);
            m_Timer.Start();
        }
    }
}