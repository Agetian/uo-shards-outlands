using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class BaseStaff : BaseMeleeWeapon
	{
		public override int BaseHitSound{ get{ return 0x233; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		public override SkillName BaseSkill{ get{ return SkillName.Macing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Bash2H; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

		public BaseStaff( int itemID ) : base( itemID )
		{
		}

		public BaseStaff( Serial serial ) : base( serial )
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