using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class Skillet : BaseTool
	{
        public static int GetSBPurchaseValue() { return 10; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override CraftSystem CraftSystem{ get{ return DefCooking.CraftSystem; } }

		[Constructable]
		public Skillet() : base( 0x97F )
		{
            Name = "skillet";
			Weight = 1.0;
		}

		[Constructable]
		public Skillet( int uses ) : base( uses, 0x97F )
		{
            Name = "skillet";
			Weight = 1.0;
		}

		public Skillet( Serial serial ) : base( serial )
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