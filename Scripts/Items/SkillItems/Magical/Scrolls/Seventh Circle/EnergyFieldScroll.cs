using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class EnergyFieldScroll : SpellScroll
	{
        public override int IconItemId { get { return 8030; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 40; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public EnergyFieldScroll() : this( 1 )
		{
            Name = "energy field scroll";
		}

		[Constructable]
        public EnergyFieldScroll(int amount): base(49, 8029, amount)
		{
            Name = "energy field scroll";
		}

		public EnergyFieldScroll( Serial serial ) : base( serial )
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