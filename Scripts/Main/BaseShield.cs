using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class BaseShield : BaseArmor
    {
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public static double ShieldParrySkillScalar = .005;
        public static double ShieldParryDamageScalar = .25;
        public static double DurabilityLossChance = .1;

        public BaseShield(int itemID): base(itemID)
        {
        }

        public BaseShield(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override double ArmorRating
        {
            get
            {
                Mobile mobile = Parent as Mobile;

                double baseRating = BaseArmorRating;

                double ratingScalarBonus = 0;
                double durabilityPenalty = 0;
                
                //Quality
                switch (Quality)
                {
                    case Server.Quality.Low: ratingScalarBonus += -.10; break;
                    case Server.Quality.Regular: ratingScalarBonus += 0; break;
                    case Server.Quality.Exceptional: ratingScalarBonus += .20; break;
                }

                //Magical
                ratingScalarBonus += (double)((int)ProtectionLevel) * .10;

                //Material
                switch (Resource)
                {
                    case CraftResource.DullCopper: ratingScalarBonus += .02; break;
                    case CraftResource.ShadowIron: ratingScalarBonus += .04; break;
                    case CraftResource.Copper: ratingScalarBonus += .06; break;
                    case CraftResource.Bronze: ratingScalarBonus += .08; break;
                    case CraftResource.Gold: ratingScalarBonus += .10; break;
                    case CraftResource.Agapite: ratingScalarBonus += .12; break;
                    case CraftResource.Verite: ratingScalarBonus += .14; break;
                    case CraftResource.Valorite: ratingScalarBonus += .16; break;

                    case CraftResource.Lunite: ratingScalarBonus += .18; break;
                }                

                //Durability Scaling
                durabilityPenalty = -.20 * (((double)MaxHitPoints - (double)HitPoints) / (double)MaxHitPoints);

                double finalArmorRating = baseRating * (1 + ratingScalarBonus + durabilityPenalty);

                if (mobile == null)
                    return finalArmorRating;

                double skillAdjustedFinalArmorRating = (finalArmorRating * .5) + ((finalArmorRating * .5) * (mobile.Skills[SkillName.Parry].Value / 100));

                return skillAdjustedFinalArmorRating;
            }
        }

        public override int OnHit(BaseWeapon weapon, int damage)
        {
            return OnHit(weapon, damage, null);
        }

        public int OnHit(BaseWeapon weapon, int damage, Mobile attacker)
        {
            Mobile owner = this.Parent as Mobile;

            if (owner == null)
                return damage;

            if (DecorativeEquipment)
                return damage;

            double successChance = owner.Skills[SkillName.Parry].Value * ShieldParrySkillScalar;

            if (owner.CheckSkill(SkillName.Parry, successChance, 1.0))
            {
                damage = (int)(Math.Round((double)damage * ShieldParryDamageScalar));

                if (damage < 1)
                    damage = 1;

                owner.FixedEffect(0x37B9, 10, 16);

                if (!ArenaFight.AllowDurabilityImmunity(owner))
                {
                    if (Utility.RandomDouble() <= DurabilityLossChance && LootType != LootType.Blessed && MaxHitPoints > 0)
                    {
                        if (HitPoints > 1)
                        {
                            HitPoints--;

                            if (HitPoints == 5)
                            {
                                if (Parent is Mobile)
                                    ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }
                        }

                        else
                            Delete();
                    }
                }
            }

            return damage;
        }        
    }
}
