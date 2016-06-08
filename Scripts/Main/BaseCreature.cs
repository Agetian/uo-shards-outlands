#if Framework_4_0
using System.Threading.Tasks;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Multis;
using Server.Spells;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.PartySystem;

using Server.SkillHandlers;


using Server.Commands;
using System.Linq;
using Server.Engines.Craft;

namespace Server.Mobiles
{
    public enum SpeedGroupType
    {
        None,
        SuperSlow,
        VerySlow,
        Slow,
        Medium,
        Fast,
        VeryFast,
        SuperFast,
    }

    public enum SlayerGroupType
    {
        None,
        Beastial,
        Daemonic,
        Elemental,
        Humanoid,
        Monstrous,
        Undead
    }

    public enum LootDropModeType
    {
        Gold,
        Hides,
        None
    }

    public enum FightMode
    {
        None,           // Never focus on others
        Aggressor,      // Only attack aggressors
        Strongest,      // Attack the strongest
        Weakest,        // Attack the weakest
        Closest,        // Attack the closest
        Evil            // Only attack aggressor -or- negative karma
    }

    public enum OrderType
    {
        None,           //When no order, let's roam
        Come,           //"(All/Name) come"  Summons all or one pet to your location.  
        Drop,           //"(Name) drop"  Drops its loot to the ground (if it carries any).  
        Follow,         //"(Name) follow"  Follows targeted being.       
        Friend,         //"(Name) friend"  Allows targeted player to confirm resurrection. 
        Unfriend,       // Remove a friend
        Guard,          //"(Name) guard"  Makes the specified pet guard you. Pets can only guard their owner.      
        Attack,         //"(All/Name) kill",       
        Patrol,         //"(Name) patrol"  Roves between two or more guarded targets.  
        Release,        //"(Name) release"  Releases pet back into the wild (removes "tame" status). 
        Stay,           //"(All/Name) stay" All or the specified pet(s) will stop and stay in current spot. 
        Stop,           //"(All/Name) stop Cancels any current orders to attack, guard or follow.  
        Transfer,       //"(Name) transfer" Transfers complete ownership to targeted player. 
        Fetch           //"(All/Name) Walks to location of item targeted, puts it in is pack if able, and then goes into Follow Mode
    }

    [Flags]
    public enum FoodType
    {
        None = 0x0000,
        Meat = 0x0001,
        FruitsAndVegies = 0x0002,
        GrainsAndHay = 0x0004,
        Fish = 0x0008,
        Eggs = 0x0010,
        Gold = 0x0020
    }

    [Flags]
    public enum PackInstinct
    {
        None = 0x0000,
        Canine = 0x0001,
        Ostard = 0x0002,
        Feline = 0x0004,
        Arachnid = 0x0008,
        Daemon = 0x0010,
        Bear = 0x0020,
        Equine = 0x0040,
        Bull = 0x0080
    }

    public enum MeatType
    {
        //Basic
        Ribs,
        Drumstick,
        FishSteak,

        //Scaled
        Meat,
        Poultry,
        Fish,

        //Special
        Bacon,
        Ham, 
        Steaks,
        MeatShank,
        Sausage,        
        Bird,
        Fillet        
    }

    public class DamageStore : IComparable
    {
        public Mobile m_Mobile;
        public int m_Damage;
        public bool m_HasRight;

        public DamageStore(Mobile m, int damage)
        {
            m_Mobile = m;
            m_Damage = damage;
        }

        public int CompareTo(object obj)
        {
            DamageStore ds = (DamageStore)obj;

            return ds.m_Damage - m_Damage;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FriendlyNameAttribute : Attribute
    {
        private TextDefinition m_FriendlyName;

        public TextDefinition FriendlyName
        {
            get
            {
                return m_FriendlyName;
            }
        }

        public FriendlyNameAttribute(TextDefinition friendlyName)
        {
            m_FriendlyName = friendlyName;
        }

        public static TextDefinition GetFriendlyNameFor(Type t)
        {
            if (t.IsDefined(typeof(FriendlyNameAttribute), false))
            {
                object[] objs = t.GetCustomAttributes(typeof(FriendlyNameAttribute), false);

                if (objs != null && objs.Length > 0)
                {
                    FriendlyNameAttribute friendly = objs[0] as FriendlyNameAttribute;

                    return friendly.FriendlyName;
                }
            }

            return t.Name;
        }
    }

    public partial class BaseCreature : Mobile
    {
        public delegate void OnBeforeDeathCB();
        public OnBeforeDeathCB m_OnBeforeDeathCallback;

        public virtual LootDropModeType LootDropMode { get { return LootDropModeType.Gold; } }

        private bool m_FreelyLootable = false;
        public bool FreelyLootable
        {
            get { return m_FreelyLootable; }
            set { m_FreelyLootable = value; }
        }

        public virtual bool AlwaysFreelyLootable { get { return false; } }

        public bool IsFreelyLootable()
        {
            if (FreelyLootable || AlwaysFreelyLootable || IsChampMinion() || IsBossMinion() || IsLoHMinion() || IsEventMinion())
                return true;

            return false;
        }

        public int MassiveBreathRange = 4;

        public virtual bool AlwaysRun { get { return false; } }

        public virtual bool MovementRestrictionImmune { get { return false; } }
        public virtual double MovementImmunityChance
        {
            get
            {
                if (Region is UOACZRegion)
                    return 0;

                if (IsChamp())
                    return .75;

                if (IsBoss())
                    return .80;

                if (IsLoHBoss())
                    return .85;

                if (IsEventBoss())
                    return .90;

                return 0;
            }
        }

        public bool CheckMovementEffectImmunity(Mobile attacker)
        {
            if (Utility.RandomDouble() >= MovementImmunityChance)
                return false;

            return true;
        }

        public override bool Paralyze(Mobile from, double duration)
        {
            if (MovementRestrictionImmune)
            {
                from.SendMessage("That creature is immune to paralyze effects.");
                from.FixedEffect(0x3735, 6, 30);

                return false;
            }

            if (Utility.RandomDouble() <= MovementImmunityChance)
            {
                from.SendMessage("The creature has overpowered your paralyze effect.");
                from.FixedEffect(0x3735, 6, 30);

                return false;
            }

            return base.Paralyze(from, duration);
        }        

        public int m_ResurrectionsRemaining = -1; //-1 is Unlimited
        [CommandProperty(AccessLevel.Counselor)]
        public int ResurrectionsRemaining
        {
            get { return m_ResurrectionsRemaining; }
            set { m_ResurrectionsRemaining = value; }
        }

        public virtual bool ImmuneToSpecialAttacks { get { return false; } }
        public virtual bool ImmuneToChargedSpells { get { return false; } }

        public PlayerMobile LastPlayerKiller
        {
            get
            {
                Mobile killer;

                if (LastKiller != null)
                {
                    killer = LastKiller;

                    if (killer is BaseCreature)
                    {
                        BaseCreature creaturekiller = killer as BaseCreature;

                        if (creaturekiller.BardProvoked)
                            killer = creaturekiller.BardMaster; // provoked
                        else
                            killer = ((BaseCreature)killer).GetMaster(); // pet
                    }

                    if (killer != null && killer.Player)
                        return (PlayerMobile)killer;
                }

                return null;
            }
        }

        public const int MaxLoyalty = 100;

        #region Var declarations

        private BaseAI m_AI;                    // THE AI

        private AIType m_CurrentAI;             // The current AI
        private AIType m_DefaultAI;             // The default AI

        private Mobile m_FocusMob;              // Use focus mob instead of combatant, maybe we don't whan to fight
        private FightMode m_FightMode;              // The style the mob uses

        private int m_iRangePerception;     // The view area
        private int m_iRangeFight;          // The fight distance

        private bool m_bDebugAI;            // Show debug AI messages

        private int m_iTeam;            // Monster Team

        private double m_dActiveSpeed;          // Timer speed when active
        private double m_dPassiveSpeed;     // Timer speed when not active
        private double m_dCurrentSpeed;     // The current speed, lets say it could be changed by something;

        public double ActiveTamedSpeed = 0.3;  //Default tamed creature active speed
        public double PassiveTamedSpeed = 0.3;  //Default tamed creature passive speed  

        public double ActiveTamedFollowModeSpeed = 0.24;
        public double PassiveTamedFollowModeSpeed = 0.24;

        private int m_AttackSpeed = 30; //Should Match Fist Speed By Default (We override this for faster attacking monsters)
        private double m_ResolveAcquireTargetDelay = -1; //If set to a value other than -1, this becomes the new delay in seconds before this creature acquires targets

        private bool m_AcquireNewTargetEveryCombatAction = false; //If True, Sets m_NextAcquireTargetAllowed to DateTime.UTC after every melee hit / spellcast / combat special action
        private bool m_AcquireRandomizedTarget = false; //If True, it's combatant will be chosen entirely randomly from all valid possible targets
        private int m_AcquireRandomizedTargetSearchRange = 10; //The search range for acquiring a randomized target 

        public TimeSpan m_NextAcquireTargetDelay = TimeSpan.FromSeconds(1);
        public DateTime m_NextAcquireTargetAllowed = DateTime.UtcNow;

        private Mobile m_SpellTarget;
        private Mobile m_HealTarget;

        private double m_SpellDelayMin = 2;   //Minimum Delay in Seconds Before Mobile is allowed to cast next spell 
        private double m_SpellDelayMax = 4;   //Maximum Delay in Seconds Before Mobile will cast next spell

        private double m_SpellSpeedScalar = 1;   //Modifies the Animation Speed / Delay Before Spells Are Cast

        private int m_SpellHue = 0;   //Modified the Hue of Spells Cast

        public bool CastOnlyFireSpells = false;
        public bool CastOnlyEnergySpells = false;

        private DateTime m_NextCombatSpecialActionAllowed = DateTime.UtcNow;   //When Mobile is allowed to use a special action in combat
        private DateTime m_NextCombatEpicActionAllowed = DateTime.UtcNow;   //When Mobile is allowed to use an epic action in combat
        private DateTime m_NextCombatHealActionAllowed = DateTime.UtcNow;   //When Mobile is allowed to use a heal action in combat
        private DateTime m_NextWanderActionAllowed = DateTime.UtcNow;   //When Mobile is allowed to use a heal action in combat

        public int CombatSpecialActionMinDelay = 13; //Min Delay in seconds Between Combat Special Actions
        public int CombatSpecialActionMaxDelay = 17; //Max Delay in seconds Between Combat Special Actions        
        public int CombatEpicActionMinDelay = 50; //Min Delay in seconds Between Combat Epic Actions  
        public int CombatEpicActionMaxDelay = 70; //Max Delay in seconds Between Combat Epic Actions    
        public int CombatHealActionMinDelay = 8; //Min Delay in seconds Between Combat Heal Actions  
        public int CombatHealActionMaxDelay = 12; //Max Delay in seconds Between Combat Heal Actions  
        public int WanderActionMinDelay = 5; //Min Delay in seconds Between Wander Actions     
        public int WanderActionMaxDelay = 10; //Max Delay in seconds Between Wander Actions       

        public int DefaultHomeRange = 10;
        public int WalkRandomOutsideHomeLimit = 10; //Length of time creature will walk around outside home
        public int WalkTowardsHomeLimit = 20; //Length of time creature will attempt to walk home before teleporting
        public DateTime PostPeacemakingTeleportDelay;
        
        public double DecisionTimeDelay = 1.0; //Delay between decisions for AI    
        public int CreatureBandageSelfDuration = 12; //Seconds it takes for creature to bandage heal self
        public int CreatureBandageOtherDuration = 5; //Seconds it takes for creature to bandage heal other
        public int CreatureSpellCastRange = 12; //Maximum Casting Distance Allowed for Creature Spells
        public int CreatureSpellRange = 8; //Spell Range for AI Casting Preference
        public int CreatureWithdrawRange = 8; //Preferred Minimum Distance for Withdrawing without weapon (otherwise use weapon distance)
        public static int DefaultPerceptionRange = 10; //Default Range for Creatures to Engage and Make Decisions Within

        public int BandageTimeoutLength = 10; //Seconds Allowed for a Creature to Try a Bandaging Action: Canceled if never reaches target after this duration
        public double GuardModeTargetDelay = 1.0; //Delay for creature in guard mode before acting on acquired target (Has 1 second inherent delay)
        public double WanderModeTargetDelay = 2.0; //Delay for creature in wander mode before acting on acquired target (Has 1 second inherent delay)
        public double LowManaPercent = 25; //If Creature is below this percent of mana, considered low on mana (will flag melee combat usually)

        public virtual int MaxDistanceAllowedFromHome { get { return 200; } }

        public static double StealthStepSuccessBasePercent = 50; //Base % chance to enter stealth / successfully move step
        public static double StealthStepSkillBonusDivider = 2; //Divided by Skill = Bonus % chance to enter stealth / successfully move steps
        public static double StealthStepFreeStepsDivider = 5; //Divided by Skill (floor) = Free Stealth Steps (not tested for skill) on Entering Stealth
        public static double StealthFootprintChance = .33; //Chance on Stealth Movement of Leaving Footprint Image Behind
        public static double StealthFootprintRevealImmuneChance = .05; //Chance on Stealth Movement for Creatures Immune to Reveal to Leave Footprint Image Behind

        public double TamedDamageAgainstPlayerDisruptChance = .02; //Tamed Creature Chance to Disrupt a Player Per Point of Damage Inflicted (Effects Bandages / Spells / Meditation)

        public bool m_WasFishedUp = false;

        private Point3D m_pHome;                // The home position of the creature, used by some AI
        private Point3D m_LastEnemyLocation;    // Last known location of enemy combatant
        private int m_iRangeHome = 10;      // The home range of the creature

        private bool m_bControlled;         // Is controlled
        private Mobile m_ControlMaster;     // My master
        private Mobile m_ControlTarget;     // My target mobile
        private Point3D m_ControlDest;          // My target destination (patrol)
        private OrderType m_ControlOrder;           // My order
        private Item m_ControlObject;       // My desired object to interact with

        private int m_Loyalty;

        private bool m_Summoned = false;
        private DateTime m_SummonEnd;
        private int m_iControlSlots = 1;

        private int m_SpellCastAnimation = 4;   //Animation to Use for Spellcasting
        private int m_SpellCastFrameCount = 4;   //Frame Count in Spellcasting Animation

        private bool m_SuperPredator = false;
        private bool m_Predator = false;
        private bool m_Prey = false;

        private WayPoint m_CurrentWaypoint = null;

        public virtual int WaypointCompletionProximityNeeded { get { return 0; } }

        private List<WayPoint> m_VisitedWaypoints = new List<WayPoint>();
        private WayPointOrder m_WaypointOrder = WayPointOrder.Forward;
        private DateTime m_NextWaypointAction = DateTime.UtcNow;
        private IPoint2D m_TargetLocation = null;

        private Mobile m_SummonMaster;

        private int m_HitsMax = -1;
        private int m_StamMax = -1;
        private int m_ManaMax = -1;
        private int m_DamageMin = -1;
        private int m_DamageMax = -1;

        private int m_PhysicalResistance, m_PhysicalDamage = 100;
        private int m_FireResistance, m_FireDamage;
        private int m_ColdResistance, m_ColdDamage;
        private int m_PoisonResistance, m_PoisonDamage;
        private int m_EnergyResistance, m_EnergyDamage;
        private int m_ChaosDamage;
        private int m_DirectDamage;

        private List<Mobile> m_Owners;

        private bool m_Paragon;

        private bool m_IsPrisoner;

        private string m_CorpseNameOverride;

        private int m_FailedReturnHome; /* return to home failure counter */

        private DateTime m_NextDecisionTime = DateTime.UtcNow;
        private bool m_AIActionInProgress = false;

        public const int DefaultRangePerception = 12;        

        public static double TamedCreatureBackstabScalar = 1.0;

        public static double BasePvPMeleeDamageScalar = .66;
        public static double BasePvPSpellDamageScalar = .66;
        public static double BasePvPAbilityDamageScalar = .66;

        public static double BasePvPHenchmenDamageScalar = 1.0;

        public double PvPMeleeDamageScalar = BasePvPMeleeDamageScalar;
        public double PvPSpellDamageScalar = BasePvPSpellDamageScalar;
        public double PvPAbilityDamageScalar = BasePvPAbilityDamageScalar;

        public static double HerdingFocusedAggressionDuration = 60;
        public static double HerdingFocusedAggressionDamageBonus = .20;
        public static double HerdingFocusedAggressionPvPDamageScalar = .50;

        public static double ForensicEvalCarveResourceScalarBonus = .5;

        public static double GoldDropVariation = .10;
        public static double NewbieRegionGoldDropScalar = .5;

        public List<Item> ArcaneItems = new List<Item>();
        public List<ArcaneItemExperienceEntry> ArcaneItemExperienceEntries = new List<ArcaneItemExperienceEntry>();

        public virtual void SetRare()
        {
        }

        public virtual void SetUniqueAI()
        {
        }

        public virtual void SetTamedAI()
        {
        }

        public void StoreBaseSummonValues()
        {
            BaseSummonedDamageMin = DamageMin;
            BaseSummonedDamageMax = DamageMax;
            BaseSummonedHitsMax = HitsMax;
        }

        public void RevertToBaseSummonValues()
        {
            DamageMin = BaseSummonedDamageMin;
            DamageMax = BaseSummonedDamageMax;
            HitsMaxSeed = BaseSummonedHitsMax;

            if (Hits >= HitsMaxSeed)
                Hits = HitsMaxSeed;
        }

        public override void LastPlayerCombatTimeChanged()
        {
            if (LastPlayerCombatTime != DateTime.MinValue && Summoned)
                RevertToBaseSummonValues();

            base.LastPlayerCombatTimeChanged();
        }

        #endregion

        #region Tamed Creature Base Values        

        private bool m_GeneratedTamedStats = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GeneratedTamedStats
        {
            get { return m_GeneratedTamedStats; }
            set { m_GeneratedTamedStats = value; }
        }

        public void GenerateTamedScalars()
        {
            double minScalar = m_MinTamedStatSkillScalar;
            double maxScalar = m_MaxTamedStatSkillScalar;

            if (RareTamable)
                minScalar = m_RareMinTamedStatSkillScalar;

            m_TamedBaseMaxHitsCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBaseDexCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBaseMaxManaCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());

            double damageScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());

            m_TamedBaseMinDamageCreationScalar = damageScalar;
            m_TamedBaseMaxDamageCreationScalar = damageScalar;

            m_TamedBaseWrestlingCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBaseEvalIntCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBaseMageryCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBaseMeditationCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBaseMagicResistCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
            m_TamedBasePoisoningCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());

            m_TamedBaseVirtualArmorCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());

            m_GeneratedTamedStats = true;
        }

        public void CheckRareTamedScalars()
        {
            if (RareTamable)
            {
                bool changedValues = false;

                double minScalar = m_RareMinTamedStatSkillScalar;
                double maxScalar = m_MaxTamedStatSkillScalar;

                if (m_TamedBaseMaxHitsCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseMaxHitsCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseDexCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseDexCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseMaxManaCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseMaxManaCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseMinDamageCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseMinDamageCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    m_TamedBaseMaxDamageCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());

                    changedValues = true;
                }

                if (m_TamedBaseWrestlingCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseWrestlingCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseEvalIntCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseEvalIntCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseMageryCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseMageryCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseMeditationCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseMeditationCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseMagicResistCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseMagicResistCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBasePoisoningCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBasePoisoningCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (m_TamedBaseVirtualArmorCreationScalar < m_RareMinTamedStatSkillScalar)
                {
                    m_TamedBaseVirtualArmorCreationScalar = minScalar + ((maxScalar - minScalar) * Utility.RandomDouble());
                    changedValues = true;
                }

                if (changedValues)
                    ApplyExperience();
            }
        }        

        private int m_CreaturesKilled = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CreaturesKilled
        {
            get { return m_CreaturesKilled; }
            set { m_CreaturesKilled = value; }
        }               

        //Base Creature Stats/Skill Tamed Adjustment: Generated Upon Creature Spawn
        public double m_MinTamedStatSkillScalar = .90;
        public double m_RareMinTamedStatSkillScalar = 1.0;
        public double m_MaxTamedStatSkillScalar = 1.10;

        public static double HitsExperienceScalar = .50;
        public static double ManaExperienceScalar = .50;
        public static double DamageExperienceScalar = .33;
        public static double WrestlingExperienceScalar = .25;
        public static double EvalIntExperienceScalar = 1.0;
        public static double MageryExperienceScalar = .5;
        public static double MeditationExperienceScalar = .5;
        public static double MagicResistExperienceScalar = .5;
        public static double PoisoningExperienceScalar = .10;
        public static double VirtualArmorExperienceScalar = .5;

        private string m_InitialName = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string InitialName
        {
            get { return m_InitialName; }
            set { m_InitialName = value; }
        }

        public string GetTamedDisplayName()
        {
            if (RawName == InitialName)
                return Utility.Capitalize(RawName);

            else            
                return Utility.Capitalize(RawName) + " the " + Utility.Capitalize(TamedDisplayName);        
        }
        
        public virtual string TamedDisplayName { get { return RawName; } }

        public virtual int TamedItemId { get { return -1; } }
        public virtual int TamedItemHue { get { return 0; } }
        public virtual int TamedItemXOffset { get { return 0; } }
        public virtual int TamedItemYOffset { get { return 0; } }

        public virtual int TamedBaseMaxHits { get { return HitsMax; } }
        public virtual int TamedBaseMaxMana { get { return ManaMax; } }

        public virtual int TamedBaseMinDamage { get { return DamageMin; } }
        public virtual int TamedBaseMaxDamage { get { return DamageMax; } }

        public virtual double TamedBaseWrestling { get { return Skills[SkillName.Wrestling].Value; } }
        public virtual double TamedBaseArchery { get { return Skills[SkillName.Archery].Value; } }
        public virtual double TamedBaseFencing { get { return Skills[SkillName.Fencing].Value; } }
        public virtual double TamedBaseMacing { get { return Skills[SkillName.Macing].Value; } }
        public virtual double TamedBaseSwords { get { return Skills[SkillName.Swords].Value; } }

        public virtual double TamedBaseEvalInt { get { return Skills[SkillName.EvalInt].Value; } }
        public virtual double TamedBaseMagery { get { return Skills[SkillName.Magery].Value; } }
        public virtual double TamedBaseMeditation { get { return Skills[SkillName.Meditation].Value; } }
        public virtual double TamedBaseMagicResist { get { return Skills[SkillName.MagicResist].Value; } }
        public virtual double TamedBasePoisoning { get { return Skills[SkillName.Poisoning].Value; } }

        public virtual int TamedBaseVirtualArmor { get { return VirtualArmor; } }

        public virtual int TamedBaseStr { get { return RawStr; } }
        public virtual int TamedBaseDex { get { return RawDex; } }
        public virtual int TamedBaseInt { get { return RawInt; } }

        public virtual double TamedBaseTactics { get { return Skills[SkillName.Tactics].Value; } }

        public virtual bool RareTamable { get { return false; } }

        //Tamed Creature Scalars: Generated Upon Spawning
        private double m_TamedBaseMaxHitsCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMaxHitsCreationScalar
        {
            get { return m_TamedBaseMaxHitsCreationScalar; }
            set { m_TamedBaseMaxHitsCreationScalar = value; }
        }

        private double m_TamedBaseDexCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseDexCreationScalar
        {
            get { return m_TamedBaseDexCreationScalar; }
            set { m_TamedBaseDexCreationScalar = value; }
        }

        private double m_TamedBaseMaxManaCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMaxManaCreationScalar
        {
            get { return m_TamedBaseMaxManaCreationScalar; }
            set { m_TamedBaseMaxManaCreationScalar = value; }
        }

        private double m_TamedBaseMinDamageCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMinDamageCreationScalar
        {
            get { return m_TamedBaseMinDamageCreationScalar; }
            set { m_TamedBaseMinDamageCreationScalar = value; }
        }

        private double m_TamedBaseMaxDamageCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMaxDamageCreationScalar
        {
            get { return m_TamedBaseMaxDamageCreationScalar; }
            set { m_TamedBaseMaxDamageCreationScalar = value; }
        }

        private double m_TamedBaseWrestlingCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseWrestlingCreationScalar
        {
            get { return m_TamedBaseWrestlingCreationScalar; }
            set { m_TamedBaseWrestlingCreationScalar = value; }
        }

        private double m_TamedBaseEvalIntCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseEvalIntCreationScalar
        {
            get { return m_TamedBaseEvalIntCreationScalar; }
            set { m_TamedBaseEvalIntCreationScalar = value; }
        }

        private double m_TamedBaseMageryCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMageryCreationScalar
        {
            get { return m_TamedBaseMageryCreationScalar; }
            set { m_TamedBaseMageryCreationScalar = value; }
        }

        private double m_TamedBaseMeditationCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMeditationCreationScalar
        {
            get { return m_TamedBaseMeditationCreationScalar; }
            set { m_TamedBaseMeditationCreationScalar = value; }
        }

        private double m_TamedBaseMagicResistCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseMagicResistCreationScalar
        {
            get { return m_TamedBaseMagicResistCreationScalar; }
            set { m_TamedBaseMagicResistCreationScalar = value; }
        }

        private double m_TamedBasePoisoningCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBasePoisoningCreationScalar
        {
            get { return m_TamedBasePoisoningCreationScalar; }
            set { m_TamedBasePoisoningCreationScalar = value; }
        }

        private double m_TamedBaseVirtualArmorCreationScalar = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedBaseVirtualArmorCreationScalar
        {
            get { return m_TamedBaseVirtualArmorCreationScalar; }
            set { m_TamedBaseVirtualArmorCreationScalar = value; }
        }

        public DateTime m_NextExperienceGain = DateTime.UtcNow;

        public static int MinExpGainActiveDelay = 5; //Minimum Delay in Minutes Before Next Check For Creature XP Gain: Actively Playing (Fighting Many Creatures)
        public static int MinExpGainMacroDelay = 30; //Minimum Delay in Minutes Before Next Check For Creature XP Gain: Macroing (Against One Creature)

        private int m_Experience = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get { return m_Experience; }
            set
            {
                int cumulativeExperience = GetCumulativeExperience();
                int maxCumulativeExperience = GetMaxCumulativeExperience();

                int incomingValue = value;

                if (incomingValue > maxCumulativeExperience)
                    incomingValue = maxCumulativeExperience;

                if (incomingValue < 0)
                    incomingValue = 0;

                if (incomingValue == cumulativeExperience)
                    return;

                int experienceLeftToAssign = incomingValue;

                if (experienceLeftToAssign < 0)
                    experienceLeftToAssign = 0;

                PlayerMobile pm_Controller = ControlMaster as PlayerMobile;

                int oldExperience = m_Experience;
                int oldLevel = ExperienceLevel;                

                int newLevel = 0;
                int newExperience = 0;                

                for (int a = 0; a < ExperiencePerLevel.Length; a++)
                {
                    if (experienceLeftToAssign == 0)
                        continue;

                    int experienceForLevel = ExperiencePerLevel[a];

                    if (experienceLeftToAssign < experienceForLevel)
                    {
                        newExperience = experienceLeftToAssign;
                        experienceLeftToAssign = 0;
                    }

                    else
                    {
                        experienceLeftToAssign -= experienceForLevel;
                        newExperience = 0;
                        newLevel++;
                    }

                    if (newLevel == ExperiencePerLevel.Length)
                        experienceLeftToAssign = 0;
                }

                ExperienceLevel = newLevel;
                m_Experience = newExperience;

                if (ExperienceLevel == ExperiencePerLevel.Length)
                    m_Experience = 0;

                if (newLevel < oldLevel)
                {
                    for (int a = 0; a < m_FollowerTraitSelections.Count; a++)
                    {
                        if (newLevel < a + 1)
                            m_FollowerTraitSelections[a] = FollowerTraitType.None;
                    }
                }

                if (newLevel > oldLevel)
                {
                    PlaySound(GetIdleSound());
                    AnimateIdle();

                    if (pm_Controller != null)
                        pm_Controller.SendMessage(0x3F, RawName + " has acquired enough experience to reach level " + ExperienceLevel.ToString() + "!");                          
                }                

                if (m_ExperienceLevel < BaseCreature.RequiredLevelForBonding && IsBonded)
                    IsBonded = false;

                else if (oldLevel < BaseCreature.RequiredLevelForBonding && m_ExperienceLevel >= BaseCreature.RequiredLevelForBonding)
                {
                    if (pm_Controller != null)                       
                        pm_Controller.SendMessage(0x3F, RawName + " has bonded to you permanently.");

                    IsBonded = true;
                }

                ApplyExperience();
            }
        }

        public static int GetMaxLevelExperience(int level)
        {
            if (level > ExperiencePerLevel.Length)
                return 0;

            else if (level < 0)
                return 0;

            else
                return ExperiencePerLevel[level];
        }

        public int GetCumulativeExperience()
        {
            int total = 0;

            for (int a = 0; a < ExperiencePerLevel.Length; a++)
            {
                if (ExperienceLevel == a)
                    total += Experience;

                else if (ExperienceLevel > a)
                    total += ExperiencePerLevel[a];
            }

            return total;
        }

        public int GetMaxCumulativeExperience()
        {           
            int total = 0;

            for (int a = 0; a < ExperiencePerLevel.Length; a++)
            {
                total += ExperiencePerLevel[a];
            }

            return total;            
        }

        public double GetExperienceScalar()
        {
            if (!Tameable)
                return 0;

            if (Controlled && ControlMaster is PlayerMobile)
                return (double)GetCumulativeExperience() / (double)GetMaxCumulativeExperience();            

            else return 0;
        }

        private int m_ExperienceLevel = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExperienceLevel
        {
            get { return m_ExperienceLevel; }
            set 
            { 
                m_ExperienceLevel = value;

                if (m_ExperienceLevel < 0)
                    m_ExperienceLevel = 0;

                if (m_ExperienceLevel > ExperiencePerLevel.Length)
                    m_ExperienceLevel = ExperiencePerLevel.Length;               
            }
        }

        public static int RequiredLevelForBonding = 1;
        public static int[] ExperiencePerLevel = new int[] { 50, 100, 150, 200, 250 };

        public static int MaxExperienceLevel
        {
            get { return ExperiencePerLevel.Length; }
        }

        public static int TraitsAvailablePerLevel = 2;

        public List<FollowerTraitType> m_FollowerTraitSelections = new List<FollowerTraitType>()
        {
            FollowerTraitType.None,
            FollowerTraitType.None,
            FollowerTraitType.None,
            FollowerTraitType.None,
            FollowerTraitType.None
        };

        public virtual List<FollowerTraitType> TraitsSelectionsAvailable         
        { 
           get 
           { 
                return new List<FollowerTraitType>()
                {
                    FollowerTraitType.Debilitate, FollowerTraitType.Entangle,
                    FollowerTraitType.Cripple, FollowerTraitType.Hinder,
                    FollowerTraitType.Pierce, FollowerTraitType.Rend,
                    FollowerTraitType.Frenzy, FollowerTraitType.Contagion,
                    FollowerTraitType.Enrage, FollowerTraitType.Contagion
                }
           ;} 
        }    
        
        #endregion

        public virtual int DoubloonValue { get { return 0; } }

        public virtual bool IsOceanCreature { get { return false; } }

        public bool DiedByShipSinking = false;

        public virtual string TitleReward { get { return ""; } }

        public virtual InhumanSpeech SpeechType { get { return null; } }
        public virtual bool IsHighSeasBodyType { get { return false; } }
        public virtual bool HasAlternateHighSeasAttackAnimation { get { return false; } }
        public virtual bool HasAlternateHighSeasHurtAnimation { get { return false; } }

        public virtual int AttackAnimation { get { return -1; } }
        public virtual int AttackFrames { get { return 0; } }
        public virtual bool AttackAnimationPlayForwards { get { return true; } }

        public virtual int HurtAnimation { get { return -1; } }
        public virtual int HurtFrames { get { return 0; } }
        public virtual bool HurtAnimationPlayForwards { get { return true; } }

        public virtual int IdleAnimation { get { return -1; } }
        public virtual int IdleFrames { get { return 0; } }
        public virtual bool IdleAnimationPlayForwards { get { return true; } }

        public int BaseSummonedDamageMin = 1;
        public int BaseSummonedDamageMax = 1;
        public int BaseSummonedHitsMax = 1;

        protected bool HasBurrowed = false;

        public BaseCreature(AIType ai,
            FightMode mode,
            int iRangePerception,
            int iRangeFight,
            double dActiveSpeed,
            double dPassiveSpeed)
        {
            m_Loyalty = MaxLoyalty;

            Hunger = Utility.RandomMinMax(10, 20);

            m_iRangePerception = iRangePerception;
            m_iRangeFight = iRangeFight;

            m_FightMode = mode;

            m_iTeam = 0;

            m_bDebugAI = false;

            m_bControlled = false;
            m_ControlMaster = null;
            m_ControlTarget = null;
            m_ControlOrder = OrderType.None;

            m_Tameable = false;

            m_Owners = new List<Mobile>();

            m_NextReacquireTime = Core.TickCount + (int)ReacquireDelay.TotalMilliseconds;

            ChangeAIType(AI);

            InhumanSpeech speechType = this.SpeechType;

            if (speechType != null)
                speechType.OnConstruct(this);

            if (IsInvulnerable && !Core.AOS)
                NameHue = 0x35;

            GenerateLoot(true);

            m_VisitedWaypoints = new List<WayPoint>();

            m_SpecialAbilityEffectEntries = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToAdd = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToRemove = new List<SpecialAbilityEffectEntry>();

            m_SpecialAbilityEffectTimer = new SpecialAbilityEffectTimer(this);
            m_SpecialAbilityEffectTimer.Stop();

            Timer.DelayCall(TimeSpan.FromMilliseconds(3000), delegate
            {
                if (this != null)
                {
                    if (!this.Deleted)
                        StartStamFreeMoveAuraTimer();
                }
            });

            SetSpeed();
            PopulateDefaultAI();
            UpdateAI(true);

            if (Controlled && ControlMaster is PlayerMobile)
                ApplyExperience();

            Timer.DelayCall(TimeSpan.FromMilliseconds(100), delegate
            {
                if (this != null)                
                    InitialName = RawName;
            });            
        }

        public BaseCreature(Serial serial): base(serial)
        {
            m_bDebugAI = false;

            if (Controlled && ControlMaster is PlayerMobile)
                ApplyExperience();
        }

        public void SetSpeed()
        {
            switch (BaseSpeedGroup)
            {
                case SpeedGroupType.SuperSlow: ActiveSpeed = 0.8; PassiveSpeed = 0.9; break;
                case SpeedGroupType.VerySlow: ActiveSpeed = 0.6; PassiveSpeed = 0.7; break;
                case SpeedGroupType.Slow: ActiveSpeed = 0.5; PassiveSpeed = 0.6; break;
                case SpeedGroupType.Medium: ActiveSpeed = 0.4; PassiveSpeed = 0.5; break;
                case SpeedGroupType.Fast: ActiveSpeed = 0.35; PassiveSpeed = 0.45; break;
                case SpeedGroupType.VeryFast: ActiveSpeed = 0.3; PassiveSpeed = 0.4; break;
                case SpeedGroupType.SuperFast: ActiveSpeed = 0.25; PassiveSpeed = 0.35; break;
            }
        }

        #region AI Dictionaries

        //Combat AI
        public Dictionary<CombatTargeting, int> DictCombatTargeting = new Dictionary<CombatTargeting, int>();
        public Dictionary<CombatTargetingWeight, int> DictCombatTargetingWeight = new Dictionary<CombatTargetingWeight, int>();
        public Dictionary<CombatRange, int> DictCombatRange = new Dictionary<CombatRange, int>();
        public Dictionary<CombatFlee, int> DictCombatFlee = new Dictionary<CombatFlee, int>();
        public Dictionary<CombatAction, int> DictCombatAction = new Dictionary<CombatAction, int>();
        public Dictionary<CombatSpell, int> DictCombatSpell = new Dictionary<CombatSpell, int>();
        public Dictionary<CombatHealSelf, int> DictCombatHealSelf = new Dictionary<CombatHealSelf, int>();
        public Dictionary<CombatHealOther, int> DictCombatHealOther = new Dictionary<CombatHealOther, int>();
        public Dictionary<CombatSpecialAction, int> DictCombatSpecialAction = new Dictionary<CombatSpecialAction, int>();
        public Dictionary<CombatEpicAction, int> DictCombatEpicAction = new Dictionary<CombatEpicAction, int>();

        //Guard AI
        public Dictionary<GuardAction, int> DictGuardAction = new Dictionary<GuardAction, int>();

        //Wander AI
        public Dictionary<WanderAction, int> DictWanderAction = new Dictionary<WanderAction, int>();

        //Waypoint AI
        public Dictionary<WaypointAction, int> DictWaypointAction = new Dictionary<WaypointAction, int>();

        //Interact AI
        public Dictionary<InteractAction, int> DictInteractAction = new Dictionary<InteractAction, int>();

        //Set Default BaseAI Values        
        public virtual void PopulateDefaultAI()
        {
            SuperPredator = false;
            Predator = false;
            Prey = false;

            //Combat Targeting
            DictCombatTargeting.Clear();
            DictCombatTargeting.Add(CombatTargeting.OpposingFaction, 1);
            DictCombatTargeting.Add(CombatTargeting.PlayerGood, 0);
            DictCombatTargeting.Add(CombatTargeting.PlayerCriminal, 0);
            DictCombatTargeting.Add(CombatTargeting.PlayerAny, 0);
            DictCombatTargeting.Add(CombatTargeting.SuperPredator, 0);
            DictCombatTargeting.Add(CombatTargeting.Predator, 0);
            DictCombatTargeting.Add(CombatTargeting.Prey, 0);
            DictCombatTargeting.Add(CombatTargeting.Good, 0);
            DictCombatTargeting.Add(CombatTargeting.Neutral, 0);
            DictCombatTargeting.Add(CombatTargeting.Evil, 0);
            DictCombatTargeting.Add(CombatTargeting.Aggressor, 3);
            DictCombatTargeting.Add(CombatTargeting.Any, 0);
            DictCombatTargeting.Add(CombatTargeting.None, 0);

            DictCombatTargeting.Add(CombatTargeting.UOACZHuman, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZIgnoreHumanSentry, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZHumanPlayer, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZEvilHumanPlayer, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZUndead, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZUndeadPlayer, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZWildlife, 0);
            DictCombatTargeting.Add(CombatTargeting.UOACZEvilWildlife, 0);

            //Combat Targeting Weights: 
            DictCombatTargetingWeight.Clear();
            DictCombatTargetingWeight.Add(CombatTargetingWeight.CurrentCombatant, 4);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Closest, 3);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.AttackOrder, 3);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.GuardOrder, 3);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.PatrolOrder, 3);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.HighestHitPoints, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.LowestHitPoints, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.HighestArmor, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.LowestArmor, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Ranged, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Spellcaster, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Summoned, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Poisoner, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.MostCombatants, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.LeastCombatants, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Tamed, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.Player, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.WeakToPoison, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.HardestToHit, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.EasiestToHit, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.HighestResist, 0);
            DictCombatTargetingWeight.Add(CombatTargetingWeight.LowestResist, 0);

            //Combat Range
            DictCombatRange.Clear();
            DictCombatRange.Add(CombatRange.WeaponAttackRange, 1);
            DictCombatRange.Add(CombatRange.SpellRange, 0);
            DictCombatRange.Add(CombatRange.Withdraw, 0);

            //Combat Flee
            DictCombatFlee.Clear();
            DictCombatFlee.Add(CombatFlee.None, 0);
            DictCombatFlee.Add(CombatFlee.Flee50, 0);
            DictCombatFlee.Add(CombatFlee.Flee25, 0);
            DictCombatFlee.Add(CombatFlee.Flee10, 0);
            DictCombatFlee.Add(CombatFlee.Flee5, 0);

            //Combat Action
            DictCombatAction.Clear();
            DictCombatAction.Add(CombatAction.None, 1);
            DictCombatAction.Add(CombatAction.AttackOnly, 10);
            DictCombatAction.Add(CombatAction.CombatSpell, 0);
            DictCombatAction.Add(CombatAction.CombatHealSelf, 0);
            DictCombatAction.Add(CombatAction.CombatHealOther, 0);
            DictCombatAction.Add(CombatAction.CombatSpecialAction, 0);
            DictCombatAction.Add(CombatAction.CombatEpicAction, 0);

            //Combat Spell
            DictCombatSpell.Clear();
            DictCombatSpell.Add(CombatSpell.None, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage1, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage2, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage3, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage4, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage5, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage6, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamage7, 0);
            DictCombatSpell.Add(CombatSpell.SpellDamageAOE7, 0);
            DictCombatSpell.Add(CombatSpell.SpellPoison, 0);
            DictCombatSpell.Add(CombatSpell.SpellNegative1to3, 0);
            DictCombatSpell.Add(CombatSpell.SpellNegative4to7, 0);
            DictCombatSpell.Add(CombatSpell.SpellSummon5, 0);
            DictCombatSpell.Add(CombatSpell.SpellSummon8, 0);
            DictCombatSpell.Add(CombatSpell.SpellDispelSummon, 0);
            DictCombatSpell.Add(CombatSpell.SpellHarmfulField, 0);
            DictCombatSpell.Add(CombatSpell.SpellNegativeField, 0);
            DictCombatSpell.Add(CombatSpell.SpellBeneficial1to2, 0);
            DictCombatSpell.Add(CombatSpell.SpellBeneficial3to5, 0);

            //Combat Heal Self Actions
            DictCombatHealSelf.Clear();
            DictCombatHealSelf.Add(CombatHealSelf.None, 0);
            DictCombatHealSelf.Add(CombatHealSelf.SpellHealSelf100, 0);
            DictCombatHealSelf.Add(CombatHealSelf.SpellHealSelf75, 0);
            DictCombatHealSelf.Add(CombatHealSelf.SpellHealSelf50, 0);
            DictCombatHealSelf.Add(CombatHealSelf.SpellHealSelf25, 0);
            DictCombatHealSelf.Add(CombatHealSelf.SpellCureSelf, 0);
            DictCombatHealSelf.Add(CombatHealSelf.PotionHealSelf100, 0);
            DictCombatHealSelf.Add(CombatHealSelf.PotionHealSelf75, 0);
            DictCombatHealSelf.Add(CombatHealSelf.PotionHealSelf50, 0);
            DictCombatHealSelf.Add(CombatHealSelf.PotionHealSelf25, 0);
            DictCombatHealSelf.Add(CombatHealSelf.PotionCureSelf, 0);
            DictCombatHealSelf.Add(CombatHealSelf.BandageHealSelf100, 0);
            DictCombatHealSelf.Add(CombatHealSelf.BandageHealSelf75, 0);
            DictCombatHealSelf.Add(CombatHealSelf.BandageHealSelf50, 0);
            DictCombatHealSelf.Add(CombatHealSelf.BandageHealSelf25, 0);
            DictCombatHealSelf.Add(CombatHealSelf.BandageCureSelf, 0);

            //Combat Heal Other Actions
            DictCombatHealOther.Clear();
            DictCombatHealOther.Add(CombatHealOther.None, 0);
            DictCombatHealOther.Add(CombatHealOther.SpellHealOther100, 0);
            DictCombatHealOther.Add(CombatHealOther.SpellHealOther75, 0);
            DictCombatHealOther.Add(CombatHealOther.SpellHealOther50, 0);
            DictCombatHealOther.Add(CombatHealOther.SpellHealOther25, 0);
            DictCombatHealOther.Add(CombatHealOther.SpellCureOther, 0);
            DictCombatHealOther.Add(CombatHealOther.BandageHealOther100, 0);
            DictCombatHealOther.Add(CombatHealOther.BandageHealOther75, 0);
            DictCombatHealOther.Add(CombatHealOther.BandageHealOther50, 0);
            DictCombatHealOther.Add(CombatHealOther.BandageHealOther25, 0);
            DictCombatHealOther.Add(CombatHealOther.BandageCureOther, 0);

            //Combat Special Actions
            DictCombatSpecialAction.Clear();
            DictCombatSpecialAction.Add(CombatSpecialAction.None, 0);
            DictCombatSpecialAction.Add(CombatSpecialAction.ApplyWeaponPoison, 0);
            DictCombatSpecialAction.Add(CombatSpecialAction.ThrowShipBomb, 0);
            DictCombatSpecialAction.Add(CombatSpecialAction.CauseWounds, 0);
            DictCombatSpecialAction.Add(CombatSpecialAction.FireBreathAttack, 0);
            DictCombatSpecialAction.Add(CombatSpecialAction.IceBreathAttack, 0);
            DictCombatSpecialAction.Add(CombatSpecialAction.PoisonBreathAttack, 0);

            //Combat Epic Actions
            DictCombatEpicAction.Clear();
            DictCombatEpicAction.Add(CombatEpicAction.None, 0);
            DictCombatEpicAction.Add(CombatEpicAction.MeleeBleedAoE, 0);
            DictCombatEpicAction.Add(CombatEpicAction.MassiveFireBreathAttack, 0);
            DictCombatEpicAction.Add(CombatEpicAction.MassiveIceBreathAttack, 0);
            DictCombatEpicAction.Add(CombatEpicAction.MassivePoisonBreathAttack, 0);
            DictCombatEpicAction.Add(CombatEpicAction.MassiveBoneBreathAttack, 0);
            DictCombatEpicAction.Add(CombatEpicAction.MassivePlantBreathAttack, 0);

            //Guard Actions
            DictGuardAction.Clear();
            DictGuardAction.Add(GuardAction.None, 0);
            DictGuardAction.Add(GuardAction.DetectHidden, 0);
            DictGuardAction.Add(GuardAction.SpellDispelSummon, 0);
            DictGuardAction.Add(GuardAction.SpellReveal, 0);

            //Wander Actions
            DictWanderAction.Clear();
            DictWanderAction.Add(WanderAction.None, 0);
            DictWanderAction.Add(WanderAction.SpellHealSelf100, 0);
            DictWanderAction.Add(WanderAction.PotionHealSelf100, 0);
            DictWanderAction.Add(WanderAction.BandageHealSelf100, 0);
            DictWanderAction.Add(WanderAction.SpellHealSelf50, 0);
            DictWanderAction.Add(WanderAction.PotionHealSelf50, 0);
            DictWanderAction.Add(WanderAction.BandageHealSelf50, 0);
            DictWanderAction.Add(WanderAction.SpellCureSelf, 0);
            DictWanderAction.Add(WanderAction.PotionCureSelf, 0);
            DictWanderAction.Add(WanderAction.BandageCureSelf, 0);

            DictWanderAction.Add(WanderAction.SpellHealOther100, 0);
            DictWanderAction.Add(WanderAction.BandageHealOther100, 0);
            DictWanderAction.Add(WanderAction.SpellHealOther50, 0);
            DictWanderAction.Add(WanderAction.BandageHealOther50, 0);
            DictWanderAction.Add(WanderAction.SpellCureOther, 0);
            DictWanderAction.Add(WanderAction.BandageCureOther, 0);

            DictWanderAction.Add(WanderAction.DetectHidden, 0);
            DictWanderAction.Add(WanderAction.SpellReveal, 0);
            DictWanderAction.Add(WanderAction.Stealing, 0);
            DictWanderAction.Add(WanderAction.Stealth, 0);
            DictWanderAction.Add(WanderAction.Tracking, 0);

            //Waypoint Actions
            DictWaypointAction.Clear();
            DictWaypointAction.Add(WaypointAction.None, 0);
            DictWaypointAction.Add(WaypointAction.DetectHidden, 0);
            DictWaypointAction.Add(WaypointAction.Stealth, 0);

            //Interact Actions
            DictInteractAction.Clear();
        }

        #endregion

        public virtual void UpdateAI(bool setUniqueAI)
        {
            if (Deleted || !Alive)
                return;

            AIDefinitions.UpdateAI(this);

            if (setUniqueAI)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(500), delegate
                {
                    SetUniqueAI();

                    BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

                    if (boat != null)
                    {
                        DictCombatFlee[CombatFlee.Flee50] = 0;
                        DictCombatFlee[CombatFlee.Flee25] = 0;
                        DictCombatFlee[CombatFlee.Flee10] = 0;
                        DictCombatFlee[CombatFlee.Flee5] = 0;

                        BoatOccupied = boat;
                        ReturnsHome = false;
                    }

                    PlayerMobile playerController = ControlMaster as PlayerMobile;

                    if (Controlled && playerController != null)
                    {
                        SetTamedAI();

                        DefaultPerceptionRange = 10;
                        RangePerception = 10;

                        CurrentSpeed = PassiveTamedSpeed;
                    }

                    InitialDifficulty = Difficulty;
                });
            }

            else
                InitialDifficulty = Difficulty;

            m_AI.WanderMode();
        }

        public virtual bool CheckDictionaryStatus()
        {
            //Use Last Dictionary Set as Indicator Whether Dictionaries Have Been Set Yet
            if (DictCombatTargeting == null || DictCombatTargeting.Keys.Count <= 0)
            {
                return false;
            }

            else
            {
                return true;
            }

            return false;
        }

        public void SetTamedBaseStats()
        {
            int currentHitPoints = Hits;
            int currentStamina = Stam;
            int currentMana = Mana;

            int hitsAdjusted = (int)(Math.Round((double)TamedBaseMaxHits * m_TamedBaseMaxHitsCreationScalar));
            int dexAdjusted = (int)(Math.Round((double)TamedBaseDex * m_TamedBaseDexCreationScalar));
            int manaAdjusted = (int)(Math.Round((double)TamedBaseMaxMana * m_TamedBaseMaxManaCreationScalar));

            int minDamageAdjusted = (int)(Math.Round((double)TamedBaseMinDamage * m_TamedBaseMinDamageCreationScalar));
            int maxDamageAdjusted = (int)(Math.Round((double)TamedBaseMaxDamage * m_TamedBaseMaxDamageCreationScalar));

            double wrestlingAdjusted = TamedBaseWrestling * m_TamedBaseWrestlingCreationScalar;
            double evalIntAdjusted = TamedBaseEvalInt * m_TamedBaseEvalIntCreationScalar;
            double mageryAdjusted = TamedBaseMagery * m_TamedBaseMageryCreationScalar;
            double meditationAdjusted = TamedBaseMeditation * m_TamedBaseMeditationCreationScalar;
            double magicResistAdjusted = TamedBaseMagicResist * m_TamedBaseMagicResistCreationScalar;
            double poisoningAdjusted = TamedBasePoisoning * m_TamedBasePoisoningCreationScalar;

            int virtualArmorAdjusted = (int)(Math.Round((double)TamedBaseVirtualArmor * m_TamedBaseVirtualArmorCreationScalar));

            SetStrMax(TamedBaseStr);
            SetDexMax(dexAdjusted);
            SetIntMax(TamedBaseInt);

            SetHitsMax(hitsAdjusted);
            SetManaMax(manaAdjusted);

            if (currentHitPoints < hitsAdjusted)
                Hits = currentHitPoints;

            if (currentStamina < dexAdjusted)
                Stam = currentStamina;

            if (currentMana < manaAdjusted)
                Mana = currentMana;

            SetDamage(minDamageAdjusted, maxDamageAdjusted);

            //Using Wrestling As Baseline
            SetSkill(SkillName.Wrestling, wrestlingAdjusted);
            SetSkill(SkillName.Archery, wrestlingAdjusted);
            SetSkill(SkillName.Fencing, wrestlingAdjusted);
            SetSkill(SkillName.Macing, wrestlingAdjusted);
            SetSkill(SkillName.Swords, wrestlingAdjusted);

            SetSkill(SkillName.EvalInt, evalIntAdjusted);
            SetSkill(SkillName.Magery, mageryAdjusted);
            SetSkill(SkillName.Meditation, meditationAdjusted);
            SetSkill(SkillName.MagicResist, magicResistAdjusted);
            SetSkill(SkillName.Poisoning, poisoningAdjusted);

            VirtualArmor = virtualArmorAdjusted;

            if (AIObject != null && AIObject.m_Timer != null)
                AIObject.m_Timer.Priority = TimerPriority.FiftyMS;
        }

        [CommandProperty(AccessLevel.Counselor)]
        public virtual Server.Loot.LootTier LootTier
        {
            get
            {
                return Loot.GetLootTier(this);
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public virtual double Difficulty
        {
            get
            {
                return CalculateDifficulty(null, false, false);
            }
        }

        public double m_InitialDifficulty = 0;
        [CommandProperty(AccessLevel.Counselor)]
        public double InitialDifficulty
        {
            get { return m_InitialDifficulty; }
            set { m_InitialDifficulty = value; }
        }

        [Usage("Provoke")]
        [Description("Manually Provoke a Creature Onto a Target")]
        public static void AdminProvoke(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Which creature would you like to provoke?");
            player.Target = new AdminProvokeFirstTarget(player);
        }

        private class AdminProvokeFirstTarget : Target
        {
            public AdminProvokeFirstTarget(Mobile from): base(100, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature bc_Target = targeted as BaseCreature;

                if (bc_Target != null)
                {
                    if (bc_Target.Alive && !bc_Target.Blessed)
                    {
                        from.SendMessage("Whom do you wish them to attack?");
                        from.Target = new InternalSecondTarget(from, bc_Target);
                    }

                    else
                        from.SendMessage("That cannot be provoked.");
                }

                else
                    from.SendMessage("That cannot be provoked.");
            }
        }

        private class InternalSecondTarget : Target
        {
            private BaseCreature bc_FirstCreature;

            public InternalSecondTarget(Mobile from, BaseCreature creature)
                : base(100, false, TargetFlags.None)
            {
                bc_FirstCreature = creature;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Mobile m_Target = targeted as Mobile;

                if (m_Target != null)
                {
                    if (m_Target.Alive && !m_Target.Blessed)
                    {
                        bc_FirstCreature.RevealingAction();

                        if (m_Target is BaseCreature)
                            m_Target.RevealingAction();

                        bc_FirstCreature.Provoke(from, m_Target, true, TimeSpan.FromHours(24), true);
                    }

                    else
                        from.SendMessage("That cannot be provoked.");
                }

                else
                    from.SendMessage("That cannot be provoked.");
            }
        }

        [Usage("Tame")]
        [Description("Manually Tame a Creature")]
        public static void AdminTame(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Which creature would you like to tame?");
            player.Target = new AdminTameTarget(player);
        }

        private class AdminTameTarget : Target
        {
            public AdminTameTarget(Mobile from)
                : base(100, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature bc_Target = targeted as BaseCreature;

                if (bc_Target != null)
                {
                    if (bc_Target.Tameable)
                    {
                        from.SendMessage("Who shall be their new tamer?");
                        from.Target = new AdminTamerTarget(from, bc_Target);
                    }
                    else
                        from.SendMessage("That cannot be tamed.");
                }

                else
                    from.SendMessage("That cannot be tamed.");
            }
        }

        private class AdminTamerTarget : Target
        {
            private BaseCreature bc_FirstCreature;

            public AdminTamerTarget(Mobile from, BaseCreature creature)
                : base(100, false, TargetFlags.None)
            {
                bc_FirstCreature = creature;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile player = targeted as PlayerMobile;

                if (player != null && bc_FirstCreature != null)
                {
                    if (!player.Deleted && !bc_FirstCreature.Deleted)
                    {
                        if (player.FollowersMax < player.Followers + bc_FirstCreature.ControlSlots)
                            from.SendMessage("That player has too many followers to tame this.");

                        else
                        {
                            bc_FirstCreature.Owners.Add(player);
                            bc_FirstCreature.TimesTamed++;
                            bc_FirstCreature.SetControlMaster(player);
                            bc_FirstCreature.IsBonded = false;
                            bc_FirstCreature.OwnerAbandonTime = DateTime.UtcNow + bc_FirstCreature.AbandonDelay;
                        }
                    }

                    else
                        from.SendMessage("That creature can not be tamed by that.");
                }

                else
                    from.SendMessage("That creature can not be tamed by that.");
            }
        }

        [Usage("GotoCurrentWaypoint")]
        [Description("Goto a Creature's CurrentWaypoint")]
        public static void GotoCurrentWaypoint(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Which creature would you like to go to it's waypoint?");
            player.Target = new GetGotoCurrentWaypointTarget(player);
        }

        public class GetGotoCurrentWaypointTarget : Target
        {
            private PlayerMobile m_Player;

            public GetGotoCurrentWaypointTarget(PlayerMobile player)
                : base(-1, false, TargetFlags.None)
            {
                m_Player = player;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                BaseCreature bc_Target = target as BaseCreature;

                if (bc_Target != null)
                {
                    if (bc_Target.CurrentWaypoint == null)
                        m_Player.SendMessage("That creature has no current waypont.");

                    else if (bc_Target.CurrentWaypoint.Deleted)
                        m_Player.SendMessage("That creature has no current waypont.");

                    else
                    {
                        m_Player.Location = bc_Target.CurrentWaypoint.Location;
                        m_Player.Map = bc_Target.CurrentWaypoint.Map;
                        m_Player.SendMessage("Creature waypoint is: " + bc_Target.CurrentWaypoint.Location.ToString());
                    }
                }
            }
        }

        [Usage("GetDifficulty")]
        [Description("Display the Difficulty for a creature")]
        public static void GetDifficulty(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Which creature would you like to evaluate?");
            player.Target = new GetDifficultyTarget(player);
        }        

        public class GetDifficultyTarget : Target
        {
            private PlayerMobile m_Player;

            public GetDifficultyTarget(PlayerMobile player)
                : base(-1, false, TargetFlags.None)
            {
                m_Player = player;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                BaseCreature bc_Target = target as BaseCreature;

                if (bc_Target != null)
                    bc_Target.CalculateDifficulty(from, true, true);
            }
        }

        public double CalculateDifficulty(Mobile from, bool showValue, bool showFactors)
        {
            double averageSpecialActionDelay = ((double)CombatSpecialActionMinDelay + (double)CombatSpecialActionMaxDelay) / 2;
            double averageEpicActionDelay = ((double)CombatEpicActionMinDelay + (double)CombatEpicActionMaxDelay) / 2;
            double averageHealActionDelay = ((double)CombatHealActionMinDelay + (double)CombatHealActionMaxDelay) / 2;

            bool isOnBoat = false;

            BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

            if (boat != null)
                isOnBoat = true;

            double attackSpeed = 5.0;

            BaseWeapon weapon = Weapon as BaseWeapon;

            if (weapon != null)
                attackSpeed = (double)weapon.GetDelay(this, true).TotalSeconds;

            //Melee
            double combatSkill = Enumerable.Max(new double[] { Skills.Swords.Base, Skills.Macing.Base, Skills.Archery.Base, Skills.Fencing.Base, Skills.Wrestling.Base }) / 100;
            bool isRanged = (Skills.Archery.Base / 100) == combatSkill;

            double estimatedOpponentSkill = .85;
            double hitChance = combatSkill / (estimatedOpponentSkill * 2) + .075;

            double expectedHitsPerSecond = (1 / attackSpeed) * hitChance;
            double averageDamage = ((double)DamageMin + (double)DamageMax) / 2;

            double mageAIPenaltyToMeleeDPS = 1;

            if (DictCombatRange[CombatRange.WeaponAttackRange] == 0)
                mageAIPenaltyToMeleeDPS = .5;

            double meleeMagePenaltyToMeleeDPS = 1;

            if (DictCombatAction[CombatAction.AttackOnly] > 0 && DictCombatAction[CombatAction.CombatSpell] > 0)
            {
                double attackOnlyFrequencyMultiplier = (double)DictCombatAction[CombatAction.AttackOnly] / (double)(DictCombatAction[CombatAction.AttackOnly] + (double)DictCombatAction[CombatAction.CombatSpell]);

                meleeMagePenaltyToMeleeDPS = .6 + (attackOnlyFrequencyMultiplier * .4);
            }

            double expectedMeleeDPS = expectedHitsPerSecond * averageDamage * mageAIPenaltyToMeleeDPS * meleeMagePenaltyToMeleeDPS;

            if (isRanged && !isOnBoat)
                expectedMeleeDPS *= 1.2;

            double movementScalar = 1 + ((1 - ActiveSpeed) / 15);

            if (DisallowAllMoves || CantWalk)
            {
                movementScalar = 1;

                if (CanSwim)
                    expectedMeleeDPS = .75;
                else
                    expectedMeleeDPS = .5;
            }

            double expectedPoisonDPS = 0;

            if (HitPoison != null)
                expectedPoisonDPS = expectedHitsPerSecond * 3 * (Math.Pow((HitPoison.Level + 1), 2)) * (Skills.Poisoning.Base / 100);

            if (DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison] > 0)
                expectedPoisonDPS = expectedHitsPerSecond * 3 * (Math.Pow((Math.Floor(Skills.Poisoning.Base / 25) + 1), 2)) * .10;

            double expectedCombatActionDPS = 0;

            if (DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] > 0 || DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack] > 0 || DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] > 0)
                expectedCombatActionDPS = (1 / averageSpecialActionDelay) * (DamageMax) * 1.33 / 1.5;

            //Spells
            double expectedAverageSpellDelayFactor = 1 / (((SpellDelayMin + SpellDelayMax) / 2) + 2.0);
            double mageryLevelFactor = Math.Floor(Skills.Magery.Base / 25) * 4;
            double evalIntFactor = (1 + (Skills.EvalInt.Base / 500));

            double onlyCastHealSpellsScalar = 1;

            if (DictCombatAction[CombatAction.CombatSpell] == 0)
            {
                bool castsHealingSpells = false;

                onlyCastHealSpellsScalar = 0;

                double bestHealSpellValue = 0;

                if (DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] > 1) if (.3 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .3;
                if (DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] > 1) if (.25 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .25;
                if (DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] > 1) if (.2 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .2;
                if (DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] > 1) if (.15 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .15;
                if (DictCombatHealSelf[CombatHealSelf.SpellCureSelf] > 1) if (.1 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .1;

                if (DictCombatHealOther[CombatHealOther.SpellHealOther100] > 1) if (.3 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .3;
                if (DictCombatHealOther[CombatHealOther.SpellHealOther75] > 1) if (.25 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .25;
                if (DictCombatHealOther[CombatHealOther.SpellHealOther50] > 1) if (.2 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .2;
                if (DictCombatHealOther[CombatHealOther.SpellHealOther25] > 1) if (.15 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .15;
                if (DictCombatHealOther[CombatHealOther.SpellCureOther] > 1) if (.1 > onlyCastHealSpellsScalar) onlyCastHealSpellsScalar = .1;

                if (bestHealSpellValue > 0)
                    onlyCastHealSpellsScalar = bestHealSpellValue;
            }

            double meleeMageSpellPenaltyToSpellDPS = 1;

            if (DictCombatAction[CombatAction.AttackOnly] > 0 && DictCombatAction[CombatAction.CombatSpell] > 0)
            {
                double combatSpellFrequencyMultiplier = (double)DictCombatAction[CombatAction.CombatSpell] / (double)(DictCombatAction[CombatAction.AttackOnly] + (double)DictCombatAction[CombatAction.CombatSpell]);

                meleeMageSpellPenaltyToSpellDPS = .6 + (combatSpellFrequencyMultiplier * .4);
            }

            double expectedMageryDPS = expectedAverageSpellDelayFactor * mageryLevelFactor * evalIntFactor * onlyCastHealSpellsScalar * meleeMageSpellPenaltyToSpellDPS;

            double damageDPS = expectedMeleeDPS + expectedPoisonDPS + expectedCombatActionDPS + expectedMageryDPS;

            double bestNonSpellHealActionScalar = 1;

            //Potion Self                
            if (DictCombatHealSelf[CombatHealSelf.PotionHealSelf100] > 0 && 1.25 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.25;
            if (DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] > 0 && 1.2 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.2;
            if (DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] > 0 && 1.15 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.15;
            if (DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] > 0 && 1.1 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.1;
            if (DictCombatHealSelf[CombatHealSelf.PotionCureSelf] > 0 && 1.05 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.05;

            //Bandage Self
            if (DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] > 0 && 1.25 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.25;
            if (DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] > 0 && 1.2 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.2;
            if (DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] > 0 && 1.15 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.15;
            if (DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] > 0 && 1.075 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.075;
            if (DictCombatHealSelf[CombatHealSelf.BandageCureSelf] > 0 && 1.05 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.05;

            //Bandage Other
            if (DictCombatHealOther[CombatHealOther.BandageHealOther100] > 0 && 1.4 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.4;
            if (DictCombatHealOther[CombatHealOther.BandageHealOther75] > 0 && 1.3 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.3;
            if (DictCombatHealOther[CombatHealOther.BandageHealOther50] > 0 && 1.2 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.2;
            if (DictCombatHealOther[CombatHealOther.BandageHealOther25] > 0 && 1.1 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.1;
            if (DictCombatHealOther[CombatHealOther.BandageCureOther] > 0 && 1.05 > bestNonSpellHealActionScalar) bestNonSpellHealActionScalar = 1.05;

            //Best Epic Action
            double bestCombatEpicActionScalar = 1;

            if (DictCombatEpicAction[CombatEpicAction.MeleeBleedAoE] > 0 && 1.05 > bestCombatEpicActionScalar) bestCombatEpicActionScalar = 1.05;
            if (DictCombatEpicAction[CombatEpicAction.MassiveFireBreathAttack] > 0 && 1.10 > bestCombatEpicActionScalar) bestCombatEpicActionScalar = 1.10;
            if (DictCombatEpicAction[CombatEpicAction.MassiveIceBreathAttack] > 0 && 1.10 > bestCombatEpicActionScalar) bestCombatEpicActionScalar = 1.10;
            if (DictCombatEpicAction[CombatEpicAction.MassivePoisonBreathAttack] > 0 && 1.10 > bestCombatEpicActionScalar) bestCombatEpicActionScalar = 1.10;

            //Best Wander Action
            double bestWanderActionScalar = 1;

            if (DictWanderAction[WanderAction.Stealth] > 0 && 1.05 > bestWanderActionScalar) bestWanderActionScalar = 1.05;
            if (DictWanderAction[WanderAction.DetectHidden] > 0 && 1.05 > bestWanderActionScalar) bestWanderActionScalar = 1.05;

            //Defense              
            double baseHitPoints = (double)HitsMax;
            double effectiveHitPoints = baseHitPoints;

            double lowRangeThreshold = 0;
            double midRangeThreshold = 1000;
            double highRangeThreshold = 2000;

            double lowRangeHitPoints = 0;
            double midRangeHitPoints = 0;
            double highRangeHitPoints = 0;

            double lowRangeWeight = 1.0;
            double midRangeWeight = 0.8;
            double highRangeWeight = 0.6;

            lowRangeHitPoints = baseHitPoints;

            if (lowRangeHitPoints > midRangeThreshold)
                lowRangeHitPoints = midRangeThreshold;

            if (baseHitPoints > midRangeThreshold)
                midRangeHitPoints = baseHitPoints - midRangeThreshold;

            if (midRangeHitPoints > (highRangeThreshold - midRangeThreshold))
                midRangeHitPoints = highRangeThreshold - midRangeThreshold;

            if (baseHitPoints > highRangeThreshold)
                highRangeHitPoints = baseHitPoints - highRangeThreshold;

            effectiveHitPoints = (lowRangeHitPoints * lowRangeWeight) + (midRangeHitPoints * midRangeWeight) + (highRangeHitPoints * highRangeWeight);

            double hitPointsScalar = .5 + (Math.Sqrt(effectiveHitPoints) / 15);

            double defenseSkillScalar = .75 + (combatSkill / 3);
            double parrySkillScalar = 1 + (Skills.Parry.Base / 500);
            double virtualArmorScalar = .99 + ((double)VirtualArmor / 2500);
            double magicResistScalar = .98 + (Skills.MagicResist.Base / 1250);
            double poisonImmunityScalar = 1;

            if (PoisonResistance > 0)
                poisonImmunityScalar = 1 + (Math.Pow((PoisonResistance), 2) / 250);

            double survivalScalar = hitPointsScalar * defenseSkillScalar * defenseSkillScalar * virtualArmorScalar * magicResistScalar * poisonImmunityScalar;

            double bardImmunityScalar = 1;

            if (BardImmune)
                bardImmunityScalar = 1.05;

            double miscScalars = UniqueCreatureDifficultyScalar * bestNonSpellHealActionScalar * bestCombatEpicActionScalar * bestWanderActionScalar * movementScalar * bardImmunityScalar;

            double finalValue = damageDPS * survivalScalar * miscScalars;

            //Boat Loot Adjustment
            if (isOnBoat)
                finalValue *= 1.0;

            if (from != null && showFactors)
            {
                from.SendMessage("---" + RawName + ": " + Math.Round(finalValue, 3).ToString() + "---");

                //Melee Damage
                from.SendMessage("MeleeDPS of " + Math.Round(expectedMeleeDPS, 3).ToString() + " == " + Math.Round(expectedHitsPerSecond, 3).ToString() + " * " + averageDamage.ToString() + " * " + Math.Round(mageAIPenaltyToMeleeDPS, 3).ToString() + " * " + Math.Round(meleeMagePenaltyToMeleeDPS, 3).ToString());

                //SpellDamage
                from.SendMessage("SpellDPS of " + Math.Round(expectedMageryDPS, 3).ToString() + " == " + Math.Round(expectedAverageSpellDelayFactor, 3).ToString() + " * " + Math.Round(mageryLevelFactor, 3).ToString() + " * " + Math.Round(evalIntFactor, 3).ToString() + " * " + Math.Round(onlyCastHealSpellsScalar, 3).ToString() + " * " + Math.Round(meleeMageSpellPenaltyToSpellDPS, 3).ToString());

                //Final DPS
                from.SendMessage("TotalDPS of " + Math.Round(damageDPS, 3).ToString() + " == " + Math.Round(expectedMeleeDPS, 3).ToString() + " * " + Math.Round(expectedMageryDPS, 3).ToString() + " * " + Math.Round(expectedPoisonDPS, 3).ToString() + " * " + Math.Round(expectedCombatActionDPS, 3).ToString());

                from.SendMessage("");

                from.SendMessage("SurvivalScalar of " + Math.Round(survivalScalar, 3).ToString() + " == " + Math.Round(hitPointsScalar, 3).ToString() + " * " + Math.Round(defenseSkillScalar, 3).ToString() + " * " + Math.Round(parrySkillScalar, 3).ToString() + " * " + Math.Round(virtualArmorScalar, 3).ToString() + " * " + Math.Round(magicResistScalar, 3).ToString() + " * " + Math.Round(poisonImmunityScalar, 3).ToString());

                from.SendMessage("");

                //ScalarModifiers 
                from.SendMessage("MiscScalars of " + Math.Round(miscScalars, 3).ToString() + " == " + Math.Round(UniqueCreatureDifficultyScalar, 3).ToString() + " * " + Math.Round(bestNonSpellHealActionScalar, 3).ToString() + " * " + Math.Round(bestCombatEpicActionScalar, 3).ToString() + " * " + Math.Round(movementScalar, 3).ToString() + " * " + Math.Round(bardImmunityScalar, 3).ToString());

                from.SendMessage("");
            }

            if (from != null && showValue)
            {
                from.SendMessage("Difficulty of " + Math.Round(finalValue, 3).ToString() + " == " + Math.Round(damageDPS, 3).ToString() + " * " + Math.Round(survivalScalar, 3).ToString() + " * " + Math.Round(miscScalars, 3).ToString());

                if (showFactors)
                    from.SendMessage("-------------------");

                PrivateOverheadMessage(MessageType.Regular, 0, false, "Difficulty: " + Math.Round(finalValue, 3).ToString(), from.NetState);
            }

            return finalValue;
        }

        public void ApplyExperience()
        {
            int currentHitPoints = Hits;
            int currentMana = Mana;

            SetTamedBaseStats();

            double experienceScalar = GetExperienceScalar();

            int newHitsMax = (int)(Math.Round((double)HitsMax * (1 + (HitsExperienceScalar * experienceScalar))));
            int newManaMax = (int)(Math.Round((double)ManaMax * (1 + (ManaExperienceScalar * experienceScalar))));

            double newWrestling = Skills.Wrestling.Base * (1 + (WrestlingExperienceScalar * experienceScalar));
            double newEvalInt = Skills.EvalInt.Base * (1 + (EvalIntExperienceScalar * experienceScalar));
            double newMagery = Skills.Magery.Base * (1 + (MageryExperienceScalar * experienceScalar));
            double newMeditation = Skills.Meditation.Base * (1 + (MeditationExperienceScalar * experienceScalar));
            double newMagicResist = Skills.MagicResist.Base * (1 + (MagicResistExperienceScalar * experienceScalar));
            double newPoisoning = Skills.Poisoning.Base * (1 + (PoisoningExperienceScalar * experienceScalar));

            int newVirtualArmor = (int)(Math.Round((double)VirtualArmor * (1 + (VirtualArmorExperienceScalar * experienceScalar))));

            SetHitsMax(newHitsMax);
            SetManaMax(newManaMax);

            if (currentHitPoints < newHitsMax)
                Hits = currentHitPoints;

            if (currentMana < newManaMax)
                Mana = currentMana;

            //Use Wrestling as Baseline
            SetSkill(SkillName.Wrestling, newWrestling);
            SetSkill(SkillName.Archery, newWrestling);
            SetSkill(SkillName.Fencing, newWrestling);
            SetSkill(SkillName.Macing, newWrestling);
            SetSkill(SkillName.Swords, newWrestling);

            SetSkill(SkillName.EvalInt, newEvalInt);
            SetSkill(SkillName.Magery, newMagery);
            SetSkill(SkillName.Meditation, newMeditation);
            SetSkill(SkillName.MagicResist, newMagicResist);
            SetSkill(SkillName.Poisoning, newPoisoning);

            VirtualArmor = newVirtualArmor;
        }

        public override void SpecialAbilityTimerTick()
        {
            SpecialAbilities.TimerTick(this);
        }
        
        private bool m_IsStealthing;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsStealthing // IsStealthing should be moved to Server.Mobiles
        {
            get { return m_IsStealthing; }
            set { m_IsStealthing = value; }
        }

        private bool m_DoingBandage = false;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool DoingBandage
        {
            get { return m_DoingBandage; }
            set { m_DoingBandage = value; }
        }

        private bool m_BandageOtherReady = false;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool BandageOtherReady
        {
            get { return m_BandageOtherReady; }
            set { m_BandageOtherReady = value; }
        }

        private DateTime m_BandageTimeout;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime BandageTimeout
        {
            get { return m_BandageTimeout; }
            set { m_BandageTimeout = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool SuperPredator
        {
            get { return m_SuperPredator; }
            set { m_SuperPredator = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool Predator
        {
            get { return m_Predator; }
            set { m_Predator = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool Prey
        {
            get { return m_Prey; }
            set { m_Prey = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime NextDecisionTime
        {
            get { return m_NextDecisionTime; }
            set { m_NextDecisionTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int SpellCastAnimation
        {
            get { return m_SpellCastAnimation; }
            set { m_SpellCastAnimation = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int SpellCastFrameCount
        {
            get { return m_SpellCastFrameCount; }
            set { m_SpellCastFrameCount = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime NextCombatSpecialActionAllowed
        {
            get { return m_NextCombatSpecialActionAllowed; }
            set { m_NextCombatSpecialActionAllowed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime NextCombatEpicActionAllowed
        {
            get { return m_NextCombatEpicActionAllowed; }
            set { m_NextCombatEpicActionAllowed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime NextCombatHealActionAllowed
        {
            get { return m_NextCombatHealActionAllowed; }
            set { m_NextCombatHealActionAllowed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime NextWanderActionAllowed
        {
            get { return m_NextWanderActionAllowed; }
            set { m_NextWanderActionAllowed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int AttackSpeed
        {
            get { return m_AttackSpeed; }
            set { m_AttackSpeed = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public double ResolveAcquireTargetDelay
        {
            get { return m_ResolveAcquireTargetDelay; }
            set
            {
                m_ResolveAcquireTargetDelay = value;

                if (AIObject != null && m_ResolveAcquireTargetDelay != -1)
                {
                    if (AIObject.m_Timer != null)
                        AIObject.m_Timer.Priority = TimerPriority.FiftyMS;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool AcquireNewTargetEveryCombatAction
        {
            get { return m_AcquireNewTargetEveryCombatAction; }
            set { m_AcquireNewTargetEveryCombatAction = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool AcquireRandomizedTarget
        {
            get { return m_AcquireRandomizedTarget; }
            set { m_AcquireRandomizedTarget = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int AcquireRandomizedTargetSearchRange
        {
            get { return m_AcquireRandomizedTargetSearchRange; }
            set { m_AcquireRandomizedTargetSearchRange = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public double SpellDelayMin
        {
            get { return m_SpellDelayMin; }
            set { m_SpellDelayMin = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public double SpellDelayMax
        {
            get { return m_SpellDelayMax; }
            set { m_SpellDelayMax = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public double SpellSpeedScalar
        {
            get { return m_SpellSpeedScalar; }
            set { m_SpellSpeedScalar = value; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int SpellHue
        {
            get { return m_SpellHue; }
            set { m_SpellHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AIActionInProgress
        {
            get { return m_AIActionInProgress; }
            set { m_AIActionInProgress = value; }
        }

        private bool m_SeeksHome;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SeeksHome
        {
            get { return m_SeeksHome; }
            set { m_SeeksHome = value; }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public string CorpseNameOverride
        {
            get { return m_CorpseNameOverride; }
            set { m_CorpseNameOverride = value; }
        }

        private bool m_IsStabled;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool IsStabled
        {
            get { return m_IsStabled; }
            set
            {
                m_IsStabled = value;

                if (m_IsStabled)
                    StopDeleteTimer();
            }
        }
        
        private DateTime m_TimeStabled;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public DateTime TimeStabled
        {
            get { return m_TimeStabled; }
            set { m_TimeStabled = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPrisoner
        {
            get { return m_IsPrisoner; }
            set { m_IsPrisoner = value; }
        }

        protected DateTime SummonEnd
        {
            get { return m_SummonEnd; }
            set { m_SummonEnd = value; }
        }
        
        #region Bonding

        public const bool BondingEnabled = true;

        public virtual bool IsBondable { get { return (BondingEnabled && !Summoned); } }

        public virtual TimeSpan AbandonDelay { get { return TimeSpan.FromHours(72); } }
        public virtual TimeSpan DeleteDelay { get { return TimeSpan.FromHours(6); } }

        public override bool CanRegenHits { get { return !m_IsDeadPet && !IsBoss() && !IsChamp() && base.CanRegenHits; } }
        public override bool CanRegenStam { get { return !m_IsDeadPet && base.CanRegenStam; } }
        public override bool CanRegenMana { get { return !m_IsDeadPet && base.CanRegenMana; } }

        public override bool IsDeadBondedPet { get { return m_IsDeadPet; } }

        public XmlSpawner m_XMLSpawner;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner XMLSpawner
        {
            get { return m_XMLSpawner; }
            set { m_XMLSpawner = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile LastOwner
        {
            get
            {
                if (m_Owners == null || m_Owners.Count == 0)
                    return null;

                return m_Owners[m_Owners.Count - 1];
            }
        }

        private bool m_IsBonded;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsBonded
        {
            get { return m_IsBonded; }
            set { m_IsBonded = value; InvalidateProperties(); }
        }

        private bool m_IsDeadPet;
        public bool IsDeadPet
        {
            get { return m_IsDeadPet; }
            set { m_IsDeadPet = value; }
        }

        private DateTime m_OwnerAbandonTime = DateTime.UtcNow + TimeSpan.FromDays(1000);
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime OwnerAbandonTime
        {
            get { return m_OwnerAbandonTime; }
            set { m_OwnerAbandonTime = value; }
        }

        #endregion

        #region Delete Previously Tamed Timer
        private DeleteTimer m_DeleteTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DeleteTimeLeft
        {
            get
            {
                if (m_DeleteTimer != null && m_DeleteTimer.Running)
                    return m_DeleteTimer.Next - DateTime.UtcNow;

                return TimeSpan.Zero;
            }
        }

        private class DeleteTimer : Timer
        {
            private Mobile m;

            public DeleteTimer(Mobile creature, TimeSpan delay): base(delay)
            {
                m = creature;
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                m.Delete();
            }
        }

        public void BeginDeleteTimer()
        {
            if (!(this is BaseEscortable) && !Summoned && !Deleted && !IsStabled)
            {
                StopDeleteTimer();

                m_DeleteTimer = new DeleteTimer(this, DeleteDelay);
                m_DeleteTimer.Start();
            }
        }

        public void StopDeleteTimer()
        {
            if (m_DeleteTimer != null)
            {
                m_DeleteTimer.Stop();
                m_DeleteTimer = null;
            }
        }

        #endregion

        #region Elemental Resistance/Damage

        public override int BasePhysicalResistance { get { return m_PhysicalResistance; } }
        public override int BaseFireResistance { get { return m_FireResistance; } }
        public override int BaseColdResistance { get { return m_ColdResistance; } }
        public override int BasePoisonResistance { get { return m_PoisonResistance; } }
        public override int BaseEnergyResistance { get { return m_EnergyResistance; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PhysicalResistanceSeed { get { return m_PhysicalResistance; } set { m_PhysicalResistance = value; UpdateResistances(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FireResistSeed { get { return m_FireResistance; } set { m_FireResistance = value; UpdateResistances(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ColdResistSeed { get { return m_ColdResistance; } set { m_ColdResistance = value; UpdateResistances(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonResistSeed { get { return m_PoisonResistance; } set { m_PoisonResistance = value; UpdateResistances(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EnergyResistSeed { get { return m_EnergyResistance; } set { m_EnergyResistance = value; UpdateResistances(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PhysicalDamage { get { return m_PhysicalDamage; } set { m_PhysicalDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FireDamage { get { return m_FireDamage; } set { m_FireDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ColdDamage { get { return m_ColdDamage; } set { m_ColdDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonDamage { get { return m_PoisonDamage; } set { m_PoisonDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EnergyDamage { get { return m_EnergyDamage; } set { m_EnergyDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ChaosDamage { get { return m_ChaosDamage; } set { m_ChaosDamage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DirectDamage { get { return m_DirectDamage; } set { m_DirectDamage = value; } }

        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsParagon
        {
            get { return m_Paragon; }
            set
            {
                if (m_Paragon == value)
                    return;

                else if (value)
                    Paragon.Convert(this);

                else
                    Paragon.UnConvert(this);

                m_Paragon = value;

                InvalidateProperties();
            }
        }

        public virtual bool HasManaOveride { get { return false; } }

        public List<Mobile> Owners { get { return m_Owners; } }

        public virtual bool AllowMaleTamer { get { return true; } }
        public virtual bool AllowFemaleTamer { get { return true; } }
        public virtual bool SubdueBeforeTame { get { return false; } }
        public virtual bool StatLossAfterTame { get { return SubdueBeforeTame; } }
        public virtual bool ReduceSpeedWithDamage { get { return true; } }
        public virtual bool IsSubdued { get { return SubdueBeforeTame && (Hits < (HitsMax / 10)); } }

        public virtual bool Commandable { get { return true; } }

        private int m_TimesTamed = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TimesTamed
        {
            get { return m_TimesTamed; }
            set { m_TimesTamed = value; }
        }

        // AC2: Made HitPoison and HitPoisonChance settable
        private Poison m_HitPoison;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Poison HitPoison
        {
            get { return m_HitPoison; }
            set { m_HitPoison = value; }
        }

        public double m_HitPoisonChance = 0.5;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HitPoisonChance
        {
            get { return m_HitPoisonChance; }
            set { m_HitPoisonChance = value; }
        }

        public virtual int PoisonResistance { get { return 0; } }

        public static double BreathDamageToPlayerScalar = 1.0;
        public static double BreathDamageToCreatureScalar = 1.5;

        private bool m_BardImmune;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool BardImmune { get { return m_BardImmune; } set { m_BardImmune = value; } }

        public virtual bool Unprovokable { get { return BardImmune || m_IsDeadPet; } }
        public virtual bool Uncalmable { get { return BardImmune || m_IsDeadPet; } }
        public virtual bool AreaPeaceImmune { get { return BardImmune || m_IsDeadPet; } }

        public virtual bool AllowParagon { get { return (LootTier < Loot.LootTier.Eight && !IsChamp() && !IsBoss() && !IsLoHBoss() && !IsEventBoss()) && !Rare; } }

        public virtual bool AlwaysChamp { get { return false; } }
        public virtual bool AlwaysChampMinion { get { return false; } }

        public virtual bool AlwaysBoss { get { return false; } }
        public virtual bool AlwaysBossMinion { get { return false; } }

        public virtual bool AlwaysLoHBoss { get { return false; } }
        public virtual bool AlwaysLoHMinion { get { return false; } }

        public virtual bool AlwaysEventBoss { get { return false; } }
        public virtual bool AlwaysEventMinion { get { return false; } }

        //Boss
        public bool IsBoss()
        {
            if (AlwaysBoss || Boss)
                return true;

            return false;
        }

        public bool IsBossMinion()
        {
            if (AlwaysBossMinion || BossMinion)
                return true;

            return false;
        }

        //Champ
        public bool IsChamp()
        {
            if (AlwaysChamp || Champ)
                return true;

            return false;
        }

        public bool IsChampMinion()
        {
            if (AlwaysChampMinion || ChampMinion)
                return true;

            return false;
        }

        //LoH
        public bool IsLoHBoss()
        {
            if (AlwaysLoHBoss || LoHBoss)
                return true;

            return false;
        }

        public bool IsLoHMinion()
        {
            if (AlwaysLoHMinion || LoHMinion)
                return true;

            return false;
        }

        //Event
        public bool IsEventBoss()
        {
            if (AlwaysEventBoss || EventBoss)
                return true;

            return false;
        }

        public bool IsEventMinion()
        {
            if (AlwaysEventMinion || EventMinion)
                return true;

            return false;
        }

        //Murderer
        public bool IsMurderer()
        {
            if (AlwaysMurderer || Murderer || ShortTermMurders >= 5)
                return true;

            return false;
        }

        //Ancient Mystery Creature
        public bool m_AncientMysteryCreature = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AncientMysteryCreature
        {
            get { return m_AncientMysteryCreature; }
            set { m_AncientMysteryCreature = value; }
        }

        //Boss
        public bool m_Boss = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Boss
        {
            get { return m_Boss; }
            set { m_Boss = value; }
        }

        public bool m_BossMinion = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BossMinion
        {
            get { return m_BossMinion; }
            set { m_BossMinion = value; }
        }

        //Champ
        public bool m_Champ = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Champ
        {
            get { return m_Champ; }
            set { m_Champ = value; }
        }

        public bool m_ChampMinion = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ChampMinion
        {
            get { return m_ChampMinion; }
            set { m_ChampMinion = value; }
        }

        //LoH
        public bool m_LoHBoss = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LoHBoss
        {
            get { return m_LoHBoss; }
            set { m_LoHBoss = value; }
        }

        public bool m_LoHMinion = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LoHMinion
        {
            get { return m_LoHMinion; }
            set { m_LoHMinion = value; }
        }

        //Event
        public bool m_EventBoss = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool EventBoss
        {
            get { return m_EventBoss; }
            set { m_EventBoss = value; }
        }

        public bool m_EventMinion = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool EventMinion
        {
            get { return m_EventMinion; }
            set { m_EventMinion = value; }
        }

        public bool m_Murderer = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Murderer
        {
            get { return m_Murderer; }
            set { m_Murderer = value; }
        }

        public virtual string BossSpawnMessage { get { return ""; } }

        public bool m_ConvertedParagon = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ConvertedParagon
        {
            get { return m_ConvertedParagon; }
            set { m_ConvertedParagon = value; }
        }

        public bool m_TakenDamageFromPoison = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TakenDamageFromPoison
        {
            get { return m_TakenDamageFromCreature; }
            set { m_TakenDamageFromCreature = value; }
        }

        public bool m_TakenDamageFromCreature = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TakenDamageFromCreature
        {
            get { return m_TakenDamageFromCreature; }
            set { m_TakenDamageFromCreature = value; }
        }

        public bool m_Rare = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Rare
        {
            get { return m_Rare; }
            set
            {
                m_Rare = value;

                if (m_Rare)
                    SetRare();
            }
        }

        public static TimeSpan StamFreeMoveDuration = TimeSpan.FromMinutes(2);
        public static int StamFreeMoveRange = 100;
        public static int StamFreeAuraDistance = 50;
        public static TimeSpan StamFreeAuraInterval = TimeSpan.FromSeconds(3);
        
        public virtual bool CanSwitchWeapons { get { return false; } }
        public virtual bool IsRangedPrimary { get { return false; } }
        public virtual int WeaponSwitchRange { get { return 2; } }

        public DateTime m_NextWeaponChangeAllowed = DateTime.UtcNow;
        public TimeSpan NextWeaponChangeDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(4, 6));

        public virtual SlayerGroupType SlayerGroup { get { return SlayerGroupType.Monstrous; } }
        public virtual SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public virtual AIGroupType AIBaseGroup { get { return AIGroupType.None; } }
        public virtual AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public virtual double BaseUniqueDifficultyScalar { get { return 1.0; } }

        private SpeedGroupType m_SpeedGroup = SpeedGroupType.None;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public SpeedGroupType SpeedGroup
        {
            get
            {
                if (m_SpeedGroup != SpeedGroupType.None)
                    return m_SpeedGroup;

                else
                    return BaseSpeedGroup;
            }

            set 
            { 
                m_SpeedGroup = value;
                SetSpeed();
            }
        }

        private AIGroupType m_AIGroup = AIGroupType.Unspecified;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public AIGroupType AIGroup
        {
            get
            {
                if (m_AIGroup == AIGroupType.Unspecified)
                    return AIBaseGroup;

                return m_AIGroup; 
            }

            set
            { 
                m_AIGroup = value; 
                UpdateAI(true);
            }
        }

        public AISubGroupType m_AISubGroup = AISubGroupType.Melee;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public AISubGroupType AISubGroup
        {
            get 
            { 
                if (m_AISubGroup == AISubGroupType.Unspecified)
                    return AIBaseSubGroup; 

                else
                    return m_AISubGroup; 
            }

            set 
            { 
                m_AISubGroup = value; 
                UpdateAI(true); 
            }
        }

        private double m_UniqueCreatureDifficultyScalar = -1;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public double UniqueCreatureDifficultyScalar
        {
            get
            {
                if (m_UniqueCreatureDifficultyScalar == -1)
                    return BaseUniqueDifficultyScalar;

                else
                    return m_UniqueCreatureDifficultyScalar;
            }

            set { m_UniqueCreatureDifficultyScalar = value; }
        }

        public StamFreeMoveAuraTimer m_StamFreeMoveAuraTimer { get; set; }

        public void StartStamFreeMoveAuraTimer()
        {
            if ((IsBoss() || IsChamp() || IsLoHBoss() || IsEventBoss()) && !(Region is UOACZRegion))
            {
                m_StamFreeMoveAuraTimer = new StamFreeMoveAuraTimer(this);
                m_StamFreeMoveAuraTimer.Start();
            }
        }

        public class StamFreeMoveAuraTimer : Timer
        {
            BaseCreature bc_Creature;

            public StamFreeMoveAuraTimer(BaseCreature creature)
                : base(TimeSpan.Zero, BaseCreature.StamFreeAuraInterval)
            {
                Priority = TimerPriority.OneSecond;
                bc_Creature = creature;
            }

            protected override void OnTick()
            {
                base.OnTick();

                if (bc_Creature == null)
                    return;                

                IPooledEnumerable eable = bc_Creature.GetMobilesInRange(BaseCreature.StamFreeAuraDistance);

                foreach (Mobile mobile in eable)
                {
                    if (mobile == null)
                        return;
                    
                    mobile.StamFreeMoveExpiration = DateTime.UtcNow + BaseCreature.StamFreeMoveDuration;
                    mobile.StamFreeMoveSource = bc_Creature.Location;
                }

                eable.Free();
            }
        }

        public virtual bool DisplayWeight { get { return Backpack is StrongBackpack; } }

        public bool m_ReturnsHome = true; //Set to True, and is Overriden and Set to False for BladeSpirits / EV / and BaseEscortable

        public virtual bool CanFly { get { return false; } }

        #region Spill Acid

        public void SpillAcid(int Amount)
        {
            SpillAcid(null, Amount);
        }

        public void SpillAcid(Mobile target, int Amount)
        {
            if ((target != null && target.Map == null) || this.Map == null)
                return;

            for (int i = 0; i < Amount; ++i)
            {
                Point3D loc = this.Location;
                Map map = this.Map;
                Item acid = NewHarmfulItem();

                if (target != null && target.Map != null && Amount == 1)
                {
                    loc = target.Location;
                    map = target.Map;
                }
                else
                {
                    bool validLocation = false;
                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        loc = new Point3D(
                            loc.X + (Utility.Random(0, 3) - 2),
                            loc.Y + (Utility.Random(0, 3) - 2),
                            loc.Z);
                        loc.Z = map.GetAverageZ(loc.X, loc.Y);
                        validLocation = map.CanFit(loc, 16, false, false);
                    }
                }
                acid.MoveToWorld(loc, map);
            }
        }

        /* 
            Solen Style, override me for other mobiles/items: 
            kappa+acidslime, grizzles+whatever, etc. 
        */

        public virtual Item NewHarmfulItem()
        {
            return new PoolOfAcid(TimeSpan.FromSeconds(10), 30, 30);
        }

        #endregion

        public void DelayNextMovement(double seconds)
        {
            if (AIObject == null)
                return;

            if (AIObject.NextMove < DateTime.UtcNow)
            {
                AIObject.NextMove = DateTime.UtcNow;
            }

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(seconds);
        }        

        public virtual bool CanFlee { get { return !m_Paragon; } }

        private DateTime m_EndFlee;

        public DateTime EndFleeTime
        {
            get { return m_EndFlee; }
            set { m_EndFlee = value; }
        }

        public void BeginFlee(TimeSpan seconds)
        {
            if (this == null)
                return;

            if (seconds == null)
                seconds = TimeSpan.FromSeconds(10);

            if (AIObject != null)
            {
                Emote("!");
                PlaySound(GetIdleSound());

                EndFleeTime = DateTime.UtcNow + seconds;

                AIObject.FleeMode();
            }
        }

        public virtual bool IsInvulnerable { get { return false; } }

        public BaseAI AIObject { get { return m_AI; } }

        public const int MaxOwners = 5;
        
        protected List<Mobile> AcquireSpellTargets(int range, int numOfTargets)
        {
            List<Mobile> targets = new List<Mobile>();

            Map map = this.Map;

            if (map != null)
            {
                var mobiles = this.GetMobilesInRange(range);
                foreach (Mobile m in mobiles)
                {
                    if (this != m && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false) && this.InLOS(m))
                    {
                        if (targets.Count <= numOfTargets)
                            targets.Add(m);
                    }
                }

                mobiles.Free();
                return targets;
            }

            return null;
        }       

        public override string ApplyNameSuffix(string suffix)
        {
            if (IsParagon && !GivesMLMinorArtifact)
            {
                if (suffix.Length == 0)
                    suffix = "(Paragon)";
                else
                    suffix = String.Concat(suffix, " (Paragon)");
            }
            return base.ApplyNameSuffix(suffix);
        }

        public virtual bool CheckControlChance(Mobile m)
        {
            //UOACZ
            if (Region is UOACZRegion)
                return true;

            //If Both AnimalTaming and AnimalLore Exceed Minimum Taming Requirement, Control Success 
            double TargetTamingLore = MinTameSkill;

            if (BardProvoked)
                return false;

            if (BardPacified)
                return false;

            //Summoned Creatures 100% Chance
            if (this.Summoned)
                return true;

            //Sufficient AnimalTaming and AnimalLore
            if (!IsHenchman && m.Skills[SkillName.AnimalTaming].Value >= TargetTamingLore && m.Skills[SkillName.AnimalLore].Value >= TargetTamingLore)
                return true;

            //Henchman: Sufficient Begging and Anatomy
            if (IsHenchman && m.Skills[SkillName.Begging].Value >= TargetTamingLore && m.Skills[SkillName.Camping].Value >= TargetTamingLore)
                return true;

            return false;
        }

        public bool IsControlledCreature()
        {
            if (Controlled && ControlMaster is PlayerMobile)
                return true;

            if (this is BladeSpirits || this is EnergyVortex)
                return true;

            return false;
        }

        public override bool CheckDisrupt(int amount, Mobile from)
        {
            return true;
        }

        public virtual bool IsBarded()
        {
            if (this.BardProvoked || this.BardPacified)
                return true;

            return false;
        }

        private static Type[] m_AnimateDeadTypes = new Type[]
            {
                typeof( MoundOfMaggots ), typeof( HellSteed ), typeof( SkeletalMount ),
                typeof( WailingBanshee ), typeof( Wraith ), typeof( SkeletalDragon ),
                typeof( LichLord ), typeof( FleshGolem ), typeof( Lich ),
                typeof( SkeletalKnight ), typeof( BoneKnight ), typeof( Mummy ),
                typeof( SkeletalMage ), typeof( BoneMagi ), typeof( PatchworkSkeleton )
            };

        public virtual bool IsAnimatedDead
        {
            get
            {
                if (!Summoned)
                    return false;

                Type type = this.GetType();

                bool contains = false;

                for (int i = 0; !contains && i < m_AnimateDeadTypes.Length; ++i)
                    contains = (type == m_AnimateDeadTypes[i]);

                return contains;
            }
        }

        public virtual bool IsNecroFamiliar
        {
            get
            {
                if (!Summoned)
                    return false;

                return false;
            }
        }

        public override int AbsorbDamage(Mobile attacker, Mobile defender, int damage, bool physical, bool melee)
        {
            return BaseArmor.AbsorbDamage(attacker, defender, damage, physical, melee);
        }

        public override void Damage(int amount, Mobile from)
        {
            double damage = (double)amount;

            PlayerMobile pm_Master = null;

            BaseCreature bc_Source = from as BaseCreature;
            PlayerMobile pm_Source = from as PlayerMobile;

            if (from != null)
            {
                pm_Master = GetPlayerMaster() as PlayerMobile;

                if (from != null && from != this)
                {
                    LastCombatTime = DateTime.UtcNow;
                    from.LastCombatTime = DateTime.UtcNow;

                    if (bc_Source != null)
                    {
                        PlayerMobile pm_SourceMaster = bc_Source.GetPlayerMaster() as PlayerMobile;

                        if (pm_SourceMaster != null)
                        {
                            pm_SourceMaster.LastCombatTime = DateTime.UtcNow;

                            if (pm_Master != null && pm_SourceMaster != pm_Master)
                            {
                                pm_SourceMaster.PlayerVsPlayerCombatOccured(pm_Master);
                                pm_Master.PlayerVsPlayerCombatOccured(pm_SourceMaster);
                            }
                        }
                    }

                    if (pm_Source != null)
                    {
                        pm_Source.LastCombatTime = DateTime.UtcNow;

                        if (pm_Master != null && pm_Source != pm_Master)
                        {
                            pm_Master.PlayerVsPlayerCombatOccured(pm_Source);
                            pm_Source.PlayerVsPlayerCombatOccured(pm_Master);
                        }
                    }
                }                
                
                if (bc_Source != null)
                {
                    //Discordance
                    damage *= 1 - bc_Source.DiscordEffect;

                    //Herding
                    if (bc_Source.FocusedAggressionTarget == this && bc_Source.FocusedAggressionExpiration > DateTime.UtcNow)
                        damage *= 1 + (bc_Source.FocusedAggresionValue);
                }

                //This Creature is Discorded
                damage *= 1 + DiscordEffect;
            }

            //Ship-Based Combat
            if (BaseBoat.UseShipBasedDamageModifer(from, this))
                damage = (double)amount * BaseBoat.shipBasedDamageToCreatureScalar;

            int oldHits = this.Hits;

            if (damage < 1)
                damage = 1;

            int finalDamage = (int)damage;

            pm_Master = null;

            if (ControlMaster is PlayerMobile)
            {
                pm_Master = ControlMaster as PlayerMobile;

                DamageTracker.RecordDamage(pm_Master, from, this, DamageTracker.DamageType.FollowerDamageTaken, finalDamage);  
            }

            else if (SummonMaster is PlayerMobile)
            {
                pm_Master = SummonMaster as PlayerMobile;

                DamageTracker.RecordDamage(pm_Master, from, this, DamageTracker.DamageType.FollowerDamageTaken, finalDamage);  
            }

            if (bc_Source != null)
            {
                if (bc_Source.ControlMaster is PlayerMobile)
                {
                    pm_Master = bc_Source.ControlMaster as PlayerMobile;

                    DamageTracker.RecordDamage(pm_Master, from, this, DamageTracker.DamageType.FollowerDamage, finalDamage);
                }

                else if (bc_Source.SummonMaster is PlayerMobile)
                {
                    pm_Master = bc_Source.SummonMaster as PlayerMobile;

                    DamageTracker.RecordDamage(pm_Master, from, this, DamageTracker.DamageType.FollowerDamage, finalDamage);
                }

                else if (bc_Source.BardProvoked && bc_Source.BardMaster is PlayerMobile)
                {
                    pm_Master = bc_Source.BardMaster as PlayerMobile;

                    DamageTracker.RecordDamage(pm_Master, from, this, DamageTracker.DamageType.ProvocationDamage, finalDamage);  
                }
            }            

            base.Damage(finalDamage, from);

            if (SubdueBeforeTame && !Controlled)
            {
                if ((oldHits > (this.HitsMax / 10)) && (this.Hits <= (this.HitsMax / 10)))
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "* The creature has been beaten into subjugation! *");
            }
        }

        public virtual bool DeleteCorpseOnDeath
        {
            get
            {
                return m_Summoned;
            }
        }

        public override void SetLocation(Point3D newLocation, bool isTeleport)
        {
            base.SetLocation(newLocation, isTeleport);

            if (isTeleport && m_AI != null)
                m_AI.OnTeleported();
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            if (MHSCreatures.RareList.Contains(GetType()))
            {
                if (Utility.RandomDouble() <= MHSPersistance.RareChance && !IsParagon)
                    m_Rare = true;
            }

            if (Paragon.CheckConvert(this, location, m))
                IsParagon = true;

            if (Tameable && !m_GeneratedTamedStats)
                GenerateTamedScalars();

            base.OnBeforeSpawn(location, m);
        }

        public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
        {
            if (!Alive || IsDeadPet)
                return ApplyPoisonResult.Immune;

            int poisonLevel = poison.Level;
          
            if (PoisonResistance >= 5)
                return ApplyPoisonResult.Immune;

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
            if (PoisonResistance >= 5)
                return true;

            return false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Loyalty
        {
            get
            {
                return m_Loyalty;
            }
            set
            {
                m_Loyalty = Math.Min(Math.Max(value, 0), MaxLoyalty);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WayPoint CurrentWaypoint
        {
            get
            {
                return m_CurrentWaypoint;
            }
            set
            {
                m_CurrentWaypoint = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public List<WayPoint> VisitedWaypoints
        {
            get
            {
                return m_VisitedWaypoints;
            }
            set
            {
                m_VisitedWaypoints = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WayPointOrder WaypointOrder
        {
            get
            {
                return m_WaypointOrder;
            }
            set
            {
                m_WaypointOrder = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextWaypointAction
        {
            get
            {
                return m_NextWaypointAction;
            }
            set
            {
                m_NextWaypointAction = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public IPoint2D TargetLocation
        {
            get
            {
                return m_TargetLocation;
            }
            set
            {
                m_TargetLocation = value;
            }
        }

        public virtual Mobile ConstantFocus { get { return null; } }

        public virtual bool DisallowAllMoves
        {
            get
            {
                return false;
            }
        }

        public virtual bool InitialInnocent
        {
            get
            {
                return false;
            }
        }

        public virtual bool AlwaysMurderer
        {
            get
            {
                return false;
            }
        }

        public virtual bool AlwaysAttackable
        {
            get
            {
                return false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageMin
        {
            get
            {
                //Tamed Creature Damage Boosts
                if (Controlled && ControlMaster is PlayerMobile)
                {
                    double experienceScalar = GetExperienceScalar();

                    double damage = (double)m_DamageMin;

                    damage *= (1 + (BaseCreature.DamageExperienceScalar * experienceScalar));

                    if (damage < 1)
                        damage = 1;

                    return (int)Math.Round(damage);
                }

                return m_DamageMin;
            }

            set { m_DamageMin = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageMax
        {
            get
            {
                //Tamed Creature Damage Boosts
                if (Controlled && ControlMaster is PlayerMobile)
                {
                    double experienceScalar = GetExperienceScalar();

                    double damage = (double)m_DamageMax;

                    damage *= (1 + (BaseCreature.DamageExperienceScalar * experienceScalar));

                    if (damage < 1)
                        damage = 1;

                    return (int)Math.Round(damage);
                }

                return m_DamageMax;
            }

            set { m_DamageMax = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                if (m_HitsMax > 0)
                {
                    int value = m_HitsMax + GetStatOffset(StatType.Str);

                    if (value < 1)
                        value = 1;
                    else if (value > 65000)
                        value = 65000;

                    return value;
                }

                return Str;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ReturnsHome
        {
            get { return m_ReturnsHome; }
            set { m_ReturnsHome = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitsMaxSeed
        {
            get { return m_HitsMax; }
            set { m_HitsMax = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int StamMax
        {
            get
            {
                if (m_StamMax > 0)
                {
                    int value = m_StamMax + GetStatOffset(StatType.Dex);

                    if (value < 1)
                        value = 1;
                    else if (value > 65000)
                        value = 65000;

                    return value;
                }

                return Dex;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StamMaxSeed
        {
            get { return m_StamMax; }
            set { m_StamMax = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax
        {
            get
            {
                if (m_ManaMax > 0)
                {
                    int value = m_ManaMax + GetStatOffset(StatType.Int);

                    if (value < 1)
                        value = 1;
                    else if (value > 65000)
                        value = 65000;

                    return value;
                }

                return Int;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ManaMaxSeed
        {
            get { return m_ManaMax; }
            set { m_ManaMax = value; }
        }

        public virtual bool CanOpenDoors
        {
            get
            {
                return !this.Body.IsAnimal && !this.Body.IsSea;
            }
        }

        public virtual bool CanMoveOverObstacles
        {
            get
            {
                return true;
            }
        }

        public virtual bool CanDestroyObstacles
        {
            get
            {
                return CanMoveOverObstacles;
            }
        }

        public void Unpacify()
        {
            BardMaster = null;
            BardTarget = null;
            Combatant = null;
            BardPacified = false;

            BardEndTime = DateTime.UtcNow;
            LastSwingTime = DateTime.UtcNow;

            PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "*breaks from trance*");
        }

        public static double MediumDifficultyThreshold = 30; //Creatures over this difficulty will autodispel
        public static double HighDifficultyThreshold = 45; //Creatures over this difficulty will autodispel more frequently and have special effects reduced     

        public double BaseDispelDamagePercent = .60; //Non-Player Casted Dispel or AutoDispel Deals This Percentage of Creature's MaxHits as Damage

        public double SummonerEnhancedSpellbookDispelDamageReduction = .15; //Dispel Damage Reduction
        public double SpiritSpeakDispelDamageReduction = .15; //Dispel Damage Reduction for Spirit Speak Skill Scaling Up to GM   
        public double InscriptionDispelDamageReduction = .15; //Dispel Damage Reduction for Inscription Skill Scaling Up to GM   

        public Dictionary<BaseCreature, DateTime> DictAutoDispelInstances = new Dictionary<BaseCreature, DateTime>();

        public virtual bool AutoDispel { get { return (!Summoned && !(Region is UOACZRegion) && (InitialDifficulty >= MediumDifficultyThreshold || IsChamp() || IsBoss() || IsLoHBoss() || IsEventBoss())); } }

        public void SetDispelResistance(Mobile caster, bool enhancedSpellcast, double bonusResist)
        {
            DispelResist = 0;

            if (enhancedSpellcast)
                DispelResist += SummonerEnhancedSpellbookDispelDamageReduction;

            DispelResist += (caster.Skills[SkillName.SpiritSpeak].Value / 100) * SpiritSpeakDispelDamageReduction;
            DispelResist += (caster.Skills[SkillName.Inscribe].Value / 100) * InscriptionDispelDamageReduction;
            DispelResist += bonusResist;
        }

        public DateTime NextAutoDispelAllowed = DateTime.UtcNow;
        public virtual TimeSpan AutoDispelCooldown
        {
            get
            {
                if (IsChamp())
                    return TimeSpan.FromSeconds(6);

                if (IsBoss())
                    return TimeSpan.FromSeconds(5);

                if (IsLoHBoss())
                    return TimeSpan.FromSeconds(4);

                if (IsEventBoss())
                    return TimeSpan.FromSeconds(3);

                if (InitialDifficulty >= HighDifficultyThreshold)
                    return TimeSpan.FromSeconds(8);

                if (InitialDifficulty >= MediumDifficultyThreshold)
                    return TimeSpan.FromSeconds(10);

                return TimeSpan.FromSeconds(12);
            }
        }

        public DateTime NextBardingEffectAllowed = DateTime.UtcNow;
        public virtual TimeSpan BardingEffectCooldown
        {
            get
            {
                if (IsParagon)
                    return TimeSpan.FromSeconds(30);

                if (Rare)
                    return TimeSpan.FromSeconds(30);

                if (IsChamp())
                    return TimeSpan.FromSeconds(45);

                if (IsBoss())
                    return TimeSpan.FromSeconds(60);

                if (IsLoHBoss())
                    return TimeSpan.FromSeconds(60);

                if (IsEventBoss())
                    return TimeSpan.FromSeconds(90);

                return TimeSpan.FromSeconds(0);
            }
        }

        public virtual double IgnoreHurtSoundChance
        {
            get
            {
                double scalar = InitialDifficulty * .01;

                if (scalar > .8)
                    scalar = .8;

                if (IsParagon)
                    return 0;

                if (Rare)
                    return 0;

                if (IsChamp())
                    return scalar;

                if (IsBoss())
                    return scalar;

                if (IsLoHBoss())
                    return scalar;

                if (IsEventBoss())
                    return .9;

                return 0;
            }
        }

        public virtual double SpecialEffectReduction
        {
            get
            {
                double scalar = 1 - (InitialDifficulty * .01);

                if (scalar < .2)
                    scalar = .2;

                if (IsParagon)
                    return scalar;

                if (Rare)
                    return scalar;

                if (IsChamp())
                    return scalar;

                if (IsBoss())
                    return scalar;

                if (IsLoHBoss())
                    return scalar;

                if (IsEventBoss())
                    return scalar;

                return scalar;
            }
        }

        public virtual double BackstabDamageRecievedScalar
        {
            get
            {
                if (IsParagon)
                    return 1;

                if (Rare)
                    return 1;

                if (IsChamp())
                    return .5;

                if (IsBoss())
                    return .5;

                if (IsLoHBoss())
                    return .5;

                if (IsEventBoss())
                    return .5;

                return 1;
            }
        }

        public override TimeSpan DamageEntryExpiration
        {
            get
            {
                if (IsChamp())
                    return TimeSpan.FromMinutes(5);

                if (IsBoss())
                    return TimeSpan.FromMinutes(10);

                if (IsLoHBoss())
                    return TimeSpan.FromMinutes(10);

                if (IsEventBoss())
                    return TimeSpan.FromMinutes(10);

                return TimeSpan.FromMinutes(2);
            }
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
        
        public virtual void OnSwing(Mobile defender)
        {
            if (AcquireNewTargetEveryCombatAction)
                m_NextAcquireTargetAllowed = DateTime.UtcNow;
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

                        if (item is UOACZStatic || item is Custom.UOACZBreakableStatic)
                        {
                            foundBlockingItem = true;
                            break;
                        }
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
            Poison p = HitPoison;

            if (m_Paragon)
                p = PoisonImpl.IncreaseLevel(p);

            double chance = 0.0;

            if (defender.FindItemOnLayer(Layer.OuterTorso) is AcidProofRobe)
                chance += 0.75;

            if (defender.FindItemOnLayer(Layer.Helm) is AcidProofChapeau)
                chance += 0.25;

            if (chance > 0.0 && Utility.RandomDouble() < chance)
            {
                defender.SendMessage("The creature's poison is resisted by your magical acid proof clothing.");
                return;
            }

            double poisonResult = Utility.RandomDouble();
            double poisonChance = Skills[SkillName.Poisoning].Base / 100;

            if (Controlled && ControlMaster is PlayerMobile && defender is PlayerMobile)
                poisonChance *= .5;

            if (defender is BaseCreature)
            {
                BaseCreature bc_Target = defender as BaseCreature;

                if (bc_Target.IsControlledCreature())
                    poisonChance *= .5;
            }

            BaseCreature bc_Defender = defender as BaseCreature;

            if (poisonResult <= poisonChance)
            {
                if (IsControlledCreature())
                {
                    if (defender is PlayerMobile)
                        p = PoisonImpl.DecreaseLevel(p);

                    if (bc_Defender != null)
                    {
                        if (bc_Defender.IsControlledCreature())
                            p = PoisonImpl.DecreaseLevel(p);
                    }
                }

                defender.ApplyPoison(this, p);
            }

            UOACZBaseUndead undeadCreature = this as UOACZBaseUndead;

            if (undeadCreature != null)
                undeadCreature.m_LastActivity = DateTime.UtcNow;

            if (bc_Defender != null)
            {
                CheckAutoDispel(bc_Defender);
                bc_Defender.CheckAutoDispel(this);
            }
        }

        public virtual void OnGotMeleeAttack(Mobile attacker)
        {
            if (HasFocusOnMob || BardProvoked)
                return;

            BaseCreature bc_Attacker = attacker as BaseCreature;

            if (bc_Attacker != null)
            {
                CheckAutoDispel(bc_Attacker);
                bc_Attacker.CheckAutoDispel(this);
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (BardPacified)
            {
            }

            int disruptThreshold;

            //NPCs can use bandages too!
            if (!Core.AOS)
                disruptThreshold = 0;

            else if (from != null && from.Player)
                disruptThreshold = 18;

            else
                disruptThreshold = 25;

            if (amount > disruptThreshold)
            {
                BandageContext c = BandageContext.GetContext(this);

                if (c != null)
                    c.Slip();
            }            

            if (from != this && from != null)
                WeightOverloading.FatigueOnDamage(this, amount, 0.5);

            InhumanSpeech speechType = this.SpeechType;

            if (speechType != null && !willKill)
                speechType.OnDamage(this, amount);

            BaseCreature bc_From = from as BaseCreature;

            if (bc_From != null)
            {
                CheckAutoDispel(bc_From);
                bc_From.CheckAutoDispel(this);
            }

            if (!willKill)
            {
            }

            base.OnDamage(amount, from, willKill);

            if (!willKill)
            {
                if (Alive && !Deleted && Combatant == null && from != null)
                {
                    if (from.Alive && from.AccessLevel == AccessLevel.Player)
                    {
                        if (Controlled && ControlMaster is PlayerMobile && AIObject != null)
                        {
                            if (ControlOrder == OrderType.Attack || ControlOrder == OrderType.Guard || ControlOrder == OrderType.Patrol || ControlOrder == OrderType.Stay)
                                Combatant = from;
                        }

                        else
                            Combatant = from;
                    }
                }
            }
        }

        public bool InPassiveTamingSkillGainRange(PlayerMobile player)
        {
            if (player == null)
                return false;

            double animalTaming = player.Skills[SkillName.AnimalTaming].Value;

            if (MinTameSkill <= animalTaming && animalTaming < MinTameSkill + SkillCheck.SkillRangeIncrement)
                return true;

            return false;
        }

        public bool AllowPassiveTamingSkillGain(BaseCreature defender, PlayerMobile player)
        {
            if (defender == null || player == null)
                return false;

            if (!(Controlled && ControlMaster == player))
                return false;

            if (IsHenchman || Summoned)
                return false;

            if (defender.NoKillAwards || defender.Summoned || defender.ControlMaster is PlayerMobile)
                return false;

            if (player.m_PassiveSkillGainRemaining == 0)
                return false;

            double passiveTamingGainScalar = GetPassiveTamingGainScalar(player.Skills[SkillName.AnimalTaming].Value);

            if (passiveTamingGainScalar <= 0)
                passiveTamingGainScalar = 0.1;

            TimeSpan PassiveTamingSkillGainDelay = TimeSpan.FromMinutes(60 / passiveTamingGainScalar);

            if (DateTime.UtcNow < player.m_LastPassiveTamingSkillGain + PassiveTamingSkillGainDelay)
                return false;

            if (InPassiveTamingSkillGainRange(player))
                return true;

            return false;
        }

        public bool AllowExperienceGain(BaseCreature defender, PlayerMobile player)
        {
            if (defender == null || player == null)
                return false;

            if (!(Controlled && ControlMaster == player))
                return false;

            if (IsHenchman || Summoned)
                return false;

            if (defender.NoKillAwards || defender.Summoned || defender.ControlMaster is PlayerMobile)
                return false;

            return true;
        }

        public virtual double ExperienceChance()
        {
            double minTamingExpGainScalar = GetCreatureMinTamingExpGainScalar(MinTameSkill); 
            double experienceChance = (InitialDifficulty * .01) * minTamingExpGainScalar;

            if (experienceChance < .01)
                experienceChance = .01;

            if (Rare)
                experienceChance = 1;

            if (IsChamp())
                experienceChance = 1;

            if (IsLoHBoss())
                experienceChance = 1;

            if (IsBoss())
                experienceChance = 1;

            if (IsEventBoss())
                experienceChance = 1;

            return experienceChance;
        }

        public static double GetCreatureMinTamingExpGainScalar(double minTaming)
        {
            double minTamingExpGainScalar = 1.0;      

            for (int a = 0; a < CreatureMinTamingExpGainScalar.Length; a++)
            {
                double rangeBottom = a * SkillCheck.SkillRangeIncrement;
                double rangeTop = (a * SkillCheck.SkillRangeIncrement) + SkillCheck.SkillRangeIncrement;

                if (minTaming >= rangeBottom && minTaming < rangeTop)
                {
                    minTamingExpGainScalar = CreatureMinTamingExpGainScalar[a];
                    break;
                }
            }

            return minTamingExpGainScalar;
        }

        public static double[] CreatureMinTamingExpGainScalar = new double[] {          
                                                        40.0, 38.0,    //0-5, 5-10
                                                        36.0, 34.0,    //10-15, 15-20
                                                        32.0, 30.0,    //20-25, 25-30
                                                        28.0, 26.0,    //30-35, 30-40
                                                        24.0, 22.0,    //40-45, 45-50
                                                        20.0, 18.0,    //50-55, 55-60
                                                        16.0, 14.0,    //60-65, 65-70
                                                        12.0, 10.0,    //70-75, 75-80
                                                        8.0, 6.0,    //80-85, 85-90
                                                        4.0, 2.0,    //90-95, 95-100
                                                        1.75, 1.5,    //100-105, 105-110
                                                        1.25, 1.0};   //110-115, 115-120

        public virtual int ExperienceValue()
        {
            int experienceValue = 1;

            if (Rare)
                experienceValue = 10;

            if (IsChamp())
                experienceValue = 10;

            if (IsLoHBoss())
                experienceValue = 15;

            if (IsBoss())
                experienceValue = 25;            

            if (IsEventBoss())
                experienceValue = 40;

            return experienceValue;
        }

        public double PassiveTamingSkillGainChance(PlayerMobile player)
        {
            double tamingSkill = player.Skills[SkillName.AnimalTaming].Value;

            double passiveTamingGainScalar = GetPassiveTamingGainScalar(tamingSkill);
            double passiveTamingGainChance = (InitialDifficulty * .01) * passiveTamingGainScalar;            

            if (Rare)
                passiveTamingGainChance = .1 * passiveTamingGainScalar;

            if (IsChamp())
                passiveTamingGainChance = .1 * passiveTamingGainScalar;

            if (IsLoHBoss())
                passiveTamingGainChance = .15 * passiveTamingGainScalar;

            if (IsBoss())
                passiveTamingGainChance = .25 * passiveTamingGainScalar;

            if (IsEventBoss())
                passiveTamingGainChance = .40 * passiveTamingGainScalar;

            return passiveTamingGainChance;
        }

        public static double GetPassiveTamingGainScalar(double tamingSkill)
        {
            double passiveTamingGainScalar = 1.0;

            for (int a = 0; a < PassiveTamingGainScalar.Length; a++)
            {
                double rangeBottom = a * SkillCheck.SkillRangeIncrement;
                double rangeTop = (a * SkillCheck.SkillRangeIncrement) + SkillCheck.SkillRangeIncrement;

                if (tamingSkill >= rangeBottom && tamingSkill < rangeTop)
                {
                    passiveTamingGainScalar = PassiveTamingGainScalar[a];
                    break;
                }
            }

            return passiveTamingGainScalar;
        }

        public static double[] PassiveTamingGainScalar = new double[] {          
                                                        10.0, 9.5,    //0-5, 5-10
                                                        9.0, 8.5,    //10-15, 15-20
                                                        8.0, 7.5,    //20-25, 25-30
                                                        7.0, 6.5,    //30-35, 30-40
                                                        6.0, 5.5,    //40-45, 45-50
                                                        5.0, 4.0,    //50-55, 55-60
                                                        3.0, 2.0,    //60-65, 65-70
                                                        1.5, 1.25,    //70-75, 75-80
                                                        1.15, 1.10,     //80-85, 85-90
                                                        1.05, 1.0,     //90-95, 95-100
                                                        1.0, 1.0,    //100-105, 105-110
                                                        1.0, 1.0};   //110-115, 115-120

        public virtual void CheckAutoDispel(BaseCreature bc_Target)
        {
            if (!AutoDispel) return;

            if (bc_Target == null) return;
            if (bc_Target.Deleted || !bc_Target.Alive) return;
            if (!bc_Target.IsDispellable) return;
            if (GetDistanceToSqrt(bc_Target.Location) >= 18) return;

            DateTime NextAutoDispelCheckAllowed = DateTime.UtcNow;

            if (!DictAutoDispelInstances.ContainsKey(bc_Target))
            {
                DictAutoDispelInstances.Add(bc_Target, DateTime.UtcNow + AutoDispelCooldown);
                return;
            }

            else
            {
                NextAutoDispelCheckAllowed = DictAutoDispelInstances[bc_Target];

                if (NextAutoDispelCheckAllowed > DateTime.UtcNow)
                    return;

                DictAutoDispelInstances[bc_Target] = DateTime.UtcNow + AutoDispelCooldown;
            }

            bc_Target.ResolveDispel(this, false, SpellHue);
        }

        public virtual void ResolveDispel(Mobile from, bool alwaysSuccess, int spellHue)
        {
            if (from == null)
                return;

            if (alwaysSuccess)
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, spellHue, 0, 5042, 0);
                Effects.PlaySound(Location, Map, 0x201);

                Delete();
            }

            else
            {
                double dispelDamagePercent = BaseDispelDamagePercent - DispelResist;

                if (dispelDamagePercent < 0)
                    dispelDamagePercent = 0;

                PlaySound(GetAngerSound());

                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, spellHue, 0, 5042, 0);
                Effects.PlaySound(Location, Map, 0x201);

                int damage = (int)(Math.Ceiling((double)HitsMax * dispelDamagePercent));

                new Blood().MoveToWorld(Location, Map);
                AOS.Damage(this, from, damage, 0, 100, 0, 0, 0);
            }
        }

        public virtual void OnDamagedBySpell(Mobile from)
        {
        }

        public virtual void OnHarmfulSpell(Mobile from)
        {
        }

        public virtual void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
        }

        public virtual void AlterDamageScalarTo(Mobile target, ref double scalar)
        {
        }

        public virtual void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
        }

        public virtual void AlterSpellDamageTo(Mobile to, ref int damage)
        {
        }

        public virtual void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
        }

        public virtual void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
        }

        public virtual void CheckReflect(Mobile caster, ref bool reflect)
        {
        }

        public virtual void OnGotCannonHit(int amount, Mobile from, bool willKill)
        {
        }

        public virtual bool HasFeathers { get { return false; } }
        public virtual int FeatherAmount { get { return -1; } }

        public virtual bool HasMeat { get { return true; } }
        public virtual int MeatAmount { get { return -1; } }
        public virtual MeatType MeatType { get { return MeatType.Ribs; } }

        public virtual bool HasCraftResource { get { return true; } }
        public virtual int ResourceAmount { get { return -1; } }
        public virtual CraftResource ResourceType { get { return CraftResource.RegularLeather; } }

        public virtual bool HasWool { get { return false; } }
        public virtual int WoolAmount { get { return -1; } }

        public virtual void OnCarve(Mobile from, Corpse corpse)
        {
            if (from == null || corpse == null) return;
            if (corpse.Deleted) return;
            if (NoKillAwards) return;

            if (corpse.Carved)
            {
                from.SendMessage("This corpse has already been carved.");
                return;
            }

            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ForensicsCooldown * 1000);
            from.CheckTargetSkill(SkillName.Forensics, corpse, 0, 120.0, 1.0);

            double carveResourceScalar = 1 + (ForensicEvalCarveResourceScalarBonus * (from.Skills[SkillName.Forensics].Value));

            from.Animate(32, 3, 1, true, false, 0);
            Effects.PlaySound(from.Location, from.Map, 0x3E3);

            new Blood(0x122D).MoveToWorld(corpse.Location, corpse.Map);
            
            int extraBlood = Utility.RandomMinMax(2, 3);

            for (int a = 0; a < extraBlood; a++)
            {
                Point3D newLocation = SpecialAbilities.GetRandomAdjustedLocation(corpse.Location, corpse.Map, true, 1, false);
                new Blood().MoveToWorld(newLocation, corpse.Map);
            }

            corpse.Carved = true;

            bool hasItems = false;

            //Feathers
            if (HasFeathers)
            {
                hasItems = true;

                int featherAmount = 0;

                if (FeatherAmount != -1)
                    
                    featherAmount = FeatherAmount;

                else                
                    featherAmount = (int)(25 + ((Double)InitialDifficulty * 50));

                featherAmount = (int)(Math.Round((double)featherAmount * carveResourceScalar));

                DropCarvedItem(corpse, typeof(Feather), featherAmount);
            }

            //Meat
            if (HasMeat)
            {
                hasItems = true;

                double basicMeatScalar = .4 + (Utility.RandomDouble() * .2);
                double specialMeatScalar = .16 + (Utility.RandomDouble() * .08);

                int meatAmount = 0;

                if (MeatAmount != -1)
                {
                    meatAmount = (int)(Math.Round((double)MeatAmount * carveResourceScalar));

                    switch (MeatType)
                    {
                        case MeatType.Ribs: DropCarvedItem(corpse, typeof(RawRibs), meatAmount); break;
                        case MeatType.Drumstick: DropCarvedItem(corpse, typeof(RawDrumstick), meatAmount); break;
                        case MeatType.FishSteak: DropCarvedItem(corpse, typeof(RawFishSteak), meatAmount); break;

                        case MeatType.Meat: DropCarvedItem(corpse, typeof(RawRibs), meatAmount); break;
                        case MeatType.Poultry: DropCarvedItem(corpse, typeof(RawDrumstick), meatAmount); break;
                        case MeatType.Fish: DropCarvedItem(corpse, typeof(RawFishSteak), meatAmount); break;

                        case MeatType.Bacon: DropCarvedItem(corpse, typeof(RawBacon), meatAmount); break;
                        case MeatType.Ham: DropCarvedItem(corpse, typeof(RawHam), meatAmount); break;
                        case MeatType.Steaks: DropCarvedItem(corpse, typeof(RawSteaks), meatAmount); break;
                        case MeatType.MeatShank: DropCarvedItem(corpse, typeof(RawMeatShank), meatAmount); break;
                        case MeatType.Sausage: DropCarvedItem(corpse, typeof(RawSausage), meatAmount); break;
                        case MeatType.Bird: DropCarvedItem(corpse, typeof(RawBird), meatAmount); break;
                        case MeatType.Fillet: DropCarvedItem(corpse, typeof(RawFishFillet), meatAmount); break;
                    }                    
                }                   

                else
                {
                    switch (MeatType)
                    {
                        //Basic
                        case MeatType.Ribs: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * basicMeatScalar * carveResourceScalar)); DropCarvedItem(corpse, typeof(RawRibs), meatAmount); break;
                        case MeatType.Drumstick: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * basicMeatScalar * carveResourceScalar)); DropCarvedItem(corpse, typeof(RawDrumstick), meatAmount); break;
                        case MeatType.FishSteak: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * basicMeatScalar * carveResourceScalar)); DropCarvedItem(corpse, typeof(RawFishSteak), meatAmount); break;

                        //Scaled
                        case MeatType.Meat:
                            meatAmount = 1 + (int)(Math.Round(InitialDifficulty * basicMeatScalar * carveResourceScalar));

                            DropCarvedItem(corpse, typeof(RawRibs), meatAmount);

                            if (Utility.RandomDouble() <= specialMeatScalar)
                            {
                                switch (Utility.RandomMinMax(1, 5))
                                {
                                    case 1: DropCarvedItem(corpse, typeof(RawBacon), 1); break;
                                    case 2: DropCarvedItem(corpse, typeof(RawHam), 1); break;
                                    case 3: DropCarvedItem(corpse, typeof(RawSteaks), 1); break;
                                    case 4: DropCarvedItem(corpse, typeof(RawMeatShank), 1); break;
                                    case 5: DropCarvedItem(corpse, typeof(RawSausage), 1); break;
                                }
                            }
                        break;

                        case MeatType.Poultry:
                            meatAmount = 1 + (int)(Math.Round(InitialDifficulty * basicMeatScalar * carveResourceScalar));
                            DropCarvedItem(corpse, typeof(RawDrumstick), meatAmount);

                            if (Utility.RandomDouble() <= specialMeatScalar)                            
                                DropCarvedItem(corpse, typeof(RawBird), 1);                            
                        break;

                        case MeatType.Fish:
                            meatAmount = 1 + (int)(Math.Round(InitialDifficulty * basicMeatScalar * carveResourceScalar));
                            DropCarvedItem(corpse, typeof(RawFishSteak), meatAmount);

                            if (Utility.RandomDouble() <= specialMeatScalar)
                                DropCarvedItem(corpse, typeof(RawFishFillet), 1); 
                        break;

                        //Special
                        case MeatType.Bacon: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawBacon), meatAmount); break;
                        case MeatType.Ham: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawHam), meatAmount); break;
                        case MeatType.Steaks: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawSteaks), meatAmount); break;
                        case MeatType.MeatShank: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawMeatShank), meatAmount); break;
                        case MeatType.Sausage: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawSausage), meatAmount); break;
                        case MeatType.Bird: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawBird), meatAmount); break;
                        case MeatType.Fillet: meatAmount = 1 + (int)(Math.Round(InitialDifficulty * specialMeatScalar)); DropCarvedItem(corpse, typeof(RawFishFillet), meatAmount); break;
                    }
                }
            }

            //Craft Resource
            if (HasCraftResource)
            {
                hasItems = true;

                int resourceAmount = 0;
                double resourceDifficultyScalar = 1.0;

                if (ResourceAmount != -1)
                {
                    resourceAmount = (int)(Math.Round((double)ResourceAmount * carveResourceScalar));

                    DropCarvedItem(corpse, CraftResources.GetCraftResourceType(ResourceType), resourceAmount);
                }

                else
                {
                    switch (ResourceType)
                    {
                        case CraftResource.RegularLeather:
                            resourceDifficultyScalar = .4 + (Utility.RandomDouble() * .2);
                            resourceAmount = 1 + (int)(Math.Round(InitialDifficulty * resourceDifficultyScalar * carveResourceScalar));
                            DropCarvedItem(corpse, CraftResources.GetCraftResourceType(ResourceType), resourceAmount);
                        break;
                    }
                }
            }

            //Wool
            if (HasWool)
            {
                hasItems = true;

                int woolAmount = 0;

                if (WoolAmount != -1)     
                    DropCarvedItem(corpse, typeof(Wool), WoolAmount);                

                else
                {
                    woolAmount = 1 + (int)(Math.Round(InitialDifficulty * 1));
                    DropCarvedItem(corpse, typeof(Wool), woolAmount);
                }                
            }

            if (hasItems)
                from.SendMessage("You carve materials from the corpse.");

            else
                from.SendMessage("You carve the corpse but found no usable materials.");
        }

        public static void DropCarvedItem(Corpse corpse, Type type, int amount)
        {
            if (corpse == null)
                return;

            Item item = (Item)Activator.CreateInstance(type);

            if (item == null)
                return;

            if (item.Stackable)
            {
                item.Amount = amount;
                corpse.DropItem(item);
            }

            else
            {
                item.Delete();

                for (int a = 0; a < amount; a++)
                {
                    item = (Item)Activator.CreateInstance(type);
                    corpse.DropItem(item);
                }
            }
        }

        private static double[] m_StandardActiveSpeeds = new double[]
            {
                0.175, 0.1, 0.15, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.8
            };

        private static double[] m_StandardPassiveSpeeds = new double[]
            {
                0.350, 0.2, 0.4, 0.5, 0.6, 0.8, 1.0, 1.2, 1.6, 2.0
            };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version            

            //Version 0 
            writer.Write(m_iRangePerception);
            writer.Write(m_pHome.X);
            writer.Write(m_pHome.Y);
            writer.Write(m_pHome.Z);
            writer.Write(m_iRangeHome);
            writer.Write(m_bControlled);
            writer.Write(m_ControlMaster);
            writer.Write(m_ControlTarget);
            writer.Write(m_ControlDest);
            writer.Write((int)m_ControlOrder);
            writer.Write(m_MinTameSkill);
            writer.Write(m_Tameable);
            writer.Write(m_Summoned);
            writer.Write(m_iControlSlots);
            writer.Write((Item)m_CurrentWaypoint);
            writer.Write(m_SummonMaster);
            writer.Write(m_HitsMax);
            writer.Write(m_StamMax);
            writer.Write(m_ManaMax);
            writer.Write(m_DamageMin);
            writer.Write(m_DamageMax);
            writer.Write(m_Owners, true);
            writer.Write(m_IsDeadPet);
            writer.Write(m_IsBonded);
            writer.Write(m_Paragon);
            writer.Write(m_RemoveIfUntamed);
            writer.Write(m_RemoveStep);
            writer.Write(m_BardImmune);
            writer.Write(m_CorpseNameOverride);
            writer.Write((int)m_SpeedGroup);
            writer.Write((int)m_AIGroup);
            writer.Write((int)m_AISubGroup);
            writer.Write(m_ControlObject);
            writer.Write((int)m_WaypointOrder);
            writer.Write(m_AttackSpeed);
            writer.Write(m_ResolveAcquireTargetDelay);
            writer.Write(m_Experience);
            writer.Write(m_CreaturesKilled);
            writer.Write(m_TimesTamed);
            writer.Write(m_TamedBaseMaxHitsCreationScalar);
            writer.Write(m_TamedBaseDexCreationScalar);
            writer.Write(m_TamedBaseMaxManaCreationScalar);
            writer.Write(m_TamedBaseMinDamageCreationScalar);
            writer.Write(m_TamedBaseMaxDamageCreationScalar);
            writer.Write(m_TamedBaseWrestlingCreationScalar);
            writer.Write(m_TamedBaseEvalIntCreationScalar);
            writer.Write(m_TamedBaseMageryCreationScalar);
            writer.Write(m_TamedBaseMeditationCreationScalar);
            writer.Write(m_TamedBaseMagicResistCreationScalar);
            writer.Write(m_TamedBasePoisoningCreationScalar);
            writer.Write(m_TamedBaseVirtualArmorCreationScalar);
            writer.Write(m_GeneratedTamedStats);
            writer.Write(m_WasFishedUp);
            writer.Write(m_Boss);
            writer.Write(m_BossMinion);
            writer.Write(m_Champ);
            writer.Write(m_ChampMinion);
            writer.Write(m_LoHBoss);
            writer.Write(m_LoHMinion);
            writer.Write(m_EventBoss);
            writer.Write(m_EventMinion);
            writer.Write(m_FreelyLootable);
            writer.Write(m_ResurrectionsRemaining);
            writer.Write(m_NoKillAwards);
            writer.Write(m_SpellSpeedScalar);
            writer.Write(m_SpellHue);
            writer.Write(m_AcquireNewTargetEveryCombatAction);
            writer.Write(m_AcquireRandomizedTarget);
            writer.Write(m_AcquireRandomizedTargetSearchRange);
            writer.Write(m_AncientMysteryCreature);
            writer.Write(BaseSummonedDamageMin);
            writer.Write(BaseSummonedDamageMax);
            writer.Write(BaseSummonedHitsMax);
            writer.Write(m_ConvertedParagon);
            writer.Write(m_TakenDamageFromPoison);
            writer.Write(m_TakenDamageFromCreature);
            writer.Write(m_Rare);
            writer.Write(m_OwnerAbandonTime);
            writer.Write(m_NextWaypointAction);
            writer.Write(m_NextExperienceGain);
            writer.Write(m_LastActivated);
            writer.Write(m_BoatOccupied);
            writer.Write(m_XMLSpawner);
            writer.Write(m_ExperienceLevel);
            writer.Write(m_TimeStabled);
            writer.Write(m_InitialName);

            writer.Write(m_FollowerTraitSelections.Count);
            for (int a = 0; a < m_FollowerTraitSelections.Count; a++)
            {
                writer.Write((int)m_FollowerTraitSelections[a]);
            }            

            if (m_Summoned)
                writer.WriteDeltaTime(m_SummonEnd);

            if (IsStabled || (Controlled && ControlMaster != null))
                writer.Write(TimeSpan.Zero);
            else
                writer.Write(DeleteTimeLeft);

            if (m_VisitedWaypoints != null)
            {
                writer.Write(m_VisitedWaypoints.Count);

                for (int a = 0; a < m_VisitedWaypoints.Count; a++)
                {
                    writer.Write(m_VisitedWaypoints[a]);
                }
            }

            else
                writer.Write(0);

            writer.Write(m_SpecialAbilityEffectEntries.Count);
            for (int a = 0; a < m_SpecialAbilityEffectEntries.Count; a++)
            {
                writer.Write((int)m_SpecialAbilityEffectEntries[a].m_SpecialAbilityEffect);
                writer.Write(m_SpecialAbilityEffectEntries[a].m_Owner);
                writer.Write(m_SpecialAbilityEffectEntries[a].m_Value);
                writer.Write(m_SpecialAbilityEffectEntries[a].m_Expiration);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            int m_StoredHits = Hits;
            int m_StoredStam = Stam;
            int m_StoredMana = Mana;

            //Version 0
            if (version >= 0)
            {
                m_iRangePerception = reader.ReadInt();
                m_pHome.X = reader.ReadInt();
                m_pHome.Y = reader.ReadInt();
                m_pHome.Z = reader.ReadInt();
                m_iRangeHome = reader.ReadInt();
                m_bControlled = reader.ReadBool();
                m_ControlMaster = reader.ReadMobile();
                m_ControlTarget = reader.ReadMobile();
                m_ControlDest = reader.ReadPoint3D();
                m_ControlOrder = (OrderType)reader.ReadInt();
                m_MinTameSkill = reader.ReadDouble();
                m_Tameable = reader.ReadBool();
                m_Summoned = reader.ReadBool();
                m_iControlSlots = reader.ReadInt();
                m_CurrentWaypoint = reader.ReadItem() as WayPoint;
                m_SummonMaster = reader.ReadMobile();
                m_HitsMax = reader.ReadInt();
                m_StamMax = reader.ReadInt();
                m_ManaMax = reader.ReadInt();
                m_DamageMin = reader.ReadInt();
                m_DamageMax = reader.ReadInt();
                m_Owners = reader.ReadStrongMobileList();
                m_IsDeadPet = reader.ReadBool();
                m_IsBonded = reader.ReadBool();
                m_Paragon = reader.ReadBool();
                m_RemoveIfUntamed = reader.ReadBool();
                m_RemoveStep = reader.ReadInt();
                m_BardImmune = reader.ReadBool();
                m_CorpseNameOverride = reader.ReadString();
                m_SpeedGroup = (SpeedGroupType)reader.ReadInt();
                m_AIGroup = (AIGroupType)reader.ReadInt();
                m_AISubGroup = (AISubGroupType)reader.ReadInt();
                m_ControlObject = (Item)reader.ReadItem();
                m_WaypointOrder = (WayPointOrder)reader.ReadInt();
                m_AttackSpeed = reader.ReadInt();
                m_ResolveAcquireTargetDelay = reader.ReadDouble();
                m_Experience = reader.ReadInt();
                m_CreaturesKilled = reader.ReadInt();
                m_TimesTamed = reader.ReadInt();
                m_TamedBaseMaxHitsCreationScalar = reader.ReadDouble();
                m_TamedBaseDexCreationScalar = reader.ReadDouble();
                m_TamedBaseMaxManaCreationScalar = reader.ReadDouble();
                m_TamedBaseMinDamageCreationScalar = reader.ReadDouble();
                m_TamedBaseMaxDamageCreationScalar = reader.ReadDouble();
                m_TamedBaseWrestlingCreationScalar = reader.ReadDouble();
                m_TamedBaseEvalIntCreationScalar = reader.ReadDouble();
                m_TamedBaseMageryCreationScalar = reader.ReadDouble();
                m_TamedBaseMeditationCreationScalar = reader.ReadDouble();
                m_TamedBaseMagicResistCreationScalar = reader.ReadDouble();
                m_TamedBasePoisoningCreationScalar = reader.ReadDouble();
                m_TamedBaseVirtualArmorCreationScalar = reader.ReadDouble();
                m_GeneratedTamedStats = reader.ReadBool();
                m_WasFishedUp = reader.ReadBool();
                m_Boss = reader.ReadBool();
                m_BossMinion = reader.ReadBool();
                m_Champ = reader.ReadBool();
                m_ChampMinion = reader.ReadBool();
                m_LoHBoss = reader.ReadBool();
                m_LoHMinion = reader.ReadBool();
                m_EventBoss = reader.ReadBool();
                m_EventMinion = reader.ReadBool();
                m_FreelyLootable = reader.ReadBool();
                m_ResurrectionsRemaining = reader.ReadInt();
                m_NoKillAwards = reader.ReadBool();
                m_SpellSpeedScalar = reader.ReadDouble();
                m_SpellHue = reader.ReadInt();
                m_AcquireNewTargetEveryCombatAction = reader.ReadBool();
                m_AcquireRandomizedTarget = reader.ReadBool();
                m_AcquireRandomizedTargetSearchRange = reader.ReadInt();
                m_AncientMysteryCreature = reader.ReadBool();
                BaseSummonedDamageMin = reader.ReadInt();
                BaseSummonedDamageMax = reader.ReadInt();
                BaseSummonedHitsMax = reader.ReadInt();
                m_ConvertedParagon = reader.ReadBool();
                m_TakenDamageFromPoison = reader.ReadBool();
                m_TakenDamageFromCreature = reader.ReadBool();
                m_Rare = reader.ReadBool();
                m_OwnerAbandonTime = reader.ReadDateTime();
                m_NextWaypointAction = reader.ReadDateTime();
                m_NextExperienceGain = reader.ReadDateTime();
                m_LastActivated = reader.ReadDateTime();
                m_BoatOccupied = reader.ReadItem() as BaseBoat;
                m_XMLSpawner = reader.ReadItem() as XmlSpawner;
                m_ExperienceLevel = reader.ReadInt();
                m_TimeStabled = reader.ReadDateTime();
                m_InitialName = reader.ReadString();

                int followerTraitSelectionCount = reader.ReadInt();
                for (int a = 0; a < followerTraitSelectionCount; a++)
                {
                    m_FollowerTraitSelections.Add((FollowerTraitType)reader.ReadInt());
                }

                if (m_Summoned)
                {
                    m_SummonEnd = reader.ReadDeltaTime();
                    new UnsummonTimer(m_ControlMaster, this, m_SummonEnd - DateTime.UtcNow).Start();
                }

                TimeSpan deleteTime = reader.ReadTimeSpan();

                if (deleteTime > TimeSpan.Zero || LastOwner != null && !Controlled && !IsStabled)
                {
                    if (deleteTime == TimeSpan.Zero)
                        deleteTime = TimeSpan.FromDays(3.0);

                    m_DeleteTimer = new DeleteTimer(this, deleteTime);
                    m_DeleteTimer.Start();
                }

                int visitedWaypoints = reader.ReadInt();
                for (int a = 0; a < visitedWaypoints; a++)
                {
                    WayPoint visitedWaypoint = reader.ReadItem() as WayPoint;

                    if (m_VisitedWaypoints.IndexOf(visitedWaypoint) < 0)
                        m_VisitedWaypoints.Add(visitedWaypoint);
                }

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
            }

            //------            

            if (IsParagon)
                Hue = Paragon.Hue;

            CheckStatTimers();

            ChangeAIType(m_CurrentAI);

            AddFollowers();

            SetSpeed();
            PopulateDefaultAI();
            UpdateAI(true);

            m_SpecialAbilityEffectTimer = new SpecialAbilityEffectTimer(this);

            if (m_SpecialAbilityEffectEntries.Count > 0)
                m_SpecialAbilityEffectTimer.Start();

            if (RareTamable)
                CheckRareTamedScalars();

            if (Controlled && ControlMaster is PlayerMobile)
            {
                if (!(Region is GuardedRegion))
                {
                    //Crash Protection
                    Hidden = true;
                    Poison = null;
                }
            }

            //Banish Effect Safety Check
            if (Squelched)
            {
                Squelched = false;
                Hidden = false;
                Blessed = false;
            }

            Hits = m_StoredHits;
            Stam = m_StoredStam;
            Mana = m_StoredMana;
        }

        public virtual bool IsHumanInTown()
        {
            return (Body.IsHuman && Region.IsPartOf(typeof(Regions.GuardedRegion)));
        }

        public virtual bool CheckGold(Mobile from, Item dropped)
        {
            if (dropped is Gold)
                return OnGoldGiven(from, (Gold)dropped);

            if (dropped is TrainingCreditDeed)
            {
                TrainingCreditDeed trainingCreditDeed = dropped as TrainingCreditDeed;

                if (CheckTeachingMatch(from))
                {
                    SkillName the_skill = m_Teaching;

                    int pointsToLearn = 0;
                    int maxPointsToLearn = trainingCreditDeed.Value * 3;

                    TeachResult res = CheckTeachSkills(the_skill, from, maxPointsToLearn, ref pointsToLearn, false);

                    int cost = (int)((double)pointsToLearn / 3);
                    int amountGiven = cost;

                    if (amountGiven > trainingCreditDeed.Value)
                        amountGiven = trainingCreditDeed.Value;

                    if (Teach(m_Teaching, from, amountGiven, true))
                    {
                        trainingCreditDeed.Value -= amountGiven;

                        if (trainingCreditDeed.Value <= 0)
                            trainingCreditDeed.Delete();                   
                    }
                }

                else
                    SayTo(from, "Alas, I cannot accept that for anything other than training skills.");
            }

            return false;
        }

        public virtual bool OnGoldGiven(Mobile from, Gold dropped)
        {
            if (CheckTeachingMatch(from))
            {
                SkillName the_skill = m_Teaching;
                if (Teach(m_Teaching, from, dropped.Amount, true))
                {
                    dropped.Delete();

                    return true;
                }
            }

            else if (IsHumanInTown())
            {
                Direction = GetDirectionTo(from);

                int oldSpeechHue = this.SpeechHue;

                this.SpeechHue = 0x23F;
                SayTo(from, "Thou art giving me gold?");

                if (dropped.Amount > 500)
                    SayTo(from, "'Tis a noble gift.");
                else
                    SayTo(from, "Money is always welcome.");

                this.SpeechHue = 0x3B2;
                SayTo(from, 501548); // I thank thee.

                this.SpeechHue = oldSpeechHue;

                dropped.Delete();

                return true;
            }

            return false;
        }

        public override bool ShouldCheckStatTimers { get { return false; } }

        public virtual bool OverrideBondingReqs()
        {
            return false;
        }

        public virtual bool CanAngerOnTame { get { return false; } }

        #region OnAction[...]

        public virtual void OnActionWander()
        {
        }

        public virtual void OnActionCombat()
        {
        }

        public virtual void OnActionGuard()
        {
        }

        public virtual void OnActionFlee()
        {
        }

        public virtual void OnActionInteract()
        {
        }

        public virtual void OnActionBackoff()
        {
        }

        #endregion

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (CheckGold(from, dropped))
                return true;

            // Note: Yes, this happens for all questers (regardless of type, e.g. escorts),
            // even if they can't offer you anything at the moment
            //if ( MLQuestSystem.Enabled && CanGiveMLQuest && from is PlayerMobile )
            //{
            //    MLQuestSystem.Tell( this, (PlayerMobile)from, 1074893 ); // You need to mark your quest items so I don't take the wrong object.  Then speak to me.
            //    return false;
            //}

            return base.OnDragDrop(from, dropped);
        }

        protected virtual BaseAI ForcedAI { get { return null; } }

        public void ChangeAIType(AIType NewAI)
        {
            if (m_AI != null)
            {
                m_AI.m_Timer.Stop();
            }

            m_AI = new GenericAI(this);
        }

        public void ChangeAIToDefault()
        {
            ChangeAIType(m_DefaultAI);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AIType AI
        {
            get
            {
                return m_CurrentAI;
            }

            set
            {
                m_CurrentAI = value;

                if (m_CurrentAI != AIType.AI_Generic)
                {
                    m_CurrentAI = AIType.AI_Generic;
                }

                ChangeAIType(m_CurrentAI);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Debug
        {
            get
            {
                return m_bDebugAI;
            }
            set
            {
                m_bDebugAI = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Team
        {
            get
            {
                return m_iTeam;
            }
            set
            {
                m_iTeam = value;

                OnTeamChange();
            }
        }

        public virtual void OnTeamChange()
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile FocusMob
        {
            get
            {
                return m_FocusMob;
            }
            set
            {
                m_FocusMob = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public FightMode FightMode
        {
            get
            {
                return m_FightMode;
            }
            set
            {
                m_FightMode = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RangePerception
        {
            get
            {
                return m_iRangePerception;
            }
            set
            {
                m_iRangePerception = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RangeFight
        {
            get
            {
                return m_iRangeFight;
            }
            set
            {
                m_iRangeFight = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RangeHome
        {
            get
            {
                return m_iRangeHome;
            }
            set
            {
                m_iRangeHome = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ActiveSpeed
        {
            get
            {
                return m_dActiveSpeed;
            }
            set
            {
                m_dActiveSpeed = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double PassiveSpeed
        {
            get
            {
                return m_dPassiveSpeed;
            }
            set
            {
                m_dPassiveSpeed = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double CurrentSpeed
        {
            get
            {
                //if ( m_TargetLocation != null )
                //  return 0.3;

                return m_dCurrentSpeed;
            }
            set
            {
                if (m_dCurrentSpeed != value)
                {
                    m_dCurrentSpeed = value;

                    if (m_AI != null)
                        m_AI.OnCurrentSpeedChanged();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Home
        {
            get
            {
                if (m_pHome.X == 0 && m_pHome.Y == 0)
                {
                    m_pHome = this.Location;
                }

                return m_pHome;
            }

            set
            {
                m_pHome = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LastEnemyLocation
        {
            get
            {

                return m_LastEnemyLocation;
            }

            set
            {
                m_LastEnemyLocation = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Controlled
        {
            get
            {
                return m_bControlled;
            }
            set
            {
                if (m_bControlled == value)
                    return;

                m_bControlled = value;
                Delta(MobileDelta.Noto);

                InvalidateProperties();
            }
        }

        public override void RevealingAction()
        {
            Spells.Sixth.InvisibilitySpell.RemoveTimer(this);

            IsStealthing = false;

            base.RevealingAction();
        }

        public void RemoveFollowers()
        {
            if (m_ControlMaster != null)
            {
                m_ControlMaster.Followers -= ControlSlots;
                if (m_ControlMaster is PlayerMobile)
                {
                    ((PlayerMobile)m_ControlMaster).AllFollowers.Remove(this);
                    if (((PlayerMobile)m_ControlMaster).AutoStabled.Contains(this))
                        ((PlayerMobile)m_ControlMaster).AutoStabled.Remove(this);
                }
            }
            else if (m_SummonMaster != null)
            {
                m_SummonMaster.Followers -= ControlSlots;
                if (m_SummonMaster is PlayerMobile)
                {
                    ((PlayerMobile)m_SummonMaster).AllFollowers.Remove(this);
                }
            }

            if (m_ControlMaster != null && m_ControlMaster.Followers < 0)
                m_ControlMaster.Followers = 0;

            if (m_SummonMaster != null && m_SummonMaster.Followers < 0)
                m_SummonMaster.Followers = 0;
        }

        public void AddFollowers()
        {
            if (m_ControlMaster != null)
            {
                m_ControlMaster.Followers += ControlSlots;

                if (m_ControlMaster is PlayerMobile)
                {
                    ((PlayerMobile)m_ControlMaster).AllFollowers.Add(this);
                }
            }

            else if (m_SummonMaster != null)
            {
                m_SummonMaster.Followers += ControlSlots;

                if (m_SummonMaster is PlayerMobile)
                {
                    ((PlayerMobile)m_SummonMaster).AllFollowers.Add(this);
                }
            }
        }        

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ControlMaster
        {
            get
            {
                return m_ControlMaster;
            }

            set
            {
                if (m_ControlMaster == value || this == value)
                    return;

                PlayerMobile player = value as PlayerMobile;

                RemoveFollowers();
                m_ControlMaster = value;
                AddFollowers();

                Aggressors.Clear();
                Combatant = null;
                Warmode = false;

                if (m_ControlMaster != null)
                    StopDeleteTimer();

                Delta(MobileDelta.Noto);

                if (player != null)
                {
                    //Convert Creature Stats into Tamed Creature Stats
                    if (m_Owners.Count == 1 && m_TimesTamed == 1)
                    {
                        SetTamedBaseStats();

                        Hits = HitsMax;
                        Stam = StamMax;
                        Mana = ManaMax;
                    }

                    UpdateAI(true);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SummonMaster
        {
            get
            {
                return m_SummonMaster;
            }
            set
            {
                if (m_SummonMaster == value || this == value)
                    return;

                RemoveFollowers();
                m_SummonMaster = value;
                AddFollowers();

                Delta(MobileDelta.Noto);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile SpellTarget
        {
            get
            {
                return m_SpellTarget;
            }
            set
            {
                m_SpellTarget = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile HealTarget
        {
            get
            {
                return m_HealTarget;
            }
            set
            {
                m_HealTarget = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ControlTarget
        {
            get
            {
                return m_ControlTarget;
            }
            set
            {
                m_ControlTarget = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ControlDest
        {
            get
            {
                return m_ControlDest;
            }
            set
            {
                m_ControlDest = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item ControlObject
        {
            get
            {
                return m_ControlObject;
            }
            set
            {
                m_ControlObject = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public OrderType ControlOrder
        {
            get
            {
                return m_ControlOrder;
            }
            set
            {
                m_ControlOrder = value;

                if (m_AI != null)
                    m_AI.OnCurrentOrderChanged();

                InvalidateProperties();

                if (m_ControlMaster != null)
                    m_ControlMaster.InvalidateProperties();
            }
        }

        private bool m_bBardProvoked = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardProvoked
        {
            get
            {
                return m_bBardProvoked;
            }
            set
            {
                m_bBardProvoked = value;
            }
        }

        private bool m_bBardPacified = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardPacified
        {
            get
            {
                return m_bBardPacified;
            }
            set
            {
                m_bBardPacified = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardDiscorded
        {
            get
            {
                Discordance.DiscordanceInfo discordInfo = SkillHandlers.Discordance.GetInfo(this);

                if (discordInfo == null)
                    return false;

                if (discordInfo.m_EndTime > DateTime.UtcNow && discordInfo.m_Effect > 0)
                    return true;

                return false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile DiscordMaster
        {
            get
            {
                Discordance.DiscordanceInfo discordInfo = SkillHandlers.Discordance.GetInfo(this);

                if (discordInfo == null)
                    return null;

                if (discordInfo.m_From != null && discordInfo.m_EndTime > DateTime.UtcNow && discordInfo.m_Effect > 0)
                    return discordInfo.m_From;                

                return null;
            }
        }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public double DiscordEffect
        {
            get
            {
                Discordance.DiscordanceInfo discordInfo = SkillHandlers.Discordance.GetInfo(this);

                if (discordInfo == null)
                    return 0;

                if (discordInfo.m_EndTime > DateTime.UtcNow && discordInfo.m_Effect > 0)
                    return discordInfo.m_Effect;

                return 0;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DiscordEndTime
        {
            get
            {
                Discordance.DiscordanceInfo discordInfo = SkillHandlers.Discordance.GetInfo(this);

                if (discordInfo == null)
                    return DateTime.UtcNow;

                if (discordInfo.m_EndTime > DateTime.UtcNow && discordInfo.m_Effect > 0)
                    return discordInfo.m_EndTime;

                return DateTime.UtcNow;
            }
        }

        private Mobile m_bBardMaster = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BardMaster
        {
            get
            {
                return m_bBardMaster;
            }
            set
            {
                m_bBardMaster = value;
            }
        }

        private Mobile m_bBardTarget = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BardTarget
        {
            get
            {
                return m_bBardTarget;
            }
            set
            {
                m_bBardTarget = value;
            }
        }

        private DateTime m_timeBardEnd;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime BardEndTime
        {
            get
            {
                return m_timeBardEnd;
            }
            set
            {
                m_timeBardEnd = value;
            }
        }

        private Mobile m_FocusedAggressionTarget;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile FocusedAggressionTarget
        {
            get { return m_FocusedAggressionTarget; }
            set { m_FocusedAggressionTarget = value; }
        }

        private double m_FocusedAggresionValue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double FocusedAggresionValue
        {
            get { return m_FocusedAggresionValue; }
            set { m_FocusedAggresionValue = value; }
        }

        private DateTime m_FocusedAggressionExpiration;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FocusedAggressionExpiration
        {
            get { return m_FocusedAggressionExpiration; }
            set { m_FocusedAggressionExpiration = value; }
        }

        private double m_MinTameSkill;
        [CommandProperty(AccessLevel.GameMaster)]
        public double MinTameSkill
        {
            get
            {
                return m_MinTameSkill;
            }
            set
            {
                m_MinTameSkill = value;
            }
        }

        private bool m_Tameable;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Tameable
        {
            get
            {
                return m_Tameable && !m_Paragon && !Rare;
            }
            set
            {
                m_Tameable = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Summoned
        {
            get
            {
                return m_Summoned;
            }
            set
            {
                if (m_Summoned == value)
                    return;

                m_NextReacquireTime = Core.TickCount;

                m_Summoned = value;
                Delta(MobileDelta.Noto);

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlots
        {
            get
            {
                return m_iControlSlots;
            }
            set
            {
                m_iControlSlots = value;
            }
        }

        public virtual bool IsHenchman { get { return false; } }

        public virtual bool NoHouseRestrictions { get { return false; } }
        public virtual bool IsHouseSummonable { get { return false; } }
        
        public virtual bool IsScaryToPets { get { return false; } }
        public virtual bool IsScaredOfScaryThings { get { return true; } }

        public virtual bool CanRummageCorpses { get { return false; } }

        public virtual bool HasFocusOnMob { get; set; }
        public virtual void OnEndFocus()
        {
            FocusMob = null;
            HasFocusOnMob = false;
        }

        public virtual void Dispel(Mobile m)
        {
            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
            Effects.PlaySound(m, m.Map, 0x201);

            BaseCreature bc = m as BaseCreature;

            if (bc != null)
            {
                //Enhanced Spellbook: Summoner (Possible Dispel Prevention Against Monsters)
                if (Utility.RandomDouble() >= bc.DispelResist)                
                    m.Delete();                
            }

            else            
                m.Delete();            
        }

        public virtual bool DeleteOnRelease { get { return m_Summoned; } }

        public override void OnAfterDelete()
        {
            if (m_AI != null)
            {
                if (m_AI.m_Timer != null)
                    m_AI.m_Timer.Stop();

                m_AI = null;
            }

            if (m_DeleteTimer != null)
            {
                m_DeleteTimer.Stop();
                m_DeleteTimer = null;
            }

            if (m_StamFreeMoveAuraTimer != null)
            {
                m_StamFreeMoveAuraTimer.Stop();
                m_StamFreeMoveAuraTimer = null;
            }

            if (m_SpecialAbilityEffectTimer != null)
            {
                m_SpecialAbilityEffectTimer.Stop();
                m_SpecialAbilityEffectTimer = null;
            }

            if (m_AddSpecialAbilityEffectTimer != null)
            {
                m_AddSpecialAbilityEffectTimer.Stop();
                m_AddSpecialAbilityEffectTimer = null;
            }

            if (m_RemoveSpecialAbilityEffectTimer != null)
            {
                m_RemoveSpecialAbilityEffectTimer.Stop();
                m_RemoveSpecialAbilityEffectTimer = null;
            }

            FocusMob = null;

            base.OnAfterDelete();
        }

        public void DebugSay(string text)
        {
            if (m_bDebugAI)
                this.PublicOverheadMessage(MessageType.Regular, 41, false, text);
        }

        public void DebugSay(string format, params object[] args)
        {
            if (m_bDebugAI)
                this.PublicOverheadMessage(MessageType.Regular, 41, false, String.Format(format, args));
        }

        public void FaceRandomDirection()
        {
            Direction d = (Direction)(Utility.RandomMinMax(0, 7));

            if (Direction != d)
                Direction = d;
        }

        // Turn, - for left, + for right
        // Basic for now, needs work
        public virtual void Turn(int iTurnSteps)
        {
            int v = (int)Direction;

            Direction = (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80));
        }

        public virtual void TurnInternal(int iTurnSteps)
        {
            int v = (int)Direction;

            SetDirection((Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80)));
        }

        public bool IsHurt()
        {
            return (Hits != HitsMax);
        }

        public double GetHomeDistance()
        {
            return GetDistanceToSqrt(m_pHome);
        }

        public virtual int GetTeamSize(int iRange)
        {
            int iCount = 0;

            IPooledEnumerable eable = this.GetMobilesInRange(iRange);

            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    if (((BaseCreature)m).Team == Team)
                    {
                        if (!m.Deleted)
                        {
                            if (m != this)
                            {
                                if (CanSee(m))
                                {
                                    iCount++;
                                }
                            }
                        }
                    }
                }
            }

            eable.Free();

            return iCount;
        }

        private class TameEntry : ContextMenuEntry
        {
            private BaseCreature m_Mobile;

            public TameEntry(Mobile from, BaseCreature creature)
                : base(6130, 6)
            {
                m_Mobile = creature;

                Enabled = Enabled && (from.Female ? creature.AllowFemaleTamer : creature.AllowMaleTamer);
            }

            public override void OnClick()
            {
                if (!Owner.From.CheckAlive())
                    return;

                Owner.From.TargetLocked = true;
                SkillHandlers.AnimalTaming.DisableMessage = true;

                if (Owner.From.UseSkill(SkillName.AnimalTaming))
                    Owner.From.Target.Invoke(Owner.From, m_Mobile);

                SkillHandlers.AnimalTaming.DisableMessage = false;
                Owner.From.TargetLocked = false;
            }
        }

        #region Teaching
        public virtual bool CanTeach { get { return false; } }

        public virtual bool CheckTeach(SkillName skill, Mobile from)
        {
            if (!CanTeach)
                return false;

            if (skill == SkillName.Stealth && from.Skills[SkillName.Hiding].Base < Stealth.HidingRequirement)
                return false;

            return true;
        }

        public enum TeachResult
        {
            Success,
            Failure,
            KnowsMoreThanMe,
            KnowsWhatIKnow,
            SkillNotRaisable,
            NotEnoughFreePoints
        }

        public virtual TeachResult CheckTeachSkills(SkillName skill, Mobile m, int maxPointsToLearn, ref int pointsToLearn, bool doTeach)
        {
            if (!CheckTeach(skill, m) || !m.CheckAlive())
                return TeachResult.Failure;

            Skill ourSkill = Skills[skill];
            Skill theirSkill = m.Skills[skill];

            if (ourSkill == null || theirSkill == null)
                return TeachResult.Failure;

            int baseToSet = ourSkill.BaseFixedPoint / 3;

            if (baseToSet > 420)
                baseToSet = 420;
            else if (baseToSet < 200)
                return TeachResult.Failure;

            if (baseToSet > theirSkill.CapFixedPoint)
                baseToSet = theirSkill.CapFixedPoint;

            pointsToLearn = baseToSet - theirSkill.BaseFixedPoint;

            if (maxPointsToLearn > 0 && pointsToLearn > maxPointsToLearn)
            {
                pointsToLearn = maxPointsToLearn;
                baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
            }

            if (pointsToLearn < 0)
                return TeachResult.KnowsMoreThanMe;

            if (pointsToLearn == 0)
                return TeachResult.KnowsWhatIKnow;

            if (theirSkill.Lock != SkillLock.Up)
                return TeachResult.SkillNotRaisable;

            int freePoints = m.Skills.Cap - m.Skills.Total;
            int freeablePoints = 0;

            if (freePoints < 0)
                freePoints = 0;

            for (int i = 0; (freePoints + freeablePoints) < pointsToLearn && i < m.Skills.Length; ++i)
            {
                Skill sk = m.Skills[i];

                if (sk == theirSkill || sk.Lock != SkillLock.Down)
                    continue;

                freeablePoints += sk.BaseFixedPoint;
            }

            if ((freePoints + freeablePoints) == 0)
                return TeachResult.NotEnoughFreePoints;

            if ((freePoints + freeablePoints) < pointsToLearn)
            {
                pointsToLearn = freePoints + freeablePoints;
                baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
            }

            if (doTeach)
            {
                int need = pointsToLearn - freePoints;

                for (int i = 0; need > 0 && i < m.Skills.Length; ++i)
                {
                    Skill sk = m.Skills[i];

                    if (sk == theirSkill || sk.Lock != SkillLock.Down)
                        continue;

                    if (sk.BaseFixedPoint < need)
                    {
                        need -= sk.BaseFixedPoint;
                        sk.BaseFixedPoint = 0;
                    }
                    else
                    {
                        sk.BaseFixedPoint -= need;
                        need = 0;
                    }
                }

                /* Sanity check */
                if (baseToSet > theirSkill.CapFixedPoint || (m.Skills.Total - theirSkill.BaseFixedPoint + baseToSet) > m.Skills.Cap)
                    return TeachResult.NotEnoughFreePoints;

                theirSkill.BaseFixedPoint = baseToSet;
            }

            return TeachResult.Success;
        }

        public virtual bool CheckTeachingMatch(Mobile m)
        {
            if (m_Teaching == (SkillName)(-1))
                return false;

            if (m is PlayerMobile)
                return (((PlayerMobile)m).Learning == m_Teaching);

            return true;
        }

        private SkillName m_Teaching = (SkillName)(-1);

        public virtual bool Teach(SkillName skill, Mobile m, int maxPointsToLearn, bool doTeach)
        {
            int pointsToLearn = 0;
            maxPointsToLearn *= 3;

            TeachResult res = CheckTeachSkills(skill, m, maxPointsToLearn, ref pointsToLearn, doTeach);

            switch (res)
            {
                case TeachResult.KnowsMoreThanMe:
                    {
                        Say(501508); // I cannot teach thee, for thou knowest more than I!
                        break;
                    }
                case TeachResult.KnowsWhatIKnow:
                    {
                        Say(501509); // I cannot teach thee, for thou knowest all I can teach!
                        break;
                    }
                case TeachResult.NotEnoughFreePoints:
                case TeachResult.SkillNotRaisable:
                    {
                        // Make sure this skill is marked to raise. If you are near the skill cap (700 points) you may need to lose some points in another skill first.
                        m.SendLocalizedMessage(501510, "", 0x22);
                        break;
                    }
                case TeachResult.Success:
                    {
                        if (doTeach)
                        {
                            Say(501539); // Let me show thee something of how this is done.
                            m.SendLocalizedMessage(501540); // Your skill level increases.

                            m_Teaching = (SkillName)(-1);

                            if (m is PlayerMobile)
                                ((PlayerMobile)m).Learning = (SkillName)(-1);
                        }

                        else
                        {
                            // I will teach thee all I know, if paid the amount in full.  The price is:
                            Say(1019077, AffixType.Append, String.Format(" {0}", (int)(pointsToLearn / 3)), "");
                            Say(1043108); // For less I shall teach thee less.

                            m_Teaching = skill;

                            if (m is PlayerMobile)
                                ((PlayerMobile)m).Learning = skill;
                        }

                        return true;
                    }
            }

            return false;
        }
        #endregion

        public override void AggressiveAction(Mobile target, bool criminal, bool causeCombat)
        {
            if (target == null) return;
            if (target.Deleted) return;
            if (target == this) return;
            if (Blessed) return;
            if (target.Blessed) return;
            if (m_AI == null) return;

            if (target is PlayerMobile && target.AccessLevel == AccessLevel.Player)
            {
                PlayerMobile playerAggressor = target as PlayerMobile;

                foreach (AggressorInfo info in playerAggressor.Aggressed)
                {
                    if (info.Defender == this)
                    {
                        playerAggressor.SystemOverloadActions++;

                        break;
                    }
                }
            }

            if (Controlled && ControlMaster is PlayerMobile && AIObject != null)
            {
                if (!(ControlOrder == OrderType.Attack || ControlOrder == OrderType.Guard || ControlOrder == OrderType.Patrol || ControlOrder == OrderType.Stay))
                    causeCombat = false;
            }

            if (this is EnergyVortex || this is BladeSpirits)
            {
                if (SummonMaster != null)
                    SummonMaster.DoHarmful(target);
            }

            base.AggressiveAction(target, criminal, causeCombat);

            //Kin Ally Violation
            /*
            if (AIKinTeamList.CheckKinTeam(this, target))
            {
                PlayerMobile pm_Aggresor = target as PlayerMobile;

                if (pm_Aggresor != null)
                {
                    bool orcMaskEquipped = false;

                    Item headItem = pm_Aggresor.FindItemOnLayer(Layer.Helm);
                    {
                        if (headItem is OrcishKinMask)
                            orcMaskEquipped = true;
                    }

                    if (orcMaskEquipped)
                    {
                        pm_Aggresor.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                        pm_Aggresor.PlaySound(0x658);

                        pm_Aggresor.SendMessage("Mystical energy sears you as attack your kin ally!");

                        Container pack = pm_Aggresor.Backpack;

                        if (pack != null)
                        {
                            pack.DropItem(headItem);
                            pm_Aggresor.SendMessage("Your mask drops into your backpack.");
                        }

                        pm_Aggresor.DoHarmful(this);

                        AOS.Damage(pm_Aggresor, 50, 0, 100, 0, 0, 0);
                    }

                    else
                    {
                        pm_Aggresor.KinPaintHue = -1;
                        pm_Aggresor.KinPaintExpiration = DateTime.MinValue;

                        pm_Aggresor.BodyMod = 0;
                        pm_Aggresor.HueMod = -1;

                        pm_Aggresor.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                        pm_Aggresor.PlaySound(0x658);

                        pm_Aggresor.SendMessage("Your kin paint fades as you attack your kin ally!");
                    }
                }
            }
            */
        }

        public override void PushNotoriety(Mobile from, Mobile to, bool aggressor)
        {
            NotorietyHandlers.PushNotoriety(from, to, aggressor);
        }

        public override bool OnMoveOver(Mobile m)
        {
            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                bool allowMoveOver = (!Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet) || (Hidden && AccessLevel > AccessLevel.Player);

                if (allowMoveOver)
                    return true;
            }

            #region Dueling
            if (Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)) && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                if (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished || pm.DuelPlayer.Eliminated)
                    return true;
            }
            #endregion

            return base.OnMoveOver(m);
        }

        public bool CanStamFreeShove()
        {
            //Mini-Bosses and Bosses Can Move Freely Through Mobiles
            if (IsBoss() || IsChamp() || IsLoHBoss() || IsEventBoss())
                return true;

            //Tamed Creatures 
            if (Controlled && ControlMaster is PlayerMobile)
            {
                //Currently Allowed Stamina-Free Movement
                if (StamFreeMoveExpiration > DateTime.UtcNow && (int)GetDistanceToSqrt(StamFreeMoveSource) <= BaseCreature.StamFreeMoveRange)
                    return true;
            }

            return false;
        }

        public override bool CheckShove(Mobile shoved)
        {
            if ((Map.Rules & MapRules.FreeMovement) == 0)
            {
                if (!shoved.Alive || !Alive || shoved.IsDeadBondedPet || IsDeadBondedPet)
                    return true;

                else if (shoved.Hidden && shoved.AccessLevel > AccessLevel.Player)
                    return true;

                if (CanStamFreeShove())
                    return true;

                if (!Pushing)
                {
                    Pushing = true;

                    int number;

                    if (this.AccessLevel > AccessLevel.Player)
                        number = shoved.Hidden ? 1019041 : 1019040;

                    else
                    {
                        if (Stam == StamMax)
                        {
                            number = shoved.Hidden ? 1019043 : 1019042;
                            Stam -= 10;
                        }

                        else
                            return false;
                    }

                    SendLocalizedMessage(number);
                }
            }

            return true;
        }

        public virtual void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public virtual bool CanDrop { get { return IsBonded; } }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_AI != null && Commandable)
                m_AI.GetContextMenuEntries(from, list);

            if (m_Tameable && !m_bControlled && from.Alive && !(from.Region is UOACZRegion))
                list.Add(new TameEntry(from, this));

            AddCustomContextEntries(from, list);

            if (CanTeach && from.Alive)
            {
                Skills ourSkills = this.Skills;
                Skills theirSkills = from.Skills;

                for (int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i)
                {
                    Skill skill = ourSkills[i];
                    Skill theirSkill = theirSkills[i];

                    if (skill != null && theirSkill != null && skill.Base >= 60.0 && CheckTeach(skill.SkillName, from))
                    {
                        double toTeach = skill.Base / 3.0;

                        if (toTeach > 42.0)
                            toTeach = 42.0;

                        list.Add(new TeachEntry((SkillName)i, this, from, (toTeach > theirSkill.Base)));
                    }
                }
            }
        }

        public override bool CanSee(Mobile m)
        {
            return base.CanSee(m);
        }

        public override bool CheckCreatureOwnership(Mobile target)
        {
            PlayerMobile player = target as PlayerMobile;

            if (player != null)
            {
                if (this.Controlled && this.ControlMaster == target)
                    return true;
            }

            return false;
        }

        public override bool CheckHearsMutatedSpeech(Mobile m, object context)
        {
            PlayerMobile player = m as PlayerMobile;

            if (player != null)
            {
                if (Controlled && ControlMaster == player)
                    return true;
            }

            return base.CheckHearsMutatedSpeech(m, context);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            InhumanSpeech speechType = this.SpeechType;

            int speechRange = m_iRangePerception;

            if (Controlled && ControlMaster is PlayerMobile)
                speechRange = (int)((double)m_iRangePerception * 1.5);

            if (speechType != null && (speechType.Flags & IHSFlags.OnSpeech) != 0 && from.InRange(this, 3))
                return true;

            return m_AI != null && m_AI.HandlesOnSpeech(from) && from.InRange(this, speechRange);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            InhumanSpeech speechType = this.SpeechType;

            int speechRange = m_iRangePerception;

            if (Controlled && ControlMaster is PlayerMobile)
                speechRange = (int)((double)m_iRangePerception * 1.5);

            if (speechType != null && speechType.OnSpeech(this, e.Mobile, e.Speech))
                e.Handled = true;

            else if (!e.Handled && m_AI != null && e.Mobile.InRange(this, speechRange))
                m_AI.OnSpeech(e);
        }

        public override bool IsHarmfulCriminal(Mobile target)
        {
            if ((Controlled && target == m_ControlMaster) || (Summoned && target == m_SummonMaster))
                return false;

            if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
                return false;

            if (target is PlayerMobile && ((PlayerMobile)target).PermaFlags.Count > 0)
                return false;

            return base.IsHarmfulCriminal(target);
        }

        public override void DoHarmful(Mobile target, bool indirect)
        {
            if (target == null)
                return;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;
            PlayerMobile pm_Controller = ControlMaster as PlayerMobile;

            if (target != this)
            {
                LastCombatTime = DateTime.UtcNow;
                target.LastCombatTime = DateTime.UtcNow;

                if (bc_Target != null)
                {
                    PlayerMobile pm_TargetController = bc_Target.ControlMaster as PlayerMobile;

                    if (pm_Controller != null)
                        pm_Controller.LastCombatTime = DateTime.UtcNow;

                    if (pm_TargetController != null)
                        pm_TargetController.LastCombatTime = DateTime.UtcNow;

                    if (pm_Controller != null && pm_TargetController != null && pm_Controller != pm_TargetController)
                    {
                        LastPlayerCombatTime = DateTime.UtcNow;

                        bc_Target.LastPlayerCombatTime = DateTime.UtcNow;

                        pm_Controller.PlayerVsPlayerCombatOccured(pm_TargetController);
                        pm_TargetController.PlayerVsPlayerCombatOccured(pm_Controller);
                    }
                }

                if (pm_Target != null)
                {
                    pm_Target.LastCombatTime = DateTime.UtcNow;

                    if (pm_Controller != null && pm_Controller != pm_Target)
                    {
                        LastPlayerCombatTime = DateTime.UtcNow;

                        pm_Controller.PlayerVsPlayerCombatOccured(pm_Target);
                        pm_Target.PlayerVsPlayerCombatOccured(pm_Controller);
                    }
                }
            }

            base.DoHarmful(target, indirect);
        }

        public void CapStatMods(Mobile mobile)
        {
            //Enhanced Spellbook: Wizard has Buff Spells with 5x Duration.
            //Need to bring their duration down to normal maximum if another player does a harmful action to them        
            for (int i = 0; i < mobile.StatMods.Count; ++i)
            {
                StatMod check = mobile.StatMods[i];

                if (check.Type == StatType.Str || check.Type == StatType.Dex || check.Type == StatType.Int)
                {
                    if (check.Duration >= TimeSpan.FromSeconds(120))
                        check.Duration = TimeSpan.FromSeconds(120);
                }
            }
        }

        public override void OnHeal(ref int amount, Mobile from)
        {
            SpecialAbilities.HealingOccured(from, this, amount);

            base.OnHeal(ref amount, from);
        }

        public override void OnHarmfulAction(Mobile target, bool isCriminal)
        {
            if (isCriminal)
            {
                CriminalAction(false);
                if (Controlled && ControlMaster != null)
                    ControlMaster.DoHarmful(target, isCriminal);
            }

        }

        private static Mobile m_NoDupeGuards;

        public void ReleaseGuardDupeLock()
        {
            m_NoDupeGuards = null;
        }

        public void ReleaseGuardLock()
        {
            EndAction(typeof(GuardedRegion));
        }

        private DateTime m_IdleReleaseTime;

        public void AnimateIdle()
        {
            //Override
            if (IdleAnimation != -1 && IdleFrames > 0)
            {
                Animate(IdleAnimation, IdleFrames, 1, IdleAnimationPlayForwards, false, 1);
                return;
            }

            //High Seas Creature Animation Override
            if (IsHighSeasBodyType)
                Animate(Utility.RandomList(1, 25, 28), 5, 1, IdleAnimationPlayForwards, false, 1);

            else if (Body.IsHuman)
            {
                switch (Utility.Random(2))
                {
                    case 0: CheckedAnimate(5, 5, 1, IdleAnimationPlayForwards, true, 1); break;
                    case 1: CheckedAnimate(6, 5, 1, IdleAnimationPlayForwards, false, 1); break;
                }
            }

            else if (Body.IsAnimal)
            {
                switch (Utility.Random(3))
                {
                    case 0: CheckedAnimate(3, 3, 1, IdleAnimationPlayForwards, false, 1); break;
                    case 1: CheckedAnimate(9, 5, 1, IdleAnimationPlayForwards, false, 1); break;
                    case 2: CheckedAnimate(10, 5, 1, IdleAnimationPlayForwards, false, 1); break;
                }
            }

            else if (Body.IsMonster)
            {
                switch (Utility.Random(2))
                {
                    case 0: CheckedAnimate(17, 5, 1, IdleAnimationPlayForwards, false, 1); break;
                    case 1: CheckedAnimate(18, 5, 1, IdleAnimationPlayForwards, false, 1); break;
                }
            }
        }

        public virtual bool CheckIdle()
        {
            if (Combatant != null)
                return false; // in combat.. not idling

            if (m_IdleReleaseTime > DateTime.MinValue)
            {
                // idling...

                if (DateTime.UtcNow >= m_IdleReleaseTime)
                {
                    m_IdleReleaseTime = DateTime.MinValue;
                    return false; // idle is over
                }

                return true; // still idling
            }

            if (95 > Utility.Random(100))
                return false; // not idling, but don't want to enter idle state

            m_IdleReleaseTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(15, 25));

            AnimateIdle();
            PlaySound(GetIdleSound());

            return true; // entered idle state
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

        private void CheckAIActive()
        {
            Map map = Map;

            if (PlayerRangeSensitive && m_AI != null && map != null && map.GetSector(Location).Active)
                m_AI.Activate();
        }

        public override void OnCombatantChange()
        {
            base.OnCombatantChange();

            Warmode = (Combatant != null && !Combatant.Deleted && Combatant.Alive);

            if (CanFly && Warmode)
            {
                Flying = false;
            }
        }

        protected override void OnMapChange(Map oldMap)
        {
            CheckAIActive();

            base.OnMapChange(oldMap);
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            if (Controlled && ControlMaster is PlayerMobile)
            {
                BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

                if (boat == null)
                    m_BoatOccupied = null;
                else
                    m_BoatOccupied = boat;
            }

            CheckAIActive();

            base.OnLocationChange(oldLocation);
        }

        public virtual void ForceReacquire()
        {
            m_NextReacquireTime = Core.TickCount;
        }

        protected override bool OnMove(Direction d)
        {
            if (Hidden)
            {
                if (CheckStealthSkillForRevealingAction())
                    RevealingAction();

                else
                {
                    double footprintChance = StealthFootprintChance;

                    if (RevealImmune)
                        footprintChance = StealthFootprintRevealImmuneChance;

                    if (Utility.RandomDouble() < footprintChance)
                        new Footsteps(d).MoveToWorld(this.Location, this.Map);
                }
            }

            return true;
        }

        private bool CheckStealthSkillForRevealingAction()
        {
            if (!m_IsStealthing)
                return true;

            //Creature Has Stealth Steps Available (No Check)           
            if (AllowedStealthSteps > 0)
            {
                AllowedStealthSteps--;
                return false;
            }

            //No Free Steps Remaining
            else
            {
                double BaseSuccessPercent = StealthStepSuccessBasePercent;
                double BonusSuccessPercent = StealthStepSkillBonusDivider;

                double chance = (BaseSuccessPercent + (this.Skills[SkillName.Stealth].Value / BonusSuccessPercent)) / 100;

                if (chance >= Utility.RandomDouble())
                {
                    return false;
                }
            }

            return true;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
           if (ReacquireOnMovement)
                ForceReacquire();

            InhumanSpeech speechType = this.SpeechType;

            if (speechType != null)
                speechType.OnMovement(this, m, oldLocation);

            if ((!m.Hidden || m.AccessLevel == AccessLevel.Player) && m.Player && m_FightMode != FightMode.Aggressor && m_FightMode != FightMode.None && Combatant == null && !Controlled && !Summoned)
            {
                if (InRange(m.Location, 18) && !InRange(oldLocation, 18))
                {
                    if (IsHighSeasBodyType)
                        Animate(Utility.RandomList(27), 5, 1, true, false, 1);

                    else if (Body.IsMonster)
                        Animate(11, 5, 1, true, false, 1);

                    PlaySound(GetAngerSound());
                }
            }

            if (m_NoDupeGuards == m)
                return;

            if (!Body.IsHuman || IsMurderer() || AlwaysAttackable || m.ShortTermMurders < 5 || !m.InRange(Location, 12) || !m.Alive)
                return;

            GuardedRegion guardedRegion = (GuardedRegion)this.Region.GetRegion(typeof(GuardedRegion));

            if (guardedRegion != null)
            {
                if (!guardedRegion.IsDisabled() && guardedRegion.IsGuardCandidate(m) && BeginAction(typeof(GuardedRegion)))
                {
                    if (!(Notoriety.Compute(m, this) == Notoriety.CanBeAttacked))
                    {
                        Say(1013037 + Utility.Random(16));
                        guardedRegion.CallGuards(this.Location);

                        Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(ReleaseGuardLock));

                        m_NoDupeGuards = m;
                        Timer.DelayCall(TimeSpan.Zero, new TimerCallback(ReleaseGuardDupeLock));
                    }
                }
            }
        }

        #region Set[...]

        public void SetDamage(int val)
        {
            m_DamageMin = val;
            m_DamageMax = val;
        }

        public void SetDamage(int min, int max)
        {
            m_DamageMin = min;
            m_DamageMax = max;
        }

        public void SetHits(int val)
        {
            m_HitsMax = val;
            Hits = HitsMax;
        }

        public void SetHits(int min, int max)
        {
            m_HitsMax = Utility.RandomMinMax(min, max);
            Hits = HitsMax;
        }

        public void SetStam(int val)
        {
            m_StamMax = val;

            Stam = StamMax;
        }

        public void SetStam(int min, int max)
        {
            m_StamMax = Utility.RandomMinMax(min, max);
            Stam = StamMax;
        }

        public void SetMana(int val)
        {
            m_ManaMax = val;
            Mana = ManaMax;
        }

        public void SetMana(int min, int max)
        {
            m_ManaMax = Utility.RandomMinMax(min, max);
            Mana = ManaMax;
        }

        public void SetStr(int val)
        {
            RawStr = val;
            Hits = HitsMax;
        }

        public void SetStr(int min, int max)
        {
            RawStr = Utility.RandomMinMax(min, max);
            Hits = HitsMax;
        }

        public void SetDex(int val)
        {
            RawDex = val;
            Stam = StamMax;
        }

        public void SetDex(int min, int max)
        {
            RawDex = Utility.RandomMinMax(min, max);
            Stam = StamMax;
        }

        public void SetInt(int val)
        {
            RawInt = val;
            Mana = ManaMax;
        }

        public void SetInt(int min, int max)
        {
            RawInt = Utility.RandomMinMax(min, max);
            Mana = ManaMax;
        }

        //Tamed Creature Overrides
        public void SetStrMax(int val)
        {
            RawStr = val;
        }

        public void SetDexMax(int val)
        {
            RawDex = val;
            m_StamMax = val;

            if (Stam > m_StamMax)
                Stam = m_StamMax;
        }

        public void SetIntMax(int val)
        {
            RawInt = val;
            m_ManaMax = val;

            if (Mana > m_ManaMax)
                Mana = m_ManaMax;
        }

        public void SetHitsMax(int val)
        {
            m_HitsMax = val;

            if (Hits > m_HitsMax)
                Hits = m_HitsMax;
        }

        public void SetManaMax(int val)
        {
            m_ManaMax = val;

            if (Mana > m_ManaMax)
                Mana = m_ManaMax;
        }

        public void SetDamageType(ResistanceType type, int min, int max)
        {
            SetDamageType(type, Utility.RandomMinMax(min, max));
        }

        public void SetDamageType(ResistanceType type, int val)
        {
            switch (type)
            {
                case ResistanceType.Physical: m_PhysicalDamage = val; break;
                case ResistanceType.Fire: m_FireDamage = val; break;
                case ResistanceType.Cold: m_ColdDamage = val; break;
                case ResistanceType.Poison: m_PoisonDamage = val; break;
                case ResistanceType.Energy: m_EnergyDamage = val; break;
            }
        }

        public void SetResistance(ResistanceType type, int min, int max)
        {
            SetResistance(type, Utility.RandomMinMax(min, max));
        }

        public void SetResistance(ResistanceType type, int val)
        {
            switch (type)
            {
                case ResistanceType.Physical: m_PhysicalResistance = val; break;
                case ResistanceType.Fire: m_FireResistance = val; break;
                case ResistanceType.Cold: m_ColdResistance = val; break;
                case ResistanceType.Poison: m_PoisonResistance = val; break;
                case ResistanceType.Energy: m_EnergyResistance = val; break;
            }

            UpdateResistances();
        }

        public void SetSkill(SkillName name, double val)
        {
            Skills[name].BaseFixedPoint = (int)(val * 10);

            if (Skills[name].Base > Skills[name].Cap)
            {
                if (Core.SE)
                    this.SkillsCap += (Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint);

                Skills[name].Cap = Skills[name].Base;
            }
        }

        public void SetSkill(SkillName name, double min, double max)
        {
            int minFixed = (int)(min * 10);
            int maxFixed = (int)(max * 10);

            Skills[name].BaseFixedPoint = Utility.RandomMinMax(minFixed, maxFixed);

            if (Skills[name].Base > Skills[name].Cap)
            {
                if (Core.SE)
                    this.SkillsCap += (Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint);

                Skills[name].Cap = Skills[name].Base;
            }
        }

        public void SetFameLevel(int level)
        {
            switch (level)
            {
                case 1: Fame = Utility.RandomMinMax(0, 1249); break;
                case 2: Fame = Utility.RandomMinMax(1250, 2499); break;
                case 3: Fame = Utility.RandomMinMax(2500, 4999); break;
                case 4: Fame = Utility.RandomMinMax(5000, 9999); break;
                case 5: Fame = Utility.RandomMinMax(10000, 10000); break;
            }
        }

        public void SetKarmaLevel(int level)
        {
            switch (level)
            {
                case 0: Karma = -Utility.RandomMinMax(0, 624); break;
                case 1: Karma = -Utility.RandomMinMax(625, 1249); break;
                case 2: Karma = -Utility.RandomMinMax(1250, 2499); break;
                case 3: Karma = -Utility.RandomMinMax(2500, 4999); break;
                case 4: Karma = -Utility.RandomMinMax(5000, 9999); break;
                case 5: Karma = -Utility.RandomMinMax(10000, 10000); break;
            }
        }

        #endregion

        public static void Cap(ref int val, int min, int max)
        {
            if (val < min)
                val = min;
            else if (val > max)
                val = max;
        }

        #region Pack & Loot

        public void PackCraftingComponent(int chanceAttempts, double chance)
        {
            for (int a = 0; a < chanceAttempts; a++)
            {
                if (Utility.RandomDouble() <= chance)
                    PackItem(CraftingComponent.GetRandomCraftingComponent(1));
            }
        }

        public void PackResearchMaterials(int chanceAttempts, double chance)
        {
            for (int a = 0; a < chanceAttempts; a++)
            {
                if (Utility.RandomDouble() <= chance)
                    PackItem(new Custom.ResearchMaterials());
            }
        }

        public void PackPrestigeScroll(int chanceAttempts, double chance)
        {
            for (int a = 0; a < chanceAttempts; a++)
            {
                if (Utility.RandomDouble() <= chance)
                    PackItem(new PrestigeScroll());
            }
        }

        public void PackPetDye(int chanceAttempts, double chance)
        {
            for (int a = 0; a < chanceAttempts; a++)
            {
                if (Utility.RandomDouble() <= chance)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: PackItem(new EmeraldPetDye()); break;
                        case 2: PackItem(new SapphirePetDye()); break;
                        case 3: PackItem(new AmethystPetDye()); break;
                    }
                }
            }
        }

        public void PackSpellHueDeed(int chanceAttempts, double chance)
        {
            for (int a = 0; a < chanceAttempts; a++)
            {
                if (Utility.RandomDouble() <= chance)
                    PackItem(new Custom.SpellHueDeed());
            }
        }

        public void PackUOACZUnlockableDeed(int chanceAttempts, double chance)
        {
            for (int a = 0; a < chanceAttempts; a++)
            {
                if (Utility.RandomDouble() <= chance)
                    PackItem(new Custom.UOACZUnlockableDeed());
            }
        }

        public void PackPotion()
        {
            PackItem(Loot.RandomPotion());
        }

        public void PackScroll(int minCircle, int maxCircle)
        {
            PackScroll(Utility.RandomMinMax(minCircle, maxCircle));
        }

        public void PackScroll(int circle)
        {
            int min = (circle - 1) * 8;

            PackItem(Loot.RandomScroll(min, min + 7, SpellbookType.Regular));
        }

        public void PackMagicItems(int minLevel, int maxLevel)
        {
            float increaseChance = 1.0f;
            float armorChance = .30f * increaseChance;
            float weaponChance = .15f * increaseChance;

            PackMagicItems(minLevel, maxLevel, armorChance, weaponChance);
        }

        public void PackMagicItems(int minLevel, int maxLevel, double armorChance, double weaponChance)
        {
            if (!PackArmor(minLevel, maxLevel, armorChance))
                PackWeapon(minLevel, maxLevel, weaponChance);
        }

        public void PackMagicItems(int minLevel, int maxLevel, double bothChance)
        {
            if (!PackArmor(minLevel, maxLevel, bothChance))
                PackWeapon(minLevel, maxLevel, bothChance);
        }

        public virtual void DropBackpack()
        {
            if (Backpack != null)
            {
                if (Backpack.Items.Count > 0)
                {
                    Backpack b = new CreatureBackpack(Name);

                    List<Item> list = new List<Item>(Backpack.Items);
                    foreach (Item item in list)
                    {
                        b.DropItem(item);
                    }

                    BaseHouse house = BaseHouse.FindHouseAt(this);
                    if (house != null)
                        b.MoveToWorld(house.BanLocation, house.Map);
                    else
                        b.MoveToWorld(Location, Map);
                }
            }
        }

        protected bool m_Spawning;
        protected int m_KillersLuck = -1;

        public virtual void GenerateLoot(bool spawning)
        {
            m_Spawning = spawning;

            if (!spawning)
            {
                int gold = GoldWorth;                

                if (gold > 0)
                {
                    if (LootDropMode != LootDropModeType.None)
                    {
                        switch (LootDropMode)
                        {
                            case LootDropModeType.Gold:
                                PackGold(gold);
                            break;

                            case LootDropModeType.Hides:
                                int hideAmount = (int)(Math.Round((double)gold / (double)Hide.GoldValue));

                                if (hideAmount < 1)
                                    hideAmount = 1;

                                PackItem(new Hide(hideAmount));
                            break;
                        }

                        Custom.GoldCoinTracker.TrackGoldCoinLoot(gold, LastPlayerKiller);

                        Loot.AddTieredLoot(this);
                    }
                }
            }

            if (!NoKillAwards)
                GenerateLoot();

            m_Spawning = false;
        }

        [CommandProperty(AccessLevel.Counselor)]
        public virtual int GoldWorth
        {
            get
            {
                double goldValue = 5;

                goldValue = InitialDifficulty * 12.5;

                if (goldValue < 5)
                    goldValue = 5;

                double goldScalar = 1.0 + RegionGoldMod.GetModifier(Region);

                if (Region is NewbieDungeonRegion)
                    goldScalar -= NewbieRegionGoldDropScalar;

                goldValue *= goldScalar;
                
                int minGold = (int)(Math.Round((1 - GoldDropVariation) * goldValue));
                int maxGold = (int)(Math.Round((1 + GoldDropVariation) * goldValue));

                if (minGold < 1)
                    minGold = 1;

                if (maxGold < 1)
                    maxGold = 1;

                int finalGoldAmount = Utility.RandomMinMax(minGold, maxGold);

                return finalGoldAmount;
            }
        }

        public virtual void GenerateLoot()
        {
        }

        public virtual void AddLoot(LootPack pack, int amount)
        {
            for (int i = 0; i < amount; ++i)
                AddLoot(pack);
        }

        public virtual void AddLoot(LootPack pack)
        {
            if (Summoned)
                return;

            Container backpack = Backpack;

            if (backpack == null)
            {
                backpack = new Backpack();

                backpack.Movable = false;

                AddItem(backpack);
            }

            pack.Generate(this, backpack, m_Spawning);
        }

        public int GetLuckChanceForKiller()
        {
            if (m_Spawning) return 0;
            if (m_KillersLuck > -1) return m_KillersLuck; // already computed

            List<DamageStore> list = GetLootingRights(DamageEntries, HitsMax);

            DamageStore highest = null;

            for (int i = 0; i < list.Count; ++i)
            {
                DamageStore ds = list[i];

                if (ds.m_HasRight && (highest == null || ds.m_Damage > highest.m_Damage))
                    highest = ds;
            }

            if (highest == null)
                return 0;

            m_KillersLuck = highest.m_Mobile.Luck;
            return m_KillersLuck;
        }

        public bool PackArmor(int minLevel, int maxLevel)
        {
            return PackArmor(minLevel, maxLevel, 1.0);
        }

        public bool PackArmor(int minLevel, int maxLevel, double chance)
        {
            double random = Utility.RandomDouble();
            double luckBonus = GetLuckChanceForKiller() / 100.0;

            if (chance <= random + luckBonus)
                return false;

            if (luckBonus > Utility.RandomDouble())
                minLevel++;

            Cap(ref minLevel, 0, 5);
            Cap(ref maxLevel, 0, 5);

            BaseArmor armor = Loot.RandomArmorOrShield();
            if (armor == null)
                return false;

            armor.ProtectionLevel = (ArmorProtectionLevel)RandomMinMaxScaled(minLevel, maxLevel);
            armor.DurabilityLevel = (ArmorDurabilityLevel)RandomMinMaxScaled(minLevel, maxLevel);
            PackItem(armor);

            return true;
        }

        public static int RandomMinMaxScaled(int min, int max)
        {
            if (min == max)
                return min;

            if (min > max)
            {
                int hold = min;
                min = max;
                max = hold;
            }

            /* Example:
             *    min: 1
             *    max: 5
             *  count: 5
             * 
             * total = (5*5) + (4*4) + (3*3) + (2*2) + (1*1) = 25 + 16 + 9 + 4 + 1 = 55
             * 
             * chance for min+0 : 25/55 : 45.45%
             * chance for min+1 : 16/55 : 29.09%
             * chance for min+2 :  9/55 : 16.36%
             * chance for min+3 :  4/55 :  7.27%
             * chance for min+4 :  1/55 :  1.81%
             * 
             * 
             * 
             * 
             * 
             * 
             */
            /*  1 - 4:
                   res 1 53,39% chance
                   res 2 30,02% chance
                   res 3 13,24% chance
                   res 4 3,35% chance
               2 - 5:
                   res 2 53,42% chance
                   res 3 30,03% chance
                   res 4 13,38% chance
                   res 5 3,17% chance
               3 - 5:
                   res 3 64,78% chance
                   res 4 28,05% chance
                   res 5 7,17% chance
               4 - 5:
                   res 4 80,17% chance
                   res 5 19,83% chance
            */

            int count = max - min + 1;
            int total = 0, toAdd = count;

            for (int i = 0; i < count; ++i, --toAdd)
                total += toAdd * toAdd;

            int rand = Utility.Random(total);
            toAdd = count;

            int val = min;

            for (int i = 0; i < count; ++i, --toAdd, ++val)
            {
                rand -= toAdd * toAdd;

                if (rand < 0)
                    break;
            }

            return val;
        }

        public bool PackSlayer()
        {
            return PackSlayer(0.05);
        }

        public bool PackSlayer(double chance)
        {
            if (chance <= Utility.RandomDouble())
                return false;

            return true;
        }

        public bool PackWeapon(int minLevel, int maxLevel)
        {
            return PackWeapon(minLevel, maxLevel, 1.0);
        }

        public bool PackWeapon(int minLevel, int maxLevel, double chance)
        {
            double random = Utility.RandomDouble();
            double luckBonus = GetLuckChanceForKiller() / 100.0;

            if (chance <= random + luckBonus)
                return false;

            if (luckBonus > Utility.RandomDouble()) //chance to up the min level with luck
                minLevel++;

            Cap(ref minLevel, 0, 5);
            Cap(ref maxLevel, 0, 5);

            BaseWeapon weapon = Loot.RandomWeapon();
            if (weapon == null)
                return false;

            weapon.DamageLevel = (WeaponDamageLevel)RandomMinMaxScaled(minLevel, maxLevel);
            if ((int)weapon.DamageLevel >= 4 && Utility.RandomBool())
                weapon.DamageLevel = (WeaponDamageLevel)((int)weapon.DamageLevel - 1);

            weapon.AccuracyLevel = (WeaponAccuracyLevel)RandomMinMaxScaled(minLevel, maxLevel);
            weapon.DurabilityLevel = (WeaponDurabilityLevel)RandomMinMaxScaled(minLevel, maxLevel);

            PackItem(weapon);

            return true;
        }

        public void PackGold(int amount)
        {
            if (amount > 0)
                PackItem(new Gold(amount));
        }

        public void PackGold(int min, int max)
        {
            PackGold(Utility.RandomMinMax(min, max));
        }

        public void PackStatue(int min, int max)
        {
            PackStatue(Utility.RandomMinMax(min, max));
        }

        public void PackStatue(int amount)
        {
            for (int i = 0; i < amount; ++i)
                PackStatue();
        }

        public void PackStatue()
        {
            PackItem(Loot.RandomStatue());
        }

        public void PackGem()
        {
            PackGem(1);
        }

        public void PackGem(int min, int max)
        {
            PackGem(Utility.RandomMinMax(min, max));
        }

        public void PackGem(int amount)
        {
            if (amount <= 0)
                return;

            Item gem = Loot.RandomGem();

            gem.Amount = amount;

            PackItem(gem);
        }

        public void PackReg(int min, int max)
        {
            PackReg(Utility.RandomMinMax(min, max));
        }

        public void PackReg(int amount)
        {
            if (amount <= 0)
                return;

            Item reg = Loot.RandomReagent();

            reg.Amount = amount;

            PackItem(reg);
        }

        public void PackItem(Item item)
        {
            if (Summoned || item == null || NoKillAwards)
            {
                if (item != null)                
                    item.Delete();                

                return;
            }

            Container pack = Backpack;

            if (pack == null)
            {
                pack = new Backpack();

                pack.Movable = false;

                AddItem(pack);
            }

            if (!item.Stackable || !pack.TryDropItem(this, item, false)) // try stack
                pack.DropItem(item); // failed, drop it anyway
        }

        #endregion

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman)
            {
                Container pack = this.Backpack;

                if (pack != null)
                    pack.DisplayTo(from);
            }

            base.OnDoubleClick(from);
        }

        private class DeathAdderCharmTarget : Target
        {
            private BaseCreature m_Charmed;

            public DeathAdderCharmTarget(BaseCreature charmed)
                : base(-1, false, TargetFlags.Harmful)
            {
                m_Charmed = charmed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Charmed.Combatant != null || !from.CanBeHarmful(m_Charmed, false))
                    return;

                Mobile targ = targeted as Mobile;

                if (targ == null || !from.CanBeHarmful(targ, false))
                    return;

                from.RevealingAction();
                from.DoHarmful(targ, true);

                m_Charmed.Combatant = targ;

                if (m_Charmed.AIObject != null)
                    m_Charmed.AIObject.Action = ActionType.Combat;
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            //if ( MLQuestSystem.Enabled && CanGiveMLQuest )
            //    list.Add( 1072269 ); // Quest Giver

            if (Core.ML)
            {
                if (DisplayWeight)
                    list.Add(TotalWeight == 1 ? 1072788 : 1072789, TotalWeight.ToString()); // Weight: ~1_WEIGHT~ stones

                if (m_ControlOrder == OrderType.Guard)
                    list.Add(1080078); // guarding
            }

            if (Summoned && !IsAnimatedDead && !IsNecroFamiliar)
                list.Add(1049646); // (summoned)

            else if (Controlled && Commandable)
            {
                if (IsBonded)   //Intentional difference (showing ONLY bonded when bonded instead of bonded & tame)
                    list.Add(1049608); // (bonded)
                else
                    list.Add(502006); // (tame)                
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            string pacifiedMessage = "";
            string provokedMessage = "";
            string discordedMessage = "";

            if (BardPacified)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, BardEndTime, true, false, false, true, true);

                if (DateTime.UtcNow + TimeSpan.FromHours(1) <= BardEndTime || from != BardMaster)
                    pacifiedMessage = "pacified";
                else
                    pacifiedMessage = "pacified " + timeRemaining;
            }

            if (BardProvoked)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, BardEndTime, true, false, false, true, true);

                if (DateTime.UtcNow + TimeSpan.FromHours(1) <= BardEndTime || from != BardMaster)
                    provokedMessage = "provoked";

                else
                    provokedMessage = "provoked " + timeRemaining;
            }

            if (BardDiscorded)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DiscordEndTime, true, false, false, true, true);

                if (DateTime.UtcNow + TimeSpan.FromHours(1) <= DiscordEndTime || from != DiscordMaster)
                    discordedMessage = "discorded";

                else
                    discordedMessage = "discorded " + timeRemaining;
            }

            if (Controlled && Commandable)
            {
                if (from.NetState != null)
                {
                    if (pacifiedMessage != "" && discordedMessage == "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.PacifiedTextHue, false, "*" + pacifiedMessage + "*", from.NetState);

                    else if (provokedMessage != "" && discordedMessage == "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.ProvokedTextHue, false, "*" + provokedMessage + "*", from.NetState);

                    else if (pacifiedMessage != "" && discordedMessage != "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.PacifiedTextHue, false, "*" + pacifiedMessage + " " + discordedMessage + "*", from.NetState);

                    else if (provokedMessage != "" && discordedMessage != "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.ProvokedTextHue, false, "*" + provokedMessage + " " + discordedMessage + "*", from.NetState);

                    else if (discordedMessage != "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.DiscordedTextHue, false, "*" + discordedMessage + "*", from.NetState);
                }

                if (IsHenchman)
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "(follower)", from.NetState);
                    base.OnSingleClick(from);

                    return;
                }

                else
                {
                    int number;

                    if (Summoned)
                        number = 1049646; // (summoned)

                    else if (IsBonded)
                        number = 1049608; // (bonded)                

                    else
                    {
                        if (Region is UOACZRegion)
                        {
                            PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "(swarm)", from.NetState);
                            base.OnSingleClick(from);

                            return;
                        }

                        else
                            number = 502006; // (tame)
                    }

                    PrivateOverheadMessage(MessageType.Regular, 0x3B2, number, from.NetState);
                }
            }

            else
            {
                if (from.NetState != null)
                {
                    if (pacifiedMessage != "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.PacifiedTextHue, false, "*" + pacifiedMessage + "*", from.NetState);

                    if (provokedMessage != "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.ProvokedTextHue, false, "*" + provokedMessage + "*", from.NetState);

                    if (discordedMessage != "")
                        PrivateOverheadMessage(MessageType.Regular, BaseInstrument.DiscordedTextHue, false, "*" + discordedMessage + "*", from.NetState);
                }
            }

            base.OnSingleClick(from);
        }

        public virtual double TreasureMapChance { get { return TreasureMap.LootChance; } }
        public virtual int TreasureMapLevel
        {
            get
            {
                switch (LootTier)
                {
                    case Loot.LootTier.Two:
                        return 1;
                        break;
                    case Loot.LootTier.Three:
                        return 2;
                        break;
                    case Loot.LootTier.Four:
                        return 3;
                        break;
                    case Loot.LootTier.Five:
                        return 3;
                        break;
                    case Loot.LootTier.Six:
                        return 4;
                        break;
                    case Loot.LootTier.Seven:
                        return 5;
                        break;
                    case Loot.LootTier.Eight:
                        return 6;
                        break;
                }

                return -1;
            }
        }

        public virtual bool IgnoreYoungProtection { get { return false; } }

        public override bool OnBeforeDeath()
        {
            if (IsBoss() && !(Region is NewbieDungeonRegion) && !DiedByShipSinking && !IsLoHBoss() && !(Region is UOACZRegion))
            {
                double dungeonArmorChance = 1;
                double dungeonArmorUpgradeHammerChance = 1;
                double powerScrollChance = 1;

                if (m_AncientMysteryCreature)
                {
                    powerScrollChance = 0;
                    dungeonArmorChance = 1.0;
                    dungeonArmorUpgradeHammerChance = 0.5;
                }

                if (Utility.RandomDouble() < dungeonArmorChance)
                {
                    //PackItem(DungeonArmor.CreateDungeonArmor(DungeonEnum.None, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified));
                    //PackItem(new ArcaneDust());
                }

                //if (Utility.RandomDouble() < dungeonArmorUpgradeHammerChance)
                //PackItem(new DungeonArmorUpgradeHammer());

                if (Utility.RandomDouble() < powerScrollChance)
                    HandoutPowerScrolls(Utility.RandomMinMax(1, 3), true);

                PackCraftingComponent(10, 0.5);
                PackPrestigeScroll(5, 0.5);
                PackResearchMaterials(2, 0.5);
                PackSpellHueDeed(1, .15);

                PackItem(new TreasureMap(RandomMinMaxScaled(3, 6)));
            }

            else if (IsChamp() && !(Region is NewbieDungeonRegion) && !DiedByShipSinking && !(Region is UOACZRegion))
            {
                double dungeonArmorChance = 1;
                double dungeonArmorUpgradeHammerChance = .25;
                double powerScrollChance = .4;

                if (Utility.RandomDouble() < dungeonArmorChance)
                {
                    //PackItem(DungeonArmor.CreateDungeonArmor(DungeonEnum.None, DungeonArmor.ArmorTierEnum.Tier1, DungeonArmor.ArmorLocation.Unspecified));
                    //PackItem(new ArcaneDust());
                }

                //if (Utility.RandomDouble() < dungeonArmorUpgradeHammerChance)
                //PackItem(new DungeonArmorUpgradeHammer());

                if (Utility.RandomDouble() < powerScrollChance)
                    HandoutPowerScrolls(Utility.RandomMinMax(1, 2), false);

                PackCraftingComponent(10, 0.1);
                PackPrestigeScroll(5, 0.1);
                PackResearchMaterials(2, 0.1);
                PackSpellHueDeed(1, .03);

                PackItem(new TreasureMap(RandomMinMaxScaled(2, 6)));
            }

            if (m_Paragon && !NoKillAwards && !DiedByShipSinking && !(Region is UOACZRegion))
            {
            }

            if (!Summoned && !NoKillAwards && !IsBonded && TreasureMapLevel >= 0 && !DiedByShipSinking && !(Region is UOACZRegion))
            {
                if (m_Paragon && Paragon.ChestChance > Utility.RandomDouble())
                    PackItem(new ParagonChest(this.Name, TreasureMapLevel));

                else if ((Map == Map.Felucca || Map == Map.Trammel) && TreasureMap.LootChance >= Utility.RandomDouble())
                    PackItem(new TreasureMap(TreasureMapLevel, Map));
            }

            if (!Summoned && !NoKillAwards && !DiedByShipSinking && !(Region is UOACZRegion))
                GenerateLoot(false);

            if (IsAnimatedDead)
                Effects.SendLocationEffect(Location, Map, 0x3728, 13, 1, 0x461, 4);

            InhumanSpeech speechType = this.SpeechType;

            if (speechType != null)
                speechType.OnDeath(this);

            if (IsLoHBoss() && m_OnBeforeDeathCallback != null)
                m_OnBeforeDeathCallback();

            //Superpredator or Predator eating Predator/Prey
            BaseCreature bc_Killer = LastKiller as BaseCreature;

            if (bc_Killer != null)
            {
                if (!(bc_Killer.Controlled && bc_Killer.ControlMaster is PlayerMobile) && bc_Killer.Alive && (bc_Killer.SuperPredator && (Predator || Prey)) || (bc_Killer.Predator && Prey))
                {
                    if (bc_Killer.GetDistanceToSqrt(this) <= 2)
                    {
                        //Stay Still for 5 seconds
                        SpecialAbilities.EntangleSpecialAbility(1, this, bc_Killer, 1.0, 5, bc_Killer.GetIdleSound(), false, "", "", "-1");

                        bc_Killer.Say("*feeds*");
                        bc_Killer.AnimateIdle();

                        for (int a = 0; a < 4; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate()
                            {
                                if (bc_Killer != null)
                                {
                                    if (bc_Killer.Alive)
                                        bc_Killer.PlaySound(Utility.RandomList(0x133, 0x4cd, 0x5ab, 0x339));
                                }
                            });
                        }
                    }

                    bc_Killer.Hunger = Food.MaxHunger;
                }
            }

            return base.OnBeforeDeath();
        }

        protected void HandoutPowerScrolls(int amount, bool boss)
        {
            var damagers = GetLootingRights(DamageEntries, HitsMax);

            for (int i = 0; i < amount && damagers.Count > 0; i++)
            {
                int index = Utility.RandomMinMax(0, Math.Min(damagers.Count - 1, amount + 2));

                var damager = damagers[index];

                if (damager != null && damager.m_Mobile != null)
                {
                    if (damager.m_Mobile is PlayerMobile && ((PlayerMobile)damager.m_Mobile).Young)
                    {
                        i--;
                        continue;
                    }

                    PowerScroll powerscroll = null;

                    if (!boss)
                        powerscroll = PowerScroll.CreateRandom(5, 10);
                    else
                    {
                        if (Utility.RandomDouble() < 0.25)
                            powerscroll = PowerScroll.CreateRandom(20);
                        else
                            powerscroll = PowerScroll.CreateRandom(15);
                    }

                    damager.m_Mobile.SendLocalizedMessage(1049524); // You have received a scroll of power!

                    if (damager.m_Mobile.Alive)
                    {
                        if (!damager.m_Mobile.AddToBackpack(powerscroll))
                            powerscroll.MoveToWorld(damager.m_Mobile.Location, damager.m_Mobile.Map);
                    }

                    else
                    {
                        if (damager.m_Mobile.Corpse != null && !damager.m_Mobile.Corpse.Deleted)
                            damager.m_Mobile.Corpse.DropItem(powerscroll);
                    }
                }
            }
        }       

        public int ComputeBonusDamage(List<DamageEntry> list, Mobile m)
        {
            int bonus = 0;

            for (int i = list.Count - 1; i >= 0; --i)
            {
                DamageEntry de = list[i];

                if (de.Damager == m || !(de.Damager is BaseCreature))
                    continue;

                BaseCreature bc = (BaseCreature)de.Damager;
                Mobile master = null;

                master = bc.GetMaster();

                if (master == m)
                    bonus += de.DamageGiven;
            }

            return bonus;
        }
        
        private class FKEntry
        {
            public Mobile m_Mobile;
            public int m_Damage;

            public FKEntry(Mobile m, int damage)
            {
                m_Mobile = m;
                m_Damage = damage;
            }
        }

        public static List<DamageStore> GetLootingRights(List<DamageEntry> damageEntries, int hitsMax)
        {
            return GetLootingRights(damageEntries, hitsMax, false);
        }

        public static List<DamageStore> GetLootingRights(List<DamageEntry> damageEntries, int hitsMax, bool partyAsIndividual)
        {
            List<DamageStore> rights = new List<DamageStore>();

            for (int i = damageEntries.Count - 1; i >= 0; --i)
            {
                if (i >= damageEntries.Count)
                    continue;

                DamageEntry de = damageEntries[i];

                if (de.HasExpired)
                {
                    damageEntries.RemoveAt(i);
                    continue;
                }

                int damage = de.DamageGiven;

                List<DamageEntry> respList = de.Responsible;

                if (respList != null)
                {
                    for (int j = 0; j < respList.Count; ++j)
                    {
                        DamageEntry subEntry = respList[j];
                        Mobile master = subEntry.Damager;

                        if (master == null || master.Deleted || !master.Player)
                            continue;

                        bool needNewSubEntry = true;

                        for (int k = 0; needNewSubEntry && k < rights.Count; ++k)
                        {
                            DamageStore ds = rights[k];

                            if (ds.m_Mobile == master)
                            {
                                ds.m_Damage += subEntry.DamageGiven;
                                needNewSubEntry = false;
                            }
                        }

                        if (needNewSubEntry)
                            rights.Add(new DamageStore(master, subEntry.DamageGiven));

                        damage -= subEntry.DamageGiven;
                    }
                }

                Mobile m = de.Damager;

                if (m == null || m.Deleted || !m.Player)
                    continue;

                if (damage <= 0)
                    continue;

                bool needNewEntry = true;

                for (int j = 0; needNewEntry && j < rights.Count; ++j)
                {
                    DamageStore ds = rights[j];

                    if (ds.m_Mobile == m)
                    {
                        ds.m_Damage += damage;
                        needNewEntry = false;
                    }
                }

                if (needNewEntry)
                    rights.Add(new DamageStore(m, damage));
            }

            if (rights.Count > 0)
            {
                rights[0].m_Damage = (int)(rights[0].m_Damage * 1.25);  //This would be the first valid person attacking it.  Gets a 25% bonus.  Per 1/19/07 Five on Friday

                if (rights.Count > 1)
                    rights.Sort(); //Sort by damage

                int topDamage = rights[0].m_Damage;
                int minDamage;

                if (hitsMax >= 3000)
                    minDamage = (int)((double)topDamage / 8); //16
                else if (hitsMax >= 1000)
                    minDamage = (int)((double)topDamage / 4); //8
                else if (hitsMax >= 200)
                    minDamage = (int)((double)topDamage / 2); //4
                else
                    minDamage = (int)((double)topDamage / 1.5); //2

                for (int i = 0; i < rights.Count; ++i)
                {
                    DamageStore ds = rights[i];

                    ds.m_HasRight = (ds.m_Damage >= minDamage);
                }
            }

            return rights;
        }

        #region Mondain's Legacy
        public virtual bool GivesMLMinorArtifact { get { return false; } }
        #endregion

        public virtual void OnKilledBy(Mobile mob)
        {
            #region Mondain's Legacy
            if (GivesMLMinorArtifact)
            {
                if (MondainsLegacy.CheckArtifactChance(mob, this))
                    MondainsLegacy.GiveArtifactTo(mob);
            }
            #endregion
            else if (m_Paragon)
            {
                if (Paragon.CheckArtifactChance(mob, this))
                    Paragon.GiveArtifactTo(mob);
            }
        }

        public override void OnDeath(Container c)
        {
            if (ResurrectionsRemaining == 0)
                IsBonded = false;            
                        
            BandageContext bandageContext = BandageContext.GetContext(this);

            if (bandageContext != null)
                bandageContext.StopHeal();

            int totalDamage = 0;

            Dictionary<PlayerMobile, int> damageInflicted = new Dictionary<PlayerMobile, int>();

            List<PlayerMobile> passiveTamingSkillGainPlayers = new List<PlayerMobile>();

            //Determine Total Damaged Inflicted and Per Player
            foreach (DamageEntry entry in DamageEntries)
            {
                if (!entry.HasExpired)
                {                    
                    BaseCreature bc_Creature = entry.Damager as BaseCreature;
                    PlayerMobile playerDamager = entry.Damager as PlayerMobile;
                    PlayerMobile creatureOwner = null;

                    bool passiveTamingSkillGainValid = false;
                   
                    if (bc_Creature != null)
                    {
                        if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile && bc_Creature.Tameable)
                        {
                            creatureOwner = bc_Creature.ControlMaster as PlayerMobile;

                            //Experience Gain
                            if (bc_Creature.AllowExperienceGain(this, creatureOwner))
                            {
                                double experienceChance = ExperienceChance();
                                int experienceValue = ExperienceValue();

                                if (Utility.RandomDouble() <= experienceChance)
                                    bc_Creature.Experience += experienceValue;
                            }                               

                            //Passive Taming Skill Gain Allowed
                            if (bc_Creature.AllowPassiveTamingSkillGain(this, creatureOwner))
                            {
                                if (!passiveTamingSkillGainPlayers.Contains(creatureOwner))
                                    passiveTamingSkillGainPlayers.Add(creatureOwner);
                            }

                            bc_Creature.CreaturesKilled++;
                        }
                    }

                    totalDamage += entry.DamageGiven;

                    if (playerDamager != null)
                    {
                        if (damageInflicted.ContainsKey(playerDamager))
                            damageInflicted[playerDamager] += entry.DamageGiven;

                        else
                            damageInflicted.Add(playerDamager, entry.DamageGiven);
                    }

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
                            if (damageInflicted.ContainsKey(creatureOwner))
                                damageInflicted[creatureOwner] += entry.DamageGiven;

                            else
                                damageInflicted.Add(creatureOwner, entry.DamageGiven);
                        }
                    }
                }
            }

            if (IsBonded)
            {
                int sound = this.GetDeathSound();

                if (sound >= 0)
                    Effects.PlaySound(this, this.Map, sound);

                Warmode = false;

                Poison = null;
                Combatant = null;

                Hits = 0;
                Stam = 0;
                Mana = 0;

                IsDeadPet = true;
                ControlTarget = ControlMaster;
                ControlOrder = OrderType.Follow;

                ProcessDeltaQueue();
                SendIncomingPacket();
                SendIncomingPacket();

                List<AggressorInfo> aggressors = this.Aggressors;

                for (int i = 0; i < aggressors.Count; ++i)
                {
                    AggressorInfo info = aggressors[i];

                    if (info.Attacker.Combatant == this)
                        info.Attacker.Combatant = null;
                }

                List<AggressorInfo> aggressed = this.Aggressed;

                for (int i = 0; i < aggressed.Count; ++i)
                {
                    AggressorInfo info = aggressed[i];

                    if (info.Defender.Combatant == this)
                        info.Defender.Combatant = null;
                }

                CheckStatTimers();
            }

            else
            {
                if (!Summoned && !m_NoKillAwards && !DiedByShipSinking)
                {
                    int totalFame = ComputedFame / 100;
                    int totalKarma = -Karma / 100;

                    List<DamageStore> list = GetLootingRights(this.DamageEntries, this.HitsMax);

                    bool givenQuestKill = false;                   

                    for (int i = 0; i < list.Count; ++i)
                    {
                        DamageStore ds = list[i];

                        if (!ds.m_HasRight)
                            continue;

                        FameKarmaTitles.AwardFame(ds.m_Mobile, totalFame, true);
                        FameKarmaTitles.AwardKarma(ds.m_Mobile, totalKarma, true);

                        XmlQuest.RegisterKill(this, ds.m_Mobile);

                        OnKilledBy(ds.m_Mobile);

                        if (givenQuestKill)
                            continue;
                    }
                }

                //Passive Taming
                /*
                BaseCreature bc_TamedCreature = from as BaseCreature;

                if (bc_TamedCreature != null && !(Region is UOACZRegion))
                {
                    PlayerMobile pm_Tamer = bc_TamedCreature.ControlMaster as PlayerMobile;

                    if (pm_Tamer != null && bc_TamedCreature.Controlled && !bc_TamedCreature.Summoned)
                    {
                        bool passiveTamingValid = true;

                        if (bc_TamedCreature.IsHenchman)
                            passiveTamingValid = false;

                        if (NoKillAwards || Summoned || ControlMaster is PlayerMobile)
                            passiveTamingValid = false;

                        if (!bc_TamedCreature.AllowPassiveTamingSkillGain(pm_Tamer))
                            passiveTamingValid = false;

                        if (pm_Tamer.m_PassiveSkillGainRemaining <= 0)
                            passiveTamingValid = false;

                        TimeSpan PassiveTamingSkillGainDelay = TimeSpan.FromMinutes(pm_Tamer.Skills[SkillName.AnimalTaming].Value / 2);

                        if (DateTime.UtcNow < pm_Tamer.m_LastPassiveTamingSkillGain + PassiveTamingSkillGainDelay)
                            passiveTamingValid = false;

                        if (passiveTamingValid)
                        {
                            Skill skill = from.Skills[SkillName.AnimalTaming];

                            pm_Tamer.m_PassiveSkillGainRemaining -= .1;

                            SkillCheck.GainSkill(pm_Tamer, skill, 1);

                            pm_Tamer.m_LastPassiveTamingSkillGain = DateTime.UtcNow;
                            pm_Tamer.m_LastPassiveTamingSkillAttacked = this;
                        }

                        double randomResult = Utility.RandomDouble();

                        //Experience Gain                   
                        if (pm_Tamer.m_LastPassiveTamingSkillAttacked != this && !(Controlled && ControlMaster is PlayerMobile) && !Summoned && !NoKillAwards && bc_TamedCreature.m_NextExperienceGain < DateTime.UtcNow && bc_TamedCreature.ExperienceLevel < BaseCreature.MaxExperienceLevel)
                        {
                            double nextExperienceGain = (double)MinExpGainActiveDelay;

                            double gainChance = .2;

                            if (randomResult <= gainChance)
                            {
                                bc_TamedCreature.Experience++;
                                bc_TamedCreature.m_NextExperienceGain = DateTime.UtcNow + TimeSpan.FromMinutes(nextExperienceGain);

                                pm_Tamer.m_LastPassiveExpAttacked = this;
                            }
                        }

                        //Passive Experience Gain: Same Creature
                        else if (bc_TamedCreature.m_NextExperienceGain < DateTime.UtcNow && !NoKillAwards && bc_TamedCreature.ExperienceLevel < BaseCreature.MaxExperienceLevel)
                        {
                            double gainChance = .01;

                            if (randomResult <= gainChance)
                            {
                                bc_TamedCreature.Experience++;
                                bc_TamedCreature.m_NextExperienceGain = DateTime.UtcNow + TimeSpan.FromMinutes(MinExpGainMacroDelay);

                                pm_Tamer.m_LastPassiveExpAttacked = this;
                            }
                        }

                    }
                }
                */

                //Rewards
                if (!(ControlMaster is PlayerMobile) && !DiedByShipSinking)
                {        
                    foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
                    {
                        PlayerMobile playerDamager = pair.Key;

                        if (playerDamager == null) continue;
                        if (playerDamager.Deleted) continue;

                        double damagePercent = (double)pair.Value / (double)totalDamage;

                        //Passive Taming Skill Gain
                        if (passiveTamingSkillGainPlayers.Contains(playerDamager))
                        {
                            double passiveTamingSkillGainChance = PassiveTamingSkillGainChance(playerDamager);

                            if (Utility.RandomDouble() <= passiveTamingSkillGainChance)
                            {                               
                                Skill skill = playerDamager.Skills[SkillName.AnimalTaming];

                                playerDamager.m_PassiveSkillGainRemaining -= .1;

                                SkillCheck.GainSkill(playerDamager, skill, 1);

                                playerDamager.m_LastPassiveTamingSkillGain = DateTime.UtcNow;
                            }
                        }

                        //Arcane Item Experience Handling

                        //Monster Hunter Society
                        MHSCreatures.CreatureKilled(this, playerDamager, damagePercent, TakenDamageFromPoison, TakenDamageFromCreature);
                        
                        //Doubloons
                        if (DoubloonValue > 0)
                        {
                            double doubloonAmount = 0;
                            bool doubloonsValid = false;

                            bool validBoat = false;

                            if (BoatOccupied != null)
                            {
                                if (!BoatOccupied.Deleted)
                                    validBoat = true;
                            }

                            if (validBoat || IsOceanCreature)
                            {
                                if (DoubloonValue > 0)
                                {
                                    doubloonAmount = (double)DoubloonValue;

                                    if (damagePercent < .5)
                                        doubloonAmount *= .5;

                                    doubloonsValid = true;
                                }
                            }

                            if (doubloonsValid)
                            {
                                if (doubloonAmount < 1)
                                    doubloonAmount = 1;

                                int finalDoubloonValue = (int)doubloonAmount;

                                if (finalDoubloonValue < 1)
                                    finalDoubloonValue = 1;

                                playerDamager.PirateScore += finalDoubloonValue;

                                if (Banker.DepositUniqueCurrency(playerDamager, typeof(Doubloon), finalDoubloonValue))
                                {
                                    Doubloon doubloonPile = new Doubloon(finalDoubloonValue);
                                    playerDamager.SendSound(doubloonPile.GetDropSound());
                                    doubloonPile.Delete();

                                    playerDamager.SendMessage("You've earned " + finalDoubloonValue.ToString() + " doubloons. They have been placed in your bank box.");
                                }

                                else
                                    playerDamager.SendMessage("You've earned doubloons but there was no available space to place them in your bank box.");
                            }
                        }

                        //Title Rewards
                        if (TitleReward != null && TitleReward != "")
                        {
                            /*
                            if (Utility.RandomDouble() < damagePercent && !playerDamager.TitlesPrefix.Contains(TitleReward))
                            {
                                playerDamager.TitlesPrefix.Add(TitleReward);
                                playerDamager.SendMessage("The title of " + TitleReward + " has now been added to your list of selectable titles.");

                                playerDamager.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
                                playerDamager.PlaySound(0x1F7);
                            }
                            */
                        }
                    }
                }

                if (m_StamFreeMoveAuraTimer != null)
                {
                    m_StamFreeMoveAuraTimer.Stop();
                    m_StamFreeMoveAuraTimer = null;
                }

                base.OnDeath(c);

                if (DeleteCorpseOnDeath)                
                    c.Delete();                
            }
        }

        [CommandProperty(AccessLevel.Counselor)]
        public int ComputedFame
        {
            get
            {
                return (int)Math.Min(Difficulty * 800, 25000);
            }
        }

        private List<PlayerMobile> ValidSkillScrollRecipients()
        {
            var validPlayers = new List<PlayerMobile>();
            Mobile highest = FindMostTotalDamger(false);

            if (highest is BaseCreature)
            {
                var creature = highest as BaseCreature;
                if (creature.Controlled && creature.ControlMaster != null)
                    highest = creature.ControlMaster;
                else if (creature.BardProvoked && creature.BardMaster != null)
                    highest = creature.BardMaster;
                else if (creature.Summoned && creature.SummonMaster != null)
                    highest = creature.SummonMaster;
            }

            if (highest != null && highest is PlayerMobile)
            {
                validPlayers.Add(highest as PlayerMobile);
                Server.Engines.PartySystem.Party p = Server.Engines.PartySystem.Party.Get(highest);

                if (p != null && p.Contains(highest))
                {
                    for (int i = 0; i < p.Members.Count; i++)
                    {
                        var pmi = p[i];

                        if (pmi != null && pmi.Mobile is PlayerMobile)
                        {
                            var mob = pmi.Mobile as PlayerMobile;
                            if (mob != null && InRange(mob.Location, 18) && !validPlayers.Contains(mob))
                                validPlayers.Add(mob as PlayerMobile);
                        }
                    }
                }
            }

            return validPlayers;
        }

        /* To save on cpu usage, RunUO creatures only reacquire creatures under the following circumstances:
         *  - 10 seconds have elapsed since the last time it tried
         *  - The creature was attacked
         *  - Some creatures, like dragons, will reacquire when they see someone move
         * 
         * This functionality appears to be implemented on OSI as well
         */

        private long m_NextReacquireTime;

        public long NextReacquireTime { get { return m_NextReacquireTime; } set { m_NextReacquireTime = value; } }

        public virtual TimeSpan ReacquireDelay { get { return TimeSpan.FromSeconds(10.0); } }
        public virtual bool ReacquireOnMovement { get { return false; } }
        public virtual bool AcquireOnApproach { get { return false; } } //m_Paragon
        public virtual int AcquireOnApproachRange { get { return 10; } }

        public override void OnDelete()
        {
            Mobile m = m_ControlMaster;

            SetControlMaster(null);
            SummonMaster = null;
            DoingBandage = false;

            base.OnDelete();

            if (m != null)
                m.InvalidateProperties();
        }

        public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
        {
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

        public override bool CanBeRenamedBy(Mobile from)
        {
            bool ret = base.CanBeRenamedBy(from);

            if (Controlled && from == ControlMaster && !(from.Region is UOACZRegion) && !from.Region.IsPartOf(typeof(Jail)))
                ret = true;

            return ret;
        }

        public bool SetControlMaster(Mobile m)
        {
            if (m == null)
            {
                ControlMaster = null;
                Controlled = false;
                ControlTarget = null;
                ControlOrder = OrderType.None;
                
                //TEST: GUILD
                //Guild = null;

                Delta(MobileDelta.Noto);
            }

            else
            {
                ISpawner se = this.Spawner;

                if (se != null && se.UnlinkOnTaming)
                {
                    this.Spawner.Remove(this);
                    this.Spawner = null;
                }

                if (m.Followers + ControlSlots > m.FollowersMax)
                {
                    m.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                    return false;
                }

                CurrentWaypoint = null; //so tamed animals don't try to go back

                ControlMaster = m;
                Controlled = true;
                ControlTarget = null;

                BardPacified = false;
                BardProvoked = false;
                BardMaster = null;

                Warmode = false;
                Combatant = null;

                if (ControlMaster.Player)
                {
                    if (AIObject != null)
                    {
                        ControlTarget = ControlMaster;
                        ControlOrder = OrderType.Follow;

                        AIObject.DoOrderFollow();

                        if (AIObject.m_Timer != null)
                            AIObject.m_Timer.Priority = TimerPriority.FiftyMS;
                    }

                    else
                        ControlOrder = OrderType.Stop;
                }

                else
                {
                    ControlOrder = OrderType.Guard;

                    if (AIObject != null)
                    {
                        AIObject.DoOrderGuard();
                    }
                }

                //TEST: GUILD
                //Guild = null;

                if (m_DeleteTimer != null)
                {
                    m_DeleteTimer.Stop();
                    m_DeleteTimer = null;
                }

                Delta(MobileDelta.Noto);
            }

            InvalidateProperties();

            return true;
        }

        public override void OnRegionChange(Region Old, Region New)
        {
            base.OnRegionChange(Old, New);

            if (this.Controlled)
            {
                SpawnEntry se = this.Spawner as SpawnEntry;

                if (se != null && !se.UnlinkOnTaming && (New == null || !New.AcceptsSpawnsFrom(se.Region)))
                {
                    this.Spawner.Remove(this);
                    this.Spawner = null;
                }
            }
        }

        private static bool m_Summoning;

        public static bool Summoning
        {
            get { return m_Summoning; }
            set { m_Summoning = value; }
        }

        public static SlayerGroupType GetRandomSlayerType()
        {
            SlayerGroupType slayerGroupType = (SlayerGroupType)Utility.RandomMinMax(1, Enum.GetNames(typeof(SlayerGroupType)).Length - 1);

            return slayerGroupType;
        }

        public static bool Summon(BaseCreature creature, Mobile caster, Point3D p, int sound, TimeSpan duration)
        {
            return Summon(creature, true, caster, p, sound, duration);
        }

        public static bool Summon(BaseCreature creature, bool controlled, Mobile caster, Point3D p, int sound, TimeSpan duration)
        {
            if (caster.Followers + creature.ControlSlots > caster.FollowersMax)
            {
                caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                creature.Delete();
                return false;
            }

            m_Summoning = true;

            if (controlled)
                creature.SetControlMaster(caster);

            creature.RangeHome = 10;
            creature.Summoned = true;
            creature.SummonMaster = caster;

            Container pack = creature.Backpack;

            if (pack != null)
            {
                for (int i = pack.Items.Count - 1; i >= 0; --i)
                {
                    if (i >= pack.Items.Count)
                        continue;
                    
                    pack.Items[i].Delete();
                }
            }

            new UnsummonTimer(caster, creature, duration).Start();
            creature.m_SummonEnd = DateTime.UtcNow + duration;

            creature.MoveToWorld(p, caster.Map);

            Effects.PlaySound(p, creature.Map, sound);

            m_Summoning = false;

            return true;
        }

        private static bool EnableRummaging = true;

        private const double ChanceToRummage = 0.5; // 50%

        private const double MinutesToNextRummageMin = 1.0;
        private const double MinutesToNextRummageMax = 4.0;

        private const double MinutesToNextChanceMin = 0.25;
        private const double MinutesToNextChanceMax = 0.75;

        private long m_NextRummageTime;

        public virtual bool IsDispellable { get { return Summoned && !IsAnimatedDead; } }

        #region Healing

        public virtual double MinHealDelay { get { return 2.0; } }
        public virtual double HealScalar { get { return 1.0; } }
        public virtual bool CanHeal { get { return false; } }
        public virtual bool CanHealOwner { get { return false; } }

        public double MaxHealDelay
        {
            get
            {
                if (ControlMaster != null)
                {
                    double max = ControlMaster.Hits / 10;

                    if (max > 10)
                        max = 10;
                    if (max < 1)
                        max = 1;

                    return max;
                }
                else
                    return 7;
            }
        }

        private long m_NextHealTime = Core.TickCount;
        private Timer m_HealTimer = null;

        public virtual void HealStart()
        {
            if (!Alive)
                return;

            if (CanHeal && Hits != HitsMax)
            {
                RevealingAction();

                double seconds = 10 - Dex / 12;

                m_HealTimer = Timer.DelayCall(TimeSpan.FromSeconds(seconds), new TimerStateCallback(Heal_Callback), this);
            }
            else if (CanHealOwner && ControlMaster != null && ControlMaster.Hits < ControlMaster.HitsMax && InRange(ControlMaster, 2))
            {
                ControlMaster.RevealingAction();

                double seconds = 10 - Dex / 15;
                double resDelay = (ControlMaster.Alive ? 0.0 : 5.0);

                seconds += resDelay;

                ControlMaster.SendLocalizedMessage(1008078, false, Name); //  : Attempting to heal you.

                m_HealTimer = Timer.DelayCall(TimeSpan.FromSeconds(seconds), new TimerStateCallback(Heal_Callback), ControlMaster);
            }
        }

        private void Heal_Callback(object state)
        {
            if (state is Mobile)
                Heal((Mobile)state);
        }

        public virtual void Heal(Mobile patient)
        {
            if (!Alive || !patient.Alive || !InRange(patient, 2))
            {
            }
            else if (patient.Poisoned)
            {
                double healing = Skills.Healing.Value;
                double anatomy = Skills.Anatomy.Value;
                double chance = (healing - 30.0) / 50.0 - patient.Poison.Level * 0.1;

                if ((healing >= 60.0 && anatomy >= 60.0) && chance > Utility.RandomDouble())
                {
                    if (patient.CurePoison(this))
                    {
                        patient.SendLocalizedMessage(1010059); // You have been cured of all poisons.                       
                        patient.PlaySound(0x57);

                        CheckSkill(SkillName.Healing, 0.0, 100.0, 1.0);
                        CheckSkill(SkillName.Anatomy, 0.0, 100.0, 1.0);
                    }
                }
            }
            else
            {
                double healing = Skills.Healing.Value;
                double anatomy = Skills.Anatomy.Value;
                double chance = (healing + 10.0) / 100.0;

                if (chance > Utility.RandomDouble())
                {
                    double min, max;

                    min = (anatomy / 10.0) + (healing / 6.0) + 4.0;
                    max = (anatomy / 8.0) + (healing / 3.0) + 4.0;

                    if (patient == this)
                        max += 10;

                    double toHeal = min + (Utility.RandomDouble() * (max - min));

                    toHeal *= HealScalar;

                    patient.Heal((int)toHeal);
                    patient.PlaySound(0x57);

                    CheckSkill(SkillName.Healing, 0.0, 100.0, 1.0);
                    CheckSkill(SkillName.Anatomy, 0.0, 100.0, 1.0);
                }
            }

            if (m_HealTimer != null)
                m_HealTimer.Stop();

            m_HealTimer = null;

            m_NextHealTime = Core.TickCount + (long)TimeSpan.FromSeconds(MinHealDelay + (Utility.RandomDouble() * MaxHealDelay)).TotalMilliseconds;
        }
        #endregion

        public virtual void OnThink()
        {
            long tc = Core.TickCount;

            if (Hits == HitsMax)
            {
                m_TakenDamageFromPoison = false;
                m_TakenDamageFromCreature = false;
            }

            if (EnableRummaging && CanRummageCorpses && !Summoned && !Controlled && tc >= m_NextRummageTime)
            {
                double min, max;

                if (ChanceToRummage > Utility.RandomDouble() && Rummage())
                {
                    min = MinutesToNextRummageMin;
                    max = MinutesToNextRummageMax;
                }

                else
                {
                    min = MinutesToNextChanceMin;
                    max = MinutesToNextChanceMax;
                }

                double delay = min + (Utility.RandomDouble() * (max - min));
                m_NextRummageTime = tc + (long)TimeSpan.FromMinutes(delay).TotalMilliseconds;
            }

            bool forceMelee = false;

            Custom.BaseHenchman bc_Henchman = this as Custom.BaseHenchman;

            //Force Henchmen to Return to Melee Weapons When Not on Boats and Ranged Isn't Their Primary Weapon

            if (CanSwitchWeapons && Combatant != null && bc_Henchman != null && BoatOccupied == null && Backpack != null && !BardPacified)
            {
                if (bc_Henchman.HenchmanHumanoid)
                {
                    if (!bc_Henchman.IsRangedPrimary)
                        forceMelee = true;
                }
            }

            if (forceMelee)
            {
                BaseWeapon weapon = this.Weapon as BaseWeapon;

                if (weapon != null)
                {
                    if (weapon is BaseRanged)
                    {
                        Item meleeWeapon = Backpack.FindItemByType(typeof(BaseMeleeWeapon));
                        Item shield = Backpack.FindItemByType(typeof(BaseShield));

                        if (meleeWeapon != null)
                        {
                            Item rightHand = FindItemOnLayer(Layer.TwoHanded);

                            if (rightHand != null)
                                Backpack.DropItem(rightHand);

                            EquipItem(meleeWeapon);

                            if (meleeWeapon.Layer != Layer.TwoHanded && shield != null)
                                EquipItem(shield);

                            m_NextWeaponChangeAllowed = DateTime.UtcNow + NextWeaponChangeDelay;
                        }
                    }
                }
            }

            if (!forceMelee && CanSwitchWeapons && Combatant != null && m_NextWeaponChangeAllowed < DateTime.UtcNow && Backpack != null && !BardPacified)
            {
                if (Backpack.Deleted)
                    return;

                BaseWeapon weapon = this.Weapon as BaseWeapon;

                double enemydistance = this.GetDistanceToSqrt(Combatant);

                if (weapon != null)
                {
                    if (weapon is BaseMeleeWeapon && (enemydistance >= WeaponSwitchRange))
                    {
                        Item rangedWeapon = Backpack.FindItemByType(typeof(BaseRanged));

                        if (rangedWeapon != null)
                        {
                            Item leftHand = FindItemOnLayer(Layer.OneHanded);

                            if (leftHand != null)
                                Backpack.DropItem(leftHand);

                            Item leftHandExtra = FindItemOnLayer(Layer.FirstValid);

                            if (leftHandExtra != null)
                                Backpack.DropItem(leftHandExtra);

                            Item rightHand = FindItemOnLayer(Layer.TwoHanded);

                            if (rightHand != null)
                                Backpack.DropItem(rightHand);

                            EquipItem(rangedWeapon);

                            m_NextWeaponChangeAllowed = DateTime.UtcNow + NextWeaponChangeDelay;
                        }
                    }

                    else if (weapon is BaseRanged && (enemydistance < WeaponSwitchRange))
                    {
                        Item meleeWeapon = Backpack.FindItemByType(typeof(BaseMeleeWeapon));
                        Item shield = Backpack.FindItemByType(typeof(BaseShield));

                        if (meleeWeapon != null)
                        {
                            Item rightHand = FindItemOnLayer(Layer.TwoHanded);

                            if (rightHand != null)
                                Backpack.DropItem(rightHand);

                            EquipItem(meleeWeapon);

                            if (meleeWeapon.Layer != Layer.TwoHanded && shield != null)
                                EquipItem(shield);

                            m_NextWeaponChangeAllowed = DateTime.UtcNow + NextWeaponChangeDelay;
                        }
                    }
                }
            }

            else if (!forceMelee && CanSwitchWeapons && Combatant == null && m_NextWeaponChangeAllowed < DateTime.UtcNow && Backpack != null && !BardPacified)
            {
                if (Backpack.Deleted)
                    return;

                BaseWeapon weapon = this.Weapon as BaseWeapon;

                if (weapon != null)
                {
                    //Return to Ranged When Out of Combat if Ranged Primary
                    if (IsRangedPrimary)
                    {
                        if (weapon is BaseMeleeWeapon)
                        {
                            Item rangedWeapon = Backpack.FindItemByType(typeof(BaseRanged));

                            if (rangedWeapon != null)
                            {
                                Item leftHand = FindItemOnLayer(Layer.OneHanded);

                                if (leftHand != null)
                                    Backpack.DropItem(leftHand);

                                Item leftHandExtra = FindItemOnLayer(Layer.FirstValid);

                                if (leftHandExtra != null)
                                    Backpack.DropItem(leftHandExtra);

                                Item rightHand = FindItemOnLayer(Layer.TwoHanded);

                                if (rightHand != null)
                                    Backpack.DropItem(rightHand);

                                EquipItem(rangedWeapon);

                                m_NextWeaponChangeAllowed = DateTime.UtcNow + NextWeaponChangeDelay;
                            }
                        }
                    }

                    //Return to Melee When Out of Combat
                    else
                    {
                        if (weapon is BaseRanged)
                        {
                            Item meleeWeapon = Backpack.FindItemByType(typeof(BaseMeleeWeapon));
                            Item shield = Backpack.FindItemByType(typeof(BaseShield));

                            if (meleeWeapon != null)
                            {
                                Item rightHand = FindItemOnLayer(Layer.TwoHanded);

                                if (rightHand != null)
                                    Backpack.DropItem(rightHand);

                                EquipItem(meleeWeapon);

                                if (meleeWeapon.Layer != Layer.TwoHanded && shield != null)
                                    EquipItem(shield);

                                m_NextWeaponChangeAllowed = DateTime.UtcNow + NextWeaponChangeDelay;
                            }
                        }
                    }
                }
            }

            //Fished Up Creature Forced Deletion
            if (m_WasFishedUp && CreationTime + TimeSpan.FromMinutes(60) < DateTime.UtcNow && Hits == HitsMax && Combatant == null)
                Delete();
        }

        public virtual bool Rummage()
        {
            Corpse toRummage = null;

            IPooledEnumerable eable = this.GetItemsInRange(2);
            foreach (Item item in eable)
            {
                if (item is Corpse && item.Items.Count > 0)
                {
                    toRummage = (Corpse)item;
                    break;
                }
            }
            eable.Free();

            if (toRummage == null)
                return false;

            Container pack = this.Backpack;

            if (pack == null)
                return false;

            List<Item> items = toRummage.Items;

            bool rejected;
            LRReason reason;

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[Utility.Random(items.Count)];

                Lift(item, item.Amount, out rejected, out reason);

                if (!rejected && Drop(this, new Point3D(-1, -1, 0)))
                {
                    // *rummages through a corpse and takes an item*
                    PublicOverheadMessage(MessageType.Emote, 0x3B2, 1008086);
                    //TODO: Instancing of Rummaged stuff.
                    return true;
                }
            }

            return false;
        }

        public Mobile GetMaster()
        {
            if (Controlled && ControlMaster != null)
                return ControlMaster;

            else if (Summoned && SummonMaster != null)
                return SummonMaster;

            return null;
        }

        public PlayerMobile GetPlayerMaster()
        {
            if (BardProvoked && BardMaster != null && BardMaster is PlayerMobile)
                return BardMaster as PlayerMobile;

            if (Summoned && SummonMaster != null && SummonMaster is PlayerMobile)
                return SummonMaster as PlayerMobile;

            if (Controlled && ControlMaster != null && ControlMaster is PlayerMobile)
                return ControlMaster as PlayerMobile;

            return null;        
        }

        public override Mobile GetDamageMaster(Mobile damagee)
        {
            if (m_bBardProvoked && damagee == m_bBardTarget)
                return m_bBardMaster;

            else if (m_bControlled && m_ControlMaster != null)
                return m_ControlMaster;

            else if (m_Summoned && m_SummonMaster != null)
                return m_SummonMaster;

            return base.GetDamageMaster(damagee);
        }

        public void Pacify(Mobile master, TimeSpan duration, bool message)
        {
            PlaySound(GetIdleSound());

            BardPacified = true;
            BardProvoked = false;

            BardMaster = master;
            BardTarget = null;

            PlayerMobile pm_Master = master as PlayerMobile;

            Warmode = false;
            Combatant = null;

            BardEndTime = DateTime.UtcNow + duration;
            LastSwingTime = DateTime.UtcNow + duration;

            if (message)
                PublicOverheadMessage(MessageType.Emote, EmoteHue, false, "*looks calmed*");

            NextBardingEffectAllowed = DateTime.UtcNow + BardingEffectCooldown;

            if (Controlled && ControlMaster != null)
            {
                if (AIObject != null)
                {
                    ControlOrder = OrderType.Stop;
                    AIObject.DoOrderStop();
                }
            }
        }

        public void Provoke(Mobile bardMaster, Mobile target, bool bSuccess, TimeSpan duration, bool fromAdmin)
        {
            if (bardMaster == null) return;
            if (target == null) return;

            if (bSuccess)
            {
                BardProvoked = true;
                BardPacified = false;

                if (!fromAdmin)
                    

                PlaySound(GetIdleSound());

                BardMaster = bardMaster;
                BardTarget = target;

                bardMaster.DoHarmful(this);
                bardMaster.DoHarmful(target);

                PlayerMobile pm_Master = bardMaster as PlayerMobile;
                BaseCreature bc_Target = target as BaseCreature;

                if (fromAdmin)
                    duration = TimeSpan.FromHours(12);

                else
                {
                    NextBardingEffectAllowed = DateTime.UtcNow + BardingEffectCooldown;

                    PublicOverheadMessage(MessageType.Emote, EmoteHue, false, "*looks furious*");
                }

                BardEndTime = DateTime.UtcNow + duration;

                if (AIObject != null)
                {
                    if (Controlled && ControlMaster != null)
                    {
                        ControlTarget = target;
                        ControlOrder = OrderType.Attack;
                        AIObject.DoOrderAttack();
                    }

                    else
                    {
                        Combatant = target;

                        if (AIObject != null)
                            AIObject.CombatMode();
                    }
                }

                if (bc_Target != null)
                {
                    bc_Target.BardProvoked = true;

                    bc_Target.BardMaster = bardMaster;
                    bc_Target.BardTarget = this;
                    bc_Target.BardEndTime = DateTime.UtcNow + duration;

                    if (!fromAdmin)
                    {
                        bc_Target.NextBardingEffectAllowed = DateTime.UtcNow + bc_Target.BardingEffectCooldown;
                        bc_Target.PublicOverheadMessage(MessageType.Emote, EmoteHue, false, "*looks furious*");
                    }

                    if (bc_Target.AIObject != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster != null)
                        {
                            bc_Target.ControlTarget = this;
                            bc_Target.ControlOrder = OrderType.Attack;
                            bc_Target.AIObject.DoOrderAttack();
                        }

                        else
                        {
                            bc_Target.Combatant = this;
                            bc_Target.AIObject.CombatMode();
                        }
                    }
                }
            }

            else
            {
                PlaySound(GetAngerSound());

                BardMaster = bardMaster;
                BardTarget = target;

                bardMaster.DoHarmful(this);
                bardMaster.DoHarmful(target);
            }
        }

        public bool FindMyName(string str, bool bWithAll)
        {
            int i, j;

            string name = this.Name;

            if (name == null || str.Length < name.Length)
                return false;

            string[] wordsString = str.Split(' ');
            string[] wordsName = name.Split(' ');

            for (j = 0; j < wordsName.Length; j++)
            {
                string wordName = wordsName[j];

                bool bFound = false;
                for (i = 0; i < wordsString.Length; i++)
                {
                    string word = wordsString[i];

                    if (Insensitive.Equals(word, wordName))
                        bFound = true;

                    if (bWithAll && Insensitive.Equals(word, "all"))
                        return true;
                }

                if (!bFound)
                    return false;
            }

            return true;
        }

        public static void TeleportPets(Mobile master, Point3D loc, Map map)
        {
            TeleportPets(master, loc, map, false);
        }

        public static bool TeleportPets(Mobile master, Point3D loc, Map map, bool onlyBonded, int range = 3)
        {
            List<Mobile> move = new List<Mobile>();

            IPooledEnumerable eable = master.GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && pet.ControlMaster == master)
                    {
                        if (!onlyBonded || pet.IsBonded)
                        {
                            if (pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come)
                                move.Add(pet);
                        }
                    }
                }
            }

            eable.Free();

            foreach (Mobile m in move)
                m.MoveToWorld(loc, map);

            return move.Count > 0;
        }

        public virtual void ResurrectPet()
        {
            if (!IsDeadPet)
                return;

            OnBeforeResurrect();

            Poison = null;

            Warmode = false;

            Hits = 10;
            Stam = StamMax;
            Mana = 0;

            ProcessDeltaQueue();

            IsDeadPet = false;

            Effects.SendPacket(Location, Map, new BondedStatus(0, this.Serial, 0));

            this.SendIncomingPacket();
            this.SendIncomingPacket();

            OnAfterResurrect();

            Mobile owner = this.ControlMaster;

            CheckStatTimers();
        }

        public override bool CanBeDamaged()
        {
            if (IsDeadPet || IsInvulnerable)
                return false;

            return base.CanBeDamaged();
        }

        public override void MoveToWorld(Point3D newLocation, Map map)
        {
            base.MoveToWorld(newLocation, map);

            //Adding a Creature to a Ship
            BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

            if (boat == null)
                m_BoatOccupied = null;

            else
            {
                m_BoatOccupied = boat;

                if (!(Controlled && ControlMaster is PlayerMobile))
                {
                    BardImmune = true;
                    boat.Crew.Add(this);
                    boat.EmbarkedMobiles.Add(this);
                }
            }
        }

        public override void OnAfterSpawn()
        {
            if (IsBoss() && BossSpawnMessage != "")
                CommandHandlers.BroadcastMessage(AccessLevel.Player, 0x4ea, BossSpawnMessage);

            base.OnAfterSpawn();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AIRunning { get { return m_AI != null && m_AI.m_Timer != null && m_AI.m_Timer.Running; } }

        private DateTime m_LastActivated;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastActivated { get { return m_LastActivated; } set { m_LastActivated = value; } }

        private BaseBoat m_BoatOccupied = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat BoatOccupied { get { return m_BoatOccupied; } set { m_BoatOccupied = value; } }

        private bool m_NoKillAwards = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool NoKillAwards
        {
            get { return m_NoKillAwards; }
            set { m_NoKillAwards = value; }
        }

        //If they are following a waypoint, they'll continue to follow it even if players aren't around
        public virtual bool PlayerRangeSensitive
        {
            get
            {
                return true; /*(this.CurrentWaypoint == null);*/
            }
        }

        private bool m_ReturnQueued;

        private bool IsSpawnerBound()
        {
            if ((Map != null) && (Map != Map.Internal))
            {
                if (FightMode != FightMode.None && (RangeHome >= 0))
                {
                    if (!Controlled && !Summoned)
                    {
                        if (Spawner != null && Spawner is Spawner && ((Spawner as Spawner).Map) == Map)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void OnSectorActivate()
        {
            if (PlayerRangeSensitive && m_AI != null)
                m_AI.Activate();

            base.OnSectorActivate();
        }

        private bool m_RemoveIfUntamed;
        private int m_RemoveStep;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveIfUntamed { get { return m_RemoveIfUntamed; } set { m_RemoveIfUntamed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RemoveStep { get { return m_RemoveStep; } set { m_RemoveStep = value; } }
    }

    public class LoyaltyTimer : Timer
    {
        private static TimeSpan InternalDelay = TimeSpan.FromMinutes(5.0);

        public static void Initialize()
        {
            new LoyaltyTimer().Start();
        }

        public LoyaltyTimer()
            : base(InternalDelay, InternalDelay)
        {
            m_NextHourlyCheck = DateTime.UtcNow + TimeSpan.FromHours(1.0);
            Priority = TimerPriority.FiveSeconds;
        }

        private DateTime m_NextHourlyCheck;

        protected override void OnTick()
        {
            if (DateTime.UtcNow >= m_NextHourlyCheck)
                m_NextHourlyCheck = DateTime.UtcNow + TimeSpan.FromHours(1.0);
            else
                return;

            List<BaseCreature> toRelease = new List<BaseCreature>();

            List<BaseCreature> toRemove = new List<BaseCreature>();

#if Framework_4_0
            Parallel.ForEach(World.Mobiles.Values, m =>
            {
#else
                        foreach ( Mobile m in World.Mobiles.Values ) {
#endif
                if (m is BaseMount && ((BaseMount)m).Rider != null)
                {
#if Framework_4_0
                    return;
#else
                                    continue;
#endif
                }

                if (m is BaseCreature)
                {
                    BaseCreature bc_Creature = (BaseCreature)m;

                    // added lines to check if a wild creature in a house region has to be removed or not
                    if (!bc_Creature.Controlled && !bc_Creature.IsStabled && ((bc_Creature.Region.IsPartOf(typeof(HouseRegion)) && bc_Creature.CanBeDamaged()) || (bc_Creature.RemoveIfUntamed && bc_Creature.Spawner == null)))
                    {
                        bc_Creature.RemoveStep++;

                        if (bc_Creature.RemoveStep >= 20)
                            lock (toRemove)
                                toRemove.Add(bc_Creature);
                    }

                    else
                        bc_Creature.RemoveStep = 0;
                }
            }
#if Framework_4_0
);
#endif

            // added code to handle removing of wild creatures in house regions
            foreach (BaseCreature c in toRemove)
            {
                c.Delete();
            }
        }
    }
}