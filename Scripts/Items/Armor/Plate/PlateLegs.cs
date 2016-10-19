using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1411, 0x141a )]
	public class PlateLegs : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.PlatemailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.PlatemailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.PlatemailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.PlatemailDurability; } }

        public override int IconItemId { get { return 5146; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 53; } }
        public override int IconOffsetY { get { return 35; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public PlateLegs() : base( 5146 )
		{
            Name = "platemail leggings";
			Weight = 7.0;            
		}

		public PlateLegs( Serial serial ) : base( serial )
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