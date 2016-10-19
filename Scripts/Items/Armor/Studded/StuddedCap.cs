using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1db9, 0x1dba )]
	public class StuddedCap : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.StuddedLeatherBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.StuddedLeatherMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.StuddedLeatherDurability; } }
        public override int InitMaxHits { get { return ArmorValues.StuddedLeatherDurability; } }

        public override int IconItemId { get { return 7610; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 51; } }
        public override int IconOffsetY { get { return 43; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Studded; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

		[Constructable]
		public StuddedCap() : base( 7610 )
		{
            Name = "studded cap";
            Hue = 1507;

			Weight = 2.0;
		}

		public StuddedCap( Serial serial ) : base( serial )
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
				Weight = 2.0;
		}
	}
}