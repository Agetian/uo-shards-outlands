using System;

namespace Server.Items
{
	public class Beeswax : Item
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public Beeswax() : this( 1 )
		{
            Name = "beeswax";
		}

		[Constructable]
		public Beeswax( int amount ) : base( 0x1422 )
		{
            Name = "beeswax";

			Weight = 1.0;
			Stackable = true;
			Amount = amount;
		}

		public Beeswax( Serial serial ) : base( serial )
		{
		}

		

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}