using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a bull corpse" )]
	public class Bull : BaseCreature
	{
		[Constructable]
		public Bull() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a bull";
			Body = Utility.RandomList( 0xE8, 0xE9 );
			BaseSoundID = 0x64;

			if ( 0.5 >= Utility.RandomDouble() )
				Hue = 0x901;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 600;
			Karma = 0;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 50;
        }
        
        public override int TamedItemId { get { return 8432; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 200; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 60; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

		public Bull(Serial serial) : base(serial)
		{
		}

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .15;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .02;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.BleedSpecialAbility(effectChance, this, defender, DamageMax, 8.0, 0x516, true, "", "The beast gores you with it's horns, causing you to bleed!");
        }

        public override void OnDoubleClick( Mobile from ) 
        { 
            if( ( InRange( from.Location, 2 ) == true ) && ( Warmode == false ) )
            {
				if( 0.1 > Utility.RandomDouble() )
				{ 
					this.Animate( 8, 5, 1, true, false, 0 ); 
					this.PlaySound( 0x7C ); 
				} 

                else 
                { 
                    int rnd = Utility.Random ( 3 ); 
                    this.PlaySound( 0x64 + rnd ); 
                } 
            }        
        } 

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}