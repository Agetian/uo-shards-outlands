using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a caribou corpse")]
    public class Caribou : BaseCreature
    {
        public override bool DropsGold { get { return false; } }
        public override double MaxSkillScrollWorth { get { return 0.0; } }
        [Constructable]
        public Caribou(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a caribou";
            Body = 0xEA;
            Hue = 1843;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

            Fame = 300;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 35;
        }

        public override int Meat { get { return 6; } }
        public override int Hides { get { return 15; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8404; } }
        public override int TamedItemHue { get { return 1843; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 5; } }
        public override int TamedBaseMaxDamage { get { return 7; } }
        public override double TamedBaseWrestling { get { return 55; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
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

        public Caribou(Serial serial): base(serial)
        {
        }

        public override int GetAttackSound() {return 0x82; }
        public override int GetHurtSound() { return 0x83;}
        public override int GetDeathSound() {return 0x84; }

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