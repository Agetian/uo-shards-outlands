using System;
using Server;

namespace Server.Items
{
	public class LesserHealPotion : BaseHealPotion
	{
        public static int GetSBPurchaseValue() { return 10; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override int MinHeal { get { return (Core.AOS ? 6 : 3); } }
		public override int MaxHeal { get { return (Core.AOS ? 8 : 10); } }
		public override double Delay{ get{ return (Core.AOS ? 3.0 : 10.0); } }

		[Constructable]
		public LesserHealPotion() : base( PotionEffect.HealLesser )
		{
            Name = "Lesser Heal potion";
		}

		public LesserHealPotion( Serial serial ) : base( serial )
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