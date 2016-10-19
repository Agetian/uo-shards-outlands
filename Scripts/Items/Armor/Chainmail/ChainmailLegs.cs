using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13be, 0x13c3 )]
	public class ChainmailLegs : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.ChainmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.ChainmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.ChainDurability; } }
        public override int InitMaxHits { get { return ArmorValues.ChainDurability; } }

        public override int IconItemId { get { return 5059; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 54; } }
        public override int IconOffsetY { get { return 32; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Chainmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public ChainmailLegs() : base( 5059 )
		{
            Name = "chainmail leggings";
			Weight = 7.0;
		}

		public ChainmailLegs( Serial serial ) : base( serial )
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