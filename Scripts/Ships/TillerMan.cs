using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Items
{
    public class TillerMan : Item
    {
        private BaseShip m_Ship;
        public BaseShip Ship { get { return m_Ship; } }

        public TillerMan(BaseShip ship): base(0x3E4E)
        {
            m_Ship = ship;
            Movable = false;
        }

        public TillerMan(Serial serial): base(serial)
        {
        }

        public void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: ItemID = 0x3E4B; break;
                case Direction.North: ItemID = m_Ship is Galleon || m_Ship is Carrack ? 0x3855 : 0x3E4E; break; //
                case Direction.West: ItemID = 0x3E50; break;
                case Direction.East: ItemID = 0x3E53; break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
        }

        public void Say(string text)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, false, text);
        }

        public void Say(int number)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void Say(int number, string args)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Ship != null && m_Ship.ShipName != null)
                list.Add(1042884, m_Ship.ShipName); // the tiller man of the ~1_SHIP_NAME~
            else
                base.AddNameProperty(list);
        }

        public override void OnSingleClick(Mobile from)
        {
            int notoriety = BaseShip.GetShipNotoriety(from, m_Ship);
            int labelHue = Notoriety.Hues[notoriety];
            string shipLabel = m_Ship.Name;

            if (!(m_Ship.ShipName == "" || m_Ship.ShipName == null))
                shipLabel += ": " + m_Ship.ShipName;

            //from.PrivateOverheadMessage(MessageType.Label, labelHue, false, shipLabel, from.NetState);

            LabelTo(from, "Hull: {0} / {1}", m_Ship.HitPoints, m_Ship.MaxHitPoints);
            LabelTo(from, "Sails: {0} / {1}", m_Ship.SailPoints, m_Ship.MaxSailPoints);
            LabelTo(from, "Guns: {0} / {1}", m_Ship.GunPoints, m_Ship.MaxGunPoints);
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Ship == null || from == null)
                return;

            //Ship Gump  
            if (m_Ship.Owner != null)
            {
                //if (!m_Ship.m_ScuttleInProgress)
                    //from.SendGump(new ShipGump(from, m_Ship));
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Ship != null)
                m_Ship.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version

            writer.Write(m_Ship);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Ship = reader.ReadItem() as BaseShip;

                        if (m_Ship == null)
                            Delete();

                        break;
                    }
            }
        }
    }
}