using System;
using Server;

using Server.Gumps;

namespace Server.Items
{
	public class ChaosShield : BaseShield
	{
		public override int InitMinHits{ get{ return 125; } }
		public override int InitMaxHits{ get{ return 125; } }

		public override int ArmorBase{ get{ return 24; } }
        public override int OldDexBonus { get { return -7; } }

        public override int IconItemId { get { return 7107; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 50; } }
        public override int IconOffsetY { get { return 41; } }

		[Constructable]
		public ChaosShield() : base( 7107 )
		{
            Name = "chaos shield";
			Weight = 5.0;
		}

		public ChaosShield( Serial serial ) : base(serial)
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

			writer.Write( (int)0 );//version
		}

		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Validate( Parent as Mobile ) )
				base.OnSingleClick( from );
		}

        public virtual bool Validate(Mobile m)
        {
            return true;
        }
    }
}