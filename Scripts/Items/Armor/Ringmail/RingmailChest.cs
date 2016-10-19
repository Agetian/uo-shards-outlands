using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13ec, 0x13ed )]
	public class RingmailChest : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.RingmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.RingmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.RingmailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.RingmailDurability; } }

        public override int IconItemId { get { return 5101; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 32; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public RingmailChest() : base( 5101 )
		{
            Name = "ringmail chest";
			Weight = 8.0;
		}

		public RingmailChest( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 15.0 )
				Weight = 8.0;
		}
	}
}