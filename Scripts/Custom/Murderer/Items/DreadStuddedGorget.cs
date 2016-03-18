using System;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.Engines.Quests;

namespace Server.Items
{
    public class DreadStuddedGorget : BaseArmor
    {
        public override int PlayerClassCurrencyValue { get { return 100; } }

        public override int BasePhysicalResistance { get { return 2; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 3; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 4; } }

        public override int InitMinHits { get { return 70; } }
        public override int InitMaxHits { get { return 90; } }

        public override int AosStrReq { get { return 25; } }
        public override int OldStrReq { get { return 25; } }

        public override int RevertArmorBase { get { return 1; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Studded; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override int ArmorBase { get { return 30; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.ThreeQuarter; } }

        [Constructable]
        public DreadStuddedGorget(): base(0x13D6)
        {
            Weight = 1.0;
            Name = "Dread Studded Gorget";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public DreadStuddedGorget(Serial serial): base(serial)
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