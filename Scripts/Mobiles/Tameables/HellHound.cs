using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a hell hound corpse" )]
	public class HellHound : BaseCreature
	{
		[Constructable]
		public HellHound() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a hell hound";
			Body = 98;
			BaseSoundID = 229;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 3400;
			Karma = -3400;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 85;
        }

        public override string TamedDisplayName { get { return "Hell Hound"; } }
        
        public override int TamedItemId { get { return 8482; } }
        public override int TamedItemHue { get { return 237; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 15; } }

        public override int TamedBaseMaxHits { get { return 175; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 75; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override void SetTamedAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Daemonic; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public HellHound( Serial serial ) : base( serial )
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
