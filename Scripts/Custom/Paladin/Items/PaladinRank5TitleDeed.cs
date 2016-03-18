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
	public class PaladinRank5TitleDeed : PlayerClassTitleDeed
	{
        public override int PlayerClassCurrencyValue { get { return 50000; } }

        public override string Title { get { return PlayerClassPersistance.PaladinTitles[4]; } }  

		[Constructable]
        public PaladinRank5TitleDeed(): base()
		{           
			Weight = 1.0;

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;
		}

        public PaladinRank5TitleDeed(Serial serial) : base(serial)
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