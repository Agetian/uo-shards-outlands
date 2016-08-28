using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseBashing : BaseMeleeWeapon
	{
		public override int BaseHitSound{ get{ return 0x233; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		public override SkillName BaseSkill{ get{ return SkillName.Macing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Bash1H; } }

		public BaseBashing( int itemID ) : base( itemID )
		{
		}

		public BaseBashing( Serial serial ) : base( serial )
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