using System;
using Server;

namespace Server.Items
{
	public class BlueBook : BaseBook
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public BlueBook() : base( 0xFF2, 40, true )
		{
            Name = "book";
		}

		[Constructable]
		public BlueBook( int pageCount, bool writable ) : base( 0xFF2, pageCount, writable )
		{
            Name = "book";
		}

		[Constructable]
		public BlueBook( string title, string author, int pageCount, bool writable ) : base( 0xFF2, title, author, pageCount, writable )
		{
            Name = "book";
		}

		public BlueBook( bool writable ) : base( 0xFF2, writable )
		{
            Name = "book";
		}

		public BlueBook( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}
	}
}
