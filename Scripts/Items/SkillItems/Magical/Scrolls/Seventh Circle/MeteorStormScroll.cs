using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MeteorSwarmScroll : SpellScroll
	{
        public override int IconItemId { get { return 8035; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public MeteorSwarmScroll() : this( 1 )
		{
            Name = "meteor swarm scroll";
		}

		[Constructable]
		public MeteorSwarmScroll( int amount ) : base( 54, 8035, amount )
		{
            Name = "meteor swarm scroll";
		}

		public MeteorSwarmScroll( Serial serial ) : base( serial )
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