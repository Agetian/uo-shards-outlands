using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class CarrackFishingBoat : CarrackBoat
	{
		[Constructable]
        public CarrackFishingBoat()
		{
		}

        public CarrackFishingBoat(Serial serial): base(serial)
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