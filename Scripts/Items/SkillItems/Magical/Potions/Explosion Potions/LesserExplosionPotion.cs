using System;
using Server;

namespace Server.Items
{
	public class LesserExplosionPotion : BaseExplosionPotion
	{
        public static int GetSBPurchaseValue() { return 10; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override int MinDamage { get { return 5; } }
		public override int MaxDamage { get { return 10; } }

		[Constructable]
		public LesserExplosionPotion() : base( PotionEffect.ExplosionLesser )
		{
            Name = "Lesser Explosion potion";
		}

		public LesserExplosionPotion( Serial serial ) : base( serial )
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