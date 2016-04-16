using System;
using Server;

namespace Server.Items
{
	public class Bascinet : BaseArmor
	{
		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 60; } }

        public override int ArmorBase { get { return 40; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5132; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 4; } }

        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.None; } }

		[Constructable]
		public Bascinet() : base( 5132 )
		{
            Name = "bascinet";
			Weight = 5.0;
		}

		public Bascinet( Serial serial ) : base( serial )
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