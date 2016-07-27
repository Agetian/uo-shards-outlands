using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MassDispelScroll : SpellScroll
	{
        public override int IconItemId { get { return 8034; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public MassDispelScroll() : this( 1 )
		{
            Name = "mass dispel scroll";
		}

		[Constructable]
		public MassDispelScroll( int amount ) : base( 53, 8034, amount )
		{
            Name = "mass dispel scroll";
		}

		public MassDispelScroll( Serial serial ) : base( serial )
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