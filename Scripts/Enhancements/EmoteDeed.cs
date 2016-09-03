using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server
{
    public class EmoteDeed : Item
    {
        private Enhancements.EmoteType m_EmoteType = Enhancements.EmoteType.Anger;
        [CommandProperty(AccessLevel.GameMaster)]
        public Enhancements.EmoteType EmoteType
        {
            get { return m_EmoteType; }
            set { m_EmoteType = value; }
        }    

        [Constructable]
        public EmoteDeed(): base(5360)
        {
            Name = "an emote deed";

            Hue = 2599;

            Randomize();
        }

        public EmoteDeed(Serial serial): base(serial)
        {
        }

        public void Randomize()
        {
            int emoteCount = Enum.GetNames(typeof(Enhancements.EmoteType)).Length;
            int emoteIndex = Utility.RandomMinMax(0, emoteCount - 1);

            m_EmoteType = (Enhancements.EmoteType)emoteIndex;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            Enhancements.EmoteDetail detail = Enhancements.GetEmoteDetail(m_EmoteType);

            LabelTo(from, "(" + detail.m_EmoteName + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;
            
            if (player == null) return;
            if (player.Backpack == null) return;

            if (!IsChildOf(player.Backpack))
            {
                player.SendMessage("That must be in your pack in order to use it.");
                return;
            }

            EnhancementsPersistance.CheckAndCreateEnhancementsAccountEntry(player);

            Enhancements.EmoteEntry entry = Enhancements.GetEmoteEntry(player, m_EmoteType);
            Enhancements.EmoteDetail detail = Enhancements.GetEmoteDetail(m_EmoteType);

            if (entry == null)
            {
                entry = new Enhancements.EmoteEntry(m_EmoteType, true);

                player.m_EnhancementsAccountEntry.m_Emotes.Add(entry);

                player.SendMessage("You unlock the emote: [" + detail.m_EmoteName + "] on your account.");
                player.PlaySound(0x5C9);

                Delete();
            }

            else            
                player.SendMessage("You have already unlocked this emote for your account.");            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
      
            //Version 0
            writer.Write((int)m_EmoteType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_EmoteType = (Enhancements.EmoteType)reader.ReadInt();
            }
        }
    }      
}