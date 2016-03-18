using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an ostard corpse")]
    public class FrenziedOstard : BaseCreature
    {
        [Constructable]
        public FrenziedOstard(): this("a frenzied ostard")
        {
        }

        [Constructable]
        public FrenziedOstard(string name) : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)      
        {
            Name = "a frenzied ostard";
            Body = 0xDA;

            Hue = Utility.RandomHairHue() | 0x8000;

            BaseSoundID = 0x275;

            SetStr(50);
            SetDex(100);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;  
            
            Fame = 1500;
            Karma = -1500;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 65;
        }

        public override int Meat { get { return 3; } }
        public override int Hides { get { return 16; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8503; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 60; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 75; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .05;
                }
            }

            SpecialAbilities.FrenzySpecialAbility(effectChance, this, defender, .25, 10, -1, true, "", "", "*becomes frenzied*");
        }
       
        public FrenziedOstard(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}