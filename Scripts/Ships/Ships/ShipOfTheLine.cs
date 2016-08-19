using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class ShipOfTheLineShip : BaseShip
	{   
        public override int NorthID{ get{ return 0x56; } }
		public override int  EastID{ get{ return 0x57; } }
		public override int SouthID{ get{ return 0x58; } }
		public override int  WestID{ get{ return 0x59; } }

		public override int HoldDistance{ get{ return 8; } }
		public override int TillerManDistance{ get{ return -7; } }
        public override int Width { get { return 5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  3, -1 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -3, -1 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 0, 3 ); } }

        public override BaseShipDeed ShipDeed { get { return new ShipOfTheLineShipDeed(); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -8, 0));
            list.Add(new Point3D(0, -7, 0));
            list.Add(new Point3D(0, -6, 0));
            list.Add(new Point3D(-1, -5, 0)); list.Add(new Point3D(0, -5, 0)); list.Add(new Point3D(1, -5, 0));
            list.Add(new Point3D(-1, -4, 0)); list.Add(new Point3D(0, -4, 0)); list.Add(new Point3D(1, -4, 0));
            list.Add(new Point3D(-1, -3, 0)); list.Add(new Point3D(0, -3, 0)); list.Add(new Point3D(1, -3, 0));
            list.Add(new Point3D(-1, -2, 0)); list.Add(new Point3D(1, -2, 0));
            list.Add(new Point3D(-2, -1, 0)); list.Add(new Point3D(-1, -1, 0)); list.Add(new Point3D(0, -1, 0)); list.Add(new Point3D(1, -1, 0)); list.Add(new Point3D(2, -1, 0));
            list.Add(new Point3D(-2, 0, 0)); list.Add(new Point3D(-1, 0, 0)); list.Add(new Point3D(0, 0, 0)); list.Add(new Point3D(1, 0, 0)); list.Add(new Point3D(2, 0, 0));
            list.Add(new Point3D(-2, 1, 0)); list.Add(new Point3D(-1, 1, 0)); list.Add(new Point3D(0, 1, 0)); list.Add(new Point3D(1, 1, 0)); list.Add(new Point3D(2, 1, 0));
            list.Add(new Point3D(-2, 2, 0)); list.Add(new Point3D(-1, 2, 0)); list.Add(new Point3D(0, 2, 0)); list.Add(new Point3D(1, 2, 0)); list.Add(new Point3D(2, 2, 0));
            list.Add(new Point3D(-2, 3, 0)); list.Add(new Point3D(-1, 3, 0)); list.Add(new Point3D(1, 3, 0)); list.Add(new Point3D(2, 3, 0));
            list.Add(new Point3D(-2, 4, 0)); list.Add(new Point3D(-1, 4, 0)); list.Add(new Point3D(0, 4, 0)); list.Add(new Point3D(1, 4, 0)); list.Add(new Point3D(2, 4, 0));
            list.Add(new Point3D(-1, 5, 0)); list.Add(new Point3D(0, 5, 0)); list.Add(new Point3D(1, 5, 0));
            list.Add(new Point3D(-1, 6, 0)); list.Add(new Point3D(0, 6, 0)); list.Add(new Point3D(1, 6, 0));
            
            return list;
        }

        public override List<Point3D> m_MastLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(0, 3, 0));

            return list;
        }

        public override void GenerateShipCannons()
        {
            base.GenerateShipCannons();
                       
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, -2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, 2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, 3, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, 4, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-3, 5, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);

            ShipCannon.PlaceShipCannon(this, new Point3D(3, -2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, 2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, 3, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, 4, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(3, 5, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);            
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
            list.Add(new Point3D(3, -2, 0));
            list.Add(new Point3D(-3, -2, 0));
            list.Add(new Point3D(1, -4, 0));
            list.Add(new Point3D(-1, -4, 0));

            list.Add(new Point3D(3, 3, 0));
            list.Add(new Point3D(-3, 3, 0));

            return list;
        }

		[Constructable]
		public ShipOfTheLineShip()
		{
            Name = "a ship of the line";
		}

		public ShipOfTheLineShip( Serial serial ) : base( serial )
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

	public class ShipOfTheLineShipDeed : BaseShipDeed
	{
		public override BaseShip Ship{ get{ return new ShipOfTheLineShip(); } }

        public override int DoubloonCost { get { return 50000; } }
        public override double DoubloonMultiplier { get { return 10; } }

		[Constructable]
        public ShipOfTheLineShipDeed() : base(0x404C, new Point3D(0, -1, 0))
		{
            Name = "a ship of the line ship deed";
		}

		public ShipOfTheLineShipDeed( Serial serial ) : base( serial )
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