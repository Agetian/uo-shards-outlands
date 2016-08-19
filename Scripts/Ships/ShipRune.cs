using System;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Regions;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x1f14, 0x1f15, 0x1f16, 0x1f17 )]
	public class ShipRune : Item
	{
        public BaseShip m_Ship;

        [Constructable]
		public ShipRune(BaseShip ship, Mobile owner) : base( 0x1F14 )
		{
            Name = "a ship rune";

            Hue = 88;
			Weight = 1.0;            

            m_Ship = ship;            
		}

        public ShipRune(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            string messageText = "a ship rune: ";

            if (m_Ship != null)
            {
                if (!m_Ship.Deleted)
                {
                    if (m_Ship.ShipName != "")
                        messageText = m_Ship.ShipName;
                }
            }

            LabelTo(from, messageText);

            if (m_Ship != null)
            {
                if (!m_Ship.Deleted)
                    LabelTo(from, "(double click to open ship gump)");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (m_Ship != null)
            {
                if (!m_Ship.Deleted)
                {
                    ShipGumpObject shipGumpObject = new ShipGumpObject(player, m_Ship, null);

                    player.CloseGump(typeof(ShipGump));
                    player.SendGump(new ShipGump(player, shipGumpObject));
                }
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int) 0); //version

            writer.Write(m_Ship);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            m_Ship = (BaseShip)reader.ReadItem();
		}
	}
}