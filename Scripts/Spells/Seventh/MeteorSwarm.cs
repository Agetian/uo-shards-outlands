using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Spells.Seventh
{
    public class MeteorSwarmSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Meteor Swarm", "Flam Kal Des Ylem",
                233,
                9042,
                false,
                Reagent.Bloodmoss,
                Reagent.MandrakeRoot,
                Reagent.SulfurousAsh,
                Reagent.SpidersSilk
            );

        public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

        public static int DamageMin { get { return 10; } }
        public static int DamageMax { get { return 20; } }

        public static double SpellSpecificCreatureDamageBonus { get { return 0.0; } }

        public MeteorSwarmSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)
                {
                    IPoint3D targetLocation = casterCreature.SpellTarget.Location as IPoint3D;

                    if (targetLocation != null)                    
                        this.Target(targetLocation);                    
                }
            }

            else            
                Caster.Target = new InternalTarget(this);            
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                bool enhancedSpellcast = false;
                Boolean chargedSpellcast = false;

                int spellHue = Enhancements.GetMobileSpellHue(Caster, Enhancements.SpellType.MeteorSwarm);      

                int radius = 3;

                if (Caster is BaseCreature)
                    radius += (int)(Math.Floor((Caster.Skills[SkillName.Magery].Value - 75) / 25));

                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                Map map = Caster.Map;

                IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(new Point3D(p), radius);

                Queue m_Queue = new Queue();

                int targetCount = 0;

                bool playerVsCreatureOccurred = false;

                if (targetCount > 0)
                    Effects.PlaySound(p, Caster.Map, 0x160);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (mobile == Caster)
                        continue;

                    if (Caster is BaseCreature)
                    {
                        if (!SpecialAbilities.MonsterCanDamage(Caster, mobile))
                            continue;
                    }

                    if (!Caster.CanBeHarmful(mobile, false))
                        continue;

                    if (Caster is PlayerMobile && mobile is BaseCreature)
                        playerVsCreatureOccurred = true;

                    targetCount++;
                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();

                if (playerVsCreatureOccurred)
                {
                    enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Fire, false, true);
                    chargedSpellcast = SpellHelper.IsChargedSpell(Caster, null, false, Scroll != null);
                }

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    double damage = (double)Utility.RandomMinMax(DamageMin, DamageMax);
                    double damageBonus = 0;
                    
                    CheckMagicResist(mobile);

                    Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                    if (Caster is PlayerMobile && mobile is BaseCreature)
                        damageBonus += SpellSpecificCreatureDamageBonus;

                    if (enhancedSpellcast && mobile is BaseCreature)
                    {
                        if (isTamedTarget)
                            damageBonus += SpellHelper.EnhancedSpellTamedCreatureBonus;

                        else
                            damageBonus += SpellHelper.EnhancedSpellBonus;
                    }

                    if (chargedSpellcast && mobile is BaseCreature)
                    {
                        if (isTamedTarget)
                            damageBonus += SpellHelper.ChargedSpellTamedCreatureBonus;

                        else
                            damageBonus += SpellHelper.ChargedSpellBonus;

                        Caster.MovingParticles(mobile, 0x36D4, 5, 0, false, true, spellHue, 0, 9501, 1, 0, 0x100);
                        Caster.MovingParticles(mobile, 0x36D4, 9, 0, false, true, spellHue, 0, 9501, 1, 0, 0x100);
                    }

                    else
                        Caster.MovingParticles(mobile, 0x36D4, 7, 0, false, true, spellHue, 0, 9501, 1, 0, 0x100);

                    BaseCreature bc_Target = mobile as BaseCreature;
                    PlayerMobile pm_Target = mobile as PlayerMobile;

                    if (bc_Target != null)
                    {
                        if (bc_Target.ShipOccupied != null)
                            damage *= BaseShip.shipBasedAoESpellDamageToCreatureScalar;
                    }

                    if (pm_Target != null)
                    {
                        if (pm_Target.ShipOccupied != null)
                            damage *= BaseShip.shipBasedAoESpellDamageToPlayerScalar;
                    }

                    damage *= GetDamageScalar(mobile, damageBonus);

                    SpellHelper.Damage(this, Caster, mobile, damage);
                }
            }

            ArenaFight.SpellCompletion(Caster, typeof(MeteorSwarmSpell));

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private MeteorSwarmSpell m_Owner;

            public InternalTarget(MeteorSwarmSpell owner): base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}