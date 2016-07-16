using System;
using Server;

namespace Server.Items
{
	public class TanBook : BaseBook
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public TanBook() : base( 0xFF0 )
		{
            Name = "book";
		}

		[Constructable]
		public TanBook( int pageCount, bool writable ) : base( 0xFF0, pageCount, writable )
		{
            Name = "book";
		}

		[Constructable]
		public TanBook( string title, string author, int pageCount, bool writable ) : base( 0xFF0, title, author, pageCount, writable )
		{
            Name = "book";
		}

		public TanBook( bool writable ) : base( 0xFF0, writable )
		{
            Name = "book";
		}

		public TanBook( Serial serial ) : base( serial )
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