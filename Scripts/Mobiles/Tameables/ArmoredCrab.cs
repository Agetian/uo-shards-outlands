﻿using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an armored crab corpse")]
    public class ArmoredCrab : BaseCreature
    {
        [Constructable]
        public ArmoredCrab(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an armored crab";
            Body = 714;
            Hue = 0;
            BaseSoundID = 0x616;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(100);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 125;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 45.0;

            Fame = 500;
            Karma = 0;
        }

        public override string TamedDisplayName { get { return "Armored Crab"; } }
        
        public override int TamedItemId { get { return 8463; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 60; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 150; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Slow; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.NeutralMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override LootDropModeType LootDropMode { get { return LootDropModeType.Hides; } }
        public override MeatType MeatType { get { return MeatType.Meat; } }
        public override int MeatAmount { get { return 5; } }
        public override int ResourceAmount { get { return 10; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public ArmoredCrab(Serial serial): base(serial)
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