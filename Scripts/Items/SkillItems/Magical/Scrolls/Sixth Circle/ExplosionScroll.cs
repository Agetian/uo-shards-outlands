using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ExplosionScroll : SpellScroll
	{
        public override int IconItemId { get { return 8023; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public ExplosionScroll() : this( 1 )
		{
            Name = "explosion scroll";
		}

		[Constructable]
        public ExplosionScroll(int amount): base(42, 8023, amount)
		{
            Name = "explosion scroll";
		}

		public ExplosionScroll( Serial serial ) : base( serial )
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