﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class SocietiesJobBoardGump : Gump
    {
        public PlayerMobile m_Player;
        public SocietiesGroupType m_SocietiesGroup;
        public SocietiesGroupPageDisplayType m_SocietiesGroupPageDisplay = SocietiesGroupPageDisplayType.Jobs;
        public int m_RewardPage = 0;

        public static int RewardItemsPerPage = 4;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x3E6;
        public static int PurchaseSound = 0x2E6;
        public static int CloseGumpSound = 0x058;        

        public SocietiesJobBoardGump(PlayerMobile player, SocietiesGroupType societiesGroup, SocietiesGroupPageDisplayType societiesGroupPageDisplay, int rewardPage): base(10, 10)
        {
            if (player == null)
                return;            

            m_Player = player;
            m_SocietiesGroup = societiesGroup;
            m_SocietiesGroupPageDisplay = societiesGroupPageDisplay;
            m_RewardPage = rewardPage;

            Societies.CheckCreateSocietiesPlayerSettings(m_Player);

            string societiesGroupName = Societies.GetSocietyGroupName(m_SocietiesGroup);
            int societiesGroupTextHue = Societies.GetSocietyGroupTextHue(m_SocietiesGroup);

            SocietyGroupPlayerData societyGroupPlayerData = m_Player.m_SocietiesPlayerSettings.GetSocietyGroupPlayerData(m_SocietiesGroup);

            if (societyGroupPlayerData == null)
                return;

            int availablePoints = societyGroupPlayerData.m_PointsAvailable;
            int monthlyPoints = societyGroupPlayerData.m_MontlyPoints;
            int lifetimePoints = societyGroupPlayerData.m_LifetimePoints;
            int pointsSpent = societyGroupPlayerData.m_PointsSpent;
                       
            int WhiteTextHue = 2499;

            #region Background Images

            AddImage(5, 5, 103);
            AddImage(140, 5, 103);
            AddImage(266, 5, 103);
            AddImage(140, 92, 103);
            AddImage(5, 92, 103);
            AddImage(266, 92, 103);
            AddImage(140, 180, 103);
            AddImage(5, 180, 103);
            AddImage(266, 180, 103);
            AddImage(140, 264, 103);
            AddImage(5, 264, 103);
            AddImage(266, 264, 103);
            AddImage(400, 5, 103);
            AddImage(400, 92, 103);
            AddImage(400, 180, 103);
            AddImage(400, 265, 103);
            AddImage(530, 5, 103);
            AddImage(530, 92, 103);
            AddImage(530, 180, 103);
            AddImage(530, 265, 103);
            AddImage(140, 353, 103);
            AddImage(5, 353, 103);
            AddImage(266, 353, 103);
            AddImage(400, 353, 103);
            AddImage(530, 353, 103);
            AddBackground(19, 22, 643, 421, 5120);
            AddBackground(15, 20, 653, 419, 9380);

            #endregion

            AddButton(14, 14, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(12, 2, 149, "Guide");

            AddLabel(287, 0, WhiteTextHue, "Societies Job Board");

            switch (m_SocietiesGroupPageDisplay)
            {
                #region Jobs

                    case SocietiesGroupPageDisplayType.Jobs:  
                  
                    #region Header Images

                    AddImage(462, 60, 103);
                    AddImage(322, 60, 103);
                    AddImage(102, 60, 103);
                    AddImage(205, 60, 103);
                    AddImage(102, 48, 103);
                    AddImage(233, 48, 103);
                    AddImage(325, 48, 103);
                    AddImage(462, 48, 103);
                    AddImage(112, 58, 5104, 2052);
                    AddImage(191, 58, 5104, 2052);
                    AddImage(234, 58, 5104, 2052);
                    AddImage(298, 58, 5104, 2052);
                    AddImage(347, 58, 5104, 2052);
                    AddImage(504, 58, 5104, 2052);
                    AddImage(431, 58, 5104, 2052);

                    #endregion                                       

                    List<KeyValuePair<PlayerMobile, int>> monthlyRanks = Societies.GetSocietyGroupMonthlyRanks(m_SocietiesGroup);
                    List<KeyValuePair<PlayerMobile, int>> lifetimeRanks = Societies.GetSocietyGroupMonthlyRanks(m_SocietiesGroup);

                    int monthlyRank = monthlyRanks.Count;
                    int lifetimeRank = lifetimeRanks.Count;

                    for (int a = 0; a < monthlyRanks.Count; a++)
                    {
                        if (monthlyRanks[a].Key == m_Player)
                        {
                            monthlyRank = a + 1;
                            break;
                        }                        
                    }

                    for (int a = 0; a < lifetimeRanks.Count; a++)
                    {
                        if (lifetimeRanks[a].Key == m_Player)
                        {
                            lifetimeRank = a + 1;
                            break;
                        }
                    }
                    
                    string monthlyRankText = monthlyRank.ToString() + Utility.DetermineNumberSuffix(monthlyRank);
                    string lifetimeRankText = lifetimeRank.ToString() + Utility.DetermineNumberSuffix(lifetimeRank);
                    
                    string timeUntilNewJobs = Utility.CreateTimeRemainingString(DateTime.UtcNow, Societies.NextJobsAdded, true, true, true, true, false);
                    string monthlyScoreText = monthlyPoints.ToString() + " Points (" + monthlyRankText + ")";
                    string lifetimeScoreText = lifetimePoints.ToString() + " Points (" + lifetimeRankText + ")";                                     

                    AddImage(203, 40, 1141);
                    AddLabel(Utility.CenteredTextOffset(350, societiesGroupName), 42, societiesGroupTextHue, societiesGroupName);

                    AddLabel(125, 67, 149, "New Jobs Available In");
                    AddLabel(Utility.CenteredTextOffset(190, timeUntilNewJobs), 87, WhiteTextHue, timeUntilNewJobs);

                    AddLabel(129, 114, societiesGroupTextHue, availablePoints.ToString() + " Points in Society");

                    AddLabel(415, 67, 2420, "Monthly Score (Server Rank)");
                    AddLabel(Utility.CenteredTextOffset(510, monthlyScoreText), 87, WhiteTextHue, monthlyScoreText);

                    AddLabel(415, 109, 2603, "Lifetime Score (Server Rank)");
                    AddLabel(Utility.CenteredTextOffset(510, lifetimeScoreText), 129, WhiteTextHue, lifetimeScoreText);

                    AddButton(130, 141, 4029, 4031, 5, GumpButtonType.Reply, 0);
                    AddLabel(165, 142, 63, "Spend Points");

                    int startX = 268;
                    int startY = 57;

                    #region Societies Images

                    switch (m_SocietiesGroup)
                    {
                        case SocietiesGroupType.FishermansCircle:
                            AddItem(startX + 34, startY + 19, 3520);
                            AddItem(startX + 66, startY + 48, 3656);
                            AddItem(startX + 35, startY + 36, 2476);
                            AddItem(startX + 76, startY + 39, 2467);
                            AddItem(startX + 45, startY + 35, 15113);
                        break;

                        case SocietiesGroupType.SmithingOrder:
                            AddItem(startX + 36, startY + 29, 5073);
                            AddItem(startX + 86, startY + 29, 5096);
                            AddItem(startX + 50, startY + 39, 7035);
                            AddItem(startX + 54, startY + 37, 5050);
                            AddItem(startX + 47, startY + 33, 5181);
                        break;

                        case SocietiesGroupType.TradesmanUnion:
                            AddItem(startX + 29, startY + 27, 4142);
                            AddItem(startX + 37, startY + 23, 4150);
                            AddItem(startX + 61, startY + 35, 2920);
                            AddItem(startX + 49, startY + 25, 2921);
                            AddItem(startX + 67, startY + 47, 4148);
                            AddItem(startX + 48, startY + 31, 4189);
                            AddItem(startX + 57, startY + 27, 2581);
                            AddItem(startX + 36, startY + 20, 2503);
                            AddItem(startX + 45, startY + 14, 4172);
                        break;

                        case SocietiesGroupType.ArtificersEnclave:
                            AddItem(startX + 62, startY + 30, 2942, 2500);
                            AddItem(startX + 37, startY + 16, 2943, 2500);
                            AddItem(startX + 40, startY + 20, 4031);
                            AddItem(startX + 65, startY + 19, 6237);
                            AddItem(startX + 59, startY + 37, 3626);
                            AddItem(startX + 45, startY + 13, 3643, 2415);
                            AddItem(startX + 40, startY + 29, 5357);
                            AddItem(startX + 44, startY + 31, 5357);
                            AddItem(startX + 65, startY + 43, 3622);
                        break;

                        case SocietiesGroupType.SeafarersLeague:
                            AddItem(startX + 70, startY + 40, 5370);
                            AddItem(startX + 46, startY + 3, 709);
                        break;

                        case SocietiesGroupType.AdventurersLodge:
                            AddItem(startX + 57, startY + 24, 4967);
                            AddItem(startX + 49, startY + 35, 4970);
                            AddItem(startX + 64, startY + 49, 2648);
                            AddItem(startX + 34, startY + 38, 5356);
                            AddItem(startX + 40, startY + 45, 3922);
                            AddItem(startX + 1, startY + 30, 3898);
                            AddItem(startX + 50, startY + 25, 5365);
                        break;

                        case SocietiesGroupType.ZoologicalFoundation:
                            AddItem(startX + 50, startY + 40, 2476);
                            AddItem(startX + 47, startY + 31, 3191);
                            AddItem(startX + 51, startY + 29, 3191);
                            AddItem(startX + 50, startY + 30, 3713);
                        break;

                        case SocietiesGroupType.ThievesGuild:
                            AddItem(startX + 58, startY + 39, 5373);
                            AddItem(startX + 48, startY + 33, 3589);
                        break;

                        case SocietiesGroupType.FarmersCooperative:
                            AddItem(startX + 54, startY + 23, 18240);
                        break;

                        case SocietiesGroupType.MonsterHuntersSociety:
                            AddItem(startX + 32, startY + 26, 7433);
                            AddItem(startX + 34, startY + 38, 4655);
                            AddItem(startX + 54, startY + 23, 7438);
                            AddItem(startX + 27, startY + 40, 7782);
                            AddItem(startX + 44, startY + 38, 3910);
                        break;
                    }

                    #endregion                    

                    AddLabel(165, 175, 149, "Job Description");
                    AddLabel(380, 175, 149, "Accepted");
                    AddLabel(520, 175, 149, "Completion");
                    
                    startY = 200;

                    int entrySpacing = 50;

                    List<SocietyJob> m_SocietiesJobs = Societies.GetSocietyJobsByGroup(m_SocietiesGroup);

                    for (int a = 0; a < m_SocietiesJobs.Count; a++)
                    {
                        SocietyJob societyJob = m_SocietiesJobs[a];

                        if (societyJob == null) continue;
                        if (societyJob.Deleted) continue;
                        if (!societyJob.m_Listed) continue;

                        SocietyJobPlayerProgress jobPlayerProgress = Societies.GetSocietiesJobPlayerProgress(m_Player, societyJob);

                        AddItem(5 + societyJob.m_IconOffsetX, -35 + startY + societyJob.m_IconOffsetY, societyJob.m_IconItemId, societyJob.m_IconHue); //Image
                        AddLabel(120, startY, WhiteTextHue, societyJob.GetJobDescriptionText());
                        AddLabel(140, startY + 20, societiesGroupTextHue, societyJob.GetJobRewardText());
                        
                        if (jobPlayerProgress != null)
                            AddButton(390, startY, 2154, 2151, 10 + a, GumpButtonType.Reply, 0);
                        else
                            AddButton(390, startY, 2151, 2154, 10 + a, GumpButtonType.Reply, 0);

                        string destinationText = societyJob.GetJobDestinationText();

                        AddLabel(Utility.CenteredTextOffset(555, destinationText), startY, 2550, destinationText);

                        startY += entrySpacing;
                    }                    

                    //-----

                    AddButton(35, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                    AddLabel(69, 416, WhiteTextHue, "Previous Society");

                    AddButton(516, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                    AddLabel(552, 416, WhiteTextHue, "Next Society");

                    AddButton(259, 415, 4008, 4010, 4, GumpButtonType.Reply, 0);
                    AddLabel(296, 416, 149, "View Server Rankings");

                break;

                #endregion

                #region Server Rankings

                case SocietiesGroupPageDisplayType.ServerRankings:

                #region Header Images

                AddImage(462, 60, 103);
                AddImage(322, 60, 103);
                AddImage(102, 60, 103);
                AddImage(205, 60, 103);
                AddImage(102, 48, 103);
                AddImage(233, 48, 103);
                AddImage(325, 48, 103);
                AddImage(462, 48, 103);
                AddImage(112, 58, 5104, 2052);
                AddImage(191, 58, 5104, 2052);
                AddImage(234, 58, 5104, 2052);
                AddImage(298, 58, 5104, 2052);
                AddImage(347, 58, 5104, 2052);
                AddImage(504, 58, 5104, 2052);
                AddImage(431, 58, 5104, 2052);

                #endregion

                monthlyRanks = Societies.GetSocietyGroupMonthlyRanks(m_SocietiesGroup);
                lifetimeRanks = Societies.GetSocietyGroupMonthlyRanks(m_SocietiesGroup);

                monthlyRank = monthlyRanks.Count;
                lifetimeRank = lifetimeRanks.Count;

                for (int a = 0; a < monthlyRanks.Count; a++)
                {
                    if (monthlyRanks[a].Key == m_Player)
                    {
                        monthlyRank = a + 1;
                        break;
                    }
                }

                for (int a = 0; a < lifetimeRanks.Count; a++)
                {
                    if (lifetimeRanks[a].Key == m_Player)
                    {
                        lifetimeRank = a + 1;
                        break;
                    }
                }

                monthlyRankText = monthlyRank.ToString() + Utility.DetermineNumberSuffix(monthlyRank);
                lifetimeRankText = lifetimeRank.ToString() + Utility.DetermineNumberSuffix(lifetimeRank);

                AddImage(203, 40, 1141);
                AddLabel(Utility.CenteredTextOffset(350, societiesGroupName), 42, societiesGroupTextHue, societiesGroupName);

                startX = 268;
                startY = 57;

                #region Societies Images

                switch (m_SocietiesGroup)
                {
                    case SocietiesGroupType.FishermansCircle:
                        AddItem(startX + 34, startY + 19, 3520);
                        AddItem(startX + 66, startY + 48, 3656);
                        AddItem(startX + 35, startY + 36, 2476);
                        AddItem(startX + 76, startY + 39, 2467);
                        AddItem(startX + 45, startY + 35, 15113);
                        break;

                    case SocietiesGroupType.SmithingOrder:
                        AddItem(startX + 36, startY + 29, 5073);
                        AddItem(startX + 86, startY + 29, 5096);
                        AddItem(startX + 50, startY + 39, 7035);
                        AddItem(startX + 54, startY + 37, 5050);
                        AddItem(startX + 47, startY + 33, 5181);
                        break;

                    case SocietiesGroupType.TradesmanUnion:
                        AddItem(startX + 29, startY + 27, 4142);
                        AddItem(startX + 37, startY + 23, 4150);
                        AddItem(startX + 61, startY + 35, 2920);
                        AddItem(startX + 49, startY + 25, 2921);
                        AddItem(startX + 67, startY + 47, 4148);
                        AddItem(startX + 48, startY + 31, 4189);
                        AddItem(startX + 57, startY + 27, 2581);
                        AddItem(startX + 36, startY + 20, 2503);
                        AddItem(startX + 45, startY + 14, 4172);
                        break;

                    case SocietiesGroupType.ArtificersEnclave:
                        AddItem(startX + 62, startY + 30, 2942, 2500);
                        AddItem(startX + 37, startY + 16, 2943, 2500);
                        AddItem(startX + 40, startY + 20, 4031);
                        AddItem(startX + 65, startY + 19, 6237);
                        AddItem(startX + 59, startY + 37, 3626);
                        AddItem(startX + 45, startY + 13, 3643, 2415);
                        AddItem(startX + 40, startY + 29, 5357);
                        AddItem(startX + 44, startY + 31, 5357);
                        AddItem(startX + 65, startY + 43, 3622);
                        break;

                    case SocietiesGroupType.SeafarersLeague:
                        AddItem(startX + 70, startY + 40, 5370);
                        AddItem(startX + 46, startY + 3, 709);
                        break;

                    case SocietiesGroupType.AdventurersLodge:
                        AddItem(startX + 57, startY + 24, 4967);
                        AddItem(startX + 49, startY + 35, 4970);
                        AddItem(startX + 64, startY + 49, 2648);
                        AddItem(startX + 34, startY + 38, 5356);
                        AddItem(startX + 40, startY + 45, 3922);
                        AddItem(startX + 1, startY + 30, 3898);
                        AddItem(startX + 50, startY + 25, 5365);
                        break;

                    case SocietiesGroupType.ZoologicalFoundation:
                        AddItem(startX + 50, startY + 40, 2476);
                        AddItem(startX + 47, startY + 31, 3191);
                        AddItem(startX + 51, startY + 29, 3191);
                        AddItem(startX + 50, startY + 30, 3713);
                        break;

                    case SocietiesGroupType.ThievesGuild:
                        AddItem(startX + 58, startY + 39, 5373);
                        AddItem(startX + 48, startY + 33, 3589);
                        break;

                    case SocietiesGroupType.FarmersCooperative:
                        AddItem(startX + 54, startY + 23, 18240);
                        break;

                    case SocietiesGroupType.MonsterHuntersSociety:
                        AddItem(startX + 32, startY + 26, 7433);
                        AddItem(startX + 34, startY + 38, 4655);
                        AddItem(startX + 54, startY + 23, 7438);
                        AddItem(startX + 27, startY + 40, 7782);
                        AddItem(startX + 44, startY + 38, 3910);
                        break;
                }

                #endregion

                string monthlyPointsRankText = monthlyPoints.ToString() + " (" + monthlyRankText + ")";
                string lifetimePointsRankText = lifetimePoints.ToString() + " (" + lifetimeRankText + ")";

                AddLabel(139, 82, 2420, "My Monthly Points (Rank)");
                AddLabel(Utility.CenteredTextOffset(220, monthlyPointsRankText), 102, WhiteTextHue, monthlyPointsRankText);

                AddLabel(406, 82, 2603, "My Lifetime Points (Rank)");
                AddLabel(Utility.CenteredTextOffset(495, lifetimePointsRankText), 102, WhiteTextHue, lifetimePointsRankText);

                AddLabel(67, 174, 2420, "Server Monthly Points Leader");
                AddLabel(391, 175, 2603, "Server Lifetime Points Leader");

                int labelTextHue = WhiteTextHue;

                //Monthly Rankings
                startY = 200;

                int monthlyCount = monthlyRanks.Count;

                if (monthlyCount > 10)
                    monthlyCount = 10;

                for (int a = 0; a < monthlyCount; a++)
                {
                    KeyValuePair<PlayerMobile, int> playerMonthlyRank = monthlyRanks[a];

                    if (playerMonthlyRank.Key != null)
                    {
                        if (playerMonthlyRank.Key == m_Player)
                            labelTextHue = 63;

                        AddLabel(46, 200, labelTextHue, (a + 1).ToString() + ".");
                        AddLabel(110, startY, labelTextHue, playerMonthlyRank.Key.RawName);
                        AddLabel(235, startY, labelTextHue, playerMonthlyRank.Value.ToString() + " Points");
                    }

                    else
                    {
                        AddLabel(46, 200, labelTextHue, (a + 1).ToString() + ".");
                        AddLabel(110, startY, labelTextHue, "-");
                    }

                    startY += 20;
                }

                //Lifetime Rankings
                startY = 200;

                int lifetimeCount = lifetimeRanks.Count;

                if (lifetimeCount > 10)
                    lifetimeCount = 10;

                for (int a = 0; a < monthlyCount; a++)
                {
                    KeyValuePair<PlayerMobile, int> playerLifetimeRank = lifetimeRanks[a];

                    if (playerLifetimeRank.Key != null)
                    {
                        if (playerLifetimeRank.Key == m_Player)
                            labelTextHue = 63;

                        AddLabel(368, 200, labelTextHue, (a + 1).ToString() + ".");
                        AddLabel(432, startY, labelTextHue, playerLifetimeRank.Key.RawName);
                        AddLabel(557, startY, labelTextHue, playerLifetimeRank.Value.ToString() + " Points");
                    }

                    else
                    {
                        AddLabel(368, 200, labelTextHue, (a + 1).ToString() + ".");
                        AddLabel(432, startY, labelTextHue, "-");
                    }

                    startY += 20;
                }

                AddButton(35, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                AddLabel(69, 416, WhiteTextHue, "Previous Society");

                AddButton(516, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddLabel(552, 416, WhiteTextHue, "Next Society");

                AddButton(259, 415, 4008, 4010, 4, GumpButtonType.Reply, 0);
                AddLabel(296, 416, 149, "View Job Contracts");
                break;

                #endregion

                #region Spend Points

                case SocietiesGroupPageDisplayType.SpendPoints:

                    #region Header Images

                    AddImage(462, 60, 103);
                    AddImage(322, 60, 103);
                    AddImage(102, 60, 103);
                    AddImage(205, 60, 103);
                    AddImage(102, 48, 103);
                    AddImage(233, 48, 103);
                    AddImage(325, 48, 103);
                    AddImage(462, 48, 103);
                    AddImage(112, 58, 5104, 2052);
                    AddImage(191, 58, 5104, 2052);
                    AddImage(234, 58, 5104, 2052);
                    AddImage(298, 58, 5104, 2052);
                    AddImage(347, 58, 5104, 2052);
                    AddImage(504, 58, 5104, 2052);
                    AddImage(431, 58, 5104, 2052);

                    #endregion

                    AddImage(203, 40, 1141);
                    AddLabel(Utility.CenteredTextOffset(350, societiesGroupName), 42, societiesGroupTextHue, societiesGroupName);

                    startX = 268;
                    startY = 57;

                    #region Societies Images

                    switch (m_SocietiesGroup)
                    {
                        case SocietiesGroupType.FishermansCircle:
                            AddItem(startX + 34, startY + 19, 3520);
                            AddItem(startX + 66, startY + 48, 3656);
                            AddItem(startX + 35, startY + 36, 2476);
                            AddItem(startX + 76, startY + 39, 2467);
                            AddItem(startX + 45, startY + 35, 15113);
                            break;

                        case SocietiesGroupType.SmithingOrder:
                            AddItem(startX + 36, startY + 29, 5073);
                            AddItem(startX + 86, startY + 29, 5096);
                            AddItem(startX + 50, startY + 39, 7035);
                            AddItem(startX + 54, startY + 37, 5050);
                            AddItem(startX + 47, startY + 33, 5181);
                            break;

                        case SocietiesGroupType.TradesmanUnion:
                            AddItem(startX + 29, startY + 27, 4142);
                            AddItem(startX + 37, startY + 23, 4150);
                            AddItem(startX + 61, startY + 35, 2920);
                            AddItem(startX + 49, startY + 25, 2921);
                            AddItem(startX + 67, startY + 47, 4148);
                            AddItem(startX + 48, startY + 31, 4189);
                            AddItem(startX + 57, startY + 27, 2581);
                            AddItem(startX + 36, startY + 20, 2503);
                            AddItem(startX + 45, startY + 14, 4172);
                            break;

                        case SocietiesGroupType.ArtificersEnclave:
                            AddItem(startX + 62, startY + 30, 2942, 2500);
                            AddItem(startX + 37, startY + 16, 2943, 2500);
                            AddItem(startX + 40, startY + 20, 4031);
                            AddItem(startX + 65, startY + 19, 6237);
                            AddItem(startX + 59, startY + 37, 3626);
                            AddItem(startX + 45, startY + 13, 3643, 2415);
                            AddItem(startX + 40, startY + 29, 5357);
                            AddItem(startX + 44, startY + 31, 5357);
                            AddItem(startX + 65, startY + 43, 3622);
                            break;

                        case SocietiesGroupType.SeafarersLeague:
                            AddItem(startX + 70, startY + 40, 5370);
                            AddItem(startX + 46, startY + 3, 709);
                            break;

                        case SocietiesGroupType.AdventurersLodge:
                            AddItem(startX + 57, startY + 24, 4967);
                            AddItem(startX + 49, startY + 35, 4970);
                            AddItem(startX + 64, startY + 49, 2648);
                            AddItem(startX + 34, startY + 38, 5356);
                            AddItem(startX + 40, startY + 45, 3922);
                            AddItem(startX + 1, startY + 30, 3898);
                            AddItem(startX + 50, startY + 25, 5365);
                            break;

                        case SocietiesGroupType.ZoologicalFoundation:
                            AddItem(startX + 50, startY + 40, 2476);
                            AddItem(startX + 47, startY + 31, 3191);
                            AddItem(startX + 51, startY + 29, 3191);
                            AddItem(startX + 50, startY + 30, 3713);
                            break;

                        case SocietiesGroupType.ThievesGuild:
                            AddItem(startX + 58, startY + 39, 5373);
                            AddItem(startX + 48, startY + 33, 3589);
                            break;

                        case SocietiesGroupType.FarmersCooperative:
                            AddItem(startX + 54, startY + 23, 18240);
                            break;

                        case SocietiesGroupType.MonsterHuntersSociety:
                            AddItem(startX + 32, startY + 26, 7433);
                            AddItem(startX + 34, startY + 38, 4655);
                            AddItem(startX + 54, startY + 23, 7438);
                            AddItem(startX + 27, startY + 40, 7782);
                            AddItem(startX + 44, startY + 38, 3910);
                            break;
                    }

                    #endregion

                    AddLabel(143, 93, societiesGroupTextHue, availablePoints + " Points Available");

                    AddLabel(451, 67, 2420, "Total Points Spent");
                    AddLabel(Utility.CenteredTextOffset(510, pointsSpent.ToString()) , 87, WhiteTextHue, pointsSpent.ToString());			        

                    AddLabel(449, 109, 2603, "Total Points Earned");
                    AddLabel(Utility.CenteredTextOffset(510, lifetimePoints.ToString()), 129, WhiteTextHue, lifetimePoints.ToString());			        

                    List<SocietyRewardItem> itemList = SocietiesRewards.SocietyRewardList[m_SocietiesGroup];

                    int totalItems = itemList.Count;
                    int totalItemPages = (int)(Math.Ceiling((double)totalItems / (double)RewardItemsPerPage));

                    if (m_RewardPage >= totalItemPages)
                        m_RewardPage = 0;

                    if (m_RewardPage < 0)
                        m_RewardPage = 0;

                    int itemStartIndex = m_RewardPage * RewardItemsPerPage;
                    int itemEndIndex = (m_RewardPage * RewardItemsPerPage) + (RewardItemsPerPage - 1);

                    if (itemEndIndex >= totalItems)
                        itemEndIndex = totalItems - 1;

                    int itemCount = itemEndIndex - itemStartIndex;

                    for (int a = 0; a < itemCount + 1; a++)
                    {
                        if (totalItems == 0)
                            continue;

                        switch (a)
                        {
                            case 0: startX = 108; startY = 198; break;
                            case 1: startX = 458; startY = 198; break;
                            case 2: startX = 108; startY = 312; break;
                            case 3: startX = 458; startY = 312; break;
                        }                        

                        int itemIndex = itemStartIndex + a;

                        if (itemIndex >= itemList.Count)
                            continue;

                        SocietyRewardItem item = itemList[itemIndex];

                        if (item == null)
                            continue;                        
                        
                        //Item 
                        AddItem(startX - 113 + item.ItemIconOffsetX, startY - 18 + item.ItemIconOffsetY, item.ItemIconItemId, item.ItemIconHue);
                        AddLabel(startX, startY, WhiteTextHue, item.ItemName);
                        AddLabel(startX, startY + 20, 53, "Cost");
                        AddLabel(startX + 40, startY + 20, societiesGroupTextHue, item.ItemCost.ToString() + " Points");
                        AddLabel(startX, startY + 40, 2599, "Info");
                        AddButton(startX + 33, startY + 43, 1209, 1210, 20 + a, GumpButtonType.Reply, 0);
                        AddLabel(startX + 62, startY + 40, 63, "Purchase");
                        AddButton(startX + 124, startY + 38, 2151, 2154, 10 + a, GumpButtonType.Reply, 0);
                    }

                    bool previousPagesExist = false;
                    bool morePagesExist = false;

                    if (m_RewardPage > 0)
                        previousPagesExist = true;

                    if (m_RewardPage < totalItemPages - 1)
                        morePagesExist = true;
                    
                    if (previousPagesExist)
                        AddButton(327, 254, 9900, 9900, 5, GumpButtonType.Reply, 0);

                    if (previousPagesExist || morePagesExist)
                        AddLabel(303, 278, WhiteTextHue, "More Items");

                    if (morePagesExist)
                        AddButton(326, 302, 9906, 9906, 6, GumpButtonType.Reply, 0);

                    AddButton(35, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                    AddLabel(69, 416, WhiteTextHue, "Previous Society");

                    AddButton(516, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                    AddLabel(552, 416, WhiteTextHue, "Next Society");

                    AddButton(259, 415, 4008, 4010, 4, GumpButtonType.Reply, 0);
                    AddLabel(296, 416, 149, "View Job Contracts");

                break;

                #endregion                
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            Societies.CheckCreateSocietiesPlayerSettings(m_Player);
            
            SocietyGroupPlayerData societyGroupPlayerData = m_Player.m_SocietiesPlayerSettings.GetSocietyGroupPlayerData(m_SocietiesGroup);

            if (societyGroupPlayerData == null)
                return;

            int availablePoints = societyGroupPlayerData.m_PointsAvailable;

            switch (m_SocietiesGroupPageDisplay)
            {
                #region Jobs

                case SocietiesGroupPageDisplayType.Jobs:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Society
                        case 2:
                            int societiesIndex = (int)m_SocietiesGroup;

                            societiesIndex--;                            

                            if (societiesIndex < 0)
                                societiesIndex = Enum.GetNames(typeof(SocietiesGroupType)).Length - 1;

                            m_SocietiesGroup = (SocietiesGroupType)societiesIndex;
                            m_RewardPage = 0;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Society
                        case 3:
                            societiesIndex = (int)m_SocietiesGroup;
                            societiesIndex++;

                            if (societiesIndex > Enum.GetNames(typeof(SocietiesGroupType)).Length - 1)
                                societiesIndex = 0;

                            m_SocietiesGroup = (SocietiesGroupType)societiesIndex;
                            m_RewardPage = 0;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //View Server Rankings
                        case 4:
                            m_SocietiesGroupPageDisplay = SocietiesGroupPageDisplayType.ServerRankings;

                            m_Player.SendSound(OpenGumpSound);                            

                            closeGump = false;
                        break;

                        //Spend Points
                        case 5:
                            m_SocietiesGroupPageDisplay = SocietiesGroupPageDisplayType.SpendPoints;

                            m_Player.SendSound(OpenGumpSound); 

                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10)
                    {
                        List<SocietyJob> m_SocietiesJobs = Societies.GetSocietyJobsByGroup(m_SocietiesGroup);

                        int jobIndex = info.ButtonID - 10;

                        if (jobIndex < m_SocietiesJobs.Count)
                        {
                            SocietyJob societyJob = m_SocietiesJobs[jobIndex];

                            if (societyJob != null)
                            {
                                if (!societyJob.Deleted)
                                    societyJob.PlayerAccept(m_Player);
                            }                            
                        }                        

                        closeGump = false;
                    }
                break;

                #endregion

                #region Server Rankings

                case SocietiesGroupPageDisplayType.ServerRankings:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Society
                        case 2:
                            int societiesIndex = (int)m_SocietiesGroup;
                            societiesIndex--;

                            if (societiesIndex < 0)
                                societiesIndex = Enum.GetNames(typeof(SocietiesGroupType)).Length - 1;

                            m_SocietiesGroup = (SocietiesGroupType)societiesIndex;
                            m_RewardPage = 0;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Society
                        case 3:
                            societiesIndex = (int)m_SocietiesGroup;
                            societiesIndex++;

                            if (societiesIndex > Enum.GetNames(typeof(SocietiesGroupType)).Length - 1)
                                societiesIndex = 0;

                            m_SocietiesGroup = (SocietiesGroupType)societiesIndex;
                            m_RewardPage = 0;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //View Job Contracts
                        case 4:
                            m_SocietiesGroupPageDisplay = SocietiesGroupPageDisplayType.Jobs;

                            m_Player.SendSound(OpenGumpSound);    

                            closeGump = false;
                        break;
                    }
                break;

                #endregion

                #region Spend Points

                case SocietiesGroupPageDisplayType.SpendPoints:

                    List<SocietyRewardItem> itemList = SocietiesRewards.SocietyRewardList[m_SocietiesGroup];

                    int totalItems = itemList.Count;
                    int totalItemPages = (int)(Math.Ceiling((double)totalItems / (double)RewardItemsPerPage));

                    if (m_RewardPage >= totalItemPages)
                        m_RewardPage = 0;

                    if (m_RewardPage < 0)
                        m_RewardPage = 0;

                    int itemStartIndex = m_RewardPage * RewardItemsPerPage;
                    int itemEndIndex = (m_RewardPage * RewardItemsPerPage) + (RewardItemsPerPage - 1);

                    if (itemEndIndex >= totalItems)
                        itemEndIndex = totalItems - 1;

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Society
                        case 2:
                            int societiesIndex = (int)m_SocietiesGroup;
                            societiesIndex--;

                            if (societiesIndex < 0)
                                societiesIndex = Enum.GetNames(typeof(SocietiesGroupType)).Length - 1;

                            m_SocietiesGroup = (SocietiesGroupType)societiesIndex;
                            m_RewardPage = 0;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Society
                        case 3:
                            societiesIndex = (int)m_SocietiesGroup;
                            societiesIndex++;

                            if (societiesIndex > Enum.GetNames(typeof(SocietiesGroupType)).Length - 1)
                                societiesIndex = 0;

                            m_SocietiesGroup = (SocietiesGroupType)societiesIndex;
                            m_RewardPage = 0;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //View Job Contracts
                        case 4:
                            m_SocietiesGroupPageDisplay = SocietiesGroupPageDisplayType.Jobs;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Previous Rewards Page
                        case 5:
                            m_RewardPage--;

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Next Rewards Page
                        case 6:
                            m_RewardPage++;

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break; 
                    }
                    
                    //Purchase Item
                    if (info.ButtonID >= 10 && info.ButtonID < 20)
                    {
                        int itemSelectionIndex = info.ButtonID - 10;
                        int itemSelected = (m_RewardPage * RewardItemsPerPage) + itemSelectionIndex;

                        if (itemSelected >= totalItems)
                            itemSelected = 0;
                        
                        SocietyRewardItem item = itemList[itemSelected];

                        if (item == null)
                            return;
                                                                        
                        bool purchaseAllowed = true;

                        int pointsCost = 0;
                        
                        Item rewardItem = (Item)Activator.CreateInstance(item.ItemType);

                        if (rewardItem == null)
                        {
                        }

                        else if (!purchaseAllowed && m_Player.AccessLevel == AccessLevel.Player)
                        {
                            rewardItem.Delete();

                            m_Player.SendMessage("Reward purchases are not allowed in this area.");
                        }

                        else if (item.ItemCost > availablePoints && m_Player.AccessLevel == AccessLevel.Player)
                        {
                            rewardItem.Delete();

                            m_Player.SendMessage("You do not have enough points available to spend in this society to purchase that item.");
                        }

                        else if (m_Player.Backpack.TotalItems + rewardItem.TotalItems > m_Player.Backpack.MaxItems && m_Player.AccessLevel == AccessLevel.Player)
                        {
                            rewardItem.Delete();

                            m_Player.SendMessage("Your backpack contains too many items to purchase this item. Please remove some items and try again.");
                        }

                        else if (m_Player.Backpack.TotalWeight + rewardItem.TotalWeight > m_Player.MaxWeight && m_Player.AccessLevel == AccessLevel.Player)
                        {
                            rewardItem.Delete();

                            m_Player.SendMessage("Your backpack is too heavy to purchase this item. Please remove some items and try again.");
                        }

                        else
                        {
                            if (m_Player.AccessLevel == AccessLevel.Player)
                            {
                                societyGroupPlayerData.m_PointsAvailable -= pointsCost;
                                societyGroupPlayerData.m_PointsSpent += pointsCost;

                                m_Player.SendMessage("You purchase the item.");
                            }

                            else
                                m_Player.SendMessage("You use your godly powers to purchase the item.");

                            m_Player.SendSound(PurchaseSound);                                

                            rewardItem.ItemGroup = ItemGroupType.PurchasedReward;

                            m_Player.Backpack.DropItem(rewardItem);
                        }

                        closeGump = false;
                    }

                    //Item Info
                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                        int itemSelectionIndex = info.ButtonID - 20;
                        int itemSelected = (m_RewardPage * RewardItemsPerPage) + itemSelectionIndex;

                        if (itemSelected >= totalItems)
                            itemSelected = 0;

                        SocietyRewardItem item = itemList[itemSelected];

                        if (item == null)
                            return;

                        string description = "";

                        if (item.ItemDescription != null)
                        {
                            for (int a = 0; a < item.ItemDescription.Count; a++)
                            {
                                description += item.ItemDescription[a];

                                if (a < item.ItemDescription.Count - 1)
                                    description += " ";
                            }
                        }

                        m_Player.SendMessage(description);

                        closeGump = false;
                    }                    
                break;

                #endregion                
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(SocietiesJobBoardGump));
                m_Player.SendGump(new SocietiesJobBoardGump(m_Player, m_SocietiesGroup, m_SocietiesGroupPageDisplay, m_RewardPage));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }
}