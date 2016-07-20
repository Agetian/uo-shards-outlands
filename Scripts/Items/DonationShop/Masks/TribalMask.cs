using System;
using Server;

namespace Server.Items
{
    [FlipableAttribute(5451, 5452)]
	public class TribalMask : BaseArmor
	{
		public override int InitMinHits{ get{ return 250; } }
		public override int InitMaxHits{ get{ return 250; } }		

        public override int ArmorBase { get { return 25; } }
        public override int OldDexBonus { get { return 0; } }

        public override int IconItemId { get { return 5451; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 0; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
        public TribalMask(): base(5451)
		{
            Name = "tribal mask";
			Weight = 2.0;

            LootType = LootType.Blessed;

            
		}

		public TribalMask( Serial serial ) : base( serial )
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