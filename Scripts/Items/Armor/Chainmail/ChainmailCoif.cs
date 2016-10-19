using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x13BB, 0x13C0 )]
	public class ChainmailCoif : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.ChainmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.ChainmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.ChainDurability; } }
        public override int InitMaxHits { get { return ArmorValues.ChainDurability; } }

        public override int IconItemId { get { return 5056; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 48; } }
        public override int IconOffsetY { get { return 42; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public ChainmailCoif() : base( 5056 )
		{
            Name = "chainmail coif";
			Weight = 1.0;
		}

		public ChainmailCoif( Serial serial ) : base( serial )
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