using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13ee, 0x13ef )]
	public class RingmailArms : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 3; } }
		public override int BaseFireResistance{ get{ return 3; } }
		public override int BaseColdResistance{ get{ return 1; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		public override int InitMinHits{ get{ return 40; } }
		public override int InitMaxHits{ get{ return 50; } }

		public override int AosStrReq{ get{ return 40; } }
		public override int OldStrReq{ get{ return 20; } }
       
        public override int RevertArmorBase{ get{ return 2; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Ringmail; } }

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Half; } }

		[Constructable]
		public RingmailArms() : base( 0x13EE )
		{
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