using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;

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
        public TimeSpan m_TimeElapsed = TimeSpan.FromSeconds(0);

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

        public static void CheckArenaDamage(PlayerMobile player, DamageTracker.DamageType damageType, int amount)
        {
            if (player == null)
                return;

            ArenaMatch arenaMatch = player.m_ArenaMatch;
            ArenaMatch arenaFight = null;

            if (arenaMatch == null) return;
            if (arenaMatch.Deleted) return;
            
            if (arenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Fighting)
                return;
                                        
            ArenaParticipant participant = arenaMatch.GetParticipant(player);

            if (participant == null) return;
            if (participant.Deleted) return;
            if (participant.m_Player == null) return;
                        
            switch (damageType)
            {
                case DamageTracker.DamageType.MeleeDamage: participant.m_DamageDealt += amount; break;
                case DamageTracker.DamageType.SpellDamage: participant.m_DamageDealt += amount; break;
                case DamageTracker.DamageType.PoisonDamage: participant.m_DamageDealt += amount; break;
                case DamageTracker.DamageType.FollowerDamage: participant.m_DamageDealt += amount; break;
                case DamageTracker.DamageType.ProvocationDamage: participant.m_DamageDealt += amount; break;

                case DamageTracker.DamageType.DamageTaken:
                    participant.m_DamageReceived += amount;

                    if (participant.m_Player.Hits < participant.m_LowestHealth)
                        participant.m_LowestHealth = participant.m_Player.Hits;
                break;
            }            
        }

        public static bool AttemptSpellUsage(PlayerMobile player, Type spellType)
        {
            if (player == null) return true;
            if (player.m_ArenaMatch == null) return true;
            if (player.m_ArenaFight == null) return true;            

            if (!player.m_ArenaFight.IsWithinArena(player.Location, player.Map)) return true;
            if (player.m_ArenaFight.m_FightPhase == FightPhaseType.StartCountdown) return false;
            if (player.m_ArenaFight.m_FightPhase == FightPhaseType.PostBattle) return false;

            ArenaRuleset ruleset = player.m_ArenaMatch.m_Ruleset;
            ArenaParticipant participant = player.m_ArenaParticipant;

            if (participant == null)
                return true;

            bool restrictedSpell = false;

            if (spellType == typeof(TelekinesisSpell)) restrictedSpell = true;
            if (spellType == typeof(RecallSpell)) restrictedSpell = true;
            if (spellType == typeof(BladeSpirits)) restrictedSpell = true;
            if (spellType == typeof(IncognitoSpell)) restrictedSpell = true;
            if (spellType == typeof(SummonCreatureSpell)) restrictedSpell = true;
            if (spellType == typeof(InvisibilitySpell)) restrictedSpell = true;
            if (spellType == typeof(MarkSpell)) restrictedSpell = true;
            if (spellType == typeof(GateTravelSpell)) restrictedSpell = true;
            if (spellType == typeof(PolymorphSpell)) restrictedSpell = true;
            if (spellType == typeof(AirElementalSpell)) restrictedSpell = true;
            if (spellType == typeof(EarthElementalSpell)) restrictedSpell = true;
            if (spellType == typeof(EnergyVortexSpell)) restrictedSpell = true;
            if (spellType == typeof(FireElementalSpell)) restrictedSpell = true;
            if (spellType == typeof(ResurrectionSpell)) restrictedSpell = true;
            if (spellType == typeof(SummonDaemonSpell)) restrictedSpell = true;
            if (spellType == typeof(WaterElementalSpell)) restrictedSpell = true;

            if (restrictedSpell)
            {
                player.SendMessage("That spell is not allowed here.");
                return false;
            }

            foreach (ArenaSpellRestriction spellRestriction in ruleset.m_SpellRestrictions)
            {
                if (spellRestriction == null)
                    continue;

                if (spellRestriction.m_SpellType == spellType)
                {
                    int maxUsesAllowed = 0;

                    switch (spellRestriction.m_RestrictionMode)
                    {
                        case ArenaSpellRestriction.SpellRestrictionModeType.Disabled: maxUsesAllowed = 0; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.OneUse: maxUsesAllowed = 1; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.ThreeUses: maxUsesAllowed = 3; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.FiveUses: maxUsesAllowed = 5; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.TenUses: maxUsesAllowed = 10; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.TwentyFiveUses: maxUsesAllowed = 25; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.FiftyUses: maxUsesAllowed = 50; break;
                        case ArenaSpellRestriction.SpellRestrictionModeType.Unlimited: return true; break;
                    }

                    if (maxUsesAllowed == 0)
                    {
                        player.SendMessage("That spell has been restricted for this match.");
                        return false;
                    }

                    ArenaSpellUsage arenaSpellUsage = participant.GetSpellUsage(spellType);

                    if (arenaSpellUsage != null)
                    {
                        if (arenaSpellUsage.m_Uses >= maxUsesAllowed)
                        {
                            player.SendMessage("You have exceeded the maximum uses of that spell allowed for this match.");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static void SpellCompletion(Mobile mobile, Type spellType)
        {
            PlayerMobile player = mobile as PlayerMobile;
            
            if (player == null) return;
            if (player.m_ArenaMatch == null) return;
            if (player.m_ArenaFight == null) return;

            if (!player.m_ArenaFight.IsWithinArena(player.Location, player.Map)) return;
            if (player.m_ArenaFight.m_FightPhase == FightPhaseType.StartCountdown) return;
            if (player.m_ArenaFight.m_FightPhase == FightPhaseType.PostBattle) return;

            ArenaRuleset ruleset = player.m_ArenaMatch.m_Ruleset;
            ArenaParticipant participant = player.m_ArenaParticipant;

            if (participant == null)
                return;

            participant.AdjustSpellUsage(spellType, 1);

            ArenaSpellUsage arenaSpellUsage = participant.GetSpellUsage(spellType);

            int spellUses = 0;

            if (arenaSpellUsage != null)
                spellUses = arenaSpellUsage.m_Uses;

            ArenaSpellRestriction arenaSpellRestriction = ruleset.GetSpellRestriction(spellType);

            if (arenaSpellRestriction != null)
            {
                int maxUsesAllowed = 0;

                switch (arenaSpellRestriction.m_RestrictionMode)
                {
                    case ArenaSpellRestriction.SpellRestrictionModeType.Disabled: maxUsesAllowed = 0; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.OneUse: maxUsesAllowed = 1; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.ThreeUses: maxUsesAllowed = 3; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.FiveUses: maxUsesAllowed = 5; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.TenUses: maxUsesAllowed = 10; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.TwentyFiveUses: maxUsesAllowed = 25; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.FiftyUses: maxUsesAllowed = 50; break;
                    case ArenaSpellRestriction.SpellRestrictionModeType.Unlimited: maxUsesAllowed = 10000; break;
                }

                if (spellUses > 0 && (maxUsesAllowed > 0 && maxUsesAllowed <= 100))
                {
                    string spellName = "";

                    if (spellType == typeof(PoisonSpell)) spellName = "Poison";
                    if (spellType == typeof(PoisonFieldSpell)) spellName = "Poison Field";
                    if (spellType == typeof(ParalyzeSpell)) spellName = "Paralyze";
                    if (spellType == typeof(ParalyzeFieldSpell)) spellName = "Paralyze Field";
                    if (spellType == typeof(MeteorSwarmSpell)) spellName = "Meteor Swarm";
                    if (spellType == typeof(ChainLightningSpell)) spellName = "Chain Lightning";
                    if (spellType == typeof(EarthquakeSpell)) spellName = "Earthquake";

                    player.SendMessage(spellName + " Casts Allowed: " + spellUses.ToString() + " / " + maxUsesAllowed.ToString());
                }
            }            
        }

        public static bool AttemptItemUsage(PlayerMobile player, Item item)
        {
            if (player == null) return true;
            if (player.m_ArenaMatch == null) return true;
            if (player.m_ArenaFight == null) return true;

            if (!player.m_ArenaFight.IsWithinArena(player.Location, player.Map)) return true;
            if (player.m_ArenaFight.m_FightPhase == FightPhaseType.StartCountdown)
            {
                if (item is BaseContainer)
                {
                    if (item is TrapableContainer)
                    {
                        TrapableContainer container = item as TrapableContainer;

                        if (container.TrapType != TrapType.None)
                        {
                            player.SendMessage("Trapped containers may not be used during match countdowns.");
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }            

            ArenaRuleset ruleset = player.m_ArenaMatch.m_Ruleset;
            ArenaParticipant participant = player.m_ArenaParticipant;

            if (participant == null)
                return true;

            if (item is Corpse)
                return false;

            bool allowedItem = false;

            if (item is Bandage)
                allowedItem = true;

            if (item is BaseHealPotion || item is BaseCurePotion || item is BaseRefreshPotion || item is BaseStrengthPotion ||
                item is BaseAgilityPotion || item is BaseExplosionPotion || item is BaseMagicResistPotion || item is BasePoisonPotion)
                allowedItem = true;

            if (item is BaseContainer)            
                allowedItem = true;

            if (!allowedItem)
            {
                player.SendMessage("That item is not allowed during arena matches.");
                return false;
            }

            Type itemType = item.GetType();

            if (item is BaseAgilityPotion) itemType = typeof(BaseAgilityPotion);
            if (item is BaseCurePotion) itemType = typeof(BaseCurePotion);
            if (item is BaseExplosionPotion) itemType = typeof(BaseExplosionPotion);
            if (item is BaseHealPotion) itemType = typeof(BaseHealPotion);
            if (item is BaseMagicResistPotion) itemType = typeof(BaseMagicResistPotion);
            if (item is BasePoisonPotion) itemType = typeof(BasePoisonPotion);
            if (item is BaseRefreshPotion) itemType = typeof(BaseRefreshPotion);
            if (item is BaseStrengthPotion) itemType = typeof(BaseStrengthPotion);

            if (item is TrapableContainer)
            {
                TrapableContainer container = item as TrapableContainer;

                if (container.TrapType != TrapType.None)
                    itemType = typeof(Pouch);
            }

            ArenaItemRestriction itemRestriction = ruleset.GetItemRestriction(itemType);

            if (itemRestriction != null)
            {
                int maxUsesAllowed = 0;

                switch (itemRestriction.m_RestrictionMode)
                {
                    case ArenaItemRestriction.ItemRestrictionModeType.Disabled: maxUsesAllowed = 0; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.OneUse: maxUsesAllowed = 1; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.ThreeUses: maxUsesAllowed = 3; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.FiveUses: maxUsesAllowed = 5; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.TenUses: maxUsesAllowed = 10; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses: maxUsesAllowed = 25; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.FiftyUses: maxUsesAllowed = 50; break;
                    case ArenaItemRestriction.ItemRestrictionModeType.Unlimited: return true; break;
                }

                if (maxUsesAllowed == 0)
                {
                    player.SendMessage("That item has been restricted for this match.");
                    return false;
                }

                ArenaItemUsage arenaItemUsage = participant.GetItemUsage(itemType);

                if (arenaItemUsage != null)
                {
                    if (arenaItemUsage.m_Uses >= maxUsesAllowed)
                    {
                        player.SendMessage("You have exceeded the maximum uses of that item allowed for this match.");
                        return false;
                    }

                    else
                    {
                        arenaItemUsage.m_Uses++;

                        string itemName = "";

                        if (itemType == typeof(BaseAgilityPotion)) itemName = "Agility Potion";
                        if (itemType == typeof(BaseCurePotion)) itemName = "Cure Potion";
                        if (itemType == typeof(BaseExplosionPotion)) itemName = "Explosion Potion";
                        if (itemType == typeof(BaseHealPotion)) itemName = "Heal Potion";
                        if (itemType == typeof(BaseMagicResistPotion)) itemName = "Magic Resist Potion";
                        if (itemType == typeof(BasePoisonPotion)) itemName = "Poison Potion";
                        if (itemType == typeof(BaseRefreshPotion)) itemName = "Refresh Potion";
                        if (itemType == typeof(BaseStrengthPotion)) itemName = "Strength Potion";
                        if (itemType == typeof(Pouch)) itemName = "Trapped Container";

                        player.SendMessage(itemName + " Uses Allowed: " + arenaItemUsage.m_Uses.ToString() + " / " + maxUsesAllowed.ToString());

                        return true;
                    }
                }
            }

            return true;
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

        public virtual bool AllowSkillUse(PlayerMobile player, SkillName skill)
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
            if (player == null) return;
            if (m_ArenaMatch == null) return;

            ArenaParticipant participant = m_ArenaMatch.GetParticipant(player);

            if (participant == null) return;
            if (participant.m_FightStatus != ArenaParticipant.FightStatusType.Alive) return;
            if (IsWithinArena(player.Location, player.Map)) return;

            participant.m_FightStatus = ArenaParticipant.FightStatusType.Eliminated;
            participant.m_LowestHealth = 0;

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

        public virtual bool IsWithinArena(Point3D location, Map map)
        {
            if (m_ArenaController == null)
                return false;

            return m_ArenaController.IsWithinArena(location, map);
        }

        public virtual bool IsWithinArenaGroupRegion(Point3D location, Map map)
        {
            if (m_ArenaController == null) return false;
            if (m_ArenaController.m_ArenaGroupController == null) return false;

            return m_ArenaController.m_ArenaGroupController.IsWithinArenaGroupRegion(location, map);
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
            if (player == null) return;
            if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false)) return;   
            if (!IsWithinArena(player.Location, player.Map)) return;

            ArenaParticipant participant = m_ArenaMatch.GetParticipant(player);

            if (participant != null)
            {
                if (participant.m_FightStatus == ArenaParticipant.FightStatusType.Alive)
                {
                    participant.m_FightStatus = ArenaParticipant.FightStatusType.Dead;
                    participant.m_LowestHealth = 0;
                }
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
                if (player == null)
                    return;

                Point3D newLocation = player.Location;
                Map newMap = player.Map;

                bool foundNewLocation = false;
                bool playerWithinArena = false;

                if (ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false) && m_ArenaController != null)
                {
                    ArenaTile exitTile = m_ArenaController.GetRandomExitTile();

                    if (exitTile != null)
                    {
                        newLocation = exitTile.Location;
                        newMap = exitTile.Map;

                        foundNewLocation = true;
                    }

                    if (IsWithinArena(player.Location, player.Map))
                        playerWithinArena = true;
                }

                if (!foundNewLocation)
                {
                    //TEST: CREATE FALLBACK PLAYER + CREATURE DROP LOCATION
                    //newLocation = Prevalia Bank
                    //newMap = Prevalia Bank Map
                }
                
                player.Location = newLocation;
                player.Map = newMap;                

                if (!player.Alive)
                    player.Resurrect();

                RestoreAndClearEffects(player);    

                foreach (Mobile mobile in player.AllFollowers)
                {
                    BaseCreature bc_Creature = mobile as BaseCreature;

                    if (bc_Creature == null) continue;
                    if (bc_Creature.Deleted) continue;
                    if (!IsWithinArena(bc_Creature.Location, bc_Creature.Map)) continue;

                    if ((!bc_Creature.Alive || bc_Creature.IsDeadFollower) && !bc_Creature.IsBonded)
                        continue;

                    bc_Creature.Location = newLocation;
                    bc_Creature.Map = newMap;

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

                    int arenaCreditCost = m_ArenaMatch.m_Ruleset.GetArenaCreditsCost();

                    player.m_ArenaAccountEntry.m_ArenaCredits -= arenaCreditCost;

                    if (player.m_ArenaAccountEntry.m_ArenaCredits < 0)
                        player.m_ArenaAccountEntry.m_ArenaCredits = 0;

                    player.SendMessage(arenaCreditCost.ToString() + " arena credits have been deducted from your account (" + player.m_ArenaAccountEntry.m_ArenaCredits.ToString() + " remaining).");
                    
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

            ArenaMatchResultEntry arenaMatchResultEntry = m_ArenaMatch.m_ArenaMatchResultEntry;

            if (arenaMatchResultEntry == null)
                arenaMatchResultEntry = new ArenaMatchResultEntry();

            //Set to Now Be Free Floating
            arenaMatchResultEntry.m_ArenaMatch = null;

            arenaMatchResultEntry.m_MatchStatus = ArenaMatchResultEntry.ArenaMatchResultStatusType.Completed;
            arenaMatchResultEntry.m_MatchType = m_ArenaMatch.m_Ruleset.m_MatchType;
            arenaMatchResultEntry.m_CompletionDate = DateTime.UtcNow;
            arenaMatchResultEntry.m_MatchDuration = m_TimeElapsed;
            
            for (int a = 0; a < m_ArenaMatch.m_Teams.Count; a++)
            {
                ArenaTeam arenaTeam = m_ArenaMatch.m_Teams[a];

                if (arenaTeam == null) continue;
                if (arenaTeam.Deleted) continue;

                bool isWinningTeam = (arenaTeam == winningTeam);
                string teamName = arenaTeam.m_TeamName;
                
                if (teamName == "")
                    teamName = "Team " + (a + 1).ToString();

                if (isWinningTeam)
                    arenaMatchResultEntry.m_WinningTeam = teamName;

                List<ArenaMatchPlayerResultEntry> arenaMatchPlayerResultEntries = new List<ArenaMatchPlayerResultEntry>();

                for (int b = 0; b < arenaTeam.m_Participants.Count; b++)
                {
                    ArenaParticipant arenaParticipant = arenaTeam.m_Participants[b];

                    if (arenaParticipant == null) continue;
                    if (arenaParticipant.Deleted) continue;
                    if (arenaParticipant.m_Player == null) continue;                   

                    PlayerMobile player = arenaParticipant.m_Player;

                    ArenaPlayerSettings.CheckCreateArenaPlayerSettings(player);

                    switch (m_ArenaMatch.m_Ruleset.m_MatchType)
                    {
                        case ArenaRuleset.MatchTypeType.Ranked1vs1:
                            if (isWinningTeam)
                                player.m_ArenaPlayerSettings.Ranked1vs1Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked1vs1Losses++;

                        break;

                        case ArenaRuleset.MatchTypeType.Ranked2vs2:
                            if (isWinningTeam)
                                player.m_ArenaPlayerSettings.Ranked2vs2Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked2vs2Losses++;
                        break;

                        case ArenaRuleset.MatchTypeType.Ranked3vs3:
                            if (isWinningTeam)
                                player.m_ArenaPlayerSettings.Ranked3vs3Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked3vs3Losses++;
                        break;

                        case ArenaRuleset.MatchTypeType.Ranked4vs4:
                            if (isWinningTeam)
                                player.m_ArenaPlayerSettings.Ranked4vs4Wins++;

                            else
                                player.m_ArenaPlayerSettings.Ranked4vs4Losses++;
                        break;
                    }

                    bool alive = (arenaParticipant.m_FightStatus == ArenaParticipant.FightStatusType.Alive);

                    ArenaMatchPlayerResultEntry arenaMatchPlayerResultEntry = new ArenaMatchPlayerResultEntry(player, player.RawName, alive, arenaParticipant.m_LowestHealth, arenaParticipant.m_TimeAlive, arenaParticipant.m_DamageDealt, arenaParticipant.m_DamageReceived);

                    arenaMatchPlayerResultEntries.Add(arenaMatchPlayerResultEntry);

                    if (IsWithinArena(player.Location, player.Map))                    
                        RestoreAndClearEffects(player);

                    foreach (Mobile mobile in player.AllFollowers)
                    {
                        BaseCreature bc_Creature = mobile as BaseCreature;

                        if (bc_Creature == null) continue;
                        if (!IsWithinArena(bc_Creature.Location, bc_Creature.Map)) continue;

                        RestoreAndClearEffects(bc_Creature);
                    }
                    
                    arenaParticipant.m_FightStatus = ArenaParticipant.FightStatusType.PostBattle;                   
                }

                ArenaMatchTeamResultEntry arenaMatchTeamResultEntry = new ArenaMatchTeamResultEntry(isWinningTeam, teamName, arenaMatchPlayerResultEntries);
            
                arenaMatchResultEntry.m_TeamResultEntries.Add(arenaMatchTeamResultEntry);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(5.0), delegate
            {
                if (!ArenaMatch.IsValidArenaMatch(m_ArenaMatch, null, false))
                    return;

                List<ArenaParticipant> m_MatchParticipants = m_ArenaMatch.GetParticipants();

                //Update Entries (OnDeath Delay Prevents Last Damage Entry From Being Added)
                foreach (ArenaParticipant participant in m_MatchParticipants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;

                    ArenaMatchPlayerResultEntry playerResultEntry = arenaMatchResultEntry.GetPlayerMatchResultEntry(participant.m_Player);

                    if (playerResultEntry != null)
                    {
                        playerResultEntry.m_LowestHP = participant.m_LowestHealth;
                        playerResultEntry.m_TimeAlive = participant.m_TimeAlive;
                        playerResultEntry.m_DamageDealt = participant.m_DamageDealt;
                        playerResultEntry.m_DamageReceived = participant.m_DamageReceived;
                    }
                }

                foreach (ArenaParticipant participant in m_MatchParticipants)
                {
                    if (participant == null) continue;
                    if (participant.Deleted) continue;
                    if (participant.m_Player == null) continue;                   

                    if (!m_ArenaMatch.m_ArenaGroupController.ArenaGroupRegionBoundary.Contains(participant.m_Player))
                        continue;

                    participant.m_Player.SendSound(0x055);
                    participant.m_Player.CloseGump(typeof(ArenaMatchResultGump));
                    participant.m_Player.SendGump(new ArenaMatchResultGump(participant.m_Player, arenaMatchResultEntry));
                }
            });            

            //New Blank Match Result Entry
            m_ArenaMatch.m_ArenaMatchResultEntry = new ArenaMatchResultEntry();
            m_ArenaMatch.m_ArenaMatchResultEntry.m_ArenaMatch = m_ArenaMatch;
            
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
                        m_ArenaFight.m_TimeElapsed = m_ArenaFight.m_TimeElapsed + TimerTickDuration;

                        List<ArenaParticipant> m_Participants = m_ArenaFight.m_ArenaMatch.GetParticipants();

                        foreach (ArenaParticipant participant in m_Participants)
                        {
                            if (participant == null) continue;
                            if (participant.Deleted) continue;
                            if (participant.m_Player == null) continue;

                            if (participant.m_FightStatus == ArenaParticipant.FightStatusType.Alive)
                                participant.m_TimeAlive = participant.m_TimeAlive + TimerTickDuration;
                        }

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

                            //m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Sudden Death: " + m_ArenaFight.m_SuddenDeathTimeRemaining.TotalSeconds.ToString());

                            if (m_ArenaFight.m_SuddenDeathTimeRemaining.TotalSeconds <= 0)
                            {
                               m_ArenaFight.ForcedSuddenDeathResolution();
                                return;
                            }
                        }

                        else
                        {
                            m_ArenaFight.m_RoundTimeRemaining = m_ArenaFight.m_RoundTimeRemaining.Subtract(ArenaFight.TimerTickDuration);

                            //m_ArenaFight.m_ArenaController.PublicOverheadMessage(MessageType.Regular, 0, false, "Fight: " + m_ArenaFight.m_RoundTimeRemaining.TotalSeconds.ToString());
                        
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
            writer.Write(m_TimeElapsed);

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
                m_TimeElapsed = reader.ReadTimeSpan();

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