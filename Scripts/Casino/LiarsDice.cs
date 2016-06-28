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
            PostCalloutResolution,
            GameResolution
        }

        #region Sounds

        public static int GameStartSound = 0x4B7;
        public static int CalloutBeginSound = 0x64A;
        public static int CalloutSuccessSound = 0x5B1;
        public static int CalloutFailSound = 0x5D0;

        public static int PlayerAlertSound = 0x5B6;
        public static int DiceRollSound = 0x033;
        public static int BidChangeSound = 0x4D2;
        public static int BidConfirmSound = 0x4D3;

        public static int WinGameSound = 0x5B4;
        public static int LoseGameSound = 0x5B3;

        #endregion

        #region Action Durations

        public static TimeSpan GameStartDuration = TimeSpan.FromSeconds(5);
        public static TimeSpan RoundStartDuration = TimeSpan.FromSeconds(5);
        public static TimeSpan BiddingDuration = TimeSpan.FromSeconds(30);
        public static TimeSpan CalloutDuration = TimeSpan.FromSeconds(5);
        public static TimeSpan CalloutResolutionDuration = TimeSpan.FromSeconds(5);
        public static TimeSpan PostCalloutResolutionDuration = TimeSpan.FromSeconds(5);
        public static TimeSpan GameResolutionDuration = TimeSpan.FromSeconds(5);

        #endregion

        public static int MaxPlayers = 5;
        public static int MaxTurnsSkippable = 3;

        public static int MaximumEffectiveBidValue = 156;

        public string m_CurrentText = "";
        public int m_CurrentTextHue = 2655;
        public CurrentActionType m_CurrentAction = CurrentActionType.LobbyWaitingForPlayers;
        public TimeSpan m_ActionTimeRemaining = TimeSpan.FromSeconds(30);

        public int m_RoundCount = -1;

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
                if (player == null)
                    continue;

                if (player.m_Playing)
                {
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

                if (player.m_Spectating)
                {
                    if (player.m_Player.NetState != null)
                        player.m_Player.SendSound(LiarsDiceInstance.DiceRollSound);
                }
            }
        }

        public int GetPlayerCount()
        {
            int playersPlaying = 0;

            foreach (LiarsDicePlayer player in m_Players)
            {
                if (player == null) continue;
                if (player.m_Playing)
                    playersPlaying++;
            }

            return playersPlaying;
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

            //TEST: ADD GAME START SOUND

            m_CurrentText = "Game will begin in 5 seconds.";
            m_CurrentAction = CurrentActionType.RoundStart;
            m_ActionTimeRemaining = RoundStartDuration;

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

            m_RoundCount++;

            m_LastBidCount = 1;
            m_LastBidValue = 0;

            m_LiarCalloutPlayer = null;
            m_LiarCalloutPlayerTarget = null;

            if (m_RoundCount > 0)
            {
                m_CurrentText = "Bidding will begin in 5 seconds.";
                m_CurrentAction = CurrentActionType.PlayerBidding;
                m_ActionTimeRemaining = BiddingDuration;
            }

            else
            {
                BeginBidding();

                return;
            }

            UpdateGumps();
        }

        public void BeginBidding()
        {
            if (m_CurrentPlayer != null)
            {
                if (m_CurrentPlayer.m_Player != null)
                {
                    m_CurrentPlayer.m_Player.SendSound(LiarsDiceInstance.PlayerAlertSound);
                    m_CurrentText = m_CurrentPlayer.m_Player.RawName + " starts the bidding.";

                    m_CurrentPlayer.m_Player.SendSound(LiarsDiceInstance.PlayerAlertSound);
                }
            }
            
            m_CurrentAction = CurrentActionType.PlayerBidding;
            m_ActionTimeRemaining = BiddingDuration;

            UpdateGumps();
        }

        public void PlayerPasses()
        {
            if (m_CurrentPlayer == null)
                return;

            int currentBidTotal = ((m_CurrentPlayer.m_NewBidCount) * 6) + (m_CurrentPlayer.m_NewBidValue);
            int effectiveLastBidTotal = ((m_LastBidCount) * 6) + (m_LastBidValue);

            if (currentBidTotal > effectiveLastBidTotal)
            {
            }

            else
            {
                if (m_CurrentPlayer.m_NewBidValue > m_LastBidValue)
                    m_CurrentPlayer.m_NewBidCount = m_LastBidCount;

                else                
                    m_CurrentPlayer.m_NewBidCount = m_LastBidCount + 1; 
            }

            m_CurrentPlayer.m_BidCount = m_CurrentPlayer.m_NewBidCount;
            m_CurrentPlayer.m_BidValue = m_CurrentPlayer.m_NewBidValue;

            m_LastBidCount = m_CurrentPlayer.m_BidCount;
            m_LastBidValue = m_CurrentPlayer.m_BidValue;

            m_CurrentPlayer.m_TurnsSkipped++;

            int skipsRemain = MaxTurnsSkippable - m_CurrentPlayer.m_TurnsSkipped;

            if (skipsRemain > 0)
            {
                if (m_CurrentPlayer.m_Player != null)
                    m_CurrentText = m_CurrentPlayer.m_Player.RawName + " passes and bids " + m_CurrentPlayer.m_BidCount.ToString() + " " + m_CurrentPlayer.m_BidValue.ToString() + "'s.  (" + skipsRemain.ToString() + " more passes until forfeit)";
            }

            else
            {
                m_CurrentPlayer.m_Playing = false;

                if (m_CurrentPlayer.m_Player != null)
                    m_CurrentText = m_CurrentPlayer.m_Player.RawName + " passes and bids " + m_CurrentPlayer.m_BidCount.ToString() + " " + m_CurrentPlayer.m_BidValue.ToString() + "'s.  (forfeits game)";
            }

            if (NextPlayer())
            {
                foreach (LiarsDicePlayer player in m_Players)
                {
                    if (player == null) continue;
                    if (!player.m_Spectating) continue;
                    if (player.m_Player == null) continue;

                    if (player.m_Player.NetState != null)
                    {
                        if (m_CurrentPlayer == player)
                            player.m_Player.SendSound(LiarsDiceInstance.PlayerAlertSound);

                        else
                            player.m_Player.SendSound(LiarsDiceInstance.BidConfirmSound);
                    }
                }

                m_CurrentAction = CurrentActionType.PlayerBidding;
                m_ActionTimeRemaining = BiddingDuration;

                UpdateGumps();
            }

            else            
                EndGame();            
        }

        public void PlaceBid(LiarsDicePlayer bidder, int bidCount, int bidValue)
        {
            if (bidder == null) 
                return;

            bidder.m_BidCount = bidCount;
            bidder.m_BidValue = bidValue;

            m_LastBidCount = bidder.m_BidCount;
            m_LastBidValue = bidder.m_BidValue;

            m_CurrentText = m_CurrentPlayer.m_Player.RawName + " bids " + m_CurrentPlayer.m_BidCount.ToString() + " " + m_CurrentPlayer.m_BidValue.ToString() + "'s.";

            if (NextPlayer())
            {
                foreach (LiarsDicePlayer player in m_Players)
                {
                    if (player == null) continue;
                    if (!player.m_Spectating) continue;
                    if (player.m_Player == null) continue;

                    if (player.m_Player.NetState != null)
                    {
                        if (m_CurrentPlayer == player)
                            player.m_Player.SendSound(LiarsDiceInstance.PlayerAlertSound);

                        else
                            player.m_Player.SendSound(LiarsDiceInstance.BidConfirmSound);
                    }
                }

                m_CurrentAction = CurrentActionType.PlayerBidding;
                m_ActionTimeRemaining = BiddingDuration;

                UpdateGumps();
            }

            else
                EndGame();
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

            foreach (LiarsDicePlayer player in m_Players)
            {
                if (player == null) continue;
                if (!player.m_Spectating) continue;
                if (player.m_Player == null) continue;

                if (player.m_Player.NetState != null)                            
                    player.m_Player.SendSound(LiarsDiceInstance.CalloutBeginSound);                
            }

            m_CurrentText = playerFrom.m_Player.RawName + " calls " + playerTarget.m_Player.RawName + " a liar! (" + playerTarget.m_BidCount.ToString() + " " + playerTarget.m_BidValue + "'s)";
            m_CurrentAction = CurrentActionType.LiarCallout;
            m_ActionTimeRemaining = CalloutDuration;

            UpdateGumps();
        }

        public void ResolveCallOut()
        {
            bool wasLiar = false;

            if (m_LiarCalloutPlayer == null || m_LiarCalloutPlayerTarget == null) return;
            if (m_LiarCalloutPlayer.m_Player == null || m_LiarCalloutPlayerTarget.m_Player == null) return;

            if (wasLiar)
            {
                m_LiarCalloutPlayerTarget.m_DiceRemaining--;

                m_CurrentText = m_LiarCalloutPlayerTarget.m_Player.RawName + " is found to be a liar and loses a die.";

                if (m_LiarCalloutPlayerTarget.m_Dice.Count > 0)                
                    m_LiarCalloutPlayerTarget.m_Dice.RemoveAt(m_LiarCalloutPlayerTarget.m_Dice.Count - 1);                

                if (m_LiarCalloutPlayerTarget.m_DiceRemaining <= 0)
                {                    
                    m_LiarCalloutPlayerTarget.m_Playing = false;
                    m_CurrentText = m_LiarCalloutPlayerTarget.m_Player.RawName + " is found to be a liar and is eliminated from the game!";
                }
            }

            else
            {
                m_LiarCalloutPlayer.m_DiceRemaining--;

                m_CurrentText = m_LiarCalloutPlayer.m_Player.RawName + " loses their challenge and loses a die.";

                if (m_LiarCalloutPlayer.m_Dice.Count > 0)
                    m_LiarCalloutPlayer.m_Dice.RemoveAt(m_LiarCalloutPlayer.m_Dice.Count - 1);   

                if (m_LiarCalloutPlayer.m_DiceRemaining <= 0)
                {
                    m_LiarCalloutPlayer.m_Playing = false;
                    m_CurrentText = m_LiarCalloutPlayer.m_Player.RawName + " loses their challenge and is eliminated from the game!";
                }
            }

            foreach (LiarsDicePlayer player in m_Players)
            {
                if (player == null) continue;
                if (!player.m_Spectating) continue;
                if (player.m_Player == null) continue;

                if (player.m_Player.NetState != null)
                {
                    if (wasLiar)
                        player.m_Player.SendSound(LiarsDiceInstance.CalloutSuccessSound);

                    else
                        player.m_Player.SendSound(LiarsDiceInstance.CalloutFailSound);
                }
            }

            m_CurrentAction = CurrentActionType.CalloutResolution;
            m_ActionTimeRemaining = CalloutResolutionDuration;

            UpdateGumps();
        }

        public void PostResolveCallOut()
        {
            int remainingPlayers = GetPlayerCount();

            if (remainingPlayers > 1)
            {
                m_CurrentAction = CurrentActionType.RoundStart;
                m_ActionTimeRemaining = RoundStartDuration;
            }

            else
            {
                m_CurrentAction = CurrentActionType.GameResolution;
                m_ActionTimeRemaining = GameResolutionDuration;
            }    
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
                            m_ActionTimeRemaining = BiddingDuration;

                            UpdateGumps();
                        }

                        else
                        {
                            EndGame();
                            return;
                        }
                    }
                break;

                case CurrentActionType.RoundStart:
                    if (m_CurrentPlayer == playerFrom)
                    {
                        if (NextPlayer())
                        {
                            m_CurrentAction = CurrentActionType.PlayerBidding;
                            m_ActionTimeRemaining = BiddingDuration;

                            UpdateGumps();
                        }

                        else
                        {
                            EndGame();
                            return;
                        }
                    }
                break;
            }            
        }

        public void EndGame()
        {
            m_CurrentText = "A player has left the game.";

            m_CurrentAction = CurrentActionType.GameResolution;
            m_ActionTimeRemaining = GameResolutionDuration;
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
                            m_Instance.StartGame();                        
                    break;

                    case CurrentActionType.RoundStart:
                        if (m_Instance.m_ActionTimeRemaining <= TimeSpan.FromSeconds(0))
                            m_Instance.RoundStart();    
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
                        m_Instance.PostResolveCallOut();
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
            writer.Write(m_CurrentTextHue);
            writer.Write((int)m_CurrentAction);
            writer.Write(m_ActionTimeRemaining);
            writer.Write(m_RoundCount);
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
                m_CurrentTextHue = reader.ReadInt();
                m_CurrentAction = (CurrentActionType)reader.ReadInt();
                m_ActionTimeRemaining = reader.ReadTimeSpan();
                m_RoundCount = reader.ReadInt();
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

            AddBackground(5, 9, 636, 414, 9270);
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
            AddImage(18, 247, 3604, 2052);
            AddImage(146, 247, 3604, 2052);
            AddImage(274, 247, 3604, 2052);
            AddImage(402, 247, 3604, 2052);
            AddImage(504, 247, 3604, 2052);            
            AddImage(234, 8, 2446, 2401);            
            AddImage(18, 282, 3604, 2052);
            AddImage(146, 282, 3604, 2052);
            AddImage(274, 282, 3604, 2052);
            AddImage(402, 282, 3604, 2052);
            AddImage(504, 282, 3604, 2052);           
            AddImage(79, -4, 9809, 0);
            AddItem(74, 19, 3823);
            AddItem(82, 9, 3823);
            AddItem(85, 19, 3823);            
            AddImage(18, 187, 62);            
            AddImage(81, 119, 9801);
            AddLabel(289, 8, WhiteTextHue, "Liar's Dice");

            #endregion

            //-----            

            AddButton(16, 17, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(12, 5, 149, "Guide");

            AddLabel(133, 19, 2550, Utility.CreateCurrencyString(m_Instance.m_Pot));
            AddLabel(133, 39, WhiteTextHue, "Gold Pot");
            
            AddLabel(522, 30, 1256, "Leave Game");    
            AddButton(599, 26, 2472, 2473, 2, GumpButtonType.Reply, 0);                  

            #region Player

            if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_Instance.m_CurrentPlayer == m_DicePlayer && dicePlayer.m_Playing)
            {
                AddImage(58, 69, 30077, 2599); //Selection
                AddLabel(Utility.CenteredTextOffset(105, m_Player.RawName), 94, 63, m_Player.RawName);

                int currentBidTotal = ((m_DicePlayer.m_NewBidCount) * 6) + (m_DicePlayer.m_NewBidValue);
                int effectiveLastBidTotal = ((m_Instance.m_LastBidCount) * 6) + (m_Instance.m_LastBidValue);

                if (m_DicePlayer.m_NewBidCount < 98)
                    AddButton(35, 119, 9900, 9900, 4, GumpButtonType.Reply, 0); //Increase Bid Count

                if (m_DicePlayer.m_NewBidCount > 1)
                    AddButton(35, 143, 9906, 9906, 5, GumpButtonType.Reply, 0); //Decrease Bid Count

                AddLabel(66, 130, WhiteTextHue, m_DicePlayer.m_NewBidCount.ToString()); //Count
                AddImage(94, 130, m_Instance.GetDiceItemId(m_DicePlayer.m_NewBidValue)); //Dice
                
                AddButton(135, 119, 9900, 9900, 6, GumpButtonType.Reply, 0); //Increase Bid Value
                AddButton(135, 143, 9906, 9906, 7, GumpButtonType.Reply, 0); //Decrease Bid Value

                if (currentBidTotal > effectiveLastBidTotal)
                {
                    AddLabel(94, 180, WhiteTextHue, "Bid");
                    AddButton(90, 205, 9721, 9725, 8, GumpButtonType.Reply, 0);
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
                        case 0: AddImage(66, 264, diceItemId, diceHue); break;
                        case 1: AddImage(96, 264, diceItemId, diceHue); break;
                        case 2: AddImage(126, 264, diceItemId, diceHue); break;
                        case 3: AddImage(81, 294, diceItemId, diceHue); break;
                        case 4: AddImage(111, 294, diceItemId, diceHue); break;
                    }
                }
            }  

            else if (dicePlayer.m_Playing)
            {
                AddLabel(Utility.CenteredTextOffset(110, m_Player.RawName), 94, WhiteTextHue, m_Player.RawName);

                int bidTextHue = WhiteTextHue;
                int bidDiceHue = 0;

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.LiarCallout && dicePlayer == m_Instance.m_LiarCalloutPlayerTarget)
                {
                    bidTextHue = 1256;
                    bidDiceHue = 1256;
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution && dicePlayer == m_Instance.m_LiarCalloutPlayerTarget)
                {
                    bidTextHue = 1256;
                    bidDiceHue = 1256;
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution && dicePlayer == m_Instance.m_LiarCalloutPlayerTarget)
                {
                    bidTextHue = 1256;
                    bidDiceHue = 1256;
                }

                if (m_DicePlayer.m_BidCount > 0)
                    AddLabel(66, 130, bidTextHue, m_DicePlayer.m_BidCount.ToString()); //Bid Count
                
                if (m_DicePlayer.m_BidValue > 0)
                    AddImage(94, 130, m_Instance.GetDiceItemId(m_DicePlayer.m_BidValue), bidDiceHue); //Dice Value

                for (int a = 0; a < dicePlayer.m_Dice.Count; a++)
                {
                    int diceValue = dicePlayer.m_Dice[a];

                    int diceItemId = m_Instance.GetDiceItemId(diceValue);
                    int diceHue = 0;

                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution)
                    {
                        if (diceValue == m_Instance.m_LiarCalloutPlayerTarget.m_BidValue)
                            diceHue = 63;
                    }

                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution)
                    {
                        if (diceValue == m_Instance.m_LiarCalloutPlayerTarget.m_BidValue)
                            diceHue = 63;
                    }

                    switch (a)
                    {
                        case 0: AddImage(66, 264, diceItemId, diceHue); break;
                        case 1: AddImage(96, 264, diceItemId, diceHue); break;
                        case 2: AddImage(126, 264, diceItemId, diceHue); break;
                        case 3: AddImage(81, 294, diceItemId, diceHue); break;
                        case 4: AddImage(111, 294, diceItemId, diceHue); break;
                    }
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.LiarCallout && m_Instance.m_LiarCalloutPlayerTarget == dicePlayer)
                {
                    if (m_Instance.m_LiarCalloutPlayer != null)
                    {
                        if (m_Instance.m_LiarCalloutPlayer.m_Player != null)
                        {
                            AddImage(157, 261, 4506, 2116);
                            AddLabel(212, 275, 2116, m_Instance.m_LiarCalloutPlayer.m_Player.RawName + " Challenges You");
                        }
                    }                   
                }

                //TEST: Switch to Win / Lose Text
                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution && m_Instance.m_LiarCalloutPlayerTarget == dicePlayer)
                {
                    if (m_Instance.m_LiarCalloutPlayer != null)
                    {
                        if (m_Instance.m_LiarCalloutPlayer.m_Player != null)
                        {
                            AddImage(157, 261, 4506, 2116);
                            AddLabel(212, 275, 2116, m_Instance.m_LiarCalloutPlayer.m_Player.RawName + " Challenges You");
                        }
                    }   
                }

                //TEST: Switch to Win / Lose Text
                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution && m_Instance.m_LiarCalloutPlayerTarget == dicePlayer)
                {
                    if (m_Instance.m_LiarCalloutPlayer != null)
                    {
                        if (m_Instance.m_LiarCalloutPlayer.m_Player != null)
                        {
                            AddImage(157, 261, 4506, 2116);
                            AddLabel(212, 275, 2116, m_Instance.m_LiarCalloutPlayer.m_Player.RawName + " Challenges You");
                        }
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
            int startY = 68;
            int playerSpacing = 105;

            for (int a = 0; a < m_OrderedPlayerList.Count; a++)
            {
                LiarsDicePlayer otherPlayer = m_OrderedPlayerList[a];

                int nameTextHue = WhiteTextHue;

                if (m_Instance.m_CurrentPlayer == otherPlayer)
                {
                    AddImage(0 + startX, 0 + startY, 30077, 2587); //Selection
                    nameTextHue = 149;
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.LiarCallout && m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                    nameTextHue = 1256;

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution && m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                    nameTextHue = 1256;

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution && m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                    nameTextHue = 1256;

                AddLabel(Utility.CenteredTextOffset(48 + startX, otherPlayer.m_Player.RawName), 24 + startY, nameTextHue, otherPlayer.m_Player.RawName);
                             
                AddImage(20 + startX, 49 + startY, 9801); //Cup
                
                int diceItemId = m_Instance.GetDiceItemId(otherPlayer.m_BidValue);
               
                int bidTextHue = WhiteTextHue;
                int bidDiceHue = 0;

                if (otherPlayer.m_BidCount == m_Instance.m_LastBidCount && otherPlayer.m_BidValue == m_Instance.m_LastBidValue)
                {
                    bidTextHue = 2603;
                    bidDiceHue = 2603;
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.LiarCallout && otherPlayer == m_Instance.m_LiarCalloutPlayerTarget)
                {
                    bidTextHue = 1256;
                    bidDiceHue = 1256;
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution && otherPlayer == m_Instance.m_LiarCalloutPlayerTarget)
                {
                    bidTextHue = 1256;
                    bidDiceHue = 1256;
                }

                if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution && otherPlayer == m_Instance.m_LiarCalloutPlayerTarget)
                {
                    bidTextHue = 1256;
                    bidDiceHue = 1256;
                }

                if (otherPlayer.m_BidCount > 0)
                    AddLabel(5 + startX, 61 + startY, bidTextHue, otherPlayer.m_BidCount.ToString());

                if (otherPlayer.m_BidValue > 0)
                    AddImage(33 + startX, 61 + startY, diceItemId, bidDiceHue); //Dice

                for (int b = 0; b < otherPlayer.m_Dice.Count; b++)
                {
                    if (!otherPlayer.m_Playing)
                        continue;

                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution || m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution)
                    {
                        int otherPlayerDieValue = otherPlayer.m_Dice[b];

                        diceItemId = m_Instance.GetDiceItemId(otherPlayerDieValue);
                        bidDiceHue = 0;
                       
                        if (m_Instance.m_LiarCalloutPlayerTarget.m_BidValue == otherPlayerDieValue)
                            bidDiceHue = 63;

                        switch (b)
                        {
                            case 0:
                                AddImage(2 + startX, 123 + startY, diceItemId, bidDiceHue); 
                            break;

                            case 1:
                                AddImage(32 + startX, 123 + startY, diceItemId, bidDiceHue);
                            break;

                            case 2:
                                AddImage(62 + startX, 123 + startY, diceItemId, bidDiceHue);
                            break;

                            case 3:
                                AddImage(17 + startX, 153 + startY, diceItemId, bidDiceHue);
                            break;

                            case 4:
                                AddImage(47 + startX, 153 + startY, diceItemId, bidDiceHue);
                            break;
                        }
                    }

                    else
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
                        string challengeText = m_Instance.m_LiarCalloutPlayer.m_Player.RawName + " Challenges";

                        AddImage(17 + startX, 180 + startY, 4500, 1256);
                        AddLabel(Utility.CenteredTextOffset(45 + startX, challengeText), 233 + startY, 1256, challengeText);
                    }
                }

                //TEST: Add Win / Lose Text
                else if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.CalloutResolution)
                {
                    if (m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                    {
                        string challengeText = m_Instance.m_LiarCalloutPlayer.m_Player.RawName + " Challenges";

                        AddImage(17 + startX, 180 + startY, 4500, 1256);
                        AddLabel(Utility.CenteredTextOffset(45 + startX, challengeText), 233 + startY, 1256, challengeText);
                    }
                }

                //TEST: Add Win / Lose Text
                else if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PostCalloutResolution)
                {
                    if (m_Instance.m_LiarCalloutPlayerTarget == otherPlayer)
                    {
                        string challengeText = m_Instance.m_LiarCalloutPlayer.m_Player.RawName + " Challenges";

                        AddImage(17 + startX, 180 + startY, 4500, 1256);
                        AddLabel(Utility.CenteredTextOffset(45 + startX, challengeText), 233 + startY, 1256, challengeText);
                    }
                }

                startX += playerSpacing;
            }

            #endregion

            AddButton(22, 395, 2117, 2118, 3, GumpButtonType.Reply, 0);
            AddLabel(44, 392, 2550, "Send Chat Message");

            //TEST: Add Multiple Text
            AddLabel(Utility.CenteredTextOffset(415, m_Instance.m_CurrentText), 330, m_Instance.m_CurrentTextHue, m_Instance.m_CurrentText);
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
                            m_Player.SendSound(LiarsDiceInstance.BidChangeSound);
                        }
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
                            m_Player.SendSound(LiarsDiceInstance.BidChangeSound);
                        }
                    }

                    closeGump = false;
                break;

                //Increase Bid Value
                case 6:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer == m_Instance.m_CurrentPlayer)
                    {
                        m_DicePlayer.m_NewBidValue++;
                        m_Player.SendSound(LiarsDiceInstance.BidChangeSound);

                        if (m_DicePlayer.m_NewBidValue > 6)
                            m_DicePlayer.m_NewBidValue = 1;
                    }

                    closeGump = false;
                break;

                //Decrease Bid Value
                case 7:
                    if (m_Instance.m_CurrentAction == LiarsDiceInstance.CurrentActionType.PlayerBidding && m_DicePlayer == m_Instance.m_CurrentPlayer)
                    {
                        m_DicePlayer.m_NewBidValue--;
                        m_Player.SendSound(LiarsDiceInstance.BidChangeSound);

                        if (m_DicePlayer.m_NewBidValue < 1)
                            m_DicePlayer.m_NewBidValue = 6;
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
