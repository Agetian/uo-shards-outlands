using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SpidersSilk : BaseReagent, ICommodity
	{
        public static int GetSBPurchaseValue() { return 3; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

		[Constructable]
		public SpidersSilk() : this( 1 )
		{
            Name = "spider's silk";
		}

		[Constructable]
		public SpidersSilk( int amount ) : base( 0xF8D, amount )
		{
            Name = "spider's silk";
		}

		public SpidersSilk( Serial serial ) : base( serial )
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