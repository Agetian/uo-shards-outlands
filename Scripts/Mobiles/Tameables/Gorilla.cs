using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a gorilla corpse")]
    public class Gorilla : BaseCreature
    {
        public override bool DropsGold { get { return false; } }
        public override double MaxSkillScrollWorth { get { return 0.0; } }
        [Constructable]
        public Gorilla(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a gorilla";
            Body = 0x1D;
            BaseSoundID = 0x9E;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 450;
            Karma = 0;            

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 40;
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 6; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8437; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 10; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public Gorilla(Serial serial): base(serial)
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