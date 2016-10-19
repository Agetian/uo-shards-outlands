using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13eb, 0x13f2 )]
	public class ChainmailGloves : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.ChainmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.ChainmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.ChainDurability; } }
        public override int InitMaxHits { get { return ArmorValues.ChainDurability; } }

        public override int IconItemId { get { return 5106; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 52; } }
        public override int IconOffsetY { get { return 37; } }

        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }

		[Constructable]
		public ChainmailGloves() : base( 5106 )
		{
            Name = "chainmail gloves";
            Hue = 2500;

			Weight = 2.0;
		}

		public ChainmailGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}