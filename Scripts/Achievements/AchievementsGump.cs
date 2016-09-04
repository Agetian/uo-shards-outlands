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
    public class AchievementsGump : Gump
    {
        public enum PageType
        {
            Main,
            Category,
            Settings
        }

        public PlayerMobile m_Player;        
        public PageType m_SelectedPageType = PageType.Main;

        //Achievement Main Page
        public int m_MainPage = 0;

        public static int categoryRows = 3;
        public static int categoryColumns = 4;       

        //Achievement Category Page
        public AchievementCategory m_AchievementCategory = AchievementCategory.Battle;
        public int m_AchievementListPage = 0;
        public int m_AchievementSelectedIndex = 0;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public AchievementsGump(Mobile from, PageType selectedPageType, int mainPage, AchievementCategory achievementCategory, int achievementListPage, int achievementSelectedIndex): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(m_Player);

            m_SelectedPageType = selectedPageType;
            m_MainPage = mainPage;
            m_AchievementCategory = achievementCategory;
            m_AchievementListPage = achievementListPage;
            m_AchievementSelectedIndex = achievementSelectedIndex;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 1102;

            int startX = 0;
            int startY = 0;

            AddPage(0);

            int totalPages;

            #region Background

            AddImage(409, 8, 103, 2401);
            AddImage(142, 105, 103);
            AddImage(143, 148, 103);
            AddImage(143, 234, 103);
            AddImage(143, 324, 103);
            AddImage(145, 410, 103, 2401);
            AddImage(8, 410, 103, 2401);
            AddImage(8, 233, 103, 2401);
            AddImage(142, 8, 103, 2401);
            AddImage(7, 147, 103, 2401);
            AddImage(8, 7, 103, 2401);
            AddImage(8, 104, 103, 2401);
            AddImage(17, 107, 3604, 2052);
            AddImage(143, 107, 3604, 2052);
            AddImage(17, 18, 3604, 2052);
            AddImage(143, 18, 3604, 2052);            
            AddImage(8, 323, 103, 2401);
            AddImage(17, 194, 3604, 2052);
            AddImage(143, 194, 3604, 2052);
            AddImage(17, 286, 3604, 2052);
            AddImage(143, 286, 3604, 2052);
            AddImage(17, 370, 3604, 2052);
            AddImage(143, 370, 3604, 2052);
            AddImage(274, 105, 103);
            AddImage(275, 148, 103);
            AddImage(275, 234, 103);
            AddImage(275, 324, 103);
            AddImage(276, 410, 103, 2401);
            AddImage(274, 8, 103, 2401);
            AddImage(409, 91, 103, 2401);
            AddImage(409, 182, 103, 2401);
            AddImage(409, 261, 103, 2401);
            AddImage(409, 354, 103, 2401);
            AddImage(409, 410, 103, 2401);
            AddImage(267, 107, 3604, 2052);
            AddImage(267, 18, 3604, 2052);
            AddImage(267, 194, 3604, 2052);
            AddImage(267, 286, 3604, 2052);
            AddImage(267, 370, 3604, 2052);
            AddImage(375, 107, 3604, 2052);
            AddImage(375, 18, 3604, 2052);
            AddImage(375, 194, 3604, 2052);
            AddImage(375, 286, 3604, 2052);
            AddImage(375, 370, 3604, 2052);
            AddImage(411, 107, 3604, 2052);
            AddImage(411, 18, 3604, 2052);
            AddImage(411, 194, 3604, 2052);
            AddImage(411, 286, 3604, 2052);
            AddImage(412, 370, 3604, 2052);

            #endregion

            //Header
            AddImage(144, -2, 1143, 2499);
            AddLabel(235, 0, 2606, "Achievements");

            //Guide
            AddButton(7, 8, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(3, -2, 149, "Guide");

            switch (m_SelectedPageType)
            {
                #region Main

                case PageType.Main:
                    int categoriesPerPage = categoryRows * categoryColumns;
                    int totalCategories = Enum.GetNames(typeof(AchievementCategory)).Length;
                    int totalCategoryPages = (int)(Math.Ceiling((double)totalCategories / (double)categoriesPerPage));

                    if (m_MainPage >= totalCategoryPages)
                        m_MainPage = 0;

                    if (m_MainPage < 0)
                        m_MainPage = 0;

                    int categoryStartIndex = m_MainPage * categoriesPerPage;
                    int categoryEndIndex = (m_MainPage * categoriesPerPage) + (categoriesPerPage - 1);

                    if (categoryEndIndex >= totalCategories)
                        categoryEndIndex = totalCategories - 1;

                    int totalCompletedAchievements = Achievements.GetCompletedAchievementCount(m_Player);
                    int totalAchievementsAvailable = Achievements.GetTotalAchievementsCount();
                    
                    //Previous
                    if (m_MainPage > 0)
                    {
                        AddLabel(57, 472, WhiteTextHue, "Previous Page");
                        AddButton(23, 471, 4014, 4016, 2, GumpButtonType.Reply, 0);
                    }

                    //Next
                    if (m_MainPage < totalCategoryPages - 1)
                    {
                        AddButton(502, 471, 4005, 4007, 3, GumpButtonType.Reply, 0);
                        AddLabel(432, 472, WhiteTextHue, "Next Page");
                    }

                    //Settings
                    AddButton(243, 471, 4029, 4031, 4, GumpButtonType.Reply, 0);
                    AddLabel(277, 472, 63, "Settings");

                    int textHue = GreyTextHue;

                    if (totalCompletedAchievements > 0)
                        textHue = YellowTextHue;

                    if (totalCompletedAchievements == totalAchievementsAvailable)
                        textHue = GreenTextHue;

                    AddLabel(175, 447, 149, "Total");
                    AddLabel(332, 447, textHue, totalCompletedAchievements.ToString() + "/" + totalAchievementsAvailable.ToString());
                                        
                    double totalAchievementProgress = (double)totalCompletedAchievements / (double)totalAchievementsAvailable;
                    
                    AddImage(216, 452, Achievements.GetProgressBarBackground(totalAchievementProgress));
                    AddImageTiled(216 + Utility.ProgressBarX(totalAchievementProgress), 455, Utility.ProgressBarWidth(totalAchievementProgress), 7, 2488);
                    
                    int iBaseX = 25;
                    int iBaseY = 30;

                    startX = iBaseX;
                    startY = iBaseY;

                    int columnIndex = 0;
                    int rowIndex = 0;

                    int rowSpacing = 140;
                    int columnSpacing = 125;
            
                    int categoryDisplayCount = categoryEndIndex - categoryStartIndex;

                    for (int a = 0; a < categoryDisplayCount + 1; a++)
                    {
                        int categoryIndex = categoryStartIndex + a;
                        int buttonIndex = 10 + categoryIndex;
                        
                        if (categoryStartIndex >= totalCategories)
                            continue;

                        AchievementCategory category = (AchievementCategory)categoryIndex;
                        AchievementCategoryDetail achievementCategoryDetail = Achievements.GetCategoryDetail(category);

                        AddGumpCollection(GumpCollections.GetGumpCollection(achievementCategoryDetail.GumpCollectionGroupId, achievementCategoryDetail.GumpCollectionItemId), startX - 40, startY + 8);

                        AddLabel(Utility.CenteredTextOffset(startX + 65, achievementCategoryDetail.m_CategoryName), startY + 5, achievementCategoryDetail.m_TextHue, achievementCategoryDetail.m_CategoryName);
                        AddImage(startX + 60, startY + 32, 10550, achievementCategoryDetail.m_IconHue);
                        AddImage(startX + 88, startY + 32, 10552, achievementCategoryDetail.m_IconHue);
                        AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                        
                        //Category Progress
                        int completedAchievementsInCategory = Achievements.GetCompletedCategoryAchievements(m_Player, category);
                        int achievementsInCategory = Achievements.GetCategoryAchievements(category).Count;

                        textHue = GreyTextHue;

                        if (completedAchievementsInCategory > 0)
                            textHue = YellowTextHue;

                        if (completedAchievementsInCategory == achievementsInCategory)
                            textHue = GreenTextHue;

                        double categoryAchievementProgress = (double)completedAchievementsInCategory / (double)achievementsInCategory;

                        string progressText = completedAchievementsInCategory.ToString() + "/" + achievementsInCategory.ToString();

                        AddImage(startX + 10, startY + 99, Achievements.GetProgressBarBackground(totalAchievementProgress));
                        AddImageTiled(startX + 10 + Utility.ProgressBarX(totalAchievementProgress), startY + 102, Utility.ProgressBarWidth(totalAchievementProgress), 7, 2488);
                        AddLabel(Utility.CenteredTextOffset(startX + 60, progressText), startY + 113, textHue, progressText);

                        startX += columnSpacing;
                        columnIndex++;

                        if (columnIndex >= categoryColumns)
                        {
                            columnIndex = 0;
                            rowIndex++;

                            startX = iBaseX;
                            startY += rowSpacing;
                        }
                    }
                break;

                #endregion

                #region Category

                case PageType.Category:
                    List<Achievement> m_AchievementsInCategory = Achievements.GetCategoryAchievements(m_AchievementCategory);
                    
                    AchievementCategoryDetail categoryDetail = Achievements.GetCategoryDetail(m_AchievementCategory);

                    if (m_AchievementsInCategory.Count == 0)
                        return;
                    
                    if (m_AchievementSelectedIndex >= m_AchievementsInCategory.Count)
                        m_AchievementSelectedIndex = 0;

                    Achievement achievementSelected = m_AchievementsInCategory[m_AchievementSelectedIndex];                    

                    int achievementsPerPage = 12;
                    int totalAchievements = m_AchievementsInCategory.Count;
                    int totalAchievementPages = (int)(Math.Ceiling((double)totalAchievements / (double)achievementsPerPage));

                    if (m_AchievementListPage >= totalAchievementPages)
                        m_AchievementListPage = 0;

                    if (m_AchievementListPage < 0)
                        m_AchievementListPage = 0;

                    int achievementStartIndex = m_AchievementListPage * achievementsPerPage;
                    int achievementEndIndex = (m_AchievementListPage * achievementsPerPage) + (achievementsPerPage - 1);

                    if (achievementEndIndex >= totalAchievements)
                        achievementEndIndex = totalAchievements - 1;
                                
                    int achievementCount = achievementEndIndex - achievementStartIndex;
                    
                    //Return to Categories
                    AddButton(23, 471, 4014, 4016, 5, GumpButtonType.Reply, 0);
                    AddLabel(57, 472, 149, "Categories");

                    //Previous List Page
                    if (m_AchievementListPage > 0)
                        AddButton(172, 471, 9909, 9909, 3, GumpButtonType.Reply, 0); 

                    if (m_AchievementListPage > 0 || m_AchievementListPage < totalAchievementPages - 1)
                        AddLabel(201, 472, 149, "More");

                    //Next List Page
                    if (m_AchievementListPage < totalAchievementPages - 1)
                        AddButton(239, 471, 9903, 9903, 4, GumpButtonType.Reply, 0);      

                    //Category
                    startX = 25;
                    startY = 30;                                        

                    int playerCompletedAchievementsInCategory = Achievements.GetCompletedCategoryAchievements(m_Player, m_AchievementCategory);
                    int totalAchievementsInCategory = m_AchievementsInCategory.Count;

                    textHue = GreyTextHue;

                    if (playerCompletedAchievementsInCategory > 0)
                        textHue = YellowTextHue;

                    if (playerCompletedAchievementsInCategory == m_AchievementsInCategory.Count)
                        textHue = GreenTextHue;

                    double playerCategoryAchievementProgress = (double)playerCompletedAchievementsInCategory / (double)totalAchievementsInCategory;

                    string categoryProgressText = playerCompletedAchievementsInCategory.ToString() + "/" + totalAchievementsInCategory.ToString();

                    startX = 25;
                    startY = 30;

                    //Category Image                    
                    AddGumpCollection(GumpCollections.GetGumpCollection(categoryDetail.GumpCollectionGroupId, categoryDetail.GumpCollectionItemId), startX - 40, startY + 8);

                    AddLabel(Utility.CenteredTextOffset(startX + 65, categoryDetail.m_CategoryName), startY + 5, categoryDetail.m_TextHue, categoryDetail.m_CategoryName);
                    AddImage(startX + 60, startY + 32, 10550, categoryDetail.m_IconHue);
                    AddImage(startX + 88, startY + 32, 10552, categoryDetail.m_IconHue);

                    //Category Progress
                    AddImage(startX + 10, startY + 99, Achievements.GetProgressBarBackground(playerCategoryAchievementProgress));
                    AddImageTiled(startX + 10 + Utility.ProgressBarX(playerCategoryAchievementProgress), startY + 102, Utility.ProgressBarWidth(playerCategoryAchievementProgress), 7, 2488);
                    AddLabel(Utility.CenteredTextOffset(startX + 60, categoryProgressText), startY + 113, textHue, categoryProgressText);
                    
                    startX = 3;
                    startY = 165;

                    int achievementSpacing = 25;

                    for (int a = 0; a < achievementCount + 1; a++)
                    {
                        int achievementIndex = achievementStartIndex + a;
                        int buttonIndex = 10 + achievementIndex;

                        if (achievementStartIndex >= totalAchievements)
                            continue;

                        Achievement achievement = m_AchievementsInCategory[achievementIndex];

                        AchievementDetail achievementDetail = Achievements.GetAchievementDetail(achievement);
                        AchievementEntry achievementEntry = Achievements.GetAchievementEntry(m_Player, achievement);

                        if (achievementDetail != null && achievementEntry != null)
                        {
                            textHue = WhiteTextHue;
                            string completionPercentageText = "0%";
                            
                            if (achievementEntry.m_Completed)
                            {
                                textHue = GreenTextHue;

                                if (!achievementEntry.m_Claimed)
                                    AddItem(startX, startY + 5, 572); //Green Orb

                                completionPercentageText = "100%";                                
                            }

                            else
                            {
                                double completionPercentage = (double)achievementEntry.m_Progress / (double)achievementDetail.m_ProgressNeeded;

                                completionPercentageText = Utility.CreateDecimalPercentageString(completionPercentage, 0);

                                if (completionPercentage > 0 && completionPercentage < .01)
                                    completionPercentageText = "1%";

                                if (completionPercentage >= .99 && completionPercentage < 1.0)
                                    completionPercentageText = "99%";

                                if (completionPercentage > 0)
                                    textHue = YellowTextHue;
                            }

                            if (!achievementEntry.m_Unlocked)
                            {
                                textHue = GreyTextHue;
                                //AddItem(startX, startY + 5, 573); //Orange Orb
                            }

                            if (achievementSelected == achievement)
                                AddButton(startX + 33, startY, 4030, 4030, buttonIndex, GumpButtonType.Reply, 0);
                            else
                                AddButton(startX + 33, startY, 4029, 4031, buttonIndex, GumpButtonType.Reply, 0);

                            AddLabel(startX + 68, startY + 1, textHue, completionPercentageText);
                            AddLabel(startX + 110, startY + 1, textHue, achievementDetail.m_DisplayName);
                        }

                        startY += achievementSpacing;
                    }

                    #region Achievement Window

                    AddImage(408, 233, 103, 2401);
			        AddImage(408, 150, 103, 2401);
			        AddImage(408, 60, 103, 2401);
			        AddImage(306, 150, 103);
			        AddImage(303, 234, 103, 2401);
			        AddImage(303, 150, 103, 2401);
			        AddImage(303, 60, 103, 2401);
			        AddImage(316, 72, 3604, 2052);
			        AddImage(316, 195, 3604, 2052);
			        AddImage(410, 72, 3604, 2052);
			        AddImage(411, 195, 3604, 2052);
			        AddImage(408, 322, 103, 2401);
			        AddImage(303, 323, 103, 2401);
			        AddImage(316, 284, 3604, 2052);
			        AddImage(411, 284, 3604, 2052);
			        AddImage(408, 362, 103, 2401);
			        AddImage(303, 363, 103, 2401);
			        AddImage(316, 324, 3604, 2052);
			        AddImage(411, 324, 3604, 2052);
                    AddImage(516, 49, 10441, 2401);                   

                    #endregion
			        
                    AchievementDetail selectedAchievementDetail = Achievements.GetAchievementDetail(achievementSelected);
                    AchievementEntry selectedAchievementEntry = Achievements.GetAchievementEntry(m_Player, achievementSelected);

                    int centerTextX = 440;
                    int textY = 80;

                    rowSpacing = 20;
                    
                    AddImage(349, 54, 2440, 1102);
                    AddLabel(Utility.CenteredTextOffset(centerTextX, selectedAchievementDetail.m_DisplayName), 55, categoryDetail.m_TextHue, selectedAchievementDetail.m_DisplayName);
                    
                    if (selectedAchievementDetail != null && selectedAchievementEntry != null)
                    {                        
                        if (!selectedAchievementEntry.m_Unlocked)
                        {
                            string unlockedText = "Stage " + selectedAchievementDetail.m_Stage.ToString() + " (Not Yet Unlocked)";

                            AddLabel(Utility.CenteredTextOffset(centerTextX, unlockedText), textY, GreyTextHue, unlockedText);

                            textY += rowSpacing;
                        }                        
                        
                        for (int a = 0; a < selectedAchievementDetail.m_Description.Length; a++)
                        {
                            AddLabel(Utility.CenteredTextOffset(centerTextX, selectedAchievementDetail.m_Description[a]), textY, WhiteTextHue, selectedAchievementDetail.m_Description[a]);

                            textY += rowSpacing;
                        }                        

                        AddLabel(409, 184, 2599, "Progress");                        

                        double selectedAchievementProgress = (double)selectedAchievementEntry.m_Progress / (double)selectedAchievementDetail.m_ProgressNeeded;
                        
                        textHue = GreyTextHue;

                        if (selectedAchievementEntry.m_Progress > 0)
                            textHue = YellowTextHue;

                        if (selectedAchievementEntry.m_Progress == selectedAchievementDetail.m_ProgressNeeded)
                            textHue = GreenTextHue;

                        AddImage(382, 205, Achievements.GetProgressBarBackground(selectedAchievementProgress));
                        AddImageTiled(382 + Utility.ProgressBarX(selectedAchievementProgress), 208, Utility.ProgressBarWidth(selectedAchievementProgress), 7, 2488);

                        string progressText = selectedAchievementEntry.m_Progress.ToString() + "/" + selectedAchievementDetail.m_ProgressNeeded.ToString();

                        AddLabel(Utility.CenteredTextOffset(centerTextX - 5, progressText), 220, textHue, progressText);

                        AddLabel(415, 250, 63, "Reward");

                        textY = 270;

                        for (int a = 0; a < selectedAchievementDetail.m_RewardDescription.Length; a++)
                        {
                            AddLabel(Utility.CenteredTextOffset(centerTextX, selectedAchievementDetail.m_RewardDescription[a]), textY, WhiteTextHue, selectedAchievementDetail.m_RewardDescription[a]);

                            textY += rowSpacing;
                        }

                        if (selectedAchievementDetail.m_RewardItemID != 0)                        
                            AddItem(360 + selectedAchievementDetail.m_RewardItemOffsetX, textY + 20 + selectedAchievementDetail.m_RewardItemOffsetY, selectedAchievementDetail.m_RewardItemID, selectedAchievementDetail.m_RewardItemHue);
                            
                        else
                            AddGumpCollection(GumpCollections.GetGumpCollection(selectedAchievementDetail.GumpCollectionGroupId, selectedAchievementDetail.GumpCollectionItemId), 355, 350);

                        if (selectedAchievementEntry.m_Completed)     
                        {
                            if (selectedAchievementEntry.m_Claimed)
                            {
                                AddLabel(426, 472, 63, "Claim");
                                AddButton(426, 442, 2151, 2154, 6, GumpButtonType.Reply, 0);
                            }

                            else
                                AddLabel(393, 462, 149, "Already Claimed");
                        }
                    }
                break;

                #endregion

                case PageType.Settings:
                break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;
            
            switch (m_SelectedPageType)
            {
                case PageType.Main:
                    int categoriesPerPage = categoryRows * categoryColumns;
                    int totalCategories = Enum.GetNames(typeof(AchievementCategory)).Length;
                    int totalPages = (int)(Math.Ceiling((double)totalCategories / (double)categoriesPerPage));

                    if (m_MainPage >= totalPages)
                        m_MainPage = 0;

                    if (m_MainPage < 0)
                        m_MainPage = 0;

                    int categoryStartIndex = m_MainPage * categoriesPerPage;
                    int categoryEndIndex = (m_MainPage * categoriesPerPage) + (categoriesPerPage - 1);

                    if (categoryEndIndex >= totalCategories)
                        categoryEndIndex = totalCategories - 1;

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Page
                        case 2:
                            if (m_MainPage > 0)
                                m_MainPage--;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 3:
                            if (m_MainPage < totalPages - 1)
                                m_MainPage++;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Settings
                        case 4:
                            m_SelectedPageType = PageType.Settings;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }

                    //Categories
                    if (info.ButtonID >= 10)
                    {
                        int categorySelectionIndex = info.ButtonID - 10;

                        if (categorySelectionIndex >= totalCategories)
                            categorySelectionIndex = 0;

                        m_SelectedPageType = PageType.Category;

                        m_AchievementCategory = (AchievementCategory)categorySelectionIndex;
                        m_AchievementListPage = 0;
                        m_AchievementSelectedIndex = 0;

                        m_Player.SendSound(ChangePageSound);

                        closeGump = false;
                    }
                break;

                case PageType.Category:
                    List<Achievement> m_AchievementsInCategory = Achievements.GetCategoryAchievements(m_AchievementCategory);

                    if (m_AchievementsInCategory.Count == 0)
                        return;

                    if (m_AchievementSelectedIndex >= m_AchievementsInCategory.Count)
                        m_AchievementSelectedIndex = 0;

                    Achievement achievementSelected = m_AchievementsInCategory[m_AchievementSelectedIndex];                    

                    int achievementsPerPage = 12;
                    int totalAchievements = m_AchievementsInCategory.Count;
                    int totalAchievementPages = (int)(Math.Ceiling((double)totalAchievements / (double)achievementsPerPage));

                    if (m_AchievementListPage >= totalAchievementPages)
                        m_AchievementListPage = 0;

                    if (m_AchievementListPage < 0)
                        m_AchievementListPage = 0;

                    int achievementStartIndex = m_AchievementListPage * achievementsPerPage;
                    int achievementEndIndex = (m_AchievementListPage * achievementsPerPage) + (achievementsPerPage - 1);

                    if (achievementEndIndex >= totalAchievements)
                        achievementEndIndex = totalAchievements - 1;
                                
                    int achievementCount = achievementEndIndex - achievementStartIndex;

                    AchievementDetail selectedAchievementDetail = Achievements.GetAchievementDetail(achievementSelected);
                    AchievementEntry selectedAchievementEntry = Achievements.GetAchievementEntry(m_Player, achievementSelected);                    

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Category Button
                        case 2:
                            closeGump = false;
                        break;

                        //Previous List Page
                        case 3:
                            if (m_AchievementListPage > 0)
                                m_AchievementListPage--;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next List Page
                        case 4:
                            if (m_AchievementListPage < totalAchievementPages - 1)
                                m_AchievementListPage++;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Return
                        case 5:
                            m_SelectedPageType = PageType.Main;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Claim Reward
                        case 6:
                            if (selectedAchievementDetail != null && selectedAchievementEntry != null)
                            {
                                if (selectedAchievementEntry.m_Completed && !selectedAchievementEntry.m_Claimed)                                
                                    Achievements.ClaimAchievement(m_Player, achievementSelected);
                            }

                            closeGump = false;
                        break;
                    }

                    //Achievement Selection
                    if (info.ButtonID >= 10)
                    {
                        int achievementSelectionIndex = info.ButtonID - 10;

                        if (achievementSelectionIndex >= totalAchievements)
                            achievementSelectionIndex = 0;

                        m_AchievementSelectedIndex = achievementSelectionIndex;

                        Achievement testAchievement = m_AchievementsInCategory[achievementSelectionIndex];

                        m_Player.SendSound(SelectionSound);

                        closeGump = false;
                    }
                break;

                case PageType.Settings:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                       //Return
                        case 2:
                            m_SelectedPageType = PageType.Main;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //On Login Show Unclaimed Achievements
                        case 3:
                            m_Player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards = !m_Player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards;

                            if (m_Player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards)
                                m_Player.SendMessage("You will now be notified upon login of unclaimed achievement rewards.");

                            else
                                m_Player.SendMessage("You will no longer be notified of unclaimed achievement rewards.");

                            closeGump = false;
                        break;

                        //Announce Completed Achievements to Guild Members
                        case 4:
                        m_Player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers = !m_Player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers;

                            if (m_Player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers)
                                m_Player.SendMessage("Your completion of achievements will now be announced to fellow guildmembers.");

                            else
                                m_Player.SendMessage("Your completion of achievements will now be kept private from other guildmembers.");

                            closeGump = false;
                        break;

                        //Announce Completed Achievements to Non-Guild Members
                        case 5:
                            m_Player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers = !m_Player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers;

                            if (m_Player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers)
                                m_Player.SendMessage("Your completion of achievements will now be announced to non-guild members.");

                            else
                                m_Player.SendMessage("Your completion of achievements will now be kept private from non-guild members.");

                            closeGump = false;
                        break;

                        //Show Achievements Completed by Other Guild Members
                        case 6:
                            m_Player.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements = !m_Player.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements;

                            if (m_Player.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements)
                                m_Player.SendMessage("You will now be notified when guildmembers complete achievements.");

                            else
                                m_Player.SendMessage("You will no longer be notified when guildmembers complete achievements.");

                            closeGump = false;
                        break;

                        //Show Achievements Completed by Non-Guild Members
                        case 7:
                        m_Player.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements = !m_Player.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements;

                            if (m_Player.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements)
                                m_Player.SendMessage("You will now be notified when non-guild members complete achievements.");

                            else
                                m_Player.SendMessage("You will no longer be notified when non-guild members complete achievements.");

                            closeGump = false;
                        break;

                        //On Completed Achievement Perform Audio
                        case 8:
                            m_Player.m_AchievementAccountEntry.AudioEnabled = !m_Player.m_AchievementAccountEntry.AudioEnabled;

                            if (m_Player.m_AchievementAccountEntry.AudioEnabled)
                                m_Player.SendMessage("Audio is now enabled for your completion of achievements.");

                            else
                                m_Player.SendMessage("Audio is now disabled for your completion of achievements.");

                            closeGump = false;
                        break;

                        //On Completed Achievement Display Pop-Up
                        case 9:
                            m_Player.m_AchievementAccountEntry.PopupEnabled = !m_Player.m_AchievementAccountEntry.PopupEnabled;

                            if (m_Player.m_AchievementAccountEntry.PopupEnabled)
                                m_Player.SendMessage("Pop-Ups will display for your completion of achievements.");

                            else
                                m_Player.SendMessage("Pop-Ups will no longer display for your completion of achievements.");

                            closeGump = false;
                        break;
                    }
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(AchievementsGump));
                m_Player.SendGump(new AchievementsGump(m_Player, m_SelectedPageType, m_MainPage, m_AchievementCategory, m_AchievementListPage, m_AchievementSelectedIndex));
            }
        }
    }
}