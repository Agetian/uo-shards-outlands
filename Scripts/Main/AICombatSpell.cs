using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if Framework_4_0
using System.Linq;
using System.Threading.Tasks;
#endif
using Server;
using Server.Items;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using Server.Regions;
using Server.ContextMenus;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;
using MoveImpl = Server.Movement.MovementImpl;
using Server.Custom;

namespace Server.Mobiles
{
    public class AICombatSpell
    {
        public static bool CanDoCombatSpell(BaseCreature creature)
        {
            if (creature.DictCombatAction[CombatAction.CombatSpell] > 0)
            {
                if (DateTime.UtcNow > creature.NextSpellTime)
                {
                    Dictionary<CombatSpell, int> DictTemp = new Dictionary<CombatSpell, int>();

                    if (AICombatSpell.CanDoCombatSpellSpellDamage1(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage1, creature.DictCombatSpell[CombatSpell.SpellDamage1]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage2(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage2, creature.DictCombatSpell[CombatSpell.SpellDamage2]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage3(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage3, creature.DictCombatSpell[CombatSpell.SpellDamage3]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage4(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage4, creature.DictCombatSpell[CombatSpell.SpellDamage4]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage5(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage5, creature.DictCombatSpell[CombatSpell.SpellDamage5]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage6(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage6, creature.DictCombatSpell[CombatSpell.SpellDamage6]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage7, creature.DictCombatSpell[CombatSpell.SpellDamage7]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamageAOE7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamageAOE7, creature.DictCombatSpell[CombatSpell.SpellDamageAOE7]); }

                    if (AICombatSpell.CanDoCombatSpellSpellPoison(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellPoison, creature.DictCombatSpell[CombatSpell.SpellPoison]); }

                    int TotalValues = 0;

                    //Calculate Total Values
                    foreach (KeyValuePair<CombatSpell, int> pair in DictTemp)
                    {
                        TotalValues += pair.Value;
                    }

                    if (TotalValues > 0)
                        return true;
                }
            }

            return false;
        }

        public static bool DoCombatSpell(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;           

            Dictionary<CombatSpell, int> DictTemp = new Dictionary<CombatSpell, int>();

            if (AICombatSpell.CanDoCombatSpellSpellDamage1(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage1, creature.DictCombatSpell[CombatSpell.SpellDamage1]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage2(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage2, creature.DictCombatSpell[CombatSpell.SpellDamage2]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage3(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage3, creature.DictCombatSpell[CombatSpell.SpellDamage3]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage4(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage4, creature.DictCombatSpell[CombatSpell.SpellDamage4]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage5(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage5, creature.DictCombatSpell[CombatSpell.SpellDamage5]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage6(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage6, creature.DictCombatSpell[CombatSpell.SpellDamage6]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage7, creature.DictCombatSpell[CombatSpell.SpellDamage7]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamageAOE7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamageAOE7, creature.DictCombatSpell[CombatSpell.SpellDamageAOE7]); }

            if (AICombatSpell.CanDoCombatSpellSpellPoison(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellPoison, creature.DictCombatSpell[CombatSpell.SpellPoison]); }
           
            int TotalValues = 0;

            //Calculate Total Values
            foreach (KeyValuePair<CombatSpell, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            Spell selectedSpell = null;

            //Determine CombatSpell                      
            foreach (KeyValuePair<CombatSpell, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    CombatSpell combatSpell = pair.Key;
                    Spell spell = null;

                    switch (combatSpell)
                    {
                        case CombatSpell.SpellDamage1: spell = AICombatSpell.GetSpellDamage1(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage2: spell = AICombatSpell.GetSpellDamage2(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage3: spell = AICombatSpell.GetSpellDamage3(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage4: spell = AICombatSpell.GetSpellDamage4(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage5: spell = AICombatSpell.GetSpellDamage5(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage6: spell = AICombatSpell.GetSpellDamage6(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage7: spell = AICombatSpell.GetSpellDamage7(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamageAOE7: spell = AICombatSpell.GetSpellDamageAOE7(creature, creature.Combatant); break;

                        case CombatSpell.SpellPoison: spell = AICombatSpell.GetSpellPoison(creature, creature.Combatant); break;
                    }

                    if (spell != null)
                    {
                        if (creature.AcquireNewTargetEveryCombatAction)
                            creature.m_NextAcquireTargetAllowed = DateTime.UtcNow;

                        spell.Cast();                                               

                        return true;
                    }

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        //----------------
        
        public static bool CanDoCombatSpellSpellDamage1(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage1] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 4)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage2(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage2] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 6)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage3(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage3] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 9)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage4(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage4] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 11)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage5(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage5] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 14)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage6(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage6] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 20)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage7(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage7] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 40)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamageAOE7(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 40)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellPoison(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;
            
            if (creature.DictCombatSpell[CombatSpell.SpellPoison] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 9)
                {
                    BaseCreature bc_Target = target as BaseCreature;

                    if (bc_Target != null)
                    {
                        if (bc_Target.PoisonResistance >= 5)
                            return false;
                    }

                    if (target.Poison == null)
                        return true;
                }
            }

            return false;
        }
               
        public static Spell GetSpellDamage1(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new MagicArrowSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage2(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new HarmSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage3(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new FireballSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage4(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new LightningSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage5(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new MindBlastSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage6(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            if (!creature.CastOnlyFireSpells)
                spells.Add(new EnergyBoltSpell(creature, null));

            if (!creature.CastOnlyEnergySpells)
                spells.Add(new ExplosionSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage7(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new FlameStrikeSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamageAOE7(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            if (!creature.CastOnlyFireSpells)
                spells.Add(new ChainLightningSpell(creature, null));

            if (!creature.CastOnlyEnergySpells)
                spells.Add(new MeteorSwarmSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellPoison(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new PoisonSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }
        
        public static bool SpellInDefaultRange(BaseCreature creature, Mobile target)
        {
            //Return False If Null Target
            if (target == null)
                return false;

            if (creature.GetDistanceToSqrt(target) <= creature.CreatureSpellCastRange)
                return true;

            return false;
        }

        public static bool CreatureHasCastingAI(BaseCreature creature)
        {
            if (creature == null)
                return false;

            bool hasCastingAI = false;

            foreach (KeyValuePair<CombatSpell, int> pair in creature.DictCombatSpell)
            {
                if (pair.Value > 0)
                {
                    hasCastingAI = true;
                    break;
                }
            }

            if (creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] > 0)
                hasCastingAI = true;

            if (creature.DictCombatHealOther[CombatHealOther.SpellHealOther100] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellCureOther] > 0)
                hasCastingAI = true;

            return hasCastingAI;
        }
    }
}
