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
    public class ShipGump : Gump
    {
        public enum ShipPageType
        {
            Overview,
            Upgrades,
            Stats,
            History,
            Players
        }

        public enum PlayersPageType
        {
            Friends,
            CoOwners
        }

        public PlayerMobile m_Player;
        public ShipGumpObject m_ShipGumpObject;

        public int WhiteTextHue = 2499;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public static int CloseGumpSound = 0x058;

        public static int PlayersPerPage = 10;

        public ShipGump(PlayerMobile player, ShipGumpObject shipGumpObject): base(10, 10)
        {
            m_Player = player;
            m_ShipGumpObject = shipGumpObject;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipGumpObject == null) return;

            #region Background Images

            #endregion

            switch (shipGumpObject.m_ShipPage)
            {
                #region Overview

                case ShipPageType.Overview:
                break;

                #endregion

                #region Upgrades

                case ShipPageType.Upgrades:
                break;

                #endregion

                #region Stats

                case ShipPageType.Stats:
                break;

                #endregion

                #region History

                case ShipPageType.History:
                break;

                #endregion

                #region Players

                case ShipPageType.Players:
                break;

                #endregion
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipGumpObject == null) return;

            if (m_ShipGumpObject.m_Ship == null)
            {
                m_Player.SendMessage("That ship no longer is accessible.");
                return;
            }

            if (m_ShipGumpObject.m_Ship.Deleted)
            {
                m_Player.SendMessage("That ship no longer is accessible.");
                return;
            }

            BaseShip ship = m_ShipGumpObject.m_Ship;
            BaseShipDeed deed = m_ShipGumpObject.m_ShipDeed;

            bool closeGump = true;

            #region Footer Tabs

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Overview
                case 2:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Overview;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Upgrades
                case 3:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Upgrades;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Stats
                case 4:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Stats;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //History
                case 5:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.History;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;

                //Players
                case 6:
                    m_ShipGumpObject.m_ShipPage = ShipPageType.Players;
                    m_Player.SendSound(ChangePageSound);

                    closeGump = false;
                break;
            }

            #endregion

            #region Page Content
            
            switch (m_ShipGumpObject.m_ShipPage)
            {
                #region Overview 

                case ShipPageType.Overview:
                    switch (info.ButtonID)
                    {
                        //Raise Anchor
                        case 10:
                            //Anchor Down
                            if (ship.Anchored)
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.RaiseAnchor(true);
                            }

                            //Anchor Raised
                            else
                            {
                                if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                    ship.LowerAnchor(true);
                            }

                            closeGump = false;
                        break;

                        //Embark + Disembark
                        case 11:
                            //Disembark
                            if (ship.Contains(m_Player))
                            {
                                if (ship.CanUseCommand(m_Player, false, false, ShipAccessLevelType.None))
                                {
                                    ship.Disembark(m_Player);
                                    return;
                                }                               
                            }

                            //Embark
                            else
                            {
                                if (ship.CanUseCommand(m_Player, false, true, ShipAccessLevelType.Friend))
                                {
                                    ship.Embark(m_Player, false);
                                    return;
                                }                                
                            }

                            closeGump = false;
                        break;

                        //Embark + Disembark Followers
                        case 12:
                            bool followersOnBoard = false;

                            foreach (Mobile follower in m_Player.AllFollowers)
                            {
                                if (ship.Contains(follower))
                                    followersOnBoard = true;
                            }

                            //Disembark Followers
                            if (followersOnBoard)
                            {
                                if (ship.CanUseCommand(m_Player, false, false, ShipAccessLevelType.None))
                                {
                                    ship.DisembarkFollowers(m_Player);
                                    return;
                                }
                            }

                            //Embark Followers
                            else
                            {
                                if (ship.CanUseCommand(m_Player, false, true, ShipAccessLevelType.Friend))
                                {
                                    ship.EmbarkFollowers(m_Player);
                                    return;
                                }
                            }

                            closeGump = false;
                        break;

                        //Clear Deck
                        case 13:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                ship.ClearTheDeck(true);

                            closeGump = false;
                        break;

                        //Throw Overboard
                        case 14:
                            if (ship.CanUseCommand(m_Player, true, false, ShipAccessLevelType.None))
                            {
                                ship.ThrowOverboardCommand(m_Player);
                                return;
                            }

                            closeGump = false;
                        break;

                        //Dock + Place Ship
                        case 15:
                            //Dock Ship
                            if (ship != null)
                            {
                                if (ship.CanUseCommand(m_Player, false, true, ShipAccessLevelType.CoOwner))
                                {
                                    ship.BeginDryDock(m_Player);
                                    return;
                                }
                            }

                            //Place Ship
                            else if (deed != null)
                            {
                                if (m_Player.Backpack != null)
                                {
                                    if (!deed.IsChildOf(m_Player.Backpack))
                                        m_Player.SendMessage("This ship deed must be in your backpack if you wish to use it.");

                                    else if (!m_Player.Alive)
                                        m_Player.SendMessage("You are dead and cannot use that.");

                                    else
                                    {
                                        m_Player.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
                                        m_Player.Target = new InternalTarget(deed);

                                        return;
                                    }
                                }
                            }

                            closeGump = false;
                        break;

                        //Minor Ability
                        case 16:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))                    
                                ship.ActivateMinorAbility(m_Player);                            

                            closeGump = false;
                        break;

                        //Major Ability
                        case 17:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                ship.ActivateMajorAbility(m_Player);   

                            closeGump = false;
                        break;

                        //Epic Ability
                        case 18:
                            if (ship.CanUseCommand(m_Player, true, true, ShipAccessLevelType.CoOwner))
                                ship.ActivateEpicAbility(m_Player);   

                            closeGump = false;
                        break;

                        //Launch Ship Hotbar
                        case 19:
                            ShipHotbarGumpObject shipHotbarGumpObject = new ShipHotbarGumpObject();

                            m_Player.CloseGump(typeof(ShipHotbarGump));
                            m_Player.SendGump(new ShipHotbarGump(m_Player, shipHotbarGumpObject));
                        break;

                        //Rename Ship
                        case 20:
                        break;
                    }
                break;

                #endregion

                #region Upgrades

                case ShipPageType.Upgrades:
                    switch (info.ButtonID)
                    {
                        //Theme
                        case 10:
                        break;

                        //Paint
                        case 11:
                        break;

                        //Cannon Metal
                        case 12:
                        break;

                        //Outfitting 
                        case 13:
                        break;

                        //Flag
                        case 14:
                        break;

                        //Charm
                        case 15:
                        break;

                        //Minor Ability
                        case 16:
                        break;

                        //Major Ability
                        case 17:
                        break;

                        //Epic Ability
                        case 18:
                        break;
                    }
                break;

                #endregion

                #region Stats

                case ShipPageType.Stats:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 10:
                        break;

                        //Next Page
                        case 11:
                        break;
                    }
                break;

                #endregion

                #region History

                case ShipPageType.History:
                    switch (info.ButtonID)
                    {
                        //Previous Page
                        case 10:
                        break;

                        //Next Page
                        case 11:
                        break;
                    }
                break;

                #endregion

                #region Players

                case ShipPageType.Players:
                    switch (info.ButtonID)
                    {
                        //Previous Player Type
                        case 10:
                        break;

                        //Next Player Type
                        case 11:
                        break;

                        //Previous Page
                        case 12:
                        break;

                        //Next Page
                        case 13:
                        break;

                        //Set All on IP
                        case 14:
                        break;

                        //Set All in Guild
                        case 15:
                        break;

                        //Remove All
                        case 16:
                        break;
                    }

                    if (info.ButtonID >= 20 && info.ButtonID < 30)
                    {
                        int playerIndex = (info.ButtonID - 20) + (m_ShipGumpObject.m_Page * ShipGump.PlayersPerPage);
                    }
                break;

                #endregion
            }

            #endregion

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(ShipGump));
                m_Player.SendGump(new ShipGump(m_Player, m_ShipGumpObject));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }

        #region Ship Placement

        private class InternalTarget : MultiTarget
        {
            private BaseShipDeed m_ShipDeed;

            public InternalTarget(BaseShipDeed deed): base(deed.MultiID, deed.Offset)
            {
                m_ShipDeed = deed;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null) return;
                if (player.Backpack == null) return;

                if (m_ShipDeed == null)
                {
                    player.SendMessage("That ship deed is no longer accessible.");
                    return;
                }

                if (m_ShipDeed.Deleted)
                {
                    player.SendMessage("That ship deed is no longer accessible.");
                    return;
                }

                if (m_ShipDeed.IsChildOf(player.Backpack))
                {
                    player.SendMessage("That ship deed is no longer accessible.");
                    return;
                }

                IPoint3D ip = o as IPoint3D;

                if (ip != null)
                {
                    if (ip is Item)
                        ip = ((Item)ip).GetWorldTop();

                    Point3D p = new Point3D(ip);
                   
                    m_ShipDeed.OnPlacement(player, p);
                }
            }
        }

        #endregion
    }

    public class ShipGumpObject
    {
        public PlayerMobile m_Player;
        public BaseShip m_Ship;
        public BaseShipDeed m_ShipDeed;

        public ShipGump.ShipPageType m_ShipPage = ShipGump.ShipPageType.Overview;
        public ShipGump.PlayersPageType m_PlayersPage = ShipGump.PlayersPageType.Friends;
        public int m_Page = 0;

        public int m_OverviewMinorAbilitySelected = 0;
        public int m_OverviewMajorAbilitySelected = 0;

        public ShipGumpObject(PlayerMobile player, BaseShip ship, BaseShipDeed deed)
        {
            m_Player = player;
            m_Ship = ship;
            m_ShipDeed = deed;
        }
    }
}