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
    public class CorsairsBannerShipUpgrade : ShipUpgradeDeed
    {
        [Constructable]
        public CorsairsBannerShipUpgrade(): base()
        {
            Name = "corsairs";            

            m_BannerUpgrade = ShipUpgrades.BannerType.Corsairs;

            m_UpgradeType = ShipUpgrades.UpgradeType.Banner;
        }

        public CorsairsBannerShipUpgrade(Serial serial): base(serial)
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