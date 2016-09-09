using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Server
{
    public class AnimalLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.AnimalLore].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTarget();
            m.SendMessage("What creature would you like to examine?");

            return TimeSpan.FromSeconds(2.5);
        }

        private class InternalTarget : Target
        {
            public InternalTarget(): base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (!from.Alive)
                    from.SendLocalizedMessage(500331); // The spirits of the dead are not the province of animal lore.

                else if (targeted is BaseCreature)
                {
                    BaseCreature bc_Creature = (BaseCreature)targeted;                    

                    if (bc_Creature.IsHenchman)
                    {
                        from.SendMessage("You feel you would make a better evaluation of that individual with the Anatomy skill.");
                        return;
                    }

                    bool gumpSuccess = false;

                    if (bc_Creature.Controlled && bc_Creature.ControlMaster == from)
                        gumpSuccess = true;

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.AnimalLoreCooldown * 1000);

                    if (from.CheckSkill(SkillName.AnimalLore, 0, 120, 1))
                        gumpSuccess = true;

                    if (!gumpSuccess)
                    {
                        from.SendMessage("You can't think of anything in particular about that creature.");
                        return;
                    }

                    from.SendGump(new AnimalLoreGump(player, new AnimalLoreGumpObject(bc_Creature)));
                    from.SendSound(0x055);
                }

                else
                    from.SendMessage("That is not a tameable creature.");
            }
        }
    }

    public class AnimalLoreGump : Gump
    {
        public enum AnimalLoreGumpPage
        {
            Stats,
            Traits,
            Info
        }

        public enum TraitSelectionType
        {
            None,
            Left,
            Right
        }

        public PlayerMobile m_Player;
        public AnimalLoreGumpObject m_AnimalLoreGumpObject;
        
        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int selectionSound = 0x4D2;
        public int traitAssignedSound = 0x5BC;

        public AnimalLoreGump(PlayerMobile player, AnimalLoreGumpObject animalLoreGumpObject): base(50, 50)
        {
            m_Player = player;
            m_AnimalLoreGumpObject = animalLoreGumpObject;

            if (m_Player == null) return;
            if (m_AnimalLoreGumpObject == null) return;
            if (m_AnimalLoreGumpObject.bc_Creature == null) return;
            if (m_AnimalLoreGumpObject.bc_Creature.Deleted) return;
            if (!m_AnimalLoreGumpObject.bc_Creature.Alive && !m_AnimalLoreGumpObject.bc_Creature.IsBonded) return;

            BaseCreature bc_Creature = m_AnimalLoreGumpObject.bc_Creature;

            int traitsEarned = FollowerTraits.GetFollowerTraitsEarned(bc_Creature);
            int traitsAvailable = FollowerTraits.GetFollowerTraitsAvailable(bc_Creature);
            int totalTraits = (int)((double)bc_Creature.TraitsList.Count / 2);
            
            //Populate Trait Selection
            if (m_AnimalLoreGumpObject.m_TraitGumpSelections.Count == 0)
            {
                for (int a = 0; a < totalTraits; a++)
                {
                    m_AnimalLoreGumpObject.m_TraitGumpSelections.Add(TraitSelectionType.None);
                }
            }

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            int HeaderTextHue = 2603;
            int WhiteTextHue = 2655;
            int GreyTextHue = 1102; //2036
            int MainTextHue = 149; 
            int BlueTextHue = 2603;
            int GreenTextHue = 0x3F;
            int RedTextHue = 1256;
            int ValueTextHue = 2655;
            int DifficultyTextHue = 2114;
            int SlayerGroupTextHue = 2606;

            AddImage(8, 4, 1250, 2499); //Background

            string creatureDisplayName = bc_Creature.GetTamedDisplayName();

            AddLabel(Utility.CenteredTextOffset(170, creatureDisplayName), 15, HeaderTextHue, creatureDisplayName);
            
            AddLabel(10, 0, 149, "Guide");
            AddButton(14, 15, 2094, 2095, 1, GumpButtonType.Reply, 0);
            
            switch (m_AnimalLoreGumpObject.m_AnimalLorePage)
            {
                #region Stats

                case AnimalLoreGumpPage.Stats:
                    if (bc_Creature.Tameable)
                    {
                        AddLabel(78, 370, WhiteTextHue, "Traits");
                        AddButton(45, 369, 4011, 4013, 2, GumpButtonType.Reply, 0);

                        if (traitsAvailable > 0)
                            AddLabel(123, 370, GreenTextHue, "(" + traitsAvailable.ToString() + " Available)");

                        if (bc_Creature.RessPenaltyCount > 0)
                        {
                            AddItem(172, 363, 6227, 0);
                            AddLabel(206, 370, 2116, bc_Creature.RessPenaltyCount.ToString());

                            AddButton(221, 369, 4029, 4031, 3, GumpButtonType.Reply, 0);
                            AddLabel(259, 370, 2116, "Info");
                        }

                        else
                        {
                            AddButton(221, 369, 4029, 4031, 3, GumpButtonType.Reply, 0);
                            AddLabel(259, 370, WhiteTextHue, "Info");
                        }
                    }
                    
                    int shrinkTableIcon = ShrinkTable.Lookup(bc_Creature);
                    
                    if (shrinkTableIcon == 6256)
                        shrinkTableIcon = 7960;

                    if (bc_Creature.IsHenchman)
                    {
                        Custom.BaseHenchman henchman = bc_Creature as Custom.BaseHenchman; 

                        int henchmanIcon = 8454;
                        int henchmanHue = 0;

                        if (bc_Creature.Female)
                            henchmanIcon = 8455;

                        if (!henchman.HenchmanHumanoid)
                        {
                            henchmanIcon = bc_Creature.TamedItemId;
                            henchmanHue = bc_Creature.TamedItemHue;
                        }

                        AddItem(74 + bc_Creature.TamedItemXOffset, 62 + bc_Creature.TamedItemYOffset, henchmanIcon, henchmanHue);
                    }

                    else
                    {
                        if (bc_Creature.TamedItemId != -1) //Creature Icon
                            AddItem(74 + bc_Creature.TamedItemXOffset, 62 + bc_Creature.TamedItemYOffset, bc_Creature.TamedItemId, bc_Creature.TamedItemHue);
                
                        else
                            AddItem(74 + bc_Creature.TamedItemXOffset, 62 + bc_Creature.TamedItemYOffset, shrinkTableIcon, 0);
                    }
                    
                    string creatureDifficulty = Utility.CreateDecimalString(bc_Creature.InitialDifficulty, 2);
                    string slayerGroup = bc_Creature.SlayerGroup.ToString();

                    int level = bc_Creature.ExperienceLevel;
                    int experience = bc_Creature.Experience;
                    int maxExperience = 0;

                    if (bc_Creature.ExperienceLevel < BaseCreature.MaxExperienceLevel)
                        maxExperience = BaseCreature.ExperiencePerLevel[bc_Creature.ExperienceLevel];

                    double passiveTamingSkillGainRemaining = player.m_PassiveSkillGainRemaining;

                    if (!bc_Creature.InPassiveTamingSkillGainRange(player))
                        passiveTamingSkillGainRemaining = 0;

                    string passiveTamingSkillGainRemainingText = Utility.CreateDecimalString(passiveTamingSkillGainRemaining, 1);

                    if (!(bc_Creature.IsStabled || bc_Creature.ControlMaster == player))
                        passiveTamingSkillGainRemainingText = "-";
                                        
                    int hitsAdjusted = bc_Creature.Hits;
                    int hitsMaxAdjusted = bc_Creature.HitsMax;

                    int stamAdjusted = bc_Creature.Stam;
                    int stamMaxAdjusted = bc_Creature.StamMax;

                    int manaAdjusted = bc_Creature.Mana;
                    int manaMaxAdjusted = bc_Creature.ManaMax;

                    int minDamageAdjusted = bc_Creature.DamageMin;
                    int maxDamageAdjusted = bc_Creature.DamageMax;

                    double wrestlingAdjusted = bc_Creature.Skills.Wrestling.Value;
                    double evalIntAdjusted = bc_Creature.Skills.EvalInt.Value;
                    double mageryAdjusted = bc_Creature.Skills.Magery.Value;
                    double magicResistAdjusted = bc_Creature.Skills.MagicResist.Value;
                    double poisoningAdjusted = bc_Creature.Skills.Poisoning.Value;

                    int virtualArmorAdjusted = bc_Creature.VirtualArmor;            

                    //Tamed Scalars 
                    string hitsTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMaxHitsCreationScalar - 1), 1);
                    int hitsTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMaxHitsCreationScalar - 1) >= 0)
                    {
                        hitsTamedScalar = "+" + hitsTamedScalar;
                        hitsTamedColor = GreenTextHue;
                    }
                    if (hitsTamedScalar.Length == 3)
                        hitsTamedScalar = hitsTamedScalar.Insert(2, ".0");

                    string stamTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseDexCreationScalar - 1), 1);
                    int stamTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseDexCreationScalar - 1) >= 0)
                    {
                        stamTamedScalar = "+" + stamTamedScalar;
                        stamTamedColor = GreenTextHue;
                    }
                    if (stamTamedScalar.Length == 3)
                        stamTamedScalar = stamTamedScalar.Insert(2, ".0");

                    string manaTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMaxManaCreationScalar - 1), 1);
                    int manaTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMaxManaCreationScalar - 1) >= 0)
                    {
                        manaTamedScalar = "+" + manaTamedScalar;
                        manaTamedColor = GreenTextHue;
                    }
                    if (manaTamedScalar.Length == 3)
                        manaTamedScalar = manaTamedScalar.Insert(2, ".0");

                    string damageTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMaxDamageCreationScalar - 1), 1);
                    int damageTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMaxDamageCreationScalar - 1) >= 0)
                    {
                        damageTamedScalar = "+" + damageTamedScalar;
                        damageTamedColor = GreenTextHue;
                    }
                    if (damageTamedScalar.Length == 3)
                        damageTamedScalar = damageTamedScalar.Insert(2, ".0");

                    string virtualArmorTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseVirtualArmorCreationScalar - 1), 1);
                    int virtualArmorTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseVirtualArmorCreationScalar - 1) >= 0)
                    {
                        virtualArmorTamedScalar = "+" + virtualArmorTamedScalar;
                        virtualArmorTamedColor = GreenTextHue;
                    }
                    if (virtualArmorTamedScalar.Length == 3)
                        virtualArmorTamedScalar = virtualArmorTamedScalar.Insert(2, ".0");

                    string wrestlingTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseWrestlingCreationScalar - 1), 1);
                    int wrestlingTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseWrestlingCreationScalar - 1) >= 0)
                    {
                        wrestlingTamedScalar = "+" + wrestlingTamedScalar;
                        wrestlingTamedColor = GreenTextHue;
                    }
                    if (wrestlingTamedScalar.Length == 3)
                        wrestlingTamedScalar = wrestlingTamedScalar.Insert(2, ".0");

                    string evalIntTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseEvalIntCreationScalar - 1), 1);
                    int evalIntTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseEvalIntCreationScalar - 1) >= 0)
                    {
                        evalIntTamedScalar = "+" + evalIntTamedScalar;
                        evalIntTamedColor = GreenTextHue;
                    }
                    if (evalIntTamedScalar.Length == 3)
                        evalIntTamedScalar = evalIntTamedScalar.Insert(2, ".0");

                    string mageryTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMageryCreationScalar - 1), 1);
                    int mageryTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMageryCreationScalar - 1) >= 0)
                    {
                        mageryTamedScalar = "+" + mageryTamedScalar;
                        mageryTamedColor = GreenTextHue;
                    }
                    if (mageryTamedScalar.Length == 3)
                        mageryTamedScalar = mageryTamedScalar.Insert(2, ".0");

                    string magicResistTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBaseMagicResistCreationScalar - 1), 1);
                    int magicResistTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBaseMagicResistCreationScalar - 1) >= 0)
                    {
                        magicResistTamedScalar = "+" + magicResistTamedScalar;
                        magicResistTamedColor = GreenTextHue;
                    }
                    if (magicResistTamedScalar.Length == 3)
                        magicResistTamedScalar = magicResistTamedScalar.Insert(2, ".0");

                    string poisoningTamedScalar = Utility.CreateDecimalPercentageString((bc_Creature.TamedBasePoisoningCreationScalar - 1), 1);
                    int poisoningTamedColor = RedTextHue;
                    if ((bc_Creature.TamedBasePoisoningCreationScalar - 1) >= 0)
                    {
                        poisoningTamedScalar = "+" + poisoningTamedScalar;
                        poisoningTamedColor = GreenTextHue;
                    }
                    if (poisoningTamedScalar.Length == 3)
                        poisoningTamedScalar = poisoningTamedScalar.Insert(2, ".0");

                    if (bc_Creature.IsStabled || (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile))
                    {
                        AddLabel(166, 50, MainTextHue, "Level:");                        
                        if (traitsAvailable > 0)
                            AddLabel(210, 50, GreenTextHue, level.ToString());
                        else
                            AddLabel(210, 50, ValueTextHue, level.ToString());

                        if (bc_Creature.ExperienceLevel < BaseCreature.MaxExperienceLevel)
                        {
                            AddLabel(175, 70, MainTextHue, "Exp:");
                            AddLabel(210, 70, ValueTextHue, experience.ToString() + " / " + maxExperience.ToString());
                        }

                        else
                        {
                            AddLabel(175, 70, MainTextHue, "Exp:");
                            AddLabel(210, 70, ValueTextHue, "Maxed");
                        }

                        AddLabel(160, 90, MainTextHue, "Passive Taming");
                        AddLabel(130, 105, MainTextHue, "Skill Gain Remaining");
                        if (passiveTamingSkillGainRemaining > 0)
                            AddLabel(260, 100, 2603, passiveTamingSkillGainRemainingText);
                        else
                            AddLabel(260, 100, 2401, passiveTamingSkillGainRemainingText);
                    }

                    else
                    {
                        AddLabel(170, 45, MainTextHue, "Creature Difficulty");
                        AddLabel(Utility.CenteredTextOffset(220, creatureDifficulty), 65, DifficultyTextHue, creatureDifficulty);

                        AddLabel(185, 85, MainTextHue, "Slayer Group");
                        AddLabel(205, 105, SlayerGroupTextHue, slayerGroup);

                        hitsAdjusted = (int)((double)bc_Creature.TamedBaseMaxHits * bc_Creature.TamedBaseMaxHitsCreationScalar);
                        hitsMaxAdjusted = hitsAdjusted;

                        stamAdjusted = (int)((double)bc_Creature.TamedBaseDex * bc_Creature.TamedBaseDexCreationScalar);
                        stamMaxAdjusted = stamAdjusted;

                        manaAdjusted = (int)((double)bc_Creature.TamedBaseMaxMana * bc_Creature.TamedBaseMaxManaCreationScalar);
                        manaMaxAdjusted = manaAdjusted;                

                        minDamageAdjusted = (int)((double)bc_Creature.TamedBaseMinDamage * bc_Creature.TamedBaseMinDamageCreationScalar);
                        maxDamageAdjusted = (int)((double)bc_Creature.TamedBaseMaxDamage * bc_Creature.TamedBaseMaxDamageCreationScalar);

                        virtualArmorAdjusted = (int)((double)bc_Creature.TamedBaseVirtualArmor * bc_Creature.TamedBaseVirtualArmorCreationScalar);

                        wrestlingAdjusted = bc_Creature.TamedBaseWrestling * bc_Creature.TamedBaseWrestlingCreationScalar;
                        evalIntAdjusted = bc_Creature.TamedBaseEvalInt * bc_Creature.TamedBaseEvalIntCreationScalar;
                        mageryAdjusted = bc_Creature.TamedBaseMagery * bc_Creature.TamedBaseMageryCreationScalar;                
                        magicResistAdjusted = bc_Creature.TamedBaseMagicResist * bc_Creature.TamedBaseMagicResistCreationScalar;
                        poisoningAdjusted = bc_Creature.TamedBasePoisoning * bc_Creature.TamedBasePoisoningCreationScalar;                
                    }
                    
                    int labelX = 45;
                    int valuesX = 140;
                    int tamedScalarsX = 245;

                    int startY = 125;

                    int rowHeight = 18;
                    int rowSpacer = 0;

                    bool showTamedScalars = false;

                    if (bc_Creature.Tameable)
                    {
                        showTamedScalars = true;

                        if (bc_Creature.IsHenchman)
                            AddLabel(labelX, startY, MainTextHue, "Min Begging:");
                        else
                            AddLabel(labelX, startY, MainTextHue, "Min Taming:");           

                        AddLabel(valuesX, startY, ValueTextHue, bc_Creature.MinTameSkill.ToString());
                        startY += rowHeight;

                        AddLabel(labelX, startY, MainTextHue, "Control Slots:");
                        AddLabel(valuesX, startY, ValueTextHue, bc_Creature.ControlSlots.ToString());               

                        AddLabel(242, startY, MainTextHue, "vs Avg.");

                        startY += rowHeight;
                        startY += rowSpacer;
                    }

                    AddLabel(labelX, startY, MainTextHue, "Hits:");
                    AddLabel(valuesX, startY, ValueTextHue, hitsAdjusted + " / " + hitsMaxAdjusted);
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, hitsTamedColor, hitsTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Stam:");
                    AddLabel(valuesX, startY, ValueTextHue, stamAdjusted + " / " + stamMaxAdjusted);
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, stamTamedColor, stamTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Mana:");
                    AddLabel(valuesX, startY, ValueTextHue, manaAdjusted + " / " + manaMaxAdjusted);
                    if (showTamedScalars && manaAdjusted > 0 && manaMaxAdjusted > 0)
                        AddLabel(tamedScalarsX, startY, manaTamedColor, manaTamedScalar);

                    startY += rowHeight;
                    startY += rowSpacer;
            
                    AddLabel(labelX, startY, MainTextHue, "Damage:");
                    AddLabel(valuesX, startY, ValueTextHue, minDamageAdjusted + " - " + maxDamageAdjusted);
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, damageTamedColor, damageTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Armor:");
                    AddLabel(valuesX, startY, ValueTextHue, virtualArmorAdjusted.ToString());
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, virtualArmorTamedColor, virtualArmorTamedScalar);

                    startY += rowHeight;
                    startY += rowSpacer;

                    if (bc_Creature.IsHenchman)
                        AddLabel(labelX, startY, MainTextHue, "Combat Skill:");
                    else
                        AddLabel(labelX, startY, MainTextHue, "Wrestling:");
                    AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(wrestlingAdjusted).ToString());
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, wrestlingTamedColor, wrestlingTamedScalar);

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Magery:");
                    if (mageryAdjusted == 0)
                        AddLabel(valuesX, startY, ValueTextHue, "-");
                    else
                    {
                        AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(mageryAdjusted).ToString());
                        if (showTamedScalars)
                            AddLabel(tamedScalarsX, startY, mageryTamedColor, mageryTamedScalar);
                    }

                    startY += rowHeight;
            
                    AddLabel(labelX, startY, MainTextHue, "Eval Int:");
                    if (evalIntAdjusted == 0)
                        AddLabel(valuesX, startY, ValueTextHue, "-");
                    else
                    {
                        AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(evalIntAdjusted).ToString());
                        if (showTamedScalars)
                            AddLabel(tamedScalarsX, startY, evalIntTamedColor, evalIntTamedScalar);
                    }

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Magic Resist:");
                    AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(magicResistAdjusted).ToString());
                    if (showTamedScalars)
                        AddLabel(tamedScalarsX, startY, magicResistTamedColor, magicResistTamedScalar);

                    startY += rowHeight;
            
                    AddLabel(labelX, startY, MainTextHue, "Poisoning:");
                    if (bc_Creature.HitPoison != null)
                    {
                        AddLabel(valuesX, startY, ValueTextHue, RoundToTenth(poisoningAdjusted).ToString() + " (" + bc_Creature.HitPoison.Name + ")");
                        if (showTamedScalars)
                            AddLabel(tamedScalarsX, startY, poisoningTamedColor, poisoningTamedScalar);
                    }

                    else
                        AddLabel(valuesX, startY, ValueTextHue, "-");           

                    startY += rowHeight;

                    AddLabel(labelX, startY, MainTextHue, "Poison Resist:");
                    if (bc_Creature.PoisonResistance > 0)
                    {
                        if (bc_Creature.PoisonResistance > 1)
                            AddLabel(valuesX, startY, ValueTextHue, "Reduced " + bc_Creature.PoisonResistance.ToString() + " Levels");
                        else
                            AddLabel(valuesX, startY, ValueTextHue, "Reduced " + bc_Creature.PoisonResistance.ToString() + " Level");
                    }

                    else
                        AddLabel(valuesX, startY, ValueTextHue, "-");

                    startY += rowHeight; 
                break;

                #endregion

                #region Traits

                case AnimalLoreGumpPage.Traits:
                    AddLabel(78, 370, WhiteTextHue, "Stats");
                    AddButton(45, 369, 4011, 4013, 2, GumpButtonType.Reply, 0);
                    
                    AddButton(221, 369, 4029, 4031, 3, GumpButtonType.Reply, 0);
                    AddLabel(259, 370, WhiteTextHue, "Info");
                    
                    string traitsText = "Traits";

                    if ((bc_Creature.IsStabled || (bc_Creature.Controlled && bc_Creature.ControlMaster == m_Player)) && traitsAvailable > 0)
                    {
                        if (traitsAvailable == 1)
                            traitsText = traitsAvailable.ToString() + " Trait Available";

                        else
                            traitsText = traitsAvailable.ToString() + " Traits Available";

                        AddLabel(Utility.CenteredTextOffset(175, traitsText), 45, 2606, traitsText);
                    }

                    else
                        AddLabel(145, 45, 2606, traitsText);

                    int traitIndex = 0;

                    int iStartY = 60;
                    int rowSpacing = 57;  
                    
                    for (int a = 0; a < bc_Creature.TraitsList.Count; a++)
                    {
                        int traitLevelIndex = (int)(Math.Floor((double)a / 2));
                        int buttonBaseIndex = 10 + (10 * traitLevelIndex);
                        
                        FollowerTraitType traitOption = bc_Creature.TraitsList[a];
                        FollowerTraitDetail followerTraitDetail = FollowerTraits.GetFollowerTraitDetail(traitOption);

                        bool controlled = false;
                        bool traitAvailable = false;                        

                        if (bc_Creature.IsStabled || bc_Creature.Controlled && bc_Creature.ControlMaster == m_Player)
                            controlled = true;

                        if ((traitLevelIndex <= (traitsEarned - 1)) && bc_Creature.m_SelectedTraits[traitLevelIndex] == FollowerTraitType.None)                        
                            traitAvailable = true;

                        //Left Side
                        if (a % 2 == 0)
                        {
                            if (traitAvailable && controlled && m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] == TraitSelectionType.Left)
                            {
                                AddItem(40 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(85, followerTraitDetail.Name), iStartY, GreenTextHue, followerTraitDetail.Name);

                                AddButton(85, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(105, iStartY + 30, 2550, "Info");

                                AddButton(145, iStartY + 30, 9909, 9910, buttonBaseIndex + 1, GumpButtonType.Reply, 0);
                            }

                            else if (traitAvailable && controlled && m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] != TraitSelectionType.Left)
                            {
                                AddItem(40 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(85, followerTraitDetail.Name), iStartY, WhiteTextHue, followerTraitDetail.Name);

                                AddButton(85, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(105, iStartY + 30, 2550, "Info");

                                AddButton(145, iStartY + 30, 9910, 9909, buttonBaseIndex + 1, GumpButtonType.Reply, 0);
                            }

                            else if (bc_Creature.m_SelectedTraits[traitLevelIndex] == traitOption)
                            {
                                AddItem(40 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(85, followerTraitDetail.Name), iStartY, 149, followerTraitDetail.Name);

                                AddButton(85, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(105, iStartY + 30, 2550, "Info");
                            }

                            else
                            {
                                AddItem(40 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(85, followerTraitDetail.Name), iStartY, 1102, followerTraitDetail.Name);

                                AddButton(85, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(105, iStartY + 30, 2401, "Info");
                            }
                        }

                        //Right Side
                        else
                        {
                            if (traitAvailable && controlled && m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] == TraitSelectionType.Right)
                            {
                                AddItem(195 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(240, followerTraitDetail.Name), iStartY, GreenTextHue, followerTraitDetail.Name);

                                AddButton(240, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(260, iStartY + 30, 2550, "Info");

                                AddButton(172, iStartY + 30, 9903, 9904, buttonBaseIndex + 2, GumpButtonType.Reply, 0);
                            }

                            else if (traitAvailable && controlled && m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] != TraitSelectionType.Right)
                            {
                                AddItem(195 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(240, followerTraitDetail.Name), iStartY, WhiteTextHue, followerTraitDetail.Name);

                                AddButton(240, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(260, iStartY + 30, 2550, "Info");

                                AddButton(172, iStartY + 30, 9904, 9903, buttonBaseIndex + 2, GumpButtonType.Reply, 0);
                            }

                            else if (bc_Creature.m_SelectedTraits[traitLevelIndex] == traitOption)
                            {
                                AddItem(195 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(240, followerTraitDetail.Name), iStartY, 149, followerTraitDetail.Name);

                                AddButton(240, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(260, iStartY + 30, 2550, "Info");
                            }

                            else
                            {
                                AddItem(195 + followerTraitDetail.IconOffsetX, iStartY + 20 + followerTraitDetail.IconOffsetY, followerTraitDetail.IconItemId, followerTraitDetail.IconHue);
                                AddLabel(Utility.CenteredTextOffset(240, followerTraitDetail.Name), iStartY, 1102, followerTraitDetail.Name);

                                AddButton(240, iStartY + 33, 2118, 2117, buttonBaseIndex, GumpButtonType.Reply, 0);
                                AddLabel(260, iStartY + 30, 2401, "Info");
                            }

                            iStartY += rowSpacing;
                        }                        
                    }
                    
                    bool selectionsMade = false;

                    for (int a = 0; a < m_AnimalLoreGumpObject.m_TraitGumpSelections.Count; a++)
                    {
                        if (m_AnimalLoreGumpObject.m_TraitGumpSelections[a] != TraitSelectionType.None)
                            selectionsMade = true;
                    }

                    if (selectionsMade && traitsAvailable > 0 && (bc_Creature.IsStabled || (bc_Creature.Controlled && bc_Creature.ControlMaster == m_Player)))
                    {
                        AddLabel(90, 345, 68, "Confirm Trait Selection");
                        AddButton(140, 370, 2076, 2075, 9, GumpButtonType.Reply, 0);
                    }                   
                break;

                #endregion

                #region Info

                case AnimalLoreGumpPage.Info:
                    AddLabel(78, 370, WhiteTextHue, "Traits");
                    AddButton(45, 369, 4011, 4013, 2, GumpButtonType.Reply, 0);

                    if (traitsAvailable > 0)            
                        AddLabel(120, 370, GreenTextHue, "(" + traitsAvailable.ToString() + " Available)");
                    
                    AddButton(221, 369, 4029, 4031, 3, GumpButtonType.Reply, 0);
                    AddLabel(259, 370, WhiteTextHue, "Stats");
                    
                    if (bc_Creature.RessPenaltyCount > 0)
                    {
                        AddLabel(73, 47, 149, "Current Ress Penalty");
                        AddItem(198, 42, 6227, 0);
                        AddLabel(232, 47, 2116, "x" + bc_Creature.RessPenaltyCount.ToString());

                        string timeRemainingText = "Expires in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_Creature.RessPenaltyExpiration, true, false, true, true, true);
                        string textA = "-" + Utility.CreateDecimalPercentageString(bc_Creature.GetRessPenaltyDamageDealtModifier(), 0) + " Damage Dealt";
                        string textB = "+" + Utility.CreateDecimalPercentageString(bc_Creature.GetRessPenaltyDamageReceivedModifier(), 0) + " Damage Received";

                        AddLabel(Utility.CenteredTextOffset(165, timeRemainingText), 67, 2550, timeRemainingText);
                        AddLabel(Utility.CenteredTextOffset(165, textA), 87, 1256, textA);
                        AddLabel(Utility.CenteredTextOffset(165, textB), 107, 1256, textB);
                    }                   
                break;

                #endregion
            }    
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_AnimalLoreGumpObject == null) return;
            if (m_AnimalLoreGumpObject.bc_Creature == null) return;
            if (m_AnimalLoreGumpObject.bc_Creature.Deleted) return;
            if (!m_AnimalLoreGumpObject.bc_Creature.Alive && !m_AnimalLoreGumpObject.bc_Creature.IsBonded) return;

            BaseCreature bc_Creature = m_AnimalLoreGumpObject.bc_Creature;

            int traitsEarned = FollowerTraits.GetFollowerTraitsEarned(bc_Creature);
            int traitsAvailable = FollowerTraits.GetFollowerTraitsAvailable(bc_Creature);
            int totalTraits = (int)((double)bc_Creature.TraitsList.Count / 2);

            bool closeGump = true;

            switch (m_AnimalLoreGumpObject.m_AnimalLorePage)
            {
                #region Stats

                case AnimalLoreGumpPage.Stats:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Traits
                        case 2:
                            if (bc_Creature.Tameable)                            
                                m_AnimalLoreGumpObject.m_AnimalLorePage = AnimalLoreGumpPage.Traits;

                            m_AnimalLoreGumpObject.m_Page = 0;
                            m_Player.SendSound(changeGumpSound);

                            closeGump = false;
                        break;

                        //Info
                        case 3:
                            if (bc_Creature.Tameable)
                                m_AnimalLoreGumpObject.m_AnimalLorePage = AnimalLoreGumpPage.Info;

                            m_AnimalLoreGumpObject.m_Page = 0;
                            m_Player.SendSound(changeGumpSound);

                            closeGump = false;
                        break;
                    }
                break;

                #endregion

                #region Traits

                case AnimalLoreGumpPage.Traits:
                    bool selectionsMade = false;

                    for (int a = 0; a < m_AnimalLoreGumpObject.m_TraitGumpSelections.Count; a++)
                    {
                        if (m_AnimalLoreGumpObject.m_TraitGumpSelections[a] != TraitSelectionType.None)
                            selectionsMade = true;
                    }

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Stats
                        case 2:
                            m_AnimalLoreGumpObject.m_AnimalLorePage = AnimalLoreGumpPage.Stats;

                            m_AnimalLoreGumpObject.m_Page = 0;
                            m_Player.SendSound(changeGumpSound);

                            closeGump = false;
                        break;

                        //Info
                        case 3:
                            m_AnimalLoreGumpObject.m_AnimalLorePage = AnimalLoreGumpPage.Info;

                            m_AnimalLoreGumpObject.m_Page = 0;
                            m_Player.SendSound(changeGumpSound);

                            closeGump = false;
                        break;

                        //Confirm Selection
                        case 9:
                            int traitsChanged = 0;
                            
                            if (selectionsMade && (bc_Creature.IsStabled || bc_Creature.Controlled && bc_Creature.ControlMaster == m_Player))
                            {
                                for (int a = 0; a < m_AnimalLoreGumpObject.m_TraitGumpSelections.Count; a++)
                                {
                                    int traitLevelIndex = a;

                                    TraitSelectionType traitSelection = m_AnimalLoreGumpObject.m_TraitGumpSelections[a];
                                    
                                    if (traitSelection == TraitSelectionType.None)
                                        continue;

                                    if (a > traitsEarned - 1)
                                        continue;

                                    if (bc_Creature.m_SelectedTraits[traitLevelIndex] != FollowerTraitType.None)
                                        continue;

                                    FollowerTraitType newTraitChosen = FollowerTraitType.None;
                                    
                                    switch (traitSelection)
                                    {
                                        case TraitSelectionType.Left: newTraitChosen = bc_Creature.TraitsList[(traitLevelIndex * 2)]; break;
                                        case TraitSelectionType.Right: newTraitChosen = bc_Creature.TraitsList[(traitLevelIndex * 2) + 1]; break;
                                    }

                                    bc_Creature.m_SelectedTraits[traitLevelIndex] = newTraitChosen;

                                    traitsChanged++;
                                }

                                if (traitsChanged > 0)
                                {
                                    if (traitsChanged == 1)
                                        m_Player.SendMessage(0x3F, "Your creature gained a new trait.");

                                    else
                                        m_Player.SendMessage(0x3F, "Your creature has gained new traits.");

                                    m_Player.SendSound(traitAssignedSound);
                                }

                                else
                                    m_Player.SendMessage("You have not made any trait selections for your creature.");
                            }

                            else
                                m_Player.SendMessage("You have not made any trait selections for your creature.");                            

                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10)         
                    {
                        int traitLevelIndex = (int)(Math.Floor((double)info.ButtonID / 10)) - 1;
                       
                        if (traitLevelIndex <= totalTraits - 1)
                        {
                            FollowerTraitType existingTrait = FollowerTraitType.None;

                            bool traitAvailable = false;

                            if (traitLevelIndex <= bc_Creature.m_SelectedTraits.Count - 1)
                            {
                                existingTrait = bc_Creature.m_SelectedTraits[traitLevelIndex];

                                if ((traitLevelIndex <= (traitsEarned - 1)) && bc_Creature.m_SelectedTraits[traitLevelIndex] == FollowerTraitType.None)
                                    traitAvailable = true;
                            }

                            int buttonIndex = info.ButtonID % 10;

                            int traitSelectionIndex = 0; 
                                                                                    
                            switch (buttonIndex)
                            {
                                //Left Info
                                case 0:
                                    traitSelectionIndex = (traitLevelIndex * 2);

                                    FollowerTraitType traitOption = bc_Creature.TraitsList[traitSelectionIndex];
                                    FollowerTraitDetail followerTraitDetail = FollowerTraits.GetFollowerTraitDetail(traitOption);

                                    m_Player.SendMessage(2550, followerTraitDetail.Name + ": " +  followerTraitDetail.Description + ".");
                                break;

                                //Left Selection
                                case 1:
                                    if (traitAvailable && (bc_Creature.IsStabled || bc_Creature.Controlled && bc_Creature.ControlMaster == m_Player))
                                    {
                                        if (m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] != TraitSelectionType.Left)
                                            m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] = TraitSelectionType.Left;

                                        else
                                            m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] = TraitSelectionType.None;

                                        m_Player.SendSound(selectionSound);
                                    }
                                break;

                                //Right Selection
                                case 2:
                                    if (traitAvailable && bc_Creature.Controlled && bc_Creature.ControlMaster == m_Player)
                                    {
                                         if (m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] != TraitSelectionType.Right)
                                            m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] = TraitSelectionType.Right;

                                        else 
                                            m_AnimalLoreGumpObject.m_TraitGumpSelections[traitLevelIndex] = TraitSelectionType.None;

                                        m_Player.SendSound(selectionSound);
                                    }
                                break;

                                //Right Info
                                case 3:
                                    traitSelectionIndex = (traitLevelIndex * 2) + 1;

                                    traitOption = bc_Creature.TraitsList[traitSelectionIndex];
                                    followerTraitDetail = FollowerTraits.GetFollowerTraitDetail(traitOption);

                                    m_Player.SendMessage(2550, followerTraitDetail.Name + ": " + followerTraitDetail.Description + ".");
                                break;                               
                            }                            
                        }

                        closeGump = false;
                    }                   
                break;

                #endregion

                #region Info

                case AnimalLoreGumpPage.Info:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Traits
                        case 2:
                            if (bc_Creature.Tameable)
                                m_AnimalLoreGumpObject.m_AnimalLorePage = AnimalLoreGumpPage.Traits;

                            m_AnimalLoreGumpObject.m_Page = 0;
                            m_Player.SendSound(changeGumpSound);

                            closeGump = false;
                        break;

                        //Stats
                        case 3:
                            m_AnimalLoreGumpObject.m_AnimalLorePage = AnimalLoreGumpPage.Stats;

                            m_AnimalLoreGumpObject.m_Page = 0;
                            m_Player.SendSound(changeGumpSound);

                            closeGump = false;
                        break;
                    }
                break;

                #endregion
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(AnimalLoreGump));
                m_Player.SendGump(new AnimalLoreGump(m_Player, m_AnimalLoreGumpObject));
            }

            else
                m_Player.SendSound(closeGumpSound);                
        }

        public double RoundToTenth(double skillValue)
        {
            double newValue = skillValue;

            newValue *= 10;
            newValue = Math.Round(newValue);
            newValue /= 10;

            return newValue;
        }
    }

    public class AnimalLoreGumpObject
    {
        public BaseCreature bc_Creature;

        public AnimalLoreGump.AnimalLoreGumpPage m_AnimalLorePage = AnimalLoreGump.AnimalLoreGumpPage.Stats;
        public int m_Page = 0;

        public List<AnimalLoreGump.TraitSelectionType> m_TraitGumpSelections = new List<AnimalLoreGump.TraitSelectionType>();

        public AnimalLoreGumpObject(BaseCreature creature)
        {
            bc_Creature = creature;
        }
    }
}