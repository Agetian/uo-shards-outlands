using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a snake corpse")]
    public class Snake : BaseCreature
    {
        [Constructable]
        public Snake(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a snake";
            Body = 52;
            Hue = Utility.RandomSnakeHue();
            BaseSoundID = 0xDB;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 10);

            VirtualArmor = 25; 

            Fame = 300;
            Karma = -300;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 30;
        }

        public override int TamedItemId { get { return 8444; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 15; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 75; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 55; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 75; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.NeutralMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override int PoisonResistance { get { return 1; } }        

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

        public Snake(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}