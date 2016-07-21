using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class SocietiesJobContract : Item
    {
        public SocietyJob m_Job = null;       

        [Constructable]
        public SocietiesJobContract(SocietyJob job): base(5357)
        {
            Name = "society job contract";

            m_Job = job;
        }

        public SocietiesJobContract(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Job == null)
            {
                LabelTo(from, "an expired society job contract.");
                return;
            }

            if (m_Job.Deleted)
            {
                LabelTo(from, "an expired society job contract.");
                return;
            }

            string professionGroupName = Societies.GetSocietyGroupName(m_Job.m_SocietiesGroupType);
            
            string titleText = "Job Contract From The " + professionGroupName;
            string jobDescriptionText = "(" + m_Job.GetJobProgressText() + ")";
            string timeRemaining = "[Expires in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, Societies.NextJobsReset, true, true, true, true, false) + "]";

            LabelTo(from, titleText);
            LabelTo(from, jobDescriptionText);
            LabelTo(from, timeRemaining);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.CloseGump(typeof(SocitiesJobContractGump));
            player.SendGump(new SocitiesJobContractGump(player));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  
       
            //Version 0
            writer.Write(m_Job);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Job = reader.ReadItem() as SocietyJob;
            }
        }
    }

    public class SocitiesJobContractGump : Gump
    {
        public PlayerMobile m_Player;
        public SocietiesGroupType m_SocietiesGroup = SocietiesGroupType.ArtificersEnclave;

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x3E6;
        public static int PurchaseSound = 0x2E6;
        public static int CloseGumpSound = 0x058;

        public SocitiesJobContractGump(PlayerMobile player): base(10, 10)
        {
            if (player == null)
                return;

            m_Player = player;

            AddImage(0, 0, 1249);

            int WhiteTextHue = 2499;

            string societiesGroupName = Societies.GetSocietyGroupName(m_SocietiesGroup);
            int societiesGroupTextHue = Societies.GetSocietyGroupTextHue(m_SocietiesGroup);

            string titleText = "Job Contract from The " + societiesGroupName;
            string timeRemaining = "23h 17m";
            string destinationText = "Any Alchemist in Prevalia";

            AddLabel(Utility.CenteredTextOffset(220, titleText), 37, societiesGroupTextHue, titleText);

            int startX = 115;
            int startY = 52;

            #region Society Images

            switch (m_SocietiesGroup)
            {
                case SocietiesGroupType.FishermansCircle:
                    AddItem(startX + 34, startY + 19, 3520);
                    AddItem(startX + 66, startY + 48, 3656);
                    AddItem(startX + 35, startY + 36, 2476);
                    AddItem(startX + 76, startY + 39, 2467);
                    AddItem(startX + 45, startY + 35, 15113);
                break;

                case SocietiesGroupType.SmithingOrder:
                    AddItem(startX + 36, startY + 29, 5073);
                    AddItem(startX + 86, startY + 29, 5096);
                    AddItem(startX + 50, startY + 39, 7035);
                    AddItem(startX + 54, startY + 37, 5050);
                    AddItem(startX + 47, startY + 33, 5181);
                break;

                case SocietiesGroupType.TradesmanUnion:
                    AddItem(startX + 29, startY + 27, 4142);
                    AddItem(startX + 37, startY + 23, 4150);
                    AddItem(startX + 61, startY + 35, 2920);
                    AddItem(startX + 49, startY + 25, 2921);
                    AddItem(startX + 67, startY + 47, 4148);
                    AddItem(startX + 48, startY + 31, 4189);
                    AddItem(startX + 57, startY + 27, 2581);
                    AddItem(startX + 36, startY + 20, 2503);
                    AddItem(startX + 45, startY + 14, 4172);
                break;

                case SocietiesGroupType.ArtificersEnclave:
                    AddItem(startX + 62, startY + 30, 2942, 2500);
                    AddItem(startX + 37, startY + 16, 2943, 2500);
                    AddItem(startX + 40, startY + 20, 4031);
                    AddItem(startX + 65, startY + 19, 6237);
                    AddItem(startX + 59, startY + 37, 3626);
                    AddItem(startX + 45, startY + 13, 3643, 2415);
                    AddItem(startX + 40, startY + 29, 5357);
                    AddItem(startX + 44, startY + 31, 5357);
                    AddItem(startX + 65, startY + 43, 3622);
                break;

                case SocietiesGroupType.SeafarersLeague:
                    AddItem(startX + 70, startY + 40, 5370);
                    AddItem(startX + 46, startY + 3, 709);
                break;

                case SocietiesGroupType.AdventurersLodge:
                    AddItem(startX + 57, startY + 24, 4967);
                    AddItem(startX + 49, startY + 35, 4970);
                    AddItem(startX + 64, startY + 49, 2648);
                    AddItem(startX + 34, startY + 38, 5356);
                    AddItem(startX + 40, startY + 45, 3922);
                    AddItem(startX + 1, startY + 30, 3898);
                    AddItem(startX + 50, startY + 25, 5365);
                break;

                case SocietiesGroupType.ZoologicalFoundation:
                    AddItem(startX + 50, startY + 40, 2476);
                    AddItem(startX + 47, startY + 31, 3191);
                    AddItem(startX + 51, startY + 29, 3191);
                    AddItem(startX + 50, startY + 30, 3713);
                break;

                case SocietiesGroupType.ThievesGuild:
                    AddItem(startX + 58, startY + 39, 5373);
                    AddItem(startX + 48, startY + 33, 3589);
                break;

                case SocietiesGroupType.FarmersCooperative:
                    AddItem(startX + 54, startY + 23, 18240);
                break;

                case SocietiesGroupType.MonsterHuntersSociety:
                    AddItem(startX + 32, startY + 26, 7433);
                    AddItem(startX + 34, startY + 38, 4655);
                    AddItem(startX + 54, startY + 23, 7438);
                    AddItem(startX + 27, startY + 40, 7782);
                    AddItem(startX + 44, startY + 38, 3910);
                break;
            }

            #endregion
            
            AddLabel(8, 14, 149, "Guide");
            AddButton(11, 30, 2094, 2095, 1, GumpButtonType.Reply, 0);
                        
            AddLabel(261, 81, 2603, "Job Expires In");
            AddLabel(Utility.CenteredTextOffset(305, timeRemaining), 102, WhiteTextHue, timeRemaining);

            AddLabel(142, 142, 149, "Job Description");

            AddItem(53, 173, 3847); //Image
            AddLabel(109, 163, WhiteTextHue, "Craft 300 Greater Cure Potions");
            AddLabel(119, 183, 2599, "(1 Profession Point Awarded)");

            AddLabel(69, 240, 149, "Turn In");
            AddButton(77, 258, 2151, 2154, 2, GumpButtonType.Reply, 0);

            AddLabel(170, 240, 149, "Completion Destination");
            AddLabel(Utility.CenteredTextOffset(245, destinationText), 260, 2550, destinationText);
        }
    }
}