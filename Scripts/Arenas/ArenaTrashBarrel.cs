using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
	public class ArenaTrashBarrel : Container
	{
		public override int DefaultMaxWeight{ get{ return 0; } } // A value of 0 signals unlimited weight
        public static int ItemLimit = 125;

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

        public ArenaController m_ArenaController;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaController ArenaController
        {
            get { return m_ArenaController; }
            set { m_ArenaController = value; }
        }

        public static List<ArenaTrashBarrel> m_Instances = new List<ArenaTrashBarrel>();

		[Constructable]
		public ArenaTrashBarrel() : base( 3703 )
		{
            Name = "a trash barrel";

			Hue = 0x3B2;
			Movable = false;

            //-----

            m_Instances.Add(this);
		}

        public ArenaTrashBarrel(Serial serial): base(serial)
		{
		}

        public static ArenaTrashBarrel GetArenaTrashBarrel(ArenaController arenaController)
        {
            foreach(ArenaTrashBarrel arenaTrashBarrel in m_Instances)
            {
                if (arenaTrashBarrel == null) continue;
                if (arenaTrashBarrel.Deleted) continue;

                if (arenaTrashBarrel.m_ArenaController == arenaController)
                    return arenaTrashBarrel;
            }

            return null;
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();           
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_ArenaController);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

             ArenaController = (ArenaController)reader.ReadItem();

            //-----

			if ( Items.Count > 0 )
			{
                m_Timer = new ArenaTrashBarrelEmptyTimer(this);
				m_Timer.Start();
			}

            m_Instances.Add(this);
		}

        public override void DropItem(Item dropped)
        {
            base.DropItem(dropped);

            if (TotalItems >= ItemLimit)
                Empty(501478); // The trash is full! Emptying!			

            else
            {
                if (m_Timer == null)
                {
                    m_Timer = new ArenaTrashBarrelEmptyTimer(this);
                    m_Timer.Start();
                }
            }
        }

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( !base.OnDragDrop( from, dropped ) )
				return false;

			if ( TotalItems >= ItemLimit )			
				Empty( 501478 ); // The trash is full!  Emptying!			

			else
			{
				SendLocalizedMessageTo( from, 1010442 ); // The item will be deleted in three minutes

				if ( m_Timer != null )
					m_Timer.Stop();

				else
                    m_Timer = new ArenaTrashBarrelEmptyTimer(this);

				m_Timer.Start();
			}

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !base.OnDragDropInto( from, item, p ) )
				return false;

			if ( TotalItems >= ItemLimit )			
				Empty( 501478 ); // The trash is full! Emptying!			

			else
			{
				SendLocalizedMessageTo( from, 1010442 ); // The item will be deleted in three minutes

				if ( m_Timer != null )
					m_Timer.Stop();

				else
                    m_Timer = new ArenaTrashBarrelEmptyTimer(this);

				m_Timer.Start();
			}

			return true;
		}

		public void Empty( int message )
		{
			List<Item> items = this.Items;

			if ( items.Count > 0 )
			{
				PublicOverheadMessage( Network.MessageType.Regular, 0x3B2, message, "" );

				for ( int i = items.Count - 1; i >= 0; --i )
				{
					if ( i >= items.Count )
						continue;

					items[i].Delete();
				}
			}

			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;
		}

		private Timer m_Timer;

		private class ArenaTrashBarrelEmptyTimer : Timer
		{
			private ArenaTrashBarrel m_Barrel;

            public ArenaTrashBarrelEmptyTimer(ArenaTrashBarrel barrel): base(TimeSpan.FromMinutes(3.0))
			{
				m_Barrel = barrel;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				m_Barrel.Empty( 501479 ); // Emptying the trashcan!
			}
		}
	}
}