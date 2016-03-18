using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a rotting corpse" )]
	public class Zombie : BaseCreature
	{
        public DateTime TatteredWrappingEnd = DateTime.MinValue;

		[Constructable]
		public Zombie() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a zombie";
			Body = 3;
			BaseSoundID = 471;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(75);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 30);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 600;
			Karma = -600;

            PackItem(new Bone(3));

            if (Utility.RandomMinMax(1, 5) == 1)
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

			switch ( Utility.Random( 5 ))
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new LeftArm() ); break;
				case 4: PackItem( new Bandage() ); break;
			}            
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

    		AwardDailyAchievementForKiller(NewbCategory.KillZombies);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new Bonemeal());
		}

		public Zombie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );

            writer.Write(TatteredWrappingEnd);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version > 0)
                TatteredWrappingEnd = reader.ReadDateTime();

            if (Controlled && TatteredWrappingEnd > DateTime.MinValue)
            {
                if (TatteredWrappingEnd < DateTime.UtcNow)
                {
                    Controlled = false;
                    ControlMaster = null;
                }

                else
                {
                    Timer.DelayCall(TatteredWrappingEnd - DateTime.UtcNow, delegate { TamableTatteredAncientMummyWrapping.WearOff(this); });
                }
            }
		}
	}
}
