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
    public class ExpediteRepairsMinorAbilityShipUpgrade : ShipUpgradeDeed
    {
        [Constructable]
        public ExpediteRepairsMinorAbilityShipUpgrade(): base()
        {
            Name = "expedite repairs";

            m_MinorAbilityUpgrade = ShipUpgrades.MinorAbilityType.ExpediteRepairs;

            m_UpgradeType = ShipUpgrades.UpgradeType.MinorAbility;
        }

        public ExpediteRepairsMinorAbilityShipUpgrade(Serial serial): base(serial)
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