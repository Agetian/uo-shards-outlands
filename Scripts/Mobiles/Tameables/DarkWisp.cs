using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a dark wisp corpse" )]
	public class DarkWisp : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Wisp; } }

        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }
        
		[Constructable]
		public DarkWisp() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a dark wisp";
			Body = 165;
			BaseSoundID = 466;

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(500);
            SetMana(2000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 200);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -5000;

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 100.1;

            AddItem(new LightSource());
        }

        public override int TamedItemId { get { return 8448; } }
        public override int TamedItemHue { get { return 1107; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 175; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseEvalInt { get { return 125; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 100; } }
        public override int TamedBaseMaxMana { get { return 2000; } }
        public override double TamedBaseMagicResist { get { return 150; } }
        public override double TamedBaseMagery { get { return 100; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 175; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {            
        }

        public override void SetTamedAI()
        {
            SetSubGroup(AISubgroup.Mage4);
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

		public DarkWisp( Serial serial ) : base( serial )
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