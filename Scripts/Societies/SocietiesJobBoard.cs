using System;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class SocietiesJobBoard : Item
    {
        [Constructable]
        public SocietiesJobBoard(): base(7774)
        {
            Name = "societies job board";
            Hue = 2599;

            Movable = false;
        }

        public SocietiesJobBoard(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {           
            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, Societies.NextJobsAdded, true, true, true, true, false);

            LabelTo(from, "Societies Job Board");              
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                player.SendSound(0x055);

                player.CloseGump(typeof(SocietiesJobBoardGump));
                player.SendGump(new SocietiesJobBoardGump(player, SocietiesGroupType.ArtificersEnclave, SocietiesGroupPageDisplayType.Jobs));
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
}