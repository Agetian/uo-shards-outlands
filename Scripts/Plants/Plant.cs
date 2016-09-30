using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class Plant : Item
    {        
        [Constructable]
        public Plant(): base(0x15FD)
        {
            Name = "a plant";

            Weight = 1;
        }

        public Plant(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            Plants.PlantDetail plantDetail = Plants.GetPlantDetail(this.GetType());

            from.SendMessage(plantDetail.Description);
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

    #region Plants

    public class LeafyBush : Plant
    {
        [Constructable]
        public LeafyBush(): base()
        {
            Name = "a leafy bush";

            ItemID = 13285;
            Weight = 5;
        }

        public LeafyBush(Serial serial): base(serial)
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

    public class PointyBush : Plant
    {
        [Constructable]
        public PointyBush(): base()
        {
            Name = "a pointy bush";

            ItemID = 13279;
            Weight = 5;
        }

        public PointyBush(Serial serial): base(serial)
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

    #endregion
}