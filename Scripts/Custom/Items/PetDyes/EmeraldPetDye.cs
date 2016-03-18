﻿using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class EmeraldPetDye : PetDye
    {   
        [Constructable]
        public EmeraldPetDye(): base()
        {
            Name = "emerald pet dye";

            Hue = 2565;
        }

        public EmeraldPetDye(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //-----

            Name = "emerald pet dye";
        }
    }
}
