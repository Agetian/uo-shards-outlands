using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Accounting;

namespace Server.Gumps
{
    public class ReportMurdererGump : Gump
    {
        private int m_Idx;

        private Mobile m_Victim;
        private List<Mobile> m_Killers;
        private DateTime m_EventTime;
        private Point3D m_Location; 
        private Map m_Map;

        public class GumpTimer : Timer
        {
            private int m_Idx;

            private Mobile m_Victim;
            private List<Mobile> m_Killers;
            private DateTime m_EventTime;
            private Point3D m_Location;
            private Map m_Map;

            public GumpTimer(Mobile victim, List<Mobile> killers, DateTime eventTime, Point3D location, Map map): base(TimeSpan.FromSeconds(2.0))
            {
                m_Victim = victim;
                m_Killers = killers;
                m_EventTime = eventTime;
                m_Location = location;  
                m_Map = map;
            }

            protected override void OnTick()
            {
                m_Victim.SendGump(new ReportMurdererGump(m_Victim, m_Killers, m_EventTime, m_Location, m_Map));
            }
        }

        public ReportMurdererGump(Mobile victim, List<Mobile> killers, DateTime eventTime, Point3D location, Map map): this(victim, killers, eventTime, location, map, 0)
        {
        }

        private ReportMurdererGump(Mobile victim, List<Mobile> killers, DateTime eventTime, Point3D location, Map map, int idx): base(0, 0)
        {
            m_Victim = victim;           
            m_Killers = killers;
            m_EventTime = eventTime;
            m_Location = location;    
            m_Map = map;

            m_Idx = idx;

            BuildGump();
        }

        private void BuildGump()
        {
            AddBackground(265, 205, 320, 290, 5054);
            Closable = false;
            Resizable = false;

            AddPage(0);

            AddImageTiled(225, 175, 50, 45, 0xCE);   //Top left corner
            AddImageTiled(267, 175, 315, 44, 0xC9);  //Top bar
            AddImageTiled(582, 175, 43, 45, 0xCF);   //Top right corner
            AddImageTiled(225, 219, 44, 270, 0xCA);  //Left side
            AddImageTiled(582, 219, 44, 270, 0xCB);  //Right side
            AddImageTiled(225, 489, 44, 43, 0xCC);   //Lower left corner
            AddImageTiled(267, 489, 315, 43, 0xE9);  //Lower Bar
            AddImageTiled(582, 489, 43, 43, 0xCD);   //Lower right corner

            AddPage(1);

            AddHtml(260, 234, 300, 140, ((Mobile)m_Killers[m_Idx]).Name, false, false); // Player's Name
            AddHtmlLocalized(260, 254, 300, 140, 1049066, false, false); // Would you like to report...

            AddButton(260, 300, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(300, 300, 300, 50, 1046362, false, false); // Yes

            AddButton(360, 300, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(400, 300, 300, 50, 1046363, false, false); // No      
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                //Report Murder
                case 1:
                    {
                        int killerCount = m_Killers.Count;

                        PlayerMobile pm_Killer = m_Killers[m_Idx] as PlayerMobile;
                        PlayerMobile pm_Victim = from as PlayerMobile;

                        if (pm_Killer != null)
                        {
                            pm_Killer.MurderCounts++;
                            pm_Killer.m_LifetimeMurderCounts++;

                            pm_Killer.ResetKillTime();
                            pm_Killer.SendLocalizedMessage(1049067); //You have been reported for murder!

                            if (pm_Killer.MurderCounts == Mobile.MurderCountsRequiredForMurderer)                            
                                pm_Killer.SendLocalizedMessage(502134); //You are now known as a murderer!

                            if (pm_Killer.Young)
                            {
                                Account account = pm_Killer.Account as Account;

                                if (account != null)
                                    account.RemoveYoungStatus(0);
                            }
                        }

                        break;
                    }

                //Ignore The Murder
                case 2:
                {
                    break;
                }
            }

            m_Idx++;

            if (m_Idx < m_Killers.Count)
                from.SendGump(new ReportMurdererGump(from, m_Killers, m_EventTime, m_Location, m_Map, m_Idx));
        }
    }
}