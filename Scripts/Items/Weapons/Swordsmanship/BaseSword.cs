using System;
using Server;
using Server.Items;
using Server.Targets;

namespace Server.Items
{
    public abstract class BaseSword : BaseMeleeWeapon
    {
        public override int BaseHitSound { get { return 0x237; } }
        public override int BaseMissSound { get { return 0x23A; } }

        public override SkillName BaseSkill { get { return SkillName.Swords; } }
        public override WeaponType BaseType { get { return WeaponType.Slashing; } }
        public override WeaponAnimation BaseAnimation { get { return WeaponAnimation.Slash1H; } }

        public BaseSword(int itemID): base(itemID)
        {
        }

        public BaseSword(Serial serial): base(serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1010018); // What do you want to use this item on?

            from.Target = new BladedItemTarget(this);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            AttemptWeaponPoison(attacker, defender);
        }
    }
}