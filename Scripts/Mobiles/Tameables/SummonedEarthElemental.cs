using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("an earth elemental corpse")]
    public class SummonedEarthElemental : BaseCreature
    {
        [Constructable]
        public SummonedEarthElemental(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an earth elemental";
            Body = 14;
            BaseSoundID = 268;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(600);

            SetDamage(26, 28);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);           
            
            VirtualArmor = 150;

            ControlSlots = 2;
        }

        public SummonedEarthElemental(Serial serial): base(serial)
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