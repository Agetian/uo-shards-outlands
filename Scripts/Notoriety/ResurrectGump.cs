using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;


namespace Server.Gumps
{
    public enum ResurrectMessage
    {
        ChaosShrine = 0,
        VirtueShrine = 1,
        Healer = 2,
        Generic = 3,
    }

    public class ResurrectGump : Gump
    {
        private Mobile m_Healer;
        private int m_Price;
        private bool m_FromSacrifice;
        private bool m_FromChaos = false;
        private double m_HitsScalar;

        public ResurrectGump(Mobile owner): this(owner, owner, ResurrectMessage.Generic, false)
        {
        }

        public ResurrectGump(Mobile owner, double hitsScalar): this(owner, owner, ResurrectMessage.Generic, false, hitsScalar)
        {
        }

        public ResurrectGump(Mobile owner, bool fromSacrifice): this(owner, owner, ResurrectMessage.Generic, fromSacrifice)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer): this(owner, healer, ResurrectMessage.Generic, false)
        {
        }

        public ResurrectGump(Mobile owner, ResurrectMessage msg): this(owner, owner, msg, false)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer, ResurrectMessage msg): this(owner, healer, msg, false)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice): this(owner, healer, msg, fromSacrifice, 0.0)
        {
        }

        public ResurrectGump(Mobile owner, Mobile healer, ResurrectMessage msg, bool fromSacrifice, double hitsScalar): base(100, 0)
        {

            if (msg == 0)
                m_FromChaos = true;

            PlayerMobile pm = (PlayerMobile)owner;

            if (owner == null || healer == null)
                return;

            m_Healer = healer;
            m_FromSacrifice = fromSacrifice;
            m_HitsScalar = hitsScalar;

            AddPage(0);

            AddBackground(0, 0, 400, 350, 2600);

            AddHtmlLocalized(0, 20, 400, 35, 1011022, false, false); // <center>Resurrection</center>            
            AddHtml(50, 55, 300, 140, /*1011023 + */"It is possible for you to be resurrected here by this healer. Do you wish to try?<br>CONTINUE - You chose to try to come back to life now.<br>CANCEL - You prefer to remain a ghost for now.", true, true);
            //AddHtmlLocalized(50, 55, 300, 140, 1011023 + (int)msg, true, true);

            AddButton(65, 227, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(100, 230, 110, 35, 1011011, false, false); // CONTINUE          

            AddButton(200, 227, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(235, 230, 110, 35, 1011012, false, false); // CANCEL              
        }
        
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            from.CloseGump(typeof(ResurrectGump));

            if (info.ButtonID == 1 || info.ButtonID == 2)
            {
                if (from.Map == null || !from.Map.CanFit(from.Location, 16, false, false))
                {
                    from.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    return;
                }

                if (m_Price > 0)
                {
                    if (info.IsSwitched(1))
                    {
                        if (Banker.Withdraw(from, m_Price))
                        {
                            from.SendLocalizedMessage(1060398, m_Price.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                            from.SendLocalizedMessage(1060022, Banker.GetBalance(from).ToString()); // You have ~1_AMOUNT~ gold in cash remaining in your bank box.
                        }

                        else
                        {
                            from.SendLocalizedMessage(1060020); // Unfortunately, you do not have enough cash in your bank to cover the cost of the healing.
                            return;
                        }
                    }

                    else
                    {
                        from.SendLocalizedMessage(1060019); // You decide against paying the healer, and thus remain dead.
                        return;
                    }
                }

                from.PlaySound(0x214);
                from.FixedEffect(0x376A, 10, 16);

                from.Resurrect();

                if (from.Fame > 0)
                {
                    int amount = from.Fame / 10;

                    Misc.FameKarmaTitles.AwardFame(from, -amount, true);
                }

                if (from.Alive && m_HitsScalar > 0)
                    from.Hits = (int)(from.HitsMax * m_HitsScalar);
            }
        }
    }
}