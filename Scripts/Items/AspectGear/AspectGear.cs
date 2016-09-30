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

        public static double BaseXPGainScalar = .01;      

        public static double LowContributionThreshold = .05; //If Total Damage Inflicted is Lower Than This Percent of Total Hit Points
        public static double LowContributionScalar = .5; //Reduction to XP Gain Chance for Low Damage Contribution 

        public static int NormalGain = 1;
        public static int ChampGain = 5;
        public static int LoHGain = 5;
        public static int BossGain = 15;
        public static int EventBossGain = 25;

        public static double BaseEffectChance = .01;
        public static double BaseEffectChancePerTier = .005;

        public static int BaselineDurability = 150;
        public static int IncreasedDurabilityPerTier = 20;

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

        #region Aspect Weapons

        public static double GetSpecialEffectWeaponSpeedScalar(double speed)
        {
            double scalar = 1.0;

            double minSpeed = 25;
            double maxSpeed = 60;

            scalar += 1 * ((maxSpeed - speed) / (maxSpeed - minSpeed));

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

        public static void CheckResolveSpecialEffect(BaseWeapon weapon, PlayerMobile attacker, BaseCreature defender)
        {
            if (weapon == null || attacker == null || defender == null) return;
            if (weapon.Aspect == AspectEnum.None) return;
            if (weapon.TierLevel == 0) return;

            double effectChance = BaseEffectChance + ((double)weapon.TierLevel * BaseEffectChancePerTier);
            double speedScalar = GetSpecialEffectWeaponSpeedScalar(weapon.Speed);

            double finalChance = effectChance * speedScalar;
                        
            if (Utility.RandomDouble() <= finalChance)
            {
                switch (weapon.Aspect)
                {
                }
            }
        }

        #endregion

        #region Aspect Armor

        public static void CheckForAndUpdateAspectArmorProperties(PlayerMobile player)
        {
            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Pants);

            BaseArmor armorPiece = null;

            for (int a = 0; a < m_Layers.Count; a++)
            {
                armorPiece = player.FindItemOnLayer(m_Layers[a]) as BaseArmor;

                if (armorPiece != null)
                {
                    if (armorPiece.Aspect != AspectEnum.None && armorPiece.TierLevel > 0)
                        break;
                }

                armorPiece = null;
            }

            if (armorPiece != null)
                UpdateArmorProperties(player);
        }

        public static void UpdateArmorProperties(Mobile from)
        {
            if (from == null)
                return;

            AspectArmorProfile aspectArmor = new AspectArmorProfile(from, null);

            bool matchingSet = false;

            if (aspectArmor.MatchingSet)
                matchingSet = true;

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

                    if (matchingSet && !from.RecentlyInPlayerCombat)
                        armor.BaseArmorRating = (int)(Math.Round(armor.ArmorBase * (1 + (aspectArmor.AspectArmorDetail.ArmorBonusPerTier * (double)aspectArmor.AspectArmorDetail.m_TierLevel))));
                }
            }

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
                player.ResetRegenTimers();
        }

        public static void OnArmorEquip(Mobile from, BaseArmor armor)
        {
            Timer.DelayCall(TimeSpan.FromMilliseconds(50), delegate
            {
                if (armor == null) return;
                if (armor.Deleted) return;
                if (from == null) return;
                if (from.Deleted) return;
                if (!from.Alive) return;

                AspectArmorProfile aspectArmor = new AspectArmorProfile(from, null);

                if (aspectArmor.MatchingSet && !from.RecentlyInPlayerCombat)
                {
                    armor.BaseArmorRating = (int)(Math.Round(armor.ArmorBase * (1 + (aspectArmor.AspectArmorDetail.ArmorBonusPerTier * (double)aspectArmor.AspectArmorDetail.m_TierLevel))));

                    string aspectName = GetAspectName(aspectArmor.AspectArmorDetail.m_Aspect);

                    int effectHue = AspectGear.GetAspectHue(aspectArmor.AspectArmorDetail.m_Aspect);
                    int effectTextHue = AspectGear.GetAspectTextHue(aspectArmor.AspectArmorDetail.m_Aspect);

                    if (from.Hidden)
                    {
                        from.AllowedStealthSteps = 0;
                        from.RevealingAction();
                    }

                    from.FixedParticles(0x373A, 10, 15, 5036, effectHue, 0, EffectLayer.Head);
                    from.SendSound(0x1ED);

                    UpdateArmorProperties(from);

                    if (from.NetState != null)
                        from.PrivateOverheadMessage(MessageType.Emote, effectTextHue, false, "*" + aspectName + " Aspect*", from.NetState);
                }
            });
        }

        public static void OnArmorRemoved(object parent, BaseArmor armor)
        {
            if (parent is PlayerMobile)
            {
                PlayerMobile player = parent as PlayerMobile;

                AspectArmorProfile aspectArmor = new AspectArmorProfile(player, null);

                UpdateArmorProperties(player);

                if (aspectArmor.Pieces == 5)
                {
                    if (player.Hidden)
                    {
                        player.AllowedStealthSteps = 0;
                        player.RevealingAction();
                    }

                    player.PlaySound(0x56B);
                    player.SendMessage("Your aspect effect fades.");
                }
            }
        }

        public class AspectArmorProfile
        {
            public AspectArmorDetail AspectArmorDetail = null;
            public PlayerMobile Player = null;

            public bool MatchingSet = false;
            public int Pieces = 0;

            BaseArmor lowestTierPiece = null;

            public AspectArmorProfile(Mobile wearer, BaseArmor armorPiece)
            {
                Player = wearer as PlayerMobile;

                if (Player == null)
                {
                    AspectArmorDetail = new AspectArmorDetail(AspectEnum.None, 1);
                    return;
                }
                
                AspectEnum currentAspect = AspectEnum.None;
                int lowestTier = 1000;

                List<Layer> m_Layers = new List<Layer>();

                m_Layers.Add(Layer.Helm);
                m_Layers.Add(Layer.Neck);
                m_Layers.Add(Layer.InnerTorso);
                m_Layers.Add(Layer.Arms);
                m_Layers.Add(Layer.Gloves);
                m_Layers.Add(Layer.Pants);

                MatchingSet = true;

                for (int a = 0; a < m_Layers.Count; a++)
                {
                    BaseArmor aspectArmor = Player.FindItemOnLayer(m_Layers[a]) as BaseArmor;

                    if (aspectArmor == null)
                    {
                        MatchingSet = false;
                        continue;
                    }

                    if (aspectArmor.Aspect == AspectEnum.None || aspectArmor.TierLevel == 0)
                        MatchingSet = false;

                    else
                    {
                        Pieces++;

                        if (currentAspect == AspectEnum.None)
                        {
                            currentAspect = aspectArmor.Aspect;
                            lowestTierPiece = aspectArmor;
                        }

                        else
                        {
                            if (currentAspect != aspectArmor.Aspect)
                                MatchingSet = false;
                        }

                        if (aspectArmor.TierLevel < lowestTier)
                        {
                            lowestTier = aspectArmor.TierLevel;
                            lowestTierPiece = aspectArmor;
                        }
                    }
                }

                //Single Piece of Armor Being Inspected
                if (armorPiece != null)
                {
                    currentAspect = armorPiece.Aspect;
                    lowestTier = armorPiece.TierLevel;
                }

                if (lowestTier == 1000)
                    lowestTier = 1;

                AspectArmorDetail = new AspectArmorDetail(currentAspect, lowestTier);
            }
        }  

        #endregion
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
