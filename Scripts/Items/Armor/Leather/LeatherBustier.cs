using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c0a, 0x1c0b )]
	public class LeatherBustier : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.LeatherBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.LeatherMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.LeatherDurability; } }
        public override int InitMaxHits { get { return ArmorValues.LeatherDurability; } }

        public override int IconItemId { get { return 7179; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 54; } }
        public override int IconOffsetY { get { return 37; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		[Constructable]
		public LeatherBustier() : base( 7179 )
		{
            Name = "leather bustier";
			Weight = 4.0;
		}

		public LeatherBustier( Serial serial ) : base( serial )
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