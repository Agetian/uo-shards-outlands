using System;
using Server;

namespace Server.Items
{
    public class HeaterShield : BaseShield
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.HeaterShieldArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.HeaterShieldMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.HeaterShieldDurability; } }
        public override int InitMaxHits { get { return ArmorValues.HeaterShieldDurability; } }

        public override int IconItemId { get { return 7031; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 32; } }

        [Constructable]
        public HeaterShield(): base(7031)
        {
            Name = "heater shield";
            Weight = 7.0;
        }

        public HeaterShield(Serial serial): base(serial)
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
