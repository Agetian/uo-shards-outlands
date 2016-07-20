using System;
using Server;

namespace Server.Items
{
	public class OrcHelmMask : BaseArmor
	{
        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 7947; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -5; } }
        public override int IconOffsetY { get { return 5; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public OrcHelmMask() : base( 7947 )
		{
            Name = "orc helm (mask)";
			Weight = 2.0;

            LootType = LootType.Blessed;

            
		}

		public OrcHelmMask( Serial serial ) : base( serial )
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