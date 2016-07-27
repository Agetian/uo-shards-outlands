using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SummonFireElementalScroll : SpellScroll
	{
        public override int IconItemId { get { return 8043; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public SummonFireElementalScroll() : this( 1 )
		{
            Name = "summon fire elemental scroll";
		}

		[Constructable]
		public SummonFireElementalScroll( int amount ) : base( 62, 8043, amount )
		{
            Name = "summon fire elemental scroll";
		}

		public SummonFireElementalScroll( Serial serial ) : base( serial )
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