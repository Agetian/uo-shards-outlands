using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c02, 0x1c03 )]
	public class FemaleStuddedChest : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.StuddedLeatherBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.StuddedLeatherMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.StuddedLeatherDurability; } }
        public override int InitMaxHits { get { return ArmorValues.StuddedLeatherDurability; } }

        public override int IconItemId { get { return 7170; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 48; } }
        public override int IconOffsetY { get { return 32; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Studded; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		[Constructable]
		public FemaleStuddedChest() : base( 7170 )
		{
            Name = "female studded chest";
			Weight = 6.0;
		}

		public FemaleStuddedChest( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
				Weight = 6.0;
		}
	}
}