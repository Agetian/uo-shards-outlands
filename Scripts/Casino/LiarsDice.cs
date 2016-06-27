﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Accounting;
using System.Linq;
using Server.Custom;
using System.Text;

namespace Server
{
    #region Liars Dice Tester

    public class LiarsDiceTester : Item
    {
        [Constructable]
        public LiarsDiceTester(): base(4007)
        {
            Name = "Liars Dice Tester";

            Hue = 2587;
        }

        public LiarsDiceTester(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;
            
            if (player == null)
                return;

            if (LiarsDicePersistance.m_Instances.Count == 0)
            {
                LiarsDiceInstance newGame = new LiarsDiceInstance();

                newGame.AddPlayer(player);
            }

            else
            {
                LiarsDiceInstance currentGame = LiarsDicePersistance.m_Instances[0];

                if (!currentGame.m_Started)
                {
                    currentGame.AddPlayer(player);

                    if (currentGame.m_Players.Count == 3)
                        currentGame.StartGame();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version         
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

    #endregion

    #region Persistance

    public static class LiarsDicePersistance
    {
        public static LiarsDicePersistanceItem PersistanceItem;

        public static List<LiarsDiceInstance> m_Instances = new List<LiarsDiceInstance>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new LiarsDicePersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new LiarsDicePersistanceItem();
            });
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

    public class LiarsDicePersistanceItem : Item
    {
        public override string DefaultName { get { return "LiarsDicePersistance"; } }

        public LiarsDicePersistanceItem(): base(0x0)
        {
            Movable = false;                        
        }

        public LiarsDicePersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            LiarsDicePersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            LiarsDicePersistance.PersistanceItem = this;
            LiarsDicePersistance.Deserialize(reader);
        }
    }

    #endregion

    public class LiarsDiceInstance : Item
    {
        public enum CurrentActionType
        {
            LobbyWaitingForPlayers,
            StartCountdown,
            RoundStart,            
            PlayerBidding,
            LiarCallout,
            CalloutResolution,
            GameResolution
        }

        public static int MaxPlayers = 5;
        public static int MaxTurnsSkippable = 3;

        public static int MaximumEffectiveBidValue = 156;

        public string m_CurrentText = "";
        public CurrentActionType m_CurrentAction = CurrentActionType.LobbyWaitingForPlayers;
        public TimeSpan m_ActionTimeRemaining = TimeSpan.FromSeconds(30);

        public int m_LastBidCount = 1;
        public int m_LastBidValue = 0;

        public LiarsDicePlayer m_CurrentPlayer;
        public int m_CurrentPlayerIndex = 0;

        public LiarsDicePlayer m_LiarCalloutPlayer;
        public int m_LiarCalloutPlayerIndex = 0;

        public LiarsDicePlayer m_LiarCalloutPlayerTarget;
        public int m_LiarCalloutPlayerTargetIndex = 0;

        public bool m_Started = false;
        public bool m_Completed = false;        

        public DateTime m_StartTime;       
        
        public int m_Pot = 25000;

        public List<LiarsDicePlayer> m_Players = new List<LiarsDicePlayer>();

        public Timer m_Timer;

        [Constructable]
        public LiarsDiceInstance(): base(0x0)
        {
            Movable = false;

            //-----

            LiarsDicePersistance.m_Instances.Add(this);
        }

        public LiarsDiceInstance(Serial serial): base(serial)
        {
        }

        public bool AddPlayer(PlayerMobile player)
        {
            if (player == null) 
                return false;

            if (m_Players.Count >= MaxPlayers)
                return false;

            LiarsDicePlayer newPlayer = new LiarsDicePlayer(player);

            m_Players.Add(newPlayer);

            return true;
        }

        public void RemovePlayer(PlayerMobile player)
        {
            LiarsDicePlayer playerMatch = null;

            foreach (LiarsDicePlayer dicePlayer in m_Players)
            {
                if (dicePlayer.m_Player == player)
                {
                    playerMatch = dicePlayer;
                    break;
                }
            }

            if (playerMatch != null)
            {
                if (m_Players.Contains(playerMatch))
                    m_Players.Remove(playerMatch);
            }
        }

        public void UpdateGumps()
        {
            foreach (LiarsDicePlayer dicePlayer in m_Players)
            {
                if (dicePlayer == null) continue;
                if (!dicePlayer.m_Spectating) continue;
                if (dicePlayer.m_Player == null) continue;

                dicePlayer.m_Player.CloseGump(typeof(LiarsDiceGump));
                dicePlayer.m_Player.SendGump(new LiarsDiceGump(dicePlayer, this));
            }
        }

        public int GetDiceItemId(int value)
        {
            switch(value)
            {
                case 1: return 11280; break;
                case 2: return 11281; break;
                case 3: return 11282; break;
                case 4: return 11283; break;
                case 5: return 11284; break;
                case 6: return 11285; break;
            }

            return 11280;
        }

        public int GetPlayerIndex(LiarsDicePlayer player)
        {
            return m_Players.IndexOf(player);
        }

        public void ShufflePlayerOrder()
        {
            int playerCount = m_Players.Count;

            List<LiarsDicePlayer> tempPlayerList = new List<LiarsDicePlayer>();

            while (m_Players.Count > 0)
            {
                int playerIndex = Utility.RandomMinMax(0, m_Players.Count - 1);

                tempPlayerList.Add(m_Players[playerIndex]);
                m_Players.RemoveAt(playerIndex);
            }

            for (int a = 0; a < playerCount; a++)
            {
                m_Players.Add(tempPlayerList[a]);
            }
        }

        public bool GetStartingPlayer()
        {
            bool foundMatch = false;

            for (int a = 0; a < m_Players.Count; a++)
            {
                LiarsDicePlayer dicePlayer = m_Players[a];

                if (dicePlayer == null) continue;
                if (!dicePlayer.m_Playing) continue;

                foundMatch = true;
                m_CurrentPlayer = dicePlayer;
                m_CurrentPlayerIndex = a;

                break;
            }

            if (foundMatch)
                return true;

            else
            {
                EndGame();

                return false;
            }
        }

        public void DistributeDice()
        {
            foreach (LiarsDicePlayer player in m_Players)
            {
                if (player == null) continue;
                if (!player.m_Playing) continue;

                player.m_NewBidCount = 1;
                player.m_NewBidValue = 1;

                player.m_BidCount = 0;
                player.m_BidValue = 0;

                player.m_TurnsSkipped = 0;

                player.m_Dice.Clear();

                for (int a = 0; a < player.m_DiceRemaining; a++)
                {
                    int diceValue = Utility.RandomMinMax(1, 6);
                    player.m_Dice.Add(diceValue);
                }
            }
        }

        public bool NextPlayer()
        {            
            int m_NewIndex = m_CurrentPlayerIndex;

            bool foundNewPlayer = false;

            for (int a = 0; a < m_Players.Count; a++)
            {
                m_NewIndex++;

                if (m_NewIndex >= m_Players.Count)
                    m_NewIndex = 0;

                LiarsDicePlayer player = m_Players[m_NewIndex];

                if (player == null) continue;
                if (player.m_Playing)
                {
                    m_CurrentPlayer = player;
                    m_CurrentPlayerIndex = m_NewIndex;

                    player.m_NewBidCount = player.m_BidCount;
                    player.m_NewBidValue = player.m_BidValue;

                    if (player.m_NewBidCount == 0)
                        player.m_NewBidCount = 1;

                    if (player.m_NewBidValue == 0)
                        player.m_NewBidValue = 1;

                    foundNewPlayer = true;

                    break;
                }
            }

            if (foundNewPlayer)
                return true;
            else
            {
                EndGame();

                return false;
            }            
        }

        public bool CanCallOutPlayer(LiarsDicePlayer playerFrom, LiarsDicePlayer playerTarget)
        {
            if (playerFrom == null) return false;
            if (playerTarget == null) return false;
            if (!playerFrom.m_Playing) return false;
            if (!playerTarget.m_Playing) return false;

            if (playerFrom == playerTarget) return false;
            if (playerTarget.m_BidCount == 0) return false;
            if (playerTarget.m_BidValue == 0) return false;

            if (m_CurrentAction != CurrentActionType.PlayerBidding)
                return false;

            return true;
        }

        public void StartGame()
        {
            m_Started = true;

            m_StartTime = DateTime.UtcNow;

            m_CurrentText = "Game beginning in 5 seconds.";
            m_CurrentAction = CurrentActionType.StartCountdown;
            m_ActionTimeRemaining = TimeSpan.FromSeconds(5);

            UpdateGumps();

            m_Timer = new LiarsDiceTimer(this);
            m_Timer.Start();
        }

        public void RoundStart()
        {
            ShufflePlayerOrder();
            DistributeDice();

            if (!GetStartingPlayer())
                return;

            m_LastBidCount = 1;
            m_LastBidValue = 0;

            m_LiarCalloutPlayer = null;
            m_LiarCalloutPlayerTarget = null;

            m_CurrentText = "New round beginning in 5 seconds.";
            m_CurrentAction = CurrentActionType.RoundStart;
            m_ActionTimeRemaining = TimeSpan.FromSeconds(5);

            UpdateGumps();
        }

        public void BeginBidding()
        {
            m_CurrentText = m_CurrentPlayer.m_Player.RawName + " begins the bidding.";
            m_CurrentAction = CurrentActionType.PlayerBidding;
            m_ActionTimeRemaining = TimeSpan.FromSeconds(20);

            UpdateGumps();
        }

        public void PlayerPasses()
        {
            m_CurrentPlayer.m_NewBidCount = m_LastBidCount;
            m_CurrentPlayer.m_NewBidValue = m_LastBidValue;

            double randomAction = Utility.RandomDouble();

            if (randomAction <= .25)
                m_CurrentPlayer.m_NewBidCount++;

            else if (randomAction <= .50)
            {
                m_CurrentPlayer.m_NewBidValue += Utility.RandomMinMax(1, 5);

                if (m_CurrentPlayer.m_NewBidValue > 6)
                {
                    m_CurrentPlayer.m_NewBidCount++;
                    m_CurrentPlayer.m_NewBidValue = m_CurrentPlayer.m_NewBidValue - 6;
                }
            }

            else
            {
                if (m_CurrentPlayer.m_BidValue > m_CurrentPlayer.m_NewBidValue)
                    m_CurrentPlayer.m_NewBidValue = m_CurrentPlayer.m_BidValue;

                else
                {
                    m_CurrentPlayer.m_NewBidCount++;
                    m_CurrentPlayer.m_NewBidValue = m_CurrentPlayer.m_BidValue;
                }
            }

            m_CurrentPlayer.m_BidCount = m_CurrentPlayer.m_NewBidCount;
            m_CurrentPlayer.m_BidValue = m_CurrentPlayer.m_NewBidValue;

            m_LastBidCount = m_CurrentPlayer.m_BidCount;
            m_LastBidValue = m_CurrentPlayer.m_BidValue;

            m_CurrentPlayer.m_TurnsSkipped++;

            int skipsRemain = MaxTurnsSkippable - m_CurrentPlayer.m_TurnsSkipped;

            if (skipsRemain > 0)
            {
                m_CurrentText = m_CurrentPlayer.m_Player.RawName + " passes and bids " + m_CurrentPlayer.m_BidCount.ToString() + " " + m_CurrentPlayer.m_BidValue.ToString() + "'s.  (" + skipsRemain.ToString() + " more passes until forfeit)";
            }

            else
            {
                m_CurrentPlayer.m_Playing = false;
                m_CurrentText = m_CurrentPlayer.m_Player.RawName + " passes and bids " + m_CurrentPlayer.m_BidCount.ToString() + " " + m_CurrentPlayer.m_BidValue.ToString() + "'s.  (forfeits game)";
            }

            if (NextPlayer())
            {
                m_CurrentAction = CurrentActionType.PlayerBidding;
                m_ActionTimeRemaining = TimeSpan.FromSeconds(20);

                UpdateGumps();
            }
        }

        public void PlaceBid(LiarsDicePlayer player, int bidCount, int bidValue)
        {
            if (player == null) 
                return;

            player.m_BidCount = bidCount;
            player.m_BidValue = bidValue;

            m_LastBidCount = player.m_BidCount;
            m_LastBidValue = player.m_BidValue;

            m_CurrentText = m_CurrentPlayer.m_Player.RawName + " bids " + m_CurrentPlayer.m_BidCount.ToString() + " " + m_CurrentPlayer.m_BidValue.ToString() + "'s.";

            if (NextPlayer())
            {
                m_CurrentAction = CurrentActionType.PlayerBidding;
                m_ActionTimeRemaining = TimeSpan.FromSeconds(20);

                UpdateGumps();
            }
        }        

        public void CallOutPlayer(LiarsDicePlayer playerFrom, LiarsDicePlayer playerTarget)
        {
            if (playerFrom == null) return;
            if (playerTarget == null) return;
            if (!playerFrom.m_Playing) return;
            if (!playerTarget.m_Playing) return;

            m_LiarCalloutPlayer = playerFrom;
            m_LiarCalloutPlayerIndex = GetPlayerIndex(playerFrom);

            m_LiarCalloutPlayerTarget = playerTarget;
            m_LiarCalloutPlayerTargetIndex = GetPlayerIndex(playerTarget);

            m_CurrentText = playerFrom.m_Player.RawName + " calls " + playerTarget.m_Player.RawName + " a liar!";
            m_CurrentAction = CurrentActionType.LiarCallout;
            m_ActionTimeRemaining = TimeSpan.FromSeconds(5);

            UpdateGumps();
        }

        public void ResolveCallOut()
        {
        }

        public void PlayerLeave(LiarsDicePlayer playerFrom)
        {
            if (playerFrom == null)
                return; 
           
            //TEST: ADD DOUBLE CLICK PREVENTION FOR LEAVING

            playerFrom.m_Playing = false;
            playerFrom.m_Spectating = false;

            if (playerFrom.m_Player == null)
                m_CurrentText = "A player has left the game.";
            else
                m_CurrentText = playerFrom.m_Player.RawName + " has left the game.";

            switch (m_CurrentAction)
            {
                case CurrentActionType.PlayerBidding:
                    if (m_CurrentPlayer == playerFrom)
                    {
                        if (NextPlayer())
                        {
                            m_CurrentAction = CurrentActionType.PlayerBidding;
                            m_ActionTimeRemaining = TimeSpan.FromSeconds(20);

                            UpdateGumps();
                        }

                        else
                            return;
                    }
                break;

                case CurrentActionType.RoundStart:
                    if (m_CurrentPlayer == playerFrom)
                    {
                        if (NextPlayer())
                        {
                            m_CurrentAction = CurrentActionType.RoundStart;
                            m_ActionTimeRemaining = TimeSpan.FromSeconds(5);

                            UpdateGumps();
                        }

                        else
                            return;
                    }
                break;
            }            
        }

        public void EndGame()
        {
        }

        public class LiarsDiceTimer : Timer
        {
            public LiarsDiceInstance m_Instance;

            public LiarsDiceTimer(LiarsDiceInstance instance): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Instance = instance;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                #region Inactive Conditions

                if (m_Instance == null)
                {
                    Stop();
                    return;
                }

                else if (m_Instance.Deleted)
                {
                    Stop();
                    return;
                }

                if (!m_Instance.m_Started)
                {
                    Stop();
                    return;
                }

                else if (m_Instance.m_Completed)
                {
                    Stop();
                    return;
                }

                #endregion

                m_Instance.m_ActionTimeRemaining = m_Instance.m_ActionTimeRemaining.Subtract(TimeSpan.FromSeconds(1));

                switch (m_Instance.m_CurrentAction)
                {
                    case CurrentActionType.LobbyWaitingForPlayers:
                    break;

                    case CurrentActionType.StartCountdown:
                        if (m_Instance.m_ActionTimeRemaining <= TimeSpan.FromSeconds(0))                        
                            m_Instance.RoundStart();                        
                    break;

                    case CurrentActionType.RoundStart:
                        if (m_Instance.m_ActionTimeRemaining <= TimeSpan.FromSeconds(0))
                            m_Instance.BeginBidding();    
                    break;

                    case CurrentActionType.PlayerBidding:
                        if (m_Instance.m_ActionTimeRemaining <= TimeSpan.FromSeconds(0))
                            m_Instance.PlayerPasses();  
                    break;

                    case CurrentActionType.LiarCallout:
                        if (m_Instance.m_ActionTimeRemaining <= TimeSpan.FromSeconds(0))
                            m_Instance.ResolveCallOut();  
                    break;

                    case CurrentActionType.CalloutResolution:
                        if (m_Instance.m_ActionTimeRemaining <= TimeSpan.FromSeconds(0))
                            m_Instance.RoundStart();  
                    break;

                    case CurrentActionType.GameResolution:
                    break;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_CurrentText);
            writer.Write((int)m_CurrentAction);
            writer.Write(m_ActionTimeRemaining);
            writer.Write(m_LastBidCount);
            writer.Write(m_LastBidValue);
            writer.Write(m_CurrentPlayerIndex);
            writer.Write(m_LiarCalloutPlayerIndex);
            writer.Write(m_LiarCalloutPlayerTargetIndex);
            writer.Write(m_Started);
            writer.Write(m_Completed);
            writer.Write(m_StartTime);
            writer.Write(m_Pot);

            writer.Write(m_Players.Count);
            for (int a = 0; a < m_Players.Count; a++)
            {        
                LiarsDicePlayer player = m_Players[a];

                writer.Write(player.m_Player);
                writer.Write(player.m_Playing);
                writer.Write(player.m_Spectating);
                writer.Write(player.m_TurnsSkipped);
                writer.Write(player.m_BidCount);
                writer.Write(player.m_BidValue);
                writer.Write(player.m_NewBidCount);
                writer.Write(player.m_NewBidValue);
                writer.Write(player.m_DiceRemaining);

                writer.Write(player.m_Dice.Count);
                for (int b = 0; b < player.m_Dice.Count; b++)
                {
                    writer.Write(player.m_Dice[b]);
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
                m_CurrentText = reader.ReadString();
                m_CurrentAction = (CurrentActionType)reader.ReadInt();
                m_ActionTimeRemaining = reader.ReadTimeSpan();
                m_LastBidCount = reader.ReadInt();
                m_LastBidValue = reader.ReadInt();
                m_CurrentPlayerIndex = reader.ReadInt();
                m_LiarCalloutPlayerIndex = reader.ReadInt();
                m_LiarCalloutPlayerTargetIndex = reader.ReadInt();
                m_Started = reader.ReadBool();
                m_Completed = reader.ReadBool();
                m_StartTime = reader.ReadDateTime();
                m_Pot = reader.ReadInt();

                int playerCount = reader.ReadInt();
                for (int a = 0; a < playerCount; a++)
                {
                    LiarsDicePlayer liarsDicePlayer = new LiarsDicePlayer((PlayerMobile)reader.ReadMobile());
                    liarsDicePlayer.m_Playing = reader.ReadBool();
                    liarsDicePlayer.m_Spectating = reader.ReadBool();
                    liarsDicePlayer.m_TurnsSkipped = reader.ReadInt();
                    liarsDicePlayer.m_BidCount = reader.ReadInt();
                    liarsDicePlayer.m_BidValue = reader.ReadInt();
                    liarsDicePlayer.m_NewBidCount = reader.ReadInt();
                    liarsDicePlayer.m_NewBidValue = reader.ReadInt();
                    liarsDicePlayer.m_DiceRemaining = reader.ReadInt();

                    int diceCount = reader.ReadInt();

                    for (int b = 0; b < diceCount; b++)
                    {
                        liarsDicePlayer.m_Dice.Add(reader.ReadInt());
                    }

                    m_Players.Add(liarsDicePlayer);
                }
            }

            //-----

            LiarsDicePersistance.m_Instances.Add(this);

            if (m_Players.Count > 0)
            {
                if (m_CurrentPlayerIndex < m_Players.Count)
                    m_CurrentPlayer = m_Players[m_CurrentPlayerIndex];

                if (m_LiarCalloutPlayerIndex < m_Players.Count)
                    m_LiarCalloutPlayer = m_Players[m_LiarCalloutPlayerIndex];

                if (m_LiarCalloutPlayerTargetIndex < m_Players.Count)
                    m_LiarCalloutPlayerTarget = m_Players[m_LiarCalloutPlayerTargetIndex];
            }

            if (m_Started && !m_Completed)
            {
                m_Timer = new LiarsDiceTimer(this);
                m_Timer.Start();
            }
        }
    }

    public class LiarsDiceGump : Gump
    {
        public PlayerMobile m_Player;

        public LiarsDicePlayer m_DicePlayer;
        public LiarsDiceInstance m_Instance;

        public List<LiarsDicePlayer> m_OrderedPlayerList = new List<LiarsDicePlayer>();

        public LiarsDiceGump(LiarsDicePlayer dicePlayer, LiarsDiceInstance instance): base(0, 0)
        {
            m_DicePlayer = dicePlayer;
            m_Instance = instance;

            if (m_DicePlayer == null) return;
            if (m_DicePlayer.m_Player == null) return;
            if (m_Instance == null) return;
            if (m_Instance.Deleted) return;

            m_Player = m_DicePlayer.m_Player;            

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;

            #region Images

            AddPage(0);

            AddBackground(5, 9, 636, 375, 9270);
            AddImage(17, 20, 3604, 2052);
            AddImage(145, 20, 3604, 2052);
            AddImage(273, 20, 3604, 2052);
            AddImage(401, 20, 3604, 2052);
            AddImage(503, 20, 3604, 2052);
            AddImage(17, 145, 3604, 2052);
            AddImage(145, 145, 3604, 2052);
            AddImage(273, 145, 3604, 2052);
            AddImage(401, 145, 3604, 2052);
            AddImage(503, 145, 3604, 2052);
            AddImage(17, 247, 3604, 2052);
            AddImage(145, 247, 3604, 2052);
            AddImage(273, 247, 3604, 2052);
            AddImage(401, 247, 3604, 2052);
            AddImage(503, 247, 3604, 2052);
            AddImage(21, 139, 62);
            AddImage(84, 71, 9801);
            AddImage(229, 8, 2446, 2401);           
            AddImage(292, 27, 9809, 0);
            AddItem(288, 50, 3823);            
            AddItem(294, 40, 3823);
            AddItem(299, 51, 3823);
            AddLabel(284, 8, WhiteTextHue, "Liar's Dice");

            #endregion

            //-----            

            AddButton(16, 17, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(12, 5, 149, "Guide");

            AddLabel(351, 44, 2550, Utility.CreateCurrencyString(m_Instance.m_Pot));
            AddLabel(351, 64, WhiteTextHue, "Gold Pot");

            AddLabel(524, 28, 1256, "Leave Game");
            AddButton(601, 24, 2472, 2473, 2, GumpButtonType.Reply, 0);

            #region Player

            if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_Instance.m_CurrentPlayer == m_DicePlayer && dicePlayer.m_Playing)
            {
                AddImage(62, 21, 30077, 2599); //Selection
                AddLabel(Utility.CenteredTextOffset(110, m_Player.RawName), 46, 63, m_Player.RawName);

                int currentBidTotal = ((m_DicePlayer.m_NewBidCount) * 6) + (m_DicePlayer.m_NewBidValue);
                int effectiveLastBidTotal = ((m_Instance.m_LastBidCount) * 6) + (m_Instance.m_LastBidValue);

                if (m_DicePlayer.m_NewBidCount < 98)
                    AddButton(38, 71, 9900, 9900, 4, GumpButtonType.Reply, 0); //Increase Bid Count

                if (m_DicePlayer.m_NewBidCount > 1)
                    AddButton(38, 95, 9906, 9906, 5, GumpButtonType.Reply, 0); //Decrease Bid Count

                AddLabel(69, 82, WhiteTextHue, m_DicePlayer.m_NewBidCount.ToString()); //Count
                AddImage(97, 83, m_Instance.GetDiceItemId(m_DicePlayer.m_NewBidValue)); //Dice
                
                AddButton(138, 71, 9900, 9900, 6, GumpButtonType.Reply, 0); //Increase Bid Value
                AddButton(139, 95, 9906, 9906, 7, GumpButtonType.Reply, 0); //Decrease Bid Value

                if (currentBidTotal > effectiveLastBidTotal)
                {
                    AddLabel(97, 132, WhiteTextHue, "Bid");
                    AddButton(93, 157, 9721, 9725, 8, GumpButtonType.Reply, 0);
                }

                for (int a = 0; a < dicePlayer.m_Dice.Count; a++)
                {
                    int diceValue = dicePlayer.m_Dice[a];

                    int diceItemId = m_Instance.GetDiceItemId(diceValue);
                    int diceHue = 0;

                    if (m_DicePlayer.m_NewBidValue == diceValue)
                        diceHue = 63;

                    switch (a)
                    {
                        case 0: AddImage(69, 216, diceItemId, diceHue); break;
                        case 1: AddImage(99, 216, diceItemId, diceHue); break;
                        case 2: AddImage(129, 216, diceItemId, diceHue); break;
                        case 3: AddImage(84, 246, diceItemId, diceHue); break;
                        case 4: AddImage(114, 246, diceItemId, diceHue); break;
                    }
                }
            }

            else if (dicePlayer.m_Playing)
            {
                AddLabel(Utility.CenteredTextOffset(110, m_Player.RawName), 46, WhiteTextHue, m_Player.RawName);
                
                if (m_DicePlayer.m_BidCount > 0)
                    AddLabel(69, 82, WhiteTextHue, m_DicePlayer.m_BidCount.ToString());
                
                if (m_DicePlayer.m_BidValue > 0)
                    AddImage(97, 83, m_Instance.GetDiceItemId(m_DicePlayer.m_BidValue)); //Dice

                for (int a = 0; a < dicePlayer.m_Dice.Count; a++)
                {
                    int diceValue = dicePlayer.m_Dice[a];

                    int diceItemId = m_Instance.GetDiceItemId(diceValue);
                    int diceHue = 0;

                    if (m_DicePlayer.m_BidValue == diceValue)
                        diceHue = 63;

                    switch (a)
                    {
                        case 0: AddImage(69, 216, diceItemId, diceHue); break;
                        case 1: AddImage(99, 216, diceItemId, diceHue); break;
                        case 2: AddImage(129, 216, diceItemId, diceHue); break;
                        case 3: AddImage(84, 246, diceItemId, diceHue); break;
                        case 4: AddImage(114, 246, diceItemId, diceHue); break;
                    }
                }
            }         
   
            else            
                AddLabel(Utility.CenteredTextOffset(110, m_Player.RawName), 46, 2401, m_Player.RawName);

            #endregion

            #region Other Players

            m_OrderedPlayerList = new List<LiarsDicePlayer>();

            int playerIndex = m_Instance.m_Players.IndexOf(m_DicePlayer);

            for (int a = 0; a < m_Instance.m_Players.Count; a++)
            {
                playerIndex++;

                if (playerIndex >= m_Instance.m_Players.Count)
                    playerIndex = 0;

                LiarsDicePlayer otherPlayer = m_Instance.m_Players[playerIndex];

                if (otherPlayer != m_DicePlayer)
                    m_OrderedPlayerList.Add(otherPlayer);
            }

            int startX = 210;
            int startY = 95;
            int playerSpacing = 105;

            for (int a = 0; a < m_OrderedPlayerList.Count; a++)
            {
                LiarsDicePlayer otherPlayer = m_OrderedPlayerList[a];

                switch(m_Instance.m_CurrentAction)
                {
                    case LiarsDiceInstance.CurrentActionType.RoundStart:
                    break;

                    case LiarsDiceInstance.CurrentActionType.PlayerBidding:
                    break;

                    case LiarsDiceInstance.CurrentActionType.LiarCallout:
                    break;

                    case LiarsDiceInstance.CurrentActionType.GameResolution:
                    break;
                }

                switch (m_Instance.m_CurrentAction)
                {
                    case LiarsDiceInstance.CurrentActionType.PlayerBidding:
                        if (m_Instance.m_CurrentPlayer == otherPlayer)
                        {
                            AddImage(0 + startX, 0 + startY, 30077, 2587); //Selection
                            AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, 149, otherPlayer.m_Player.RawName);
                        }

                        else
                            AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, WhiteTextHue, otherPlayer.m_Player.RawName);
                    break;

                    case LiarsDiceInstance.CurrentActionType.LiarCallout:
                        if (m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                        {
                            AddImage(0 + startX, 0 + startY, 30077, 1256); //Selection
                            AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, 1256, otherPlayer.m_Player.RawName);
                        }

                        else if (m_Instance.m_LiarCalloutPlayer == otherPlayer)
                            AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, 63, otherPlayer.m_Player.RawName);
                        
                    break;

                    default:
                        if (otherPlayer.m_Playing)
                            AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, WhiteTextHue, otherPlayer.m_Player.RawName);
                        else
                            AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, 2401, otherPlayer.m_Player.RawName);
                    break;
                }

                AddImage(20 + startX, 49 + startY, 9801); //Cup
                
                int diceItemId = m_Instance.GetDiceItemId(otherPlayer.m_BidValue);
                int diceHue = 0;

                if (otherPlayer.m_BidCount > 0)
                    AddLabel(5 + startX, 61 + startY, WhiteTextHue, otherPlayer.m_BidCount.ToString());

                if (otherPlayer.m_BidValue > 0)
                    AddImage(33 + startX, 61 + startY, diceItemId, diceHue); //Dice

                for (int b = 0; b < otherPlayer.m_Dice.Count; b++)
                {
                    switch (b)
                    {
                        case 0:
                            AddBackground(2 + startX, 123 + startY, 20, 20, 3000);
                            AddLabel(8 + startX, 123 + startY, WhiteTextHue, "?");
                        break;

                        case 1:
                            AddBackground(32 + startX, 123 + startY, 20, 20, 3000);
                            AddLabel(38 + startX, 123 + startY, WhiteTextHue, "?");
                        break;

                        case 2:
                            AddBackground(62 + startX, 123 + startY, 20, 20, 3000);
                            AddLabel(68 + startX, 123 + startY, WhiteTextHue, "?");
                        break;

                        case 3:
                            AddBackground(17 + startX, 153 + startY, 20, 20, 3000);
                            AddLabel(22 + startX, 153 + startY, WhiteTextHue, "?");
                        break;

                        case 4:
                            AddBackground(47 + startX, 153 + startY, 20, 20, 3000);
                            AddLabel(53 + startX, 153 + startY, WhiteTextHue, "?");
                        break;
                    }
                }

                if (m_Instance.CanCallOutPlayer(dicePlayer, otherPlayer))
                {
                    AddButton(17 + startX, 180 + startY, 4500, 4500, 10 + a, GumpButtonType.Reply, 0);
                    AddLabel(13 + startX, 233 + startY, WhiteTextHue, "Call Liar!");
                }

                else if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.LiarCallout)
                {
                    if (m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                    {                        
                        AddImage(17 + startX, 180 + startY, 4500, 1256);
                        AddLabel(10 + startX, 233 + startY, WhiteTextHue, "Called Liar");
                    }
                }

                startX += playerSpacing;
            }

            #endregion

            AddButton(34, 354, 2117, 2118, 3, GumpButtonType.Reply, 0);
            AddLabel(56, 351, 2550, "Send Chat Message");

            AddLabel(Utility.CenteredTextOffset(415, m_Instance.m_CurrentText), 351, 2603, m_Instance.m_CurrentText);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_DicePlayer == null) return;
            if (m_Instance == null) return;
            if (m_Instance.Deleted) return;

            bool closeGump = true;

            int maximumNewBidValue = 156;

            int currentBidTotal = ((m_DicePlayer.m_NewBidCount) * 6) + (m_DicePlayer.m_NewBidValue);
            int effectiveLastBidTotal = ((m_Instance.m_LastBidCount) * 6) + (m_Instance.m_LastBidValue);

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Leave
                case 2:
                    if (m_DicePlayer.m_Playing)
                    {
                        if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding || m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.RoundStart)
                        {
                            m_Instance.PlayerLeave(m_DicePlayer);
                            return;
                        }

                        else
                            m_Player.SendMessage("You must wait until the current phase is resolved before you may leave this game.");
                    }

                    closeGump = false;
                break;

                //Send Chat Message
                case 3:
                    closeGump = false;
                break;

                //Increase Bid Count
                case 4:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer == m_Instance.m_CurrentPlayer)
                    {
                        if (m_DicePlayer.m_NewBidValue < 98)
                        {
                            m_DicePlayer.m_NewBidCount++;
                            m_Player.SendSound(0x3E6);
                        }

                        /*
                        int effectiveNewBidValue = ((m_DicePlayer.m_NewBidCount + 1) * 6) + (m_DicePlayer.m_NewBidValue);

                        if (effectiveNewBidValue <= maximumNewBidValue)
                        {
                            m_DicePlayer.m_NewBidCount++;
                            m_Player.SendSound(0x3E6);
                        }
                        */
                    }

                    closeGump = false;
                break;

                //Decrease Bid Count
                case 5:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer == m_Instance.m_CurrentPlayer)
                    {
                        if (m_DicePlayer.m_NewBidCount > 1)
                        {
                            m_DicePlayer.m_NewBidCount--;
                            m_Player.SendSound(0x3E6);
                        }

                        /*
                        int effectiveNewBidValue = ((m_DicePlayer.m_NewBidCount - 1) * 6) + (m_DicePlayer.m_NewBidValue);

                        if (effectiveNewBidValue >= effectiveLastBidTotal)
                        {
                            m_DicePlayer.m_NewBidCount--;
                            m_Player.SendSound(0x3E6);
                        }
                        */
                    }

                    closeGump = false;
                break;

                //Increase Bid Value
                case 6:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer == m_Instance.m_CurrentPlayer)
                    {
                        m_DicePlayer.m_NewBidValue++;
                        m_Player.SendSound(0x3E6);

                        if (m_DicePlayer.m_NewBidValue > 6)
                            m_DicePlayer.m_NewBidValue = 1;

                        /*
                        int effectiveNewBidValue = ((m_DicePlayer.m_NewBidCount) * 6) + (m_DicePlayer.m_NewBidValue + 1);

                        if (effectiveNewBidValue <= maximumNewBidValue)
                        {
                            m_DicePlayer.m_NewBidValue++;

                            if (m_DicePlayer.m_NewBidValue == 7)
                            {
                                m_DicePlayer.m_NewBidCount++;
                                m_DicePlayer.m_NewBidValue = 1;
                            }

                            m_Player.SendSound(0x3E6);
                        }
                        */
                    }

                    closeGump = false;
                break;

                //Decrease Bid Value
                case 7:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer == m_Instance.m_CurrentPlayer)
                    {
                        m_DicePlayer.m_NewBidValue--;
                        m_Player.SendSound(0x3E6);

                        if (m_DicePlayer.m_NewBidValue < 1)
                            m_DicePlayer.m_NewBidValue = 6;

                        /*
                        int effectiveNewBidValue = ((m_DicePlayer.m_NewBidCount) * 6) + (m_DicePlayer.m_NewBidValue - 1);

                        if (effectiveNewBidValue >= effectiveLastBidTotal)
                        {
                            m_DicePlayer.m_NewBidValue--;

                            if (m_DicePlayer.m_NewBidValue == 0)
                            {
                                m_DicePlayer.m_NewBidCount--;
                                m_DicePlayer.m_NewBidValue = 6;
                            }

                            m_Player.SendSound(0x3E6);
                        }
                        */
                    }

                    closeGump = false;
                break;

                //Place Bid
                case 8:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer ==  m_Instance.m_CurrentPlayer)
                    {
                        if (m_DicePlayer.m_NewBidCount > 99)
                            m_Player.SendMessage("You have exceeded the maximum bid allowed.");

                        else if (currentBidTotal <= effectiveLastBidTotal)
                            m_Player.SendMessage("Your bid must be of a higher dice count or higher dice face value than the last bidder.");

                        else           
                        {
                            m_Instance.PlaceBid(m_DicePlayer, m_DicePlayer.m_NewBidCount, m_DicePlayer.m_NewBidValue);

                            return;
                        }
                    }

                    closeGump = false;
                break;
            }

            //Callout Liar
            if (info.ButtonID >= 10)
            {
                int playerIndex = info.ButtonID - 10;

                if (playerIndex < m_OrderedPlayerList.Count)
                {
                    LiarsDicePlayer playerTarget = m_OrderedPlayerList[playerIndex];

                    if (playerTarget != null)
                    {
                        if (m_Instance.CanCallOutPlayer(m_DicePlayer, playerTarget)) 
                        {
                            m_Instance.CallOutPlayer(m_DicePlayer, playerTarget); 

                            return;
                        }
                    }
                }

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(LiarsDiceGump));
                m_Player.SendGump(new LiarsDiceGump(m_DicePlayer, m_Instance));
            }

            else
                m_Player.SendSound(0x058);
        }
    }

    public class LiarsDicePlayer
    {
        public PlayerMobile m_Player;
        public bool m_Playing = true;
        public bool m_Spectating = true;
        public int m_TurnsSkipped = 0;

        public int m_BidCount = 0;
        public int m_BidValue = 0;

        public int m_NewBidCount = 1;
        public int m_NewBidValue = 1;

        public int m_DiceRemaining = 5;

        public List<int> m_Dice = new List<int>();

        public LiarsDicePlayer(PlayerMobile player)
        {
            m_Player = player;
        }
    }
}
