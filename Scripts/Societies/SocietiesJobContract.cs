using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class SocietiesJobContract : Item
    {
        public enum JobStatusType
        {
            None,
            Expired,
            Ready,
            Completed
        }

        public PlayerMobile m_Player;
        public SocietyJob m_Job;

        public JobStatusType m_JobStatus;
        [CommandProperty(AccessLevel.GameMaster)]
        public JobStatusType JobStatus
        {
            get { return m_JobStatus; }
            set 
            { 
                m_JobStatus = value;

                switch (m_JobStatus)
                {
                    case JobStatusType.None: Hue = 0; break;
                    case JobStatusType.Expired: Hue = 2401; break;
                    case JobStatusType.Ready: Hue = 2003; break;
                    case JobStatusType.Completed: Hue = 2401; break;
                }
            }
        }        

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
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

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

            if (m_Player == null)
            {
                LabelTo(from, "an expired society job contract.");
                return;
            }

            bool accountHasCompleted = m_Job.AccountHasCompleted(player);

            if (accountHasCompleted && m_JobStatus != JobStatusType.Completed)
                m_JobStatus = JobStatusType.Completed;

            else if (m_Job.m_Expiration <= DateTime.UtcNow && m_JobStatus != JobStatusType.Expired)
                m_JobStatus = JobStatusType.Expired;

            string jobText = "Job Contract: " + m_Job.GetJobDescriptionProgressText(m_Player);
            string ownerText = "(Accepted by " + m_Player.RawName + ")";
            string expirationText = "[Expires in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Job.m_Expiration, true, true, true, true, false) + "]";
           
            switch (m_JobStatus)
            {
                case JobStatusType.None:
                    LabelTo(from, jobText);
                    LabelTo(from, ownerText);
                    LabelTo(from, expirationText);
                break;

                case JobStatusType.Ready:
                    LabelTo(from, jobText);
                    LabelTo(from, ownerText);
                    LabelTo(from, expirationText);
                break;

                case JobStatusType.Completed:
                    ownerText = "(Completed by Account)";

                    LabelTo(from, jobText);
                    LabelTo(from, ownerText);
                break;

                case JobStatusType.Expired:
                    LabelTo(from, "an expired society job contract.");
                break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            player.SendSound(0x055);

            player.CloseGump(typeof(SocitiesJobContractGump));
            player.SendGump(new SocitiesJobContractGump(player, this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version  
       
            //Version 0
            writer.Write(m_Player);
            writer.Write(m_Job);
            writer.Write((int)m_JobStatus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;
                m_Job = reader.ReadItem() as SocietyJob;
                m_JobStatus = (JobStatusType)reader.ReadInt();
            }
        }
    }

    public class SocitiesJobContractGump : Gump
    {
        public PlayerMobile m_Player;
        public SocietiesJobContract m_SocietiesJobContract; 

        public static int OpenGumpSound = 0x055;
        public static int ChangePageSound = 0x057;
        public static int SelectionSound = 0x3E6;
        public static int PurchaseSound = 0x2E6;
        public static int CloseGumpSound = 0x058;

        public SocitiesJobContractGump(PlayerMobile player, SocietiesJobContract societiesJobContract): base(10, 10)
        {
            m_Player = player;
            m_SocietiesJobContract = societiesJobContract;

            if (m_Player == null) return;
            if (m_SocietiesJobContract == null)
            {
                m_Player.SendMessage("That job contract is no longer accessible");
                return;
            }

            if (m_SocietiesJobContract.Deleted)
            {
                m_Player.SendMessage("That job contract is no longer accessible");
                return;
            }

            if (m_SocietiesJobContract.m_Job == null)
            {
                m_Player.SendMessage("That job is no longer available");
                return;
            }

            if (m_SocietiesJobContract.m_Job.Deleted)
            {
                m_Player.SendMessage("That job is no longer available");
                return;
            }

            SocietyJob societyJob = m_SocietiesJobContract.m_Job;
            SocietyJobPlayerProgress jobPlayerProgress = Societies.GetSocietiesJobPlayerProgress(m_Player, societyJob);
            
            double progress = 0;
            double turnedInAmount = 0;

            if (jobPlayerProgress != null)
            {
                progress = jobPlayerProgress.m_ProgressAmount;
                turnedInAmount = jobPlayerProgress.m_TurnedInAmount;
            }

            bool readyForTurnIn = false;

            if (progress >= societyJob.m_TargetNumber)
                readyForTurnIn = true;

            AddImage(0, 0, 1249);

            int WhiteTextHue = 2499;

            string societiesGroupName = Societies.GetSocietyGroupName(societyJob.m_SocietiesGroupType);
            int societiesGroupTextHue = Societies.GetSocietyGroupTextHue(societyJob.m_SocietiesGroupType);

            string titleText = "Job Contract From " + societiesGroupName;
            string ownerName = "-";

            if (m_SocietiesJobContract.m_Player != null)
                ownerName = m_SocietiesJobContract.m_Player.RawName;

            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, societyJob.m_Expiration, true, true, true, true, false);
            string jobDescriptionText = societyJob.GetJobDescriptionText();
            string jobProgressText = "Progress: " + progress + " / " + societyJob.m_TargetNumber.ToString();
            string jobTurnInText = "Turned In: " + turnedInAmount + " / " + societyJob.m_TurnInRequirementAmount.ToString();
            string destinationText = societyJob.GetJobDestinationText();
            
            AddLabel(Utility.CenteredTextOffset(220, titleText), 37, societiesGroupTextHue, titleText); //Title

            int startX = 115;
            int startY = 50;

            #region Society Images

            switch (societyJob.m_SocietiesGroupType)
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

            AddLabel(58, 80, 2599, "Accepted By");
            AddLabel(Utility.CenteredTextOffset(95, ownerName), 100, WhiteTextHue, ownerName);
                        
            AddLabel(250, 80, 2603, "Job Expires In");
            AddLabel(Utility.CenteredTextOffset(295, timeRemaining), 100, WhiteTextHue, timeRemaining);

            AddLabel(142, 140, 149, "Job Description");
            
            AddItem(-2 + societyJob.m_IconOffsetX, 125 + societyJob.m_IconOffsetY, societyJob.m_IconItemId, societyJob.Hue); //Image

            startY = 160;
            int rowSpacing = 20;

            AddLabel(105, startY, WhiteTextHue, jobDescriptionText);
            startY += rowSpacing;

            if (readyForTurnIn)
                AddLabel(105, startY, 63, jobProgressText);
            else
                AddLabel(105, startY, 2114, jobProgressText);
            startY += rowSpacing;

            if (societyJob.m_TurnInRequirementAmount > 0)
            {
                AddLabel(105, startY, 2301, jobTurnInText);
                startY += rowSpacing;
            }

            AddLabel(105, startY, societiesGroupTextHue, societyJob.GetJobRewardText());

            AddLabel(69, 245, 149, "Turn In");
            AddButton(77, 265, 2151, 2154, 2, GumpButtonType.Reply, 0);

            AddLabel(170, 245, 149, "Completion Destination");
            AddLabel(Utility.CenteredTextOffset(245, destinationText), 265, 2550, destinationText);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            if (m_Player == null) return;
            if (m_SocietiesJobContract == null)
            {
                m_Player.SendMessage("That job contract is no longer accessible");
                return;
            }

            if (m_SocietiesJobContract.Deleted)
            {
                m_Player.SendMessage("That job contract is no longer accessible");
                return;
            }

            if (m_SocietiesJobContract.m_Job == null)
            {
                m_Player.SendMessage("That job is no longer available");
                return;
            }

            if (m_SocietiesJobContract.m_Job.Deleted)
            {
                m_Player.SendMessage("That job is no longer available");
                return;
            }

            SocietyJob societyJob = m_SocietiesJobContract.m_Job;
            SocietyJobPlayerProgress jobPlayerProgress = Societies.GetSocietiesJobPlayerProgress(m_Player, societyJob);
            
            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Turn In
                case 2:
                    if (m_SocietiesJobContract.m_Player != m_Player)                    
                        m_Player.SendMessage("That job contract does not belong to you.");

                    else if (societyJob.m_Expiration <= DateTime.UtcNow)
                        m_Player.SendMessage("That job contract has expired.");

                    else if (jobPlayerProgress != null)
                    {
                        if (jobPlayerProgress.m_ProgressAmount < societyJob.m_TargetNumber)
                            m_Player.SendMessage("You have not completed all of the requirements of that contract yet.");

                        else if (societyJob.m_DestinationTown != null)
                        {
                            if (m_Player.Region != societyJob.m_DestinationTown.region)
                                m_Player.SendMessage("You must be within " + societyJob.m_DestinationTown.TownName + " to turn in that contract.");

                            else
                            {
                                Mobile matchingMobile = null;

                                IPooledEnumerable m_NearbyMobiles = m_Player.Map.GetMobilesInRange(m_Player.Location, 8);

                                int closestMobile = 10000;

                                foreach (Mobile mobile in m_NearbyMobiles)
                                {
                                    if (!mobile.Alive) continue;
                                    if (mobile.Hidden) continue;

                                    if (mobile.GetType() == societyJob.m_DestinationMobile)
                                    {
                                        int distance = Utility.GetDistance(m_Player.Location, mobile.Location);

                                        if (distance < closestMobile)
                                        {
                                            matchingMobile = mobile;
                                            closestMobile = distance;
                                        }
                                    }
                                }

                                m_NearbyMobiles.Free();

                                if (matchingMobile != null)
                                {
                                    bool validCompletion = societyJob.TurnIn(m_Player, matchingMobile);

                                    if (validCompletion)                                    
                                        societyJob.Complete(m_Player);                                    
                                }

                                else                                
                                    m_Player.SendMessage("You must be near a " + societyJob.m_DestinationMobileName + " in order to turn in that contract.");                                
                            }
                        }
                    }

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(SocitiesJobContractGump));
                m_Player.SendGump(new SocitiesJobContractGump(m_Player, m_SocietiesJobContract));
            }

            else
                m_Player.SendSound(CloseGumpSound);
        }
    }
}