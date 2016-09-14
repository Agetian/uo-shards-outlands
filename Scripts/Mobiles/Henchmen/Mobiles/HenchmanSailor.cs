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
    public class HenchmanSailor : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "Yar! Sign me up fer yer crew!",
                                                                                };
            }
        }

        public override string[] IdleSpeech
        {
            get
            {
                return new string[] {       "*scans horizon*",
                                                                                "*inspects sail*",
                                                                                "*fastens rope*",
                                                                                "*stares blankly*"
                                                                                };
            }
        }

        public override string[] CombatSpeech
        {
            get
            {
                return new string[] {     "Die scum!",
                                                                                "No quarter!",
                                                                                "Run em' through!",
                                                                                "For Britain!" 
                                                                                };
            }
        }

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Navy; } }

		[Constructable]
		public HenchmanSailor() : base()
		{
            SpeechHue = Utility.RandomBlueHue();

            Title = "the sailor";                 

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
            ControlSlots = 1;
            MinTameSkill = 100;
		}

        public override string TamedDisplayName { get { return "Sailor"; } }

        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 200; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }

        public override double TamedBaseWrestling { get { return 85; } }
        public override double TamedBaseArchery { get { return 85; } }
        public override double TamedBaseFencing { get { return 85; } }
        public override double TamedBaseMacing { get { return 85; } }
        public override double TamedBaseSwords { get { return 85; } }

        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 75; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 25; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void GenerateItems()
        {
            int colorTheme = Utility.RandomDyedHue();
            int customTheme = 0;

            m_Items.Add(new ThighBoots() { LootType = LootType.Blessed });

            m_Items.Add(new ShortPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed });
            m_Items.Add(new FancyShirt(colorTheme) { LootType = LootType.Blessed });
            m_Items.Add(new SkullCap(colorTheme) { LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { m_Items.Add(new Cutlass() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 2: { m_Items.Add(new Longsword() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 3: { m_Items.Add(new Mace() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
                case 4: { m_Items.Add(new Maul() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { Hue = customTheme, LootType = LootType.Blessed }); break; }
            }

            Bow bow = new Bow();
            bow.LootType = LootType.Blessed;
            m_Items.Add(bow);

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override bool CanSwitchWeapons { get { return true; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatAction[CombatAction.CombatHealSelf] = 1;

            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;

            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 5;

            CombatSpecialActionMinDelay = 50;
            CombatSpecialActionMaxDelay = 70;

            CombatHealActionMinDelay = 50;
            CombatHealActionMaxDelay = 70;

            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }

        public HenchmanSailor(Serial serial): base(serial)
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
