using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a llama corpse")]
    public class Llama : BaseCreature
    {
        public override bool DropsGold { get { return false; } }
        public override double MaxSkillScrollWorth { get { return 0.0; } }
        [Constructable]
        public Llama(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a llama";
            Body = 0xDC;
            BaseSoundID = 0x3F3;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 30);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

            Fame = 300;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 30;
        }

        public override int Meat { get { return 2; } }
        public override int Hides { get { return 12; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8438; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
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

        public override void GenerateLoot()
        {
            switch (Utility.Random(1000))
            {
                case 1: { AddItem(new Diamond()); } break;
                case 2: { AddItem(new Ruby()); } break;
                case 3: { AddItem(new HealPotion()); } break;
                case 4: { AddItem(new HealScroll()); } break;
            }
        }        
        
        public Llama(Serial serial) : base(serial)
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