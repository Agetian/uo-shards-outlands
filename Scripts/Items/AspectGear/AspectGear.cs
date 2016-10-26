using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Server.Items
{
    public class AspectGear
    {        
        public enum WeaponSpecialEffectType
        {
            LightningStorm,
            Inspiration,
            Tremor,
            EnergySurge,
            Meteor,
            Chaos,
            Toxicity,
            Vanish,
            Doom,
            WaveBlast
        }

        public static int DistillationNeededForWeaponCreation = 60;
        public static int DistillationNeededForWeaponUpgrade = 30;  
        public static int DistillationNeededForArmorCreation = 10;              
        public static int DistillationNeededForArmorUpgrade = 5;

        public static int CoresNeededForWeaponCreation = 12;
        public static int CoresNeededForWeaponUpgrade = 6;
        public static int CoresNeededForArmorCreation = 2;
        public static int CoresNeededForArmorUpgrade = 1; 

        public static int BaseCraftingSkillNeeded = 100;
        public static int ExtraCraftingSkillNeededPerTier = 2;

        public static int MaxTierLevel = 10;
        public static int ExperienceNeededToUpgrade = 250;

        public static int AspectStartingArcaneCharges = 50;
        public static int ArcaneMaxCharges = 250;

        public static int ArcaneChargesCautionThreshold = 25;
        public static int ArcaneChargesLostOnDeath = 50;

        public static int ArcaneChargesUpgradeSound = 0x655;
        public static int ArcaneChargesCautionSound = 0x5D1;
        public static int ArcaneChargesWarningSound = 0x659;

        public static int ArcaneChargesUpgradeTextHue = 63;
        public static int ArcaneChargesCautionTextHue = 1256;
        public static int ArcaneChargesWarningTextHue = 2117;

        public static TimeSpan ArcaneChargesRechargeDelay = TimeSpan.FromMinutes(30);

        //Stats
        public static double BaseAccuracy = .05;
        public static double AccuracyPerTier = .02;

        public static int BaseTactics = 10;
        public static int TacticsPerTier = 3;

        public static double BaseArmorRatingBonus = .20;
        public static double ArmorRatingBonusPerTier = .8;

        public static int BaselineDurability = 150;
        public static int IncreasedDurabilityPerTier = 20;

        public static double AspectWeaponEffectChance = .01;
        public static double AspectWeaponEffectChancePerTier = .001;       

        //Aspect Armor Bonuses
        public static double AirMeleeSwingSpeedBonus = .05;
        public static double AirMeleeSwingSpeedBonusPerTier = .005;
        public static double AirMeleeDodgeChance = .025;
        public static double AirMeleeDodgeChancePerTier = .0025;
        public static double AirMeleeAvoidMovementEffectAvoidance = .05;
        public static double AirMeleeAvoidMovementEffectAvoidancePerTier = .005;

        public static double CommandFollowerDamageDealtBonus = .1;
        public static double CommandFollowerDamageDealtBonusPerTier = .01;
        public static double CommandFollowerExperienceBonus = .25;
        public static double CommandFollowerExperienceBonusPerTier = .025;

        public static double EarthMeleeSpecialChance = .1;
        public static double EarthMeleeSpecialChancePerTier = .01;
        public static double EarthMeleeSpecialDamageBonus = .5;
        public static double EarthDamageReduction = .1;
        public static double EarthDamageReductionPerTier = .01;
        public static double EarthKnockbackAvoidanceChance = .05;
        public static double EarthKnockbackAvoidanceChancePerTier = .005;

        public static double EldritchChargedSpellcastChanceBonus = .05;
        public static double EldritchChargedSpellcastChanceBonusPerTier = .005;
        public static double EldritchManaCostReductionChance = .10;
        public static double EldritchManaCostReductionChancePerTier = .01;
        public static double EldritchReagentCostReductionChance = .10;
        public static double EldritchReagentCostReductionChancePerTier = .01;

        public static double FireEffectOnAttackChance = .05;
        public static double FireEffectOnAttackChancePerTier = .005;
        public static double FireEffectOnHitChance = .1;
        public static double FireEffectOnHitChancePerTier = .01;

        public static double LyricEffectiveBardingSkillBonus = 5;
        public static double LyricEffectiveBardingSkillBonusPerTier = 1;
        public static double LyricDamageBardedTargetsBonus = .05;
        public static double LyricDamageToBardedTargetsBonusPerTier = .005;
        public static double LyricDamageReceivedReductionFromFailedBardingTarget = .10;
        public static double LyricDamageReceivedReductionFromFailedBardingTargetPerTier = .01;
        public static TimeSpan LyricDamageReceivedReductionFromFailedBardingDuration = TimeSpan.FromSeconds(5);

        public static double PoisonDamageBonus = .1;
        public static double PoisonDamageBonusPerTier = .01;
        public static double PoisonDamageReceivedDamageReduction = .1;
        public static double PoisonDamageReceivedDamageReductionPerTier = .01;
        public static double PoisonCureChanceBonus = .10;
        public static double PoisonCureChanceBonusPerTier = .1;

        public static double ShadowBackstabDamageBonus = .25;
        public static double ShadowBackstabDamageBonusPerTier = .025;
        public static double ShadowPostBackstabDamageReceivedReduction = .25;
        public static double ShadowPostBackstabDamageReceivedReductionPerTier = .025;
        public static TimeSpan ShadowPostBackstabDamageReceivedReductionDuration = TimeSpan.FromSeconds(5);

        public static double VoidWeaponSpecialAttackChanceBonus = .05;
        public static double VoidWeaponSpecialAttackChanceBonusPerTier = .005;
        public static double VoidChanceToRegenStatsOnAttack = .05;
        public static double VoidChanceToRegenStatsOnAttackPerTier = .005;
        public static double VoidChanceToNullifyDamageOnHit = .025;
        public static double VoidChanceToNullifyDamageOnHitPerTier = .0025;

        public static double WaterDamageDealtOnShips = .15;
        public static double WaterDamageDealtOnShipsPerTier = .015;
        public static double WaterHealingAmountReceived = .1;
        public static double WaterHealingAmountReceivedPerTier = .01;
        public static double WaterChanceForPotionNoConsume = .1;
        public static double WaterChanceForPotionNoConsumePerTier = .01;
        public static double WaterBleedDamageTakenReduction = .25;
        public static double WaterBleedDamageTakenReductionPerTier = .025;      

        //----

        public static double BaseXPGainScalar = .01;      

        public static double LowContributionThreshold = .05; //If Total Damage Inflicted is Lower Than This Percent of Total Hit Points
        public static double LowContributionScalar = .5; //Reduction to XP Gain Chance for Low Damage Contribution 

        public static int NormalGain = 1;
        public static int ChampGain = 5;
        public static int LoHGain = 5;
        public static int BossGain = 15;
        public static int EventBossGain = 25;        

        #region Aspect Properties and Functions

        public static AspectEnum GetRandomAspect()
        {
            int aspectCount = Enum.GetNames(typeof(AspectEnum)).Length;

            return (AspectEnum)Utility.RandomMinMax(1, aspectCount - 1);
        }

        public static string GetAspectName(AspectEnum aspect)
        {
            switch (aspect)
            {              
                case AspectEnum.Air: return "Air";
                case AspectEnum.Command: return "Command";
                case AspectEnum.Earth: return "Earth";
                case AspectEnum.Eldritch: return "Eldritch";
                case AspectEnum.Fire: return "Fire";
                case AspectEnum.Lyric: return "Lyric";
                case AspectEnum.Poison: return "Poison";
                case AspectEnum.Shadow: return "Shadow";
                case AspectEnum.Void: return "Void";
                case AspectEnum.Water: return "Water";
            }

            return "";
        }

        public static int GetAspectHue(AspectEnum aspect)
        {
            switch (aspect)
            {
                case AspectEnum.None: return 0;
                                   
                case AspectEnum.Air: return 2657;
                case AspectEnum.Command: return 1428;
                case AspectEnum.Earth: return 2653;
                case AspectEnum.Eldritch: return 2660;
                case AspectEnum.Fire: return 2635;
                case AspectEnum.Lyric: return 2241; 
                case AspectEnum.Poison: return 2127;
                case AspectEnum.Shadow: return 1757;
                case AspectEnum.Void: return 2599;
                case AspectEnum.Water: return 2592;                
            }

            return 2658;
        }

        public static int GetAspectTextHue(AspectEnum aspect)
        {
            switch (aspect)
            {
                case AspectEnum.None: return 2499;
                                   
                case AspectEnum.Air: return 90;
                case AspectEnum.Command: return 1427;   
                case AspectEnum.Earth: return 2550;
                case AspectEnum.Eldritch: return 2606;
                case AspectEnum.Fire: return 2116;
                case AspectEnum.Lyric: return 2240;   
                case AspectEnum.Poison: return 2126;
                case AspectEnum.Shadow: return 1756;
                case AspectEnum.Void: return 2629;
                case AspectEnum.Water: return 2591;
            }

            return 1154;
        }

        public static void PlayerResurrected(PlayerMobile player)
        {
            List<Item> m_Items = player.Backpack.FindItemsByType<Item>();

            bool issueArcaneChargesCaution = false;            
            bool issueArcaneChargesWarning = false;

            int previousArcaneCharges = 0;
            int currentArcaneCharges = 0;

            foreach (Item item in m_Items)
            {
                if (item is BaseWeapon || item is BaseArmor && item.TierLevel > 0 && item.Aspect != AspectEnum.None)
                {
                    previousArcaneCharges = item.ArcaneCharges;

                    ArcaneChargeLoss(item, AspectGear.ArcaneChargesLostOnDeath);

                    currentArcaneCharges = item.ArcaneCharges;

                    if (previousArcaneCharges > AspectGear.ArcaneChargesCautionThreshold && currentArcaneCharges <= AspectGear.ArcaneChargesCautionThreshold)
                        issueArcaneChargesCaution = true;

                    if (previousArcaneCharges > 0 && currentArcaneCharges == 0)
                        issueArcaneChargesWarning = true;
                }
            }

            if (issueArcaneChargesWarning)
                AspectGear.IssueWarning(player);

            else if (issueArcaneChargesCaution)
                AspectGear.IssueCaution(player);
        }

        public static void RecordDamage(PlayerMobile player, BaseCreature creature, int amount)
        {
            if (player == null || creature == null) return;
            if (amount == 0) return;
            if (creature.Summoned || creature.ControlMaster is PlayerMobile || creature.NoKillAwards) return;

            AspectGearExperienceEntry entry = GetAspectGearExperienceEntry(player, creature);

            if (entry == null)
            {
                entry = new AspectGearExperienceEntry(player);
                creature.m_AspectGearExperienceEntries.Add(entry);
            }

            entry.m_LastDamage = DateTime.UtcNow;
            entry.m_TotalDamage += amount;

            BaseWeapon oneHandWeapon = player.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
            BaseWeapon twoHandWeapon = player.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            BaseArmor helm = player.FindItemOnLayer(Layer.Helm) as BaseArmor;
            BaseArmor gorget = player.FindItemOnLayer(Layer.Neck) as BaseArmor;
            BaseArmor arms = player.FindItemOnLayer(Layer.Arms) as BaseArmor;
            BaseArmor gloves = player.FindItemOnLayer(Layer.Gloves) as BaseArmor;
            BaseArmor chest = player.FindItemOnLayer(Layer.InnerTorso) as BaseArmor;
            BaseArmor legs = player.FindItemOnLayer(Layer.Pants) as BaseArmor;

            #region Layers

            if (oneHandWeapon != null)
            {
                if (oneHandWeapon.Aspect != AspectEnum.None && oneHandWeapon.TierLevel > 0)
                    entry.m_Weapon = oneHandWeapon;
            }

            if (twoHandWeapon != null)
            {
                if (twoHandWeapon.Aspect != AspectEnum.None && twoHandWeapon.TierLevel > 0)
                    entry.m_Weapon = twoHandWeapon;
            }

            if (helm != null)
            {
                if (helm.Aspect != AspectEnum.None && helm.TierLevel > 0)
                    entry.m_Helm = helm;
            }

            if (gorget != null)
            {
                if (gorget.Aspect != AspectEnum.None && gorget.TierLevel > 0)
                    entry.m_Gorget = gorget;
            }

            if (arms != null)
            {
                if (arms.Aspect != AspectEnum.None && arms.TierLevel > 0)
                    entry.m_Arms = arms;
            }

            if (gloves != null)
            {
                if (gloves.Aspect != AspectEnum.None && gloves.TierLevel > 0)
                    entry.m_Gloves = gloves;
            }

            if (chest != null)
            {
                if (chest.Aspect != AspectEnum.None && chest.TierLevel > 0)
                    entry.m_Chest = chest;
            }

            if (legs != null)
            {
                if (legs.Aspect != AspectEnum.None && legs.TierLevel > 0)
                    entry.m_Legs = legs;
            }

            #endregion
        }

        public static void ResolveExperienceGainChargeLoss(PlayerMobile player, BaseCreature creature, Item item)
        {
            if (player == null || creature == null || item == null) return;
            if (item.Deleted) return;
            if (item.Aspect == AspectEnum.None) return;

            double experienceChance = creature.AspectGearExperienceChanceOnKill();
            int experienceValue = creature.AspectGearExperienceEarnedOnKill();

            if (Utility.RandomDouble() <= experienceChance)
            {
                int previousArcaneCharges = item.ArcaneCharges;

                ArcaneChargeLoss(item, experienceValue);               

                if (item.TierLevel > 0 && item.TierLevel < MaxTierLevel)
                {
                    item.Experience += experienceValue;

                    if (item.Experience > ExperienceNeededToUpgrade)
                        item.Experience = ExperienceNeededToUpgrade;
                }
            }
        }

        public static void ArcaneChargeLoss(Item item, int amount)
        {
            if (item == null)
                return;

            int previousChargeAmount = item.ArcaneCharges;

            item.ArcaneCharges -= amount;

            if (previousChargeAmount > 0 && item.ArcaneCharges == 0)
                item.NextArcaneRechargeAllowed = DateTime.UtcNow + AspectGear.ArcaneChargesRechargeDelay;
        }
        
        public static void TierUpgradeAvailable(PlayerMobile player)
        {
            if (player == null)
                return;

            player.SendSound(ArcaneChargesUpgradeSound); //0x650
            player.SendMessage(AspectGear.ArcaneChargesUpgradeTextHue, "One or more of your Aspect items has reached enough experience to be upgraded.");
        }

        public static void IssueCaution(PlayerMobile player)
        {
            if (player == null)
                return;

            player.SendSound(ArcaneChargesCautionSound);
            player.SendMessage(AspectGear.ArcaneChargesCautionTextHue, "Caution: One or more of your Aspect items is low on arcane charges. It will become un-Blessed at zero charges)");
        }

        public static void IssueWarning(PlayerMobile player)
        {
            if (player == null)
                return;

            player.SendSound(ArcaneChargesWarningSound);
            player.SendMessage(AspectGear.ArcaneChargesWarningTextHue, "Warning: One or more of your Aspect items is out of arcane charges and is no longer blessed!");
        }

        public static AspectGearExperienceEntry GetAspectGearExperienceEntry(PlayerMobile player, BaseCreature creature)
        {
            foreach (AspectGearExperienceEntry entry in creature.m_AspectGearExperienceEntries)
            {
                if (entry == null) continue;
                if (entry.m_Player == player)
                    return entry;
            }

            return null;
        }

        #endregion

        #region Aspect Weapons

        public static AspectWeaponProfile GetAspectWeaponProfile(Mobile from)
        {
            if (!(from is PlayerMobile)) 
                return null;

            PlayerMobile player = from as PlayerMobile;

            if (!player.Alive) return null;
            if (player.RecentlyInPlayerCombat) return null;

            if (player.m_AspectWeaponProfile == null)
                player.m_AspectWeaponProfile = new AspectWeaponProfile();

            if (player.m_AspectWeaponProfile.m_Aspect == AspectEnum.None || player.m_AspectWeaponProfile.m_TierLevel == 0)
                return null;

            BaseWeapon weapon = null;

            BaseWeapon oneHandedWeapon = player.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
            BaseWeapon twoHandedWeapon = player.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (oneHandedWeapon != null)
                weapon = oneHandedWeapon;

            if (twoHandedWeapon != null)
                weapon = twoHandedWeapon;

            if (weapon == null)
                return null;

            if (weapon.Aspect == AspectEnum.None || weapon.TierLevel <= 0 || weapon.ArcaneCharges == 0)
                return null;

            return player.m_AspectWeaponProfile;
        }

        public static void OnWeaponEquip(Mobile from, BaseWeapon weapon)
        {
            if (from == null) return;
            if (!(from is PlayerMobile)) return;

            PlayerMobile player = from as PlayerMobile;

            player.m_AspectWeaponProfile = new AspectWeaponProfile();

            if (weapon == null) return;
            if (weapon.Deleted) return;

            player.m_AspectWeaponProfile.m_Aspect = weapon.Aspect;
            player.m_AspectWeaponProfile.m_TierLevel = weapon.TierLevel;
        }

        public static void OnWeaponRemoved(object parent, BaseWeapon weapon)
        {
            if (!(parent is PlayerMobile)) 
                return;

            PlayerMobile player = parent as PlayerMobile;

            player.m_AspectWeaponProfile = new AspectWeaponProfile();

            player.m_AspectWeaponProfile.m_Aspect = AspectEnum.None;
            player.m_AspectWeaponProfile.m_TierLevel = 0;
        }

        public static double GetEffectWeaponSpeedScalar(BaseWeapon weapon)
        {
            double scalar = 1.0;

            if (weapon == null)
                return scalar;          

            double minSpeed = 25;
            double maxSpeed = 60;

            scalar += 1 * ((maxSpeed - weapon.Speed) / (maxSpeed - minSpeed));
                        
            return scalar;
        }

        public static AspectWeaponDetail GetAspectWeaponDetail(AspectEnum aspect)
        {
            AspectWeaponDetail detail = new AspectWeaponDetail();

            #region Aspects

            switch (aspect)
            {
                case AspectEnum.Lyric:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Inspiration;
                    detail.m_EffectDisplayName = "Inspiration";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Air:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.LightningStorm;
                    detail.m_EffectDisplayName = "Lightning Storm";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Command:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Chaos;
                    detail.m_EffectDisplayName = "Chaos";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Earth:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Tremor;
                    detail.m_EffectDisplayName = "Tremor";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Eldritch:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.EnergySurge;
                    detail.m_EffectDisplayName = "Energy Surge";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Fire:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Meteor;
                    detail.m_EffectDisplayName = "Meteor";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Poison:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Toxicity;
                    detail.m_EffectDisplayName = "Toxicity";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Shadow:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Vanish;
                    detail.m_EffectDisplayName = "Vanish";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Void:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.Doom;
                    detail.m_EffectDisplayName = "Doom";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Water:
                    detail.m_SpecialEffect = WeaponSpecialEffectType.WaveBlast;
                    detail.m_EffectDisplayName = "Wave Blast";
                    detail.m_EffectDescription = "";
                break;
            }

            #endregion

            return detail;
        }

        public static void ResolveSpecialEffect(BaseWeapon weapon, PlayerMobile player, BaseCreature bc_Creature)
        {
            if (weapon == null) return;
            if (weapon.Deleted) return;
            if (weapon.Aspect == AspectEnum.None) return;
            if (weapon.TierLevel == 0) return;
            if (player == null) return;
            if (bc_Creature == null) return;

            List<Mobile> m_TargetMobiles = new List<Mobile>();
            Queue m_Queue = new Queue();

            IPooledEnumerable mobilesInRange = null;

            int maxTargetCount = 10;

            double totalDamageToDistribute = 600;
            double maxIndividualDamage = 200;
            double individualDamage = 0;

            double damageVariation = .1;
            
            switch (weapon.Aspect)
            {
                #region Air Aspect

                case AspectEnum.Air:
                    totalDamageToDistribute = 600;
                    maxIndividualDamage = 200;

                    if (SpecialAbilities.HostileToPlayer(player, bc_Creature))
                        m_TargetMobiles.Add(bc_Creature);

                    mobilesInRange = player.Map.GetMobilesInRange(player.Location, 8);

                    foreach (Mobile mobile in mobilesInRange)
                    {
                        if (mobile == bc_Creature) continue;
                        if (mobile is PlayerMobile) continue;
                        if (!SpecialAbilities.HostileToPlayer(player, mobile)) continue;

                        if (m_TargetMobiles.Count < maxTargetCount)
                            m_TargetMobiles.Add(mobile);
                    }

                    if (m_TargetMobiles.Count == 0)
                        return;

                    individualDamage = totalDamageToDistribute / m_TargetMobiles.Count;

                    if (individualDamage > maxIndividualDamage)
                        individualDamage = maxIndividualDamage;

                    foreach (BaseCreature creature in m_TargetMobiles)
                    {
                        m_Queue.Enqueue(creature);
                    }

                    while (m_Queue.Count > 0)
                    {
                        BaseCreature creature = (BaseCreature)m_Queue.Dequeue();
                                                
                        creature.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);

                        SpecialAbilities.HinderSpecialAbility(1.0, null, creature, 1.0, 5.0, false, -1, false, "", "You have been shocked!", "-1");

                        int damage = (int)(Math.Round((1 - damageVariation + (Utility.RandomDouble() * damageVariation * .2)) * individualDamage));

                        AOS.Damage(creature, player, damage, 0, 0, 0, 0, 0);

                        for (int a = 0; a < 5; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds((double)a * 0.2), delegate
                            {
                                if (!SpecialAbilities.Exists(player)) return;
                                if (!SpecialAbilities.Exists(creature)) return;
                                if (player.Map != creature.Map) return;
                                if (Utility.GetDistance(player.Location, creature.Location) > 20) return;

                                IEntity startLocation = new Entity(Serial.Zero, new Point3D(player.Location.X, player.Location.Y, player.Location.Z + 5), player.Map);
                                IEntity endLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z + 5), creature.Map);

                                int particleSpeed = 5;

                                Effects.PlaySound(player.Location, player.Map, 0x211);

                                Effects.SendMovingParticles(startLocation, endLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);

                                double distance = Utility.GetDistanceToSqrt(player.Location, creature.Location);
                                double distanceDelay = (double)distance * .08;

                                Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                                {
                                    if (!SpecialAbilities.Exists(player)) return;
                                    if (!SpecialAbilities.Exists(creature)) return;
                                    if (Utility.GetDistance(player.Location, creature.Location) > 30) return;

                                    creature.FixedParticles(0x3967, 10, 40, 5036, 2603, 0, EffectLayer.CenterFeet);

                                    new Blood().MoveToWorld(creature.Location, creature.Map);
                                });
                            });
                        }
                    }
                break;

                #endregion

                #region Command Aspect

                case AspectEnum.Command:
                break;

                #endregion

                #region Earth Aspect

                case AspectEnum.Earth:
                break;

                #endregion

                #region Eldritch Aspect

                case AspectEnum.Eldritch:
                break;

                #endregion

                #region Fire Aspect

                case AspectEnum.Fire:
                break;

                #endregion

                #region Lyric Aspect

                case AspectEnum.Lyric:
                break;

                #endregion

                #region Poison Aspect

                case AspectEnum.Poison:
                break;

                #endregion

                #region Shadow Aspect

                case AspectEnum.Shadow:
                break;

                #endregion

                #region Void Aspect

                case AspectEnum.Void:
                break;

                #endregion

                #region Water Aspect

                case AspectEnum.Water:
                break;

                #endregion
            }

            mobilesInRange.Free();
        }

        #endregion

        #region Aspect Armor

        public static AspectArmorProfile GetAspectArmorProfile(Mobile from)
        {
            if (!(from is PlayerMobile))
                return null;

            PlayerMobile player = from as PlayerMobile;

            if (!player.Alive) return null;
            if (player.RecentlyInPlayerCombat) return null;

            if (player.m_AspectArmorProfile == null)
                player.m_AspectArmorProfile = new AspectArmorProfile();

            if (player.m_AspectArmorProfile.m_Aspect == AspectEnum.None || player.m_AspectArmorProfile.m_TierLevel == 0)
                return null;

            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Pants);

            int aspectArmorCount = 0;

            for (int a = 0; a < m_Layers.Count; a++)
            {
                BaseArmor armor = from.FindItemOnLayer(m_Layers[a]) as BaseArmor;

                if (armor == null) return null;
                if (armor.Aspect == AspectEnum.None) return null;
                if (armor.TierLevel <= 0) return null;
                if (armor.ArcaneCharges == 0) return null;

                aspectArmorCount++;
            }

            if (aspectArmorCount < 6)
                return null;

            return player.m_AspectArmorProfile;
        }

        public static void CheckForAndUpdateAspectArmorProperties(PlayerMobile player)
        {
            if (player == null)
                return;

            OnArmorEquip(player, null);
        }

        public static void UpdateArmorProperties(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            PlayerMobile player = from as PlayerMobile;

            if (player.m_AspectArmorProfile == null)
                player.m_AspectArmorProfile = new AspectArmorProfile();

            bool validAspectArmorSet = false;

            AspectArmorProfile aspectArmorProfile = GetAspectArmorProfile(from);

            if (aspectArmorProfile != null)
                validAspectArmorSet = true;

            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Pants);

            for (int a = 0; a < m_Layers.Count; a++)
            {
                BaseArmor armor = from.FindItemOnLayer(m_Layers[a]) as BaseArmor;

                if (armor != null)
                {
                    armor.BaseArmorRating = armor.ArmorBase;

                    if (validAspectArmorSet)
                        armor.BaseArmorRating = armor.ArmorBase * (1 + AspectGear.BaseArmorRatingBonus + (AspectGear.ArmorRatingBonusPerTier * (double)armor.TierLevel));

                    else if (armor.Aspect != AspectEnum.None && armor.TierLevel > 0)
                        armor.BaseArmorRating = armor.ArmorBase * (1 + AspectGear.BaseArmorRatingBonus);
                }
            }
        }

        public static void OnArmorEquip(Mobile from, BaseArmor armor)
        {
            if (from == null) return;
            if (!(from is PlayerMobile)) return;

            PlayerMobile player = from as PlayerMobile;
            
            player.m_AspectArmorProfile = new AspectArmorProfile();

            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Pants);

            AspectEnum currentAspect = AspectEnum.None;

            int lowestTier = 1000;
            int aspectArmorCount = 0;

            for (int a = 0; a < m_Layers.Count; a++)
            {
                BaseArmor targetArmor = player.FindItemOnLayer(m_Layers[a]) as BaseArmor;

                if (targetArmor == null) return;
                if (targetArmor.Deleted) return;
                if (targetArmor.Aspect == AspectEnum.None || targetArmor.TierLevel == 0) return;

                if (currentAspect == AspectEnum.None)
                {
                    currentAspect = targetArmor.Aspect;
                    lowestTier = targetArmor.TierLevel;
                }

                else
                {
                    if (targetArmor.Aspect != currentAspect)
                        return;

                    if (targetArmor.TierLevel < lowestTier)
                        lowestTier = targetArmor.TierLevel;
                }

                aspectArmorCount++;
            }

            if (aspectArmorCount == 6 && currentAspect != AspectEnum.None && lowestTier > 0 && lowestTier < 1000)
            {
                player.m_AspectArmorProfile.m_Aspect = currentAspect;
                player.m_AspectArmorProfile.m_TierLevel = lowestTier;

                string aspectName = GetAspectName(currentAspect);
                int effectHue = AspectGear.GetAspectHue(currentAspect);
                int effectTextHue = AspectGear.GetAspectTextHue(currentAspect);

                if (from.Hidden)
                {
                    from.AllowedStealthSteps = 0;
                    from.RevealingAction();
                }

                from.FixedParticles(0x373A, 10, 15, 5036, effectHue, 0, EffectLayer.Head);
                from.SendSound(0x1ED);

                if (from.NetState != null)
                    from.PrivateOverheadMessage(MessageType.Emote, effectTextHue, false, "*" + aspectName + " Aspect*", from.NetState);
            }

            UpdateArmorProperties(from);
        }

        public static void OnArmorRemoved(object parent, BaseArmor armor)
        {
            if (!(parent is PlayerMobile))
                return;

            PlayerMobile player = parent as PlayerMobile;

            if (player.m_AspectArmorProfile != null)
            {
                if (player.m_AspectArmorProfile.m_Aspect != AspectEnum.None && player.m_AspectArmorProfile.m_TierLevel > 0)
                {
                    if (player.Hidden)
                    {
                        player.AllowedStealthSteps = 0;
                        player.RevealingAction();
                    }

                    player.SendSound(0x56B);
                    player.SendMessage("Your aspect effect fades.");
                }
            }

            player.m_AspectArmorProfile = new AspectArmorProfile();

            UpdateArmorProperties(player);
        }
        
        #endregion
    }

    public class AspectWeaponProfile
    {
        public AspectEnum m_Aspect = AspectEnum.None;
        public int m_TierLevel = 0;

        public AspectWeaponProfile()
        {
        }
    }

    public class AspectWeaponDetail
    {
        public AspectGear.WeaponSpecialEffectType m_SpecialEffect = AspectGear.WeaponSpecialEffectType.LightningStorm;
        public string m_EffectDisplayName = "Effect Name";
        public string m_EffectDescription = "Effect Description";

        public AspectWeaponDetail()
        {
        }
    }

    public class AspectArmorProfile
    {
        public AspectEnum m_Aspect = AspectEnum.None;
        public int m_TierLevel = 0;

        public AspectArmorProfile()
        {
        }
    }

    public class AspectArmorDetail
    {
        public AspectEnum m_Aspect;
        public int m_TierLevel;

        public double ArmorBonusPerTier = .1;

        public string[] gumpText = new string[0];

        public AspectArmorDetail(AspectEnum aspectType, int tierLevel)
        {
            m_Aspect = aspectType;
            m_TierLevel = tierLevel;

            switch (aspectType)
            {
                case AspectEnum.Lyric:
                break;

                case AspectEnum.Air:
                break;

                case AspectEnum.Command:
                break;

                case AspectEnum.Earth:
                break;

                case AspectEnum.Eldritch:
                break;

                case AspectEnum.Fire:
                break;

                case AspectEnum.Poison:
                break;

                case AspectEnum.Shadow:
                break;

                case AspectEnum.Void:
                break;

                case AspectEnum.Water:
                break;
            }
        }
    }

    public class AspectGearExperienceEntry
    {
        public PlayerMobile m_Player;
        public int m_TotalDamage;
        public DateTime m_LastDamage;

        public BaseWeapon m_Weapon;
        public BaseArmor m_Helm;
        public BaseArmor m_Gorget;       
        public BaseArmor m_Arms;
        public BaseArmor m_Gloves;
        public BaseArmor m_Chest;
        public BaseArmor m_Legs;

        public AspectGearExperienceEntry(PlayerMobile player)
        {
            m_Player = player;
        }
    }
}
