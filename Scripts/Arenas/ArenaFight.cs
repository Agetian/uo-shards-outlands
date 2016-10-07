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

        public ArenaMatch m_ArenaMatch;
        public FightPhaseType m_FightPhase = FightPhaseType.StartCountdown;
        public TimeSpan m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);
        public TimeSpan m_RoundTimeRemaining = TimeSpan.FromMinutes(3);

        public bool m_SuddenDeath = false;
        public int m_SuddenDeathTickCounter = 0;
        public TimeSpan m_SuddenDeathTimeRemaining = TimeSpan.FromMinutes(3);

        public List<ArenaTeam> m_Teams = new List<ArenaTeam>();

        public static TimeSpan TimerTickDuration = TimeSpan.FromSeconds(1);

        public Timer m_Timer;
        
        //----

        public CompetitionContext m_CompetitionContext;
        
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

        public virtual void OnMapChanged(PlayerMobile player)
        {
            OnLocationChanged(player);
        }

        public virtual void OnLocationChanged(PlayerMobile player)
        {
            if (player == null)
                return;

            ArenaParticipant arenaParticipant = GetParticipant(player);

            if (arenaParticipant != null && m_ArenaController != null)
            {
                if (arenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive && !m_ArenaController.IsWithin(player.Location))
                {
                    arenaParticipant.m_EventStatus = ArenaParticipant.EventStatusType.Inactive;
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Disqualified;

                    if (player.Map == Map.Internal)
                    {
                        ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                        if (exitTile != null)
                            player.LogoutLocation = exitTile.Location;
                    }

                    if (!CheckTeamsRemaining())
                        StartPostBattle();
                }                
            }
        }

        public virtual void OnDeath(PlayerMobile player, Container corpse)
        {
            ArenaParticipant arenaParticipant = GetParticipant(player);

            if (arenaParticipant != null)
            {
                if (arenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive)
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Dead;
            }

            if (!CheckTeamsRemaining())
                StartPostBattle();
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

        public virtual void CancelSpell(PlayerMobile player)
        {
            if (player.Spell is Spell)
            {
                Spell spell = player.Spell as Spell;
                spell.Disturb(DisturbType.Kill);
            }

            Targeting.Target.Cancel(player);
        }

        public virtual void ClearEffects(PlayerMobile player)
        {
            SpecialAbilities.ClearSpecialEffects(player);

            player.RemoveStatMod("[Magic] Str Offset");
            player.RemoveStatMod("[Magic] Dex Offset");
            player.RemoveStatMod("[Magic] Int Offset");
            //TEST: CLEAR MAGIC RESIST POTION

            player.Paralyzed = false;
            player.Hidden = false;

            player.MagicDamageAbsorb = 0;
            player.MeleeDamageAbsorb = 0;

            Spells.Second.ProtectionSpell.Registry.Remove(player);
            player.EndAction(typeof(DefensiveSpell));

            TransformationSpellHelper.RemoveContext(player, true);

            BaseArmor.ValidateMobile(player);
            BaseClothing.ValidateMobile(player);

            player.Hits = player.HitsMax;
            player.Stam = player.StamMax;
            player.Mana = player.ManaMax;

            player.Poison = null;
        }

        public virtual void RemoveAggressions(PlayerMobile player)
        {
            /*
            for (int i = 0; i < m_Participants.Count; ++i)
            {
                Participant p = (Participant)m_Participants[i];

                for (int j = 0; j < p.Players.Length; ++j)
                {
                    DuelPlayer dp = (DuelPlayer)p.Players[j];

                    if (dp == null || dp.Mobile == mob)
                        continue;

                    mob.RemoveAggressed(dp.Mobile);
                    mob.RemoveAggressor(dp.Mobile);
                    dp.Mobile.RemoveAggressed(mob);
                    dp.Mobile.RemoveAggressor(mob);
                }
            }
            */
        }

        #endregion

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

                arenaTeam.m_LastEventTime = DateTime.UtcNow;

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

                    if (arenaParticipant.m_EventStatus != ArenaParticipant.EventStatusType.Inactive)
                        arenaParticipant.m_LastEventTime = DateTime.UtcNow;
                    
                    if (arenaParticipant.m_FightStatus != ArenaParticipant.FightStatusType.Alive)
                        playersEliminated++;                    
                }

                if (playersEliminated >= participantCount)
                    teamsRemaining--;
            }

            if (teamsRemaining <= 1)
                return false;

            return true;
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
                    //TEST: MISSING WALL TILE
                }
            }

            for (int a = 0; a < m_Teams.Count; a++)
            {
                ArenaTeam arenaTeam = m_Teams[a];

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

                    ClearEffects(player);

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

                    //TEST: Participant Settings Set

                    //TEST: ADD COMPETITION CONTEXT                    

                    //TEST: FREEZE PLAYER UNTIL FIGHT START
                }
            }

            StartCountdown();
        }

        public void SendArenaParticipantsSound(int sound)
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
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
            foreach (ArenaTeam arenaTeam in m_Teams)
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

        public void StartCountdown()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            m_FightPhase = FightPhaseType.StartCountdown;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);

            List<ArenaParticipant> m_Participants = m_ArenaMatch.GetParticipants();

            foreach (ArenaParticipant participant in m_Participants)
            {
                if (participant == null) continue;
                if (participant.Deleted) continue;
                if (participant.m_Player == null) continue;

                participant.m_Player.CloseAllGumps();
            }
        }

        public void StartFight()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            m_ArenaController.ClearWalls();

            //TEST: SEND SOUND TO PLAYERS

            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;

                    arenaParticipant.m_EventStatus = ArenaParticipant.EventStatusType.Playing;

                    //TEST: UNFREEZE
                }
            }

            m_FightPhase = FightPhaseType.Fight;
            m_PhaseTimeRemaining = TimeSpan.FromDays(1);
        }

        public void StartSuddenDeath()
        {
        }
        
        public void StartPostBattle()
        {
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;
                    
                    if (arenaParticipant.m_EventStatus != ArenaParticipant.EventStatusType.Inactive)                    
                        arenaParticipant.m_EventStatus = ArenaParticipant.EventStatusType.PostBattle;

                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.PostBattle;
                }
            }
            
            m_FightPhase = FightPhaseType.PostBattle;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);
        }

        public void FightCompleted()
        {
            m_FightPhase = FightPhaseType.Completed;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(5);
        }

        public void ClearFight()
        {
            /*
            foreach (ArenaTeam arenaTeam in m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                arenaTeam.m_LastEventTime = DateTime.UtcNow;

                foreach(ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;

                    if (arenaParticipant.m_EventStatus != ArenaParticipant.EventStatusType.Inactive)
                    {
                        arenaParticipant.m_EventStatus = ArenaParticipant.EventStatusType.Waiting;
                        arenaParticipant.m_LastEventTime = DateTime.UtcNow;
                    }

                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Alive;

                    arenaParticipant.m_ArenaFight = null;                    

                    //TEST: Record Player Fight Data
                }
            }
            */

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_ArenaController != null)
                m_ArenaController.MatchComplete();
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
                        if (!m_ArenaFight.CheckTeamsRemaining())
                        {
                            m_ArenaFight.StartPostBattle();
                            return;
                        }

                        if (m_ArenaFight.m_SuddenDeath)
                        {
                            m_ArenaFight.m_SuddenDeathTickCounter++;
                            m_ArenaFight.m_SuddenDeathTimeRemaining = m_ArenaFight.m_SuddenDeathTimeRemaining.Subtract(ArenaFight.TimerTickDuration);

                            m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Sudden Death: " + m_ArenaFight.m_SuddenDeathTimeRemaining.TotalSeconds.ToString());

                            if (m_ArenaFight.m_SuddenDeathTimeRemaining.TotalSeconds <= 0)
                            {
                                m_ArenaFight.StartPostBattle();

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
            writer.Write(m_ArenaMatch);
            writer.Write((int)m_FightPhase);
            writer.Write(m_PhaseTimeRemaining);
            writer.Write(m_RoundTimeRemaining);

            writer.Write(m_SuddenDeath);
            writer.Write(m_SuddenDeathTickCounter);
            writer.Write(m_SuddenDeathTimeRemaining);            

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
                m_ArenaMatch = (ArenaMatch)reader.ReadItem();
                m_FightPhase = (FightPhaseType)reader.ReadInt();
                m_PhaseTimeRemaining = reader.ReadTimeSpan();
                m_RoundTimeRemaining = reader.ReadTimeSpan();

                m_SuddenDeath = reader.ReadBool();
                m_SuddenDeathTickCounter = reader.ReadInt();
                m_SuddenDeathTimeRemaining = reader.ReadTimeSpan();

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