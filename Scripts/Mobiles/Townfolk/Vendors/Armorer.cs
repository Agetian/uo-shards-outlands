using System;
using System.Collections.Generic;
using Server;

namespace Server.Mobiles
{
	public class Armorer : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Armorer() : base( "the armorer" )
		{
			SetSkill( SkillName.ArmsLore, 64.0, 100.0 );
			SetSkill( SkillName.Blacksmith, 60.0, 83.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBArmorer() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Boots; }
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.HalfApron( Utility.RandomYellowHue() ) );
			AddItem( new Server.Items.Bascinet() );
		}

		public Armorer( Serial serial ) : base( serial )
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