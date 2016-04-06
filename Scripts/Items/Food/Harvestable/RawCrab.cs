using System;

namespace Server.Items
{
    public class RawCrab : Item
    {
        [Constructable]
        public RawCrab(): this(1)
        {
        }

        [Constructable]
        public RawCrab(int amount): base(0x44D1)
        {
            Name = "raw crab";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawCrab(Serial serial): base(serial)
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