﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Items;

namespace Server
{
    public static class EnhancementsPersistance
    {
        public static EnhancementsPersistanceItem PersistanceItem;

        public static List<EnhancementsAccountEntry> m_EnhancementsEntries = new List<EnhancementsAccountEntry>();
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new EnhancementsPersistanceItem();               
            });
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckAndCreateEnhancementsAccountEntry(player);
        }

        public static void CheckAndCreateEnhancementsAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            if (player.m_EnhancementsAccountEntry == null)
                CreateEnhancementsAccountEntry(player);

            if (player.m_EnhancementsAccountEntry.Deleted)
                CreateEnhancementsAccountEntry(player);            
        }

        public static void CreateEnhancementsAccountEntry(PlayerMobile player)
        {
            if (player == null)
                return;

            string accountName = player.Account.Username;

            EnhancementsAccountEntry enhancementsAccountEntry = null;

            bool foundEntry = false;

            foreach (EnhancementsAccountEntry entry in m_EnhancementsEntries)
            {
                if (entry.m_AccountUsername == accountName)
                {
                    player.m_EnhancementsAccountEntry = entry;
                    foundEntry = true;

                    return;
                }
            }

            if (!foundEntry)
            {
                EnhancementsAccountEntry newEntry = new EnhancementsAccountEntry(accountName);

                Account account = player.Account as Account;

                for (int a = 0; a < account.accountMobiles.Length; a++)
                {
                    Mobile mobile = account.accountMobiles[a] as Mobile;

                    if (mobile != null)
                    {
                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                            pm_Mobile.m_EnhancementsAccountEntry = newEntry;
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

    public class EnhancementsPersistanceItem : Item
    {
        public override string DefaultName { get { return "EnhancementsPersistance"; } }

        public EnhancementsPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public EnhancementsPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            EnhancementsPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            EnhancementsPersistance.PersistanceItem = this;
            EnhancementsPersistance.Deserialize(reader);
        }
    }

    public class EnhancementsAccountEntry : Item
    {
        public string m_AccountUsername = "";

        public List<Enhancements.CustomizationEntry> m_Customizations = new List<Enhancements.CustomizationEntry>();
        public List<Enhancements.SpellHueEntry> m_SpellHues = new List<Enhancements.SpellHueEntry>();
        public List<Enhancements.EmoteEntry> m_Emotes = new List<Enhancements.EmoteEntry>();
                
        [Constructable]
        public EnhancementsAccountEntry(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;

            //-----

            EnhancementsPersistance.m_EnhancementsEntries.Add(this);
        }

        public EnhancementsAccountEntry(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_AccountUsername);

            writer.Write(m_Customizations.Count);
            for (int a = 0; a < m_Customizations.Count; a++)
            {
                writer.Write((int)m_Customizations[a].m_Customization);
                writer.Write(m_Customizations[a].m_Unlocked);
                writer.Write(m_Customizations[a].m_Active);
            }

            writer.Write(m_SpellHues.Count);
            for (int a = 0; a < m_SpellHues.Count; a++)
            {
                writer.Write((int)m_SpellHues[a].m_SpellType);

                for (int b = 0; b < m_SpellHues[a].m_UnlockedHues.Count; b++)
                {
                    writer.Write((int)m_SpellHues[a].m_UnlockedHues[b]);
                }

                writer.Write((int)m_SpellHues[a].m_SelectedHue);
            }

            writer.Write(m_Emotes.Count);
            for (int a = 0; a < m_Emotes.Count; a++)
            {
                writer.Write((int)m_Emotes[a].m_Emote);
                writer.Write(m_Emotes[a].m_Unlocked);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();           

            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();

                int customizationCount = reader.ReadInt();
                for (int a = 0; a < customizationCount; a++)
                {
                    Enhancements.CustomizationType customizationType = (Enhancements.CustomizationType)reader.ReadInt();
                    bool unlocked = reader.ReadBool();
                    bool active = reader.ReadBool();

                    Enhancements.CustomizationEntry customization = new Enhancements.CustomizationEntry(customizationType, unlocked, active);

                    m_Customizations.Add(customization);                   
                }

                int spellHuesCount = reader.ReadInt();
                for (int a = 0; a < spellHuesCount; a++)
                {        
                    Enhancements.SpellType spellType = (Enhancements.SpellType)reader.ReadInt();

                    List<Enhancements.SpellHueType> m_SpellHueTypes = new List<Enhancements.SpellHueType>();

                    int spellHueSpellsCount = reader.ReadInt();
                    for (int b = 0; b < spellHueSpellsCount; b++)
                    {
                        m_SpellHueTypes.Add((Enhancements.SpellHueType)reader.ReadInt());
                    }

                    Enhancements.SpellHueType selectedSpellHue = (Enhancements.SpellHueType)reader.ReadInt();

                    if (spellType != null)
                    {
                        Enhancements.SpellHueEntry spellHueEntry = new Enhancements.SpellHueEntry(spellType);

                        spellHueEntry.m_UnlockedHues = m_SpellHueTypes;
                        spellHueEntry.m_SelectedHue = selectedSpellHue;

                        m_SpellHues.Add(spellHueEntry);
                    }
                }

                int emotesCount = reader.ReadInt();
                for (int a = 0; a < emotesCount; a++)
                {
                    Enhancements.EmoteType emoteType = (Enhancements.EmoteType)reader.ReadInt();
                    bool unlocked = reader.ReadBool();

                    Enhancements.EmoteEntry emote = new Enhancements.EmoteEntry(emoteType, unlocked);

                    m_Emotes.Add(emote);
                }
            }

            //-----

            EnhancementsPersistance.m_EnhancementsEntries.Add(this);
        }
    }
}
