using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class InvisibilityScroll : SpellScroll
	{
        public override int IconItemId { get { return 8024; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public InvisibilityScroll() : this( 1 )
		{
            Name = "invisbility scroll";
		}

		[Constructable]
        public InvisibilityScroll(int amount): base(43, 8024, amount)
		{
            Name = "invisbility scroll";
		}

		public InvisibilityScroll( Serial serial ) : base( serial )
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