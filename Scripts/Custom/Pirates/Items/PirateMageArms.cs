using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAttribute(0x13cd, 0x13c5)]
    public class PirateMageArms : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 125; } }

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 3; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 75; } }

        public override int AosStrReq { get { return 20; } }
        public override int OldStrReq { get { return 15; } }

        public override int ArmorBase { get { return 33; } }       
        public override int RevertArmorBase { get { return 3; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        [Constructable]
        public PirateMageArms(): base(0x13CD)
        {
            Weight = 2.0;            
            Name = "Pirate Mage Arms";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public PirateMageArms(Serial serial): base(serial)
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