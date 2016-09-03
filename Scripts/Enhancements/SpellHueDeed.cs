using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server
{
    public class SpellHueDeed : Item
    {
        private Enhancements.SpellType m_SpellType = Enhancements.SpellType.MagicArrow;
        [CommandProperty(AccessLevel.GameMaster)]
        public Enhancements.SpellType SpellType
        {
            get { return m_SpellType; }
            set { m_SpellType = value; }
        }

        private Enhancements.SpellHueType m_SpellHueType = Enhancements.SpellHueType.Charcoal;
        [CommandProperty(AccessLevel.GameMaster)]
        public Enhancements.SpellHueType SpellHueType
        {
            get { return m_SpellHueType; }
            set { m_SpellHueType = value; }
        }  

        [Constructable]
        public SpellHueDeed(): base(5360)
        {
            Name = "a spell hue deed";

            Hue = 2606;

            Randomize();
        }

        public SpellHueDeed(Serial serial): base(serial)
        {
        }

        public void Randomize()
        {
            int spellTypeCount = Enum.GetNames(typeof(Enhancements.SpellType)).Length;
            int spellTypeIndex = Utility.RandomMinMax(0, spellTypeCount - 1);

            int spellHueTypeCount = Enum.GetNames(typeof(Enhancements.SpellHueType)).Length;
            int spellHueTypeIndex = Utility.RandomMinMax(1, spellHueTypeCount - 1);

            m_SpellType = (Enhancements.SpellType)spellTypeIndex;
            m_SpellHueType = (Enhancements.SpellHueType)spellHueTypeIndex;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            Enhancements.SpellTypeDetail spellTypeDetail = Enhancements.GetSpellTypeDetail(m_SpellType);
            Enhancements.SpellHueTypeDetail spellHueTypeDetail = Enhancements.GetSpellHueTypeDetail(m_SpellHueType);

            LabelTo(from, "(" + spellTypeDetail.m_SpellName + ")");
            LabelTo(from, "[" + spellHueTypeDetail.m_SpellHueTypeName + " - Hue " + spellHueTypeDetail.m_SpellHue.ToString() + "]");
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

            Enhancements.SpellHueEntry entry = Enhancements.GetSpellHueEntry(player, m_SpellType);

            Enhancements.SpellTypeDetail spellTypeDetail = Enhancements.GetSpellTypeDetail(m_SpellType);
            Enhancements.SpellHueTypeDetail spellHueTypeDetail = Enhancements.GetSpellHueTypeDetail(m_SpellHueType);

            if (entry == null)
            {
                entry = new Enhancements.SpellHueEntry(m_SpellType);
                entry.m_UnlockedHues.Add(m_SpellHueType);

                player.m_EnhancementsAccountEntry.m_SpellHues.Add(entry);

                player.SendMessage("You unlock [" + spellTypeDetail.m_SpellName + ": " + spellHueTypeDetail.m_SpellHueTypeName + " - Hue " + spellHueTypeDetail.m_SpellHue.ToString() + "] on your account.");
                player.PlaySound(0x5C9);

                Delete();
            }

            else
            {

                bool foundHue = false;

                foreach (Enhancements.SpellHueType spellHueType in entry.m_UnlockedHues)
                {
                    if (spellHueType == null)
                        continue;

                    if (spellHueType == m_SpellHueType)
                        foundHue = true;
                }

                if (foundHue)
                    player.SendMessage("You have already unlocked that hue for that particular spell.");

                else
                {
                    entry.m_UnlockedHues.Add(m_SpellHueType);

                    player.SendMessage("You unlock [" + spellTypeDetail.m_SpellName + ": " + spellHueTypeDetail.m_SpellHueTypeName + " - Hue " + spellHueTypeDetail.m_SpellHue.ToString() + "] on your account.");
                    player.PlaySound(0x5C9);

                    Delete();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
      
            //Version 0
            writer.Write((int)m_SpellType);
            writer.Write((int)m_SpellHueType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_SpellType = (Enhancements.SpellType)reader.ReadInt();
                m_SpellHueType = (Enhancements.SpellHueType)reader.ReadInt();
            }
        }
    }      
}