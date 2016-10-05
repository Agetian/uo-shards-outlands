using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Server
{
    public class ArenaPlayerSettings: Item
    {
        public PlayerMobile m_Player;
        public ArenaRuleset m_SavedRulesetPreset;

        public ArenaMatch m_ArenaMatch;
        public ArenaParticipant m_ArenaParticipant 
        { 
            get 
            {
                if (m_ArenaMatch != null)                
                    return m_ArenaMatch.GetParticipant(m_Player);
                
                else
                    return null;
            } 
        }

        #region Stored Records

        public int Ranked1vs1Wins = 0;
        public int Ranked1vs1Losses = 0;
        public int Ranked2vs2Wins = 0;
        public int Ranked2vs2Losses = 0;
        public int Ranked3vs3Wins = 0;
        public int Ranked3vs3Losses = 0;
        public int Ranked4vs4Wins = 0;
        public int Ranked4vs4Losses = 0;

        public int Tournament1vs1EventsWon = 0;
        public int Tournament1vs1RoundsWon = 0;
        public int Tournament1vs1RoundsLost = 0;
        public int Tournament2vs2EventsWon = 0;
        public int Tournament2vs2RoundsWon = 0;
        public int Tournament2vs2RoundsLost = 0;
        public int Tournament3vs3EventsWon = 0;
        public int Tournament3vs3RoundsWon = 0;
        public int Tournament3vs3RoundsLost = 0;
        public int Tournament4vs4EventsWon = 0;
        public int Tournament4vs4RoundsWon = 0;
        public int Tournament4vs4RoundsLost = 0;

        #endregion

        [Constructable]
        public ArenaPlayerSettings(PlayerMobile player): base(0x0)
        {
            m_Player = player;

            m_SavedRulesetPreset = new ArenaRuleset();

            Visible = false;
            Movable = false;
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckCreateArenaPlayerSettings(player);
        }

        public static void CheckCreateArenaPlayerSettings(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;

            if (player.m_ArenaPlayerSettings == null)
                player.m_ArenaPlayerSettings = new ArenaPlayerSettings(player);

            else if (player.m_ArenaPlayerSettings == null)
                player.m_ArenaPlayerSettings = new ArenaPlayerSettings(player);

            if (player.m_ArenaPlayerSettings.m_SavedRulesetPreset == null)
                player.m_ArenaPlayerSettings.m_SavedRulesetPreset = new ArenaRuleset();

            else if (player.m_ArenaPlayerSettings.m_SavedRulesetPreset.Deleted)
                player.m_ArenaPlayerSettings.m_SavedRulesetPreset = new ArenaRuleset();
        }

        public bool CurrentlyInMatch()
        {
            if (m_ArenaMatch == null)
                return false;

            else if (m_ArenaMatch.Deleted)
                return false;
            
            return true;
        }

        public ArenaPlayerSettings(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_Player);
            writer.Write(m_SavedRulesetPreset);
            writer.Write(m_ArenaMatch);

            writer.Write(Ranked1vs1Wins);
            writer.Write(Ranked1vs1Losses);
            writer.Write(Ranked2vs2Wins);
            writer.Write(Ranked2vs2Losses);
            writer.Write(Ranked3vs3Wins);
            writer.Write(Ranked3vs3Losses);
            writer.Write(Ranked4vs4Wins);
            writer.Write(Ranked4vs4Losses);

            writer.Write(Tournament1vs1EventsWon);
            writer.Write(Tournament1vs1RoundsWon);
            writer.Write(Tournament1vs1RoundsLost);
            writer.Write(Tournament2vs2EventsWon);
            writer.Write(Tournament2vs2RoundsWon);
            writer.Write(Tournament2vs2RoundsLost);
            writer.Write(Tournament3vs3EventsWon);
            writer.Write(Tournament3vs3RoundsWon);
            writer.Write(Tournament3vs3RoundsLost);
            writer.Write(Tournament4vs4EventsWon);
            writer.Write(Tournament4vs4RoundsWon);
            writer.Write(Tournament4vs4RoundsLost);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = (PlayerMobile)reader.ReadMobile();
                m_SavedRulesetPreset = (ArenaRuleset)reader.ReadItem();
                m_ArenaMatch = (ArenaMatch)reader.ReadItem();

                Ranked1vs1Wins = reader.ReadInt();
                Ranked1vs1Losses = reader.ReadInt();
                Ranked2vs2Wins = reader.ReadInt();
                Ranked2vs2Losses = reader.ReadInt();
                Ranked3vs3Wins = reader.ReadInt();
                Ranked3vs3Losses = reader.ReadInt();
                Ranked4vs4Wins = reader.ReadInt();
                Ranked4vs4Losses = reader.ReadInt();

                Tournament1vs1EventsWon = reader.ReadInt();
                Tournament1vs1RoundsWon = reader.ReadInt();
                Tournament1vs1RoundsLost = reader.ReadInt();
                Tournament2vs2EventsWon = reader.ReadInt();
                Tournament2vs2RoundsWon = reader.ReadInt();
                Tournament2vs2RoundsLost = reader.ReadInt();
                Tournament3vs3EventsWon = reader.ReadInt();
                Tournament3vs3RoundsWon = reader.ReadInt();
                Tournament3vs3RoundsLost = reader.ReadInt();
                Tournament4vs4EventsWon = reader.ReadInt();
                Tournament4vs4RoundsWon = reader.ReadInt();
                Tournament4vs4RoundsLost = reader.ReadInt();
            }

            //-----

            if (m_Player == null)
                Delete();

            else if (m_Player.Deleted)
                Delete();
        }
    }
}