
//////////////////////////////////////////////////////////////////////
// Automatically generated by Bradley's GumpStudio and roadmaster's 
// exporter.dll,  Special thanks goes to Daegon whose work the exporter
// was based off of, and Shadow wolf for his Template Idea.
//////////////////////////////////////////////////////////////////////


using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Custom;
using Server.Mobiles;

namespace Server.Gumps
{
    public class CompanionCheck : Gump
    {
        private PlayerMobile m_Companion;
        private PlayerMobile m_Player;

        public CompanionCheck(PlayerMobile companion, PlayerMobile player)
            : base(200, 200)
        {
            m_Player = player;
            m_Companion = companion;

            Closable = false;
            Dragable = true;

            AddPage(0);
            AddBackground(50, 45, 227, 105, 0x2436);
            AddLabel(61, 54, 1153, @"This player is in a dangerous area");
            AddLabel(91, 75, 1153, @"are you prepared to assist?");
            AddButton(82, 110, 247, 248, 0, GumpButtonType.Reply, 0);
            AddButton(189, 109, 241, 243, 1, GumpButtonType.Reply, 0);
        }



        public override void OnResponse(NetState sender, RelayInfo info)
        {

            switch (info.ButtonID)
            {

                case 0:
                    m_Companion.CompanionLastLocation = m_Companion.Location;
                    m_Companion.Hidden = true;
                    m_Companion.Location = m_Player.Location;
                    m_Companion.CompanionTarget = (PlayerMobile)m_Player;
                    CommandLogging.WriteLine(m_Companion, "{0} {1} teleporting to {2} at {3}", m_Companion.AccessLevel, CommandLogging.Format(m_Companion), m_Player, new Point3D(m_Player.Location));
                    break;

                case 1:
                    m_Companion.SendMessage("You decide not to assist.");
                    break;
            }
        }
    }
}