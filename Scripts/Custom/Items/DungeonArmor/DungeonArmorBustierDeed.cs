﻿using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonArmorBustierDeed : Item
    {
        [Constructable]
        public DungeonArmorBustierDeed(): base(0x14F0)
        {
            Name = "a dungeon armor bustier conversion deed";
            Hue = 2952; 
        }

        public DungeonArmorBustierDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the dungeon armor chest piece you wish to convert.");
            from.Target = new DungeonArmorChestTarget(this);
        }

        public class DungeonArmorChestTarget : Target
        {
            private DungeonArmorBustierDeed m_DungeonArmorBustierDeed;

            public DungeonArmorChestTarget(DungeonArmorBustierDeed DungeonArmorBustierDeed): base(18, false, TargetFlags.None)
            {
                m_DungeonArmorBustierDeed = DungeonArmorBustierDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (m_DungeonArmorBustierDeed == null) return;
                if (m_DungeonArmorBustierDeed.Deleted) return;
                
                if (!m_DungeonArmorBustierDeed.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                BaseDungeonArmor dungeonArmorChest = target as BaseDungeonArmor;

                if (dungeonArmorChest == null)
                {
                    player.SendMessage("That is not a dungeon armor chest piece.");
                    return;
                }

                if (dungeonArmorChest.Layer != Layer.InnerTorso)
                {
                    player.SendMessage("That is not a dungeon armor chest piece.");
                    return;
                }

                if (!dungeonArmorChest.IsChildOf(player.Backpack))
                {
                    from.SendMessage("The target item must be in your pack.");
                    return;
                }

                dungeonArmorChest.ItemID = 0x1c0a;

                player.SendSound(0x64E);

                from.SendMessage("You change the appearance of your dungeon armor chest piece.");
                
                m_DungeonArmorBustierDeed.Delete();  
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
        }
    }
}