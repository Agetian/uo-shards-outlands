using System;
using Server;
using Server.Guilds;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class DreadLightShield : BaseShield
    {
        public override int PlayerClassCurrencyValue { get { return 150; } }

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 1; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 200; } }

        public override int AosStrReq { get { return 20; } }

        public override int ArmorBase { get { return 26; } }
        public override int OldDexBonus { get { return -2; } }

        [Constructable]
        public DreadLightShield(): base(0x1B7B)
        {
            Weight = 5.0;
            Name = "Dread Light Shield";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public DreadLightShield(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
