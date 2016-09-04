using System;
using System.Collections;
using Server;
using Server.Gumps;

namespace Server
{
    public class GumpCollections
    {
        public static GumpCollection GetGumpCollection(string gumpName, int gumpItemId)
        {
            GumpCollection gumpCollection = new GumpCollection();

            #region GumpName

            switch (gumpName)
            {
                #region Ship Upgrades

                case "PirateShipThemeUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(49, 33, 2542, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(44, 41, 5184, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(62, 42, 5742, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "DarkGreyShipPaintUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(25, 28, 16052, 1105, GumpCollectionObject.ObjectType.Item));
                break;

                case "BloodstoneShipCannonMetalUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(50, 38, 7141, 2117, GumpCollectionObject.ObjectType.Item));
                break;

                case "HunterShipOutfittingUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(56, 28, 2942, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(39, 22, 2943, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(40, 27, 5365, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(56, 33, 5355, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(58, 28, 4183, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "CorsairsShipBannerUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(32, 22, 10930, 2401, GumpCollectionObject.ObjectType.Item));
                break;

                case "BarrelOfLimesShipCharmUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(47, 35, 18251, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "ExpediteRepairsShipMinorAbilityUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(57, 38, 7866, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "SmokescreenShipMajorAbilityUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(35, -18, 14123, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "HellfireShipEpicAbilityUpgrade":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(35, -56, 14095, 2564, GumpCollectionObject.ObjectType.Item));
                break;

                #endregion

                #region Customizations

                case "BenchPlayerCustomization":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(18, 58, 8454, 2500, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(43, 59, 8455, 1025, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(68, 58, 8454, 2587, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(18, 13, 8455, 2660, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(44, 12, 8454, 2550, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(66, 13, 8455, 2515, GumpCollectionObject.ObjectType.Item));
                break;

                case "CarnageCustomization":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(45, 42, 7573, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(14, 48, 7430, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(65, 43, 7437, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(58, 26, 7435, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(34, 34, 7392, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(62, 47, 7396, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(34, 58, 7415, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "ShapeShifterCustomization":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(36, 57, 8470, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(65, 18, 8504, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(17, 21, 8468, 0, GumpCollectionObject.ObjectType.Item));               
                break;

                case "HoarderCustomization":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(36, 20, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(35, 13, 3647, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(35, 11, 2473, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(53, 36, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(52, 29, 3647, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(51, 25, 2473, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(24, 33, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(23, 26, 3647, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(25, 21, 2473, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(41, 50, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(40, 43, 3647, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(41, 36, 2473, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(13, 44, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(12, 37, 3647, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(14, 32, 2473, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(31, 62, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(30, 55, 3647, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(31, 47, 2473, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(56, 64, 3645, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(56, 56, 3646, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(55, 50, 3710, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "VenomousCustomization":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(0, 55, 9592, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(52, 28, 9800, 0, GumpCollectionObject.ObjectType.Image));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(-5, 58, 7570, 63, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(48, 37, 3850, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(57, 37, 3850, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(65, 37, 3850, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(52, 42, 3850, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(61, 43, 3850, 0, GumpCollectionObject.ObjectType.Item));
                break;

                case "SomethingFishyCustomization":
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(38, 48, 6044, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(59, 33, 6047, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(16, 36, 6051, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(37, 32, 6063, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(19, 67, 6050, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(-3, 57, 6064, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(62, 67, 6046, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(40, 84, 6065, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(80, 53, 6053, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(63, 71, 4604, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(41, 52, 3520, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(90, 72, 2468, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(57, 78, 7863, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(37, 87, 3656, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(8, 73, 2448, 0, GumpCollectionObject.ObjectType.Item));
			   
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(6, 38, 3254, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(68, 26, 3254, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(38, 13, 3255, 0, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(19, 73, 15102, 2498, GumpCollectionObject.ObjectType.Item));
                break;

                #endregion

                #region Achievement Categories

                case "BattleAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(59, 30, 18210, 2500, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(61, 29, 5049, 2500, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(54, 18, 5138, 2500, GumpCollectionObject.ObjectType.Item));
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(44, 36, 7028, 2500, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "SeafaringAchievementCategory":                    
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(44, 43, 5363, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(62, 53, 5365, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(63, 30, 5370, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "AnimalHandlingAchievementCategory":      
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(54, 43, 2476, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(51, 34, 3191, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(55, 32, 3191, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(54, 33, 3713, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "CraftingAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(25, 34, 4142, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(36, 34, 4150, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(57, 42, 2920, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(45, 32, 2921, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(75, 70, 4148, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(44, 38, 4189, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(57, 54, 4179, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(38, 46, 4139, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(56, 36, 2581, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(38, 22, 2503, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(46, 15, 4172, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "AdventuringAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(46, 11, 3226, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(61, 36, 4967, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(53, 47, 4970, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(68, 61, 2648, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(38, 50, 5356, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(44, 57, 3922, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(5, 42, 3898, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "LuxuryAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(36, 52, 2448, 2425, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(58, 14, 16508, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(40, 47, 2459, 2562, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(31, 47, 2459, 2600, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(35, 51, 2459, 2606, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "HarvestingAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(68, 33, 3346, 2208, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(47, 22, 3670, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(77, 18, 3351, 2208, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(26, 68, 3352, 2208, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(43, 54, 3344, 2208, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(59, 51, 7137, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(68, 46, 3908, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(69, 60, 2482, 2500, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "SkillMasteryAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(59, 34, 2942, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(34, 20, 2943, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(54, 25, 2507, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(53, 41, 4030, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(66, 20, 7716, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(34, 12, 7717, 2652, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(56, 31, 4031, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "SlayingAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(33, 25, 7433, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(38, 50, 4655, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(51, 14, 7438, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(75, 28, 7419, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(98, 27, 7418, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(36, 48, 7782, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(73, 62, 7430, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(53, 46, 3910, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "CompetitionAchievementCategory":                
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(48, 30, 16434, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(59, 16, 16433, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(49, 25, 4006, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(41, 38, 4008, 2500, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(51, 24, 4008, 1107, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "VirtueAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(27, 9, 2, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(47, 11, 3, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(55, 72, 3618, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(60, 53, 3619, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                case "ViceAchievementCategory":                    
                    gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(64, 58, 6872, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(43, 38, 6873, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(27, 29, 6874, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(62, 19, 6875, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(88, 39, 6876, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(106, 33, 6877, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(51, 71, 6880, 0, GumpCollectionObject.ObjectType.Item));
			        gumpCollection.m_GumpObjects.Add(new GumpCollectionObject(20, 70, 6883, 0, GumpCollectionObject.ObjectType.Item));                    
                break;

                #endregion
            }

            #endregion

            #region GumpItemId

            switch (gumpItemId)
            {
            }

            #endregion

            return gumpCollection;
        }
    }
}
