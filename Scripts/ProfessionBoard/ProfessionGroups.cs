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
    public enum ProfessionGroupType
    {
        FishermansCircle,
        SmithingOrder,
        TradesmanUnion,
        ArtificersEnclave,
        SeafarersLeague,
        AdventurersLodge,
        ZoologicalFoundation,
        ThievesGuild,
        FarmersCooperative,
        MonsterHuntersSociety
    }

    public enum ProfessionGroupPageDisplayType
    {
        Jobs,
        SpendPoints,
        ServerRankings
    }

    public static class ProfessionGroups
    {
        public static void Initialize()
        {
        }

        public static string GetProfessionGroupName(ProfessionGroupType professionGroupType)
        {
            switch (professionGroupType)
            {
                case ProfessionGroupType.AdventurersLodge: return "Adventurer's Lodge"; break;
                case ProfessionGroupType.ArtificersEnclave: return "Artificer's Enclave"; break;
                case ProfessionGroupType.FarmersCooperative: return "Farmer's Cooperative"; break;
                case ProfessionGroupType.FishermansCircle: return "Fisherman's Circle"; break;
                case ProfessionGroupType.MonsterHuntersSociety: return "Monster Hunter's Society"; break;
                case ProfessionGroupType.SeafarersLeague: return "Seafarer's League"; break;
                case ProfessionGroupType.SmithingOrder: return "Smithing Order"; break;
                case ProfessionGroupType.ThievesGuild: return "Thieves Guild"; break;
                case ProfessionGroupType.TradesmanUnion: return "Tradesman Union"; break;
                case ProfessionGroupType.ZoologicalFoundation: return "Zoological Foundation"; break;  
            }

            return "";
        }

        public static int GetProfessionGroupTextHue(ProfessionGroupType professionGroupType)
        {
            switch (professionGroupType)
            {
                case ProfessionGroupType.AdventurersLodge: return 152; break;
                case ProfessionGroupType.ArtificersEnclave: return 2606; break;
                case ProfessionGroupType.FarmersCooperative: return 2599; break;
                case ProfessionGroupType.FishermansCircle: return 2590; break;
                case ProfessionGroupType.MonsterHuntersSociety: return 2116; break;
                case ProfessionGroupType.SeafarersLeague: return 2603; break;
                case ProfessionGroupType.SmithingOrder: return 2401; break;
                case ProfessionGroupType.ThievesGuild: return 2036; break;
                case ProfessionGroupType.TradesmanUnion: return 2417; break;
                case ProfessionGroupType.ZoologicalFoundation: return 63; break;
            }

            return 2499;
        }
    } 
}
