using System;
using Server.Items;

namespace Server.Items
{
	public class BoneGorget : BaseArmor
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

        public override int ArmorBase { get { return ArmorValues.BoneBaseArmorValue; } }
        public override int OldDexBonus { get { return 0; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorValues.BoneMeditationAllowed; } }

        public override int InitMinHits { get { return ArmorValues.BoneDurability; } }
        public override int InitMaxHits { get { return ArmorValues.BoneDurability; } }

        public override int IconItemId { get { return 5139; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 56; } }
        public override int IconOffsetY { get { return 44; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Bone; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

		[Constructable]
		public BoneGorget() : base( 5139 )
		{
            Name = "bone gorget";
            Hue = 2958;

			Weight = 2.0;
		}

		public BoneGorget( Serial serial ) : base( serial )
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