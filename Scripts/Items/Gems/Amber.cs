using System;
using Server;

namespace Server.Items
{
	public class Amber : Item
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Amber() : this( 1 )
		{
            Name = "amber";
		}

		[Constructable]
		public Amber( int amount ) : base( 0xF25 )
		{
            Name = "amber";

			Stackable = true;
			Amount = amount;
		}

		public Amber( Serial serial ) : base( serial )
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