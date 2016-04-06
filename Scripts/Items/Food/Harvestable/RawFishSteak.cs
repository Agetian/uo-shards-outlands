using System;

namespace Server.Items
{
    public class RawFishSteak : Item
    {
        [Constructable]
        public RawFishSteak(): this(1)
        {
        }

        [Constructable]
        public RawFishSteak(int amount): base(2426)
        {
            Name = "raw fish steak";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawFishSteak(Serial serial): base(serial)
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