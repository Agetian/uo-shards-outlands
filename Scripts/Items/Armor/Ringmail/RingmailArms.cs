using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13ee, 0x13ef )]
	public class RingmailArms : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

		public override int InitMinHits{ get{ return 40; } }
		public override int InitMaxHits{ get{ return 50; } }

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5103; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 4; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Half; } }

		[Constructable]
		public RingmailArms() : base( 5103 )
		{
            Name = "ringmail arms";
			Weight = 3.0;
		}

		public RingmailArms( Serial serial ) : base( serial )
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
				Weight = 3.0;
		}
	}
}