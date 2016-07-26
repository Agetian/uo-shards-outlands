using System;

namespace Server.Items
{
    public class CookedLobster : Food
    {
        public override string DisplayName { get { return "cooked lobster"; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Adequate; } }

        public override int IconItemId { get { return 17619; } }
        public override int IconHue { get { return 1850; } }
        public override int IconOffsetX { get { return 49; } }
        public override int IconOffsetY { get { return 33; } }

        public override int FillFactor { get { return 10; } }
        public override bool IsStackable { get { return true; } }
        public override int MaxCharges { get { return 1; } }
        public override double WeightPerCharge { get { return 1; } }

        public override bool Decays { get { return false; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 20; } }
        public override int MaxStaminaRegained { get { return 40; } }

        [Constructable]
        public CookedLobster(): this(1)
        {
        }

        [Constructable]
        public CookedLobster(int amount): base(17619)
        {
            Name = "cooked lobster";
            Hue = 1850;

            Amount = amount;
        }

        public CookedLobster(Serial serial): base(serial)
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