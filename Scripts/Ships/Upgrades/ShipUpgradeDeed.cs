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
        public ShipUpgrades.BannerType m_Banner = ShipUpgrades.BannerType.None;
        public ShipUpgrades.CharmType m_Charm = ShipUpgrades.CharmType.None;
        public ShipUpgrades.MinorAbilityType m_MinorAbility = ShipUpgrades.MinorAbilityType.None;
        public ShipUpgrades.MajorAbilityType m_MajorAbility = ShipUpgrades.MajorAbilityType.None;
        public ShipUpgrades.EpicAbilityType m_EpicAbility = ShipUpgrades.EpicAbilityType.None;

        [Constructable]
        public ShipUpgradeDeed(): base(5362)
        {
            Name = "ship upgrade deed";
        }

        public ShipUpgradeDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            switch (m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme: LabelTo(from, "(ship theme upgrade)"); break;
                case ShipUpgrades.UpgradeType.Paint: LabelTo(from, "(ship paint upgrade)"); break;
                case ShipUpgrades.UpgradeType.CannonMetal: LabelTo(from, "(ship cannon metal upgrade)"); break;

                case ShipUpgrades.UpgradeType.Outfitting: LabelTo(from, "(ship outfitting upgrade)"); break;
                case ShipUpgrades.UpgradeType.Banner: LabelTo(from, "(ship banner upgrade)"); break;
                case ShipUpgrades.UpgradeType.Charm: LabelTo(from, "(ship charm upgrade)"); break;

                case ShipUpgrades.UpgradeType.MinorAbility: LabelTo(from, "(ship minor ability upgrade)"); break;
                case ShipUpgrades.UpgradeType.MajorAbility: LabelTo(from, "(ship major ability upgrade)"); break;
                case ShipUpgrades.UpgradeType.EpicAbility: LabelTo(from, "(ship epic ability upgrade)"); break;
            }
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
                case ShipUpgrades.UpgradeType.Banner: shipUpgradeGumpObject.m_Banner = m_Banner; break;
                case ShipUpgrades.UpgradeType.Charm: shipUpgradeGumpObject.m_Charm = m_Charm; break;

                case ShipUpgrades.UpgradeType.MinorAbility: shipUpgradeGumpObject.m_MinorAbility = m_MinorAbility; break;
                case ShipUpgrades.UpgradeType.MajorAbility: shipUpgradeGumpObject.m_MajorAbility = m_MajorAbility; break;
                case ShipUpgrades.UpgradeType.EpicAbility: shipUpgradeGumpObject.m_EpicAbility = m_EpicAbility; break;
            }            

            player.CloseGump(typeof(ShipUpgradeGump));
            player.SendGump(new ShipUpgradeGump(player, shipUpgradeGumpObject));
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
            writer.Write((int)m_Banner);
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
                m_Banner = (ShipUpgrades.BannerType)reader.ReadInt();
                m_Charm = (ShipUpgrades.CharmType)reader.ReadInt();

                m_MinorAbility = (ShipUpgrades.MinorAbilityType)reader.ReadInt();
                m_MajorAbility = (ShipUpgrades.MajorAbilityType)reader.ReadInt();
                m_EpicAbility = (ShipUpgrades.EpicAbilityType)reader.ReadInt();
            }
        }
    }
}