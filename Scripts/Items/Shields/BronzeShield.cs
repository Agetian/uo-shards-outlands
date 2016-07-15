using System;
using Server;

namespace Server.Items
{
    public class BronzeShield : BaseShield
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

        public override int InitMinHits { get { return 85; } }
        public override int InitMaxHits { get { return 85; } }
        
        public override int ArmorBase { get { return 18; } }
        public override int OldDexBonus { get { return -4; } }

        public override int IconItemId { get { return 7026; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 1; } }
        public override int IconOffsetY { get { return 5; } }

        [Constructable]
        public BronzeShield() : base(7026)
        {
            Name = "bronze shield";
            Weight = 6.0;
        }

        public BronzeShield(Serial serial)
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
