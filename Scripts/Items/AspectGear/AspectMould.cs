using System;
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
    public class AspectMould : Item, ICraftable
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
                    case MouldSkillType.Carpentry: Hue = 0; break;
                    case MouldSkillType.Tailoring: Hue = 1846; break;
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
            TierLevel = 2;
        }

        public AspectMould(Serial serial): base(serial)
        {
        }

        public void CheckMaxTierLevel()
        {
            if (TierLevel > AspectGear.MaxTierLevel)
                TierLevel = AspectGear.MaxTierLevel;

            if (TierLevel < 1)
                TierLevel = 1; 
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

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = Quality.Regular;

            double craftingSkill = 0;

            switch (MouldType)
            {
                case MouldSkillType.Blacksmithy: craftingSkill = from.Skills.Blacksmith.Value; break;
                case MouldSkillType.Carpentry: craftingSkill = from.Skills.Carpentry.Value; break;
                case MouldSkillType.Tailoring: craftingSkill = from.Skills.Tailoring.Value; break;
            }

            double skillBase = craftingSkill - 100;

            if (skillBase > 20)
                skillBase = 20;

            if (skillBase < 0)
                skillBase = 0;

            int tierLevel = (int)(Math.Ceiling(skillBase / 2));

            return quality;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "[Uses Remaining: " + m_Charges.ToString() + "/" + MaxCharges + "]");           
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
                    from.SendMessage("Target a mastermarked blacksmith-based weapon or armor to create a new Aspect item, or an existing Aspect item to upgrade it's tier.");
                break;
                    
                case MouldSkillType.Carpentry:
                    from.SendMessage("Target a mastermarked carpenty-based weapon or armor to create a new Aspect item, or an existing Aspect item to upgrade it's tier.");
                break;

                case MouldSkillType.Tailoring:
                    from.SendMessage("Target a mastermarked tailoring-based armor to create a new Aspect item, or an existing Aspect item to upgrade it's tier.");
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

                if (!(targetItem.IsChildOf(m_Player.Backpack) || targetItem.RootParent == m_Player))
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
                        m_Player.SendMessage("Only mastermarked items may be improved.");
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

    #region Smithing Moulds

    public class BasicSmithingMould : AspectMould
    {
        [Constructable]
        public BasicSmithingMould(): base()
        {
            Name = "Basic Smithing Mould";

            TierLevel = 2;
            MouldType = MouldSkillType.Blacksmithy;
        }

        public BasicSmithingMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class AdvancedSmithingMould : AspectMould
    {
        [Constructable]
        public AdvancedSmithingMould(): base()
        {
            Name = "Advanced Smithing Mould";

            TierLevel = 4;
            MouldType = MouldSkillType.Blacksmithy;
        }

        public AdvancedSmithingMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class ExpertSmithingMould : AspectMould
    {
        [Constructable]
        public ExpertSmithingMould(): base()
        {
            Name = "Expert Smithing Mould";

            TierLevel = 6;
            MouldType = MouldSkillType.Blacksmithy;
        }

        public ExpertSmithingMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class MasterSmithingMould : AspectMould
    {
        [Constructable]
        public MasterSmithingMould(): base()
        {
            Name = "Master Smithing Mould";

            TierLevel = 8;
            MouldType = MouldSkillType.Blacksmithy;
        }

        public MasterSmithingMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class LegendarySmithingMould : AspectMould
    {
        [Constructable]
        public LegendarySmithingMould(): base()
        {
            Name = "Legendary Smithing Mould";

            TierLevel = 10;
            MouldType = MouldSkillType.Blacksmithy;
        }

        public LegendarySmithingMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    #endregion

    #region Carpentry Moulds

    public class BasicCarpentryMould : AspectMould
    {
        [Constructable]
        public BasicCarpentryMould(): base()
        {
            Name = "Basic Carpentry Mould";

            TierLevel = 2;
            MouldType = MouldSkillType.Carpentry;
        }

        public BasicCarpentryMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class AdvancedCarpentryMould : AspectMould
    {
        [Constructable]
        public AdvancedCarpentryMould(): base()
        {
            Name = "Advanced Carpentry Mould";

            TierLevel = 4;
            MouldType = MouldSkillType.Carpentry;
        }

        public AdvancedCarpentryMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class ExpertCarpentryMould : AspectMould
    {
        [Constructable]
        public ExpertCarpentryMould(): base()
        {
            Name = "Expert Carpentry Mould";

            TierLevel = 6;
            MouldType = MouldSkillType.Carpentry;
        }

        public ExpertCarpentryMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class MasterCarpentryMould : AspectMould
    {
        [Constructable]
        public MasterCarpentryMould(): base()
        {
            Name = "Master Carpentry Mould";

            TierLevel = 8;
            MouldType = MouldSkillType.Carpentry;
        }

        public MasterCarpentryMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class LegendaryCarpentryMould : AspectMould
    {
        [Constructable]
        public LegendaryCarpentryMould(): base()
        {
            Name = "Legendary Carpentry Mould";

            TierLevel = 10;
            MouldType = MouldSkillType.Carpentry;
        }

        public LegendaryCarpentryMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    #endregion

    #region Tailoring Moulds

    public class BasicTailoringMould : AspectMould
    {
        [Constructable]
        public BasicTailoringMould(): base()
        {
            Name = "Basic Tailoring Mould";

            TierLevel = 2;
            MouldType = MouldSkillType.Tailoring;
        }

        public BasicTailoringMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class AdvancedTailoringMould : AspectMould
    {
        [Constructable]
        public AdvancedTailoringMould(): base()
        {
            Name = "Advanced Tailoring Mould";

            TierLevel = 4;
            MouldType = MouldSkillType.Tailoring;
        }

        public AdvancedTailoringMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class ExpertTailoringMould : AspectMould
    {
        [Constructable]
        public ExpertTailoringMould(): base()
        {
            Name = "Expert Tailoring Mould";

            TierLevel = 6;
            MouldType = MouldSkillType.Tailoring;
        }

        public ExpertTailoringMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class MasterTailoringMould : AspectMould
    {
        [Constructable]
        public MasterTailoringMould() : base()
        {
            Name = "Master Tailoring Mould";

            TierLevel = 8;
            MouldType = MouldSkillType.Tailoring;
        }

        public MasterTailoringMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class LegendaryTailoringMould : AspectMould
    {
        [Constructable]
        public LegendaryTailoringMould(): base()
        {
            Name = "Legendary Tailoring Mould";

            TierLevel = 10;
            MouldType = MouldSkillType.Tailoring;
        }

        public LegendaryTailoringMould(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    #endregion
}