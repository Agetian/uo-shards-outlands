using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom;

namespace Server.Items
{
    public class CaptchaGump : Gump
    {
        PlayerMobile m_Player;

        public static int OpenGumpSound = 0x055;
        public static int SelectionSound = 0x059;
        public static int CloseGumpSound = 0x058;

        public CaptchaGump(PlayerMobile player): base(10, 10)
        {
            int WhiteTextHue = 2036;

            if (player == null)
                return;

            m_Player = player;

            CaptchaPersistance.CheckAndCreateCaptchaAccountEntry(m_Player);

            CaptchaAccountData captchaData = m_Player.m_CaptchaAccountData;
            
            #region Images 

            AddImage(9, 7, 103, 2499);
            AddImage(144, 7, 103, 2499);
            AddImage(270, 7, 103, 2499);
            AddImage(144, 94, 103, 2499);
            AddImage(9, 94, 103, 2499);
            AddImage(270, 94, 103, 2499);
            AddImage(104, 22, 5104, 2052);
            AddImage(18, 22, 5104, 2052);
            AddImage(168, 22, 5104, 2052);
            AddImage(257, 22, 5104, 2052);
            AddImage(311, 22, 5104, 2052);
            AddImage(104, 93, 5104, 2052);
            AddImage(18, 94, 5104, 2052);
            AddImage(168, 92, 5104, 2052);
            AddImage(257, 93, 5104, 2052);
            AddImage(311, 93, 5104, 2052);

            AddImage(125, 2, 2446, 2401);

            AddLabel(188, 2, 2599, "Captcha");
            AddLabel(154, 27, 149, "Match the Symbols");

            #endregion           

            int offsetX = 0;
            int offsetY = 0;

            //Correct
            if (captchaData.m_Row1CorrectIndex >= captchaData.m_Row1IDs.Count)
                captchaData.m_Row1CorrectIndex = 0;
            int row1CorrectId = captchaData.m_Row1IDs[captchaData.m_Row1CorrectIndex];

            if (captchaData.m_Row2CorrectIndex >= captchaData.m_Row2IDs.Count)
                captchaData.m_Row2CorrectIndex = 0;
            int row2CorrectId = captchaData.m_Row2IDs[captchaData.m_Row2CorrectIndex];

            if (captchaData.m_Row3CorrectIndex >= captchaData.m_Row3IDs.Count)
                captchaData.m_Row3CorrectIndex = 0;
            int row3CorrectId = captchaData.m_Row3IDs[captchaData.m_Row3CorrectIndex];

            //Selected
            int row1SelectedIndex = captchaData.m_SelectedRow1Index;
            if (row1SelectedIndex >= captchaData.m_Row1IDs.Count)
                row1SelectedIndex = 0;
            int row1SelectedID = captchaData.m_Row1IDs[row1SelectedIndex];

            int row2SelectedIndex = captchaData.m_SelectedRow2Index;
            if (row2SelectedIndex >= captchaData.m_Row2IDs.Count)
                row2SelectedIndex = 0;
            int row2SelectedID = captchaData.m_Row2IDs[row2SelectedIndex];

            int row3SelectedIndex = captchaData.m_SelectedRow3Index;
            if (row3SelectedIndex >= captchaData.m_Row3IDs.Count)
                row3SelectedIndex = 0;
            int row3SelectedID = captchaData.m_Row3IDs[row3SelectedIndex];

            //Symbols
            captchaData.GetIconOffsets(row1CorrectId, out offsetX, out offsetY);
            AddItem(9 + offsetX, 24 + offsetY, row1CorrectId);

            captchaData.GetIconOffsets(row2CorrectId, out offsetX, out offsetY);
            AddItem(141 + offsetX, 24 + offsetY, row2CorrectId);

            captchaData.GetIconOffsets(row3CorrectId, out offsetX, out offsetY);
            AddItem(268 + offsetX, 24 + offsetY, row3CorrectId);

            //Guide
            AddLabel(-1, 0, 149, "Guide");
            AddButton(1, 15, 2094, 2095, 1, GumpButtonType.Reply, 0);

            //Row 1            
            AddButton(31, 117, 9909, 9909, 3, GumpButtonType.Reply, 0);
            captchaData.GetIconOffsets(row1SelectedID, out offsetX, out offsetY);
            AddItem(9 + offsetX, 78 + offsetY, row1SelectedID);
            AddButton(111, 116, 9903, 9903, 4, GumpButtonType.Reply, 0);

            //Row 2
            AddButton(162, 117, 9909, 9909, 5, GumpButtonType.Reply, 0);
            captchaData.GetIconOffsets(row2SelectedID, out offsetX, out offsetY);
            AddItem(141 + offsetX, 78 + offsetY, row2SelectedID);
            AddButton(242, 116, 9903, 9903, 6, GumpButtonType.Reply, 0);

            //Row 3
            AddButton(289, 117, 9909, 9909, 7, GumpButtonType.Reply, 0);
            captchaData.GetIconOffsets(row3SelectedID, out offsetX, out offsetY);
            AddItem(268 + offsetX, 78 + offsetY, row3SelectedID);
            AddButton(369, 116, 9903, 9903, 8, GumpButtonType.Reply, 0);

            if (captchaData.m_ConfirmPrompt)
                AddLabel(143, 150, WhiteTextHue, "Click Again to Confirm");

            AddButton(181, 176, 247, 249, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            CaptchaPersistance.CheckAndCreateCaptchaAccountEntry(m_Player);

            CaptchaAccountData captchaData = m_Player.m_CaptchaAccountData;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Confirm
                case 2:
                    if (captchaData.m_ConfirmPrompt)
                    {
                        bool correct = false;

                        if (captchaData.m_Row1CorrectIndex == captchaData.m_SelectedRow1Index && captchaData.m_Row2CorrectIndex == captchaData.m_SelectedRow2Index && captchaData.m_Row3CorrectIndex == captchaData.m_SelectedRow3Index)
                            correct = true;

                        if (correct)
                        {
                            captchaData.m_LastPrompt = DateTime.UtcNow;
                            captchaData.m_CaptchaRequired = false;
                            captchaData.m_CaptchaAttempt = 0;
                            captchaData.m_ConfirmPrompt = false;

                            m_Player.SendSound(0x5B6);

                            m_Player.SendMessage("Captcha successful.");

                            //TEST: CONTINUE WITH HARVEST

                            return;
                        }

                        else
                        {
                            captchaData.m_CaptchaAttempt++;

                            if (captchaData.m_CaptchaAttempt == 3)
                            {
                                string responseMessage = "";

                                if (captchaData.m_PreviousPenalty != CaptchaAccountData.PenaltyLevelType.None && captchaData.m_PenaltyProbationExpiration > DateTime.UtcNow)
                                {
                                    switch (captchaData.m_PreviousPenalty)
                                    {
                                        case CaptchaAccountData.PenaltyLevelType.None:
                                            captchaData.m_CurrentPenalty = CaptchaAccountData.PenaltyLevelType.Minor;
                                            captchaData.m_CurrentPenaltyExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyDuration(captchaData.m_CurrentPenalty);

                                            captchaData.m_PreviousPenalty = captchaData.m_CurrentPenalty;
                                            captchaData.m_PenaltyProbationExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyProbationDuration(captchaData.m_PreviousPenalty);

                                            responseMessage = "You have failed a captcha response and your account will now be prevented from gathering resources for ";
                                            responseMessage += Utility.CreateTimeRemainingString(DateTime.UtcNow, captchaData.m_CurrentPenaltyExpiration, false, true, true, true, false) + ".";

                                            m_Player.SendMessage(2115, responseMessage);
                                        break;

                                        case CaptchaAccountData.PenaltyLevelType.Minor:
                                            captchaData.m_CurrentPenalty = CaptchaAccountData.PenaltyLevelType.Major;
                                            captchaData.m_CurrentPenaltyExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyDuration(captchaData.m_CurrentPenalty);

                                            captchaData.m_PreviousPenalty = captchaData.m_CurrentPenalty;
                                            captchaData.m_PenaltyProbationExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyProbationDuration(captchaData.m_PreviousPenalty);

                                            responseMessage = "You have failed a second captcha response within one day, and your account will now be prevented from gathering resources for ";
                                            responseMessage += Utility.CreateTimeRemainingString(DateTime.UtcNow, captchaData.m_CurrentPenaltyExpiration, false, true, true, true, false) + ".";

                                            m_Player.SendMessage(2115, responseMessage);
                                        break;

                                        case CaptchaAccountData.PenaltyLevelType.Major:
                                            captchaData.m_CurrentPenalty = CaptchaAccountData.PenaltyLevelType.Epic;
                                            captchaData.m_CurrentPenaltyExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyDuration(captchaData.m_CurrentPenalty);

                                            captchaData.m_PreviousPenalty = captchaData.m_CurrentPenalty;
                                            captchaData.m_PenaltyProbationExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyProbationDuration(captchaData.m_PreviousPenalty);

                                            responseMessage = "You have failed a third captcha response within one week, and your account will now be prevented from gathering resources for ";
                                            responseMessage += Utility.CreateTimeRemainingString(DateTime.UtcNow, captchaData.m_CurrentPenaltyExpiration, false, true, true, true, false) + ".";

                                            m_Player.SendMessage(2115, responseMessage);
                                        break;

                                        case CaptchaAccountData.PenaltyLevelType.Epic:
                                            captchaData.m_CurrentPenalty = CaptchaAccountData.PenaltyLevelType.Permanent;
                                            captchaData.m_CurrentPenaltyExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyDuration(captchaData.m_CurrentPenalty);

                                            captchaData.m_PreviousPenalty = captchaData.m_CurrentPenalty;
                                            captchaData.m_PenaltyProbationExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyProbationDuration(captchaData.m_PreviousPenalty);

                                            responseMessage = "You have failed a fourth captcha response within one month, and your account will now be permanently prevented from gathering resources.";

                                            m_Player.SendMessage(2115, responseMessage);
                                        break;

                                        case CaptchaAccountData.PenaltyLevelType.Permanent:
                                            captchaData.m_CurrentPenalty = CaptchaAccountData.PenaltyLevelType.Permanent;
                                            captchaData.m_CurrentPenaltyExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyDuration(captchaData.m_CurrentPenalty);

                                            captchaData.m_PreviousPenalty = captchaData.m_CurrentPenalty;
                                            captchaData.m_PenaltyProbationExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyProbationDuration(captchaData.m_PreviousPenalty);

                                            responseMessage = "You have failed a fourth captcha response within one month, and your account will now be permanently prevented from gathering resources.";

                                            m_Player.SendMessage(2115, responseMessage);
                                        break;
                                    }
                                }

                                else
                                {
                                    captchaData.m_CurrentPenalty = CaptchaAccountData.PenaltyLevelType.Minor;
                                    captchaData.m_CurrentPenaltyExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyDuration(captchaData.m_CurrentPenalty);

                                    captchaData.m_PreviousPenalty = captchaData.m_CurrentPenalty;
                                    captchaData.m_PenaltyProbationExpiration = DateTime.UtcNow + CaptchaAccountData.GetPenaltyProbationDuration(captchaData.m_PreviousPenalty);

                                    responseMessage = "You have failed a captcha response and your account will now be blocked from gathering resources for " + Utility.CreateTimeRemainingString(DateTime.UtcNow, captchaData.m_CurrentPenaltyExpiration, false, true, true, true, false) + ".";

                                    m_Player.SendMessage(2115, responseMessage);
                                }

                                captchaData.m_LastPrompt = DateTime.UtcNow;
                                captchaData.m_CaptchaRequired = false;
                                captchaData.m_CaptchaAttempt = 0;
                                captchaData.m_ConfirmPrompt = false;

                                m_Player.SendSound(0x5B3);

                                return;
                            }

                            else
                            {
                                switch(captchaData.m_CaptchaAttempt)
                                {
                                    case 1: m_Player.SendMessage("Captcha response incorrect. You have two more attempts."); break;
                                    case 2: m_Player.SendMessage("Captcha response incorrect. You have one more attempt."); break;
                                }

                                captchaData.m_LastPrompt = DateTime.UtcNow;
                                captchaData.GenerateIDs();
                                captchaData.m_ConfirmPrompt = false;
                            }
                        }                        
                    }

                    else
                        captchaData.m_ConfirmPrompt = true;                    
                    
                    closeGump = false;
                break;

                //Row 1 Previous
                case 3:
                    captchaData.m_SelectedRow1Index--;

                    if (captchaData.m_SelectedRow1Index < 0)
                        captchaData.m_SelectedRow1Index = CaptchaAccountData.ItemsPerRow - 1;

                    captchaData.m_ConfirmPrompt = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Row 1 Next
                case 4:
                    captchaData.m_SelectedRow1Index++;

                    if (captchaData.m_SelectedRow1Index >= CaptchaAccountData.ItemsPerRow)
                        captchaData.m_SelectedRow1Index = 0;

                    captchaData.m_ConfirmPrompt = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Row 2 Previous
                case 5:
                    captchaData.m_SelectedRow2Index--;

                    if (captchaData.m_SelectedRow2Index < 0)
                        captchaData.m_SelectedRow2Index = CaptchaAccountData.ItemsPerRow - 1;

                    captchaData.m_ConfirmPrompt = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Row 2 Next
                case 6:
                    captchaData.m_SelectedRow2Index++;

                    if (captchaData.m_SelectedRow2Index >= CaptchaAccountData.ItemsPerRow)
                        captchaData.m_SelectedRow2Index = 0;

                    captchaData.m_ConfirmPrompt = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Row 3 Previous
                case 7:
                    captchaData.m_SelectedRow3Index--;

                    if (captchaData.m_SelectedRow3Index < 0)
                        captchaData.m_SelectedRow3Index = CaptchaAccountData.ItemsPerRow - 1;

                    captchaData.m_ConfirmPrompt = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Row 3 Next
                case 8:
                    captchaData.m_SelectedRow3Index++;

                    if (captchaData.m_SelectedRow3Index >= CaptchaAccountData.ItemsPerRow)
                        captchaData.m_SelectedRow3Index = 0;

                    captchaData.m_ConfirmPrompt = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(CaptchaGump));
                m_Player.SendGump(new CaptchaGump(m_Player));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }
}