using System;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF52, 0xF51 )]
	public class UOACZDagger : BaseKnife
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

        public override int AosStrengthReq { get { return 10; } }
        public override int AosMinDamage { get { return 10; } }
        public override int AosMaxDamage { get { return 12; } }
        public override int AosSpeed { get { return 53; } }
        public override float MlSpeed { get { return 2.00f; } }

        public override int OldStrengthReq { get { return 10; } }
        public override int OldMinDamage { get { return 6; } }
        public override int OldMaxDamage { get { return 12; } }
        public override int OldSpeed { get { return 58; } }

        public override int DefHitSound { get { return 0x23C; } }
        public override int DefMissSound { get { return 0x238; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 90; } }

        public override SkillName DefSkill { get { return SkillName.Fencing; } }
        public override WeaponType DefType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public UOACZDagger() : base( 0xF52 )
		{
			Weight = 1.0;
		}

		public UOACZDagger( Serial serial ) : base( serial )
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