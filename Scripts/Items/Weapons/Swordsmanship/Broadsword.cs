using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF5E, 0xF5F )]
	public class Broadsword : BaseSword
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int BaseMinDamage { get { return 14; } }
        public override int BaseMaxDamage { get { return 24; } }
        public override int BaseSpeed { get { return 50; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 3935; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 55; } }
        public override int IconOffsetY { get { return 40; } }

		[Constructable]
		public Broadsword() : base( 0xF5E )
		{
            Name = "broadsword";

			Weight = 4.0;
		}

		public Broadsword( Serial serial ) : base( serial )
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