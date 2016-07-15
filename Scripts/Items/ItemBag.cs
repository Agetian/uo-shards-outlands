using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
    #region Armor

    public class BagOfLeatherArmor : Bag
    {
        [Constructable]
        public BagOfLeatherArmor()
        {
            List<Type> m_Types = new List<Type>() 
            {
                typeof(LeatherCap), 
                typeof(LeatherGorget),
                typeof(LeatherArms),
                typeof(LeatherGloves),
                typeof(LeatherChest),
                typeof(LeatherLegs),
            };

            foreach (Type type in m_Types)
            {
                BaseArmor armor = (BaseArmor)Activator.CreateInstance(type);

                if (armor == null)
                    continue;

                armor.Quality = Quality.Exceptional;
                DropItem(armor);
            }
        }

        public BagOfLeatherArmor(Serial serial): base(serial)
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

    public class BagOfStuddedLeatherArmor : Bag
    {
        [Constructable]
        public BagOfStuddedLeatherArmor()
        {
            List<Type> m_Types = new List<Type>() 
            {
                typeof(StuddedCap), 
                typeof(StuddedGorget),
                typeof(StuddedArms),
                typeof(StuddedGloves),
                typeof(StuddedChest),
                typeof(StuddedLegs),
            };

            foreach (Type type in m_Types)
            {
                BaseArmor armor = (BaseArmor)Activator.CreateInstance(type);

                if (armor == null)
                    continue;

                armor.Quality = Quality.Exceptional;
                DropItem(armor);
            }
        }

        public BagOfStuddedLeatherArmor(Serial serial): base(serial)
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

    public class BagOfBoneArmor : Bag
    {
        [Constructable]
        public BagOfBoneArmor()
        {
            List<Type> m_Types = new List<Type>() 
            {
                typeof(BoneHelm), 
                typeof(BoneGorget),
                typeof(BoneArms),
                typeof(BoneGloves),
                typeof(BoneChest),
                typeof(BoneLegs),
            };

            foreach (Type type in m_Types)
            {
                BaseArmor armor = (BaseArmor)Activator.CreateInstance(type);

                if (armor == null)
                    continue;

                armor.Quality = Quality.Exceptional;
                DropItem(armor);
            }
        }

        public BagOfBoneArmor(Serial serial): base(serial)
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

    public class BagOfRingmailArmor : Bag
    {
        [Constructable]
        public BagOfRingmailArmor()
        {
            List<Type> m_Types = new List<Type>() 
            {
                typeof(RingmailHelm), 
                typeof(RingmailGorget),
                typeof(RingmailArms),
                typeof(RingmailGloves),
                typeof(RingmailChest),
                typeof(RingmailLegs),
            };

            foreach (Type type in m_Types)
            {
                BaseArmor armor = (BaseArmor)Activator.CreateInstance(type);

                if (armor == null)
                    continue;

                armor.Quality = Quality.Exceptional;
                DropItem(armor);
            }
        }

        public BagOfRingmailArmor(Serial serial): base(serial)
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

    public class BagOfChainmailArmor : Bag
    {
        [Constructable]
        public BagOfChainmailArmor()
        {
            List<Type> m_Types = new List<Type>() 
            {
                typeof(ChainmailCoif), 
                typeof(ChainmailGorget),
                typeof(ChainmailArms),
                typeof(ChainmailGloves),
                typeof(ChainmailChest),
                typeof(ChainmailLegs),
            };

            foreach (Type type in m_Types)
            {
                BaseArmor armor = (BaseArmor)Activator.CreateInstance(type);

                if (armor == null)
                    continue;

                armor.Quality = Quality.Exceptional;
                DropItem(armor);
            }            
        }

        public BagOfChainmailArmor(Serial serial): base(serial)
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

    public class BagOfPlateArmor : Bag
    {
        [Constructable]
        public BagOfPlateArmor()
        {
            List<Type> m_Types = new List<Type>() 
            {
                typeof(PlateHelm), 
                typeof(PlateGorget),
                typeof(PlateArms),
                typeof(PlateGloves),
                typeof(PlateChest),
                typeof(PlateLegs),
            };

            foreach (Type type in m_Types)
            {
                BaseArmor armor = (BaseArmor)Activator.CreateInstance(type);

                if (armor == null)
                    continue;

                armor.Quality = Quality.Exceptional;
                DropItem(armor);
            }
        }

        public BagOfPlateArmor(Serial serial): base(serial)
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

    #region Regeants

    public class BagOfRegeants : Bag
    {
        [Constructable]
        public BagOfRegeants()
        {
            int amount = 50;

            List<Type> m_Types = new List<Type>() 
            {
                typeof(BlackPearl), 
                typeof(Bloodmoss),
                typeof(MandrakeRoot),
                typeof(Garlic),
                typeof(Ginseng),
                typeof(SpidersSilk),
                typeof(Nightshade),
                typeof(SulfurousAsh),
            };

            foreach (Type type in m_Types)
            {
                Item item = (Item)Activator.CreateInstance(type);

                if (item == null)
                    continue;

                item.Amount = amount;
                DropItem(item);
            }
        }

        public BagOfRegeants(Serial serial): base(serial)
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
