using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xE89, 0xE8a )]
	public class QuarterStaff : BaseStaff
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq{ get{ return 30; } }
		public override int AosMinDamage{ get{ return 11; } }
		public override int AosMaxDamage{ get{ return 14; } }
		public override int AosSpeed{ get{ return 48; } }
		public override float MlSpeed{ get{ return 2.25f; } }

		public override int OldStrengthReq { get { return 20; } }
        public override int OldMinDamage { get { return 20; } }
        public override int OldMaxDamage { get { return 32; } }
        public override int OldSpeed { get { return 35; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

        public override int IconItemId { get { return 3722; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -13; } }
        public override int IconOffsetY { get { return -13; } }

		[Constructable]
		public QuarterStaff() : base( 0xE89 )
		{
            Name = "quarter staff";
			Weight = 4.0;
		}

		public QuarterStaff( Serial serial ) : base( serial )
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

            Name = "quarter staff";
		}
	}
}