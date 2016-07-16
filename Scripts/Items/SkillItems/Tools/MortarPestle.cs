using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	public class MortarPestle : BaseTool
	{
        public static int GetSBPurchaseValue() { return 10; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override CraftSystem CraftSystem{ get{ return DefAlchemy.CraftSystem; } }

		[Constructable]
		public MortarPestle() : base( 0xE9B )
		{
            Name = "mortar and pestle";
			Weight = 1.0;
		}

		[Constructable]
		public MortarPestle( int uses ) : base( uses, 0xE9B )
		{
            Name = "mortar and pestle";
			Weight = 1.0;
		}

		public MortarPestle( Serial serial ) : base( serial )
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