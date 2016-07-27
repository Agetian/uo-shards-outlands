using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class ManaVampireScroll : SpellScroll
	{
        public override int IconItemId { get { return 8033; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public ManaVampireScroll() : this( 1 )
		{
            Name = "mana vampire scroll";
		}

		[Constructable]
        public ManaVampireScroll(int amount): base(52, 8033, amount)
		{
            Name = "mana vampire scroll";
		}

		public ManaVampireScroll( Serial serial ) : base( serial )
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