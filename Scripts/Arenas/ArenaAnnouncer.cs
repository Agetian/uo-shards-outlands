using System;
using System.Collections.Generic;
using Server;

namespace Server.Mobiles
{
	public class ArenaAnnouncer : BaseVendor
	{
        public ArenaController m_ArenaController;
        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaController ArenaController
        {
            get { return m_ArenaController; }
            set { m_ArenaController = value; }
        }

        public static List<ArenaAnnouncer> m_Instances = new List<ArenaAnnouncer>();

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public ArenaAnnouncer() : base( "the announcer" )
		{
            m_Instances.Add(this);
		}

		public override void InitSBInfo()
		{
		}

        public ArenaAnnouncer(Serial serial): base(serial)
		{
		}

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_ArenaController);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ArenaController = reader.ReadItem() as ArenaController;
            }

            //-----

            m_Instances.Add(this);
		}
	}
}