using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1452, 0x1457 )]
	public class BoneLegs : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 2; } }
		public override int BaseEnergyResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 16; } }
		public override int InitMaxHits{ get{ return 25; } }

		public override int AosStrReq{ get{ return 55; } }
		public override int OldStrReq{ get{ return 40; } }
               
		public override int RevertArmorBase{ get{ return 4; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Bone; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return -1; } }

        public override int IconItemId { get { return 5207; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return 4; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

		[Constructable]
		public BoneLegs() : base( 5207 )
		{
			Weight = 3.0;
		}

		public BoneLegs( Serial serial ) : base( serial )
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
		}
	}
}