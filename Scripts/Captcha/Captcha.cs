using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;

namespace Server.Items
{
    public enum CaptchaSourceType
    {
        Mining,
        Lumberjacking,
        Fishing,
        DungeonChest
    }

    public class CaptchaPersistance
    {
        public static CaptchaPersistanceItem PersistanceItem;

        public static List<CaptchaAccountData> m_AccountEntries = new List<CaptchaAccountData>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new CaptchaPersistanceItem();
            });
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckAndCreateCaptchaAccountEntry(player);
        }

        public static void CheckAndCreateCaptchaAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            if (player.m_CaptchaAccountData == null)
                CreateCaptchaAccountEntry(player);

            if (player.m_CaptchaAccountData.Deleted)
                CreateCaptchaAccountEntry(player);
        }

        public static void CreateCaptchaAccountEntry(PlayerMobile player)
        {
            if (player == null)
                return;

            string accountName = player.Account.Username;

            CaptchaAccountData captchaAccountEntry = null;
            
            foreach (CaptchaAccountData entry in m_AccountEntries)
            {
                if (entry.m_AccountName == accountName)
                {
                    player.m_CaptchaAccountData = entry;

                    return;
                }
            }
            
            CaptchaAccountData newEntry = new CaptchaAccountData(accountName);

            Account account = player.Account as Account;

            for (int a = 0; a < account.accountMobiles.Length; a++)
            {
                Mobile mobile = account.accountMobiles[a] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)
                        pm_Mobile.m_CaptchaAccountData = newEntry;
                }
            }            
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

    public class CaptchaPersistanceItem : Item
    {
        public override string DefaultName { get { return "CaptchaPersistance"; } }

        public CaptchaPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public CaptchaPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            CaptchaPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            CaptchaPersistance.PersistanceItem = this;
            CaptchaPersistance.Deserialize(reader);
        }
    }

    public class CaptchaAccountData: Item
    {
        public enum PenaltyLevelType
        {
            None,
            Minor,
            Major,
            Epic,
            Permanent
        }
        
        public string m_AccountName;

        public CaptchaSourceType m_CaptchaSourceType = CaptchaSourceType.Mining;

        public DateTime m_LastPrompt = DateTime.UtcNow;
        public bool m_CaptchaRequired = false;
        public int m_CaptchaAttempt = 0;
        public bool m_ConfirmPrompt = false;

        public PenaltyLevelType m_CurrentPenalty = PenaltyLevelType.None;
        public DateTime m_CurrentPenaltyExpiration = DateTime.UtcNow;

        public PenaltyLevelType m_PreviousPenalty = PenaltyLevelType.None;
        public DateTime m_PenaltyProbationExpiration = DateTime.UtcNow;

        public int m_SelectedRow1Index = 0;
        public int m_SelectedRow2Index = 0;
        public int m_SelectedRow3Index = 0;

        public int m_Row1CorrectIndex;
        public int m_Row2CorrectIndex;
        public int m_Row3CorrectIndex;

        public List<int> m_Row1IDs = new List<int>();
        public List<int> m_Row2IDs = new List<int>();
        public List<int> m_Row3IDs = new List<int>();

        public static int ItemsPerRow = 5;

        [Constructable]
        public CaptchaAccountData(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountName = accountName;

            //-----

            CaptchaPersistance.m_AccountEntries.Add(this);
        }

        public CaptchaAccountData(Serial serial): base(serial)
        {
        }

        public bool Attempt(PlayerMobile player, CaptchaSourceType captchaSource)
        {
            if (m_CurrentPenalty != PenaltyLevelType.None && DateTime.UtcNow >= m_CurrentPenaltyExpiration)
                m_CurrentPenalty = PenaltyLevelType.None;

            if (m_PreviousPenalty != null && DateTime.UtcNow >= m_PenaltyProbationExpiration)            
                m_PreviousPenalty = PenaltyLevelType.None;

            if (m_CurrentPenalty != PenaltyLevelType.None)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_CurrentPenaltyExpiration, false, true, true, true, false);

                player.SendMessage(2115, "Due to previous captcha failures, your account is not allowed to gather resources for another " + timeRemaining + ".");

                return false;
            }            

            if (m_CaptchaRequired)
            {
                m_CaptchaSourceType = captchaSource;

                m_ConfirmPrompt = false;

                player.SendSound(0x055);               

                player.CloseGump(typeof(CaptchaGump));
                player.SendGump(new CaptchaGump(player));

                return false;
            }

            else
            {
                //Check If Captcha Needed

                //TEST
                bool launchCaptcha = true;

                if (launchCaptcha)
                {
                    GenerateIDs();

                    player.SendSound(0x055);

                    m_LastPrompt = DateTime.UtcNow;
                    m_CaptchaRequired = true;
                    m_CaptchaAttempt = 0;
                    m_ConfirmPrompt = false;

                    m_CaptchaSourceType = captchaSource;

                    player.CloseGump(typeof(CaptchaGump));
                    player.SendGump(new CaptchaGump(player));

                    return false;
                }

                else
                    return true;
            }
        }

        public static TimeSpan GetPenaltyDuration(PenaltyLevelType penaltyLevelType)
        {
            switch (penaltyLevelType)
            {
                case PenaltyLevelType.None:
                    return TimeSpan.FromHours(0);
                break;

                case PenaltyLevelType.Minor:
                    return TimeSpan.FromHours(6);
                break;

                case PenaltyLevelType.Major:
                    return TimeSpan.FromHours(24);
                break;

                case PenaltyLevelType.Epic:
                    return TimeSpan.FromDays(7);
                break;

                case PenaltyLevelType.Permanent:
                    return TimeSpan.FromDays(1000);
                break;
            }

            return TimeSpan.FromHours(0);
        }

        public static TimeSpan GetPenaltyProbationDuration(PenaltyLevelType penaltyLevelType)
        {
            switch (penaltyLevelType)
            {
                case PenaltyLevelType.None:
                    return TimeSpan.FromHours(0);
                break;

                case PenaltyLevelType.Minor:
                    return TimeSpan.FromHours(24);
                break;

                case PenaltyLevelType.Major:
                    return TimeSpan.FromDays(7);
                break;

                case PenaltyLevelType.Epic:
                    return TimeSpan.FromDays(30);
                break;

                case PenaltyLevelType.Permanent:
                    return TimeSpan.FromDays(1000);
                break;
            }

            return TimeSpan.FromHours(0);
        }

        public static List<int> GetIDList()
        {
            List<int> list = new List<int>()
            { 
                525, 571, 731, 2330, 2431,
                2451, 2471, 2473, 2476, 2500,
                2505, 2537, 2581, 2598, 2599,
                2648, 2886, 3271, 3347, 3367, 
                3385, 3542, 3567, 3589, 3612,
                3619, 3629, 3638, 3652, 3700,
                3707, 3740, 3762, 3763, 3823,
                3834, 3850, 3916, 3920, 3935,
                3980, 3976, 4006, 4009, 4029,
                4647, 5134, 5136, 5207, 5356,
                5368, 5373, 5363, 5642, 5643,
                5692, 5912, 5921, 5995, 6160, 
                6225, 6231, 6235, 6264, 6464,
                7034, 7131, 7134, 7158, 7781,
                8928, 12585, 13704, 15106, 15793                
            };

            return list;
        }

        public void GetIconOffsets(int idValue, out int offsetX, out int offsetY)
        {
            offsetX = 0;
            offsetY = 0;

            switch (idValue)
            {
                #region ID Values

                case 525: offsetX = 46; offsetY = 33; break;
                case 571: offsetX = 53; offsetY = 43; break;
                case 731: offsetX = 57; offsetY = 33; break;
                case 2330: offsetX = 52; offsetY = 34; break;
                case 2431: offsetX = 49; offsetY = 43; break;

                case 2451: offsetX = 47; offsetY = 37; break;
                case 2471: offsetX = 46; offsetY = 40; break;
                case 2473: offsetX = 46; offsetY = 35; break;
                case 2476: offsetX = 45; offsetY = 35; break;
                case 2500: offsetX = 48; offsetY = 33; break;

                case 2505: offsetX = 51; offsetY = 44; break;
                case 2537: offsetX = 56; offsetY = 39; break;
                case 2581: offsetX = 43; offsetY = 37; break;
                case 2598: offsetX = 51; offsetY = 34; break;
                case 2599: offsetX = 50; offsetY = 29; break;
                  
                case 2648: offsetX = 52; offsetY = 39; break;
                case 2886: offsetX = 47; offsetY = 35; break;
                case 3271: offsetX = 51; offsetY = 29; break;
                case 3347: offsetX = 51; offsetY = 35; break;
                case 3367: offsetX = 50; offsetY = 33; break;

                case 3385: offsetX = 53; offsetY = 37; break;
                case 3542: offsetX = 48; offsetY = 31; break;
                case 3567: offsetX = 50; offsetY = 39; break;
                case 3589: offsetX = 63; offsetY = 37; break;
                case 3612: offsetX = 48; offsetY = 28; break;

                case 3619: offsetX = 53; offsetY = 41; break;
                case 3629: offsetX = 54; offsetY = 37; break;
                case 3638: offsetX = 54; offsetY = 39; break;
                case 3652: offsetX = 54; offsetY = 39; break;
                case 3700: offsetX = 59; offsetY = 40; break;

                case 3707: offsetX = 49; offsetY = 30; break;
                case 3740: offsetX = 54; offsetY = 35; break;
                case 3762: offsetX = 45; offsetY = 36; break;
                case 3763: offsetX = 49; offsetY = 41; break;
                case 3823: offsetX = 49; offsetY = 36; break;

                case 3834: offsetX = 52; offsetY = 40; break;
                case 3850: offsetX = 59; offsetY = 40; break;
                case 3916: offsetX = 57; offsetY = 39; break;
                case 3920: offsetX = 54; offsetY = 39; break;
                case 3935: offsetX = 54; offsetY = 39; break;

                case 3980: offsetX = 50; offsetY = 43; break;
                case 3976: offsetX = 51; offsetY = 40; break;
                case 4006: offsetX = 50; offsetY = 31; break;
                case 4009: offsetX = 53; offsetY = 42; break;
                case 4029: offsetX = 48; offsetY = 37; break;

                case 4647: offsetX = 55; offsetY = 30; break;
                case 5134: offsetX = 50; offsetY = 41; break;
                case 5136: offsetX = 51; offsetY = 36; break;
                case 5207: offsetX = 46; offsetY = 36; break;
                case 5356: offsetX = 53; offsetY = 34; break;

                case 5368: offsetX = 51; offsetY = 38; break;
                case 5373: offsetX = 55; offsetY = 42; break;
                case 5363: offsetX = 54; offsetY = 37; break;
                case 5642: offsetX = 52; offsetY = 44; break;
                case 5643: offsetX = 52; offsetY = 31; break;

                case 5692: offsetX = 53; offsetY = 38; break;
                case 5912: offsetX = 53; offsetY = 40; break;
                case 5921: offsetX = 49; offsetY = 39; break;
                case 5995: offsetX = 53; offsetY = 42; break;
                case 6160: offsetX = 53; offsetY = 37; break;

                case 6225: offsetX = 50; offsetY = 34; break;
                case 6231: offsetX = 51; offsetY = 37; break;
                case 6235: offsetX = 52; offsetY = 34; break;
                case 6264: offsetX = 54; offsetY = 41; break;
                case 6464: offsetX = 65; offsetY = 36; break;                

                case 7034: offsetX = 52; offsetY = 40; break;
                case 7131: offsetX = 50; offsetY = 35; break;
                case 7134: offsetX = 50; offsetY = 35; break;
                case 7158: offsetX = 50; offsetY = 35; break;
                case 7781: offsetX = 46; offsetY = 32; break;

                case 8928: offsetX = 58; offsetY = 30; break;
                case 12585: offsetX = 52; offsetY = 26; break;
                case 13704: offsetX = 62; offsetY = 28; break;
                case 15106: offsetX = 49; offsetY = 41; break;
                case 15793: offsetX = 53; offsetY = 29; break;

                #endregion
            }
        }

        public void GenerateIDs()
        {
            m_Row1IDs.Clear();
            m_Row2IDs.Clear();
            m_Row3IDs.Clear();            

            //Row 1
            List<int> m_PossibleIDs = GetIDList();

            for (int a = 0; a < ItemsPerRow; a++)
            {
                int addedID = m_PossibleIDs[Utility.RandomMinMax(0, m_PossibleIDs.Count - 1)];

                if (m_PossibleIDs.Contains(addedID))
                    m_PossibleIDs.Remove(addedID);

                if (!m_Row1IDs.Contains(addedID))
                    m_Row1IDs.Add(addedID);
            }

            //Row 2
            m_PossibleIDs = GetIDList();

            for (int a = 0; a < ItemsPerRow; a++)
            {
                int addedID = m_PossibleIDs[Utility.RandomMinMax(0, m_PossibleIDs.Count - 1)];

                if (m_PossibleIDs.Contains(addedID))
                    m_PossibleIDs.Remove(addedID);

                if (!m_Row2IDs.Contains(addedID))
                    m_Row2IDs.Add(addedID);
            }

            //Row 3
            m_PossibleIDs = GetIDList();

            for (int a = 0; a < ItemsPerRow; a++)
            {
                int addedID = m_PossibleIDs[Utility.RandomMinMax(0, m_PossibleIDs.Count - 1)];

                if (m_PossibleIDs.Contains(addedID))
                    m_PossibleIDs.Remove(addedID);

                if (!m_Row3IDs.Contains(addedID))
                    m_Row3IDs.Add(addedID);
            }

            m_SelectedRow1Index = 0;
            m_SelectedRow2Index = 0;
            m_SelectedRow3Index = 0;

            m_Row1CorrectIndex = Utility.RandomMinMax(0, ItemsPerRow - 1);
            m_Row2CorrectIndex = Utility.RandomMinMax(0, ItemsPerRow - 1);
            m_Row3CorrectIndex = Utility.RandomMinMax(0, ItemsPerRow - 1);            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_AccountName);
            writer.Write((int)m_CaptchaSourceType);
            writer.Write(m_LastPrompt);
            writer.Write(m_CaptchaRequired);
            writer.Write(m_CaptchaAttempt);
            writer.Write(m_ConfirmPrompt);
            writer.Write((int)m_CurrentPenalty);
            writer.Write(m_CurrentPenaltyExpiration);
            writer.Write((int)m_PreviousPenalty);
            writer.Write(m_PenaltyProbationExpiration);

            writer.Write(m_SelectedRow1Index);
            writer.Write(m_SelectedRow2Index);
            writer.Write(m_SelectedRow3Index);

            writer.Write(m_Row1CorrectIndex);
            writer.Write(m_Row2CorrectIndex);
            writer.Write(m_Row3CorrectIndex);

            writer.Write(m_Row1IDs.Count);
            for (int a = 0; a < m_Row1IDs.Count; a++)
            {
                writer.Write(m_Row1IDs[a]);
            }

            writer.Write(m_Row2IDs.Count);
            for (int a = 0; a < m_Row2IDs.Count; a++)
            {
                writer.Write(m_Row2IDs[a]);
            }

            writer.Write(m_Row2IDs.Count);
            for (int a = 0; a < m_Row2IDs.Count; a++)
            {
                writer.Write(m_Row2IDs[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_AccountName = reader.ReadString();
                m_CaptchaSourceType = (CaptchaSourceType)reader.ReadInt();
                m_LastPrompt = reader.ReadDateTime();
                m_CaptchaRequired = reader.ReadBool();
                m_CaptchaAttempt = reader.ReadInt();
                m_ConfirmPrompt = reader.ReadBool();
                m_CurrentPenalty = (PenaltyLevelType)reader.ReadInt();
                m_CurrentPenaltyExpiration = reader.ReadDateTime();
                m_PreviousPenalty = (PenaltyLevelType)reader.ReadInt();
                m_PenaltyProbationExpiration = reader.ReadDateTime();

                m_SelectedRow1Index = reader.ReadInt();
                m_SelectedRow2Index = reader.ReadInt();
                m_SelectedRow3Index = reader.ReadInt();

                m_Row1CorrectIndex = reader.ReadInt();
                m_Row2CorrectIndex = reader.ReadInt();
                m_Row3CorrectIndex = reader.ReadInt();

                int row1Count = reader.ReadInt();
                for (int a = 0; a < row1Count; a++)
                {
                    m_Row1IDs.Add(reader.ReadInt());
                }

                int row2Count = reader.ReadInt();
                for (int a = 0; a < row2Count; a++)
                {
                    m_Row2IDs.Add(reader.ReadInt());
                }

                int row3Count = reader.ReadInt();
                for (int a = 0; a < row3Count; a++)
                {
                    m_Row3IDs.Add(reader.ReadInt());
                }
            }

            //-----

            CaptchaPersistance.m_AccountEntries.Add(this);
        }
    }
}