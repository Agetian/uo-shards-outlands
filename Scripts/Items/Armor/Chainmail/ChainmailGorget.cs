using System;
using Server.Items;

namespace Server.Items
{
	public class ChainmailGorget : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 37; } }

        public override int IconItemId { get { return 5063; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 59; } }
        public override int IconOffsetY { get { return 43; } }		

        public override int ArmorBase { get { return 30; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.Quarter; } }

		[Constructable]
		public ChainmailGorget() : base( 5063 )
		{
            Name = "chainmail gorget";
            Hue = 2500;

			Weight = 1.0;
		}

		public ChainmailGorget( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}