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
    public class CompetitionContext: Item
    {
        public ArenaParticipant m_ArenaParticipant;

        [Constructable]
        public CompetitionContext(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public CompetitionContext(Serial serial): base(serial)
        {
        }

        #region OnEvents

        public void OnMapChanged(PlayerMobile player)
        {
            OnLocationChanged(player);
        }

        public void OnLocationChanged(PlayerMobile player)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    m_ArenaParticipant.m_ArenaFight.OnLocationChanged(player);
            }
        }

        public void OnDeath(PlayerMobile player, Container corpse)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    m_ArenaParticipant.m_ArenaFight.OnDeath(player, corpse);
            }
        }

        public bool AllowFreeConsume(PlayerMobile player)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    return m_ArenaParticipant.m_ArenaFight.AllowFreeConsume(player);
            }

            return true;
        }

        public bool AllowItemEquip(PlayerMobile player, Item item)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    return m_ArenaParticipant.m_ArenaFight.AllowItemEquip(player, item);
            }

            return true;
        }

        public bool AllowItemRemove(PlayerMobile player, Item item)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    return m_ArenaParticipant.m_ArenaFight.AllowItemRemove(player, item);
            }

            return true;
        }

        public bool AllowItemUse(PlayerMobile player, Item item)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    return m_ArenaParticipant.m_ArenaFight.AllowItemUse(player, item);
            }

            return true;
        }

        public bool AllowSkillUse(PlayerMobile player, SkillName skill)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    return m_ArenaParticipant.m_ArenaFight.AllowSkillUse(player, skill);
            }

            return true;
        }

        public bool AllowSpellCast(PlayerMobile player, Spell spell)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    return m_ArenaParticipant.m_ArenaFight.AllowSpellCast(player, spell);
            }
            
            return true;
        }

        public void CancelSpell(PlayerMobile player)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    m_ArenaParticipant.m_ArenaFight.CancelSpell(player);       
            }           
        }

        public void ClearEffects(PlayerMobile player)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    m_ArenaParticipant.m_ArenaFight.ClearEffects(player);    
            } 
        }

        public void RemoveAggressions(PlayerMobile player)
        {
            if (m_ArenaParticipant != null)
            {
                if (m_ArenaParticipant.m_ArenaFight != null)
                    m_ArenaParticipant.m_ArenaFight.RemoveAggressions(player); 
            } 
        }

        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ArenaParticipant);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaParticipant = (ArenaParticipant)reader.ReadItem();
            }

            //-----

            if (m_ArenaParticipant == null)
                Delete();

            else if (m_ArenaParticipant.Deleted)
                Delete();
        }
    }
}