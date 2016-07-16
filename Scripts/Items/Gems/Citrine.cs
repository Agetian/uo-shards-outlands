using System;
using Server;

namespace Server.Items
{
	public class Citrine : Item
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Citrine() : this( 1 )
		{
            Name = "citrine";
		}

		[Constructable]
		public Citrine( int amount ) : base( 0xF15 )
		{
            Name = "citrine";

			Stackable = true;
			Amount = amount;
		}

		public Citrine( Serial serial ) : base( serial )
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