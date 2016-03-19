// Delceri

using System;

namespace Server.Items
{
    public class HangingPinkFlowersVinesSouth : Item
    {
        [Constructable]
        public HangingPinkFlowersVinesSouth()
            : base(0x2CFC)
        {
            this.Name = ("hanging vines");
            this.Weight = 1.0;
        }

        public HangingPinkFlowersVinesSouth(Serial serial) : base(serial) { }

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