using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Accounting;

namespace Server
{
    public class ResurrectionGump : Gump
    {
        public PlayerMobile m_Player;
        public bool m_PreviewMode = true;
        public int m_SelectionIndex = 0;
        public bool m_Confirmed = false;

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public static int SelectionSound = 0x4D2;
        public static int LargeSelectionSound = 0x4D3;
        public int closeGumpSound = 0x058;

        public int WhiteTextHue = 2499;

        public ResurrectionGump(PlayerMobile player, bool previewMode, int selectionIndex, bool confirmed): base(400, 25)
        {
            m_Player = player;
            m_PreviewMode = previewMode;
            m_SelectionIndex = selectionIndex;
            m_Confirmed = confirmed;

            if (m_Player == null)
                return;

            Account account = m_Player.Account as Account;

            if (account == null)
                return;

            if (selectionIndex > 2)
                selectionIndex = 0;

            #region Images

            AddImage(57, 56, 103, 2075);
            AddImage(190, 153, 103, 2075);
            AddImage(191, 196, 103, 2075);
            AddImage(191, 282, 103, 2075);
            AddImage(191, 372, 103, 2075);
            AddImage(193, 472, 103, 2075);
            AddImage(57, 472, 103, 2075);
            AddImage(57, 281, 103, 2075);
            AddImage(189, 56, 103, 2075);
            AddImage(56, 195, 103, 2075);
            AddImage(57, 152, 103, 2075);
            AddImage(67, 194, 3604, 2052);
            AddImage(195, 184, 3604, 2052);
            AddImage(67, 66, 3604, 2052);
            AddImage(195, 66, 3604, 2052);
            AddImage(57, 371, 103, 2075);
            AddImage(67, 242, 3604, 2052);
            AddImage(196, 241, 3604, 2052);
            AddImage(67, 334, 3604, 2052);
            AddImage(195, 333, 3604, 2052);
            AddImage(67, 434, 3604, 2052);
            AddImage(195, 434, 3604, 2052);
            AddImage(77, -28, 102, 1107);
            AddItem(297, 58, 3812, 2101);

            #endregion

            AddLabel(161, 31, 149, "Punishable");
            AddLabel(147, 51, 149, "Murder Counts");
            AddLabel(152, 71, 2550, "(any above " + Mobile.MurderCountsRequiredForMurderer.ToString() + ")");

            int punishableMurderCounts = m_Player.MurderCounts - Mobile.MurderCountsRequiredForMurderer;

            if (punishableMurderCounts < 0)
                punishableMurderCounts = 0;

            AddItem(158, 102, 7574);

            if (punishableMurderCounts >= 1)            
                AddLabel(200, 112, 2116, punishableMurderCounts.ToString()); 

            else
                AddLabel(200, 112, WhiteTextHue, punishableMurderCounts.ToString());

            AddLabel(134, 161, WhiteTextHue, "Counts Will Reset");
            AddLabel(118, 178, WhiteTextHue, "to 5 Upon Resurrection");

            if (m_PreviewMode)
                AddLabel(147, 214, 2603, "Preview Mode");
            else
                AddLabel(127, 214, 2603, "Resurrection Options");

            AddButton(49, 49, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(45, 38, 149, "Guide");

            int resurrectionTotalCost = 0;
            int goldAvailableInBank = 0;
            int goldAvailableInAccount = 0;

            foreach (Mobile mobile in account.accountMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;

                if (mobile == m_Player)
                    goldAvailableInBank += Banker.GetBalance(m_Player);

                goldAvailableInAccount += Banker.GetBalance(mobile);
            }

            if (m_SelectionIndex == 0)
            {
                resurrectionTotalCost = punishableMurderCounts * PlayerMobile.RessPenaltyAccountWideAggressionRestrictionFeePerCount;

                AddButton(75, 258, 9724, 9721, 2, GumpButtonType.Reply, 0);
                AddLabel(110, 254, 63, "Pay " + PlayerMobile.RessPenaltyAccountWideAggressionRestrictionFeePerCount.ToString() + " Gold Per Murder Count");
            }

            else
            {
                AddButton(75, 258, 9721, 9724, 2, GumpButtonType.Reply, 0);
                AddLabel(110, 254, 2599, "Pay " + PlayerMobile.RessPenaltyAccountWideAggressionRestrictionFeePerCount.ToString() + " Gold Per Murder Count");
            }

            AddLabel(110, 274, WhiteTextHue, "No Aggressive Actions Allowed");
            AddLabel(110, 294, WhiteTextHue, "Account-Wide for 24 Hours");

            if (m_SelectionIndex == 1)
            {
                resurrectionTotalCost = punishableMurderCounts * PlayerMobile.RessPenaltyEffectivenessReductionFeePerCount;

                AddButton(75, 330, 9724, 9721, 3, GumpButtonType.Reply, 0);
                AddLabel(110, 325, 63, "Pay " + PlayerMobile.RessPenaltyEffectivenessReductionFeePerCount.ToString() + " Gold Per Murder Count");
            }

            else
            {
                AddButton(75, 330, 9721, 9724, 3, GumpButtonType.Reply, 0);
                AddLabel(110, 325, 2599, "Pay " + PlayerMobile.RessPenaltyEffectivenessReductionFeePerCount.ToString() + " Gold Per Murder Count");
            }

            double damagePenalty = PlayerMobile.RessPenaltyDamageScalar * (m_Player.m_RessPenaltyEffectivenessReductionCount + 1);
            double healingPenalty = PlayerMobile.RessPenaltyHealingScalar * (m_Player.m_RessPenaltyEffectivenessReductionCount + 1);
            double fizzlePenalty = PlayerMobile.RessPenaltyFizzleScalar * (m_Player.m_RessPenaltyEffectivenessReductionCount + 1);

            AddLabel(110, 344, WhiteTextHue, "-" + Utility.CreatePercentageString(damagePenalty) + " Damage Dealt");
            AddLabel(110, 364, WhiteTextHue, "-" + Utility.CreatePercentageString(healingPenalty) + " Healing Amounts");
            AddLabel(110, 384, WhiteTextHue, Utility.CreatePercentageString(fizzlePenalty) + " Chance to Fizzle Field");
            AddLabel(110, 404, WhiteTextHue, "and Paralyze Spells for 24 Hours");
            AddLabel(110, 423, 2550, "(Penalties are Stackable)");

            if (m_SelectionIndex == 2)
            {
                resurrectionTotalCost = punishableMurderCounts * PlayerMobile.RessPenaltyNoPenaltyFeePerCount;

                AddButton(75, 446, 9724, 9721, 4, GumpButtonType.Reply, 0);
                AddLabel(112, 451, 63, "Pay " + PlayerMobile.RessPenaltyNoPenaltyFeePerCount.ToString() +  " Gold Per Murder Count");
            }

            else
            {
                AddButton(75, 446, 9721, 9724, 4, GumpButtonType.Reply, 0);
                AddLabel(112, 451, 2599, "Pay " + PlayerMobile.RessPenaltyNoPenaltyFeePerCount.ToString() +  " Gold Per Murder Count");
            }

            AddLabel(85, 483, 2603, "Selected Option Cost:");

            AddLabel(230, 483, 63, Utility.CreateCurrencyString(resurrectionTotalCost));            

            AddLabel(85, 511, 149, "Gold Available in Bank:");
            AddLabel(230, 511, WhiteTextHue, Utility.CreateCurrencyString(goldAvailableInBank));

            AddLabel(85, 531, 149, "Gold Across Account:");
            AddLabel(230, 531, WhiteTextHue, Utility.CreateCurrencyString(goldAvailableInAccount));

            if (!m_PreviewMode)
            {
                AddButton(159, 555, 239, 240, 5, GumpButtonType.Reply, 0); //Apply

                if (m_Confirmed)
                    AddLabel(228, 555, 63, "Click Again to Confirm");
            }
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

                //Account-Wide Aggression Restriction
                case 2:
                    m_SelectionIndex = 0;
                    m_Confirmed = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Player Effectiveness Penalty
                case 3:
                    m_SelectionIndex = 1;
                    m_Confirmed = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Full Cost
                case 4:
                    m_SelectionIndex = 2;
                    m_Confirmed = false;

                    m_Player.SendSound(SelectionSound);

                    closeGump = false;
                break;

                //Apply
                case 5:
                    if (m_Confirmed)
                    {
                        //TEST
                        //RESOLVE
                    }

                    else
                    {
                        m_Confirmed = true;
                        m_Player.SendSound(LargeSelectionSound);

                        closeGump = false;
                    }                    
                break;
            }

            if (closeGump)
                m_Player.SendSound(closeGumpSound);

            else
            {
                m_Player.CloseGump(typeof(ResurrectionGump));
                m_Player.SendGump(new ResurrectionGump(m_Player, m_PreviewMode, m_SelectionIndex, m_Confirmed));
            }
        }           
    }
}
