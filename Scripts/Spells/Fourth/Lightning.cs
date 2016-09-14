using System;
using Server.Targeting;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fourth
{
	public class LightningSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Lightning", "Por Ort Grav",
				239,
				9021,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

        public static int DamageMin { get { return 12; } }
        public static int DamageMax { get { return 18; } }

        public static double SpellSpecificCreatureDamageBonus { get { return 0.0; } }

		public LightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
        
		public void Target(Mobile mobile)
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.			

            else if (CheckHSequence(mobile))
            {
                SpellHelper.Turn(Caster, mobile);
                SpellHelper.CheckReflect((int)this.Circle, Caster, ref mobile);

                double damage = (double)Utility.RandomMinMax(8, 16);
                double damageBonus = 0;

                CheckMagicResist(mobile);               

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Energy, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, mobile, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                if (Caster is PlayerMobile && mobile is BaseCreature)
                    damageBonus += SpellSpecificCreatureDamageBonus;
                
                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.EnhancedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.EnhancedSpellBonus;
                }

                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.ChargedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.ChargedSpellBonus;

                    mobile.BoltEffect(0);     
                }

                else
                    mobile.BoltEffect(0);   

                Caster.PlaySound(0x29);

                damage *= GetDamageScalar(mobile, damageBonus);

                SpellHelper.Damage(this, Caster, mobile, damage);
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private LightningSpell m_Owner;

			public InternalTarget( LightningSpell owner ) : base(12, false, TargetFlags.Harmful )
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