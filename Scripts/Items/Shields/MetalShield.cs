using System;
using Server;

namespace Server.Items
{
    public class MetalShield : BaseShield
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int InitMinHits { get { return 90; } }
        public override int InitMaxHits { get { return 90; } }

        public override int ArmorBase { get { return 12; } }
        public override int OldDexBonus { get { return -2; } }

        public override int IconItemId { get { return 7035; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 57; } }
        public override int IconOffsetY { get { return 32; } }

        [Constructable]
        public MetalShield(): base(7035)
        {
            Name = "metal shield";
            Weight = 5.0;
        }

        public MetalShield(Serial serial)
            : base(serial)
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
