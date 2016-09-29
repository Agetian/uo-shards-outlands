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

        public int WhiteTextHue = 2499;

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

        private ReportMurdererGump(Mobile victim, List<Mobile> killers, DateTime eventTime, Point3D location, Map map, int idx): base(200, 100)
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
            Closable = false;
            Resizable = false;

            #region Images 

            AddImage(5, 4, 103, 2075);
            AddImage(5, 64, 103, 2075);
            AddImage(140, 4, 103, 2075);
            AddImage(140, 64, 103, 2075);
            AddImage(140, 144, 103, 2075);
            AddImage(5, 144, 103, 2075);
            AddImage(15, 106, 3604, 2052);
            AddImage(143, 106, 3604, 2052);
            AddImage(15, 14, 3604, 2052);
            AddImage(143, 14, 3604, 2052);            
            AddItem(124, 137, 2942);
            AddItem(100, 121, 2943);
            AddItem(112, 127, 5359);
            AddItem(126, 120, 4031);
            AddItem(126, 146, 5357);
            AddItem(130, 148, 5357);

            #endregion

            AddLabel(96, 20, 2117, "Report Murder");

            AddLabel(62, 40, WhiteTextHue, "Do you wish to report the");
            AddLabel(52, 60, WhiteTextHue, "following player for murder?");

            string killerName = ((Mobile)m_Killers[m_Idx]).Name;

            AddLabel(Utility.CenteredTextOffset(140, killerName), 90, 2599, killerName);

            AddLabel(77, 203, 63, "Accept");
            AddButton(42, 198, 9721, 9724, 1, GumpButtonType.Reply, 0);

            AddLabel(200, 203, 2101, "Decline");
            AddButton(165, 199, 9721, 9724, 2, GumpButtonType.Reply, 0);    
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