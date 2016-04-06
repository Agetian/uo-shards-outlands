using System;

namespace Server.Items
{
    public class RawHam : Item
    {
        [Constructable]
        public RawHam(): this(1)
        {
        }

        [Constructable]
        public RawHam(int amount): base(2515)
        {
            Name = "raw ham";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawHam(Serial serial): base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}