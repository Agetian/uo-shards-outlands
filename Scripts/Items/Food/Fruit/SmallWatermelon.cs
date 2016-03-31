using System;

namespace Server.Items
{
    public class SmallWatermelon : Food
    {
        [Constructable]
        public SmallWatermelon(): this(1)
        {
        }

        [Constructable]
        public SmallWatermelon(int amount): base(amount, 0xC5D)
        {
            Weight = 2.0;
        }

        public SmallWatermelon(Serial serial): base(serial)
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