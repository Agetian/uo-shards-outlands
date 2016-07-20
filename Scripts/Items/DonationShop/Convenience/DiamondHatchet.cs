using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF43, 0xF44 )]
    public class DiamondHatchet : Hatchet
	{
		[Constructable]
		public DiamondHatchet() : base()
		{
            Name = "diamond hatchet";
            Hue = 2500;

			Weight = 3.0;

            UsesRemaining = 500;
            ShowUsesRemaining = true;

            LootType = LootType.Blessed;

            
		}

        public DiamondHatchet(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}