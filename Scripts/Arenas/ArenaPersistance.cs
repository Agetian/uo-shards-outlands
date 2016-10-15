﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;

namespace Server
{
    public static class ArenaPersistance
    {
        public static ArenaPersistanceItem PersistanceItem;

        public static TimeSpan MatchResultExpiration = TimeSpan.FromHours(24);
        public static TimeSpan TournamentMatchResultExpiration = TimeSpan.FromDays(30);

        public static List<ArenaAccountEntry> m_ArenaAccountEntries = new List<ArenaAccountEntry>();
        public static List<ArenaMatchResultEntry> m_ArenaMatchResultEntries = new List<ArenaMatchResultEntry>();

        public static Timer m_Timer;
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new ArenaPersistanceItem();               
            });

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {                
                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }

                m_Timer = new ArenaPersistanceTimer();
                m_Timer.Start();                
            });
        }

        public class ArenaPersistanceTimer : Timer
        {
            public ArenaPersistanceTimer(): base(TimeSpan.Zero, TimeSpan.FromHours(12))
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                Queue m_Queue = new Queue();

                foreach (ArenaMatchResultEntry arenaMatchResultEntry in m_ArenaMatchResultEntries)
                {
                    if (arenaMatchResultEntry == null) continue;
                    if (arenaMatchResultEntry.Deleted) continue;

                    TimeSpan expirationLength = MatchResultExpiration;

                    //Test: Add Tournament Expiration

                    if (arenaMatchResultEntry.m_CompletionDate + expirationLength <= DateTime.UtcNow)                    
                        m_Queue.Enqueue(arenaMatchResultEntry);                    
                }

                while (m_Queue.Count > 0)
                {
                    ArenaMatchResultEntry entryResult = (ArenaMatchResultEntry)m_Queue.Dequeue();

                    if (entryResult != null)
                        entryResult.Delete();
                }
            }
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckAndCreateArenaAccountEntry(player);
        }

        public static void CheckAndCreateArenaAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            if (player.m_ArenaAccountEntry == null)
                CreateArenaAccountEntry(player);

            if (player.m_ArenaAccountEntry.Deleted)
                CreateArenaAccountEntry(player);
        }

        public static void CreateArenaAccountEntry(PlayerMobile player)
        {
            if (player == null)
                return;

            string accountName = player.Account.Username;

            ArenaAccountEntry arenaAccountEntry = null;

            bool foundEntry = false;

            foreach (ArenaAccountEntry entry in m_ArenaAccountEntries)
            {
                if (entry.m_AccountUsername == accountName)
                {
                    player.m_ArenaAccountEntry = entry;
                    foundEntry = true;

                    return;
                }
            }

            if (!foundEntry)
            {
                arenaAccountEntry = new ArenaAccountEntry(accountName);

                Account account = player.Account as Account;

                for (int a = 0; a < account.accountMobiles.Length; a++)
                {
                    Mobile mobile = account.accountMobiles[a] as Mobile;

                    if (mobile != null)
                    {
                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                            pm_Mobile.m_ArenaAccountEntry = arenaAccountEntry;
                    }
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

    public class ArenaPersistanceItem : Item
    {
        public override string DefaultName { get { return "ArenaPersistance"; } }

        public ArenaPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public ArenaPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            ArenaPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            ArenaPersistance.PersistanceItem = this;
            ArenaPersistance.Deserialize(reader);
        }
    }

    public class ArenaAccountEntry: Item
    {
        public string m_AccountUsername = "";

        public int m_ArenaCredits = 0;
        
        [Constructable]
        public ArenaAccountEntry(string accountName): base(0x0)
        {
            m_AccountUsername = accountName;

            //-----    

            ArenaPersistance.m_ArenaAccountEntries.Add(this);

            Visible = false;
            Movable = false;
        }

        public ArenaAccountEntry(Serial serial): base(serial)
        {
        }

        public override void OnDelete()
        {
            if (ArenaPersistance.m_ArenaAccountEntries.Contains(this))
                ArenaPersistance.m_ArenaAccountEntries.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_AccountUsername);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();           

            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();
            }

            //-----

            ArenaPersistance.m_ArenaAccountEntries.Add(this);
        }
    }

    public class ArenaMatchResultEntry : Item
    {
        public enum ArenaMatchResultStatusType
        {
            Waiting,
            Fighting,
            Completed
        }

        public ArenaMatch m_ArenaMatch;

        public ArenaMatchResultStatusType m_MatchStatus = ArenaMatchResultStatusType.Waiting;
        public DateTime m_CompletionDate = DateTime.UtcNow;
        public ArenaRuleset.MatchTypeType m_MatchType = ArenaRuleset.MatchTypeType.Ranked1vs1;
        public TimeSpan m_MatchDuration = TimeSpan.FromSeconds(0);
        public string m_WinningTeam = "";

        public List<ArenaMatchTeamResultEntry> m_TeamResultEntries = new List<ArenaMatchTeamResultEntry>();

        [Constructable]
        public ArenaMatchResultEntry(): base(0x0)
        {
            ArenaPersistance.m_ArenaMatchResultEntries.Add(this);

            Visible = false;
            Movable = false;
        }

        public ArenaMatchResultEntry(Serial serial): base(serial)
        {
        }

        public ArenaMatchPlayerResultEntry GetPlayerMatchResultEntry(PlayerMobile player)
        {
            foreach (ArenaMatchTeamResultEntry teamResult in m_TeamResultEntries)
            {
                if (teamResult == null)
                    continue;

                foreach (ArenaMatchPlayerResultEntry playerResult in teamResult.m_PlayerResultEntries)
                {
                    if (playerResult == null)
                        continue;

                    if (playerResult.m_Player == player)
                        return playerResult;
                }
            }

            return null;
        }

        public override void OnDelete()
        {
            if (ArenaPersistance.m_ArenaMatchResultEntries.Contains(this))
                ArenaPersistance.m_ArenaMatchResultEntries.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0 
            writer.Write(m_ArenaMatch);

            writer.Write((int)m_MatchStatus);
            writer.Write(m_CompletionDate);
            writer.Write((int)m_MatchType);
            writer.Write(m_MatchDuration);
            writer.Write(m_WinningTeam);

            writer.Write(m_TeamResultEntries.Count);
            for (int a = 0; a < m_TeamResultEntries.Count; a++)
            {
                ArenaMatchTeamResultEntry teamResultEntry = m_TeamResultEntries[a];

                writer.Write(teamResultEntry.m_Winner);
                writer.Write(teamResultEntry.m_TeamName);

                writer.Write(teamResultEntry.m_PlayerResultEntries.Count);
                for (int b = 0; b < teamResultEntry.m_PlayerResultEntries.Count; b++)
                {
                    ArenaMatchPlayerResultEntry playerResultEntry = teamResultEntry.m_PlayerResultEntries[b];

                    writer.Write(playerResultEntry.m_Player);
                    writer.Write(playerResultEntry.m_PlayerName);
                    writer.Write(playerResultEntry.m_Alive);
                    writer.Write(playerResultEntry.m_LowestHP);
                    writer.Write(playerResultEntry.m_TimeAlive);
                    writer.Write(playerResultEntry.m_DamageDealt);
                    writer.Write(playerResultEntry.m_DamageReceived);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaMatch = reader.ReadItem() as ArenaMatch;

                m_MatchStatus = (ArenaMatchResultStatusType)reader.ReadInt();
                m_CompletionDate = reader.ReadDateTime();
                m_MatchType = (ArenaRuleset.MatchTypeType)reader.ReadInt();
                m_MatchDuration = reader.ReadTimeSpan();
                m_WinningTeam = reader.ReadString();

                int teamResultEntriesCount = reader.ReadInt();
                for (int a = 0; a < teamResultEntriesCount; a++)
                {
                    bool winner = reader.ReadBool();
                    string teamName = reader.ReadString();

                    List<ArenaMatchPlayerResultEntry> m_playerResultEntries = new List<ArenaMatchPlayerResultEntry>();

                    int playerResultEntriesCount = reader.ReadInt();
                    for (int b = 0; b < playerResultEntriesCount; b++)
                    {
                        PlayerMobile player = reader.ReadMobile() as PlayerMobile;
                        string playerName = reader.ReadString();
                        bool alive = reader.ReadBool();
                        int lowestHP = reader.ReadInt();
                        TimeSpan timeAlive = reader.ReadTimeSpan();
                        int damageDealt = reader.ReadInt();
                        int damageReceived = reader.ReadInt();

                        ArenaMatchPlayerResultEntry playerResultEntry = new ArenaMatchPlayerResultEntry(player, playerName, alive, lowestHP, timeAlive, damageDealt, damageReceived);

                        m_playerResultEntries.Add(playerResultEntry);
                    }

                    ArenaMatchTeamResultEntry teamResultEntry = new ArenaMatchTeamResultEntry(winner, teamName, m_playerResultEntries);

                    m_TeamResultEntries.Add(teamResultEntry);
                }
            }

            //-----

            ArenaPersistance.m_ArenaMatchResultEntries.Add(this);

            if (m_ArenaMatch != null)
                m_ArenaMatch.m_ArenaMatchResultEntry = this;
        }
    }

    public class ArenaMatchTeamResultEntry
    {
        public bool m_Winner = true;
        public string m_TeamName = "";
        public List<ArenaMatchPlayerResultEntry> m_PlayerResultEntries = new List<ArenaMatchPlayerResultEntry>();

        public ArenaMatchTeamResultEntry(bool winner, string teamName, List<ArenaMatchPlayerResultEntry> playerResultEntries)
        {
            m_Winner = winner;
            m_TeamName = teamName;

            foreach (ArenaMatchPlayerResultEntry entry in playerResultEntries)
            {
                m_PlayerResultEntries.Add(entry);
            }
        }

        public int GetTotalDamageDealt()
        {
            int totalDamageDealt = 0;

            foreach (ArenaMatchPlayerResultEntry playerEntry in m_PlayerResultEntries)
            {
                if (playerEntry == null) 
                    continue;

                totalDamageDealt += playerEntry.m_DamageDealt;
            }

            return totalDamageDealt;
        }

        public int GetTotalDamageReceived()
        {
            int totalDamageReceived = 0;

            foreach (ArenaMatchPlayerResultEntry playerEntry in m_PlayerResultEntries)
            {
                if (playerEntry == null)
                    continue;

                totalDamageReceived += playerEntry.m_DamageReceived;
            }

            return totalDamageReceived;
        }
    }

    public class ArenaMatchPlayerResultEntry
    {
        public PlayerMobile m_Player;
        public string m_PlayerName = "";

        public bool m_Alive = true;

        public int m_LowestHP = 100;
        public TimeSpan m_TimeAlive = TimeSpan.FromSeconds(0);
        public int m_DamageDealt = 0;
        public int m_DamageReceived = 0;

        public ArenaMatchPlayerResultEntry(PlayerMobile player, string playerName, bool alive, int lowestHP, TimeSpan timeAlive, int damageDealt, int damageReceived)
        {
            m_Player = player;
            m_PlayerName = playerName;
            m_Alive = alive;
            m_LowestHP = lowestHP;
            m_TimeAlive = timeAlive;
            m_DamageDealt = damageDealt;
            m_DamageReceived = damageReceived;
        }
    }
}
