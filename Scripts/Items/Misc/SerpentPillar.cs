using System;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class SerpentPillar : Item
	{
		private bool m_Active;
		private string m_Word;
		private Rectangle2D m_Destination;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get{ return m_Active; }
			set{ m_Active = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Word
		{
			get{ return m_Word; }
			set{ m_Word = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Rectangle2D Destination
		{
			get{ return m_Destination; }
			set{ m_Destination = value; }
		}

		[Constructable]
		public SerpentPillar() : this( null, new Rectangle2D(), false )
		{
		}

		public SerpentPillar( string word, Rectangle2D destination ) : this( word, destination, true )
		{
		}

		public SerpentPillar( string word, Rectangle2D destination, bool active ) : base( 0x233F )
		{
			Movable = false;

			m_Active = active;
			m_Word = word;
			m_Destination = destination;
		}

		public override bool HandlesOnSpeech{ get{ return true; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !e.Handled && from.InRange( this, 10 ) && e.Speech.ToLower() == this.Word )
			{
				BaseShip ship = BaseShip.FindShipAt( from, from.Map );
                
				if ( ship == null )
					return;

				if ( !this.Active )
				{
					if ( ship.TillerMan != null )
						ship.TillerMan.Say( 502507 ); // Ar, Legend has it that these pillars are inactive! No man knows how it might be undone!

					return;
				}

                
                if (T2AAccess.IsT2A(m_Destination.Start, from.Map))
                {
                    if (ship.Map != null && ship.Map != Map.Internal)
                    {
                        MultiComponentList mcl = ship.Components;

                        IPooledEnumerable eable = ship.Map.GetClientsInBounds(new Rectangle2D(ship.X + mcl.Min.X, ship.Y + mcl.Min.Y, mcl.Width, mcl.Height));

                        foreach (NetState ns in eable)
                            if (ns != null && ns.Mobile != null && ship.Contains((Mobile)ns.Mobile) && !T2AAccess.HasAccess(ns.Mobile))
                            {
                                eable.Free();
                                from.SendMessage("One or more players on board does not have access to T2A at this time.");
                                return;
                            }

                        eable.Free();
                    }


                }

				Map map = from.Map;

				for ( int i = 0; i < 5; i++ ) // Try 5 times
				{
					int x = Utility.Random( Destination.X, Destination.Width );
					int y = Utility.Random( Destination.Y, Destination.Height );
					int z = map.GetAverageZ( x, y );

					Point3D dest = new Point3D( x, y, z );

					if ( ship.CanFit( dest, map, ship.ItemID ) )
					{
						int xOffset = x - ship.X;
						int yOffset = y - ship.Y;
						int zOffset = z - ship.Z;

						ship.Teleport( xOffset, yOffset, zOffset );

						return;
					}
				}

				if ( ship.TillerMan != null )
					ship.TillerMan.Say( 502508 ); // Ar, I refuse to take that matey through here!
			}
		}

		public SerpentPillar( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (bool) m_Active );
			writer.Write( (string) m_Word );
			writer.Write( (Rectangle2D) m_Destination );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Active = reader.ReadBool();
			m_Word = reader.ReadString();
			m_Destination = reader.ReadRect2D();
		}
	}
}