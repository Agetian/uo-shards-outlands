using System;
using Server;

namespace Server.Items
{
	public class PlateHelm : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }

		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 65; } }

		public override int AosStrReq{ get{ return 80; } }
		public override int OldStrReq{ get{ return 40; } }
              
        public override int RevertArmorBase{ get{ return 4; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return -1; } }

        public override int IconItemId { get { return 5138; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 6; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public PlateHelm() : base( 5138 )
		{
            Name = "platemail helm";
			Weight = 5.0;
		}

		public PlateHelm( Serial serial ) : base( serial )
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
				Weight = 5.0;
		}
	}
}