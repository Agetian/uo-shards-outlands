using System;
using Server;

namespace Server.Items
{
    public class CrabPuffs : Food
    {
        public override string DisplayName { get { return "crab puffs"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Appetizing; } }

        public override int IconItemId { get { return 2554; } }
        public override int IconItemHue { get { return 2514; } }
        public override int IconOffsetX { get { return 1; } }
        public override int IconOffsetY { get { return 8; } }

        public override int FillFactor { get { return 15; } }
        public override bool IsStackable { get { return false; } }
        public override int MaxCharges { get { return 3; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return true; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(7); } }

        public override int MinStaminaRegained { get { return 30; } }
        public override int MaxStaminaRegained { get { return 60; } }

        [Constructable]
        public CrabPuffs(): base(2554)
        {
            Name = "crab puffs";
            Hue = 2514;
        }

        public CrabPuffs(Serial serial): base(serial)
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
        }
    }
}