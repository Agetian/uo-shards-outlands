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
    public enum SocietiesGroupType
    {
        AdventurersLodge,
        ArtificersEnclave,
        FarmersCooperative,
        FishermansCircle,
        MonsterHuntersSociety,
        SeafarersLeague,
        SmithingOrder,
        ThievesGuild,
        TradesmanUnion,
        ZoologicalFoundation
    }

    public enum SocietiesGroupPageDisplayType
    {
        Jobs,
        SpendPoints,
        ServerRankings
    }

    public static class Societies
    {
        public static SocietiesPersistanceItem PersistanceItem;

        public static bool Enabled = true;

        public static List<SocietyJob> m_SocietyJobs = new List<SocietyJob>();

        public static TimeSpan JobCycleDuration = TimeSpan.FromHours(72);
        public static DateTime NextJobsAdded = DateTime.UtcNow;

        public static TimeSpan TickDuration = TimeSpan.FromMinutes(15);

        public static Timer m_Timer;

        public static void Initialize()
        {
            CommandSystem.Register("AddNewSocietyJobs", AccessLevel.Administrator, new CommandEventHandler(AddNewSocietyJobs));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new SocietiesPersistanceItem();
            });

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (Enabled)
                {
                    if (m_Timer != null)
                    {
                        m_Timer.Stop();
                        m_Timer = null;
                    }

                    m_Timer = new SocietiesTimer();
                    m_Timer.Start();
                }
            });
        }

        #region Commands

        [Usage("AddSocietyJobs")]
        [Description("Adds New Society Jobs to Public Boards")]
        public static void AddNewSocietyJobs(CommandEventArgs e)
        {
            AddSocietyJobs();
        }

        #endregion

        public class SocietiesTimer : Timer
        {
            public SocietiesTimer() : base(TimeSpan.Zero, TickDuration)
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (!Societies.Enabled)
                    return;

                CheckForExpiredSocietyJobs();

                if (DateTime.UtcNow >= NextJobsAdded)
                    AddSocietyJobs();
            }
        }

        public static void CheckForExpiredSocietyJobs()
        {           
            List<SocietyJob> m_ExpiredJobs = new List<SocietyJob>();
            Queue m_Queue = new Queue();

            foreach (SocietyJob societyJob in m_SocietyJobs)
            {
                if (societyJob.m_Expiration <= DateTime.UtcNow)
                {                    
                    m_ExpiredJobs.Add(societyJob);
                    m_Queue.Enqueue(societyJob);
                }
            }
            
            foreach (Mobile mobile in World.Mobiles.Values)
            {
                if (!mobile.Player)
                    continue;

                PlayerMobile player = mobile as PlayerMobile;

                CheckCreateSocietiesPlayerSettings(player);

                foreach (SocietyJob societyJob in m_ExpiredJobs)
                {
                    for (int a = 0; a < player.m_SocietiesPlayerSettings.m_JobProgress.Count; a++)
                    {
                        SocietyJobPlayerProgress jobPlayerProgress = player.m_SocietiesPlayerSettings.m_JobProgress[a];

                        if (jobPlayerProgress == null)
                            continue;

                        if (jobPlayerProgress.m_Job == societyJob)
                        {
                            if (jobPlayerProgress.m_JobContract != null)
                                jobPlayerProgress.m_JobContract.JobStatus = SocietiesJobContract.JobStatusType.Expired;

                            player.m_SocietiesPlayerSettings.m_JobProgress.RemoveAt(a);

                            break;
                        }
                    }
                }
            }
            
            while (m_Queue.Count > 0)
            {
                SocietyJob societyJob = (SocietyJob)m_Queue.Dequeue();
                societyJob.Delete();
            }
        }

        public static void AddSocietyJobs()
        {
            NextJobsAdded = NextJobsAdded + JobCycleDuration;

            ClearOldJobListings();
            GenerateNewJobs();

            Utility.BroadcastMessage("New society job contracts are now available at society boards in each township.", 63);
        }

        public static void ClearOldJobListings()
        {
            foreach (SocietyJob societyJob in m_SocietyJobs)
            {
                if (societyJob == null)
                    continue;

                societyJob.m_Listed = false;
            }
        }

        public static void GenerateNewJobs()
        {
            int societyGroupCount = Enum.GetNames(typeof(SocietiesGroupType)).Length;

            for (int a = 0; a < societyGroupCount; a++)
            {
                SocietiesJobs.GenerateJobs((SocietiesGroupType)a);
            }
        }

        public static string GetSocietyGroupName(SocietiesGroupType societyGroupType)
        {
            switch (societyGroupType)
            {
                case SocietiesGroupType.AdventurersLodge: return "Adventurer's Lodge"; break;
                case SocietiesGroupType.ArtificersEnclave: return "Artificer's Enclave"; break;
                case SocietiesGroupType.FarmersCooperative: return "Farmer's Cooperative"; break;
                case SocietiesGroupType.FishermansCircle: return "Fisherman's Circle"; break;
                case SocietiesGroupType.MonsterHuntersSociety: return "Monster Hunter's Society"; break;
                case SocietiesGroupType.SeafarersLeague: return "Seafarer's League"; break;
                case SocietiesGroupType.SmithingOrder: return "Smithing Order"; break;
                case SocietiesGroupType.ThievesGuild: return "Thieves Guild"; break;
                case SocietiesGroupType.TradesmanUnion: return "Tradesman Union"; break;
                case SocietiesGroupType.ZoologicalFoundation: return "Zoological Foundation"; break;  
            }

            return "";
        }

        public static int GetSocietyGroupTextHue(SocietiesGroupType societyGroupType)
        {
            switch (societyGroupType)
            {
                case SocietiesGroupType.AdventurersLodge: return 152; break;
                case SocietiesGroupType.ArtificersEnclave: return 2606; break;
                case SocietiesGroupType.FarmersCooperative: return 2599; break;
                case SocietiesGroupType.FishermansCircle: return 2590; break;
                case SocietiesGroupType.MonsterHuntersSociety: return 2116; break;
                case SocietiesGroupType.SeafarersLeague: return 2603; break;
                case SocietiesGroupType.SmithingOrder: return 2401; break;
                case SocietiesGroupType.ThievesGuild: return 2036; break;
                case SocietiesGroupType.TradesmanUnion: return 2417; break;
                case SocietiesGroupType.ZoologicalFoundation: return 63; break;
            }

            return 2499;
        }

        public static List<SocietyJob> GetSocietyJobsByGroup(SocietiesGroupType societyGroupType)
        {
            List<SocietyJob> m_SocietiesJobs = new List<SocietyJob>();

            for (int a = 0; a < m_SocietyJobs.Count; a++)
            {
                if (m_SocietyJobs[a].m_SocietiesGroupType == societyGroupType)
                    m_SocietiesJobs.Add(m_SocietyJobs[a]);
            }

            return m_SocietiesJobs;
        }

        public static int GetSocietyGroupMonthlyRank(PlayerMobile player, SocietiesGroupType societyGroupType)
        {
            int rank = 1;

            //TEST
            return Utility.RandomMinMax(1, 125);

            return rank;
        }

        public static int GetSocietyGroupLifetimeRank(PlayerMobile player, SocietiesGroupType societyGroupType)
        {
            int rank = 1;

            //TEST
            return Utility.RandomMinMax(1, 125);

            return rank;
        }

        public static void OnLogin(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateSocietiesPlayerSettings(player);
        }

        public static void CheckCreateSocietiesPlayerSettings(PlayerMobile player)
        {
            if (player == null)
                return;

            if (player.m_SocietiesPlayerSettings == null)
                player.m_SocietiesPlayerSettings = new SocietiesPlayerSettings(player);

            else if (player.m_SocietiesPlayerSettings.Deleted)
                player.m_SocietiesPlayerSettings = new SocietiesPlayerSettings(player);
        }

        public static SocietyJobPlayerProgress GetSocietiesJobPlayerProgress(PlayerMobile player, SocietyJob job)
        {
            SocietyJobPlayerProgress jobPlayerProgress = null;

            if (player == null) return jobPlayerProgress;
            if (job == null) return jobPlayerProgress;

            CheckCreateSocietiesPlayerSettings(player);

            foreach (SocietyJobPlayerProgress jobPlayerProgressEntry in player.m_SocietiesPlayerSettings.m_JobProgress)
            {
                if (jobPlayerProgressEntry == null) continue;
                if (jobPlayerProgressEntry.m_Job == job)
                    return jobPlayerProgressEntry;
            }

            return jobPlayerProgress;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0
            writer.Write(NextJobsAdded);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                NextJobsAdded = reader.ReadDateTime();
            }
        }
    }

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

        public enum CreatureModifier
        {
            Normal,
            Paragon,
            Rare
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
        public double m_PrimaryNumber;
        public double m_SecondaryNumber;
        public Quality m_Quality;
        public CraftResource m_CraftResource;
        public CreatureModifier m_PrimaryCreatureModifier;
        public CreatureModifier m_SecondaryCreatureModifier;
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
                case JobType.CraftItem: description = "Craft " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.KillCreature: description = "Kill " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.RetrieveFish: description = "Catch " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.SinkShip: description = "Sink " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.StealItem: description = "Steal " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName; break;
                case JobType.TameCreature: description = "Tame " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName; break;
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
                primaryNumber = jobPlayerProgress.m_PrimaryProgress;
                secondaryNumber = jobPlayerProgress.m_SecondaryProgress;
            }

            switch (m_JobType)
            {
                case JobType.CraftItem: description =  primaryNumber.ToString() + " / " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName + " Crafted"; break;
                case JobType.KillCreature: description = primaryNumber.ToString() + " / " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName + " Killed"; break;
                case JobType.RetrieveFish: description = primaryNumber.ToString() + " / " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName + " Caught"; break;
                case JobType.SinkShip: description = primaryNumber.ToString() + " / " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName + " Sunk"; break;
                case JobType.StealItem: description = primaryNumber.ToString() + " / " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName + " Stolen"; break;
                case JobType.TameCreature: description = primaryNumber.ToString() + " / " + m_PrimaryNumber.ToString() + " " + m_PrimaryTypeName + " Tamed"; break;
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
                return "(" + m_PointValue.ToString() + " Society Point Earned)";

            else
                return "(" + m_PointValue.ToString() + " Society Points Earned)";

            return description;
        }

        public bool HasNeccessaryItems(PlayerMobile player)
        {
            return false;
        }

        public void ConsumeNeccesaryItems(PlayerMobile player)
        {
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
            if (player == null)  return;
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

            writer.Write(m_PrimaryNumber);
            writer.Write(m_SecondaryNumber);
            writer.Write((int)m_Quality);
            writer.Write((int)m_CraftResource);
            writer.Write((int)m_PrimaryCreatureModifier);
            writer.Write((int)m_SecondaryCreatureModifier);

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

                m_PrimaryNumber = reader.ReadDouble();
                m_SecondaryNumber = reader.ReadDouble();
                m_Quality = (Quality)reader.ReadInt();
                m_CraftResource = (CraftResource)reader.ReadInt();
                m_PrimaryCreatureModifier = (CreatureModifier)reader.ReadInt();
                m_SecondaryCreatureModifier = (CreatureModifier)reader.ReadInt();

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

    public class SocietiesPlayerSettings : Item
    {
        public PlayerMobile m_Player;
        public List<SocietyJobPlayerProgress> m_JobProgress = new List<SocietyJobPlayerProgress>();
        public List<SocietyGroupPlayerData> m_SocietyPlayerData = new List<SocietyGroupPlayerData>();

        [Constructable]
        public SocietiesPlayerSettings(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Player = player;

            if (player == null)
                Delete();

            else
            {
                m_Player.m_SocietiesPlayerSettings = this;
                CreateSocietyData();
            }
        }

        public SocietiesPlayerSettings(Serial serial): base(serial)
        {
        }

        public SocietyGroupPlayerData GetSocietyGroupPlayerData(SocietiesGroupType societyGroupType)
        {
            foreach (SocietyGroupPlayerData societyPlayerData in m_SocietyPlayerData)
            {
                if (societyPlayerData == null)
                    continue;

                if (societyPlayerData.m_SocietyGroupType == societyGroupType)
                    return societyPlayerData;
            }

            return null;
        }        

        public void CreateSocietyData()
        {
            int societyTypesCount = Enum.GetNames(typeof(SocietiesGroupType)).Length;

            for (int a = 0; a < societyTypesCount; a++)
            {
                SocietiesGroupType societyGroupType = (SocietiesGroupType)a;
                SocietyGroupPlayerData societyPlayerData = new SocietyGroupPlayerData(societyGroupType);

                m_SocietyPlayerData.Add(societyPlayerData);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Player);

            writer.Write(m_JobProgress.Count);
            for (int a = 0; a < m_JobProgress.Count; a++)
            {
                writer.Write(m_JobProgress[a].m_Job);
                writer.Write(m_JobProgress[a].m_JobContract);
                writer.Write(m_JobProgress[a].m_PrimaryProgress);
                writer.Write(m_JobProgress[a].m_SecondaryProgress);
            }

            writer.Write(m_SocietyPlayerData.Count);
            for (int a = 0; a < m_SocietyPlayerData.Count; a++)
            {
                writer.Write((int)m_SocietyPlayerData[a].m_SocietyGroupType);
                writer.Write(m_SocietyPlayerData[a].m_PointsAvailable);
                writer.Write(m_SocietyPlayerData[a].m_MontlyPoints);
                writer.Write(m_SocietyPlayerData[a].m_LifetimePoints);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            CreateSocietyData();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;

                int jobProgressCount = reader.ReadInt();
                for (int a = 0; a < jobProgressCount; a++)
                {
                    SocietyJob job = reader.ReadItem() as SocietyJob;
                    SocietiesJobContract jobContract = reader.ReadItem() as SocietiesJobContract;
                    double primaryProgress = reader.ReadDouble();
                    double secondaryProgress = reader.ReadDouble();

                    if (job == null) continue;
                    if (job.Deleted) continue;

                    SocietyJobPlayerProgress jobProgress = new SocietyJobPlayerProgress(job);
                    jobProgress.m_JobContract = jobContract;
                    jobProgress.m_PrimaryProgress = primaryProgress;
                    jobProgress.m_SecondaryProgress = secondaryProgress;

                    m_JobProgress.Add(jobProgress);                    
                }                

                int recordedSocietyGroupDataCount = reader.ReadInt();
                for (int a = 0; a < recordedSocietyGroupDataCount; a++)
                {
                    SocietiesGroupType societyGroupType = (SocietiesGroupType)reader.ReadInt();
                    int pointsAvailable = reader.ReadInt();
                    int monthlyPoints = reader.ReadInt();
                    int lifetimePoints = reader.ReadInt();

                    foreach(SocietyGroupPlayerData societyGroupPlayerData in m_SocietyPlayerData)
                    {
                        if (societyGroupPlayerData.m_SocietyGroupType == societyGroupType)
                        {
                            societyGroupPlayerData.m_PointsAvailable = pointsAvailable;
                            societyGroupPlayerData.m_MontlyPoints = monthlyPoints;
                            societyGroupPlayerData.m_LifetimePoints = lifetimePoints;

                            break;
                        }
                    }
                }
            }

            //-----

            if (m_Player != null)
                m_Player.m_SocietiesPlayerSettings = this;

            else
                Delete();
        }
    }

    public class SocietyGroupPlayerData
    {
        public SocietiesGroupType m_SocietyGroupType;

        public int m_PointsAvailable = 0;
        public int m_MontlyPoints = 0;
        public int m_LifetimePoints = 0;

        public SocietyGroupPlayerData(SocietiesGroupType societyGroupType)
        {
            m_SocietyGroupType = societyGroupType;
        }
    }

    public class SocietyJobPlayerProgress
    {
        public SocietyJob m_Job;
        public SocietiesJobContract m_JobContract = null;

        public double m_PrimaryProgress = 0.0;
        public double m_SecondaryProgress = 0.0;
        
        public SocietyJobPlayerProgress(SocietyJob job)
        {
            m_Job = job;
        }
    }

    public class SocietiesPersistanceItem : Item
    {
        public override string DefaultName { get { return "SocietiesPersistance"; } }

        public SocietiesPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public SocietiesPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            Societies.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            Societies.PersistanceItem = this;
            Societies.Deserialize(reader);
        }
    }
}
