using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Custom;

namespace Server
{
    public class ConsiderSinsGump : Gump
    {
        public PlayerMobile m_Player;

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;

        public int WhiteTextHue = 2499;

        public ConsiderSinsGump(PlayerMobile player): base(10, 10)
        {
            m_Player = player;

            if (m_Player == null) 
                return;

            #region Background

            AddImage(16, 12, 103, 2499);
            AddImage(151, 12, 103, 2499);
            AddImage(277, 12, 103, 2499);
            AddImage(152, 99, 103, 2499);
            AddImage(18, 99, 103, 2499);
            AddImage(279, 99, 103, 2499);
            AddImage(151, 187, 103, 2499);
            AddImage(16, 187, 103, 2499);
            AddImage(277, 187, 103, 2499);
            AddImage(151, 271, 103, 2499);
            AddImage(16, 271, 103, 2499);
            AddImage(277, 271, 103, 2499);
            AddBackground(31, 30, 377, 329, 5120);
            AddBackground(16, 24, 403, 335, 9380);            
            AddItem(84, 52, 7574);                             
            AddImage(83, 25, 1142);

            #endregion

            AddLabel(162, 26, WhiteTextHue, "Criminal Summary");
            
            //Guide
            AddButton(10, 13, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(6, 1, 149, "Guide");

            int startY = 65;
            int rowSpacing = 30;

            AddLabel(128, startY, 149, "Current Murder Counts:");            
            if (m_Player.MurderCounts >= Mobile.MurderCountsRequiredForMurderer)
                AddLabel(287, startY, 2116, m_Player.MurderCounts.ToString());
            else
                AddLabel(287, startY, WhiteTextHue, m_Player.MurderCounts.ToString());

            startY += rowSpacing;
            
            if (m_Player.MurderCounts > 0)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + (m_Player.m_MurderCountDecayTimeRemaining - m_Player.GameTime), true, true, true, true, false);

                AddItem(57, startY - 2, 6169);
                AddLabel(96, startY, 149, "Next Murder Count Decays in:");                 
                AddLabel(288, startY, WhiteTextHue, timeRemaining);

                startY += rowSpacing;
            }

            AddLabel(125, startY, 149, "Lifetime Murder Counts:");
            AddLabel(287, startY, WhiteTextHue, m_Player.m_LifetimeMurderCounts.ToString());

            AddItem(82, startY + 2, 6884);
            AddItem(89, startY + 2, 6884);
            AddItem(84, startY + 7, 6884);   

            startY += rowSpacing;
            startY += rowSpacing;

            rowSpacing = 25;

            //Ress Penalties
            if (m_Player.m_RessPenaltyExpiration > DateTime.UtcNow)
            {
                AddLabel(117, startY, 2599, "Current Resurrection Penalties");
                startY += rowSpacing;

                if (m_Player.m_RessPenaltyEffectivenessReductionCount > 0)
                {
                    double damagePenalty = PlayerMobile.RessPenaltyDamageScalar * m_Player.m_RessPenaltyEffectivenessReductionCount;
                    double healingPenalty = PlayerMobile.RessPenaltyHealingScalar * m_Player.m_RessPenaltyEffectivenessReductionCount;
                    double fizzlePenalty = PlayerMobile.RessPenaltyFizzleScalar * m_Player.m_RessPenaltyEffectivenessReductionCount;
                    
                    AddLabel(145, startY, WhiteTextHue, "-" + Utility.CreatePercentageString(damagePenalty) + " Damage Dealt");
                    startY += rowSpacing;

                    AddLabel(140, startY, WhiteTextHue, "-" + Utility.CreatePercentageString(healingPenalty) + " Healing Amounts");
                    startY += rowSpacing;

                    AddLabel(67, startY, WhiteTextHue, Utility.CreatePercentageString(fizzlePenalty) + " Chance to Fizzle Field and Paralyze Spells");
                    startY += rowSpacing;
                }

                if (m_Player.m_RessPenaltyAccountWideAggressionRestriction)
                {
                    AddLabel(80, startY, 2550, "Cannot Attack Other Players (Account Wide)");
                    startY += rowSpacing;
                }

                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Player.m_RessPenaltyExpiration, true, true, true, true, false);

                AddItem(122, startY - 6, 6227);
                AddLabel(160, startY, 63, "Expires in " + timeRemaining + ")");
            }

            else
            {
                AddLabel(154, startY, 2599, "Current Penalties");
                startY += rowSpacing;

                AddLabel(194, startY, WhiteTextHue, "None");
            }

            AddButton(60, 332, 2151, 2154, 2, GumpButtonType.Reply, 0);
            AddLabel(95, 336, 2603, "View Potential Resurrection Penalty Options");        
        }
        
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //View Potential Penalties
                case 2:
                    m_Player.SendSound(openGumpSound);

                    m_Player.CloseGump(typeof(ConsiderSinsGump));
                    m_Player.SendGump(new ConsiderSinsGump(m_Player));

                    m_Player.CloseGump(typeof(ResurrectionGump));
                    m_Player.SendGump(new ResurrectionGump(m_Player, true, 0, false));

                    return;
                break;
            }            

            if (closeGump)
                m_Player.SendSound(closeGumpSound);

            else
            {
                m_Player.CloseGump(typeof(ConsiderSinsGump));
                m_Player.SendGump(new ConsiderSinsGump(m_Player));
            }
        }           
    }
}
