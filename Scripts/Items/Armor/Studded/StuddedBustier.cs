using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c0c, 0x1c0d )]
	public class StuddedBustier : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 45; } }
		
		public override int ArmorBase{ get{ return 20; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 7181; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 54; } }
        public override int IconOffsetY { get { return 37; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Studded; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.ThreeQuarter; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		[Constructable]
		public StuddedBustier() : base( 7181 )
		{
            Name = "studded bustier";
			Weight = 4.0;
		}

		public StuddedBustier( Serial serial ) : base( serial )
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
		}
	}
}