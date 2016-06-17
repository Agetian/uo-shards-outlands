﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    public enum GuildMemberRank
    {
        Recruit,
        Initiate,
        Veteran,
        Officer,
        Guildmaster
    }

    public static class Guilds
    {
        public enum InvitationSortCriteria
        {
            None,
            GuildName,
            Expiration
        }

        public enum CandidateSortCriteria
        {
            None,
            Accepted,
            PlayerName,
            Expiration
        }

        public enum MemberSortCriteria
        {
            None,
            LastOnline,
            PlayerName,
            GuildRank,
            Fealty
        }

        public enum GuildRelationshipType
        {
            None,
            War,
            Ally,
            WarRequest,
            AllyRequest
        }

        public static int GuildNameCharacterLimit = 35;
        public static int GuildAbbreviationCharacterLimit = 3;

        public static int GuildRegistrationFee = 50000;

        public static int GuildTextHue = 63;
        
        public static TimeSpan InvitationExpiration = TimeSpan.FromDays(30);
        public static TimeSpan InactivityThreshold = TimeSpan.FromDays(60);

        public static TimeSpan GuildRequestExpiration = TimeSpan.FromDays(30);

        public static int GuildGumpChangePageSound = 0x057;
        public static int GuildGumpOpenGumpSound = 0x055;
        public static int GuildGumpCloseGumpSound = 0x058;

        public static string[] GuildRankNames = new string[] { "Recruit", "Initiate", "Veteran", "Officer", "Guildmaster" };

        public static GuildPersistanceItem PersistanceItem;

        public static List<Guild> m_Guilds = new List<Guild>();
        
        public static void Initialize()
        {
            CommandSystem.Register("Guild", AccessLevel.Player, new CommandEventHandler(GuildCommand));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new GuildPersistanceItem();               
            });
        }

        [Usage("[Guild")]
        [Description("Launches the Guild interface")]
        public static void GuildCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            GuildGumpObject guildGumpObject = new GuildGumpObject();

            int startingGuildTabPage = Guilds.DetermineStartingGuildTabPage(player);

            guildGumpObject.m_Guild = player.Guild;
            guildGumpObject.m_GuildTabPage = startingGuildTabPage;

            Guilds.SendGuildGump(player, guildGumpObject);
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckCreateGuildGuildSettings(player);
        }

        public static void OnPlayerDeleted(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateGuildGuildSettings(player);

            if (player.m_GuildSettings != null)
            {
                //Invitations
                Queue m_Queue = new Queue();

                foreach (GuildInvitation invitation in player.m_GuildSettings.m_GuildInvitations)
                {
                    if (invitation == null) 
                        continue;

                    m_Queue.Enqueue(invitation);
                }

                while (m_Queue.Count > 0)
                {
                    GuildInvitation invitation = (GuildInvitation)m_Queue.Dequeue();

                    invitation.Delete();
                }

                player.m_GuildSettings.Delete();
            }

            if (player.Guild != null)            
                player.Guild.DismissMember(player, false, true);            
        } 

        public static void CheckCreateGuildGuildSettings(PlayerMobile player)
        {
            if (player == null)
                return;

            if (player.m_GuildSettings == null)
                player.m_GuildSettings = new GuildSettings(player);

            else if (player.m_GuildSettings.Deleted)
                player.m_GuildSettings = new GuildSettings(player);
        }

        public static List<GuildGumpPageType> GetGuildPageTypeList(PlayerMobile player)
        {
            List<GuildGumpPageType> guildPageTypes = new List<GuildGumpPageType>();

            if (player == null)
                return guildPageTypes;

            CheckCreateGuildGuildSettings(player);

            if (player.Guild == null)
            {
                guildPageTypes.Add(GuildGumpPageType.CreateGuild);
                guildPageTypes.Add(GuildGumpPageType.Invitations);
            }

            else
            {
                guildPageTypes.Add(GuildGumpPageType.Overview);
                guildPageTypes.Add(GuildGumpPageType.Members);
                guildPageTypes.Add(GuildGumpPageType.Candidates);
                guildPageTypes.Add(GuildGumpPageType.Diplomacy);

                guildPageTypes.Add(GuildGumpPageType.Faction);
                guildPageTypes.Add(GuildGumpPageType.Messages);
            }            

            return guildPageTypes;
        }

        public static void GuildGumpCheckGuild(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateGuildGuildSettings(player);

            if (player.Guild == null)
            {
                if (!(player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Invitations))
                    player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.CreateGuild;
            }

            else
            {
                if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Invitations)
                    player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Overview;
            }
        }

        public static int DetermineStartingGuildTabPage(PlayerMobile player)
        {
            CheckCreateGuildGuildSettings(player);
            GuildGumpCheckGuild(player);

            List<GuildGumpPageType> validGuildTabs = Guilds.GetGuildPageTypeList(player);

            if (validGuildTabs.Count == 0)
                return 0;

            int GuildTabsPerPage = 4;
            int TotalGuildTabs = validGuildTabs.Count;
            int TotalGuildTabPages = (int)(Math.Ceiling((double)TotalGuildTabs / (double)GuildTabsPerPage));

            if (validGuildTabs.Contains(player.m_GuildSettings.m_GuildGumpPage))
            {
                int tabIndex = validGuildTabs.IndexOf(player.m_GuildSettings.m_GuildGumpPage);

                return (int)(Math.Floor((double)tabIndex / (double)GuildTabsPerPage));
            }

            return 0;
        }

        public static void SendGuildGump(PlayerMobile player, GuildGumpObject guildGumpObject)
        {
            if (player == null) return;
            if (guildGumpObject == null) return; 
            
            CheckCreateGuildGuildSettings(player);
            GuildGumpCheckGuild(player);
            
            List<GuildGumpPageType> validGuildTabs = Guilds.GetGuildPageTypeList(player);

            if (validGuildTabs.Count == 0)
                return;

            int GuildTabsPerPage = 4;
            int TotalGuildTabs = validGuildTabs.Count;
            int TotalGuildTabPages = (int)(Math.Ceiling((double)TotalGuildTabs / (double)GuildTabsPerPage));

            if (guildGumpObject.m_GuildTabPage >= TotalGuildTabPages)
                guildGumpObject.m_GuildTabPage = TotalGuildTabPages - 1;

            if (guildGumpObject.m_GuildTabPage < 0)
                guildGumpObject.m_GuildTabPage = 0;

            if (!validGuildTabs.Contains(player.m_GuildSettings.m_GuildGumpPage))
            {
                player.m_GuildSettings.m_GuildGumpPage = validGuildTabs[0];
                guildGumpObject.m_GuildTabPage = 0;
            }
            
            player.CloseGump(typeof(GuildGump));
            player.SendGump(new GuildGump(player, guildGumpObject));
        }

        public static void StandaloneGuildGump(PlayerMobile player, Guild guild)
        {
            if (player == null) return;
            if (guild == null) return;
        }

        public static bool GuildNameExists(string guildName)
        {
            foreach (Guild guild in m_Guilds)
            {
                if (guild == null) continue;
                if (guild.Name.ToLower() == guildName.ToLower())
                    return true;
            }

            return false;
        }

        public static bool GuildAbbreviationExists(string guildAbbreviation)
        {
            foreach (Guild guild in m_Guilds)
            {
                if (guild == null) continue;
                if (guild.m_Abbreviation.ToLower() == guildAbbreviation.ToLower())
                    return true;
            }

            return false;
        }

        public static bool CheckProfanity(string s)
        {
            return CheckProfanity(s, 50);
        }

        public static bool CheckProfanity(string s, int maxLength)
        {
            if (s.Length < 1 || s.Length > maxLength)
                return false;

            char[] exceptions = ProfanityProtection.Exceptions;

            s = s.ToLower();

            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];

                if ((c < 'a' || c > 'z') && (c < '0' || c > '9'))
                {
                    bool except = false;

                    for (int j = 0; !except && j < exceptions.Length; j++)
                        if (c == exceptions[j])
                            except = true;

                    if (!except)
                        return false;
                }
            }

            string[] disallowed = ProfanityProtection.Disallowed;

            for (int i = 0; i < disallowed.Length; i++)
            {
                if (s.IndexOf(disallowed[i]) != -1)
                    return false;
            }

            return true;
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

    public class GuildPersistanceItem : Item
    {
        public override string DefaultName { get { return "GuildPersistance"; } }

        public GuildPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public GuildPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            Guilds.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            Guilds.PersistanceItem = this;
            Guilds.Deserialize(reader);
        }
    }

    public class GuildRelationship: Item
    {
        public Guild m_GuildFrom;
        public Guild m_GuildTarget;
        public Guilds.GuildRelationshipType m_RelationshipType;
        public DateTime m_DateIssued;
        public DateTime m_DateBegan;

        [Constructable]
        public GuildRelationship(Guild guildFrom, Guild guildTarget, Guilds.GuildRelationshipType relationshipType, DateTime dateIssued, DateTime dateBegan): base(0x0)
        {
            m_GuildFrom = guildFrom;
            m_GuildTarget = guildTarget;
            m_RelationshipType = relationshipType;
            m_DateIssued = dateIssued;
            m_DateBegan = dateBegan;
        }        

        public GuildRelationship(Serial serial): base(serial)
        {
        }

        public bool CheckExpired()
        {
            if (m_RelationshipType == Guilds.GuildRelationshipType.WarRequest || m_RelationshipType == Guilds.GuildRelationshipType.AllyRequest)
            {
                if (m_DateIssued + Guilds.GuildRequestExpiration <= DateTime.UtcNow)
                    return true;
            }

            if (m_GuildFrom == null) return true;
            if (m_GuildFrom.Deleted) return true;
            if (m_GuildTarget == null) return true;
            if (m_GuildTarget.Deleted) return true;

            return false;
        }

        public override void OnDelete()
        {
            if (m_GuildFrom != null)
            {
                if (m_GuildFrom.m_Relationships.Contains(this))
                    m_GuildFrom.m_Relationships.Remove(this);
            }

            if (m_GuildTarget != null)
            {
                if (m_GuildTarget.m_Relationships.Contains(this))
                    m_GuildTarget.m_Relationships.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_GuildFrom);
            writer.Write(m_GuildTarget);
            writer.Write((int)m_RelationshipType);
            writer.Write(m_DateIssued);
            writer.Write(m_DateBegan);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_GuildFrom = (Guild)reader.ReadItem();
                m_GuildTarget = (Guild)reader.ReadItem();
                m_RelationshipType = (Guilds.GuildRelationshipType)reader.ReadInt();
                m_DateIssued = reader.ReadDateTime();
                m_DateBegan = reader.ReadDateTime();
            }   

            //-----

            if (CheckExpired())
                Delete();

            else
            {
                m_GuildFrom.m_Relationships.Add(this);
                m_GuildTarget.m_Relationships.Add(this);
            }            
        }
    }
    
    public class GuildInvitation : Item
    {
        public PlayerMobile m_PlayerTarget;
        public PlayerMobile m_PlayerInviter;
        public Guild m_Guild;
        public DateTime m_InvitationTime;
        public bool m_Accepted = false;

        [Constructable]
        public GuildInvitation(PlayerMobile playerTarget, PlayerMobile playerInviter, Guild guild, DateTime invitationTime, bool accepted): base(0x0)
        {
            m_PlayerTarget = playerTarget;
            m_PlayerInviter = playerInviter;
            m_Guild = guild;
            m_InvitationTime = invitationTime;
            m_Accepted = accepted;
        }

        public GuildInvitation(Serial serial): base(serial)
        {
        }

        public bool CheckExpired()
        {
            if (m_InvitationTime + Guilds.InvitationExpiration <= DateTime.UtcNow) return true;
            if (m_PlayerTarget == null) return true;
            if (m_PlayerTarget.Deleted) return true;
            if (m_Guild == null) return true;
            if (m_Guild.Deleted) return true;

            return false;
        }

        public override void OnDelete()
        {
            if (m_PlayerTarget != null)
            {
                Guilds.CheckCreateGuildGuildSettings(m_PlayerTarget);

                if (m_PlayerTarget.m_GuildSettings.m_GuildInvitations.Contains(this))
                    m_PlayerTarget.m_GuildSettings.m_GuildInvitations.Remove(this);
            }

            if (m_Guild != null)
            {
                if (m_Guild.m_Candidates.Contains(this))
                    m_Guild.m_Candidates.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_PlayerTarget);
            writer.Write(m_PlayerInviter);
            writer.Write(m_Guild);
            writer.Write(m_InvitationTime);
            writer.Write(m_Accepted);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_PlayerTarget = (PlayerMobile)reader.ReadMobile();
                m_PlayerInviter = (PlayerMobile)reader.ReadMobile();
                m_Guild = (Guild)reader.ReadItem();
                m_InvitationTime = reader.ReadDateTime();
                m_Accepted = reader.ReadBool();
            }

            //-----

            if (CheckExpired())
                Delete();

            else
            {
                Guilds.CheckCreateGuildGuildSettings(m_PlayerTarget);

                m_PlayerTarget.m_GuildSettings.m_GuildInvitations.Add(this);
                m_Guild.m_Candidates.Add(this);
            }
        }
    }    

    public class GuildSettings: Item
    {
        public enum SortCriteria
        {
            None,
            GuildName,
            Expiration,
        }

        public PlayerMobile m_Player;

        public GuildGumpPageType m_GuildGumpPage = GuildGumpPageType.CreateGuild;

        public bool m_IgnoreGuildInvitations = false;
        public bool m_ShowGuildTitle = true;

        public List<GuildInvitation> m_GuildInvitations = new List<GuildInvitation>();

        [Constructable]
        public GuildSettings(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Player = player;

            if (player == null)
                Delete();

            else
                m_Player.m_GuildSettings = this;
        }        

        public List<GuildInvitation> GetInvitations(Guilds.InvitationSortCriteria sortCriteria, bool ascending)
        {
            List<GuildInvitation> invitationsList = new List<GuildInvitation>();

            if (sortCriteria == Guilds.InvitationSortCriteria.None)            
                return m_GuildInvitations;

            bool addToStart = true;
         
            for (int a = 0; a < m_GuildInvitations.Count; a++)
            {
                GuildInvitation invitationEntry = m_GuildInvitations[a];

                if (invitationEntry == null) continue;
                if (invitationEntry.m_Guild == null) continue;
                
                int newIndexPosition = -1;               

                for (int b = 0; b < invitationsList.Count; b++)
                {
                    GuildInvitation invitationListItem = invitationsList[b];

                    if (invitationListItem == null) continue;
                    if (invitationListItem.m_Guild == null) continue;

                    switch (sortCriteria)
                    {
                        case Guilds.InvitationSortCriteria.GuildName:
                            if (string.Compare(invitationEntry.m_Guild.Name, invitationListItem.m_Guild.Name) >= 0)
                                newIndexPosition = b + 1;
                        break;

                        case Guilds.InvitationSortCriteria.Expiration:
                            addToStart = false;

                            if (invitationEntry.m_InvitationTime <= invitationListItem.m_InvitationTime)
                                newIndexPosition = b + 1;    
                        break;
                    }
                }

                if (newIndexPosition == -1)
                {
                    if (addToStart)
                        invitationsList.Insert(0, invitationEntry);

                    else
                        invitationsList.Add(invitationEntry);
                }

                else
                {
                    if (newIndexPosition >= invitationsList.Count)
                        invitationsList.Add(invitationEntry);

                    else
                        invitationsList.Insert(newIndexPosition, invitationEntry);
                }               
            }

            if (!ascending)
                invitationsList.Reverse(0, invitationsList.Count);
            
            return invitationsList;
        }

        public void AuditInvitations()
        {
            Queue m_Queue = new Queue();

            foreach (GuildInvitation invitation in m_GuildInvitations)
            {
                if (invitation == null) continue;
                if (invitation.Deleted) continue;

                if (invitation.CheckExpired())
                    m_Queue.Enqueue(invitation);
            }

            while (m_Queue.Count > 0)
            {
                GuildInvitation invitation = (GuildInvitation)m_Queue.Dequeue();

                invitation.Delete();
            }
        }

        public GuildSettings(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Player);
            writer.Write((int)m_GuildGumpPage);
            writer.Write(m_IgnoreGuildInvitations);
            writer.Write(m_ShowGuildTitle);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;
                m_GuildGumpPage = (GuildGumpPageType)reader.ReadInt();
                m_IgnoreGuildInvitations = reader.ReadBool();
                m_ShowGuildTitle = reader.ReadBool();
            }

            if (m_Player == null)
                Delete();

            else if (m_Player.Deleted)
                Delete();

            else
                m_Player.m_GuildSettings = this;
        }
    }

    public class GuildMemberEntry
    {
        public PlayerMobile m_Player;
        public GuildMemberRank m_Rank = GuildMemberRank.Recruit;
        public DateTime m_JoinDate = DateTime.UtcNow;
        public PlayerMobile m_DeclaredFealty;

        public GuildMemberEntry(PlayerMobile player, GuildMemberRank rank, DateTime joinDate, PlayerMobile declaredFealty)
        {
            m_Player = player;
            m_Rank = rank;
            m_JoinDate = joinDate;
            m_DeclaredFealty = declaredFealty;
        }
    }

    public class FealtyVote
    {
        public string m_AccountName = "";
        public DateTime m_LastOnline;
        public PlayerMobile m_FealtyTarget;
        public bool m_Active;

        public FealtyVote(string accountName, DateTime lastOnline, PlayerMobile fealtyTarget, bool active)
        {
            m_AccountName = accountName;
            m_LastOnline = lastOnline;
            m_FealtyTarget = fealtyTarget;
            m_Active = active;
        }
    }

    public class Guild : Item
    {
        public PlayerMobile m_Guildmaster;

        public string m_Abbreviation = "";
        public DateTime m_CreationTime;

        public int m_Icon = 4014;
        public int m_IconHue = 0;

        public string m_Website = "http://www.outlandsuo.com/index.html";

        public BaseHouse m_Guildhouse;

        public Faction m_Faction = null;

        public string[] m_RankNames = new string[] { Guilds.GuildRankNames[0], Guilds.GuildRankNames[1], Guilds.GuildRankNames[2], Guilds.GuildRankNames[3], Guilds.GuildRankNames[4] };

        public List<GuildMemberEntry> m_Members = new List<GuildMemberEntry>();
        public List<GuildInvitation> m_Candidates = new List<GuildInvitation>();
        public List<GuildRelationship> m_Relationships = new List<GuildRelationship>();

        [Constructable]
        public Guild(string name, string abbreviation): base(0x0)
        {
            Visible = false;
            Movable = false;

            Name = name;
            m_Abbreviation = abbreviation;

            m_CreationTime = DateTime.UtcNow;

            Guilds.m_Guilds.Add(this);
        }

        public Guild(Serial serial): base(serial)
        {
        }

        public string GetDisplayName(bool addAbbreviation)
        {
            if (addAbbreviation)
                return Name + " [" + m_Abbreviation + "]";

            else
                return Name;
        }

        public string GetRankName(GuildMemberRank rank)
        {
            string rankName = "";

            int rankIndex = (int)rank;

            if (rankIndex < m_RankNames.Length)
                rankName = m_RankNames[rankIndex];

            return rankName;
        }

        public int GetRankHue(GuildMemberRank rank)
        {
            int rankHue = 0;

            switch (rank)
            {
                case GuildMemberRank.Recruit: rankHue = 2655; break;
                case GuildMemberRank.Initiate: rankHue = 2599; break;
                case GuildMemberRank.Veteran: rankHue = 169; break;
                case GuildMemberRank.Officer: rankHue = 2603; break;
                case GuildMemberRank.Guildmaster: rankHue = 1259; break;
            }

            return rankHue;
        }

        public bool CanAddCandidates(GuildMemberRank rank)
        {
            switch (rank)
            {
                case GuildMemberRank.Recruit: return false;
                case GuildMemberRank.Initiate: return true;
                case GuildMemberRank.Veteran: return true;
                case GuildMemberRank.Officer: return true;
                case GuildMemberRank.Guildmaster: return true;
            }

            return false;
        }

        public bool CanApproveCandidates(GuildMemberRank rank)
        {
            switch (rank)
            {
                case GuildMemberRank.Recruit: return false;
                case GuildMemberRank.Initiate: return false;
                case GuildMemberRank.Veteran: return false;
                case GuildMemberRank.Officer: return true;
                case GuildMemberRank.Guildmaster: return true;
            }

            return false;
        }

        public void GuildAnnouncement(string message, List<PlayerMobile> ignoredPlayers, GuildMemberRank minimumGuildRank)
        {
            List<GuildMemberEntry> memberEntries = GetGuildMemberEntries(Guilds.MemberSortCriteria.None, true);

            foreach (GuildMemberEntry entry in memberEntries)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;
                if (ignoredPlayers.Contains(entry.m_Player)) continue;
                if (((int)entry.m_Rank < (int)minimumGuildRank)) continue;

                entry.m_Player.SendMessage(Guilds.GuildTextHue, message);                
            }
        }

        public bool IsMember(PlayerMobile player)
        {
            if (player == null)
                return false;

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry.m_Player == player)
                    return true;
            }

            return false;
        }

        public bool IsPlayerActive(PlayerMobile player)
        {
            if (player == null)
                return false;

            if (IsMember(player))
            {
                if (player.LastOnline + Guilds.InactivityThreshold >= DateTime.UtcNow)
                    return true;
            }

            return false;
        }  

        public GuildInvitation GetCandidate(PlayerMobile player)
        {
            GuildInvitation invitationMatch = null;

            foreach (GuildInvitation invitation in m_Candidates)
            {
                if (invitation == null) continue;
                if (invitation.m_PlayerTarget == player)
                    return invitation;
            }

            return null;
        }

        public List<GuildInvitation> GetCandidates(Guilds.CandidateSortCriteria sortCriteria, bool ascending)
        {
            List<GuildInvitation> candidatesList = new List<GuildInvitation>();

            if (sortCriteria == Guilds.CandidateSortCriteria.None)
                return m_Candidates;

            bool addToStart = true;

            for (int a = 0; a < m_Candidates.Count; a++)
            {
                GuildInvitation candidateEntry = m_Candidates[a];

                if (candidateEntry == null) continue;
                if (candidateEntry.m_PlayerTarget == null) continue;

                int newIndexPosition = -1;

                for (int b = 0; b < candidatesList.Count; b++)
                {
                    GuildInvitation invitationListItem = candidatesList[b];

                    if (invitationListItem == null) continue;
                    if (invitationListItem.m_PlayerTarget == null) continue;

                    switch (sortCriteria)
                    {
                        case Guilds.CandidateSortCriteria.Accepted:
                            if (candidateEntry.m_Accepted && !invitationListItem.m_Accepted)
                                    newIndexPosition = b + 1;
                            break;

                        case Guilds.CandidateSortCriteria.PlayerName:
                            if (string.Compare(candidateEntry.m_PlayerTarget.RawName, invitationListItem.m_PlayerTarget.RawName) >= 0)
                                newIndexPosition = b + 1;
                        break;

                        case Guilds.CandidateSortCriteria.Expiration:
                            addToStart = false;

                            if (candidateEntry.m_InvitationTime <= invitationListItem.m_InvitationTime)
                                newIndexPosition = b + 1;
                        break;
                    }
                }

                if (newIndexPosition == -1)
                {
                    if (addToStart)
                        candidatesList.Insert(0, candidateEntry);

                    else
                        candidatesList.Add(candidateEntry);
                }

                else
                {
                    if (newIndexPosition >= candidatesList.Count)
                        candidatesList.Add(candidateEntry);

                    else
                        candidatesList.Insert(newIndexPosition, candidateEntry);
                }  
            }

            if (!ascending)
                candidatesList.Reverse(0, candidatesList.Count);

            return candidatesList;
        }

        public void AuditCandidates()
        {
            Queue m_Queue = new Queue();

            foreach (GuildInvitation candidate in m_Candidates)
            {
                if (candidate == null) continue;
                if (candidate.Deleted) continue;

                if (candidate.CheckExpired())
                    m_Queue.Enqueue(candidate);
            }

            while (m_Queue.Count > 0)
            {
                GuildInvitation candidate = (GuildInvitation)m_Queue.Dequeue();

                candidate.Delete();
            }
        }

        public GuildMemberEntry GetGuildMemberEntry(PlayerMobile player)
        {
            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry.m_Player == player)
                    return entry;
            }

            return null;
        }

        public List<PlayerMobile> GetGuildMembers()
        {
            List<PlayerMobile> membersList = new List<PlayerMobile>();

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;
                if (entry.m_Player.Deleted) continue;

                membersList.Add(entry.m_Player);
            }

            return membersList;
        }  

        public List<GuildMemberEntry> GetGuildMemberEntries(Guilds.MemberSortCriteria sortCriteria, bool ascending)
        {
            List<GuildMemberEntry> membersList = new List<GuildMemberEntry>();

            if (sortCriteria == Guilds.MemberSortCriteria.None)
                return m_Members;

            bool addToStart = true;

            for (int a = 0; a < m_Members.Count; a++)
            {
                GuildMemberEntry memberEntry = m_Members[a];

                if (memberEntry == null) continue;
                if (memberEntry.m_Player == null) continue;
                if (memberEntry.m_Player.Deleted) continue;

                int newIndexPosition = -1;

                for (int b = 0; b < membersList.Count; b++)
                {
                    GuildMemberEntry memberListItem = membersList[b];

                    if (memberListItem == null) continue;
                    if (memberListItem.m_Player == null) continue;
                    if (memberListItem.m_Player.Deleted) continue;
                    
                    switch (sortCriteria)
                    {
                        case Guilds.MemberSortCriteria.LastOnline:  
                            if (memberEntry.m_Player.LastOnline >= memberListItem.m_Player.LastOnline)
                                newIndexPosition = b + 1;                            
                        break;

                        case Guilds.MemberSortCriteria.PlayerName:
                            int compareResult = string.Compare(memberEntry.m_Player.RawName, memberListItem.m_Player.RawName);

                            if (compareResult >= 0)
                                newIndexPosition = b + 1;                            
                        break;  
                     
                        case Guilds.MemberSortCriteria.GuildRank:
                            if ((int)memberEntry.m_Rank >= (int)memberListItem.m_Rank)
                                newIndexPosition = b + 1;
                        break;
                    }
                }

                if (newIndexPosition == -1)
                {
                    if (addToStart)
                        membersList.Insert(0, memberEntry);

                    else
                        membersList.Add(memberEntry);
                }

                else
                {
                    if (newIndexPosition >= membersList.Count)
                        membersList.Add(memberEntry);

                    else
                        membersList.Insert(newIndexPosition, memberEntry);
                }
            }

            if (!ascending)
                membersList.Reverse(0, membersList.Count);

            return membersList;
        }

        public void AuditMembers()
        {            
            Queue m_Queue = new Queue();

            foreach (GuildMemberEntry member in m_Members)
            {
                if (member == null) 
                    continue;

                if (member.m_Player == null)
                    m_Queue.Enqueue(member);

                else if (member.m_Player.Deleted)
                    m_Queue.Enqueue(member);

                else
                {
                    if (member.m_DeclaredFealty == null)
                    {
                        if (m_Guildmaster != null)
                            member.m_DeclaredFealty = m_Guildmaster;

                        else
                            member.m_DeclaredFealty = member.m_Player;
                    }                        
                }
            }

            while (m_Queue.Count > 0)
            {
                GuildMemberEntry member = (GuildMemberEntry)m_Queue.Dequeue();

                if (m_Members.Contains(member))
                    m_Members.Remove(member);
            }            
        }

        public bool IsAlliedGuild(Guild guild)
        {
            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null) continue;
                if (relationship.Deleted) continue;

                if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.Ally)
                    return true;
            }

            return false;
        }

        public bool IsEnemyGuild(Guild guild)
        {
            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null) continue;
                if (relationship.Deleted) continue;

                if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.War)
                    return true;
            }

            return false;
        }

        public bool AddMember(PlayerMobile player, GuildMemberRank rank)
        {
            if (player == null)
                return false;

            GuildInvitation invitation = GetCandidate(player);

            if (invitation != null)
                invitation.Delete();

            if (IsMember(player))
                return false;

            GuildMemberEntry entry = new GuildMemberEntry(player, rank, DateTime.UtcNow, m_Guildmaster);

            player.Guild = this;
            player.m_GuildMemberEntry = entry;

            m_Members.Add(entry);           

            return true;
        }

        public void DismissMember(PlayerMobile player, bool forced, bool announce)
        {
            if (!IsMember(player))
                return;

            GuildMemberEntry entry = GetGuildMemberEntry(player);

            if (entry == null)
                return;

            bool wasGuildmaster = (entry.m_Rank == GuildMemberRank.Guildmaster);

            if (m_Members.Contains(entry))
                m_Members.Remove(entry);
            
            string guildText = Name + " [" + m_Abbreviation + "]";

            if (player != null)
            {
                player.Guild = null;
                player.m_GuildMemberEntry = null;

                if (forced)
                    player.SendMessage(Guilds.GuildTextHue, "You have been removed from " + guildText + ".");

                else
                    player.SendMessage(Guilds.GuildTextHue, "You leave the guild.");

                if (announce)
                {
                    if (forced)
                        GuildAnnouncement(player.RawName + " has been removed from the guild.", new List<PlayerMobile> { }, GuildMemberRank.Recruit);

                    else
                        GuildAnnouncement(player.RawName + " has left the guild.", new List<PlayerMobile> { }, GuildMemberRank.Recruit);
                }
            } 

            if (wasGuildmaster)            
                AssignNewGuildmaster(player);            
        }

        public void AssignNewGuildmaster(PlayerMobile previousGuildmaster)
        {
            List<GuildMemberEntry> guildMemberEntries = GetGuildMemberEntries(Guilds.MemberSortCriteria.None, true);

            if (guildMemberEntries.Count == 0)
            {
                if (previousGuildmaster != null)
                    previousGuildmaster.SendMessage(Guilds.GuildTextHue, GetDisplayName(true) + " has disbanded.");

                DisbandGuild();

                return;
            }

            else
            {
                guildMemberEntries = GetGuildMemberEntries(Guilds.MemberSortCriteria.None, true);

                List<FealtyVote> fealtyVotes = new List<FealtyVote>();
                List<FealtyVote> inactiveMemberfealtyVotes = new List<FealtyVote>();

                foreach (GuildMemberEntry entry in guildMemberEntries)
                {
                    if (entry == null) continue;
                    if (entry.m_Player == null) continue;
                    if (entry.m_Player.Deleted) continue;
                    if (!IsMember(entry.m_DeclaredFealty)) continue;

                    Account account = entry.m_Player.Account as Account;

                    if (account == null)
                        continue;

                    bool isActiveMember = IsPlayerActive(entry.m_Player);

                    //TEST: FINISH!!!

                    if (isActiveMember)
                    {
                        bool foundAccountVote = false;

                        FealtyVote fealtyVote = new FealtyVote(account.Username, entry.m_Player.LastOnline, entry.m_DeclaredFealty, isActiveMember);      
                    }

                    else
                    {
                        bool foundAccountVote = false;

                        FealtyVote fealtyVote = new FealtyVote(account.Username, entry.m_Player.LastOnline, entry.m_DeclaredFealty, isActiveMember);      
                    }                                 
                }
            }
        }

        public void DisbandGuild()
        {
            GuildAnnouncement(GetDisplayName(true) + " has disbanded.", new List<PlayerMobile> { }, GuildMemberRank.Recruit);

            Delete();
        }

        public void ReapplyMemberships()
        {
            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;

                entry.m_Player.Guild = this;
                entry.m_Player.m_GuildMemberEntry = entry;
            }
        }

        public void AuditGuild()
        {
            List<GuildMemberEntry> guildMemberEntries = GetGuildMemberEntries(Guilds.MemberSortCriteria.None, true);

            if (guildMemberEntries.Count == 0)            
                DisbandGuild();            

            else if (m_Guildmaster == null)
                AssignNewGuildmaster(m_Guildmaster);

            else if (m_Guildmaster.Deleted)
                AssignNewGuildmaster(m_Guildmaster);
        }

        public void OnFealtyChange()
        {
        }    

        public void RecruitMember(PlayerMobile player)
        {
            if (!IsMember(player))
            {
                player.SendMessage("You are not a member of this guild.");
                return;
            }

            GuildMemberEntry playerEntry = GetGuildMemberEntry(player);

            if (playerEntry == null)
                return;

            if (CanApproveCandidates(playerEntry.m_Rank))
            {
                player.SendMessage("Which player do you wish to recruit into the guild?");
                player.Target = new RecruitMemberTarget(player, this);
            }

            else if (CanAddCandidates(playerEntry.m_Rank))
            {
                player.SendMessage("Which player do you wish to nominate as a candidate for guild membership?");
                player.Target = new RecruitMemberTarget(player, this);
            }

            else
            {
                player.SendMessage("You do not have a high enough rank in this guild to recruit new members.");
                return;
            }
        }

        private class RecruitMemberTarget : Target
        {           
            public PlayerMobile m_Player;
            public Guild m_Guild; 

            public RecruitMemberTarget(PlayerMobile player, Guild guild): base(100, false, TargetFlags.None)
            {               
                m_Player = player;
                m_Guild = guild;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Player == null) return;
                if (m_Player.Deleted) return;

                if (m_Guild == null)
                {
                    m_Player.SendMessage("That guild no longer exists.");
                    return;
                }

                if (m_Guild.Deleted)
                {
                    m_Player.SendMessage("That guild no longer exists.");
                    return;
                }

                if (!m_Guild.IsMember(m_Player))
                {
                    m_Player.SendMessage("You are not a member of this guild.");
                    return;
                }

                GuildMemberEntry playerEntry = m_Guild.GetGuildMemberEntry(m_Player);

                if (playerEntry == null)
                    return;

                if (!(m_Guild.CanAddCandidates(playerEntry.m_Rank) || m_Guild.CanApproveCandidates(playerEntry.m_Rank)))
                {
                    m_Player.SendMessage("You no longer have the neccessary rank to recruit new candidates to the guild.");
                    return;
                }

                PlayerMobile playerTarget = target as PlayerMobile;

                if (playerTarget == null)
                {
                    m_Player.SendMessage("That is not a player.");
                    return;
                }

                Guilds.CheckCreateGuildGuildSettings(playerTarget);

                if (Utility.GetDistance(m_Player.Location, playerTarget.Location) > 20)
                {
                    m_Player.SendMessage("That player is too far away.");
                    return;
                }

                if (playerTarget.Guild != null)
                {
                    m_Player.SendMessage("That player is already in a guild.");
                    return;
                }

                if (playerTarget.m_GuildSettings.m_IgnoreGuildInvitations)
                {
                    m_Player.SendMessage("That player is not accepting invitations for guild membership.");
                    return;
                }

                if (m_Guild.GetCandidate(playerTarget) != null)
                {
                    m_Player.SendMessage("That player is already a candidate for membership in this guild.");
                    return;
                }

                else
                {
                    GuildInvitation invitation = new GuildInvitation(playerTarget, m_Player, m_Guild, DateTime.UtcNow, false);

                    playerTarget.m_GuildSettings.m_GuildInvitations.Add(invitation);
                    m_Guild.m_Candidates.Add(invitation);
                    
                    m_Player.SendMessage(Guilds.GuildTextHue, playerTarget.RawName + " has been made a candidate for guild membership.");
                    playerTarget.SendMessage(Guilds.GuildTextHue, "You have received an invitation for membership in a guild.");

                    return;
                }
            }            
        }

        public override void OnDelete()
        {
            Queue m_Queue = new Queue();

            //Members
            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry.m_Player == null)
                    continue;

                entry.m_Player.Guild = null;
                entry.m_Player.m_GuildMemberEntry = null;

                m_Queue.Enqueue(entry);
            }

            while (m_Queue.Count > 0)
            {
                GuildMemberEntry entry = (GuildMemberEntry)m_Queue.Dequeue();

                if (m_Members.Contains(entry))
                    m_Members.Remove(entry);
            }    
 
            //Candiates
            m_Queue = new Queue();

            foreach (GuildInvitation invitation in m_Candidates)
            {
                if (invitation == null)
                    continue;

                m_Queue.Enqueue(invitation);
            }

            while (m_Queue.Count > 0)
            {
                GuildInvitation invitation = (GuildInvitation)m_Queue.Dequeue();

                invitation.Delete();
            }

            //Relationships   
            m_Queue = new Queue();

            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null)
                    continue;

                m_Queue.Enqueue(relationship);
            }

            while (m_Queue.Count > 0)
            {
                GuildRelationship relationship = (GuildRelationship)m_Queue.Dequeue();

                relationship.Delete();
            }

            if (Guilds.m_Guilds.Contains(this))
                Guilds.m_Guilds.Remove(this);

            base.OnDelete();
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Guildmaster);
            writer.Write(m_Abbreviation);
            writer.Write(m_CreationTime);
            writer.Write(m_Icon);
            writer.Write(m_IconHue);
            writer.Write(m_Website);
            writer.Write(m_Guildhouse);
            writer.Write(m_Faction);

            writer.Write(m_RankNames.Length);
            for (int a = 0; a < m_RankNames.Length; a++)
            {
                writer.Write(m_RankNames[a]);
            }

            writer.Write(m_Members.Count);
            for (int a = 0; a < m_Members.Count; a++)
            {
                writer.Write(m_Members[a].m_Player);
                writer.Write((int)m_Members[a].m_Rank);
                writer.Write(m_Members[a].m_JoinDate);
                writer.Write(m_Members[a].m_DeclaredFealty);
            }            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Guildmaster = (PlayerMobile)reader.ReadMobile();
                m_Abbreviation = reader.ReadString();
                m_CreationTime = reader.ReadDateTime();
                m_Icon = reader.ReadInt();
                m_IconHue = reader.ReadInt();
                m_Website = reader.ReadString();
                m_Guildhouse = reader.ReadItem() as BaseHouse;
                m_Faction = reader.ReadItem() as Faction;

                int rankNamesCount = reader.ReadInt();
                for (int a = 0; a < rankNamesCount; a++)
                {
                    m_RankNames[a] = reader.ReadString();
                }     
           
                int membersCount = reader.ReadInt();
                for (int a = 0; a < membersCount; a++)
                {
                    PlayerMobile player = reader.ReadMobile() as PlayerMobile;
                    GuildMemberRank rank = (GuildMemberRank)reader.ReadInt();
                    DateTime joinDate = reader.ReadDateTime();
                    PlayerMobile fealtyPlayer = reader.ReadMobile() as PlayerMobile;

                    if (player != null)
                    {
                        if (!player.Deleted)
                        {
                            GuildMemberEntry guildMemberEntry = new GuildMemberEntry(player, rank, joinDate, fealtyPlayer);

                            m_Members.Add(guildMemberEntry);
                        }
                    }                    
                }
            }

            //-----

            Guilds.m_Guilds.Add(this);

            ReapplyMemberships();

            AuditGuild();
        }
    }
}
