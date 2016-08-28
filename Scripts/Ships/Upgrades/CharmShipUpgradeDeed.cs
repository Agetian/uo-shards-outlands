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
    public class BarrelOfLimesCharmShipUpgrade : ShipUpgradeDeed
    {
        [Constructable]
        public BarrelOfLimesCharmShipUpgrade(): base()
        {
            Name = "barrel of limes";

            m_CharmUpgrade = ShipUpgrades.CharmType.BarrelOfLimes;

            m_UpgradeType = ShipUpgrades.UpgradeType.Charm;
        }

        public BarrelOfLimesCharmShipUpgrade(Serial serial): base(serial)
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