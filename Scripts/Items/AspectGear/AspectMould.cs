﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using System.Globalization;
using Server.Engines.Craft;

namespace Server.Items
{
    public class AspectMould : Item
    {
        public enum MouldSkillType
        {
            Blacksmithy,            
            Carpentry,
            Tailoring
        }
        
        private MouldSkillType m_MouldType = MouldSkillType.Blacksmithy;
        [CommandProperty(AccessLevel.GameMaster)]
        public MouldSkillType MouldType
        {
            get { return m_MouldType; }
            set
            {
                m_MouldType = value;

                switch (m_MouldType)
                {
                    case MouldSkillType.Blacksmithy: Hue = 2500; break;
                    case MouldSkillType.Carpentry: Hue = 2311; break;
                    case MouldSkillType.Tailoring: Hue = 2498; break;
                }
            }
        }

        private int m_Charges = 6;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        public static int MaxCharges = 6;

        public static int ChargesNeededForWeapon = 6;
        public static int ChargesNeededForArmor = 1;

        [Constructable]
        public AspectMould(): base(4323)
        {
            Name = "aspect mould";

            RandomizeCraftType();
            TierLevel = 1;
        }

        [Constructable]
        public AspectMould(MouldSkillType mouldType): base(0x14F0)
        {
            Name = "aspect mould";

            m_MouldType = mouldType;
            TierLevel = 1;
        }

        [Constructable]
        public AspectMould(int tierLevel): base(0x14F0)
        {
            Name = "aspect mould";

            RandomizeCraftType();
            TierLevel = tierLevel;

            if (TierLevel > 10)
                TierLevel = 10;

            if (TierLevel < 1)
                TierLevel = 1;
        }

        [Constructable]
        public AspectMould(MouldSkillType mouldType, int tierLevel): base(0x14F0)
        {
            Name = "aspect mould";

            m_MouldType = mouldType;
            TierLevel = tierLevel;

            if (TierLevel > 10)
                TierLevel = 10;

            if (TierLevel < 1)
                TierLevel = 1;
        }

        public AspectMould(Serial serial): base(serial)
        {
        }

        public void RandomizeCraftType()
        {
            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: m_MouldType = MouldSkillType.Blacksmithy; break;
                case 2: m_MouldType = MouldSkillType.Carpentry; break;
                case 3: m_MouldType = MouldSkillType.Tailoring; break;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, GetDisplayName());
            LabelTo(from, "[Uses Remaining: " + m_Charges.ToString() + "/" + MaxCharges + "]");           
        }

        public string GetDisplayName()
        {
            switch (m_MouldType)
            {
                case MouldSkillType.Blacksmithy: return "Smithing Mould: Tier " + TierLevel.ToString(); break;
                case MouldSkillType.Carpentry: return "Carpentry Mould: Tier " + TierLevel.ToString(); break;
                case MouldSkillType.Tailoring: return "Tailoring Mould: Tier " + TierLevel.ToString(); break;
            }

            return "Aspect Mould";
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The mould you wish to use must be in your pack in order to use it.");
                return;
            }

            switch (m_MouldType)
            {
                case MouldSkillType.Blacksmithy:
                    from.SendMessage("Target a master-marked blacksmith-based weapon or armor to create a new Aspect item. Or target an existing Aspect item to upgrade it's tier.");
                break;
                    
                case MouldSkillType.Carpentry:
                    from.SendMessage("Target a master-marked carpenty-based weapon or armor to create a new Aspect item. Or target an existing Aspect item to upgrade it's tier.");
                break;

                case MouldSkillType.Tailoring:
                    from.SendMessage("Target a master-marked tailoring-based armor to create a new Aspect item. Or target an existing Aspect item to upgrade it's tier.");
                break;
            }                 
            
            from.Target = new AspectTarget(this);
        }

        public class AspectTarget : Target
        {
            public AspectMould m_AspectMould;
            public PlayerMobile m_Player;

            public AspectTarget(AspectMould aspectMould): base(18, false, TargetFlags.None)
            {
                m_AspectMould = aspectMould;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                m_Player = from as PlayerMobile;

                if (m_Player == null)
                    return;

                if (m_AspectMould == null) return;
                if (m_AspectMould.Deleted) return;

                if (!m_AspectMould.IsChildOf(m_Player.Backpack))
                {
                    from.SendMessage("The mould you wish to use must be in your backpack in order to use it.");
                    return;
                }

                Item targetItem = target as Item;

                if (targetItem == null)
                {
                    from.SendMessage("You cannot improve that.");
                    return;
                }

                if (!(targetItem is BaseWeapon || targetItem is BaseArmor))
                {
                    from.SendMessage("You may only improve weapons and armor.");
                    return;
                }

                if (targetItem is BaseShield)
                {
                    from.SendMessage("You may only improve weapons and armor.");
                    return;
                }

                if (!(targetItem.IsChildOf(m_Player.Backpack) || targetItem.RootParent != m_Player))
                {
                    from.SendMessage("The item you wish to improve must be equipped or in your backpack.");
                    return;
                }

                Type itemType = targetItem.GetType();
                CraftItem craftItem = null;

                switch (m_AspectMould.MouldType)
                {
                    case MouldSkillType.Blacksmithy:
                        craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor(itemType);

                        if (craftItem == null)
                        {
                            m_Player.SendMessage("You may only improve items craftable through blacksmithing with this mould.");
                            return;
                        }
                    break;

                    case MouldSkillType.Carpentry:
                        craftItem = DefCarpentry.CraftSystem.CraftItems.SearchFor(itemType);

                        if (craftItem == null)
                        {
                            m_Player.SendMessage("You may only improve items craftable through carpentry with this mould.");
                            return;
                        }
                    break;

                    case MouldSkillType.Tailoring:
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

                    m_Player.SendSound(0x055);

                    m_Player.CloseGump(typeof(AspectGearGump));
                    m_Player.SendGump(new AspectGearGump(m_Player, m_AspectMould, targetItem, AspectEnum.Air));

                    return;
                }

                else if (targetItem.Aspect == AspectEnum.None)
                {
                    if (!(targetItem.Quality == Quality.Exceptional && targetItem.DisplayCrafter))
                    {
                        m_Player.SendMessage("Only master-marked items may be improved.");
                        return;
                    }

                    m_Player.SendSound(0x055);

                    m_Player.CloseGump(typeof(AspectGearGump));
                    m_Player.SendGump(new AspectGearGump(m_Player, m_AspectMould, targetItem, AspectEnum.Air));

                    return;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
    
            //Version 0
            writer.Write((int)m_MouldType);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_MouldType = (MouldSkillType)reader.ReadInt();
                m_Charges = reader.ReadInt();
            }
        }
    }
}

/*
        public static bool HasRequiredMaterials(Mobile from, Mobile mobileTarget, DungeonMould dungeonMould, bool newWeapon, DungeonEnum dungeon)
        {
            if (from == null) return false;
            if (from.Backpack == null) return false;
            if (mobileTarget == null) return false;
            if (mobileTarget.Backpack == null) return false;

            if (dungeonMould == null) return false;
            if (dungeonMould.Deleted) return false;

            if (!dungeonMould.IsChildOf(from.Backpack))            
                return false;

            List<DungeonCore> m_DungeonCores = mobileTarget.Backpack.FindItemsByType<DungeonCore>();
            List<DungeonDistillation> m_DungeonDistillations = mobileTarget.Backpack.FindItemsByType<DungeonDistillation>();

            List<DungeonCore> m_MatchingDungeonCores = new List<DungeonCore>();
            List<DungeonDistillation> m_MatchingDungeonDistillations = new List<DungeonDistillation>();

            int totalMatchingDungeonCores = 0;

            foreach (DungeonCore dungeonCore in m_DungeonCores)
            {
                if (dungeonCore.Dungeon == dungeon)
                {
                    totalMatchingDungeonCores += dungeonCore.Amount;
                    m_MatchingDungeonCores.Add(dungeonCore);
                }
            }

            foreach (DungeonDistillation dungeonDistillation in m_DungeonDistillations)
            {
                if (dungeonDistillation.Dungeon == dungeon)
                    m_MatchingDungeonDistillations.Add(dungeonDistillation);
            }

            if (newWeapon)
            {
                if (totalMatchingDungeonCores >= DungeonWeapon.CoresNeededForCreation && m_MatchingDungeonDistillations.Count >= DungeonWeapon.DistillationNeededForCreation)
                    return true;
            }

            else
            {
                if (totalMatchingDungeonCores >= DungeonWeapon.CoresNeededForUpgrade && m_MatchingDungeonDistillations.Count >= DungeonWeapon.DistillationNeededForUpgrade)
                    return true;
            }

            return false;
        }

        public static void ConsumeMaterials(Mobile from, Mobile mobileTarget, DungeonMould dungeonMould, bool newWeapon, DungeonEnum dungeon)
        {
            if (from == null || mobileTarget == null)
                return;

            List<DungeonCore> m_DungeonCores = mobileTarget.Backpack.FindItemsByType<DungeonCore>();
            List<DungeonDistillation> m_DungeonDistillations = mobileTarget.Backpack.FindItemsByType<DungeonDistillation>();

            List<DungeonCore> m_MatchingDungeonCores = new List<DungeonCore>();
            List<DungeonDistillation> m_MatchingDungeonDistillations = new List<DungeonDistillation>();

            int coresNeeded = DungeonWeapon.CoresNeededForUpgrade;
            int distillationsNeeded = DungeonWeapon.DistillationNeededForUpgrade;

            if (newWeapon)
            {
                coresNeeded = DungeonWeapon.CoresNeededForCreation;
                distillationsNeeded = DungeonWeapon.DistillationNeededForCreation;
            }

            Queue m_Queue = new Queue();    
                    
            foreach (DungeonCore dungeonCore in m_DungeonCores)
            {
                if (dungeonCore.Dungeon == dungeon)
                {
                    if (dungeonCore.Amount > coresNeeded)
                    {
                        dungeonCore.Amount -= coresNeeded;
                        coresNeeded = 0;
                    }

                    else
                    {
                        //Queue for Deletion
                        coresNeeded -= dungeonCore.Amount;

                        m_Queue.Enqueue(dungeonCore);   
                    }           
                }

                if (coresNeeded <= 0)
                    break;
            }

            while (m_Queue.Count > 0)
            {
                DungeonCore dungeonCore = (DungeonCore)m_Queue.Dequeue();
                dungeonCore.Delete();
            }            

            m_Queue = new Queue();

            foreach (DungeonDistillation dungeonDistillation in m_DungeonDistillations)
            {
                if (dungeonDistillation.Dungeon == dungeon)
                {
                    m_Queue.Enqueue(dungeonDistillation);
                    distillationsNeeded--;
                }

                if (distillationsNeeded <= 0)
                    break;
            }

            while (m_Queue.Count > 0)
            {
                DungeonDistillation dungeonDistillation = (DungeonDistillation)m_Queue.Dequeue();
                dungeonDistillation.Delete();
            }

            if (dungeonMould != null)
                dungeonMould.Delete();
        }

        public static BaseWeapon CreateDungeonWeapon(BaseWeapon weapon, DungeonEnum dungeon)
        {
            weapon.Quality = Quality.Regular;
            weapon.ArcaneCharges = DungeonWeapon.ArcaneMaxCharges;
            weapon.ArcaneChargesMax = DungeonWeapon.ArcaneMaxCharges;
            weapon.Dungeon = dungeon;
            weapon.TierLevel = 1;
            weapon.Experience = 0;

            return weapon;
        }
        */

/*
public class DungeonMouldWeaponGump : Gump
{
    public Mobile m_GumpTarget;
    public Mobile m_Crafter;
    public Mobile m_MobileTarget;
    public DungeonMould m_DungeonMould;
    public bool m_NewWeapon;
    public BaseWeapon m_Weapon;
    public DungeonEnum m_Dungeon;
    public int m_AmountDemanded;
    public bool m_Confirmed;

    public DungeonMouldWeaponGump(Mobile gumpTarget, Mobile crafter, Mobile mobileTarget, DungeonMould mould, bool newWeapon, BaseWeapon weapon, DungeonEnum dungeon, int amountDemanded, bool confirmed): base(10, 10)
    {
        m_GumpTarget = gumpTarget;
        m_Crafter = crafter;
        m_MobileTarget = mobileTarget;
        m_DungeonMould = mould;
        m_NewWeapon = newWeapon;
        m_Weapon = weapon;
        m_Dungeon = dungeon;

        m_AmountDemanded = amountDemanded;
        m_Confirmed = confirmed;

        bool selfCraft = (m_Crafter == m_MobileTarget);

        Closable = true;
        Disposable = true;
        Dragable = true;
        Resizable = false;

        AddImage(70, 100, 103);
        AddImage(5, 100, 103);
        AddImage(70, 5, 103);
        AddImage(5, 5, 103);
        AddImage(200, 100, 103);
        AddImage(200, 5, 103);
        AddImage(70, 281, 103);
        AddImage(5, 281, 103);
        AddImage(70, 186, 103);
        AddImage(5, 186, 103);
        AddImage(200, 281, 103);
        AddImage(200, 186, 103);
        AddImage(70, 374, 103);
        AddImage(5, 374, 103);
        AddImage(200, 374, 103);
        AddImage(17, 18, 3604, 2052);
        AddImage(76, 18, 3604, 2052);
        AddImage(204, 18, 3604, 2052);
        AddImage(17, 140, 3604, 2052);
        AddImage(76, 140, 3604, 2052);
        AddImage(204, 140, 3604, 2052);
        AddImage(17, 262, 3604, 2052);
        AddImage(76, 262, 3604, 2052);
        AddImage(204, 262, 3604, 2052);
        AddImage(17, 335, 3604, 2052);
        AddImage(76, 335, 3604, 2052);
        AddImage(204, 335, 3604, 2052);			   
			    
        //-----
        int WhiteTextHue = 2655;
        int EffectHue = WhiteTextHue;

        string offerText = "";

        if (!selfCraft)
        {
            if (m_GumpTarget == m_MobileTarget)
            {
                offerText = "Offer from " + m_Crafter.Name;
                AddLabel(Utility.CenteredTextOffset(175, offerText), 17, 149, offerText);
            }
        }

        if (m_NewWeapon)
        {
            offerText = "Create Dungeon Weapon";

            if (selfCraft || m_GumpTarget == m_Crafter)
                AddLabel(Utility.CenteredTextOffset(175, offerText), 17, WhiteTextHue, offerText);
            else
                AddLabel(Utility.CenteredTextOffset(175, offerText), 37, WhiteTextHue, offerText);
        }

        else
        {
            offerText = "Upgrade Dungeon Weapon";

            if (selfCraft || m_GumpTarget == m_Crafter)
                AddLabel(Utility.CenteredTextOffset(175, offerText), 17, WhiteTextHue, offerText);
            else
                AddLabel(Utility.CenteredTextOffset(175, offerText), 37, WhiteTextHue, offerText);
        }

        //Weapon Info
        int weaponTier = m_Weapon.TierLevel;
        string dungeonName = GetDungeonName(m_Dungeon) + " Dungeon";
        string weaponName = m_Weapon.Name;

        if (weaponName == null)
            weaponName = "";

        weaponName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(weaponName);
        int newDurability = DungeonWeapon.BaselineDurability + (DungeonWeapon.IncreasedDurabilityPerTier * weaponTier);

        int adjustedBlessedCharges = DungeonWeapon.ArcaneMaxCharges;

        double accuracy = 100 * (DungeonWeapon.BaseAccuracy + (DungeonWeapon.AccuracyPerTier * (double)weaponTier));
        double tactics = DungeonWeapon.BaseTactics + (DungeonWeapon.TacticsPerTier * (double)weaponTier);
                
        double effectChance = DungeonWeapon.BaseEffectChance + (DungeonWeapon.BaseEffectChancePerTier * (double)weaponTier);

        effectChance *= DungeonWeapon.GetSpeedScalar(m_Weapon.Speed);

        DungeonArmor.DungeonArmorDetail detail = new DungeonArmor.DungeonArmorDetail(m_Dungeon, 1);

        EffectHue = detail.Hue - 1;

        int itemId = weapon.ItemID;
        int itemHue = EffectHue;
        int offsetX = 0;
        int offsetY = 0;

        AddLabel(Utility.CenteredTextOffset(120, dungeonName) , 60, EffectHue, dungeonName);
        AddLabel(Utility.CenteredTextOffset(120, weaponName), 80, EffectHue, weaponName);
        AddItem(103 + m_Weapon.IconOffsetX, 115 + m_Weapon.IconOffsetY, m_Weapon.IconItemId, itemHue);
        AddLabel(97, 165, EffectHue, "Tier " + weaponTier.ToString());                

        AddLabel(71, 190, WhiteTextHue, "Charges:");
        AddLabel(135, 190, EffectHue, adjustedBlessedCharges.ToString());

        AddLabel(55, 210, WhiteTextHue, "Experience:");
        AddLabel(135, 210, EffectHue, "0/" + DungeonWeapon.MaxDungeonExperience.ToString());

        AddLabel(60, 230, WhiteTextHue, "Durability:");
        AddLabel(135, 230, EffectHue, newDurability.ToString() + "/" + newDurability.ToString());
			    
        AddLabel(65, 250, WhiteTextHue, "Accuracy:");
        AddLabel(135, 250, EffectHue, "+" + accuracy.ToString() + "%");

        AddLabel(74, 270, WhiteTextHue, "Tactics:");
        AddLabel(135, 270, EffectHue, "+" + tactics.ToString());                

        AddLabel(31, 290, WhiteTextHue, "Effect Chance:");
        AddLabel(135, 290, EffectHue, Utility.CreateDecimalPercentageString(effectChance, 1));	
        AddLabel(30, 310, WhiteTextHue, "(scaled for weapon speed)");

        DungeonWeaponDetail weaponDetail = DungeonWeapon.GetDungeonWeaponDetail(m_Dungeon);

        AddLabel(70, 335, WhiteTextHue, "Special Effect:");
        AddButton(78, 359, 1210, 1209, 4, GumpButtonType.Reply, 0);
        AddLabel(98, 355, EffectHue, weaponDetail.m_EffectDisplayName);

        //-----

        if (newWeapon)
        {
            if (selfCraft || m_GumpTarget == m_MobileTarget)
            {
                AddLabel(225, 60, WhiteTextHue, "Choose Dungeon");

                AddLabel(265, 95, WhiteTextHue, "Covetous");
                AddLabel(265, 130, WhiteTextHue, "Deceit");
                AddLabel(265, 165, WhiteTextHue, "Despise");
                AddLabel(265, 200, WhiteTextHue, "Destard");
                AddLabel(265, 235, WhiteTextHue, "Fire");
                AddLabel(265, 270, WhiteTextHue, "Hythloth");
                AddLabel(265, 305, WhiteTextHue, "Ice");
                AddLabel(265, 340, WhiteTextHue, "Shame");
                AddLabel(265, 375, WhiteTextHue, "Wrong");

                if (m_Dungeon == DungeonEnum.Covetous)
                    AddButton(228, 90, 9724, 9721, 10, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 90, 9721, 9724, 10, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Deceit)
                    AddButton(228, 125, 9724, 9721, 11, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 125, 9721, 9724, 11, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Despise)
                    AddButton(228, 160, 9724, 9721, 12, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 160, 9721, 9724, 12, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Destard)
                    AddButton(228, 195, 9724, 9721, 13, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 195, 9721, 9724, 13, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Fire)
                    AddButton(228, 230, 9724, 9721, 14, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 230, 9721, 9724, 14, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Hythloth)
                    AddButton(228, 265, 9724, 9721, 15, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 265, 9721, 9724, 15, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Ice)
                    AddButton(228, 300, 9724, 9721, 16, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 300, 9721, 9724, 16, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Shame)
                    AddButton(228, 335, 9724, 9721, 17, GumpButtonType.Reply, 0);
                else
                    AddButton(228, 335, 9721, 9724, 17, GumpButtonType.Reply, 0);

                if (m_Dungeon == DungeonEnum.Wrong)
                    AddButton(228, 370, 9724, 9721, 18, GumpButtonType.Reply, 0); 
                else
                    AddButton(228, 370, 9721, 9724, 18, GumpButtonType.Reply, 0); 
            }
        }

        if (!selfCraft)
        {
            if (m_GumpTarget == m_Crafter)
            {
                AddLabel(26, 388, 149, "Price");
                AddItem(57, 386, 3823);
                AddImage(103, 387, 2445);
                AddTextEntry(108, 388, 95, 20, WhiteTextHue, 4, m_AmountDemanded.ToString());
                AddButton(214, 392, 1209, 1210, 3, GumpButtonType.Reply, 0);                 
            }

            else
            {
                AddLabel(26, 388, 149, "Price");
                AddItem(57, 386, 3823);
                AddLabel(103, 388, WhiteTextHue, m_AmountDemanded.ToString());
            }
        }

        if (selfCraft)
        {                    
            AddButton(24, 423, 2151, 2154, 1, GumpButtonType.Reply, 0);
            AddLabel(57, 426, WhiteTextHue, "Confirm");
        }

        else
        {
            if (m_GumpTarget == m_Crafter)
            {
                if (m_Confirmed)
                    AddButton(191, 423, 2154, 2151, 2, GumpButtonType.Reply, 0);
                else
                    AddButton(191, 423, 2151, 2154, 2, GumpButtonType.Reply, 0);

                AddLabel(227, 427, WhiteTextHue, "Confirm Offer");
            }

            else
            {
                AddButton(24, 423, 2151, 2154, 1, GumpButtonType.Reply, 0);
                AddLabel(57, 426, WhiteTextHue, "Accept Their Offer");

                if (m_Confirmed)
                    AddImage(191, 423, 9729);
                else
                    AddImage(191, 423, 9731);

                AddLabel(227, 427, WhiteTextHue, "They Are Ready");	
            }                    
        }                		   
    }

    public override void OnResponse(NetState sender, RelayInfo info)
    {
        #region Validate Mobiles Involved

        //Validate Mobiles
        bool crafterValid = true;
        bool mobileTargetValid = true;

        if (m_Crafter == null)
            crafterValid = false;

        else if (m_Crafter.Deleted || !m_Crafter.Alive)
            crafterValid = false;

        if (m_MobileTarget == null)
            mobileTargetValid = false;

        else if (m_MobileTarget.Deleted || !m_MobileTarget.Alive)
            mobileTargetValid = false;

        if (!crafterValid || !mobileTargetValid)
        {
            if (m_Crafter != null)
            {
                if (m_Crafter.HasGump(typeof(DungeonMouldWeaponGump)))
                    m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));

                m_Crafter.SendMessage("You can no longer proceed with the crafting of that dungeon weapon.");
            }

            if (m_MobileTarget != null && m_Crafter != m_MobileTarget)
            {
                if (m_MobileTarget.HasGump(typeof(DungeonMouldWeaponGump)))
                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));

                m_MobileTarget.SendMessage("You can no longer proceed with the crafting of that dungeon weapon.");
            }

            return;
        }

        if (crafterValid && mobileTargetValid && m_Crafter != m_MobileTarget)
        {
            if (Utility.GetDistance(m_Crafter.Location, m_MobileTarget.Location) > 2)
            {
                if (m_Crafter.HasGump(typeof(DungeonMouldWeaponGump)))
                    m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));

                m_Crafter.SendMessage("You are too far away from the target to proceed with the crafting.");

                if (m_MobileTarget.HasGump(typeof(DungeonMouldWeaponGump)))
                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));

                m_MobileTarget.SendMessage("You are too far away from the crafter to proceed with the crafting.");

                return;
            }

            if (!m_Crafter.InLOS(m_MobileTarget))
            {
                if (m_Crafter.HasGump(typeof(DungeonMouldWeaponGump)))
                    m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));

                m_Crafter.SendMessage("You are out of sight of the target and cannot continue crafting.");

                if (m_MobileTarget.HasGump(typeof(DungeonMouldWeaponGump)))
                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));

                m_MobileTarget.SendMessage("You are out of sight of the crafter and cannot continue crafting.");

                return;
            }
        }

        #endregion

        bool selfCraft = (m_Crafter == m_MobileTarget);
        bool attemptCraft = false;

        //Close Gumps For Other Player if One Party Closes Theirs
        if (info.ButtonID == 0)
        {
            if (!selfCraft)
            {
                if (m_GumpTarget == m_Crafter)
                {
                    if (m_MobileTarget.HasGump(typeof(DungeonMouldWeaponGump)))
                    {

                        m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                        m_MobileTarget.SendMessage("They have canceled the offer.");

                        return;
                    }                            
                }
                        
                if (m_GumpTarget == m_MobileTarget)
                {
                    if (m_Crafter.HasGump(typeof(DungeonMouldWeaponGump)))
                    {

                        m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));
                        m_Crafter.SendMessage("They have declined the offer.");

                        return;
                    }
                }

            }
        }

        //Confirm / Accept
        if (info.ButtonID == 1)
        {
            if (selfCraft)
                attemptCraft = true;

            else if (m_GumpTarget == m_MobileTarget)
            {
                if (!m_Confirmed)
                {
                    if (m_Crafter != null)
                        m_MobileTarget.SendMessage(m_Crafter.Name + " has not confirmed their offer price yet.");

                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                    m_MobileTarget.SendGump(new DungeonMouldWeaponGump(m_MobileTarget, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

                    return;
                }

                else
                    attemptCraft = true;
            }

            else
            {
                m_GumpTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                m_GumpTarget.SendGump(new DungeonMouldWeaponGump(m_Crafter, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

                return;
            }
        }

        //Ready
        if (info.ButtonID == 2)
        {
            if (!selfCraft && m_GumpTarget == m_Crafter)
            {
                m_Confirmed = !m_Confirmed;                        

                m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                m_MobileTarget.SendGump(new DungeonMouldWeaponGump(m_MobileTarget, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));                                              
            }

            m_GumpTarget.CloseGump(typeof(DungeonMouldWeaponGump));
            m_GumpTarget.SendGump(new DungeonMouldWeaponGump(m_Crafter, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

            return;
        }

        //Adjust Price
        if (info.ButtonID == 3)
        {
            if (!selfCraft && m_GumpTarget == m_Crafter)
            {
                //Set Gold Amount
                TextRelay demandAmountTextRelay = info.GetTextEntry(4);
                string demandAmountText = demandAmountTextRelay.Text.Trim();

                try { m_AmountDemanded = Convert.ToInt32(demandAmountText); }
                catch (Exception e) { m_AmountDemanded = 0; }

                if (m_AmountDemanded > 500000)
                    m_AmountDemanded = 500000;

                if (m_AmountDemanded < 0)
                    m_AmountDemanded = 0;

                m_Confirmed = false;

                m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                m_MobileTarget.SendGump(new DungeonMouldWeaponGump(m_MobileTarget, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

                m_MobileTarget.SendMessage("They have changed the asking price for their service.");
                m_GumpTarget.SendMessage("You change the asking price for your services. Click 'Confirm' to proceed.");
            }

            m_GumpTarget.CloseGump(typeof(DungeonMouldWeaponGump));
            m_GumpTarget.SendGump(new DungeonMouldWeaponGump(m_Crafter, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));
                            
            return;
        }

        //Special Effect
        if (info.ButtonID == 4)
        {
            DungeonWeaponDetail weaponDetail = DungeonWeapon.GetDungeonWeaponDetail(m_Dungeon);

            m_GumpTarget.SendMessage(weaponDetail.m_EffectDisplayName + ": " + weaponDetail.m_EffectDescription);

            m_GumpTarget.CloseGump(typeof(DungeonMouldWeaponGump));
            m_GumpTarget.SendGump(new DungeonMouldWeaponGump(m_Crafter, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

            return;
        }

        //Dungeon Selection
        if (info.ButtonID >= 10 && info.ButtonID <= 18)
        {
            if (m_NewWeapon)
            {
                if (selfCraft || m_GumpTarget == m_MobileTarget)
                {
                    switch (info.ButtonID)
                    {
                        case 10: m_Dungeon = DungeonEnum.Covetous; break;
                        case 11: m_Dungeon = DungeonEnum.Deceit; break;
                        case 12: m_Dungeon = DungeonEnum.Despise; break;
                        case 13: m_Dungeon = DungeonEnum.Destard; break;
                        case 14: m_Dungeon = DungeonEnum.Fire; break;
                        case 15: m_Dungeon = DungeonEnum.Hythloth; break;
                        case 16: m_Dungeon = DungeonEnum.Ice; break;
                        case 17: m_Dungeon = DungeonEnum.Shame; break;
                        case 18: m_Dungeon = DungeonEnum.Wrong; break;
                    }
                }                      
            }
                   
            m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));
            m_Crafter.SendGump(new DungeonMouldWeaponGump(m_Crafter, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));
                    
            if (m_Crafter != m_MobileTarget)
            {
                m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                m_MobileTarget.SendGump(new DungeonMouldWeaponGump(m_MobileTarget, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));
            }

            return;
        }

        //Attempt Crafting
        if (attemptCraft)
        {
            bool mouldValid = true;

            if (m_DungeonMould == null)
                mouldValid = false;

            else if (m_DungeonMould.Deleted)
                mouldValid = false;

            if (m_DungeonMould.RootParent != m_Crafter)
                mouldValid = false;

            if (!mouldValid)
            {
                m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));
                m_Crafter.SendMessage("The dungeon mould being used is no longer accessible.");

                if (m_Crafter != m_MobileTarget)
                {
                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                    m_MobileTarget.SendMessage("The dungeon mould being used is no longer accessible.");
                }

                return;
            }

            bool weaponValid = true;

            if (m_Weapon == null)
                weaponValid = false;

            else if (m_Weapon.Deleted)
                weaponValid = false;

            if (selfCraft)
            {
                if (m_Weapon.RootParent != m_Crafter)
                    weaponValid = false;
            }

            else
            {
                if (m_Weapon.RootParent != m_MobileTarget)
                    weaponValid = false;
            }

            if (m_Weapon.TierLevel == DungeonWeapon.MaxDungeonTier)
                weaponValid = false;

            if (m_NewWeapon)
            {
                if (m_Weapon.TierLevel > 0)
                    weaponValid = false;

                if (m_Weapon.CrafterName == "" || m_Weapon.Quality != Quality.Exceptional)
                    weaponValid = false;

                if (m_Weapon.DamageLevel != WeaponDamageLevel.Regular || m_Weapon.AccuracyLevel != WeaponAccuracyLevel.Regular || m_Weapon.DurabilityLevel != WeaponDurabilityLevel.Regular)
                    weaponValid = false;
            }

            if (!weaponValid)
            {
                m_Crafter.CloseGump(typeof(DungeonMouldWeaponGump));
                m_Crafter.SendMessage("The weapon being targeted is no longer accessible.");

                if (m_Crafter != m_MobileTarget)
                {
                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                    m_MobileTarget.SendMessage("The weapon being targeted is no longer accessible.");
                }

                return;
            }

            if (!selfCraft && m_AmountDemanded > 0)
            {
                if (Banker.GetBalance(m_MobileTarget) < m_AmountDemanded)
                {
                    m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                    m_MobileTarget.SendGump(new DungeonMouldWeaponGump(m_MobileTarget, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

                    m_MobileTarget.SendMessage("You do not have the gold amount requested for this transaction in your bankbox.");

                    return;
                }
            }

            if (!HasRequiredMaterials(m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Dungeon))
            {
                m_MobileTarget.CloseGump(typeof(DungeonMouldWeaponGump));
                m_MobileTarget.SendGump(new DungeonMouldWeaponGump(m_MobileTarget, m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Weapon, m_Dungeon, m_AmountDemanded, m_Confirmed));

                if (m_NewWeapon)
                    m_MobileTarget.SendMessage("You must have " + DungeonWeapon.CoresNeededForCreation.ToString() + " dungeon cores and " + DungeonWeapon.DistillationNeededForCreation.ToString() + " dungeon distillation of the matching dungeon in your backpack to create this dungeon weapon.");

                else
                    m_MobileTarget.SendMessage("You must have " + DungeonWeapon.CoresNeededForUpgrade.ToString() + " dungeon cores and " + DungeonWeapon.DistillationNeededForUpgrade.ToString() + " dungeon distillation of the matching dungeon in your backpack to upgrade it's tier.");

                return;
            }

            ConsumeMaterials(m_Crafter, m_MobileTarget, m_DungeonMould, m_NewWeapon, m_Dungeon);

            if (!selfCraft && m_AmountDemanded > 0)
            {
                Banker.Withdraw(m_MobileTarget, m_AmountDemanded);
                Banker.Deposit(m_Crafter, m_AmountDemanded);

                m_MobileTarget.SendSound(0x037);

                if (m_NewWeapon)
                {
                    m_Crafter.SendMessage("You convert their weapon into a dungeon weapon and receive " + m_AmountDemanded.ToString() + " gold for your services.");
                    m_MobileTarget.SendMessage("You pay " + m_AmountDemanded.ToString() + " gold for them to create a dungeon weapon for you.");
                }

                else
                {
                    m_Crafter.SendMessage("You upgrade their dugneon weapon's tier and receive " + m_AmountDemanded.ToString() + " gold for your services.");
                    m_MobileTarget.SendMessage("You pay " + m_AmountDemanded.ToString() + " gold for them to upgrade your dungeon weapon.");
                }
            }

            else
            {
                if (selfCraft)
                {
                    if (m_NewWeapon)
                        m_Crafter.SendMessage("You convert your weapon into a dungeon weapon.");

                    else
                        m_Crafter.SendMessage("You upgrade your dungeon weapon's tier.");                            
                }

                else
                {
                    if (m_NewWeapon)
                    {
                        m_Crafter.SendMessage("You convert their weapon into a dungeon weapon.");
                        m_MobileTarget.SendMessage("They create a dungeon weapon for you.");
                    }

                    else
                    {
                        m_Crafter.SendMessage("You upgrade their dungeon weapon's tier.");
                        m_MobileTarget.SendMessage("They upgrade your dungeon weapon's tier for you.");

                    }
                }                         
            }

            m_Crafter.PlaySound(0x2A);

            if (m_NewWeapon)
            {
                m_Weapon.Quality = Quality.Regular;
                m_Weapon.ArcaneCharges = DungeonWeapon.ArcaneMaxCharges;
                m_Weapon.ArcaneChargesMax = DungeonWeapon.ArcaneMaxCharges;
                m_Weapon.Dungeon = m_Dungeon;
                m_Weapon.TierLevel = 1;                                       
            }

            else
                m_Weapon.TierLevel++;

            m_Weapon.Experience = 0;

            m_Weapon.MaxHitPoints = DungeonWeapon.BaselineDurability + (DungeonWeapon.IncreasedDurabilityPerTier * m_Weapon.TierLevel);
            m_Weapon.HitPoints = m_Weapon.MaxHitPoints;

            return;
        }
    }               
}        
*/