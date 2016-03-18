using System;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.Engines.Quests;

namespace Server.Items
{
    [FlipableAttribute(0x13eb, 0x13f2)]
    public class PirateRingmailGloves : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 100; } }

        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 1; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 3; } }

        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 100; } }

        public override int AosStrReq { get { return 40; } }
        public override int OldStrReq { get { return 20; } }

        public override int RevertArmorBase { get { return 2; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }

        public override int ArmorBase { get { return 35; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Half; } }

        [Constructable]
        public PirateRingmailGloves(): base(0x13EB)
        {
            Weight = 2.0;
            Name = "Pirate Ringmail Gloves";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public PirateRingmailGloves(Serial serial): base(serial)
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