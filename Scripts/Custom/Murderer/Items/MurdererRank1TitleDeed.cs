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
	public class MurdererRank1TitleDeed : PlayerClassTitleDeed
	{
        public override int PlayerClassCurrencyValue { get { return 100; } }

        public override string Title { get { return PlayerClassPersistance.MurdererTitles[0]; } }  

		[Constructable]
        public MurdererRank1TitleDeed(): base()
		{           
			Weight = 1.0;

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
		}

        public MurdererRank1TitleDeed(Serial serial) : base(serial)
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