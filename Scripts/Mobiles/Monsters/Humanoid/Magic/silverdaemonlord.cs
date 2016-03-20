using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a silver daemon lord's corpse" )]
	public class SilverDaemonLord : BaseCreature
	{
		[Constructable]
		public SilverDaemonLord () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a silver daemon lord";
			Body = 40;
			BaseSoundID = 357;
			Hue = 0x835;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(5000);
            SetMana(5000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;

			Fame = 500000;
			Karma = -500000;
		}

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

		public SilverDaemonLord( Serial serial ) : base( serial )
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