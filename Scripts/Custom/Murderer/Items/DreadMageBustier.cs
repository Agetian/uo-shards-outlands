using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAttribute(0x1c0a, 0x1c0b)]
	public class DreadMageBustier : BaseArmor
	{
        public override int PlayerClassCurrencyValue { get { return 200; } }

		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }
        
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 75; } }

        public override int AosStrReq { get { return 25; } }
        public override int OldStrReq { get { return 15; } }

        public override int ArmorBase { get { return 33; } }        
        public override int RevertArmorBase { get { return 9; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		[Constructable]
        public DreadMageBustier(): base(0x1c0a)
		{
			Weight = 6.0;            
            Name = "Dread Mage Bustier";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public DreadMageBustier(Serial serial): base(serial)
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
				Weight = 6.0;
		}
	}
}