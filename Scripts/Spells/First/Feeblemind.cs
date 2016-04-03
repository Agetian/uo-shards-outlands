using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;


namespace Server.Spells.First
{
	public class FeeblemindSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Feeblemind", "Rel Wis",
				212,
				9031,
				Reagent.Ginseng,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public FeeblemindSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
				SpellHelper.AddStatCurse( Caster, mobile, StatType.Int );

                CheckMagicResist(mobile);

                if (mobile.Spell != null)
                    mobile.Spell.OnCasterHurt();

                mobile.Paralyzed = false;

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Feeblemind); 

                mobile.FixedParticles(0x3779, 10, 15, 5002, spellHue, 0, EffectLayer.Head);
				mobile.PlaySound( 0x1E4 );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FeeblemindSpell m_Owner;

			public InternalTarget( FeeblemindSpell owner ) : base( 12, false, TargetFlags.Harmful )
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