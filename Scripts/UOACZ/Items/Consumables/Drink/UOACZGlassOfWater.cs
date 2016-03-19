using System;
using Server;

namespace Server.Items
{
    public class UOACZGlassOfWater : UOACZConsumptionItem
    {
        public override ConsumptionMode ConsumptionType { get { return ConsumptionMode.Drink; } }

        public override int HitsChange { get { return 0; } }
        public override int StamChange { get { return 0; } }
        public override int ManaChange { get { return 0; } }

        public override int HungerChange { get { return 0; } }
        public override int ThirstChange { get { return Utility.RandomMinMax(4, 6); } }
        public override ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } } 

        public override int MaxCharges { get { return 1; } }

        public override Type DropContainer { get { return typeof(UOACZGlass); } }   

        [Constructable]
        public UOACZGlassOfWater(): base(8081)
        {
            Name = "glass of water";
            Hue = 0;
            Weight = 1;
        }

        public UOACZGlassOfWater(Serial serial): base(serial)
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