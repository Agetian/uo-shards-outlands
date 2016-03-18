using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x143B, 0x143A )]
	public class Maul : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

        public override int AosStrengthReq { get { return 45; } }
        public override int AosMinDamage { get { return 12; } }
        public override int AosMaxDamage { get { return 14; } }
        public override int AosSpeed { get { return 40; } }
        public override float MlSpeed { get { return 2.75f; } }

        public override int OldStrengthReq { get { return 30; } }
        public override int OldMinDamage { get { return 15; } }
        public override int OldMaxDamage { get { return 25; } }
        public override int OldSpeed { get { return 42; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 75; } }

        public override int IconItemId { get { return 5178; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return -3; } }

		[Constructable]
		public Maul() : base( 0x143B )
		{
            Name = "maul";
			Weight = 10.0;
		}

		public Maul( Serial serial ) : base( serial )
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

			if ( Weight == 14.0 )
				Weight = 10.0;

            Name = "maul";
		}
	}
}