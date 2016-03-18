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
	public class PaladinRank1TitleDeed : PlayerClassTitleDeed
	{
        public override int PlayerClassCurrencyValue { get { return 100; } }

        public override string Title { get { return PlayerClassPersistance.PaladinTitles[0]; } }  

		[Constructable]
        public PaladinRank1TitleDeed(): base()
		{           
			Weight = 1.0;

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;
		}

        public PaladinRank1TitleDeed(Serial serial) : base(serial)
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