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

        public int WhiteTextHue = 2499;

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

            #region Images 

            AddImage(5, 4, 103, 2499);
            AddImage(5, 64, 103, 2499);
            AddImage(140, 4, 103, 2499);
            AddImage(140, 64, 103, 2499);
            AddImage(140, 144, 103, 2499);
            AddImage(5, 144, 103, 2499);
            AddImage(15, 106, 3604, 2052);
            AddImage(143, 106, 3604, 2052);
            AddImage(15, 14, 3604, 2052);
            AddImage(143, 14, 3604, 2052);           
            AddItem(102, 121, 3816);
            AddItem(124, 136, 3808);
            AddItem(95, 103, 4455);
            AddItem(101, 145, 2322);
            AddItem(141, 139, 2322);
            AddItem(131, 121, 7681, 2415);
            AddItem(76, 145, 3898);
            AddItem(98, 156, 2581);

            #endregion

            AddLabel(99, 20, 2603, "Resurrection");
            AddLabel(25, 50, WhiteTextHue, "Do you wish to resurrect at this time?");

            AddLabel(77, 203, 63, "Accept");
            AddButton(42, 198, 9721, 9724, 1, GumpButtonType.Reply, 0);

            AddLabel(200, 203, 2101, "Decline");      
            AddButton(165, 199, 9721, 9724, 2, GumpButtonType.Reply, 0);                  
        }
        
        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            from.CloseGump(typeof(ResurrectGump));

            if (info.ButtonID == 1)
            {
                if (from.Map == null || !from.Map.CanFit(from.Location, 16, false, false))
                {
                    from.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    return;
                }                

                from.Resurrect();                

                if (from.Alive && m_HitsScalar > 0)
                    from.Hits = (int)(from.HitsMax * m_HitsScalar);
            }
        }
    }
}