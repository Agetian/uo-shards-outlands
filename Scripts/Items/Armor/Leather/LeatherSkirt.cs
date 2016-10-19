using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1c08, 0x1c09 )]
	public class LeatherSkirt : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.LeatherBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.LeatherMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.LeatherDurability; } }
        public override int InitMaxHits { get { return ArmorValues.LeatherDurability; } }

        public override int IconItemId { get { return 7177; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 43; } }
        public override int IconOffsetY { get { return 41; } }        

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override bool AllowMaleWearer{ get{ return false; } }

		[Constructable]
		public LeatherSkirt() : base( 7177 )
		{
            Name = "leather skirt";
			Weight = 1.0;
		}

		public LeatherSkirt( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );

			if ( Weight == 3.0 )
				Weight = 1.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}