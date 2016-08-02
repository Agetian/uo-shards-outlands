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
        public static void Initialize()
        {
            //EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
            //EventSink.Login += new LoginEventHandler(EventSink_Login);

            //CommandSystem.Register("vli", AccessLevel.GameMaster, new CommandEventHandler(vli_oc));
        }

        [Constructable]
        public CompetitionContext(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public CompetitionContext(Serial serial): base(serial)
        {
        }

        public virtual bool AllowFreeConsume(PlayerMobile player)
        {
            return true;
        }

        public virtual bool AllowItemEquip(PlayerMobile player, Item item)
        {
            return true;
        }

        public virtual bool AllowItemRemove(PlayerMobile player, Item item)
        {
            return true;
        }

        public virtual bool AllowItemUse(PlayerMobile player, Item item)
        {
            return true;
        }

        public virtual bool AllowSkillUse(PlayerMobile player, SkillName skill)
        {
            return true;
        }

        public virtual bool AllowSpellCast(PlayerMobile player, Spell spell)
        {
            return true;
        }

        public virtual void CancelSpell(PlayerMobile player)
        {
            if (player.Spell is Spell)
            {
                Spell spell = player.Spell as Spell;
                spell.Disturb(DisturbType.Kill);
            }

            Targeting.Target.Cancel(player);
        }

        public virtual void ClearEffects(PlayerMobile player)
        {
            SpecialAbilities.ClearSpecialEffects(player);

            player.RemoveStatMod("[Magic] Str Offset");
            player.RemoveStatMod("[Magic] Dex Offset");
            player.RemoveStatMod("[Magic] Int Offset");

            player.Paralyzed = false;
            player.Hidden = false;

            player.MagicDamageAbsorb = 0;
            player.MeleeDamageAbsorb = 0;

            Spells.Second.ProtectionSpell.Registry.Remove(player);
            player.EndAction(typeof(DefensiveSpell));

            TransformationSpellHelper.RemoveContext(player, true);

            BaseArmor.ValidateMobile(player);
            BaseClothing.ValidateMobile(player);

            player.Hits = player.HitsMax;
            player.Stam = player.StamMax;
            player.Mana = player.ManaMax;

            player.Poison = null;            
        }

        public virtual void RemoveAggressions(PlayerMobile player)
        {
            /*
            for (int i = 0; i < m_Participants.Count; ++i)
            {
                Participant p = (Participant)m_Participants[i];

                for (int j = 0; j < p.Players.Length; ++j)
                {
                    DuelPlayer dp = (DuelPlayer)p.Players[j];

                    if (dp == null || dp.Mobile == mob)
                        continue;

                    mob.RemoveAggressed(dp.Mobile);
                    mob.RemoveAggressor(dp.Mobile);
                    dp.Mobile.RemoveAggressed(mob);
                    dp.Mobile.RemoveAggressor(mob);
                }
            }
            */
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
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

    /*
    public class NewDuelContext : CompetitionContext
    {
    }

    public class TournamentContext : CompetitionContext
    {
    }

    public class BattlegroundContext : CompetitionContext
    {
    }
    */
}