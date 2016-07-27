using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class EarthquakeScroll : SpellScroll
	{
        public override int IconItemId { get { return 8037; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public EarthquakeScroll() : this( 1 )
		{
            Name = "earthquake scroll";
		}

		[Constructable]
		public EarthquakeScroll( int amount ) : base( 56, 8037, amount )
		{
            Name = "earthquake scroll";
		}

		public EarthquakeScroll( Serial serial ) : base( serial )
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