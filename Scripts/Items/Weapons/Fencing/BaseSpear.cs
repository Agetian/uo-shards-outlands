
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public abstract class BaseSpear : BaseMeleeWeapon
    {
        public override int BaseHitSound { get { return 0x23C; } }
        public override int BaseMissSound { get { return 0x238; } }

        public override SkillName BaseSkill { get { return SkillName.Fencing; } }
        public override WeaponType BaseType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation BaseAnimation { get { return WeaponAnimation.Pierce2H; } }

        public BaseSpear(int itemID): base(itemID)
        {
        }

        public BaseSpear(Serial serial): base(serial)
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

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            AttemptWeaponPoison(attacker, defender);
        }
    }
}