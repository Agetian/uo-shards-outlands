using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class GateTravelScroll : SpellScroll
	{
        public override int IconItemId { get { return 8032; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public GateTravelScroll() : this( 1 )
		{
            Name = "gate travel scroll";
		}

		[Constructable]
        public GateTravelScroll(int amount): base(51, 8032, amount)
		{
            Name = "gate travel scroll";
		}

		public GateTravelScroll( Serial serial ) : base( serial )
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