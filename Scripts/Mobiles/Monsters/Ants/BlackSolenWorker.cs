using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a solen worker corpse" )]
	public class BlackSolenWorker : BaseCreature
	{
		[Constructable]
		public BlackSolenWorker() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a black solen worker";
			Body = 805;
			BaseSoundID = 959;
			Hue = 2051;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

			Fame = 1500;
			Karma = -1500;

			PackGold( Utility.Random( 100, 180 ) );

			SolenHelper.PackPicnicBasket( this );

			PackItem( new ZoogiFungus() );
		}

		public override int GetAngerSound()
		{
			return 0x269;
		}

		public override int GetIdleSound()
		{
			return 0x269;
		}

		public override int GetAttackSound()
		{
			return 0x186;
		}

		public override int GetHurtSound()
		{
			return 0x1BE;
		}

		public override int GetDeathSound()
		{
			return 0x8E;
		}

		public BlackSolenWorker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
