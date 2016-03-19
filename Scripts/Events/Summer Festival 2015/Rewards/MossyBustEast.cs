using System;

namespace Server.Items
{
    public class MossyBustEast : Item
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }

        [Constructable]
        public MossyBustEast() : base(0x12CA)
        {
            Name = "a mossy bust";
            Hue = 2210;
           
            Weight = 10;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("So handsome.");

            if (from.Female)
                from.SendSound(0x32B);

            else
                from.SendSound(0x43D);
        }

        public MossyBustEast(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}