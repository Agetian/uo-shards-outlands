using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Accounting;
using Server.Gumps;
using Server.Custom;

namespace Server.Items
{
    public class TreasureMapLibrary : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
        }        

        private LockedDownAccessLevelType m_LockedDownAccessLevel = LockedDownAccessLevelType.Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public LockedDownAccessLevelType LockedDownAccessLevel
        {
            get { return m_LockedDownAccessLevel; }
            set { m_LockedDownAccessLevel = value; }
        }

        private bool m_RemoveAllOnSelection = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveAllOnSelection
        {
            get { return m_RemoveAllOnSelection; }
            set { m_RemoveAllOnSelection = value; }
        }

        public override bool AlwaysAllowDoubleClick { get { return true; } }

        public List<TreasureMapLibraryEntry> m_LibraryEntries = new List<TreasureMapLibraryEntry>();
        public List<TreasureMap> m_DecodedMaps = new List<TreasureMap>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public TreasureMapLibrary(): base(8793)
        {
            Name = "a treasure map library";
            Hue = 2503;
            Weight = 5;

            CreateEntries();
        }

        public TreasureMapLibrary(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            int undecoded = GetTotalCount(false);
            int decoded = GetTotalCount(true);

            LabelTo(from, "(" + undecoded.ToString() + " undecoded and " + decoded.ToString() + " decoded)");

            if (IsLockedDown)
            {
                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner: LabelTo(from, "[owner accessable]"); break;
                    case LockedDownAccessLevelType.CoOwner: LabelTo(from, "[co-owner accessable]"); break;
                    case LockedDownAccessLevelType.Friend: LabelTo(from, "[friend accessable]"); break;
                    case LockedDownAccessLevelType.Anyone: LabelTo(from, "[freely access]"); break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!CanUse(player))
                return;

            player.SendSound(openGumpSound);

            from.CloseGump(typeof(TreasureMapLibraryGump));
            from.SendGump(new TreasureMapLibraryGump(player, this, 1));
        }

        public int GetTotalCount(bool decoded)
        {
            int totalCount = 0;

            foreach (TreasureMapLibraryEntry entry in m_LibraryEntries)
            {
                if (entry == null) continue;
                if (decoded != entry.Decoded) continue;

                totalCount += entry.Count;
            }

            return totalCount;
        }

        public void CreateEntries()
        {
            for (int a = TreasureMap.MinLevel; a < TreasureMap.MaxLevel + 1; a++)
            {
                TreasureMapLibraryEntry entry = new TreasureMapLibraryEntry();
                entry.Decoded = false;
                entry.MapLevel = a;

                m_LibraryEntries.Add(entry);
            }

            for (int a = TreasureMap.MinLevel; a < TreasureMap.MaxLevel + 1; a++)
            {
                TreasureMapLibraryEntry entry = new TreasureMapLibraryEntry();
                entry.Decoded = true;
                entry.MapLevel = a;

                m_LibraryEntries.Add(entry);
            }
        }

        public void AuditEntries()
        {
            for (int a = TreasureMap.MinLevel; a < TreasureMap.MaxLevel + 1; a++)
            {
                TreasureMapLibraryEntry existingEntry = GetLibraryEntry(a, false);

                if (existingEntry == null)
                {
                    TreasureMapLibraryEntry newEntry = new TreasureMapLibraryEntry();
                    newEntry.Decoded = false;
                    newEntry.MapLevel = a;

                    m_LibraryEntries.Add(newEntry);
                }
            }

            for (int a = TreasureMap.MinLevel; a < TreasureMap.MaxLevel + 1; a++)
            {
                TreasureMapLibraryEntry existingEntry = GetLibraryEntry(a, true);

                if (existingEntry == null)
                {
                    TreasureMapLibraryEntry newEntry = new TreasureMapLibraryEntry();
                    newEntry.Decoded = true;
                    newEntry.MapLevel = a;

                    m_LibraryEntries.Add(newEntry);
                }
            }
        }

        public TreasureMapLibraryEntry GetLibraryEntry(int mapLevel, bool decoded)
        {
            TreasureMapLibraryEntry targetEntry = null;

            foreach (TreasureMapLibraryEntry entry in m_LibraryEntries)
            {
                if (entry.MapLevel == mapLevel && entry.Decoded == decoded)
                    return entry;
            }

            return targetEntry;
        }
        
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is TreasureMap)
            {
                TreasureMap treasureMap = dropped as TreasureMap;

                if (treasureMap.Completed)
                {
                    from.SendMessage("Only uncompleted treasure maps may be stored within this library.");
                    return false;
                }

                TreasureMapLibraryEntry entry = GetLibraryEntry(treasureMap.Level, treasureMap.Decoded);

                if (entry != null)
                {
                    if (treasureMap.Decoded)
                    {
                        treasureMap.Archived = true;
                        m_DecodedMaps.Add(treasureMap);
                        treasureMap.Internalize();
                    }

                    else
                        treasureMap.Delete();

                    entry.Count++;

                    from.SendMessage("You add a treasure map to the library.");
                    from.SendSound(addItemSound);
                }

                return true;
            }

            else
                return false;
        }

        public void AddAllMapsInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;

            List<TreasureMap> m_TreasureMaps = from.Backpack.FindItemsByType<TreasureMap>();

            int totalCount = 0;

            Queue m_Queue = new Queue();

            foreach (TreasureMap treasureMap in m_TreasureMaps)
            {
                if (treasureMap.Completed)
                    continue;

                m_Queue.Enqueue(treasureMap);                
            }

            while (m_Queue.Count > 0)
            {
                TreasureMap treasureMap = (TreasureMap)m_Queue.Dequeue();
                TreasureMapLibraryEntry entry = GetLibraryEntry(treasureMap.Level, treasureMap.Decoded);

                if (entry == null)
                    continue;

                entry.Count++;                
                totalCount++;

                if (treasureMap.Decoded)
                {
                    m_DecodedMaps.Add(treasureMap);

                    treasureMap.Archived = true;                    
                    treasureMap.Internalize();
                }

                else
                    treasureMap.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " treasure maps to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a treasure map to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any uncompleted treasure maps in your backpack.");
        }

        public void EjectMap(Mobile from, int mapLevel, bool decoded, bool ejectAll)
        {
            if (from == null )
                return;

            TreasureMapLibraryEntry entry = GetLibraryEntry(mapLevel, decoded);

            if (entry == null)
                return;

            if (entry.Count == 0)
            {
                from.SendMessage("You do not have any treasure maps of that type currently stored within this library.");
                return;
            }            

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems == from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is at capacity. Please remove some items and try again.");
                return;
            }

            if (ejectAll)
            {
                int spaceAvailable = from.Backpack.MaxItems - from.Backpack.TotalItems;

                int retrievalAmount = 0;
                bool partialRetrieval = false;

                if (spaceAvailable >= entry.Count)
                    retrievalAmount = entry.Count;

                else
                {
                    partialRetrieval = true;
                    retrievalAmount = spaceAvailable;
                }

                for (int a = 0; a < retrievalAmount; a++)
                {
                    if (decoded)
                    {
                        TreasureMap mapMatch = null;

                        foreach (TreasureMap existingMap in m_DecodedMaps)
                        {
                            if (entry.MapLevel == existingMap.Level)
                            {
                                mapMatch = existingMap;
                                break;
                            }
                        }

                        if (mapMatch != null)
                        {
                            mapMatch.Archived = false;
                            m_DecodedMaps.Remove(mapMatch);

                            from.AddToBackpack(mapMatch);
                        }
                    }

                    else
                    {
                        TreasureMap newMap = new TreasureMap(mapLevel, Map.Felucca);
                        newMap.Decoded = decoded;
                        from.Backpack.DropItem(newMap);
                    }
                }

                entry.Count -= retrievalAmount;
                from.SendSound(addItemSound);

                if (entry.Count == 1)
                    from.SendMessage("You retrieve a treasure map from the library.");

                else if (partialRetrieval)
                    from.SendMessage("You retrieve several treasure maps from the library but require more backpack space in order to retrieve the remaining maps.");

                else
                    from.SendMessage("You retrieve several treasure maps from the library.");
            }

            else
            {
                if (decoded)
                {
                    TreasureMap mapMatch = null;

                    foreach (TreasureMap existingMap in m_DecodedMaps)
                    {
                        if (entry.MapLevel == existingMap.Level)
                        {
                            mapMatch = existingMap;
                            break;
                        }
                    }

                    m_DecodedMaps.Remove(mapMatch);
                    from.Backpack.DropItem(mapMatch);
                }

                else
                {
                    TreasureMap newMap = new TreasureMap(mapLevel, Map.Felucca);
                    newMap.Decoded = decoded;

                    from.Backpack.DropItem(newMap);                   
                }

                entry.Count--;
                from.SendSound(addItemSound);
                from.SendMessage("You retrieve a treasure map from the library.");
            }
        }

        public bool CanUse(Mobile from)
        {
            if (from == null)
                return false;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            if (!from.Alive)
                return false;
            
            if (IsChildOf(from.Backpack) || IsChildOf(from.BankBox))
                return true;

            if (from.Map != Map || !from.InRange(GetWorldLocation(), 2) )
            {
                from.SendMessage("That is too far away to use.");
                return false;
            }

            if (!from.InLOS(this))
            {
                from.SendMessage("That item is out of your line of sight.");
                return false;
            }

            if (IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(Location, Map, 64);

                if (house == null)
                {
                    from.SendMessage("That is not accessible.");
                    return false;
                }
                
                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner:
                        if (house.Owner != null)
                        {
                            Account ownerAccount = house.Owner.Account as Account;
                            Account playerAccount = from.Account as Account;

                            if (ownerAccount != null && playerAccount != null && ownerAccount == playerAccount)
                                return true;

                            else
                            {
                                from.SendMessage("You do not have the neccessary access rights to use that.");
                                return false;
                            }
                        }
                    break;

                    case LockedDownAccessLevelType.CoOwner:
                        if (house.IsCoOwner(from))
                            return true;
                        else
                        {
                            from.SendMessage("You do not have the neccessary access rights to use that.");
                            return false;
                        }
                    break;

                    case LockedDownAccessLevelType.Friend:
                        if (house.IsFriend(from))
                            return true;

                        else
                            {
                                from.SendMessage("You do not have the neccessary access rights to use that.");
                                return false;
                            }

                    break;

                    case LockedDownAccessLevelType.Anyone:
                        return true;
                    break;
                }
            }

            if (RootParent is PlayerMobile && RootParent != from)
            {
                from.SendMessage("That is not accessible.");
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 

            writer.Write((int)m_LockedDownAccessLevel);
            writer.Write(m_RemoveAllOnSelection);

            //Version 0
            writer.Write(m_LibraryEntries.Count);
            for (int a = 0; a < m_LibraryEntries.Count; a++)
            {
                TreasureMapLibraryEntry entry = m_LibraryEntries[a];

                writer.Write(entry.MapLevel);
                writer.Write(entry.Decoded);
                writer.Write(entry.Count);
            }

            writer.Write(m_DecodedMaps.Count);
            for (int a = 0; a < m_DecodedMaps.Count; a++)
            {
                writer.Write(m_DecodedMaps[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version
            if (version >= 0)
            {
                m_LockedDownAccessLevel = (LockedDownAccessLevelType)reader.ReadInt();
                m_RemoveAllOnSelection = reader.ReadBool();

                int libraryEntryCount = reader.ReadInt();
                for (int a = 0; a < libraryEntryCount; a++)
                {
                    int mapLevel = reader.ReadInt();
                    bool decoded = reader.ReadBool();
                    int count = reader.ReadInt();

                    TreasureMapLibraryEntry entry = new TreasureMapLibraryEntry();

                    entry.MapLevel = mapLevel;
                    entry.Decoded = decoded;
                    entry.Count = count;

                    m_LibraryEntries.Add(entry);
                }

                int decodedCount = reader.ReadInt();
                for (int a = 0; a < decodedCount; a++)
                {
                    TreasureMap decodedMap = (TreasureMap)reader.ReadItem();

                    if (decodedMap != null)
                        m_DecodedMaps.Add(decodedMap);
                }
            }

            //-----

            AuditEntries();
        }
    }

    public class TreasureMapLibraryEntry
    {
        public int MapLevel = 1;
        public bool Decoded = false;
        public int Count = 0;
    }

    public class TreasureMapLibraryGump : Gump
    {
        PlayerMobile m_Player;
        TreasureMapLibrary m_Library; 

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 8;
        int EntriesPerPage = 16;

        int WhiteTextHue = 2499; 

        public TreasureMapLibraryGump(PlayerMobile player, TreasureMapLibrary library, int pageNumber): base(10, 10)
        {
            if (player == null) return;
            if (library == null) return;
            if (library.Deleted) return;

            m_Player = player;    
            m_Library = library;
            m_PageNumber = pageNumber;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            #region Images 

            AddImage(205, 193, 11015, 2503);
            AddImage(204, 1, 11015, 2503);
            AddImage(3, 192, 11015, 2503);
            AddImage(3, 1, 11015, 2503);

            AddImage(302, 75, 2081, 2499);
            AddImage(300, 270, 2081, 2499);
            AddImage(301, 141, 2081, 2499);
            AddImage(301, 5, 2081, 2499);
            AddImage(301, 206, 2081, 2499);
            AddImage(299, 338, 2081, 2499);
            AddImage(44, 6, 2081, 2499);
            AddImage(44, 75, 2081, 2499);
            AddImage(43, 141, 2081, 2499);
            AddImage(43, 206, 2081, 2499);
            AddImage(41, 335, 2081);
            AddImage(43, 274, 2081, 2499);           
            AddImage(41, 338, 2081, 2499);            
            AddImage(49, 80, 3001, 2615);
            AddImage(56, 80, 3001, 2615);
            AddImage(306, 80, 3001, 2615);
            AddImage(315, 80, 3001, 2615);
            AddImageTiled(301, 2, 6, 405, 2701);

            AddItem(153, 25, 5356, 0);

            #endregion

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);            
         
            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case TreasureMapLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case TreasureMapLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case TreasureMapLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case TreasureMapLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
            }

            AddLabel(Utility.CenteredTextOffset(435, accessName), 25, 2562, accessName);

            AddButton(366, 28, 2223, 2223, 2, GumpButtonType.Reply, 0);
            AddButton(488, 29, 2224, 2224, 3, GumpButtonType.Reply, 0);

            AddLabel(347, 53, WhiteTextHue, "Remove All Possible on Selection");
            
            if (m_Library.RemoveAllOnSelection)
                AddButton(313, 48, 2154, 2151, 4, GumpButtonType.Reply, 0);
            else
                AddButton(313, 48, 2151, 2154, 4, GumpButtonType.Reply, 0);

            //-----

            AddLabel(105, 5, 2415, "Treasure Map Library");

            int leftStartY = 90;
            int rightStartY = 90;
            int itemSpacing = 30;
            
            //Left Side: Undecoded
            AddLabel(142, leftStartY, 2502, "Undecoded");

            leftStartY += itemSpacing;

            for (int a = TreasureMap.MinLevel; a < TreasureMap.MaxLevel + 1; a++)
            {
                TreasureMapLibraryEntry entry = m_Library.GetLibraryEntry(a, false);

                int numberTextHue = WhiteTextHue;

                if (entry.Count > 0)
                    numberTextHue = 2502;

                if (entry != null)
                {
                    AddLabel(60, leftStartY, 2502, TreasureMap.GetMapDisplayName(a));
                    AddButton(230, leftStartY + 3, 2117, 2118, 10 + a, GumpButtonType.Reply, 0);
                    AddLabel(250, leftStartY, numberTextHue, entry.Count.ToString());

                    leftStartY += itemSpacing;
                }
            }

            //Right Side: Decoded
            AddLabel(408, rightStartY, 2413, "Decoded");

            rightStartY += itemSpacing;

            for (int a = TreasureMap.MinLevel; a < TreasureMap.MaxLevel + 1; a++)
            {
                TreasureMapLibraryEntry entry = m_Library.GetLibraryEntry(a, true);

                int numberTextHue = WhiteTextHue;

                if (entry.Count > 0)
                    numberTextHue = 2413;

                if (entry != null)
                {
                    AddLabel(325, rightStartY, 2413, TreasureMap.GetMapDisplayName(a));
                    AddButton(495, rightStartY + 3, 2117, 2118, 20 + a, GumpButtonType.Reply, 0);
                    AddLabel(515, rightStartY, numberTextHue, entry.Count.ToString());

                    rightStartY += itemSpacing;
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Backpack == null) return;

            if (m_Library == null) return;
            if (m_Library.Deleted) return;

            if (!m_Library.CanUse(m_Player))
                return;         

            bool closeGump = true;

            //-----

            switch (info.ButtonID)
            {
                case 1:
                    //Add All From Backpack
                    m_Library.AddAllMapsInPack(m_Player);

                    closeGump = false;
                break;

                case 2:
                    //Previous Access Level
                    if (m_Library.IsLockedDown && m_Player.AccessLevel == AccessLevel.Player)                    
                        m_Player.SendMessage("You may not change the access level of this item while it is currently locked down.");                    

                    else
                    {
                        switch (m_Library.LockedDownAccessLevel)
                        {
                            case TreasureMapLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.Anyone; break;
                            case TreasureMapLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.Owner; break;
                            case TreasureMapLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case TreasureMapLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.Friend; break;
                        }
                    }

                    closeGump = false;
                break;

                case 3:
                    //Next Access Level
                    if (m_Library.IsLockedDown && m_Player.AccessLevel == AccessLevel.Player)    
                        m_Player.SendMessage("You may not change the access level of this item while it is currently locked down.");

                    else
                    {
                        switch (m_Library.LockedDownAccessLevel)
                        {
                            case TreasureMapLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case TreasureMapLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.Friend; break;
                            case TreasureMapLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.Anyone; break;
                            case TreasureMapLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = TreasureMapLibrary.LockedDownAccessLevelType.Owner; break;
                        }
                    }

                    closeGump = false;
                break;

                case 4:
                    //Remove All Possible on Selection
                    m_Library.RemoveAllOnSelection = !m_Library.RemoveAllOnSelection;

                    closeGump = false;
                break;
            }

            //Eject Items
            if (info.ButtonID >= 10)
            {
                bool decoded = false;

                if (info.ButtonID >= 20)
                    decoded = true;

                int mapLevel = info.ButtonID % 10;

                TreasureMapLibraryEntry entry = m_Library.GetLibraryEntry(mapLevel, decoded);

                if (entry != null)
                {
                    bool removeAll = m_Library.RemoveAllOnSelection;

                    m_Library.EjectMap(m_Player, mapLevel, decoded, removeAll);
                }

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(TreasureMapLibraryGump));
                m_Player.SendGump(new TreasureMapLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}