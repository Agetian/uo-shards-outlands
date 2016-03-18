﻿using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReindeerCarrot: Food
    {
        public override string DefaultName {
            get {
                return "Reindeer Carrot";
            }
        }

        private static readonly TimeSpan Duration = TimeSpan.FromMinutes(10);

        [Constructable]
        public ReindeerCarrot()
            : base(0x0C77) 
        {
        }

        public ReindeerCarrot(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            bool ret = base.Eat(from);

            if (ret && Utility.RandomDouble() < 0.99) {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null) {
                    pm.BodyMod = 237;
                    InternalTimer t = new InternalTimer(from);
                    t.Start();
                }
            }

            return ret;
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

        private class InternalTimer : Timer
        {
            private Mobile mob;

            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(120)) {
                mob = m;
            }
            protected override void OnTick() {
                mob.BodyMod = 0;
            }
        }
    }
}
