using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class MarkScroll : SpellScroll
	{
        public override int IconItemId { get { return 8025; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public MarkScroll() : this( 1 )
		{
            Name = "mark scroll";
		}

		[Constructable]
        public MarkScroll(int amount): base(44, 8025, amount)
		{
            Name = "mark scroll";
		}

		public MarkScroll( Serial serial ) : base( serial )
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