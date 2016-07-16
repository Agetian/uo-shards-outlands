using System;
using Server;

namespace Server.Items
{
    public class Buckler : BaseShield
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int InitMinHits { get { return 75; } }
        public override int InitMaxHits { get { return 75; } }

        public override int ArmorBase { get { return 6; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 7027; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -1; } }
        public override int IconOffsetY { get { return 6; } }

        [Constructable]
        public Buckler(): base(7027)
        {
            Name = "buckler";
            Weight = 4.0;
        }

        public Buckler(Serial serial)
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
