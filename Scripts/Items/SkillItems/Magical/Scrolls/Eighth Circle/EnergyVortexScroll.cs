using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class EnergyVortexScroll : SpellScroll
	{
        public override int IconItemId { get { return 8038; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public EnergyVortexScroll() : this( 1 )
		{
            Name = "energy vortex scroll";
		}

		[Constructable]
		public EnergyVortexScroll( int amount ) : base( 57, 8038, amount )
		{
            Name = "energy vortex scroll";
		}

		public EnergyVortexScroll( Serial serial ) : base( serial )
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