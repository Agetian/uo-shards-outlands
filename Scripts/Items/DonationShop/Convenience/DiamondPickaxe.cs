using System;
using Server.Items;
using Server.Network;
using Server.Engines.Harvest;

namespace Server.Items
{
	[FlipableAttribute( 0xE86, 0xE85 )]
    public class DiamondPickaxe : Pickaxe
	{
        [Constructable]
        public DiamondPickaxe(): base()
        {
            Name = "diamond pickaxe";
            Hue = 2500;

            Weight = 3.0;

            UsesRemaining = 5000;
            ShowUsesRemaining = true;

            LootType = LootType.Blessed;

            
        }

        public DiamondPickaxe(Serial serial): base(serial)
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