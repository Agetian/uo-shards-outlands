using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class Ghoul : BaseCreature
	{
		[Constructable]
		public Ghoul() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a ghoul";
			Body = 153;
			BaseSoundID = 0x482;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

			Fame = 2500;
			Karma = -2500;

            if (Utility.RandomMinMax(1, 4) == 1)
            {
                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: PackItem(new BoneHelm()); break;
                    case 2: PackItem(new BoneChest()); break;
                    case 3: PackItem(new BoneArms()); break;
                    case 4: PackItem(new BoneLegs()); break;
                    case 5: PackItem(new BoneGloves()); break;
                }
            }

			PackItem( Loot.RandomWeapon() );
            PackItem(new Bone(3));
		}

		public override bool OnBeforeDeath()
		{
            PackItem( new Bone() );			

			return base.OnBeforeDeath();
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new Bonemeal());
        }
		
		public Ghoul( Serial serial ) : base( serial )
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
