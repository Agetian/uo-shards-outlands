using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SummonDaemonScroll : SpellScroll
	{
        public override int IconItemId { get { return 8041; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public SummonDaemonScroll() : this( 1 )
		{
            Name = "summon daemon scroll";
		}

		[Constructable]
		public SummonDaemonScroll( int amount ) : base( 60, 8041, amount )
		{
            Name = "summon daemon scroll";
		}

		public SummonDaemonScroll( Serial serial ) : base( serial )
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