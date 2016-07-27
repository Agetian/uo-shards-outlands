using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ParalyzeFieldScroll : SpellScroll
	{
        public override int IconItemId { get { return 8027; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 45; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public ParalyzeFieldScroll() : this( 1 )
		{
            Name = "paralyze field scroll";
		}

		[Constructable]
        public ParalyzeFieldScroll(int amount): base(46, 8027, amount)
		{
            Name = "paralyze field scroll";
		}

		public ParalyzeFieldScroll( Serial serial ) : base( serial )
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