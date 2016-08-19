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
        public PlayerMobile m_Player;
        public ShipUpgradeGumpObject m_ShipUpgradeGumpObject;

        public bool m_PreviewOnly = true;
        public int m_DesiredSlot = 0;

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


        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_ShipUpgradeGumpObject == null) return;
        }
    }

    public class ShipUpgradeGumpObject
    {
        public PlayerMobile m_Player;

        public ShipUpgradeGumpObject(PlayerMobile player)
        {
            m_Player = player;
        }
    }
}