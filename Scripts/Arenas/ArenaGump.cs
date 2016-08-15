using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class ArenaGump : Gump
    {
        public enum ArenaPageType
        {
            AvailableMatches,
            ScheduledTournaments,
            RecordsAndTeams,

            CreateMatch,
            MatchInfo,
            TournamentRounds
        }
                
        public PlayerMobile m_Player;
        public ArenaGumpObject m_ArenaGumpObject;

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2; //0x051; //0x3E6
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public static int MatchListingsPerAvailableMatchesPage = 7;

        public static int BasicRulesPerCreateMatchPage = 7;
        public static int BasicRulesPerMatchInfoPage = 7;

        public static int SpellRulesPerCreateMatchPage = 5;
        public static int SpellRulesPerMatchInfoPage = 5;

        public static int ItemRulesPerCreateMatchPage = 9;
        public static int ItemRulesPerMatchInfoPage = 9;

        public List<ArenaMatch> m_AvailableMatches = new List<ArenaMatch>();

        public ArenaGump(PlayerMobile player, ArenaGumpObject arenaGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ArenaGumpObject = arenaGumpObject;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ArenaGumpObject == null) return;
            if (m_ArenaGumpObject.m_ArenaRuleset == null) return;
            if (m_ArenaGumpObject.m_ArenaGroupController == null) return;
                        
            #region Background Images

            AddImage(9, 3, 103, 2401);
            AddImage(143, 100, 103, 2401);
            AddImage(144, 143, 103, 2401);
            AddImage(144, 229, 103);
            AddImage(144, 319, 103);
            AddImage(146, 419, 103, 2401);
            AddImage(9, 419, 103, 2401);
            AddImage(9, 228, 103, 2401);
            AddImage(142, 3, 103, 2401);
            AddImage(8, 142, 103, 2401);
            AddImage(9, 99, 103, 2401);
            AddImage(18, 141, 3604, 2052);
            AddImage(144, 131, 3604, 2052);
            AddImage(18, 13, 3604, 2052);
            AddImage(144, 13, 3604, 2052);
            AddImage(9, 318, 103, 2401);
            AddImage(18, 189, 3604, 2052);
            AddImage(144, 189, 3604, 2052);
            AddImage(18, 281, 3604, 2052);
            AddImage(144, 281, 3604, 2052);
            AddImage(18, 381, 3604, 2052);
            AddImage(144, 381, 3604, 2052);
            AddImage(275, 100, 103, 2401);
            AddImage(276, 143, 103, 2401);
            AddImage(276, 229, 103);
            AddImage(276, 319, 103);
            AddImage(277, 419, 103, 2401);
            AddImage(275, 3, 103, 2401);
            AddImage(407, 100, 103, 2401);
            AddImage(408, 143, 103, 2401);
            AddImage(408, 229, 103, 2401);
            AddImage(408, 319, 103, 2401);
            AddImage(409, 419, 103, 2401);
            AddImage(407, 3, 103, 2401);
            AddImage(268, 131, 3604, 2052);
            AddImage(268, 13, 3604, 2052);
            AddImage(268, 189, 3604, 2052);
            AddImage(268, 281, 3604, 2052);
            AddImage(268, 381, 3604, 2052);
            AddImage(376, 131, 3604, 2052);
            AddImage(357, 13, 3604, 2052);
            AddImage(388, 189, 3604, 2052);
            AddImage(376, 281, 3604, 2052);
            AddImage(376, 381, 3604, 2052);
            AddImage(410, 125, 3604, 2052);
            AddImage(410, 281, 3604, 2052);
            AddImage(410, 381, 3604, 2052);
            AddImage(540, 99, 103, 2401);
            AddImage(541, 143, 103, 2401);
            AddImage(541, 229, 103, 2401);
            AddImage(541, 319, 103, 2401);
            AddImage(542, 419, 103, 2401);
            AddImage(540, 3, 103, 2401);
            AddImage(509, 131, 3604, 2052);
            AddImage(483, 13, 3604, 2052);
            AddImage(509, 189, 3604, 2052);
            AddImage(509, 281, 3604, 2052);
            AddImage(509, 381, 3604, 2052);
            AddImage(543, 137, 3604, 2052);
            AddImage(543, 13, 3604, 2052);
            AddImage(543, 189, 3604, 2052);
            AddImage(543, 281, 3604, 2052);
            AddImage(543, 381, 3604, 2052);
            AddImage(18, 94, 96, 1102);
            AddImage(173, 94, 96, 1102);
            AddImage(300, 94, 96, 1102);
            AddImage(409, 94, 96, 1102);
            AddImage(491, 94, 96, 1102);
            AddImage(18, 93, 96, 1102);
            AddImage(173, 93, 96, 1102);
            AddImage(300, 93, 96, 1102);
            AddImage(409, 93, 96, 1102);
            AddImage(491, 93, 96, 1102);

            #endregion

            AddImage(212, 0, 1143, 2499);
            AddLabel(331, 2, 2515, "Arena");

            //Guide
            AddButton(7, 4, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(30, 4, 149, "Guide");

            //Available Matches
            AddItem(15, 28, 8454, 2515);
            AddItem(36, 28, 8455, 2515);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.AvailableMatches) 
                AddButton(75, 41, 9724, 9721, 2, GumpButtonType.Reply, 0);
            else
                AddButton(75, 41, 9721, 9724, 2, GumpButtonType.Reply, 0);

            AddLabel(108, 35, 2603, "Available");
            AddLabel(108, 53, 2603, "Matches");            
           
            //Scheduled Tournaments
            AddItem(211, 29, 2826);
            AddItem(208, 23, 9920);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.ScheduledTournaments)
                AddButton(261, 41, 9724, 9721, 3, GumpButtonType.Reply, 0);
            else
                AddButton(261, 41, 9721, 9724, 3, GumpButtonType.Reply, 0);

            AddLabel(302, 35, 53, "Scheduled");
            AddLabel(294, 55, 53, "Tournaments");
                 
            //Teams and Records
            AddItem(413, 56, 5357);
            AddItem(405, 43, 4030);
            AddItem(407, 34, 4031);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.RecordsAndTeams)
                AddButton(444, 41, 9724, 9721, 4, GumpButtonType.Reply, 0);  
            else
                AddButton(444, 41, 9721, 9724, 4, GumpButtonType.Reply, 0);  
  
            AddLabel(481, 35, 2606, "Teams");
            AddLabel(491, 48, 2606, "and");
            AddLabel(476, 61, 2606, "Records");

            switch (arenaGumpObject.m_ArenaPage)
            {
                #region Available Matches

                case ArenaPageType.AvailableMatches:                    
                    AddLabel(293, 84, 2603, "Available Matches");

                    m_AvailableMatches = m_ArenaGumpObject.m_ArenaGroupController.GetArenaMatches(m_Player);
                    
                    int MatchesPerPage = MatchListingsPerAvailableMatchesPage;

                    int totalMatches = m_AvailableMatches.Count;
                    int totalMatchPages = (int)(Math.Ceiling((double)totalMatches / (double)MatchesPerPage));

                    if (m_ArenaGumpObject.m_Page >= totalMatchPages)
                        m_ArenaGumpObject.m_Page = 0;

                    if (m_ArenaGumpObject.m_Page < 0)
                        m_ArenaGumpObject.m_Page = 0;

                    int matchStartIndex = m_ArenaGumpObject.m_Page * MatchesPerPage;
                    int matchEndIndex = (m_ArenaGumpObject.m_Page * MatchesPerPage) + (MatchesPerPage - 1);

                    if (matchEndIndex >= totalMatches)
                        matchEndIndex = totalMatches - 1;

                    //Matches
                    int matchCount = matchEndIndex - matchStartIndex;

                    int startY = 110;
                    int rowSpacing = 50;

                    for (int a = 0; a < matchCount + 1; a++)
                    {
                        if (totalMatches == 0)
                            continue;

                        int matchIndex = matchStartIndex + a;

                        if (matchIndex >= totalMatches)
                            continue;

                        ArenaMatch arenaMatch = m_AvailableMatches[matchIndex];

                        if (arenaMatch == null) continue;
                        if (arenaMatch.Deleted) continue;
                        if (arenaMatch.m_Ruleset == null) continue;
                        if (arenaMatch.m_Ruleset.Deleted) continue;

                        int teamSize = arenaMatch.m_Ruleset.TeamSize;

                        int team1Players = 0;
                        int team1ReadyPlayers = 0;

                        int team2Players = 0;
                        int team2ReadyPlayers = 0;

                        bool createdMatch = false;

                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch == arenaMatch)
                        {
                            if (arenaMatch.m_Creator == m_Player)
                                createdMatch = true;
                        }
                        
                        bool isOnTeam1 = false;
                        bool isOnTeam2 = false;

                        ArenaTeam team1 = arenaMatch.GetTeam(0);
                        ArenaTeam team2 = arenaMatch.GetTeam(1);

                        ArenaParticipant arenaParticipant = null;

                        if (team1 != null)
                        {
                            arenaParticipant = team1.GetPlayerParticipant(m_Player);

                            if (arenaParticipant != null)
                                isOnTeam1 = true;

                            foreach (ArenaParticipant participant in team1.m_Participants)
                            {
                                if (participant == null) continue;
                                if (participant.Deleted) continue;

                                team1Players++;

                                if (participant.m_ReadyToggled)
                                    team1ReadyPlayers++;
                            }
                        }

                        if (team2 != null)
                        {
                            arenaParticipant = team2.GetPlayerParticipant(m_Player);

                            if (arenaParticipant != null)
                                isOnTeam2 = true;

                            foreach (ArenaParticipant participant in team2.m_Participants)
                            {
                                if (participant == null) continue;
                                if (participant.Deleted) continue;
                               
                                team2Players++;

                                if (participant.m_ReadyToggled)
                                    team2ReadyPlayers++;
                            }
                        }
                        
                        #region Teamsize / Ranked

                        int playerCountTextHue = WhiteTextHue;

                        if (isOnTeam1 || isOnTeam2)
                            playerCountTextHue = 63;
                        
                        switch (arenaMatch.m_Ruleset.m_MatchType)
                        {
                            //Unranked
                            case ArenaRuleset.MatchTypeType.Unranked1vs1:
                                AddLabel(137, startY, playerCountTextHue, "1 vs 1");
                                //AddLabel(128, startY + 20, 2550, "Unranked"); 
                            break;

                            case ArenaRuleset.MatchTypeType.Unranked2vs2:
                                AddLabel(137, startY, playerCountTextHue, "2 vs 2");
                                //AddLabel(128, startY + 20, 2550, "Unranked");
                            break;

                            case ArenaRuleset.MatchTypeType.Unranked3vs3:
                                AddLabel(137, startY, playerCountTextHue, "3 vs 3");
                                //AddLabel(128, startY + 20, 2550, "Unranked");
                            break;

                            case ArenaRuleset.MatchTypeType.Unranked4vs4:
                                AddLabel(137, startY, playerCountTextHue, "4 vs 4");
                                //AddLabel(128, startY + 20, 2550, "Unranked");
                            break;

                            //Ranked
                            case ArenaRuleset.MatchTypeType.Ranked1vs1:
                                AddLabel(137, startY, playerCountTextHue, "1 vs 1");
                                //AddLabel(128, startY + 20, 2606, "Ranked");
                            break;

                            case ArenaRuleset.MatchTypeType.Ranked2vs2:
                                AddLabel(137, startY, playerCountTextHue, "2 vs 2");
                                //AddLabel(128, startY + 20, 2606, "Ranked");
                            break;

                            case ArenaRuleset.MatchTypeType.Ranked3vs3:
                                AddLabel(137, startY, playerCountTextHue, "3 vs 3");
                                //AddLabel(128, startY + 20, 2606, "Ranked");
                            break;

                            case ArenaRuleset.MatchTypeType.Ranked4vs4:
                                AddLabel(137, startY, playerCountTextHue, "4 vs 4");
                                //AddLabel(128, startY + 20, 2606, "Ranked");
                            break;
                        }

                        #endregion

                        if (arenaMatch.m_Creator != null)
                        {
                            string matchName = arenaMatch.m_Creator.RawName;

                            if (createdMatch)
                                AddLabel(Utility.CenteredTextOffset(162, matchName), startY + 20, 63, matchName);

                            else
                                AddLabel(Utility.CenteredTextOffset(162, matchName), startY + 20, 2550, matchName);                        
                        }

                        #region Soldier Icons
                        
                        //Team 1 Statues
                        for (int b = 0; b < teamSize; b++)
                        {
                            int playerHue = 2401;

                            if (team1 != null)
                            {
                                for (int c = 0; c < team1.m_Participants.Count; c++)
                                {
                                    if (b != c) 
                                        continue;
                                
                                    ArenaParticipant participant = team1.m_Participants[c];

                                    if (participant == null)
                                        continue;

                                    playerHue = 2500;

                                    if (participant.m_ReadyToggled)                                        
                                        playerHue = 2208;   
                                }
                            }

                            AddItem(35 - ((teamSize - 1) * 7) + (b * 7), startY - 10, 15178, playerHue);
                        }

                        //Team 2 Statues
                        for (int b = 0; b < teamSize; b++)
                        {
                            int playerHue = 2401;

                            if (team2 != null)
                            {
                                for (int c = 0; c < team2.m_Participants.Count; c++)
                                {
                                    if (b != c)
                                        continue;
                                
                                    ArenaParticipant participant = team2.m_Participants[c];

                                    if (participant == null)
                                        continue;

                                    playerHue = 2500;

                                    if (participant.m_ReadyToggled)
                                        playerHue = 2208;                                    
                                }
                            }

                            AddItem(60 + (b * 7), startY - 10, 15179, playerHue);
                        }

                        #endregion

                        if (isOnTeam1)
                        {
                            AddLabel(385, startY, 63, "Team 1");
                            AddLabel(385, startY + 20, WhiteTextHue, "Players:");

                            if (!createdMatch)
                            {
                                AddLabel(307, startY + 9, 2401, "Leave");
                                AddButton(347, startY + 6, 2151, 2154, 20 + a, GumpButtonType.Reply, 0);
                            }

                            else
                                AddImage(347, startY + 6, 9724, 0);
                        }

                        else
                        {
                            AddLabel(385, startY, 149, "Team 1");
                            AddLabel(385, startY + 20, WhiteTextHue, "Players:");

                            if (team1Players < teamSize)
                            {
                                if (isOnTeam2)
                                {
                                    AddLabel(305, startY + 9, 63, "Switch");
                                    AddButton(347, startY + 6, 2151, 2154, 20 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(313, startY + 9, WhiteTextHue, "Join");
                                    AddButton(347, startY + 6, 2151, 2154, 20 + a, GumpButtonType.Reply, 0);
                                }
                            }
                        }                        

                        if (team1Players >= teamSize)
                            AddLabel(438, startY + 20, WhiteTextHue, team1Players.ToString() + "/" + teamSize.ToString());
                        else
                            AddLabel(438, startY + 20, 2401, team1Players.ToString() + "/" + teamSize.ToString());

                        if (isOnTeam2)
                        {
                            AddLabel(573, startY, 63, "Team 2");
                            AddLabel(573, startY + 20, WhiteTextHue, "Players:");

                            if (!createdMatch)
                            {
                                AddLabel(493, startY + 9, 2401, "Leave");
                                AddButton(533, startY + 6, 2151, 2154, 30 + a, GumpButtonType.Reply, 0);
                            }

                            else
                                AddImage(533, startY + 6, 9724, 0);
                        }

                        else
                        {
                            AddLabel(573, startY, 149, "Team 2");
                            AddLabel(573, startY + 20, WhiteTextHue, "Players:");

                            if (team2Players < teamSize)
                            {
                                if (isOnTeam1)
                                {
                                    AddLabel(491, startY + 8, 63, "Switch");
                                    AddButton(533, startY + 6, 2151, 2154, 30 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(501, startY + 8, WhiteTextHue, "Join");
                                    AddButton(533, startY + 6, 2151, 2154, 30 + a, GumpButtonType.Reply, 0);
                                }
                            }
                        }

                        if (team2Players >= teamSize)
                            AddLabel(628, startY + 20, WhiteTextHue, team2Players.ToString() + "/" + teamSize.ToString());
                        else
                            AddLabel(628, startY + 20, 2401, team2Players.ToString() + "/" + teamSize.ToString());

                        if (isOnTeam1 || isOnTeam2)
                        {
                            AddLabel(217, startY, 63, "Match Info");
                            AddButton(243, startY + 20, 30008, 30009, 40 + a, GumpButtonType.Reply, 0);
                        }

                        else
                        {
                            AddLabel(217, startY, 149, "Match Info");
                            AddButton(243, startY + 20, 30008, 30009, 40 + a, GumpButtonType.Reply, 0);
                        }

                        startY += rowSpacing;
                    }                    

                    //-----

                    if (m_ArenaGumpObject.m_Page > 0)
                    {
                        AddButton(23, 483, 4014, 4016, 10, GumpButtonType.Reply, 0);
                        AddLabel(57, 483, WhiteTextHue, "Previous Page");
                    }

                    AddButton(283, 483, 4008, 4010, 12, GumpButtonType.Reply, 0);
			        AddLabel(317, 483, 63, "Create New Match");

                    if (m_ArenaGumpObject.m_Page < totalMatchPages - 1)
                    {
                        AddButton(563, 483, 4005, 4007, 11, GumpButtonType.Reply, 0);
                        AddLabel(599, 483, WhiteTextHue, "Next Page");
                    }
                break;

                #endregion

                #region Create Match

                case ArenaPageType.CreateMatch:
                    AddLabel(289, 84, 63, "Create New Match");
                    
                    List<ArenaRuleDetails> m_BasicRules = ArenaRuleset.GetBasicRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);
                    List<ArenaRuleDetails> m_SpellRules = ArenaRuleset.GetSpellRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);
                    List<ArenaRuleDetails> m_ItemRules = ArenaRuleset.GetItemRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);

                    int totalBasicRulesPages = (int)(Math.Ceiling((double)m_BasicRules.Count / (double)BasicRulesPerCreateMatchPage));
                    int totalSpellRulesPages = (int)(Math.Ceiling((double)m_SpellRules.Count / (double)SpellRulesPerCreateMatchPage));
                    int totalItemRulesPages = (int)(Math.Ceiling((double)m_ItemRules.Count / (double)ItemRulesPerCreateMatchPage));

                    int totalSettingsPages = totalBasicRulesPages;

                    if (totalSpellRulesPages > totalSettingsPages)
                        totalSettingsPages = totalSpellRulesPages;

                    if (totalItemRulesPages > totalSettingsPages)
                        totalSettingsPages = totalItemRulesPages;

                    if (m_ArenaGumpObject.m_SettingsPage >= totalSettingsPages)
                        m_ArenaGumpObject.m_SettingsPage = totalSettingsPages - 1;

                    if (m_ArenaGumpObject.m_SettingsPage < 0)
                        m_ArenaGumpObject.m_SettingsPage = 0;
                    
                    #region Basic Rules   

                    int startingRuleIndex = (m_ArenaGumpObject.m_SettingsPage * BasicRulesPerCreateMatchPage);

                    int startX = 65;
                    startY = 120;
                    rowSpacing = 40;

                    for (int a = 0; a < BasicRulesPerCreateMatchPage; a++)
                    {
                        int index = a + startingRuleIndex;

                        if (index >= m_BasicRules.Count)
                            continue;

                        ArenaRuleDetails ruleDetail = m_BasicRules[index];

                        #region Basic Rules

                        ArenaBasicRuleDetail basicRuleDetail = m_ArenaGumpObject.m_ArenaRuleset.GetBasicRuleDetail(index);

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.MatchTypeType))
                        {
                            AddItem(startX - 40, startY - 17, 15178, 0);
                            AddLabel(startX, startY, 149, "Match Type:");
                            AddLabel(startX + 78, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                            AddLabel(startX + 133, startY, basicRuleDetail.m_Line2Hue, "(" + basicRuleDetail.m_Line2Text + ")");
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.ListingModeType))
                        {
                            AddItem(startX - 43, startY + 2, 5365, 0);
                            AddLabel(startX, startY, 149, "Listing Mode:");
                            AddLabel(startX + 83, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.RoundDurationType))
                        {
                            AddItem(startX - 40, startY - 0, 6169, 0);
                            AddLabel(startX, startY, 149, "Round Duration:");
                            AddLabel(startX + 100, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                            AddLabel(startX + 100, startY + 15, basicRuleDetail.m_Line2Hue, basicRuleDetail.m_Line2Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.SuddenDeathModeType))
                        {
                            AddItem(startX - 42, startY - 5, 7960, 0);
                            AddLabel(startX, startY, 149, "Sudden Death Mode:");
                            AddLabel(startX + 128, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.EquipmentAllowedType))
                        {
                            AddItem(startX - 30, startY - 10, 5073, 0);
                            AddLabel(startX, startY, 149, "Equipment Allowed:");
                            AddLabel(startX + 118, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.PoisonedWeaponsStartingRestrictionType))
                        {
                            AddItem(startX - 35, startY - 5, 5118, 2208);
                            AddLabel(startX, startY, 149, "Poisoned Weapons:");
                            AddLabel(startX + 118, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.MountsRestrictionType))
                        {
                            AddItem(startX - 47, startY - 10, 8484, 2500);
                            AddLabel(startX, startY, 149, "Mounts:");
                            AddLabel(startX + 55, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.FollowersRestrictionType))
                        {
                            AddItem(startX - 47, startY - 1, 8532, 0);
                            AddLabel(startX, startY, 149, "Followers:");
                            AddLabel(startX + 65, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.ResourceConsumptionType))
                        {
                            AddItem(startX - 55, startY - 4, 3817, 0);
                            AddItem(startX - 58, startY - 8, 3817, 0);

                            AddItem(startX - 37, startY - 8, 3903, 0);
                            AddItem(startX - 34, startY - 5, 3903, 0);

                            AddItem(startX - 44, startY + 15, 3973, 0);
                            AddItem(startX - 38, startY + 18, 3973, 0);

                            AddItem(startX - 50, startY + 12, 3852, 0);
                            AddItem(startX - 54, startY + 10, 3852, 0);

                            AddLabel(startX, startY, 149, "Resource Consumption:");
                            AddLabel(startX + 140, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                            AddLabel(startX + 60, startY + 15, basicRuleDetail.m_Line2Hue, basicRuleDetail.m_Line2Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.ItemDurabilityDamageType))
                        {
                            AddItem(startX - 43, startY + 0, 7031, 0);
                            AddItem(startX - 44, startY + 6, 6916, 0);

                            AddLabel(startX, startY, 149, "Item Durability:");
                            AddLabel(startX + 98, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        //Buttons
                        if (ruleDetail.m_AccessLevel <= m_Player.AccessLevel)
                        {
                            AddButton(startX + 10, startY + 20, 2223, 2223, 100 + (index * 2), GumpButtonType.Reply, 0);
                            AddButton(startX + 35, startY + 20, 2224, 2224, 100 + (index * 2) + 1, GumpButtonType.Reply, 0);
                        }

                        #endregion

                        startY += rowSpacing;
                    }

                    #endregion

                    #region Spell Restrictions

                    startingRuleIndex = (m_ArenaGumpObject.m_SettingsPage * SpellRulesPerCreateMatchPage);

                    startX = 460;
                    startY = 120;
                    rowSpacing = 20;

                    for (int a = 0; a < SpellRulesPerCreateMatchPage; a++)
                    {
                        int index = a + startingRuleIndex;

                        if (index >= m_SpellRules.Count)
                            continue;

                        ArenaRuleDetails ruleDetail = m_SpellRules[index];
                        ArenaSpellRuleDetail spellRuleDetail = m_ArenaGumpObject.m_ArenaRuleset.GetSpellRuleDetail(index);  
                        
                        AddItem(startX + spellRuleDetail.m_ItemOffsetX, startY + spellRuleDetail.m_ItemOffsetY, spellRuleDetail.m_ItemID, spellRuleDetail.m_ItemHue);
                        AddLabel(startX, startY, 149, spellRuleDetail.m_SpellName);
                        AddLabel(startX + 120, startY, spellRuleDetail.m_TextHue, spellRuleDetail.m_RuleText);

                        //Buttons
                        if (ruleDetail.m_AccessLevel <= m_Player.AccessLevel)
                        {
                            AddButton(startX - 75, startY + 4, 2223, 2223, 200 + (index * 2), GumpButtonType.Reply, 0);
                            AddButton(startX - 50, startY + 4, 2224, 2224, 200 + (index * 2) + 1, GumpButtonType.Reply, 0);
                        }

                        startY += rowSpacing;
                    }
                    
                    #endregion

                    #region Item Restrictions

                    startingRuleIndex = (m_ArenaGumpObject.m_SettingsPage * ItemRulesPerCreateMatchPage);

                    startX = 460;
                    startY = 220;
                    rowSpacing = 20;

                    for (int a = 0; a < ItemRulesPerCreateMatchPage; a++)
                    {
                        int index = a + startingRuleIndex;

                        if (index >= m_ItemRules.Count)
                            continue;

                        ArenaRuleDetails ruleDetail = m_ItemRules[index];
                        ArenaItemRuleDetail itemRuleDetail = m_ArenaGumpObject.m_ArenaRuleset.GetItemRuleDetail(index);

                        AddItem(startX + itemRuleDetail.m_ItemOffsetX, startY + itemRuleDetail.m_ItemOffsetY, itemRuleDetail.m_ItemID, itemRuleDetail.m_ItemHue);
                        AddLabel(startX, startY, 149, itemRuleDetail.m_ItemName);
                        AddLabel(startX + 120, startY, itemRuleDetail.m_TextHue, itemRuleDetail.m_RuleText);

                        //Buttons
                        if (ruleDetail.m_AccessLevel <= m_Player.AccessLevel)
                        {
                            AddButton(startX - 75, startY + 4, 2223, 2223, 300 + (index * 2), GumpButtonType.Reply, 0);
                            AddButton(startX - 50, startY + 4, 2224, 2224, 300 + (index * 2) + 1, GumpButtonType.Reply, 0);
                        }

                        startY += rowSpacing;
                    }                    

                    #endregion

                    //Controls
                    if (m_ArenaGumpObject.m_SettingsPage > 0)
                        AddButton(179, 402, 9909, 9909, 15, GumpButtonType.Reply, 0);

                    if (m_ArenaGumpObject.m_SettingsPage > 0 || m_ArenaGumpObject.m_SettingsPage < totalSettingsPages)
                        AddLabel(208, 403, 2599, "More Settings");

                    if (m_ArenaGumpObject.m_SettingsPage < totalSettingsPages - 1)
                        AddButton(304, 402, 9903, 9903, 16, GumpButtonType.Reply, 0);

                    ArenaPresetDetail GetPresetDetail = m_ArenaGumpObject.m_ArenaRuleset.GetPresetDetail();

                    AddButton(124, 483, 9909, 9909, 13, GumpButtonType.Reply, 0);
                    AddLabel(208, 464, 149, "Match Presets");
                    AddLabel(Utility.CenteredTextOffset(265, GetPresetDetail.m_Text), 484, GetPresetDetail.m_Hue, GetPresetDetail.m_Text);
                    AddButton(358, 482, 9903, 9903, 14, GumpButtonType.Reply, 0);

                    AddLabel(57, 483, WhiteTextHue, "Cancel");
                    AddButton(23, 483, 4014, 4016, 10, GumpButtonType.Reply, 0);

                    AddButton(405, 483, 4011, 4013, 11, GumpButtonType.Reply, 0);
			        AddLabel(439, 483, 2550, "Save Presets");

                    AddButton(540, 483, 4008, 4010, 12, GumpButtonType.Reply, 0);
			        AddLabel(573, 483, 63, "Create Match");
                break;

                #endregion

                #region Match Info

                case ArenaPageType.MatchInfo:
                    ArenaMatch selectedArenaMatch = m_ArenaGumpObject.m_ArenaMatchViewing;

                    bool validArenaMatch = true;

                    if (selectedArenaMatch == null)                    
                        validArenaMatch = false;                    

                    else if (selectedArenaMatch.Deleted)                    
                        validArenaMatch = false;                    

                    else if (selectedArenaMatch.m_ArenaGroupController == null)                    
                        validArenaMatch = false;                    

                    else if (selectedArenaMatch.m_ArenaGroupController.Deleted)                    
                        validArenaMatch = false;                    

                    else if (selectedArenaMatch.m_Ruleset == null)                    
                        validArenaMatch = false;                    

                    else if (selectedArenaMatch.m_Ruleset.Deleted)                    
                        validArenaMatch = false;

                    else if (!selectedArenaMatch.CanPlayerJoinMatch(m_Player))                    
                        validArenaMatch = false;                    

                    else if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)                    
                        validArenaMatch = false;                    

                    ArenaTeam arenaTeam1 = selectedArenaMatch.GetTeam(0);
                    ArenaTeam arenaTeam2 = selectedArenaMatch.GetTeam(1);

                    if (arenaTeam1 == null)                    
                        validArenaMatch = false;                    

                    else if (arenaTeam1.Deleted)                    
                        validArenaMatch = false;

                    if (arenaTeam2 == null)                    
                        validArenaMatch = false;

                    else if (arenaTeam2.Deleted)                    
                        validArenaMatch = false;

                    if (!validArenaMatch)
                    {
                        m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                        m_ArenaGumpObject.m_ArenaMatchViewing = null;

                        m_Player.CloseGump(typeof(ArenaGump));
                        m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                        m_Player.SendMessage("That match is now longer viewable.");

                        return;
                    }

                    int teamPlayersNeeded = selectedArenaMatch.m_Ruleset.TeamSize;

                    int team1PlayerCount = 0;
                    int team1ReadyPlayerCount = 0;

                    int team2PlayerCount = 0;
                    int team2ReadyPlayerCount = 0;

                    bool playerCreatedMatch = false;

                    bool playerIsOnTeam1 = false;
                    bool playerIsOnTeam2 = false;

                    bool playerReady = false;

                    ArenaParticipant playerParticipant = selectedArenaMatch.GetParticipant(m_Player);

                    if (arenaTeam1.GetPlayerParticipant(m_Player) != null)
                        playerIsOnTeam1 = true;

                    if (arenaTeam2.GetPlayerParticipant(m_Player) != null)
                        playerIsOnTeam2 = true;

                    if (playerParticipant != null)                    
                        playerReady = playerParticipant.m_ReadyToggled;                     

                    if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch == selectedArenaMatch)
                    {
                        if (selectedArenaMatch.m_Creator == m_Player)
                            playerCreatedMatch = true;
                    }
                    
                    AddLabel(316, 84, 2603, "Match Info");

                    //Team 1
                    startY = 120;
                    rowSpacing = 15;

                    int statueStartX = 39 - ((teamPlayersNeeded - 1) * 7);
                    
                    for (int a = 0; a < teamPlayersNeeded; a++)
                    {
                        int playerHue = 2401;

                        string playerName = "";

                        for (int b = 0; b < teamPlayersNeeded; b++)
                        {
                            if (a != b) continue;          
                            if (b >= arenaTeam1.m_Participants.Count) continue;

                            ArenaParticipant participant = arenaTeam1.m_Participants[b];

                            if (participant == null) 
                                continue;

                            team1PlayerCount++;
                            playerHue = 2500;

                            playerName = participant.m_Player.RawName;

                            if (participant.m_ReadyToggled)
                            {                              
                                team1ReadyPlayerCount++;
                                playerHue = 2208;
                            }                                
                        }

                        AddItem(statueStartX + (a * 15), 108, 15178, playerHue);

                        if (playerName != "")
                            AddButton(145, startY + 3, 1209, 1210, 0, GumpButtonType.Reply, 0);

                        if (playerName == "")
                            playerName = "-Empty-";

                        AddLabel(165, startY, playerHue, playerName);

                        startY += rowSpacing;
                    }

                    int teamTextHue = 2401;

                    if (team1PlayerCount >= teamPlayersNeeded)
                        teamTextHue = 2499;

                    if (team1ReadyPlayerCount >= teamPlayersNeeded)
                        teamTextHue = 63;

                    if (teamPlayersNeeded == 1)
                        AddLabel(46, 161, teamTextHue, "[" + team1PlayerCount.ToString() + "/" + teamPlayersNeeded.ToString() + "]");
                    else
                        AddLabel(41, 161, teamTextHue, "[" + team1PlayerCount.ToString() + "/" + teamPlayersNeeded.ToString() + "]");

                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(53, 182, 30008, 30009, 0, GumpButtonType.Reply, 0);
                        AddLabel(74, 179, 54, "Message");
                    }

                    if (playerIsOnTeam1)
                    {
                        AddLabel(99, 100, 63, "Team 1");

                        if (playerCreatedMatch)
                            AddImage(104, 120, 9724, 0);

                        else
                        {
                            AddButton(104, 120, 9724, 9724, 0, GumpButtonType.Reply, 0);
                            AddLabel(99, 148, 2401, "Leave");
                        }
                    }

                    else
                    {
                        AddLabel(99, 100, 149, "Team 1");

                        if (team1PlayerCount < teamPlayersNeeded)
                        {
                            if (playerIsOnTeam2)
                            {
                                AddButton(104, 120, 2151, 2154, 0, GumpButtonType.Reply, 0);
                                AddLabel(95, 148, 63, "Switch");
                            }

                            else
                            {
                                AddButton(104, 120, 2151, 2154, 0, GumpButtonType.Reply, 0);
                                AddLabel(104, 148, WhiteTextHue, "Join");
                            }
                        }

                        else
                        {
                            //TEST: ADD TEAM FULL GRAPHIC
                        }
                    }

                    //Team 2
                    startY = 120;
                    rowSpacing = 15;

                    statueStartX = 298 - ((teamPlayersNeeded - 1) * 7);

                    for (int a = 0; a < teamPlayersNeeded; a++)
                    {
                        int playerHue = 2401;

                        string playerName = "";

                        for (int b = 0; b < teamPlayersNeeded; b++)
                        {
                            if (a != b) continue;          
                            if (b >= arenaTeam2.m_Participants.Count) continue;

                            ArenaParticipant participant = arenaTeam2.m_Participants[b];

                            if (participant == null) 
                                continue;

                            team2PlayerCount++;
                            playerHue = 2500;

                            if (participant.m_ReadyToggled)
                            {                              
                                team2ReadyPlayerCount++;
                                playerHue = 2208;
                            }                                
                        }

                        AddItem(statueStartX + (a * 15), 108, 15178, playerHue);

                        if (playerName != "")
                            AddButton(405, startY + 3, 1209, 1210, 0, GumpButtonType.Reply, 0);

                        if (playerName == "")
                            playerName = "-Empty-";

                        AddLabel(425, startY, playerHue, playerName);

                        startY += rowSpacing;
                    }

                    teamTextHue = 2401;

                    if (team2PlayerCount >= teamPlayersNeeded)
                        teamTextHue = 2499;

                    if (team2ReadyPlayerCount >= teamPlayersNeeded)
                        teamTextHue = 63;

                    if (teamPlayersNeeded == 1)
                        AddLabel(304, 161, teamTextHue, "[" + team2PlayerCount.ToString() + "/" + teamPlayersNeeded.ToString() + "]");
                    else
                        AddLabel(299, 161, teamTextHue, "[" + team2PlayerCount.ToString() + "/" + teamPlayersNeeded.ToString() + "]");

                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(312, 182, 30008, 30009, 0, GumpButtonType.Reply, 0);
                        AddLabel(333, 179, 54, "Message");
                    }

                    if (playerIsOnTeam2)
                    {
                        AddLabel(356, 100, 63, "Team 2");

                        if (playerCreatedMatch)
                            AddImage(362, 120, 9724, 0);

                        else
                        {
                            AddButton(362, 120, 9724, 9724, 0, GumpButtonType.Reply, 0);
                            AddLabel(358, 148, 2401, "Leave");
                        }
                    }

                    else
                    {
                        AddLabel(356, 100, 149, "Team 2");

                        if (team2PlayerCount < teamPlayersNeeded)
                        {
                            if (playerIsOnTeam1)
                            {
                                AddButton(362, 120, 2151, 2154, 0, GumpButtonType.Reply, 0);
                                AddLabel(357, 148, 63, "Switch");
                            }

                            else
                            {
                                AddButton(362, 120, 2151, 2154, 0, GumpButtonType.Reply, 0);
                                AddLabel(362, 148, WhiteTextHue, "Join");
                            }
                        }

                        else
                        {
                            //TEST: ADD TEAM FULL GRAPHIC
                        }
                    }
                   	
                    //Match Controls
                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(546, 105, 9721, 9724, 0, GumpButtonType.Reply, 0);
                        AddLabel(582, 109, 54, "Message All");
                    }

                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        if (playerReady)
                            AddButton(546, 137, 2154, 2151, 0, GumpButtonType.Reply, 0);
                        else
                            AddButton(546, 137, 2151, 2154, 0, GumpButtonType.Reply, 0);
                        AddLabel(582, 141, 63, "Ready");
                    }

                    if (playerCreatedMatch)
                    {
                        AddButton(546, 170, 2472, 2473, 0, GumpButtonType.Reply, 0);
                        AddLabel(582, 174, 1256, "Cancel Match");
                    }

                    #region Rules Settings

                    m_BasicRules = ArenaRuleset.GetBasicRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);
                    m_SpellRules = ArenaRuleset.GetSpellRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);
                    m_ItemRules = ArenaRuleset.GetItemRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);

                    totalBasicRulesPages = (int)(Math.Ceiling((double)m_BasicRules.Count / (double)BasicRulesPerCreateMatchPage));
                    totalSpellRulesPages = (int)(Math.Ceiling((double)m_SpellRules.Count / (double)SpellRulesPerCreateMatchPage));
                    totalItemRulesPages = (int)(Math.Ceiling((double)m_ItemRules.Count / (double)ItemRulesPerCreateMatchPage));

                    totalSettingsPages = totalBasicRulesPages;

                    if (totalSpellRulesPages > totalSettingsPages)
                        totalSettingsPages = totalSpellRulesPages;

                    if (totalItemRulesPages > totalSettingsPages)
                        totalSettingsPages = totalItemRulesPages;

                    if (m_ArenaGumpObject.m_SettingsPage >= totalSettingsPages)
                        m_ArenaGumpObject.m_SettingsPage = totalSettingsPages - 1;

                    if (m_ArenaGumpObject.m_SettingsPage < 0)
                        m_ArenaGumpObject.m_SettingsPage = 0;

                    //Rules
                    #region Basic Rules   

                    startingRuleIndex = (m_ArenaGumpObject.m_SettingsPage * BasicRulesPerMatchInfoPage);

                    startX = 65;
                    startY = 220;
                    rowSpacing = 36;

                    for (int a = 0; a < BasicRulesPerCreateMatchPage; a++)
                    {
                        int index = a + startingRuleIndex;

                        if (index >= m_BasicRules.Count)
                            continue;

                        ArenaRuleDetails ruleDetail = m_BasicRules[index];

                        #region Basic Rules

                        ArenaBasicRuleDetail basicRuleDetail = m_ArenaGumpObject.m_ArenaRuleset.GetBasicRuleDetail(index);

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.MatchTypeType))
                        {
                            AddItem(startX - 40, startY - 17, 15178, 0);
                            AddLabel(startX, startY, 149, "Match Type:");
                            AddLabel(startX + 78, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                            AddLabel(startX + 133, startY, basicRuleDetail.m_Line2Hue, "(" + basicRuleDetail.m_Line2Text + ")");
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.ListingModeType))
                        {
                            AddItem(startX - 43, startY + 2, 5365, 0);
                            AddLabel(startX, startY, 149, "Listing Mode:");
                            AddLabel(startX + 83, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.RoundDurationType))
                        {
                            AddItem(startX - 40, startY - 0, 6169, 0);
                            AddLabel(startX, startY, 149, "Round Duration:");
                            AddLabel(startX + 100, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                            AddLabel(startX + 100, startY + 15, basicRuleDetail.m_Line2Hue, basicRuleDetail.m_Line2Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.SuddenDeathModeType))
                        {
                            AddItem(startX - 42, startY - 5, 7960, 0);
                            AddLabel(startX, startY, 149, "Sudden Death Mode:");
                            AddLabel(startX + 128, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.EquipmentAllowedType))
                        {
                            AddItem(startX - 30, startY - 10, 5073, 0);
                            AddLabel(startX, startY, 149, "Equipment Allowed:");
                            AddLabel(startX + 118, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.PoisonedWeaponsStartingRestrictionType))
                        {
                            AddItem(startX - 35, startY - 5, 5118, 2208);
                            AddLabel(startX, startY, 149, "Poisoned Weapons:");
                            AddLabel(startX + 118, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.MountsRestrictionType))
                        {
                            AddItem(startX - 47, startY - 10, 8484, 2500);
                            AddLabel(startX, startY, 149, "Mounts:");
                            AddLabel(startX + 55, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.FollowersRestrictionType))
                        {
                            AddItem(startX - 47, startY - 1, 8532, 0);
                            AddLabel(startX, startY, 149, "Followers:");
                            AddLabel(startX + 65, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.ResourceConsumptionType))
                        {
                            AddItem(startX - 55, startY - 4, 3817, 0);
                            AddItem(startX - 58, startY - 8, 3817, 0);

                            AddItem(startX - 37, startY - 8, 3903, 0);
                            AddItem(startX - 34, startY - 5, 3903, 0);

                            AddItem(startX - 44, startY + 15, 3973, 0);
                            AddItem(startX - 38, startY + 18, 3973, 0);

                            AddItem(startX - 50, startY + 12, 3852, 0);
                            AddItem(startX - 54, startY + 10, 3852, 0);

                            AddLabel(startX, startY, 149, "Resource Consumption:");
                            AddLabel(startX + 140, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                            AddLabel(startX + 60, startY + 15, basicRuleDetail.m_Line2Hue, basicRuleDetail.m_Line2Text);
                        }

                        if (ruleDetail.m_RuleType == typeof(ArenaRuleset.ItemDurabilityDamageType))
                        {
                            AddItem(startX - 43, startY + 0, 7031, 0);
                            AddItem(startX - 44, startY + 6, 6916, 0);

                            AddLabel(startX, startY, 149, "Item Durability:");
                            AddLabel(startX + 98, startY, basicRuleDetail.m_Line1Hue, basicRuleDetail.m_Line1Text);
                        }

                        //Buttons
                        if (playerCreatedMatch && ruleDetail.m_AccessLevel <= m_Player.AccessLevel)
                        {
                            AddButton(startX + 10, startY + 20, 2223, 2223, 100 + (index * 2), GumpButtonType.Reply, 0);
                            AddButton(startX + 35, startY + 20, 2224, 2224, 100 + (index * 2) + 1, GumpButtonType.Reply, 0);
                        }

                        #endregion

                        startY += rowSpacing;
                    }

                    #endregion

                    #region Spell Restrictions

                    startingRuleIndex = (m_ArenaGumpObject.m_SettingsPage * SpellRulesPerMatchInfoPage);

                    startX = 455;
                    startY = 215;
                    rowSpacing = 20;

                    for (int a = 0; a < SpellRulesPerCreateMatchPage; a++)
                    {
                        int index = a + startingRuleIndex;

                        if (index >= m_SpellRules.Count)
                            continue;

                        ArenaRuleDetails ruleDetail = m_SpellRules[index];
                        ArenaSpellRuleDetail spellRuleDetail = m_ArenaGumpObject.m_ArenaRuleset.GetSpellRuleDetail(index);  
                        
                        AddItem(startX + spellRuleDetail.m_ItemOffsetX, startY + spellRuleDetail.m_ItemOffsetY, spellRuleDetail.m_ItemID, spellRuleDetail.m_ItemHue);
                        AddLabel(startX, startY, 149, spellRuleDetail.m_SpellName);
                        AddLabel(startX + 120, startY, spellRuleDetail.m_TextHue, spellRuleDetail.m_RuleText);

                        //Buttons
                        if (playerCreatedMatch && ruleDetail.m_AccessLevel <= m_Player.AccessLevel)
                        {
                            AddButton(startX - 75, startY + 4, 2223, 2223, 200 + (index * 2), GumpButtonType.Reply, 0);
                            AddButton(startX - 50, startY + 4, 2224, 2224, 200 + (index * 2) + 1, GumpButtonType.Reply, 0);
                        }

                        startY += rowSpacing;
                    }
                    
                    #endregion

                    #region Item Restrictions

                    startingRuleIndex = (m_ArenaGumpObject.m_SettingsPage * ItemRulesPerMatchInfoPage);

                    startX = 455;
                    startY = 315;
                    rowSpacing = 20;

                    for (int a = 0; a < ItemRulesPerCreateMatchPage; a++)
                    {
                        int index = a + startingRuleIndex;

                        if (index >= m_ItemRules.Count)
                            continue;

                        ArenaRuleDetails ruleDetail = m_ItemRules[index];
                        ArenaItemRuleDetail itemRuleDetail = m_ArenaGumpObject.m_ArenaRuleset.GetItemRuleDetail(index);

                        AddItem(startX + itemRuleDetail.m_ItemOffsetX, startY + itemRuleDetail.m_ItemOffsetY, itemRuleDetail.m_ItemID, itemRuleDetail.m_ItemHue);
                        AddLabel(startX, startY, 149, itemRuleDetail.m_ItemName);
                        AddLabel(startX + 120, startY, itemRuleDetail.m_TextHue, itemRuleDetail.m_RuleText);

                        //Buttons
                        if (playerCreatedMatch && ruleDetail.m_AccessLevel <= m_Player.AccessLevel)
                        {
                            AddButton(startX - 75, startY + 4, 2223, 2223, 300 + (index * 2), GumpButtonType.Reply, 0);
                            AddButton(startX - 50, startY + 4, 2224, 2224, 300 + (index * 2) + 1, GumpButtonType.Reply, 0);
                        }

                        startY += rowSpacing;
                    }                    

                    #endregion

                    #endregion

                    //Controls                    
                    if (m_ArenaGumpObject.m_SettingsPage > 0)
                        AddButton(171, 467, 9909, 9909, 15, GumpButtonType.Reply, 0);

                    if (m_ArenaGumpObject.m_SettingsPage > 0 || m_ArenaGumpObject.m_SettingsPage < totalSettingsPages)
                        AddLabel(200, 468, 2599, "More Settings");

                    if (m_ArenaGumpObject.m_SettingsPage < totalSettingsPages - 1)
                        AddButton(296, 467, 9903, 9903, 16, GumpButtonType.Reply, 0);                    
                    

                    AddButton(23, 483, 4014, 4016, 0, GumpButtonType.Reply, 0);
			        AddLabel(57, 483, WhiteTextHue, "Return");

                    if (playerCreatedMatch)
                    {
                        AddButton(321, 499, 2151, 2154, 0, GumpButtonType.Reply, 0);
                        AddLabel(357, 502, 63, "Save Changes");

                        AddButton(468, 498, 2472, 2473, 0, GumpButtonType.Reply, 0);
                        AddLabel(504, 502, 1256, "Cancel Changes");
                    }
                    
                break;

                #endregion

                #region Records and Teams

                case ArenaPageType.RecordsAndTeams:
                break;

                #endregion

                #region Tournament Rounds

                case ArenaPageType.TournamentRounds:
                break;

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)  return;
            if (m_ArenaGumpObject == null) return;
            if (m_ArenaGumpObject.m_ArenaRuleset == null) return;
            if (m_ArenaGumpObject.m_ArenaGroupController == null) return;

            ArenaPlayerSettings.CheckCreateArenaPlayerSettings(m_Player);
            
            bool closeGump = true;

            #region Header Tabs

            //Header  Tabs
            switch (info.ButtonID)
            {
                //Guide
                case 1:
                break;

                //Page: Available Matches
                case 2:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;

                break;

                //Page: Scheduled Events
                case 3:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.ScheduledTournaments;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Page: Records and Teams
                case 4:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.RecordsAndTeams;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;
            }

            #endregion

            //Page Content
            switch (m_ArenaGumpObject.m_ArenaPage)
            {
                #region Available Matches

                case ArenaPageType.AvailableMatches:
                    int totalMatches = m_AvailableMatches.Count;
                    int totalMatchPages = (int)(Math.Ceiling((double)m_AvailableMatches.Count / (double)MatchListingsPerAvailableMatchesPage));
                    
                    if (m_ArenaGumpObject.m_Page >= totalMatchPages)
                        m_ArenaGumpObject.m_Page = totalMatchPages - 1;

                    if (m_ArenaGumpObject.m_Page < 0)
                        m_ArenaGumpObject.m_Page = 0;
                                        
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 10:
                            m_ArenaGumpObject.m_Page--;

                            m_Player.SendSound(ChangePageSound);
                            closeGump = false;
                        break;

                        //Next Page
                        case 11:
                            m_ArenaGumpObject.m_Page++;

                            m_Player.SendSound(ChangePageSound);
                            closeGump = false;
                        break;

                        //Create Match
                        case 12:
                            if (m_Player.m_ArenaPlayerSettings.CurrentlyInMatch())                            
                                m_Player.SendMessage("You must leave your current match before you may create a new match.");                            

                            else
                            {
                                if (m_ArenaGumpObject.m_ArenaRuleset != null)
                                    m_ArenaGumpObject.m_ArenaRuleset.Delete();

                                m_ArenaGumpObject.m_ArenaRuleset = new ArenaRuleset();
                                m_ArenaGumpObject.m_ArenaRuleset.IsTemporary = true;

                                m_ArenaGumpObject.m_ArenaPage = ArenaPageType.CreateMatch;
                                m_Player.SendSound(ChangePageSound);
                            }

                            closeGump = false;
                        break;
                    }

                    int matchIndex = 0;
                    int newTeamIndex = -1;

                    #region Join Teams

                    //Join Team 1
                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                        newTeamIndex = 0;
                        matchIndex = (info.ButtonID - 20) + (m_ArenaGumpObject.m_Page * MatchListingsPerAvailableMatchesPage);

                        if (matchIndex >= totalMatches)
                            matchIndex = totalMatches - 1;

                        if (matchIndex < 0)
                            matchIndex = 0;

                        m_Player.SendSound(LargeSelectionSound);
                        closeGump = false;
                    }

                    //Join Team 2
                    if (info.ButtonID >= 30 && info.ButtonID < 40)
                    {
                        newTeamIndex = 1;
                        matchIndex = (info.ButtonID - 30) + (m_ArenaGumpObject.m_Page * MatchListingsPerAvailableMatchesPage);

                        if (matchIndex >= totalMatches)
                            matchIndex = totalMatches - 1;

                        if (matchIndex < 0)
                            matchIndex = 0;

                        m_Player.SendSound(LargeSelectionSound);
                        closeGump = false;
                    }

                    if (newTeamIndex > -1 && m_AvailableMatches.Count > 0)
                    {
                        ArenaMatch selectedArenaMatch = m_AvailableMatches[matchIndex];

                        bool validArenaMatch = true;                        

                        if (selectedArenaMatch == null)
                        {
                            m_Player.SendMessage("That match is no longer accessible.");
                            validArenaMatch = false;
                        }

                        else if (selectedArenaMatch.Deleted)
                        {
                            m_Player.SendMessage("That match is no longer accessible.");
                            validArenaMatch = false;
                        }

                        else if (selectedArenaMatch.m_ArenaGroupController == null)
                        {
                            m_Player.SendMessage("That match is no longer accessible.");
                            validArenaMatch = false;
                        }

                        else if (selectedArenaMatch.m_ArenaGroupController.Deleted)
                        {
                            m_Player.SendMessage("That match is no longer accessible.");
                            validArenaMatch = false;
                        }

                        else if (selectedArenaMatch.m_Ruleset == null)
                        {
                            m_Player.SendMessage("That match is no longer accessible.");
                            validArenaMatch = false;
                        }

                        else if (selectedArenaMatch.m_Ruleset.Deleted)
                        {
                            m_Player.SendMessage("That match is no longer accessible.");
                            validArenaMatch = false;
                        }

                        else if (!selectedArenaMatch.CanPlayerJoinMatch(m_Player))
                        {
                            m_Player.SendMessage("You do not meet the criteria required to join that team.");
                            validArenaMatch = false;
                        }

                        else if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)
                        {
                            m_Player.SendMessage("That match is now currently in progress.");
                            validArenaMatch = false;
                        }

                        else
                        {
                            int teamSize = selectedArenaMatch.m_Ruleset.TeamSize;
                            ArenaTeam team = selectedArenaMatch.GetTeam(newTeamIndex);

                            if (team == null)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (team.Deleted)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else
                            {
                                int teamPlayers = 0;
                                int teamReadyPlayers = 0;

                                foreach (ArenaParticipant participant in team.m_Participants)
                                {
                                    if (participant == null) continue;
                                    if (participant.Deleted) continue;

                                    teamPlayers++;

                                    if (participant.m_ReadyToggled)
                                        teamReadyPlayers++;
                                }

                                if (teamPlayers >= teamSize)
                                {
                                    m_Player.SendMessage("That team is already at player capacity.");
                                    validArenaMatch = false;
                                }

                                if (validArenaMatch)
                                {
                                    ArenaTeam team1 = selectedArenaMatch.GetTeam(0);
                                    ArenaTeam team2 = selectedArenaMatch.GetTeam(1);

                                    ArenaParticipant team1Participant = null;
                                    ArenaParticipant team2Participant = null;

                                    if (team1 != null)
                                        team1Participant = team1.GetPlayerParticipant(m_Player);

                                    else
                                        validArenaMatch = false;

                                    if (team2 != null)
                                        team2Participant = team2.GetPlayerParticipant(m_Player);

                                    else
                                        validArenaMatch = false;

                                    if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                                    {
                                        //Currently In Another Match
                                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != selectedArenaMatch)
                                        {
                                            if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch.m_MatchStatus == ArenaMatch.MatchStatusType.Fighting)
                                            {
                                                m_Player.SendMessage("You are currently involved in a match still in progress.");
                                                validArenaMatch = false;
                                            }

                                            else
                                                m_Player.m_ArenaPlayerSettings.m_ArenaMatch.LeaveMatch(m_Player);
                                        }
                                    }

                                    if (validArenaMatch)
                                    {
                                        //Attempt to Join Team 1
                                        if (newTeamIndex == 0)
                                        {
                                            if (team1Participant != null)
                                                m_Player.SendMessage("You are already on this team.");

                                            else if (team2Participant != null)
                                            {
                                                if (team2.m_Participants.Contains(team2Participant))
                                                    team2.m_Participants.Remove(team2Participant);

                                                team1.m_Participants.Insert(0, team2Participant);

                                                m_Player.SendMessage("You change teams.");
                                            }

                                            else
                                            {
                                                ArenaParticipant newParticipant = new ArenaParticipant(m_Player, selectedArenaMatch, 0);

                                                m_Player.SendMessage("You join the match. When all players have clicked the 'Ready' button the fight will commence.");
                                            }
                                        }

                                         //Attempt to Join Team 2
                                        else if (newTeamIndex == 1)
                                        {
                                            if (team2Participant != null)
                                                m_Player.SendMessage("You are already on this team.");

                                            else if (team1Participant != null)
                                            {
                                                if (team1.m_Participants.Contains(team1Participant))
                                                    team1.m_Participants.Remove(team1Participant);

                                                team2.m_Participants.Insert(0, team1Participant);

                                                m_Player.SendMessage("You change teams.");
                                            }

                                            else
                                            {
                                                ArenaParticipant newParticipant = new ArenaParticipant(m_Player, selectedArenaMatch, 0);

                                                m_Player.SendMessage("You join the match. When all players have clicked the 'Ready' button the fight will commence.");
                                            }
                                        }
                                    }
                                }
                            }
                        }     
                    }

                    #endregion

                    #region View Match

                    //View Match Info
                    if (info.ButtonID >= 40 && info.ButtonID < 50)
                    {
                        matchIndex = (info.ButtonID - 40) + (m_ArenaGumpObject.m_Page * MatchListingsPerAvailableMatchesPage);

                        if (matchIndex >= totalMatches)
                            matchIndex = totalMatches - 1;

                        if (matchIndex < 0)
                            matchIndex = 0;

                        if (m_AvailableMatches.Count > 0)
                        {
                            ArenaMatch selectedArenaMatch = m_AvailableMatches[matchIndex];

                            bool validArenaMatch = true;

                            if (selectedArenaMatch == null)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (selectedArenaMatch.Deleted)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (selectedArenaMatch.m_ArenaGroupController == null)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (selectedArenaMatch.m_ArenaGroupController.Deleted)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (selectedArenaMatch.m_Ruleset == null)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (selectedArenaMatch.m_Ruleset.Deleted)
                            {
                                m_Player.SendMessage("That match is no longer accessible.");
                                validArenaMatch = false;
                            }

                            else if (!selectedArenaMatch.CanPlayerJoinMatch(m_Player))
                            {
                                m_Player.SendMessage("You do not meet the criteria required to view that match.");
                                validArenaMatch = false;
                            }

                            else if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)
                            {
                                m_Player.SendMessage("That match is now currently in progress.");
                                validArenaMatch = false;
                            }

                            if (validArenaMatch)
                            {
                                m_ArenaGumpObject.m_ArenaMatchViewing = selectedArenaMatch;
                                m_ArenaGumpObject.m_ArenaPage = ArenaPageType.MatchInfo;

                                m_Player.SendSound(SelectionSound);
                            }
                        }
                        
                        closeGump = false;
                    }

                    #endregion
                break;

                #endregion
                     
                #region Create Match

                case ArenaPageType.CreateMatch:

                    List<ArenaRuleDetails> m_BasicRules = ArenaRuleset.GetBasicRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);
                    List<ArenaRuleDetails> m_SpellRules = ArenaRuleset.GetSpellRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);
                    List<ArenaRuleDetails> m_ItemRules = ArenaRuleset.GetItemRulesDetails(m_ArenaGumpObject.m_ArenaRuleset.m_RulesetType);

                    int totalBasicRulesPages = (int)(Math.Ceiling((double)m_BasicRules.Count / (double)BasicRulesPerCreateMatchPage));
                    int totalSpellRulesPages = (int)(Math.Ceiling((double)m_SpellRules.Count / (double)SpellRulesPerCreateMatchPage));
                    int totalItemRulesPages = (int)(Math.Ceiling((double)m_ItemRules.Count / (double)ItemRulesPerCreateMatchPage));

                    int totalSettingsPages = totalBasicRulesPages;

                    if (totalSpellRulesPages > totalSettingsPages)
                        totalSettingsPages = totalSpellRulesPages;

                    if (totalItemRulesPages > totalSettingsPages)
                        totalSettingsPages = totalItemRulesPages;

                    if (m_ArenaGumpObject.m_SettingsPage >= totalSettingsPages)
                        m_ArenaGumpObject.m_SettingsPage = totalSettingsPages - 1;

                    if (m_ArenaGumpObject.m_SettingsPage < 0)
                        m_ArenaGumpObject.m_SettingsPage = 0;
                    
                    switch (info.ButtonID)
                    {
                        //Cancel
                        case 10:
                            m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;

                        //Save Preset
                        case 11:
                            ArenaRuleset.SavePlayerPresetSettings(m_ArenaGumpObject);

                            ArenaPresetDetail arenaPresetDetail = m_ArenaGumpObject.m_ArenaRuleset.GetPresetDetail();

                            m_Player.SendMessage(63, "Current Match Preset settings are now stored as [" + arenaPresetDetail.m_Text + "].");                          
                            m_Player.SendSound(SelectionSound);

                            closeGump = false;
                        break;

                        //Create Match
                        case 12:
                            if (m_Player.m_ArenaPlayerSettings.CurrentlyInMatch())
                            {
                                m_Player.SendMessage("You must leave your current match before you may create a new one.");
                            }

                            else
                            {
                                ArenaGroupController arenaGroupController = m_ArenaGumpObject.m_ArenaGroupController;
                                ArenaMatch arenaMatch = new ArenaMatch(arenaGroupController, m_Player);

                                arenaMatch.m_MatchStatus = ArenaMatch.MatchStatusType.Listed;
                                arenaMatch.m_Ruleset = m_ArenaGumpObject.m_ArenaRuleset;
                                arenaMatch.m_Ruleset.IsTemporary = false;
                                
                                arenaMatch.m_Teams.Add(new ArenaTeam());
                                arenaMatch.m_Teams.Add(new ArenaTeam());

                                ArenaParticipant participant = new ArenaParticipant(m_Player, arenaMatch, 0);

                                participant.m_ReadyToggled = true;                               

                                m_ArenaGumpObject.m_ArenaRuleset = new ArenaRuleset();
                                m_ArenaGumpObject.m_ArenaRuleset.IsTemporary = true;

                                m_ArenaGumpObject.m_ArenaPage = ArenaPageType.MatchInfo;
                                m_ArenaGumpObject.m_ArenaMatchViewing = arenaMatch;

                                arenaGroupController.m_MatchListings.Add(arenaMatch);

                                m_Player.m_ArenaPlayerSettings.m_ArenaMatch = arenaMatch;

                                m_Player.SendMessage(63, "You create a new match listing.");
                                m_Player.SendSound(ChangePageSound);

                                
                            }

                            closeGump = false;
                        break;

                        //Previous Preset 
                        case 13:
                            int presetsCount = Enum.GetNames(typeof(ArenaRuleset.ArenaPresetType)).Length;
                            int presetValue = (int)m_ArenaGumpObject.m_ArenaRuleset.m_PresetType;

                            presetValue--;

                            if (presetValue < 0)
                                presetValue = presetsCount - 1;

                            if (presetValue >= presetsCount)
                                presetValue = 0;

                            m_ArenaGumpObject.m_ArenaRuleset.m_PresetType = (ArenaRuleset.ArenaPresetType)presetValue;

                            ArenaRuleset.ApplyRulesetPreset(m_ArenaGumpObject);

                            ArenaPresetDetail GetPresetDetail = m_ArenaGumpObject.m_ArenaRuleset.GetPresetDetail();

                            m_Player.SendMessage("Loading Match Presets: " + GetPresetDetail.m_Text + ".");
                            m_Player.SendSound(LargeSelectionSound);

                            closeGump = false;
                        break;

                        //Next Preset 
                        case 14:
                            presetsCount = Enum.GetNames(typeof(ArenaRuleset.ArenaPresetType)).Length;
                            presetValue = (int)m_ArenaGumpObject.m_ArenaRuleset.m_PresetType;

                            presetValue++;

                            if (presetValue < 0)
                                presetValue = presetsCount - 1;

                            if (presetValue >= presetsCount)
                                presetValue = 0;

                            m_ArenaGumpObject.m_ArenaRuleset.m_PresetType = (ArenaRuleset.ArenaPresetType)presetValue;

                            ArenaRuleset.ApplyRulesetPreset(m_ArenaGumpObject);

                            GetPresetDetail = m_ArenaGumpObject.m_ArenaRuleset.GetPresetDetail();

                            m_Player.SendMessage("Loading Match Presets: " + GetPresetDetail.m_Text + "."); 
                            m_Player.SendSound(LargeSelectionSound);

                            closeGump = false;
                        break;

                        //Previous Settings Page
                        case 15:                            
                            m_ArenaGumpObject.m_SettingsPage--;
                           
                            m_Player.SendSound(ChangePageSound);
                            closeGump = false;
                        break;

                        //Next Settings Page
                        case 16:
                            m_ArenaGumpObject.m_SettingsPage++;

                            m_Player.SendSound(ChangePageSound);
                            closeGump = false;
                        break;
                    }

                    //Change Basic Rule Setting
                    if (info.ButtonID >= 100 && info.ButtonID < 200)
                    {
                        int ruleIndex = (int)(Math.Floor(((double)info.ButtonID - 100) / 2));

                        int changeValue = 1;

                        if (info.ButtonID % 2 == 0)
                            changeValue = -1;

                        m_ArenaGumpObject.m_ArenaRuleset.ChangeBasicSetting(m_Player, ruleIndex, changeValue);

                        m_Player.SendSound(SelectionSound);
                        closeGump = false;
                    }

                    //Change Spell Rule Setting
                    if (info.ButtonID >= 200 && info.ButtonID < 300)
                    {
                        int ruleIndex = (int)(Math.Floor(((double)info.ButtonID - 200) / 2));

                        int changeValue = 1;

                        if (info.ButtonID % 2 == 0)
                            changeValue = -1;

                        m_ArenaGumpObject.m_ArenaRuleset.ChangeSpellSetting(m_Player, ruleIndex, changeValue);

                        m_Player.SendSound(SelectionSound);
                        closeGump = false;
                    }

                    //Change Item Rule Setting
                    if (info.ButtonID >= 300 && info.ButtonID < 400)
                    {
                        int ruleIndex = (int)(Math.Floor(((double)info.ButtonID - 300) / 2));

                        int changeValue = 1;

                        if (info.ButtonID % 2 == 0)
                            changeValue = -1;

                        m_ArenaGumpObject.m_ArenaRuleset.ChangeItemSetting(m_Player, ruleIndex, changeValue);

                        m_Player.SendSound(SelectionSound);
                        closeGump = false;
                    }

                break;

                #endregion

                #region Match Info

                case ArenaPageType.MatchInfo:
                    switch (info.ButtonID)
                    {
                        //Return
                        case 10:
                        break;

                        //Previous Settings
                        case 11:
                        break;

                        //Next Settings
                        case 12:
                        break;

                        //Select Team 1
                        case 13:
                        break;

                        //Select Team 2
                        case 14:
                        break;

                        //Message Team 1
                        case 15:
                        break;

                        //Message Team 2
                        case 16:
                        break;

                        //Message All
                        case 17:
                        break;

                        //Ready + Start Match
                        case 18:
                        break;

                        //Cancel Match
                        case 19:
                        break;
                    }    
                break;

                #endregion
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ArenaGump));
                m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }

    public class ArenaGumpObject
    {
        public PlayerMobile m_Player;

        public ArenaGroupController m_ArenaGroupController;
        public ArenaGump.ArenaPageType m_ArenaPage = ArenaGump.ArenaPageType.AvailableMatches;
        public int m_Page = 0;
        public int m_SettingsPage = 0;

        public ArenaRuleset m_ArenaRuleset;
        public ArenaMatch m_ArenaMatchViewing;

        public ArenaGumpObject(PlayerMobile player, ArenaGroupController arenaGroupController)
        {
            m_Player = player;
            m_ArenaGroupController = arenaGroupController;
            m_ArenaRuleset = new ArenaRuleset();
        }
    }
}