using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1415, 0x1416 )]
	public class PlateChest : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 2; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 2; } }
        
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 95; } }
		public override int OldStrReq{ get{ return 60; } }
		
        public override int RevertArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return -5; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public PlateChest() : base( 0x1415 )
		{
            Name = "platemail chest";
			Weight = 10.0;
		}

		public PlateChest( Serial serial ) : base( serial )
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
				Weight = 10.0;
		}
	}
}