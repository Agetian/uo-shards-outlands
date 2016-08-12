using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class ArenaGump : Gump
    {
        public enum ArenaPageType
        {
            AvailableMatches,
            CreateMatch,
            MatchInfo,

            TournamentRounds
        }

        public enum ArenaRulsetButtonPress
        {
        }

        public PlayerMobile m_Player;
        public ArenaPageType m_ArenaPage = ArenaPageType.MatchInfo; //TEST
        public int m_Page = 0;
        public int m_SettingsPage = 0;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x3E6;
        public static int PurchaseSound = 0x2E6;
        public static int CloseGumpSound = 0x058;

        public int WhiteTextHue = 2499;

        public ArenaGump(PlayerMobile player): base(10, 10)
        {
            m_Player = player;
            
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
            AddButton(75, 41, 9724, 9721, 2, GumpButtonType.Reply, 0);
            AddLabel(108, 35, 2603, "Available");
            AddLabel(108, 53, 2603, "Matches");            
           
            //Scheduled Tournaments
            AddItem(211, 29, 2826);
            AddItem(208, 23, 9920);
            AddButton(261, 41, 9721, 9724, 3, GumpButtonType.Reply, 0);
            AddLabel(302, 35, 53, "Scheduled");
            AddLabel(294, 55, 53, "Tournaments");
                 
            //Teams and Records
            AddItem(413, 56, 5357);
            AddItem(405, 43, 4030);
            AddItem(407, 34, 4031);
            AddButton(444, 41, 9721, 9724, 4, GumpButtonType.Reply, 0);      
            AddLabel(481, 35, 2606, "Teams");
            AddLabel(491, 48, 2606, "and");
            AddLabel(476, 61, 2606, "Records");

            switch (m_ArenaPage)
            {
                case ArenaPageType.AvailableMatches:
                    AddLabel(293, 84, 2603, "Available Matches");

                    //Matches

                    AddItem(12, 97, 15178);
			        AddLabel(137, 108, 0, "4 vs 4");
			        AddLabel(128, 128, 2550, "Unranked");
			        AddItem(59, 97, 15179, 2499);
			        AddItem(68, 97, 15179, 2499);
			        AddItem(77, 97, 15179, 2499);
			        AddItem(84, 97, 15179, 2499);
			        AddItem(21, 97, 15178);
			        AddItem(27, 97, 15178);
			        AddItem(35, 97, 15178);
			        AddButton(347, 116, 2151, 2154, 0, GumpButtonType.Reply, 0);
			        AddLabel(385, 108, 149, "Team 1");
			        AddLabel(385, 128, 0, "Players:");
			        AddLabel(444, 128, 2401, "3/4");
			        AddLabel(313, 119, 2599, "Join");
			        AddButton(533, 114, 2151, 2154, 0, GumpButtonType.Reply, 0);
			        AddLabel(573, 108, 149, "Team 2");
			        AddLabel(573, 128, 0, "Players:");
			        AddLabel(633, 128, 2401, "0/4");
			        AddLabel(501, 118, 2599, "Join");
			        AddLabel(217, 108, 149, "Match Info");
			        AddButton(243, 130, 30008, 248, 0, GumpButtonType.Reply, 0);

                    //-----

                    AddButton(23, 483, 4014, 4016, 5, GumpButtonType.Reply, 0);
                    AddLabel(57, 483, WhiteTextHue, "Previous Page");

                    AddButton(283, 483, 4008, 4010, 6, GumpButtonType.Reply, 0);
			        AddLabel(317, 483, 63, "Create New Match");

                    AddButton(563, 483, 4005, 4007, 7, GumpButtonType.Reply, 0);
                    AddLabel(599, 483, WhiteTextHue, "Next Page");
                break;

                case ArenaPageType.CreateMatch:
                    AddLabel(289, 84, 63, "Create New Match");

                    //Rules: Left Side
                    AddLabel(67, 160, 149, "Round Duration:");
                    AddLabel(174, 160, WhiteTextHue, "5 Minutes Normal +");
                    AddLabel(174, 177, 2550, "3 Minutes of Sudden Death");
			        AddButton(82, 180, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 180, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(22, 157, 6169);
			        AddItem(23, 105, 15178);
			        AddLabel(67, 122, 149, "Match Type:");
			        AddLabel(147, 122, WhiteTextHue, "1 vs 1 - Unranked");
			        AddButton(82, 142, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 142, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(67, 198, 149, "Sudden Death Mode:");
                    AddLabel(199, 198, WhiteTextHue, "+25% Damage Per Minute");
			        AddButton(82, 218, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 218, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(20, 192, 7960);
			        AddItem(33, 229, 5073);
			        AddLabel(69, 236, 149, "Equipment:");
                    AddLabel(140, 235, WhiteTextHue, "GM Only (Regular Materials)");
			        AddButton(82, 256, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 256, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(28, 272, 5118, 2208);
			        AddLabel(67, 274, 149, "Poisoned Weapons:");
                    AddLabel(191, 274, WhiteTextHue, "None Allowed");
			        AddButton(82, 294, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 294, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(20, 307, 8484, 2500);
			        AddLabel(67, 312, 149, "Mounts:");
                    AddLabel(126, 312, WhiteTextHue, "None Allowed");
			        AddButton(81, 333, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(104, 333, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(20, 348, 8532);
			        AddLabel(67, 350, 149, "Followers:");
                    AddLabel(139, 350, WhiteTextHue, "None Allowed");
			        AddButton(81, 371, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(104, 371, 2224, 248, 0, GumpButtonType.Reply, 0);			        

                    //Rules: Right Side
                    AddItem(421, 122, 8023);
			        AddLabel(460, 122, 149, "Poison");
			        AddLabel(579, 122, 2550, "25 Casts Max");
			        AddItem(427, 142, 7981);
			        AddLabel(460, 142, 149, "Poison Field");
			        AddLabel(579, 142, 2550, "10 Casts Max");
			        AddItem(427, 162, 7985);
			        AddLabel(460, 162, 149, "Paralyze");
			        AddLabel(579, 162, 2550, "25 Casts Max");
			        AddItem(421, 182, 8023);
			        AddLabel(460, 182, 149, "Paralyze Field");
			        AddLabel(579, 182, 2550, "10 Casts Max");
			        AddItem(426, 201, 7985);
			        AddLabel(460, 201, 149, "AoE Spells");
			        AddLabel(579, 201, 2401, "Disabled");
			        AddButton(385, 126, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 126, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 146, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 146, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 166, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 166, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 185, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 185, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 205, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 205, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(425, 241, 3852);
			        AddLabel(459, 241, 149, "Heal Potions");
			        AddLabel(580, 241, 2550, "10 Uses Max");
			        AddItem(425, 261, 3847);
			        AddLabel(459, 261, 149, "Cure Potions");
			        AddLabel(580, 261, 2550, "25 Uses Max");
			        AddItem(425, 281, 3851);
			        AddLabel(459, 281, 149, "Refresh Potions");
			        AddLabel(580, 281, 63, "Unlimited");
			        AddItem(425, 301, 3849);
			        AddLabel(459, 301, 149, "Strength Potions");
			        AddLabel(580, 301, 2550, "10 Uses Max");
			        AddItem(425, 321, 3848);
			        AddLabel(459, 321, 149, "Agility Potions");
			        AddLabel(580, 321, 2550, "10 Uses Max");
			        AddItem(425, 341, 3853);
			        AddLabel(459, 341, 149, "Explosion Potions");
			        AddLabel(580, 341, 2550, "5 Uses Max");
			        AddItem(425, 361, 3850);
			        AddLabel(458, 361, 149, "Poison Potions");
			        AddLabel(580, 361, 2550, "3 Uses Max");
			        AddItem(425, 381, 3846);
			        AddLabel(459, 381, 149, "Resist Potions");
			        AddLabel(580, 381, 2401, "Disabled");
			        AddButton(385, 245, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 245, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 265, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 265, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 285, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 285, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 305, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 305, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 325, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 325, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 344, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 344, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 364, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 364, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(385, 385, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 385, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(417, 223, 2480);
			        AddLabel(459, 221, 149, "Trapped Pouches");
			        AddLabel(580, 221, 2550, "25 Uses Max");
			        AddButton(385, 225, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(408, 225, 2224, 248, 0, GumpButtonType.Reply, 0);

                    //Controls
                    AddButton(179, 402, 9909, 248, 0, GumpButtonType.Reply, 0);
                    AddLabel(208, 403, 2599, "More Settings");
			        AddButton(304, 402, 9903, 248, 0, GumpButtonType.Reply, 0);

                    AddButton(124, 483, 9909, 248, 0, GumpButtonType.Reply, 0);
                    AddLabel(208, 464, 149, "Match Presets");
			        AddLabel(194, 484, 2606, "Default Tournament");
                    AddButton(358, 482, 9903, 248, 0, GumpButtonType.Reply, 0);

                    AddLabel(57, 483, WhiteTextHue, "Cancel");
                    AddButton(23, 483, 4014, 4016, 0, GumpButtonType.Reply, 0);

                    AddButton(405, 483, 4011, 4016, 0, GumpButtonType.Reply, 0);
			        AddLabel(439, 483, 2550, "Save Presets");

                    AddButton(540, 483, 4008, 4016, 0, GumpButtonType.Reply, 0);
			        AddLabel(573, 483, 63, "Create Match");
                break;

                case ArenaPageType.MatchInfo:
                    AddLabel(316, 84, 2603, "Match Info");

                    //Teams
                    AddLabel(74, 179, 54, "Message");
			        AddButton(53, 182, 30008, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(548, 138, 2472, 9724, 0, GumpButtonType.Reply, 0);
			        AddLabel(582, 141, 1256, "Cancel Match");
			        AddButton(546, 170, 9721, 2473, 0, GumpButtonType.Reply, 0);
			        AddLabel(582, 174, 54, "Message All");
			        AddButton(104, 120, 9724, 9721, 0, GumpButtonType.Reply, 0);
			        AddLabel(99, 100, 63, "Team 1");
			        AddLabel(165, 120, 2603, "Luthius");
			        AddLabel(41, 161, 2401, "[3/4]");
			        AddLabel(99, 148, 2401, "Leave");
			        AddButton(146, 123, 1210, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(165, 137, 2599, "Merrill Calder");
			        AddButton(146, 140, 1210, 248, 0, GumpButtonType.Reply, 0);
                    AddLabel(165, 153, WhiteTextHue, "Fendrake");
			        AddButton(146, 156, 1210, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(165, 169, 2401, "Empty");
			        AddItem(61, 108, 15178, 1102);
			        AddItem(46, 108, 15178);
			        AddItem(31, 108, 15178, 63);
			        AddItem(16, 108, 15178, 63);
			        AddLabel(356, 100, 149, "Team 2");
			        AddButton(362, 120, 2151, 2154, 0, GumpButtonType.Reply, 0);
			        AddLabel(423, 120, 2599, "Serathi");
			        AddLabel(363, 148, 2599, "Join");
			        AddButton(404, 123, 1210, 248, 0, GumpButtonType.Reply, 0);
                    AddLabel(423, 137, WhiteTextHue, "Garet Fleshborne");
			        AddButton(404, 140, 1210, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(423, 153, 2401, "Empty");
			        AddButton(404, 156, 1210, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(423, 169, 2401, "Empty");
			        AddItem(319, 108, 15179, 1102);
			        AddItem(304, 108, 15179, 1102);
			        AddItem(289, 108, 15179, 2500);
			        AddItem(274, 108, 15179, 63);
			        AddLabel(300, 161, 2401, "[2/4]");
			        AddLabel(333, 179, 54, "Message");
			        AddButton(312, 182, 30008, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(546, 105, 2154, 9721, 0, GumpButtonType.Reply, 0);
			        AddLabel(582, 109, 63, "Ready");

                    //Rules: Left Side
                    AddLabel(67, 254, 149, "Round Duration:");
                    AddLabel(174, 254, WhiteTextHue, "5 Minutes Normal +");
			        AddButton(82, 274, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 274, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(22, 251, 6169);
			        AddItem(23, 199, 15178);
			        AddLabel(67, 216, 149, "Match Type:");
                    AddLabel(147, 216, WhiteTextHue, "1 vs 1 - Unranked");
			        AddButton(82, 236, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 236, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(67, 292, 149, "Sudden Death Mode:");
                    AddLabel(199, 292, WhiteTextHue, "+25% Damage Per Minute");
			        AddButton(82, 312, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 312, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(20, 286, 7960);
			        AddItem(33, 323, 5073);
			        AddLabel(69, 330, 149, "Equipment:");
                    AddLabel(140, 329, WhiteTextHue, "GM Only (Regular Materials)");
			        AddButton(82, 350, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 350, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(28, 366, 5118, 2208);
			        AddLabel(67, 368, 149, "Poisoned Weapons:");
                    AddLabel(191, 368, WhiteTextHue, "None Allowed");
			        AddButton(82, 388, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(105, 388, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(20, 401, 8484, 2500);
			        AddLabel(67, 406, 149, "Mounts:");
                    AddLabel(126, 406, WhiteTextHue, "None Allowed");
			        AddButton(81, 427, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(104, 427, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(20, 442, 8532);
			        AddLabel(67, 444, 149, "Followers:");
                    AddLabel(139, 444, WhiteTextHue, "None Allowed");
			        AddButton(81, 465, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(104, 465, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(171, 271, 2550, " 3 Minutes of Sudden Death");

                    //Rules: Right Side
                    AddItem(418, 215, 8023);
			        AddLabel(457, 215, 149, "Poison");
			        AddLabel(576, 215, 2550, "25 Casts Max");
			        AddItem(424, 235, 7981);
			        AddLabel(457, 235, 149, "Poison Field");
			        AddLabel(576, 235, 2550, "10 Casts Max");
			        AddItem(424, 255, 7985);
			        AddLabel(457, 255, 149, "Paralyze");
			        AddLabel(576, 255, 2550, "25 Casts Max");
			        AddItem(418, 275, 8023);
			        AddLabel(457, 275, 149, "Paralyze Field");
			        AddLabel(576, 275, 2550, "10 Casts Max");
			        AddItem(423, 294, 7985);
			        AddLabel(457, 294, 149, "AoE Spells");
			        AddLabel(576, 294, 2401, "Disabled");
			        AddButton(382, 219, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 219, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 239, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 239, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 259, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 259, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 278, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 278, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 298, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 298, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(422, 334, 3852);
			        AddLabel(456, 334, 149, "Heal Potions");
			        AddLabel(577, 334, 2550, "10 Uses Max");
			        AddItem(422, 354, 3847);
			        AddLabel(456, 354, 149, "Cure Potions");
			        AddLabel(577, 354, 2550, "25 Uses Max");
			        AddItem(422, 374, 3851);
			        AddLabel(456, 374, 149, "Refresh Potions");
			        AddLabel(577, 374, 63, "Unlimited");
			        AddItem(422, 394, 3849);
			        AddLabel(456, 394, 149, "Strength Potions");
			        AddLabel(577, 394, 2550, "10 Uses Max");
			        AddItem(422, 414, 3848);
			        AddLabel(456, 414, 149, "Agility Potions");
			        AddLabel(577, 414, 2550, "10 Uses Max");
			        AddItem(422, 434, 3853);
			        AddLabel(456, 434, 149, "Explosion Potions");
			        AddLabel(577, 434, 2550, "5 Uses Max");
			        AddItem(422, 454, 3850);
			        AddLabel(455, 454, 149, "Poison Potions");
			        AddLabel(577, 454, 2550, "3 Uses Max");
			        AddItem(422, 474, 3846);
			        AddLabel(456, 474, 149, "Resist Potions");
			        AddLabel(577, 474, 2401, "Disabled");
			        AddButton(382, 338, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 338, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 358, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 358, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 378, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 378, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 398, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 398, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 418, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 418, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 437, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 437, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 457, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 457, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(382, 478, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 478, 2224, 248, 0, GumpButtonType.Reply, 0);
			        AddItem(414, 316, 2480);
			        AddLabel(456, 314, 149, "Trapped Pouches");
			        AddLabel(577, 314, 2550, "25 Uses Max");
			        AddButton(382, 318, 2223, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(405, 318, 2224, 248, 0, GumpButtonType.Reply, 0);

                    //Controls
                    AddButton(23, 483, 4014, 4016, 0, GumpButtonType.Reply, 0);
			        AddLabel(57, 483, 0, "Return");
			        AddButton(321, 499, 2151, 2151, 0, GumpButtonType.Reply, 0);
			        AddLabel(357, 502, 63, "Save Changes");
			        AddButton(468, 498, 2472, 2473, 0, GumpButtonType.Reply, 0);
			        AddLabel(504, 502, 1256, "Cancel Changes");
			        AddButton(296, 467, 9903, 248, 0, GumpButtonType.Reply, 0);
			        AddButton(171, 467, 9909, 248, 0, GumpButtonType.Reply, 0);
			        AddLabel(200, 468, 2599, "More Settings");
                break;

                case ArenaPageType.TournamentRounds:
                break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (m_ArenaPage)
            {
                #region Available Matches

                case ArenaPageType.AvailableMatches:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                        break;

                        //Page: Available Matches
                        case 2:
                        break;

                        //Page: Scheduled Events
                        case 3:
                        break;

                        //Page: Records and Teams
                        case 4:
                        break;

                        //-----

                        //Previous Page
                        case 5:
                        break;

                        //Next Page
                        case 6:
                        break;

                        //Create Match
                        case 7:
                        break;
                    }

                    if (info.ButtonID >= 10 && info.ButtonID < 20)
                    {
                    }

                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                    }

                    if (info.ButtonID >= 30 && info.ButtonID < 40)
                    {
                    }
                break;

                #endregion

                #region Create Match

                case ArenaPageType.CreateMatch:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                        break;

                        //Page: Available Matches
                        case 2:
                        break;

                        //Page: Scheduled Events
                        case 3:
                        break;

                        //Page: Records and Teams
                        case 4:
                        break;

                        //Previous Page
                        case 5:
                        break;

                        //-----

                        //Cancel
                        case 6:
                        break;

                        //Save Preset
                        case 7:
                        break;

                        //Create Match
                        case 8:
                        break;

                        //Previous Preset 
                        case 9:
                        break;

                        //Next Preset 
                        case 10:
                        break;

                        //Previous Settings
                        case 11:
                        break;

                        //Next Settings
                        case 12:
                        break;
                    }

                    //Settings Page
                    switch (m_SettingsPage)
                    {
                        //Page 1
                        case 0:
                            if (info.ButtonID >= 100)
                            {
                                switch (info.ButtonID)
                                {
                                    #region Basic Rules

                                    //Match Type
                                    case 100: break;
                                    case 101: break;

                                    //Round Duration
                                    case 102: break;
                                    case 103: break;

                                    //Sudden Death Mode
                                    case 104: break;
                                    case 105: break;

                                    //Equipment Restriction
                                    case 106: break;
                                    case 107: break;

                                    //Weapons Poisoned At Start
                                    case 108: break;
                                    case 109: break;

                                    //Item Durability Damage
                                    case 110: break;
                                    case 111: break;

                                    //Resource Consumption Mode
                                    case 112: break;
                                    case 113: break;

                                    #endregion

                                    #region Spell Restrictions

                                    //Poison
                                    case 200: break;
                                    case 201: break;

                                    //Poison Field
                                    case 202: break;
                                    case 203: break;

                                    //Paralyze
                                    case 204: break;
                                    case 205: break;

                                    //Paralyze Field
                                    case 206: break;
                                    case 207: break;

                                    //AoE Spells
                                    case 208: break;
                                    case 209: break;

                                    #endregion

                                    #region Item Restrictions

                                    //Trapped Containers
                                    case 300: break;
                                    case 301: break;

                                    //Heal Potions
                                    case 302: break;
                                    case 303: break;

                                    //Cure Potions
                                    case 304: break;
                                    case 305: break;

                                    //Refresh Potions
                                    case 306: break;
                                    case 307: break;

                                    //Strength Potions
                                    case 308: break;
                                    case 309: break;

                                    //Agility Potions
                                    case 310: break;
                                    case 311: break;

                                    //Explosion Potions
                                    case 312: break;
                                    case 313: break;

                                    //Poison Potions
                                    case 314: break;
                                    case 315: break;

                                    //Magic Resist Potions
                                    case 316: break;
                                    case 317: break;

                                    #endregion
                                }
                            }
                        break;

                        //Page 2
                        case 1:
                            if (info.ButtonID >= 100)
                            {
                            }
                        break;
                    }
                break;

                #endregion

                #region Match Info

                case ArenaPageType.MatchInfo:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            break;

                        //Page: Available Matches
                        case 2:
                            break;

                        //Page: Scheduled Events
                        case 3:
                            break;

                        //Page: Records and Teams
                        case 4:
                            break;

                        //Previous Page
                        case 5:
                        break;

                        //-----

                        //Return
                        case 6:
                        break;

                        //Previous Settings
                        case 7:
                        break;

                        //Next Settings
                        case 8:
                        break;

                        //Select Team 1
                        case 9:
                        break;

                        //Select Team 2
                        case 10:
                        break;

                        //Start Match
                        case 11:
                        break;

                        //Cancel Match
                        case 12:
                        break;
                    }

                //Settings Page
                switch (m_SettingsPage)
                {
                    //Page 1
                    case 0:
                        if (info.ButtonID >= 100)
                        {
                            switch (info.ButtonID)
                            {
                                #region Basic Rules

                                //Match Type
                                case 100: break;
                                case 101: break;

                                //Round Duration
                                case 102: break;
                                case 103: break;

                                //Sudden Death Mode
                                case 104: break;
                                case 105: break;

                                //Equipment Restriction
                                case 106: break;
                                case 107: break;

                                //Weapons Poisoned At Start
                                case 108: break;
                                case 109: break;

                                //Item Durability Damage
                                case 110: break;
                                case 111: break;

                                //Resource Consumption Mode
                                case 112: break;
                                case 113: break;

                                #endregion

                                #region Spell Restrictions

                                //Poison
                                case 200: break;
                                case 201: break;

                                //Poison Field
                                case 202: break;
                                case 203: break;

                                //Paralyze
                                case 204: break;
                                case 205: break;

                                //Paralyze Field
                                case 206: break;
                                case 207: break;

                                //AoE Spells
                                case 208: break;
                                case 209: break;

                                #endregion

                                #region Item Restrictions

                                //Trapped Containers
                                case 300: break;
                                case 301: break;

                                //Heal Potions
                                case 302: break;
                                case 303: break;

                                //Cure Potions
                                case 304: break;
                                case 305: break;

                                //Refresh Potions
                                case 306: break;
                                case 307: break;

                                //Strength Potions
                                case 308: break;
                                case 309: break;

                                //Agility Potions
                                case 310: break;
                                case 311: break;

                                //Explosion Potions
                                case 312: break;
                                case 313: break;

                                //Poison Potions
                                case 314: break;
                                case 315: break;

                                //Magic Resist Potions
                                case 316: break;
                                case 317: break;

                                #endregion
                            }
                        }
                    break;

                    //Page 2
                    case 1:
                        if (info.ButtonID >= 100)
                        {
                        }
                    break;
                }
                break;

                #endregion
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ArenaGump));
                m_Player.SendGump(new ArenaGump(m_Player));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }
}