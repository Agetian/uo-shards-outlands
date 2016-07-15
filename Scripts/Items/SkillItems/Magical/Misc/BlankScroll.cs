using System;

namespace Server.Items
{
	public class BlankScroll : Item, ICommodity
    {
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

		[Constructable]
		public BlankScroll() : this( 1 )
		{
            Name = "blank scroll";
		}

		[Constructable]
		public BlankScroll( int amount ) : base( 0xEF3 )
		{
            Name = "blank scroll";

			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

		public BlankScroll( Serial serial ) : base( serial )
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

            //Override weight to 0.1
            Weight = 0.1;
        }
	}
}