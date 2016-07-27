using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class FlamestrikeScroll : SpellScroll
	{
        public override int IconItemId { get { return 8031; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public FlamestrikeScroll() : this( 1 )
		{
            Name = "flamestrike scroll";
		}

		[Constructable]
        public FlamestrikeScroll(int amount): base(50, 8031, amount)
		{
            Name = "flamestrike scroll";
		}

		public FlamestrikeScroll( Serial serial ) : base( serial )
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