using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ChainLightningScroll : SpellScroll
	{
        public override int IconItemId { get { return 8029; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public ChainLightningScroll() : this( 1 )
		{
            Name = "chain lightning scroll";
		}

		[Constructable]
        public ChainLightningScroll(int amount): base(48, 8029, amount)
		{
            Name = "chain lightning scroll";
		}

		public ChainLightningScroll( Serial serial ) : base( serial )
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