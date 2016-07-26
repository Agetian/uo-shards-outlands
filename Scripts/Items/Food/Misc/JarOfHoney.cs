using System;

namespace Server.Items
{
    public class JarOfHoney : Food
    {
        public override string DisplayName { get { return "jar of honey"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Meagre; } }

        public override int IconItemId { get { return 2540; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

        public override int FillFactor { get { return 6; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 10; } }
        public override int MaxStaminaRegained { get { return 20; } }

        [Constructable]
        public JarOfHoney(): this(1)
        {
        }

        [Constructable]
        public JarOfHoney(int amount): base(2540)
        {
            Name = "jar of honey";

            Amount = amount;
        }

        public JarOfHoney(Serial serial): base(serial)
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