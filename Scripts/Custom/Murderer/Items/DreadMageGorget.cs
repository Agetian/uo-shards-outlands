using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class DreadMageGorget : BaseArmor
	{
        public override int PlayerClassCurrencyValue { get { return 100; } }

		public override int BasePhysicalResistance{ get{ return 2; } }
		public override int BaseFireResistance{ get{ return 4; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 3; } }
       
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 75; } }

        public override int AosStrReq { get { return 20; } }
        public override int OldStrReq { get { return 10; } }

        public override int ArmorBase { get { return 33; } }        
        public override int RevertArmorBase { get { return 1; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		[Constructable]
		public DreadMageGorget() : base( 0x13C7 )
		{
			Weight = 1.0;            
            Name = "Dread Mage Gorget";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public DreadMageGorget(Serial serial): base(serial)
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