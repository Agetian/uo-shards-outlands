using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Eighth
{
    public class EnergyVortexSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Energy Vortex", "Vas Corp Por",
                260,
                9032,
                false,
                Reagent.Bloodmoss,
                Reagent.BlackPearl,
                Reagent.MandrakeRoot,
                Reagent.Nightshade
            );

        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }


        public override TimeSpan GetCastDelay()
        {
            return TimeSpan.FromSeconds(4.0);
        }


        public EnergyVortexSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;
            
            if ((Caster.Followers + 1) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            Map map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z) || BaseShip.FindShipAt(p, map) != null)            
                Caster.SendLocalizedMessage(501942); // That location is blocked.            

            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Summoner, false, true);

                BaseCreature summon = new EnergyVortex();

                summon.StoreBaseSummonValues();

                double duration = 2.0 * Caster.Skills[SkillName.Magery].Value;

                if (enhancedSpellcast)
                {
                    duration *= SpellHelper.EnhancedSummonDurationMultiplier;

                    summon.DamageMin = (int)((double)summon.DamageMin * SpellHelper.EnhancedSummonDamageMultiplier);
                    summon.DamageMax = (int)((double)summon.DamageMax * SpellHelper.EnhancedSummonDamageMultiplier);

                    summon.SetHitsMax((int)((double)summon.HitsMax * SpellHelper.EnhancedSummonHitPointsMultiplier));
                    summon.Hits = summon.HitsMax;
                }

                summon.SetDispelResistance(Caster, enhancedSpellcast, 0);

                int spellHue = Enhancements.GetMobileSpellHue(Caster, Enhancements.SpellType.EnergyVortex);      

                summon.Hue = spellHue;

                BaseCreature.Summon(summon, false, Caster, new Point3D(p), 0x212, TimeSpan.FromSeconds(duration));                
            }

            FinishSequence();
        }

        // IPY Napoleon Dynamite easter egg
        public static void BeginAction(object state)
        {
            List<Mobile> mobs = (List<Mobile>)state;
            Mobile tina = mobs[0];
            Mobile ham = mobs[1];

            SpellHelper.Turn(tina, ham);
            SpellHelper.Turn(ham, tina);

            tina.Say("???");
        }

        // IPY Napoleon Dynamite easter egg
        public static void EndAction(object state)
        {
            List<Mobile> mobs = (List<Mobile>)state;

            foreach (Mobile mob in mobs)
            {
                mob.Say("Kal Ort Por");
                mob.PlaySound(0x1FC); // Recall sound
            }
        }

        private class InternalTarget : Target
        {
            private EnergyVortexSpell m_Owner;

            public InternalTarget(EnergyVortexSpell owner): base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetOutOfLOS(Mobile from, object o)
            {
                from.SendLocalizedMessage(501943); // Target cannot be seen. Try again.
                from.Target = new InternalTarget(m_Owner);
                from.Target.BeginTimeout(from, TimeoutTime - DateTime.UtcNow);
                m_Owner = null;
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_Owner != null)
                    m_Owner.FinishSequence();
            }
        }
    }
}