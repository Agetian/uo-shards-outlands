using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using Server.Spells;
using Server.Regions;
using Server.Network;
using Server.SkillHandlers;

namespace Server
{
    public static class SkillCheck
    {
        public static void Initialize()
        {
            Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckLocation);
            Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation);

            Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckTarget);
            Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler(XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget);
        }

        public static double HousingSkillGainModifier = .25;
        public static double SkillRangeIncrement = 5.0;
        public static double StatRangeIncrement = 5.0;

        public static bool AdminShowSkillGainChance = false;

        public enum Stat 
        {   
            Str,
            Dex, 
            Int 
        }

        public static string GetSkillName(SkillName skillName)
        {
            string name = "";

            #region Skills

            switch (skillName)
            {
                case SkillName.Alchemy: name = "Alchemy"; break;
                case SkillName.Anatomy: name = "Anatomy"; break;
                case SkillName.AnimalLore: name = "Animal Lore"; break;
                case SkillName.AnimalTaming: name = "Animal Taming"; break;
                case SkillName.Archery: name = "Archery"; break;
                case SkillName.ArmsLore: name = "Arms Lore"; break;
                case SkillName.Begging: name = "Begging"; break;
                case SkillName.Blacksmith: name = "Blacksmithy"; break;
                case SkillName.Camping: name = "Camping"; break;
                case SkillName.Carpentry: name = "Carpentry"; break;
                case SkillName.Cartography: name = "Cartography"; break;
                case SkillName.Cooking: name = "Cooking"; break;
                case SkillName.DetectHidden: name = "Detect Hidden"; break;
                case SkillName.Discordance: name = "Discordance"; break;
                case SkillName.EvalInt: name = "Eval Int"; break;
                case SkillName.Fencing: name = "Fencing"; break;
                case SkillName.Fishing: name = "Fishing"; break;
                case SkillName.Forensics: name = "Forensic Eval"; break;
                case SkillName.Healing: name = "Healing"; break;
                case SkillName.Herding: name = "Herding"; break;
                case SkillName.Hiding: name = "Hiding"; break;
                case SkillName.Inscribe: name = "Inscription"; break;
                case SkillName.ItemID: name = "Item Id"; break;
                case SkillName.Lockpicking: name = "Lockpicking"; break;
                case SkillName.Lumberjacking: name = "Lumberjacking"; break;
                case SkillName.Macing: name = "Macing"; break;
                case SkillName.Magery: name = "Magery"; break;
                case SkillName.MagicResist: name = "Magic Resist"; break;
                case SkillName.Meditation: name = "Meditation"; break;
                case SkillName.Mining: name = "Mining"; break;
                case SkillName.Musicianship: name = "Musicianship"; break;
                case SkillName.Parry: name = "Parrying"; break;
                case SkillName.Peacemaking: name = "Peacemaking"; break;
                case SkillName.Poisoning: name = "Poisoning"; break;
                case SkillName.Provocation: name = "Provocation"; break;
                case SkillName.RemoveTrap: name = "Remove Trap"; break;
                case SkillName.Snooping: name = "Snooping"; break;
                case SkillName.SpiritSpeak: name = "Spirit Speak"; break;
                case SkillName.Stealing: name = "Stealing"; break;
                case SkillName.Stealth: name = "Stealth"; break;
                case SkillName.Swords: name = "Swordsmanship"; break;
                case SkillName.Tactics: name = "Tactics"; break;
                case SkillName.Tailoring: name = "Tailoring"; break;
                case SkillName.TasteID: name = "Taste Id"; break;
                case SkillName.Tinkering: name = "Tinkering"; break;
                case SkillName.Tracking: name = "Tracking"; break;
                case SkillName.Veterinary: name = "Veterinary"; break;
                case SkillName.Wrestling: name = "Wrestling"; break;
            }

            #endregion

            return name;
        }

        private static SkillName[] m_CombatSkills = new SkillName[]
		{
            SkillName.Archery, 
            SkillName.Fencing, 
            SkillName.Macing, 
            SkillName.Swords,
            SkillName.Wrestling, 
            SkillName.Tactics
        };

        public static bool IsCombatSkill(SkillName skillName)
        {
            for (int a = 0; a < m_CombatSkills.Length; a++)
            {
                if (m_CombatSkills[a] == skillName)                
                    return true;                
            }

            return false;
        }

        public static double SkillSpeedScalar(Mobile from, SkillName skillName)
        {
            //Weapon Swings
            if (IsCombatSkill(skillName))
            {
                int weaponSpeed = BaseWeapon.PlayerFistSpeed;

                if (from.Weapon != null)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (!(weapon is Fists))
                        weaponSpeed = weapon.Speed;
                }
                                
                double playerSwing = 15000.0 / (int)(((double)from.Stam + 100) * weaponSpeed);
                double fastestSwingPossible = 15000.0 / (int)(((double)100 + 100) * 60);
                
                double weaponSwingScalar = playerSwing / fastestSwingPossible;

                return weaponSwingScalar;
            }

            //Other Skills
            switch (skillName)
            {
                case SkillName.Parry: return SkillCooldown.ParryCooldown; break;
                case SkillName.MagicResist: return SkillCooldown.MagicResistCooldown; break;
                case SkillName.Healing: return (SkillCooldown.HealingSelfCooldown + SkillCooldown.HealingOtherCooldown) / 2; break;
                case SkillName.Anatomy: return SkillCooldown.AnatomyCooldown; break;
                case SkillName.Veterinary: return SkillCooldown.HealingOtherCooldown; break;
                case SkillName.AnimalLore: return SkillCooldown.AnimalLoreCooldown; break;
                case SkillName.Herding: return (SkillCooldown.HerdingSuccessCooldown + SkillCooldown.HerdingFailureCooldown) / 2; break;
                case SkillName.ArmsLore: return SkillCooldown.ArmsLoreCooldown; break;
                case SkillName.Magery: return SkillCooldown.MageryCooldown; break;
                case SkillName.EvalInt: return SkillCooldown.EvalIntCooldown; break;
                case SkillName.Meditation: return SkillCooldown.MeditationValidCooldown; break;
                case SkillName.Musicianship: return SkillCooldown.MusicianshipCooldown; break;
                case SkillName.Provocation: return (SkillCooldown.ProvocationSuccessCooldown + SkillCooldown.ProvocationFailureCooldown) / 2; break;
                case SkillName.Peacemaking: return (SkillCooldown.PeacemakingSuccessCooldown + SkillCooldown.PeacemakingFailureCooldown) / 2; break;
                case SkillName.Discordance: return (SkillCooldown.DiscordanceSuccessCooldown + SkillCooldown.DiscordanceFailureCooldown) / 2; break;
                case SkillName.SpiritSpeak: return SkillCooldown.SpiritSpeakCooldown; break;
                case SkillName.Tracking: return SkillCooldown.TrackingCooldown; break;
                case SkillName.Forensics: return SkillCooldown.ForensicsCooldown; break;
                case SkillName.Hiding: return SkillCooldown.HidingCooldown; break;
                case SkillName.Stealth: return SkillCooldown.StealthCooldown; break;
                case SkillName.DetectHidden: return SkillCooldown.DetectHiddenCooldown; break;
                case SkillName.Lockpicking: return SkillCooldown.LockpickingCooldown; break;
                case SkillName.RemoveTrap: return SkillCooldown.RemoveTrapCooldown; break;
                case SkillName.AnimalTaming: return SkillCooldown.AnimalTamingCooldown; break;

                case SkillName.Blacksmith: return SkillCooldown.BlacksmithCooldown; break;
                case SkillName.Carpentry: return SkillCooldown.CarpentryCooldown; break;
                case SkillName.Tailoring: return SkillCooldown.TailoringCooldown; break;
                case SkillName.Tinkering: return SkillCooldown.TinkeringCooldown; break;
                case SkillName.Alchemy: return SkillCooldown.AlchemyCooldown; break;
                case SkillName.Cartography: return SkillCooldown.CartographyCooldown; break;
                case SkillName.Cooking: return SkillCooldown.CookingCooldown; break;
                case SkillName.Poisoning: return SkillCooldown.PoisoningCooldown; break;
                case SkillName.Begging: return (SkillCooldown.BeggingSuccessCooldown + SkillCooldown.BeggingFailureCooldown) / 2; break;
                case SkillName.Camping: return SkillCooldown.CampingCooldown; break;
                case SkillName.Fishing: return SkillCooldown.FishingCooldown; break;
                case SkillName.Inscribe: return SkillCooldown.InscribeCooldown; break;
                case SkillName.ItemID: return SkillCooldown.ItemIDCooldown; break;
                case SkillName.Lumberjacking: return SkillCooldown.LumberjackingCooldown; break;
                case SkillName.Mining: return SkillCooldown.MiningCooldown; break;
                case SkillName.Snooping: return SkillCooldown.SnoopingCooldown; break;
                case SkillName.Stealing: return SkillCooldown.StealingCooldown; break;
                case SkillName.TasteID: return SkillCooldown.TasteIDCooldown; break;
            }

            return 1.0;
        }
        
        public static double DungeonSkillScalar(SkillName skillName)
        {
            switch (skillName)
            {
                //Dungeon Boosted
                case SkillName.Archery:         return 4.0; break;
                case SkillName.Fencing:         return 4.0; break;
                case SkillName.Macing:          return 4.0; break;
                case SkillName.Swords:          return 4.0; break;
                case SkillName.Wrestling:       return 4.0; break;
                case SkillName.Tactics:         return 4.0; break;
                case SkillName.Parry:           return 4.0; break;
                case SkillName.MagicResist:     return 4.0; break;
                case SkillName.Healing:         return 4.0; break;
                case SkillName.Anatomy:         return 4.0; break;
                case SkillName.Veterinary:      return 4.0; break;
                case SkillName.AnimalLore:      return 4.0; break;
                case SkillName.Herding:         return 4.0; break;
                case SkillName.ArmsLore:        return 4.0; break;
                case SkillName.Magery:          return 4.0; break;
                case SkillName.EvalInt:         return 4.0; break;
                case SkillName.Meditation:      return 4.0; break;
                case SkillName.Musicianship:    return 4.0; break;
                case SkillName.Provocation:     return 4.0; break;
                case SkillName.Peacemaking:     return 4.0; break;
                case SkillName.Discordance:     return 4.0; break;
                case SkillName.SpiritSpeak:     return 4.0; break;
                case SkillName.Tracking:        return 4.0; break;
                case SkillName.Forensics:       return 4.0; break;
                case SkillName.Hiding:          return 4.0; break;
                case SkillName.Stealth:         return 4.0; break;
                case SkillName.DetectHidden:    return 4.0; break;
                case SkillName.Lockpicking:     return 4.0; break;
                case SkillName.RemoveTrap:      return 4.0; break;
                case SkillName.AnimalTaming:    return 4.0; break;
                
                //No Boost
                case SkillName.Blacksmith:      return 1.0; break;
                case SkillName.Carpentry:       return 1.0; break;
                case SkillName.Tailoring:       return 1.0; break;
                case SkillName.Tinkering:       return 1.0; break;
                case SkillName.Alchemy:         return 1.0; break;
                case SkillName.Cartography:     return 1.0; break;
                case SkillName.Cooking:         return 1.0; break;
                case SkillName.Poisoning:       return 1.0; break;
                case SkillName.Begging:         return 1.0; break;
                case SkillName.Camping:         return 1.0; break;
                case SkillName.Fishing:         return 1.0; break;
                case SkillName.Inscribe:        return 1.0; break;
                case SkillName.ItemID:          return 1.0; break;
                case SkillName.Lumberjacking:   return 1.0; break;
                case SkillName.Mining:          return 1.0; break;
                case SkillName.Snooping:        return 1.0; break;
                case SkillName.Stealing:        return 1.0; break;
                case SkillName.TasteID:         return 1.0; break;
            }

            return 1.0;
        }

        public static double StatGainCooldown(double statValue)
        {
            double statGainCooldown = 0; //Minutes That Must Pass Between Stat Gain Increases For Each Stat

            double[] cooldown = new double[] {          
                                                        .05, .05,    //0-5, 5-10
                                                        .1,  .15,    //10-15, 15-20
                                                        .2,  .25,    //20-25, 25-30
                                                        .3,  .35,    //30-35, 30-40
                                                        .4,  .45,    //40-45, 45-50
                                                        .5,  .55,    //50-55, 55-60
                                                        .6,  .65,    //60-65, 65-70
                                                        .7,  .75,    //70-75, 75-80
                                                        .8,  .85,    //80-85, 85-90
                                                         1,   2};    //90-95, 95-100

            //Determine Cooldown
            for (int a = 0; a < cooldown.Length; a++)
            {
                double rangeBottom = a * StatRangeIncrement;
                double rangeTop = (a * StatRangeIncrement) + StatRangeIncrement;

                if (statValue >= rangeBottom && statValue < rangeTop)
                {
                    return cooldown[a];
                    break;
                }
            }

            return statGainCooldown;
        }

        public static double SkillGainCooldown(SkillName skillName, double skillValue)
        {
            double skillGainCooldown = 0; //Minutes That Must Pass Between Skill Gain Increases For Each Skill

            double[] cooldown = new double[] {          
                                                        .05, .05,    //0-5, 5-10
                                                        .1,  .15,    //10-15, 15-20
                                                        .2,  .25,    //20-25, 25-30
                                                        .3,  .35,    //30-35, 30-40
                                                        .4,  .45,    //40-45, 45-50
                                                        .5,  .55,    //50-55, 55-60
                                                        .6,  .65,    //60-65, 65-70
                                                        .7,  .75,    //70-75, 75-80
                                                        .8,  .85,    //80-85, 85-90
                                                        1,   2,      //90-95, 95-100
                                                        3,   4,     //100-105, 105-110
                                                        6,   8};    //110-115, 115-120

            //Crafting Skill Cooldown Overrides
            bool craftingSKill = (  skillName == SkillName.Alchemy || skillName == SkillName.Blacksmith || skillName == SkillName.Carpentry ||
                                    skillName == SkillName.Cartography || skillName == SkillName.Cooking || skillName == SkillName.Inscribe ||
                                    skillName == SkillName.Poisoning || skillName == SkillName.Tailoring || skillName == SkillName.Tinkering);
           
            if (craftingSKill)
            {
                cooldown = new double[] {               
                                                        .05,  .05,   //0-5, 5-10
                                                        .1,   .1,    //10-15, 15-20
                                                        .15,  .15,   //20-25, 25-30
                                                        .2,   .2,    //30-35, 30-40
                                                        .25,  .25,   //40-45, 45-50
                                                        .3,   .3,    //50-55, 55-60
                                                        .35,  .35,   //60-65, 65-70
                                                        .4,   .4,    //70-75, 75-80
                                                        .5,   .6,    //80-85, 85-90
                                                        .7,   .8,    //90-95, 95-100
                                                        1,    1.25,  //100-105, 105-110
                                                        1.5,  2};    //110-115, 115-120
            }

            //Individual Skill Cooldown Overrides
            else
            {                
                switch (skillName)
                {
                }
            }

            //Determine Cooldown
            for (int a = 0; a < cooldown.Length; a++)
            {
                double rangeBottom = a * SkillRangeIncrement;
                double rangeTop = (a * SkillRangeIncrement) + SkillRangeIncrement;

                if (skillValue >= rangeBottom && skillValue < rangeTop)
                {
                    return cooldown[a];
                    break;
                }
            }

            return skillGainCooldown;
        }

        public static bool Mobile_SkillCheckLocation(Mobile from, SkillName skillName, double minSkill, double maxSkill, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < minSkill)
                return false;

            else if (value >= maxSkill)
                return true;

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false;

            else if (chance >= 1.0)
                return true;

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool Mobile_SkillCheckTarget(Mobile from, SkillName skillName, object target, double minSkill, double maxSkill, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < minSkill)
                return false;

            else if (value >= maxSkill)
                return true;

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance, double skillGainScalar)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            if (chance < 0.0)
                return false;

            else if (chance >= 1.0)
                return true;

            return CheckSkill(from, skill, chance, skillGainScalar);
        }

        public static bool CheckSkill(Mobile from, Skill skill, double chance, double skillGainScalar)
        {  
            Skill mobileSkill = from.Skills[skill.SkillName];
            SkillName skillName = skill.SkillName;
            
            double skillValue = mobileSkill.Base;

            if (from.Skills.Cap == 0)
                return false;
            
            bool success = chance >= Utility.RandomDouble();

            if (from is BaseCreature)
                return success;
            
            //Check For Stat Gain            
            CheckStatGain(from, skill);

            SkillGainRange skillGainRange = m_SkillGainRanges[(int)skill.SkillName];
            
            double requiredUses = skillGainRange.m_UsesPerRange[0];

            for (int a = 0; a < skillGainRange.m_UsesPerRange.Length; a++)
            {
                double rangeBottom = a * SkillRangeIncrement;
                double rangeTop = (a * SkillRangeIncrement) + SkillRangeIncrement;

                if (skillValue >= rangeBottom && skillValue < rangeTop)
                {
                    requiredUses = skillGainRange.m_UsesPerRange[a];                    
                    break;
                }
            }

            double baseSkillGainScalar = skillGainScalar;
            double dungeonModifier = 1.0;
            double skillSpeedModifier = 1.0;
            double housingModifier = 1.0;
            double perkBonus = 1.0;
            double townBonus = 1.0;
            double craftingSquareBonus = 1.0;

            //Dungeon
            if (from.Region is DungeonRegion || from.Region is NewbieDungeonRegion)
                dungeonModifier = DungeonSkillScalar(skillName);

            //If Weapon Skill: Scale for Relative Speed
            if (IsCombatSkill(skillName))
                skillSpeedModifier = SkillSpeedScalar(from, skillName);
            
            //Housing
            BaseHouse house = BaseHouse.FindHouseAt(from.Location, from.Map, 16);

            if (house != null)
                housingModifier = HousingSkillGainModifier;            

            double finalSkillGainScalar = baseSkillGainScalar * dungeonModifier * skillSpeedModifier * housingModifier * perkBonus * townBonus * craftingSquareBonus;

            if (requiredUses == 0)
                return success;

            if (finalSkillGainScalar == 0)
                return success;           

            double skillGainChance = 1.0 / ((double)requiredUses / finalSkillGainScalar);
            int increaseAmount = (int)(Math.Ceiling(skillGainChance));

            if (increaseAmount < 1)
                increaseAmount = 1;

            if (increaseAmount > 5)
                increaseAmount = 5;

            if (AdminShowSkillGainChance && from.AccessLevel > AccessLevel.Player && from.NetState != null)            
                from.PrivateOverheadMessage(MessageType.Regular, 2550, false, skillName.ToString() + " " + skillGainChance.ToString(), from.NetState);

            if (from.Alive && Utility.RandomDouble() <= skillGainChance)
            {
                if (AllowSkillGain(from, skill))
                    GainSkill(from, skill, increaseAmount);
            }

            return success;
        }

        private static bool AllowStatGain(Mobile from, Stat stat)
        {
            if (from.Region is UOACZRegion)
                return false;
            
            switch (stat)
            {
                case Stat.Str:
                    if (from.NextStrGainAllowed > DateTime.UtcNow)
                        return false;
                break;

                case Stat.Dex:
                    if (from.NextDexGainAllowed > DateTime.UtcNow)
                        return false;
                break;

                case Stat.Int:
                    if (from.NextIntGainAllowed > DateTime.UtcNow)
                        return false;
                break;
            }

            return true;
        }

        private static bool AllowSkillGain(Mobile from, Skill skill)
        {
            if (from.NextSkillGainAllowed.ContainsKey(skill))
            {
                if (from.NextSkillGainAllowed[skill] > DateTime.UtcNow)
                    return false;
            }

            if (from.Region is UOACZRegion)
                return false;

            if (from.Region is NewbieDungeonRegion)
            {
                if (skill.Base >= NewbieDungeonRegion.SkillGainCap)
                {
                    from.SendMessage(string.Format("You have exceeded the skill cap for this area in {0}.", skill.SkillName));
                    return false;
                }
            }

            return true;
        }

        public static void CheckStatGain(Mobile from, Skill skill)
        {
            SkillInfo info = skill.Info;

            double skillSpeedScalar = SkillSpeedScalar(from, skill.SkillName);

            double strGainChance = info.StrGain * skillSpeedScalar;
            double dexGainChance = info.DexGain * skillSpeedScalar;
            double intGainChance = info.IntGain * skillSpeedScalar;
            
            if (from.StrLock == StatLockType.Up && strGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Str);

            else if (from.DexLock == StatLockType.Up && dexGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Dex);

            else if (from.IntLock == StatLockType.Up && intGainChance > Utility.RandomDouble())
                GainStat(from, Stat.Int);
        }     

        public static void GainSkill(Mobile from, Skill skill, int increase)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;               

            if (from.Region.IsPartOf(typeof(Regions.Jail)))
                return;

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;
                        
            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                Skills skills = from.Skills;

                if ((skills.Total / skills.Cap) >= Utility.RandomDouble())
                {
                    for (int i = 0; i < skills.Length; ++i)
                    {
                        Skill toLower = skills[i];

                        if (toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= increase)
                        {
                            toLower.BaseFixedPoint -= increase;
                            break;
                        }
                    }
                }

                if ((skills.Total + increase) <= skills.Cap)
                {
                    skill.BaseFixedPoint += increase;

                    double skillGainCooldown = SkillGainCooldown(skill.SkillName, skill.Base);

                    if (from.NextSkillGainAllowed.ContainsKey(skill))
                        from.NextSkillGainAllowed[skill] = DateTime.UtcNow + TimeSpan.FromMinutes(skillGainCooldown);

                    else
                        from.NextSkillGainAllowed.Add(skill, DateTime.UtcNow + TimeSpan.FromMinutes(skillGainCooldown));
                }
            }            
        }

        public static void GainStat(Mobile from, Stat stat)
        {
            if (!AllowStatGain(from, stat))
                return;

            bool atrophy = ((from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble());

            IncreaseStat(from, stat, atrophy);
        }

        public static void IncreaseStat(Mobile from, Stat stat, bool atrophy)
        {
            atrophy = (from.RawStatTotal >= from.StatCap);// || atrophy;

            switch (stat)
            {
                case Stat.Str:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Dex) && (from.RawDex < from.RawInt || !CanLower(from, Stat.Int)))
                            --from.RawDex;

                        else if (CanLower(from, Stat.Int))
                            --from.RawInt;
                    }

                    if (CanRaise(from, Stat.Str))
                    {
                        ++from.RawStr;

                        double cooldown = StatGainCooldown((double)from.RawStr);
                        from.NextStrGainAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(cooldown);
                    }

                    break;
                }

                case Stat.Dex:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Str) && (from.RawStr < from.RawInt || !CanLower(from, Stat.Int)))
                            --from.RawStr;

                        else if (CanLower(from, Stat.Int))
                            --from.RawInt;
                    }

                    if (CanRaise(from, Stat.Dex))
                    {
                        ++from.RawDex;

                        double cooldown = StatGainCooldown((double)from.RawDex);
                        from.NextDexGainAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(cooldown);
                    }

                    break;
                }

                case Stat.Int:
                {
                    if (atrophy)
                    {
                        if (CanLower(from, Stat.Str) && (from.RawStr < from.RawDex || !CanLower(from, Stat.Dex)))
                            --from.RawStr;

                        else if (CanLower(from, Stat.Dex))
                            --from.RawDex;
                    }

                    if (CanRaise(from, Stat.Int))
                    {
                        ++from.RawInt;

                        double cooldown = StatGainCooldown((double)from.RawInt);
                        from.NextIntGainAllowed = DateTime.UtcNow + TimeSpan.FromMinutes(cooldown);
                    }

                    break;
                }
            }
        }  

        public static bool CanLower(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str: return (from.StrLock == StatLockType.Down && from.RawStr > 10);
                case Stat.Dex: return (from.DexLock == StatLockType.Down && from.RawDex > 10);
                case Stat.Int: return (from.IntLock == StatLockType.Down && from.RawInt > 10);
            }

            return false;
        }

        public static bool CanRaise(Mobile from, Stat stat)
        {
            if (!(from is BaseCreature && ((BaseCreature)from).Controlled))
            {
                if (from.RawStatTotal >= from.StatCap)
                    return false;
            }

            switch (stat)
            {
                case Stat.Str: return (from.StrLock == StatLockType.Up && from.RawStr < 100);
                case Stat.Dex: return (from.DexLock == StatLockType.Up && from.RawDex < 100);
                case Stat.Int: return (from.IntLock == StatLockType.Up && from.RawInt < 100);
            }

            return false;
        }   
       
        #region SkillGainRanges

        public class SkillGainRange
        {
            public SkillName m_SkillName;
            public double[] m_UsesPerRange = new double[] { };

            public SkillGainRange(SkillName skillName, double[] usesPerRange)
            {
                m_SkillName = skillName;
                m_UsesPerRange = usesPerRange;
            }
        }

        private static SkillGainRange[] m_SkillGainRanges = new SkillGainRange[]
		{
            new SkillGainRange(SkillName.Alchemy, new double[]{     
                                                                    25, 50,     //0-5, 5-10
                                                                    75, 100,     //10-15, 15-20
                                                                    125, 150,     //20-25, 25-30
                                                                    175, 200,     //30-35, 30-40
                                                                    225, 250,     //40-45, 45-50
                                                                    500, 1000,     //50-55, 55-60
                                                                    750, 1000,     //60-65, 65-70
                                                                    1250, 1500,     //70-75, 75-80
                                                                    1167, 133,     //80-85, 85-90
                                                                    1500, 1667,     //90-95, 95-100
                                                                    1250, 1250,     //100-105, 105-110
                                                                    1250, 1250}),   //110-115, 115-120

            new SkillGainRange(SkillName.Anatomy, new double[]{     
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

            new SkillGainRange(SkillName.AnimalLore, new double[]{  
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

            new SkillGainRange(SkillName.ItemID, new double[]{      
                                                                    300, 600,     //0-5, 5-10
                                                                    900, 1200,     //10-15, 15-20
                                                                    1800, 2400,     //20-25, 25-30
                                                                    3000, 3600,     //30-35, 30-40
                                                                    4800, 6000,     //40-45, 45-50
                                                                    9000, 12000,     //50-55, 55-60
                                                                    15000, 18000,     //60-65, 65-70
                                                                    24000, 30000,     //70-75, 75-80
                                                                    36000, 48000,     //80-85, 85-90
                                                                    60000, 72000,     //90-95, 95-100
                                                                    105000, 120000,     //100-105, 105-110
                                                                    135000, 150000}),   //110-115, 115-120

            new SkillGainRange(SkillName.ArmsLore, new double[]{
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Parry, new double[]{  
                                                                    300, 600,     //0-5, 5-10
                                                                    900, 1200,     //10-15, 15-20
                                                                    1800, 2400,     //20-25, 25-30
                                                                    3000, 3600,     //30-35, 30-40
                                                                    4800, 6000,     //40-45, 45-50
                                                                    9000, 12000,     //50-55, 55-60
                                                                    15000, 18000,     //60-65, 65-70
                                                                    24000, 30000,     //70-75, 75-80
                                                                    36000, 48000,     //80-85, 85-90
                                                                    60000, 72000,     //90-95, 95-100
                                                                    105000, 120000,     //100-105, 105-110
                                                                    135000, 150000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Begging, new double[]{  
                                                                    40, 80,     //0-5, 5-10
                                                                    120, 160,     //10-15, 15-20
                                                                    240, 320,     //20-25, 25-30
                                                                    400, 480,     //30-35, 30-40
                                                                    640, 800,     //40-45, 45-50
                                                                    1200, 1600,     //50-55, 55-60
                                                                    2000, 2400,     //60-65, 65-70
                                                                    3200, 4000,     //70-75, 75-80
                                                                    4800, 6400,     //80-85, 85-90
                                                                    8000, 9600,     //90-95, 95-100
                                                                    14000, 16000,     //100-105, 105-110
                                                                    18000, 20000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Blacksmith, new double[]{ 
                                                                    13, 19,     //0-5, 5-10
                                                                    25, 31,     //10-15, 15-20
                                                                    38, 44,     //20-25, 25-30
                                                                    50, 56,     //30-35, 30-40
                                                                    63, 75,     //40-45, 45-50
                                                                    125, 125,     //50-55, 55-60
                                                                    167, 333,     //60-65, 65-70
                                                                    667, 1000,     //70-75, 75-80
                                                                    1333, 2000,     //80-85, 85-90
                                                                    2500, 2500,     //90-95, 95-100
                                                                    833, 833,     //100-105, 105-110
                                                                    714, 556}),   //110-115, 115-120

            new SkillGainRange(SkillName.Bowcraft, new double[]{ 
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Peacemaking, new double[]{  
                                                                    40, 80,     //0-5, 5-10
                                                                    120, 160,     //10-15, 15-20
                                                                    240, 320,     //20-25, 25-30
                                                                    400, 480,     //30-35, 30-40
                                                                    640, 800,     //40-45, 45-50
                                                                    1200, 1600,     //50-55, 55-60
                                                                    2000, 2400,     //60-65, 65-70
                                                                    3200, 4000,     //70-75, 75-80
                                                                    4800, 6400,     //80-85, 85-90
                                                                    8000, 9600,     //90-95, 95-100
                                                                    14000, 16000,     //100-105, 105-110
                                                                    18000, 20000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Camping, new double[]{  
                                                                    100, 200,     //0-5, 5-10
                                                                    300, 400,     //10-15, 15-20
                                                                    600, 800,     //20-25, 25-30
                                                                    1000, 1200,     //30-35, 30-40
                                                                    1600, 2000,     //40-45, 45-50
                                                                    3000, 4000,     //50-55, 55-60
                                                                    5000, 6000,     //60-65, 65-70
                                                                    8000, 10000,     //70-75, 75-80
                                                                    12000, 16000,     //80-85, 85-90
                                                                    20000, 24000,     //90-95, 95-100
                                                                    35000, 40000,     //100-105, 105-110
                                                                    45000, 50000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Carpentry, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Cartography, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.Cooking, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

            new SkillGainRange(SkillName.DetectHidden, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Discordance, new double[]{  
                                                                    40, 80,     //0-5, 5-10
                                                                    120, 160,     //10-15, 15-20
                                                                    240, 320,     //20-25, 25-30
                                                                    400, 480,     //30-35, 30-40
                                                                    640, 800,     //40-45, 45-50
                                                                    1200, 1600,     //50-55, 55-60
                                                                    2000, 2400,     //60-65, 65-70
                                                                    3200, 4000,     //70-75, 75-80
                                                                    4800, 6400,     //80-85, 85-90
                                                                    8000, 9600,     //90-95, 95-100
                                                                    14000, 16000,     //100-105, 105-110
                                                                    18000, 20000}),   //110-115, 115-120

            new SkillGainRange(SkillName.EvalInt, new double[]{  
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

            new SkillGainRange(SkillName.Healing, new double[]{  
                                                                    33, 67,     //0-5, 5-10
                                                                    100, 133,     //10-15, 15-20
                                                                    200, 267,     //20-25, 25-30
                                                                    333, 400,     //30-35, 30-40
                                                                    533, 667,     //40-45, 45-50
                                                                    1000, 1333,     //50-55, 55-60
                                                                    1667, 2000,     //60-65, 65-70
                                                                    2667, 3333,     //70-75, 75-80
                                                                    4000, 5333,     //80-85, 85-90
                                                                    6667, 8000,     //90-95, 95-100
                                                                    11667, 13333,     //100-105, 105-110
                                                                    15000, 16667}),   //110-115, 115-120

         new SkillGainRange(SkillName.Fishing, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

         new SkillGainRange(SkillName.Forensics, new double[]{  
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Herding, new double[]{  
                                                                    40, 80,     //0-5, 5-10
                                                                    120, 160,     //10-15, 15-20
                                                                    240, 320,     //20-25, 25-30
                                                                    400, 480,     //30-35, 30-40
                                                                    640, 800,     //40-45, 45-50
                                                                    1200, 1600,     //50-55, 55-60
                                                                    2000, 2400,     //60-65, 65-70
                                                                    3200, 4000,     //70-75, 75-80
                                                                    4800, 6400,     //80-85, 85-90
                                                                    8000, 9600,     //90-95, 95-100
                                                                    14000, 16000,     //100-105, 105-110
                                                                    18000, 20000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Hiding, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Provocation, new double[]{  
                                                                    40, 80,     //0-5, 5-10
                                                                    120, 160,     //10-15, 15-20
                                                                    240, 320,     //20-25, 25-30
                                                                    400, 480,     //30-35, 30-40
                                                                    640, 800,     //40-45, 45-50
                                                                    1200, 1600,     //50-55, 55-60
                                                                    2000, 2400,     //60-65, 65-70
                                                                    3200, 4000,     //70-75, 75-80
                                                                    4800, 6400,     //80-85, 85-90
                                                                    8000, 9600,     //90-95, 95-100
                                                                    14000, 16000,     //100-105, 105-110
                                                                    18000, 20000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Inscribe, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Lockpicking, new double[]{  
                                                                    100, 200,     //0-5, 5-10
                                                                    300, 400,     //10-15, 15-20
                                                                    600, 800,     //20-25, 25-30
                                                                    1000, 1200,     //30-35, 30-40
                                                                    1600, 2000,     //40-45, 45-50
                                                                    3000, 4000,     //50-55, 55-60
                                                                    5000, 6000,     //60-65, 65-70
                                                                    8000, 10000,     //70-75, 75-80
                                                                    12000, 16000,     //80-85, 85-90
                                                                    20000, 24000,     //90-95, 95-100
                                                                    35000, 40000,     //100-105, 105-110
                                                                    45000, 50000}),   //110-115, 115-120

        
        new SkillGainRange(SkillName.Magery, new double[]{          
                                                                    75, 150,     //0-5, 5-10
                                                                    225, 300,     //10-15, 15-20
                                                                    450, 600,     //20-25, 25-30
                                                                    500, 600,     //30-35, 30-40
                                                                    533, 667,     //40-45, 45-50
                                                                    818, 1091,     //50-55, 55-60
                                                                    1071, 1286,     //60-65, 65-70
                                                                    1200, 1500,     //70-75, 75-80
                                                                    900, 1200,     //80-85, 85-90
                                                                    1200, 1440,     //90-95, 95-100
                                                                    2100, 2400,     //100-105, 105-110
                                                                    2700, 3000}),   //110-115, 115-120

        new SkillGainRange(SkillName.MagicResist, new double[]{  
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Tactics, new double[]{  
                                                                    240, 480,     //0-5, 5-10
                                                                    720, 960,     //10-15, 15-20
                                                                    1440, 1920,     //20-25, 25-30
                                                                    2400, 2880,     //30-35, 30-40
                                                                    3840, 4800,     //40-45, 45-50
                                                                    7200, 9600,     //50-55, 55-60
                                                                    12000, 14400,     //60-65, 65-70
                                                                    19200, 24000,     //70-75, 75-80
                                                                    28800, 38400,     //80-85, 85-90
                                                                    48000, 57600,     //90-95, 95-100
                                                                    84000, 96000,     //100-105, 105-110
                                                                    108000, 120000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Snooping, new double[]{  
                                                                    200, 400,     //0-5, 5-10
                                                                    600, 800,     //10-15, 15-20
                                                                    1200, 1600,     //20-25, 25-30
                                                                    2000, 2400,     //30-35, 30-40
                                                                    3200, 4000,     //40-45, 45-50
                                                                    6000, 8000,     //50-55, 55-60
                                                                    10000, 12000,     //60-65, 65-70
                                                                    16000, 20000,     //70-75, 75-80
                                                                    24000, 32000,     //80-85, 85-90
                                                                    40000, 48000,     //90-95, 95-100
                                                                    70000, 80000,     //100-105, 105-110
                                                                    90000, 100000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Musicianship, new double[]{  
                                                                    60, 120,     //0-5, 5-10
                                                                    180, 240,     //10-15, 15-20
                                                                    360, 480,     //20-25, 25-30
                                                                    600, 720,     //30-35, 30-40
                                                                    960, 1200,     //40-45, 45-50
                                                                    1800, 2400,     //50-55, 55-60
                                                                    3000, 3600,     //60-65, 65-70
                                                                    4800, 6000,     //70-75, 75-80
                                                                    7200, 9600,     //80-85, 85-90
                                                                    12000, 14400,     //90-95, 95-100
                                                                    21000, 24000,     //100-105, 105-110
                                                                    27000, 30000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Poisoning, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

        new SkillGainRange(SkillName.Archery, new double[]{  
                                                                    240, 480,     //0-5, 5-10
                                                                    720, 960,     //10-15, 15-20
                                                                    1440, 1920,     //20-25, 25-30
                                                                    2400, 2880,     //30-35, 30-40
                                                                    3840, 4800,     //40-45, 45-50
                                                                    7200, 9600,     //50-55, 55-60
                                                                    12000, 14400,     //60-65, 65-70
                                                                    19200, 24000,     //70-75, 75-80
                                                                    28800, 38400,     //80-85, 85-90
                                                                    48000, 57600,     //90-95, 95-100
                                                                    84000, 96000,     //100-105, 105-110
                                                                    108000, 120000}),   //110-115, 115-120
   
        new SkillGainRange(SkillName.SpiritSpeak, new double[]{  
                                                                    150, 300,     //0-5, 5-10
                                                                    450, 600,     //10-15, 15-20
                                                                    900, 1200,     //20-25, 25-30
                                                                    1500, 1800,     //30-35, 30-40
                                                                    2400, 3000,     //40-45, 45-50
                                                                    4500, 6000,     //50-55, 55-60
                                                                    7500, 9000,     //60-65, 65-70
                                                                    12000, 15000,     //70-75, 75-80
                                                                    18000, 24000,     //80-85, 85-90
                                                                    30000, 36000,     //90-95, 95-100
                                                                    52500, 60000,     //100-105, 105-110
                                                                    67500, 75000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Stealing, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Tailoring, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

         new SkillGainRange(SkillName.AnimalTaming, new double[]{  
                                                                    5, 5,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    15, 15,     //20-25, 25-30
                                                                    20, 20,     //30-35, 30-40
                                                                    25, 25,     //40-45, 45-50
                                                                    30, 45,     //50-55, 55-60
                                                                    55, 77,     //60-65, 65-70
                                                                    96, 135,     //70-75, 75-80
                                                                    169, 200,     //80-85, 85-90
                                                                    227, 252,     //90-95, 95-100
                                                                    274, 295,     //100-105, 105-110
                                                                    313, 330}),   //110-115, 115-120

        new SkillGainRange(SkillName.TasteID, new double[]{  
                                                                    300, 600,     //0-5, 5-10
                                                                    900, 1200,     //10-15, 15-20
                                                                    1800, 2400,     //20-25, 25-30
                                                                    3000, 3600,     //30-35, 30-40
                                                                    4800, 6000,     //40-45, 45-50
                                                                    9000, 12000,     //50-55, 55-60
                                                                    15000, 18000,     //60-65, 65-70
                                                                    24000, 30000,     //70-75, 75-80
                                                                    36000, 48000,     //80-85, 85-90
                                                                    60000, 72000,     //90-95, 95-100
                                                                    105000, 120000,     //100-105, 105-110
                                                                    135000, 150000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Tinkering, new double[]{  
                                                                    10, 10,     //0-5, 5-10
                                                                    10, 10,     //10-15, 15-20
                                                                    10, 10,     //20-25, 25-30
                                                                    10, 10,     //30-35, 30-40
                                                                    10, 10,     //40-45, 45-50
                                                                    10, 10,     //50-55, 55-60
                                                                    10, 10,     //60-65, 65-70
                                                                    10, 10,     //70-75, 75-80
                                                                    10, 10,     //80-85, 85-90
                                                                    10, 10,     //90-95, 95-100
                                                                    10, 10,     //100-105, 105-110
                                                                    10, 10}),   //110-115, 115-120

         new SkillGainRange(SkillName.Tracking, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Veterinary, new double[]{  
                                                                    38, 75,     //0-5, 5-10
                                                                    113, 150,     //10-15, 15-20
                                                                    225, 300,     //20-25, 25-30
                                                                    375, 450,     //30-35, 30-40
                                                                    600, 750,     //40-45, 45-50
                                                                    1125, 1500,     //50-55, 55-60
                                                                    1875, 2250,     //60-65, 65-70
                                                                    3000, 3750,     //70-75, 75-80
                                                                    4500, 6000,     //80-85, 85-90
                                                                    7500, 9000,     //90-95, 95-100
                                                                    13125, 1500,     //100-105, 105-110
                                                                    16875, 18750}),   //110-115, 115-120

        new SkillGainRange(SkillName.Swords, new double[]{          
                                                                    240, 480,     //0-5, 5-10
                                                                    720, 960,     //10-15, 15-20
                                                                    1440, 1920,     //20-25, 25-30
                                                                    2400, 2880,     //30-35, 30-40
                                                                    3840, 4800,     //40-45, 45-50
                                                                    7200, 9600,     //50-55, 55-60
                                                                    12000, 14400,     //60-65, 65-70
                                                                    19200, 24000,     //70-75, 75-80
                                                                    28800, 38400,     //80-85, 85-90
                                                                    48000, 57600,     //90-95, 95-100
                                                                    84000, 96000,     //100-105, 105-110
                                                                    108000, 120000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Macing, new double[]{  
                                                                    240, 480,     //0-5, 5-10
                                                                    720, 960,     //10-15, 15-20
                                                                    1440, 1920,     //20-25, 25-30
                                                                    2400, 2880,     //30-35, 30-40
                                                                    3840, 4800,     //40-45, 45-50
                                                                    7200, 9600,     //50-55, 55-60
                                                                    12000, 14400,     //60-65, 65-70
                                                                    19200, 24000,     //70-75, 75-80
                                                                    28800, 38400,     //80-85, 85-90
                                                                    48000, 57600,     //90-95, 95-100
                                                                    84000, 96000,     //100-105, 105-110
                                                                    108000, 120000}),   //110-115, 115-120

        new SkillGainRange(SkillName.Fencing, new double[]{  
                                                                    240, 480,     //0-5, 5-10
                                                                    720, 960,     //10-15, 15-20
                                                                    1440, 1920,     //20-25, 25-30
                                                                    2400, 2880,     //30-35, 30-40
                                                                    3840, 4800,     //40-45, 45-50
                                                                    7200, 9600,     //50-55, 55-60
                                                                    12000, 14400,     //60-65, 65-70
                                                                    19200, 24000,     //70-75, 75-80
                                                                    28800, 38400,     //80-85, 85-90
                                                                    48000, 57600,     //90-95, 95-100
                                                                    84000, 96000,     //100-105, 105-110
                                                                    108000, 120000}),   //110-115, 115-120

    new SkillGainRange(SkillName.Wrestling, new double[]{  
                                                                    240, 480,     //0-5, 5-10
                                                                    720, 960,     //10-15, 15-20
                                                                    1440, 1920,     //20-25, 25-30
                                                                    2400, 2880,     //30-35, 30-40
                                                                    3840, 4800,     //40-45, 45-50
                                                                    7200, 9600,     //50-55, 55-60
                                                                    12000, 14400,     //60-65, 65-70
                                                                    19200, 24000,     //70-75, 75-80
                                                                    28800, 38400,     //80-85, 85-90
                                                                    48000, 57600,     //90-95, 95-100
                                                                    84000, 96000,     //100-105, 105-110
                                                                    108000, 120000}),   //110-115, 115-120

    new SkillGainRange(SkillName.Lumberjacking, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

    new SkillGainRange(SkillName.Mining, new double[]{  
                                                                    30, 60,     //0-5, 5-10
                                                                    90, 120,     //10-15, 15-20
                                                                    180, 240,     //20-25, 25-30
                                                                    300, 360,     //30-35, 30-40
                                                                    480, 600,     //40-45, 45-50
                                                                    900, 1200,     //50-55, 55-60
                                                                    1500, 1800,     //60-65, 65-70
                                                                    2400, 3000,     //70-75, 75-80
                                                                    3600, 4800,     //80-85, 85-90
                                                                    6000, 7200,     //90-95, 95-100
                                                                    10500, 12000,     //100-105, 105-110
                                                                    13500, 15000}),   //110-115, 115-120

    new SkillGainRange(SkillName.Meditation, new double[]{  
                                                                    40, 80,     //0-5, 5-10
                                                                    120, 160,     //10-15, 15-20
                                                                    240, 320,     //20-25, 25-30
                                                                    400, 480,     //30-35, 30-40
                                                                    640, 800,     //40-45, 45-50
                                                                    1200, 1600,     //50-55, 55-60
                                                                    2000, 2400,     //60-65, 65-70
                                                                    3200, 4000,     //70-75, 75-80
                                                                    4800, 6400,     //80-85, 85-90
                                                                    8000, 9600,     //90-95, 95-100
                                                                    14000, 16000,     //100-105, 105-110
                                                                    18000, 20000}),   //110-115, 115-120

    new SkillGainRange(SkillName.Stealth, new double[]{  
                                                                    10, 20,     //0-5, 5-10
                                                                    30, 40,     //10-15, 15-20
                                                                    60, 80,     //20-25, 25-30
                                                                    100, 120,     //30-35, 30-40
                                                                    160, 200,     //40-45, 45-50
                                                                    300, 400,     //50-55, 55-60
                                                                    500, 600,     //60-65, 65-70
                                                                    800, 1000,     //70-75, 75-80
                                                                    1200, 1600,     //80-85, 85-90
                                                                    2000, 2400,     //90-95, 95-100
                                                                    3500, 4000,     //100-105, 105-110
                                                                    4500, 5000}),   //110-115, 115-120

        new SkillGainRange(SkillName.RemoveTrap, new double[]{  
                                                                    100, 200,     //0-5, 5-10
                                                                    300, 400,     //10-15, 15-20
                                                                    600, 800,     //20-25, 25-30
                                                                    1000, 1200,     //30-35, 30-40
                                                                    1600, 2000,     //40-45, 45-50
                                                                    3000, 4000,     //50-55, 55-60
                                                                    5000, 6000,     //60-65, 65-70
                                                                    8000, 10000,     //70-75, 75-80
                                                                    12000, 16000,     //80-85, 85-90
                                                                    20000, 24000,     //90-95, 95-100
                                                                    35000, 40000,     //100-105, 105-110
                                                                    45000, 50000}),   //110-115, 115-120
        };

        #endregion
    }
}