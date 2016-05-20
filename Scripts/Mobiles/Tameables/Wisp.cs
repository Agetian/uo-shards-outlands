using System;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a wisp corpse" )]
	public class Wisp : BaseCreature
	{           
		[Constructable]
		public Wisp() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a wisp";
			Body = 58;
			BaseSoundID = 466;

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(400);
            SetMana(2000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -5000;

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 90;

            AddItem(new LightSource());
        }

        public override string TamedDisplayName { get { return "Wisp"; } }

        public override int TamedItemId { get { return 8448; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 100; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 100; } }
        public override int TamedBaseMaxMana { get { return 2000; } }
        public override double TamedBaseMagicResist { get { return 125; } }
        public override double TamedBaseMagery { get { return 100; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 175; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.Mage4;
            UpdateAI(false);

            DictCombatRange[CombatRange.Withdraw] = 0;

            DictCombatSpell[CombatSpell.SpellDamage4] += 3;
            DictCombatSpell[CombatSpell.SpellDamage5] += 3;
            DictCombatSpell[CombatSpell.SpellDamage6] -= 3;
            DictCombatSpell[CombatSpell.SpellDamage7] -= 3;
            DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;

            SpellDelayMin *= 1.33;
            SpellDelayMax *= 1.33;
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Elemental; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Mage4; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Wisp; } }

        public override bool CanFly { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

		public Wisp( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}