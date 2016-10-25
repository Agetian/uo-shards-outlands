using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.Craft;
using System.Globalization;

namespace Server
{
    public class AspectGearGump : Gump
    {
        PlayerMobile m_Player;
        AspectMould m_AspectMould;
        Item m_Item;

        AspectEnum m_SelectedAspect = AspectEnum.Air;

        public AspectGearGump(Mobile player, AspectMould aspectMould, Item item, AspectEnum selectedAspect): base(10, 10)
        {
            m_Player = player as PlayerMobile;
            m_AspectMould = aspectMould;
            m_Item = item;
            m_SelectedAspect = selectedAspect;

            if (m_Player == null) return;
            if (m_Player.Deleted || !m_Player.Alive) return;
            if (m_AspectMould == null) return;
            if (m_AspectMould.Deleted) return;
            if (m_Item == null) return;
            if (m_Item.Deleted) return;

            if (!m_AspectMould.IsChildOf(m_Player.Backpack))
            {
                m_Player.SendMessage("The mould you wish to use must be in your backpack in order to use it.");
                return;
            }

            Item targetItem = m_Item as Item;

            if (targetItem == null)
            {
                m_Player.SendMessage("You cannot improve that.");
                return;
            }

            if (!(targetItem is BaseWeapon || targetItem is BaseArmor))
            {
                m_Player.SendMessage("You may only improve weapons and armor.");
                return;
            }

            if (targetItem is BaseShield)
            {
                m_Player.SendMessage("You may only improve weapons and armor.");
                return;
            }

            if (!(targetItem.IsChildOf(m_Player.Backpack) || targetItem.RootParent == m_Player))
            {
                m_Player.SendMessage("The item you wish to improve must be equipped or in your backpack.");
                return;
            }

            Type itemType = targetItem.GetType();
            CraftItem craftItem = null;

            switch (m_AspectMould.MouldType)
            {
                case AspectMould.MouldSkillType.Blacksmithy:
                    craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem == null)
                    {
                        m_Player.SendMessage("You may only improve items craftable through blacksmithing with this mould.");
                        return;
                    }
                break;

                case AspectMould.MouldSkillType.Carpentry:
                    craftItem = DefCarpentry.CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem == null)
                    {
                        m_Player.SendMessage("You may only improve items craftable through carpentry with this mould.");
                        return;
                    }
                break;

                case AspectMould.MouldSkillType.Tailoring:
                    craftItem = DefTailoring.CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem == null)
                    {
                        m_Player.SendMessage("You may only improve items craftable through tailoring with this mould.");
                        return;
                    }
                break;
            }

            if (targetItem.DecorativeEquipment)
            {
                m_Player.SendMessage("Decorative equipment may not be improved.");
                return;
            }

            if (targetItem.LootType == LootType.Newbied)
            {
                m_Player.SendMessage("Newbied equipment may not be improved.");
                return;
            }

            if (targetItem.LootType == LootType.Blessed && targetItem.Aspect == AspectEnum.None)
            {
                m_Player.SendMessage("Blessed equipment may not be improved.");
                return;
            }

            BaseWeapon weapon = targetItem as BaseWeapon;
            BaseArmor armor = targetItem as BaseArmor;

            if (weapon != null)
            {
                if (weapon.TrainingWeapon)
                {
                    m_Player.SendMessage("Training weapons may not be improved.");
                    return;
                }

                if (weapon.DurabilityLevel != WeaponDurabilityLevel.Regular || weapon.AccuracyLevel != WeaponAccuracyLevel.Regular || weapon.DamageLevel != WeaponDamageLevel.Regular)
                {
                    m_Player.SendMessage("Magical weapons may not be improved.");
                    return;
                }
            }

            if (armor != null)
            {
                if (armor.DurabilityLevel != ArmorDurabilityLevel.Regular || armor.ProtectionLevel != ArmorProtectionLevel.Regular)
                {
                    m_Player.SendMessage("Magical weapons may not be improved.");
                    return;
                }
            }

            //Aspect Item
            if (targetItem.Aspect != AspectEnum.None && targetItem.TierLevel >= 1 && targetItem.TierLevel <= 10)
            {
                if (targetItem.TierLevel == 10)
                {
                    m_Player.SendMessage("That item is already at it's maximum tier level.");
                    return;
                }
            }

            else if (targetItem.Aspect == AspectEnum.None)
            {
                if (!(targetItem.Quality == Quality.Exceptional && targetItem.DisplayCrafter))
                {
                    m_Player.SendMessage("Only mastermarked items may be improved.");
                    return;
                }
            }

            #region Images 

            AddImage(256, 251, 103);
            AddImage(127, 251, 103);
            AddImage(4, 251, 103);
            AddImage(4, 329, 103);
            AddImage(129, 329, 103);
            AddImage(69, 97, 103);
            AddImage(4, 97, 103);
            AddImage(128, 2, 103);
            AddImage(4, 2, 103);
            AddImage(256, 97, 103);
            AddImage(256, 2, 103);
            AddImage(127, 183, 103);
            AddImage(4, 183, 103);
            AddImage(256, 329, 103);
            AddImage(256, 183, 103);
            AddImage(128, 380, 103);
            AddImage(4, 380, 103);
            AddImage(256, 380, 103);
            AddImage(16, 15, 3604, 2052);
            AddImage(133, 15, 3604, 2052);
            AddImage(260, 15, 3604, 2052);
            AddImage(16, 137, 3604, 2052);
            AddImage(133, 135, 3604, 2052);
            AddImage(260, 137, 3604, 2052);
            AddImage(16, 203, 3604, 2052);
            AddImage(133, 203, 3604, 2052);
            AddImage(260, 203, 3604, 2052);
            AddImage(17, 342, 3604, 2052);
            AddImage(134, 342, 3604, 2052);
            AddImage(260, 342, 3604, 2052);

            #endregion

            int WhiteTextHue = 2499;
            int DetailTextHue = 2603;            

            //Guide
            AddButton(18, 16, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(16, 3, 149, "Guide");

            if (targetItem.Aspect != AspectEnum.None && targetItem.TierLevel > 0)
                m_SelectedAspect = targetItem.Aspect;

            int newTier = targetItem.TierLevel + 1;

            string requiredMouldName = "";

            #region Mould Names

            switch (m_AspectMould.MouldType)
            {
                case AspectMould.MouldSkillType.Blacksmithy:
                    switch(newTier)
                    {
                        case 1: requiredMouldName = "Basic Smithing Mould"; break;
                        case 2: requiredMouldName = "Basic Smithing Mould"; break;

                        case 3: requiredMouldName = "Advanced Smithing Mould"; break;
                        case 4: requiredMouldName = "Advanced Smithing Mould"; break;

                        case 5: requiredMouldName = "Expert Smithing Mould"; break;
                        case 6: requiredMouldName = "Expert Smithing Mould"; break;

                        case 7: requiredMouldName = "Master Smithing Mould"; break;
                        case 8: requiredMouldName = "Master Smithing Mould"; break;

                        case 9: requiredMouldName = "Legendary Smithing Mould"; break;
                        case 10: requiredMouldName = "Legendary Smithing Mould"; break;
                    }
                break;

                case AspectMould.MouldSkillType.Carpentry:
                    switch (newTier)
                    {
                        case 1: requiredMouldName = "Basic Carpentry Mould"; break;
                        case 2: requiredMouldName = "Basic Carpentry Mould"; break;

                        case 3: requiredMouldName = "Advanced Carpentry Mould"; break;
                        case 4: requiredMouldName = "Advanced Carpentry Mould"; break;

                        case 5: requiredMouldName = "Expert Carpentry Mould"; break;
                        case 6: requiredMouldName = "Expert Carpentry Mould"; break;

                        case 7: requiredMouldName = "Master Carpentry Mould"; break;
                        case 8: requiredMouldName = "Master Carpentry Mould"; break;

                        case 9: requiredMouldName = "Legendary Carpentry Mould"; break;
                        case 10: requiredMouldName = "Legendary Carpentry Mould"; break;
                    }
                break;

                case AspectMould.MouldSkillType.Tailoring:
                    switch (newTier)
                    {
                        case 1: requiredMouldName = "Basic Tailoring Mould"; break;
                        case 2: requiredMouldName = "Basic Tailoring Mould"; break;

                        case 3: requiredMouldName = "Advanced Tailoring Mould"; break;
                        case 4: requiredMouldName = "Advanced Tailoring Mould"; break;

                        case 5: requiredMouldName = "Expert Tailoring Mould"; break;
                        case 6: requiredMouldName = "Expert Tailoring Mould"; break;

                        case 7: requiredMouldName = "Master Tailoring Mould"; break;
                        case 8: requiredMouldName = "Master Tailoring Mould"; break;

                        case 9: requiredMouldName = "Legendary Tailoring Mould"; break;
                        case 10: requiredMouldName = "Legendary Tailoring Mould"; break;
                    }
                break;
            }

            #endregion

            string itemName = "";
            string itemAspectName = "";

            string aspectName = AspectGear.GetAspectName(m_SelectedAspect);
            int aspectHue = AspectGear.GetAspectHue(m_SelectedAspect);
            int aspectTextHue = AspectGear.GetAspectTextHue(m_SelectedAspect);

            int itemOffsetX = 0;
            int itemOffsetY = 0;
            int itemIconItemdId = 0;

            if (weapon != null)
            {
                AspectWeaponDetail aspectWeaponDetail = AspectGear.GetAspectWeaponDetail(m_SelectedAspect);

                if (weapon.TierLevel == 0)
                    AddLabel(129, 25, 149, "Create Aspect Weapon"); 
    
                else
                    AddLabel(129, 25, 149, "Upgrade Aspect Tier");

                itemName = weapon.Name;

                if (itemName == null)
                    itemName = "";

                itemName = Utility.Capitalize(itemName);
                itemAspectName = aspectName + " Tier " + newTier.ToString();

                itemOffsetX = weapon.IconOffsetX;
                itemOffsetY = weapon.IconOffsetY;
                itemIconItemdId = weapon.IconItemId;

                AddLabel(Utility.CenteredTextOffset(195, itemName), 49, aspectTextHue, itemName);
                AddLabel(Utility.CenteredTextOffset(195, itemAspectName), 74, aspectTextHue, itemAspectName);
                AddItem(120 + itemOffsetX, 75 + itemOffsetY, itemIconItemdId, aspectHue); //Image                

                int newDurability = AspectGear.BaselineDurability + (AspectGear.IncreasedDurabilityPerTier * newTier);

                int adjustedBlessedCharges = AspectGear.ArcaneMaxCharges;

                double accuracy = 100 * (AspectGear.BaseAccuracy + (AspectGear.AccuracyPerTier * (double)newTier));
                double tactics = AspectGear.BaseTactics + (AspectGear.TacticsPerTier * (double)newTier);

                double effectChance = AspectGear.BaseEffectChance + (AspectGear.BaseEffectChancePerTier * (double)newTier);

                effectChance *= AspectGear.GetEffectWeaponSpeedScalar(weapon);
               
                AddLabel(137, 168, WhiteTextHue, "Durability:");
                AddLabel(214, 168, aspectTextHue, newDurability.ToString() + "/" + newDurability.ToString());
                AddLabel(142, 188, WhiteTextHue, "Accuracy:");
                AddLabel(214, 188, aspectTextHue, "+" + accuracy.ToString() + "%");
                AddLabel(153, 208, WhiteTextHue, "Tactics:");
                AddLabel(214, 208, aspectTextHue, "+" + tactics.ToString());
                AddLabel(110, 228, WhiteTextHue, "Effect Chance:");
                AddLabel(214, 228, aspectTextHue, Utility.CreateDecimalPercentageString(effectChance, 1));
                AddLabel(109, 248, WhiteTextHue, "(scaled for weapon speed)");
                AddLabel(149, 273, WhiteTextHue, "Special Effect");
                AddLabel(169, 293, aspectTextHue, aspectWeaponDetail.m_EffectDisplayName);
                AddButton(149, 297, 1209, 1210, 2, GumpButtonType.Reply, 0);   
            }

            if (armor != null)
            {
                if (armor.TierLevel == 0)
                    AddLabel(135, 25, 149, "Create Aspect Armor");

                else
                    AddLabel(135, 25, 149, "Upgrade Aspect Tier");

                itemName = armor.Name;

                if (itemName == null)
                    itemName = "";

                itemName = Utility.Capitalize(itemName);
                itemAspectName = aspectName + " Tier " + (targetItem.TierLevel + 1).ToString();

                itemOffsetX = armor.IconOffsetX;
                itemOffsetY = armor.IconOffsetY;
                itemIconItemdId = armor.IconItemId;

                AddLabel(Utility.CenteredTextOffset(200, itemName), 49, aspectTextHue, itemName);
                AddLabel(Utility.CenteredTextOffset(200, itemAspectName), 74, aspectTextHue, itemAspectName);
                AddItem(120 + itemOffsetX, 75 + itemOffsetY, itemIconItemdId, aspectHue); //Image

                //TEST: FINISH!!!

                //Armor Details
                AddLabel(39, 165, WhiteTextHue, "Total Armor:");
                AddLabel(126, 165, DetailTextHue, "20");
                AddLabel(60, 185, WhiteTextHue, "Dex Loss:");
                AddLabel(126, 185, DetailTextHue, "-5");
                AddLabel(47, 205, WhiteTextHue, "Meditation:");
                AddLabel(126, 205, DetailTextHue, "0%");

                //Effects Loop 
                int startY = 165;
                int spacingY = 40;

                for (int a = 0; a < 4; a++)
                {
                    string effectName = "Effect";
                    string effectPercent = "25%";

                    AddLabel(Utility.CenteredTextOffset(260, effectName), startY, WhiteTextHue, effectName);
                    AddLabel(Utility.CenteredTextOffset(260, effectPercent), startY + 20, DetailTextHue, effectPercent);

                    startY += spacingY;
                }
            }

            //Change Aspect

            AddLabel(73, 347, 149, "Aspect");
            AddLabel(Utility.CenteredTextOffset(94, aspectName), 376, aspectTextHue, aspectName);

            if (m_Item.TierLevel == 0)
            {
                AddButton(25, 376, 9909, 9909, 3, GumpButtonType.Reply, 0); //Left
                AddButton(139, 376, 9903, 9903, 4, GumpButtonType.Reply, 0); //Right
            }

            //Create or Upgrade
            AddButton(38, 432, 2152, 2154, 5, GumpButtonType.Reply, 0);
            
            if (targetItem.TierLevel == 0)
                AddLabel(76, 436, 63, "Create");
            else
                AddLabel(76, 436, 63, "Upgrade");

            //Materials Required
            int distillationNeeded = 0;
            int coresNeeded = 0;
            
            if (targetItem.TierLevel == 0)
            {
                if (weapon != null)
                {
                    distillationNeeded = AspectGear.DistillationNeededForWeaponCreation;
                    coresNeeded = AspectGear.CoresNeededForWeaponCreation;
                }

                if (armor != null)
                {
                    distillationNeeded = AspectGear.DistillationNeededForArmorCreation;
                    coresNeeded = AspectGear.CoresNeededForArmorCreation;
                }
                   
            }

            else
            {
                if (weapon != null)
                {
                    distillationNeeded = AspectGear.DistillationNeededForWeaponUpgrade;
                    coresNeeded = AspectGear.CoresNeededForWeaponUpgrade;
                }

                if (armor != null)
                {
                    distillationNeeded = AspectGear.DistillationNeededForArmorUpgrade;
                    coresNeeded = AspectGear.CoresNeededForArmorUpgrade;
                }
            }            

            AddLabel(228, 347, 149, "Required Materials");

            AddItem(174, 368, 4323);
            AddLabel(226, 373, WhiteTextHue, requiredMouldName);

            AddItem(181, 403, 17686, aspectHue);
            AddLabel(225, 406, aspectTextHue, distillationNeeded.ToString() + " " + aspectName + " Distillation");

            AddItem(184, 439, 3985, aspectHue);
            AddLabel(225, 436, aspectTextHue, coresNeeded.ToString() + " " + aspectName + " Cores");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Backpack == null) return;

            Item targetItem = m_Item as Item;

            if (targetItem == null)
            {
                m_Player.SendMessage("You cannot improve that.");
                return;
            }

            if (!(targetItem is BaseWeapon || targetItem is BaseArmor))
            {
                m_Player.SendMessage("You may only improve weapons and armor.");
                return;
            }

            if (targetItem is BaseShield)
            {
                m_Player.SendMessage("You may only improve weapons and armor.");
                return;
            }

            if (!(targetItem.IsChildOf(m_Player.Backpack) || targetItem.RootParent == m_Player))
            {
                m_Player.SendMessage("The item you wish to improve must be equipped or in your backpack.");
                return;
            }

            Type itemType = targetItem.GetType();
            CraftItem craftItem = null;

            switch (m_AspectMould.MouldType)
            {
                case AspectMould.MouldSkillType.Blacksmithy:
                    craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem == null)
                    {
                        m_Player.SendMessage("You may only improve items craftable through blacksmithing with this mould.");
                        return;
                    }
                break;

                case AspectMould.MouldSkillType.Carpentry:
                    craftItem = DefCarpentry.CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem == null)
                    {
                        m_Player.SendMessage("You may only improve items craftable through carpentry with this mould.");
                        return;
                    }
                break;

                case AspectMould.MouldSkillType.Tailoring:
                    craftItem = DefTailoring.CraftSystem.CraftItems.SearchFor(itemType);

                    if (craftItem == null)
                    {
                        m_Player.SendMessage("You may only improve items craftable through tailoring with this mould.");
                        return;
                    }
                break;
            }

            if (targetItem.DecorativeEquipment)
            {
                m_Player.SendMessage("Decorative equipment may not be improved.");
                return;
            }

            if (targetItem.LootType == LootType.Newbied)
            {
                m_Player.SendMessage("Newbied equipment may not be improved.");
                return;
            }

            if (targetItem.LootType == LootType.Blessed && targetItem.Aspect == AspectEnum.None)
            {
                m_Player.SendMessage("Blessed equipment may not be improved.");
                return;
            }

            BaseWeapon weapon = targetItem as BaseWeapon;
            BaseArmor armor = targetItem as BaseArmor;

            if (weapon != null)
            {
                if (weapon.TrainingWeapon)
                {
                    m_Player.SendMessage("Training weapons may not be improved.");
                    return;
                }

                if (weapon.DurabilityLevel != WeaponDurabilityLevel.Regular || weapon.AccuracyLevel != WeaponAccuracyLevel.Regular || weapon.DamageLevel != WeaponDamageLevel.Regular)
                {
                    m_Player.SendMessage("Magical weapons may not be improved.");
                    return;
                }
            }

            if (armor != null)
            {

                if (armor.DurabilityLevel != ArmorDurabilityLevel.Regular || armor.ProtectionLevel != ArmorProtectionLevel.Regular)
                {
                    m_Player.SendMessage("Magical weapons may not be improved.");
                    return;
                }
            }

            //Aspect Item
            if (targetItem.Aspect != AspectEnum.None && targetItem.TierLevel >= 1 && targetItem.TierLevel <= 10)
            {
                if (targetItem.TierLevel == 10)
                {
                    m_Player.SendMessage("That item is already at it's maximum tier level.");
                    return;
                }
            }

            else if (targetItem.Aspect == AspectEnum.None)
            {
                if (!(targetItem.Quality == Quality.Exceptional && targetItem.DisplayCrafter))
                {
                    m_Player.SendMessage("Only mastermarked items may be improved.");
                    return;
                }
            }

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Effect Info
                case 2:
                    closeGump = false;
                break;

                //Previous Aspect
                case 3:
                    if (m_Item.TierLevel == 0)
                    {
                        int aspectCount = Enum.GetNames(typeof(AspectEnum)).Length;
                        int aspectIndex = (int)m_SelectedAspect - 1;
                        
                        if (aspectIndex <= 0)
                            aspectIndex = aspectCount - 1;

                        m_SelectedAspect = (AspectEnum)aspectIndex;

                        m_Player.SendSound(0x057);

                        closeGump = false;
                    }
                break;

                //Next Aspect
                case 4:
                    if (m_Item.TierLevel == 0)
                    {
                        int aspectCount = Enum.GetNames(typeof(AspectEnum)).Length;
                        int aspectIndex = (int)m_SelectedAspect + 1;

                        if (aspectIndex >= aspectCount)
                            aspectIndex = 1;

                        m_SelectedAspect = (AspectEnum)aspectIndex;

                        m_Player.SendSound(0x057);

                        closeGump = false;
                    }
                break;

                //Create or Upgrade
                case 5:
                    bool isValid = true;

                    if (weapon != null)
                    {
                        if (m_AspectMould.Charges < AspectMould.ChargesNeededForWeapon)
                        {
                            m_Player.SendMessage("Improving a weapon requires a mould with at least " + AspectMould.ChargesNeededForWeapon.ToString() + " uses remaining.");
                            
                            closeGump = false;
                            isValid = false;
                        }
                    }

                    if (isValid && armor != null)
                    {
                        if (m_AspectMould.Charges < AspectMould.ChargesNeededForArmor)
                        {
                            m_Player.SendMessage("Improving armor requires a mould with at least " + AspectMould.ChargesNeededForWeapon.ToString() + " uses remaining.");
                            
                            closeGump = false;
                            isValid = false;
                        }
                    }

                    if (isValid && targetItem.Aspect != AspectEnum.None && targetItem.TierLevel >= 1 && targetItem.TierLevel <= 10)
                    {
                        if (targetItem.TierLevel >= m_AspectMould.TierLevel)
                        {
                            m_Player.SendMessage("You will need a mould of at least tier " + (targetItem.TierLevel + 1).ToString() + " or higher to upgrade this item's tier further.");
                           
                            closeGump = false;
                            isValid = false;
                        }
                    }

                    if (isValid && targetItem.TierLevel >= 1 && targetItem.Experience < AspectGear.ExperienceNeededToUpgrade)
                    {
                        m_Player.SendMessage("That Aspect item has not accumulated enough experience neccessary to upgrade it's tier.");

                        closeGump = false;
                        isValid = false;
                    }

                    //Materials Required
                    string aspectName = AspectGear.GetAspectName(m_SelectedAspect);

                    int distillationNeeded = 0;
                    int coresNeeded = 0;

                    if (targetItem.TierLevel == 0)
                    {
                        if (weapon != null)
                        {
                            distillationNeeded = AspectGear.DistillationNeededForWeaponCreation;
                            coresNeeded = AspectGear.CoresNeededForWeaponCreation;
                        }

                        if (armor != null)
                        {
                            distillationNeeded = AspectGear.DistillationNeededForArmorCreation;
                            coresNeeded = AspectGear.CoresNeededForArmorCreation;
                        }

                    }

                    else
                    {
                        if (weapon != null)
                        {
                            distillationNeeded = AspectGear.DistillationNeededForWeaponUpgrade;
                            coresNeeded = AspectGear.CoresNeededForWeaponUpgrade;
                        }

                        if (armor != null)
                        {
                            distillationNeeded = AspectGear.DistillationNeededForArmorUpgrade;
                            coresNeeded = AspectGear.CoresNeededForArmorUpgrade;
                        }
                    }

                    if (isValid)
                    {
                        List<AspectDistillation> m_DistillationHeld = m_Player.Backpack.FindItemsByType<AspectDistillation>();
                        List<AspectCore> m_AspectCoresHeld = m_Player.Backpack.FindItemsByType<AspectCore>();

                        List<AspectDistillation> m_MatchingDistillationHeld = new List<AspectDistillation>();
                        List<AspectCore> m_MatchingAspectCoresHeld = new List<AspectCore>();

                        int totalMatchingDistillation = 0;
                        int totalMatchingCores = 0;

                        foreach (AspectDistillation aspectDistillation in m_DistillationHeld)
                        {
                            if (aspectDistillation.Aspect == m_SelectedAspect)
                            {
                                totalMatchingDistillation += aspectDistillation.Amount;
                                m_MatchingDistillationHeld.Add(aspectDistillation);
                            }
                        }

                        foreach (AspectCore aspectCore in m_AspectCoresHeld)
                        {
                            if (aspectCore.Aspect == m_SelectedAspect)
                            {
                                totalMatchingCores += aspectCore.Amount;
                                m_MatchingAspectCoresHeld.Add(aspectCore);
                            }
                        }

                        bool hasRequiredMaterials = true;

                        if (distillationNeeded > totalMatchingDistillation)
                        {
                            hasRequiredMaterials = false;
                            m_Player.SendMessage(distillationNeeded.ToString() + " " + aspectName + " Distillation is required to proceed.");

                            closeGump = false;
                        }

                        else if (coresNeeded > totalMatchingCores)
                        {
                            hasRequiredMaterials = false;
                            m_Player.SendMessage(coresNeeded.ToString() + " " + aspectName + " Cores are required to proceed.");

                            closeGump = false;
                        }

                        else
                        {
                            Queue m_Queue = new Queue();

                            foreach (AspectDistillation aspectDistillation in m_MatchingDistillationHeld)
                            {
                                if (aspectDistillation.Amount > distillationNeeded)
                                {
                                    aspectDistillation.Amount -= distillationNeeded;
                                    distillationNeeded = 0;
                                }

                                else
                                {
                                    distillationNeeded -= aspectDistillation.Amount;
                                    m_Queue.Enqueue(aspectDistillation);
                                }

                                if (distillationNeeded <= 0)
                                    break;
                            }

                            while (m_Queue.Count > 0)
                            {
                                AspectDistillation aspectDistillation = (AspectDistillation)m_Queue.Dequeue();
                                aspectDistillation.Delete();
                            }

                            m_Queue = new Queue();

                            foreach (AspectCore aspectCore in m_MatchingAspectCoresHeld)
                            {
                                if (aspectCore.Amount > coresNeeded)
                                {
                                    aspectCore.Amount -= coresNeeded;
                                    coresNeeded = 0;
                                }

                                else
                                {
                                    coresNeeded -= aspectCore.Amount;
                                    m_Queue.Enqueue(aspectCore);
                                }

                                if (coresNeeded <= 0)
                                    break;
                            }

                            while (m_Queue.Count > 0)
                            {
                                AspectCore aspectCore = (AspectCore)m_Queue.Dequeue();
                                aspectCore.Delete();
                            }

                            if (m_AspectMould != null)
                            {
                                if (weapon != null)                                
                                    m_AspectMould.Charges -= AspectMould.ChargesNeededForWeapon;

                                else if (armor != null)
                                    m_AspectMould.Charges -= AspectMould.ChargesNeededForArmor;

                                if (m_AspectMould.Charges <= 0)
                                    m_AspectMould.Delete();
                            }

                            //Creation
                            if (targetItem.TierLevel == 0)
                            {
                                targetItem.Quality = Quality.Regular;
                                targetItem.DisplayCrafter = false;
                                targetItem.ArcaneCharges = AspectGear.AspectStartingArcaneCharges;
                                targetItem.ArcaneChargesMax = AspectGear.ArcaneMaxCharges;
                                targetItem.TierLevel = 1;
                                targetItem.Aspect = m_SelectedAspect; 

                                if (weapon != null)
                                {
                                    m_Player.SendMessage("You create a new Aspect weapon.");

                                    weapon.MaxHitPoints = AspectGear.BaselineDurability + (AspectGear.IncreasedDurabilityPerTier * targetItem.TierLevel);
                                    weapon.HitPoints = weapon.MaxHitPoints;
                                }

                                if (armor != null)
                                {
                                    m_Player.SendMessage("You create a new Aspect armor.");

                                    armor.MaxHitPoints = AspectGear.BaselineDurability + (AspectGear.IncreasedDurabilityPerTier * targetItem.TierLevel);
                                    armor.HitPoints = armor.MaxHitPoints;
                                }                                
                            }

                            //Upgrade
                            else
                            {
                                targetItem.TierLevel++;

                                if (weapon != null)
                                {
                                    m_Player.SendMessage("You upgrade your Aspect weapon's tier.");

                                    weapon.MaxHitPoints = AspectGear.BaselineDurability + (AspectGear.IncreasedDurabilityPerTier * targetItem.TierLevel);
                                    weapon.HitPoints = weapon.MaxHitPoints;
                                }

                                if (armor != null)
                                {
                                    m_Player.SendMessage("You upgrade your Aspect armor's tier.");

                                    armor.MaxHitPoints = AspectGear.BaselineDurability + (AspectGear.IncreasedDurabilityPerTier * targetItem.TierLevel);
                                    armor.HitPoints = armor.MaxHitPoints;
                                }                              
                            }

                            targetItem.Experience = 0;

                            m_Player.PlaySound(0x64E);

                            return;
                        }
                    }                    
                break;
            }

            if (closeGump)            
                m_Player.SendSound(0x058);

            else
            {
                m_Player.CloseGump(typeof(AspectGearGump));
                m_Player.SendGump(new AspectGearGump(m_Player, m_AspectMould, m_Item, m_SelectedAspect));
            }                         
        }
    }
}