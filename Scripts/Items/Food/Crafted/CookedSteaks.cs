using System;
using Server;

namespace Server.Items
{
    public class CookedSteaks : Food
    {
        public override string DisplayName { get { return "cooked steaks"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 7711; } }
        public override int IconHue { get { return 1848; } }
        public override int IconOffsetX { get { return 51; } }
        public override int IconOffsetY { get { return 45; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedSteaks(): this(1)
        {
        } 

        [Constructable]
        public CookedSteaks(int amount): base(7711)
        {
            Name = "cooked steaks";
            Hue = 1848;

            Amount = amount;
        }

        public CookedSteaks(Serial serial) : base(serial)
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