using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class DispelScroll : SpellScroll
	{
        public override int IconItemId { get { return 8021; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public DispelScroll() : this( 1 )
		{
            Name = "dispel scroll";
		}

		[Constructable]
		public DispelScroll( int amount ) : base( 40, 8021, amount )
		{
            Name = "dispel scroll";
		}

		public DispelScroll( Serial serial ) : base( serial )
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