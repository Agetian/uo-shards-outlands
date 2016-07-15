using System;
using Server;

namespace Server.Items
{
	public class Diamond : Item
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Diamond() : this( 1 )
		{
            Name = "diamond";
		}

		[Constructable]
		public Diamond( int amount ) : base( 0xF26 )
		{
            Name = "diamond";

			Stackable = true;
			Amount = amount;
		}

		public Diamond( Serial serial ) : base( serial )
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