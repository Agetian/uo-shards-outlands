using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1450, 0x1455 )]
	public class BoneGloves : BaseArmor
	{
		public override int InitMinHits{ get{ return 16; } }
		public override int InitMaxHits{ get{ return 25; } }

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5205; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Bone; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

		[Constructable]
		public BoneGloves() : base( 5205 )
		{
            Name = "bone gloves";
			Weight = 2.0;
		}

		public BoneGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}