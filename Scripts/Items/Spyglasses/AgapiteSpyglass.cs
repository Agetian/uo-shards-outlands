using System;
using Server;
using Server.Network;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom;

namespace Server.Items
{
    public class AgapiteSpyglass : Spyglass
    {
        [Constructable]
        public AgapiteSpyglass(): base()
        {
            Resource = CraftResource.Agapite;
        }

        public AgapiteSpyglass(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {               
            }
        }
    }
}