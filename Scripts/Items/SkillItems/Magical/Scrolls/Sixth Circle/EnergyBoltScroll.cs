using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class EnergyBoltScroll : SpellScroll
	{
        public override int IconItemId { get { return 8022; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 40; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public EnergyBoltScroll() : this( 1 )
		{
            Name = "energy bolt scroll";
		}

		[Constructable]
		public EnergyBoltScroll( int amount ) : base( 41, 8022, amount )
		{
            Name = "energy bolt scroll";
		}

		public EnergyBoltScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		
	}
}