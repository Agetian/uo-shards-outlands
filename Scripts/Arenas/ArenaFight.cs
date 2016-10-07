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
                if (arenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive && !m_ArenaController.IsWithinArena(player.Location))
                {
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.Eliminated;

                    if (player.Map == Map.Internal)
                    {
                        ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                        if (exitTile != null)
                            player.LogoutLocation = exitTile.Location;
                    }

                    ArenaTeam winningTeam = CheckForTeamVictory();

                    if (winningTeam != null)
                    {
                        StartPostBattle(winningTeam, false);
                        return;
                    }
                }                
            }
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
            
            Timer.DelayCall(TimeSpan.FromSeconds(4.0), delegate
            {
                if (player == null)
                    return;

                ClearEffects(player);

                player.Hits = player.HitsMax;
                player.Stam = player.StamMax;
                player.Mana = player.ManaMax;

                player.DropHolding();

                if (player.BankBox != null)
                    player.BankBox.Close();

                player.CloseAllGumps();

                if (player.NetState != null)
                    player.NetState.CancelAllTrades();

                CancelSpell(player);

                Target.Cancel(player);

                if (!player.Alive)                
                    player.Resurrect();                

                //Force Exit
                if (m_ArenaController != null)
                {
                    if (m_ArenaController.IsWithinArena(player.Location))
                    {
                        ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                        if (exitTile != null)
                            player.Location = exitTile.Location;

                        else
                            player.Location = Location;  
                    }
                }
            });

            ArenaTeam winningTeam = CheckForTeamVictory();

            if (winningTeam != null)
            {
                StartPostBattle(winningTeam, false);
                return;
            }
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
            if (player == null)
                return;

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

            player.RemoveStatModsBeginningWith("[Magic]");

            player.MagicDamageAbsorb = 0;
            player.MeleeDamageAbsorb = 0;
            player.VirtualArmorMod = 0;

            BuffInfo.RemoveBuff(player, BuffIcon.Agility);
            BuffInfo.RemoveBuff(player, BuffIcon.ArchProtection);
            BuffInfo.RemoveBuff(player, BuffIcon.Bless);
            BuffInfo.RemoveBuff(player, BuffIcon.Clumsy);
            BuffInfo.RemoveBuff(player, BuffIcon.Incognito);
            BuffInfo.RemoveBuff(player, BuffIcon.MagicReflection);
            BuffInfo.RemoveBuff(player, BuffIcon.MassCurse);
            BuffInfo.RemoveBuff(player, BuffIcon.Invisibility);
            BuffInfo.RemoveBuff(player, BuffIcon.HidingAndOrStealth);
            BuffInfo.RemoveBuff(player, BuffIcon.Paralyze);
            BuffInfo.RemoveBuff(player, BuffIcon.Poison);
            BuffInfo.RemoveBuff(player, BuffIcon.Polymorph);
            BuffInfo.RemoveBuff(player, BuffIcon.Protection);
            BuffInfo.RemoveBuff(player, BuffIcon.ReactiveArmor);
            BuffInfo.RemoveBuff(player, BuffIcon.Strength);
            BuffInfo.RemoveBuff(player, BuffIcon.Weaken);
            BuffInfo.RemoveBuff(player, BuffIcon.FeebleMind);            
            
            player.Paralyzed = false;
            player.RevealingAction();

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
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                foreach (ArenaParticipant arenaParticipant in arenaTeam.m_Participants)
                {
                    if (arenaParticipant.m_Player == null) continue;
                    if (arenaParticipant.m_Player.Deleted) continue;

                    arenaParticipant.m_Player.RemoveAggressed(player);
                    arenaParticipant.m_Player.RemoveAggressor(player);

                    player.RemoveAggressed(arenaParticipant.m_Player);
                    player.RemoveAggressor(arenaParticipant.m_Player);                      
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

            //TEST: FIX AND FINISH
            ArenaTeam winningTeam = new ArenaTeam();

            StartPostBattle(winningTeam, true);
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

                    RemoveAggressions(player);

                    ClearEffects(player);

                    player.Hits = player.HitsMax;
                    player.Stam = player.StamMax;
                    player.Mana = player.ManaMax;

                    player.DropHolding();

                    if (player.BankBox != null)
                        player.BankBox.Close();

                    player.CloseAllGumps();

                    if (player.NetState != null)
                        player.NetState.CancelAllTrades();

                    CancelSpell(player);

                    Target.Cancel(player);

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

        public void StartCountdown()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            m_FightPhase = FightPhaseType.StartCountdown;
            m_PhaseTimeRemaining = TimeSpan.FromSeconds(10);
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
        }

        public void StartSuddenDeath()
        {
        }
        
        public void StartPostBattle(ArenaTeam winningTeam, bool forcedSuddenDeathVictory)
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            //Announce Resolution

            //

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
        }

        public void FightCompleted()
        {
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                return;

            Queue m_TeamsQueue = new Queue();
            Queue m_ParticipantQueue = new Queue();

            Queue m_ItemsToTrashQueue = new Queue();
            Queue m_ItemsToDeleteQueue = new Queue();

            foreach (ArenaTeam arenaTeam in m_ArenaMatch.m_Teams)
            {
                if (arenaTeam == null)
                    continue;

                m_TeamsQueue.Enqueue(arenaTeam);

                foreach (ArenaParticipant participant in arenaTeam.m_Participants)
                {
                    if (participant == null)
                        continue;

                    m_ParticipantQueue.Enqueue(participant);

                    PlayerMobile player = participant.m_Player;

                    if (m_ArenaController.IsWithinArena(player.Location))
                    {
                        ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                        if (exitTile != null)
                            player.Location = exitTile.Location;

                        else
                            player.Location = Location;
                    }

                    participant.m_FightStatus = ArenaParticipant.FightStatusType.Waiting;
                    participant.m_ReadyToggled = false;
                    participant.m_LastEventTime = DateTime.UtcNow;

                    participant.ResetArenaFightValues();                   
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
            }

            arenaObjects.Free();

            while (m_ItemsToTrashQueue.Count > 0)
            {
                Item arenaItem = (Item)m_ItemsToTrashQueue.Dequeue();

                //TEST: Move Item to Arena-Specific Trash Can
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
                            m_ArenaFight.StartPostBattle(winningTeam, false);
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