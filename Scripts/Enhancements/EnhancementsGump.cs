using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server
{
    public class EnhancementsGump : Gump
    {
        public enum PageType
        {
            Customizations,
            SpellHues,
            Emotes
        }

        public PlayerMobile m_Player;
        public EnhancementsGumpObject m_EnhancementsGumpObject;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public int EntriesPerCustomizationPage = 6;
        public int EntriesPerSpellHuePage = 6;
        public int EntriesPerEmotePage = 8;

        public int WhiteTextHue = 2499;
        
        public EnhancementsGump(PlayerMobile player, EnhancementsGumpObject enhancementsGumpObject): base(10, 10)
        {
            m_Player = player;
            m_EnhancementsGumpObject = enhancementsGumpObject;

            if (m_Player == null) return;
            if (m_EnhancementsGumpObject == null) return;

            EnhancementsPersistance.CheckAndCreateEnhancementsAccountEntry(m_Player);

            List<Enhancements.CustomizationEntry> m_Customizations = m_Player.m_EnhancementsAccountEntry.m_Customizations;
            List<Enhancements.SpellHueEntry> m_SpellHues = m_Player.m_EnhancementsAccountEntry.m_SpellHues;
            List<Enhancements.EmoteEntry> m_Emotes = m_Player.m_EnhancementsAccountEntry.m_Emotes;

            #region Background 

            AddImage(148, 110, 103);
            AddImage(149, 153, 103);
            AddImage(149, 239, 103);
            AddImage(149, 329, 103);
            AddImage(151, 415, 103, 2401);
            AddImage(14, 415, 103, 2401);
            AddImage(14, 238, 103, 2401);
            AddImage(148, 13, 103, 2401);
            AddImage(13, 152, 103, 2401);
            AddImage(14, 12, 103, 2401);
            AddImage(14, 109, 103, 2401);
            AddImage(23, 112, 3604, 2052);
            AddImage(149, 112, 3604, 2052);
            AddImage(23, 23, 3604, 2052);
            AddImage(149, 23, 3604, 2052);           
            AddImage(14, 328, 103, 2401);
            AddImage(23, 199, 3604, 2052);
            AddImage(149, 199, 3604, 2052);
            AddImage(23, 291, 3604, 2052);
            AddImage(149, 291, 3604, 2052);
            AddImage(23, 377, 3604, 2052);
            AddImage(149, 377, 3604, 2052);
            AddImage(280, 110, 103);
            AddImage(281, 153, 103);
            AddImage(281, 239, 103);
            AddImage(281, 329, 103);
            AddImage(282, 415, 103, 2401);
            AddImage(280, 13, 103, 2401);
            AddImage(412, 110, 103, 2401);
            AddImage(413, 153, 103, 2401);
            AddImage(413, 239, 103, 2401);
            AddImage(413, 329, 103, 2401);
            AddImage(414, 415, 103, 2401);
            AddImage(412, 13, 103, 2401);
            AddImage(273, 112, 3604, 2052);
            AddImage(273, 23, 3604, 2052);
            AddImage(273, 199, 3604, 2052);
            AddImage(273, 291, 3604, 2052);
            AddImage(273, 376, 3604, 2052);
            AddImage(381, 112, 3604, 2052);
            AddImage(381, 23, 3604, 2052);
            AddImage(381, 199, 3604, 2052);
            AddImage(381, 291, 3604, 2052);
            AddImage(381, 376, 3604, 2052);
            AddImage(415, 112, 3604, 2052);
            AddImage(415, 23, 3604, 2052);
            AddImage(415, 199, 3604, 2052);
            AddImage(415, 291, 3604, 2052);
            AddImage(415, 377, 3604, 2052);          

            #endregion

            //Header
            AddImage(150, 3, 1143, 2499);
            AddLabel(239, 5, 2590, "Enhancements");

            //Guide            
            AddButton(13, 9, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(7, -1, 149, "Guide");

            #region Footers

            #region Images
            
            AddImage(23, 457, 96, 2401);
            AddImage(178, 457, 96, 2401);
            AddImage(245, 457, 96, 2401);
            AddImage(23, 456, 96, 2401);
            AddImage(178, 456, 96, 2401);
            AddImage(245, 456, 96, 2401);
            AddImage(296, 457, 96, 2401);
            AddImage(364, 457, 96, 2401);
            AddImage(296, 456, 96, 2401);
            AddImage(364, 456, 96, 2401);
            AddItem(411, 465, 4810, 2500);
            AddItem(210, 371, 14094, 2963);
            AddItem(26, 458, 8312, 2500);          

            #endregion
                       
            if (m_EnhancementsGumpObject.m_PageType == PageType.Customizations)
                AddButton(64, 468, 9724, 9721, 2, GumpButtonType.Reply, 0);
            else
                AddButton(64, 468, 9721, 9724, 2, GumpButtonType.Reply, 0);
            AddLabel(98, 476, 2603, "Customizations");
           
            if (m_EnhancementsGumpObject.m_PageType == PageType.SpellHues)
                AddButton(270, 470, 9724, 9721, 3, GumpButtonType.Reply, 0);
            else
                AddButton(270, 470, 9721, 9724, 3, GumpButtonType.Reply, 0);
            AddLabel(304, 473, 2606, "Spell Hues");
           
            if (m_EnhancementsGumpObject.m_PageType == PageType.Emotes)
                AddButton(444, 470, 9724, 9721, 4, GumpButtonType.Reply, 0);
            else
                AddButton(444, 470, 9721, 9724, 4, GumpButtonType.Reply, 0);
            AddLabel(480, 473, 2599, "Emotes");

            #endregion

            switch (m_EnhancementsGumpObject.m_PageType)
            {
                #region Customizations

                case PageType.Customizations:
                    int customizationStartIndex = m_EnhancementsGumpObject.m_Page * EntriesPerCustomizationPage;
                    int customizationCount = Enum.GetNames(typeof(Enhancements.CustomizationType)).Length;

                    int totalCustomizationPages = (int)(Math.Ceiling((double)customizationCount / (double)EntriesPerCustomizationPage));

                    if (m_EnhancementsGumpObject.m_Page < 0)
                        m_EnhancementsGumpObject.m_Page = 0;

                    if (m_EnhancementsGumpObject.m_Page >= totalCustomizationPages)
                        m_EnhancementsGumpObject.m_Page = totalCustomizationPages - 1;

                    int leftStartX = 35;
                    int leftStartY = 35;
                    
                    int rightStartX = 300;
                    int rightStartY = 35;

                    int rowSpacing = 130;

                    for (int a = 0; a < EntriesPerCustomizationPage; a++)
                    {
                        int customizationIndex = customizationStartIndex + a;

                        if (customizationIndex >= customizationCount)
                            continue;

                        Enhancements.CustomizationType customizationType = (Enhancements.CustomizationType)customizationIndex;
                        Enhancements.CustomizationEntry customizationEntry = Enhancements.GetCustomizationEntry(m_Player, customizationType);
                        Enhancements.CustomizationDetail customizationDetail = Enhancements.GetCustomizationDetail(customizationType);

                        if (customizationEntry == null)
                            customizationEntry = new Enhancements.CustomizationEntry(customizationType, false, false);

                         List<Enhancements.SpellHueType> m_AvailableHues = new List<Enhancements.SpellHueType>();

                        m_AvailableHues.Add(Enhancements.SpellHueType.Standard);

                        //Left Side
                        if (a < (EntriesPerCustomizationPage / 2))
                        {
                            if (customizationEntry.m_Unlocked)
                            {
                                if (customizationEntry.m_Active)
                                {
                                    AddLabel(leftStartX + 129, leftStartY + 16, 63, customizationDetail.m_CustomizationName);
                                    AddLabel(leftStartX + 156, leftStartY + 74, 63, "Active");

                                    AddButton(leftStartX + 121, leftStartY + 71, 2154, 2151, 10 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(leftStartX + 129, leftStartY + 16, 149, customizationDetail.m_CustomizationName);
                                    AddLabel(leftStartX + 156, leftStartY + 74, 149, "Inactive");

                                    AddButton(leftStartX + 121, leftStartY + 71, 2151, 2154, 10 + a, GumpButtonType.Reply, 0);
                                }
                            }

                            else
                            {
                                AddLabel(leftStartX + 129, leftStartY + 16, 149, customizationDetail.m_CustomizationName);
                                AddLabel(leftStartX + 156, leftStartY + 74, 2401, "Not Acquired");

                                AddButton(leftStartX + 121, leftStartY + 71, 9721, 9721, 10 + a, GumpButtonType.Reply, 0);
                            }                            

                            AddBackground(leftStartX + 0, leftStartY + 0, 122, 122, 9270);
                            AddImageTiled(leftStartX + 12, leftStartY + 12, 100, 100, 2624);                                                     
                            AddLabel(leftStartX + 156, leftStartY + 41, 2550, "Info");
                            AddButton(leftStartX + 129, leftStartY + 44, 2117, 2118, 20 + a, GumpButtonType.Reply, 0);

                            AddGumpCollection(GumpCollections.GetGumpCollection(customizationDetail.GumpCollectionId, -1), leftStartX, leftStartY);

                            leftStartY += rowSpacing;
                        }

                        //Right Side
                        else
                        {
                            if (customizationEntry.m_Unlocked)
                            {
                                if (customizationEntry.m_Active)
                                {
                                    AddLabel(rightStartX + 129, rightStartY + 16, 63, customizationDetail.m_CustomizationName);
                                    AddLabel(rightStartX + 156, rightStartY + 74, 63, "Active");

                                    AddButton(rightStartX + 121, rightStartY + 71, 2154, 2151, 10 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(rightStartX + 129, rightStartY + 16, 149, customizationDetail.m_CustomizationName);
                                    AddLabel(rightStartX + 156, rightStartY + 74, 149, "Inactive");

                                    AddButton(rightStartX + 121, rightStartY + 71, 2151, 2154, 10 + a, GumpButtonType.Reply, 0);
                                }
                            }

                            else
                            {
                                AddLabel(rightStartX + 129, rightStartY + 16, 149, customizationDetail.m_CustomizationName);
                                AddLabel(rightStartX + 156, rightStartY + 74, 2401, "Not Acquired");

                                AddButton(rightStartX + 121, rightStartY + 71, 9721, 9721, 10 + a, GumpButtonType.Reply, 0);
                            }

                            AddBackground(rightStartX + 0, rightStartY + 0, 122, 122, 9270);
                            AddImageTiled(rightStartX + 12, rightStartY + 12, 100, 100, 2624);
                            AddLabel(rightStartX + 156, rightStartY + 41, 2550, "Info");
                            AddButton(rightStartX + 129, rightStartY + 44, 2117, 2118, 20 + a, GumpButtonType.Reply, 0);

                            AddGumpCollection(GumpCollections.GetGumpCollection(customizationDetail.GumpCollectionId, -1), rightStartX, rightStartY);

                            rightStartY += rowSpacing;
                        }
                    }

                    if (m_EnhancementsGumpObject.m_Page > 0)
                    {
                        AddButton(37, 428, 4014, 4016, 5, GumpButtonType.Reply, 0);
                        AddLabel(71, 429, WhiteTextHue, "Previous Page");
                    }

                    if (m_EnhancementsGumpObject.m_Page < totalCustomizationPages - 1)
                    {
                        AddButton(497, 428, 4005, 4007, 6, GumpButtonType.Reply, 0);
                        AddLabel(427, 429, WhiteTextHue, "Next Page");
                    }

                break;

                #endregion

                #region Spell Hues

                case PageType.SpellHues:
                    int spellHuesStartIndex = m_EnhancementsGumpObject.m_Page * EntriesPerSpellHuePage;
                    int spellHuesCount = Enum.GetNames(typeof(Enhancements.SpellType)).Length;

                    int totalSpellHuesPages = (int)(Math.Ceiling((double)spellHuesCount / (double)EntriesPerSpellHuePage));

                    if (m_EnhancementsGumpObject.m_Page < 0)
                        m_EnhancementsGumpObject.m_Page = 0;

                    if (m_EnhancementsGumpObject.m_Page >= totalSpellHuesPages)
                        m_EnhancementsGumpObject.m_Page = totalSpellHuesPages - 1;
                                        
                    leftStartX = 65;
                    leftStartY = 65;
                    
                    rightStartX = 325;
                    rightStartY = 65;

                    rowSpacing = 130;

                    for (int a = 0; a < EntriesPerSpellHuePage; a++)
                    {
                        int spellHueIndex = spellHuesStartIndex + a;

                        if (spellHueIndex >= spellHuesCount)
                            continue;

                        Enhancements.SpellType spellType = (Enhancements.SpellType)spellHueIndex;
                        Enhancements.SpellHueEntry spellHueEntry = Enhancements.GetSpellHueEntry(m_Player, spellType);
                        Enhancements.SpellHueDetail spellHueDetail = Enhancements.GetSpellHueDetail(spellType);

                        if (spellHueEntry == null)
                            spellHueEntry = new Enhancements.SpellHueEntry(spellType);

                        Enhancements.SpellHueTypeDetail spellHueTypeDetail = Enhancements.GetSpellHueTypeDetail(spellHueEntry.m_SelectedHue);

                        List<Enhancements.SpellHueType> m_AvailableSpellHues = new List<Enhancements.SpellHueType>();

                        m_AvailableSpellHues.Add(Enhancements.SpellHueType.Standard);

                        for (int b = 0; b < spellHueEntry.m_UnlockedHues.Count; b++)
                        {
                            if (spellHueEntry.m_UnlockedHues[b] == null)
                                continue;

                            m_AvailableSpellHues.Add(spellHueEntry.m_UnlockedHues[b]);
                        }

                        int spellHueTypeIndex = -1;

                        spellHueTypeIndex = m_AvailableSpellHues.IndexOf(spellHueEntry.m_SelectedHue);

                        int spellHue = spellHueTypeDetail.m_Hue;

                        if (spellHue == 0)
                            spellHue = 2499;

                        //Left Side
                        if (a < (EntriesPerSpellHuePage / 2))
                        {
                            if (m_AvailableSpellHues.Count > 1)
                            {
                                if (spellHueTypeIndex > 0)
                                    AddButton(leftStartX + 5, leftStartY + 66, 9909, 2151, 10 + a, GumpButtonType.Reply, 0);

                                if (spellHueTypeIndex < m_AvailableSpellHues.Count - 1)
                                    AddButton(leftStartX + 40, leftStartY + 65, 9903, 2151, 20 + a, GumpButtonType.Reply, 0);
                            }

                            AddBackground(leftStartX + 0, leftStartY + 0, 64, 61, 9270);
                            AddItem(leftStartX + 10, leftStartY + 9, spellHueDetail.m_ItemID, spellHueTypeDetail.m_Hue);

                            AddLabel(leftStartX + 66, leftStartY + -1, 149, spellHueDetail.m_SpellName);
                            AddLabel(leftStartX + 66, leftStartY + 19, spellHue, spellHueTypeDetail.m_SpellHueTypeName);
                            AddLabel(leftStartX + 66, leftStartY + 39, WhiteTextHue, "Hue ");
                            AddLabel(leftStartX + 96, leftStartY + 39, spellHue, spellHueTypeDetail.m_Hue.ToString());

                            if (m_AvailableSpellHues.Count > 1)
                                AddLabel(leftStartX + 66, leftStartY + 66, WhiteTextHue, (spellHueTypeIndex + 1).ToString() + " / " + m_AvailableSpellHues.Count.ToString());

                            leftStartY += rowSpacing;
                        }

                        //Right Side
                        else
                        {
                            if (m_AvailableSpellHues.Count > 1)
                            {
                                if (spellHueTypeIndex > 0)
                                    AddButton(rightStartX + 5, rightStartY + 66, 9909, 2151, 10 + a, GumpButtonType.Reply, 0);

                                if (spellHueTypeIndex < m_AvailableSpellHues.Count - 1)
                                    AddButton(rightStartX + 40, rightStartY + 65, 9903, 2151, 20 + a, GumpButtonType.Reply, 0);
                            }

                            AddBackground(rightStartX + 0, rightStartY + 0, 64, 61, 9270);
                            AddItem(rightStartX + 10, rightStartY + 9, spellHueDetail.m_ItemID, spellHueTypeDetail.m_Hue);

                            AddLabel(rightStartX + 66, rightStartY + -1, 149, spellHueDetail.m_SpellName);
                            AddLabel(rightStartX + 66, rightStartY + 19, spellHue, spellHueTypeDetail.m_SpellHueTypeName);
                            AddLabel(rightStartX + 66, rightStartY + 39, WhiteTextHue, "Hue ");
                            AddLabel(rightStartX + 96, rightStartY + 39, spellHue, spellHueTypeDetail.m_Hue.ToString());

                            if (m_AvailableSpellHues.Count > 1)
                                AddLabel(rightStartX + 66, rightStartY + 66, WhiteTextHue, (spellHueTypeIndex + 1).ToString() + " / " + m_AvailableSpellHues.Count.ToString());

                            rightStartY += rowSpacing;
                        }
                    }

                    if (m_EnhancementsGumpObject.m_Page > 0)
                    {
                        AddButton(37, 428, 4014, 4016, 5, GumpButtonType.Reply, 0);
                        AddLabel(71, 429, WhiteTextHue, "Previous Page");
                    }

                    if (m_EnhancementsGumpObject.m_Page < totalSpellHuesPages - 1)
                    {
                        AddButton(497, 428, 4005, 4007, 6, GumpButtonType.Reply, 0);
                        AddLabel(427, 429, WhiteTextHue, "Next Page");
                    }
                break;

                #endregion

                #region Emotes

                case PageType.Emotes:
                    int emoteStartIndex = m_EnhancementsGumpObject.m_Page * EntriesPerEmotePage;
                    int emoteCount = Enum.GetNames(typeof(Enhancements.EmoteType)).Length;

                    int totalEmotesPages = (int)(Math.Ceiling((double)emoteCount / (double)EntriesPerEmotePage));

                    if (m_EnhancementsGumpObject.m_Page < 0)
                        m_EnhancementsGumpObject.m_Page = 0;

                    if (m_EnhancementsGumpObject.m_Page >= totalEmotesPages)
                        m_EnhancementsGumpObject.m_Page = totalEmotesPages - 1;
                    
                    leftStartX = 75;
                    leftStartY = 35;

                    rightStartX = 320;
                    rightStartY = 35;

                    rowSpacing = 100;

                    for (int a = 0; a < EntriesPerEmotePage; a++)
                    {
                        int emoteIndex = emoteStartIndex + a;

                        if (emoteIndex >= emoteCount)
                            continue;

                        Enhancements.EmoteType emoteType = (Enhancements.EmoteType)emoteIndex;
                        Enhancements.EmoteEntry emoteEntry = Enhancements.GetEmoteEntry(m_Player, emoteType);
                        Enhancements.EmoteDetail emoteDetail = Enhancements.GetEmoteDetail(emoteType);

                        if (emoteEntry == null)
                            emoteEntry = new Enhancements.EmoteEntry(emoteType, false);                        

                        //Left Side
                        if (a < (EntriesPerEmotePage / 2))
                        {
                            AddLabel(leftStartX + 89, leftStartY + 17, emoteDetail.m_Hue, emoteDetail.m_EmoteName);

                            if (emoteEntry.m_Unlocked)
                            {
                                AddImage(leftStartX + 0, leftStartY + 0, 1417, 63);
                                AddLabel(leftStartX + 89, leftStartY + 37, 63, "Unlocked");
                            }

                            else
                            {
                                AddImage(leftStartX + 0, leftStartY + 0, 1417, 0);
                                AddLabel(leftStartX + 89, leftStartY + 37, 2401, "Not Acquired");
                            }

                            AddImage(leftStartX + 10, leftStartY + 9, 5576, emoteDetail.m_Hue);

                            leftStartY += rowSpacing;
                        }

                        //Right Side
                        else
                        {
                            AddLabel(rightStartX + 89, rightStartY + 17, emoteDetail.m_Hue, emoteDetail.m_EmoteName);

                            if (emoteEntry.m_Unlocked)
                            {
                                AddImage(rightStartX + 0, rightStartY + 0, 1417, 63);
                                AddLabel(rightStartX + 89, rightStartY + 37, 63, "Unlocked");
                            }

                            else
                            {
                                AddImage(rightStartX + 0, rightStartY + 0, 1417, 0);
                                AddLabel(rightStartX + 89, rightStartY + 37, 2401, "Not Acquired");
                            }

                            AddImage(rightStartX + 10, rightStartY + 9, 5576, emoteDetail.m_Hue);

                            rightStartY += rowSpacing;
                        }
                    }

                    if (m_EnhancementsGumpObject.m_Page > 0)
                    {
                        AddButton(37, 428, 4014, 4016, 5, GumpButtonType.Reply, 0);
                        AddLabel(71, 429, WhiteTextHue, "Previous Page");
                    }

                    if (m_EnhancementsGumpObject.m_Page < totalEmotesPages - 1)
                    {
                        AddButton(497, 428, 4005, 4007, 6, GumpButtonType.Reply, 0);
                        AddLabel(427, 429, WhiteTextHue, "Next Page");
                    }
                break;

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_EnhancementsGumpObject == null) return;

            bool closeGump = true;

            #region Footers 

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Customizations
                case 2:
                    m_EnhancementsGumpObject.m_PageType = PageType.Customizations;
                    m_EnhancementsGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Spell Hues
                case 3:
                    m_EnhancementsGumpObject.m_PageType = PageType.SpellHues;
                    m_EnhancementsGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Emotes
                case 4:
                    m_EnhancementsGumpObject.m_PageType = PageType.Emotes;
                    m_EnhancementsGumpObject.m_Page = 0;

                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;
            }

            #endregion
            
            switch (m_EnhancementsGumpObject.m_PageType)
            {
                #region Customizations

                case PageType.Customizations:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 5:
                            m_EnhancementsGumpObject.m_Page--;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 6:
                            m_EnhancementsGumpObject.m_Page++;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10 && info.ButtonID < 20)
                    {
                        int customizationIndex = (m_EnhancementsGumpObject.m_Page * EntriesPerCustomizationPage) + info.ButtonID - 10;
                        int customizationCount = Enum.GetNames(typeof(Enhancements.CustomizationType)).Length;

                        if (customizationIndex < customizationCount)
                        {
                            Enhancements.CustomizationType customizationType = (Enhancements.CustomizationType)customizationIndex;
                            Enhancements.CustomizationEntry customizationEntry = Enhancements.GetCustomizationEntry(m_Player, customizationType);

                            if (customizationEntry == null)
                                customizationEntry = new Enhancements.CustomizationEntry(customizationType, false, false);

                            if (customizationEntry.m_Unlocked)
                            {
                                customizationEntry.m_Active = !customizationEntry.m_Active;

                                m_Player.SendSound(LargeSelectionSound);
                            }
                        }

                        closeGump = false;
                    }

                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                        int customizationIndex = (m_EnhancementsGumpObject.m_Page * EntriesPerCustomizationPage) + info.ButtonID - 10;
                        int customizationCount = Enum.GetNames(typeof(Enhancements.CustomizationType)).Length;

                        if (customizationIndex < customizationCount)
                        {
                            Enhancements.CustomizationType customizationType = (Enhancements.CustomizationType)customizationIndex;
                            Enhancements.CustomizationDetail customizationDetail = Enhancements.GetCustomizationDetail(customizationType);

                            m_Player.SendMessage(2590, customizationDetail.m_Description);
                        }

                        closeGump = false;
                    }                    
                break;

                #endregion

                #region Spell Hues

                case PageType.SpellHues:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 5:
                            m_EnhancementsGumpObject.m_Page--;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 6:
                            m_EnhancementsGumpObject.m_Page++;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }

                    int spellHuesCount = Enum.GetNames(typeof(Enhancements.CustomizationType)).Length;
                    
                    //Previous Hue
                    if (info.ButtonID >= 10 && info.ButtonID < 20)
                    {
                        int spellHueIndex = (m_EnhancementsGumpObject.m_Page * EntriesPerSpellHuePage) + info.ButtonID - 10;

                        if (spellHueIndex < spellHuesCount)
                        {
                            Enhancements.SpellType spellType = (Enhancements.SpellType)spellHueIndex;
                            Enhancements.SpellHueEntry spellHueEntry = Enhancements.GetSpellHueEntry(m_Player, spellType);

                            if (spellHueEntry == null)
                                spellHueEntry = new Enhancements.SpellHueEntry(spellType);

                            List<Enhancements.SpellHueType> m_AvailableHues = new List<Enhancements.SpellHueType>();

                            m_AvailableHues.Add(Enhancements.SpellHueType.Standard);

                            for (int a = 0; a < spellHueEntry.m_UnlockedHues.Count; a++)
                            {
                                if (spellHueEntry.m_UnlockedHues[a] == null)
                                    continue;

                                m_AvailableHues.Add(spellHueEntry.m_UnlockedHues[a]);
                            }

                            if (m_AvailableHues.Count > 0)
                            {
                                spellHueIndex = m_AvailableHues.IndexOf(spellHueEntry.m_SelectedHue);
                                                               
                                spellHueIndex--;                                

                                if (spellHueIndex < 0)
                                    spellHueIndex = m_AvailableHues.Count - 1;

                                else if (spellHueIndex >= m_AvailableHues.Count)
                                    spellHueIndex = 0;

                                spellHueEntry.m_SelectedHue = m_AvailableHues[spellHueIndex];

                                m_Player.SendSound(SelectionSound);
                            }
                        }

                        closeGump = false;
                    }

                    //Next Hue
                    if (info.ButtonID >= 202 && info.ButtonID < 30)
                    {
                        int spellHueIndex = (m_EnhancementsGumpObject.m_Page * EntriesPerSpellHuePage) + info.ButtonID - 20;

                        if (spellHueIndex < spellHuesCount)
                        {
                            Enhancements.SpellType spellType = (Enhancements.SpellType)spellHueIndex;
                            Enhancements.SpellHueEntry spellHueEntry = Enhancements.GetSpellHueEntry(m_Player, spellType);

                            if (spellHueEntry == null)
                                spellHueEntry = new Enhancements.SpellHueEntry(spellType);

                            List<Enhancements.SpellHueType> m_AvailableHues = new List<Enhancements.SpellHueType>();

                            m_AvailableHues.Add(Enhancements.SpellHueType.Standard);

                            for (int a = 0; a < spellHueEntry.m_UnlockedHues.Count; a++)
                            {
                                if (spellHueEntry.m_UnlockedHues[a] == null)
                                    continue;

                                m_AvailableHues.Add(spellHueEntry.m_UnlockedHues[a]);
                            }

                            if (m_AvailableHues.Count > 0)
                            {
                                spellHueIndex = m_AvailableHues.IndexOf(spellHueEntry.m_SelectedHue);

                                spellHueIndex++;

                                if (spellHueIndex < 0)
                                    spellHueIndex = m_AvailableHues.Count - 1;

                                else if (spellHueIndex >= m_AvailableHues.Count)
                                    spellHueIndex = 0;

                                spellHueEntry.m_SelectedHue = m_AvailableHues[spellHueIndex];

                                m_Player.SendSound(SelectionSound);
                            }
                        }

                        closeGump = false;
                    }
                break;

                #endregion

                #region Emotes

                case PageType.Emotes:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 5:
                            m_EnhancementsGumpObject.m_Page--;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 6:
                            m_EnhancementsGumpObject.m_Page++;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }
                break;

                #endregion
            }
            
            
            if (!closeGump)
            {
                m_Player.CloseGump(typeof(EnhancementsGump));
                m_Player.SendGump(new EnhancementsGump(m_Player, m_EnhancementsGumpObject));
            }   
      
            else
                m_Player.SendSound(CloseGumpSound);
        }
    }

    public class EnhancementsGumpObject
    {
        public PlayerMobile m_Player;

        public EnhancementsGump.PageType m_PageType = EnhancementsGump.PageType.Customizations;
        public int m_Page = 0;

        public EnhancementsGumpObject(PlayerMobile player)
        {
            m_Player = player;
        }
    }
}