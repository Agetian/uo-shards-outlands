using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class BluePaintUpgrade : BaseBoatPaintUpgradeDeed
    {
        public override int DoubloonCost { get { return 0; } }
        public override string DisplayName { get { return "Blue"; } }
        
        [Constructable]
        public BluePaintUpgrade(): base()
        {
            Name = "a ship paint upgrade: Blue";
            BoatHue = 399;
        }

        public BluePaintUpgrade(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
    }
}