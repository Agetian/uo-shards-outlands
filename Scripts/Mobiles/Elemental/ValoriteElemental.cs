using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a valorite elemental corpse" )]
	public class ValoriteElemental : BaseCreature
	{
        [Constructable]
		public ValoriteElemental( ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{			
			Name = "a valorite elemental";
			Body = 112;
			BaseSoundID = 268;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(1250);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 150;

			Fame = 3500;
			Karma = -3500;
		}

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Slow; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override void SetUniqueAI()
        {
        }

        public DateTime m_NextReflectAllowed;

        public double NextReflectDelayMin = .5;
        public double NextReflectDelayMax = 1.5;        

        public override void OnThink()
        {
            base.OnThink();
           
            if (!Hidden && DateTime.UtcNow > m_NextReflectAllowed && MagicDamageAbsorb < 1)
            {
                FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                MagicDamageAbsorb = 1;

                PlaySound(0x1E9);

                double reflectDelay = NextReflectDelayMin + (Utility.RandomDouble() * (NextReflectDelayMax - NextReflectDelayMin));
                m_NextReflectAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(reflectDelay);
            }            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public ValoriteElemental( Serial serial ) : base( serial )
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
