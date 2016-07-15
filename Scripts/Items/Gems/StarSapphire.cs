using System;
using Server;

namespace Server.Items
{
	public class StarSapphire : Item
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public StarSapphire() : this( 1 )
		{
            Name = "star sapphire";
		}

		[Constructable]
		public StarSapphire( int amount ) : base( 0xF21 )
		{
            Name = "star sapphire";

			Stackable = true;
			Amount = amount;
		}

		public StarSapphire( Serial serial ) : base( serial )
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