using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fifth
{
	public class ParalyzeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Paralyze", "An Ex Por",
				218,
				9012,
				Reagent.Garlic,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public ParalyzeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}
        
		public override void OnCast()
		{
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)                
                    this.Target(casterCreature.SpellTarget);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
		}

		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.			

			else if ( CheckHSequence( mobile ) )
			{
				SpellHelper.Turn( Caster, mobile );
				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref mobile );

				double duration;

				if (mobile.Player)
				{						
					duration = 5 + (Caster.Skills[SkillName.Magery].Value * 0.05);

					if (CheckMagicResist(mobile))
						duration *= 0.5;
				}

				else
				{						
					duration = 10.0 + (Caster.Skills[SkillName.Magery].Value * 0.2);

					if (CheckMagicResist(mobile))
						duration *= 0.75;
				}				
                                
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Warlock, true, true);

                int spellHue = 0;

                AspectArmorProfile defenderAspectArmorProfile = AspectGear.GetAspectArmorProfile(mobile);

                double airAvoidanceChance = 0;

                if (defenderAspectArmorProfile != null)
                {
                    if (defenderAspectArmorProfile.m_Aspect == AspectEnum.Air)
                        airAvoidanceChance = AspectGear.AirMeleeAvoidMovementEffectAvoidance * (AspectGear.AirMeleeAvoidMovementEffectAvoidancePerTier * (double)defenderAspectArmorProfile.m_TierLevel);
                }

                if (Utility.RandomDouble() <= airAvoidanceChance)
                {
                    //TEST: Add Aspect Visuals
                }

                else if (enhancedSpellcast)
                {
                    if (mobile.Paralyze(Caster, duration * 5))
                    {
                        mobile.FixedEffect(0x376A, 10, 30, spellHue, 0);
                        mobile.PlaySound(0x204);         
                    }

                    else if (mobile is PlayerMobile)
                    {
                        mobile.FixedEffect(0x376A, 10, 30, spellHue, 0);
                        mobile.PlaySound(0x204);   
                    }
                }

                else
                {
                    if (mobile.Paralyze(Caster, duration))
                    {
                        mobile.FixedEffect(0x376A, 10, 15, spellHue, 0);
                        mobile.PlaySound(0x204);      
                    }

                    else if (mobile is PlayerMobile)
                    {
                        mobile.FixedEffect(0x376A, 10, 15, spellHue, 0);
                        mobile.PlaySound(0x204);  
                    }
                }				

				HarmfulSpell( mobile );
			}

            ArenaFight.SpellCompletion(Caster, typeof(ParalyzeSpell));

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private ParalyzeSpell m_Owner;

			public InternalTarget( ParalyzeSpell owner ) : base(12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
