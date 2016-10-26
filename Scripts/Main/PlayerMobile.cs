using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.ContextMenus;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Targeting;
using Server.Regions;
using Server.Accounting;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Engines.PartySystem;
using Server.Commands;
using Server.Custom;
using Server.SkillHandlers;
using Server.Regions;
using System.Text;
using System.Net;

namespace Server.Mobiles
{
    [Flags]
    public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
    {
        None = 0x00000000,
        Glassblowing = 0x00000001,
        Masonry = 0x00000002,
        SandMining = 0x00000004,
        StoneMining = 0x00000008,
        ToggleMiningStone = 0x00000010,
        KarmaLocked = 0x00000020,
        AutoRenewInsurance = 0x00000040,
        UseOwnFilter = 0x00000080,
        PublicMyRunUO = 0x00000100,
        PagingSquelched = 0x00000200,
        Young = 0x00000400,
        AcceptGuildInvites = 0x00000800,
        DisplayChampionTitle = 0x00001000,
        HasStatReward = 0x00002000,
        RefuseTrades = 0x00004000,
        Paladin = 0x00010000,
        KilledByPaladin = 0x00020000,
        YewJailed = 0x00040000,
        ShipMovement = 0x00080000
    }

    public enum NpcGuild
    {
        None,
        MagesGuild,
        WarriorsGuild,
        ThievesGuild,
        RangersGuild,
        HealersGuild,
        MinersGuild,
        MerchantsGuild,
        TinkersGuild,
        TailorsGuild,
        FishermensGuild,
        BardsGuild,
        BlacksmithsGuild,
        DetectivesGuild
    }

    public enum BlockMountType
    {
        None = -1,
        Dazed = 1040024,
        BolaRecovery = 1062910,
        DismountRecovery = 1070859
    }

    public enum DamageDisplayMode
    {
        None,
        PrivateMessage,
        PrivateOverhead
    }

    public enum StealthStepsDisplayMode
    {
        None,
        PrivateMessage,
        PrivateOverhead
    }

    public enum HenchmenSpeechDisplayMode
    {
        Normal,
        Infrequent,
        IdleOnly,
        IdleOnlyInfrequent,
        CombatOnly,
        CombatOnlyInfrequent,
        None
    }

    public partial class PlayerMobile : Mobile
    {
        public static void PlayerCountCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage(string.Format("{0} online", Server.RemoteAdmin.ServerInfo.NetStateCount()));
        }

        public static void SetThresholdCommand(CommandEventArgs e)
        {
            double multip = Server.RemoteAdmin.ServerInfo.Multiplier;
            double.TryParse(e.ArgString, out multip);

            Server.RemoteAdmin.ServerInfo.Multiplier = multip;
            e.Mobile.SendMessage(string.Format("Multiplier has been set to {0}", multip));
        }

        public static void ToggleThresholdCommand(CommandEventArgs e)
        {
            Server.RemoteAdmin.ServerInfo.SpoofPlayerCount = !Server.RemoteAdmin.ServerInfo.SpoofPlayerCount;
            e.Mobile.SendMessage("Threshold has been {0}.", Server.RemoteAdmin.ServerInfo.SpoofPlayerCount ? "enabled" : "disabled");
        }

        public static void GoToEntranceCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            var region = player.Region as DungeonRegion;

            if (region != null)
            {
                player.MoveToWorld(region.EntranceLocation, Map.Felucca);
            }
        }       

        public static void Initialize()
        {
            if (FastwalkPrevention)
                PacketHandlers.RegisterThrottler(0x02, new ThrottlePacketCallback(MovementThrottle_Callback));

            EventSink.Login += new LoginEventHandler(OnLogin);
            EventSink.Logout += new LogoutEventHandler(OnLogout);
            EventSink.Connected += new ConnectedEventHandler(EventSink_Connected);
            EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);

            CommandSystem.Register("WipePlayerMobiles", AccessLevel.Administrator, new CommandEventHandler(WipeAllPlayerMobiles_OnCommand));
            CommandSystem.Register("UseTrapPouch", AccessLevel.Player, new CommandEventHandler(UseTrappedPouch_OnCommand));

            CommandSystem.Register("ShowMeleeDamage", AccessLevel.Player, new CommandEventHandler(ShowMeleeDamage));
            CommandSystem.Register("ShowSpellDamage", AccessLevel.Player, new CommandEventHandler(ShowSpellDamage));
            CommandSystem.Register("ShowFollowerDamage", AccessLevel.Player, new CommandEventHandler(ShowFollowerDamage));
            CommandSystem.Register("ShowProvocationDamage", AccessLevel.Player, new CommandEventHandler(ShowProvocationDamage));
            CommandSystem.Register("ShowPoisonDamage", AccessLevel.Player, new CommandEventHandler(ShowPoisonDamage));
            CommandSystem.Register("ShowDamageTaken", AccessLevel.Player, new CommandEventHandler(ShowDamageTaken));
            CommandSystem.Register("ShowFollowerDamageTaken", AccessLevel.Player, new CommandEventHandler(ShowFollowerDamageTaken));
            CommandSystem.Register("ShowHealing", AccessLevel.Player, new CommandEventHandler(ShowHealing));

            CommandSystem.Register("ShowStealthSteps", AccessLevel.Player, new CommandEventHandler(ShowStealthSteps));
            CommandSystem.Register("ShowHenchmenSpeech", AccessLevel.Player, new CommandEventHandler(ShowHenchmenSpeech));
            CommandSystem.Register("ShowAdminTextFilter", AccessLevel.Counselor, new CommandEventHandler(ShowAdminTextFilter));

            CommandSystem.Register("AutoStealth", AccessLevel.Player, new CommandEventHandler(ToggleAutoStealth));
            CommandSystem.Register("DamageTracker", AccessLevel.Player, new CommandEventHandler(ShowDamageTracker));
            CommandSystem.Register("ConsiderSins", AccessLevel.Player, new CommandEventHandler(ConsiderSins));

            CommandSystem.Register("GetDifficulty", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.GetDifficulty));
            CommandSystem.Register("GetDifficultyFull", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.GetDifficultyFull));
            CommandSystem.Register("Provoke", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.AdminProvoke));
            CommandSystem.Register("Tame", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.AdminTame));
            CommandSystem.Register("GotoCurrentWaypoint", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.GotoCurrentWaypoint));

            CommandSystem.Register("gotoentrance", AccessLevel.Administrator, new CommandEventHandler(GoToEntranceCommand));
            CommandSystem.Register("playercount", AccessLevel.Administrator, new CommandEventHandler(PlayerCountCommand));
            CommandSystem.Register("setthreshold", AccessLevel.Administrator, new CommandEventHandler(SetThresholdCommand));
            CommandSystem.Register("togglethreshold", AccessLevel.Administrator, new CommandEventHandler(ToggleThresholdCommand));

            //Used for Locally Testing Content
            CommandSystem.Register("CreateTestLoadout", AccessLevel.GameMaster, new CommandEventHandler(CreateTestLoadout));
            CommandSystem.Register("Anim", AccessLevel.GameMaster, new CommandEventHandler(Anim));
            CommandSystem.Register("AnimationTest", AccessLevel.GameMaster, new CommandEventHandler(AnimationTest));
            CommandSystem.Register("AnimationTestFast", AccessLevel.GameMaster, new CommandEventHandler(AnimationTestFast));
            CommandSystem.Register("BodyTest", AccessLevel.GameMaster, new CommandEventHandler(BodyTest));
            CommandSystem.Register("SetAllHues", AccessLevel.GameMaster, new CommandEventHandler(SetAllHues));
            CommandSystem.Register("SetAllAspect", AccessLevel.GameMaster, new CommandEventHandler(SetAllAspect));
            CommandSystem.Register("SetAllExceptional", AccessLevel.GameMaster, new CommandEventHandler(SetAllExceptional));
        }

        #region Commands

        [Usage("ShowMeleeDamage")]
        [Description("Cycles between Display Modes of Player Melee Damage")]
        public static void ShowMeleeDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowMeleeDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowMeleeDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting player melee damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowMeleeDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting player melee damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowMeleeDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting player melee damage display mode: None");
                    break;
            }
        }

        [Usage("ShowSpellDamage")]
        [Description("Cycles between Display Modes of Player Spell Damage")]
        public static void ShowSpellDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowSpellDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowSpellDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting player spell damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowSpellDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting player spell damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowSpellDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting player spell damage display mode: None");
                    break;
            }
        }

        [Usage("ShowFollowerDamage")]
        [Description("Cycles between Display Modes of Player's Followers Damage")]
        public static void ShowFollowerDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowFollowerDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowFollowerDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting follower damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowFollowerDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting follower damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowFollowerDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting follower damage display mode: None");
                    break;
            }
        }

        [Usage("ShowProvokeDamage")]
        [Description("Cycles between Display Modes of Player's Provoked Creature Damage")]
        public static void ShowProvocationDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowProvocationDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowProvocationDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting provoked creature damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowProvocationDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting provoked creature damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowProvocationDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting provoked creature damage display mode: None");
                    break;
            }
        }

        [Usage("ShowPoisonDamage")]
        [Description("Cycles between Display Modes of Player's Provoked Creature Damage")]
        public static void ShowPoisonDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowPoisonDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowPoisonDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting poison damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowPoisonDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting poison damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowPoisonDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting poison damage display mode: None");
                    break;
            }
        }

        [Usage("ShowDamageTaken")]
        [Description("Cycles between Display Modes of Player's Damage They Take")]
        public static void ShowDamageTaken(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowDamageTaken)
            {
                case DamageDisplayMode.None:
                    player.m_ShowDamageTaken = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting damage taken display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowDamageTaken = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting damage taken display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowDamageTaken = DamageDisplayMode.None;
                    player.SendMessage("Setting damage taken display mode: None");
                    break;
            }
        }

        [Usage("ShowFollowerDamageTaken")]
        [Description("Cycles between Display Modes of Player's Followers Damage Taken")]
        public static void ShowFollowerDamageTaken(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowFollowerDamageTaken)
            {
                case DamageDisplayMode.None:
                    player.m_ShowFollowerDamageTaken = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting follower damage taken display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowFollowerDamageTaken = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting follower damage taken display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowFollowerDamageTaken = DamageDisplayMode.None;
                    player.SendMessage("Setting follower damage taken display mode: None");
                    break;
            }
        }

        [Usage("ShowHealing")]
        [Description("Cycles between Display Modes of Healing Effects")]
        public static void ShowHealing(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowHealing)
            {
                case DamageDisplayMode.None:
                    player.m_ShowHealing = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting healing display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowHealing = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting healing display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowHealing = DamageDisplayMode.None;
                    player.SendMessage("Setting healing display mode: None");
                    break;
            }
        }

        [Usage("ShowStealthSteps")]
        [Description("Cycles between Display Modes of Player's Stealth Steps Feedback")]
        public static void ShowStealthSteps(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_StealthStepsDisplayMode)
            {
                case StealthStepsDisplayMode.None:
                    player.m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateMessage;
                    player.SendMessage("Setting stealth steps display mode: System Message");
                    break;

                case StealthStepsDisplayMode.PrivateMessage:
                    player.m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting stealth steps display mode: Overhead Text");
                    break;

                case StealthStepsDisplayMode.PrivateOverhead:
                    player.m_StealthStepsDisplayMode = StealthStepsDisplayMode.None;
                    player.SendMessage("Setting stealth steps display mode: None");
                    break;
            }
        }

        [Usage("ShowHenchmenSpeech")]
        [Description("Cycles between Modes of Controlled Henchmen Speech")]
        public static void ShowHenchmenSpeech(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_HenchmenSpeechDisplayMode)
            {
                case HenchmenSpeechDisplayMode.Normal:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Infrequent;
                    player.SendMessage("Setting henchmen speech mode: Infrequent");
                    break;

                case HenchmenSpeechDisplayMode.Infrequent:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.IdleOnly;
                    player.SendMessage("Setting henchmen speech mode: Idle Only");
                    break;

                case HenchmenSpeechDisplayMode.IdleOnly:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.IdleOnlyInfrequent;
                    player.SendMessage("Setting henchmen speech mode: Idle Only - Infrequent");
                    break;

                case HenchmenSpeechDisplayMode.IdleOnlyInfrequent:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.CombatOnly;
                    player.SendMessage("Setting henchmen speech mode: Combat Only");
                    break;

                case HenchmenSpeechDisplayMode.CombatOnly:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.CombatOnlyInfrequent;
                    player.SendMessage("Setting henchmen speech mode: Combat Only - Infrequent");
                    break;

                case HenchmenSpeechDisplayMode.CombatOnlyInfrequent:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.None;
                    player.SendMessage("Setting henchmen speech mode: None");
                    break;

                case HenchmenSpeechDisplayMode.None:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Normal;
                    player.SendMessage("Setting henchmen speech mode: Normal");
                    break;
            }
        }

        [Usage("ShowAdminTextFilter")]
        [Description("Toggles between Text Filter Modes")]
        public static void ShowAdminTextFilter(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm.m_ShowAdminFilterText == true)
            {
                pm.SendMessage("Admin Text Filter is now disabled.");
                pm.m_ShowAdminFilterText = false;
            }

            else
            {
                pm.SendMessage("Admin Text Filter is now enabled.");
                pm.m_ShowAdminFilterText = true;
            }
        }

        [Usage("AutoStealth")]
        [Description("Toggles between AutoStealth Modes")]
        public static void ToggleAutoStealth(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm.m_AutoStealth == true)
            {
                pm.SendMessage("Auto-Stealth is now disabled.");
                pm.m_AutoStealth = false;
            }

            else
            {
                pm.SendMessage("Auto-Stealth is now enabled.");
                pm.m_AutoStealth = true;
            }
        }

        [Usage("WipePlayerMobiles")]
        [Description("Changes the password of the commanding players account. Requires the same C-class IP address as the account's creator.")]
        public static void WipeAllPlayerMobiles_OnCommand(CommandEventArgs e)
        {
            List<PlayerMobile> to_delete = new List<PlayerMobile>();
            foreach (Mobile m in World.Mobiles.Values)
            {
                PlayerMobile pm = m as PlayerMobile;
                if (pm != null && pm.AccessLevel == AccessLevel.Player)
                {
                    to_delete.Add(pm);
                }
            }

            foreach (PlayerMobile p in to_delete)
            {
                p.Delete();
            }
        }

        [Usage("UseTrappedPouch")]
        [Description("Uses a trapped pouch in your backpack")]
        public static void UseTrappedPouch_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && pm.Backpack != null)
            {
                if (pm.m_LastTrapPouchUse + TimeSpan.FromSeconds(0.75) > DateTime.UtcNow)
                    pm.SendMessage("You must wait 0.75 seconds between each use of this command");

                else
                {
                    List<TrapableContainer> tcs = pm.Backpack.FindItemsByType<TrapableContainer>();

                    foreach (TrapableContainer tc in tcs)
                    {
                        if (tc != null && tc.TrapType == TrapType.MagicTrap)
                        {
                            tc.Open(pm);
                            Target.Cancel(pm);
                            pm.m_LastTrapPouchUse = DateTime.UtcNow;

                            return;
                        }
                    }
                }
            }
        }

        [Usage("DamageTracker")]
        [Description("Displays the Damage Tracker")]
        public static void ShowDamageTracker(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (player.m_DamageTracker == null)
                player.m_DamageTracker = new DamageTracker(player);

            player.CloseGump(typeof(DamageTrackerGump));
            player.SendGump(new DamageTrackerGump(player));
        }

        [Usage("ConsiderSins")]
        [Description("Displays the 'I Must Consider My Sins' window")]
        public static void ConsiderSins(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendSound(0x055);

            player.CloseGump(typeof(ConsiderSinsGump));
            player.SendGump(new ConsiderSinsGump(player));
        }

        [Usage("CreateTestLoadout")]
        [Description("Sets Character Stats, Skills, and Equipment for TESting")]
        public static void CreateTestLoadout(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Target the character to put in testing mode");
            player.Target = new CreateTestLoadoutTarget(player);
        }

        private class CreateTestLoadoutTarget : Target
        {
            public CreateTestLoadoutTarget(Mobile from)
                : base(100, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target is PlayerMobile)
                {
                    PlayerMobile pm_Target = target as PlayerMobile;

                    if (!pm_Target.Alive)
                        return;

                    pm_Target.RawStr = 10000;
                    pm_Target.Hits = pm_Target.HitsMax;

                    pm_Target.RawDex = 100;
                    pm_Target.Stam = pm_Target.StamMax;

                    pm_Target.RawInt = 1000;
                    pm_Target.Mana = pm_Target.ManaMax;

                    pm_Target.Young = false;

                    foreach (Skill skill in pm_Target.Skills)
                    {
                        skill.Base = 100;
                    }

                    pm_Target.DeleteAllEquipment();

                    pm_Target.Backpack.DropItem(new Arrow(1000));
                    

                    TotalRefreshPotion potion = new TotalRefreshPotion();
                    potion.Amount = 50;
                    pm_Target.Backpack.DropItem(potion);

                    Bandage bandage = new Bandage();
                    bandage.Amount = 200;
                    pm_Target.Backpack.DropItem(bandage);

                    BagOfRegeants bagOfReagents = new BagOfRegeants();
                    pm_Target.Backpack.DropItem(bagOfReagents);

                    bagOfReagents = new BagOfRegeants();
                    pm_Target.Backpack.DropItem(bagOfReagents);

                    Spellbook spellbook = new Spellbook();
                    if (spellbook.BookCount == 64)
                        spellbook.Content = ulong.MaxValue;
                    else
                        spellbook.Content = (1ul << spellbook.BookCount) - 1;

                    pm_Target.Backpack.DropItem(spellbook);

                    int dungeonCount = Enum.GetNames(typeof(AspectEnum)).Length;
                    AspectEnum aspect = (AspectEnum)Utility.RandomMinMax(1, dungeonCount - 1);

                    int aspectHue = AspectGear.GetAspectHue(aspect);

                    pm_Target.AddItem(new Bow() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });

                    pm_Target.AddItem(new PlateHelm() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });
                    pm_Target.AddItem(new PlateGorget() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });
                    pm_Target.AddItem(new PlateArms() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });
                    pm_Target.AddItem(new PlateGloves() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });
                    pm_Target.AddItem(new PlateChest() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });
                    pm_Target.AddItem(new PlateLegs() { Aspect = aspect, TierLevel = 1, ArcaneCharges = AspectGear.AspectStartingArcaneCharges, ArcaneChargesMax = AspectGear.ArcaneMaxCharges });

                    pm_Target.AddItem(new Cloak(aspectHue));

                    AspectGear.CheckForAndUpdateAspectArmorProperties(pm_Target);

                    pm_Target.Backpack.DropItem(new StaffBracelet());
                }

                else
                {
                    from.SendMessage("That is not a player.");
                    return;
                }
            }
        }

        public void DeleteAllEquipment()
        {
            //Clean Out Backpack
            if (Backpack != null)
            {
                if (!Backpack.Deleted)
                {
                    Backpack.Delete();
                    AddItem(new Backpack());
                }
            }

            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Bracelet);
            m_Layers.Add(Layer.Cloak);
            m_Layers.Add(Layer.Earrings);
            m_Layers.Add(Layer.FirstValid);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.InnerLegs);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.MiddleTorso);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.OneHanded);
            m_Layers.Add(Layer.OuterLegs);
            m_Layers.Add(Layer.OuterTorso);
            m_Layers.Add(Layer.Pants);
            m_Layers.Add(Layer.Ring);
            m_Layers.Add(Layer.Shirt);
            m_Layers.Add(Layer.Shoes);
            m_Layers.Add(Layer.Talisman);
            m_Layers.Add(Layer.TwoHanded);
            m_Layers.Add(Layer.Waist);

            foreach (Layer layer in m_Layers)
            {
                Item item = FindItemOnLayer(layer);

                if (item != null)
                {
                    if (!item.Deleted)
                        item.Delete();
                }
            }
        }

        [Usage("Anim <action> <frameCount>")]
        [Description("Makes your character do a specified animation.")]
        public static void Anim(CommandEventArgs e)
        {
            if (e.Length == 2)
                e.Mobile.Animate(e.GetInt32(0), e.GetInt32(1), 1, true, false, 0);
        }

        [Usage("Animation Test")]
        [Description("Loop through all animations of a Bodyvalue")]
        public static void AnimationTest(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            int animations = 32;
            int frameCount = 15;
            int delayBetween = 10;

            Point3D location = player.Location;
            Map map = player.Map;

            for (int a = 1; a < animations + 1; a++)
            {
                int animation = a;

                Timer.DelayCall(TimeSpan.FromSeconds((animation - 1) * delayBetween), delegate
                {
                    if (player == null) return;
                    if (player.Location != location) return;

                    player.Say("Animation: " + animation.ToString());
                    player.Animate(animation, frameCount, 1, true, false, 0);
                });
            }
        }

        [Usage("Animation Test Fast")]
        [Description("Loop through all animations of a Bodyvalue Faster")]
        public static void AnimationTestFast(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            int animations = 32;
            int frameCount = 15;
            int delayBetween = 3;

            Point3D location = player.Location;
            Map map = player.Map;

            for (int a = 1; a < animations + 1; a++)
            {
                int animation = a;

                Timer.DelayCall(TimeSpan.FromSeconds((animation - 1) * delayBetween), delegate
                {
                    if (player == null) return;
                    if (player.Location != location) return;

                    player.Say("Animation: " + animation.ToString());
                    player.Animate(animation, frameCount, 1, true, false, 0);
                });
            }
        }

        [Usage("Body Test")]
        [Description("Loop through all BodyValues starting at index")]
        public static void BodyTest(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            int startingBodyValue = 1;

            if (arg.Length == 1)
                startingBodyValue = arg.GetInt32(0);

            int totalBodyValues = 2000;

            Point3D location = player.Location;
            Map map = player.Map;

            for (int a = 0; a < totalBodyValues; a++)
            {
                int bodyTestValue = startingBodyValue + a;

                Timer.DelayCall(TimeSpan.FromSeconds(a), delegate
                {
                    if (player == null) return;
                    if (player.Location != location) return;

                    player.BodyValue = bodyTestValue;
                    player.Say("BodyValue: " + bodyTestValue.ToString());
                });
            }
        }

        [Usage("SetAllHues <huecolor>")]
        [Description("Sets All Equipped Items on Mobile to Hue Color")]
        public static void SetAllHues(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (e.Length == 1)
            {
                int hue = e.GetInt32(0);

                player.SendMessage("Target the mobile you wish to change item hues for.");
                player.Target = new SetAllHuesTarget(player, hue);
            }
        }

        private class SetAllHuesTarget : Target
        {
            public int m_Hue;

            public SetAllHuesTarget(Mobile from, int hue): base(100, false, TargetFlags.None)
            {
                m_Hue = hue;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile pm_Target = target as PlayerMobile;

                if (pm_Target != null)
                {
                    List<Layer> layers = new List<Layer>();

                    layers.Add(Layer.Arms);
                    layers.Add(Layer.Bracelet);
                    layers.Add(Layer.Cloak);
                    layers.Add(Layer.Earrings);
                    layers.Add(Layer.Gloves);
                    layers.Add(Layer.Helm);
                    layers.Add(Layer.InnerLegs);
                    layers.Add(Layer.InnerTorso);
                    layers.Add(Layer.MiddleTorso);
                    layers.Add(Layer.Neck);
                    layers.Add(Layer.OneHanded);
                    layers.Add(Layer.OuterLegs);
                    layers.Add(Layer.OuterTorso);
                    layers.Add(Layer.Pants);
                    layers.Add(Layer.Ring);
                    layers.Add(Layer.Shirt);
                    layers.Add(Layer.Shoes);
                    layers.Add(Layer.Talisman);
                    layers.Add(Layer.TwoHanded);
                    layers.Add(Layer.Waist);

                    foreach (Layer layer in layers)
                    {
                        Item item = pm_Target.FindItemOnLayer(layer);

                        if (item == null) continue;
                        if (item.Deleted) continue;
                        if (item == pm_Target.Backpack) continue;

                        item.Hue = m_Hue;
                    }

                    from.SendMessage("Setting Hue: " + m_Hue.ToString());
                }
            }
        }

        [Usage("SetAllAspect <aspect> <tierlevel")]
        [Description("Sets All Equipped Items on Mobile to be of <aspect> and <tierLevel>")]
        public static void SetAllAspect(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (e.Length == 2)
            {
                AspectEnum aspect = AspectEnum.Air;

                Enum.TryParse(e.GetString(0), true, out aspect);
                int tierLevel = e.GetInt32(1);

                player.SendMessage("Target the mobile you wish to change item aspects for.");
                player.Target = new SetAllAspectTarget(player, aspect, tierLevel);
            }
        }

        private class SetAllAspectTarget : Target
        {
            public AspectEnum m_Aspect;
            public int m_TierLevel;

            public SetAllAspectTarget(Mobile from, AspectEnum aspect, int tierLevel): base(100, false, TargetFlags.None)
            {
                m_Aspect = aspect;
                m_TierLevel = tierLevel;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile pm_Target = target as PlayerMobile;

                if (pm_Target != null)
                {
                    if (m_TierLevel >= AspectGear.MaxTierLevel)
                        m_TierLevel = AspectGear.MaxTierLevel;

                    if (m_TierLevel < 1)
                        m_TierLevel = 1;

                    List<Layer> layers = new List<Layer>();

                    layers.Add(Layer.Arms);
                    layers.Add(Layer.Bracelet);
                    layers.Add(Layer.Cloak);
                    layers.Add(Layer.Earrings);
                    layers.Add(Layer.Gloves);
                    layers.Add(Layer.Helm);
                    layers.Add(Layer.InnerLegs);
                    layers.Add(Layer.InnerTorso);
                    layers.Add(Layer.MiddleTorso);
                    layers.Add(Layer.Neck);
                    layers.Add(Layer.OneHanded);
                    layers.Add(Layer.OuterLegs);
                    layers.Add(Layer.OuterTorso);
                    layers.Add(Layer.Pants);
                    layers.Add(Layer.Ring);
                    layers.Add(Layer.Shirt);
                    layers.Add(Layer.Shoes);
                    layers.Add(Layer.Talisman);
                    layers.Add(Layer.Waist);

                    layers.Add(Layer.OneHanded);
                    layers.Add(Layer.TwoHanded);

                    foreach (Layer layer in layers)
                    {
                        Item item = pm_Target.FindItemOnLayer(layer);

                        if (item == null) continue;
                        if (item.Deleted) continue;
                        if (item == pm_Target.Backpack) continue;

                        if (item is BaseWeapon || item is BaseArmor && !(item is BaseShield))
                        {
                            item.Aspect = m_Aspect;
                            item.TierLevel = m_TierLevel;
                        }

                        else
                            item.Hue = AspectGear.GetAspectHue(m_Aspect);                        
                    }

                    AspectGear.CheckForAndUpdateAspectArmorProperties(pm_Target);

                    from.SendMessage("Setting Aspect: " + AspectGear.GetAspectName(m_Aspect));
                }
            }
        }

        [Usage("SetAllExceptional")]
        [Description("Sets All Equipped Items on Mobile to be of exceptional quality")]
        public static void SetAllExceptional(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Target the mobile you wish to set all items as exceptional for.");
            player.Target = new SetAllExceptionalTarget(player);
            
        }

        private class SetAllExceptionalTarget : Target
        {
            public SetAllExceptionalTarget(Mobile from): base(100, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile pm_Target = target as PlayerMobile;

                if (pm_Target != null)
                {
                    List<Layer> layers = new List<Layer>();

                    layers.Add(Layer.Helm);
                    layers.Add(Layer.Neck);
                    layers.Add(Layer.Arms);
                    layers.Add(Layer.Gloves);
                    layers.Add(Layer.InnerTorso);
                    layers.Add(Layer.Pants); 

                    layers.Add(Layer.OneHanded);
                    layers.Add(Layer.TwoHanded);

                    foreach (Layer layer in layers)
                    {
                        Item item = pm_Target.FindItemOnLayer(layer);

                        if (item == null) continue;
                        if (item.Deleted) continue;
                        if (item == pm_Target.Backpack) continue;

                        if (item is BaseWeapon || item is BaseArmor)
                        {
                            item.LootType = LootType.Regular;

                            item.Quality = Quality.Exceptional;
                            item.DisplayCrafter = true;
                            item.CraftedBy = pm_Target;
                            item.CrafterName = pm_Target.RawName;
                        }
                    }
                }
            }
        }

        #endregion

        private static void OnLogin(LoginEventArgs e)
        {
            Mobile from = e.Mobile;

            CheckAtrophies(from);

            from.FollowersMax = 5;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
                player.m_SessionStart = DateTime.UtcNow;

            if (AccountHandler.LockdownLevel > AccessLevel.Player)
            {
                string notice;

                Accounting.Account acct = from.Account as Accounting.Account;

                if (acct == null || !acct.HasAccess(from.NetState))
                {
                    if (from.AccessLevel == AccessLevel.Player)
                        notice = "The server is currently under lockdown. No players are allowed to log in at this time.";

                    else
                        notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";

                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(Disconnect), from);
                }

                else if (from.AccessLevel >= AccessLevel.Administrator)
                    notice = "The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";

                else
                    notice = "The server is currently under lockdown. You have sufficient access level to connect.";

                from.SendGump(new NoticeGump(1060637, 30720, notice, 0xFFC000, 300, 140, null, null));
                return;
            }

            if (player != null)
            {
                if ((player.Young || player.Companion) && !YoungChatListeners.Contains(player))
                    YoungChatListeners.Add(player);
                
                //TEST: Implement Fix for this?
                //pm_From.ClaimAutoStabledPets();

                if (player.AccessLevel > AccessLevel.Player)
                    player.Send(SpeedControl.MountSpeed);
            }
            
            //Damage Tracker
            player.m_DamageTracker = new DamageTracker(player);

            Guilds.OnLogin(player);
            Faction.OnLogin(player);
            TitlePersistance.OnLogin(player);
            AchievementsPersistance.OnLogin(player);
            CaptchaPersistance.OnLogin(player);          
            ChatPersistance.OnLogin(player);
            MHSPersistance.CheckAndCreateMHSAccountEntry(player);
            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(player);
            Societies.OnLogin(player);
            ArenaPersistance.OnLogin(player);
            ArenaPlayerSettings.OnLogin(player);            

            //Aspect Armor
            AspectGear.CheckForAndUpdateAspectArmorProperties(player);

            //OverloadProtectionSystem
            player.SystemOverloadActions = 0;
        }

        private static void EventSink_Connected(ConnectedEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                pm.m_SessionStart = DateTime.UtcNow;

                pm.BedrollLogout = false;
                pm.LastOnline = DateTime.UtcNow;
            }

            DisguiseTimers.StartTimer(e.Mobile);
        }      
        
        private static void EventSink_Disconnected(DisconnectedEventArgs e)
        {
            Mobile from = e.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                DesignContext.Remove(from);

                from.RevealingAction();

                foreach (Item item in context.Foundation.GetItems())
                    item.Location = context.Foundation.BanLocation;

                foreach (Mobile mobile in context.Foundation.GetMobiles())
                    mobile.Location = context.Foundation.BanLocation;

                context.Foundation.RestoreRelocatedEntities();
            }

            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player != null)
            {
                if (YoungChatListeners.Contains(player))
                    YoungChatListeners.Remove(player);

                TimeSpan gameTime = DateTime.UtcNow - player.m_SessionStart;

                player.m_GameTime += gameTime;

                player.LastOnline = DateTime.UtcNow;
                player.SetSallos(false);
            }

            DisguiseTimers.StopTimer(from);
        }

        private static void Disconnect(object state)
        {
            NetState ns = ((Mobile)state).NetState;

            if (ns != null)
                ns.Dispose();
        }

        private static void OnLogout(LogoutEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            Guilds.OnLogout(player);

            ArenaPlayerSettings.CheckCreateArenaPlayerSettings(player);

            if (player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                player.m_ArenaPlayerSettings.m_ArenaMatch.LeaveMatch(player, true, true);
        }

        public static bool IPMatch(PlayerMobile player1, PlayerMobile player2)
        {
            if (player1 == null) return false;
            if (player2 == null) return false;

            Account account1 = player1.Account as Account;
            Account account2 = player2.Account as Account;

            if (account1 == null) return false;
            if (account2 == null) return false;

            foreach (IPAddress player1IPAddress in account1.LoginIPs)
            {
                if (player1IPAddress == null)
                    continue;

                foreach (IPAddress player2IPAddress in account2.LoginIPs)
                {
                    if (player2IPAddress == null)
                        continue;

                    if (player1IPAddress.Equals(player2IPAddress))
                        return true;
                }
            }

            return false;
        }

        public void ResetRegenTimers()
        {
            //Reset Regen Timers
            if (m_HitsTimer != null)
            {
                m_HitsTimer.Stop();
                m_HitsTimer = null;

                m_HitsTimer = new Mobile.HitsTimer(this);
                m_HitsTimer.Start();
            }

            if (m_StamTimer != null)
            {
                m_StamTimer.Stop();
                m_StamTimer = null;

                m_StamTimer = new Mobile.StamTimer(this);
                m_StamTimer.Start();
            }

            if (m_ManaTimer != null)
            {
                m_ManaTimer.Stop();
                m_ManaTimer = null;

                m_ManaTimer = new Mobile.ManaTimer(this);
                m_ManaTimer.Start();
            }
        }

        public DateTime m_LastTrapPouchUse = DateTime.UtcNow;

        public Type m_LastItemIdWorldItemCountSearchType = null;
        public DateTime m_LastItemIdWorldItemCountSearch = DateTime.UtcNow;
        public int m_LastItemIdWorldItemCountSearchCount = 0;

        public List<BaseCreature> m_LyricAspectFailedBardingAttemptTargets = new List<BaseCreature>();
        public DateTime m_LyricAspectFailedBardingAttemptExpiration = DateTime.UtcNow;
        public double m_LyricAspectFailedBardingAttemptDamageReduction = 0;

        public DateTime m_ShadowAspectPostBackstabDamageReceivedReductionExpiration = DateTime.UtcNow;
        public double m_ShadowAspectPostBackstabDamageReceivedReduction = 0;

        public DateTime m_LastPassiveTamingSkillGain = DateTime.MinValue;
        public DateTime m_LastExperienceGain = DateTime.MinValue; 

        public double m_PassiveSkillGainRemaining = MaxPassiveSkillgainAllowed[0];

        public static double[] MaxPassiveSkillgainAllowed = new double[] {          
                                                        2.5, 2.5,    //0-5, 5-10
                                                        2.5, 2.5,    //10-15, 15-20
                                                        2.5, 2.5,    //20-25, 25-30
                                                        2.5, 2.5,    //30-35, 30-40
                                                        2.5, 2.5,    //40-45, 45-50
                                                        2.5, 2.5,    //50-55, 55-60
                                                        2.5, 2.5,    //60-65, 65-70
                                                        2.5, 2.5,    //70-75, 75-80
                                                        2.5, 2.5,    //80-85, 85-90
                                                        2.5, 2.5,    //90-95, 95-100
                                                        2.5, 2.5,    //100-105, 105-110
                                                        2.5, 2.5};   //110-115, 115-120

        public void CheckPassiveTamingReset(double oldSkillBase)
        {
            double animalTaming = Skills.AnimalTaming.Value;

            int oldRangeIndex = (int)(Math.Floor(oldSkillBase / SkillCheck.SkillRangeIncrement));
            int newRangeIndex = (int)(Math.Floor(animalTaming / SkillCheck.SkillRangeIncrement));

            if (oldRangeIndex == newRangeIndex)
                return;

            m_PassiveSkillGainRemaining = 0;

            for (int a = 0; a < MaxPassiveSkillgainAllowed.Length; a++)
            {
                double rangeBottom = a * SkillCheck.SkillRangeIncrement;
                double rangeTop = (a * SkillCheck.SkillRangeIncrement) + SkillCheck.SkillRangeIncrement;

                if (animalTaming >= rangeBottom && animalTaming < rangeTop)
                {
                    m_PassiveSkillGainRemaining = MaxPassiveSkillgainAllowed[a];
                    break;
                }
            }
        }

        public static int MurderCountDecayHours = 72;

        public static int DamageEntryClaimExpiration = 120;

        public static int MinDamageRequiredForPlayerDeath = 25;
        public static int MinDamageRequiredForPaladinDeath = 25;
        public static int MinDamageRequiredForMurdererDeath = 25;
        public static int MinIndividualDamageRequiredForDeathClaim = 10;
        
        public DamageDisplayMode m_ShowMeleeDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowSpellDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowFollowerDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowProvocationDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowPoisonDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowDamageTaken = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowFollowerDamageTaken = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowHealing = DamageDisplayMode.None;

        public int PlayerMeleeDamageTextHue = 34;
        public int PlayerSpellDamageTextHue = 117;
        public int PlayerFollowerDamageTextHue = 89;
        public int PlayerProvocationDamageTextHue = 2417;
        public int PlayerPoisonDamageTextHue = 63;
        public int PlayerDamageTakenTextHue = 0;
        public int PlayerFollowerDamageTakenTextHue = 0;
        public int PlayerHealingTextHue = 2213;

        public StealthStepsDisplayMode m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateMessage;
        public HenchmenSpeechDisplayMode m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Normal;

        public bool m_ShowAdminFilterText = true;

        public bool m_AutoStealth = true;

        private BaseShip m_ShipOccupied = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseShip ShipOccupied
        { 
            get { return m_ShipOccupied; }
            set 
            {
                BaseShip m_OldValue = m_ShipOccupied;

                m_ShipOccupied = value;

                if (m_OldValue != m_ShipOccupied && HasGump(typeof(ShipHotbarGump)))
                {
                    ShipHotbarGumpObject shipHotbarGumpObject = new ShipHotbarGumpObject();

                    CloseGump(typeof(ShipHotbarGump));
                    SendGump(new ShipHotbarGump(this, shipHotbarGumpObject));
                }
            }
        }
        
        public TitleCollection m_TitleCollection = null;
        public AchievementAccountEntry m_AchievementAccountEntry = null;
        public EnhancementsAccountEntry m_EnhancementsAccountEntry = null;
        public CaptchaAccountData m_CaptchaAccountData = null;             
        public Guild Guild = null;
        public GuildMemberEntry m_GuildMemberEntry = null;
        public GuildSettings m_GuildSettings = null;
        public SocietiesPlayerSettings m_SocietiesPlayerSettings = null;
        public ArenaPlayerSettings m_ArenaPlayerSettings = null;

        public AspectWeaponProfile m_AspectWeaponProfile = new AspectWeaponProfile();
        public AspectArmorProfile m_AspectArmorProfile = new AspectArmorProfile();

        public CompetitionContext m_CompetitionContext = null;

        public ArenaGumpObject m_ArenaGumpObject;

        #region Arena Properties

        public ArenaMatch m_ArenaMatch
        {
            get
            {
                if (m_ArenaPlayerSettings == null) return null;
                if (m_ArenaPlayerSettings.Deleted) return null;

                if (!ArenaMatch.IsValidArenaMatch(m_ArenaPlayerSettings.m_ArenaMatch, this, false)) return null;
                
                return m_ArenaPlayerSettings.m_ArenaMatch;
            }
        }

        public ArenaParticipant m_ArenaParticipant
        {
            get
            {
                if (m_ArenaPlayerSettings == null) return null;
                if (m_ArenaPlayerSettings.Deleted) return null;

                if (!ArenaMatch.IsValidArenaMatch(m_ArenaPlayerSettings.m_ArenaMatch, this, false)) return null;
                
                return m_ArenaPlayerSettings.m_ArenaMatch.GetParticipant(this);
            }
        }

        public ArenaFight m_ArenaFight
        {
            get
            {
                if (m_ArenaPlayerSettings == null) return null;
                if (m_ArenaPlayerSettings.Deleted) return null;

                if (!ArenaMatch.IsValidArenaMatch(m_ArenaPlayerSettings.m_ArenaMatch, this, false)) return null;
                
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight == null) return null;
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.Deleted) return null;
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.m_ArenaController == null) return null;
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.m_ArenaController.Deleted) return null;

                return m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight;
            }
        }

        public ArenaFight m_ActiveInsideArenaFight
        {
            get
            {
                if (m_ArenaPlayerSettings == null) return null;
                if (m_ArenaPlayerSettings.Deleted) return null;

                if (!ArenaMatch.IsValidArenaMatch(m_ArenaPlayerSettings.m_ArenaMatch, this, false)) return null;

                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight == null) return null;
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.Deleted) return null;
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.m_ArenaController == null) return null;
                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.m_ArenaController.Deleted) return null;

                if (m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight.IsWithinArena(Location, Map))
                    return m_ArenaPlayerSettings.m_ArenaMatch.m_ArenaFight;

                return null;
            }
        }


        #endregion

        public override bool KeepsItemsOnDeath 
        { 
            get 
            {
                if (ArenaGroupController.GetArenaGroupRegionAtLocation(Location, Map) != null)
                    return true;

                return (AccessLevel > AccessLevel.Player); 
            }
        }       

        public DateTime NextEmoteAllowed = DateTime.UtcNow;
        public static TimeSpan EmoteCooldownLong = TimeSpan.FromSeconds(120);
        public static TimeSpan EmoteCooldownShort = TimeSpan.FromSeconds(30);

        public DamageTracker m_DamageTracker = null;

        public FactionPlayerProfile m_FactionPlayerProfile = null;
        public EventCalendarAccount m_EventCalendarAccount = null;
        public MHSPlayerEntry m_MHSPlayerEntry = null;
        public WorldChatAccountEntry m_WorldChatAccountEntry = null;
        public ArenaAccountEntry m_ArenaAccountEntry = null;

        public static int SkillCap = 7000;
        public static int MaxSkillCap = 7200;
        
        private Food.SatisfactionLevelType m_SatisfactionLevel = Food.SatisfactionLevelType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public Food.SatisfactionLevelType SatisfactionLevel
        {
            get { return m_SatisfactionLevel; }
            set { m_SatisfactionLevel = value; }
        }

        private DateTime m_SatisfactionExpiration = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SatisfactionExpiration
        {
            get { return m_SatisfactionExpiration; }
            set { m_SatisfactionExpiration = value; }
        }

        public override void OnHitsRegen()
        {
            Food.CheckFoodHitsRegen(this);
        }

        public override void OnStamRegen()
        {
            Food.CheckFoodStamRegen(this);
        }

        public override void OnManaRegen()
        {
            Food.CheckFoodManaRegen(this);
        }

        public int SystemOverloadActions = 0;
        public static int SystemOverloadActionThreshold = 180; //Player flagged if attacking a single target this many times over the SystemOverloadInterval
        public static TimeSpan SystemOverloadInterval = TimeSpan.FromSeconds(60);

        public DateTime LastTeamSwitch = DateTime.MinValue;
        
        private int m_NumGoldCoinsGenerated;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NumGoldCoinsGenerated
        {
            get { return m_NumGoldCoinsGenerated; }
            set { m_NumGoldCoinsGenerated = value; }
        }

        #region PlayerFlags

        public PlayerFlag Flags
        {
            get { return m_Flags; }
            set { m_Flags = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PagingSquelched
        {
            get { return GetFlag(PlayerFlag.PagingSquelched); }
            set { SetFlag(PlayerFlag.PagingSquelched, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Glassblowing
        {
            get { return GetFlag(PlayerFlag.Glassblowing); }
            set { SetFlag(PlayerFlag.Glassblowing, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Masonry
        {
            get { return GetFlag(PlayerFlag.Masonry); }
            set { SetFlag(PlayerFlag.Masonry, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SandMining
        {
            get { return GetFlag(PlayerFlag.SandMining); }
            set { SetFlag(PlayerFlag.SandMining, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool StoneMining
        {
            get { return GetFlag(PlayerFlag.StoneMining); }
            set { SetFlag(PlayerFlag.StoneMining, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ToggleMiningStone
        {
            get { return GetFlag(PlayerFlag.ToggleMiningStone); }
            set { SetFlag(PlayerFlag.ToggleMiningStone, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool KarmaLocked
        {
            get { return GetFlag(PlayerFlag.KarmaLocked); }
            set { SetFlag(PlayerFlag.KarmaLocked, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AutoRenewInsurance
        {
            get { return GetFlag(PlayerFlag.AutoRenewInsurance); }
            set { SetFlag(PlayerFlag.AutoRenewInsurance, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UseOwnFilter
        {
            get { return GetFlag(PlayerFlag.UseOwnFilter); }
            set { SetFlag(PlayerFlag.UseOwnFilter, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AcceptGuildInvites
        {
            get { return GetFlag(PlayerFlag.AcceptGuildInvites); }
            set { SetFlag(PlayerFlag.AcceptGuildInvites, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasStatReward
        {
            get { return GetFlag(PlayerFlag.HasStatReward); }
            set { SetFlag(PlayerFlag.HasStatReward, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RefuseTrades
        {
            get { return GetFlag(PlayerFlag.RefuseTrades); }
            set { SetFlag(PlayerFlag.RefuseTrades, value); }
        }
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Murderer
        {
            get
            {
                return (MurderCounts >= Mobile.MurderCountsRequiredForMurderer);
            }
        }

        private PlayerMobile m_LastPlayerKilledBy;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile LastPlayerKilledBy
        {
            get { return m_LastPlayerKilledBy; }
            set { m_LastPlayerKilledBy = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShipMovement
        {
            get { return GetFlag(PlayerFlag.ShipMovement); }
            set { SetFlag(PlayerFlag.ShipMovement, value); }
        }

        public PlayerTitleColors TitleColorState { get; set; }
        public int SelectedTitleColorIndex;
        public EColorRarity SelectedTitleColorRarity;
        private int m_CanReprieve;
        private bool CanReprieveBool = false;
        public TimeSpan m_TimeSpanDied;
        public DateTime m_DateTimeDied;
        public TimeSpan m_TimeSpanResurrected;
        public List<string> PreviousNames = new List<string>();
        public DateTime HueModEnd { get; set; }
        public TimeSpan LoginElapsedTime { get; set; }        

        private DateTime m_Created = DateTime.UtcNow;
        public DateTime CreatedOn { set { m_Created = value; } get { return m_Created; } }
        public Boolean CloseRunebookGump;

        public TimeSpan m_MurderCountDecayTimeRemaining = TimeSpan.FromHours(MurderCountDecayHours);
        public int m_LifetimeMurderCounts = 0;

        public static TimeSpan RessPenaltyDuration = TimeSpan.FromHours(24);

        private DateTime m_RessPenaltyExpiration = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RessPenaltyExpiration
        {
            get 
            {
                if (m_RessPenaltyExpiration <= DateTime.UtcNow)
                {
                    m_RessPenaltyAccountWideAggressionRestriction = false;
                    m_RessPenaltyEffectivenessReductionCount = 0;
                }

                return m_RessPenaltyExpiration; 
            }

            set
            {
                m_RessPenaltyExpiration = value;

                if (m_RessPenaltyExpiration <= DateTime.UtcNow)
                {
                    m_RessPenaltyAccountWideAggressionRestriction = false;
                    m_RessPenaltyEffectivenessReductionCount = 0;
                }
            }
        }

        public bool CheckRessPenaltyFizzle()
        {
            return false;
        }

        public bool m_RessPenaltyAccountWideAggressionRestriction = false;
        public int m_RessPenaltyEffectivenessReductionCount = 0;

        public static int RessPenaltyAccountWideAggressionRestrictionFeePerCount = 200;
        public static int RessPenaltyEffectivenessReductionFeePerCount = 350;
        public static int RessPenaltyNoPenaltyFeePerCount = 500;

        public static double RessPenaltyDamageScalar = .10;
        public static double RessPenaltyHealingScalar = .10;
        public static double RessPenaltyFizzleScalar = .10;

        public static double PlayerVsCreaturePoisonDamageBonus = .5;

        public void EnterContestedRegion(bool ressingHere)
        {
        }

        private DateTime m_HideRestrictionExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime HideRestrictionExpiration
        {
            get { return m_HideRestrictionExpiration; }
            set { m_HideRestrictionExpiration = value; }
        }

        public DateTime m_RecallRestrictionExpiration;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RecallRestrictionExpiration
        {
            get { return m_RecallRestrictionExpiration; }
            set { m_RecallRestrictionExpiration = value; }
        }

        public override void DoHarmful(Mobile target, bool indirect)
        {
            if (target == null)
                return;

            bool pvpValid = false;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            if (target != this)
            {
                LastCombatTime = DateTime.UtcNow;
                target.LastCombatTime = DateTime.UtcNow;

                if (bc_Target != null)
                {
                    if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile && bc_Target.ControlMaster != this)
                    {
                        PlayerMobile pm_Controller = bc_Target.ControlMaster as PlayerMobile;
                        PlayerMobile pm_TargetController = bc_Target.ControlMaster as PlayerMobile;

                        bc_Target.LastPlayerCombatTime = DateTime.UtcNow;

                        PlayerVsPlayerCombatOccured(pm_Controller);
                        pm_TargetController.PlayerVsPlayerCombatOccured(this);
                    }
                }

                if (pm_Target != null)
                {
                    PlayerVsPlayerCombatOccured(pm_Target);
                    pm_Target.PlayerVsPlayerCombatOccured(this);
                }
            }

            base.DoHarmful(target, indirect);
        }

        public void CapStatMods(Mobile mobile)
        {
            //Bring Boosted Stat Durations Down to Normal Maximum If PvP Occurs
            TimeSpan MaximumPvPDuration = TimeSpan.FromMinutes(2);

            for (int i = 0; i < mobile.StatMods.Count; ++i)
            {
                StatMod check = mobile.StatMods[i];
                
                if (check.Type == StatType.Str || check.Type == StatType.Dex || check.Type == StatType.Int)
                {
                    if (check.Duration >= MaximumPvPDuration)
                        check.Duration = MaximumPvPDuration;
                }
            }
        }

        public override void Heal(int amount, Mobile from, bool message)
        {
            double healingAmount = (double)amount;

            double poisonScalar = 0;

            double ressPenaltyModifier = 0;
            double waterAspectModifier = 0;

            if (Poisoned)
                poisonScalar = SpellHelper.HealThroughPoisonScalar;

            if (RessPenaltyExpiration > DateTime.UtcNow && m_RessPenaltyEffectivenessReductionCount > 0)
                ressPenaltyModifier = (double)m_RessPenaltyEffectivenessReductionCount * PlayerMobile.RessPenaltyHealingScalar;

            PlayerMobile playerFrom = from as PlayerMobile;

            if (playerFrom != null)
            {
                if (playerFrom.RessPenaltyExpiration > DateTime.UtcNow && playerFrom.m_RessPenaltyEffectivenessReductionCount > 0)
                {
                    double newRessPenaltyModifier = (double)playerFrom.m_RessPenaltyEffectivenessReductionCount * PlayerMobile.RessPenaltyHealingScalar;

                    if (newRessPenaltyModifier > ressPenaltyModifier)
                        ressPenaltyModifier = newRessPenaltyModifier;
                }               
            }

            AspectArmorProfile aspectArmorProfile = AspectGear.GetAspectArmorProfile(this);

            //Water Aspect
            if (aspectArmorProfile != null)
            {
                if (aspectArmorProfile.m_Aspect == AspectEnum.Water)
                    waterAspectModifier = AspectGear.WaterHealingAmountReceived * (AspectGear.WaterHealingAmountReceivedPerTier * (double)aspectArmorProfile.m_TierLevel);
            }

            healingAmount = (double)amount * (1 - ressPenaltyModifier + waterAspectModifier);
            healingAmount *= poisonScalar;

            int adjustedHealingAmount = (int)(Math.Round(healingAmount));

            if (adjustedHealingAmount < 1)
                adjustedHealingAmount = 1;

            base.Heal(amount, from, message);
        }

        public override void OnHeal(ref int amount, Mobile from)
        {
            base.OnHeal(ref amount, from);

            SpecialAbilities.HealingOccured(from, this, amount);
        }

        //Easy UO Detection
        private Serial m_LastTarget;
        [CommandProperty(AccessLevel.GameMaster)]
        public Serial LastTarget
        {
            get { return m_LastTarget; }
            set { m_LastTarget = value; }
        }

        public bool m_UserOptHideFameTitles;
        public override bool ShowFameTitle
        {
            get { return m_UserOptHideFameTitles ? false : base.ShowFameTitle; }
        }

        private DateTime m_LastDeathByPlayer;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastDeathByPlayer
        {
            get { return m_LastDeathByPlayer; }
            set { m_LastDeathByPlayer = value; }
        }

        private int m_PirateScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PirateScore
        {
            get { return m_PirateScore; }
            set
            {
                m_PirateScore = value;
            }
        }
        
        private DateTime m_LastTownSquareNotification = DateTime.MinValue;
        public DateTime LastTownSquareNotification
        {
            get { return m_LastTownSquareNotification; }
            set { m_LastTownSquareNotification = value; }
        }

        private class CountAndTimeStamp
        {
            private int m_Count;
            private DateTime m_Stamp;

            public CountAndTimeStamp()
            {
            }

            public DateTime TimeStamp { get { return m_Stamp; } }
            public int Count
            {
                get { return m_Count; }
                set { m_Count = value; m_Stamp = DateTime.UtcNow; }
            }
        }

        private DesignContext m_DesignContext;

        private static List<PlayerMobile> m_YoungChatListeners = new List<PlayerMobile>();
        public static List<PlayerMobile> YoungChatListeners
        {
            get { return m_YoungChatListeners; }
        }

        private NpcGuild m_NpcGuild;
        private DateTime m_NpcGuildJoinTime;
        private DateTime m_NextBODTurnInTime;

        private TimeSpan m_NpcGuildGameTime;

        private PlayerFlag m_Flags;

        private bool m_IgnoreMobiles; // IgnoreMobiles should be moved to Server.Mobiles        
        
        private bool m_Companion;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public bool Companion
        {
            get { return m_Companion; }
            set
            {
                m_Companion = value;

                if (value)
                {
                    if (!YoungChatListeners.Contains(this))
                        YoungChatListeners.Add(this);
                }

                else
                {
                    if (YoungChatListeners.Contains(this))
                        YoungChatListeners.Remove(this);
                }
            }
        }

        private PlayerMobile m_CompanionTarget;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile CompanionTarget
        {
            get { return m_CompanionTarget; }
            set { m_CompanionTarget = value; }
        }

        private Point3D m_CompanionLastLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CompanionLastLocation
        {
            get { return m_CompanionLastLocation; }
            set { m_CompanionLastLocation = value; }
        }

        private Point3D m_LastLocation = Point3D.Zero;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LastLocation
        {
            get { return m_LastLocation; }
            set { m_LastLocation = value; }
        }

        private BaseInstrument m_LastInstrument;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseInstrument LastInstrument
        {
            get { return m_LastInstrument; }
            set { m_LastInstrument = value; }
        }

        private List<Mobile> m_AutoStabled = new List<Mobile>();
        public List<Mobile> AutoStabled
        {
            get { return m_AutoStabled; }
        }

        private List<Mobile> m_AllFollowers;
        public List<Mobile> AllFollowers
        {
            get
            {
                if (m_AllFollowers == null)
                    m_AllFollowers = new List<Mobile>();

                return m_AllFollowers;
            }
        }        

        private int m_Profession;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Profession
        {
            get { return m_Profession; }
            set { m_Profession = value; }
        }

        private int m_StepsTaken;
        public int StepsTaken
        {
            get { return m_StepsTaken; }
            set { m_StepsTaken = value; }
        }

        private bool m_IsStealthing;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsStealthing
        {
            get { return m_IsStealthing; }
            set { m_IsStealthing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreMobiles
        {
            get
            {
                return m_IgnoreMobiles;
            }

            set
            {
                if (m_IgnoreMobiles != value)
                {
                    m_IgnoreMobiles = value;
                    Delta(MobileDelta.Flags);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NpcGuild NpcGuild
        {
            get { return m_NpcGuild; }
            set { m_NpcGuild = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NpcGuildJoinTime
        {
            get { return m_NpcGuildJoinTime; }
            set { m_NpcGuildJoinTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextBODTurnInTime
        {
            get { return m_NextBODTurnInTime; }
            set { m_NextBODTurnInTime = value; }
        }

        private DateTime m_LastOnline;   
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastOnline
        {
            get
            {
                if (NetState != null)
                    return DateTime.UtcNow;

                return m_LastOnline; 
            }

            set { m_LastOnline = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public long LastMoved
        {
            get { return LastMoveTime; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NpcGuildGameTime
        {
            get { return m_NpcGuildGameTime; }
            set { m_NpcGuildGameTime = value; }
        }

        private DateTime m_AnkhNextUse;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AnkhNextUse
        {
            get { return m_AnkhNextUse; }
            set { m_AnkhNextUse = value; }
        }

        private Boolean m_TrueHidden = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public Boolean TrueHidden
        {
            get { return m_TrueHidden; }
            set { m_TrueHidden = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DisguiseTimeLeft
        {
            get { return DisguiseTimers.TimeRemaining(this); }
        }

        public static Direction GetDirection4(Point3D from, Point3D to)
        {
            int dx = from.X - to.X;
            int dy = from.Y - to.Y;

            int rx = dx - dy;
            int ry = dx + dy;

            Direction ret;

            if (rx >= 0 && ry >= 0)
                ret = Direction.West;
            else if (rx >= 0 && ry < 0)
                ret = Direction.South;
            else if (rx < 0 && ry < 0)
                ret = Direction.East;
            else
                ret = Direction.North;

            return ret;
        }

        public override bool AllowItemUse(Item item)
        {
            if (!ArenaFight.AttemptItemUsage(this, item))
                return false;

            return DesignContext.Check(this);
        }

        public override bool OnDragLift(Item item)
        {
            return base.OnDragLift(item);
        }

        public override bool OnDroppedItemInto(Item item, Container container, Point3D loc)
        {
            return base.OnDroppedItemInto(item, container, loc);
        }

        public override bool OnDroppedItemOnto(Item item, Item target)
        {
            return base.OnDroppedItemOnto(item, target);
        }

        public override bool OnDroppedItemToMobile(Item item, Mobile target)
        {
            return base.OnDroppedItemToMobile(item, target);
        }

        public override bool OnDroppedItemToWorld(Item item, Point3D location)
        {
            if (!base.OnDroppedItemToWorld(item, location))
                return false;
            
            BounceInfo bi = item.GetBounce();

            if (bi != null)
            {
                Type type = item.GetType();

                if (type.IsDefined(typeof(FurnitureAttribute), true) || type.IsDefined(typeof(DynamicFlipingAttribute), true))
                {
                    object[] objs = type.GetCustomAttributes(typeof(FlipableAttribute), true);

                    if (objs != null && objs.Length > 0)
                    {
                        FlipableAttribute fp = objs[0] as FlipableAttribute;

                        if (fp != null)
                        {
                            int[] itemIDs = fp.ItemIDs;

                            Point3D oldWorldLoc = bi.m_WorldLoc;
                            Point3D newWorldLoc = location;

                            if (oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y)
                            {
                                Direction dir = GetDirection4(oldWorldLoc, newWorldLoc);

                                if (itemIDs.Length == 2)
                                {
                                    switch (dir)
                                    {
                                        case Direction.North:
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East:
                                        case Direction.West: item.ItemID = itemIDs[1]; break;
                                    }
                                }
                                else if (itemIDs.Length == 4)
                                {
                                    switch (dir)
                                    {
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East: item.ItemID = itemIDs[1]; break;
                                        case Direction.North: item.ItemID = itemIDs[2]; break;
                                        case Direction.West: item.ItemID = itemIDs[3]; break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override int GetPacketFlags()
        {
            int flags = base.GetPacketFlags();

            if (m_IgnoreMobiles)
                flags |= 0x10;

            return flags;
        }

        public override int GetOldPacketFlags()
        {
            int flags = base.GetOldPacketFlags();

            if (m_IgnoreMobiles)
                flags |= 0x10;

            return flags;
        }

        public bool GetFlag(PlayerFlag flag)
        {
            return ((m_Flags & flag) != 0);
        }

        public void SetFlag(PlayerFlag flag, bool value)
        {
            if (value)
                m_Flags |= flag;
            else
                m_Flags &= ~flag;
        }

        public DesignContext DesignContext
        {
            get { return m_DesignContext; }
            set { m_DesignContext = value; }
        }
        
        private MountBlock m_MountBlock;

        public BlockMountType MountBlockReason
        {
            get
            {
                return (CheckBlock(m_MountBlock)) ? m_MountBlock.m_Type : BlockMountType.None;
            }
        }

        private static bool CheckBlock(MountBlock block)
        {
            return ((block is MountBlock) && block.m_Timer.Running);
        }

        private class MountBlock
        {
            public BlockMountType m_Type;
            public Timer m_Timer;

            public MountBlock(TimeSpan duration, BlockMountType type, Mobile mobile)
            {
                m_Type = type;

                m_Timer = Timer.DelayCall(duration, new TimerStateCallback<Mobile>(RemoveBlock), mobile);
            }

            private void RemoveBlock(Mobile mobile)
            {
                (mobile as PlayerMobile).m_MountBlock = null;
            }
        }

        public void SetMountBlock(BlockMountType type, TimeSpan duration, bool dismount)
        {
            if (dismount)
            {
                if (Mount != null)
                    Mount.Rider = null;
            }

            if ((m_MountBlock == null) || !m_MountBlock.m_Timer.Running || (m_MountBlock.m_Timer.Next < (DateTime.UtcNow + duration)))
                m_MountBlock = new MountBlock(duration, type, this);
        }

        public override void OnSkillInvalidated(Skill skill)
        {
            if (Core.AOS && skill.SkillName == SkillName.MagicResist)
                UpdateResistances();
        }

        public override int GetMaxResistance(ResistanceType type)
        {
            if (AccessLevel > AccessLevel.Player)
                return 100;

            int max = base.GetMaxResistance(type);

            if (type != ResistanceType.Physical && 60 < max && Spells.Fourth.CurseSpell.UnderEffect(this))
                max = 60;

            if (Core.ML && this.Race == Race.Elf && type == ResistanceType.Energy)
                max += 5; //Intended to go after the 60 max from curse

            return max;
        }

        protected override void OnRaceChange(Race oldRace)
        {
            ValidateEquipment();
            UpdateResistances();
        }

        public override int MaxWeight { get { return (((Core.ML && this.Race == Race.Human) ? 100 : 40) + (int)(3.5 * this.Str)); } }

        private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

        public override void OnNetStateChanged()
        {
            m_LastGlobalLight = -1;
            m_LastPersonalLight = -1;
        }

        public override void ComputeBaseLightLevels(out int global, out int personal)
        {
            global = LightCycle.ComputeLevelFor(this);

            bool racialNightSight = (Core.ML && this.Race == Race.Elf);

            if (this.LightLevel < 21 && (AosAttributes.GetValue(this, AosAttribute.NightSight) > 0 || racialNightSight))
                personal = 21;
            else
                personal = this.LightLevel;
        }

        public override void CheckLightLevels(bool forceResend)
        {
            NetState ns = this.NetState;

            if (ns == null)
                return;

            int global, personal;

            ComputeLightLevels(out global, out personal);

            if (!forceResend)
                forceResend = (global != m_LastGlobalLight || personal != m_LastPersonalLight);

            if (!forceResend)
                return;

            m_LastGlobalLight = global;
            m_LastPersonalLight = personal;

            ns.Send(GlobalLightLevel.Instantiate(global));
            ns.Send(new PersonalLightLevel(this, personal));
        }

        public override int GetMinResistance(ResistanceType type)
        {
            int magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
            int min = int.MinValue;

            if (magicResist >= 1000)
                min = 40 + ((magicResist - 1000) / 50);
            else if (magicResist >= 400)
                min = (magicResist - 400) / 15;

            if (min > MaxPlayerResistance)
                min = MaxPlayerResistance;

            int baseMin = base.GetMinResistance(type);

            if (min < baseMin)
                min = baseMin;

            return min;
        }

        private bool m_NoDeltaRecursion;

        public void ValidateEquipment()
        {
            if (m_NoDeltaRecursion || Map == null || Map == Map.Internal)
                return;

            if (this.Items == null)
                return;

            m_NoDeltaRecursion = true;
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(ValidateEquipment_Sandbox));
        }

        private void ValidateEquipment_Sandbox()
        {
            try
            {
                if (Map == null || Map == Map.Internal)
                    return;

                List<Item> items = this.Items;

                if (items == null)
                    return;

                bool moved = false;

                int str = this.Str;
                int dex = this.Dex;
                int intel = this.Int;

                #region Factions
                int factionItemCount = 0;
                #endregion

                Mobile from = this;
                
                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    Item item = items[i];                    

                    if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        bool drop = false;
                       
                        if (weapon.RequiredRace != null && weapon.RequiredRace != this.Race)
                            drop = true;

                        if (drop)
                        {
                            string name = weapon.Name;

                            if (name == null)
                                name = String.Format("#{0}", weapon.LabelNumber);

                            from.SendLocalizedMessage(1062001, name); // You can no longer wield your ~1_WEAPON~
                            from.AddToBackpack(weapon);
                            moved = true;
                        }
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;

                        bool drop = false;

                        if (!armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (!armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (armor.RequiredRace != null && armor.RequiredRace != this.Race)
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = armor.ComputeStatBonus(StatType.Str), strReq = armor.ComputeStatReq(StatType.Str);
                            int dexBonus = armor.ComputeStatBonus(StatType.Dex), dexReq = armor.ComputeStatReq(StatType.Dex);
                            int intBonus = armor.ComputeStatBonus(StatType.Int), intReq = armor.ComputeStatReq(StatType.Int);

                            if (dex < dexReq || (dex + dexBonus) < 1)
                                drop = true;
                            else if (str < strReq || (str + strBonus) < 1)
                                drop = true;
                            else if (intel < intReq || (intel + intBonus) < 1)
                                drop = true;
                        }

                        if (drop)
                        {
                            string name = armor.Name;

                            if (name == null)
                                name = String.Format("#{0}", armor.LabelNumber);

                            if (armor is BaseShield)
                                from.SendLocalizedMessage(1062003, name); // You can no longer equip your ~1_SHIELD~
                            else
                                from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack(armor);
                            moved = true;
                        }
                    }

                    else if (item is BaseClothing)
                    {
                        BaseClothing clothing = (BaseClothing)item;

                        bool drop = false;

                        if (!clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (!clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (clothing.RequiredRace != null && clothing.RequiredRace != this.Race)
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = clothing.ComputeStatBonus(StatType.Str);
                            int strReq = clothing.ComputeStatReq(StatType.Str);

                            if (str < strReq || (str + strBonus) < 1)
                                drop = true;
                        }

                        if (drop)
                        {
                            string name = clothing.Name;

                            if (name == null)
                                name = String.Format("#{0}", clothing.LabelNumber);

                            from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack(clothing);
                            moved = true;
                        }
                    }                    
                }

                if (moved)
                    from.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            finally
            {
                m_NoDeltaRecursion = false;
            }
        }

        public override void Delta(MobileDelta flag)
        {
            base.Delta(flag);

            if ((flag & MobileDelta.Stat) != 0)
                ValidateEquipment();
        }

        private bool _Sallos;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Sallos
        {
            get { return _Sallos; }
        }

        public void SetSallos(bool value)
        {
            _Sallos = value;
        }

        public override void RevealingAction()
        {
            if (m_DesignContext != null)
                return;

            Spells.Sixth.InvisibilitySpell.RemoveTimer(this);

            base.RevealingAction();

            TrueHidden = false;
            m_IsStealthing = false; // IsStealthing should be moved to Server.Mobiles
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Hidden
        {
            get { return base.Hidden; }

            set
            {
                if (value && HideRestrictionExpiration > DateTime.UtcNow)
                {
                    string hideRestrictionRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, HideRestrictionExpiration, false, false, false, true, true);

                    SendMessage("You are unable to hide for another " + hideRestrictionRemaining + ".");

                    return;
                }

                base.Hidden = value;

                RemoveBuff(BuffIcon.Invisibility);	//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

                if (!Hidden)
                    RemoveBuff(BuffIcon.HidingAndOrStealth);

                else // if( !InvisibilitySpell.HasTimer( this ) )                
                    BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));	//Hidden/Stealthing & You Are Hidden                
            }
        }

        public override void OnSubItemAdded(Item item)
        {
            if (AccessLevel < AccessLevel.GameMaster && item.IsChildOf(this.Backpack))
            {
                int maxWeight = WeightOverloading.GetMaxWeight(this);
                int curWeight = Mobile.BodyWeight + this.TotalWeight;

                if (curWeight > maxWeight)
                    this.SendLocalizedMessage(1019035, true, String.Format(" : {0} / {1}", curWeight, maxWeight));
            }
        }

        public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
        {
            if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
                return false;

            if ((target is BaseCreature && ((BaseCreature)target).IsInvulnerable) || target is PlayerVendor)
            {
                if (message)
                {
                    if (target.Title == null)
                        SendMessage("{0} cannot be harmed.", target.Name);

                    else
                        SendMessage("{0} {1} cannot be harmed.", target.Name, target.Title);
                }

                return false;
            }

            return base.CanBeHarmful(target, message, ignoreOurBlessedness);
        }

        public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
        {
            if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
                return false;

            return base.CanBeBeneficial(target, message, allowDead);
        }

        public override bool CheckContextMenuDisplay(IEntity target)
        {
            return (m_DesignContext == null);
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            if (item is BaseArmor || item is BaseWeapon)
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if (NetState != null)
                CheckLightLevels(false);
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item is BaseArmor || item is BaseWeapon)
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if (NetState != null)
                CheckLightLevels(false);
        }

        public override double ArmorRating
        {
            get
            {
                double rating = 0.0;

                AddArmorRating(ref rating, NeckArmor);
                AddArmorRating(ref rating, HandArmor);
                AddArmorRating(ref rating, HeadArmor);
                AddArmorRating(ref rating, ArmsArmor);
                AddArmorRating(ref rating, LegsArmor);
                AddArmorRating(ref rating, ChestArmor);
                AddArmorRating(ref rating, ShieldArmor);

                return VirtualArmor + VirtualArmorMod + rating;
            }
        }

        private void AddArmorRating(ref double rating, Item armor)
        {
            BaseArmor ar = armor as BaseArmor;

            if (ar != null)
                rating += ar.ArmorRatingScaled;
        }

        #region [Stats]Max
        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                int strBase = this.RawStr;
                int strOffs = GetStatOffset(StatType.Str);

                /*		if ( Core.AOS )
                        {
                            strBase = this.Str;
                            strOffs += AosAttributes.GetValue( this, AosAttribute.BonusHits );
                        }
                        else
                        {
                            strBase = this.RawStr;
                        }
                */
                return strBase + strOffs;
                //	return strBase;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int StamMax
        {
            get { return base.StamMax + AosAttributes.GetValue(this, AosAttribute.BonusStam); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax
        {
            get { return base.ManaMax + AosAttributes.GetValue(this, AosAttribute.BonusMana) + ((Core.ML && Race == Race.Elf) ? 20 : 0); }
        }
        #endregion

        #region Stat Getters/Setters

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Str
        {
            get
            {
                if (Core.ML && this.AccessLevel == AccessLevel.Player)
                    return Math.Min(base.Str, 150);

                return base.Str;
            }
            set
            {
                base.Str = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Int
        {
            get
            {
                if (Core.ML && this.AccessLevel == AccessLevel.Player)
                    return Math.Min(base.Int, 150);

                return base.Int;
            }
            set
            {
                base.Int = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Dex
        {
            get
            {
                if (Core.ML && this.AccessLevel == AccessLevel.Player)
                    return Math.Min(base.Dex, 150);

                return base.Dex;
            }
            set
            {
                base.Dex = value;
            }
        }

        #endregion

        public override bool Move(Direction d)
        {
            NetState ns = this.NetState;

            if (ns != null)
            {
                if (m_ArenaFight != null)
                {
                    if (m_ArenaFight.m_FightPhase == ArenaFight.FightPhaseType.StartCountdown)
                        return false;
                }

                if (IsHindered())
                    return false;

                if (HasGump(typeof(ResurrectGump)))
                {
                    if (Alive)
                        CloseGump(typeof(ResurrectGump));

                    else
                    {
                        SendLocalizedMessage(500111); // You are frozen and cannot move.
                        return false;
                    }
                }
            }

            if (CloseRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseGump(typeof(RuneTomeGump));

                CloseRunebookGump = false;
            }

            int speed = ComputeMovementSpeed(d);

            bool res;

            if (!Alive)
                Server.Movement.MovementImpl.IgnoreMovableImpassables = true;

            res = base.Move(d);

            Server.Movement.MovementImpl.IgnoreMovableImpassables = false;

            if (!res)
                return false;

            m_NextMovementTime += speed;

            return true;
        }

        public override bool CheckMovement(Direction d, out int newZ)
        {
            DesignContext context = m_DesignContext;

            if (context == null)
                return base.CheckMovement(d, out newZ);

            HouseFoundation foundation = context.Foundation;

            newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

            int newX = this.X, newY = this.Y;
            Movement.Movement.Offset(d, ref newX, ref newY);

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            return (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map);
        }

        public SkillName[] AnimalFormRestrictedSkills { get { return m_AnimalFormRestrictedSkills; } }

        private SkillName[] m_AnimalFormRestrictedSkills = new SkillName[]
		{
			SkillName.ArmsLore,	SkillName.Begging, SkillName.Discordance, SkillName.Forensics,
			SkillName.Inscribe, SkillName.ItemID, SkillName.Meditation, SkillName.Peacemaking,
			SkillName.Provocation, SkillName.RemoveTrap, SkillName.SpiritSpeak, SkillName.Stealing,	
			SkillName.TasteID
		};

        public override bool AllowSkillUse(SkillName skill)
        {
            if (!ArenaFight.AllowSkillUse(this, skill))            
                return false;            

            return DesignContext.Check(this);
        }

        private bool m_LastProtectedMessage;
        private int m_NextProtectionCheck = 10;

        public virtual void RecheckTownProtection()
        {
            m_NextProtectionCheck = 10;

            Regions.GuardedRegion reg = (Regions.GuardedRegion)this.Region.GetRegion(typeof(Regions.GuardedRegion));
            bool isProtected = (reg != null && !reg.IsDisabled());

            if (isProtected != m_LastProtectedMessage)
            {
                if (isProtected)
                    SendLocalizedMessage(500112); // You are now under the protection of the town guards.
                else
                    SendLocalizedMessage(500113); // You have left the protection of the town guards.

                m_LastProtectedMessage = isProtected;
            }
        }

        public override void MoveToWorld(Point3D loc, Map map)
        {
            base.MoveToWorld(loc, map);

            if (CloseRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseGump(typeof(RuneTomeGump));

                CloseRunebookGump = false;
            }

            RecheckTownProtection();
        }

        public override void SetLocation(Point3D loc, bool isTeleport)
        {
            if (CloseRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseGump(typeof(RuneTomeGump));

                CloseRunebookGump = false;
            }

            if (!isTeleport && AccessLevel == AccessLevel.Player)
            {
                // moving, not teleporting
                int zDrop = (this.Location.Z - loc.Z);

                if (zDrop > 20) // we fell more than one story
                    Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
            }

            base.SetLocation(loc, isTeleport);

            if (isTeleport || --m_NextProtectionCheck == 0)
                RecheckTownProtection();
        }

        public override void UpdateRegion()
        {
            Region newRegion = Region.Find(Location, Map);
            
            base.UpdateRegion();
        }

        public override void OnRegionChange(Region Old, Region New)
        {
            if (New.IndexedName == IndexedRegionName.NotIndexed)
                return;            
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from == this)
            {   
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null)
                {
                    if (Alive && house.InternalizedVendors.Count > 0 && house.IsOwner(this))
                        list.Add(new CallbackEntry(6204, new ContextCallback(GetVendor)));
                }
            }
        }

        private void CancelProtection()
        {
        }
        
        private void ToggleTrades()
        {
            RefuseTrades = !RefuseTrades;
        }

        private void GetVendor()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (CheckAlive() && house != null && house.IsOwner(this) && house.InternalizedVendors.Count > 0)
            {
                CloseGump(typeof(ReclaimVendorGump));
                SendGump(new ReclaimVendorGump(house));
            }
        }

        private delegate void ContextCallback();

        private class CallbackEntry : ContextMenuEntry
        {
            private ContextCallback m_Callback;

            public CallbackEntry(int number, ContextCallback callback): this(number, -1, callback)
            {
            }

            public CallbackEntry(int number, int range, ContextCallback callback): base(number, range)
            {
                m_Callback = callback;
            }

            public override void OnClick()
            {
                if (m_Callback != null)
                    m_Callback();
            }
        }

        public override void DisruptiveAction()
        {
            if (Meditating)            
                RemoveBuff(BuffIcon.ActiveMeditation);            

            base.DisruptiveAction();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this == from && !Warmode)
            {
                IMount mount = Mount;

                if (mount != null && !DesignContext.Check(this))
                    return;
            }

            base.OnDoubleClick(from);
        }

        public override void DisplayPaperdollTo(Mobile to)
        {
            if (DesignContext.Check(this))
                base.DisplayPaperdollTo(to);
        }

        private static bool m_NoRecursion;

        public override bool CheckEquip(Item item)
        {
            if (!base.CheckEquip(item))
                return false;
                                    
            if (this.AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && this.HasTrade)
            {
                BounceInfo bounce = item.GetBounce();

                if (bounce != null)
                {
                    if (bounce.m_Parent is Item)
                    {
                        Item parent = (Item)bounce.m_Parent;

                        if (parent == this.Backpack || parent.IsChildOf(this.Backpack))
                            return true;
                    }
                    else if (bounce.m_Parent == this)
                    {
                        return true;
                    }
                }

                SendLocalizedMessage(1004042); // You can only equip what you are already carrying while you have a trade pending.
                return false;
            }

            return true;
        }

        public override bool CheckTrade(Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            int msgNum = 0;

            //// no trades allowed inside the arena areas
            //if (Region is ArenaRegion || to.Region is ArenaRegion)
            //{
            //    SendMessage("Trading is not allowed in this area");
            //    return false;
            //}

            if (cont == null)
            {
                if (to.Holding != null)
                    msgNum = 1062727; // You cannot trade with someone who is dragging something.
                else if (this.HasTrade)
                    msgNum = 1062781; // You are already trading with someone else!
                else if (to.HasTrade)
                    msgNum = 1062779; // That person is already involved in a trade
                else if (to is PlayerMobile && ((PlayerMobile)to).RefuseTrades)
                    msgNum = 1154111; // ~1_NAME~ is refusing all trades.
            }

            if (msgNum == 0)
            {
                if (cont != null)
                {
                    plusItems += cont.TotalItems;
                    plusWeight += cont.TotalWeight;
                }

                if (this.Backpack == null || !this.Backpack.CheckHold(this, item, false, checkItems, plusItems, plusWeight))
                    msgNum = 1004040; // You would not be able to hold this if the trade failed.
                else if (to.Backpack == null || !to.Backpack.CheckHold(to, item, false, checkItems, plusItems, plusWeight))
                    msgNum = 1004039; // The recipient of this trade would not be able to carry this.
                else
                    msgNum = CheckContentForTrade(item);
            }

            if (msgNum != 0)
            {
                if (message)
                {
                    if (msgNum == 1154111)
                        SendLocalizedMessage(msgNum, to.Name);
                    else
                        SendLocalizedMessage(msgNum);
                }

                return false;
            }

            return true;
        }

        private static int CheckContentForTrade(Item item)
        {
            if (item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None)
                return 1004044; // You may not trade trapped items.

            if (SkillHandlers.StolenItem.IsStolen(item))
                return 1004043; // You may not trade recently stolen items.

            if (item is Container)
            {
                foreach (Item subItem in item.Items)
                {
                    int msg = CheckContentForTrade(subItem);

                    if (msg != 0)
                        return msg;
                }
            }

            return 0;
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (!base.CheckNonlocalDrop(from, item, target))
                return false;

            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            Container pack = this.Backpack;
            if (from == this && this.HasTrade && (target == pack || target.IsChildOf(pack)))
            {
                BounceInfo bounce = item.GetBounce();

                if (bounce != null && bounce.m_Parent is Item)
                {
                    Item parent = (Item)bounce.m_Parent;

                    if (parent == pack || parent.IsChildOf(pack))
                        return true;
                }

                SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
                return false;
            }

            return true;
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            CheckLightLevels(false);
            
            BaseShip ship = BaseShip.FindShipAt(Location, Map);

            if (ship == null)
                ShipOccupied = null;
            else
                ShipOccupied = ship;

            if (m_ArenaFight != null)
                m_ArenaFight.OnLocationChanged(this);
            
            DesignContext context = m_DesignContext;

            if (context == null || m_NoRecursion)
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            int newX = this.X, newY = this.Y;
            int newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            if (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map)
            {
                if (Z != newZ)
                    Location = new Point3D(X, Y, newZ);

                m_NoRecursion = false;
                return;
            }

            Location = new Point3D(foundation.X, foundation.Y, newZ);
            Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                bool allowMoveOver = (!Alive || !m.Alive || IsDeadBondedFollower || m.IsDeadBondedFollower) || (Hidden && AccessLevel > AccessLevel.Player);

                if (allowMoveOver)
                    return true;
            }

            return base.OnMoveOver(m);
        }

        public override bool CheckShove(Mobile shoved)
        {
            bool InStamFreeRange = (int)GetDistanceToSqrt(StamFreeMoveSource) <= BaseCreature.StamFreeMoveRange;

            //Currently Allowed Stamina-Free Movement
            if (StamFreeMoveExpiration > DateTime.UtcNow && InStamFreeRange || ShipOccupied != null)
                return true;

            if (shoved.Blessed)
                return true;            

            return base.CheckShove(shoved);
        }

        protected override void OnMapChange(Map oldMap)
        {
            if (oldMap == Map.Ilshenar)
                this.LightLevel = 0;

            if (AccessLevel == AccessLevel.Player)
                if (Mount != null)
                    Mount.Rider = null;

            if (m_ArenaFight != null)
                m_ArenaFight.OnMapChanged(this);
            
            DesignContext context = m_DesignContext;

            if (context == null || m_NoRecursion)
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            if (Map != foundation.Map)
                Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override void OnBeneficialAction(Mobile target, bool isCriminal)
        {
            base.OnBeneficialAction(target, isCriminal);
        }

        public override void CriminalAction(bool message)
        {
            base.CriminalAction(message);
        }

        public override bool CheckDisrupt(int damage, Mobile from)
        {
            bool disrupt = true;

            BaseCreature bc_From = from as BaseCreature;

            if (bc_From != null)
            {
                if (bc_From.IsControlledCreature())
                {
                    double disruptChance = (double)damage * bc_From.TamedDamageAgainstPlayerDisruptChance;

                    if (Utility.RandomDouble() >= disruptChance)
                        disrupt = false;
                }
            }

            if (ShipOccupied != null)
                return false;

            return disrupt;
        }

        public override bool ReadyForSwing()
        {
            BaseWeapon weapon = Weapon as BaseWeapon;

            if (weapon == null)
                return false;

            TimeSpan stationaryDelayRequired = weapon.GetStationaryDelayRequired(this);

            if (DateTime.UtcNow < LastMovement + stationaryDelayRequired)
                return false;

            if (DateTime.UtcNow < LastSwingTime + NextSwingDelay)
                return false;

            return true;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            BaseCreature bc_From = from as BaseCreature;

            BandageContext bandageContext = BandageContext.GetContext(this);

            if (bandageContext != null)
            {
                bool causeSlip = true;

                if (bc_From != null)
                {
                    if (!CheckDisrupt(amount, bc_From))
                        causeSlip = false;
                }

                if (causeSlip)
                    bandageContext.Slip();
            }
            
            WeightOverloading.FatigueOnDamage(this, amount, 1.0);

            base.OnDamage(amount, from, willKill);
        }

        public PlayerCombatTimer m_PlayerCombatTimer;

        public class PlayerCombatTimer : Timer
        {
            private PlayerMobile m_Player;

            public PlayerCombatTimer(PlayerMobile player): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Player = player;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null)
                    Stop();

                if (m_Player.Deleted)
                    Stop();

                if (!m_Player.RecentlyInPlayerCombat)
                {
                    AspectGear.CheckForAndUpdateAspectArmorProperties(m_Player);
                    Stop();
                }
            }
        }        

        public override void Resurrect()
        {
            bool arenaOverride = false;

            if (m_ArenaMatch != null)
            {
                if (m_ArenaFight != null)
                {
                    if (m_ArenaMatch.m_ArenaGroupController.IsWithinArenaGroupRegion(Location, Map))
                        arenaOverride = true;
                }
            }

            if (!arenaOverride && MurderCounts > Mobile.MurderCountsRequiredForMurderer && AccessLevel == AccessLevel.Player)
            {
                SendMessage(63, "Select a resurrection option and click 'Apply' twice to proceed.");

                CloseGump(typeof(MurderPenaltyGump));
                SendGump(new MurderPenaltyGump(this, false, 0, false));

                return;
            }

            m_TimeSpanResurrected = this.GameTime;

            if (CloseRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseGump(typeof(RuneTomeGump));

                CloseRunebookGump = false;
            }

            bool wasAlive = this.Alive;

            base.Resurrect();

            if (Alive && !wasAlive)
            {
                Item deathRobe = new DeathRobe();

                if (Backpack.FindItemByType<DeathRobe>() != null)
                    deathRobe.Delete();

                else if (!EquipItem(deathRobe))
                    deathRobe.Delete();
            }

            PlaySound(0x214);
            FixedEffect(0x376A, 10, 16);

            if (Fame > 0)
            {
                int amount = Fame / 10;

                Misc.FameKarmaTitles.AwardFame(this, -amount, true);
            }
        }

        public override void OnAfterResurrect()
        {
            if (!Warmode && NetState != null && AccessLevel == Server.AccessLevel.Player)
            {
                IPooledEnumerable enu = GetClientsInRange(Core.GlobalMaxUpdateRange);
                foreach (NetState state in enu)
                {
                    if (state == null || NetState == null || state.Mobile == this || !CanSee(state.Mobile))
                        continue;

                    Send(MobileIncoming.Create(NetState, this, state.Mobile));

                    if (NetState.StygianAbyss)
                    {
                        if (Poison != null)
                            Send(new HealthbarPoison(state.Mobile));

                        if (Blessed || YellowHealthbar)
                            Send(new HealthbarYellow(state.Mobile));
                    }

                    if (IsDeadBondedFollower)
                        Send(new BondedStatus(0, state.Mobile.Serial, 1));

                    if (ObjectPropertyList.Enabled)
                    {
                        Send(OPLPacket);
                    }
                }

                enu.Free();
            }

            //Reapply Kin Paint
            if (KinPaintHue != -1 && KinPaintExpiration > DateTime.UtcNow)
                HueMod = KinPaintHue;

            //Player Enhancement Customization: Lifegiver
            bool reborn = false; //PlayerEnhancementPersistance.IsCustomizationEntryActive(this, CustomizationType.Reborn);

            if (reborn)
                CustomizationAbilities.Reborn(this);

            bool arenaOverride = false;

            if (m_ArenaMatch != null)
            {
                if (m_ArenaFight != null)
                {
                    if (m_ArenaMatch.m_ArenaGroupController.IsWithinArenaGroupRegion(Location, Map))
                        arenaOverride = true;
                }
            }

            if (!arenaOverride)
                AspectGear.PlayerResurrected(this);

            Stam = StamMax;
        }

        public override double RacialSkillBonus
        {
            get
            {
                if (Core.ML && this.Race == Race.Human)
                    return 20.0;

                return 0;
            }
        }

        public override void OnWarmodeChanged()
        {
            if (!Alive && NetState != null && AccessLevel == Server.AccessLevel.Player)
            {
                IPooledEnumerable enu = GetClientsInRange(Core.GlobalMaxUpdateRange);
                try
                {
                    foreach (NetState state in enu)
                    {
                        if (state == null || NetState == null)
                            continue;

                        if (state.Mobile == this || state.Mobile == null)
                            continue;

                        if (state.Mobile.AccessLevel > AccessLevel.Player)
                            continue;

                        if (!Warmode)
                        {
                            if (Utility.InUpdateRange(state.Mobile.Location, Location))
                                Send(state.Mobile.RemovePacket);
                        }

                        else
                        {
                            if (state.Mobile.Alive && !state.Mobile.Hidden)
                            {
                                Send(MobileIncoming.Create(NetState, this, state.Mobile));

                                if (NetState.StygianAbyss)
                                {
                                    if (Poison != null)
                                        Send(new HealthbarPoison(state.Mobile));

                                    if (Blessed || YellowHealthbar)
                                        Send(new HealthbarYellow(state.Mobile));
                                }

                                if (IsDeadBondedFollower)
                                    Send(new BondedStatus(0, state.Mobile.Serial, 1));

                                if (ObjectPropertyList.Enabled)
                                    Send(OPLPacket);
                            }
                        }

                    }
                }
                catch (NullReferenceException exception)
                {
                    // SNUFF IT LOL
                }

                enu.Free();
            }
        }

        private List<Item> m_EquipSnapshot;
        public List<Item> EquipSnapshot
        {
            get { return m_EquipSnapshot; }
        }

        public void YoungPlayerChat(string text)
        {
            string message = string.Format("[{0}{1}]: {2}", Name, Companion ? " [Companion]" : "", text);
            foreach (var young in YoungChatListeners)
                young.SendMessage(32, message);
        }

        public override bool OnBeforeDeath()
        {
            NetState state = NetState;

            if (state != null)
                state.CancelAllTrades();

            m_EquipSnapshot = new List<Item>(this.Items);

            DropHolding();

            return base.OnBeforeDeath();
        }

        public override DeathMoveResult GetParentMoveResultFor(Item item)
        {         
            DeathMoveResult res = base.GetParentMoveResultFor(item);

            if (res == DeathMoveResult.MoveToCorpse && item.Movable && (Region.IsBlessedRegion || (Young && !Criminal)))
                res = DeathMoveResult.MoveToBackpack;

            return res;
        }

        public override DeathMoveResult GetInventoryMoveResultFor(Item item)
        {
            DeathMoveResult result;

            result = base.GetInventoryMoveResultFor(item);

            if (result == DeathMoveResult.MoveToCorpse && item.Movable && (Region.IsBlessedRegion || (Young && !Criminal)))
                result = DeathMoveResult.MoveToBackpack;

            return result;
        }

        public override void OnDeath(Container corpse)
        {
            base.OnDeath(corpse);

            SpecialAbilities.ClearSpecialEffects(this);

            if (CloseRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseGump(typeof(RuneTomeGump));

                CloseRunebookGump = false;
            }

            BandageContext bandageContext = BandageContext.GetContext(this);

            if (bandageContext != null)
                bandageContext.StopHeal();

            m_EquipSnapshot = null;

            if (KinPaintHue == -1)
                HueMod = -1;

            NameMod = null;
            SetHairMods(-1, -1);

            PolymorphSpell.StopTimer(this);
            IncognitoSpell.StopTimer(this);
            DisguiseTimers.RemoveTimer(this);

            EndAction(typeof(PolymorphSpell));
            EndAction(typeof(IncognitoSpell));

            MeerMage.StopEffect(this, false);

            SkillHandlers.StolenItem.ReturnOnDeath(this, corpse);

            if (m_PermaFlags.Count > 0)
            {
                m_PermaFlags.Clear();

                if (corpse is Corpse)
                    ((Corpse)corpse).Criminal = true;

                if (SkillHandlers.Stealing.ClassicMode)
                    Criminal = true;
            }

            //Determine if Murdered
            List<Mobile> killers = new List<Mobile>();
            List<Mobile> toGive = new List<Mobile>();

            foreach (AggressorInfo ai in this.Aggressors)
            {
                if (ai != null && ai.Attacker != null && ai.Attacker.Player && ai.CanReportMurder && !ai.Reported)
                {
                    if (ai.Attacker.AccessLevel == Server.AccessLevel.Player)
                    {
                        killers.Add(ai.Attacker);

                        ai.Reported = true;
                        ai.CanReportMurder = false;
                    }
                }

                if (ai != null && ai.Attacker != null && ai.Attacker.Player && (DateTime.UtcNow - ai.LastCombatTime) < TimeSpan.FromSeconds(30.0) && !toGive.Contains(ai.Attacker))
                    toGive.Add(ai.Attacker);
            }

            foreach (AggressorInfo ai in this.Aggressed)
            {
                if (ai != null && ai.Defender != null && ai.Defender.Player && (DateTime.UtcNow - ai.LastCombatTime) < TimeSpan.FromSeconds(30.0) && !toGive.Contains(ai.Defender))
                    toGive.Add(ai.Defender);
            }

            foreach (Mobile g in toGive)
            {
                int n = Notoriety.Compute(g, this);

                int theirKarma = this.Karma, ourKarma = g.Karma;
                bool innocent = (n == Notoriety.Innocent);
                bool criminal = (n == Notoriety.Criminal || n == Notoriety.Murderer);

                int fameAward = this.Fame / 200;
                int karmaAward = 0;

                if (innocent)
                    karmaAward = (ourKarma > -2500 ? -850 : -110 - (this.Karma / 100));

                else if (criminal)
                    karmaAward = 50;

                Server.Misc.FameKarmaTitles.AwardFame(g, fameAward, false);
                Server.Misc.FameKarmaTitles.AwardKarma(g, karmaAward, true);
            }

            if (NpcGuild == NpcGuild.ThievesGuild)
                return;

            bool justiceDisabledZone = SpellHelper.InBuccs(Map, Location) || SpellHelper.InYewOrcFort(Map, Location) || SpellHelper.InYewCrypts(Map, Location) ||
                                        GreyZoneTotem.InGreyZoneTotemArea(Location, Map) || Hotspot.InHotspotArea(Location, Map, true);

            //Has Valid Killer(s)
            if (killers.Count > 0 && !justiceDisabledZone)            
                new ReportMurdererGump.GumpTimer(this, killers, DateTime.UtcNow, Location,Map).Start();            

            //Player, Paladin, and Murderer Handling
            bool killedByPlayer = false;
            bool killedByPaladin = false;
            bool killedByMurderer = false;

            double totalPlayerDamage = 0;
            double totalPaladinDamage = 0;
            double totalMurdererDamage = 0;

            int totalDamage = 0;

            //TEST: GUILD
            bool playerInGuild = false; //Guild != null;

            Dictionary<PlayerMobile, int> damageInflicted = new Dictionary<PlayerMobile, int>();

            //If Not in Justice-Free Zone
            if (!justiceDisabledZone)
            {
                //Damage Entries for Player
                foreach (DamageEntry de in this.DamageEntries)
                {
                    if (de == null)
                        continue;

                    if (de.HasExpired)
                        continue;

                    if (de.Damager == this)
                        continue;

                    if (de.LastDamage + TimeSpan.FromSeconds(DamageEntryClaimExpiration) <= DateTime.UtcNow)
                        continue;

                    PlayerMobile playerDamager = de.Damager as PlayerMobile;
                    PlayerMobile creatureOwner = null;
                    BaseCreature bc_Creature = de.Damager as BaseCreature;

                    bool sameGuild = false;

                    //Same Guild: Ignore Damage
                    if (playerDamager != null)
                    {
                        if (Guild != null && playerDamager.Guild != null)
                        {
                            if (Guild == playerDamager.Guild)
                                continue;
                        }

                        if (damageInflicted.ContainsKey(playerDamager))
                            damageInflicted[playerDamager] += de.DamageGiven;

                        else
                            damageInflicted.Add(playerDamager, de.DamageGiven);
                    }

                    //Damager is Creature: And Is Controlled By Someone
                    else if (bc_Creature != null)
                    {
                        if (bc_Creature.Summoned && bc_Creature.SummonMaster != null)
                        {
                            if (bc_Creature.SummonMaster is PlayerMobile)
                                creatureOwner = bc_Creature.SummonMaster as PlayerMobile;
                        }

                        else if (bc_Creature.Controlled && bc_Creature.ControlMaster != null)
                        {
                            if (bc_Creature.ControlMaster is PlayerMobile)
                                creatureOwner = bc_Creature.ControlMaster as PlayerMobile;
                        }

                        else if (bc_Creature.BardProvoked && bc_Creature.BardMaster != null)
                        {
                            if (bc_Creature.BardMaster is PlayerMobile)
                                creatureOwner = bc_Creature.BardMaster as PlayerMobile;
                        }

                        //Creature is Controlled by Player in Some Fashion
                        if (creatureOwner != null)
                        {
                            if (creatureOwner == this)
                                continue;

                            if (creatureOwner.Guild != null && this.Guild != null)
                            {
                                if (this.Guild == creatureOwner.Guild)
                                    continue;
                            }

                            if (damageInflicted.ContainsKey(creatureOwner))
                                damageInflicted[creatureOwner] += de.DamageGiven;
                            else
                                damageInflicted.Add(creatureOwner, de.DamageGiven);
                        }
                    }
                }
            }

            PlayerMobile highestPlayerDamager = null;
            PlayerMobile highestPaladinDamager = null;
            PlayerMobile highestMurdererDamager = null;

            int highestPlayerDamage = 0;
            int highestPaladinDamage = 0;
            int highestMurdererDamage = 0;

            int playerClaimCount = 0;
            int paladinClaimCount = 0;
            int murdererClaimCount = 0;

            //Check Player Damage Entries
            foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
            {
                if (pair.Key == null) continue;

                PlayerMobile playerDamager = pair.Key;

                if (playerDamager == null) continue;
                if (playerDamager.Deleted) continue;

                int damageAmount = pair.Value;

                //Determine Claims
                totalDamage += damageAmount;
                totalPlayerDamage += damageAmount;

                //Player Damage
                if (damageAmount >= MinIndividualDamageRequiredForDeathClaim)
                {
                    playerClaimCount++;

                    if (damageAmount > highestPlayerDamage)
                    {
                        highestPlayerDamager = playerDamager;
                        highestPlayerDamage = damageAmount;
                    }
                }

                //Murderer Damage
                else if (playerDamager.Murderer)
                {
                    totalMurdererDamage += damageAmount;

                    if (damageAmount >= MinIndividualDamageRequiredForDeathClaim)
                    {
                        murdererClaimCount++;

                        if (damageAmount > highestMurdererDamage)
                        {
                            highestMurdererDamager = playerDamager;
                            highestMurdererDamage = damageAmount;
                        }
                    }
                }
            }

            //If Non-Instant Killed: i.e by GM or Explosive Chest
            if (totalDamage > 0)
            {
                if (totalPlayerDamage >= MinDamageRequiredForPlayerDeath && playerClaimCount > 0 && highestPlayerDamager != null)
                    killedByPlayer = true;

                if (totalMurdererDamage >= MinDamageRequiredForMurdererDeath && murdererClaimCount > 0 && highestMurdererDamager != null)
                    killedByMurderer = true;
            }

            //Last Mobile to Damage Player
            Mobile killer = FindMostRecentDamager(true);

            PlayerMobile pm_Killer = killer as PlayerMobile;

            if (killer is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)killer;

                Mobile master = bc.GetMaster();

                if (master != null)
                    killer = master;
            }

            //Player Enhancement Customization: Carnage and Violent Death
            bool carnage = false; //PlayerEnhancementPersistance.IsCustomizationEntryActive(killer, CustomizationType.Carnage);
            bool violentDeath = false; //PlayerEnhancementPersistance.IsCustomizationEntryActive(this, CustomizationType.ViolentDeath);

            if (carnage && Utility.RandomDouble() >= .75)
                carnage = false;

            if (violentDeath && Utility.RandomDouble() >= .75)
                violentDeath = false;

            if ((carnage || violentDeath))
                CustomizationAbilities.PlayerDeathExplosion(Location, Map, carnage, violentDeath);
            
            if (m_ArenaFight != null)
                m_ArenaFight.OnDeath(this, corpse);         
            
            if (m_BuffTable != null)
            {
                List<BuffInfo> list = new List<BuffInfo>();

                foreach (BuffInfo buff in m_BuffTable.Values)
                {
                    if (!buff.RetainThroughDeath)
                    {
                        list.Add(buff);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    RemoveBuff(list[i]);
                }
            }

            OnWarmodeChanged();
        }
        
        private List<Mobile> m_PermaFlags = new List<Mobile>();
        private List<Mobile> m_VisList;
        private TimeSpan m_GameTime;
        private DateTime m_SessionStart;
        private DateTime m_LastEscortTime;
        private DateTime m_LastPetBallTime;
        private DateTime m_NextSmithBulkOrder;
        private DateTime m_NextTailorBulkOrder;
        private DateTime m_SavagePaintExpiration;
        private SkillName m_Learning = (SkillName)(-1);
        private DateTime m_NextFireAttempt = DateTime.MinValue;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextFireAttempt
        {
            get { return m_NextFireAttempt; }
            set { try { m_NextFireAttempt = value; } catch { } }
        }

        public SkillName Learning
        {
            get { return m_Learning; }
            set { m_Learning = value; }
        }

        private int m_KinPaintHue = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int KinPaintHue
        {
            get { return m_KinPaintHue; }
            set { m_KinPaintHue = value; }
        }

        private DateTime m_KinPaintExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime KinPaintExpiration
        {
            get { return m_KinPaintExpiration; }
            set { m_KinPaintExpiration = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextSmithBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextSmithBulkOrder - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextSmithBulkOrder = DateTime.UtcNow + value; }
                catch { }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextTailorBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextTailorBulkOrder - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextTailorBulkOrder = DateTime.UtcNow + value; }
                catch { }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastEscortTime
        {
            get { return m_LastEscortTime; }
            set { m_LastEscortTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastPetBallTime
        {
            get { return m_LastPetBallTime; }
            set { m_LastPetBallTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HideExiledStatus { get { return NameMod != null && NameMod.Length > 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HideMurdererStatus { get; set; }

        public PlayerMobile()
        {
            m_LastTarget = Serial.MinusOne;
            m_AutoStabled = new List<Mobile>();

            m_VisList = new List<Mobile>();
            m_PermaFlags = new List<Mobile>();
            
            m_GameTime = TimeSpan.Zero;

            m_MurderCountDecayTimeRemaining = TimeSpan.FromHours(MurderCountDecayHours);

            m_UserOptHideFameTitles = true;

            TitleColorState = new PlayerTitleColors();

            m_SpecialAbilityEffectEntries = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToAdd = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToRemove = new List<SpecialAbilityEffectEntry>();
        }

        public override bool MutateSpeech(List<Mobile> hears, ref string text, ref object context)
        {
            if (Alive)
                return false;

            if (Core.ML && Skills[SkillName.SpiritSpeak].Value >= 100.0)
                return false;

            for (int i = 0; i < hears.Count; ++i)
            {
                Mobile m = hears[i];

                if (m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0)
                    return false;
            }

            return base.MutateSpeech(hears, ref text, ref context);
        }

        public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
        {
            if (type == MessageType.Guild) //Guilds.Guild.NewGuildSystem && ( || type == MessageType.Alliance
            {
                //TEST: GUILD
                /*
                Guilds.Guild g = this.Guild as Guilds.Guild;

                if (g == null)
                    SendLocalizedMessage(1063142); // You are not in a guild!

                else	//Type == MessageType.Guild
                {
                    //m_GuildMessageHue = hue;

                    g.GuildChat(this, text);

                    SendToStaffMessage(this, "[Guild]: {0}", text);
                }
                */
            }

            else if (type == MessageType.Alliance)
            {
            }

            else
                base.DoSpeech(text, keywords, type, hue);
        }

        public void SendAllianceMessage(Mobile from, int hue, string text)
        {
            Packet p = null;

            NetState state = this.NetState;

            if (state != null)
            {
                if (p == null)
                    p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Alliance, hue, 3, from.Language, from.Name, text));

                state.Send(p);
            }

            Packet.Release(p);
        }

        public static void SendToStaffMessage(Mobile from, string text)
        {
            Packet p = null;

            foreach (NetState ns in from.GetClientsInRange(8))
            {
                Mobile mob = ns.Mobile;

                if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel)
                {
                    if (p == null)
                        p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text));

                    ns.Send(p);
                }
            }

            Packet.Release(p);
        }

        private static void SendToStaffMessage(Mobile from, string format, params object[] args)
        {
            SendToStaffMessage(from, String.Format(format, args));
        }

        public override bool RangeExemption(Mobile mobileTarget)
        {
            if (mobileTarget == null)
                return false;

            double phalanxValue = GetSpecialAbilityEntryValue(SpecialAbilityEffect.Phalanx);

            int extraRange = (int)(Math.Floor(phalanxValue));

            BaseWeapon weapon = Weapon as BaseWeapon;

            if (weapon != null)
            {
                if (!(weapon is BaseRanged))
                {
                    int adjustedRange = weapon.MaxRange + extraRange;

                    bool foundBlockingItem = false;

                    IPooledEnumerable itemsOnTile = Map.GetItemsInRange(mobileTarget.Location, 1);

                    foreach (Item item in itemsOnTile)
                    {
                        if (Utility.GetDistance(Location, item.Location) > 1)
                            continue;
                    }

                    itemsOnTile.Free();

                    if (InRange(mobileTarget, adjustedRange) && foundBlockingItem)
                        return true;
                }
            }

            return false;
        }

        public override bool IsHindered()
        {
            double hinderValue = GetSpecialAbilityEntryValue(SpecialAbilityEffect.Hinder);

            if (hinderValue != 0)
                return true;

            return base.IsHindered();
        }

        public virtual void OnGaveMeleeAttack(Mobile defender)
        {
            BaseWeapon weapon = Weapon as BaseWeapon;

            if (weapon == null)
                return;

            AspectWeaponProfile aspectWeaponProfile = AspectGear.GetAspectWeaponProfile(this);
            AspectArmorProfile aspectArmorProfile = AspectGear.GetAspectArmorProfile(this);

            bool aspectWeaponEffectTriggered = false;

            if (aspectWeaponProfile != null && defender is BaseCreature)
            {
                BaseCreature bc_Defender = defender as BaseCreature;

                double aspectWeaponEffectChance = AspectGear.AspectWeaponEffectChance + (AspectGear.AspectWeaponEffectChancePerTier * (double)aspectWeaponProfile.m_TierLevel);
                
                aspectWeaponEffectChance *= AspectGear.GetEffectWeaponSpeedScalar(weapon);

                if (Utility.RandomDouble() <= aspectWeaponEffectChance)
                {
                    aspectWeaponEffectTriggered = true;

                    AspectGear.ResolveSpecialEffect(weapon, this, bc_Defender);
                }                
            }

            if (!aspectWeaponEffectTriggered && aspectArmorProfile != null && defender is BaseCreature)
            {
                double fireAspectEffectChance = 0;
                double voidAspectEffectChance = 0;

                if (aspectArmorProfile.m_Aspect == AspectEnum.Fire)
                    fireAspectEffectChance = AspectGear.FireEffectOnAttackChance + (AspectGear.FireEffectOnAttackChancePerTier * (double)aspectArmorProfile.m_TierLevel);

                if (aspectArmorProfile.m_Aspect == AspectEnum.Void)
                    voidAspectEffectChance = AspectGear.VoidChanceToRegenStatsOnAttack + (AspectGear.VoidChanceToRegenStatsOnAttackPerTier * (double)aspectArmorProfile.m_TierLevel);

                fireAspectEffectChance *= AspectGear.GetEffectWeaponSpeedScalar(weapon);

                if (Utility.RandomDouble() <= fireAspectEffectChance)
                {
                    //TEST: Add Aspect Visuals
                }

                else if (Utility.RandomDouble() <= voidAspectEffectChance)
                {
                    //TEST: Add Aspect Visuals
                }

                /*
                int damage = Utility.RandomMinMax(40, 60);

                if (Utility.RandomDouble() <= flamestrikeChance)
                {
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);
                    SpecialAbilities.FlamestrikeSpecialAbility(1.0, this, defender, damage, 0, -1, true, "", "");
                }

                if (Utility.RandomDouble() <= energySiphonChance)
                {
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);
                    SpecialAbilities.EnergySiphonSpecialAbility(1.0, this, defender, 1.0, 1, -1, true, "You siphon energy from your target.", "");
                }
                */
            }
        }

        public virtual void OnGotMeleeAttack(Mobile attacker)
        {
            AspectArmorProfile aspectArmorProfile = AspectGear.GetAspectArmorProfile(this);

            if (aspectArmorProfile != null && attacker is BaseCreature)
            {
                double fireAspectEffectChance = 0;

                if (aspectArmorProfile.m_Aspect == AspectEnum.Fire)
                    fireAspectEffectChance = AspectGear.FireEffectOnHitChance + (AspectGear.FireEffectOnHitChancePerTier * (double)aspectArmorProfile.m_TierLevel);
                
                if (Utility.RandomDouble() <= fireAspectEffectChance)
                {
                    //TEST: Add Aspect Visuals
                }

                /*
                int damage = Utility.RandomMinMax(40, 60);

                if (Utility.RandomDouble() < flamestrikeChance)
                {
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5005);

                    SpecialAbilities.FlamestrikeSpecialAbility(1.0, null, attacker, damage, 0, -1, true, "", "");
                }
                */
            }
        }

        public void PlayerVsPlayerCombatOccured(PlayerMobile pm_From)
        {
            if (pm_From == null)
                return;

            LastCombatTime = DateTime.UtcNow;
            pm_From.LastCombatTime = DateTime.UtcNow;

            if (Guild != null && pm_From.Guild != null && Guild == pm_From.Guild)
                return;

            if (Party != null && pm_From.Party != null && Party == pm_From.Party)
                return;

            //Cancel Polymorph Potion Effect
            if (!pm_From.CanBeginAction(typeof(PolymorphPotion)) && pm_From.BodyMod != 0)
            {
                pm_From.SendMessage("Your polymorph potion effect fades as you enter combat with another player.");

                pm_From.BodyMod = 0;
                pm_From.HueMod = -1;

                pm_From.EndAction(typeof(PolymorphPotion));
                pm_From.EndAction(typeof(PolymorphSpell));

                BaseArmor.ValidateMobile(pm_From);
            }

            LastPlayerCombatTime = DateTime.UtcNow;
            pm_From.LastPlayerCombatTime = DateTime.UtcNow;

            AspectGear.CheckForAndUpdateAspectArmorProperties(this);

            CapStatMods(this);

            if (m_PlayerCombatTimer == null)
            {
                m_PlayerCombatTimer = new PlayerCombatTimer(this);
                m_PlayerCombatTimer.Start();
            }

            else
            {
                m_PlayerCombatTimer.Stop();
                m_PlayerCombatTimer = null;

                m_PlayerCombatTimer = new PlayerCombatTimer(this);
                m_PlayerCombatTimer.Start();
            }
        }

        public override int AbsorbDamage(Mobile attacker, Mobile defender, int damage, bool physical, bool melee)
        {
            return BaseArmor.AbsorbDamage(attacker, defender, damage, physical, melee);
        }

        public override void Damage(int amount, Mobile from)
        {
            double damage = (double)amount;

            double discordEffect = 0;
            double focusedAggressionEffect = 0;
            double earthAspectEffect = 0;
            double lyricAspectEffect = 0;
            double shadowAspectEffect = 0;

            BaseCreature bc_Source = from as BaseCreature;
            PlayerMobile pm_Source = from as PlayerMobile;

            PlayerMobile pm_SourceMaster = null;

            AspectArmorProfile aspectArmorProfile = AspectGear.GetAspectArmorProfile(this);
            
            if (from != null)
            {
                if (from != null && from != this)
                {
                    LastCombatTime = DateTime.UtcNow;
                    from.LastCombatTime = DateTime.UtcNow;

                    if (bc_Source != null)
                    {
                        pm_SourceMaster = bc_Source.GetPlayerMaster() as PlayerMobile;

                        if (pm_SourceMaster != null && pm_SourceMaster != this)
                        {
                            pm_SourceMaster.LastCombatTime = DateTime.UtcNow;

                            PlayerVsPlayerCombatOccured(pm_SourceMaster);
                            pm_SourceMaster.PlayerVsPlayerCombatOccured(this);                            
                        }
                    }

                    if (pm_Source != null)
                    {
                        PlayerVsPlayerCombatOccured(pm_Source);
                        pm_Source.PlayerVsPlayerCombatOccured(this);
                    }
                }
                               
                if (bc_Source != null)
                {
                    //Discord
                    discordEffect = -1 * bc_Source.DiscordEffect;

                    //Herding
                    if (bc_Source.FocusedAggressionTarget == this && bc_Source.FocusedAggressionExpiration > DateTime.UtcNow)
                        focusedAggressionEffect = (bc_Source.FocusedAggresionValue * BaseCreature.HerdingFocusedAggressionPvPDamageScalar);

                    //Earth Aspect
                    if (aspectArmorProfile != null)
                    {
                        if (aspectArmorProfile.m_Aspect == AspectEnum.Earth)
                            earthAspectEffect = AspectGear.EarthDamageReduction + (AspectGear.EarthDamageReductionPerTier * (double)aspectArmorProfile.m_TierLevel);
                    }

                    //Lyric Aspect
                    if (aspectArmorProfile != null)
                    {
                        if (aspectArmorProfile.m_Aspect == AspectEnum.Lyric && m_LyricAspectFailedBardingAttemptTargets.Contains(from) && m_LyricAspectFailedBardingAttemptExpiration > DateTime.UtcNow)
                            lyricAspectEffect = m_LyricAspectFailedBardingAttemptDamageReduction;
                    }

                    //Shadow Aspect
                    if (aspectArmorProfile != null)
                    {
                        if (aspectArmorProfile.m_Aspect == AspectEnum.Shadow && m_ShadowAspectPostBackstabDamageReceivedReductionExpiration > DateTime.UtcNow)
                            shadowAspectEffect = m_ShadowAspectPostBackstabDamageReceivedReduction;
                    }
                }
            }

            damage *= 1 + discordEffect + focusedAggressionEffect - earthAspectEffect - lyricAspectEffect - shadowAspectEffect;

            //Player Ress Penalty
            if (pm_Source != null)
            {
                if (pm_Source.RessPenaltyExpiration > DateTime.UtcNow && pm_Source.m_RessPenaltyEffectivenessReductionCount > 0)
                    damage *= 1 - (PlayerMobile.RessPenaltyDamageScalar * (double)pm_Source.m_RessPenaltyEffectivenessReductionCount);
            }

            else if (pm_SourceMaster != null)
            {
                if (pm_SourceMaster.RessPenaltyExpiration > DateTime.UtcNow && pm_SourceMaster.m_RessPenaltyEffectivenessReductionCount > 0)
                    damage *= 1 - (PlayerMobile.RessPenaltyDamageScalar * (double)pm_SourceMaster.m_RessPenaltyEffectivenessReductionCount);
            }

            //Ship-Based Combat
            if (BaseShip.UseShipBasedDamageModifer(from, this))
                damage *= BaseShip.shipBasedDamageToPlayerScalar;

            //Void Aspect
            if (aspectArmorProfile != null && bc_Source != null)
            {
                if (aspectArmorProfile.m_Aspect == AspectEnum.Void)
                {
                    double voidNullifyChance = AspectGear.VoidChanceToNullifyDamageOnHit + (AspectGear.VoidChanceToNullifyDamageOnHitPerTier * (double)aspectArmorProfile.m_TierLevel);

                    if (Utility.RandomDouble() <= voidNullifyChance)
                    {
                        damage = 1;

                        //TEST: Add Aspect Visuals
                    }                
                }                    
            }

            if (damage < 1)
                damage = 1;

            int finalDamage = (int)(Math.Round(damage));

            pm_SourceMaster = null;

            if (bc_Source != null)
            {
                if (bc_Source.ControlMaster is PlayerMobile)
                {
                    pm_SourceMaster = bc_Source.ControlMaster as PlayerMobile;
                    DamageTracker.RecordDamage(pm_SourceMaster, from, this, DamageTracker.DamageType.FollowerDamage, finalDamage);
                }

                else if (bc_Source.SummonMaster is PlayerMobile)
                {
                    pm_SourceMaster = bc_Source.SummonMaster as PlayerMobile;
                    DamageTracker.RecordDamage(pm_SourceMaster, from, this, DamageTracker.DamageType.FollowerDamage, finalDamage);
                }

                else if (bc_Source.BardProvoked && bc_Source.BardMaster is PlayerMobile)
                {
                    pm_SourceMaster = bc_Source.BardMaster as PlayerMobile;
                    DamageTracker.RecordDamage(pm_SourceMaster, from, this, DamageTracker.DamageType.ProvocationDamage, finalDamage);
                }
            }

            DamageTracker.RecordDamage(this, from, this, DamageTracker.DamageType.DamageTaken, finalDamage);

            base.Damage(finalDamage, from);
        }        

        #region Poison

        public int GetPoisonResistance(Mobile from)
        {
            int resistanceLevel = 0;

            if (Skills[SkillName.Poisoning].Value >= 50)
                resistanceLevel = 1;

            if (Skills[SkillName.Poisoning].Value >= 100)
                resistanceLevel = 2;

            if (from is BaseCreature && Skills[SkillName.Poisoning].Value >= 120)
                resistanceLevel = 3;

            return resistanceLevel;
        }

        public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
        {
            if (!Alive)
                return ApplyPoisonResult.Immune;

            int poisonLevel = poison.Level;
            int poisonResistance = GetPoisonResistance(from);

            poisonLevel -= PoisonResistance;

            if (PoisonResistance > 0)
                FixedEffect(0x37B9, 10, 5, 2210, 0);

            if (poisonLevel < 0)
                poisonLevel = 0;

            poison = Poison.GetPoison(poisonLevel);
            
            ApplyPoisonResult result = base.ApplyPoison(from, poison);

            if (from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer)            
                (PoisonTimer as PoisonImpl.PoisonTimer).From = from;            

            return result;
        }

        public override bool CheckPoisonImmunity(Mobile from, Poison poison)
        {
            if (Young)
                return true;

            return base.CheckPoisonImmunity(from, poison);
        }

        public override void OnPoisonImmunity(Mobile from, Poison poison)
        {
            if (Young)
                SendLocalizedMessage(502808); // You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
            else
                base.OnPoisonImmunity(from, poison);
        }

        #endregion

        public PlayerMobile(Serial s): base(s)
        {
            m_VisList = new List<Mobile>();
        }

        public List<Mobile> VisibilityList
        {
            get { return m_VisList; }
        }

        public List<Mobile> PermaFlags
        {
            get { return m_PermaFlags; }
        }

        // luck returned as an int representing percentage
        public override int Luck
        {
            get
            {
                int luck = 0;

                return luck;
            }
        }

        public override bool IsHarmfulCriminal(Mobile target)
        {
            if (SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0)
            {
                int noto = Notoriety.Compute(this, target);

                if (noto == Notoriety.Innocent)
                    target.Delta(MobileDelta.Noto);

                return false;
            }

            if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
                return false;

            if (Core.ML && target is BaseCreature && ((BaseCreature)target).Controlled && this == ((BaseCreature)target).ControlMaster)
                return false;

            return base.IsHarmfulCriminal(target);
        }

        private void RevertHair()
        {
            SetHairMods(-1, -1);
        }
        
        public override void Serialize(GenericWriter writer)
        {
            CheckMurderCountDecay();

            if (KinPaintHue != -1)
            {
                if (DateTime.UtcNow >= KinPaintExpiration)
                {
                    KinPaintHue = -1;
                    KinPaintExpiration = DateTime.MinValue;

                    BodyMod = 0;
                    HueMod = -1;

                    SendMessage("Your kin paint has faded.");
                }
            }

            if (KinPaintHue != -1)
            {
                if (HueModEnd > DateTime.MinValue && HueModEnd < DateTime.UtcNow)
                {
                    HueMod = -1;
                    HueModEnd = DateTime.MinValue;
                }
            }

            CheckAtrophies(this);

            base.Serialize(writer);

            writer.Write((int)0); //Version

            //Version 0            
            writer.Write(m_SpecialAbilityEffectEntries.Count);
            for (int a = 0; a < m_SpecialAbilityEffectEntries.Count; a++)
            {
                writer.Write((int)m_SpecialAbilityEffectEntries[a].m_SpecialAbilityEffect);
                writer.Write((Mobile)m_SpecialAbilityEffectEntries[a].m_Owner);
                writer.Write((double)m_SpecialAbilityEffectEntries[a].m_Value);
                writer.Write((DateTime)m_SpecialAbilityEffectEntries[a].m_Expiration);
            }

            writer.Write(PreviousNames.Count);
            for (int a = 0; a < PreviousNames.Count; a++)
            {
                writer.Write(PreviousNames[a]);
            }
            
            writer.Write(m_PassiveSkillGainRemaining);
            writer.Write((int)m_SatisfactionLevel);
            writer.Write(m_SatisfactionExpiration);
            writer.Write(m_FactionPlayerProfile);
            writer.Write(m_EventCalendarAccount);
            writer.Write(m_MHSPlayerEntry);
            writer.Write((int)m_ShowHealing);
            writer.Write(m_WorldChatAccountEntry);
            writer.Write(m_HideRestrictionExpiration);
            writer.Write((int)m_HenchmenSpeechDisplayMode);
            writer.Write((int)m_StealthStepsDisplayMode);
            writer.Write(m_ShowAdminFilterText);
            writer.Write(m_TitleCollection);
            writer.Write(m_AchievementAccountEntry);
            writer.Write(m_EnhancementsAccountEntry);
            writer.Write(m_CaptchaAccountData);
            writer.Write((int)m_ShowFollowerDamageTaken);
            writer.Write(m_LastPlayerKilledBy);
            writer.Write(m_LastInstrument);
            writer.Write((int)m_ShowDamageTaken);
            writer.Write((int)m_ShowProvocationDamage);
            writer.Write((int)m_ShowPoisonDamage);
            writer.Write(m_AutoStealth);
            writer.Write(m_ShipOccupied);
            writer.Write(KinPaintHue);
            writer.Write(KinPaintExpiration);
            writer.Write((int)m_ShowMeleeDamage);
            writer.Write((int)m_ShowSpellDamage);
            writer.Write((int)m_ShowFollowerDamage);
            writer.Write(m_RecallRestrictionExpiration);
            writer.Write(m_LastLocation);
            writer.Write(m_PirateScore);
            writer.Write(m_CompanionLastLocation);
            writer.Write(m_Companion);
            writer.Write((int)m_NumGoldCoinsGenerated);
            writer.Write(CreatedOn);
            writer.Write((byte)SelectedTitleColorIndex);
            writer.Write((byte)SelectedTitleColorRarity);
            TitleColorState.Serialize(writer);
            writer.Write(m_UserOptHideFameTitles);
            writer.Write(LoginElapsedTime);
            writer.Write((DateTime)m_DateTimeDied);
            writer.Write((TimeSpan)m_TimeSpanDied);
            writer.Write((TimeSpan)m_TimeSpanResurrected);
            writer.Write((DateTime)m_AnkhNextUse);
            writer.Write(m_AutoStabled, true);
            writer.Write((Serial)m_LastTarget);
            writer.Write((DateTime)m_LastDeathByPlayer);
            writer.Write(m_LastOnline);
            writer.Write((int)m_NpcGuild);
            writer.Write((DateTime)m_NpcGuildJoinTime);
            writer.Write((TimeSpan)m_NpcGuildGameTime);
            writer.Write(m_PermaFlags, true);
            writer.Write(NextTailorBulkOrder);
            writer.Write(NextSmithBulkOrder);
            writer.Write((int)m_Flags);
            writer.Write(m_MurderCountDecayTimeRemaining);
            writer.Write(m_LifetimeMurderCounts);
            writer.Write(RessPenaltyExpiration);
            writer.Write(m_RessPenaltyAccountWideAggressionRestriction);
            writer.Write(m_RessPenaltyEffectivenessReductionCount);
            writer.Write(GameTime);
            writer.Write(m_GuildSettings);
            writer.Write(m_CompetitionContext);
            writer.Write(m_ArenaPlayerSettings);

            writer.Write((int)m_HairModID);
            writer.Write((int)m_HairModHue);
            writer.Write((int)m_BeardModID);
            writer.Write((int)m_BeardModHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                int specialAbilityEntries = reader.ReadInt();
                for (int a = 0; a < specialAbilityEntries; a++)
                {
                    SpecialAbilityEffect effect = (SpecialAbilityEffect)reader.ReadInt();
                    Mobile owner = reader.ReadMobile();
                    double value = reader.ReadDouble();
                    DateTime expiration = reader.ReadDateTime();

                    SpecialAbilityEffectEntry entry = new SpecialAbilityEffectEntry(effect, owner, value, expiration);

                    m_SpecialAbilityEffectEntries.Add(entry);
                }

                int previousNamesCount = reader.ReadInt();
                for (int i = 0; i < previousNamesCount; i++)
                {
                    PreviousNames.Add(reader.ReadString());
                }
                
                m_PassiveSkillGainRemaining = reader.ReadDouble();
                m_SatisfactionLevel = (Food.SatisfactionLevelType)reader.ReadInt();
                m_SatisfactionExpiration = reader.ReadDateTime();
                m_FactionPlayerProfile = (FactionPlayerProfile)reader.ReadItem() as FactionPlayerProfile;
                m_EventCalendarAccount = (EventCalendarAccount)reader.ReadItem() as EventCalendarAccount;
                m_MHSPlayerEntry = (MHSPlayerEntry)reader.ReadItem() as MHSPlayerEntry;
                m_ShowHealing = (DamageDisplayMode)reader.ReadInt();
                m_WorldChatAccountEntry = (WorldChatAccountEntry)reader.ReadItem() as WorldChatAccountEntry;
                m_HideRestrictionExpiration = reader.ReadDateTime();
                m_HenchmenSpeechDisplayMode = (HenchmenSpeechDisplayMode)reader.ReadInt();
                m_StealthStepsDisplayMode = (StealthStepsDisplayMode)reader.ReadInt();
                m_ShowAdminFilterText = reader.ReadBool();
                m_TitleCollection = (TitleCollection)reader.ReadItem() as TitleCollection;
                m_AchievementAccountEntry = (AchievementAccountEntry)reader.ReadItem() as AchievementAccountEntry;
                m_EnhancementsAccountEntry = (EnhancementsAccountEntry)reader.ReadItem() as EnhancementsAccountEntry;
                m_CaptchaAccountData = (CaptchaAccountData)reader.ReadItem() as CaptchaAccountData;
                m_ShowFollowerDamageTaken = (DamageDisplayMode)reader.ReadInt();
                m_LastPlayerKilledBy = (PlayerMobile)reader.ReadMobile();
                m_LastInstrument = (BaseInstrument)reader.ReadItem();
                m_ShowDamageTaken = (DamageDisplayMode)reader.ReadInt();
                m_ShowProvocationDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowPoisonDamage = (DamageDisplayMode)reader.ReadInt();
                m_AutoStealth = reader.ReadBool();
                ShipOccupied = (BaseShip)reader.ReadItem();
                KinPaintHue = reader.ReadInt();
                KinPaintExpiration = reader.ReadDateTime();
                m_ShowMeleeDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowSpellDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowFollowerDamage = (DamageDisplayMode)reader.ReadInt();
                m_RecallRestrictionExpiration = reader.ReadDateTime();
                m_LastLocation = reader.ReadPoint3D();
                m_PirateScore = reader.ReadInt();
                m_CompanionLastLocation = reader.ReadPoint3D();
                m_Companion = reader.ReadBool();
                m_NumGoldCoinsGenerated = reader.ReadInt();
                CreatedOn = reader.ReadDateTime();
                SelectedTitleColorIndex = (int)reader.ReadByte();
                SelectedTitleColorRarity = (EColorRarity)reader.ReadByte();
                TitleColorState = new PlayerTitleColors();
                TitleColorState.Deserialize(reader);
                m_UserOptHideFameTitles = reader.ReadBool();
                LoginElapsedTime = reader.ReadTimeSpan();
                m_DateTimeDied = reader.ReadDateTime();
                m_TimeSpanDied = reader.ReadTimeSpan();
                m_TimeSpanResurrected = reader.ReadTimeSpan();
                m_AnkhNextUse = reader.ReadDateTime();
                m_AutoStabled = reader.ReadStrongMobileList();
                m_LastTarget = (Serial)reader.ReadInt();
                m_LastDeathByPlayer = reader.ReadDateTime();
                m_LastOnline = reader.ReadDateTime();
                m_NpcGuild = (NpcGuild)reader.ReadInt();
                m_NpcGuildJoinTime = reader.ReadDateTime();
                m_NpcGuildGameTime = reader.ReadTimeSpan();
                m_PermaFlags = reader.ReadStrongMobileList();
                NextTailorBulkOrder = reader.ReadTimeSpan();
                NextSmithBulkOrder = reader.ReadTimeSpan();
                m_Flags = (PlayerFlag)reader.ReadInt();
                m_MurderCountDecayTimeRemaining = reader.ReadTimeSpan();
                m_LifetimeMurderCounts = reader.ReadInt();
                RessPenaltyExpiration = reader.ReadDateTime();
                m_RessPenaltyAccountWideAggressionRestriction = reader.ReadBool();
                m_RessPenaltyEffectivenessReductionCount = reader.ReadInt();

                m_GameTime = reader.ReadTimeSpan();
                m_GuildSettings = (GuildSettings)reader.ReadItem();
                m_CompetitionContext = (CompetitionContext)reader.ReadItem();
                m_ArenaPlayerSettings = (ArenaPlayerSettings)reader.ReadItem();

                m_HairModID = reader.ReadInt();
                m_HairModHue = reader.ReadInt();
                m_BeardModID = reader.ReadInt();
                m_BeardModHue = reader.ReadInt();
            }

            //----------------  

            //Safety Measures
            Squelched = false;
            Frozen = false;
            CantWalk = false;

            if (m_LastOnline == DateTime.MinValue && Account != null)
                m_LastOnline = ((Account)Account).LastLogin;

            if (AccessLevel > AccessLevel.Player)
                m_IgnoreMobiles = true;

            if (TitleColorState == null)
                TitleColorState = new PlayerTitleColors();

            List<Mobile> list = Stabled;

            for (int i = 0; i < list.Count; ++i)
            {
                BaseCreature bc = list[i] as BaseCreature;

                if (bc != null)
                {
                    bc.IsStabled = true;

                    bc.OwnerAbandonTime = DateTime.UtcNow + TimeSpan.FromDays(1000);
                }
            }

            CheckAtrophies(this);

            if (Hidden)	//Hiding is the only buff where it has an effect that's serialized.
                AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));

            //Reapply Kin Paint
            if (KinPaintHue != -1 && KinPaintExpiration > DateTime.UtcNow)
                HueMod = KinPaintHue;

            if (!(Region.Find(LogoutLocation, LogoutMap) is GuardedRegion))
            {
                // crash protection
                Hidden = true;
                Poison = null;
            }
            
            if (LastPlayerCombatTime > DateTime.MinValue)
            {
                if (RecentlyInPlayerCombat)
                {
                    m_PlayerCombatTimer = new PlayerCombatTimer(this);
                    m_PlayerCombatTimer.Start();
                }
            }

            m_DamageTracker = new DamageTracker(this);
        }

        public static void CheckAtrophies(Mobile m)
        {
        }

        public void CheckMurderCountDecay()
        {            
            if (m_MurderCountDecayTimeRemaining < GameTime)
            {
                m_MurderCountDecayTimeRemaining += TimeSpan.FromHours(MurderCountDecayHours);

                bool wasMurderer = false;

                if (Murderer)
                    wasMurderer = true;

                if (MurderCounts > 0)
                {
                    --MurderCounts;

                    if (wasMurderer)
                        SendMessage("You are no longer known as a murderer.");
                }
            }
        }

        public void ResetKillTime()
        {
            m_MurderCountDecayTimeRemaining = GameTime + TimeSpan.FromHours(MurderCountDecayHours);
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Developer)]
        public DateTime SessionStart
        {
            get { return m_SessionStart; }
            set { m_SessionStart = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan GameTime
        {
            get
            {
                if (NetState != null)
                    return m_GameTime + (DateTime.UtcNow - m_SessionStart);

                else
                    return m_GameTime;
            }
        }

        private bool SameParty(Mobile target)
        {
            bool sameParty = false;
            if (Party != null && target.Party != null)
            {
                if (Party == target.Party)
                    sameParty = true;
            }
            return sameParty;
        }

        public override bool CanSee(Mobile m)
        {
            if (m is CharacterStatue)
                ((CharacterStatue)m).OnRequestedAnimation(this);

            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                if (bc_Creature.Hidden && bc_Creature.Controlled)
                {
                    if (bc_Creature.ControlMaster == this)
                        return true;
                }
            }

            if (m != this && !Alive && !Warmode && !m.Hidden && m is PlayerMobile && AccessLevel == AccessLevel.Player && m.AccessLevel == AccessLevel.Player)
            {
                Send(m.RemovePacket);

                return false;
            }

            if (m is PlayerMobile && ((PlayerMobile)m).m_VisList.Contains(this))
                return true;

            return base.CanSee(m);
        }

        public virtual void CheckedAnimate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
        {
            if (!Mounted)
            {
                base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
            }
        }

        public override void Animate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
        {
            base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
        }

        public override bool CanSee(Item item)
        {
            if (m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer(item))
                return false;

            return base.CanSee(item);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            BaseHouse.HandleDeletion(this);

            DisguiseTimers.RemoveTimer(this);
        }
        
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);            
        }

        //IPY: Added this for Custom Titles (Sean)
        private static string[] m_GuildTypes = new string[]
		{
			"",
			" (Chaos)",
			" (Order)"
		};

        public override void AggressiveAction(Mobile aggressor, bool criminal, bool causeCombat)
        {
            if (aggressor == null) return;
            if (aggressor.Deleted) return;
            if (aggressor == this) return;
            if (Blessed) return;
            if (aggressor.Blessed) return;

            base.AggressiveAction(aggressor, criminal, causeCombat);
        }

        public override void PushNotoriety(Mobile from, Mobile to, bool aggressor)
        {
            NotorietyHandlers.PushNotoriety(from, to, aggressor);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Deleted)
                return;

            else if (AccessLevel == AccessLevel.Player && DisableHiddenSelfClick && Hidden && from == this)
                return;

            try
            {
                //TEST: GUILD
                //bool show_guild = GuildClickMessage && Guild != null && (DisplayGuildTitle || (Player && Guild.Type != Guilds.GuildType.Regular));
                bool show_other_titles = true;
                
                int newhue;

                if (NameHue != -1)
                    newhue = NameHue;

                else if (AccessLevel > AccessLevel.Player)
                    newhue = 11;

                else
                    newhue = Notoriety.GetHue(Notoriety.Compute(from, this));

                //TEST: GUILD
                /*
                if (show_guild) // GUILD NO FACTION
                {
                    string title = GuildTitle;
                    string type;

                    if (title == null)
                        title = "";
                    else
                        title = title.Trim();

                    if (Guild.Type >= 0 && (int)Guild.Type < m_GuildTypes.Length)
                        type = m_GuildTypes[(int)Guild.Type];
                    else
                        type = "";

                    string text = String.Format(title.Length <= 0 ? "[{1}]{2}" : "[{0}, {1}]{2}", title, Guild.Abbreviation, type);
                    PrivateOverheadMessage(MessageType.Regular, SpeechHue, true, text, from.NetState);
                }
                */

                if (show_other_titles)
                {
                    string fullname_line = "";

                    if ((ShowFameTitle && (Player || Body.IsHuman) && Fame >= 10000))
                        fullname_line = Female ? "Lady " : "Lord ";

                    fullname_line += Name == null ? String.Empty : Name;
                    fullname_line = ApplyNameSuffix(fullname_line); // (Young) for example

                    PrivateOverheadMessage(MessageType.Label, newhue, AsciiClickMessage, fullname_line, from.NetState);
                }

                else
                {
                    string fullname_line = "";

                    fullname_line += Name == null ? String.Empty : Name;

                    PrivateOverheadMessage(MessageType.Label, newhue, AsciiClickMessage, fullname_line, from.NetState);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("OnSingleClick failed {0}", e.Message);
            }
        }

        protected override bool OnMove(Direction d)
        {
            if (AccessLevel != AccessLevel.Player)
                return true;

            bool stealthMove = false;

            bool leaveFootsteps = false;
            double footstepChance = .33;
            
            if (Hidden && DesignContext.Find(this) == null)	//Hidden & NOT customizing a house
            {
                if (!Mounted && (Skills.Stealth.Value >= 20.0))
                {
                    bool running = (d & Direction.Running) != 0;

                    if (running)
                    {
                        AllowedStealthSteps = -1;
                        RevealingAction();

                        return true;
                    }

                    AllowedStealthSteps--;
                    stealthMove = true;

                    if (m_AutoStealth)
                    {
                        if (AllowedStealthSteps < 0 || CanBeginAction(typeof(Stealth)))
                        {
                            NextSkillTime = Core.TickCount + (int)Server.SkillHandlers.Stealth.OnUse(this).TotalMilliseconds;

                            //If Stealth Success
                            if (Hidden)
                            {
                                AllowedStealthSteps--;
                                stealthMove = true;

                                if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage && AllowedStealthSteps < 100)
                                    SendMessage("You have " + this.AllowedStealthSteps.ToString() + " stealth steps remaining.");

                                else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead && AllowedStealthSteps < 100)
                                    PrivateOverheadMessage(MessageType.Regular, 0, false, "You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.", NetState);
                            }
                        }

                        else
                        {
                            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage && AllowedStealthSteps < 100)
                                SendMessage("You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.");

                            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead && AllowedStealthSteps < 100)
                                PrivateOverheadMessage(MessageType.Regular, 0, false, "You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.", NetState);
                        }
                    }

                    else
                    {
                        if (AllowedStealthSteps < 0)
                            RevealingAction();

                        else
                        {
                            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage && AllowedStealthSteps < 100)
                                SendMessage("You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.");

                            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead && AllowedStealthSteps < 100)
                                PrivateOverheadMessage(MessageType.Regular, 0, false, "You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.", NetState);
                        }
                    }

                    if (Hidden && leaveFootsteps && Utility.RandomDouble() <= footstepChance)
                        new Footsteps(d).MoveToWorld(Location, Map);
                }

                else
                    RevealingAction();
            }
            
            return true;
        }

        public override void SendStealthReadyNotification()
        {
            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage)
                SendMessage("You feel comfortable enough to begin stealthing.");

            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead)
                PrivateOverheadMessage(MessageType.Regular, 0, false, "You feel comfortable enough to begin stealthing.", NetState);
        }

        public override void SendStealthMovementReadyNotification()
        {
            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage)
                SendMessage("You feel ready to continue stealthing.");

            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead)
                PrivateOverheadMessage(MessageType.Regular, 0, false, "You feel ready to continue stealthing.", NetState);
        }

        private bool m_BedrollLogout;

        public bool BedrollLogout
        {
            get { return m_BedrollLogout; }
            set { m_BedrollLogout = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Paralyzed
        {
            get
            {
                return base.Paralyzed;
            }
            set
            {
                base.Paralyzed = value;

                if (value)
                    AddBuff(new BuffInfo(BuffIcon.Paralyze, 1075827));	//Paralyze/You are frozen and can not move
                else
                    RemoveBuff(BuffIcon.Paralyze);
            }
        }

        public void RemoveGoldFromBankbox(int amount)
        {
            Item[] gold, checks;
            int balance = Banker.GetBalance(this, out gold, out checks);

            if (balance < amount)
                return;

            for (int i = 0; amount > 0 && i < gold.Length; ++i)
            {
                if (gold[i].Amount <= amount)
                {
                    amount -= gold[i].Amount;
                    gold[i].Delete();
                }

                else
                {
                    gold[i].Amount -= amount;
                    amount = 0;
                }
            }

            for (int i = 0; amount > 0 && i < checks.Length; ++i)
            {
                BankCheck check = (BankCheck)checks[i];

                if (check.Worth <= amount)
                {
                    amount -= check.Worth;
                    check.Delete();
                }
                else
                {
                    check.Worth -= amount;
                    amount = 0;
                }
            }
        }           

        #region MyRunUO Invalidation

        /*
        public override void OnKillsChange(int oldValue)
        {
            if (this.Young && MurderCounts > oldValue)
            {
                Account acc = Account as Account;

                if (acc != null)
                    acc.RemoveYoungStatus(0);
            }
        }
        */

        public override void OnGenderChanged(bool oldFemale)
        {
        }

        public override void OnKarmaChange(int oldValue)
        {
        }

        public override void OnFameChange(int oldValue)
        {
        }

        public override void OnSkillChange(SkillName skill, double oldBase)
        {
            if (Young && SkillsTotal >= 5000)
            {
                Account acc = this.Account as Account;

                if (acc != null)
                    acc.RemoveYoungStatus(1019036); // You have successfully obtained a respectable skill level, and have outgrown your status as a young player!
            }

            if (skill == SkillName.AnimalTaming)            
                CheckPassiveTamingReset(oldBase);
        }

        public override void OnAccessLevelChanged(AccessLevel oldLevel)
        {
            if (AccessLevel == AccessLevel.Player)
                IgnoreMobiles = false;
            else
                IgnoreMobiles = true;
        }

        public override void OnRawStatChange(StatType stat, int oldValue)
        {
        }

        public void ReleaseAllFollowers()
        {
            var toRelease = new List<BaseCreature>();
            foreach (Mobile follower in AllFollowers)
            {
                if (follower == null) continue;
                if (follower is BladeSpirits || follower is EnergyVortex) continue;

                BaseCreature bc_Follower = follower as BaseCreature;

                if (bc_Follower != null)
                {
                    if (bc_Follower.AIObject != null)
                        toRelease.Add(bc_Follower);
                }
            }

            foreach (var follower in toRelease)
                follower.AIObject.DoOrderRelease();
        }

        public override void OnDelete()
        {
            ReleaseAllFollowers();
            
            Guilds.OnPlayerDeleted(this);

            if (m_TitleCollection != null)
                m_TitleCollection.Delete();

            if (m_SocietiesPlayerSettings != null)
                m_SocietiesPlayerSettings.Delete();

            if (m_ArenaPlayerSettings != null)
                m_ArenaPlayerSettings.Delete();

            if (m_CompetitionContext != null)
                m_CompetitionContext.Delete();
        }

        #endregion

        #region Fastwalk Prevention
        private static bool FastwalkPrevention = true; // Is fastwalk prevention enabled?
        private static int FastwalkThreshold = 400; // Fastwalk prevention will become active after 0.4 seconds

        private long m_NextMovementTime;
        private bool m_HasMoved;

        public virtual bool UsesFastwalkPrevention { get { return (AccessLevel < AccessLevel.Counselor); } }

        public override int ComputeMovementSpeed(Direction dir, bool checkTurning)
        {
            if (checkTurning && (dir & Direction.Mask) != (this.Direction & Direction.Mask))
                return Mobile.RunMount;	// We are NOT actually moving (just a direction change)

            TransformContext context = TransformationSpellHelper.GetContext(this);
            
            bool running = ((dir & Direction.Running) != 0);
            
            bool onHorse = (Mount != null);

            if (onHorse)
                return (running ? Mobile.RunMount : Mobile.WalkMount);

            return (running ? Mobile.RunFoot : Mobile.WalkFoot);
        }

        public static bool MovementThrottle_Callback(NetState ns)
        {
            PlayerMobile pm = ns.Mobile as PlayerMobile;

            if (pm == null || !pm.UsesFastwalkPrevention)
                return true;

            if (!pm.m_HasMoved)
            {
                // has not yet moved
                pm.m_NextMovementTime = Core.TickCount;
                pm.m_HasMoved = true;
                return true;
            }

            long ts = pm.m_NextMovementTime - Core.TickCount;

            if (ts < 0)
            {
                // been a while since we've last moved
                pm.m_NextMovementTime = Core.TickCount;
                return true;
            }

            return (ts < FastwalkThreshold);
        }

        #endregion

        #region Enemy of One
        private Type m_EnemyOfOneType;
        private bool m_WaitingForEnemy;

        public Type EnemyOfOneType
        {
            get { return m_EnemyOfOneType; }
            set
            {
                Type oldType = m_EnemyOfOneType;
                Type newType = value;

                if (oldType == newType)
                    return;

                m_EnemyOfOneType = value;

                DeltaEnemies(oldType, newType);
            }
        }

        public bool WaitingForEnemy
        {
            get { return m_WaitingForEnemy; }
            set { m_WaitingForEnemy = value; }
        }

        private void DeltaEnemies(Type oldType, Type newType)
        {
            IPooledEnumerable eable = this.GetMobilesInRange(18);

            foreach (Mobile m in eable)
            {
                Type t = m.GetType();

                if (t == oldType || t == newType)
                {
                    NetState ns = this.NetState;

                    if (ns != null)
                    {
                        if (ns.StygianAbyss)
                        {
                            ns.Send(new MobileMoving(m, Notoriety.Compute(this, m)));
                        }
                        else
                        {
                            ns.Send(new MobileMovingOld(m, Notoriety.Compute(this, m)));
                        }
                    }
                }
            }

            eable.Free();
        }

        #endregion

        #region Hair and beard mods
        private int m_HairModID = -1, m_HairModHue;
        private int m_BeardModID = -1, m_BeardModHue;

        public void SetHairMods(int hairID, int beardID)
        {
            if (hairID == -1)
                InternalRestoreHair(true, ref m_HairModID, ref m_HairModHue);
            else if (hairID != -2)
                InternalChangeHair(true, hairID, ref m_HairModID, ref m_HairModHue);

            if (beardID == -1)
                InternalRestoreHair(false, ref m_BeardModID, ref m_BeardModHue);
            else if (beardID != -2)
                InternalChangeHair(false, beardID, ref m_BeardModID, ref m_BeardModHue);
        }

        private void CreateHair(bool hair, int id, int hue)
        {
            if (hair)
            {
                //TODO Verification?
                HairItemID = id;
                HairHue = hue;
            }
            else
            {
                FacialHairItemID = id;
                FacialHairHue = hue;
            }
        }

        private void InternalRestoreHair(bool hair, ref int id, ref int hue)
        {
            if (id == -1)
                return;

            if (hair)
                HairItemID = 0;
            else
                FacialHairItemID = 0;

            //if( id != 0 )
            CreateHair(hair, id, hue);

            id = -1;
            hue = 0;
        }

        private void InternalChangeHair(bool hair, int id, ref int storeID, ref int storeHue)
        {
            if (storeID == -1)
            {
                storeID = hair ? HairItemID : FacialHairItemID;
                storeHue = hair ? HairHue : FacialHairHue;
            }

            CreateHair(hair, id, 0);
        }

        #endregion

        #region Young system
        [CommandProperty(AccessLevel.GameMaster)]

        public bool Young
        {
            get
            {
                return GetFlag(PlayerFlag.Young);
            }

            set
            {
                SetFlag(PlayerFlag.Young, value);

                if (value)
                {
                    if (!YoungChatListeners.Contains(this))
                        YoungChatListeners.Add(this);
                }
                else
                {
                    if (YoungChatListeners.Contains(this))
                        YoungChatListeners.Remove(this);
                }

                InvalidateProperties();
            }
        }

        public override string ApplyNameSuffix(string suffix)
        {
            if (Young)
            {
                if (suffix.Length == 0)
                    suffix = "(Young)";
                else
                    suffix = String.Concat(suffix, " (Young)");
            }            

            return base.ApplyNameSuffix(suffix);
        }

        public override TimeSpan GetLogoutDelay()
        {
            if ((Young) || BedrollLogout || TestCenter.Enabled)
                return TimeSpan.Zero;

            return base.GetLogoutDelay();
        }

        private DateTime m_LastYoungMessage = DateTime.MinValue;

        public bool CheckYoungProtection(Mobile from)
        {
            if (!this.Young)
                return false;
            
            if (Region is BaseRegion && !((BaseRegion)Region).YoungProtected)
                return false;

            if (Region.IsPartOf(typeof(DungeonRegion)))
                return false;

            if (from is BaseCreature && ((BaseCreature)from).IgnoreYoungProtection)
                return false;

            if (DateTime.UtcNow - m_LastYoungMessage > TimeSpan.FromMinutes(1.0))
            {
                m_LastYoungMessage = DateTime.UtcNow;
                SendLocalizedMessage(1019067); // A monster looks at you menacingly but does not attack.  You would be under attack now if not for your status as a new citizen of Britannia.
            }

            return true;
        }

        private DateTime m_LastYoungHeal = DateTime.MinValue;

        public bool CheckYoungHealTime()
        {
            if (DateTime.UtcNow - m_LastYoungHeal > TimeSpan.FromMinutes(5.0))
            {
                m_LastYoungHeal = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /*private static Point3D[] m_TrammelDeathDestinations = new Point3D[]
            {
                new Point3D( 1481, 1612, 20 ),
                new Point3D( 2708, 2153,  0 ),
                new Point3D( 2249, 1230,  0 ),
                new Point3D( 5197, 3994, 37 ),
                new Point3D( 1412, 3793,  0 ),
                new Point3D( 3688, 2232, 20 ),
                new Point3D( 2578,  604,  0 ),
                new Point3D( 4397, 1089,  0 ),
                new Point3D( 5741, 3218, -2 ),
                new Point3D( 2996, 3441, 15 ),
                new Point3D(  624, 2225,  0 ),
                new Point3D( 1916, 2814,  0 ),
                new Point3D( 2929,  854,  0 ),
                new Point3D(  545,  967,  0 ),
                new Point3D( 3665, 2587,  0 )
            };

        private static Point3D[] m_IlshenarDeathDestinations = new Point3D[]
            {
                new Point3D( 1216,  468, -13 ),
                new Point3D(  723, 1367, -60 ),
                new Point3D(  745,  725, -28 ),
                new Point3D(  281, 1017,   0 ),
                new Point3D(  986, 1011, -32 ),
                new Point3D( 1175, 1287, -30 ),
                new Point3D( 1533, 1341,  -3 ),
                new Point3D(  529,  217, -44 ),
                new Point3D( 1722,  219,  96 )
            };

        private static Point3D[] m_MalasDeathDestinations = new Point3D[]
            {
                new Point3D( 2079, 1376, -70 ),
                new Point3D(  944,  519, -71 )
            };

        private static Point3D[] m_TokunoDeathDestinations = new Point3D[]
            {
                new Point3D( 1166,  801, 27 ),
                new Point3D(  782, 1228, 25 ),
                new Point3D(  268,  624, 15 )
            };

        public bool YoungDeathTeleport()
        {
            if (this.Region.IsPartOf(typeof(Jail))
                || this.Region.IsPartOf("Samurai start location")
                || this.Region.IsPartOf("Ninja start location")
                || this.Region.IsPartOf("Ninja cave"))
                return false;

            Point3D loc;
            Map map;

            DungeonRegion dungeon = (DungeonRegion)this.Region.GetRegion(typeof(DungeonRegion));
            if (dungeon != null && dungeon.EntranceLocation != Point3D.Zero)
            {
                loc = dungeon.EntranceLocation;
                map = dungeon.EntranceMap;
            }
            else
            {
                loc = this.Location;
                map = this.Map;
            }

            Point3D[] list;

            if (map == Map.Trammel)
                list = m_TrammelDeathDestinations;
            else if (map == Map.Ilshenar)
                list = m_IlshenarDeathDestinations;
            else if (map == Map.Malas)
                list = m_MalasDeathDestinations;
            else if (map == Map.Tokuno)
                list = m_TokunoDeathDestinations;
            else
                return false;

            Point3D dest = Point3D.Zero;
            int sqDistance = int.MaxValue;

            for (int i = 0; i < list.Length; i++)
            {
                Point3D curDest = list[i];

                int width = loc.X - curDest.X;
                int height = loc.Y - curDest.Y;
                int curSqDistance = width * width + height * height;

                if (curSqDistance < sqDistance)
                {
                    dest = curDest;
                    sqDistance = curSqDistance;
                }
            }

            this.MoveToWorld(dest, map);
            return true;
        }*/

        private void SendYoungDeathNotice()
        {
            this.SendGump(new YoungDeathNotice());
        }

        #endregion

        public override bool CanHear(Mobile from)
        {
            return true;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return base.HandlesOnSpeech(from);
        }
        
        #region Recipes

        private Dictionary<int, bool> m_AcquiredRecipes;

        public virtual bool HasRecipe(Recipe r)
        {
            if (r == null)
                return false;

            return HasRecipe(r.ID);
        }

        public virtual bool HasRecipe(int recipeID)
        {
            if (m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey(recipeID))
                return m_AcquiredRecipes[recipeID];

            return false;
        }

        public virtual void AcquireRecipe(Recipe r)
        {
            if (r != null)
                AcquireRecipe(r.ID);
        }

        public virtual void AcquireRecipe(int recipeID)
        {
            if (m_AcquiredRecipes == null)
                m_AcquiredRecipes = new Dictionary<int, bool>();

            m_AcquiredRecipes[recipeID] = true;
        }

        public virtual void ResetRecipes()
        {
            m_AcquiredRecipes = null;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int KnownRecipes
        {
            get
            {
                if (m_AcquiredRecipes == null)
                    return 0;

                return m_AcquiredRecipes.Count;
            }
        }

        #endregion

        #region Buff Icons

        public void ResendBuffs()
        {
            if (!BuffInfo.Enabled || m_BuffTable == null)
                return;

            NetState state = this.NetState;

            if (state != null && state.BuffIcon)
            {
                foreach (BuffInfo info in m_BuffTable.Values)
                {
                    state.Send(new AddBuffPacket(this, info));
                }
            }
        }

        private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

        public void AddBuff(BuffInfo b)
        {
            if (!BuffInfo.Enabled || b == null)
                return;

            RemoveBuff(b);	//Check & subsequently remove the old one.

            if (m_BuffTable == null)
                m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();

            m_BuffTable.Add(b.ID, b);

            NetState state = this.NetState;

            if (state != null && state.BuffIcon)
            {
                state.Send(new AddBuffPacket(this, b));
            }
        }

        public void RemoveBuff(BuffInfo b)
        {
            if (b == null)
                return;

            RemoveBuff(b.ID);
        }

        public void RemoveBuff(BuffIcon b)
        {
            if (m_BuffTable == null || !m_BuffTable.ContainsKey(b))
                return;

            BuffInfo info = m_BuffTable[b];

            if (info.Timer != null && info.Timer.Running)
                info.Timer.Stop();

            m_BuffTable.Remove(b);

            NetState state = this.NetState;

            if (state != null && state.BuffIcon)
            {
                state.Send(new RemoveBuffPacket(this, b));
            }

            if (m_BuffTable.Count <= 0)
                m_BuffTable = null;
        }
        #endregion

        public override void SpecialAbilityTimerTick()
        {
            SpecialAbilities.TimerTick(this);
        }
    }
}
