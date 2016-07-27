using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class PolymorphScroll : SpellScroll
	{
        public override int IconItemId { get { return 8036; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public PolymorphScroll() : this( 1 )
		{
            Name = "polymorph scroll";
		}

		[Constructable]
		public PolymorphScroll( int amount ) : base( 55, 8036, amount )
		{
            Name = "polymorph scroll";
		}

		public PolymorphScroll( Serial serial ) : base( serial )
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