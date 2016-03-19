﻿using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class MilkForSanta : Food
    {
        public override string DefaultName {
            get {
                return "Milk for Santa";
            }
        }

        private static readonly TimeSpan Duration = TimeSpan.FromMinutes(10);

        [Constructable]
        public MilkForSanta()
	    : base (0x1F89)
        {
        }

        public MilkForSanta(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            bool ret = base.Eat(from);

            if (ret) {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null) {

                    bool show1 = Spells.SpellHelper.AddStatOffset(from, StatType.Dex, 10, Duration);
                    bool show2 = Spells.SpellHelper.AddStatOffset(from, StatType.Str, -5, Duration);

                    if (show1 || show2) {
                        from.FixedEffect(0x375A, 10, 15);
                        from.PlaySound(0x1E7);
                        return true;
                    }
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
    }
}
