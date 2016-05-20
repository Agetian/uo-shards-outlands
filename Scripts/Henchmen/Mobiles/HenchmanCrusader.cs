using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom
{
    public class HenchmanCrusader : BaseHenchman
	{
        public override string[] recruitSpeech
        {
            get
            {
                return new string[] {   "We shall purge the land of injustice.",
                                                                                };
            }
        }

        public override string[] idleSpeech
        {
            get
            {
                return new string[] {       "The land is sick with malevolence. We are the cure.",
                                                                                "Walk with me and deliver this land from evil.",
                                                                                "*gazes longingly towards the heavens*",
                                                                                "*prays*" 
                                                                                };
            }
        }

        public override string[] combatSpeech
        {
            get
            {
                return new string[] {     "We do not suffer the wicked.",
                                                                                "You will be scoured of all impurity.",
                                                                                "We do not rest. We do not yield. You will find no mercy.",
                                                                                "I am justice made manifest." 
                                                                                };
            }
        }

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Honorable; } }

		[Constructable]
		public HenchmanCrusader() : base()
		{
            SpeechHue = Utility.RandomPinkHue();

            Title = "the crusader";                 

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 5);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;

            Tameable = true;
            ControlSlots = 5;
            MinTameSkill = 120;
		}

        public override string TamedDisplayName { get { return "Crusader"; } }

        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 32; } }
        public override int TamedBaseMaxDamage { get { return 34; } }

        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseArchery { get { return 100; } }
        public override double TamedBaseFencing { get { return 100; } }
        public override double TamedBaseMacing { get { return 100; } }
        public override double TamedBaseSwords { get { return 100; } }

        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 75; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 25; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 100; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 150; } }

        public override void GenerateItems()
        {
            int colorTheme = Utility.RandomDyedHue();
            int customTheme = Utility.RandomMetalHue();

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: m_Items.Add(new DragonHelm() {Hue = colorTheme, Name = "Crusader's Helm", LootType = LootType.Blessed }); break;
                case 2: m_Items.Add(new DragonHelm() {Hue = colorTheme, Name = "Crusader's Helm", LootType = LootType.Blessed }); break;
            }

            m_Items.Add(new PlateGorget() { LootType = LootType.Blessed });
            m_Items.Add(new PlateChest() { LootType = LootType.Blessed });
            m_Items.Add(new PlateArms() { LootType = LootType.Blessed });
            m_Items.Add(new PlateLegs() { LootType = LootType.Blessed });
            m_Items.Add(new PlateGloves() { LootType = LootType.Blessed });           
           
            m_Items.Add(new Kilt(colorTheme) { LootType = LootType.Blessed });           
            m_Items.Add(new BodySash(colorTheme) { LootType = LootType.Blessed });
            m_Items.Add(new Cloak(colorTheme) { LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { m_Items.Add(new VikingSword() { LootType = LootType.Blessed }); m_Items.Add(new DupresShield() {Hue = colorTheme,  Name = "Crusader's Shield", LootType = LootType.Blessed }); break; }
                case 2: { m_Items.Add(new WarMace() { LootType = LootType.Blessed }); m_Items.Add(new DupresShield() { Hue = colorTheme, Name = "Crusader's Shield", LootType = LootType.Blessed }); break; }
                case 3: { m_Items.Add(new Halberd() { LootType = LootType.Blessed }); break; }
                case 4: { m_Items.Add(new WarHammer() { LootType = LootType.Blessed }); break; }
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .10;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.ExpertiseSpecialAbility(effectChance, this, defender, .05, 10, -1, true, "", "", "draws upon expertise*");
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (attacker is PlayerMobile)
                        effectChance = .10;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.FortitudeSpecialAbility(effectChance, attacker, this, 50, 10, -1, true, "They draw upon fortitude from your attack.", "", "draws upon fortitude*"); 
        }

        public override bool OnBeforeHarmfulSpell()
        {
            double effectChance = .1;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                    effectChance = .1;
            }

            if (Utility.RandomDouble() <= effectChance)
                MagicDamageAbsorb = 1;

            return true;
        }
        
        public HenchmanCrusader(Serial serial): base(serial)
        {
        }    

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
