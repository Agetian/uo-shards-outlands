using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[Flipable( 0x102C, 0x102D )]
	public class WoodenPlane : BaseTool
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return 1; }

		public override CraftSystem CraftSystem{ get{ return DefCarpentry.CraftSystem; } }

		[Constructable]
		public WoodenPlane() : base( 0x102C )
		{
            Name = "wooden plane";
			Weight = 2.0;
		}

		[Constructable]
		public WoodenPlane( int uses ) : base( uses, 0x102C )
		{
            Name = "wooden plane";
			Weight = 2.0;
		}

		public WoodenPlane( Serial serial ) : base( serial )
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