using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xDF1, 0xDF0 )]
	public class BlackStaff : BaseStaff
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

        public override int AosStrengthReq { get { return 20; } }
        public override int AosMinDamage { get { return 15; } }
        public override int AosMaxDamage { get { return 17; } }
        public override int AosSpeed { get { return 33; } }
        public override float MlSpeed { get { return 3.25f; } }

        public override int OldStrengthReq { get { return 20; } }
        public override int OldMinDamage { get { return 20; } }
        public override int OldMaxDamage { get { return 32; } }
        public override int OldSpeed { get { return 35; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 75; } }

        public override int IconItemId { get { return 3568; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -14; } }
        public override int IconOffsetY { get { return -13; } }

		[Constructable]
		public BlackStaff() : base( 0xDF0 )
		{
            Name = "black staff";
			Weight = 6.0;
		}

		public BlackStaff( Serial serial ) : base( serial )
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

            Name = "black staff";
		}
	}
}