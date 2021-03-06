using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class LargeDragonShip : BaseShip
	{   
        public override int NorthID { get { return 0x14; } }
        public override int EastID { get { return 0x15; } }
        public override int SouthID { get { return 0x16; } }
        public override int WestID { get { return 0x17; } }

		public override int HoldDistance{ get{ return 5; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, -1 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, -1 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 0, 3 ); } }

        public override Type ShipDeedType { get { return typeof(LargeDragonShipDeed); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -4, 0));
            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(-1, -2, 0)); list.Add(new Point3D(0, -2, 0)); list.Add(new Point3D(1, -2, 0));
            list.Add(new Point3D(-1, -1, 0));                                  list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, 0, 0));  list.Add(new Point3D(0, 0, 0));  list.Add(new Point3D(1, 0, 0));
            list.Add(new Point3D(-1, 1, 0));  list.Add(new Point3D(0, 1, 0));  list.Add(new Point3D(1, 1, 0));
            list.Add(new Point3D(-1, 2, 0));  list.Add(new Point3D(0, 2, 0));  list.Add(new Point3D(1, 2, 0));
            list.Add(new Point3D(0, 3, 0));
            list.Add(new Point3D(0, 4, 0));

            return list;
        }

        public override List<Point3D> m_MastLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -1, 0));

            return list;
        }

        public override void GenerateShipCannons()
        {
            base.GenerateShipCannons();

            ShipCannon.PlaceShipCannon(this, new Point3D(-2, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 3, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);

            ShipCannon.PlaceShipCannon(this, new Point3D(2, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 3, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
        }

        public override List<Point3D> m_ShipFireLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 3, 0));
            list.Add(new Point3D(-2, 0, 0));
            list.Add(new Point3D(2, 1, 0));
            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(-1, 1, 0));
            list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, -1, 0));
            list.Add(new Point3D(1, 2, 0));
            list.Add(new Point3D(1, 4, 0));
            list.Add(new Point3D(-1, 4, 0));

            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(-2, 2, 0));

            return list;
        }

		[Constructable]
		public LargeDragonShip()
		{
		}

		public LargeDragonShip( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}

	public class LargeDragonShipDeed : BaseShipDeed
	{
        public override Type ShipType { get { return typeof(LargeDragonShip); } }
		private static int m_ShipId = 0x4014;

		[Constructable]
		public LargeDragonShipDeed() : base( m_ShipId, new Point3D( 0, -1, 0 ) )
		{
            Name = "a large dragon ship";
		}

		public LargeDragonShipDeed( Serial serial ) : base( serial )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}
}