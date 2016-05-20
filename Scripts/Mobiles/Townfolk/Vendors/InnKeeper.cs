using System; 
using System.Collections.Generic; 
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles 
{ 
	public class InnKeeper : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public InnKeeper() : base( "the innkeeper" ) 
		{ 
		} 

		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBInnKeeper() ); 			
		} 

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes; }
		}


        public override bool HandlesOnSpeech(Mobile from)
        {
            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            string text = e.Speech.Trim().ToLower();

            if (player.Alive)
            {
                if (text.IndexOf("house") != -1 || text.IndexOf("housing") != -1 || text.IndexOf("claim") != -1 || text.IndexOf("stable") != -1)
                {
                    e.Handled = true;

                    player.CloseGump(typeof(StableGump));
                    player.SendGump(new StableGump(this, player, 0));

                    player.SendSound(0x055);

                    return;
                }
            }

            else
                base.OnSpeech(e);
        }

        private class StableEntry : ContextMenuEntry
        {
            private Mobile m_Vendor;
            private Mobile m_From;

            public StableEntry(Mobile vendor, Mobile from): base(6126, 12)
            {
                m_Vendor = vendor;
                m_From = from;
            }

            public override void OnClick()
            {
                PlayerMobile player = m_From as PlayerMobile;

                if (player == null)
                    return;

                player.CloseGump(typeof(StableGump));
                player.SendGump(new StableGump(m_Vendor, player, 0));

                player.SendSound(0x055);
            }
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)            
                list.Add(new StableEntry(this, from));            

            base.AddCustomContextEntries(from, list);
        }

		public InnKeeper( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	} 
}