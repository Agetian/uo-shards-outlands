using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.ContextMenus;

namespace Server.Items
{
	public class TreasureMap : MapItem
	{
        public static int MinLevel = 1;
        public static int MaxLevel = 6;

        public const double LootChance = 0.005; // 1% chance to appear as loot

        private static Type[][] m_SpawnTypes = new Type[][]
		{
			new Type[]{ typeof(Orc), typeof(Skeleton), typeof(Ratman), typeof(SkitteringHopper), typeof(LavaSnake), typeof(EarthOreling), typeof(RatmanArcher), typeof(Shade) },
			new Type[]{ typeof(RatmanMage), typeof(EarthElemental), typeof(Brigand), typeof(Gazer), typeof(Troll), typeof(OrcishLord), typeof(Ogre), typeof(Salamander) },
			new Type[]{ typeof(CorruptRunecaster), typeof(WaterElemental), typeof(Savage), typeof(OrcishSurjin), typeof(BurrowBeetle), typeof(Bootlegger), typeof(DragonWhelp), typeof(GraveRobber) },
			new Type[]{ typeof(Lich), typeof(Drake), typeof(TombRaider), typeof(Minotaur), typeof(DriderWarrior), typeof(Smuggler), typeof(OgreMage), typeof(CyclopsShaman) },
			new Type[]{ typeof(OgreLord), typeof(ElderGazer), typeof(ElderAirElemental), typeof(DarkWisp), typeof(MinotaurCaptain), typeof(SilverSerpent), typeof(OrcishMaurk), typeof(Dragon) },
			new Type[]{ typeof(LichLord), typeof(Daemon), typeof(BloodElemental), typeof(GreaterCyclops), typeof(SanguinWizard), typeof(FrostDragon), typeof(IceFiend) },
			new Type[]{ typeof(AncientWyrm), typeof(Balron), typeof(ElderBloodElemental), typeof(PoisonElemental), typeof(DriderHarbinger), typeof(SilverDaemon), typeof(ArmoredTitan), typeof(AncientLich) }
		};

        private int m_Level;
		[CommandProperty( AccessLevel.GameMaster )]
		public int Level
        { 
            get{ return m_Level; }
            set
            {
                m_Level = value;
                InvalidateProperties(); 
            }
        }

        private bool m_Archived;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Archived
        {
            get { return m_Archived; }
            set
            {
                m_Archived = value;
            }
        }

        private bool m_Decoded;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Decoded
        {
            get { return m_Decoded; }
            set
            {
                m_Decoded = value;
                InvalidateProperties();
            }
        }

        private bool m_Completed;
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Completed
        {
            get{ return m_Completed; } 
            set{ m_Completed = value; InvalidateProperties();
            } 
        }

        private Mobile m_CompletedBy;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile CompletedBy
        {
            get{ return m_CompletedBy; } 
            set{ m_CompletedBy = value; InvalidateProperties(); }
        }

        private Map m_ChestMap;
		[CommandProperty( AccessLevel.GameMaster )]
		public Map ChestMap
        {
            get{ return m_ChestMap; } 
            set
            {
                m_ChestMap = value; 
                InvalidateProperties();
            }
        }

        private Point2D m_ActualLocation;
		[CommandProperty( AccessLevel.GameMaster )]
		public Point2D ActualLocation
        { 
            get{ return m_ActualLocation; }
            set{ m_ActualLocation = value; } 
        }

        private Point2D m_DisplayedLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D DisplayedLocation
        {
            get { return m_DisplayedLocation; }
            set { m_DisplayedLocation = value; }
        }

		private static Point2D[] m_Locations;		

        [Constructable]
        public TreasureMap(int level, Map map)
        {
            m_Level = level;
            m_ChestMap = map;            
        }

        public TreasureMap(Serial serial): base(serial)
        {
        }

        public static string GetMapDisplayName(int mapLevel)
        {
            switch (mapLevel)
            {
                case 1: return "Plainly Drawn"; break;
                case 2: return "Expertly Drawn"; break;
                case 3: return "Adeptly Drawn"; break;
                case 4: return "Cleverly Drawn"; break;
                case 5: return "Deviously Drawn"; break;
                case 6: return "Ingeniously Drawn"; break;
            }

            return "Simply Drawn";
        }

		public static Point2D GetRandomLocation()
		{
			if ( m_Locations == null )
				LoadLocations();

			if ( m_Locations.Length > 0 )
				return m_Locations[Utility.Random( m_Locations.Length )];

			return Point2D.Zero;
		}
        
		private static void LoadLocations()
		{
			string filePath = Path.Combine( Core.BaseDirectory, "Data/treasure.cfg" );

			List<Point2D> list = new List<Point2D>();
			List<Point2D> havenList = new List<Point2D>();

			if ( File.Exists( filePath ) )
			{
				using ( StreamReader ip = new StreamReader( filePath ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						try
						{
							string[] split = line.Split( ' ' );

							int x = Convert.ToInt32( split[0] ), y = Convert.ToInt32( split[1] );

							Point2D loc = new Point2D( x, y );
							list.Add( loc );
						}

						catch
						{
						}
					}
				}
			}

			m_Locations = list.ToArray();
		}
        
		public static BaseCreature Spawn( int level, Point3D p, bool guardian )
		{
			if ( level >= 0 && level < m_SpawnTypes.Length )
			{
				BaseCreature bc;

				try
				{
					bc = (BaseCreature)Activator.CreateInstance( m_SpawnTypes[level][Utility.Random( m_SpawnTypes[level].Length )] );
				}

				catch
				{
					return null;
				}

				bc.Home = p;
				bc.RangeHome = 5;

				if ( guardian && level == 0 )
				{
					bc.Name = "a chest guardian";
					bc.Hue = 0x835;
				}

				return bc;
			}

			return null;
		}

		public static BaseCreature Spawn( int level, Point3D p, Map map, Mobile target, bool guardian )
		{
			if ( map == null )
				return null;

			BaseCreature c = Spawn( level, p, guardian );

			if ( c != null )
			{
				bool spawned = false;

				for ( int i = 0; !spawned && i < 10; ++i )
				{
					int x = p.X - 3 + Utility.Random( 7 );
					int y = p.Y - 3 + Utility.Random( 7 );

					if ( map.CanSpawnMobile( x, y, p.Z ) )
					{
						c.MoveToWorld( new Point3D( x, y, p.Z ), map );
						spawned = true;
					}

					else
					{
						int z = map.GetAverageZ( x, y );

						if ( map.CanSpawnMobile( x, y, z ) )
						{
							c.MoveToWorld( new Point3D( x, y, z ), map );
							spawned = true;
						}
					}
				}

				if ( !spawned )
				{
					c.Delete();
					return null;
				}

				if ( target != null )
					c.Combatant = target;

				return c;
			}

			return null;
		}		

		public static bool HasDiggingTool( Mobile m )
		{
			if ( m.Backpack == null )
				return false;

			List<BaseHarvestTool> items = m.Backpack.FindItemsByType<BaseHarvestTool>();

			foreach ( BaseHarvestTool tool in items )
			{
				if ( tool.HarvestSystem == Engines.Harvest.Mining.System )
					return true;
			}

			return false;
		}

		public void OnBeginDig( Mobile from )
		{
            if (m_Completed)
                from.SendMessage("This treasure map has already been completed.");

            else if ( m_Level == 0 && !CheckYoung( from ) )			
                from.SendMessage("Only a young player may attempt this treasure map.");

            else if ( !m_Decoded )			
                from.SendMessage("This treasure map has not yet been decoded.");

            else if (!HasRequiredSkill( from ) )	
		        from.SendMessage("You are not skilled enough in cartography to attempt to locate this treasure.");

            else if ( !from.CanBeginAction( typeof( TreasureMap ) ) )			
				from.SendMessage("You are already digging for treasure.");	

            else if ( from.Map != this.m_ChestMap )		
				from.SendMessage("You seem to be in the right location but wrong facet!");	

            else
			{
				from.SendMessage("Where do you wish to dig?");
				from.Target = new DigTarget( this );
			}
		}

		private class DigTarget : Target
		{
			private TreasureMap m_Map;

			public DigTarget( TreasureMap map ) : base( 6, true, TargetFlags.None )
			{
				m_Map = map;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Map.Deleted )
					return;

				Map map = m_Map.m_ChestMap;

				if ( m_Map.m_Completed )
                    from.SendMessage("This treasure map has not yet been decoded.");		

                else if (!m_Map.Decoded)
                    from.SendMessage("You have not yet decoded this treasure map.");

                else if (!m_Map.HasRequiredSkill(from))
                    from.SendMessage("You are not skilled enough in cartography to attempt to locate this treasure.");

                else if (!from.CanBeginAction(typeof(TreasureMap)))	
                    from.SendMessage("You are already digging for treasure.");

                else if (from.Map != m_Map.m_ChestMap)
                    from.SendMessage("You seem to be in the right location but wrong facet!");			

				else
				{
					IPoint3D p = targeted as IPoint3D;

					Point3D targ3D;

					if ( p is Item )
						targ3D = ((Item)p).GetWorldLocation();

					else
						targ3D = new Point3D( p );

					int maxRange;
					double skillValue = from.Skills[SkillName.Mining].Value;

					if ( skillValue >= 100.0 )
						maxRange = 4;

					else if ( skillValue >= 81.0 )
						maxRange = 3;

					else if ( skillValue >= 51.0 )
						maxRange = 2;

					else
						maxRange = 1;

					Point2D loc = m_Map.m_ActualLocation;
					int x = loc.X, y = loc.Y;

					Point3D chest3D0 = new Point3D( loc, 0 );

					if ( Utility.InRange( targ3D, chest3D0, maxRange ) )
					{
						if ( from.Location.X == x && from.Location.Y == y )						
							from.SendLocalizedMessage( 503030 ); // The chest can't be dug up because you are standing on top of it.
						
						else if ( map != null )
						{
							int z = map.GetAverageZ( x, y );

							if ( !map.CanFit( x, y, z, 16, true, true ) )							
								from.SendLocalizedMessage( 503021 ); // You have found the treasure chest but something is keeping it from being dug up.
							
							else if ( from.BeginAction( typeof( TreasureMap ) ) )							
								new DigTimer( from, m_Map, new Point3D( x, y, z ), map ).Start();
							
							else							
								from.SendLocalizedMessage( 503020 ); // You are already digging treasure.							
						}
					}

					else if ( m_Map.Level > 0 )
					{
						if ( Utility.InRange( targ3D, chest3D0, 8 ) ) // We're close, but not quite						
							from.SendLocalizedMessage( 503032 ); // You dig and dig but no treasure seems to be here.
						
						else						
							from.SendLocalizedMessage( 503035 ); // You dig and dig but fail to find any treasure.						
					}					
				}
			}
		}

		private class DigTimer : Timer
		{
			private Mobile m_From;
			private TreasureMap m_TreasureMap;

			private Point3D m_Location;
			private Map m_Map;

			private TreasureChestDirt m_Dirt1;
			private TreasureChestDirt m_Dirt2;
			private TreasureMapChest m_Chest;

			private int m_Count;

			private long m_NextSkillTime;
			private DateTime m_NextSpellTime;
			private long m_NextActionTime;
			private long m_LastMoveTime;

			public DigTimer( Mobile from, TreasureMap treasureMap, Point3D location, Map map ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
			{
				m_From = from;
				m_TreasureMap = treasureMap;

				m_Location = location;
				m_Map = map;

				m_NextSkillTime = from.NextSkillTime;
				m_NextSpellTime = from.NextSpellTime;
				m_NextActionTime = from.NextActionTime;
				m_LastMoveTime = from.LastMoveTime;

				Priority = TimerPriority.TenMS;
			}

			private void Terminate()
			{
				Stop();
				m_From.EndAction( typeof( TreasureMap ) );

				if ( m_Chest != null )
					m_Chest.Delete();

				if ( m_Dirt1 != null )
				{
					m_Dirt1.Delete();
					m_Dirt2.Delete();
				}
			}

			protected override void OnTick()
			{
				if ( m_NextSkillTime != m_From.NextSkillTime || m_NextSpellTime != m_From.NextSpellTime || m_NextActionTime != m_From.NextActionTime )
				{
					Terminate();
					return;
				}

				if ( m_LastMoveTime != m_From.LastMoveTime )
				{
					m_From.SendLocalizedMessage( 503023 ); // You cannot move around while digging up treasure. You will need to start digging anew.
					Terminate();

					return;
				}

				int z = ( m_Chest != null ) ? m_Chest.Z + m_Chest.ItemData.Height : int.MinValue;
				int height = 16;

				if ( z > m_Location.Z )
					height -= ( z - m_Location.Z );

				else
					z = m_Location.Z;

				if ( !m_Map.CanFit( m_Location.X, m_Location.Y, z, height, true, true, false ) )
				{
					m_From.SendLocalizedMessage( 503024 ); // You stop digging because something is directly on top of the treasure chest.
					Terminate();
					return;
				}

				m_Count++;

				m_From.RevealingAction();
				m_From.Direction = m_From.GetDirectionTo( m_Location );

				if ( m_Count > 1 && m_Dirt1 == null )
				{
					m_Dirt1 = new TreasureChestDirt();
					m_Dirt1.MoveToWorld( m_Location, m_Map );

					m_Dirt2 = new TreasureChestDirt();
					m_Dirt2.MoveToWorld( new Point3D( m_Location.X, m_Location.Y - 1, m_Location.Z ), m_Map );
				}

				if ( m_Count == 5 )				
					m_Dirt1.Turn1();
				
				else if ( m_Count == 10 )
				{
					m_Dirt1.Turn2();
					m_Dirt2.Turn2();
				}

				else if ( m_Count > 10 )
				{
					if ( m_Chest == null )
					{
						m_Chest = new TreasureMapChest( m_From, m_TreasureMap.Level, true );
						m_Chest.MoveToWorld( new Point3D( m_Location.X, m_Location.Y, m_Location.Z - 15 ), m_Map );
					}

					else
					{
						m_Chest.Z++;
					}

					Effects.PlaySound( m_Chest, m_Map, 0x33B );
				}

				if ( m_Chest != null && m_Chest.Location.Z >= m_Location.Z )
				{
					Stop();
					m_From.EndAction( typeof( TreasureMap ) );

					m_Chest.Temporary = false;
					m_TreasureMap.Completed = true;
					m_TreasureMap.CompletedBy = m_From;

					int spawns;

					switch ( m_TreasureMap.Level )
					{
						case 0: spawns = 3; break;
						case 1: spawns = 0; break;
						default: spawns = 4; break;
					}

					for ( int i = 0; i < spawns; ++i )
					{
						BaseCreature bc = Spawn( m_TreasureMap.Level, m_Chest.Location, m_Chest.Map, null, true );

						if ( bc != null )
							m_Chest.Guardians.Add( bc );
					}
				}

				else
				{
					if ( m_From.Body.IsHuman && !m_From.Mounted )
						m_From.Animate( 11, 5, 1, true, false, 0 );

					new SoundTimer( m_From, 0x125 + (m_Count % 2) ).Start();
				}
			}

			private class SoundTimer : Timer
			{
				private Mobile m_From;
				private int m_SoundID;

				public SoundTimer( Mobile from, int soundID ) : base( TimeSpan.FromSeconds( 0.9 ) )
				{
					m_From = from;
					m_SoundID = soundID;

					Priority = TimerPriority.TenMS;
				}

				protected override void OnTick()
				{
					m_From.PlaySound( m_SoundID );
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
				return;
			}

            /*
            if (m_Completed)
            {
                from.SendMessage("You may now apply your findings from this towards an ancient mystery scroll.");
                return;
            }
            */

			if ( !m_Completed && !m_Decoded )
				Decode( from );

			else
				DisplayTo( from );
		}

		private bool CheckYoung( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			if ( from is PlayerMobile && ((PlayerMobile)from).Young )
				return true;

			return false;
		}

		private double GetMinSkillLevel()
		{
			switch ( m_Level )
			{
				case 1: return 17.0;
				case 2: return 61.0;
				case 3: return 71.0;
				case 4: return 81.0;
				case 5: return 91.0;
				case 6: return 99.9;

				default: return 0.0;
			}
		}

		private bool HasRequiredSkill( Mobile from )
		{
			return ( from.Skills[SkillName.Cartography].Value >= GetMinSkillLevel() );
		}

		public void Decode( Mobile from )
		{
			if ( m_Completed || Decoded )
				return;

			if ( m_Level == 0 )
			{
				if ( !CheckYoung( from ) )
				{
					from.SendLocalizedMessage( 1046447 ); // Only a young player may use this treasure map.
					return;
				}
			}

			else
			{
				double minSkill = GetMinSkillLevel();

				if ( from.Skills[SkillName.Cartography].Value < minSkill )
					from.SendLocalizedMessage( 503013 ); // The map is too difficult to attempt to decode.

				double maxSkill = minSkill + 20;

				if ( !from.CheckSkill( SkillName.Cartography, minSkill - 10, maxSkill, 1.0) )
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 503018 ); // You fail to make anything of the map.
					return;
				}
			}

			from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 503019 ); // You successfully decode a treasure map!
			Decoded = true;

            m_ActualLocation = GetRandomLocation();

            m_DisplayedLocation = m_ActualLocation;

            int locationDeviation = m_Level * 5;

            if (Utility.RandomDouble() <= .5)
                m_DisplayedLocation.X += Utility.RandomMinMax(0, locationDeviation);
            else
                m_DisplayedLocation.X -= Utility.RandomMinMax(0, locationDeviation);

            if (Utility.RandomDouble() <= .5)
                m_DisplayedLocation.Y += Utility.RandomMinMax(0, locationDeviation);
            else
                m_DisplayedLocation.Y -= Utility.RandomMinMax(0, locationDeviation);

            Width = 150; //300
            Height = 150; //300

            int width = 150; //600
            int height = 150; //600

            int x1 = m_DisplayedLocation.X - Utility.RandomMinMax(width / 4, (width / 4) * 3);
            int y1 = m_DisplayedLocation.Y - Utility.RandomMinMax(height / 4, (height / 4) * 3);

            if (x1 < 0)
                x1 = 0;

            if (y1 < 0)
                y1 = 0;

            int x2 = x1 + width;
            int y2 = y1 + height;

            if (x2 >= 5120)
                x2 = 5119;

            if (y2 >= 4096)
                y2 = 4095;

            x1 = x2 - width;
            y1 = y2 - height;

            Bounds = new Rectangle2D(x1, y1, width, height);
            Protected = true;

            AddWorldPin(m_DisplayedLocation.X, m_DisplayedLocation.Y);

			DisplayTo( from );
		}

		public override void DisplayTo( Mobile from )
		{
            if (m_Completed)
            {
                from.SendMessage("This treasure map has already been completed.");
                return;
            }

            else if (m_Level == 0 && !CheckYoung(from))
            {
                from.SendMessage("Only a young player may attempt this treasure map.");
                return;
            }

            else if (!m_Decoded)
            {
                from.SendMessage("This treasure map has not yet been decoded.");
                return;
            }

            else if (!HasRequiredSkill(from))
            {
                from.SendMessage("You are not skilled enough in cartography to attempt to locate this treasure.");
                return;
            }

            else
                from.SendMessage("The treasure's estimated location is now marked by a pin on the map. Use a shovel to excavate it.");

			from.PlaySound( 0x249 );

			base.DisplayTo( from );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( !m_Completed )
			{
				if ( !Decoded )
				{
					list.Add( new DecodeMapEntry( this ) );
				}

				else
				{
					bool digTool = HasDiggingTool( from );

					list.Add( new OpenMapEntry( this ) );
					list.Add( new DigEntry( this, digTool ) );
				}
			}
		}

		private class DecodeMapEntry : ContextMenuEntry
		{
			private TreasureMap m_Map;

			public DecodeMapEntry( TreasureMap map ) : base( 6147, 2 )
			{
				m_Map = map;
			}

			public override void OnClick()
			{
				if ( !m_Map.Deleted )
					m_Map.Decode( Owner.From );
			}
		}

		private class OpenMapEntry : ContextMenuEntry
		{
			private TreasureMap m_Map;

			public OpenMapEntry( TreasureMap map ) : base( 6150, 2 )
			{
				m_Map = map;
			}

			public override void OnClick()
			{
				if ( !m_Map.Deleted )
					m_Map.DisplayTo( Owner.From );
			}
		}

		private class DigEntry : ContextMenuEntry
		{
			private TreasureMap m_Map;

			public DigEntry( TreasureMap map, bool enabled ) : base( 6148, 2 )
			{
				m_Map = map;

				if ( !enabled )
					this.Flags |= CMEFlags.Disabled;
			}

			public override void OnClick()
			{
				if ( m_Map.Deleted )
					return;

				Mobile from = Owner.From;

				if ( HasDiggingTool( from ) )
					m_Map.OnBeginDig( from );

				else
					from.SendMessage( "You must have a digging tool to dig for treasure." );
			}
		}

		public override int LabelNumber
		{ 
			get
			{ 
				if ( Decoded )
				{
					if ( m_Level == 6 )
						return 1063453;	

					else
						return 1041516 + m_Level;
				}

				else if ( m_Level == 6 )
					return 1063452;

				else
					return 1041510 + m_Level;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( m_ChestMap == Map.Felucca ? 1041502 : 1041503 ); // for somewhere in Felucca : for somewhere in Trammel

			if ( m_Completed )			
				list.Add( 1041507, m_CompletedBy == null ? "someone" : m_CompletedBy.Name ); // completed by ~1_val~			
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Completed )			
				from.Send( new MessageLocalizedAffix( Serial, ItemID, MessageType.Label, 0x3B2, 3, 1048030, "", AffixType.Append, String.Format( " completed by {0}", m_CompletedBy == null ? "someone" : m_CompletedBy.Name ), "" ) );
			
			else if ( Decoded )
			{
				if ( m_Level == 6 )
					LabelTo( from, 1063453 );

				else
					LabelTo( from, 1041516 + m_Level );
			}

			else
			{
				if ( m_Level == 6 )
					LabelTo( from, 1041522, String.Format( "#{0}\t \t#{1}", 1063452, m_ChestMap == Map.Felucca ? 1041502 : 1041503 ) );

				else
					LabelTo( from, 1041522, String.Format( "#{0}\t \t#{1}", 1041510 + m_Level, m_ChestMap == Map.Felucca ? 1041502 : 1041503 ) );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write(0);

			writer.Write(m_CompletedBy);
			writer.Write(m_Level);
            writer.Write(m_Archived);
            writer.Write(m_Decoded);
			writer.Write(m_Completed);
			writer.Write(m_ChestMap);
			writer.Write(m_ActualLocation);
            writer.Write(m_DisplayedLocation);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_CompletedBy = reader.ReadMobile();
                m_Level = reader.ReadInt();
                m_Archived = reader.ReadBool();
                m_Decoded = reader.ReadBool();
                m_Completed = reader.ReadBool();
                m_ChestMap = reader.ReadMap();
                m_ActualLocation = reader.ReadPoint2D();
                m_DisplayedLocation = reader.ReadPoint2D();
            }
		}
	}

	public class TreasureChestDirt : Item
	{
		public TreasureChestDirt() : base( 0x912 )
		{
			Movable = false;

			Timer.DelayCall( TimeSpan.FromMinutes( 2.0 ), new TimerCallback( Delete ) );
		}

		public TreasureChestDirt( Serial serial ) : base( serial )
		{
		}

		public void Turn1()
		{
			ItemID = 0x913;
		}

		public void Turn2()
		{
			ItemID = 0x914;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

			Delete();
		}
	}
}