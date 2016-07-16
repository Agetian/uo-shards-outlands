using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0x0FBF, 0x0FC0 )]
	public class ScribesPen : BaseTool
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override CraftSystem CraftSystem{ get{ return DefInscription.CraftSystem; } }

		[Constructable]
		public ScribesPen() : base( 0x0FBF )
		{
            Name = "scribe's pen";
			Weight = 1.0;
		}

		[Constructable]
		public ScribesPen( int uses ) : base( uses, 0x0FBF )
		{
            Name = "scribe's pen";
			Weight = 1.0;
		}

		public ScribesPen( Serial serial ) : base( serial )
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

			if ( Weight == 2.0 )
				Weight = 1.0;
		}
	}
}