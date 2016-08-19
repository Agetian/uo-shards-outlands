using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class MediumDragonShip : BaseShip
	{
        public override int NorthID { get { return 0xC; } }
        public override int EastID { get { return 0xD; } }
        public override int SouthID { get { return 0xE; } }
        public override int WestID { get { return 0xF; } }

		public override int HoldDistance{ get{ return 4; } }
		public override int TillerManDistance{ get{ return -5; } }

		public override Point2D StarboardOffset{ get{ return new Point2D(  2, 0 ); } }
		public override Point2D      PortOffset{ get{ return new Point2D( -2, 0 ); } }

		public override Point3D MarkOffset{ get{ return new Point3D( 0, 1, 3 ); } }

        public override BaseShipDeed ShipDeed { get { return new MediumDragonShipDeed(); } }

        public override List<Point3D> m_EmbarkLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, -3, 0));
            list.Add(new Point3D(0, -2, 0));
            list.Add(new Point3D(-1, -1, 0)); list.Add(new Point3D(0, -1, 0)); list.Add(new Point3D(1, -1, 0));
            list.Add(new Point3D(-1, 0, 0)); list.Add(new Point3D(1, 0, 0));
            list.Add(new Point3D(-1, 1, 0)); list.Add(new Point3D(0, 1, 0)); list.Add(new Point3D(1, 1, 0));
            list.Add(new Point3D(-1, 2, 0)); list.Add(new Point3D(0, 2, 0)); list.Add(new Point3D(1, 2, 0));
            list.Add(new Point3D(0, 3, 0));
            list.Add(new Point3D(0, 4, 0));

            return list;
        }

        public override List<Point3D> m_MastLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 0, 0));

            return list;
        }

        public override void GenerateShipCannons()
        {
            base.GenerateShipCannons();

            ShipCannon.PlaceShipCannon(this, new Point3D(-2, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);
            ShipCannon.PlaceShipCannon(this, new Point3D(-2, 2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Left);

            ShipCannon.PlaceShipCannon(this, new Point3D(2, -1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 0, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 1, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
            ShipCannon.PlaceShipCannon(this, new Point3D(2, 2, 0), ShipCannon.CannonType.Small, ShipCannon.CannonPosition.Right);
        }

        public override List<Point3D> m_ShipFireLocations()
        {
            List<Point3D> list = new List<Point3D>();

            list.Add(new Point3D(0, 3, 2));
            list.Add(new Point3D(-2, 0, 2));
            list.Add(new Point3D(2, 1, 2));
            list.Add(new Point3D(0, -3, 2));
            list.Add(new Point3D(-1, 1, 2));
            list.Add(new Point3D(1, -1, 2));
            list.Add(new Point3D(-1, -1, 2));
            list.Add(new Point3D(1, 2, 2));

            list.Add(new Point3D(1, 4, 2));
            list.Add(new Point3D(-1, 4, 2));

            return list;
        }

		[Constructable]
		public MediumDragonShip()
		{
		}

		public MediumDragonShip( Serial serial ) : base( serial )
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

	public class MediumDragonShipDeed : BaseShipDeed
	{
		public override BaseShip Ship{ get{ return new MediumDragonShip(); } }
		private static int m_ShipId = 0x400c;

        public override int DoubloonCost { get { return 125; } }
        public override double DoubloonMultiplier { get { return 2; } }

		[Constructable]
		public MediumDragonShipDeed() : base( m_ShipId, Point3D.Zero )
		{
            Name = "a medium dragon ship deed";
		}

		public MediumDragonShipDeed( Serial serial ) : base( serial )
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