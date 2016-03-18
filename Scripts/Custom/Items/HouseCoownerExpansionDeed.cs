using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class HouseCoownerExpansionDeed : Item
    {
        public static int CoownerIncreaseAmount = 3;

        [Constructable]
        public HouseCoownerExpansionDeed(): base(0x14F0)
        {
            Name = "a housing co-owner expansion deed";
            Hue = 2220;
        }

        public HouseCoownerExpansionDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + CoownerIncreaseAmount.ToString() + " co-owners)");               
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(from.Location, from.Map, 64);

            if (house == null)
            {
                from.SendMessage("You must be inside a house in order to use this deed.");
                return;
            }

            if (house.Owner != from)
            {
                from.SendMessage("Only the owner of this house may use this deed.");
                return;
            }

            house.MaxCoOwners += CoownerIncreaseAmount;

            from.SendMessage("You increase the maximum number of co-owners available to the house by " + CoownerIncreaseAmount.ToString() + ".");

            from.PlaySound(0x0F7);
            from.FixedParticles(0x373A, 10, 15, 5012, 2587, 0, EffectLayer.Waist);

            Delete();
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
        }
    }
}