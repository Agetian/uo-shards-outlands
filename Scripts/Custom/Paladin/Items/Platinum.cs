using System;
using Server;

namespace Server.Items
{
    public class Platinum : Item
    {
        public override double DefaultWeight
        {
            get { return 0.02; }
        }

        [Constructable]
        public Platinum(): this(1)
        {
        }

        [Constructable]
        public Platinum(int amountFrom, int amountTo): this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Platinum(int amount): base(0xEF0)
        {
            Hue = 0x47E;
            Name = "platinum coin";
            Stackable = true;
            Amount = amount;

            PlayerClass = Server.PlayerClass.Paladin;
            PlayerClassRestricted = true;

            Server.Custom.CurrencyTracking.RegisterPlatinum(amount);
        }

        public Platinum(Serial serial): base(serial)
        {
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

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped.PlayerClassOwner != PlayerClassOwner)
                return false;
            
            return base.StackWith(from, dropped, playSound);
        }   

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            UpdateTotal(this, TotalType.Gold, newValue - oldValue);

            Server.Custom.CurrencyTracking.RegisterPlatinum(newValue - oldValue);
        }

        public override void OnDelete()
        {
            Server.Custom.CurrencyTracking.DeletePlatinum(this.Amount);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Server.Custom.CurrencyTracking.RegisterPlatinum(this.Amount);
        }
    }
}