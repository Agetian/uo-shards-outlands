using System;
using Server;

namespace Server.Items
{
    public class WoodenKiteShield : BaseShield
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int InitMinHits { get { return 80; } }
        public override int InitMaxHits { get { return 80; } }
        
        public override int ArmorBase { get { return 10; } }
        public override int OldDexBonus { get { return -1; } }

        public override int IconItemId { get { return 7033; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 58; } }
        public override int IconOffsetY { get { return 35; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

        [Constructable]
        public WoodenKiteShield(): base(7033)
        {
            Name = "wooden kite shield";
            Weight = 5.0;
        }

        public WoodenKiteShield(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Weight == 7.0)
                Weight = 5.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
