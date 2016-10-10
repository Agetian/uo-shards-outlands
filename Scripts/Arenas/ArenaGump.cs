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
            RanksAndRecords,
            CreditsAndRewards,

            CreateMatch,
            MatchInfo,
            TournamentMatches
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

            #region Available Matches

            AddItem(15, 28, 8454, 2515);
            AddItem(36, 28, 8455, 2515);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.AvailableMatches) 
                AddButton(75, 41, 9724, 9721, 2, GumpButtonType.Reply, 0);
            else
                AddButton(75, 41, 9721, 9724, 2, GumpButtonType.Reply, 0);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.TournamentMatches)
            {
                AddLabel(108, 35, 2603, "Tournament");
                AddLabel(118, 53, 2603, "Matches");
            }

            else
            {
                AddLabel(108, 35, 2603, "Available");
                AddLabel(108, 53, 2603, "Matches");
            }

            #endregion

            #region Tournaments

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.TournamentMatches)
            {
                AddItem(175, 43, 9556, 0);
                AddItem(205, 35, 3976, 0);
                AddItem(219, 47, 3852, 0);

                if (arenaGumpObject.m_ArenaPage == ArenaPageType.ScheduledTournaments)
                    AddButton(244, 41, 9724, 9721, 3, GumpButtonType.Reply, 0);
                else
                    AddButton(244, 41, 9721, 9724, 3, GumpButtonType.Reply, 0);

                AddLabel(279, 35, 53, "Tournament");
                AddLabel(290, 53, 53, "Ruleset");
            }

            else
            {
                AddItem(194, 29, 2826);
                AddItem(193, 25, 9920);

                if (arenaGumpObject.m_ArenaPage == ArenaPageType.ScheduledTournaments)
                    AddButton(244, 41, 9724, 9721, 3, GumpButtonType.Reply, 0);
                else
                    AddButton(244, 41, 9721, 9724, 3, GumpButtonType.Reply, 0);

                AddLabel(285, 35, 53, "Scheduled");
                AddLabel(277, 55, 53, "Tournaments");
            }

            #endregion

            #region Ranks and Records

            AddItem(383, 56, 5357);
            AddItem(375, 43, 4030);
            AddItem(377, 34, 4031);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.RanksAndRecords)
                AddButton(414, 41, 9724, 9721, 4, GumpButtonType.Reply, 0);  
            else
                AddButton(414, 41, 9721, 9724, 4, GumpButtonType.Reply, 0);

            AddLabel(455, 35, 2606, "Ranks");
            AddLabel(461, 48, 2606, "and");
            AddLabel(448, 61, 2606, "Records");

            #endregion

            #region Credits and Rewards

            AddItem(519, 25, 13042, 0);
            AddItem(519, 36, 5441, 2052);

            if (arenaGumpObject.m_ArenaPage == ArenaPageType.CreditsAndRewards)
                AddButton(568, 41, 9724, 9721, 5, GumpButtonType.Reply, 0);
            else
                AddButton(568, 41, 9721, 9724, 5, GumpButtonType.Reply, 0);

            AddLabel(605, 35, 90, "Credits");
            AddLabel(617, 48, 90, "and");
            AddLabel(605, 61, 90, "Rewards");

            #endregion

            switch (arenaGumpObject.m_ArenaPage)
            {
                #region Available Matches

                case ArenaPageType.AvailableMatches:                    
                    m_AvailableMatches = m_ArenaGumpObject.m_ArenaGroupController.GetArenaMatches(m_Player);

                    ArenaMatch playersCurrentMatch = m_Player.m_ArenaPlayerSettings.m_ArenaMatch;

                    bool hasCreatedMatchActive = false;

                    if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                    {
                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch.Deleted)
                            m_Player.m_ArenaPlayerSettings.m_ArenaMatch = null;

                        else
                        {
                            if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch.m_Creator == m_Player)
                                hasCreatedMatchActive = true;
                        }

                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                            playersCurrentMatch = m_Player.m_ArenaPlayerSettings.m_ArenaMatch;
                    }
                    
                    if (playersCurrentMatch != null && m_AvailableMatches.Contains(playersCurrentMatch))
                    {  
                        AddLabel(302, 84, 63, "Current Match");

                        #region Images 

                        AddImage(18, 154, 96, 1102);
                        AddImage(173, 154, 96, 1102);
                        AddImage(300, 154, 96, 1102);
                        AddImage(409, 154, 96, 1102);
                        AddImage(491, 154, 96, 1102);
                        AddImage(18, 153, 96, 1102);
                        AddImage(173, 153, 96, 1102);
                        AddImage(300, 153, 96, 1102);
                        AddImage(409, 153, 96, 1102);
                        AddImage(491, 153, 96, 1102);

                        #endregion

                        AddLabel(294, 144, 2603, "Available Matches");
                    }

                    else                    
                        AddLabel(293, 84, 2603, "Available Matches");                    
                                        
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

                    //Always Place Current Match at Top of Page 
                    if (playersCurrentMatch != null && m_AvailableMatches.Contains(playersCurrentMatch))
                    {
                        m_AvailableMatches.Remove(playersCurrentMatch);
                        m_AvailableMatches.Insert(matchStartIndex, playersCurrentMatch);
                    }

                    //Matches
                    int matchCount = matchEndIndex - matchStartIndex;
                                        
                    int startY = 110;
                    int rowSpacing = 50;
                                        
                    for (int a = 0; a < matchCount + 1; a++)
                    {
                        if (totalMatches == 0)
                            continue;

                        ArenaMatch arenaMatch = null;

                        if (a == 0 && playersCurrentMatch != null)
                            arenaMatch = playersCurrentMatch;

                        else
                        {
                            int matchIndex = matchStartIndex + a;

                            if (matchIndex >= totalMatches)
                                continue;

                            arenaMatch = m_AvailableMatches[matchIndex];
                        }
                        
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

                        ArenaParticipant arenaParticipant = arenaMatch.GetParticipant(m_Player);
                                                
                        if (team1 != null)
                        {
                            if (team1.GetPlayerParticipant(m_Player) != null)                            
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
                            if (team2.GetPlayerParticipant(m_Player) != null)                                
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

                        int playerCountTextHue = 149;

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
                                AddLabel(Utility.CenteredTextOffset(162, matchName), startY + 20, WhiteTextHue, matchName);                        
                        }

                        #region Soldier Icons
                        
                        //Team 1 Statues
                        for (int b = 0; b < teamSize; b++)
                        {
                            int playerIconHue = 1105;

                            if (team1 != null)
                            {
                                for (int c = 0; c < team1.m_Participants.Count; c++)
                                {
                                    if (b != c) 
                                        continue;
                                
                                    ArenaParticipant participant = team1.m_Participants[c];

                                    if (participant == null)
                                        continue;

                                    playerIconHue = 2500;

                                    if (participant.m_ReadyToggled)                                    
                                        playerIconHue = 2208;                                    
                                }
                            }

                            AddItem(35 - ((teamSize - 1) * 7) + (b * 7), startY - 10, 15178, playerIconHue);
                        }

                        //Team 2 Statues
                        for (int b = 0; b < teamSize; b++)
                        {
                            int playerIconHue = 1105;

                            if (team2 != null)
                            {
                                for (int c = 0; c < team2.m_Participants.Count; c++)
                                {
                                    if (b != c)
                                        continue;
                                
                                    ArenaParticipant participant = team2.m_Participants[c];

                                    if (participant == null)
                                        continue;

                                    playerIconHue = 2500;

                                    if (participant.m_ReadyToggled)
                                        playerIconHue = 2208;                                                             
                                }
                            }

                            AddItem(60 + (b * 7), startY - 10, 15179, playerIconHue);
                        }

                        #endregion
                        
                        //Team 1
                        if (isOnTeam1)
                            AddLabel(385, startY, 63, "Team 1");

                        else
                            AddLabel(385, startY, 149, "Team 1");

                        AddLabel(385, startY + 20, 2550, "Players:");

                        if (team1Players >= teamSize && team1ReadyPlayers >= teamSize)
                            AddLabel(438, startY + 20, 63, team1Players.ToString() + "/" + teamSize.ToString());

                        else if (team1Players >= teamSize)
                            AddLabel(438, startY + 20, WhiteTextHue, team1Players.ToString() + "/" + teamSize.ToString());

                        else
                            AddLabel(438, startY + 20, 2401, team1Players.ToString() + "/" + teamSize.ToString());

                        //Team 2
                        if (isOnTeam2)
                            AddLabel(573, startY, 63, "Team 2");

                        else
                            AddLabel(573, startY, 149, "Team 2");

                        AddLabel(573, startY + 20, 2550, "Players:");

                        if (team2Players >= teamSize && team2ReadyPlayers >= teamSize)
                            AddLabel(633, startY + 20, 63, team2Players.ToString() + "/" + teamSize.ToString());

                        else if (team2Players >= teamSize)
                            AddLabel(633, startY + 20, WhiteTextHue, team2Players.ToString() + "/" + teamSize.ToString());

                        else
                            AddLabel(633, startY + 20, 2401, team2Players.ToString() + "/" + teamSize.ToString());

                        if (playersCurrentMatch == arenaMatch)
                        {
                            //Team 1
                            if (isOnTeam1)
                            {                                
                                if (arenaParticipant.m_ReadyToggled)
                                {
                                    AddLabel(305, startY + 9, 63, "Ready");
                                    AddButton(347, startY + 6, 2154, 2151, 20 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(305, startY + 9, WhiteTextHue, "Ready");
                                    AddButton(347, startY + 6, 2151, 2154, 20 + a, GumpButtonType.Reply, 0);
                                }
                            }

                            else
                            {
                                if (team1Players < teamSize)
                                {
                                    AddLabel(318, startY + 9, WhiteTextHue, "Join");
                                    AddButton(347, startY + 6, 9721, 9724, 20 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(318, startY + 9, WhiteTextHue, "Full");
                                    AddImage(347, startY + 6, 9721, 1102);
                                }
                            }

                            //Team 2
                            if (isOnTeam2)
                            {                                
                                if (arenaParticipant.m_ReadyToggled)
                                {
                                    AddLabel(491, startY + 9, 63, "Ready");
                                    AddButton(533, startY + 6, 2154, 2151, 30 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(491, startY + 9, WhiteTextHue, "Ready");
                                    AddButton(533, startY + 6, 2151, 2154, 30 + a, GumpButtonType.Reply, 0);
                                }                                
                            }

                            else
                            {
                                if (team2Players < teamSize)
                                {
                                    AddLabel(502, startY + 9, WhiteTextHue, "Join");
                                    AddButton(533, startY + 6, 9721, 9724, 30 + a, GumpButtonType.Reply, 0);
                                }

                                else
                                {
                                    AddLabel(502, startY + 9, WhiteTextHue, "Full");
                                    AddImage(533, startY + 6, 9721, 1102);
                                }
                            }
                        }

                        else if (playersCurrentMatch == null)
                        {
                            if (team1Players < teamSize)
                            {
                                AddLabel(318, startY + 9, WhiteTextHue, "Join");
                                AddButton(347, startY + 6, 9721, 9724, 20 + a, GumpButtonType.Reply, 0);
                            }

                            else
                            {
                                AddLabel(318, startY + 9, 1102, "Full");
                                AddImage(347, startY + 6, 9721, 1102);
                            }

                            if (team2Players < teamSize)
                            {
                                AddLabel(502, startY + 9, WhiteTextHue, "Join");
                                AddButton(533, startY + 6, 9721, 9724, 30 + a, GumpButtonType.Reply, 0);
                            }

                            else
                            {
                                AddLabel(502, startY + 9, 1102, "Full");
                                AddImage(533, startY + 6, 9721, 1102);
                            }
                        }

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

                    if (m_ArenaGumpObject.m_Page < totalMatchPages - 1)
                    {
                        AddButton(563, 483, 4005, 4007, 11, GumpButtonType.Reply, 0);
                        AddLabel(599, 483, WhiteTextHue, "Next Page");
                    }

                    AddButton(196, 483, 4029, 4031, 13, GumpButtonType.Reply, 0);
                    AddLabel(230, 483, 2603, "Refresh Listings");

                    if (playersCurrentMatch != null)
                    {
                        AddButton(366, 483, 4008, 4010, 12, GumpButtonType.Reply, 0);

                        if (hasCreatedMatchActive)
                            AddLabel(400, 483, 1256, "Cancel Current Match");

                        else
                            AddLabel(400, 483, 1256, "Leave Current Match");
                    }

                    else
                    {
                        AddButton(366, 483, 4008, 4010, 12, GumpButtonType.Reply, 0);
                        AddLabel(400, 483, 63, "Create New Match");
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

                    if (!ArenaMatch.IsValidArenaMatch(selectedArenaMatch, m_Player, true))
                    {
                        m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                        m_ArenaGumpObject.m_ArenaMatchViewing = null;

                        m_Player.CloseGump(typeof(ArenaGump));
                        m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                        m_Player.SendMessage("That match is now longer viewable.");

                        return;
                    }

                    if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)
                    {
                        m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                        m_ArenaGumpObject.m_ArenaMatchViewing = null;

                        m_Player.CloseGump(typeof(ArenaGump));
                        m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                        m_Player.SendMessage("That match is now currently in progress.");

                        return;
                    }

                    ArenaTeam arenaTeam1 = selectedArenaMatch.GetTeam(0);
                    ArenaTeam arenaTeam2 = selectedArenaMatch.GetTeam(1);

                    ArenaParticipant playerParticipant = selectedArenaMatch.GetParticipant(m_Player);

                    playersCurrentMatch = m_Player.m_ArenaPlayerSettings.m_ArenaMatch;

                    hasCreatedMatchActive = false;

                    if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                    {
                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch.Deleted)
                            m_Player.m_ArenaPlayerSettings.m_ArenaMatch = null;

                        else
                        {
                            if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch.m_Creator == m_Player)
                                hasCreatedMatchActive = true;
                        }

                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                            playersCurrentMatch = m_Player.m_ArenaPlayerSettings.m_ArenaMatch;
                    }

                    int teamPlayersNeeded = selectedArenaMatch.m_Ruleset.TeamSize;

                    int team1PlayerCount = 0;
                    int team1ReadyPlayerCount = 0;

                    int team2PlayerCount = 0;
                    int team2ReadyPlayerCount = 0;

                    bool playerCreatedMatch = false;

                    bool playerIsOnTeam1 = false;
                    bool playerIsOnTeam2 = false;

                    if (arenaTeam1.GetPlayerParticipant(m_Player) != null)
                        playerIsOnTeam1 = true;

                    if (arenaTeam2.GetPlayerParticipant(m_Player) != null)
                        playerIsOnTeam2 = true;

                    if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch == selectedArenaMatch)
                    {
                        if (selectedArenaMatch.m_Creator == m_Player)
                            playerCreatedMatch = true;
                    }
                    
                    AddLabel(316, 84, 2603, "Match Info");

                    #region Images

                    AddImage(18, 206, 96, 1102);
			        AddImage(173, 206, 96, 1102);
			        AddImage(300, 206, 96, 1102);
			        AddImage(409, 206, 96, 1102);
			        AddImage(491, 206, 96, 1102);
			        AddImage(18, 205, 96, 1102);
			        AddImage(173, 205, 96, 1102);
			        AddImage(300, 205, 96, 1102);
			        AddImage(409, 205, 96, 1102);
			        AddImage(491, 205, 96, 1102);

                    #endregion

			        AddLabel(300, 196, 2603, "Match Settings");

                    #region Team 1

                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(53, 177, 30008, 30009, 21, GumpButtonType.Reply, 0);
                        AddLabel(74, 174, 54, "Message Team");
                    }

                    startY = 120;
                    rowSpacing = 15;

                    int statueStartX = 39 - ((teamPlayersNeeded - 1) * 7);
                                        
                    for (int a = 0; a < teamPlayersNeeded; a++)
                    {
                        int playerTextHue = 1102;
                        int playerIconHue = 1105;                       

                        string playerName = "";

                        for (int b = 0; b < teamPlayersNeeded; b++)
                        {
                            if (a != b) continue;          
                            if (b >= arenaTeam1.m_Participants.Count) continue;

                            ArenaParticipant participant = arenaTeam1.m_Participants[b];

                            if (participant == null) 
                                continue;

                            team1PlayerCount++;
                            
                            playerTextHue = WhiteTextHue;
                            playerIconHue = 2500;   

                            if (participant.m_Player != null)
                                playerName = participant.m_Player.RawName;

                            if (participant.m_ReadyToggled)
                            {                              
                                team1ReadyPlayerCount++;

                                playerTextHue = 63;
                                playerIconHue = 2208;                               
                            }                                
                        }

                        AddItem(statueStartX + (a * 15), 108, 15178, playerIconHue);

                        if (playerName != "")
                        {
                            AddButton(145, startY + 3, 1209, 1210, 30 + a, GumpButtonType.Reply, 0);
                            AddLabel(165, startY, playerTextHue, playerName);
                        }

                        else
                        {
                            playerName = "(open player slot)";
                            AddLabel(165, startY, playerTextHue, playerName);
                        }
                        
                        startY += rowSpacing;
                    }                     

                    #endregion

                    #region Team 2

                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(312, 177, 30008, 30009, 23, GumpButtonType.Reply, 0);
                        AddLabel(333, 174, 54, "Message Team");
                    }

                    startY = 120;
                    rowSpacing = 15;

                    statueStartX = 298 - ((teamPlayersNeeded - 1) * 7);

                    for (int a = 0; a < teamPlayersNeeded; a++)
                    {
                        int playerTextHue = 1102;
                        int playerIconHue = 1105;                        

                        string playerName = "";

                        for (int b = 0; b < teamPlayersNeeded; b++)
                        {
                            if (a != b) continue;          
                            if (b >= arenaTeam2.m_Participants.Count) continue;

                            ArenaParticipant participant = arenaTeam2.m_Participants[b];

                            if (participant == null) 
                                continue;

                            team2PlayerCount++;

                            playerTextHue = WhiteTextHue;
                            playerIconHue = 2500;                           

                            if (participant.m_Player != null)
                                playerName = participant.m_Player.RawName;

                            if (participant.m_ReadyToggled)
                            {                              
                                team2ReadyPlayerCount++;

                                playerTextHue = 63;
                                playerIconHue = 2208;                               
                            }                                
                        }

                        AddItem(statueStartX + (a * 15), 108, 15178, playerIconHue);

                        if (playerName != "")
                        {
                            AddButton(405, startY + 3, 1209, 1210, 40 + a, GumpButtonType.Reply, 0);
                            AddLabel(425, startY, playerTextHue, playerName);
                        }

                        else
                        {
                            playerName = "(open player slot)";
                            AddLabel(425, startY, playerTextHue, playerName);
                        }
                        
                        startY += rowSpacing;
                    }                  

                    #endregion

                    #region Ready / Join

                    if (playerIsOnTeam1)
                        AddLabel(99, 100, 63, "Team 1");
                    else
                        AddLabel(99, 100, 149, "Team 1");

                    if (playerIsOnTeam2)
                        AddLabel(356, 100, 63, "Team 2");
                    else
                        AddLabel(356, 100, 149, "Team 2");
                    
                    if (playersCurrentMatch == selectedArenaMatch)
                    {
                        //Team 1
                        if (playerIsOnTeam1 && playerParticipant != null)
                        {
                            if (playerParticipant.m_ReadyToggled)
                            {
                                AddButton(104, 120, 2154, 2151, 20, GumpButtonType.Reply, 0);
                                AddLabel(100, 148, 63, "Ready");                               
                            }

                            else
                            {
                                AddButton(104, 120, 2151, 2154, 20, GumpButtonType.Reply, 0);
                                AddLabel(100, 148, WhiteTextHue, "Ready");                               
                            }
                        }

                        else
                        {
                            if (team1PlayerCount < teamPlayersNeeded)
                            {
                                AddButton(104, 120, 9721, 9724, 20, GumpButtonType.Reply, 0);
                                AddLabel(107, 148, WhiteTextHue, "Join");
                            }

                            else
                            {
                                AddImage(104, 120, 9721, 1102);
                                AddLabel(109, 148, 1102, "Full");
                            }
                        }

                        //Team 2
                        if (playerIsOnTeam2)
                        {
                            if (playerParticipant.m_ReadyToggled)
                            {
                                AddButton(362, 120, 2154, 2151, 22, GumpButtonType.Reply, 0);
                                AddLabel(358, 148, 63, "Ready");
                               
                            }

                            else
                            {
                                AddButton(362, 120, 2151, 2154, 22, GumpButtonType.Reply, 0);
                                AddLabel(358, 148, WhiteTextHue, "Ready");                              
                            }
                        }

                        else
                        {
                            if (team2PlayerCount < teamPlayersNeeded)
                            {
                                AddButton(362, 120, 9721, 9724, 22, GumpButtonType.Reply, 0);
                                AddLabel(363, 148, WhiteTextHue, "Join");
                            }

                            else
                            {
                                AddImage(362, 120, 9721, 1102);
                                AddLabel(367, 148, 1102, "Full");
                            }
                        }
                    }

                    else if (playersCurrentMatch == null)
                    {
                        if (team1PlayerCount < teamPlayersNeeded)
                        {
                            AddButton(104, 120, 9721, 9724, 20, GumpButtonType.Reply, 0);
                            AddLabel(109, 148, WhiteTextHue, "Join");
                        }

                        else
                        {
                            AddImage(104, 120, 9721, 1102);
                            AddLabel(109, 148, 1102, "Full");
                        }

                        if (team2PlayerCount < teamPlayersNeeded)
                        {
                            AddButton(362, 120, 9721, 9724, 22, GumpButtonType.Reply, 0);
                            AddLabel(363, 148, WhiteTextHue, "Join");
                        }

                        else
                        {
                            AddImage(362, 120, 9721, 1102);
                            AddLabel(367, 148, 1102, "Full");
                        }
                    }

                    #endregion

                    #region Match Controls
                   	
                    //Match Controls
                    AddButton(541, 106, 4029, 4031, 18, GumpButtonType.Reply, 0);
                    AddLabel(579, 106, 2603, "Refresh Page");

                    if (playerCreatedMatch)
                    {
                        AddButton(543, 137, 2472, 2473, 10, GumpButtonType.Reply, 0);
                        AddLabel(579, 141, 1256, "Cancel Match");
                    }

                    else if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(543, 137, 2472, 2473, 10, GumpButtonType.Reply, 0);
                        AddLabel(579, 141, 1256, "Leave Match");
                    }

                    if (playerIsOnTeam1 || playerIsOnTeam2)
                    {
                        AddButton(543, 170, 9721, 9724, 11, GumpButtonType.Reply, 0);
                        AddLabel(578, 174, 54, "Message All");
                    }   
                 
                    #endregion

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

                    #endregion

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

                    #region Settings Controls      
          
                    AddButton(23, 483, 4014, 4016, 13, GumpButtonType.Reply, 0);
			        AddLabel(57, 483, WhiteTextHue, "Return");

                    //Previous Settings Page
                    if (m_ArenaGumpObject.m_SettingsPage > 0)
                        AddButton(171, 467, 9909, 9909, 14, GumpButtonType.Reply, 0);

                    if (m_ArenaGumpObject.m_SettingsPage > 0 || m_ArenaGumpObject.m_SettingsPage < totalSettingsPages)
                        AddLabel(200, 468, 2599, "More Settings");

                    //Next Settings Page
                    if (m_ArenaGumpObject.m_SettingsPage < totalSettingsPages - 1)
                        AddButton(296, 467, 9903, 9903, 15, GumpButtonType.Reply, 0);                   

                    if (playerCreatedMatch && m_ArenaGumpObject.ArenaRulesetEdited == true)
                    {
                        AddButton(388, 499, 2151, 2154, 16, GumpButtonType.Reply, 0);
                        AddLabel(424, 502, 63, "Save Changes");

                        AddButton(535, 498, 2472, 2473, 17, GumpButtonType.Reply, 0);
                        AddLabel(571, 502, 2550, "Cancel Changes");
                    }

                    #endregion
                    
                break;

                #endregion

                #region Ranks and Records

                case ArenaPageType.RanksAndRecords:
                break;

                #endregion

                #region Tournament Rounds

                case ArenaPageType.ScheduledTournaments:
                    #region TEST

                AddLabel(102, 98, 63, "Round 1");
			    AddLabel(329, 98, 149, "Round 2");
			    AddLabel(549, 98, 149, "Round 3");
			    AddItem(248, 128, 6169);
			    AddLabel(315, 156, WhiteTextHue, "Azure Paladins");
                AddLabel(349, 141, WhiteTextHue, "vs");
                AddLabel(313, 126, WhiteTextHue, "Outlands Rangers");
			    AddLabel(263, 156, 149, "Info");
			    AddButton(245, 160, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(116, 221, 7574);
			    AddLabel(99, 232, 1102, "Deathwatch");
                AddLabel(132, 217, WhiteTextHue, "vs");
			    AddLabel(95, 202, 2599, "Azure Paladins");
			    AddLabel(47, 232, 149, "Info");
			    AddButton(29, 236, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(29, 202, 7960);
			    AddImage(19, 119, 96, 1102);
			    AddImage(174, 119, 96, 1102);
			    AddImage(301, 119, 96, 1102);
			    AddImage(410, 119, 96, 1102);
			    AddImage(492, 119, 96, 1102);
			    AddImage(227, 95, 2701, 1105);
			    AddImage(227, 121, 2701, 1105);
			    AddLabel(70, 503, 63, "Focus on Current Match");
			    AddImage(463, 95, 2701, 1105);
			    AddImage(463, 121, 2701, 1105);
			    AddItem(116, 296, 7574);
			    AddLabel(99, 307, 1102, "Nightstalkers");
                AddLabel(132, 292, WhiteTextHue, "vs");
			    AddLabel(96, 277, 2599, "Chosen Destiny");
			    AddLabel(47, 307, 149, "Info");
			    AddButton(29, 311, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(29, 277, 7960);
			    AddItem(248, 205, 6169);
                AddLabel(352, 233, WhiteTextHue, "?");
                AddLabel(349, 218, WhiteTextHue, "vs");
                AddLabel(318, 203, WhiteTextHue, "Chosen Destiny");
			    AddLabel(263, 233, 149, "Info");
			    AddButton(245, 237, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(248, 278, 6169);
                AddLabel(352, 306, WhiteTextHue, "?");
                AddLabel(349, 291, WhiteTextHue, "vs");
                AddLabel(352, 276, WhiteTextHue, "?");
			    AddLabel(263, 306, 149, "Info");
			    AddButton(245, 310, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddButton(490, 500, 9721, 9724, 0, GumpButtonType.Reply, 0);
			    AddLabel(526, 503, 53, "Manage Tournament");
			    AddButton(29, 500, 2151, 2151, 0, GumpButtonType.Reply, 0);
			    AddItem(116, 147, 7574);
			    AddLabel(99, 158, 1102, "Team Fortune");
                AddLabel(132, 143, WhiteTextHue, "vs");
			    AddLabel(91, 128, 2599, "Outlands Rangers");
			    AddLabel(47, 158, 149, "Info");
			    AddButton(29, 162, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(29, 128, 7960);
			    AddLabel(97, 383, 63, "Chosen Destiny");
			    AddLabel(132, 368, 63, "vs");
			    AddLabel(96, 353, 63, "Azure Paladins");
			    AddLabel(47, 383, 63, "Info");
			    AddItem(-3, 352, 3937);
			    AddItem(23, 353, 3936);
			    AddButton(29, 387, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(32, 426, 6169);
                AddLabel(84, 454, WhiteTextHue, "Skull and Crossbones");
                AddLabel(132, 439, WhiteTextHue, "vs");
                AddLabel(103, 424, WhiteTextHue, "Deathwatch");
			    AddLabel(47, 454, 149, "Info");
			    AddButton(29, 458, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(248, 354, 6169);
                AddLabel(352, 382, WhiteTextHue, "?");
                AddLabel(349, 367, WhiteTextHue, "vs");
                AddLabel(352, 352, WhiteTextHue, "?");
			    AddLabel(263, 382, 149, "Info");
			    AddButton(245, 386, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(489, 126, 6169);
                AddLabel(593, 154, WhiteTextHue, "?");
                AddLabel(590, 139, WhiteTextHue, "vs");
                AddLabel(593, 124, WhiteTextHue, "?");
			    AddLabel(504, 154, 149, "Info");
			    AddButton(486, 158, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddItem(490, 205, 6169);
                AddLabel(594, 233, WhiteTextHue, "?");
                AddLabel(591, 218, WhiteTextHue, "vs");
                AddLabel(594, 203, WhiteTextHue, "?");
			    AddLabel(505, 233, 149, "Info");
			    AddButton(487, 237, 1210, 248, 0, GumpButtonType.Reply, 0);
			    AddImage(324, 503, 2444, 2401);
			    AddButton(346, 476, 9900, 248, 0, GumpButtonType.Reply, 0);
			    AddButton(347, 530, 9906, 248, 0, GumpButtonType.Reply, 0);
			    AddButton(390, 503, 9903, 248, 0, GumpButtonType.Reply, 0);
			    AddButton(300, 505, 9909, 248, 0, GumpButtonType.Reply, 0);
			    AddLabel(336, 504, 149, "Scroll");
			    AddImage(16, 94, 96, 1102);
			    AddImage(171, 94, 96, 1102);
			    AddImage(298, 94, 96, 1102);
			    AddImage(407, 94, 96, 1102);
			    AddImage(489, 94, 96, 1102);
			    AddImage(16, 93, 96, 1102);
			    AddImage(171, 93, 96, 1102);
			    AddImage(298, 93, 96, 1102);
			    AddImage(407, 93, 96, 1102);
			    AddImage(489, 93, 96, 1102);

                #endregion
                break;

                #endregion

                #region Tournament Rounds

                case ArenaPageType.TournamentMatches:
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

            ArenaMatch selectedArenaMatch = null;

            bool validArenaMatch = false;

            bool playerCreatedMatch = false;

            bool playerIsOnTeam1 = false;
            bool playerIsOnTeam2 = false;

            bool team1Full = false;
            bool team2Full = false;

            ArenaParticipant playerParticipant = null;

            ArenaTeam arenaTeam1 = null;
            ArenaTeam arenaTeam2 = null;

            ArenaTeam newTeam = null;
            
            bool closeGump = true;

            #region Header Tabs

            //Header  Tabs
            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Page: Available Matches
                case 2:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Page: Scheduled Tournaments
                case 3:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.ScheduledTournaments;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Page: Ranks and Records
                case 4:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.RanksAndRecords;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Page: Credits and Rewards
                case 5:
                    m_ArenaGumpObject.m_ArenaPage = ArenaPageType.CreditsAndRewards;
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

                        //Leave Match / Create Match
                        case 12:
                            if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                            {
                                if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch.m_Creator == m_Player)
                                    m_Player.m_ArenaPlayerSettings.m_ArenaMatch.CancelMatch();

                                else
                                {
                                    m_Player.m_ArenaPlayerSettings.m_ArenaMatch.LeaveMatch(m_Player, true, true);
                                    m_Player.SendMessage("You leave your current match.");
                                }
                            }                           

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

                        //Refresh Matches
                        case 13:
                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break;
                    }

                    int matchIndex = 0;
                    int newTeamIndex = -1;

                    #region Join Teams

                    //Ready or Join Team 1
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

                    //Ready or Team 2
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
                        selectedArenaMatch = m_AvailableMatches[matchIndex];

                        if (!ArenaMatch.IsValidArenaMatch(selectedArenaMatch, m_Player, true))
                        {
                            m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                            m_ArenaGumpObject.m_ArenaMatchViewing = null;

                            m_Player.CloseGump(typeof(ArenaGump));
                            m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));
                            
                            m_Player.SendMessage("That match is now longer viewable.");

                            return;
                        }

                        if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)
                        {
                            m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                            m_ArenaGumpObject.m_ArenaMatchViewing = null;

                            m_Player.CloseGump(typeof(ArenaGump));
                            m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));
                            
                            m_Player.SendMessage("That match is now currently in progress.");

                            return;
                        }

                        int teamSize = selectedArenaMatch.m_Ruleset.TeamSize;                            

                        arenaTeam1 = selectedArenaMatch.GetTeam(0);
                        arenaTeam2 = selectedArenaMatch.GetTeam(1);

                        newTeam = selectedArenaMatch.GetTeam(newTeamIndex);

                        playerParticipant = selectedArenaMatch.GetParticipant(m_Player);

                        playerCreatedMatch = false;

                        playerIsOnTeam1 = false;
                        playerIsOnTeam2 = false;

                        team1Full = false;
                        team2Full = false;

                        teamSize = selectedArenaMatch.m_Ruleset.TeamSize;

                        int team1Players = 0;
                        int team1ReadyPlayers = 0;

                        int team2Players = 0;
                        int team2ReadyPlayers = 0;

                        if (arenaTeam1.GetPlayerParticipant(m_Player) != null)
                            playerIsOnTeam1 = true;

                        foreach (ArenaParticipant participant in arenaTeam1.m_Participants)
                        {
                            if (participant == null) continue;
                            if (participant.Deleted) continue;

                            team1Players++;
                        }

                        if (arenaTeam2.GetPlayerParticipant(m_Player) != null)
                            playerIsOnTeam2 = true;

                        foreach (ArenaParticipant participant in arenaTeam2.m_Participants)
                        {
                            if (participant == null) continue;
                            if (participant.Deleted) continue;

                            team2Players++;
                        }

                        if (team1Players >= teamSize)
                            team1Full = true;

                        if (team2Players >= teamSize)
                            team2Full = true;

                        if (arenaTeam1.GetPlayerParticipant(m_Player) != null)
                            playerIsOnTeam1 = true;

                        if (arenaTeam2.GetPlayerParticipant(m_Player) != null)
                            playerIsOnTeam2 = true;

                        if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch == selectedArenaMatch)
                        {
                            if (selectedArenaMatch.m_Creator == m_Player)
                                playerCreatedMatch = true;
                        }

                        //Currently Already in Match
                        if (playerParticipant != null)
                        {
                            if (newTeamIndex == 0 && playerIsOnTeam1)                            
                                playerParticipant.m_ReadyToggled = !playerParticipant.m_ReadyToggled;                            

                            else if (newTeamIndex == 1 && playerIsOnTeam2)                            
                                playerParticipant.m_ReadyToggled = !playerParticipant.m_ReadyToggled;                            

                            else if (newTeamIndex == 0 && !playerIsOnTeam1)
                            {
                                if (team1Full)
                                    m_Player.SendMessage("That team is already at player capacity.");

                                else
                                {
                                    if (arenaTeam2.m_Participants.Contains(playerParticipant))
                                        arenaTeam2.m_Participants.Remove(playerParticipant);

                                    if (playerCreatedMatch)
                                        arenaTeam1.m_Participants.Insert(0, playerParticipant);

                                    else
                                        arenaTeam1.m_Participants.Add(playerParticipant);


                                    playerParticipant.m_ReadyToggled = false;

                                    selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has switched teams.", 0);

                                    selectedArenaMatch.ParticipantsForceGumpUpdate();
                                }
                            }

                            else if (newTeamIndex == 1 && !playerIsOnTeam2)
                            {
                                if (team2Full)
                                    m_Player.SendMessage("That team is already at player capacity.");

                                else
                                {
                                    if (arenaTeam1.m_Participants.Contains(playerParticipant))
                                        arenaTeam1.m_Participants.Remove(playerParticipant);

                                    if (playerCreatedMatch)
                                        arenaTeam2.m_Participants.Insert(0, playerParticipant);

                                    else
                                        arenaTeam2.m_Participants.Add(playerParticipant);

                                    playerParticipant.m_ReadyToggled = false;

                                    selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has switched teams.", 0);

                                    selectedArenaMatch.ParticipantsForceGumpUpdate();
                                }
                            }
                        }

                        else if (m_Player.m_ArenaPlayerSettings.CurrentlyInMatch())
                        {
                            m_Player.SendMessage("You must leave your current match before joining another one.");
                        }

                        else
                        {
                            if (newTeamIndex == 0)
                            {
                                if (team1Full)
                                    m_Player.SendMessage("That team is already at player capacity.");

                                else
                                {
                                    selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has joined the match.", 0);
                                    
                                    ArenaParticipant newArenaParticipant = new ArenaParticipant(m_Player, selectedArenaMatch, 0);

                                    m_Player.m_ArenaPlayerSettings.m_ArenaMatch = selectedArenaMatch;

                                    m_Player.SendMessage("You join the match.");

                                    selectedArenaMatch.ParticipantsForceGumpUpdate();
                                }
                            }

                            if (newTeamIndex == 1)
                            {
                                if (team2Full)
                                    m_Player.SendMessage("That team is already at player capacity.");

                                else
                                {
                                    selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has joined the match.", 0);

                                    ArenaParticipant newArenaParticipant = new ArenaParticipant(m_Player, selectedArenaMatch, 1);

                                    m_Player.m_ArenaPlayerSettings.m_ArenaMatch = selectedArenaMatch;

                                    m_Player.SendMessage("You join the match.");

                                    selectedArenaMatch.ParticipantsForceGumpUpdate();
                                }
                            }
                        }

                        closeGump = false;                           
                    }

                    #endregion

                    #region Match Selection

                    //Select Match
                    if (info.ButtonID >= 40 && info.ButtonID < 50)
                    {
                        matchIndex = (info.ButtonID - 40) + (m_ArenaGumpObject.m_Page * MatchListingsPerAvailableMatchesPage);

                        if (matchIndex >= totalMatches)
                            matchIndex = totalMatches - 1;

                        if (matchIndex < 0)
                            matchIndex = 0;

                        if (m_AvailableMatches.Count > 0)
                        {
                            selectedArenaMatch = m_AvailableMatches[matchIndex];

                            if (!ArenaMatch.IsValidArenaMatch(selectedArenaMatch, m_Player, true))
                            {
                                m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                                m_ArenaGumpObject.m_ArenaMatchViewing = null;

                                m_Player.CloseGump(typeof(ArenaGump));
                                m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                                m_Player.SendMessage("That match is now longer viewable.");

                                return;
                            }

                            if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)
                            {
                                m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                                m_ArenaGumpObject.m_ArenaMatchViewing = null;

                                m_Player.CloseGump(typeof(ArenaGump));
                                m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                                m_Player.SendMessage("That match is now currently in progress.");

                                return;
                            }
                            
                            ArenaRuleset.CopyRulesetSettings(selectedArenaMatch.m_Ruleset, m_ArenaGumpObject.m_ArenaRuleset);

                            m_ArenaGumpObject.ArenaRulesetEdited = false;

                            m_ArenaGumpObject.m_ArenaMatchViewing = selectedArenaMatch;
                            m_ArenaGumpObject.m_ArenaPage = ArenaPageType.MatchInfo;                                

                            m_Player.SendSound(SelectionSound);                            
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
                                m_Player.SendMessage("You must leave your current match before you may create a new one.");                            

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

                                m_ArenaGumpObject.m_ArenaRuleset = new ArenaRuleset();
                                m_ArenaGumpObject.m_ArenaRuleset.IsTemporary = true;                               

                                arenaGroupController.m_MatchListings.Add(arenaMatch);

                                m_Player.m_ArenaPlayerSettings.m_ArenaMatch = arenaMatch;

                                m_Player.SendMessage(63, "You create a new match listing.");
                                m_Player.SendSound(ChangePageSound);

                                m_ArenaGumpObject.ArenaRulesetEdited = false;

                                m_ArenaGumpObject.m_ArenaPage = ArenaPageType.MatchInfo;
                                m_ArenaGumpObject.m_ArenaMatchViewing = arenaMatch;

                                ArenaRuleset.CopyRulesetSettings(arenaMatch.m_Ruleset, m_ArenaGumpObject.m_ArenaRuleset);
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
                    selectedArenaMatch = m_ArenaGumpObject.m_ArenaMatchViewing;
                    
                    if (!ArenaMatch.IsValidArenaMatch(selectedArenaMatch, m_Player, true))
                    {
                        m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                        m_ArenaGumpObject.m_ArenaMatchViewing = null;

                        m_Player.CloseGump(typeof(ArenaGump));
                        m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                        m_Player.SendMessage("That match is now longer viewable.");

                        return;
                    }

                    if (selectedArenaMatch.m_MatchStatus != ArenaMatch.MatchStatusType.Listed)
                    {
                        m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;
                        m_ArenaGumpObject.m_ArenaMatchViewing = null;

                        m_Player.CloseGump(typeof(ArenaGump));
                        m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                        m_Player.SendMessage("That match is now currently in progress.");

                        return;
                    }

                    arenaTeam1 = selectedArenaMatch.GetTeam(0);
                    arenaTeam2 = selectedArenaMatch.GetTeam(1);
                    
                    int teamPlayersNeeded = selectedArenaMatch.m_Ruleset.TeamSize;

                    int team1PlayerCount = 0;
                    int team1ReadyPlayerCount = 0;

                    int team2PlayerCount = 0;
                    int team2ReadyPlayerCount = 0;

                    playerCreatedMatch = false;

                    playerIsOnTeam1 = false;
                    playerIsOnTeam2 = false;

                    team1Full = false;
                    team2Full = false;

                    if (team1PlayerCount >= teamPlayersNeeded)
                        team1Full = true;

                    if (team2PlayerCount >= teamPlayersNeeded)
                        team2Full = true;

                    playerParticipant = selectedArenaMatch.GetParticipant(m_Player);

                    if (arenaTeam1.GetPlayerParticipant(m_Player) != null)
                        playerIsOnTeam1 = true;

                    if (arenaTeam2.GetPlayerParticipant(m_Player) != null)
                        playerIsOnTeam2 = true;

                    if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch == selectedArenaMatch)
                    {
                        if (selectedArenaMatch.m_Creator == m_Player)
                            playerCreatedMatch = true;
                    }

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

                    switch (info.ButtonID)
                    {
                        //Leave or Cancel Match
                        case 10:
                            if (playerCreatedMatch)                            
                                selectedArenaMatch.CancelMatch();

                            else if (playerIsOnTeam1 || playerIsOnTeam2)
                            {
                                selectedArenaMatch.LeaveMatch(m_Player, true, true);

                                m_Player.SendMessage("You leave the match.");
                            }

                            m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;

                            closeGump = false;
                        break;

                        //Message All
                        case 11:
                            if (playerParticipant != null)
                            {
                                List<ArenaParticipant> m_ArenaParticipants = selectedArenaMatch.GetParticipants();

                                foreach (ArenaParticipant participant in m_ArenaParticipants)
                                {
                                    if (participant == null) continue;
                                    if (participant.Deleted) continue;
                                    if (participant.m_Player == null) continue;

                                    if (participant.m_Player == m_Player)
                                        continue;

                                    //TEST: CHANGE TO TEXT PROMPT
                                    string message = "[Arena: " + m_Player.RawName + "] " + "TEST.";

                                    participant.m_Player.SendMessage(message, 2550);
                                } 
                            }

                            closeGump = false;
                        break;

                        //Team 1: Join + Ready
                        case 20:
                            if (playerParticipant != null)
                            {
                                if (playerIsOnTeam1)
                                    playerParticipant.m_ReadyToggled = !playerParticipant.m_ReadyToggled;

                                else
                                {
                                    if (team1Full)
                                        m_Player.SendMessage("That team is already at player capacity.");

                                    else
                                    {
                                        if (arenaTeam2.m_Participants.Contains(playerParticipant))
                                            arenaTeam2.m_Participants.Remove(playerParticipant);

                                        if (playerCreatedMatch)
                                            arenaTeam1.m_Participants.Insert(0, playerParticipant);

                                        else
                                            arenaTeam1.m_Participants.Add(playerParticipant);

                                        selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has joined the match.", 0);

                                        selectedArenaMatch.ParticipantsForceGumpUpdate();
                                    }
                                }
                            }

                            else if (m_Player.m_ArenaPlayerSettings.m_ArenaMatch != null)
                                m_Player.SendMessage("You must leave your current match before joining another one.");

                            else
                            {
                                if (team1Full)
                                    m_Player.SendMessage("That team is already at player capacity.");

                                else
                                {
                                    selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has joined the match.", 0);                                    

                                    ArenaParticipant newArenaParticipant = new ArenaParticipant(m_Player, selectedArenaMatch, 0);

                                    m_Player.m_ArenaPlayerSettings.m_ArenaMatch = selectedArenaMatch;

                                    m_Player.SendMessage("You join the match.");

                                    selectedArenaMatch.ParticipantsForceGumpUpdate();
                                }
                            }

                            closeGump = false;
                        break;

                        //Team 1: Message
                        case 21:
                            if (playerParticipant != null)
                            { 
                                foreach (ArenaParticipant participant in arenaTeam1.m_Participants)
                                {
                                    if (participant == null) continue;
                                    if (participant.Deleted) continue;
                                    if (participant.m_Player == null) continue;

                                    if (participant.m_Player == m_Player)
                                        continue;

                                    //TEST: CHANGE TO TEXT PROMPT
                                    string message = "[Arena: "+ m_Player.RawName + "] " + "TEST.";

                                    participant.m_Player.SendMessage(message, 2550);
                                }                                
                            }

                            closeGump = false;
                        break;

                        //Team 2: Join + Ready
                        case 22:                            
                            if (playerParticipant != null)
                            {
                                if (playerIsOnTeam2)
                                    playerParticipant.m_ReadyToggled = !playerParticipant.m_ReadyToggled;

                                else
                                {
                                    if (team2Full)
                                        m_Player.SendMessage("That team is already at player capacity.");

                                    else
                                    {
                                        if (arenaTeam1.m_Participants.Contains(playerParticipant))
                                            arenaTeam1.m_Participants.Remove(playerParticipant);
                                        
                                        if (playerCreatedMatch)
                                            arenaTeam2.m_Participants.Insert(0, playerParticipant);

                                        else
                                            arenaTeam2.m_Participants.Add(playerParticipant);

                                        selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has joined the match.", 0);

                                        selectedArenaMatch.ParticipantsForceGumpUpdate();
                                    }
                                }
                            }

                            else if (m_Player.m_ArenaPlayerSettings.CurrentlyInMatch())
                                m_Player.SendMessage("You must leave your current match before joining another one.");

                            else
                            {
                                if (team2Full)
                                    m_Player.SendMessage("That team is already at player capacity.");

                                else
                                {
                                    selectedArenaMatch.BroadcastMessage(m_Player.RawName + " has joined the match.", 0);

                                    ArenaParticipant newArenaParticipant = new ArenaParticipant(m_Player, selectedArenaMatch, 1);

                                    m_Player.m_ArenaPlayerSettings.m_ArenaMatch = selectedArenaMatch;

                                    m_Player.SendMessage("You join the match.");

                                    selectedArenaMatch.ParticipantsForceGumpUpdate();
                                }
                            }

                            closeGump = false;
                        break;

                        //Team 2: Message
                        case 23:
                            if (playerParticipant != null)
                            {
                                foreach (ArenaParticipant participant in arenaTeam2.m_Participants)
                                {
                                    if (participant == null) continue;
                                    if (participant.Deleted) continue;
                                    if (participant.m_Player == null) continue;

                                    if (participant.m_Player == m_Player)
                                        continue;

                                    //TEST: CHANGE TO TEXT PROMPT
                                    string message = "[Arena: " + m_Player.RawName + "] " + "TEST.";

                                    participant.m_Player.SendMessage(message, 2550);
                                } 
                            }

                            closeGump = false;
                        break;

                        //-----

                        //Return
                        case 13:
                            m_ArenaGumpObject.m_ArenaPage = ArenaPageType.AvailableMatches;

                            closeGump = false;
                        break;

                        //Previous Settings
                        case 14:
                            if (m_ArenaGumpObject.m_SettingsPage > 0)
                            {
                                m_ArenaGumpObject.m_SettingsPage--;
                                m_Player.SendSound(ChangePageSound);
                            }

                            closeGump = false;
                        break;

                        //Next Settings
                        case 15:
                            if (m_ArenaGumpObject.m_SettingsPage < totalSettingsPages - 1)
                            {
                                m_ArenaGumpObject.m_SettingsPage++;
                                m_Player.SendSound(ChangePageSound);
                            }

                            closeGump = false;
                        break;

                        //Save Changes
                        case 16:
                            if (playerCreatedMatch)
                            {
                                ArenaRuleset.CopyRulesetSettings(m_ArenaGumpObject.m_ArenaRuleset, selectedArenaMatch.m_Ruleset);

                                List<ArenaParticipant> m_Participants = selectedArenaMatch.GetParticipants();

                                foreach (ArenaParticipant participant in m_Participants)
                                {
                                    if (participant == null) continue;
                                    if (participant.Deleted) continue;
                                    if (participant.m_Player == null) continue;

                                    if (participant.m_Player == m_Player)
                                        m_Player.SendMessage("You change the rules for the arena match.");

                                    else
                                    {
                                        if (participant.m_ReadyToggled)
                                            participant.m_Player.SendMessage(1256, "Rules of your arena match have changed. Review the changes and click 'Ready'.");

                                        else
                                            participant.m_Player.SendMessage(1256, "Rules of your arena match have changed.");
                                    }

                                    participant.m_Player.SendSound(0x5B6);
                                }

                                m_ArenaGumpObject.ArenaRulesetEdited = false;

                                m_Player.SendSound(SelectionSound);

                                //TEST: NEED TO DUMP EXTRA TEAMMATES IF DOWNGRADING TEAMSIZE SETTING
                                //TEST: Make sure new players meet criteria (i.e. belong to Guild / Party
                            }

                            closeGump = false;
                        break;

                        //Cancel Changes
                        case 17:
                            if (playerCreatedMatch)
                            {
                                m_ArenaGumpObject.ArenaRulesetEdited = false;

                                ArenaRuleset.CopyRulesetSettings(selectedArenaMatch.m_Ruleset, m_ArenaGumpObject.m_ArenaRuleset);
                            }

                            closeGump = false;
                        break;

                        //Refresh Listing
                        case 18:
                            m_Player.SendSound(ChangePageSound);

                            closeGump = false;
                        break; 
                    }

                    //Team 1 Player Selection
                    if (info.ButtonID >= 30 && info.ButtonID < 40)
                    {
                        int playerIndex = info.ButtonID - 30;

                        if (playerIndex < arenaTeam1.m_Participants.Count)
                        {
                            ArenaParticipant targetParticipant = arenaTeam1.m_Participants[playerIndex];

                            if (targetParticipant.m_Player != null)
                            {
                                m_Player.CloseGump(typeof(ArenaGump));
                                m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                                m_Player.SendGump(new ArenaPlayerInfoGump(m_Player,  targetParticipant.m_Player, selectedArenaMatch, m_ArenaGumpObject));

                                return; 
                            }
                        }

                        closeGump = false;
                    }

                    //Team 2 Player Selection
                    if (info.ButtonID >= 40 && info.ButtonID < 50)
                    {
                        int playerIndex = info.ButtonID - 40;

                        if (playerIndex < arenaTeam2.m_Participants.Count)
                        {
                            ArenaParticipant targetParticipant = arenaTeam2.m_Participants[playerIndex];

                            if (targetParticipant.m_Player != null)
                            {
                                m_Player.CloseGump(typeof(ArenaGump));
                                m_Player.SendGump(new ArenaGump(m_Player, m_ArenaGumpObject));

                                m_Player.SendGump(new ArenaPlayerInfoGump(m_Player, targetParticipant.m_Player, selectedArenaMatch, m_ArenaGumpObject));

                                return;
                            }
                        }

                        closeGump = false;
                    }

                    //Change Basic Rule Setting
                    if (playerCreatedMatch)
                    {
                        if (info.ButtonID >= 100 && info.ButtonID < 200)
                        {
                            int ruleIndex = (int)(Math.Floor(((double)info.ButtonID - 100) / 2));

                            int changeValue = 1;

                            if (info.ButtonID % 2 == 0)
                                changeValue = -1;

                            m_ArenaGumpObject.m_ArenaRuleset.ChangeBasicSetting(m_Player, ruleIndex, changeValue);
                            m_ArenaGumpObject.ArenaRulesetEdited = true;

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
                            m_ArenaGumpObject.ArenaRulesetEdited = true;

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
                            m_ArenaGumpObject.ArenaRulesetEdited = true;

                            m_Player.SendSound(SelectionSound);
                            closeGump = false;
                        }
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

        public bool ArenaRulesetEdited = false;

        public ArenaGumpObject(PlayerMobile player, ArenaGroupController arenaGroupController)
        {
            m_Player = player;
            m_ArenaGroupController = arenaGroupController;
            m_ArenaRuleset = new ArenaRuleset();
        }
    }

    public class ArenaPlayerInfoGump : Gump
    {
        public PlayerMobile m_Player;
        public PlayerMobile m_TargetPlayer;
        public ArenaMatch m_ArenaMatch;
        public ArenaGumpObject m_ArenaGumpObject;

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public ArenaPlayerInfoGump(PlayerMobile player, PlayerMobile targetPlayer, ArenaMatch arenaMatch, ArenaGumpObject arenaGumpObject): base(10, 10)
        {
            m_Player = player;
            m_TargetPlayer = targetPlayer;
            m_ArenaMatch = arenaMatch;
            m_ArenaGumpObject = arenaGumpObject;

            if (m_Player == null) return;
            if (m_TargetPlayer == null) return;
            if (m_ArenaMatch == null) return;
            if (m_ArenaGumpObject == null) return;

            //TEST: VALIDATE ARENA MATCH AND PLAYER

            AddImage(4, 3, 1248, 2401);

            AddLabel(44, 13, 163, "Team Member:");
            AddLabel(140, 13, WhiteTextHue, m_TargetPlayer.RawName);

            AddLabel(139, 50, 2606, "Ranked Matches");
            AddLabel(135, 70, 2599, "Wins");
            AddLabel(198, 70, 1256, "Losses");
            AddLabel(75, 95, 149, "1 vs 1");
            AddLabel(69, 115, 149, "2 vs 2");
            AddLabel(69, 135, 149, "3 vs 3");
            AddLabel(69, 155, 149, "4 vs 4");
            AddLabel(142, 95, WhiteTextHue, "10");
            AddLabel(211, 95, WhiteTextHue, "10");
            AddLabel(142, 115, WhiteTextHue, "10");
            AddLabel(211, 115, WhiteTextHue, "10");
            AddLabel(142, 135, WhiteTextHue, "10");
            AddLabel(211, 135, WhiteTextHue, "10");
            AddLabel(142, 155, WhiteTextHue, "10");
            AddLabel(211, 155, WhiteTextHue, "10");

            AddLabel(140, 182, 2625, "Tournaments");
            AddLabel(92, 202, 2603, "Events");
            AddLabel(98, 222, 2603, "Won");
            AddLabel(161, 202, 2599, "Rounds");
            AddLabel(168, 222, 2599, "Won");
            AddLabel(231, 202, 1256, "Rounds");
            AddLabel(237, 222, 1256, "Lost");
            AddLabel(44, 242, 149, "1 vs 1");
            AddLabel(38, 262, 149, "2 vs 2");
            AddLabel(38, 282, 149, "3 vs 3");
            AddLabel(38, 302, 149, "4 vs 4");
            AddLabel(106, 242, WhiteTextHue, "10");
            AddLabel(176, 242, WhiteTextHue, "10");
            AddLabel(243, 242, WhiteTextHue, "10");
            AddLabel(106, 262, WhiteTextHue, "10");
            AddLabel(176, 262, WhiteTextHue, "10");
            AddLabel(243, 262, WhiteTextHue, "10");
            AddLabel(106, 282, WhiteTextHue, "10");
            AddLabel(176, 282, WhiteTextHue, "10");
            AddLabel(243, 282, WhiteTextHue, "10");
            AddLabel(106, 302, WhiteTextHue, "10");
            AddLabel(176, 302, WhiteTextHue, "10");
            AddLabel(243, 302, WhiteTextHue, "10");

            AddLabel(65, 370, 63, "Send Message");
            AddButton(34, 365, 2151, 2154, 1, GumpButtonType.Reply, 0);

            AddLabel(191, 370, 53, "Kick from Match");
            AddButton(160, 365, 9721, 9724, 2, GumpButtonType.Reply, 0);

            AddButton(89, 326, 2473, 2473, 3, GumpButtonType.Reply, 0);
            AddLabel(118, 330, 2115, "Ban From Match");            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_ArenaGumpObject == null) return;
            if (m_ArenaGumpObject.m_ArenaRuleset == null) return;
            if (m_ArenaGumpObject.m_ArenaGroupController == null) return;
        }
    }
}