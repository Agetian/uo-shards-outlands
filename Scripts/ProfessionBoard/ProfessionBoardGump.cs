﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class ProfessionBoardGump : Gump
    {
        public PlayerMobile m_Player;
        public ProfessionGroupType m_ProfessionGroup;
        public ProfessionGroupPageDisplayType m_ProfessionGroupPageDisplayType = ProfessionGroupPageDisplayType.Jobs;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x3E6;
        public static int PurchaseSound = 0x2E6;
        public static int CloseGumpSound = 0x058;        

        public ProfessionBoardGump(PlayerMobile player, ProfessionGroupType professionGroup, ProfessionGroupPageDisplayType professionGroupPageDisplay): base(10, 10)
        {
            if (player == null) return;

            m_Player = player;
            m_ProfessionGroup = professionGroup;
            m_ProfessionGroupPageDisplayType = professionGroupPageDisplay;

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

            AddLabel(287, 0, WhiteTextHue, "Profession Board");

            switch (m_ProfessionGroupPageDisplayType)
            {
                #region Jobs

                    case ProfessionGroupPageDisplayType.Jobs:  
                  
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

                    string professionGroupName = ProfessionBoard.GetProfessionGroupName(professionGroup);
                    int professionGroupTextHue = ProfessionBoard.GetProfessionGroupTextHue(professionGroup);

                    string timeUntilJobsReset = "23h 17m";
                    string monthlyScoreText = "83 pts (25th)";
                    string lifetimeScoreText = "1,017 pts (3rd)";
                    int professionPointsAvailable = 500;                    

                    AddImage(203, 40, 1141);
                    AddLabel(Utility.CenteredTextOffset(350, professionGroupName), 42, professionGroupTextHue, professionGroupName);

                    AddLabel(133, 67, 149, "Jobs Will Reset in");
                    AddLabel(Utility.CenteredTextOffset(190, timeUntilJobsReset), 87, WhiteTextHue, timeUntilJobsReset);

                    AddLabel(129, 114, 63, professionPointsAvailable.ToString() + " Points Available");

                    AddLabel(437, 67, 2420, "Monthly Score (Rank)");
                    AddLabel(Utility.CenteredTextOffset(510, monthlyScoreText), 87, WhiteTextHue, monthlyScoreText);

                    AddLabel(435, 109, 2603, "Lifetime Score (Rank)");
                    AddLabel(Utility.CenteredTextOffset(510, lifetimeScoreText), 129, WhiteTextHue, lifetimeScoreText);

                    AddButton(178, 141, 4029, 4031, 5, GumpButtonType.Reply, 0);
                    AddLabel(215, 142, 2599, "Spend Points");

                    int startX = 272;
                    int startY = 57;

                    #region Profession Images

                    switch (professionGroup)
                    {
                        case ProfessionGroupType.FishermansCircle:
                            AddItem(startX + 34, startY + 19, 3520);
                            AddItem(startX + 66, startY + 48, 3656);
                            AddItem(startX + 35, startY + 36, 2476);
                            AddItem(startX + 76, startY + 39, 2467);
                            AddItem(startX + 45, startY + 35, 15113);
                        break;

                        case ProfessionGroupType.SmithingOrder:
                            AddItem(startX + 36, startY + 29, 5073);
                            AddItem(startX + 86, startY + 29, 5096);
                            AddItem(startX + 50, startY + 39, 7035);
                            AddItem(startX + 54, startY + 37, 5050);
                            AddItem(startX + 47, startY + 33, 5181);
                        break;

                        case ProfessionGroupType.TradesmanUnion:
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

                        case ProfessionGroupType.ArtificersEnclave:
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

                        case ProfessionGroupType.SeafarersLeague:
                            AddItem(startX + 70, startY + 40, 5370);
                            AddItem(startX + 46, startY + 3, 709);
                        break;

                        case ProfessionGroupType.AdventurersLodge:
                            AddItem(startX + 57, startY + 24, 4967);
                            AddItem(startX + 49, startY + 35, 4970);
                            AddItem(startX + 64, startY + 49, 2648);
                            AddItem(startX + 34, startY + 38, 5356);
                            AddItem(startX + 40, startY + 45, 3922);
                            AddItem(startX + 1, startY + 30, 3898);
                            AddItem(startX + 50, startY + 25, 5365);
                        break;

                        case ProfessionGroupType.ZoologicalFoundation:
                            AddItem(startX + 50, startY + 40, 2476);
                            AddItem(startX + 47, startY + 31, 3191);
                            AddItem(startX + 51, startY + 29, 3191);
                            AddItem(startX + 50, startY + 30, 3713);
                        break;

                        case ProfessionGroupType.ThievesGuild:
                            AddItem(startX + 58, startY + 39, 5373);
                            AddItem(startX + 48, startY + 33, 3589);
                        break;

                        case ProfessionGroupType.FarmersCooperative:
                            AddItem(startX + 54, startY + 23, 18240);
                        break;

                        case ProfessionGroupType.MonsterHuntersSociety:
                            AddItem(startX + 32, startY + 26, 7433);
                            AddItem(startX + 34, startY + 38, 4655);
                            AddItem(startX + 54, startY + 23, 7438);
                            AddItem(startX + 27, startY + 40, 7782);
                            AddItem(startX + 44, startY + 38, 3910);
                        break;
                    }

                    #endregion

                    //Jobs-----

                    AddLabel(165, 174, 149, "Job Description");
                    AddLabel(415, 174, 149, "Accepted");
                    AddLabel(527, 174, 149, "Completion");

                    //-----

                    AddItem(58, 212, 3847); //Image
                    AddLabel(120, 204, WhiteTextHue, "Craft 300 Greater Cure Potions");
                    AddLabel(130, 224, 2599, "(1 Profession Point Awarded)");
                    AddButton(428, 206, 2151, 2154, 10, GumpButtonType.Reply, 0);
                    AddLabel(Utility.CenteredTextOffset(560, "Any Alchemist In"), 204, WhiteTextHue, "Any Alchemist In");
                    AddLabel(Utility.CenteredTextOffset(560, "Prevalia"), 224, WhiteTextHue, "Prevalia");

                    //-----

                    AddButton(35, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                    AddLabel(69, 416, WhiteTextHue, "Previous Profession");

                    AddButton(516, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                    AddLabel(552, 416, WhiteTextHue, "Next Profession");

                    AddButton(259, 415, 4008, 4010, 4, GumpButtonType.Reply, 0);
                    AddLabel(296, 416, 149, "View Server Rankings");

                break;

                #endregion

                #region Spend Points

                case ProfessionGroupPageDisplayType.SpendPoints:
                break;

                #endregion

                #region Server Rankings

                case ProfessionGroupPageDisplayType.ServerRankings:
                break;

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (m_ProfessionGroupPageDisplayType)
            {
                #region Jobs

                case ProfessionGroupPageDisplayType.Jobs:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Profession
                        case 2:
                            int professionIndex = (int)m_ProfessionGroup;
                            professionIndex--;

                            if (professionIndex < 0)
                                professionIndex = Enum.GetNames(typeof(ProfessionGroupType)).Length - 1;

                            m_ProfessionGroup = (ProfessionGroupType)professionIndex;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Profession
                        case 3:
                            professionIndex = (int)m_ProfessionGroup;
                            professionIndex++;

                            if (professionIndex > Enum.GetNames(typeof(ProfessionGroupType)).Length - 1)
                                professionIndex = 0;

                            m_ProfessionGroup = (ProfessionGroupType)professionIndex;

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //View Server Rankings
                        case 4:
                            closeGump = false;
                        break;

                        //Spend Points
                        case 5:
                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10)
                    {
                        m_Player.Say("Pressed: " + info.ButtonID.ToString());

                        closeGump = false;
                    }

                    if (!closeGump)
                    {
                        m_Player.CloseGump(typeof(ProfessionBoardGump));
                        m_Player.SendGump(new ProfessionBoardGump(m_Player, m_ProfessionGroup, m_ProfessionGroupPageDisplayType));
                    }

                    else
                        m_Player.SendSound(CloseGumpSound);
                break;

                #endregion

                #region Spend Points

                case ProfessionGroupPageDisplayType.SpendPoints:
                break;

                #endregion

                #region Server Rankings

                case ProfessionGroupPageDisplayType.ServerRankings:
                break;

                #endregion
            }
        }
    }
}