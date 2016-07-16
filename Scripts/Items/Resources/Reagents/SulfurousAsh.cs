using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SulfurousAsh : BaseReagent, ICommodity
	{
        public static int GetSBPurchaseValue() { return 3; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

		[Constructable]
		public SulfurousAsh() : this( 1 )
		{
            Name = "sulfurous ash";
		}

		[Constructable]
		public SulfurousAsh( int amount ) : base( 0xF8C, amount )
		{
            Name = "sulfurous ash";
		}

		public SulfurousAsh( Serial serial ) : base( serial )
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