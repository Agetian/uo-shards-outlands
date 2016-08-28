using System;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class Doubloon : Item
    {
        [Constructable]
        public Doubloon(): this(1)
        {
        }

        [Constructable]
        public Doubloon(int amountFrom, int amountTo): this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Doubloon(int amount): base(2539)
        {
            Name = "doubloon";

            Weight = 0;

            Stackable = true;
            Amount = amount;
            Hue = 2125;           

            Server.Custom.CurrencyTracking.RegisterDoubloons(amount);
        }

        public Doubloon(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override int GetDropSound()
        {
            if (Amount <= 1)
                return 0x2E4;

            else if (Amount <= 5)
                return 0x2E5;

            else
                return 0x2E6;
        }

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            UpdateTotal(this, TotalType.Gold, newValue - oldValue);

            Server.Custom.CurrencyTracking.RegisterDoubloons(newValue - oldValue);
        }

        public override void OnDelete()
        {
            Server.Custom.CurrencyTracking.DeleteDoubloons(this.Amount);

            base.OnDelete();
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

            Server.Custom.CurrencyTracking.RegisterDoubloons(this.Amount);
        }
    }
}