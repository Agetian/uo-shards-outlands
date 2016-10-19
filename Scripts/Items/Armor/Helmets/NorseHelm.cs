using System;
using Server;

namespace Server.Items
{
	public class NorseHelm : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.PlatemailBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.PlatemailMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.PlatemailDurability; } }
        public override int InitMaxHits { get { return ArmorValues.PlatemailDurability; } }

        public override CraftResource DefaultResource { get { return CraftResource.Iron; } }
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

		[Constructable]
		public NorseHelm() : base( 5135 )
		{
            Name = "norse helm";
			Weight = 5.0;
		}

		public NorseHelm( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 5.0;
		}
	}
}