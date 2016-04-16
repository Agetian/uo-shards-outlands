using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Guilds;
using Server.Items;
using Server.Misc;
using Server.Regions;
using Server.Spells;

namespace Server.Multis
{
	public enum HousePlacementResult
	{
		Valid,
		BadRegion,
		BadLand,
		BadStatic,
		BadItem,
		NoSurface,
        BadRegionExistingHouse,
		BadRegionHidden,
		BadRegionTemp,
		InvalidCastleKeep,
		BadRegionRaffle
	}

	public class HousePlacement
	{
		private const int YardSize = 5;
        private const int SideyardSize = 3;
		
		private static int[] m_RoadIDs = new int[]
		{
			0x0071, 0x0078,
			0x00E8, 0x00EB,
			0x07AE, 0x07B1,
			0x3FF4, 0x3FF4,
			0x3FF8, 0x3FFB,
			0x0442, 0x0479,
			0x0501, 0x0510,
			0x0009, 0x0015,
			0x0150, 0x015C
		};

        public static void ShowBlockingTiles(Mobile from, List<Point2D> blockedPoints, List<Point2D> yardPoints, Map map)
        {
            if (from == null)
                return;
            
            from.SendSound(0x64B);

            int blockedPointsCount = blockedPoints.Count;

            if (blockedPointsCount > 100)
                blockedPointsCount = 100;            

            for (int a = 0; a < blockedPointsCount; a++)
            {
                Point3D location;
                Point2D point = blockedPoints[a];

                BlockedHousingTimedStatic blockedLocation = new BlockedHousingTimedStatic(0x3709, 3);
                blockedLocation.Name = "blocked housing tile";
                blockedLocation.Hue = 2953;
                blockedLocation.m_Owner = from;

                LandTile landTile = map.Tiles.GetLandTile( point.X, point.Y );

                location = new Point3D(point.X, point.Y, landTile.Z);

                blockedLocation.MoveToWorld(location, map);
            }

            int blockedYardCount = yardPoints.Count;

            if (blockedYardCount > 100)
                blockedYardCount = 100;

            for (int a = 0; a < blockedYardCount; a++)
            {
                Point3D location;
                Point2D point = yardPoints[a];

                BlockedHousingTimedStatic blockedLocation = new BlockedHousingTimedStatic(0x3709, 3);
                blockedLocation.Name = "blocked housing tile";
                blockedLocation.Hue = 2579;
                blockedLocation.m_Owner = from;

                LandTile landTile = map.Tiles.GetLandTile(point.X, point.Y);

                location = new Point3D(point.X, point.Y, landTile.Z);

                blockedLocation.MoveToWorld(location, map);
            }
        }

        public static HousePlacementResult Check(Mobile from, int multiID, Point3D center, out ArrayList toMove, bool east_facing_door)
		{
			toMove = new ArrayList();

            //Basic Limitations

			Map map = from.Map;
            
			if ( map == null || map == Map.Internal )
				return HousePlacementResult.BadLand; // A house cannot go here
            
			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return HousePlacementResult.Valid; // Staff can place anywhere
            
			if ( map == Map.Ilshenar || SpellHelper.IsFeluccaT2A( map, center ) )
				return HousePlacementResult.BadRegion; // No houses in Ilshenar/T2A
            
			if ( map == Map.Malas && ( multiID == 0x007C || multiID == 0x007E ) )
				return HousePlacementResult.InvalidCastleKeep;
            
			NoHousingRegion noHousingRegion = (NoHousingRegion) Region.Find( center, map ).GetRegion( typeof( NoHousingRegion ) );

			if ( noHousingRegion != null )
				return HousePlacementResult.BadRegion;
            
            //Tile-Specific Limitations

             /* Placement Rules:			  
			 1) All tiles which are around the -outside- of the foundation must not have anything impassable.
			 2) No impassable object or land tile may come in direct contact with any part of the house.
			 3) Five tiles from the front and back of the house must be completely clear of all house tiles.
			 4) The foundation must rest flatly on a surface. Any bumps around the foundation are not allowed.
			 5) No foundation tile may reside over terrain which is viewed as a road.
			 */

            HousePlacementResult firstBadResult = HousePlacementResult.Valid;
            List<Point2D> m_BlockedTiles = new List<Point2D>();
            List<Point2D> m_BadProximityTiles = new List<Point2D>();  
            Point2D badTile;

			MultiComponentList mcl = MultiData.GetComponents( multiID );

            //AOS House With Stairs
			if ( multiID >= 0x13EC && multiID < 0x1D00 )
				HouseFoundation.AddStairsTo( ref mcl );

			//Northwest Corner of House
			Point3D start = new Point3D( center.X + mcl.Min.X, center.Y + mcl.Min.Y, center.Z );

			List<Item> items = new List<Item>();
			List<Mobile> mobiles = new List<Mobile>();
			List<Point2D> yard = new List<Point2D>(), borders = new List<Point2D>();			 

			for ( int x = 0; x < mcl.Width; ++x )
			{
				for ( int y = 0; y < mcl.Height; ++y )
				{
					int tileX = start.X + x;
					int tileY = start.Y + y;

                    StaticTile[] addTiles = mcl.Tiles[x][y];

					if ( addTiles.Length == 0 )
						continue;

					Point3D testPoint = new Point3D( tileX, tileY, center.Z );
					Region reg = Region.Find( testPoint, map );                    

					if ( !reg.AllowHousing( from, testPoint ) ) // Cannot place houses in dungeons, towns, treasure map areas etc
					{
                        if (reg.IsPartOf(typeof(HouseRegion)))
                        {
                            if (firstBadResult == HousePlacementResult.Valid)
                                firstBadResult = HousePlacementResult.BadRegionExistingHouse;

                            badTile = new Point2D(tileX, tileY);

                            if (!m_BadProximityTiles.Contains(badTile))
                                m_BadProximityTiles.Add(badTile);
                        }

                        else
                        {
                            if (reg.IsPartOf(typeof(TempNoHousingRegion)))
                                return HousePlacementResult.BadRegionTemp;

                            if (reg.IsPartOf(typeof(TreasureRegion)))
                                return HousePlacementResult.BadRegionHidden;

                            if (reg.IsPartOf(typeof(HouseRaffleRegion)))
                                return HousePlacementResult.BadRegionRaffle;

                            return HousePlacementResult.BadRegion;
                        }
					}

                    LandTile landTile = map.Tiles.GetLandTile(tileX, tileY);
                    int landID = landTile.ID & TileData.MaxLandValue;
                    
                    StaticTile[] oldTiles = map.Tiles.GetStaticTiles(tileX, tileY, true);

					Sector sector = map.GetSector( tileX, tileY );

					items.Clear();

					for ( int i = 0; i < sector.Items.Count; ++i )
					{
						Item item = sector.Items[i];

						if ( item.Visible && item.X == tileX && item.Y == tileY )
							items.Add( item );
					}

					mobiles.Clear();

					for ( int i = 0; i < sector.Mobiles.Count; ++i )
					{
						Mobile m = sector.Mobiles[i];

						if ( m.X == tileX && m.Y == tileY )
							mobiles.Add( m );
					}

					int landStartZ = 0, landAvgZ = 0, landTopZ = 0;

					map.GetAverageZ( tileX, tileY, ref landStartZ, ref landAvgZ, ref landTopZ );

					bool hasFoundation = false;

					for ( int i = 0; i < addTiles.Length; ++i )
					{
                        StaticTile addTile = addTiles[i];

						if ( addTile.ID == 0x1 ) //Nodraw
							continue;

						TileFlag addTileFlags = TileData.ItemTable[addTile.ID & TileData.MaxItemValue].Flags;

						bool isFoundation = ( addTile.Z == 0 && (addTileFlags & TileFlag.Wall) != 0 );
						bool hasSurface = false;

						if ( isFoundation )
							hasFoundation = true;

						int addTileZ = center.Z + addTile.Z;
						int addTileTop = addTileZ + addTile.Height;

						if ( (addTileFlags & TileFlag.Surface) != 0 )
							addTileTop += 16;

                        //Broke Rule 2
                        if (addTileTop > landStartZ && landAvgZ > addTileZ)
                        {
                            if (firstBadResult == HousePlacementResult.Valid)
                                firstBadResult = HousePlacementResult.BadLand;

                            badTile = new Point2D(tileX, tileY);

                            if (!m_BlockedTiles.Contains(badTile))
                                m_BlockedTiles.Add(badTile);
                        }

						if ( isFoundation && ((TileData.LandTable[landTile.ID & TileData.MaxLandValue].Flags & TileFlag.Impassable) == 0) && landAvgZ == center.Z )						
							hasSurface = true;

						for ( int j = 0; j < oldTiles.Length; ++j )
						{
                            StaticTile oldTile = oldTiles[j];
							ItemData id = TileData.ItemTable[oldTile.ID & TileData.MaxItemValue];

                            //Rules 2 Broken
                            if ((id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0)) && addTileTop > oldTile.Z && (oldTile.Z + id.CalcHeight) > addTileZ)
                            {
                                if (firstBadResult == HousePlacementResult.Valid)
                                    firstBadResult = HousePlacementResult.BadStatic;

                                badTile = new Point2D(tileX, tileY);

                                if (!m_BlockedTiles.Contains(badTile))
                                    m_BlockedTiles.Add(badTile);
                            }
						}

						for ( int j = 0; j < items.Count; ++j )
						{
							Item item = items[j];
							ItemData id = item.ItemData;

							if ( addTileTop > item.Z && (item.Z + id.CalcHeight) > addTileZ )
							{
								if ( item.Movable )
									toMove.Add( item );

                                //Broke Rule 2
                                else if ((id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0)))
                                {
                                    if (firstBadResult == HousePlacementResult.Valid)
                                        firstBadResult = HousePlacementResult.BadItem;

                                    badTile = new Point2D(tileX, tileY);

                                    if (!m_BlockedTiles.Contains(badTile))
                                        m_BlockedTiles.Add(badTile);
                                }
							}
						}

                        //Broke Rule 4
                        if (isFoundation && !hasSurface)
                        {
                            if (firstBadResult == HousePlacementResult.Valid)
                                firstBadResult = HousePlacementResult.NoSurface;

                            badTile = new Point2D(tileX, tileY);

                            if (!m_BlockedTiles.Contains(badTile))
                                m_BlockedTiles.Add(badTile);
                        }

						for ( int j = 0; j < mobiles.Count; ++j )
						{
							Mobile m = mobiles[j];

							if ( addTileTop > m.Z && (m.Z + 16) > addTileZ )
								toMove.Add( m );
						}
					}

					for ( int i = 0; i < m_RoadIDs.Length; i += 2 )
					{
                        //Broke Rule 5                        
                        if (landID >= m_RoadIDs[i] && landID <= m_RoadIDs[i + 1])
                        {
                            if (firstBadResult == HousePlacementResult.Valid)
                                firstBadResult = HousePlacementResult.BadLand;

                            badTile = new Point2D(tileX, tileY);

                            if (!m_BlockedTiles.Contains(badTile))
                                m_BlockedTiles.Add(badTile);
                        }
					}

					if ( hasFoundation || east_facing_door)
					{						
						int x_expanse = east_facing_door ? YardSize : SideyardSize;
						int y_expanse = east_facing_door ? YardSize : YardSize;
                        						
						for (int xOffset = -x_expanse; xOffset <= x_expanse; ++xOffset)
						{
							for (int yOffset = -y_expanse; yOffset <= y_expanse; ++yOffset)
							{
								Point2D yardPoint = new Point2D( tileX + xOffset, tileY + yOffset );

								if ( !yard.Contains( yardPoint ) )
									yard.Add( yardPoint );
							}
						}

						for ( int xOffset = -1; xOffset <= 1; ++xOffset )
						{
							for ( int yOffset = -1; yOffset <= 1; ++yOffset )
							{
								if ( xOffset == 0 && yOffset == 0 )
									continue;

								int vx = x + xOffset;
								int vy = y + yOffset;

								if ( vx >= 0 && vx < mcl.Width && vy >= 0 && vy < mcl.Height )
								{
									StaticTile[] breakTiles = mcl.Tiles[vx][vy];
									bool shouldBreak = false;

									for ( int i = 0; !shouldBreak && i < breakTiles.Length; ++i )
									{
										StaticTile breakTile = breakTiles[i];

										if ( breakTile.Height == 0 && breakTile.Z <= 8 && TileData.ItemTable[breakTile.ID & TileData.MaxItemValue].Surface )
											shouldBreak = true;
									}

									if ( shouldBreak )
										continue;
								}

								Point2D borderPoint = new Point2D( tileX + xOffset, tileY + yOffset );

								if ( !borders.Contains( borderPoint ) )
									borders.Add( borderPoint );
							}
						}
					}
				}
			}

			for ( int i = 0; i < borders.Count; ++i )
			{
				Point2D borderPoint = borders[i];

				LandTile landTile = map.Tiles.GetLandTile( borderPoint.X, borderPoint.Y );
				int landID = landTile.ID & TileData.MaxLandValue;

                //Broke Rule
                if ((TileData.LandTable[landID].Flags & TileFlag.Impassable) != 0)
                {
                    if (firstBadResult == HousePlacementResult.Valid)
                        firstBadResult = HousePlacementResult.BadLand;

                    badTile = new Point2D(borderPoint.X, borderPoint.Y);

                    if (!m_BlockedTiles.Contains(badTile))
                        m_BlockedTiles.Add(badTile);
                }

				for ( int j = 0; j < m_RoadIDs.Length; j += 2 )
				{
                    //Broke Rule 5                    
                    if (landID >= m_RoadIDs[j] && landID <= m_RoadIDs[j + 1])
                    {
                        if (firstBadResult == HousePlacementResult.Valid)
                            firstBadResult = HousePlacementResult.BadLand;

                        badTile = new Point2D(borderPoint.X, borderPoint.Y);

                        if (!m_BlockedTiles.Contains(badTile))
                            m_BlockedTiles.Add(badTile);
                    }
				}

                StaticTile[] tiles = map.Tiles.GetStaticTiles(borderPoint.X, borderPoint.Y, true);

				for ( int j = 0; j < tiles.Length; ++j )
				{
                    StaticTile tile = tiles[j];
					ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                    //Broke Rule 1
                    if (id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0 && (tile.Z + id.CalcHeight) > (center.Z + 2)))
                    {
                        if (firstBadResult == HousePlacementResult.Valid)
                            firstBadResult = HousePlacementResult.BadStatic;

                        badTile = new Point2D(borderPoint.X, borderPoint.Y);

                        if (!m_BlockedTiles.Contains(badTile))
                            m_BlockedTiles.Add(badTile);
                    }
				}

				Sector sector = map.GetSector( borderPoint.X, borderPoint.Y );
				List<Item> sectorItems = sector.Items;

				for ( int j = 0; j < sectorItems.Count; ++j )
				{
					Item item = sectorItems[j];

					if ( item.X != borderPoint.X || item.Y != borderPoint.Y || item.Movable )
						continue;

					ItemData id = item.ItemData;

                    //Broke Rule 1
                    if (id.Impassable || (id.Surface && (id.Flags & TileFlag.Background) == 0 && (item.Z + id.CalcHeight) > (center.Z + 2)))
                    {
                        if (firstBadResult == HousePlacementResult.Valid)
                            firstBadResult = HousePlacementResult.BadItem;

                        badTile = new Point2D(borderPoint.X, borderPoint.Y);

                        if (!m_BlockedTiles.Contains(badTile))
                            m_BlockedTiles.Add(badTile);
                    }
				}
			}

			List<Sector> sectors = new List<Sector>();
			List<BaseHouse> houses = new List<BaseHouse>();

			for ( int i = 0; i < yard.Count; i++ ) 
            {
				Sector sector = map.GetSector( yard[i] );
				
				if ( !sectors.Contains( sector ) ) 
                {
					sectors.Add( sector );
					
					if ( sector.Multis != null ) 
                    {
						for ( int j = 0; j < sector.Multis.Count; j++ ) 
                        {
							if ( sector.Multis[j] is BaseHouse ) 
                            {
								BaseHouse house = (BaseHouse)sector.Multis[j];

								if ( !houses.Contains( house ) )                                 
									houses.Add( house );								
							}
						}
					}
				}
			}

			for ( int i = 0; i < yard.Count; ++i )
			{
				foreach ( BaseHouse b in houses )
                {
                    //Broke Rule 3
                    if (b.Contains(yard[i]))
                    {
                        if (firstBadResult != HousePlacementResult.Valid)
                            firstBadResult = HousePlacementResult.BadStatic;

                        badTile = yard[i];

                        if (!m_BadProximityTiles.Contains(badTile) && !m_BlockedTiles.Contains(badTile))
                            m_BadProximityTiles.Add(badTile);
                    }
				}
			}

            if (firstBadResult != HousePlacementResult.Valid)
            {
                ShowBlockingTiles(from, m_BlockedTiles, m_BadProximityTiles, from.Map);    
            
                return firstBadResult;
            }

			return HousePlacementResult.Valid;
		}
	}
}