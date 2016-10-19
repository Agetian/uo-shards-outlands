using System;
using Server;

namespace Server.Items
{
	public class OrcHelm : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.RingmailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.RingmailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.RingmailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.RingmailDurability; } }

        public override int IconItemId { get { return 7947; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 46; } }
        public override int IconOffsetY { get { return 42; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Ringmail; } }
        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }

		[Constructable]
		public OrcHelm() : base( 7947 )
		{
            Name = "orc helm";
			Weight = 2.0;
		}

		public OrcHelm( Serial serial ) : base( serial )
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
		}
	}
}