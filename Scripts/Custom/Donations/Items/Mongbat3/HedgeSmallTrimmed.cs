﻿using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class HedgeSmallTrimmed : Item
    {
        public override string DefaultName { get { return "Small Hedge, Trimmed"; } }

        [Constructable]
        public HedgeSmallTrimmed()
            : base(3215)
        {
            Hue = 0;
        }

        public HedgeSmallTrimmed(Serial serial)
            : base(serial)
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
