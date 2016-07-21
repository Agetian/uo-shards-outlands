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

        public static TimeSpan JobDuration = TimeSpan.FromHours(72);
        public static DateTime NextJobsReset = DateTime.UtcNow;

        public static TimeSpan TickDuration = TimeSpan.FromMinutes(15);

        public static Timer m_Timer;

        public static void Initialize()
        {
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

                if (DateTime.UtcNow >= NextJobsReset)
                    JobsReset();
            }
        }

        public static void JobsReset()
        {
            NextJobsReset = NextJobsReset + JobDuration;

            DeleteAllJobs();
            GenerateAllJobs();
        }

        public static void DeleteAllJobs()
        {
            Queue m_Queue = new Queue();

            foreach (Item item in World.Items.Values)
            {
                if (item is SocietyJob)
                    m_Queue.Enqueue(item);               
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();

                item.Delete();
            }

            foreach (Mobile mobile in World.Mobiles.Values)
            {
                if (!mobile.Player) 
                    continue;

                PlayerMobile player = mobile as PlayerMobile;

                CheckCreateSocietiesPlayerSettings(player);

                player.m_SocietiesPlayerSettings.m_JobProgress.Clear();
            }

            m_SocietyJobs.Clear();
        }

        public static void GenerateAllJobs()
        {
            //TEST
            SocietyJob newJob = new SocietyJob();
            newJob.m_SocietiesGroupType = SocietiesGroupType.ArtificersEnclave;
            newJob.m_PrimaryNumber = 150;
            newJob.m_Expiration = Societies.NextJobsReset;
            newJob.m_PointValue = 1;

            m_SocietyJobs.Add(newJob);

            //TEST
            newJob = new SocietyJob();
            newJob.m_SocietiesGroupType = SocietiesGroupType.ArtificersEnclave;
            newJob.m_PrimaryNumber = 300;
            newJob.m_Expiration = Societies.NextJobsReset;
            newJob.m_PointValue = 2;

            m_SocietyJobs.Add(newJob);
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
            writer.Write(NextJobsReset);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                NextJobsReset = reader.ReadDateTime();
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
            TameCreature,
            TameCreatureThenKillCreature
        }

        public enum CreatureModifier
        {
            Normal,
            Paragon,
            Rare
        }

        public DateTime m_Expiration = DateTime.UtcNow + Societies.JobDuration;

        public int m_PointValue = 1;

        public SocietiesGroupType m_SocietiesGroupType = SocietiesGroupType.AdventurersLodge;
        public JobType m_JobType = JobType.CraftItem;
        public Type m_PrimaryType;
        public Type m_SecondaryType;
        public double m_PrimaryNumber;
        public double m_SecondaryNumber;
        public Quality m_Quality;
        public CraftResource m_CraftResource;
        public CreatureModifier m_PrimaryCreatureModifier;
        public CreatureModifier m_SecondaryCreatureModifier;

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

            return description;
        }

        public string GetJobProgressText()
        {
            string description = "";

            //TEST
            description = "Craft " + m_PrimaryNumber.ToString() + " Cure Potions";

            return description;
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
                    SocietiesJobContract jobContract = new SocietiesJobContract(this);

                    player.Backpack.DropItem(jobContract);

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
            writer.Write(m_Expiration);
            writer.Write(m_PointValue);
            writer.Write((int)m_SocietiesGroupType);
            writer.Write((int)m_JobType);

            if (m_PrimaryType == null)
                writer.Write("null");
            else
                writer.Write((string)m_PrimaryType.ToString());

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
                m_Expiration = reader.ReadDateTime();
                m_PointValue = reader.ReadInt();
                m_SocietiesGroupType = (SocietiesGroupType)reader.ReadInt();
                m_JobType = (JobType)reader.ReadInt();

                string primaryType = reader.ReadString();
                if (primaryType == "null")
                    m_PrimaryType = null;
                else
                    m_PrimaryType = Type.GetType(primaryType);

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

        [Constructable]
        public SocietiesPlayerSettings(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Player = player;

            if (player == null)
                Delete();

            else
                m_Player.m_SocietiesPlayerSettings = this;
        }

        public SocietiesPlayerSettings(Serial serial): base(serial)
        {
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
                writer.Write(m_JobProgress[a].m_PrimaryProgress);
                writer.Write(m_JobProgress[a].m_SecondaryProgress);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;

                int jobProgressCount = reader.ReadInt();
                for (int a = 0; a < jobProgressCount; a++)
                {
                    SocietyJob job = reader.ReadItem() as SocietyJob;
                    double primaryProgress = reader.ReadDouble();
                    double secondaryProgress = reader.ReadDouble();

                    if (job == null) continue;
                    if (job.Deleted) continue;
                    if (job.m_Expiration <= DateTime.UtcNow) continue;

                    SocietyJobPlayerProgress jobProgress = new SocietyJobPlayerProgress(job);
                    jobProgress.m_PrimaryProgress = primaryProgress;
                    jobProgress.m_SecondaryProgress = secondaryProgress;

                    m_JobProgress.Add(jobProgress);                    
                }
            }
        }
    }

    public class SocietyJobPlayerProgress
    {
        public SocietyJob m_Job;

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
