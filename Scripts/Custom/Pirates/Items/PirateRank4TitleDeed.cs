using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class PirateRank4TitleDeed : PlayerClassTitleDeed
	{
        public override int PlayerClassCurrencyValue { get { return 10000; } }

        public override string Title { get { return PlayerClassPersistance.PirateTitles[3]; } }  

		[Constructable]
        public PirateRank4TitleDeed(): base()
		{           
			Weight = 1.0;

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
		}

        public PirateRank4TitleDeed(Serial serial) : base(serial)
		{
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0); //Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}