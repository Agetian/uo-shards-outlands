using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public enum AchievementCategory
    {
        Battle,
        Seafaring,
        AnimalHandling,
        Crafting,

        Adventuring,
        Luxury,
        Harvesting,
        SkillMastery,

        Slaying,
        Competition,
        Virtue,
        Vice,
    }

    public enum Achievement
    {
        SinkOrSwim,
        WalkThePlank,
        HighTide,
        AFewGoodMen,
        Circumnavigate,
        Stormchaser,
        BoardToDeath,
        ForSail,
        ToTheDepths,
        GunsAndRum,
        BackFromTheBrink,
        GhostPirate,

        TEST1,
        TEST2,
        TEST3
    }

    public class Achievements
    {
        public static int AchievementTextHue = 2606;

        public static AchievementCategoryDetail GetCategoryDetail(AchievementCategory achievementCategory)
        {
            AchievementCategoryDetail detail = new AchievementCategoryDetail();

            detail.m_AchievementCategory = achievementCategory;

            #region Categories

            switch (achievementCategory)
            {
                case AchievementCategory.Adventuring:
                    detail.GumpCollectionGroupId = "AdventuringAchievementCategory";
                    detail.m_TextHue = 148;
                    detail.m_IconHue = 149;
                    detail.m_CategoryName = "Adventuring";
                break;

                case AchievementCategory.AnimalHandling:
                    detail.GumpCollectionGroupId = "AnimalHandlingAchievementCategory";
                    detail.m_TextHue = 2599;
                    detail.m_IconHue = 2208;
                    detail.m_CategoryName = "Animal Handling";
                break;

                case AchievementCategory.Battle:
                    detail.GumpCollectionGroupId = "BattleAchievementCategory";
                    detail.m_TextHue = 1256;
                    detail.m_IconHue = 1256;
                    detail.m_CategoryName = "Battle";
                break;

                case AchievementCategory.Competition:
                    detail.GumpCollectionGroupId = "CompetitionAchievementCategory";
                    detail.m_TextHue = 2963;
                    detail.m_IconHue = 2963;
                    detail.m_CategoryName = "Competition";
                break;

                case AchievementCategory.Crafting:
                    detail.GumpCollectionGroupId = "CraftingAchievementCategory";
                    detail.m_TextHue = 2499;
                    detail.m_IconHue = 2401;
                    detail.m_CategoryName = "Crafting";
                break;

                case AchievementCategory.Harvesting:
                    detail.GumpCollectionGroupId = "HarvestingAchievementCategory";
                    detail.m_TextHue = 2417;
                    detail.m_IconHue = 2417;
                    detail.m_CategoryName = "Harvesting";
                break;

                case AchievementCategory.Luxury:
                    detail.GumpCollectionGroupId = "LuxuryAchievementCategory";
                    detail.m_TextHue = 2618;
                    detail.m_IconHue = 2618;
                    detail.m_CategoryName = "Luxury";
                break;

                case AchievementCategory.Seafaring:
                    detail.GumpCollectionGroupId = "SeafaringAchievementCategory";
                    detail.m_TextHue = 2603;
                    detail.m_IconHue = 2602;
                    detail.m_CategoryName = "Seafaring";
                break;

                case AchievementCategory.SkillMastery:
                    detail.GumpCollectionGroupId = "SkillMasteryAchievementCategory";
                    detail.m_TextHue = 2550;
                    detail.m_IconHue = 2652;
                    detail.m_CategoryName = "Skill Mastery";
                break;

                case AchievementCategory.Slaying:
                    detail.GumpCollectionGroupId = "SlayingAchievementCategory";
                    detail.m_TextHue = 2116;
                    detail.m_IconHue = 2116;
                    detail.m_CategoryName = "Slaying";
                break;

                case AchievementCategory.Vice:
                    detail.GumpCollectionGroupId = "ViceAchievementCategory";
                    detail.m_TextHue = 1106;
                    detail.m_IconHue = 1106;
                    detail.m_CategoryName = "Vice";
                break;

                case AchievementCategory.Virtue:
                    detail.GumpCollectionGroupId = "VirtueAchievementCategory";
                    detail.m_TextHue = 2590;
                    detail.m_IconHue = 2589;
                    detail.m_CategoryName = "Virtue";
                break;
            }

            #endregion

            return detail;
        }

        public static List<Achievement> GetCategoryAchievements(AchievementCategory category)
        {
            List<Achievement> achievementsList = new List<Achievement>();

            #region Achievement Lists

            switch (category)
            {
                case AchievementCategory.Seafaring:
                    achievementsList.Add(Achievement.SinkOrSwim);
                    achievementsList.Add(Achievement.WalkThePlank);
                    achievementsList.Add(Achievement.HighTide);
                    achievementsList.Add(Achievement.AFewGoodMen);
                    achievementsList.Add(Achievement.Circumnavigate);
                    achievementsList.Add(Achievement.Stormchaser);
                    achievementsList.Add(Achievement.BoardToDeath);
                    achievementsList.Add(Achievement.ForSail);
                    achievementsList.Add(Achievement.ToTheDepths);
                    achievementsList.Add(Achievement.GunsAndRum);
                    achievementsList.Add(Achievement.BackFromTheBrink);
                    achievementsList.Add(Achievement.GhostPirate);

                    achievementsList.Add(Achievement.TEST1);
                    achievementsList.Add(Achievement.TEST2);
                    achievementsList.Add(Achievement.TEST3);
                break;
            }

            #endregion

            return achievementsList;
        }

        public static AchievementDetail GetAchievementDetail(Achievement achievement)
        {
            AchievementDetail detail = new AchievementDetail();

            detail.m_Achievement = achievement;

            switch (achievement)
            {
                #region Category: Seafaring

                case Achievement.SinkOrSwim:
                    detail.m_Category = AchievementCategory.Seafaring;

                    detail.m_Stages = new Achievement[] { Achievement.SinkOrSwim, Achievement.WalkThePlank, Achievement.HighTide };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "Sink or Swim";
                    detail.m_Description = new string[] { "Kill 1,000 enemy ship combatants" };

                    detail.m_RewardDescription = new string[] { "Liquor" };

                    detail.GumpCollectionGroupId = "";
                    detail.GumpCollectionItemId = 0;

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 48;
                    detail.m_RewardItemOffsetY = 34;
                break;

                case Achievement.WalkThePlank:
                    detail.m_Category = AchievementCategory.Seafaring;

                    detail.m_Stages = new Achievement[] { Achievement.SinkOrSwim, Achievement.WalkThePlank, Achievement.HighTide };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "Walk the Plank";
                    detail.m_Description = new string[] { "Kill 5,000 enemy ship combatants" };

                    detail.m_RewardDescription = new string[] { "Driftwood" };

                    detail.GumpCollectionGroupId = "";
                    detail.GumpCollectionItemId = 0;

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 48;
                    detail.m_RewardItemOffsetY = 34;
                break;

                case Achievement.HighTide:
                    detail.m_Category = AchievementCategory.Seafaring;

                    detail.m_Stages = new Achievement[] { Achievement.SinkOrSwim, Achievement.WalkThePlank, Achievement.HighTide };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "High Tide";
                    detail.m_Description = new string[] { "Kill 25,000 enemy ship combatants" };

                    detail.m_RewardDescription = new string[] { "Driftwood" };

                    detail.GumpCollectionGroupId = "";
                    detail.GumpCollectionItemId = 0;

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 48;
                    detail.m_RewardItemOffsetY = 34;
                break;                

                #endregion
            }

            //Determine Achievement Stage
            if (detail.m_Stages == null)
                detail.m_Stage = 1;

            else if (detail.m_Stages.Length == 0)
                detail.m_Stage = 1;

            else
            {
                for (int a = 0; a < detail.m_Stages.Length; a++)
                {
                    if (detail.m_Stages[a] == detail.m_Achievement)
                    {
                        detail.m_Stage = a + 1;
                        break;
                    }
                }
            }

            return detail;
        }

        public static void ClaimAchievement(PlayerMobile player, Achievement achievement)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);

            if (entry == null || detail == null)
                return;

            int rewardTextHue = AchievementTextHue;

            bool claimed = true;

            Item item = null;

            switch (achievement)
            {
                #region Category: Seafaring

                case Achievement.SinkOrSwim:
                    if (TryAcceptReward(player, entry, 1, 1))
                    {
                        item = new CyprusTreeSeed();

                        item.ItemGroup = ItemGroupType.Achievement;

                        player.Backpack.AddItem(item);
                        player.SendMessage(rewardTextHue, "You receive a Cyprus Tree seed.");                        
                    }
                break;

                case Achievement.WalkThePlank:
                break;

                case Achievement.HighTide:
                break;

                case Achievement.AFewGoodMen:
                break;

                case Achievement.Circumnavigate:
                break;

                case Achievement.Stormchaser:
                break;

                case Achievement.BoardToDeath:
                break;

                case Achievement.ForSail:
                break;

                case Achievement.ToTheDepths:
                break;

                case Achievement.GunsAndRum:
                break;

                case Achievement.BackFromTheBrink:
                break;

                case Achievement.GhostPirate:
                break;

                #endregion
            }            
        }

        public static bool TryAcceptReward(PlayerMobile player, AchievementEntry entry, int itemCount, int totalWeight)
        {
            if (player == null) return false;
            if (player.Backpack == null) return false;
            if (entry == null) return false;

            if (player.Backpack.TotalItems + itemCount > player.Backpack.MaxItems)
            {
                player.SendMessage("Your backpack would not have enough space available to receive that reward. Remove some items and try again.");
                return false;
            }

            if (player.Backpack.TotalWeight + totalWeight > player.Backpack.MaxWeight)
            {
                player.SendMessage("Your backpack is too heavy to receive that reward. Remove some weight and try again.");
                return false;
            }

            entry.m_Claimed = true;

            return true;
        }

        public static int GetUnclaimedAchievementRewards(PlayerMobile player)
        {
            if (player == null)
                return 0;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            int unclaimedCount = 0;

            foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
            {
                if (entry == null)
                    continue;

                if (!entry.m_Claimed)
                    unclaimedCount++;
            }

            return unclaimedCount;
        }

        public static int GetTotalAchievementsCount()
        {
            int achievementsTotal = Enum.GetNames(typeof(Achievement)).Length;

            return achievementsTotal;
        }

        public static int GetCompletedAchievementCount(PlayerMobile player)
        {
            if (player == null)
                return 0;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            int completedAchievements = 0;

            foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
            {
                if (entry == null) 
                    continue;

                if (entry.m_Completed)
                    completedAchievements++;
            }

            return completedAchievements;
        }

        public static int GetProgressBarBackground(double completionPercentage)
        {
            int progressItemId = 2053;

            if (completionPercentage >= .33)
                progressItemId = 2057;

            if (completionPercentage >= .66)
                progressItemId = 2054;

            if (completionPercentage >= 1.0)
                progressItemId = 2056;

            return progressItemId;
        }

        public static int GetCompletedCategoryAchievements(PlayerMobile player, AchievementCategory category)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            int completionCount = 0;

            List<Achievement> categoryAchievements = GetCategoryAchievements(category);

            foreach (Achievement achievement in categoryAchievements)
            {
                foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
                {
                    if (entry == null) continue;
                    if (entry.m_Achievement == achievement && entry.m_Completed)
                    {
                        completionCount++;
                        break;
                    }
                }
            }

            return completionCount;
        }        

        public static AchievementEntry GetAchievementEntry(PlayerMobile player, Achievement achievement)
        {
            if (player == null)
                return null;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
            {
                if (entry.m_Achievement == achievement)
                    return entry;
            }

            AchievementEntry newEntry = new AchievementEntry(achievement, 0);
            AchievementDetail entryDetail = GetAchievementDetail(achievement);
            
            if (entryDetail.m_Stage > 1)
            {
                bool allCompleted = true;

                for (int a = 0; a < entryDetail.m_Stages.Length; a++)
                {
                    if (entryDetail.m_Stages[a] == achievement)
                        break;

                    Achievement priorAchievement = entryDetail.m_Stages[a];
                    AchievementEntry priorEntry = Achievements.GetAchievementEntry(player, priorAchievement);

                    if (priorEntry == null)
                        continue;

                    if (!priorEntry.m_Completed)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                if (allCompleted)
                    newEntry.m_Unlocked = true;
            }

            else
                newEntry.m_Unlocked = true;

            player.m_AchievementAccountEntry.m_Achievements.Add(newEntry);

            return newEntry;
        }

        public static void UpdateAchievement(PlayerMobile player, Achievement achievement, int progressAmount)
        {
            if (player == null)
                return;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);

            if (entry == null) return;
            if (detail == null) return;           

            //Multi-Staged Achievement: Get Current Stage
            if (detail.m_Stages.Length > 0)
            {
                for (int a = 0; a < detail.m_Stages.Length; a++)
                {
                    Achievement achievementStage = detail.m_Stages[a];
                    
                    AchievementEntry stageEntry = GetAchievementEntry(player, achievementStage);
                    AchievementDetail stageDetail = GetAchievementDetail(achievementStage);

                    if (stageEntry == null) continue;
                    if (stageDetail == null) continue;

                    if (stageEntry.m_Completed || !stageDetail.Enabled)
                        continue;

                    entry = stageEntry;
                    detail = stageDetail;

                    break;
                }
            }

            if (entry.m_Completed || !detail.Enabled)
                return;

            entry.m_Progress += progressAmount;           

            if (entry.m_Progress >= detail.m_ProgressNeeded)
            {
                entry.m_Progress = detail.m_ProgressNeeded;
                CompleteAchievement(player, achievement);
            }            
        }

        public static void CompleteAchievement(PlayerMobile player, Achievement achievement)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);

            if (entry == null) return;
            if (detail == null) return; 

            entry.m_Progress = detail.m_ProgressNeeded;
            entry.m_Completed = true;

            int playerAchievementTextHue = AchievementTextHue;
            int guildAchievementTextHue = 63;
            int nonGuildAchievementTextHue = 2550;

            player.SendMessage(playerAchievementTextHue, "You have completed the achievement: " + detail.m_DisplayName + ".");

            //Play Audio

            //Create Popup
                                    
            foreach (NetState netstate in NetState.Instances)
            {
                PlayerMobile pm_Mobile = netstate.Mobile as PlayerMobile;

                if (pm_Mobile == null) continue;
                if (pm_Mobile == player) continue;

                AchievementsPersistance.CheckAndCreateAchievementAccountEntry(pm_Mobile);
                
                if (player.Guild != null && player.Guild == pm_Mobile.Guild)
                {
                    if (player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers && pm_Mobile.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements)
                        pm_Mobile.SendMessage(guildAchievementTextHue, player.RawName + " has completed the achievement: " + detail.m_DisplayName + ".");
                }

                else
                {
                    if (player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers && pm_Mobile.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements)
                        pm_Mobile.SendMessage(nonGuildAchievementTextHue, player.RawName + " has completed the achievement: " + detail.m_DisplayName + ".");
                }
            }   

            //Unlock Next Achievement if Multiple Stages Exist
            int achievementIndex = detail.m_Stage - 1;
            int stageCount = 1;

            if (detail.m_Stages != null)            
                stageCount = detail.m_Stages.Length;

            if (detail.m_Stage < stageCount)
            {
                if (achievementIndex + 1 < detail.m_Stages.Length)
                {
                    Achievement nextAchievement = detail.m_Stages[achievementIndex + 1];
                    AchievementEntry nextAchievementEntry = GetAchievementEntry(player, achievement);

                    if (nextAchievementEntry != null)
                        nextAchievementEntry.m_Unlocked = true;
                }
            }
        }               
    }  
  
    public class AchievementCategoryDetail
    {
        public AchievementCategory m_AchievementCategory;
        public string m_CategoryName = "";
        public int m_TextHue = 2499;
        public int m_IconHue = 0;
        public string GumpCollectionGroupId = "";
        public int GumpCollectionItemId = -1;

        public AchievementCategoryDetail()
        {
        }
    }

    public class AchievementDetail
    {
        public bool Enabled = true;

        public Achievement m_Achievement;
        public AchievementCategory m_Category = AchievementCategory.Adventuring;

        public int m_Stage = 1;
        public Achievement[] m_Stages = new Achievement[] { };

        public int m_ProgressNeeded = 1;

        public string m_DisplayName = "Achievement Name";
        public string[] m_Description = new string[] { "Achievement Description" };

        public string[] m_RewardDescription = new string[] { "Achievement Description" };

        public string GumpCollectionGroupId = "";
        public int GumpCollectionItemId = -1;

        public int m_RewardItemID = 0;
        public int m_RewardItemHue = 0;
        public int m_RewardItemOffsetX = 0;
        public int m_RewardItemOffsetY = 0;

        public AchievementDetail()
        {            
        }
    }

    public class AchievementEntry
    {
        public Achievement m_Achievement;
        public int m_Progress = 0;

        public bool m_Unlocked = false;
        public bool m_Completed = false;
        public bool m_Claimed = false;

        public AchievementEntry(Achievement achievement, int progress)
        {
            m_Achievement = achievement;
            m_Progress = progress;
        }
    }
}