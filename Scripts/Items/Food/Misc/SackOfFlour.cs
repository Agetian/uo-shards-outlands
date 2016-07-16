using System;

namespace Server.Items
{
    public class SackOfFlour : Item
    {
        public static int GetSBPurchaseValue() { return 20; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        [Constructable]
        public SackOfFlour(): this(1)
        {
            Name = "sack of flour";
        }

        [Constructable]
        public SackOfFlour(int amount): base(4153)
        {
            Name = "sack of flour";

            Stackable = true;
            Weight = 2;
            Amount = amount;
        }

        public SackOfFlour(Serial serial): base(serial)
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