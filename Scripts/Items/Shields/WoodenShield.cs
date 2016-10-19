using System;
using Server;

namespace Server.Items
{
    public class WoodenShield : BaseShield
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.WoodenShieldArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.WoodenShieldMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.WoodenShieldDurability; } }
        public override int InitMaxHits { get { return ArmorValues.WoodenShieldDurability; } }

        public override int IconItemId { get { return 7034; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 38; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

        [Constructable]
        public WoodenShield(): base(7034)
        {
            Name = "wooden shield";
            Weight = 4.0;
        }

        public WoodenShield(Serial serial): base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
