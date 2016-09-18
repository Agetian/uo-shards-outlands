using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Prompts;

namespace Server
{
    public class RuneTomeGump : Gump
    {
        public enum PageType
        {           
            Overview,
            EntryDetail
        }

        public PlayerMobile m_Player;
        public RuneTomeGumpObject m_RuneTomeGumpObject;

        public static int MaxRuneDescriptionLength = 40;

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;       

        public RuneTomeGump(PlayerMobile player, RuneTomeGumpObject runeTomeGumpObject): base(10, 10)
        {
            m_Player = player;
            m_RuneTomeGumpObject = runeTomeGumpObject;            

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_RuneTomeGumpObject == null) return;
            if (m_RuneTomeGumpObject.m_RuneTome == null) return;
            if (m_RuneTomeGumpObject.m_RuneTome.Deleted) return;           

            #region Background

            AddImage(208, 202, 11015, 2415);
            AddImage(207, 10, 11015, 2415);
            AddImage(6, 201, 11015, 2415);
            AddImage(6, 10, 11015, 2415);           
            AddImage(46, 15, 2081, 2499);
            AddImage(46, 84, 2081, 2499);
            AddImage(46, 150, 2081, 2499);
            AddImage(46, 215, 2081, 2499);
            AddImage(44, 344, 2081);
            AddImage(46, 283, 2081, 2499);
            AddImage(46, 347, 2081, 2499);
            AddImage(305, 84, 2081, 2499);
            AddImage(305, 279, 2081, 2499);
            AddImage(305, 150, 2081, 2499);
            AddImage(305, 14, 2081, 2499);
            AddImage(305, 215, 2081, 2499);
            AddImage(305, 347, 2081, 2499);

            AddImage(51, 83, 3001, 2415);
            AddImage(60, 83, 3001, 2415);
            AddImage(310, 83, 3001, 2415);
            AddImage(319, 83, 3001, 2415);

            AddImageTiled(304, 8, 6, 410, 2701);

            #endregion
            
            RuneTome runeTome = m_RuneTomeGumpObject.m_RuneTome;

            if (!runeTome.CanAccess(m_Player))
            {
                m_Player.SendMessage("That is no longer accessible.");
                return;
            }

            string displayName = runeTome.DisplayName;

            if (displayName == "")
                displayName = "Rune Tome";

            string accessLevelText = "Owner";

            switch (runeTome.LockedDownAccessLevel)
            {
                case RuneTome.LockedDownAccessLevelType.Owner: accessLevelText = "Owner"; break;
                case RuneTome.LockedDownAccessLevelType.CoOwner: accessLevelText = "Co-Owners"; break;
                case RuneTome.LockedDownAccessLevelType.Friend: accessLevelText = "Friends"; break;
                case RuneTome.LockedDownAccessLevelType.Anyone: accessLevelText = "Anyone"; break;
            }

            bool hasChargeAccess = runeTome.HasChargeAccess(m_Player);     

            //Guide           
            AddButton(29, 6, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -4, 149, "Guide");

            //Header
            AddImage(168, 2, 1143, 2499);
            AddLabel(Utility.CenteredTextOffset(315, displayName), 3, 149, displayName);

            if (m_RuneTomeGumpObject.m_RuneTomePageType == PageType.EntryDetail)
            {
                if (runeTome.m_RecallRuneEntries.Count == 0)
                {
                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                    m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                }

                else if (m_RuneTomeGumpObject.m_SelectedRuneEntry == null)
                    m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[0];

                else
                {
                    if (runeTome.m_RecallRuneEntries.IndexOf(m_RuneTomeGumpObject.m_SelectedRuneEntry) == -1)                    
                        m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[0];                    
                }
            }

            if (m_RuneTomeGumpObject.m_SelectedRuneEntry == null)
            {
                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
            }

            switch (m_RuneTomeGumpObject.m_RuneTomePageType)
            {
                #region Overview 

                case PageType.Overview:
                    AddLabel(85, 30, 2603, "Recall Charges");
                    AddItem(174, 30, 8012);
                    if (runeTome.RecallCharges > 0)
                        AddLabel(215, 30, WhiteTextHue, runeTome.RecallCharges.ToString() + "/" + RuneTome.MaxRecallCharges.ToString());
                    else
                        AddLabel(215, 30, WhiteTextHue, runeTome.RecallCharges.ToString() + "/" + RuneTome.MaxRecallCharges.ToString());

                    AddLabel(93, 55, 2629, "Gate Charges");
                    AddItem(174, 55, 8032);
                    if (runeTome.GateCharges > 0)
                        AddLabel(215, 55, WhiteTextHue, runeTome.GateCharges.ToString() + "/" + RuneTome.MaxGateCharges.ToString());
                    else
                        AddLabel(215, 55, WhiteTextHue, runeTome.GateCharges.ToString() + "/" + RuneTome.MaxGateCharges.ToString());

                    if (hasChargeAccess)
                    {
                        AddLabel(322, 35, 90, "Charge Use Allowed For");
                        AddLabel(Utility.CenteredTextOffset(395, accessLevelText), 55, 2562, accessLevelText);

                        AddButton(322, 58, 2223, 2223, 4, GumpButtonType.Reply, 0);
                        AddButton(444, 59, 2224, 2224, 5, GumpButtonType.Reply, 0);

                        AddLabel(479, 23, 2599, "Reorder");
                        if (m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder)
                            AddButton(530, 20, 9730, 9727, 2, GumpButtonType.Reply, 0);
                        else
                            AddButton(530, 20, 9727, 9730, 2, GumpButtonType.Reply, 0);

                        AddLabel(482, 55, 149, "Rename");
                        AddButton(530, 53, 9721, 9724, 3, GumpButtonType.Reply, 0);
                    }

                    else
                    {
                        AddLabel(361, 35, 90, "Charge Use Allowed For");
                        AddLabel(Utility.CenteredTextOffset(440, accessLevelText), 55, 2562, accessLevelText);
                    }

                    int leftStartX = 105;
                    int leftStartY = 92;

                    int rightStartX = 365;
                    int rightStartY = 92;

                    int rowSpacing = 23;

                    for (int a = 0; a < runeTome.m_RecallRuneEntries.Count; a++)
                    {
                        RuneTomeRuneEntry recallRuneEntry = runeTome.m_RecallRuneEntries[a];

                        //Left
                        if (a < RuneTome.EntriesPerSide)
                        {
                            AddButton(leftStartX - 48, leftStartY + 2, 2117, 2118, 10 + (a * 10), GumpButtonType.Reply, 0);
                            AddButton(leftStartX - 25, leftStartY, 210, 211, 10 + (a * 10) + 1, GumpButtonType.Reply, 0);

                            string runeName = recallRuneEntry.m_Description;

                            if (runeName.Length > MaxRuneDescriptionLength)
                                runeName = runeName.Substring(0, MaxRuneDescriptionLength);

                            if (recallRuneEntry.m_IsDefaultRune)
                                AddLabel(leftStartX, leftStartY, 63, runeName);
                            else
                                AddLabel(leftStartX, leftStartY, WhiteTextHue, runeName);

                            if (m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder)
                            {
                                if (a > 0)
                                    AddButton(leftStartX + 160, leftStartY + 1, 5600, 5604, 10 + (a * 10) + 2, GumpButtonType.Reply, 0);
                                
                                if (a < runeTome.m_RecallRuneEntries.Count - 1)
                                    AddButton(leftStartX + 175, leftStartY + 2, 5606, 5602, 10 + (a * 10) + 3, GumpButtonType.Reply, 0);
                            }

                            leftStartY += rowSpacing;
                        }

                        //Right
                        else
                        {
                            AddButton(rightStartX - 48, rightStartY + 2, 2117, 2118, 10 + (a * 10), GumpButtonType.Reply, 0);
                            AddButton(rightStartX - 25, rightStartY, 210, 211, 10 + (a * 10) + 1, GumpButtonType.Reply, 0);

                            string runeName = recallRuneEntry.m_Description;

                            if (runeName.Length > MaxRuneDescriptionLength)
                                runeName = runeName.Substring(0, MaxRuneDescriptionLength);

                            if (recallRuneEntry.m_IsDefaultRune)
                                AddLabel(rightStartX, rightStartY, 63, runeName);
                            else
                                AddLabel(rightStartX, rightStartY, WhiteTextHue, runeName);

                            if (m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder)
                            {
                                if (a > 0)
                                    AddButton(rightStartX + 160, rightStartY + 1, 5600, 5604, 10 + (a * 10) + 2, GumpButtonType.Reply, 0);

                                if (a < runeTome.m_RecallRuneEntries.Count - 1)
                                    AddButton(rightStartX + 175, rightStartY + 2, 5606, 5602, 10 + (a * 10) + 3, GumpButtonType.Reply, 0);
                            }

                            rightStartY += rowSpacing;
                        }
                    }		       
                break;

                #endregion

                #region Entry Detail

                case PageType.EntryDetail:
                    RuneTomeRuneEntry leftRecallRuneEntry = null;
                    RuneTomeRuneEntry rightRecallRuneEntry = null;

                    int selectedEntryIndex = runeTome.m_RecallRuneEntries.IndexOf(m_RuneTomeGumpObject.m_SelectedRuneEntry);
                    int pageIndex = (int)(Math.Floor(((double)selectedEntryIndex / 2)));
                    int totalPages = (int)(Math.Ceiling((double)runeTome.m_RecallRuneEntries.Count / 2));
                    
                    //Selected is on Left
                    if (selectedEntryIndex % 2 == 0)
                    {
                        leftRecallRuneEntry = m_RuneTomeGumpObject.m_SelectedRuneEntry;

                        if (runeTome.m_RecallRuneEntries.Count > selectedEntryIndex + 1)
                            rightRecallRuneEntry = runeTome.m_RecallRuneEntries[selectedEntryIndex + 1];
                    }

                    //Selected is on Right
                    else
                    {                       
                        if (runeTome.m_RecallRuneEntries.Count > 1)
                            leftRecallRuneEntry = runeTome.m_RecallRuneEntries[selectedEntryIndex - 1];

                        rightRecallRuneEntry = m_RuneTomeGumpObject.m_SelectedRuneEntry;
                    }        
            
                    //Top
                    AddLabel(85, 30, 2603, "Recall Charges");
			        AddItem(174, 30, 8012);
                    AddLabel(215, 30, WhiteTextHue, runeTome.RecallCharges.ToString() + "/" + RuneTome.MaxRecallCharges.ToString());
                    AddLabel(101, 55, 2603, "(Requires 20 Magery)");

                    AddLabel(370, 30, 2629, "Gate Charges");
			        AddItem(451, 30, 8032);
                    AddLabel(492, 30, WhiteTextHue, runeTome.GateCharges.ToString() + "/" + RuneTome.MaxGateCharges.ToString());
                    AddLabel(378, 55, 2629, "(Requires 50 Magery)");

                    //Left
                    if (leftRecallRuneEntry != null)
                    {
                        string coordinatesText = ""; 

                        int xLong = 0, yLat = 0;
                        int xMins = 0, yMins = 0;
                        bool xEast = false, ySouth = false;

                        if (Sextant.Format(leftRecallRuneEntry.m_Target, leftRecallRuneEntry.m_TargetMap, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                        {
                            coordinatesText = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                            coordinatesText += "  (" + leftRecallRuneEntry.m_Target.X.ToString() + ", " + leftRecallRuneEntry.m_Target.Y.ToString() + ")";
                        }

                        string runeName = leftRecallRuneEntry.m_Description;

                        if (runeName.Length > MaxRuneDescriptionLength)
                            runeName = runeName.Substring(0, MaxRuneDescriptionLength);

                        if (leftRecallRuneEntry.m_IsDefaultRune)
                            AddLabel(Utility.CenteredTextOffset(190, runeName), 92, 63, runeName);
                        else
                            AddLabel(Utility.CenteredTextOffset(190, runeName), 92, 2550, runeName);

                        AddLabel(Utility.CenteredTextOffset(190, coordinatesText), 112, WhiteTextHue, coordinatesText);

                        //Recall
                        AddButton(68, 151, 2271, 2271, 5, GumpButtonType.Reply, 0);
                        if (hasChargeAccess)
                        {
                            AddItem(171, 163, 8012, 0);
                            AddLabel(207, 163, 2603, "->");
                            AddButton(231, 151, 2271, 2271, 6, GumpButtonType.Reply, 0);
                        }

                        //Gate
                        AddButton(68, 207, 2291, 2291, 7, GumpButtonType.Reply, 0);
                        if (hasChargeAccess)
                        {
                            AddItem(171, 219, 8032, 0);
                            AddLabel(207, 219, 2629, "->");
                            AddButton(231, 207, 2291, 2291, 8, GumpButtonType.Reply, 0);
                        }

                        if (hasChargeAccess)
                        {
                            AddItem(65, 267, 7956);
                            AddButton(116, 260, 9721, 9724, 9, GumpButtonType.Reply, 0);
                            AddLabel(153, 264, WhiteTextHue, "Drop Rune");

                            AddItem(65, 309, 7956, 63);
                            AddButton(116, 301, 9721, 9724, 10, GumpButtonType.Reply, 0);
                            AddLabel(153, 305, WhiteTextHue, "Set as Default Rune");

                            AddItem(70, 344, 4031, 0);
                            AddButton(116, 343, 9721, 9724, 11, GumpButtonType.Reply, 0);
                            AddLabel(153, 347, WhiteTextHue, "Rename Rune");
                        }
                    }

                    //Right
                    if (rightRecallRuneEntry != null)
                    {
                        string coordinatesText = "";

                        int xLong = 0, yLat = 0;
                        int xMins = 0, yMins = 0;
                        bool xEast = false, ySouth = false;

                        if (Sextant.Format(rightRecallRuneEntry.m_Target, rightRecallRuneEntry.m_TargetMap, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                        {
                            coordinatesText = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                            coordinatesText += "  (" + rightRecallRuneEntry.m_Target.X.ToString() + ", " + rightRecallRuneEntry.m_Target.Y.ToString() + ")";
                        }

                        string runeName = rightRecallRuneEntry.m_Description;

                        if (runeName.Length > MaxRuneDescriptionLength)
                            runeName = runeName.Substring(0, MaxRuneDescriptionLength);

                        if (rightRecallRuneEntry.m_IsDefaultRune)
                            AddLabel(Utility.CenteredTextOffset(450, runeName), 92, 63, runeName);
                        else
                            AddLabel(Utility.CenteredTextOffset(450, runeName), 92, 2550, runeName);

                        AddLabel(Utility.CenteredTextOffset(450, coordinatesText), 112, WhiteTextHue, coordinatesText);
                        
                        AddButton(329, 151, 2271, 2271, 12, GumpButtonType.Reply, 0);
                        if (hasChargeAccess)
                        {
                            AddItem(432, 163, 8012, 0);
                            AddLabel(468, 163, 2603, "->");
                            AddButton(494, 151, 2271, 2271, 13, GumpButtonType.Reply, 0);
                        }

                        AddButton(329, 207, 2291, 2291, 14, GumpButtonType.Reply, 0);
                        if (hasChargeAccess)
                        {
                            AddItem(432, 219, 8032, 0);
                            AddLabel(469, 219, 2629, "->");
                            AddButton(494, 207, 2291, 2291, 15, GumpButtonType.Reply, 0);
                        }

                        if (hasChargeAccess)
                        {
                            AddItem(326, 267, 7956);
                            AddButton(377, 260, 9721, 9724, 16, GumpButtonType.Reply, 0);
                            AddLabel(414, 264, WhiteTextHue, "Drop Rune");

                            AddItem(326, 309, 7956, 63);
                            AddButton(377, 301, 9721, 9724, 17, GumpButtonType.Reply, 0);
                            AddLabel(414, 305, WhiteTextHue, "Set as Default Rune");

                            AddItem(331, 344, 4031, 0);
                            AddButton(377, 343, 9721, 9724, 18, GumpButtonType.Reply, 0);
                            AddLabel(414, 347, WhiteTextHue, "Rename Rune");
                        }
                    }

                    //Bottom
                    if (pageIndex > 0)
                    {
                        AddButton(56, 390, 4014, 4016, 2, GumpButtonType.Reply, 0);
			            AddLabel(90, 390, 149, "Previous");
                    }

                    AddButton(291, 390, 4011, 4013, 4, GumpButtonType.Reply, 0);
			        AddLabel(324, 390, 149, "Main");

                    if (pageIndex < totalPages - 1)
                    {
			            AddButton(523, 390, 4005, 4007, 3, GumpButtonType.Reply, 0);
			            AddLabel(488, 390, 149, "Next");
                    }
                break;
            }

                #endregion
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Player.Backpack == null) return;
            if (m_RuneTomeGumpObject == null) return;
            if (m_RuneTomeGumpObject.m_RuneTome == null) return;
            if (m_RuneTomeGumpObject.m_RuneTome.Deleted) return;

            RuneTome runeTome = m_RuneTomeGumpObject.m_RuneTome;

            if (!runeTome.CanAccess(m_Player))
            {
                m_Player.SendMessage("That is no longer accessible.");
                return;
            }

            bool closeGump = true;

            bool hasChargeAccess = runeTome.HasChargeAccess(m_Player);     

            switch (m_RuneTomeGumpObject.m_RuneTomePageType)
            {
                #region Overview 

                case PageType.Overview:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Toggle Rearrange Rune Order
                        case 2:
                            if (hasChargeAccess)
                            {
                                m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder = !m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder;

                                m_Player.SendSound(SelectionSound);
                            }

                            else
                                m_Player.SendMessage("You do not have the neccessary access rights required to reorder runes in this rune tome.");

                            closeGump = false;
                        break;

                        //Rename Runebook
                        case 3:
                            if (hasChargeAccess)
                            {
                                m_Player.SendMessage("What do you wish to rename this rune tome to?");
                                m_Player.Prompt = new RuneTomeRenamePrompt(m_Player, m_RuneTomeGumpObject);

                                m_Player.SendSound(SelectionSound);
                            }

                            else
                                m_Player.SendMessage("You do not have the neccessary access rights to rename this rune tome.");

                            closeGump = false;
                        break;                        

                        //Previous Access Level
                        case 4:
                            if (runeTome.IsChildOf(m_Player.Backpack))
                            {
                                switch (runeTome.LockedDownAccessLevel)
                                {
                                    case RuneTome.LockedDownAccessLevelType.Owner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Anyone; break;
                                    case RuneTome.LockedDownAccessLevelType.CoOwner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Owner; break;
                                    case RuneTome.LockedDownAccessLevelType.Friend: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.CoOwner; break;
                                    case RuneTome.LockedDownAccessLevelType.Anyone: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Friend; break;
                                }

                                m_Player.SendSound(SelectionSound);
                            }

                            else
                                m_Player.SendMessage("A rune tome must be in your backpack if you wish to change it's access level.");

                            closeGump = false;
                        break;

                        //Next Access Level
                        case 5:
                            if (runeTome.IsChildOf(m_Player.Backpack))
                            {
                                switch (runeTome.LockedDownAccessLevel)
                                {
                                    case RuneTome.LockedDownAccessLevelType.Owner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.CoOwner; break;
                                    case RuneTome.LockedDownAccessLevelType.CoOwner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Friend; break;
                                    case RuneTome.LockedDownAccessLevelType.Friend: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Anyone; break;
                                    case RuneTome.LockedDownAccessLevelType.Anyone: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Owner; break;
                                }

                                m_Player.SendSound(SelectionSound);
                            }

                            else
                                m_Player.SendMessage("A rune tome must be in your backpack if you wish to change it's access level.");

                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10)
                    {
                        int runeSelectionIndex = (int)(Math.Floor((double)info.ButtonID / 10)) - 1;                      
                        int buttonIndex = info.ButtonID % 10;

                        if (runeSelectionIndex < runeTome.m_RecallRuneEntries.Count)
                        {                            
                            switch (buttonIndex)
                            {
                                //Recall
                                case 0:
                                    RuneTomeRuneEntry runeEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                    if (hasChargeAccess)
                                    {
                                        if (!hasChargeAccess)
                                            m_Player.SendMessage("You do not have the neccessary access rights required to use recall charges from this rune tome.");

                                        else if (runeTome.RecallCharges == 0)
                                            m_Player.SendMessage("This rune tome is out of recall charges.");

                                        else
                                        {
                                            new RecallSpell(m_Player, runeTome, null, null, runeEntry, runeTome).Cast();

                                            runeTome.Openers.Remove(m_Player);

                                            m_Player.CloseGump(typeof(RuneTomeGump));
                                            m_Player.CloseRunebookGump = true;

                                            return;
                                        }
                                    }

                                    else
                                        m_Player.SendMessage("You do not have the neccessary access rights required to use recall charges from this rune tome.");
                                break;

                                //Open Detail View
                                case 1:
                                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.EntryDetail;
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                    m_Player.SendSound(OpenGumpSound);
                                break;

                                //Move Up
                                case 2:
                                    if (hasChargeAccess && m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder)
                                    {
                                        if (runeSelectionIndex > 0)
                                        {
                                            runeEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                            runeTome.m_RecallRuneEntries.Remove(runeEntry);
                                            runeTome.m_RecallRuneEntries.Insert(runeSelectionIndex - 1, runeEntry);

                                            m_Player.SendSound(SelectionSound);
                                        }
                                    }

                                    else
                                        m_Player.SendMessage("You do not have the neccessary access rights required to do that.");
                                break;

                                //Move Down
                                case 3:
                                    if (hasChargeAccess && m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder)
                                    {
                                        if (runeSelectionIndex < runeTome.m_RecallRuneEntries.Count - 1)
                                        {
                                            runeEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                            runeTome.m_RecallRuneEntries.Remove(runeEntry);

                                            if (runeSelectionIndex == runeTome.m_RecallRuneEntries.Count - 1)
                                                runeTome.m_RecallRuneEntries.Add(runeEntry);

                                            else
                                                runeTome.m_RecallRuneEntries.Insert(runeSelectionIndex + 1, runeEntry);

                                            m_Player.SendSound(SelectionSound);
                                        }
                                    }

                                    else
                                        m_Player.SendMessage("You do not have the neccessary access rights required to do that.");
                                break;
                            }
                        }

                        closeGump = false;
                    }
                break;

                #endregion

                #region Entry Detail

                case PageType.EntryDetail: 
                    RuneTomeRuneEntry leftRecallRuneEntry = null;
                    RuneTomeRuneEntry rightRecallRuneEntry = null;

                    if (m_RuneTomeGumpObject.m_SelectedRuneEntry != null)
                    {
                        int selectedEntryIndex = runeTome.m_RecallRuneEntries.IndexOf(m_RuneTomeGumpObject.m_SelectedRuneEntry);
                        int pageIndex = (int)(Math.Floor(((double)selectedEntryIndex / 2)));
                        int totalPages = (int)(Math.Ceiling((double)runeTome.m_RecallRuneEntries.Count / 2));

                        //Selected is on Left
                        if (selectedEntryIndex % 2 == 0)
                        {
                            leftRecallRuneEntry = m_RuneTomeGumpObject.m_SelectedRuneEntry;

                            if (runeTome.m_RecallRuneEntries.Count > selectedEntryIndex + 1)
                                rightRecallRuneEntry = runeTome.m_RecallRuneEntries[selectedEntryIndex + 1];
                        }

                        //Selected is on Right
                        else
                        {
                            if (runeTome.m_RecallRuneEntries.Count > 1)
                                leftRecallRuneEntry = runeTome.m_RecallRuneEntries[selectedEntryIndex - 1];

                            rightRecallRuneEntry = m_RuneTomeGumpObject.m_SelectedRuneEntry;
                        } 
                    }

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Page
                        case 2:
                            if (runeTome.m_RecallRuneEntries.Count == 0)
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            else if (m_RuneTomeGumpObject.m_SelectedRuneEntry == null)
                            {
                                if (runeTome.m_RecallRuneEntries.Count > 0)                                
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[0];                                

                                else
                                {
                                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                                }
                            }

                            else
                            {
                                int index = runeTome.m_RecallRuneEntries.IndexOf(m_RuneTomeGumpObject.m_SelectedRuneEntry);
                                
                                if (index == -1)
                                {
                                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                                }

                                else
                                {
                                    int pageIndex = (int)(Math.Floor(((double)index / 2)));

                                    if (pageIndex > 0)
                                    {
                                        int newIndex = (pageIndex - 1) * 2;

                                        m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[newIndex];
                                    }
                                }
                            }

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Next Page
                        case 3:
                            if (runeTome.m_RecallRuneEntries.Count == 0)
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            else if (m_RuneTomeGumpObject.m_SelectedRuneEntry == null)
                            {
                                if (runeTome.m_RecallRuneEntries.Count > 0)
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[0];

                                else
                                {
                                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                                }
                            }

                            else
                            {
                                int index = runeTome.m_RecallRuneEntries.IndexOf(m_RuneTomeGumpObject.m_SelectedRuneEntry);

                                if (index == -1)
                                {
                                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                                }

                                else
                                {
                                    int pageIndex = (int)(Math.Floor(((double)index / 2)));
                                    int totalPages = (int)(Math.Floor((double)runeTome.m_RecallRuneEntries.Count / 2)) + 1;

                                    if (pageIndex + 1 < totalPages)
                                    {
                                        int newIndex = (pageIndex + 1) * 2;

                                        m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[newIndex];
                                    }
                                }
                            }

                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Overview
                        case 4:
                            m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                            m_RuneTomeGumpObject.m_SelectedRuneEntry = null;

                            m_Player.SendSound(OpenGumpSound);

                            closeGump = false;
                        break;

                        #region Left

                        //Recall
                        case 5:
                            if (leftRecallRuneEntry != null)
                            {
                                new RecallSpell(m_Player, null, null, null, leftRecallRuneEntry, null).Cast();

                                runeTome.Openers.Remove(m_Player);

                                m_Player.CloseGump(typeof(RuneTomeGump));
                                m_Player.CloseRunebookGump = true;

                                return;
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;                       

                        //Recall Via Charge
                        case 6:
                            if (leftRecallRuneEntry != null)
                            {
                                if (!hasChargeAccess)
                                    m_Player.SendMessage("You do not have the neccessary access rights required to use recall charges from this rune tome.");

                                else if (runeTome.RecallCharges == 0)
                                    m_Player.SendMessage("This rune tome is out of recall charges.");

                                else
                                {
                                    new RecallSpell(m_Player, runeTome, null, null, leftRecallRuneEntry, runeTome).Cast();

                                    runeTome.Openers.Remove(m_Player);

                                    m_Player.CloseGump(typeof(RuneTomeGump));
                                    m_Player.CloseRunebookGump = true;

                                    return;
                                }
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }
                            
                            closeGump = false;
                        break;

                        //Gate
                        case 7:
                            if (leftRecallRuneEntry != null)
                            {
                                new GateTravelSpell(m_Player, null, null, null, leftRecallRuneEntry, null).Cast();

                                runeTome.Openers.Remove(m_Player);

                                m_Player.CloseGump(typeof(RuneTomeGump));
                                m_Player.CloseRunebookGump = true;

                                return;
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Gate via Charge
                        case 8:
                            if (leftRecallRuneEntry != null)
                            {
                                if (!hasChargeAccess)
                                    m_Player.SendMessage("You do not have the neccessary access rights required to use gate charges from this rune tome.");

                                else if (runeTome.GateCharges == 0)
                                    m_Player.SendMessage("This rune tome is out of gate charges.");

                                else
                                {
                                    new GateTravelSpell(m_Player, runeTome, null, null, leftRecallRuneEntry, runeTome).Cast();

                                    runeTome.Openers.Remove(m_Player);

                                    m_Player.CloseGump(typeof(RuneTomeGump));
                                    m_Player.CloseRunebookGump = true;

                                    return;
                                }
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Drop Rune
                        case 9:
                            if (leftRecallRuneEntry != null)
                            {
                                if (hasChargeAccess)
                                {
                                    if (m_Player.Backpack.TotalItems >= m_Player.Backpack.MaxItems)
                                        m_Player.SendMessage("You do not have enough space in your backpack for this item. Remove some items and try again.");

                                    else
                                    {
                                        bool wasDefaultRune = false;

                                        if (leftRecallRuneEntry.m_IsDefaultRune)                                        
                                            wasDefaultRune = true;                                        

                                        RecallRune recallRune = new RecallRune();

                                        recallRune.Description = leftRecallRuneEntry.m_Description;
                                        recallRune.House = leftRecallRuneEntry.m_House;
                                        recallRune.Target = leftRecallRuneEntry.m_Target;
                                        recallRune.TargetMap = leftRecallRuneEntry.m_TargetMap;
                                        recallRune.Marked = true;

                                        m_Player.AddToBackpack(recallRune);
                                        m_Player.SendMessage("You remove the rune from the runebook.");

                                        if (m_RuneTomeGumpObject.m_SelectedRuneEntry == leftRecallRuneEntry)
                                            m_RuneTomeGumpObject.m_SelectedRuneEntry = null;

                                        if (runeTome.m_RecallRuneEntries.Contains(leftRecallRuneEntry))
                                            runeTome.m_RecallRuneEntries.Remove(leftRecallRuneEntry);

                                        if (wasDefaultRune && runeTome.m_RecallRuneEntries.Count > 0)
                                        {
                                            if (runeTome.m_RecallRuneEntries[0] != null)
                                                runeTome.m_RecallRuneEntries[0].m_IsDefaultRune = true;
                                        }

                                        m_Player.SendSound(0x42);
                                    }                                    
                                }

                                else                                
                                    m_Player.SendMessage("You do not have the neccessary access rights required to remove this rune.");
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Set Rune as Default
                        case 10:
                            if (leftRecallRuneEntry != null)
                            {
                                if (hasChargeAccess)
                                {
                                    foreach (RuneTomeRuneEntry runeEntry in runeTome.m_RecallRuneEntries)
                                    {
                                        if (runeEntry == null) 
                                            continue;

                                        if (runeEntry == leftRecallRuneEntry)
                                            runeEntry.m_IsDefaultRune = true;

                                        else
                                            runeEntry.m_IsDefaultRune = false;
                                    }

                                    m_Player.SendMessage("You set the rune as the new default for this rune tome.");

                                    m_Player.SendSound(SelectionSound);
                                }     

                                else
                                    m_Player.SendMessage("You do not have the neccessary access rights required to set this rune as the default for this rune tome.");
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Rename Rune
                        case 11:
                            if (leftRecallRuneEntry != null)
                            {
                                if (hasChargeAccess)
                                {
                                    m_Player.SendMessage("What do you wish to rename this recall rune to?");
                                    m_Player.Prompt = new RuneTomeRuneEntryRenamePrompt(m_Player, m_RuneTomeGumpObject, leftRecallRuneEntry);
                                    
                                    m_Player.SendSound(SelectionSound);
                                }

                                else
                                    m_Player.SendMessage("You do not have the neccessary access rights required to rename this rune.");
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        #endregion

                        #region Right

                        //Recall
                        case 12:
                            if (rightRecallRuneEntry != null)
                            {
                                new RecallSpell(m_Player, null, null, null, rightRecallRuneEntry, null).Cast();

                                runeTome.Openers.Remove(m_Player);

                                m_Player.CloseGump(typeof(RuneTomeGump));
                                m_Player.CloseRunebookGump = true;

                                return;
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Recall Via Charge
                        case 13:
                            if (rightRecallRuneEntry != null)
                            {                                
                                if (!hasChargeAccess)
                                    m_Player.SendMessage("You do not have the neccessary access rights required to use recall charges from this rune tome.");

                                else if (runeTome.RecallCharges == 0)
                                    m_Player.SendMessage("This rune tome is out of recall charges.");

                                else
                                {
                                    new RecallSpell(m_Player, runeTome, null, null, rightRecallRuneEntry, runeTome).Cast();

                                    runeTome.Openers.Remove(m_Player);

                                    m_Player.CloseGump(typeof(RuneTomeGump));
                                    m_Player.CloseRunebookGump = true;

                                    return;
                                }                                
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }
                            
                            closeGump = false;
                        break;

                        //Gate:
                        case 14:
                            if (rightRecallRuneEntry != null)
                            {
                                new GateTravelSpell(m_Player, null, null, null, rightRecallRuneEntry, null).Cast();

                                runeTome.Openers.Remove(m_Player);

                                m_Player.CloseGump(typeof(RuneTomeGump));
                                m_Player.CloseRunebookGump = true;
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;                      

                        //Gate Via Charge
                        case 15:
                            if (rightRecallRuneEntry != null)
                            {
                                if (!hasChargeAccess)
                                    m_Player.SendMessage("You do not have the neccessary access rights required to use gate charges from this rune tome.");

                                else if (runeTome.GateCharges == 0)
                                    m_Player.SendMessage("This rune tome is out of gate charges.");

                                else
                                {
                                    new GateTravelSpell(m_Player, runeTome, null, null, rightRecallRuneEntry, runeTome).Cast();

                                    runeTome.Openers.Remove(m_Player);

                                    m_Player.CloseGump(typeof(RuneTomeGump));
                                    m_Player.CloseRunebookGump = true;

                                    return;
                                }
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }
                            
                            closeGump = false;
                        break;

                        //Drop Rune
                        case 16:
                            if (rightRecallRuneEntry != null)
                            {
                                if (hasChargeAccess)
                                {
                                    bool wasDefaultRune = false;

                                    if (rightRecallRuneEntry.m_IsDefaultRune)
                                        wasDefaultRune = true;   

                                    RecallRune recallRune = new RecallRune();

                                    recallRune.Description = rightRecallRuneEntry.m_Description;
                                    recallRune.House = rightRecallRuneEntry.m_House;
                                    recallRune.Target = rightRecallRuneEntry.m_Target;
                                    recallRune.TargetMap = rightRecallRuneEntry.m_TargetMap;
                                    recallRune.Marked = true;

                                    m_Player.AddToBackpack(recallRune);
                                    m_Player.SendMessage("You remove the rune from the runebook.");

                                    if (m_RuneTomeGumpObject.m_SelectedRuneEntry == rightRecallRuneEntry)
                                        m_RuneTomeGumpObject.m_SelectedRuneEntry = null;

                                    if (runeTome.m_RecallRuneEntries.Contains(rightRecallRuneEntry))
                                        runeTome.m_RecallRuneEntries.Remove(rightRecallRuneEntry);

                                    if (wasDefaultRune && runeTome.m_RecallRuneEntries.Count > 0)
                                    {
                                        if (runeTome.m_RecallRuneEntries[0] != null)
                                            runeTome.m_RecallRuneEntries[0].m_IsDefaultRune = true;
                                    }

                                    m_Player.SendSound(0x42);
                                }

                                else
                                    m_Player.SendMessage("You do not have the neccessary access rights required to use recall charges from this rune tome.");
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Set Rune as Default
                        case 17:
                            if (rightRecallRuneEntry != null)
                            {
                                if (hasChargeAccess)
                                {
                                    foreach (RuneTomeRuneEntry runeEntry in runeTome.m_RecallRuneEntries)
                                    {
                                        if (runeEntry == null)
                                            continue;

                                        if (runeEntry == rightRecallRuneEntry)
                                            runeEntry.m_IsDefaultRune = true;

                                        else
                                            runeEntry.m_IsDefaultRune = false;
                                    }

                                    m_Player.SendMessage("You set the rune as the new default for this rune tome.");

                                    m_Player.SendSound(SelectionSound);
                                }

                                else
                                    m_Player.SendMessage("You do not have the neccessary access rights required to set this rune as the rune tome default.");
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            closeGump = false;
                        break;

                        //Rename Rune
                        case 18:
                            if (rightRecallRuneEntry != null)
                            {
                                if (hasChargeAccess)
                                {
                                    m_Player.SendMessage("What do you wish to rename this recall rune to?");
                                    m_Player.Prompt = new RuneTomeRuneEntryRenamePrompt(m_Player, m_RuneTomeGumpObject, rightRecallRuneEntry);
                                }

                                else
                                    m_Player.SendMessage("You do not have the neccessary access rights required to rename this rune.");
                            }

                            else
                            {
                                m_RuneTomeGumpObject.m_RuneTomePageType = PageType.Overview;
                                m_RuneTomeGumpObject.m_SelectedRuneEntry = null;
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        #endregion
                    }
                break;

                #endregion
            }            

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(RuneTomeGump));
                m_Player.SendGump(new RuneTomeGump(m_Player, m_RuneTomeGumpObject));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }

        private class RuneTomeRenamePrompt : Prompt
        {
            private PlayerMobile m_Player;
            private RuneTomeGumpObject m_RuneTomeGumpObject;

            public RuneTomeRenamePrompt(PlayerMobile player, RuneTomeGumpObject runeTomeGumpObject)
            {
                m_Player = player;
                m_RuneTomeGumpObject = runeTomeGumpObject;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Player == null) return;
                if (m_Player.Deleted) return;
                if (m_RuneTomeGumpObject == null) return;
                if (m_RuneTomeGumpObject.m_RuneTome == null) return;
                if (m_RuneTomeGumpObject.m_RuneTome.Deleted) return;

                RuneTome runeTome = m_RuneTomeGumpObject.m_RuneTome;

                if (!runeTome.CanAccess(m_Player))
                {
                    m_Player.SendMessage("That is no longer accessible.");
                    return;
                }

                if (!runeTome.HasChargeAccess(m_Player))
                {
                    m_Player.SendMessage("You no longer have the neccessary access rights required to rename this rune tome.");
                    return;
                }

                string newName = Utility.FixHtml(text.Trim());

                int maxNameLength = 35;

                if (newName.Length == 0)                                              
                        m_Player.SendMessage("Rune tome names must be at least 1 character.");

                else if (newName.Length > maxNameLength)
                    m_Player.SendMessage("Rune tome names may be no longer than " + maxNameLength.ToString() + " characters.");

                //else if (!Guilds.CheckProfanity(newName))
                    //m_Player.SendMessage("That is an unnacceptable name for that rune tome.");

                else
                {
                    runeTome.DisplayName = newName;
                    m_Player.SendMessage("The rune tome's name has changed.");
                }

                from.CloseGump(typeof(RuneTomeGump));
                from.SendGump(new RuneTomeGump(m_Player, m_RuneTomeGumpObject));
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(502415); // Request cancelled.

                if (m_RuneTomeGumpObject != null)
                {
                    from.CloseGump(typeof(RuneTomeGump));
                    from.SendGump(new RuneTomeGump(m_Player, m_RuneTomeGumpObject));
                }
            }
        }

        private class RuneTomeRuneEntryRenamePrompt : Prompt
        {
            private PlayerMobile m_Player;
            private RuneTomeGumpObject m_RuneTomeGumpObject;
            private RuneTomeRuneEntry m_RuneEntry;

            public RuneTomeRuneEntryRenamePrompt(PlayerMobile player, RuneTomeGumpObject runeTomeGumpObject, RuneTomeRuneEntry runeEntry)
            {
                m_Player = player;
                m_RuneTomeGumpObject = runeTomeGumpObject;
                m_RuneEntry = runeEntry;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Player == null) return;
                if (m_Player.Deleted) return;
                if (m_RuneTomeGumpObject == null) return;
                if (m_RuneTomeGumpObject.m_RuneTome == null) return;
                if (m_RuneTomeGumpObject.m_RuneTome.Deleted) return;

                RuneTome runeTome = m_RuneTomeGumpObject.m_RuneTome;

                if (!runeTome.CanAccess(m_Player))
                {
                    m_Player.SendMessage("That is no longer accessible.");
                    return;
                }

                if (!runeTome.HasChargeAccess(m_Player))
                {
                    m_Player.SendMessage("You no longer have the neccessary access rights required to rename this rune tome.");
                    return;
                }

                bool foundMatch = false;

                foreach (RuneTomeRuneEntry runeEntryInstance in runeTome.m_RecallRuneEntries)
                {
                    if (runeEntryInstance == null)
                        continue;

                    if (runeEntryInstance == m_RuneEntry)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {                    
                    m_Player.SendMessage("That recall rune is no longer accessible.");
                    return;                    
                }

                string newName = Utility.FixHtml(text.Trim());

                int maxNameLength = 35;

                if (newName.Length == 0)
                    m_Player.SendMessage("Recall rune names must be at least 1 character.");

                else if (newName.Length > maxNameLength)
                    m_Player.SendMessage("Recall rune names may be no longer than " + maxNameLength.ToString() + " characters.");

                //else if (!Guilds.CheckProfanity(newName))
                //m_Player.SendMessage("That is an unnacceptable name for that recall rune.");

                else
                {
                    m_RuneEntry.m_Description = newName;
                    m_Player.SendMessage("The recall rune's name has changed.");
                }

                from.CloseGump(typeof(RuneTomeGump));
                from.SendGump(new RuneTomeGump(m_Player, m_RuneTomeGumpObject));
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(502415); // Request cancelled.

                if (m_RuneTomeGumpObject != null)
                {
                    from.CloseGump(typeof(RuneTomeGump));
                    from.SendGump(new RuneTomeGump(m_Player, m_RuneTomeGumpObject));
                }
            }
        }
    }

    public class RuneTomeGumpObject
    {
        public RuneTome m_RuneTome;

        public RuneTomeGump.PageType m_RuneTomePageType = RuneTomeGump.PageType.Overview;       

        public bool m_ToggleRearrangeRuneOrder = false;

        public RuneTomeRuneEntry m_SelectedRuneEntry = null;

        public RuneTomeGumpObject(RuneTome runeTome)
        {
            m_RuneTome = runeTome;
        }
    }
}