using System;

namespace Server.Items
{
    public class SheafOfHay : Item
    {
        [Constructable]
        public SheafOfHay(): base(3894)
        {
            Name = "sheaf of hay";

            Weight = 5;
        }

        public SheafOfHay(Serial serial): base(serial)
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