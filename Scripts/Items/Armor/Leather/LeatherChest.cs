using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13cc, 0x13d3 )]
	public class LeatherChest : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 37; } }		

        public override int ArmorBase { get { return 15; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5075; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -1; } }
        public override int IconOffsetY { get { return -1; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public LeatherChest() : base( 5075 )
		{
            Name = "leather chest";
			Weight = 6.0;
		}

		public LeatherChest( Serial serial ) : base( serial )
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