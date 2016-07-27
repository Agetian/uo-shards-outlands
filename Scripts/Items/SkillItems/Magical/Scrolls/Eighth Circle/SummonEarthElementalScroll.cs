using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SummonEarthElementalScroll : SpellScroll
	{
        public override int IconItemId { get { return 8042; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public SummonEarthElementalScroll() : this( 1 )
		{
            Name = "summon earth elemental scroll";
		}

		[Constructable]
		public SummonEarthElementalScroll( int amount ) : base( 61, 8042, amount )
		{
            Name = "summon earth elemental scroll";
		}

		public SummonEarthElementalScroll( Serial serial ) : base( serial )
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