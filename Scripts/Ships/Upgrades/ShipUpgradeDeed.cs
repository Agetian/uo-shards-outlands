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
    public class ShipUpgradeDeed : Item
    {
        public ShipUpgrades.UpgradeType m_UpgradeType = ShipUpgrades.UpgradeType.Theme;

        public ShipUpgrades.ThemeType m_Theme = ShipUpgrades.ThemeType.None;
        public ShipUpgrades.PaintType m_Paint = ShipUpgrades.PaintType.None;
        public ShipUpgrades.CannonMetalType m_CannonMetal = ShipUpgrades.CannonMetalType.None;
        public ShipUpgrades.OutfittingType m_Outfitting = ShipUpgrades.OutfittingType.None;
        public ShipUpgrades.FlagType m_Flag = ShipUpgrades.FlagType.None;
        public ShipUpgrades.CharmType m_Charm = ShipUpgrades.CharmType.None;
        public ShipUpgrades.MinorAbilityType m_MinorAbility = ShipUpgrades.MinorAbilityType.None;
        public ShipUpgrades.MajorAbilityType m_MajorAbility = ShipUpgrades.MajorAbilityType.None;
        public ShipUpgrades.EpicAbilityType m_EpicAbility = ShipUpgrades.EpicAbilityType.None;

        [Constructable]
        public ShipUpgradeDeed(): base(5357)
        {
            Name = "ship upgrade deed";
        }

        public ShipUpgradeDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null) return;
            if (player.Backpack == null) return;

            if (!IsChildOf(player.Backpack))
            {
                player.SendMessage("That must be in your backpack if you wish to use it.");
                return;
            }

            ShipUpgradeGumpObject shipUpgradeGumpObject = new ShipUpgradeGumpObject();

            shipUpgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.DeedUse;
            shipUpgradeGumpObject.m_ShipUpgradeDeed = this;

            shipUpgradeGumpObject.m_UpgradeType = m_UpgradeType;

            switch(m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme: shipUpgradeGumpObject.m_Theme = m_Theme; break;
                case ShipUpgrades.UpgradeType.Paint: shipUpgradeGumpObject.m_Paint = m_Paint; break;
                case ShipUpgrades.UpgradeType.CannonMetal: shipUpgradeGumpObject.m_CannonMetal = m_CannonMetal; break;

                case ShipUpgrades.UpgradeType.Outfitting: shipUpgradeGumpObject.m_Outfitting = m_Outfitting; break;
                case ShipUpgrades.UpgradeType.Flag: shipUpgradeGumpObject.m_Flag = m_Flag; break;
                case ShipUpgrades.UpgradeType.Charm: shipUpgradeGumpObject.m_Charm = m_Charm; break;

                case ShipUpgrades.UpgradeType.MinorAbility: shipUpgradeGumpObject.m_MinorAbility = m_MinorAbility; break;
                case ShipUpgrades.UpgradeType.MajorAbility: shipUpgradeGumpObject.m_MajorAbility = m_MajorAbility; break;
                case ShipUpgrades.UpgradeType.EpicAbility: shipUpgradeGumpObject.m_EpicAbility = m_EpicAbility; break;
            }            

            player.CloseGump(typeof(ShipUpgradeGump));
            player.SendGump(new ShipUpgradeGump(player, shipUpgradeGumpObject));
        }

        public static int GetDoubloonCost(ShipUpgrades.UpgradeType upgradeType)
        {
            int doubloonCost = 250;

            switch (upgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.Paint: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.CannonMetal: doubloonCost = 1000; break;

                case ShipUpgrades.UpgradeType.Outfitting: doubloonCost = 2000; break;
                case ShipUpgrades.UpgradeType.Flag: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.Charm: doubloonCost = 1000; break;

                case ShipUpgrades.UpgradeType.MinorAbility: doubloonCost = 500; break;
                case ShipUpgrades.UpgradeType.MajorAbility: doubloonCost = 1000; break;
                case ShipUpgrades.UpgradeType.EpicAbility: doubloonCost = 2000; break;
            }

            return doubloonCost;
        }

        public static double GetDoubloonCostModifier(Type shipType)
        {
            double modifier = 1.0;

            if (shipType == typeof(SmallShip) || shipType == typeof(SmallShipDeed) || shipType == typeof(SmallDragonShip) || shipType == typeof(SmallDragonShipDeed))
                return 1.0;

            if (shipType == typeof(MediumShip) || shipType == typeof(MediumShipDeed) || shipType == typeof(MediumDragonShip) || shipType == typeof(MediumDragonShipDeed))
                return 1.5;

            if (shipType == typeof(LargeShip) || shipType == typeof(LargeShipDeed) || shipType == typeof(LargeDragonShip) || shipType == typeof(LargeDragonShipDeed))
                return 2.0;

            if (shipType == typeof(Carrack) || shipType == typeof(CarrackShipDeed))
                return 3.0;

            if (shipType == typeof(Galleon) || shipType == typeof(GalleonShipDeed))
                return 4.0;

            if (shipType == typeof(ShipOfTheLineShip) || shipType == typeof(ShipOfTheLineShipDeed))
                return 5.0;

            return modifier;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  
       
            //Version 0
            writer.Write((int)m_UpgradeType);

            writer.Write((int)m_Theme);
            writer.Write((int)m_Paint);
            writer.Write((int)m_CannonMetal);
            writer.Write((int)m_Outfitting);
            writer.Write((int)m_Flag);
            writer.Write((int)m_Charm);
            writer.Write((int)m_MinorAbility);
            writer.Write((int)m_MajorAbility);
            writer.Write((int)m_EpicAbility);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_UpgradeType = (ShipUpgrades.UpgradeType)reader.ReadInt();

                m_Theme = (ShipUpgrades.ThemeType)reader.ReadInt();
                m_Paint = (ShipUpgrades.PaintType)reader.ReadInt();
                m_CannonMetal = (ShipUpgrades.CannonMetalType)reader.ReadInt();

                m_Outfitting = (ShipUpgrades.OutfittingType)reader.ReadInt();
                m_Flag = (ShipUpgrades.FlagType)reader.ReadInt();
                m_Charm = (ShipUpgrades.CharmType)reader.ReadInt();

                m_MinorAbility = (ShipUpgrades.MinorAbilityType)reader.ReadInt();
                m_MajorAbility = (ShipUpgrades.MajorAbilityType)reader.ReadInt();
                m_EpicAbility = (ShipUpgrades.EpicAbilityType)reader.ReadInt();
            }
        }
    }
}