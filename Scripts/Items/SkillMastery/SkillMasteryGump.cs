using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public enum SkillMasteryPageType
    {
        Overview,
        Orb,
        Scroll
    }

    public class SkillMasteryGump : Gump
    {
        PlayerMobile m_Player;
        SkillMasteryPageType m_SkillMasteryPage = SkillMasteryPageType.Overview;
        Item m_Item;
        SkillMasteryOrb m_SkillMasteryOrb;
        SkillMasteryScroll m_SkillMasteryScroll;
        
        int EntriesPerSide = 12;

        int WhiteTextHue = 2499;

        public SkillMasteryGump(Mobile player, SkillMasteryPageType skillMasteryPageType, Item item): base(10, 10)
        {
            m_Player = player as PlayerMobile;
            m_SkillMasteryPage = skillMasteryPageType;
            m_Item = item;

            if (m_Player == null) 
                return;

            switch (skillMasteryPageType)
            {
                case SkillMasteryPageType.Overview:
                break;

                case SkillMasteryPageType.Orb:
                    if (!(item is SkillMasteryOrb)) return;
                    if (item == null) return;
                    if (item.Deleted) return; 

                    m_SkillMasteryOrb = item as SkillMasteryOrb;
                break;

                case SkillMasteryPageType.Scroll:
                    if (!(item is SkillMasteryScroll)) return;
                    if (item == null) return;
                    if (item.Deleted) return;

                    m_SkillMasteryScroll = item as SkillMasteryScroll;
                break;
            }

            #region Images

            AddImage(5, 5, 103, 2499);
            AddImage(140, 5, 103, 2499);
            AddImage(266, 5, 103, 2499);
            AddImage(140, 92, 103, 2499);
            AddImage(5, 92, 103, 2499);
            AddImage(266, 92, 103, 2499);
            AddImage(140, 180, 103, 2499);
            AddImage(5, 180, 103, 2499);
            AddImage(266, 180, 103, 2499);
            AddImage(140, 264, 103, 2499);
            AddImage(5, 264, 103, 2499);
            AddImage(266, 264, 103, 2499);
            AddImage(400, 5, 103, 2499);
            AddImage(400, 92, 103, 2499);
            AddImage(400, 180, 103, 2499);
            AddImage(400, 265, 103, 2499);
            AddImage(400, 362, 103, 2499);
            AddImage(266, 361, 103, 2499);
            AddImage(140, 361, 103, 2499);
            AddImage(5, 361, 103, 2499);
            AddImage(100, 361, 5104, 2052);
            AddImage(19, 361, 5104, 2052);
            AddImage(164, 361, 5104, 2052);
            AddImage(253, 361, 5104, 2052);
            AddImage(312, 361, 5104, 2052);
            AddImage(398, 361, 5104, 2052);
            AddImage(442, 361, 5104, 2052);
            AddImage(273, 83, 2081, 2499);
            AddImage(271, 282, 2081, 2499);
            AddImage(272, 149, 2081, 2499);
            AddImage(272, 13, 2081, 2499);
            AddImage(272, 214, 2081, 2499);
            AddImage(15, 14, 2081, 2499);
            AddImage(15, 83, 2081, 2499);
            AddImage(14, 149, 2081, 2499);
            AddImage(14, 214, 2081, 2499);
            AddImage(14, 282, 2081, 2499);
            AddImageTiled(272, 10, 6, 341, 2701);

            AddItem(48, 390, 2942);
            AddItem(23, 376, 2943);
            AddItem(43, 381, 2507);
            AddItem(42, 397, 4030);
            AddItem(55, 376, 7716);
            AddItem(23, 368, 7717, 2652);
            AddItem(45, 387, 4031);
           
            AddImage(184, 12, 2446, 2401);
            AddLabel(232, 12, 2599, "Skill Mastery");
            
            AddLabel(65, 41, 149, "Skill");
            AddLabel(151, 41, 149, "Value");
            AddLabel(199, 41, 149, "Cap");

            AddLabel(324, 40, 149, "Skill");
            AddLabel(410, 40, 149, "Value");
            AddLabel(458, 40, 149, "Cap");                        

            AddLabel(95, 379, 149, "Total Skills");
            AddLabel(95, 405, 149, "Total Skill Cap");            

            #endregion

            List<SkillName> m_SkillList = new List<SkillName>();

            foreach (SkillName skillName in SkillMasteryScroll.Skills)
            {
                m_SkillList.Add(skillName);
            }

            int leftStartY = 60;
            int rightStartY = 60;

            int spacingY = 20;

            bool canUseOrb = false;
            bool canUseScroll = false;

            if (m_Player.SkillsCap < PlayerMobile.MaxSkillCap)
                canUseOrb = true;

            for (int a = 0; a < m_SkillList.Count; a++)
            {
                int textHue = WhiteTextHue;

                SkillName skillName = m_SkillList[a];
                string skillValueText = m_Player.Skills[skillName].Base.ToString();
                string skillCapText = (m_Player.Skills[skillName].CapFixedPoint / 10).ToString();
                int skillCap = m_Player.Skills[skillName].CapFixedPoint / 10;
                
                if ((m_Player.Skills[skillName].CapFixedPoint / 10) > 100)
                    textHue = 2599;

                bool canIncrease = false;

                if (skillMasteryPageType == SkillMasteryPageType.Scroll)
                {
                    if (skillName == m_SkillMasteryScroll.Skill)
                    {
                        if (m_Player.Skills[skillName].Cap < 120)
                        {                        
                            canUseScroll = true;
                            canIncrease = true;
                            textHue = 63;

                            skillCapText = ((m_Player.Skills[skillName].CapFixedPoint / 10) + 1).ToString();
                        }
                    }
                }

                //Left Side
                if (a < EntriesPerSide)
                {
                    AddLabel(25, leftStartY, textHue, SkillCheck.GetSkillName(skillName));
                    AddLabel(Utility.CenteredTextOffset(165, skillValueText), leftStartY, textHue, skillValueText);
                    AddLabel(Utility.CenteredTextOffset(210, skillCapText), leftStartY, textHue, skillCapText);

                    if (canUseScroll && canIncrease)
                    {
                        AddImage(232, leftStartY + 1, 5600, 63);
                        AddLabel(252, leftStartY, 63, "+1");
                    }

                    leftStartY += spacingY;
                }

                //Right Side
                else
                {
                    AddLabel(285, rightStartY, textHue, SkillCheck.GetSkillName(skillName));
                    AddLabel(Utility.CenteredTextOffset(426, skillValueText), rightStartY, textHue, skillValueText);
                    AddLabel(Utility.CenteredTextOffset(470, skillCapText), rightStartY, textHue, skillCapText);

                    if (canUseScroll && canIncrease)
                    {
                        AddImage(488, rightStartY + 1, 5600, 63);
                        AddLabel(508, rightStartY, 63, "+1");
                    }

                    rightStartY += spacingY;
                }
            }

            //Guide
            AddButton(19, 15, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(15, 1, 149, "Guide");

            string actionText = ""; 
            string resultText = "";

            int totalSkills = (m_Player.SkillsTotal / 10);
            int totalSkillCap = (m_Player.SkillsCap / 10);

            int totalSkillsHue = WhiteTextHue;
            int totalSkillCapHue = WhiteTextHue;

            if (totalSkills > 700)
                totalSkillsHue = 2599;

            if (totalSkillCap > 700)
                totalSkillCapHue = 2599;

            switch (m_SkillMasteryPage)
            {
                case SkillMasteryPageType.Overview:
                    AddLabel(199, 379, totalSkillsHue, totalSkills.ToString());
                    AddLabel(199, 405, totalSkillCapHue, totalSkillCap.ToString());
                break;

                case SkillMasteryPageType.Orb:
                    if (canUseOrb)
                    {
                        int newSkillCap = totalSkillCap + 1;

                        AddLabel(199, 379, totalSkillsHue, totalSkills.ToString());
                        AddLabel(199, 405, 63, newSkillCap.ToString());
                        AddImage(232, 406, 5600, 63);
                        AddLabel(252, 405, 63, "+1");                        

                        actionText = "Use Mastery Orb?";
                        resultText = "(Increase Total Skill Cap to " + newSkillCap + ")";                                               

                        AddLabel(Utility.CenteredTextOffset(400, actionText), 364, 63, actionText);
                        AddLabel(Utility.CenteredTextOffset(410, resultText), 382, 2599, resultText);
                        AddItem(375, 415, 22336, 2966); //Orb
                        AddButton(423, 411, 247, 248, 2, GumpButtonType.Reply, 0);
                    }

                    else
                    {
                        AddLabel(199, 379, totalSkillsHue, totalSkills.ToString());
                        AddLabel(199, 405, totalSkillCapHue, totalSkillCap.ToString());

                        actionText = "Mastery Orb Unusable";
                        resultText = "(Already at Maximum Total Skill Cap)";

                        AddLabel(Utility.CenteredTextOffset(400, actionText), 364, 149, actionText);
                        AddLabel(Utility.CenteredTextOffset(410, resultText), 382, 149, resultText);
                    }

                break;

                case SkillMasteryPageType.Scroll:
                    if (canUseScroll)
                    {
                        AddLabel(199, 379, totalSkillsHue, totalSkills.ToString());
                        AddLabel(199, 405, totalSkillCapHue, totalSkillCap.ToString());

                        string skillName = SkillCheck.GetSkillName(m_SkillMasteryScroll.Skill);
                        int newSkillCap = (m_Player.Skills[m_SkillMasteryScroll.Skill].CapFixedPoint / 10) + 1;

                        actionText = "Use Mastery Scroll?";
                        resultText = "(Increase " + skillName + " Skill Cap to " + newSkillCap + ")";

                        AddLabel(Utility.CenteredTextOffset(400, actionText), 364, 63, actionText);
                        AddLabel(Utility.CenteredTextOffset(410, resultText), 382, 2599, resultText);
                        AddItem(372, 406, 5360, 2963); //Scroll
                        AddButton(423, 411, 247, 248, 2, GumpButtonType.Reply, 0);
                    }

                    else
                    {
                        AddLabel(199, 379, totalSkillsHue, totalSkills.ToString());
                        AddLabel(199, 405, totalSkillCapHue, totalSkillCap.ToString());

                        actionText = "Mastery Scroll Unusable";
                        resultText = "(Already At Skill's Maximum Cap)";

                        AddLabel(Utility.CenteredTextOffset(400, actionText), 364, 149, actionText);
                        AddLabel(Utility.CenteredTextOffset(410, resultText), 382, 149, resultText);
                    }                    
                break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Confirm
                case 2:
                    switch (m_SkillMasteryPage)
                    {
                        case SkillMasteryPageType.Overview:
                        break;

                        case SkillMasteryPageType.Orb:
                            if (m_SkillMasteryOrb == null)
                            {
                                m_Player.SendMessage("That item no longer exists.");
                                return;
                            }

                            if (m_SkillMasteryOrb.Deleted)
                            {
                                m_Player.SendMessage("That item no longer exists.");
                                return;
                            }

                            if (!m_SkillMasteryOrb.IsChildOf(m_Player.Backpack))
                            {
                                m_Player.SendMessage("This must be in your backpack in order to use it.");
                                return;
                            }

                            if (m_Player.SkillsCap < PlayerMobile.MaxSkillCap)
                            {
                                m_Player.SkillsCap += 10;

                                m_Player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                                m_Player.PlaySound(0x3BD);

                                m_Player.SendMessage(63, "You increase your maximum total skill cap by 1.");

                                if (m_SkillMasteryOrb.Amount > 1)
                                {
                                    m_SkillMasteryOrb.Amount--;

                                    m_Player.CloseGump(typeof(SkillMasteryGump));
                                    m_Player.SendGump(new SkillMasteryGump(m_Player, SkillMasteryPageType.Orb, m_SkillMasteryOrb));
                                }

                                else
                                {
                                    m_SkillMasteryOrb.Delete();

                                    m_Player.CloseGump(typeof(SkillMasteryGump));
                                    m_Player.SendGump(new SkillMasteryGump(m_Player, SkillMasteryPageType.Overview, null));
                                }                               

                                return;
                            }

                            else
                            {
                                m_Player.SendMessage("You are already at your maximum total skill cap.");
                                return;
                            }
                        break;

                        case SkillMasteryPageType.Scroll:
                            if (m_SkillMasteryScroll == null)
                            {
                                m_Player.SendMessage("That item no longer exists.");
                                return;
                            }

                            if (m_SkillMasteryScroll.Deleted)
                            {
                                m_Player.SendMessage("That item no longer exists.");
                                return;
                            }

                            if (!m_SkillMasteryScroll.IsChildOf(m_Player.Backpack))
                            {
                                m_Player.SendMessage("This must be in your backpack in order to use it.");
                                return;
                            }

                            SkillName skillName = m_SkillMasteryScroll.Skill;
                            string skillNameText = SkillCheck.GetSkillName(skillName);

                            if ((m_Player.Skills[skillName].CapFixedPoint / 10) < 120)
                            {
                                m_Player.Skills[skillName].CapFixedPoint += 10;

                                m_Player.FixedParticles(0x373A, 10, 15, 5036, EffectLayer.Head);
                                m_Player.PlaySound(0x3BD);

                                m_Player.SendMessage(63, "You increase your maximum skill cap in " + skillNameText + " by 1.");

                                m_SkillMasteryScroll.Delete();
                                
                                m_Player.CloseGump(typeof(SkillMasteryGump));
                                m_Player.SendGump(new SkillMasteryGump(m_Player, SkillMasteryPageType.Overview, null));

                                return;
                            }

                            else
                            {
                                m_Player.SendMessage("You are already at the maximum skill cap for " + skillNameText + ".");
                                return;
                            }
                        break;
                    }

                    return;
                break;
            }

            if (closeGump)            
                m_Player.SendSound(0x058);

            else
            {
                m_Player.CloseGump(typeof(SkillMasteryGump));
                m_Player.SendGump(new SkillMasteryGump(m_Player, m_SkillMasteryPage, m_Item));
            }
                         
        }
    }
}