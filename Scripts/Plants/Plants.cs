using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server
{
    public class Plants
    {
        public enum PlantGroupType
        {
            Crop,
            Fern,
            Flower,
            Grass,
            Tree,
            Vine
        }

        public static Type OnSeedGrowthComplete(Type seedType)
        {
            Type plantType = null;

            double chanceResult = Utility.RandomDouble();

            #region Tree Seeds

            if (seedType == typeof(LeafySeed))
            {
                if (chanceResult < .50)
                    return typeof(LeafyBush);

                else
                    return typeof(PointyBush);
            }

            #endregion

            return plantType;
        }

        public static List<Item> OnPlantHarvest(Type plantType)
        {
            List<Item> m_Items = new List<Item>();

            #region Specific Plant Overrides 

            if (plantType == typeof(TreeStump))
            {
            }

            #endregion

            else            
                m_Items.Add((Item)Activator.CreateInstance(plantType));            

            return m_Items;
        }

        public static SeedDetail GetSeedDetail(Type seedType)
        {
            SeedDetail seedDetail = new SeedDetail();

            #region Trees

            if (seedType == typeof(LeafySeed))
            {
                seedDetail.Name = "a leafy seed";
                seedDetail.DisplayName = "Leafy Seed";
                seedDetail.DisplayNameHue = 2599;

                seedDetail.Description = "a vibrant, green seed";

                seedDetail.IconItemID = 22326;
                seedDetail.IconHue = 0;
                seedDetail.IconOffsetX = 57;
                seedDetail.IconOffsetY = 39;
                seedDetail.GumpCollectionId = "";

                seedDetail.PlantGroup = PlantGroupType.Tree;

                seedDetail.GrowthTarget = 70;
                seedDetail.WaterTarget = 50;
                seedDetail.SoilTarget = 25;
                seedDetail.HeatTarget = 25;
            }

            #endregion


            return seedDetail;
        }

        public static PlantDetail GetPlantDetail(Type plantType)
        {
            PlantDetail plantDetail = new PlantDetail();

            #region Trees

            if (plantType == typeof(LeafyBush))
            {
                plantDetail.Name = "a leafy bush";

                plantDetail.DisplayName = "Leafy Bush";
                plantDetail.DisplayNameHue = 2499;

                plantDetail.Description = "a leafy and vibrant bush";

                plantDetail.IconItemID = 13285;
                plantDetail.IconHue = 0;
                plantDetail.IconOffsetX = 48;
                plantDetail.IconOffsetY = 21;
                plantDetail.GumpCollectionId = "";

                plantDetail.PlantGroup = PlantGroupType.Tree;
            }

            if (plantType == typeof(PointyBush))
            {
                plantDetail.Name = "a pointy bush";

                plantDetail.DisplayName = "Pointy Bush";
                plantDetail.DisplayNameHue = 2499;

                plantDetail.Description = "a sharp and pointy bush";

                plantDetail.IconItemID = 13279;
                plantDetail.IconHue = 0;
                plantDetail.IconOffsetX = 54;
                plantDetail.IconOffsetY = -11;
                plantDetail.GumpCollectionId = "";

                plantDetail.PlantGroup = PlantGroupType.Tree;
            }

            #endregion

            return plantDetail;
        }

        public class SeedDetail
        {
            public string Name = "Seed Name";
            public string DisplayName = "Seed Display Name";
            public int DisplayNameHue = 2499;

            public string Description = "Seed Description";

            public int IconItemID = 22326;
            public int IconHue = 0;
            public int IconOffsetX = 0;
            public int IconOffsetY = 0;
            public string GumpCollectionId = "";

            public PlantGroupType PlantGroup = PlantGroupType.Tree;

            public double GrowthTarget = 70;
            public double WaterTarget = 50;
            public double SoilTarget = 25;
            public double HeatTarget = 25;

            public SeedDetail()
            {
            }
        }

        public class PlantDetail
        {
            public string Name = "Plant Name";
            public string DisplayName = "Plant Display Name";
            public int DisplayNameHue = 2499;

            public string Description = "Plant Description";

            public int IconItemID = 22326;
            public int IconHue = 0;
            public int IconOffsetX = 0;
            public int IconOffsetY = 0;
            public string GumpCollectionId = "";
            
            public PlantGroupType PlantGroup = PlantGroupType.Tree;

            public PlantDetail()
            {
            }
        }
    }
}