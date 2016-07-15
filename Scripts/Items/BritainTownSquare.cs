using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
    public class BritainTownSquare : Item
    {
        public override string DefaultName { get { return "Town Square"; } }
        private static readonly int MaximumRange = 20;
        private int m_ActiveRange;
		private Region m_Region;
        private static TimeSpan m_TimeBetweenNotifications = TimeSpan.FromMinutes(3);

        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveRange { get { return m_ActiveRange; } set { m_ActiveRange = Math.Min(value, MaximumRange); } }

        [Constructable]
        public BritainTownSquare()
            : base(0x23F)
        {
            m_ActiveRange = 20;
            Movable = Visible = false;
        }

        public static void BritainTownSquareModifier(Mobile from, Skill skill, ref double gc)
        {
            IPooledEnumerable eable = from.GetItemsInRange(MaximumRange);
            foreach (Item item in eable)
            {
                if (item is BritainTownSquare && from.InRange(item.Location, ((BritainTownSquare)item).ActiveRange))
                {
                    if (from is PlayerMobile)
                    {
                        var pm = from as PlayerMobile;
                        if (pm.LastTownSquareNotification + m_TimeBetweenNotifications < DateTime.Now)
                        {
                            from.SendMessage("You feel your skills are quickly improving!");
                            pm.LastTownSquareNotification = DateTime.Now;
                        }
                    }
                    gc *= 1.5;
                    eable.Free();
                    return;
                }
            }
            eable.Free();
        }

        public BritainTownSquare(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(2); // version
            writer.Write((int)0); // LEGACY (number of activated skills)
            writer.Write(m_ActiveRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
			
			// LEGACY (number of activated skills)
			int activated_count = reader.ReadInt(); 
			byte dummy = 0;
			for (int i = 0; i < activated_count; i++)
				dummy = reader.ReadByte();

			m_ActiveRange = reader.ReadInt();
			Point3D min = new Point3D(this.Location.X - m_ActiveRange, this.Location.Y - m_ActiveRange, -150);
			Point3D max = new Point3D(this.Location.X + m_ActiveRange, this.Location.Y + m_ActiveRange, 150);

			string BritainTownSquare_name = Serial.ToString(); //dummy name to get away from the "duplicate region" warnings
			m_Region = new BritainTownSquareRegion(new Rectangle3D(min, max), Region.Find(Location, Map.Felucca), BritainTownSquare_name);
			m_Region.Register();
        }
    }

	public class BritainTownSquareRegion : Region
	{
		public BritainTownSquareRegion(Rectangle3D area, Region parent, string name)
			: base(name, Map.Felucca, parent, area)
		{
		}
		public override bool AllowHousing(Mobile from, Point3D p)
		{
			return false;
		}
		public override void OnEnter(Mobile m)
		{
			base.OnEnter(m);
		}
		public override void OnExit(Mobile m)
		{
			base.OnExit(m);
		}
	}
}