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
    public class GrapeshotAmmunitionEpicAbilityUpgradeDeed : BaseBoatEpicAbilityUpgradeDeed
    {
        public override string DisplayName { get { return "Grapeshot Ammunition"; } }

        public override EpicAbilityType EpicAbility { get { return EpicAbilityType.GrapeshotAmmunition; } }

        [Constructable]
        public GrapeshotAmmunitionEpicAbilityUpgradeDeed(): base()
        {
            Name = "a ship epic ability upgrade: Grapeshot Ammunition";
        }

        public GrapeshotAmmunitionEpicAbilityUpgradeDeed(Serial serial): base(serial)
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