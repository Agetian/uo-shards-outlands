using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF43, 0xF44 )]
	public class Hatchet : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 41; } }
		public override float MlSpeed{ get{ return 2.75f; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldMinDamage { get { return 1; } }
        public override int OldMaxDamage { get { return 2; } }
        public override int OldSpeed { get { return 50; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

        public override int IconItemId { get { return 3908; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -8; } }
        public override int IconOffsetY { get { return -3; } }

		[Constructable]
		public Hatchet() : base( 0xF43 )
		{
            Name = "hatchet";

			Weight = 4.0;
		}

		public Hatchet( Serial serial ) : base( serial )
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

            Name = "hatchet";
		}
	}
}