﻿using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZNorthSouthGateLeftWall : UOACZBreakableStatic
    {
        public override int OverrideNormalItemId { get { return 31; } }
        public override int OverrideNormalHue { get { return 1109; } }

        public override int OverrideLightlyDamagedItemId { get { return 31; } }
        public override int OverrideLightlyDamagedHue { get { return 1109; } }

        public override int OverrideHeavilyDamagedItemId { get { return 31; } }
        public override int OverrideHeavilyDamagedHue { get { return 1109; } }

        public override int OverrideBrokenItemId { get { return 6004; } }
        public override int OverrideBrokenHue { get { return 2406; } }

        [Constructable]
        public UOACZNorthSouthGateLeftWall(): base()
        {
            Name = "wall";
        }

        public UOACZNorthSouthGateLeftWall(Serial serial): base(serial)
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
