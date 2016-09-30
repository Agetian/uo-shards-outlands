using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class PlantSeed : Item
    {
        [Constructable]
        public PlantSeed(): base(22326)
        {
            Name = "a plant seed";

            Stackable = true;
            Amount = 1;
            Weight = .1;        
        }

        [Constructable]
        public PlantSeed(int amount): base(22326)
        {
            Name = "a plant seed";

            Stackable = true;
            Amount = amount;
            Weight = .1;
        }

        public PlantSeed(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            Plants.SeedDetail seedDetail = Plants.GetSeedDetail(this.GetType());

            if (Amount > 1)
                LabelTo(from, seedDetail.Name + " : " + Amount.ToString());
            else
                LabelTo(from, seedDetail.Name);

            LabelTo(from, "(place in plant bowl to grow)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            Plants.SeedDetail seedDetail = Plants.GetSeedDetail(this.GetType());

            from.SendMessage(seedDetail.Description);
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

    #region Seeds

    public class LeafySeed : PlantSeed
    {
        [Constructable]
        public LeafySeed(): base()
        {
            Name = "a leafy seed";       
        }

        [Constructable]
        public LeafySeed(int amount): base(amount)
        {
            Name = "a leafy seed";

            Amount = amount;
        }

        public LeafySeed(Serial serial): base(serial)
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