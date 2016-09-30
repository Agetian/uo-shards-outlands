using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom;
using Server.Gumps;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace Server.SkillHandlers
{
	public class ArmsLore
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ArmsLore].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500349 ); // What item do you wish to get information about?

			return TimeSpan.FromSeconds( 2.5 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() : base( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile m_Player, object targeted )
			{
                PlayerMobile player = m_Player as PlayerMobile;

                if (player == null)
                    return;

                Item item = null;

                if (targeted is BaseWeapon)
                {
                    item = targeted as Item;

                    bool success = m_Player.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    m_Player.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    m_Player.CloseGump(typeof(ArmsLoreGump));
                    m_Player.SendGump(new ArmsLoreGump(player, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    m_Player.SendSound(0x055);

                    if (!success)
                        m_Player.SendMessage("You are uncertain of the full details of that weapon.");
                }

                else if (targeted is BaseShield)
                {
                    item = targeted as Item;

                    bool success = m_Player.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    m_Player.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    m_Player.CloseGump(typeof(ArmsLoreGump));
                    m_Player.SendGump(new ArmsLoreGump(player, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    m_Player.SendSound(0x055);

                    if (!success)
                        m_Player.SendMessage("You are uncertain of the full details of that shield.");
                }

                else if (targeted is BaseArmor)
                {
                    item = targeted as Item;

                    bool success = m_Player.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    m_Player.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    m_Player.CloseGump(typeof(ArmsLoreGump));
                    m_Player.SendGump(new ArmsLoreGump(player, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    m_Player.SendSound(0x055);

                    if (!success)
                        m_Player.SendMessage("You are uncertain of the full details of that armor.");
                }

                else if (targeted is BaseInstrument)
                {
                    item = targeted as Item;

                    bool success = m_Player.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0);

                    m_Player.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                    m_Player.CloseGump(typeof(ArmsLoreGump));
                    m_Player.SendGump(new ArmsLoreGump(player, item, success, ArmsLoreGump.DisplayMode.Normal, ArmsLoreGump.BardMode.Provocation));

                    m_Player.SendSound(0x055);

                    if (!success)
                        m_Player.SendMessage("You are uncertain of the full details of that instrument.");
                }

                else
                    m_Player.SendMessage("You cannot inspect that.");				
			}
		}
	}

    public class ArmsLoreGump : Gump
    {
        public PlayerMobile m_Player;
        public Item m_Item;
        public bool m_Success;
        public DisplayMode m_DisplayMode = DisplayMode.Normal;
        public BardMode m_BardMode = BardMode.Provocation;

        public int whiteTextHue = 2499;

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int selectionSound = 0x4D2;
        public int traitAssignedSound = 0x5BC;

        public enum DisplayMode
        {
            Normal,
            Adjusted
        }

        public enum BardMode
        {
            Provocation,
            Peacemaking,
            Discordance
        }

        public ArmsLoreGump(PlayerMobile player, Item item, bool success, DisplayMode displayMode, BardMode bardMode): base(50, 50)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (item == null) return;           
            if (item.Deleted) return;

            m_Player = player;
            m_Item = item;
            m_Success = success;
            m_DisplayMode = displayMode;
            m_BardMode = bardMode;

            bool showDetailedInfo = true;
            bool showDurability = m_Success;
            
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;         

            BaseWeapon weapon = item as BaseWeapon;
            BaseArmor armor = item as BaseArmor;
            BaseShield shield = item as BaseShield;
            BaseInstrument instrument = item as BaseInstrument;

            int categoryTextHue = 149;
            int materialTextHue = whiteTextHue;

            //Guide                        
            AddButton(18, 15, 2094, 2095, 2, GumpButtonType.Reply, 0);
            AddLabel(16, -3, 149, "Guide");    
                        
            #region Weapon

            if (weapon != null)
            {
                #region Images 

                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                #endregion

                string weaponNameText = weapon.Name;

                if (weaponNameText == null)
                    weaponNameText = "";

                weaponNameText = Utility.Capitalize(weaponNameText);

                if (weaponNameText == null)
                    weaponNameText = "";

                string weaponTypeText = "";

                if (weapon.TierLevel > 0 && weapon.Aspect != AspectEnum.None)
                {
                    weaponTypeText = AspectGear.GetAspectName(weapon.Aspect) + " Aspect: Tier " + weapon.TierLevel.ToString();

                    materialTextHue = AspectGear.GetAspectTextHue(weapon.Aspect);
                }

                else if (!(weapon.Resource == CraftResource.Iron || weapon.Resource == CraftResource.RegularWood))
                    weaponTypeText = CraftResources.GetCraftResourceName(weapon.Resource);

                double creatureAccuracyBonus = weapon.GetHitChanceBonus(true, false);
                double playerAccuracyBonus = weapon.GetHitChanceBonus(true, true);

                double tacticsBonus = 0;

                double swingDelay = weapon.GetDelay(m_Player, true).TotalSeconds;

                double accuracyVsCreature = weapon.GetSimulatedHitChance(m_Player, 100, false);
                double accuracyVsPlayer = weapon.GetSimulatedHitChance(m_Player, 100, true);

                double armsLoreCreatureDamageScalarBonus = weapon.GetArmsLoreDamageBonus(m_Player, false);
                double armsLorePlayerDamageScalarBonus = weapon.GetArmsLoreDamageBonus(m_Player, true);

                double damageVsCreatureScalar = (weapon.GetDamageScalar(m_Player, true, false) + armsLoreCreatureDamageScalarBonus) * BaseWeapon.PlayerVsCreatureDamageScalar;
                int damageVsCreatureMin = (int)Math.Round((double)weapon.BaseMinDamage * damageVsCreatureScalar);
                int damageVsCreatureMax = (int)Math.Round((double)weapon.BaseMaxDamage * damageVsCreatureScalar);

                double damageVsCreatureSlayerScalar = (weapon.GetDamageScalar(m_Player, true, false) + armsLoreCreatureDamageScalarBonus) + BaseWeapon.SlayerDamageScalarBonus * BaseWeapon.PlayerVsCreatureDamageScalar;
                int damageVsCreatureSlayerMin = (int)Math.Round((double)weapon.BaseMinDamage * damageVsCreatureSlayerScalar);
                int damageVsCreatureSlayerMax = (int)Math.Round((double)weapon.BaseMaxDamage * damageVsCreatureSlayerScalar);

                double damageVsPlayerScalar = (weapon.GetDamageScalar(m_Player, true, true) + armsLorePlayerDamageScalarBonus) * BaseWeapon.PlayerVsPlayerDamageScalar;
                int damageVsPlayerMin = (int)Math.Round((double)weapon.BaseMinDamage * damageVsPlayerScalar);
                int damageVsPlayerMax = (int)Math.Round((double)weapon.BaseMaxDamage * damageVsPlayerScalar);

                if (damageVsCreatureMin < 1)
                    damageVsCreatureMin = 1;

                if (damageVsCreatureMax < 1)
                    damageVsCreatureMax = 1;

                if (damageVsCreatureSlayerMin < 1)
                    damageVsCreatureSlayerMin = 1;

                if (damageVsCreatureSlayerMax < 1)
                    damageVsCreatureSlayerMax = 1;

                if (damageVsPlayerMin < 1)
                    damageVsPlayerMin = 1;

                if (damageVsPlayerMax < 1)
                    damageVsPlayerMax = 1;

                if (weapon.m_SkillMod != null)
                {
                    if (weapon.m_SkillMod.Skill == SkillName.Tactics)
                        tacticsBonus = weapon.m_SkillMod.Value;
                }

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, weaponNameText), 25, categoryTextHue, weaponNameText);
                        AddLabel(Utility.CenteredTextOffset(115, weaponTypeText), 45, materialTextHue, weaponTypeText);

                        //Image
                        AddItem(34 + weapon.IconOffsetX, 68 + weapon.IconOffsetY, weapon.IconItemId, weapon.IconHue);

                        //Display Mode
                        AddLabel(235, 25, 63, "Base Values");
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);

                        //Speed
                        AddLabel(232, 45, categoryTextHue, "Speed:");
                        if (showDetailedInfo)
                            AddLabel(281, 45, whiteTextHue, weapon.Speed.ToString());
                        else
                            AddLabel(281, 45, whiteTextHue, "?");

                        //Damage
                        AddLabel(222, 65, categoryTextHue, "Damage:");
                        if (showDetailedInfo)
                            AddLabel(280, 65, whiteTextHue, weapon.BaseMinDamage + "-" + weapon.BaseMaxDamage);
                        else
                            AddLabel(280, 65, whiteTextHue, "?");

                        //Durability
                        AddLabel(209, 85, categoryTextHue, "Durability:");
                        if (showDetailedInfo && showDurability)
                            AddLabel(280, 85, whiteTextHue, weapon.HitPoints + "/" + weapon.MaxHitPoints);
                        else
                            AddLabel(280, 85, whiteTextHue, "?/?");

                        //Accuracy
                        AddLabel(212, 105, categoryTextHue, "Accuracy:");
                        AddLabel(280, 105, whiteTextHue, "+" + Utility.CreatePercentageString(creatureAccuracyBonus));

                        //Tactics
                        AddLabel(222, 125, categoryTextHue, "Tactics:");
                        AddLabel(280, 125, whiteTextHue, "+" + tacticsBonus);

                        //Slayer
                        if (weapon.SlayerGroup != SlayerGroupType.None)
                        {
                            string slayerName = weapon.SlayerGroup.ToString() + " Slaying";

                            AddLabel(Utility.CenteredTextOffset(280, slayerName), 145, whiteTextHue, slayerName);
                        }

                        //Arcane Charges
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(169, 165, categoryTextHue, "Arcane Charges:");
                            AddLabel(280, 165, whiteTextHue, weapon.ArcaneCharges.ToString());
                        }

                        //Experience
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(199, 185, categoryTextHue, "Experience:");
                            AddLabel(279, 185, whiteTextHue, weapon.Experience.ToString() + "/250");
                        }

                        //Effect Chance
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(26, 165, categoryTextHue, "Effect Chance:");
                            AddLabel(127, 165, whiteTextHue, "4.1%"); //TEST: FIX THIS
                        }

                        //Effect
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(25, 185, categoryTextHue, "Effect:");
                            AddLabel(75, 185, whiteTextHue, "Firestorm");
                        }
                    break;
                        
                    case DisplayMode.Adjusted:
                        //Display Mode
                        AddLabel(235, 25, 63, "Your Values");
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, weaponNameText), 25, categoryTextHue, weaponNameText);
                        AddLabel(Utility.CenteredTextOffset(115, weaponTypeText), 45, materialTextHue, weaponTypeText);
		            
                        //Image
                        AddItem(34 + weapon.IconOffsetX, 68 + weapon.IconOffsetY, weapon.IconItemId, weapon.IconHue);

                        //Swing Speed
                        AddLabel(213, 45, categoryTextHue, "Swing Delay:");
                        if (showDetailedInfo)
                            AddLabel(297, 45, whiteTextHue, Utility.CreateDecimalString(swingDelay, 2) + "s");
                        else
                            AddLabel(297, 45, whiteTextHue, "?");

                        //Accuracy vs Creature
                        AddLabel(153, 82, categoryTextHue, "Accuracy vs Creature:");
                        if (showDetailedInfo)
                            AddLabel(297, 82, whiteTextHue, Utility.CreateDecimalPercentageString(accuracyVsCreature, 2));
                        else
                            AddLabel(297, 82, whiteTextHue, "?");

                        //Damage vs Creature
                        AddLabel(162, 102, categoryTextHue, "Damage vs Creature:");
                        if (showDetailedInfo)
                            AddLabel(297, 102, whiteTextHue, damageVsCreatureMin.ToString() + "-" + damageVsCreatureMax.ToString());
                        else
                            AddLabel(297, 102, whiteTextHue, "?");

                        //Damage If Slayer Type
                        if (weapon.SlayerGroup != SlayerGroupType.None)
                        {
                            AddLabel(147, 122, categoryTextHue, "Damage If Slayer Type:");

                            if (showDetailedInfo)
                                AddLabel(297, 122, whiteTextHue, damageVsCreatureSlayerMin.ToString() + "-" + damageVsCreatureSlayerMax.ToString());
                            else
                                AddLabel(297, 122, whiteTextHue, "?");
                        }

                        //Accuracy vs Player
                        AddLabel(168, 165, categoryTextHue, "Accuracy vs Player:");
                        if (showDetailedInfo)
                            AddLabel(297, 165, whiteTextHue, Utility.CreateDecimalPercentageString(accuracyVsPlayer, 2));
                        else
                            AddLabel(297, 165, whiteTextHue, "?");

                        //Daamge vs Player
                        AddLabel(177, 185, categoryTextHue, "Damage vs Player:");
                        if (showDetailedInfo)
                            AddLabel(297, 185, whiteTextHue, damageVsPlayerMin.ToString() + "-" + damageVsPlayerMax.ToString());
                        else
                            AddLabel(297, 185, whiteTextHue, "?");
                       
                        //Effect Chance
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(26, 165, categoryTextHue, "Effect Chance:");
                            AddLabel(127, 165, whiteTextHue, "4.1%"); //TEST: FIX THIS
                        }

                        //Effect
                        if (weapon.TierLevel > 0)
                        {
                            AddLabel(25, 185, categoryTextHue, "Effect:");
                            AddLabel(75, 185, whiteTextHue, "Firestorm");
                        }
                    break;
                }
            }

            #endregion

            #region Armor

            if (armor != null && shield == null)
            {
                #region Images 

                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                #endregion

                string armorNameText = armor.Name;

                if (armorNameText == null)
                    armorNameText = "";

                armorNameText = Utility.Capitalize(armorNameText);

                if (armorNameText == null)
                    armorNameText = "";

                string armorTypeText = "";

                if (armor.TierLevel > 0 && armor.Aspect != AspectEnum.None)
                {
                    armorTypeText = AspectGear.GetAspectName(armor.Aspect) + " Tier " + armor.TierLevel.ToString();

                    materialTextHue = AspectGear.GetAspectTextHue(armor.Aspect);
                }

                else if (!(armor.Resource == CraftResource.Iron || armor.Resource == CraftResource.RegularWood || armor.Resource == CraftResource.RegularLeather))
                    armorTypeText = CraftResources.GetCraftResourceName(armor.Resource);

                double armorValue = armor.ArmorRatingScaled;

                string meditationText = "0%";
                string suitThemeText = "Resistance";

                double fullSuitArmorValue = armor.ArmorRating; //TEST: FINISH THIS
                double fullSuitDexPenalty = 0; //TEST: FINISH THIS
                double fullSuitMeditation = 0; //TEST: FINISH THIS

                string fullSuitMeditationText = "0%";

                List<string> m_SuitEffects = new List<string>();
                List<string> m_SuitEffectValues = new List<string>();

                switch (armor.Aspect)
                {
                }

                switch (armor.MeditationAllowance)
                {
                    case ArmorMeditationAllowance.None: meditationText = "0%"; break;
                    case ArmorMeditationAllowance.Quarter: meditationText = "25%"; break;
                    case ArmorMeditationAllowance.Half: meditationText = "50%"; break;
                    case ArmorMeditationAllowance.ThreeQuarter: meditationText = "75%"; break;
                    case ArmorMeditationAllowance.All: meditationText = "100%"; break;
                }

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Display Mode
                        if (armor.TierLevel > 0)
                        {
                            AddLabel(231, 25, 63, "Base Values");
                            AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);
                        }

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, armorNameText), 25, categoryTextHue, armorNameText);
                        AddLabel(Utility.CenteredTextOffset(115, armorTypeText), 45, materialTextHue, armorTypeText);

                        //Image
                        AddItem(34 + armor.IconOffsetX, 68 + armor.IconOffsetY, armor.IconItemId, armor.IconHue);

                        //Properties
                        AddLabel(231, 45, categoryTextHue, "Armor:");
                        AddLabel(281, 45, whiteTextHue, Utility.CreateDecimalString(armorValue, 1));

                        AddLabel(212, 65, categoryTextHue, "Dex Loss:");
                        AddLabel(280, 65, whiteTextHue, armor.DexBonus.ToString());

                        AddLabel(212, 85, categoryTextHue, "Durability:");
                        if (showDurability)
                            AddLabel(280, 85, whiteTextHue, armor.HitPoints.ToString() + "/" + armor.MaxHitPoints.ToString());
                        else
                            AddLabel(280, 85, whiteTextHue, "?/?");

                        AddLabel(207, 105, categoryTextHue, "Meditation:");
                        AddLabel(280, 105, whiteTextHue, meditationText);

                        if (armor.TierLevel > 0)
                        {
                            AddLabel(175, 165, categoryTextHue, "Arcane Charges:");
                            AddLabel(280, 165, whiteTextHue, armor.ArcaneCharges.ToString());
                        }

                        if (armor.TierLevel > 0)
                        {
                            AddLabel(207, 185, categoryTextHue, "Experience:");
                            AddLabel(279, 185, whiteTextHue, armor.Experience.ToString() + "/250");
                        }

                        //Theme
                        if (armor.TierLevel > 0)
                        {
                            AddLabel(Utility.CenteredTextOffset(113, "Suit Theme"), 165, categoryTextHue, "Suit Theme");
                            AddLabel(Utility.CenteredTextOffset(115, suitThemeText), 185, whiteTextHue, suitThemeText);
                        }
                    break;

                    case DisplayMode.Adjusted:
                        //Display Mode
                        AddLabel(209, 25, 63, "Full Suit Effects");
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);                        
                        
                        //Description
                        AddLabel(82, 25, categoryTextHue, "Full Suit");
                        AddLabel(54, 45, materialTextHue, armorTypeText);
			            
                        //Image
                        AddItem(34 + armor.IconOffsetX, 68 + armor.IconOffsetY, armor.IconItemId, armor.IconHue);
			           
                        //Suit Effects
                        int startY = 45;

                        for (int a = 0; a < m_SuitEffects.Count; a++)
                        {
                            AddLabel(Utility.CenteredTextOffset(260, m_SuitEffects[a]), startY, categoryTextHue, m_SuitEffects[a]);
                            AddLabel(Utility.CenteredTextOffset(260, m_SuitEffectValues[a]), startY + 20, whiteTextHue, m_SuitEffectValues[a]);

                            startY += 40;
                        }                        			            
			          
                        //Properties
                        AddLabel(33, 145, categoryTextHue, "Total Armor:");
                        AddLabel(115, 145, whiteTextHue, fullSuitArmorValue.ToString());

                        AddLabel(51, 165, categoryTextHue, "Dex Loss:");
                        AddLabel(115, 165, whiteTextHue, fullSuitDexPenalty.ToString());

                        AddLabel(46, 185, categoryTextHue, "Meditation:");
                        AddLabel(115, 185, whiteTextHue, fullSuitMeditationText);			            
                    break;
                }
            }

            #endregion

            #region Shield

            if (shield != null)
            {
                #region Images

                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                #endregion

                string shieldNameText = shield.Name;

                if (shieldNameText == null)
                    shieldNameText = "";

                shieldNameText = Utility.Capitalize(shieldNameText);

                if (shieldNameText == null)
                    shieldNameText = "";

                string shieldTypeText = "";

                if (shield.TierLevel > 0 && shield.Aspect != AspectEnum.None)
                {
                    shieldTypeText = AspectGear.GetAspectName(shield.Aspect) + " Tier " + shield.TierLevel.ToString();

                    materialTextHue = AspectGear.GetAspectTextHue(shield.Aspect);
                }

                else if (!(shield.Resource == CraftResource.Iron || shield.Resource == CraftResource.RegularWood))
                    shieldTypeText = CraftResources.GetCraftResourceName(shield.Resource);

                double armorValue = shield.ArmorRating;
                double dexPenalty = shield.DexBonus;

                double parryChance = m_Player.Skills[SkillName.Parry].Value * BaseShield.ShieldParrySkillScalar;
                double damageReduction = 1.0 - BaseShield.ShieldParryDamageScalar;

                string meditationText = "0%";
                string effectText = "Resistance";                

                switch (displayMode)
                {
                    case DisplayMode.Normal:
                        //Display Mode
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);
                        AddLabel(231, 25, 63, "Base Values");

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, shieldNameText), 25, categoryTextHue, shieldNameText);
                        AddLabel(Utility.CenteredTextOffset(115, shieldTypeText), 45, materialTextHue, shieldTypeText);

                        //Image			           
                        AddItem(34 + shield.IconOffsetX, 68 + shield.IconOffsetY, shield.IconItemId, shield.IconHue);
                        			            
                        //Properties
                        AddLabel(228, 45, categoryTextHue, "Armor:");
                        AddLabel(281, 45, whiteTextHue, armorValue.ToString());

                        AddLabel(210, 65, categoryTextHue, "Dex Loss:");
                        AddLabel(280, 65, whiteTextHue, dexPenalty.ToString());

                        AddLabel(210, 85, categoryTextHue, "Durability:");
                        if (success)
                            AddLabel(280, 85, whiteTextHue, shield.HitPoints.ToString() + "/" + shield.MaxHitPoints.ToString());
                        else
                            AddLabel(280, 85, whiteTextHue, "?/?");

                        AddLabel(205, 105, categoryTextHue, "Meditation:");
                        AddLabel(280, 105, whiteTextHue, meditationText);

                        if (shield.TierLevel > 0)
                        {
                            AddLabel(174, 165, categoryTextHue, "Arcane Charges:");
                            AddLabel(280, 165, whiteTextHue, shield.ArcaneCharges.ToString());
                        }

                        if (shield.TierLevel > 0)
                        {
                            AddLabel(206, 185, categoryTextHue, "Experience:");
                            AddLabel(279, 185, whiteTextHue, shield.Experience.ToString() + "/250");
                        }
			            
                        //Shield Effect
                        if (shield.TierLevel > 0)
                        {
                            AddLabel(Utility.CenteredTextOffset(113, "Shield Effect"), 165, categoryTextHue, "Shield Effect");
                            AddLabel(Utility.CenteredTextOffset(115, effectText), 185, whiteTextHue, effectText);
                        }
                    break;

                    case DisplayMode.Adjusted:
                        //Display Mode
                        AddLabel(231, 25, 63, "Your Values");
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, shieldNameText), 25, categoryTextHue, shieldNameText);
                        AddLabel(Utility.CenteredTextOffset(115, shieldTypeText), 45, materialTextHue, shieldTypeText);

                        //Image
                        AddItem(34 + shield.IconOffsetX, 68 + shield.IconOffsetY, shield.IconItemId, shield.IconHue);
                        		
	                    //Values
                        string parryChanceText = Utility.CreateDecimalPercentageString(parryChance, 1);
                        string parryReductionText = Utility.CreateDecimalPercentageString(damageReduction, 0);

                        AddLabel(218, 45, categoryTextHue, "Parry Chance");
                        AddLabel(Utility.CenteredTextOffset(260, parryChanceText), 65, whiteTextHue, parryChanceText);

                        AddLabel(190, 84, categoryTextHue, "Parry Damage Reduction");
                        AddLabel(Utility.CenteredTextOffset(260, parryReductionText), 104, whiteTextHue, parryReductionText);

                        //Properties
                        AddLabel(68, 145, categoryTextHue, "Armor:");
                        AddLabel(115, 145, whiteTextHue, armorValue.ToString());

                        AddLabel(49, 165, categoryTextHue, "Dex Loss:");
                        AddLabel(115, 165, whiteTextHue, dexPenalty.ToString());

                        AddLabel(44, 185, categoryTextHue, "Meditation:");
                        AddLabel(115, 185, whiteTextHue, meditationText);
                    break;
                }
            }

            #endregion

            #region Instrument

            if (instrument != null)
            {
                #region Images

                AddImage(220, 123, 103);
                AddImage(220, 65, 103);
                AddImage(220, 4, 103);
                AddImage(98, 124, 103);
                AddImage(98, 65, 103);
                AddImage(98, 4, 103);
                AddImage(5, 124, 103);
                AddImage(5, 65, 103);
                AddImage(5, 4, 103);
                AddImage(18, 16, 3604, 2052);
                AddImage(18, 85, 3604, 2052);
                AddImage(102, 16, 3604, 2052);
                AddImage(102, 85, 3604, 2052);
                AddImage(222, 16, 3604, 2052);
                AddImage(223, 85, 3604, 2052);

                #endregion

                string instrumentNameText = instrument.Name;

                if (instrumentNameText == null)
                    instrumentNameText = "";

                instrumentNameText = Utility.Capitalize(instrumentNameText);

                if (instrumentNameText == null)
                    instrumentNameText = "";

                string instrumentTypeText = "";

                if (instrument.TierLevel > 0 && instrument.Aspect != AspectEnum.None)
                {
                    instrumentTypeText = AspectGear.GetAspectName(instrument.Aspect) + " Tier " + instrument.TierLevel.ToString();

                    materialTextHue = AspectGear.GetAspectTextHue(instrument.Aspect);
                }

                else if (instrument.Resource != CraftResource.RegularWood)
                    instrumentTypeText = CraftResources.GetCraftResourceName(instrument.Resource);

                double bardSkillBonus = BaseInstrument.GetBardBonusSkill(m_Player, null, instrument);
                double effectiveSkill = 0;                

                switch (m_BardMode)
                {
                    case BardMode.Provocation: effectiveSkill = m_Player.Skills[SkillName.Provocation].Value + bardSkillBonus; break;
                    case BardMode.Peacemaking: effectiveSkill = m_Player.Skills[SkillName.Peacemaking].Value + bardSkillBonus; break;
                    case BardMode.Discordance: effectiveSkill = m_Player.Skills[SkillName.Discordance].Value + bardSkillBonus; break;
                }

                double normal10 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 10);
                double slayer10 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 10);

                double normal20 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 20);
                double slayer20 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 20);

                double normal30 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 30);
                double slayer30 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 30);

                double normal40 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 40);
                double slayer40 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 40);

                double normal50 = BaseInstrument.GetBardSuccessChance(effectiveSkill, 50);
                double slayer50 = BaseInstrument.GetBardSuccessChance(effectiveSkill + BaseInstrument.SlayerSkillBonus, 50);

                double normalMinimum = effectiveSkill * BaseInstrument.MinimumEffectiveChanceScalar;
                double slayerMinimum = (effectiveSkill + BaseInstrument.SlayerSkillBonus) * BaseInstrument.MinimumEffectiveChanceScalar;

                switch (displayMode)
                {
                    case DisplayMode.Normal:                        
                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, instrumentNameText), 25, categoryTextHue, instrumentNameText);
                        AddLabel(Utility.CenteredTextOffset(115, instrumentTypeText), 45, materialTextHue, instrumentTypeText);

                        //Display Mode
                        AddLabel(231, 25, 63, "Base Values");
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);
			            
                        //Image
                        AddItem(34 + instrument.IconOffsetX, 68 + instrument.IconOffsetY, instrument.IconItemId, instrument.IconHue);

                        //Properties
                        AddLabel(212, 45, categoryTextHue, "Durability:");
                        AddLabel(281, 45, whiteTextHue, instrument.UsesRemaining.ToString() + "/" + instrument.InitMaxUses.ToString());

                        AddLabel(207, 65, categoryTextHue, "Bard Skill:");
                        AddLabel(280, 65, whiteTextHue, "+" + bardSkillBonus);

                        if (instrument.SlayerGroup != SlayerGroupType.None)
                        {
                            string slayerName = instrument.SlayerGroup.ToString() + " Slaying";
                            AddLabel(Utility.CenteredTextOffset(280, slayerName), 85, whiteTextHue, slayerName);
                        }

                        if (instrument.TierLevel > 0)
                        {
                            AddLabel(169, 165, categoryTextHue, "Arcane Charges:");
                            AddLabel(280, 165, whiteTextHue, instrument.ArcaneCharges.ToString());
                        }

                        if (instrument.TierLevel > 0)
                        {
                            AddLabel(199, 185, categoryTextHue, "Experience:");
                            AddLabel(279, 185, whiteTextHue, instrument.Experience.ToString() + "/250");
                        }
                    break;

                    case DisplayMode.Adjusted:
                        //Display Mode
                        AddButton(327, 29, 1210, 1209, 2, GumpButtonType.Reply, 0);
                        AddLabel(231, 25, 63, "Your Values");

                        //Description
                        AddLabel(Utility.CenteredTextOffset(115, instrumentNameText), 25, categoryTextHue, instrumentNameText);
                        AddLabel(Utility.CenteredTextOffset(115, instrumentTypeText), 45, materialTextHue, instrumentTypeText);

                        //Image
                        AddItem(34 + instrument.IconOffsetX, 68 + instrument.IconOffsetY, instrument.IconItemId, instrument.IconHue);

                        //Skill Type
                        AddLabel(65, categoryTextHue, 2562, "Displaying");
                        AddButton(33, 174, 2223, 2223, 3, GumpButtonType.Reply, 0);
                        AddButton(138, 174, 2224, 2224, 4, GumpButtonType.Reply, 0);

                        switch (m_BardMode)
                        {
                            case BardMode.Provocation: AddLabel(61, 170, 2420, "Provocation"); break; //BaseInstrument.ProvokedTextHue
                            case BardMode.Peacemaking: AddLabel(61, 170, BaseInstrument.PacifiedTextHue, "Peacemaking"); break;
                            case BardMode.Discordance: AddLabel(61, 170, BaseInstrument.DiscordedTextHue, "Discordance"); break;
                        }

                        AddLabel(40, 190, categoryTextHue, "Effective Skill:");
                        AddLabel(136, 190, whiteTextHue, Utility.CreateDecimalString(effectiveSkill, 1));

                        AddLabel(172, 50, 2562, "Success vs Difficulty Value");
                        AddLabel(245, 70, categoryTextHue, "Normal");
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(296, 70, categoryTextHue, "Slayer");

                        AddLabel(186, 90, categoryTextHue, "Diff");
                        AddLabel(221, 90, whiteTextHue, "10:");
                        AddLabel(250, 90, whiteTextHue, Utility.CreateDecimalPercentageString(normal10, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 90, whiteTextHue, Utility.CreateDecimalPercentageString(slayer10, 1));

                        AddLabel(186, 110, categoryTextHue, "Diff 20:");
                        AddLabel(250, 110, whiteTextHue, Utility.CreateDecimalPercentageString(normal20, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 110, whiteTextHue, Utility.CreateDecimalPercentageString(slayer20, 1));

                        AddLabel(186, 130, categoryTextHue, "Diff 30:");
                        AddLabel(250, 130, whiteTextHue, Utility.CreateDecimalPercentageString(normal30, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 130, whiteTextHue, Utility.CreateDecimalPercentageString(slayer30, 1));

                        AddLabel(186, 150, categoryTextHue, "Diff 40:");
                        AddLabel(250, 150, whiteTextHue, Utility.CreateDecimalPercentageString(normal40, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 150, whiteTextHue, Utility.CreateDecimalPercentageString(slayer40, 1));

                        AddLabel(186, 170, categoryTextHue, "Diff 50:");
                        AddLabel(250, 170, whiteTextHue, Utility.CreateDecimalPercentageString(normal50, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 170, whiteTextHue, Utility.CreateDecimalPercentageString(slayer50, 1));

                        AddLabel(187, 190, categoryTextHue, "Minimum:");
                        AddLabel(250, 190, whiteTextHue, Utility.CreateDecimalPercentageString(normalMinimum, 1));
                        if (instrument.SlayerGroup != SlayerGroupType.None)
                            AddLabel(298, 190, whiteTextHue, Utility.CreateDecimalPercentageString(slayerMinimum, 1));				           
                    break;
                }
            }

            #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Item == null) return;
            if (m_Item.Deleted) return;

            BaseWeapon weapon = m_Item as BaseWeapon;
            BaseArmor armor = m_Item as BaseArmor;
            BaseShield shield = m_Item as BaseShield;
            BaseInstrument instrument = m_Item as BaseInstrument;

            bool isWeapon = weapon != null;
            bool isArmor = armor != null && shield == null;
            bool isShield = shield != null;
            bool isInstrument = instrument != null;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                break;

                //Display Made
                case 2:
                    switch (m_DisplayMode)
                    {
                        case DisplayMode.Normal:
                            if (isArmor && !isShield)
                            {
                                if (armor.TierLevel > 0)
                                {
                                    m_DisplayMode = DisplayMode.Adjusted;
                                    m_Player.SendSound(0x055);
                                }
                            }

                            else
                            {
                                m_DisplayMode = DisplayMode.Adjusted;
                                m_Player.SendSound(0x055);
                            }
                        break;

                        case DisplayMode.Adjusted:
                            m_DisplayMode = DisplayMode.Normal;
                            m_Player.SendSound(0x055);
                        break;
                    }

                    closeGump = false;
                break;

                //Barding Skill Previous
                case 3:
                    if (isInstrument)
                    {
                        switch (m_BardMode)
                        {
                            case BardMode.Provocation: m_BardMode = BardMode.Discordance; break;
                            case BardMode.Peacemaking: m_BardMode = BardMode.Provocation; break;
                            case BardMode.Discordance: m_BardMode = BardMode.Peacemaking; break;
                        }
                    }

                    closeGump = false;
                    break;

                //Barding Skill Right
                case 4:
                    if (isInstrument)
                    {
                        switch (m_BardMode)
                        {
                            case BardMode.Provocation: m_BardMode = BardMode.Peacemaking; break;
                            case BardMode.Peacemaking: m_BardMode = BardMode.Discordance; break;
                            case BardMode.Discordance: m_BardMode = BardMode.Provocation; break;
                        }
                    }

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ArmsLoreGump));
                m_Player.SendGump(new ArmsLoreGump(m_Player, m_Item, m_Success, m_DisplayMode, m_BardMode));
            }

            else
                m_Player.SendSound(closeGumpSound);
        }
    }
}