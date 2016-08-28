using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class SmokescreenMajorAbilityShipUpgrade : ShipUpgradeDeed
    {
        [Constructable]
        public SmokescreenMajorAbilityShipUpgrade(): base()
        {
            Name = "smokescreen";

            m_MajorAbilityUpgrade = ShipUpgrades.MajorAbilityType.Smokescreen;

            m_UpgradeType = ShipUpgrades.UpgradeType.MajorAbility;
        }

        public SmokescreenMajorAbilityShipUpgrade(Serial serial): base(serial)
        {
        } 

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}