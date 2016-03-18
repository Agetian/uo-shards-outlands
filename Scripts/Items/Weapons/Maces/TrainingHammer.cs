using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    [FlipableAttribute(0x13E3, 0x13E4)]
	public class TrainingHammer : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq{ get{ return 40; } }
		public override int AosMinDamage{ get{ return 11; } }
		public override int AosMaxDamage{ get{ return 13; } }
		public override int AosSpeed{ get{ return 44; } }
		public override float MlSpeed{ get{ return 2.50f; } }

		public override int OldStrengthReq{ get{ return 10; } }
        public override int OldMinDamage { get { return 1; } }
        public override int OldMaxDamage { get { return 2; } }
        public override int OldSpeed { get { return 50; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 75; } }

        public override int IconItemId { get { return 5091; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -7; } }
        public override int IconOffsetY { get { return 0; } }

		[Constructable]
        public TrainingHammer(): base(0x13E3)
		{
            Name = "a training hammer";
			Weight = 3.0;
			Layer = Layer.OneHanded;
		}

        public TrainingHammer(Serial serial): base(serial)
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