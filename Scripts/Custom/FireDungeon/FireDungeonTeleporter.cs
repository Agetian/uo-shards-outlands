﻿using System;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Items
{
    public class FireDungeonTeleporter : Teleporter
    {
        [Constructable]
        public FireDungeonTeleporter() {
        }

        public FireDungeonTeleporter(Serial serial)
            : base(serial) {
        }

        public override void DoTeleport(Mobile m) {
            FireDungeon.Teleporter_OnMoveOver(m);
        }

        public override void Serialize(GenericWriter writer) {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader) {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
