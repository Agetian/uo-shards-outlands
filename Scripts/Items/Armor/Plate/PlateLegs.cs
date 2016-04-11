using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1411, 0x141a )]
	public class PlateLegs : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 90; } }

		public override int OldStrReq{ get{ return 60; } }
       
        public override int RevertArmorBase{ get{ return 4; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return -3; } }

        public override int IconItemId { get { return 5146; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 4; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public PlateLegs() : base( 5146 )
		{
            Name = "platemail legs";
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