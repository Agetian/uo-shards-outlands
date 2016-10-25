using System;
using Server.Targeting;
using Server.Network;

using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Second
{
	public class CureSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Cure", "An Nox",
				212,
				9061,
				Reagent.Garlic,
				Reagent.Ginseng
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public CureSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			return base.CheckCast();
		}

		public override void OnCast()
		{
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)
                {
                    this.Target(casterCreature.SpellTarget);
                }
            }

            else
            {
                Caster.Target = new InternalTarget(this);
            }
		}

		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

            else if (CheckBSequence(mobile))
            {
                SpellHelper.Turn(Caster, mobile);
                Poison p = mobile.Poison;

                if (p != null)
                {
                    double chanceToCure = (Caster.Skills[SkillName.Magery].Value * 0.75) + (110.0 - (p.Level + 1) * 33.0);
                    
                    chanceToCure /= 100;

                    if (chanceToCure > Utility.RandomDouble())
                    {
                        if (mobile.CurePoison(Caster))
                        {
                            if (Caster != mobile)                            
                                Caster.SendLocalizedMessage(1010058); // You have cured the target of all poisons!
                            
                            mobile.SendLocalizedMessage(1010059); // You have been cured of all poisons.
                        }
                    }

                    else
                    {
                        bool cured = false;

                        AspectArmorProfile aspectArmorProfile = AspectGear.GetAspectArmorProfile(mobile);                        

                        //Poison Aspect
                        if (aspectArmorProfile != null)
                        {
                            if (aspectArmorProfile.m_Aspect == AspectEnum.Poison)
                            {
                                double extraCureChance = AspectGear.PoisonCureChanceBonus * (AspectGear.PoisonCureChanceBonusPerTier * (double)aspectArmorProfile.m_TierLevel);

                                if (Utility.RandomDouble() <= extraCureChance)
                                {
                                    mobile.CurePoison(Caster);

                                    cured = true;

                                    //TEST: Add Aspect Visuals
                                }
                            }
                        } 

                        if (!cured)
                            mobile.SendLocalizedMessage(1010060); // You have failed to cure your target!
                    }
                }

                int spellHue = Enhancements.GetMobileSpellHue(Caster, Enhancements.SpellType.Cure);      

                mobile.FixedParticles(0x373A, 10, 15, 5012, spellHue, 0, EffectLayer.Waist);
                mobile.PlaySound(0x1E0);
            }

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private CureSpell m_Owner;

			public InternalTarget( CureSpell owner ) : base( 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}