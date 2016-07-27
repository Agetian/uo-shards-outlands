using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Server;
using Server.Items;
using Server.Commands;
using Server.Mobiles;
using Server.Gumps;
using Server.Accounting;

namespace Server
{
    public class SocietyJob : Item
    {
        public enum JobType
        {
            CraftItem,
            RetrieveFish,
            KillCreature,
            SinkShip,
            StealItem,
            TameCreature
        }

        public enum JobModifierType
        {
            NoModifier,
            ParagonCreature,
            RareCreature,
            ExceptionalQuality,
        }

        public bool m_Listed = true;
        public DateTime m_Expiration = DateTime.UtcNow + Societies.JobCycleDuration;

        public int m_PointValue = 1;

        public SocietiesGroupType m_SocietiesGroupType = SocietiesGroupType.AdventurersLodge;
        public JobType m_JobType = JobType.CraftItem;
        public string m_PrimaryTypeName;
        public Type m_PrimaryType;
        public string m_SecondaryTypeName;
        public Type m_SecondaryType;
        public double m_TargetNumber;
        public double m_TurnInRequirementAmount;
        public JobModifierType m_PrimaryJobModifier;
        public JobModifierType m_SecondaryJobModifier;
        public CraftResource m_CraftResourceRequired;        
        public int m_IconItemId = 3847;
        public int m_IconHue;
        public int m_IconOffsetX;
        public int m_IconOffsetY;
        public Type m_DestinationMobile;
        public string m_DestinationMobileName;
        public Town m_DestinationTown;

        public List<PlayerMobile> m_PlayersCompleted = new List<PlayerMobile>();

        [Constructable]
        public SocietyJob(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public SocietyJob(Serial serial): base(serial)
        {
        }

        public string GetJobDescriptionText()
        {
            string description = "";

            switch (m_JobType)
            {
                case JobType.CraftItem: description = "Craft " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.KillCreature: description = "Kill " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.RetrieveFish: description = "Catch " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.SinkShip: description = "Sink " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.StealItem: description = "Steal " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.TameCreature: description = "Tame " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName; break;
            }

            return description;
        }

        public string GetJobDescriptionProgressText(PlayerMobile player)
        {
            string description = "";

            Societies.CheckCreateSocietiesPlayerSettings(player);

            SocietyJobPlayerProgress jobPlayerProgress = Societies.GetSocietiesJobPlayerProgress(player, this);

            double primaryNumber = 0;
            double secondaryNumber = 0;

            if (jobPlayerProgress != null)
            {
                primaryNumber = jobPlayerProgress.m_ProgressAmount;
                secondaryNumber = jobPlayerProgress.m_TurnedInAmount;
            }

            switch (m_JobType)
            {
                case JobType.CraftItem: description = primaryNumber.ToString() + " / " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName + " Crafted"; break;
                case JobType.KillCreature: description = primaryNumber.ToString() + " / " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName + " Killed"; break;
                case JobType.RetrieveFish: description = primaryNumber.ToString() + " / " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName + " Caught"; break;
                case JobType.SinkShip: description = primaryNumber.ToString() + " / " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName + " Sunk"; break;
                case JobType.StealItem: description = primaryNumber.ToString() + " / " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName + " Stolen"; break;
                case JobType.TameCreature: description = primaryNumber.ToString() + " / " + m_TargetNumber.ToString() + " " + m_PrimaryTypeName + " Tamed"; break;
            }

            return description;
        }

        public string GetJobDestinationText()
        {
            string description = "";

            description = "Any " + m_DestinationMobileName + " in " + m_DestinationTown.TownName;

            return description;
        }

        public string GetJobRewardText()
        {
            string description = "";

            string societiesGroupName = Societies.GetSocietyGroupName(m_SocietiesGroupType);

            if (m_PointValue == 1)
                return "" + m_PointValue.ToString() + " Society Point Awarded";

            else
                return "" + m_PointValue.ToString() + " Society Points Awarded";

            return description;
        }
        
        public bool TurnIn(PlayerMobile player, Mobile vendor)
        {
            bool completed = false;

            if (player == null) return false;
            if (player.Backpack == null) return false;

            SocietyJobPlayerProgress jobProgress = Societies.GetSocietiesJobPlayerProgress(player, this);

            if (jobProgress == null)
                return false;

            if (m_TurnInRequirementAmount >= 1)
            {
                bool turnInItem = false;
                bool turnInCreature = false;

                double remainingNeeded = m_TurnInRequirementAmount - jobProgress.m_TurnedInAmount;
                int amountTurnedIn = 0;

                Queue m_Queue = new Queue();

                if (m_JobType == JobType.CraftItem || m_JobType == JobType.RetrieveFish || m_JobType == JobType.StealItem)
                {
                    turnInItem = true;

                    Item[] m_MatchingItems = player.Backpack.FindItemsByType(m_PrimaryType);

                    foreach (Item item in m_MatchingItems)
                    {
                        if (item == null) continue;
                        if (m_CraftResourceRequired != CraftResource.None && item.Resource != m_CraftResourceRequired) continue;
                        if (m_PrimaryJobModifier == JobModifierType.ExceptionalQuality && item.Quality != Quality.Exceptional) continue;
                        if (m_SecondaryJobModifier == JobModifierType.ExceptionalQuality && item.Quality != Quality.Exceptional) continue;

                        jobProgress.m_TurnedInAmount++;
                        amountTurnedIn++;
                        remainingNeeded--;                        

                        m_Queue.Enqueue(item);

                        if (remainingNeeded <= 0)
                            break;
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item mobile = (Item)m_Queue.Dequeue();
                        mobile.Delete();
                    }
                }

                else if (m_JobType == JobType.TameCreature)
                {  
                    turnInCreature = true;

                    for (int a = 0; a < player.AllFollowers.Count; a++)
                    {
                        Mobile follower = player.AllFollowers[a];

                        if (follower.GetType() != m_PrimaryType) continue;
                        if (follower == null) continue;
                        if (!follower.Alive || follower.IsDeadBondedPet) continue;
                        if (Utility.GetDistance(follower.Location, vendor.Location) >= 12) continue;

                        jobProgress.m_TurnedInAmount++;
                        amountTurnedIn++;
                        remainingNeeded--;                        

                        m_Queue.Enqueue(follower);

                        if (remainingNeeded <= 0)
                            break;
                    }

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();
                        mobile.Delete();
                    }
                }

                if (amountTurnedIn == 0)
                {
                    if (turnInCreature)
                        player.SendMessage("You do not have any of the required creatures nearby needed to complete that job.");
                    
                    else if (turnInItem)
                        player.SendMessage("You do not have any of the required items neccessary to complete that job in your backpack.");

                    return false;
                }

                else if (amountTurnedIn > 0 && remainingNeeded > 0)
                {
                    if (turnInCreature)
                        player.SendMessage("You turn in some of the creatures neccessary for this job but still require more to complete it fully.");

                    else if (turnInItem)
                        player.SendMessage("You turn in some of the items neccessary for this job but still require more to complete it fully.");

                    return false;
                }

                else 
                    return true;
            }

            else if (jobProgress.m_ProgressAmount >= m_TargetNumber)     
                return true;  

            return completed;
        }

        public void Complete(PlayerMobile player)
        {
            SocietyJobPlayerProgress jobProgress = Societies.GetSocietiesJobPlayerProgress(player, this);

            if (jobProgress == null)
                return;

            if (!m_PlayersCompleted.Contains(player))
                m_PlayersCompleted.Add(player);

            SocietyGroupPlayerData societyGroupPlayerData = player.m_SocietiesPlayerSettings.GetSocietyGroupPlayerData(m_SocietiesGroupType);

            if (societyGroupPlayerData != null)
            {
                societyGroupPlayerData.m_PointsAvailable += m_PointValue;
                societyGroupPlayerData.m_MontlyPoints += m_PointValue;
                societyGroupPlayerData.m_LifetimePoints += m_PointValue;

                player.SendMessage(Societies.GetSocietyGroupTextHue(m_SocietiesGroupType), "You have been awarded " + m_PointValue.ToString() + " with the " + Societies.GetSocietyGroupName(m_SocietiesGroupType) + " for completion of a job contract.");
            }

            if (jobProgress.m_JobContract != null)
                jobProgress.m_JobContract.JobStatus = SocietiesJobContract.JobStatusType.Completed;

            player.m_SocietiesPlayerSettings.m_JobProgress.Remove(jobProgress);

            Account account = player.Account as Account;

            foreach (Mobile mobile in account.accountMobiles)    
            {                
                PlayerMobile pm_Target = mobile as PlayerMobile;  

                if (pm_Target == null) continue;
                if (pm_Target == player) continue;

                Societies.CheckCreateSocietiesPlayerSettings(pm_Target);

                SocietyJobPlayerProgress otherPlayerJobProgress = Societies.GetSocietiesJobPlayerProgress(player, this);

                if (otherPlayerJobProgress == null)
                    continue;

                if (otherPlayerJobProgress.m_JobContract != null)
                    otherPlayerJobProgress.m_JobContract.JobStatus = SocietiesJobContract.JobStatusType.Completed;

                player.m_SocietiesPlayerSettings.m_JobProgress.Remove(otherPlayerJobProgress);
            }
        }

        public bool AccountHasCompleted(PlayerMobile player)
        {
            if (player == null)
                return false;

            Account account = player.Account as Account;

            if (account == null)
                return false;

            foreach (PlayerMobile targetPlayer in m_PlayersCompleted)
            {
                if (player == null)
                    continue;

                Account targetAccount = targetPlayer.Account as Account;

                if (targetAccount == null) continue;

                if (account == targetAccount)
                    return true;
            }

            return false;
        }

        public void PlayerAccept(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Backpack == null) return;

            if (AccountHasCompleted(player))
            {
                player.SendMessage("A character on this account has already completed this job.");
                return;
            }

            SocietyJobPlayerProgress playerJobProgress = Societies.GetSocietiesJobPlayerProgress(player, this);

            if (playerJobProgress == null)
            {
                if (player.Backpack.TotalItems >= player.Backpack.MaxItems)
                {
                    player.SendMessage("You have too many items in your backpack to accept a new job contract.");
                    return;
                }

                else
                {
                    playerJobProgress = new SocietyJobPlayerProgress(this);
                    player.m_SocietiesPlayerSettings.m_JobProgress.Add(playerJobProgress);

                    player.SendMessage("You accept the job offer. A contract has been placed in your backpack.");

                    SocietiesJobContract jobContract = new SocietiesJobContract(this);
                    playerJobProgress.m_JobContract = jobContract;
                    jobContract.m_Player = player;

                    player.SendSound(0x249);

                    player.Backpack.DropItem(jobContract);
                }
            }

            else
            {
                List<SocietiesJobContract> m_ContractsHeld = player.Backpack.FindItemsByType<SocietiesJobContract>();

                bool foundMatch = false;

                foreach (SocietiesJobContract contract in m_ContractsHeld)
                {
                    if (contract.m_Job == this)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (foundMatch)
                {
                    player.SendMessage("You already have a contract for this job in your backpack.");
                    return;
                }

                if (player.Backpack.TotalItems >= player.Backpack.MaxItems)
                {
                    player.SendMessage("You have too many items in your backpack to receive a replacement contract for this job.");
                    return;
                }

                else
                {
                    //Delete Any Old Instances of this Contract Created by Player
                    Queue m_Queue = new Queue();

                    foreach (Item item in World.Items.Values)
                    {
                        if (item is SocietiesJobContract)
                        {
                            SocietiesJobContract oldContract = item as SocietiesJobContract;

                            if (oldContract == null)
                                continue;

                            if (oldContract.m_Job == this && oldContract.m_Player == player)
                                m_Queue.Enqueue(oldContract);
                        }
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();

                        item.Delete();
                    }

                    SocietiesJobContract jobContract = new SocietiesJobContract(this);

                    jobContract.m_Player = player;
                    playerJobProgress.m_JobContract = jobContract;

                    player.Backpack.DropItem(jobContract);

                    player.SendSound(0x249);

                    player.SendMessage("You have previouly accepted this job offer, and receive a replacement contract for the job.");
                    return;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Listed);
            writer.Write(m_Expiration);
            writer.Write(m_PointValue);
            writer.Write((int)m_SocietiesGroupType);
            writer.Write((int)m_JobType);
            writer.Write(m_PrimaryTypeName);

            if (m_PrimaryType == null)
                writer.Write("null");
            else
                writer.Write((string)m_PrimaryType.ToString());

            writer.Write(m_SecondaryTypeName);

            if (m_SecondaryType == null)
                writer.Write("null");
            else
                writer.Write((string)m_SecondaryType.ToString());

            writer.Write(m_TargetNumber);
            writer.Write(m_TurnInRequirementAmount);
            writer.Write((int)m_CraftResourceRequired);
            writer.Write((int)m_PrimaryJobModifier);
            writer.Write((int)m_SecondaryJobModifier);
            writer.Write(m_IconItemId);
            writer.Write(m_IconHue);
            writer.Write(m_IconOffsetX);
            writer.Write(m_IconOffsetY);

            if (m_DestinationMobile == null)
                writer.Write("null");
            else
                writer.Write((string)m_DestinationMobile.ToString());

            writer.Write(m_DestinationMobileName);
            writer.Write(m_DestinationTown);

            writer.Write(m_PlayersCompleted.Count);
            for (int a = 0; a < m_PlayersCompleted.Count; a++)
            {
                writer.Write(m_PlayersCompleted[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Listed = reader.ReadBool();
                m_Expiration = reader.ReadDateTime();
                m_PointValue = reader.ReadInt();
                m_SocietiesGroupType = (SocietiesGroupType)reader.ReadInt();
                m_JobType = (JobType)reader.ReadInt();

                m_PrimaryTypeName = reader.ReadString();

                string primaryType = reader.ReadString();
                if (primaryType == "null")
                    m_PrimaryType = null;
                else
                    m_PrimaryType = Type.GetType(primaryType);

                m_SecondaryTypeName = reader.ReadString();

                string secondaryType = reader.ReadString();
                if (secondaryType == "null")
                    m_SecondaryType = null;
                else
                    m_SecondaryType = Type.GetType(secondaryType);

                m_TargetNumber = reader.ReadDouble();
                m_TurnInRequirementAmount = reader.ReadDouble();                
                m_CraftResourceRequired = (CraftResource)reader.ReadInt();
                m_PrimaryJobModifier = (JobModifierType)reader.ReadInt();
                m_SecondaryJobModifier = (JobModifierType)reader.ReadInt();

                m_IconItemId = reader.ReadInt();
                m_IconHue = reader.ReadInt();
                m_IconOffsetX = reader.ReadInt();
                m_IconOffsetY = reader.ReadInt();

                string destionationMobile = reader.ReadString();
                if (destionationMobile == "null")
                    m_DestinationMobile = null;
                else
                    m_DestinationMobile = Type.GetType(destionationMobile);

                m_DestinationMobileName = reader.ReadString();
                m_DestinationTown = reader.ReadItem() as Town;

                int playersCompletedCount = reader.ReadInt();
                for (int a = 0; a < playersCompletedCount; a++)
                {
                    m_PlayersCompleted.Add(reader.ReadMobile() as PlayerMobile);
                }
            }

            //-----

            Societies.m_SocietyJobs.Add(this);
        }
    }
}
