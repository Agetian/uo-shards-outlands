using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class RevealScroll : SpellScroll
	{
        public override int IconItemId { get { return 8028; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public RevealScroll() : this( 1 )
		{
            Name = "reveal scroll";
		}

		[Constructable]
        public RevealScroll(int amount): base(47, 8028, amount)
		{
            Name = "reveal scroll";
		}

		public RevealScroll( Serial serial ) : base( serial )
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