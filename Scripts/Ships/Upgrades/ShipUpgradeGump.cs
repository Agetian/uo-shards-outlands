using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Regions;

namespace Server
{
    public class ShipUpgradeGump : Gump
    {
        public enum UpgradeDisplayMode
        {           
            DeedUse,
            DeedAttemptInstall,
            InstalledOnShip
        }

        public PlayerMobile m_Player;
        public ShipUpgradeGumpObject m_ShipUpgradeGumpObject;        

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public ShipUpgradeGump(PlayerMobile player, ShipUpgradeGumpObject shipUpgradeGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ShipUpgradeGumpObject = shipUpgradeGumpObject;            

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipUpgradeGumpObject == null) return;

            int startX = 0;
            int startY = 0;
            
            //Background

            //Upgrade
            switch (m_ShipUpgradeGumpObject.m_UpgradeType)
            {
                case ShipUpgrades.UpgradeType.Theme:
                break;

                case ShipUpgrades.UpgradeType.Paint:
                break;

                case ShipUpgrades.UpgradeType.CannonMetal:
                break;

                case ShipUpgrades.UpgradeType.Outfitting:
                break;

                case ShipUpgrades.UpgradeType.Banner:
                break;

                case ShipUpgrades.UpgradeType.Charm:
                break;

                case ShipUpgrades.UpgradeType.MinorAbility:
                break;

                case ShipUpgrades.UpgradeType.MajorAbility:
                break;

                case ShipUpgrades.UpgradeType.EpicAbility:
                break;
            }

            switch (m_ShipUpgradeGumpObject.m_UpgradeDisplayMode)
            {
                case UpgradeDisplayMode.DeedUse: 
                break;

                case UpgradeDisplayMode.DeedAttemptInstall:
                break;

                case UpgradeDisplayMode.InstalledOnShip:
                break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Player.Backpack == null) return;
            if (m_ShipUpgradeGumpObject == null) return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Install
                case 2:
                    switch(m_ShipUpgradeGumpObject.m_UpgradeDisplayMode)
                    {
                        case UpgradeDisplayMode.DeedUse:
                            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed == null)
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Deleted)
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else if (!m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That deed is no longer accessible.");

                            else
                            {
                                m_Player.SendMessage("Which ship do you wish to install this upgrade into?");
                                m_Player.Target = new ShipUpgradeTarget(m_Player, m_ShipUpgradeGumpObject);

                                return;
                            }
                        break;

                        case UpgradeDisplayMode.DeedAttemptInstall:
                            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed == null)
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Deleted)
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (!m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That upgrade deed is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipDeed == null)
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else if (m_ShipUpgradeGumpObject.m_ShipDeed.Deleted)
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else if (!m_ShipUpgradeGumpObject.m_ShipDeed.IsChildOf(m_Player.Backpack))
                                m_Player.SendMessage("That ship token is no longer accessible.");

                            else
                            {
                                //TEST: CHECK DOUBLOONS IN BANK
                            }                            
                        break;

                        case UpgradeDisplayMode.InstalledOnShip:
                        break;
                    }

                    closeGump = false; 
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipUpgradeGump));
                m_Player.SendGump(new ShipUpgradeGump(m_Player, m_ShipUpgradeGumpObject));
            }
            else
                m_Player.SendSound(CloseGumpSound);
        }
    }

    public class ShipUpgradeGumpObject
    {
        public ShipUpgradeGump.UpgradeDisplayMode m_UpgradeDisplayMode;

        public BaseShipDeed m_ShipDeed;
        public ShipUpgradeDeed m_ShipUpgradeDeed;

        public ShipUpgrades.UpgradeType m_UpgradeType = ShipUpgrades.UpgradeType.Theme;

        public ShipUpgrades.ThemeType m_Theme = ShipUpgrades.ThemeType.None;
        public ShipUpgrades.PaintType m_Paint = ShipUpgrades.PaintType.None;
        public ShipUpgrades.CannonMetalType m_CannonMetal = ShipUpgrades.CannonMetalType.None;
        public ShipUpgrades.OutfittingType m_Outfitting = ShipUpgrades.OutfittingType.None;
        public ShipUpgrades.BannerType m_Flag = ShipUpgrades.BannerType.None;
        public ShipUpgrades.CharmType m_Charm = ShipUpgrades.CharmType.None;
        public ShipUpgrades.MinorAbilityType m_MinorAbility = ShipUpgrades.MinorAbilityType.None;
        public ShipUpgrades.MajorAbilityType m_MajorAbility = ShipUpgrades.MajorAbilityType.None;
        public ShipUpgrades.EpicAbilityType m_EpicAbility = ShipUpgrades.EpicAbilityType.None;

        public ShipUpgradeGumpObject()
        {
        }
    }

    public class ShipUpgradeTarget : Target
    {
        public PlayerMobile m_Player;
        public ShipUpgradeGumpObject m_ShipUpgradeGumpObject;

        public ShipUpgradeTarget(PlayerMobile player, ShipUpgradeGumpObject shipUpgradeGumpObject): base(1, false, TargetFlags.None, false)
        {
            m_Player = player;
            m_ShipUpgradeGumpObject = shipUpgradeGumpObject;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Player == null) return;
            if (m_Player.Backpack == null) return;
            if (m_ShipUpgradeGumpObject == null) return;

            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed == null)
            {
                m_Player.SendMessage("That upgrade deed is no longer accessible.");
                return;
            }

            if (m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.Deleted)
            {
                m_Player.SendMessage("That upgrade deed is no longer accessible.");
                return;
            }

            if (!m_ShipUpgradeGumpObject.m_ShipUpgradeDeed.IsChildOf(m_Player.Backpack))
            {
                m_Player.SendMessage("The upgrade deed you wish to use must be in your backpack.");
                return;
            }

            BaseShipDeed shipDeed = target as BaseShipDeed;

            if (shipDeed == null)
            {
                m_Player.SendMessage("You must target a ship token.");
                return;
            }

            if (shipDeed.Deleted)
            {
                m_Player.SendMessage("You must target a ship token.");
                return;
            }

            if (!shipDeed.IsChildOf(m_Player.Backpack))
            {
                m_Player.SendMessage("You must target a ship token in your backpack.");
                return;
            }

            m_ShipUpgradeGumpObject.m_ShipDeed = shipDeed;
            m_ShipUpgradeGumpObject.m_UpgradeDisplayMode = ShipUpgradeGump.UpgradeDisplayMode.DeedAttemptInstall;

            m_Player.CloseGump(typeof(ShipUpgradeGump));
            m_Player.SendGump(new ShipUpgradeGump(m_Player, m_ShipUpgradeGumpObject));
        }
    }
}