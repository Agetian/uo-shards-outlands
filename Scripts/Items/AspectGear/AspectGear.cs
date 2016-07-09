using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Globalization;

namespace Server.Items
{
    public class AspectGear
    {        
        public enum SpecialEffectType
        {
            LightningStorm,
            Inspiration,
            Tremor,
            EnergySurge,
            Meteor,
            Dirge,
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

        public static int MaxDungeonTier = 10;
        public static int MaxDungeonExperience = 250;
        public static int ArcaneMaxCharges = 200;

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

        public static string GetAspectName(AspectEnum aspect)
        {
            switch (aspect)
            {
                case AspectEnum.Air: return "Air";               
                case AspectEnum.Command: return "Command";
                case AspectEnum.Earth: return "Earth";
                case AspectEnum.Eldritch: return "Eldritch";
                case AspectEnum.Fire: return "Fire";
                case AspectEnum.Harmony: return "Harmony";
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
                case AspectEnum.Air: return 2658;              
                case AspectEnum.Command: return 1410;
                case AspectEnum.Earth: return 2709;
                case AspectEnum.Eldritch: return 2615;
                case AspectEnum.Fire: return 2635;
                case AspectEnum.Harmony: return 2651;               
                case AspectEnum.Poison: return 2127;
                case AspectEnum.Shadow: return 2412;
                case AspectEnum.Void: return 2599;
                case AspectEnum.Water: return 2590;
            }

            return 2658;
        }

        public static int GetAspectTextHue(AspectEnum aspect)
        {
            switch (aspect)
            {
                case AspectEnum.Air: return 1153;                
                case AspectEnum.Command: return 1412;
                case AspectEnum.Earth: return 2709;
                case AspectEnum.Eldritch: return 2618;
                case AspectEnum.Fire: return 1257;
                case AspectEnum.Harmony: return 1228;                
                case AspectEnum.Poison: return 2126;
                case AspectEnum.Shadow: return 2403;
                case AspectEnum.Void: return 2592;
                case AspectEnum.Water: return 2603;
            }

            return 1154;
        }

        public static double GetSpeedScalar(double speed)
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
            
            switch (aspect)
            {
                case AspectEnum.Air:
                    detail.m_SpecialEffect = SpecialEffectType.LightningStorm;
                    detail.m_EffectDisplayName = "Lightning Storm";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Command:
                    detail.m_SpecialEffect = SpecialEffectType.Inspiration;
                    detail.m_EffectDisplayName = "Inspiration";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Earth:
                    detail.m_SpecialEffect = SpecialEffectType.Tremor;
                    detail.m_EffectDisplayName = "Tremor";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Eldritch:
                    detail.m_SpecialEffect = SpecialEffectType.EnergySurge;
                    detail.m_EffectDisplayName = "Energy Surge";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Fire:
                    detail.m_SpecialEffect = SpecialEffectType.Meteor;
                    detail.m_EffectDisplayName = "Meteor";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Harmony:
                    detail.m_SpecialEffect = SpecialEffectType.Dirge;
                    detail.m_EffectDisplayName = "Dirge";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Poison:
                    detail.m_SpecialEffect = SpecialEffectType.Toxicity;
                    detail.m_EffectDisplayName = "Toxicity";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Shadow:
                    detail.m_SpecialEffect = SpecialEffectType.Vanish;
                    detail.m_EffectDisplayName = "Vanish";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Void:
                    detail.m_SpecialEffect = SpecialEffectType.Doom;
                    detail.m_EffectDisplayName = "Doom";
                    detail.m_EffectDescription = "";
                break;

                case AspectEnum.Water:
                    detail.m_SpecialEffect = SpecialEffectType.WaveBlast;
                    detail.m_EffectDisplayName = "Wave Blast";
                    detail.m_EffectDescription = "";
                break;
            }
            
            return detail;
        }

        public static void CheckResolveSpecialEffect(BaseWeapon weapon, PlayerMobile attacker, BaseCreature defender)
        {
            if (weapon == null || attacker == null || defender == null) return;
            if (weapon.Aspect == AspectEnum.None) return;
            if (weapon.TierLevel == 0) return;

            double effectChance = BaseEffectChance + ((double)weapon.TierLevel * BaseEffectChancePerTier);
            double speedScalar = GetSpeedScalar(weapon.Speed);

            double finalChance = effectChance * speedScalar;
                        
            if (Utility.RandomDouble() <= finalChance)
            {
                switch (weapon.Aspect)
                {
                }
            }
        }
    }

    public class AspectWeaponDetail
    {
        public AspectGear.SpecialEffectType m_SpecialEffect = AspectGear.SpecialEffectType.LightningStorm;
        public string m_EffectDisplayName = "Effect Name";
        public string m_EffectDescription = "Effect Description";
    }   
}