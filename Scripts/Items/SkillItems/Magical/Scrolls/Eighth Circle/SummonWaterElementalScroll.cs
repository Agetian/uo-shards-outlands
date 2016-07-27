using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SummonWaterElementalScroll : SpellScroll
	{
        public override int IconItemId { get { return 8044; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public SummonWaterElementalScroll() : this( 1 )
		{
            Name = "summon water elemental scroll";
		}

		[Constructable]
		public SummonWaterElementalScroll( int amount ) : base( 63, 8044, amount )
		{
            Name = "summon water elemental scroll";
		}

		public SummonWaterElementalScroll( Serial serial ) : base( serial )
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