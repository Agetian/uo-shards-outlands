using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom;

namespace Server.Items
{
    public class PlantGump : Gump
    {
        public Mobile m_Player;
        public PlantBowl m_PlantBowl;

        public int WhiteTextHue = 2499;

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public int closeGumpSound = 0x058;

        public PlantGump(Mobile from, PlantBowl plantBowl): base(10, 10)
        {
            if (from == null || plantBowl == null) return;
            if (from.Deleted || !from.Alive || plantBowl.Deleted) return;

            m_Player = from;
            m_PlantBowl = plantBowl;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            #region Images

            AddImage(183, 263, 103, 2206);
            AddImage(136, 263, 103);
            AddImage(13, 263, 103, 2206);
            AddImage(13, 341, 103, 2206);
            AddImage(138, 341, 103);
            AddImage(78, 109, 103);
            AddImage(13, 109, 103, 2206);
            AddImage(146, 14, 103, 2206);
            AddImage(13, 13, 103, 2206);
            AddImage(183, 109, 103, 2206);
            AddImage(183, 14, 103, 2206);
            AddImage(136, 195, 103);
            AddImage(13, 195, 103, 2206);
            AddImage(183, 341, 103, 2206);
            AddImage(183, 195, 103, 2206);
            AddImage(137, 392, 103, 2206);
            AddImage(13, 392, 103, 2206);
            AddImage(183, 392, 103, 2206);
            AddImage(25, 27, 3604, 2052);
            AddImage(142, 27, 3604, 2052);
            AddImage(187, 27, 3604, 2052);
            AddImage(25, 149, 3604, 2052);
            AddImage(142, 147, 3604, 2052);
            AddImage(187, 149, 3604, 2052);
            AddImage(25, 215, 3604, 2052);
            AddImage(142, 215, 3604, 2052);
            AddImage(187, 215, 3604, 2052);
            AddImage(26, 337, 3604, 2052);
            AddImage(143, 337, 3604, 2052);
            AddImage(187, 337, 3604, 2052);
            AddImage(26, 355, 3604, 2052);
            AddImage(143, 355, 3604, 2052);
            AddImage(187, 355, 3604, 2052);
            AddImage(40, 9, 1142, 2206); 

            #endregion

            string plantName = "";
                      
            double growthPercent = 0;
            double waterPercent = 0;
            double soilPercent = 0;
            double heatPercent = 0;

            double targetWaterPercent = 0;
            double targetSoilPercent = 0;
            double targetHeatPercent = 0;

            string dailyGrowthText = "";
            string growthEfficiencyText = "";

            string growthText = "";
            string waterText = "";
            string soilText = "";
            string heatText = "";

            int plantItemId = 6818;
            int plantItemHue = 0;
            int plantItemOffsetX = -1;
            int plantItemOffsetY = 18;

            AddButton(10, 8, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(5, -2, 149, "Guide");

            int seedImageStartX = 100;
            int seedImageStartY = 25;

            int plantImageStartX = 100;
            int plantImageStartY = 208;

            if (plantBowl.SeedType != null)
            {
                Plants.SeedDetail seedDetail = Plants.GetSeedDetail(plantBowl.SeedType);

                AddLabel(Utility.CenteredTextOffset(175, seedDetail.DisplayName), 42, seedDetail.DisplayNameHue, seedDetail.DisplayName);

                //Seed Image
                if (seedDetail.GumpCollectionId != "")                
                    AddGumpCollection(GumpCollections.GetGumpCollection(seedDetail.GumpCollectionId, -1), seedImageStartX, seedImageStartY);
                  
                else
                    AddItem(seedImageStartX + seedDetail.IconOffsetX, seedImageStartY + seedDetail.IconOffsetY, seedDetail.IconItemID, seedDetail.IconHue);

                targetWaterPercent = seedDetail.WaterTarget / PlantPersistance.MaxWater;
                targetSoilPercent = seedDetail.SoilTarget / PlantPersistance.MaxSoilQuality;
                targetHeatPercent = seedDetail.HeatTarget / PlantPersistance.MaxHeat;

                plantBowl.DetermineHeatLevel();

                if (plantBowl.PlantType != null && plantBowl.ReadyForHarvest)
                {
                    Plants.PlantDetail plantDetail = Plants.GetPlantDetail(plantBowl.PlantType);

                    AddLabel(Utility.CenteredTextOffset(175, plantDetail.DisplayName), 12, plantDetail.DisplayNameHue, plantDetail.DisplayName);

                    growthText = plantBowl.GrowthValue.ToString();
                    growthPercent = 100;

                    //Plant Image
                    if (plantDetail.GumpCollectionId != "")                    
                        AddGumpCollection(GumpCollections.GetGumpCollection(plantDetail.GumpCollectionId, -1), plantImageStartX, plantImageStartY);
                    
                    else
                        AddItem(plantImageStartX + plantDetail.IconOffsetX, plantImageStartY + plantDetail.IconOffsetY, plantDetail.IconItemID, plantDetail.IconHue);

                    dailyGrowthText = "Ready to Harvest";
                }

                else
                {
                    double progressPercent = plantBowl.GrowthValue / seedDetail.GrowthTarget;
                    double dailyGrowthValue = plantBowl.GetDailyGrowthScalar() * PlantPersistance.GrowthPerDay;

                    dailyGrowthText = Utility.CreateDecimalString(dailyGrowthValue, 1);

                    if (dailyGrowthText.IndexOf(".") == -1)
                        dailyGrowthText = dailyGrowthText + ".0";

                    dailyGrowthText += " Daily Growth";

                    growthText = Utility.CreateDecimalString(plantBowl.GrowthValue, 1);
                    growthText = growthText + " / " + seedDetail.GrowthTarget.ToString();

                    waterText = Utility.CreateDecimalString(plantBowl.WaterValue, 0);
                    if (plantBowl.WaterValue != seedDetail.WaterTarget)
                        waterText = waterText + " (-" + Utility.CreateDecimalPercentageString(plantBowl.GetWaterPenalty(), 0) + ")";

                    soilText = Utility.CreateDecimalString(plantBowl.SoilValue, 0);
                    if (plantBowl.SoilValue != seedDetail.SoilTarget)
                        soilText = soilText + " (-" + Utility.CreateDecimalPercentageString(plantBowl.GetSoilPenalty(), 0) + ")";

                    heatText = Utility.CreateDecimalString(plantBowl.HeatValue, 0);
                    if (plantBowl.HeatValue != seedDetail.HeatTarget)
                        heatText = heatText + " (-" + Utility.CreateDecimalPercentageString(plantBowl.GetHeatPenalty(), 0) + ")";

                    growthPercent = progressPercent;
                    waterPercent = plantBowl.WaterValue / PlantPersistance.MaxWater;
                    soilPercent = plantBowl.SoilValue / PlantPersistance.MaxSoilQuality;
                    heatPercent = plantBowl.HeatValue / PlantPersistance.MaxHeat;

                    #region Plant Images

                    switch (seedDetail.PlantGroup)
                    {
                        case Plants.PlantGroupType.Crop:
                            plantName = "Unknown Crop";

                            if (progressPercent < .33)
                            {
                                plantItemId = 6818;
                                plantItemHue = 0;
                                plantItemOffsetX = 49;
                                plantItemOffsetY = 34;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 731;
                                plantItemHue = 2208;
                                plantItemOffsetX = 57;
                                plantItemOffsetY = 17;
                            }

                            else
                            {
                                plantItemId = 3155;
                                plantItemHue = 2208;
                                plantItemOffsetX = 55;
                                plantItemOffsetY = 15;
                            }
                        break;

                        case Plants.PlantGroupType.Fern:
                            plantName = "Unknown Fern";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3169;
                                plantItemHue = 0;
                                plantItemOffsetX = 55;
                                plantItemOffsetY = 30;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3267;
                                plantItemHue = 0;
                                plantItemOffsetX = 51;
                                plantItemOffsetY = 7;
                            }

                            else
                            {
                                plantItemId = 3269;
                                plantItemHue = 0;
                                plantItemOffsetX = 54;
                                plantItemOffsetY = 8;
                            }
                        break;

                        case Plants.PlantGroupType.Flower:
                            plantName = "Unknown Flower";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3176;
                                plantItemHue = 0;
                                plantItemOffsetX = 49;
                                plantItemOffsetY = 21;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3177;
                                plantItemHue = 0;
                                plantItemOffsetX = 49;
                                plantItemOffsetY =22;
                            }

                            else
                            {
                                plantItemId = 3183;
                                plantItemHue = 0;
                                plantItemOffsetX = 49;
                                plantItemOffsetY = 20;
                            }
                        break;

                        case Plants.PlantGroupType.Grass:
                            plantName = "Unknown Grass";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3253;
                                plantItemHue = 0;
                                plantItemOffsetX = 53;
                                plantItemOffsetY = 24;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3254;
                                plantItemHue = 0;
                                plantItemOffsetX = 51;
                                plantItemOffsetY = 24;
                            }

                            else
                            {
                                plantItemId = 3219;
                                plantItemHue = 0;
                                plantItemOffsetX = 53;
                                plantItemOffsetY = 22;
                            }
                        break;

                        case Plants.PlantGroupType.Tree:
                            plantName = "Unknown Tree";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3198;
                                plantItemHue = 0;
                                plantItemOffsetX = 51;
                                plantItemOffsetY = 7;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3306;
                                plantItemHue = 0;
                                plantItemOffsetX = 58;
                                plantItemOffsetY = -13;
                            }

                            else
                            {
                                plantItemId = 3305;
                                plantItemHue = 0;
                                plantItemOffsetX = 52;
                                plantItemOffsetY = -24;
                            }
                        break;

                        case Plants.PlantGroupType.Vine:
                            plantName = "Unknown Vine";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3251;
                                plantItemHue = 0;
                                plantItemOffsetX = 68;
                                plantItemOffsetY = 17;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3166;
                                plantItemHue = 0;
                                plantItemOffsetX = 56;
                                plantItemOffsetY = 20;
                            }

                            else
                            {
                                plantItemId = 3167;
                                plantItemHue = 0;
                                plantItemOffsetX = 54;
                                plantItemOffsetY = 18;
                            }
                        break;

                    }

                    #endregion                   
                    
                    AddLabel(Utility.CenteredTextOffset(175, plantName), 12, WhiteTextHue, plantName);

                    AddItem(151, 240, 4551, 0); //Plant Bowl
                    AddItem(plantImageStartX + plantItemOffsetX, plantImageStartY + plantItemOffsetY, plantItemId, plantItemHue); //Plant Image
                }                
            }

            else
                AddLabel(Utility.CenteredTextOffset(175, "No Seed Planted"), 12, WhiteTextHue, "No Seed Planted");

            if (plantBowl.ReadyForHarvest)
                AddLabel(115, 301, 2599, "Ready to Harvest");
                
            else if (plantBowl.SeedType != null && !plantBowl.ReadyForHarvest)
            {
                double dailyGrowthScalar = plantBowl.GetDailyGrowthScalar();

                growthEfficiencyText = "Growth Efficiency: " + Utility.CreateDecimalPercentageString(dailyGrowthScalar, 0);

                AddLabel(Utility.CenteredTextOffset(175, dailyGrowthText), 281, 63, dailyGrowthText);
                AddLabel(Utility.CenteredTextOffset(175, growthEfficiencyText), 301, 2599, growthEfficiencyText);
            }

            //Growth
            AddItem(23, 314, 13238);
            AddLabel(63, 326, 267, "Growth");
            AddImage(117, 332, 2056);
            AddImageTiled(117 + Utility.ProgressBarX(growthPercent), 335, Utility.ProgressBarWidth(growthPercent), 7, 2488);
            AddLabel(230, 326, 267, growthText);

            //Water
            AddItem(19, 353, 2471);
            AddLabel(68, 352, 187, "Water");
            AddImage(117, 359, 2054);
            AddImageTiled(117 + Utility.ProgressBarX(waterPercent), 361, Utility.ProgressBarWidth(waterPercent), 7, 2488);
            AddLabel(230, 352, 187, waterText);
            if (plantBowl.SeedType != null)
                AddImage(117 + Utility.ProgressBarX(targetWaterPercent), 371, 2436, 187);

            //Soil
            AddItem(22, 380, 2323);
            AddLabel(82, 382, 542, "Soil");
            if (soilPercent > 0)
                AddImage(117, 387, 2057, 542);
            else
                AddImage(117, 387, 2057);
            AddImageTiled(117 + Utility.ProgressBarX(soilPercent), 389, Utility.ProgressBarWidth(soilPercent), 7, 2488);
            AddLabel(230, 382, 542, soilText);
            if (plantBowl.SeedType != null)
                AddImage(117 + Utility.ProgressBarX(targetSoilPercent), 399, 2436, 542);

            //Heat
            AddItem(20, 406, 2842);
            AddLabel(77, 411, 52, "Heat");
            AddImage(117, 416, 2057);
            AddImageTiled(117 + Utility.ProgressBarX(heatPercent), 418, Utility.ProgressBarWidth(heatPercent), 7, 2488);
            AddLabel(230, 411, 52, heatText);
            if (plantBowl.SeedType != null)
                AddImage(117 + Utility.ProgressBarX(targetHeatPercent), 429, 2436, 52);
            
            if (m_PlantBowl.SeedType == null)
                AddLabel(142, 446, WhiteTextHue, "Add Seed");

            else if (m_PlantBowl.SeedType != null && !m_PlantBowl.ReadyForHarvest)
                AddLabel(127, 446, WhiteTextHue, "Add Ingredient");

            else
                AddLabel(145, 446, 63, "Harvest");

            AddButton(136, 469, 1147, 1148, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;            
            if (m_Player.Deleted) return;
            if (!m_Player.Alive) return;
            if (m_PlantBowl == null) return;
            if (m_PlantBowl.Deleted) return;
            
            if (!m_Player.InRange(m_PlantBowl.GetWorldLocation(), 2))
            {
                m_Player.SendMessage("You are too far away to access that.");
                return;
            }

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Action
                case 2:
                    if (m_PlantBowl.ReadyForHarvest)                    
                        m_PlantBowl.Harvest(m_Player);                    

                    else
                        m_PlantBowl.AddIngredient(m_Player);

                    closeGump = false;
                break;                
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(PlantGump));
                m_Player.SendGump(new PlantGump(m_Player, m_PlantBowl));
            }

            else
                m_Player.PlaySound(closeGumpSound);
        }
    }
}