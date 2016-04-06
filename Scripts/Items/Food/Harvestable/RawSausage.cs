using System;

namespace Server.Items
{
    public class RawSausage : Item
    {
        [Constructable]
        public RawSausage(): base(2496)
        {
            Name = "raw sausage";

            Stackable = true;
            Weight = 0.1;
        }

        public RawSausage(Serial serial): base(serial)
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