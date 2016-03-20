using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a panther corpse")]
    public class Panther : BaseCreature
    {
        [Constructable]
        public Panther(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a panther";
            Body = 0xD6;
            Hue = 0x901;
            BaseSoundID = 0x462;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(125);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 450;
            Karma = 0;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 45;
        }

        public override int TamedItemId { get { return 8473; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 6; } }
        public override int TamedBaseMaxDamage { get { return 8; } }
        public override double TamedBaseWrestling { get { return 65; } }
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
                
        public Panther(Serial serial): base(serial)
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