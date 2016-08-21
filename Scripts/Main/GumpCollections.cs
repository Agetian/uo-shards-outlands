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
