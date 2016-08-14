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
            }

            //-----

            if (m_Player == null)
                Delete();

            else if (m_Player.Deleted)
                Delete();
        }
    }
}