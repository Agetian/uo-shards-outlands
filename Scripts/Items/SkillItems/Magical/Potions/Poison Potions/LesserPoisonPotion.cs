using System;
using Server;

namespace Server.Items
{
	public class LesserPoisonPotion : BasePoisonPotion
	{
        public static int GetSBPurchaseValue() { return 10; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override Poison Poison{ get{ return Poison.Lesser; } }

		public override double MinPoisoningSkill{ get{ return 0.0; } }
		public override double MaxPoisoningSkill{ get{ return 60.0; } }

		[Constructable]
		public LesserPoisonPotion() : base( PotionEffect.PoisonLesser )
		{
            Name = "Lesser Poison potion";
		}

		public LesserPoisonPotion( Serial serial ) : base( serial )
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