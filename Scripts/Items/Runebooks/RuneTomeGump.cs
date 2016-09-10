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

        public static int MaxRecallCharges = 100;
        public static int MaxGateCharges = 25;

        public static int EntriesPerSide = 14;

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

            string displayName = runeTome.DisplayName;

            string accessLevelText = "Owner";

            switch (runeTome.LockedDownAccessLevel)
            {
                case RuneTome.LockedDownAccessLevelType.Owner: accessLevelText = "Owner"; break;
                case RuneTome.LockedDownAccessLevelType.CoOwner: accessLevelText = "Co-Owners"; break;
                case RuneTome.LockedDownAccessLevelType.Friend: accessLevelText = "Friends"; break;
                case RuneTome.LockedDownAccessLevelType.Anyone: accessLevelText = "Anyone"; break;
            }

            //TEST
            bool hasChargeAccess = true;
            bool hasAdminAccess = true;

            //Guide           
            AddButton(29, 6, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -4, 149, "Guide");

            //Header
            AddImage(168, 2, 1143, 2499);
            AddLabel(Utility.CenteredTextOffset(310, displayName), 3, 149, displayName);

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

            switch (m_RuneTomeGumpObject.m_RuneTomePageType)
            {
                #region Overview 

                case PageType.Overview:
                    AddLabel(85, 30, 2603, "Recall Charges");
                    AddItem(174, 30, 8012);
                    if (runeTome.RecallCharges > 0)
                        AddLabel(215, 30, WhiteTextHue, runeTome.RecallCharges.ToString() + "/" + MaxRecallCharges.ToString());
                    else
                        AddLabel(215, 30, WhiteTextHue, runeTome.RecallCharges.ToString() + "/" + MaxRecallCharges.ToString());

                    AddLabel(93, 55, 2629, "Gate Charges");
                    AddItem(174, 55, 8032);
                    if (runeTome.GateCharges > 0)
                        AddLabel(215, 55, WhiteTextHue, runeTome.GateCharges.ToString() + "/" + MaxGateCharges.ToString());
                    else
                        AddLabel(215, 55, WhiteTextHue, runeTome.GateCharges.ToString() + "/" + MaxGateCharges.ToString());

                    if (hasAdminAccess)
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

                    //--------------

                    //TEST
                    if (runeTome.m_RecallRuneEntries.Count == 0)
                    {
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(true, "East Prevalia Bank", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 1", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 2", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 3", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 4", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 5", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 6", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 7", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 8", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 9", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 10", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 11", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 12", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 13", new Point3D(0, 0, 0), Map.Felucca, null));
                        runeTome.m_RecallRuneEntries.Add(new RecallRuneEntry(false, "Prevalia 14", new Point3D(0, 0, 0), Map.Felucca, null));
                    }

                    int leftStartX = 105;
                    int leftStartY = 92;

                    int rightStartX = 365;
                    int rightStartY = 92;

                    int rowSpacing = 23;

                    for (int a = 0; a < runeTome.m_RecallRuneEntries.Count; a++)
                    {
                        RecallRuneEntry recallRuneEntry = runeTome.m_RecallRuneEntries[a];

                        //Left
                        if (a < EntriesPerSide)
                        {
                            AddButton(leftStartX - 48, leftStartY + 2, 2117, 2118, 10 + (a * 10), GumpButtonType.Reply, 0);
                            AddButton(leftStartX - 25, leftStartY, 210, 211, 10 + (a * 10) + 1, GumpButtonType.Reply, 0);

                            if (recallRuneEntry.m_IsDefaultRune)
                                AddLabel(leftStartX, leftStartY, 63, recallRuneEntry.m_Description);
                            else
                                AddLabel(leftStartX, leftStartY, WhiteTextHue, recallRuneEntry.m_Description);

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

                            if (recallRuneEntry.m_IsDefaultRune)
                                AddLabel(rightStartX, rightStartY, 63, recallRuneEntry.m_Description);
                            else
                                AddLabel(rightStartX, rightStartY, WhiteTextHue, recallRuneEntry.m_Description);

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
                    RecallRuneEntry leftRecallRuneEntry = null;
                    RecallRuneEntry rightRecallRuneEntry = null;

                    int selectedEntryIndex = runeTome.m_RecallRuneEntries.IndexOf(m_RuneTomeGumpObject.m_SelectedRuneEntry);
                    int pageIndex = (int)(Math.Floor(((double)selectedEntryIndex / 2)));
                    int totalPages = (int)(Math.Floor((double)runeTome.m_RecallRuneEntries.Count / 2)) + 1;
                    
                    //Selected is on Left
                    if (selectedEntryIndex % 2 == 0)
                    {
                        leftRecallRuneEntry = m_RuneTomeGumpObject.m_SelectedRuneEntry;

                        if (selectedEntryIndex < runeTome.m_RecallRuneEntries.Count - 2)
                            rightRecallRuneEntry = runeTome.m_RecallRuneEntries[selectedEntryIndex + 1];
                    }

                    //Selected is on Right
                    else
                    {
                        if (selectedEntryIndex > 0)
                            leftRecallRuneEntry = runeTome.m_RecallRuneEntries[selectedEntryIndex - 1];

                        rightRecallRuneEntry = m_RuneTomeGumpObject.m_SelectedRuneEntry;
                    }        
            
                    //Top
                    AddLabel(85, 30, 2603, "Recall Charges");
			        AddItem(174, 30, 8012);
			        AddLabel(215, 30, WhiteTextHue, runeTome.RecallCharges.ToString() + "/" + MaxRecallCharges.ToString());
                    AddLabel(101, 55, 2603, "(Requires 20 Magery)");

                    AddLabel(370, 30, 2629, "Gate Charges");
			        AddItem(451, 30, 8032);
                    AddLabel(492, 30, WhiteTextHue, runeTome.GateCharges.ToString() + "/" + MaxGateCharges.ToString());
                    AddLabel(378, 55, 2629, "(Requires 50 Magery)");

                    //Left
                    if (leftRecallRuneEntry != null)
                    {
                        string coordinatesText = "9° 18' N     24° 49' E";

                        if (leftRecallRuneEntry.m_IsDefaultRune)
                            AddLabel(Utility.CenteredTextOffset(170, leftRecallRuneEntry.m_Description), 92, 63, leftRecallRuneEntry.m_Description);
                        else
                            AddLabel(Utility.CenteredTextOffset(170, leftRecallRuneEntry.m_Description), 92, 2550, leftRecallRuneEntry.m_Description);

                        AddLabel(Utility.CenteredTextOffset(170, coordinatesText), 112, WhiteTextHue, coordinatesText);

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

                        if (hasAdminAccess)
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
                        string coordinatesText = "9° 18' N     20° 45' E";

                        if (rightRecallRuneEntry.m_IsDefaultRune)
                            AddLabel(Utility.CenteredTextOffset(440, rightRecallRuneEntry.m_Description), 92, 63, rightRecallRuneEntry.m_Description);
                        else
                            AddLabel(Utility.CenteredTextOffset(440, rightRecallRuneEntry.m_Description), 92, 2550, rightRecallRuneEntry.m_Description);

                        AddLabel(Utility.CenteredTextOffset(440, coordinatesText), 112, WhiteTextHue, coordinatesText);
                        
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
                            AddButton(494, 207, 2291, 2291, 17, GumpButtonType.Reply, 0);
                        }

                        if (hasAdminAccess)
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

            bool closeGump = true;

            //TEST
            bool hasChargeAccess = true;
            bool hasAdminAccess = true;    

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
                            if (hasAdminAccess)                              
                                m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder = !m_RuneTomeGumpObject.m_ToggleRearrangeRuneOrder;

                            m_Player.SendSound(LargeSelectionSound);

                            closeGump = false;
                        break;

                        //Rename Runebook
                        case 3:
                            if (hasAdminAccess)
                            {
                            }

                            m_Player.SendSound(LargeSelectionSound);

                            closeGump = false;
                        break;                        

                        //Previous Access Level
                        case 4:
                            if (hasAdminAccess)
                            {
                                switch (runeTome.LockedDownAccessLevel)
                                {
                                    case RuneTome.LockedDownAccessLevelType.Owner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Anyone; break;
                                    case RuneTome.LockedDownAccessLevelType.CoOwner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Owner; break;
                                    case RuneTome.LockedDownAccessLevelType.Friend: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.CoOwner; break;
                                    case RuneTome.LockedDownAccessLevelType.Anyone: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Friend; break;
                                }
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Next Access Level
                        case 5:
                            if (hasAdminAccess)
                            {
                                switch (runeTome.LockedDownAccessLevel)
                                {
                                    case RuneTome.LockedDownAccessLevelType.Owner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.CoOwner; break;
                                    case RuneTome.LockedDownAccessLevelType.CoOwner: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Friend; break;
                                    case RuneTome.LockedDownAccessLevelType.Friend: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Anyone; break;
                                    case RuneTome.LockedDownAccessLevelType.Anyone: runeTome.LockedDownAccessLevel = RuneTome.LockedDownAccessLevelType.Owner; break;
                                }
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;
                    }

                    if (info.ButtonID >= 10)
                    {
                        int runeSelectionIndex = (int)(Math.Floor((double)info.ButtonID / 10)) - 1;                      
                        int buttonIndex = info.ButtonID % 10;

                        if (runeSelectionIndex < runeTome.m_RecallRuneEntries.Count)
                        {
                            bool leftSide = true;

                            if (runeSelectionIndex % 2 == 1)
                                leftSide = false;

                            switch (buttonIndex)
                            {
                                //Recall
                                case 0:
                                break;

                                //Open Detail View
                                case 1:
                                    m_RuneTomeGumpObject.m_RuneTomePageType = PageType.EntryDetail;
                                    m_RuneTomeGumpObject.m_SelectedRuneEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                    m_Player.SendSound(OpenGumpSound);
                                break;

                                //Move Up
                                case 2:
                                    if (runeSelectionIndex > 0)
                                    {
                                        RecallRuneEntry runeEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                        runeTome.m_RecallRuneEntries.Remove(runeEntry);
                                        runeTome.m_RecallRuneEntries.Insert(runeSelectionIndex - 1, runeEntry);

                                        m_Player.SendSound(SelectionSound);
                                    }
                                break;

                                //Move Down
                                case 3:
                                    if (runeSelectionIndex < runeTome.m_RecallRuneEntries.Count - 1)
                                    {
                                        RecallRuneEntry runeEntry = runeTome.m_RecallRuneEntries[runeSelectionIndex];

                                        runeTome.m_RecallRuneEntries.Remove(runeEntry);

                                        if (runeSelectionIndex == runeTome.m_RecallRuneEntries.Count - 1)
                                            runeTome.m_RecallRuneEntries.Add(runeEntry);

                                        else
                                            runeTome.m_RecallRuneEntries.Insert(runeSelectionIndex + 1, runeEntry);

                                        m_Player.SendSound(SelectionSound);
                                    }
                                break;
                            }
                        }

                        closeGump = false;
                    }
                break;

                #endregion

                #region Entry Detail

                case PageType.EntryDetail:                    

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
                            closeGump = false;
                        break;                       

                        //Recall Via Charge
                        case 6:
                            if (hasChargeAccess)
                            {
                            }
                            
                            closeGump = false;
                        break;

                        //Gate
                        case 7:
                            closeGump = false;
                        break;

                        //Gate via Charge
                        case 8:
                            if (hasChargeAccess)
                            {
                            }

                            closeGump = false;
                        break;

                        //Drop Rune
                        case 9:
                            if (hasAdminAccess)
                            {
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Set Rune as Default
                        case 10:
                            if (hasAdminAccess)
                            {
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Rename Rune
                        case 11:
                            if (hasAdminAccess)
                            {
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        #endregion

                        #region Right

                        //Recall
                        case 12:
                            closeGump = false;
                        break;

                        //Recall Via Charge
                        case 13:
                            if (hasChargeAccess)
                            {
                            }
                            
                            closeGump = false;
                        break;

                        //Gate:
                        case 14:
                            closeGump = false;
                        break;                      

                        //Gate Via Charge
                        case 15:
                            if (hasChargeAccess)
                            {
                            }
                            
                            closeGump = false;
                        break;

                        //Drop Rune
                        case 16:
                            if (hasAdminAccess)
                            {
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Set Rune as Default
                        case 17:
                            if (hasAdminAccess)
                            {
                            }

                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Rename Rune
                        case 18:
                            if (hasAdminAccess)
                            {
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
    }

    public class RuneTomeGumpObject
    {
        public RuneTome m_RuneTome;

        public RuneTomeGump.PageType m_RuneTomePageType = RuneTomeGump.PageType.Overview;       

        public bool m_ToggleRearrangeRuneOrder = false;

        public RecallRuneEntry m_SelectedRuneEntry = null;

        public RuneTomeGumpObject(RuneTome runeTome)
        {
            m_RuneTome = runeTome;
        }
    }
}