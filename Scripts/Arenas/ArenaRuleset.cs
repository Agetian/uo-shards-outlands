using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
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
    public class ArenaRuleset: Item
    {
        public enum ArenaRulesetFailureType
        {
            None,
            ArenaInvalid,
            NotOnline,
            Dead,
            NotInArenaRegion,
            Young,
            Transformed,
            EquipmentAllowed,
            PoisonedWeapon,
            Mount,
            Follower,
            PackAnimal,
        }

        public enum ArenaRulesetType
        {
            Duel,
            Tournament
        }

        public enum ArenaPresetType
        {
            PlayerSaved,
            DuelBasic,
            DuelDexerFriendly,
            DuelTamerFriendly
        }

        //-----

        public enum MatchTypeType
        {
            Unranked1vs1,
            Unranked2vs2,
            Unranked3vs3,
            Unranked4vs4,

            Ranked1vs1,
            Ranked2vs2,
            Ranked3vs3,
            Ranked4vs4,
        }

        public enum ListingModeType
        {
            Public,
            GuildOnly,
            PartyOnly
        } 

        public enum RoundDurationType
        {
            ThreeMinutes,
            FiveMinutes,
            TenMinutes,
            FifteenMinutes,
            TwentyMinutes,
        }

        public enum SuddenDeathModeType
        {
            DamageIncreasedTenPercentPerMinute,
            DamageIncreasedTwentyFivePercentPerMinute,
            DamageIncreasedFiftyPercentPerMinute,

            DamageIncreasedTwentyFivePercent,
            DamageIncreasedFiftyPercent,
            DamageIncreasedOneHundredPercent,                       
        }       

        public enum EquipmentAllowedType
        {
            GMRegularMaterials,
            GMAnyMaterials,
            NoRestrictions
        }

        public enum PoisonedWeaponsStartingRestrictionType
        {
            None,
            One,
            Two,
            Three,
            Five,
            Unlimited
        }

        public enum MountsRestrictionType
        {
            NotAllowed,
            Allowed
        }

        public enum FollowersRestrictionType
        {
            None,
            OneControlSlot,
            TwoControlSlot,
            ThreeControlSlot,
            FourControlSlot,
            FiveControlSlot,
        }

        public enum ResourceConsumptionType
        {
            Normal,
            UnlimitedReagentsBandagesArrows,
            UnlimitedEverything
        }

        public enum ItemDurabilityDamageType
        {
            Normal,
            HalfDamage,
            NoDamage,
        }
   
        [CommandProperty(AccessLevel.GameMaster)]
        public int TeamSize
        {
            get
            {
                switch (m_MatchType)
                {
                    case MatchTypeType.Unranked1vs1: return 1; break;
                    case MatchTypeType.Unranked2vs2: return 2; break;
                    case MatchTypeType.Unranked3vs3: return 3; break;
                    case MatchTypeType.Unranked4vs4: return 4; break;

                    case MatchTypeType.Ranked1vs1: return 1; break;
                    case MatchTypeType.Ranked2vs2: return 2; break;
                    case MatchTypeType.Ranked3vs3: return 3; break;
                    case MatchTypeType.Ranked4vs4: return 4; break;
                }

                return 1;
            }            
        }
        
        public ArenaRulesetType m_RulesetType = ArenaRulesetType.Duel;
        public ArenaPresetType m_PresetType = ArenaPresetType.DuelBasic;

        public MatchTypeType m_MatchType = MatchTypeType.Unranked1vs1;
        public ListingModeType m_ListingMode = ListingModeType.Public;
        public RoundDurationType m_RoundDuration = RoundDurationType.TenMinutes;
        public SuddenDeathModeType m_SuddenDeathMode = SuddenDeathModeType.DamageIncreasedTwentyFivePercentPerMinute;       
        public PoisonedWeaponsStartingRestrictionType m_PoisonedWeaponsRestriction = PoisonedWeaponsStartingRestrictionType.None;
        public MountsRestrictionType m_MountsRestriction = MountsRestrictionType.NotAllowed;
        public FollowersRestrictionType m_FollowersRestrictionType = FollowersRestrictionType.None;
        public EquipmentAllowedType m_EquipmentAllowed = EquipmentAllowedType.GMRegularMaterials;
        public ResourceConsumptionType m_ResourceConsumption = ResourceConsumptionType.Normal;
        public ItemDurabilityDamageType m_ItemDurabilityDamage = ItemDurabilityDamageType.Normal;

        public List<ArenaSpellRestriction> m_SpellRestrictions = new List<ArenaSpellRestriction>();
        public List<ArenaItemRestriction> m_ItemRestrictions = new List<ArenaItemRestriction>();

        public bool IsTemporary = false;

        [Constructable]
        public ArenaRuleset(): base(0x0)
        {
            CreateSpellRestrictionEntries();
            CreateItemRestrictionEntries();

            ApplyDefaultRuleset();

            Visible = false;
            Movable = false;
        }

        public ArenaRuleset(Serial serial): base(serial)
        {
        }

        public void ApplyDefaultRuleset()
        {
            m_MatchType = MatchTypeType.Unranked1vs1;
            m_ListingMode = ListingModeType.Public;
            m_RoundDuration = RoundDurationType.TenMinutes;
            m_SuddenDeathMode = SuddenDeathModeType.DamageIncreasedTwentyFivePercentPerMinute;
            m_EquipmentAllowed = EquipmentAllowedType.GMRegularMaterials;
            m_PoisonedWeaponsRestriction = PoisonedWeaponsStartingRestrictionType.None;
            m_MountsRestriction = MountsRestrictionType.NotAllowed;
            m_FollowersRestrictionType = FollowersRestrictionType.None;
            m_ResourceConsumption = ResourceConsumptionType.Normal;
            m_ItemDurabilityDamage = ItemDurabilityDamageType.Normal;

            SetSpellRestriction(typeof(PoisonSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited);
            SetSpellRestriction(typeof(PoisonFieldSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited);
            SetSpellRestriction(typeof(ParalyzeSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited);
            SetSpellRestriction(typeof(ParalyzeFieldSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited);
            SetSpellRestriction(typeof(EarthquakeSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited);

            SetItemRestriction(typeof(Pouch), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
            SetItemRestriction(typeof(BaseHealPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
            SetItemRestriction(typeof(BaseCurePotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
            SetItemRestriction(typeof(BaseRefreshPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
            SetItemRestriction(typeof(BaseStrengthPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
            SetItemRestriction(typeof(BaseAgilityPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
            SetItemRestriction(typeof(BaseExplosionPotion), ArenaItemRestriction.ItemRestrictionModeType.Disabled);
            SetItemRestriction(typeof(BasePoisonPotion), ArenaItemRestriction.ItemRestrictionModeType.Disabled);
            SetItemRestriction(typeof(BaseMagicResistPotion), ArenaItemRestriction.ItemRestrictionModeType.Disabled);
        }

        public static void ApplyRulesetPreset(ArenaGumpObject arenaGumpObject)
        {
            if (arenaGumpObject == null) return;
            if (arenaGumpObject.m_Player == null) return;
            if (arenaGumpObject.m_ArenaRuleset == null) return;

            ArenaPresetType m_ArenaPreset = arenaGumpObject.m_ArenaRuleset.m_PresetType;

            arenaGumpObject.m_ArenaRuleset.Delete();

            ArenaPlayerSettings.CheckCreateArenaPlayerSettings(arenaGumpObject.m_Player);

            switch (arenaGumpObject.m_ArenaRuleset.m_PresetType)
            {
                case ArenaPresetType.PlayerSaved:
                    arenaGumpObject.m_ArenaRuleset = new ArenaRuleset();

                    ArenaRuleset.LoadPlayerPresetSettings(arenaGumpObject);
                break;

                case ArenaPresetType.DuelBasic:
                    arenaGumpObject.m_ArenaRuleset = new ArenaRuleset();
                break;

                case ArenaPresetType.DuelDexerFriendly:
                    arenaGumpObject.m_ArenaRuleset = new ArenaRuleset();

                    arenaGumpObject.m_ArenaRuleset.m_PoisonedWeaponsRestriction = PoisonedWeaponsStartingRestrictionType.Two;

                    arenaGumpObject.m_ArenaRuleset.SetSpellRestriction(typeof(PoisonSpell), ArenaSpellRestriction.SpellRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetSpellRestriction(typeof(PoisonFieldSpell), ArenaSpellRestriction.SpellRestrictionModeType.TenUses);
                    arenaGumpObject.m_ArenaRuleset.SetSpellRestriction(typeof(ParalyzeSpell), ArenaSpellRestriction.SpellRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetSpellRestriction(typeof(ParalyzeFieldSpell), ArenaSpellRestriction.SpellRestrictionModeType.TenUses);

                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(Pouch), ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseHealPotion), ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseCurePotion), ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseRefreshPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseStrengthPotion), ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseAgilityPotion), ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseExplosionPotion), ArenaItemRestriction.ItemRestrictionModeType.TenUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BasePoisonPotion), ArenaItemRestriction.ItemRestrictionModeType.FiveUses);
                    arenaGumpObject.m_ArenaRuleset.SetItemRestriction(typeof(BaseMagicResistPotion), ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses);
                break;

                case ArenaPresetType.DuelTamerFriendly:
                    arenaGumpObject.m_ArenaRuleset = new ArenaRuleset();

                    arenaGumpObject.m_ArenaRuleset.m_FollowersRestrictionType = FollowersRestrictionType.ThreeControlSlot;
                break;
            }

            arenaGumpObject.m_ArenaRuleset.m_PresetType = m_ArenaPreset;
        }

        public static List<ArenaRuleDetails> GetBasicRulesDetails(ArenaRulesetType arenaRulesetType)
        {
            List<ArenaRuleDetails> m_Rules = new List<ArenaRuleDetails>();

            switch (arenaRulesetType)
            {
                case ArenaRulesetType.Duel:
                    m_Rules.Add(new ArenaRuleDetails(typeof(MatchTypeType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ListingModeType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(RoundDurationType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(SuddenDeathModeType), AccessLevel.Player));                   
                    m_Rules.Add(new ArenaRuleDetails(typeof(PoisonedWeaponsStartingRestrictionType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(MountsRestrictionType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(FollowersRestrictionType), AccessLevel.Player));

                    m_Rules.Add(new ArenaRuleDetails(typeof(EquipmentAllowedType), AccessLevel.Player));
                    //m_Rules.Add(new ArenaRuleDetails(typeof(ResourceConsumptionType), AccessLevel.Seer));
                    //m_Rules.Add(new ArenaRuleDetails(typeof(ItemDurabilityDamageType), AccessLevel.Seer));
                break;

                case ArenaRulesetType.Tournament:
                    m_Rules.Add(new ArenaRuleDetails(typeof(MatchTypeType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ListingModeType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(RoundDurationType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(SuddenDeathModeType), AccessLevel.Player));                   
                    m_Rules.Add(new ArenaRuleDetails(typeof(PoisonedWeaponsStartingRestrictionType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(MountsRestrictionType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(FollowersRestrictionType), AccessLevel.Player));

                    m_Rules.Add(new ArenaRuleDetails(typeof(EquipmentAllowedType), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ResourceConsumptionType), AccessLevel.Seer));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ItemDurabilityDamageType), AccessLevel.Seer));
                break;
            }

            return m_Rules;
        }
        

        public static void SavePlayerPresetSettings(ArenaGumpObject arenaGumpObject)
        {
            if (arenaGumpObject == null) return;
            if (arenaGumpObject.m_Player == null) return;

            ArenaPlayerSettings.CheckCreateArenaPlayerSettings(arenaGumpObject.m_Player);

            ArenaRuleset playerRuleset = arenaGumpObject.m_Player.m_ArenaPlayerSettings.m_SavedRulesetPreset;
            ArenaRuleset gumpRuleset = arenaGumpObject.m_ArenaRuleset;

            #region Rules

            playerRuleset.m_MatchType = gumpRuleset.m_MatchType;
            playerRuleset.m_ListingMode = gumpRuleset.m_ListingMode;
            playerRuleset.m_RoundDuration = gumpRuleset.m_RoundDuration;
            playerRuleset.m_SuddenDeathMode = gumpRuleset.m_SuddenDeathMode;
            playerRuleset.m_EquipmentAllowed = gumpRuleset.m_EquipmentAllowed;
            playerRuleset.m_PoisonedWeaponsRestriction = gumpRuleset.m_PoisonedWeaponsRestriction;
            playerRuleset.m_MountsRestriction = gumpRuleset.m_MountsRestriction;
            playerRuleset.m_FollowersRestrictionType = gumpRuleset.m_FollowersRestrictionType;
            playerRuleset.m_ResourceConsumption = gumpRuleset.m_ResourceConsumption;
            playerRuleset.m_ItemDurabilityDamage = gumpRuleset.m_ItemDurabilityDamage;

            playerRuleset.SetSpellRestriction(typeof(PoisonSpell), gumpRuleset.GetSpellRestriction(typeof(PoisonSpell)).m_RestrictionMode);
            playerRuleset.SetSpellRestriction(typeof(PoisonFieldSpell), gumpRuleset.GetSpellRestriction(typeof(PoisonFieldSpell)).m_RestrictionMode);
            playerRuleset.SetSpellRestriction(typeof(ParalyzeSpell), gumpRuleset.GetSpellRestriction(typeof(ParalyzeSpell)).m_RestrictionMode);
            playerRuleset.SetSpellRestriction(typeof(ParalyzeFieldSpell), gumpRuleset.GetSpellRestriction(typeof(ParalyzeFieldSpell)).m_RestrictionMode);
            playerRuleset.SetSpellRestriction(typeof(EarthquakeSpell), gumpRuleset.GetSpellRestriction(typeof(EarthquakeSpell)).m_RestrictionMode);

            playerRuleset.SetItemRestriction(typeof(Pouch), gumpRuleset.GetItemRestriction(typeof(Pouch)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseHealPotion), gumpRuleset.GetItemRestriction(typeof(BaseHealPotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseCurePotion), gumpRuleset.GetItemRestriction(typeof(BaseCurePotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseRefreshPotion), gumpRuleset.GetItemRestriction(typeof(BaseRefreshPotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseStrengthPotion), gumpRuleset.GetItemRestriction(typeof(BaseStrengthPotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseAgilityPotion), gumpRuleset.GetItemRestriction(typeof(BaseAgilityPotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseExplosionPotion), gumpRuleset.GetItemRestriction(typeof(BaseExplosionPotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BasePoisonPotion), gumpRuleset.GetItemRestriction(typeof(BasePoisonPotion)).m_RestrictionMode);
            playerRuleset.SetItemRestriction(typeof(BaseMagicResistPotion), gumpRuleset.GetItemRestriction(typeof(BaseMagicResistPotion)).m_RestrictionMode);

            #endregion

            arenaGumpObject.m_ArenaRuleset.m_PresetType = ArenaRuleset.ArenaPresetType.PlayerSaved;
        }

        public static void CopyRulesetSettings(ArenaRuleset rulesetFrom, ArenaRuleset rulesetTarget)
        {
            if (rulesetFrom == null) return;
            if (rulesetTarget == null) return;

            rulesetTarget.m_MatchType = rulesetFrom.m_MatchType;
            rulesetTarget.m_ListingMode = rulesetFrom.m_ListingMode;
            rulesetTarget.m_RoundDuration = rulesetFrom.m_RoundDuration;
            rulesetTarget.m_SuddenDeathMode = rulesetFrom.m_SuddenDeathMode;
            rulesetTarget.m_EquipmentAllowed = rulesetFrom.m_EquipmentAllowed;
            rulesetTarget.m_PoisonedWeaponsRestriction = rulesetFrom.m_PoisonedWeaponsRestriction;
            rulesetTarget.m_MountsRestriction = rulesetFrom.m_MountsRestriction;
            rulesetTarget.m_FollowersRestrictionType = rulesetFrom.m_FollowersRestrictionType;
            rulesetTarget.m_ResourceConsumption = rulesetFrom.m_ResourceConsumption;
            rulesetTarget.m_ItemDurabilityDamage = rulesetFrom.m_ItemDurabilityDamage;

            rulesetTarget.SetSpellRestriction(typeof(PoisonSpell), rulesetFrom.GetSpellRestriction(typeof(PoisonSpell)).m_RestrictionMode);
            rulesetTarget.SetSpellRestriction(typeof(PoisonFieldSpell), rulesetFrom.GetSpellRestriction(typeof(PoisonFieldSpell)).m_RestrictionMode);
            rulesetTarget.SetSpellRestriction(typeof(ParalyzeSpell), rulesetFrom.GetSpellRestriction(typeof(ParalyzeSpell)).m_RestrictionMode);
            rulesetTarget.SetSpellRestriction(typeof(ParalyzeFieldSpell), rulesetFrom.GetSpellRestriction(typeof(ParalyzeFieldSpell)).m_RestrictionMode);
            rulesetTarget.SetSpellRestriction(typeof(EarthquakeSpell), rulesetFrom.GetSpellRestriction(typeof(EarthquakeSpell)).m_RestrictionMode);

            rulesetTarget.SetItemRestriction(typeof(Pouch), rulesetFrom.GetItemRestriction(typeof(Pouch)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseHealPotion), rulesetFrom.GetItemRestriction(typeof(BaseHealPotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseCurePotion), rulesetFrom.GetItemRestriction(typeof(BaseCurePotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseRefreshPotion), rulesetFrom.GetItemRestriction(typeof(BaseRefreshPotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseStrengthPotion), rulesetFrom.GetItemRestriction(typeof(BaseStrengthPotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseAgilityPotion), rulesetFrom.GetItemRestriction(typeof(BaseAgilityPotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseExplosionPotion), rulesetFrom.GetItemRestriction(typeof(BaseExplosionPotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BasePoisonPotion), rulesetFrom.GetItemRestriction(typeof(BasePoisonPotion)).m_RestrictionMode);
            rulesetTarget.SetItemRestriction(typeof(BaseMagicResistPotion), rulesetFrom.GetItemRestriction(typeof(BaseMagicResistPotion)).m_RestrictionMode);
        }

        public static void LoadPlayerPresetSettings(ArenaGumpObject arenaGumpObject)
        {
            if (arenaGumpObject == null) return;
            if (arenaGumpObject.m_Player == null) return;

            ArenaPlayerSettings.CheckCreateArenaPlayerSettings(arenaGumpObject.m_Player);

            ArenaRuleset playerRuleset = arenaGumpObject.m_Player.m_ArenaPlayerSettings.m_SavedRulesetPreset;
            ArenaRuleset gumpRuleset = arenaGumpObject.m_ArenaRuleset;

            gumpRuleset.m_MatchType = playerRuleset.m_MatchType;
            gumpRuleset.m_ListingMode = playerRuleset.m_ListingMode;
            gumpRuleset.m_RoundDuration = playerRuleset.m_RoundDuration;
            gumpRuleset.m_SuddenDeathMode = playerRuleset.m_SuddenDeathMode;
            gumpRuleset.m_EquipmentAllowed = playerRuleset.m_EquipmentAllowed;
            gumpRuleset.m_PoisonedWeaponsRestriction = playerRuleset.m_PoisonedWeaponsRestriction;
            gumpRuleset.m_MountsRestriction = playerRuleset.m_MountsRestriction;
            gumpRuleset.m_FollowersRestrictionType = playerRuleset.m_FollowersRestrictionType;
            gumpRuleset.m_ResourceConsumption = playerRuleset.m_ResourceConsumption;
            gumpRuleset.m_ItemDurabilityDamage = playerRuleset.m_ItemDurabilityDamage;

            gumpRuleset.SetSpellRestriction(typeof(PoisonSpell), playerRuleset.GetSpellRestriction(typeof(PoisonSpell)).m_RestrictionMode);
            gumpRuleset.SetSpellRestriction(typeof(PoisonFieldSpell), playerRuleset.GetSpellRestriction(typeof(PoisonFieldSpell)).m_RestrictionMode);
            gumpRuleset.SetSpellRestriction(typeof(ParalyzeSpell), playerRuleset.GetSpellRestriction(typeof(ParalyzeSpell)).m_RestrictionMode);
            gumpRuleset.SetSpellRestriction(typeof(ParalyzeFieldSpell), playerRuleset.GetSpellRestriction(typeof(ParalyzeFieldSpell)).m_RestrictionMode);
            gumpRuleset.SetSpellRestriction(typeof(EarthquakeSpell), playerRuleset.GetSpellRestriction(typeof(EarthquakeSpell)).m_RestrictionMode);

            gumpRuleset.SetItemRestriction(typeof(Pouch), playerRuleset.GetItemRestriction(typeof(Pouch)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseHealPotion), playerRuleset.GetItemRestriction(typeof(BaseHealPotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseCurePotion), playerRuleset.GetItemRestriction(typeof(BaseCurePotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseRefreshPotion), playerRuleset.GetItemRestriction(typeof(BaseRefreshPotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseStrengthPotion), playerRuleset.GetItemRestriction(typeof(BaseStrengthPotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseAgilityPotion), playerRuleset.GetItemRestriction(typeof(BaseAgilityPotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseExplosionPotion), playerRuleset.GetItemRestriction(typeof(BaseExplosionPotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BasePoisonPotion), playerRuleset.GetItemRestriction(typeof(BasePoisonPotion)).m_RestrictionMode);
            gumpRuleset.SetItemRestriction(typeof(BaseMagicResistPotion), playerRuleset.GetItemRestriction(typeof(BaseMagicResistPotion)).m_RestrictionMode);
        }       

        public static TimeSpan GetRoundDuration(ArenaRuleset.RoundDurationType durationType)
        {
            switch (durationType)
            {
                case RoundDurationType.ThreeMinutes: return TimeSpan.FromMinutes(3); break;
                case RoundDurationType.FiveMinutes: return TimeSpan.FromMinutes(5); break;
                case RoundDurationType.TenMinutes: return TimeSpan.FromMinutes(10); break;
                case RoundDurationType.FifteenMinutes: return TimeSpan.FromMinutes(15); break;
                case RoundDurationType.TwentyMinutes: return TimeSpan.FromMinutes(20); break;
            }

            return TimeSpan.FromMinutes(3);
        }

        public static TimeSpan GetSuddenDeathDuration(ArenaRuleset.RoundDurationType durationType)
        {
            switch (durationType)
            {
                case RoundDurationType.ThreeMinutes: return TimeSpan.FromMinutes(2); break;
                case RoundDurationType.FiveMinutes: return TimeSpan.FromMinutes(3); break;
                case RoundDurationType.TenMinutes: return TimeSpan.FromMinutes(5); break;
                case RoundDurationType.FifteenMinutes: return TimeSpan.FromMinutes(7); break;
                case RoundDurationType.TwentyMinutes: return TimeSpan.FromMinutes(10); break;
            }

            return TimeSpan.FromMinutes(3);
        }

        public static List<ArenaRuleDetails> GetSpellRulesDetails(ArenaRulesetType arenaRulesetType)
        {
            List<ArenaRuleDetails> m_Rules = new List<ArenaRuleDetails>();

            switch (arenaRulesetType)
            {
                case ArenaRulesetType.Duel:
                    m_Rules.Add(new ArenaRuleDetails(typeof(PoisonSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(PoisonFieldSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ParalyzeSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ParalyzeFieldSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(EarthquakeSpell), AccessLevel.Player));
                break;

                case ArenaRulesetType.Tournament:
                    m_Rules.Add(new ArenaRuleDetails(typeof(PoisonSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(PoisonFieldSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ParalyzeSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(ParalyzeFieldSpell), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(EarthquakeSpell), AccessLevel.Player));
                break;
            }

            return m_Rules;
        }

        public static List<ArenaRuleDetails> GetItemRulesDetails(ArenaRulesetType arenaRulesetType)
        {
            List<ArenaRuleDetails> m_Rules = new List<ArenaRuleDetails>();

            switch (arenaRulesetType)
            {
                case ArenaRulesetType.Duel:
                    m_Rules.Add(new ArenaRuleDetails(typeof(Pouch), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseHealPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseCurePotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseRefreshPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseStrengthPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseAgilityPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseExplosionPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BasePoisonPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseMagicResistPotion), AccessLevel.Player));
                break;

                case ArenaRulesetType.Tournament:
                    m_Rules.Add(new ArenaRuleDetails(typeof(Pouch), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseHealPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseCurePotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseRefreshPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseStrengthPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseAgilityPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseExplosionPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BasePoisonPotion), AccessLevel.Player));
                    m_Rules.Add(new ArenaRuleDetails(typeof(BaseMagicResistPotion), AccessLevel.Player));
                break;
            }

            return m_Rules;
        }

        public void ChangeBasicSetting(PlayerMobile player, int ruleIndex, int changeValue)
        {
            if (player == null) 
                return;

            List<ArenaRuleDetails> m_Rules = ArenaRuleset.GetBasicRulesDetails(m_RulesetType);
            
            if (ruleIndex >= m_Rules.Count || ruleIndex < 0)
                return;

            ArenaRuleDetails ruleDetail = m_Rules[ruleIndex];

            if (ruleDetail == null) return;
            if (ruleDetail.m_AccessLevel > player.AccessLevel) return;

            Type ruleType = ruleDetail.m_RuleType;
            int enumLength = Enum.GetNames(ruleDetail.m_RuleType).Length;
            int ruleValue = 0;

            #region Change Rule Value

            if (ruleType == typeof(MatchTypeType))
            {
                ruleValue = (int)m_MatchType;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_MatchType = (MatchTypeType)ruleValue;
            }

            if (ruleType == typeof(ListingModeType))
            {
                ruleValue = (int)m_ListingMode;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_ListingMode = (ListingModeType)ruleValue;
            }

            if (ruleType == typeof(RoundDurationType))
            {
                ruleValue = (int)m_RoundDuration;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_RoundDuration = (RoundDurationType)ruleValue;
            }

            if (ruleType == typeof(SuddenDeathModeType))
            {
                ruleValue = (int)m_SuddenDeathMode;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_SuddenDeathMode = (SuddenDeathModeType)ruleValue;
            }

            if (ruleType == typeof(EquipmentAllowedType))
            {
                ruleValue = (int)m_EquipmentAllowed;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_EquipmentAllowed = (EquipmentAllowedType)ruleValue;
            }

            if (ruleType == typeof(PoisonedWeaponsStartingRestrictionType))
            {
                ruleValue = (int)m_PoisonedWeaponsRestriction;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_PoisonedWeaponsRestriction = (PoisonedWeaponsStartingRestrictionType)ruleValue;
            }

            if (ruleType == typeof(MountsRestrictionType))
            {
                ruleValue = (int)m_MountsRestriction;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_MountsRestriction = (MountsRestrictionType)ruleValue;
            }

            if (ruleType == typeof(FollowersRestrictionType))
            {
                ruleValue = (int)m_FollowersRestrictionType;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_FollowersRestrictionType = (FollowersRestrictionType)ruleValue;
            }

            if (ruleType == typeof(ResourceConsumptionType))
            {
                ruleValue = (int)m_ResourceConsumption;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_ResourceConsumption = (ResourceConsumptionType)ruleValue;
            }

            if (ruleType == typeof(ItemDurabilityDamageType))
            {
                ruleValue = (int)m_ItemDurabilityDamage;
                ruleValue += changeValue;

                if (ruleValue >= enumLength)
                    ruleValue = 0;

                if (ruleValue < 0)
                    ruleValue = enumLength - 1;

                m_ItemDurabilityDamage = (ItemDurabilityDamageType)ruleValue;
            }

            #endregion
        }

        public void ChangeSpellSetting(PlayerMobile player, int ruleIndex, int changeValue)
        {
            if (player == null)
                return;

            List<ArenaRuleDetails> m_Rules = ArenaRuleset.GetSpellRulesDetails(m_RulesetType);
            
            if (ruleIndex >= m_Rules.Count || ruleIndex < 0)
                return;

            ArenaRuleDetails ruleDetail = m_Rules[ruleIndex];

            if (ruleDetail == null) return;
            if (ruleDetail.m_AccessLevel > player.AccessLevel) return;

            Type ruleType = ruleDetail.m_RuleType;
            int enumLength = Enum.GetNames(typeof(ArenaSpellRestriction.SpellRestrictionModeType)).Length;
            int ruleValue = 0;

            foreach (ArenaSpellRestriction spellRestriction in m_SpellRestrictions)
            {
                if (spellRestriction == null) continue;
                if (spellRestriction.m_SpellType == ruleType)
                {
                    ruleValue = (int)spellRestriction.m_RestrictionMode;
                    ruleValue += changeValue;

                    if (ruleValue >= enumLength)
                        ruleValue = 0;

                    if (ruleValue < 0)
                        ruleValue = enumLength - 1;

                    spellRestriction.m_RestrictionMode = (ArenaSpellRestriction.SpellRestrictionModeType)ruleValue;

                    break;
                }
            }
        }

        public void ChangeItemSetting(PlayerMobile player, int ruleIndex, int changeValue)
        {
            if (player == null) 
                return;

            List<ArenaRuleDetails> m_Rules = ArenaRuleset.GetItemRulesDetails(m_RulesetType);
            
            if (ruleIndex >= m_Rules.Count || ruleIndex < 0)
                return;

            ArenaRuleDetails ruleDetail = m_Rules[ruleIndex];

            if (ruleDetail == null) return;
            if (ruleDetail.m_AccessLevel > player.AccessLevel) return;

            Type ruleType = ruleDetail.m_RuleType;
            int enumLength = Enum.GetNames(typeof(ArenaItemRestriction.ItemRestrictionModeType)).Length;
            int ruleValue = 0;

            foreach (ArenaItemRestriction ItemRestriction in m_ItemRestrictions)
            {
                if (ItemRestriction == null) continue;
                if (ItemRestriction.m_ItemType == ruleType)
                {
                    ruleValue = (int)ItemRestriction.m_RestrictionMode;
                    ruleValue += changeValue;

                    if (ruleValue >= enumLength)
                        ruleValue = 0;

                    if (ruleValue < 0)
                        ruleValue = enumLength - 1;

                    ItemRestriction.m_RestrictionMode = (ArenaItemRestriction.ItemRestrictionModeType)ruleValue;

                    break;
                }
            }
        }

        public ArenaBasicRuleDetail GetBasicRuleDetail(int ruleIndex)
        {
            List<ArenaRuleDetails> m_Rules = ArenaRuleset.GetBasicRulesDetails(m_RulesetType);

            if (ruleIndex >= m_Rules.Count || ruleIndex < 0)
                return null;

            ArenaRuleDetails ruleDetail = m_Rules[ruleIndex];

            if (ruleDetail == null)
                return null;

            ArenaBasicRuleDetail basicRuleDetail = new ArenaBasicRuleDetail();
            
            #region Basic Rules

            if (ruleDetail.m_RuleType == typeof(MatchTypeType))
            {
                switch (m_MatchType)
                {
                    //Unranked
                    case MatchTypeType.Unranked1vs1:
                        basicRuleDetail.m_Line1Text = "1 vs 1";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Unranked";
                        basicRuleDetail.m_Line2Hue = 2550;
                    break;

                    case MatchTypeType.Unranked2vs2:
                        basicRuleDetail.m_Line1Text = "2 vs 2";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Unranked";
                        basicRuleDetail.m_Line2Hue = 2550;
                    break;

                    case MatchTypeType.Unranked3vs3:
                        basicRuleDetail.m_Line1Text = "3 vs 3";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Unranked";
                        basicRuleDetail.m_Line2Hue = 2550;
                    break;

                    case MatchTypeType.Unranked4vs4:
                        basicRuleDetail.m_Line1Text = "4 vs 4";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Unranked";
                        basicRuleDetail.m_Line2Hue = 2550;
                    break;

                    //Ranked
                    case MatchTypeType.Ranked1vs1:
                        basicRuleDetail.m_Line1Text = "1 vs 1";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Ranked";
                        basicRuleDetail.m_Line2Hue = 2606;
                    break;

                    case MatchTypeType.Ranked2vs2:
                        basicRuleDetail.m_Line1Text = "2 vs 2";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Ranked";
                        basicRuleDetail.m_Line2Hue = 2606;
                    break;

                    case MatchTypeType.Ranked3vs3:
                        basicRuleDetail.m_Line1Text = "3 vs 3";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Ranked";
                        basicRuleDetail.m_Line2Hue = 2606;
                    break;

                    case MatchTypeType.Ranked4vs4:
                        basicRuleDetail.m_Line1Text = "4 vs 4";
                        basicRuleDetail.m_Line1Hue = 2499;

                        basicRuleDetail.m_Line2Text = "Ranked";
                        basicRuleDetail.m_Line2Hue = 2606;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(ListingModeType))
            {
                switch (m_ListingMode)
                {
                    case ListingModeType.Public:
                        basicRuleDetail.m_Line1Text = "Public";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case ListingModeType.GuildOnly:
                        basicRuleDetail.m_Line1Text = "Guild Members Only";
                        basicRuleDetail.m_Line1Hue = 63;
                    break;

                    case ListingModeType.PartyOnly:
                        basicRuleDetail.m_Line1Text = "Party Members Only";
                        basicRuleDetail.m_Line1Hue = 2603;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(RoundDurationType))
            {
                TimeSpan m_RoundTimeRemaining = ArenaRuleset.GetRoundDuration(m_RoundDuration);
                TimeSpan m_SuddenDeathTimeRemaining = ArenaRuleset.GetSuddenDeathDuration(m_RoundDuration);

                basicRuleDetail.m_Line1Text = m_RoundTimeRemaining.TotalMinutes.ToString() + " Minutes +";
                basicRuleDetail.m_Line1Hue = 2499;
                basicRuleDetail.m_Line2Text = m_SuddenDeathTimeRemaining.TotalMinutes.ToString() + " Minutes of Sudden Death";
                basicRuleDetail.m_Line2Hue = 2550;
            }

            if (ruleDetail.m_RuleType == typeof(SuddenDeathModeType))
            {
                switch (m_SuddenDeathMode)
                {
                    case SuddenDeathModeType.DamageIncreasedTenPercentPerMinute:
                        basicRuleDetail.m_Line1Text = "+10% Damage per Minute";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case SuddenDeathModeType.DamageIncreasedTwentyFivePercentPerMinute:
                        basicRuleDetail.m_Line1Text = "+25% Damage per Minute";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case SuddenDeathModeType.DamageIncreasedFiftyPercentPerMinute:
                        basicRuleDetail.m_Line1Text = "+50% Damage per Minute";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case SuddenDeathModeType.DamageIncreasedTwentyFivePercent:
                        basicRuleDetail.m_Line1Text = "+25% Damage";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case SuddenDeathModeType.DamageIncreasedFiftyPercent:
                        basicRuleDetail.m_Line1Text = "+50% Damage";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case SuddenDeathModeType.DamageIncreasedOneHundredPercent:
                        basicRuleDetail.m_Line1Text = "+100% Damage";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(EquipmentAllowedType))
            {
                switch (m_EquipmentAllowed)
                {
                    case EquipmentAllowedType.NoRestrictions:
                        basicRuleDetail.m_Line1Text = "No Restrictions";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case EquipmentAllowedType.GMRegularMaterials:
                        basicRuleDetail.m_Line1Text = "GM or Less (Regular Materials)"; 
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case EquipmentAllowedType.GMAnyMaterials:
                        basicRuleDetail.m_Line1Text = "GM or Less (Any Materials)";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(PoisonedWeaponsStartingRestrictionType))
            {
                switch (m_PoisonedWeaponsRestriction)
                {
                    case PoisonedWeaponsStartingRestrictionType.None:
                        basicRuleDetail.m_Line1Text = "None Allowed in Inventory";
                        basicRuleDetail.m_Line1Hue = 2401;
                    break;

                    case PoisonedWeaponsStartingRestrictionType.One:
                        basicRuleDetail.m_Line1Text = "One Allowed in Inventory";
                        basicRuleDetail.m_Line1Hue = 2599;
                    break;

                    case PoisonedWeaponsStartingRestrictionType.Two:
                        basicRuleDetail.m_Line1Text = "Two Allowed in Inventory";
                        basicRuleDetail.m_Line1Hue = 2599;
                    break;

                    case PoisonedWeaponsStartingRestrictionType.Three:
                        basicRuleDetail.m_Line1Text = "Three Allowed in Inventory";
                        basicRuleDetail.m_Line1Hue = 2599;
                    break;

                    case PoisonedWeaponsStartingRestrictionType.Five:
                        basicRuleDetail.m_Line1Text = "Five Allowed in Inventory";
                        basicRuleDetail.m_Line1Hue = 2599;
                    break;

                    case PoisonedWeaponsStartingRestrictionType.Unlimited:
                        basicRuleDetail.m_Line1Text = "Unlimited Allowed in Inventory";
                        basicRuleDetail.m_Line1Hue = 63;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(MountsRestrictionType))
            {
                switch (m_MountsRestriction)
                {
                    case MountsRestrictionType.NotAllowed:
                        basicRuleDetail.m_Line1Text = "Not Allowed";
                        basicRuleDetail.m_Line1Hue = 2401;
                    break;

                    case MountsRestrictionType.Allowed:
                        basicRuleDetail.m_Line1Text = "Allowed";
                        basicRuleDetail.m_Line1Hue = 63;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(FollowersRestrictionType))
            {
                switch (m_FollowersRestrictionType)
                {
                    case FollowersRestrictionType.None:
                        basicRuleDetail.m_Line1Text = "None Allowed";
                        basicRuleDetail.m_Line1Hue = 2401;
                    break;

                    case FollowersRestrictionType.OneControlSlot:
                        basicRuleDetail.m_Line1Text = "Up to 1 Control Slot";
                        basicRuleDetail.m_Line1Hue = 2550;
                    break;

                    case FollowersRestrictionType.TwoControlSlot:
                        basicRuleDetail.m_Line1Text = "Up to 2 Control Slots";
                        basicRuleDetail.m_Line1Hue = 2550;
                    break;

                    case FollowersRestrictionType.ThreeControlSlot:
                        basicRuleDetail.m_Line1Text = "Up to 3 Control Slots";
                        basicRuleDetail.m_Line1Hue = 2550;
                    break;

                    case FollowersRestrictionType.FourControlSlot:
                        basicRuleDetail.m_Line1Text = "Up to 4 Control Slots";
                        basicRuleDetail.m_Line1Hue = 2550;
                    break;

                    case FollowersRestrictionType.FiveControlSlot:
                        basicRuleDetail.m_Line1Text = "Up to 5 Control Slots";
                        basicRuleDetail.m_Line1Hue = 63;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(ResourceConsumptionType))
            {
                switch (m_ResourceConsumption)
                {
                    case ResourceConsumptionType.Normal:
                        basicRuleDetail.m_Line1Text = "Normal";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case ResourceConsumptionType.UnlimitedReagentsBandagesArrows:
                        basicRuleDetail.m_Line1Text = "Free Usage of";
                        basicRuleDetail.m_Line1Hue = 2599;

                        basicRuleDetail.m_Line2Text = "Reagents, Bandages, and Arrows";
                        basicRuleDetail.m_Line2Hue = 2599;
                    break;

                    case ResourceConsumptionType.UnlimitedEverything:
                        basicRuleDetail.m_Line1Text = "Free Usage of";
                        basicRuleDetail.m_Line1Hue = 63;

                        basicRuleDetail.m_Line2Text = "Reagents, Bandages, Arrows, and Potions";
                        basicRuleDetail.m_Line2Hue = 63;
                    break;
                }
            }

            if (ruleDetail.m_RuleType == typeof(ItemDurabilityDamageType))
            {
                switch (m_ItemDurabilityDamage)
                {
                    case ItemDurabilityDamageType.Normal:
                        basicRuleDetail.m_Line1Text = "Normal Durability Loss";
                        basicRuleDetail.m_Line1Hue = 2499;
                    break;

                    case ItemDurabilityDamageType.HalfDamage:
                        basicRuleDetail.m_Line1Text = "-50% Durability Loss";
                        basicRuleDetail.m_Line1Hue = 2599;
                    break;

                    case ItemDurabilityDamageType.NoDamage:
                        basicRuleDetail.m_Line1Text = "No Durability Loss";
                        basicRuleDetail.m_Line1Hue = 63;
                    break;
                }
            }

            #endregion

            return basicRuleDetail;
        }

        public ArenaSpellRuleDetail GetSpellRuleDetail(int ruleIndex)
        {
            List<ArenaRuleDetails> m_Rules = ArenaRuleset.GetSpellRulesDetails(m_RulesetType);

            if (ruleIndex >= m_Rules.Count || ruleIndex < 0)
                return null;

            ArenaRuleDetails ruleDetail = m_Rules[ruleIndex];

            if (ruleDetail == null)
                return null;

            ArenaSpellRestriction spellRestriction = GetSpellRestriction(ruleDetail.m_RuleType);

            if (spellRestriction == null)
                return null;

            ArenaSpellRuleDetail spellRuleDetail = new ArenaSpellRuleDetail();

            #region Spells

            if (ruleDetail.m_RuleType == typeof(PoisonSpell))
            {
                spellRuleDetail.m_SpellName = "Poison";                

                spellRuleDetail.m_ItemID = 8023;
                spellRuleDetail.m_ItemHue = 0;
                spellRuleDetail.m_ItemOffsetX = -40;
                spellRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(PoisonFieldSpell))
            {
                spellRuleDetail.m_SpellName = "Poison Field";

                spellRuleDetail.m_ItemID = 7981;
                spellRuleDetail.m_ItemHue = 0;
                spellRuleDetail.m_ItemOffsetX = -33;
                spellRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(ParalyzeSpell))
            {
                spellRuleDetail.m_SpellName = "Paralyze";

                spellRuleDetail.m_ItemID = 7985;
                spellRuleDetail.m_ItemHue = 0;
                spellRuleDetail.m_ItemOffsetX = -33;
                spellRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(ParalyzeFieldSpell))
            {
                spellRuleDetail.m_SpellName = "Paralyze Field";

                spellRuleDetail.m_ItemID = 8023;
                spellRuleDetail.m_ItemHue = 0;
                spellRuleDetail.m_ItemOffsetX = -40;
                spellRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(EarthquakeSpell))
            {
                spellRuleDetail.m_SpellName = "AoE Spells";

                spellRuleDetail.m_ItemID = 7981;
                spellRuleDetail.m_ItemHue = 0;
                spellRuleDetail.m_ItemOffsetX = -33;
                spellRuleDetail.m_ItemOffsetY = 0;
            }

            #endregion

            #region Text

            switch (spellRestriction.m_RestrictionMode)
            {
                case ArenaSpellRestriction.SpellRestrictionModeType.Disabled:
                    spellRuleDetail.m_RuleText = "Disabled";
                    spellRuleDetail.m_TextHue = 2401;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.OneUse:
                    spellRuleDetail.m_RuleText = "1 Cast Max";
                    spellRuleDetail.m_TextHue = 2550;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.ThreeUses:
                    spellRuleDetail.m_RuleText = "3 Casts Max";
                    spellRuleDetail.m_TextHue = 2550;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.FiveUses:
                    spellRuleDetail.m_RuleText = "5 Casts Max";
                    spellRuleDetail.m_TextHue = 2550;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.TenUses:
                    spellRuleDetail.m_RuleText = "10 Casts Max";
                    spellRuleDetail.m_TextHue = 2550;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.TwentyFiveUses:
                    spellRuleDetail.m_RuleText = "25 Casts Max";
                    spellRuleDetail.m_TextHue = 2550;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.FiftyUses:
                    spellRuleDetail.m_RuleText = "50 Casts Max";
                    spellRuleDetail.m_TextHue = 2550;
                break;

                case ArenaSpellRestriction.SpellRestrictionModeType.Unlimited:
                    spellRuleDetail.m_RuleText = "Unlimited";
                    spellRuleDetail.m_TextHue = 63;
                break;
            }

            #endregion

            return spellRuleDetail;
        }

        public ArenaItemRuleDetail GetItemRuleDetail(int ruleIndex)
        {
            List<ArenaRuleDetails> m_Rules = ArenaRuleset.GetItemRulesDetails(m_RulesetType);

            if (ruleIndex >= m_Rules.Count || ruleIndex < 0)
                return null;

            ArenaRuleDetails ruleDetail = m_Rules[ruleIndex];

            if (ruleDetail == null)
                return null;

            ArenaItemRestriction itemRestriction = GetItemRestriction(ruleDetail.m_RuleType);

            if (itemRestriction == null)
                return null;

            ArenaItemRuleDetail itemRuleDetail = new ArenaItemRuleDetail();

            #region Items

            if (ruleDetail.m_RuleType == typeof(Pouch))
            {
                itemRuleDetail.m_ItemName = "Trapped Pouches";                

                itemRuleDetail.m_ItemID = 2480;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -42;
                itemRuleDetail.m_ItemOffsetY = 2;
            }

            if (ruleDetail.m_RuleType == typeof(BaseHealPotion))
            {
                itemRuleDetail.m_ItemName = "Heal Potions";

                itemRuleDetail.m_ItemID = 3852;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BaseCurePotion))
            {
                itemRuleDetail.m_ItemName = "Cure Potions";

                itemRuleDetail.m_ItemID = 3847;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BaseRefreshPotion))
            {
                itemRuleDetail.m_ItemName = "Refresh Potions";

                itemRuleDetail.m_ItemID = 3851;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BaseStrengthPotion))
            {
                itemRuleDetail.m_ItemName = "Strength Potions";

                itemRuleDetail.m_ItemID = 3849;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BaseAgilityPotion))
            {
                itemRuleDetail.m_ItemName = "Agility Potions";

                itemRuleDetail.m_ItemID = 3848;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BaseExplosionPotion))
            {
                itemRuleDetail.m_ItemName = "Explosion Potions";

                itemRuleDetail.m_ItemID = 3853;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BasePoisonPotion))
            {
                itemRuleDetail.m_ItemName = "Poison Potions";

                itemRuleDetail.m_ItemID = 3850;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            if (ruleDetail.m_RuleType == typeof(BaseMagicResistPotion))
            {
                itemRuleDetail.m_ItemName = "Resist Potions";

                itemRuleDetail.m_ItemID = 3846;
                itemRuleDetail.m_ItemHue = 0;
                itemRuleDetail.m_ItemOffsetX = -35;
                itemRuleDetail.m_ItemOffsetY = 0;
            }

            #endregion

            #region Text

            switch (itemRestriction.m_RestrictionMode)
            {
                case ArenaItemRestriction.ItemRestrictionModeType.Disabled:
                    itemRuleDetail.m_RuleText = "Disabled";
                    itemRuleDetail.m_TextHue = 2401;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.OneUse:
                    itemRuleDetail.m_RuleText = "1 Uses Max";
                    itemRuleDetail.m_TextHue = 2550;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.ThreeUses:
                    itemRuleDetail.m_RuleText = "3 Uses Max";
                    itemRuleDetail.m_TextHue = 2550;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.FiveUses:
                    itemRuleDetail.m_RuleText = "5 Uses Max";
                    itemRuleDetail.m_TextHue = 2550;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.TenUses:
                    itemRuleDetail.m_RuleText = "10 Uses Max";
                    itemRuleDetail.m_TextHue = 2550;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.TwentyFiveUses:
                    itemRuleDetail.m_RuleText = "25 Uses Max";
                    itemRuleDetail.m_TextHue = 2550;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.FiftyUses:
                    itemRuleDetail.m_RuleText = "50 Uses Max";
                    itemRuleDetail.m_TextHue = 2550;
                break;

                case ArenaItemRestriction.ItemRestrictionModeType.Unlimited:
                    itemRuleDetail.m_RuleText = "Unlimited";
                    itemRuleDetail.m_TextHue = 63;
                break;
            }

            #endregion

            return itemRuleDetail;
        }

        public ArenaPresetDetail GetPresetDetail()
        {
            ArenaPresetDetail presetDetail = new ArenaPresetDetail();

            switch (m_PresetType)
            {
                case ArenaPresetType.PlayerSaved:
                    presetDetail.m_Text = "Player Saved Presets";
                    presetDetail.m_Hue = 2603;
                break;

                case ArenaPresetType.DuelBasic:
                    presetDetail.m_Text = "Basic Duel";
                    presetDetail.m_Hue = 2499;
                break;

                case ArenaPresetType.DuelDexerFriendly:
                    presetDetail.m_Text = "Dexer-Friendly Duel";
                    presetDetail.m_Hue = 2499;
                break;

                case ArenaPresetType.DuelTamerFriendly:
                    presetDetail.m_Text = "Tamer-Friendly Duel";
                    presetDetail.m_Hue = 2499;
                break;
            }

            return presetDetail;
        }

        public void CreateSpellRestrictionEntries()
        {
            //Custom Restrictions            
            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(PoisonSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));
            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(PoisonFieldSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));

            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(ParalyzeSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));
            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(ParalyzeFieldSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));

            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(MeteorSwarmSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));
            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(ChainLightningSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));
            m_SpellRestrictions.Add(new ArenaSpellRestriction(typeof(EarthquakeSpell), ArenaSpellRestriction.SpellRestrictionModeType.Unlimited));
        }

        public void CreateItemRestrictionEntries()
        {
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(Pouch), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseHealPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseCurePotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseRefreshPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseStrengthPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseAgilityPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseExplosionPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BasePoisonPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
            m_ItemRestrictions.Add(new ArenaItemRestriction(typeof(BaseMagicResistPotion), ArenaItemRestriction.ItemRestrictionModeType.Unlimited));
        }

        public ArenaSpellRestriction GetSpellRestriction(Type spellType)
        {
            foreach (ArenaSpellRestriction spellRestriction in m_SpellRestrictions)
            {
                if (spellRestriction == null)
                    continue;

                if (spellRestriction.m_SpellType == spellType)
                    return spellRestriction;
            }

            return null;
        }

        public void SetSpellRestriction(Type spellType, ArenaSpellRestriction.SpellRestrictionModeType spellRestrictionValue)
        {
            foreach (ArenaSpellRestriction spellRestriction in m_SpellRestrictions)
            {
                if (spellRestriction == null)
                    continue;

                if (spellRestriction.m_SpellType == spellType)
                {
                    spellRestriction.m_RestrictionMode = spellRestrictionValue;                   
                }
            }
        }

        public ArenaItemRestriction GetItemRestriction(Type itemType)
        {
            foreach (ArenaItemRestriction itemRestriction in m_ItemRestrictions)
            {
                if (itemRestriction == null)
                    continue;

                if (itemRestriction.m_ItemType == itemType)
                    return itemRestriction;
            }

            return null;
        }

        public void SetItemRestriction(Type itemType, ArenaItemRestriction.ItemRestrictionModeType itemRestrictionValue)
        {
            foreach (ArenaItemRestriction itemRestriction in m_ItemRestrictions)
            {
                if (itemRestriction == null)
                    continue;

                if (itemRestriction.m_ItemType == itemType)
                {
                    itemRestriction.m_RestrictionMode = itemRestrictionValue;
                    break;
                }
            }
        }

        public bool AttemptUseSpell(PlayerMobile player, Type spellType)
        {
            ArenaParticipant arenaParticipant = player.m_ActiveArenaParticipant;

            if (arenaParticipant == null)
                return false;

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
            
            foreach (ArenaSpellRestriction spellRestriction in m_SpellRestrictions)
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

                    ArenaSpellUsage arenaSpellUsage = arenaParticipant.GetSpellUsage(spellType);

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

            if (player == null)
                return;
           
            if (player.m_ActiveArenaParticipant != null && player.m_ActiveArenaRuleset != null)
            {
                player.m_ActiveArenaParticipant.AdjustSpellUsage(spellType, 1);

                ArenaSpellUsage arenaSpellUsage = player.m_ActiveArenaParticipant.GetSpellUsage(spellType);

                int spellUses = 0;

                if (arenaSpellUsage != null)
                    spellUses = arenaSpellUsage.m_Uses;

                ArenaSpellRestriction arenaSpellRestriction = player.m_ActiveArenaRuleset.GetSpellRestriction(spellType);

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
        }
        
        public bool AttemptItemUsage(PlayerMobile player, Item item)
        {
            ArenaParticipant arenaParticipant = player.m_ActiveArenaParticipant;

            if (arenaParticipant == null)
                return false;

            bool restrictedItem = false;

            //TEST: ADD RESTRICTED ITEMS

            if (restrictedItem)
            {
                player.SendMessage("That item is not allowed here.");
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

            ArenaItemRestriction itemRestriction = GetItemRestriction(itemType);

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

                ArenaItemUsage arenaItemUsage = arenaParticipant.GetItemUsage(itemType);

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

        public ArenaRulesetFailureType CheckForRulesetViolations(ArenaMatch arenaMatch, PlayerMobile player)
        {
            if (arenaMatch == null) 
                return ArenaRulesetFailureType.ArenaInvalid;

            if (arenaMatch.Deleted) 
                return ArenaRulesetFailureType.ArenaInvalid;

            if (arenaMatch.m_ArenaGroupController == null)
                return ArenaRulesetFailureType.ArenaInvalid;

            if (arenaMatch.m_ArenaGroupController.Deleted)
                return ArenaRulesetFailureType.ArenaInvalid;
            
            if (player == null)
                return ArenaRulesetFailureType.NotOnline;

            if (player.Deleted)
                return ArenaRulesetFailureType.NotOnline;

            if (!player.Alive)
                return ArenaRulesetFailureType.Dead;

            if (player.Young)
                return ArenaRulesetFailureType.Young;

            if (!arenaMatch.m_ArenaGroupController.ArenaGroupRegionBoundary.Contains(player.Location) || arenaMatch.m_ArenaGroupController.Map != player.Map)
                return ArenaRulesetFailureType.NotInArenaRegion;

            //Polymorphed / Transformed / Disguise Kit / Incognito
            if (!player.CanBeginAction(typeof(PolymorphSpell)))
                return ArenaRulesetFailureType.Transformed;

            if (player.IsBodyMod)
                return ArenaRulesetFailureType.Transformed;

            if (DisguiseTimers.IsDisguised(player))
                return ArenaRulesetFailureType.Transformed;

            if (!player.CanBeginAction(typeof(IncognitoSpell)))
                return ArenaRulesetFailureType.Transformed;

            //Mount
            if (player.Mount != null && m_MountsRestriction == MountsRestrictionType.NotAllowed)
                return ArenaRulesetFailureType.Mount;

            //Follower
            int followerCount = 0;
            int maxFollowerCountAllowed = 0;

            if (player.Mount != null && m_MountsRestriction == MountsRestrictionType.Allowed)
                maxFollowerCountAllowed++; 

            switch(m_FollowersRestrictionType)
            {
                case FollowersRestrictionType.None: maxFollowerCountAllowed = 0; break;
                case FollowersRestrictionType.OneControlSlot: maxFollowerCountAllowed = 1; break;
                case FollowersRestrictionType.TwoControlSlot: maxFollowerCountAllowed = 2; break;
                case FollowersRestrictionType.ThreeControlSlot: maxFollowerCountAllowed = 3; break;
                case FollowersRestrictionType.FourControlSlot: maxFollowerCountAllowed = 4; break;
                case FollowersRestrictionType.FiveControlSlot: maxFollowerCountAllowed = 5; break;
            }

            foreach (Mobile mobile in player.AllFollowers)
            {
                BaseCreature bc_Creature = mobile as BaseCreature;

                if (bc_Creature == null)
                    continue;

                if (bc_Creature is PackAnimal)
                    return ArenaRulesetFailureType.PackAnimal;

                if (!arenaMatch.m_ArenaGroupController.ArenaGroupRegionBoundary.Contains(bc_Creature.Location) || arenaMatch.m_ArenaGroupController.Map != bc_Creature.Map)
                    continue;

                followerCount += bc_Creature.ControlSlots;
            }

            if (followerCount > maxFollowerCountAllowed)
                return ArenaRulesetFailureType.Follower;

            //Poisoned Weapon
            int poisonedWeaponCount = 0;
            int maxPoisonedWeaponCountAllowed = 0;

            switch (m_PoisonedWeaponsRestriction)
            {
                case PoisonedWeaponsStartingRestrictionType.None: maxPoisonedWeaponCountAllowed = 0; break;
                case PoisonedWeaponsStartingRestrictionType.One: maxPoisonedWeaponCountAllowed = 1; break;
                case PoisonedWeaponsStartingRestrictionType.Two: maxPoisonedWeaponCountAllowed = 2; break;
                case PoisonedWeaponsStartingRestrictionType.Three: maxPoisonedWeaponCountAllowed = 3; break;
                case PoisonedWeaponsStartingRestrictionType.Five: maxPoisonedWeaponCountAllowed = 5; break;
                case PoisonedWeaponsStartingRestrictionType.Unlimited: maxPoisonedWeaponCountAllowed = 1000; break;
            }

            BaseWeapon oneHandedWeapon = player.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
            BaseWeapon twoHandedWeapon = player.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (oneHandedWeapon != null)
            {
                if (oneHandedWeapon.Poison != null)
                    poisonedWeaponCount++;
            }

            if (twoHandedWeapon != null)
            {
                if (twoHandedWeapon.Poison != null)
                    poisonedWeaponCount++;
            }

            foreach (Item item in player.Backpack.Items)
            {
                BaseWeapon weapon = item as BaseWeapon;

                if (weapon == null)
                    continue;

                if (weapon.Poison != null)
                    poisonedWeaponCount++;
            }

            if (poisonedWeaponCount > maxPoisonedWeaponCountAllowed)
                return ArenaRulesetFailureType.PoisonedWeapon;

            //Equipment
            if (player.Backpack == null)
                return ArenaRulesetFailureType.EquipmentAllowed;

            List<Item> m_Items = player.GetAllItems();

            foreach (Item item in m_Items)
            {
                if (!(item is BaseWeapon || item is BaseArmor))
                    continue;

                BaseWeapon weapon = item as BaseWeapon;
                BaseArmor armor = item as BaseArmor;

                if (weapon != null)
                {
                    switch (m_EquipmentAllowed)
                    {
                        case EquipmentAllowedType.NoRestrictions:
                            break;

                        case EquipmentAllowedType.GMRegularMaterials:
                            if (weapon.IsMagical)
                                return ArenaRulesetFailureType.EquipmentAllowed;

                            if (!(weapon.Resource == CraftResource.None || weapon.Resource == CraftResource.Iron || weapon.Resource == CraftResource.RegularWood || weapon.Resource == CraftResource.RegularLeather))
                                return ArenaRulesetFailureType.EquipmentAllowed;
                            break;
                    }
                }

                if (armor != null)
                {
                    switch (m_EquipmentAllowed)
                    {
                        case EquipmentAllowedType.NoRestrictions:
                            break;

                        case EquipmentAllowedType.GMRegularMaterials:
                            if (armor.IsMagical)
                                return ArenaRulesetFailureType.EquipmentAllowed;

                            if (!(armor.Resource == CraftResource.None || armor.Resource == CraftResource.Iron || armor.Resource == CraftResource.RegularWood || armor.Resource == CraftResource.RegularLeather))
                                return ArenaRulesetFailureType.EquipmentAllowed;
                        break;

                        case EquipmentAllowedType.GMAnyMaterials:
                            if (armor.IsMagical)
                                return ArenaRulesetFailureType.EquipmentAllowed;
                        break;
                    }
                }
            }

            return ArenaRulesetFailureType.None;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write((int)m_RulesetType);
            writer.Write((int)m_PresetType);

            writer.Write((int)m_MatchType);
            writer.Write((int)m_ListingMode);
            writer.Write((int)m_RoundDuration);
            writer.Write((int)m_SuddenDeathMode);
            writer.Write((int)m_EquipmentAllowed);
            writer.Write((int)m_PoisonedWeaponsRestriction);
            writer.Write((int)m_MountsRestriction);
            writer.Write((int)m_FollowersRestrictionType);
            writer.Write((int)m_ResourceConsumption);
            writer.Write((int)m_ItemDurabilityDamage);
            
            writer.Write(m_SpellRestrictions.Count);
            for (int a = 0; a < m_SpellRestrictions.Count; a++)
            {
                if (m_SpellRestrictions[a].m_SpellType == null)
                    writer.Write("null");
                else
                    writer.Write(m_SpellRestrictions[a].m_SpellType.ToString());

                writer.Write((int)m_SpellRestrictions[a].m_RestrictionMode);
            }

            writer.Write(m_ItemRestrictions.Count);
            for (int a = 0; a < m_ItemRestrictions.Count; a++)
            {
                if (m_ItemRestrictions[a].m_ItemType == null)
                    writer.Write("null");
                else
                    writer.Write(m_ItemRestrictions[a].m_ItemType.ToString());

                writer.Write((int)m_ItemRestrictions[a].m_RestrictionMode);
            }

            writer.Write(IsTemporary);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_RulesetType = (ArenaRulesetType)reader.ReadInt();
                m_PresetType = (ArenaPresetType)reader.ReadInt();

                m_MatchType = (MatchTypeType)reader.ReadInt();
                m_ListingMode = (ListingModeType)reader.ReadInt();
                m_RoundDuration = (RoundDurationType)reader.ReadInt();
                m_SuddenDeathMode = (SuddenDeathModeType)reader.ReadInt();
                m_EquipmentAllowed = (EquipmentAllowedType)reader.ReadInt();
                m_PoisonedWeaponsRestriction = (PoisonedWeaponsStartingRestrictionType)reader.ReadInt();
                m_MountsRestriction = (MountsRestrictionType)reader.ReadInt();
                m_FollowersRestrictionType = (FollowersRestrictionType)reader.ReadInt();
                m_ResourceConsumption = (ResourceConsumptionType)reader.ReadInt();
                m_ItemDurabilityDamage = (ItemDurabilityDamageType)reader.ReadInt();

                int spellRestrictionCount = reader.ReadInt();
                for (int a = 0; a < spellRestrictionCount; a++)
                {
                    string typeText = reader.ReadString();
                    Type spellType = null;

                    if (typeText != "null")
                        spellType = Type.GetType(typeText);

                    ArenaSpellRestriction.SpellRestrictionModeType restrictionMode = (ArenaSpellRestriction.SpellRestrictionModeType)reader.ReadInt();

                    if (spellType != null)
                        m_SpellRestrictions.Add(new ArenaSpellRestriction(spellType, restrictionMode));
                }

                int itemRestrictionCount = reader.ReadInt();
                for (int a = 0; a < itemRestrictionCount; a++)
                {
                    string typeText = reader.ReadString();
                    Type itemType = null;

                    if (typeText != "null")
                        itemType = Type.GetType(typeText);

                    ArenaItemRestriction.ItemRestrictionModeType restrictionMode = (ArenaItemRestriction.ItemRestrictionModeType)reader.ReadInt();

                    if (itemType != null)
                        m_ItemRestrictions.Add(new ArenaItemRestriction(itemType, restrictionMode));
                }

                IsTemporary = reader.ReadBool();
            }

            //-----

            if (IsTemporary)
                Delete();
        }
    }

    public class ArenaSpellRestriction
    {
        public enum SpellRestrictionModeType
        {          
            Unlimited,
            OneUse,
            ThreeUses,
            FiveUses,
            TenUses,
            TwentyFiveUses,
            FiftyUses,
            Disabled,
        }

        public Type m_SpellType;
        public SpellRestrictionModeType m_RestrictionMode = SpellRestrictionModeType.Unlimited;

        public ArenaSpellRestriction(Type spellType, SpellRestrictionModeType restrictionMode)
        {
            m_SpellType = spellType;
            m_RestrictionMode = restrictionMode;
        }
    }

    public class ArenaItemRestriction
    {
        public enum ItemRestrictionModeType
        {            
            Unlimited,
            OneUse,
            ThreeUses,
            FiveUses,
            TenUses,
            TwentyFiveUses,
            FiftyUses,
            Disabled,
        }

        public Type m_ItemType;
        public ItemRestrictionModeType m_RestrictionMode = ItemRestrictionModeType.Unlimited;

        public ArenaItemRestriction(Type itemType, ItemRestrictionModeType restrictionMode)
        {
            m_ItemType = itemType;
            m_RestrictionMode = restrictionMode;
        }
    }

    public class ArenaRuleDetails
    {
        public Type m_RuleType;
        public AccessLevel m_AccessLevel = AccessLevel.Player;

        public ArenaRuleDetails(Type basicRule, AccessLevel accessLevel)
        {
            m_RuleType = basicRule;
            m_AccessLevel = accessLevel;
        }
    }

    public class ArenaBasicRuleDetail
    {
        public string m_Line1Text = "Disabled";
        public int m_Line1Hue = 2401;

        public string m_Line2Text = "";
        public int m_Line2Hue = 2499;

        public ArenaBasicRuleDetail()
        {
        }
    }

    public class ArenaSpellRuleDetail
    {
        public string m_SpellName = "Poison";
        public int m_TextHue = 2499;
        public string m_RuleText = "25 Casts Max";

        public int m_ItemID = 3828;
        public int m_ItemHue = 0; 
        public int m_ItemOffsetX = -32;
        public int m_ItemOffsetY = 0;

        public ArenaSpellRuleDetail()
        {
        }        
    }

    public class ArenaItemRuleDetail
    {
        public string m_ItemName = "Pouch";
        public int m_TextHue = 2499;
        public string m_RuleText = "25 Uses Max";

        public int m_ItemID = 2480;
        public int m_ItemHue = 0; 
        public int m_ItemOffsetX = 0;
        public int m_ItemOffsetY = 0;

        public ArenaItemRuleDetail()
        {
        }        
    }

    public class ArenaPresetDetail
    {
        public string m_Text = "Player Saved Presets";
        public int m_Hue = 2606;

        public ArenaPresetDetail()
        {
        }
    }
}