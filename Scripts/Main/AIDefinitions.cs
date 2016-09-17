using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server
{
    public class AIDefinitions
    {
        public static void UpdateAI(BaseCreature bc_Creature)
        {
            AIGroupType m_AIGroup = bc_Creature.AIGroup;
            AISubGroupType m_AISubGroup = bc_Creature.AISubGroup;

            //AI Group
            switch (m_AIGroup)
            {
                case AIGroupType.None:
                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 0;

                    bc_Creature.DictCombatAction[CombatAction.None] = 0;
                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatEpicAction] = 0;
                break;

                case AIGroupType.EvilMonster:
                    bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 2;
                break;

                case AIGroupType.NeutralMonster:

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 2;
                break;

                case AIGroupType.GoodMonster:
                    bc_Creature.DictCombatTargeting[CombatTargeting.Evil] = 1;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 2;
                break;

                case AIGroupType.EvilAnimal:
                    bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 2;
                break;

                case AIGroupType.NeutralAnimal:
                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 2;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 3;
                break;

                case AIGroupType.GoodAnimal:
                    bc_Creature.DictCombatTargeting[CombatTargeting.Evil] = 1;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 2;
                break;

                case AIGroupType.Summoned:
                break;

                case AIGroupType.Boss:
                    bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;
                break;
            }

            //AI SubGroup
            switch (m_AISubGroup)
            {
                #region Melee

                case AISubGroupType.Melee:
                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 0;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatEpicAction] = 0;
                break;

                #endregion

                #region MeleeMage

                case AISubGroupType.MeleeMage1:
                    bc_Creature.SpellDelayMin = 7;
                    bc_Creature.SpellDelayMax = 8;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                break;

                case AISubGroupType.MeleeMage2:
                    bc_Creature.SpellDelayMin = 5;
                    bc_Creature.SpellDelayMax = 7;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 2;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                break;

                case AISubGroupType.MeleeMage3:
                    bc_Creature.SpellDelayMin = 4;
                    bc_Creature.SpellDelayMax = 5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 3;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.MeleeMage4:
                    bc_Creature.SpellDelayMin = 3;
                    bc_Creature.SpellDelayMax = 4;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 4;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.MeleeMage5:
                    bc_Creature.SpellDelayMin = 1;
                    bc_Creature.SpellDelayMax = 2;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 5;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.MeleeMage6:
                    bc_Creature.SpellDelayMin = .5;
                    bc_Creature.SpellDelayMax = 1;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 6;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                #endregion

                #region Mage

                case AISubGroupType.Mage1:
                    bc_Creature.SpellDelayMin = 4.5;
                    bc_Creature.SpellDelayMax = 5.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.Mage2:
                    bc_Creature.SpellDelayMin = 3.5;
                    bc_Creature.SpellDelayMax = 4.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 2;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] =0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.Mage3:
                    bc_Creature.SpellDelayMin = 2.5;
                    bc_Creature.SpellDelayMax = 3.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 3;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.Mage4:
                    bc_Creature.SpellDelayMin = 1.5;
                    bc_Creature.SpellDelayMax = 2.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 4;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.Mage5:
                    bc_Creature.SpellDelayMin = 0.5;
                    bc_Creature.SpellDelayMax = 1.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 5;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                case AISubGroupType.Mage6:
                    bc_Creature.SpellDelayMin = 0;
                    bc_Creature.SpellDelayMax = 1;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 6;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

                    bc_Creature.DictWanderAction[WanderAction.None] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 0;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 0;
                    break;

                #endregion

                #region Group Healer Melee

                case AISubGroupType.GroupHealerMelee:
                    bc_Creature.SpellDelayMin = 5;
                    bc_Creature.SpellDelayMax = 7;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther100] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 4;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 4;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 5;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                #endregion

                #region Group Healer MeleeMage

                case AISubGroupType.GroupHealerMeleeMage1:
                    bc_Creature.SpellDelayMin = 7;
                    bc_Creature.SpellDelayMax = 8;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubGroupType.GroupHealerMeleeMage2:
                    bc_Creature.SpellDelayMin = 5;
                    bc_Creature.SpellDelayMax = 7;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 2;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 0;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMeleeMage3:
                    bc_Creature.SpellDelayMin = 4;
                    bc_Creature.SpellDelayMax = 5;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 3;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMeleeMage4:
                    bc_Creature.SpellDelayMin = 2;
                    bc_Creature.SpellDelayMax = 4;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 4;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 6;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 6;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMeleeMage5:
                    bc_Creature.SpellDelayMin = 1;
                    bc_Creature.SpellDelayMax = 2;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 5;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 4;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 9;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 4;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 9;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMeleeMage6:
                    bc_Creature.SpellDelayMin = .5;
                    bc_Creature.SpellDelayMax = 1;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 6;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 5;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 12;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 5;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 12;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                #endregion

                #region Group Healer Mage

                case AISubGroupType.GroupHealerMage1:
                    bc_Creature.SpellDelayMin = 4.5;
                    bc_Creature.SpellDelayMax = 5.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 1;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 0;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 0;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMage2:
                    bc_Creature.SpellDelayMin = 3.5;
                    bc_Creature.SpellDelayMax = 4.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 2;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 0;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMage3:
                    bc_Creature.SpellDelayMin = 2.5;
                    bc_Creature.SpellDelayMax = 3.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 3;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMage4:
                    bc_Creature.SpellDelayMin = 1.5;
                    bc_Creature.SpellDelayMax = 2.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 4;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 6;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 6;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMage5:
                    bc_Creature.SpellDelayMin = 0.5;
                    bc_Creature.SpellDelayMax = 1.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 5;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 4;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 9;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 4;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 9;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubGroupType.GroupHealerMage6:
                    bc_Creature.SpellDelayMin = 0;
                    bc_Creature.SpellDelayMax = 1;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 0;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 6;

                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 5;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 12;
                    bc_Creature.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 5;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 12;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                #endregion

                #region Group Medic

                case AISubGroupType.GroupMedicMelee:
                    bc_Creature.SpellDelayMin = 2.5;
                    bc_Creature.SpellDelayMax = 3.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;
                    bc_Creature.DictCombatRange[CombatRange.SpellRange] = 0;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 0;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther100] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther75] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther50] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther25] = 4;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageCureOther] = 5;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] = 4;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageCureSelf] = 5;

                    bc_Creature.DictWanderAction[WanderAction.BandageHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.BandageHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.BandageCureOther] = 1;
                    bc_Creature.DictWanderAction[WanderAction.BandageCureSelf] = 1;
                break;

                case AISubGroupType.GroupMedicRanged:
                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealOther] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther100] = 1;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther75] = 2;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther50] = 3;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageHealOther25] = 4;
                    bc_Creature.DictCombatHealOther[CombatHealOther.BandageCureOther] = 5;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] = 3;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] = 4;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageCureSelf] = 5;

                    bc_Creature.DictWanderAction[WanderAction.BandageHealOther100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.BandageHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.BandageCureOther] = 1;
                    bc_Creature.DictWanderAction[WanderAction.BandageCureSelf] = 1;
                    break;

                #endregion

                case AISubGroupType.WanderingHealer:
                    bc_Creature.SpellDelayMin = 1.5;
                    bc_Creature.SpellDelayMax = 2.5;

                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpell] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    bc_Creature.DictCombatSpell[CombatSpell.SpellPoison] = 4;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 2;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    bc_Creature.DictWanderAction[WanderAction.SpellCureSelf] = 1;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 0;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 0;
                break;

                case AISubGroupType.Hunter:
                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictCombatTargeting[CombatTargeting.SuperPredator] = 3;
                    bc_Creature.DictCombatTargeting[CombatTargeting.Predator] = 2;
                    bc_Creature.DictCombatTargeting[CombatTargeting.Prey] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.Stealth] = 1;

                    break;

                case AISubGroupType.SuperPredator:
                    bc_Creature.SuperPredator = true;

                    bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor] = 3;
                    bc_Creature.DictCombatTargeting[CombatTargeting.Predator] = 1;
                    bc_Creature.DictCombatTargeting[CombatTargeting.Prey] = 2;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 1;
                break;

                case AISubGroupType.Predator:
                    bc_Creature.Predator = true;

                    bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor] = 2;
                    bc_Creature.DictCombatTargeting[CombatTargeting.Prey] = 1;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 2;
                break;

                case AISubGroupType.Prey:
                    bc_Creature.Prey = true;

                    bc_Creature.DictCombatFlee[CombatFlee.Flee25] = 1;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 3;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 5;
                break;

                case AISubGroupType.Berserk:
                    bc_Creature.DictCombatTargeting[CombatTargeting.Any] = 1;

                    bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 10;
                break;

                case AISubGroupType.MeleePotion:
                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] = 5;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;

                    bc_Creature.CombatHealActionMinDelay = 15;
                    bc_Creature.CombatHealActionMaxDelay = 30;
                break;

                case AISubGroupType.Swarm:
                    bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 10;
                break;

                case AISubGroupType.Duelist:
                    bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;
                break;

                case AISubGroupType.Ranged:
                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;
                break;

                case AISubGroupType.Scout:
                    bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    bc_Creature.DictCombatRange[CombatRange.Withdraw] = 1;

                    bc_Creature.DictGuardAction[GuardAction.None] = 1;
                    bc_Creature.DictGuardAction[GuardAction.DetectHidden] = 3;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.DetectHidden] = 1;

                    bc_Creature.DictWaypointAction[WaypointAction.None] = 0;
                    bc_Creature.DictWaypointAction[WaypointAction.DetectHidden] = 1;

                    bc_Creature.ResolveAcquireTargetDelay = 1;
                break;
                    
                case AISubGroupType.Assassin:
                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction] = 1;

                    bc_Creature.DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 1;

                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.Stealth] = 1;
                break;

                case AISubGroupType.Poisoner:
                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 1;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction] = 1;

                    bc_Creature.DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison] = 1;
                break;

                case AISubGroupType.Stealther:
                    bc_Creature.DictWanderAction[WanderAction.None] = 1;
                    bc_Creature.DictWanderAction[WanderAction.Stealth] = 3;
                break;

                case AISubGroupType.Sailor:
                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 0;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 0;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 20;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction] = 3;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 5;
                break;

                case AISubGroupType.ShipCaptain:
                    bc_Creature.DictCombatFlee[CombatFlee.Flee10] = 0;
                    bc_Creature.DictCombatFlee[CombatFlee.Flee5] = 0;

                    bc_Creature.DictCombatAction[CombatAction.AttackOnly] = 15;
                    bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction] = 3;
                    bc_Creature.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    bc_Creature.DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 5;

                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] = 1;
                    bc_Creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 5;
                break;
            }
        }
    }
}