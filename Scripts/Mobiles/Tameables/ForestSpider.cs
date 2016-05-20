﻿using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a forest spider corpse")]
    public class ForestSpider : BaseCreature
    {
        [Constructable]
        public ForestSpider(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a forest spider";
            Body = 736;
            Hue = 0;
            BaseSoundID = 0x388;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(100);
            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 30.0;

            Fame = 500;
            Karma = -500;
        }

        public override string TamedDisplayName { get { return "Forest Spider"; } }

        public override int TamedItemId { get { return 8532; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 100; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
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

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override int PoisonResistance { get { return 1; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

        public ForestSpider(Serial serial): base(serial)
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