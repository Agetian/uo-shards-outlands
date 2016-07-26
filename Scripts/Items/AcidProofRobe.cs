using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class AcidProofRobe : Robe
	{
		public override int BaseFireResistance{ get{ return 4; } }

		[Constructable]
		public AcidProofRobe()
		{
            Name = "acid proof robe";
			Hue = 2006;
            
		}

		public AcidProofRobe( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
