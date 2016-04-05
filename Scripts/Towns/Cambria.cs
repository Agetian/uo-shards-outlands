﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class Cambria : Town
    {
        public override string TownName { get { return "Cambria"; } }
        public override TownIDValue TownID { get { return TownIDValue.Cambria; } }        
        public override IndexedRegionName RegionName { get { return IndexedRegionName.Cambria; } }

        public override int TownHue { get { return 2500; } }

        public override int TownIconItemId { get { return 0x13B9; } }
        public override int TownIconHue { get { return 0; } }

        [Constructable]
        public Cambria(): base(TownIDValue.Cambria, Map.Felucca)
        {
        }

        public Cambria(Serial serial): base(serial)
        {
        }

        public override void CreateVendors()
        {
            CreateVendor(TownID, new Point3D(1433, 1687, 0), region.Map, TownVendorType.Banker, 2, 3, 5);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}