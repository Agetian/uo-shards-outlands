using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class DamageTracker
    {
        public enum DamageType
        {
            MeleeDamage,
            SpellDamage,
            PoisonDamage,
            FollowerDamage,
            ProvocationDamage,
            DamageTaken,
            FollowerDamageTaken,
            HealingDealt
        }

        public PlayerMobile m_Player;
        public bool m_Running = false;

        public bool m_Collapsed = false;

        public DamageTrackerTimer m_Timer;

        public int MeleeDamage = 0;
        public int SpellDamage = 0;
        public int FollowerDamage = 0;
        public int ProvocationDamage = 0;
        public int PoisonDamage = 0;
        public int DamageTaken = 0;
        public int FollowerDamageTaken = 0;
        public int HealingDealt = 0;

        public bool AddMeleeDamageToTotal = true;
        public bool AddSpellDamageToTotal = true;
        public bool AddPoisonDamageToTotal = true;
        public bool AddProvocationDamageToTotal = true;
        public bool AddFollowerDamageToTotal = true;

        public TimeSpan TimerAmountElapsed = TimeSpan.FromSeconds(0);
        public bool TimerEnabled = true;
        public TimeSpan TimerDuration = TimerDefaultDuration;
        public static TimeSpan TimerDefaultDuration = TimeSpan.FromMinutes(1);
        public static TimeSpan TimerMinimumDuration = TimeSpan.FromSeconds(5);

        public static TimeSpan TickSpeed = TimeSpan.FromSeconds(1);

        public DamageTracker(PlayerMobile player)
        {
            m_Player = player;
        }

        public void ClearResults()
        {
            MeleeDamage = 0;
            SpellDamage = 0;
            FollowerDamage = 0;
            ProvocationDamage = 0;
            PoisonDamage = 0;

            DamageTaken = 0;
            FollowerDamageTaken = 0;

            HealingDealt = 0;
        }

        public static int AdjustDisplayedDamage(Mobile from, Mobile target, int amount)
        {
            BaseCreature bc_Source = from as BaseCreature;
            PlayerMobile pm_Source = from as PlayerMobile;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;
            
            PlayerMobile pm_SourceMaster = null;

            if (pm_Source != null)            
                pm_SourceMaster = bc_Source.GetPlayerMaster() as PlayerMobile;            

            double displayedDamage = amount;

            if (bc_Target != null)
            {
                //Discordance
                displayedDamage *= 1 + bc_Target.DiscordEffect;

                //Ship Combat
                if (BaseShip.UseShipBasedDamageModifer(from, bc_Target))
                    displayedDamage *= BaseShip.shipBasedDamageToCreatureScalar;
            }

            //Ress Penalty
            if (pm_Source != null)
            {
                if (pm_Source.RessPenaltyExpiration > DateTime.UtcNow && pm_Source.m_RessPenaltyEffectivenessReductionCount > 0)
                    displayedDamage *= 1 - (PlayerMobile.RessPenaltyDamageScalar * (double)pm_Source.m_RessPenaltyEffectivenessReductionCount);
            }

            else if (pm_SourceMaster != null)
            {
                if (pm_SourceMaster.RessPenaltyExpiration > DateTime.UtcNow && pm_SourceMaster.m_RessPenaltyEffectivenessReductionCount > 0)
                    displayedDamage *= 1 - (PlayerMobile.RessPenaltyDamageScalar * (double)pm_SourceMaster.m_RessPenaltyEffectivenessReductionCount);
            }

            //Ship Combat
            if (pm_Target != null)
            {
                if (BaseShip.UseShipBasedDamageModifer(from, pm_Target))
                    displayedDamage *= BaseShip.shipBasedDamageToPlayerScalar;
            }

            return (int)(Math.Round(displayedDamage));
        }

        public static void RecordDamage(Mobile owner, Mobile from, Mobile target, DamageType damageType, int amount)
        {
            PlayerMobile player = owner as PlayerMobile;

            if (player == null)
                return;

            if (player.m_DamageTracker == null)
                player.m_DamageTracker = new DamageTracker(player);

            BaseCreature bc_From = from as BaseCreature;
            BaseCreature bc_Target = target as BaseCreature;

            PlayerMobile pm_From = from as PlayerMobile;
            PlayerMobile pm_Target = target as PlayerMobile;

            PlayerMobile rootPlayerFrom = pm_From;
            PlayerMobile rootPlayerTarget = pm_Target;

            #region Ship Damage to Creature: For Doubloon Distribution on Death

            if (bc_Target != null)
            {
                if (bc_Target.DoubloonValue > 0)
                {
                    BaseShip shipFrom = null;                   

                    if (bc_From != null)
                    {
                        if (bc_From.ShipOccupied != null)
                            shipFrom = bc_From.ShipOccupied;
                    }

                    if (pm_From != null)
                    {
                        if (pm_From.ShipOccupied != null)
                            shipFrom = pm_From.ShipOccupied;
                    }

                    if (shipFrom != null)
                    {
                        bool foundDamageEntry = false;

                        foreach (DamageFromShipEntry damageFromShipEntry in bc_Target.m_DamageFromShipEntries)
                        {
                            if (damageFromShipEntry == null)
                                continue;

                            if (damageFromShipEntry.m_Ship == shipFrom)
                            {
                                damageFromShipEntry.m_TotalAmount += amount;
                                damageFromShipEntry.m_LastDamageTime = DateTime.UtcNow;

                                foundDamageEntry = true;
                            }
                        }

                        if (!foundDamageEntry)
                        {
                            DamageFromShipEntry damageEntry = new DamageFromShipEntry(shipFrom, amount, DateTime.UtcNow);

                            bc_Target.m_DamageFromShipEntries.Add(damageEntry);
                        }
                    }
                }
            }

            #endregion           
            
            #region Arena

            bool checkArenaDamage = false;

            if (bc_From != null)
            {
                if (bc_From.ControlMaster is PlayerMobile)
                    rootPlayerFrom = bc_From.ControlMaster as PlayerMobile;
            }

            if (bc_Target != null)
            {
                if (bc_Target.ControlMaster is PlayerMobile)
                    rootPlayerTarget = bc_Target.ControlMaster as PlayerMobile;
            }

            if (rootPlayerFrom != null && rootPlayerTarget != null)
            {
                ArenaGroupController arenaGroupControllerFrom = ArenaGroupController.GetArenaGroupRegionAtLocation(rootPlayerFrom.Location, rootPlayerFrom.Map);
                ArenaGroupController arenaGroupControllerTarget = ArenaGroupController.GetArenaGroupRegionAtLocation(rootPlayerFrom.Location, rootPlayerFrom.Map);

                if (arenaGroupControllerFrom != null && arenaGroupControllerTarget != null && arenaGroupControllerFrom == arenaGroupControllerTarget)
                {
                    if (rootPlayerFrom.m_ArenaMatch != null && rootPlayerTarget.m_ArenaMatch != null && rootPlayerFrom.m_ArenaMatch == rootPlayerTarget.m_ArenaMatch)
                    {
                        if (rootPlayerFrom.m_ArenaMatch.m_MatchStatus == ArenaMatch.MatchStatusType.Fighting)
                            checkArenaDamage = true;
                    }
                }
            }
        
            #endregion            

            switch (damageType)
            {
                case DamageType.MeleeDamage:
                    if (player.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerMeleeDamageTextHue, "You attack " + target.Name + " for " + amount.ToString() + " damage.");

                    if (player.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerMeleeDamageTextHue, false, "-" + amount.ToString(), player.NetState);

                    if (checkArenaDamage)
                        ArenaFight.CheckArenaDamage(player, DamageType.MeleeDamage, amount);
                break;

                case DamageType.SpellDamage:
                    if (player.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerSpellDamageTextHue, "Your spell hits " + target.Name + " for " + amount.ToString() + " damage.");

                    if (player.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerSpellDamageTextHue, false, "-" + amount.ToString(), player.NetState);

                    if (checkArenaDamage)
                        ArenaFight.CheckArenaDamage(player, DamageType.SpellDamage, amount);
                break;

                case DamageType.FollowerDamage:
                    if (player.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerFollowerDamageTextHue, "Follower: " + from.Name + " attacks " + target.Name + " for " + amount.ToString() + " damage.");

                    if (player.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerFollowerDamageTextHue, false, "-" + amount.ToString(), player.NetState);

                    if (checkArenaDamage)
                        ArenaFight.CheckArenaDamage(player, DamageType.FollowerDamage, amount);
                break;

                case DamageType.ProvocationDamage:
                    if (player.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerProvocationDamageTextHue, "Provocation: " + from.Name + " inflicts " + amount.ToString() + " damage on " + target.Name + ".");

                    if (player.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerProvocationDamageTextHue, false, "-" + amount.ToString(), player.NetState);

                    if (checkArenaDamage)
                        ArenaFight.CheckArenaDamage(player, DamageType.ProvocationDamage, amount);
                break;

                case DamageType.PoisonDamage:
                    if (player.m_ShowPoisonDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerPoisonDamageTextHue, "You inflict " + amount.ToString() + " poison damage on " + target.Name + ".");

                    if (player.m_ShowPoisonDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerPoisonDamageTextHue, false, "-" + amount.ToString(), player.NetState);

                    if (checkArenaDamage)
                        ArenaFight.CheckArenaDamage(player, DamageType.PoisonDamage, amount);
                break;

                case DamageType.DamageTaken:
                    if (player.m_ShowDamageTaken == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerDamageTakenTextHue, from.Name + " attacks you for " + amount.ToString() + " damage.");

                    if (player.m_ShowDamageTaken == DamageDisplayMode.PrivateOverhead)
                        player.PrivateOverheadMessage(MessageType.Regular, player.PlayerDamageTakenTextHue, false, "-" + amount.ToString(), player.NetState);

                    if (checkArenaDamage)
                        ArenaFight.CheckArenaDamage(player, DamageType.DamageTaken, amount);
                break;

                case DamageType.FollowerDamageTaken:
                    if (player.m_ShowFollowerDamageTaken == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerFollowerDamageTakenTextHue, "Follower: " + target.Name + " is hit for " + amount.ToString() + " damage.");

                    if (player.m_ShowFollowerDamageTaken == DamageDisplayMode.PrivateOverhead && player.NetState != null)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerFollowerDamageTakenTextHue, false, "-" + amount.ToString(), player.NetState);
                break;
            }

            if (!player.m_DamageTracker.m_Running)
                return;

            switch (damageType)
            {
                case DamageType.MeleeDamage: player.m_DamageTracker.MeleeDamage += amount; break;
                case DamageType.SpellDamage: player.m_DamageTracker.SpellDamage += amount; break;
                case DamageType.FollowerDamage: player.m_DamageTracker.FollowerDamage += amount; break;
                case DamageType.ProvocationDamage: player.m_DamageTracker.ProvocationDamage += amount; break;
                case DamageType.PoisonDamage: player.m_DamageTracker.PoisonDamage += amount; break;
                case DamageType.DamageTaken: player.m_DamageTracker.DamageTaken += amount; break;
                case DamageType.FollowerDamageTaken: player.m_DamageTracker.FollowerDamageTaken += amount; break;
                case DamageType.HealingDealt: player.m_DamageTracker.HealingDealt += amount; break;
            }
        }

        public void RefreshGump()
        {
            if (m_Player != null)
            {
                if (m_Player.HasGump(typeof(DamageTrackerGump)))
                {
                    m_Player.CloseGump(typeof(DamageTrackerGump));
                    m_Player.SendGump(new DamageTrackerGump(m_Player));
                }
            }
        }

        public void StopTimer()
        {
            m_Running = false;

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public void StartTimer()
        {
            m_Running = true;

            if (m_Timer == null)
            {
                m_Timer = new DamageTrackerTimer(this);
                m_Timer.Start();
            }

            else
                m_Timer.Start();
        }

        public class DamageTrackerTimer : Timer
        {
            public DamageTracker m_DamageTracker;

            public DamageTrackerTimer(DamageTracker damageTracker): base(TickSpeed, TickSpeed)
            {
                m_DamageTracker = damageTracker;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_DamageTracker == null)
                {
                    Stop();
                    return;
                }

                if (m_DamageTracker.m_Player == null)
                {
                    Stop();
                    return;
                }

                if (m_DamageTracker.m_Player.Deleted || m_DamageTracker.m_Player.NetState == null)
                {
                    Stop();
                    return;
                }

                if (!m_DamageTracker.m_Running)
                {
                    Stop();
                    return;
                }

                m_DamageTracker.TimerAmountElapsed += DamageTracker.TickSpeed;

                if (m_DamageTracker.TimerEnabled)
                {
                    if (m_DamageTracker.TimerAmountElapsed >= m_DamageTracker.TimerDuration)
                        m_DamageTracker.StopTimer();
                }

                m_DamageTracker.RefreshGump();
            }
        }
    }

    public class DamageTrackerGump : Gump
    {
        public PlayerMobile m_Player;

        public int WhiteTextHue = 2499;

        public DamageTrackerGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (player.m_DamageTracker == null) return;

            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            int totalDamage = 0;

            if (player.m_DamageTracker.AddMeleeDamageToTotal)
                totalDamage += player.m_DamageTracker.MeleeDamage;

            if (player.m_DamageTracker.AddSpellDamageToTotal)
                totalDamage += player.m_DamageTracker.SpellDamage;

            if (player.m_DamageTracker.AddFollowerDamageToTotal)
                totalDamage += player.m_DamageTracker.FollowerDamage;

            if (player.m_DamageTracker.AddProvocationDamageToTotal)
                totalDamage += player.m_DamageTracker.ProvocationDamage;

            if (player.m_DamageTracker.AddPoisonDamageToTotal)
                totalDamage += player.m_DamageTracker.PoisonDamage;

            string timerDuration = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + m_Player.m_DamageTracker.TimerDuration, true, true, true, true, true);
            string timerAmountElapsed = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + m_Player.m_DamageTracker.TimerAmountElapsed, true, true, true, true, true);

            if (m_Player.m_DamageTracker.TimerAmountElapsed == TimeSpan.FromSeconds(0))
                timerAmountElapsed = "0s";           

            if (player.m_DamageTracker.m_Collapsed)
            {
                #region Images

                AddImage(38, 17, 103, 2499);
                AddImage(181, 17, 103, 2499);
                AddImage(48, 25, 2081, 2499);
                AddImage(48, 37, 2081, 2499);
                AddItem(43, 46, 6160);

                #endregion

                AddButton(9, 16, 9906, 9906, 1, GumpButtonType.Reply, 0); //Expand Gump    

                AddLabel(69, 30, 149, "Time Elapsed");
                if (m_Player.m_DamageTracker.TimerAmountElapsed >= m_Player.m_DamageTracker.TimerDuration && !m_Player.m_DamageTracker.m_Running)
                    AddLabel(77, 50, 2101, timerAmountElapsed);
                else
                    AddLabel(77, 50, 63, timerAmountElapsed);             

                AddLabel(187, 30, 2577, "Dealt");
                AddLabel(229, 30, WhiteTextHue, Utility.CreateCurrencyString(totalDamage));

                AddLabel(184, 50, 2603, "Taken");
                AddLabel(229, 50, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.DamageTaken));

                AddLabel(57, 79, 1256, "Reset Timer");
                AddButton(79, 101, 4020, 4022, 2, GumpButtonType.Reply, 0);

                AddLabel(151, 79, 53, "Clear Damage");
                AddButton(176, 98, 9721, 9724, 3, GumpButtonType.Reply, 0);

                if (player.m_DamageTracker.m_Running)
                {
                    AddLabel(253, 79, 2599, "Pause");
                    AddButton(257, 101, 4017, 4019, 4, GumpButtonType.Reply, 0);
                }

                else
                {
                    AddLabel(253, 79, 2599, "Start");
                    AddButton(257, 101, 4005, 4007, 4, GumpButtonType.Reply, 0);
                }   
            }

            else
            {
                #region Images

                AddImage(38, 17, 103, 2499);
                AddImage(181, 17, 103, 2499);
                AddImage(181, 110, 103, 2499);
                AddImage(38, 110, 103, 2499);
                AddImage(181, 206, 103, 2499);
                AddImage(39, 206, 103, 2499);
                AddImage(181, 306, 103, 2499);
                AddImage(38, 306, 103, 2499);
                AddImage(48, 25, 2081, 2499);
                AddImage(48, 94, 2081, 2499);
                AddImage(47, 160, 2081, 2499);
                AddImage(47, 225, 2081, 2499);
                AddImage(47, 293, 2081, 2499);
                AddImage(48, 326, 2081, 2499);                

                AddImage(51, 165, 3001, 1105);
                AddImage(65, 165, 3001, 1105);
                AddImage(51, 238, 3001, 1105);
                AddImage(65, 238, 3001, 1105);
                AddImage(51, 333, 3001, 1105);
                AddImage(65, 333, 3001, 1105);

                #endregion

                AddButton(36, 14, 2094, 2095, 5, GumpButtonType.Reply, 0);
                AddLabel(36, 2, 149, "Guide");

                AddButton(9, 16, 9900, 9900, 1, GumpButtonType.Reply, 0); //Collapse Gump

                //------------

                AddLabel(137, 25, 149, "Damage Dealt");               

                AddLabel(107, 45, 34, "Melee");
                if (player.m_DamageTracker.AddMeleeDamageToTotal)
                    AddButton(149, 45, 211, 210, 6, GumpButtonType.Reply, 0);
                else
                    AddButton(149, 45, 210, 211, 6, GumpButtonType.Reply, 0);
                AddLabel(175, 45, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.MeleeDamage));

                AddLabel(110, 65, 117, "Spell");
                if (player.m_DamageTracker.AddSpellDamageToTotal)
                    AddButton(149, 65, 211, 210, 7, GumpButtonType.Reply, 0);
                else
                    AddButton(149, 65, 210, 211, 7, GumpButtonType.Reply, 0);
                AddLabel(175, 65, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.SpellDamage));

                AddLabel(100, 85, 63, "Poison");
                if (player.m_DamageTracker.AddPoisonDamageToTotal)
                    AddButton(149, 85, 211, 210, 8, GumpButtonType.Reply, 0);
                else
                    AddButton(149, 85, 210, 211, 8, GumpButtonType.Reply, 0);
                AddLabel(175, 85, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.PoisonDamage));

                AddLabel(65, 105, 2417, "Provocation");
                if (player.m_DamageTracker.AddProvocationDamageToTotal)
                    AddButton(149, 105, 211, 210, 9, GumpButtonType.Reply, 0);
                else
                    AddButton(149, 105, 210, 211, 9, GumpButtonType.Reply, 0);
                AddLabel(175, 105, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.ProvocationDamage));

                AddLabel(81, 125, 89, "Followers");
                if (player.m_DamageTracker.AddFollowerDamageToTotal)
                    AddButton(149, 124, 211, 210, 10, GumpButtonType.Reply, 0);
                else
                    AddButton(149, 124, 210, 211, 10, GumpButtonType.Reply, 0);
                AddLabel(175, 125, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.FollowerDamage));

                AddLabel(106, 145, 2577, "Total");
                AddLabel(175, 145, WhiteTextHue, Utility.CreateCurrencyString(totalDamage));

                //----------

                AddLabel(136, 170, 149, "Damage Taken");

                AddLabel(140, 195, 2603, "Self");
                AddLabel(175, 195, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.DamageTaken));

                AddLabel(106, 215, 2550, "Followers");
                AddLabel(175, 215, WhiteTextHue, Utility.CreateCurrencyString(player.m_DamageTracker.FollowerDamageTaken));

                //---------

                AddLabel(136, 243, 149, "Damage Timer");

                AddLabel(80, 263, 2599, "Force Pause After Duration");
                if (player.m_DamageTracker.TimerEnabled)
                    AddButton(265, 264, 211, 210, 17, GumpButtonType.Reply, 0);
                else
                    AddButton(265, 264, 210, 211, 17, GumpButtonType.Reply, 0);

                AddLabel(107, 286, 2599, "Timer Duration");
                AddLabel(211, 286, WhiteTextHue, timerDuration);

                AddButton(70, 314, 2223, 2223, 11, GumpButtonType.Reply, 0);
                AddLabel(94, 310, WhiteTextHue, "Hour");
                AddButton(127, 314, 2224, 2224, 12, GumpButtonType.Reply, 0);

                AddButton(154, 314, 2223, 2223, 13, GumpButtonType.Reply, 0);
                AddLabel(177, 310, WhiteTextHue, "Min");
                AddButton(202, 314, 2224, 2224, 14, GumpButtonType.Reply, 0);

                AddButton(228, 314, 2223, 2223, 15, GumpButtonType.Reply, 0);
                AddLabel(252, 310, WhiteTextHue, "Sec");
                AddButton(278, 314, 2224, 2224, 16, GumpButtonType.Reply, 0);

                //-------------

                AddLabel(96, 341, 149, "Time Elapsed");
                AddItem(165, 338, 6160);
                if (m_Player.m_DamageTracker.TimerAmountElapsed >= m_Player.m_DamageTracker.TimerDuration && !m_Player.m_DamageTracker.m_Running)
                    AddLabel(200, 343, 2101, timerAmountElapsed);
                else
                    AddLabel(200, 343, 63, timerAmountElapsed);

                //----------

                AddLabel(58, 369, 1256, "Reset Timer");
                AddButton(81, 391, 4020, 4022, 2, GumpButtonType.Reply, 0);

                AddLabel(148, 369, 53, "Clear Damage");
                AddButton(172, 388, 9721, 9724, 3, GumpButtonType.Reply, 0);

                if (player.m_DamageTracker.m_Running)
                {
                    AddLabel(251, 369, 2599, "Pause");
                    AddButton(256, 391, 4017, 4019, 4, GumpButtonType.Reply, 0);
                }

                else
                {
                    AddLabel(251, 369, 2599, "Start");
                    AddButton(256, 391, 4005, 4007, 4, GumpButtonType.Reply, 0);
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.m_DamageTracker == null) return;

            bool closeGump = true;

            if (m_Player.m_DamageTracker.m_Collapsed)
            {
                switch (info.ButtonID)
                {
                    //Expand
                    case 1:
                        m_Player.m_DamageTracker.m_Collapsed = false;

                        closeGump = false;
                    break;

                    //Reset Timer
                    case 2:
                        m_Player.m_DamageTracker.TimerAmountElapsed = TimeSpan.FromSeconds(0);

                        closeGump = false;
                    break;

                    //Reset Damage
                    case 3:
                        m_Player.m_DamageTracker.ClearResults();

                        closeGump = false;
                    break;

                    //Pause + Start
                    case 4:
                        if (m_Player.m_DamageTracker.m_Running)
                            m_Player.m_DamageTracker.StopTimer();

                        else
                            m_Player.m_DamageTracker.StartTimer();

                        closeGump = false;
                    break;
                }
            }

            else
            {
                switch (info.ButtonID)
                {
                    //Collapse
                    case 1:
                        m_Player.m_DamageTracker.m_Collapsed = true;

                        closeGump = false;
                    break;

                    //Reset Timer
                    case 2:
                        m_Player.m_DamageTracker.TimerAmountElapsed = TimeSpan.FromSeconds(0);                       

                        closeGump = false;
                    break;

                    //Reset Damage
                    case 3:
                        m_Player.m_DamageTracker.ClearResults();                        

                        closeGump = false;
                    break;

                    //Pause + Start
                    case 4:
                        if (m_Player.m_DamageTracker.m_Running)                        
                            m_Player.m_DamageTracker.StopTimer();                        

                        else
                            m_Player.m_DamageTracker.StartTimer();

                        closeGump = false;
                    break;

                    //Guide
                    case 5:
                        closeGump = false;
                    break;

                    //Add to Total: Melee                    
                    case 6:
                        m_Player.m_DamageTracker.AddMeleeDamageToTotal = !m_Player.m_DamageTracker.AddMeleeDamageToTotal;

                        if (m_Player.m_DamageTracker.AddMeleeDamageToTotal)
                            m_Player.SendMessage("Your melee damage inflicted will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Your melee damage inflicted will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Spell
                        case 7:
                        m_Player.m_DamageTracker.AddSpellDamageToTotal = !m_Player.m_DamageTracker.AddSpellDamageToTotal;

                        if (m_Player.m_DamageTracker.AddSpellDamageToTotal)
                            m_Player.SendMessage("Your spell damage inflicted will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Your spell damage inflicted will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Poison
                    case 8:
                        m_Player.m_DamageTracker.AddPoisonDamageToTotal = !m_Player.m_DamageTracker.AddPoisonDamageToTotal;

                        if (m_Player.m_DamageTracker.AddPoisonDamageToTotal)
                            m_Player.SendMessage("Your poison damage inflicted will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Your poison damage inflicted will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Provocation
                    case 9:
                        m_Player.m_DamageTracker.AddProvocationDamageToTotal = !m_Player.m_DamageTracker.AddProvocationDamageToTotal;

                        if (m_Player.m_DamageTracker.AddPoisonDamageToTotal)
                            m_Player.SendMessage("Damage caused by your provoked creatures will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Damaged caused by your provoked creatures will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Follower
                    case 10:
                        m_Player.m_DamageTracker.AddFollowerDamageToTotal = !m_Player.m_DamageTracker.AddFollowerDamageToTotal;

                        if (m_Player.m_DamageTracker.AddPoisonDamageToTotal)
                            m_Player.SendMessage("Damage caused by your followers will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Damage caused by your followers will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Auto-Stop: Add 1 Hour                   
                    case 11:
                        if (m_Player.m_DamageTracker.TimerDuration < TimeSpan.FromHours(1))
                            m_Player.m_DamageTracker.TimerDuration = TimeSpan.FromHours(1);

                        else
                            m_Player.m_DamageTracker.TimerDuration = m_Player.m_DamageTracker.TimerDuration + TimeSpan.FromHours(1);

                        closeGump = false;
                    break;

                    //Auto-Stop: Remove 1 Hour                   
                    case 12:
                        m_Player.m_DamageTracker.TimerDuration = m_Player.m_DamageTracker.TimerDuration - TimeSpan.FromHours(1);                            

                        if (m_Player.m_DamageTracker.TimerDuration < DamageTracker.TimerMinimumDuration)
                            m_Player.m_DamageTracker.TimerDuration = DamageTracker.TimerMinimumDuration;
                        
                    closeGump = false;
                    break;

                    //Auto-Stop: Add 1 Minute                 
                    case 13:
                        if (m_Player.m_DamageTracker.TimerDuration < TimeSpan.FromMinutes(1))
                            m_Player.m_DamageTracker.TimerDuration = TimeSpan.FromMinutes(1);     

                        else
                            m_Player.m_DamageTracker.TimerDuration = m_Player.m_DamageTracker.TimerDuration + TimeSpan.FromMinutes(1);

                        closeGump = false;
                    break;

                    //Auto-Stop: Remove 1 Minute                   
                    case 14:                        
                        m_Player.m_DamageTracker.TimerDuration = m_Player.m_DamageTracker.TimerDuration - TimeSpan.FromMinutes(1);

                        if (m_Player.m_DamageTracker.TimerDuration < DamageTracker.TimerMinimumDuration)
                            m_Player.m_DamageTracker.TimerDuration = DamageTracker.TimerMinimumDuration;                        

                        closeGump = false;
                    break;

                    //Auto-Stop: Add 5 Seconds                 
                    case 15:
                        m_Player.m_DamageTracker.TimerDuration = m_Player.m_DamageTracker.TimerDuration + TimeSpan.FromSeconds(5);

                        closeGump = false;
                    break;

                    //Auto-Stop: Remove 5 Seconds                   
                    case 16:                        
                        m_Player.m_DamageTracker.TimerDuration = m_Player.m_DamageTracker.TimerDuration - TimeSpan.FromSeconds(5);

                        if (m_Player.m_DamageTracker.TimerDuration < DamageTracker.TimerMinimumDuration)
                            m_Player.m_DamageTracker.TimerDuration = DamageTracker.TimerMinimumDuration;                        

                        closeGump = false;
                    break;

                    //Auto Stop
                    case 17:
                        m_Player.m_DamageTracker.TimerEnabled = !m_Player.m_DamageTracker.TimerEnabled;

                        closeGump = false;
                    break;
                }
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(DamageTrackerGump));
                m_Player.SendGump(new DamageTrackerGump(m_Player));
            }
        }
    }
}