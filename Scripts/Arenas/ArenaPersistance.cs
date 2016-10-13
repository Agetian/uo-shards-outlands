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

        public static List<ArenaAccountEntry> m_ArenaAccountEntries = new List<ArenaAccountEntry>();
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new ArenaPersistanceItem();               
            });
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
}
