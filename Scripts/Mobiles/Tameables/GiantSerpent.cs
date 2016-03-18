using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

namespace Server.Mobiles
{
	[CorpseName( "a giant serpent corpse" )]
	[TypeAlias( "Server.Mobiles.Serpant" )]
	public class GiantSerpent : BaseCreature
	{
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

		[Constructable]
		public GiantSerpent() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant serpent";
			Body = 0x15;
			Hue = Utility.RandomSnakeHue();
			BaseSoundID = 219;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(175);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25;

			Fame = 2500;
			Karma = -2500;		

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 80;

            PackItem(new Bone(6));
        }

        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }

        public override int Meat { get { return 4; } }
        public override int Hides { get { return 15; } }
        public override HideType HideType { get { return HideType.Spined; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 9663; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 10; } }
        public override int TamedBaseMaxDamage { get { return 12; } }
        public override double TamedBaseWrestling { get { return 80; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 75; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

		public GiantSerpent(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( BaseSoundID == -1 )
				BaseSoundID = 219;
		}
	}
}
