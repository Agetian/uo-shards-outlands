using System;

using Server.Mobiles;

namespace Server.Items
{
	public class Drums : BaseInstrument
	{
		[Constructable]
		public Drums() : base( 0xE9C, 0x38, 0x39 )
		{
            Name = "drums";
			Weight = 3.0;
		}

		public Drums( Serial serial ) : base( serial )
		{
		}

		public override void PlayInstrumentWell(Mobile from)
		{
			base.PlayInstrumentWell(from);
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