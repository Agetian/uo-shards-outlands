using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server
{
    public class CustomizationDeed : Item
    {
        private Enhancements.CustomizationType m_CustomizationType = Enhancements.CustomizationType.BenchPlayer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Enhancements.CustomizationType CustomizationType
        {
            get { return m_CustomizationType; }
            set { m_CustomizationType = value; }
        }    

        [Constructable]
        public CustomizationDeed(): base(5360)
        {
            Name = "a customization deed";

            Hue = 2603;

            Randomize();
        }

        public CustomizationDeed(Serial serial): base(serial)
        {
        }

        public void Randomize()
        {
            int customizationCount = Enum.GetNames(typeof(Enhancements.CustomizationType)).Length;
            int customizationIndex = Utility.RandomMinMax(0, customizationCount - 1);

            m_CustomizationType = (Enhancements.CustomizationType)customizationIndex;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            Enhancements.CustomizationDetail detail = Enhancements.GetCustomizationDetail(m_CustomizationType);

            LabelTo(from, "(" + detail.m_CustomizationName + ")");
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

            Enhancements.CustomizationEntry entry = Enhancements.GetCustomizationEntry(player, m_CustomizationType);
            Enhancements.CustomizationDetail detail = Enhancements.GetCustomizationDetail(m_CustomizationType);

            if (entry == null)
            {
                entry = new Enhancements.CustomizationEntry(m_CustomizationType, true, true);

                player.m_EnhancementsAccountEntry.m_Customizations.Add(entry);

                player.SendMessage("You unlock the customization: [" + detail.m_CustomizationName + "] on your account.");
                player.PlaySound(0x5C9);

                Delete();
            }

            else            
                player.SendMessage("You have already unlocked this customization for your account.");            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
      
            //Version 0
            writer.Write((int)m_CustomizationType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_CustomizationType = (Enhancements.CustomizationType)reader.ReadInt();
            }
        }
    }      
}