using System;
using Server;

namespace Server.Items
{
    public class UOACZBottleOfLiquor : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Drink; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return 0; } }
        public override int ThirstChange { get { return Utility.RandomMinMax(6, 8); } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int ItemIdFor1Charges { get { return 2503; } }
        public override int ItemIdFor2Charges { get { return 2502; } }
        public override int ItemIdFor3Charges { get { return 2501; } }
        public override int ItemIdFor4Charges { get { return 2500; } }

        public override int MaxCharges { get { return 4; } }

        public override Type DropContainer { get { return typeof(Bottle); } }   

        [Constructable]
        public UOACZBottleOfLiquor(): base(2503)
        {
            Name = "bottle of liquor";
            Hue = 0;
            Weight = 1;
        }

        public UOACZBottleOfLiquor(Serial serial): base(serial)
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