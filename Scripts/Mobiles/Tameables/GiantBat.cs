﻿using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a giant bat corpse")]
    public class GiantBat : BaseCreature
    {
        [Constructable]
        public GiantBat(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant bat";
            Body = 317;
            Hue = 0;
            BaseSoundID = 0x642;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(75);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 60;

            Fame = 500;
            Karma = -500;
        }

        public override int TamedItemId { get { return 9777; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 6; } }
        public override int TamedBaseMaxDamage { get { return 8; } }
        public override double TamedBaseWrestling { get { return 80; } }
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
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.NeutralMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override bool CanFly { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .20;
                }
            }

            SpecialAbilities.BleedSpecialAbility(effectChance, this, defender, DamageMax, 8.0, -1, true, "", "Their bite causes you to bleed!", "-1");
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

        public GiantBat(Serial serial): base(serial)
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