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

        public enum RelationshipSortCriteria
        {
            None,
            GuildName,
            Relationship,
            PlayerCount
        }

        public enum RelationshipFilterType
        {            
            ShowActiveAgreements,
            ShowAgreementsReceived,
            ShowAgreementsSent,
            ShowAvailableGuilds
        }

        public enum GuildRelationshipType
        {
            Neutral,
            War,
            Alliance,
            WarRequest,
            AllianceRequest
        }

        public static int GuildNameCharacterLimit = 40;
        public static int GuildAbbreviationCharacterLimit = 3;

        public static int GuildRegistrationFee = 50000;

        public static int GuildTextHue = 63;

        public static double RequiredFealtyPercentageForChange = .67;
        
        public static TimeSpan InvitationExpiration = TimeSpan.FromDays(14);
        public static TimeSpan GuildRequestExpiration = TimeSpan.FromDays(14);

        public static TimeSpan CharacterInactivityThreshold = TimeSpan.FromDays(60);

        public static int GuildGumpSelectionSound = 0x3E6;
        public static int GuildGumpChangePageSound = 0x057;
        public static int GuildGumpOpenGumpSound = 0x055;
        public static int GuildGumpCloseGumpSound = 0x058;

        public static int SuccessSound = 0x5A7;
        public static int PromotionSound = 0x5A7;

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

                Guilds.AuditAllGuilds();
            });
        }

        [Usage("[Guild")]
        [Description("Launches the Guild interface")]
        public static void GuildCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            LaunchGuildGump(player);
        }

        public static void LaunchGuildGump(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateGuildGuildSettings(player);

            if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CandidatePreview)
                player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Candidates;

            if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.DiplomacyManagement)
                player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Diplomacy;

            if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.InvitationGuildPreview)
                player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Invitations;

            if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.MemberManagement)
                player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.Members;

            GuildGumpObject guildGumpObject = new GuildGumpObject();

            int startingGuildTabPage = Guilds.DetermineStartingGuildTabPage(player);

            guildGumpObject.m_Guild = player.Guild;
            guildGumpObject.m_GuildTabPage = startingGuildTabPage;
            guildGumpObject.m_RelationshipFilterType = player.m_GuildSettings.m_RelationshipFilterType;
            
            Guilds.SendGuildGump(player, guildGumpObject);
        }

        public static void OnLogin(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateGuildGuildSettings(player);

            if (player.Guild != null)
            {
                player.Guild.UpdatePlayerCount();
                player.Guild.UpdateCharacterCount();
            }
        }

        public static void OnLogout(PlayerMobile player)
        {
            if (player == null)
                return;

            if (player.Guild != null)
            {
                player.Guild.UpdatePlayerCount();
                player.Guild.UpdateCharacterCount();
            }
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

        public static void AuditAllGuilds()
        {
            Queue m_Queue = new Queue();

            foreach (Guild guild in m_Guilds)
            {
                if (guild == null) continue;
                if (guild.Deleted) continue;

                m_Queue.Enqueue(guild);
            }

            while (m_Queue.Count > 0)
            {
                Guild guild = (Guild)m_Queue.Dequeue();
                
                guild.AuditGuild();
            }
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
                if (!(player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Invitations || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.InvitationGuildPreview))
                    player.m_GuildSettings.m_GuildGumpPage = GuildGumpPageType.CreateGuild;
            }

            else
            {
                if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.Invitations || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.InvitationGuildPreview)
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

            bool skipPageSearch = false;

            if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.InvitationGuildPreview || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.CandidatePreview)
                skipPageSearch = true;

            if (player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.MemberManagement || player.m_GuildSettings.m_GuildGumpPage == GuildGumpPageType.DiplomacyManagement)
                skipPageSearch = true;

            int GuildTabsPerPage = 4;
            int TotalGuildTabs = validGuildTabs.Count;
            int TotalGuildTabPages = (int)(Math.Ceiling((double)TotalGuildTabs / (double)GuildTabsPerPage));

            if (guildGumpObject.m_GuildTabPage >= TotalGuildTabPages)
                guildGumpObject.m_GuildTabPage = TotalGuildTabPages - 1;

            if (guildGumpObject.m_GuildTabPage < 0)
                guildGumpObject.m_GuildTabPage = 0;

            if (!skipPageSearch && !validGuildTabs.Contains(player.m_GuildSettings.m_GuildGumpPage))
            {               
                player.m_GuildSettings.m_GuildGumpPage = validGuildTabs[0];
                guildGumpObject.m_GuildTabPage = 0;
            }
            
            player.CloseGump(typeof(GuildGump));
            player.SendGump(new GuildGump(player, guildGumpObject));
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

    public class DiplomacyEntry
    {
        public Guild m_Guild;
        public GuildRelationship m_GuildRelationship;

        public DiplomacyEntry(Guild guild, GuildRelationship guildRelationship)
        {
            m_Guild = guild;
            m_GuildRelationship = guildRelationship;
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

        public static string GetDisplayName(Guilds.GuildRelationshipType relationshipType, bool guildFrom)
        {
            string relationshipText = "";

            switch (relationshipType)
            {
                case Guilds.GuildRelationshipType.Neutral:
                    relationshipText = "Neutral";
                break;

                case Guilds.GuildRelationshipType.War:
                    relationshipText = "At War";
                break;

                case Guilds.GuildRelationshipType.Alliance:
                    relationshipText = "Alliance";
                break;

                case Guilds.GuildRelationshipType.WarRequest:
                    if (guildFrom)
                        relationshipText = "War Offered";
                    else
                        relationshipText = "War Requested";
                break;

                case Guilds.GuildRelationshipType.AllianceRequest:
                    if (guildFrom)
                        relationshipText = "Alliance Offered";
                    else
                        relationshipText = "Allianced Requested";
                break;
            }

            return relationshipText;
        }

        public static int GetHue(Guilds.GuildRelationshipType relationshipType, bool guildFrom)
        {
            int relationshipTextHue = 2655;

            switch (relationshipType)
            {
                case Guilds.GuildRelationshipType.Neutral:
                    relationshipTextHue = 2655;
                break;

                case Guilds.GuildRelationshipType.War:
                    relationshipTextHue = 1256;
                break;

                case Guilds.GuildRelationshipType.Alliance:
                    relationshipTextHue = 2599;
                break;

                case Guilds.GuildRelationshipType.WarRequest:
                    if (guildFrom)
                        relationshipTextHue = 1256;
                    else
                        relationshipTextHue = 1256;
                    break;

                case Guilds.GuildRelationshipType.AllianceRequest:
                    if (guildFrom)
                        relationshipTextHue = 2599;
                    else
                        relationshipTextHue = 2599;
                break;
            }

            return relationshipTextHue;
        }

        public bool CheckExpired()
        {
            if (m_RelationshipType == Guilds.GuildRelationshipType.WarRequest || m_RelationshipType == Guilds.GuildRelationshipType.AllianceRequest)
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

        public Guilds.RelationshipFilterType m_RelationshipFilterType = Guilds.RelationshipFilterType.ShowActiveAgreements;

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
            writer.Write((int)m_RelationshipFilterType);
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
                m_RelationshipFilterType = (Guilds.RelationshipFilterType)reader.ReadInt();
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

        public GuildSymbolType m_GuildSymbol;

        public string m_Website = "http://www.outlandsuo.com/index.html";

        public BaseHouse m_Guildhouse;

        public Faction m_Faction = null;

        public int m_ActivePlayers = 0;
        public int m_ActiveCharacters = 0;

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

        public void UpdatePlayerCount()
        {
            List<string> m_AccountNames = new List<string>();

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;
                if (entry.m_Player.Deleted) continue;

                if (IsCharacterActive(entry.m_Player))
                {
                    Account account = entry.m_Player.Account as Account;                    

                    if (account == null)
                        continue;

                    if (!m_AccountNames.Contains(account.Username))
                        m_AccountNames.Add(account.Username);
                }
            }

            m_ActivePlayers = m_AccountNames.Count;
        }

        public void UpdateCharacterCount()
        {
            m_ActiveCharacters = 0;

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;
                if (entry.m_Player.Deleted) continue;

                if (IsCharacterActive(entry.m_Player))                
                    m_ActiveCharacters++;                
            }            
        }

        public int GetWarCount(bool activeOnly)
        {
            int totalWars = 0;

            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null) continue;

                if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.War)
                    totalWars++;                     
            }

            return totalWars;
        }

        public int GetAllyCount(bool activeOnly)
        {
            int totalAllies = 0;

            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null) continue;

                if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.Alliance)
                    totalAllies++;
            }

            return totalAllies;
        }

        public string GetGuildAge()
        {
            int guildAge = (int)(Math.Floor((DateTime.UtcNow - m_CreationTime).TotalDays));

            string guildAgeText = "";

            if (guildAge > 1)
                guildAgeText = guildAge.ToString() + " Days";

            else if (guildAge == 1)
                guildAgeText = "1 Day";

            else
                guildAgeText = "Founded Today";

            return guildAgeText;
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

        public bool CanPromoteMembers(GuildMemberRank rank)
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

        public bool CanDismissPlayer(GuildMemberRank playerRank, GuildMemberRank playerTargetRank)
        {
            if (playerRank == GuildMemberRank.Recruit || playerRank == GuildMemberRank.Initiate || playerRank == GuildMemberRank.Veteran)
                return false;

            if ((int)playerRank > (int)playerTargetRank)
                return true;

            return false;
        }

        public bool CanManageDiplomacy(GuildMemberRank rank)
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

        public bool IsCharacterActive(PlayerMobile player)
        {
            if (player == null) return false;
            if (player.Deleted) return false;

            if (IsMember(player))
            {
                if (player.LastOnline + Guilds.CharacterInactivityThreshold >= DateTime.UtcNow)
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
                            if (string.Compare(memberEntry.m_Player.RawName, memberListItem.m_Player.RawName) >= 0)
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
            
            List<string> accountList = new List<string>();

            int characterCount = 0;

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
                    if (IsCharacterActive(member.m_Player))
                    {
                        characterCount++;

                        Account account = member.m_Player.Account as Account;

                        if (account != null)
                        {
                            if (!accountList.Contains(account.Username))
                                accountList.Add(account.Username);
                        }
                    }

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

            m_ActiveCharacters = characterCount;
            m_ActivePlayers = accountList.Count;
        }

        public GuildRelationship GetGuildRelationship(Guild targetGuild)
        {
            GuildRelationship relationship = null;

            foreach (GuildRelationship guildRelationship in m_Relationships)
            {
                if (guildRelationship == null) continue;
                if (guildRelationship.Deleted) continue;
                if (guildRelationship.m_GuildFrom == null) continue;
                if (guildRelationship.m_GuildFrom.Deleted) continue;
                if (guildRelationship.m_GuildTarget == null) continue;
                if (guildRelationship.m_GuildTarget.Deleted) continue;

                if (guildRelationship.m_GuildFrom == this && guildRelationship.m_GuildTarget != this)
                    return guildRelationship;

                if (guildRelationship.m_GuildFrom != this && guildRelationship.m_GuildTarget == this)
                    return guildRelationship;
            }

            return relationship;
        }

        public List<DiplomacyEntry> GetGuildRelationships(Guilds.RelationshipFilterType filterType, Guilds.RelationshipSortCriteria sortCriteria, bool ascending)
        {
            List<DiplomacyEntry> relationshipList = new List<DiplomacyEntry>();            
            
            switch (filterType)
            {
                case Guilds.RelationshipFilterType.ShowActiveAgreements:
                    foreach (GuildRelationship relationship in m_Relationships)
                    {
                        if (relationship == null) continue;
                        if (relationship.Deleted) continue;

                        if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.War || relationship.m_RelationshipType == Guilds.GuildRelationshipType.Alliance)
                        {
                            if (relationship.m_GuildFrom == this)
                            {
                                DiplomacyEntry diplomacyEntry = new DiplomacyEntry(relationship.m_GuildTarget, relationship);

                                relationshipList.Add(diplomacyEntry);
                            }

                            else
                            {
                                DiplomacyEntry diplomacyEntry = new DiplomacyEntry(relationship.m_GuildFrom, relationship);

                                relationshipList.Add(diplomacyEntry);
                            }
                        }
                    }
                break;                    

                case Guilds.RelationshipFilterType.ShowAgreementsSent:
                    foreach (GuildRelationship relationship in m_Relationships)
                    {
                        if (relationship == null) continue;
                        if (relationship.Deleted) continue;

                        if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.WarRequest || relationship.m_RelationshipType == Guilds.GuildRelationshipType.AllianceRequest)
                        {
                            if (relationship.m_GuildFrom == this)
                            {
                                DiplomacyEntry diplomacyEntry = new DiplomacyEntry(relationship.m_GuildTarget, relationship);

                                relationshipList.Add(diplomacyEntry);
                            }
                        }
                    }                        
                break;

                case Guilds.RelationshipFilterType.ShowAgreementsReceived:
                    foreach (GuildRelationship relationship in m_Relationships)
                    {
                        if (relationship == null) continue;
                        if (relationship.Deleted) continue;

                        if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.WarRequest || relationship.m_RelationshipType == Guilds.GuildRelationshipType.AllianceRequest)
                        {
                            if (relationship.m_GuildTarget == this)
                            {
                                DiplomacyEntry diplomacyEntry = new DiplomacyEntry(relationship.m_GuildFrom, relationship);

                                relationshipList.Add(diplomacyEntry);
                            }
                        }
                    }

                break;

                case Guilds.RelationshipFilterType.ShowAvailableGuilds:
                    foreach (Guild guild in Guilds.m_Guilds)
                    {
                        if (guild == null) continue;
                        if (guild.Deleted) continue;
                        if (guild == this) continue;

                        DiplomacyEntry diplomacyEntry = new DiplomacyEntry(guild, null);

                        bool foundAgreement = false;

                        for (int a = 0; a < guild.m_Relationships.Count; a++)
                        {
                            if (guild.m_Relationships[a].m_GuildFrom == this || guild.m_Relationships[a].m_GuildTarget == this)
                            {
                                foundAgreement = true;
                                break;
                            }
                        }

                        if (!foundAgreement)
                            relationshipList.Add(diplomacyEntry);
                    }
                break;
            }

            if (sortCriteria == Guilds.RelationshipSortCriteria.None)
                return relationshipList;

            List<DiplomacyEntry> relationshipListSorted = new List<DiplomacyEntry>();
            
            for (int a = 0; a < relationshipList.Count; a++)
            {
                DiplomacyEntry diplomacyEntry = relationshipList[a];

                if (diplomacyEntry.m_Guild == null) continue;
                if (diplomacyEntry.m_Guild.Deleted) continue;
                
                int newIndexPosition = -1;

                for (int b = 0; b < relationshipListSorted.Count; b++)
                {
                    DiplomacyEntry diplomacyEntryTarget = relationshipListSorted[b];

                    if (diplomacyEntryTarget.m_Guild == null) 
                        continue;

                    switch (sortCriteria)
                    {
                        case Guilds.RelationshipSortCriteria.GuildName:
                            if (string.Compare(diplomacyEntry.m_Guild.Name, diplomacyEntryTarget.m_Guild.Name) >= 0)
                                newIndexPosition = b + 1;
                            break;

                        case Guilds.RelationshipSortCriteria.Relationship:
                            Guilds.GuildRelationshipType fromRelationship = Guilds.GuildRelationshipType.Neutral;
                            Guilds.GuildRelationshipType targetRelationship = Guilds.GuildRelationshipType.Neutral;

                            if (diplomacyEntry.m_GuildRelationship != null)
                                fromRelationship = diplomacyEntry.m_GuildRelationship.m_RelationshipType;

                            if (diplomacyEntryTarget.m_GuildRelationship != null)
                                targetRelationship = diplomacyEntryTarget.m_GuildRelationship.m_RelationshipType;

                            if ((int)fromRelationship >= (int)targetRelationship)
                                newIndexPosition = b + 1;
                        break;

                        case Guilds.RelationshipSortCriteria.PlayerCount:
                            if (diplomacyEntry.m_Guild.m_ActivePlayers >= diplomacyEntryTarget.m_Guild.m_ActivePlayers)
                                newIndexPosition = b + 1;
                        break;
                    }
                }

                if (newIndexPosition == -1)                
                    relationshipListSorted.Insert(0, diplomacyEntry);                

                else
                {
                    if (newIndexPosition >= relationshipList.Count)
                        relationshipListSorted.Add(diplomacyEntry);

                    else
                        relationshipListSorted.Insert(newIndexPosition, diplomacyEntry);
                }
            }

            if (!ascending)
                relationshipListSorted.Reverse(0, relationshipListSorted.Count);

            return relationshipListSorted;  
        }

        public void AuditRelationships()
        {
            Queue m_Queue = new Queue();

            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null)
                    continue;

                if (relationship.m_GuildFrom == null)
                    m_Queue.Enqueue(relationship);

                else if (relationship.m_GuildFrom.Deleted)
                    m_Queue.Enqueue(relationship);

                else if (relationship.m_GuildTarget == null)
                    m_Queue.Enqueue(relationship);

                else if (relationship.m_GuildTarget.Deleted)
                    m_Queue.Enqueue(relationship);

                if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.WarRequest || relationship.m_RelationshipType == Guilds.GuildRelationshipType.AllianceRequest)
                {
                    if (relationship.m_DateIssued + Guilds.GuildRequestExpiration <= DateTime.UtcNow)
                        m_Queue.Enqueue(relationship);
                }
            }

            while (m_Queue.Count > 0)
            {
                GuildRelationship relationship = (GuildRelationship)m_Queue.Dequeue();

                relationship.Delete();
            }
        }

        public bool IsAlliedGuild(Guild guild)
        {
            foreach (GuildRelationship relationship in m_Relationships)
            {
                if (relationship == null) continue;
                if (relationship.Deleted) continue;

                if (relationship.m_RelationshipType == Guilds.GuildRelationshipType.Alliance)
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

            UpdatePlayerCount();
            UpdateCharacterCount();

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
            
            if (player != null)
            {
                player.Guild = null;
                player.m_GuildMemberEntry = null;

                if (forced)
                    player.SendMessage(Guilds.GuildTextHue, "You have been removed from " + GetDisplayName(true) + ".");

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

            UpdatePlayerCount();
            UpdateCharacterCount();
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

                    bool isActiveMember = IsCharacterActive(entry.m_Player);

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
            {
                DisbandGuild();

                return;
            }

            else if (m_Guildmaster == null)
                AssignNewGuildmaster(m_Guildmaster);

            else if (m_Guildmaster.Deleted)
                AssignNewGuildmaster(m_Guildmaster);

            UpdatePlayerCount();
            UpdateCharacterCount();
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
            writer.Write((int)m_GuildSymbol);
            writer.Write(m_Website);
            writer.Write(m_Guildhouse);
            writer.Write(m_Faction);
            writer.Write(m_ActivePlayers);
            writer.Write(m_ActiveCharacters);

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
                m_GuildSymbol = (GuildSymbolType)reader.ReadInt();
                m_Website = reader.ReadString();
                m_Guildhouse = reader.ReadItem() as BaseHouse;
                m_Faction = reader.ReadItem() as Faction;
                m_ActivePlayers = reader.ReadInt();
                m_ActiveCharacters = reader.ReadInt();

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
        }
    }
}
