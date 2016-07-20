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
    public enum ProfessionGroupType
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

    public enum ProfessionGroupPageDisplayType
    {
        Jobs,
        SpendPoints,
        ServerRankings
    }

    public static class ProfessionBoard
    {
        public static ProfessionBoardPersistanceItem PersistanceItem;

        public static List<ProfessionJob> m_ProfessionJobs = new List<ProfessionJob>();

        public static TimeSpan JobExpiration = TimeSpan.FromHours(72);

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new ProfessionBoardPersistanceItem();
            });
        }

        public static string GetProfessionGroupName(ProfessionGroupType professionGroupType)
        {
            switch (professionGroupType)
            {
                case ProfessionGroupType.AdventurersLodge: return "Adventurer's Lodge"; break;
                case ProfessionGroupType.ArtificersEnclave: return "Artificer's Enclave"; break;
                case ProfessionGroupType.FarmersCooperative: return "Farmer's Cooperative"; break;
                case ProfessionGroupType.FishermansCircle: return "Fisherman's Circle"; break;
                case ProfessionGroupType.MonsterHuntersSociety: return "Monster Hunter's Society"; break;
                case ProfessionGroupType.SeafarersLeague: return "Seafarer's League"; break;
                case ProfessionGroupType.SmithingOrder: return "Smithing Order"; break;
                case ProfessionGroupType.ThievesGuild: return "Thieves Guild"; break;
                case ProfessionGroupType.TradesmanUnion: return "Tradesman Union"; break;
                case ProfessionGroupType.ZoologicalFoundation: return "Zoological Foundation"; break;  
            }

            return "";
        }

        public static int GetProfessionGroupTextHue(ProfessionGroupType professionGroupType)
        {
            switch (professionGroupType)
            {
                case ProfessionGroupType.AdventurersLodge: return 152; break;
                case ProfessionGroupType.ArtificersEnclave: return 2606; break;
                case ProfessionGroupType.FarmersCooperative: return 2599; break;
                case ProfessionGroupType.FishermansCircle: return 2590; break;
                case ProfessionGroupType.MonsterHuntersSociety: return 2116; break;
                case ProfessionGroupType.SeafarersLeague: return 2603; break;
                case ProfessionGroupType.SmithingOrder: return 2401; break;
                case ProfessionGroupType.ThievesGuild: return 2036; break;
                case ProfessionGroupType.TradesmanUnion: return 2417; break;
                case ProfessionGroupType.ZoologicalFoundation: return 63; break;
            }

            return 2499;
        }

        public static void OnLogin(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateProfessionPlayerSettings(player);
        }

        public static void CheckCreateProfessionPlayerSettings(PlayerMobile player)
        {
            if (player == null)
                return;

            if (player.m_ProfessionPlayerSettings == null)
                player.m_ProfessionPlayerSettings = new ProfessionPlayerSettings(player);

            else if (player.m_ProfessionPlayerSettings.Deleted)
                player.m_ProfessionPlayerSettings = new ProfessionPlayerSettings(player);
        }

        public static ProfessionJobPlayerProgress GetProfessionJobPlayerProgress(PlayerMobile player, ProfessionJob professionJob)
        {
            ProfessionJobPlayerProgress jobPlayerProgress = null;

            if (player == null) return jobPlayerProgress;
            if (professionJob == null) return jobPlayerProgress;

            CheckCreateProfessionPlayerSettings(player);

            foreach (ProfessionJobPlayerProgress jobPlayerProgressEntry in player.m_ProfessionPlayerSettings.m_JobProgress)
            {
                if (jobPlayerProgressEntry == null) continue;
                if (jobPlayerProgressEntry.m_ProfessionJob == professionJob)
                    return jobPlayerProgressEntry;
            }

            return jobPlayerProgress;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version
            //Version 0
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class ProfessionJob : Item
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

        public DateTime m_Expiration = DateTime.UtcNow + ProfessionBoard.JobExpiration;

        public int m_PointValue = 1;

        public ProfessionGroupType m_ProfessionGroupType = ProfessionGroupType.AdventurersLodge;
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
        public ProfessionJob(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public ProfessionJob(Serial serial): base(serial)
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
            if (player == null) 
                return;

            if (AccountHasCompleted(player))
            {
                //Another Player on Account Already Completed
                return;
            }

            ProfessionJobPlayerProgress playerJobProgress = ProfessionBoard.GetProfessionJobPlayerProgress(player, this);

            if (playerJobProgress == null)
            {
                playerJobProgress = new ProfessionJobPlayerProgress(player, this);
                player.m_ProfessionPlayerSettings.m_JobProgress.Add(playerJobProgress);

                //SendMessage
                //Add Job Item
            }

            else
            {
                //Already Accepted
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }

            //-----

            if (DateTime.UtcNow >= m_Expiration)
                Delete();

            else
                ProfessionBoard.m_ProfessionJobs.Add(this);
        }
    }

    public class ProfessionPlayerSettings : Item
    {
        public PlayerMobile m_Player;
        public List<ProfessionJobPlayerProgress> m_JobProgress = new List<ProfessionJobPlayerProgress>();

        [Constructable]
        public ProfessionPlayerSettings(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Player = player;

            if (player == null)
                Delete();

            else
                m_Player.m_ProfessionPlayerSettings = this;
        }

        public ProfessionPlayerSettings(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class ProfessionJobPlayerProgress : Item
    {
        public PlayerMobile m_Player;
        public ProfessionJob m_ProfessionJob;

        public double m_ProgressValue1 = 0.0;
        public double m_ProgressValue2 = 0.0;
        public double m_ProgressValue3 = 0.0;

        [Constructable]
        public ProfessionJobPlayerProgress(PlayerMobile player, ProfessionJob professionJob): base(0x0)
        {
            m_Player = player;
            m_ProfessionJob = professionJob;

            Visible = false;
            Movable = false;
        }

        public ProfessionJobPlayerProgress(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Player);
            writer.Write(m_ProfessionJob);
            writer.Write(m_ProgressValue1);
            writer.Write(m_ProgressValue2);
            writer.Write(m_ProgressValue3);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;
                m_ProfessionJob = reader.ReadItem() as ProfessionJob;
                m_ProgressValue1 = reader.ReadDouble();
                m_ProgressValue2 = reader.ReadDouble();
                m_ProgressValue3 = reader.ReadDouble();
            }

            //-----

            if (m_Player == null || m_ProfessionJob == null)
                Delete();

            else if (m_Player.Deleted || m_ProfessionJob.Deleted)
                Delete();
        }
    }

    public class ProfessionBoardPersistanceItem : Item
    {
        public override string DefaultName { get { return "ProfessionBoardPersistance"; } }

        public ProfessionBoardPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public ProfessionBoardPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            ProfessionBoard.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            ProfessionBoard.PersistanceItem = this;
            ProfessionBoard.Deserialize(reader);
        }
    }
}
