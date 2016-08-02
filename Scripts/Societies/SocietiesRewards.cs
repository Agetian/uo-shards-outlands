using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public static class SocietiesRewards
    {
        public static Dictionary<SocietiesGroupType, List<SocietyRewardItem>> SocietyRewardList = new Dictionary<SocietiesGroupType, List<SocietyRewardItem>>()
        {              
        };

        public static void Initialize()
        {
            #region Adeventurer's Lodge

            SocietyRewardList.Add(SocietiesGroupType.AdventurersLodge,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Artificer's Enclave

            SocietyRewardList.Add(SocietiesGroupType.ArtificersEnclave,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            }); 

            #endregion

            #region Farmer's Cooperative

            SocietyRewardList.Add(SocietiesGroupType.FarmersCooperative,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Fisherman's Circle

            SocietyRewardList.Add(SocietiesGroupType.FishermansCircle,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Monster Hunter's Society

            SocietyRewardList.Add(SocietiesGroupType.MonsterHuntersSociety,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Seafarer's League

            SocietyRewardList.Add(SocietiesGroupType.SeafarersLeague,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Smithing Order

            SocietyRewardList.Add(SocietiesGroupType.SmithingOrder,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Thieves Guild

            SocietyRewardList.Add(SocietiesGroupType.ThievesGuild,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Tradesman Union

            SocietyRewardList.Add(SocietiesGroupType.TradesmanUnion,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion

            #region Zoological Foundation

            SocietyRewardList.Add(SocietiesGroupType.ZoologicalFoundation,
            new List<SocietyRewardItem>()
            {
                new SocietyRewardItem(typeof(SkillMasteryOrb), "Skill Mastery Orb", new List<string>{"Increase Your Total Skill Cap by 1.", "Maximum of 720 Skill Cap."}, 100, 22336, 2966, 52, 43),
            });

            #endregion
        }
    }        

    public class SocietyRewardItem
    {
        public Type ItemType;
        public string ItemName;
        public List<string> ItemDescription = new List<string>() { };
        public int ItemCost;
        public int ItemIconItemId;
        public int ItemIconHue;
        public int ItemIconOffsetX;
        public int ItemIconOffsetY;

        public SocietyRewardItem(Type itemType, string itemName, List<string> itemDescription, int itemCost, int itemIconItemId, int itemIconHue, int itemIconOffsetX, int itemIconOffsetY)
        {
            ItemType = itemType;
            ItemName = itemName;
            ItemDescription = itemDescription;
            ItemCost = itemCost;
            ItemIconItemId = itemIconItemId;
            ItemIconHue = itemIconHue;
            ItemIconOffsetX = itemIconOffsetY;
            ItemIconOffsetY = itemIconOffsetY;
        }
    }
}
