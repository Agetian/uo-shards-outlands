using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public abstract class BaseCloak : BaseClothing
	{
		public BaseCloak( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseCloak( int itemID, int hue ) : base( itemID, Layer.Cloak, hue )
		{
		}

		public BaseCloak( Serial serial ) : base( serial )
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

	[Flipable]
	public class Cloak : BaseCloak
	{
		[Constructable]
		public Cloak() : this( 0 )
		{
		}

		[Constructable]
		public Cloak( int hue ) : base( 0x1515, hue )
		{
			Weight = 1.0;
		}

		public Cloak( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[Flipable( 0x230A, 0x2309 )]
	public class FurCape : BaseCloak
	{
		[Constructable]
		public FurCape() : this( 0 )
		{
            Name = "fur cape";
		}

		[Constructable]
		public FurCape( int hue ) : base( 0x230A, hue )
		{
            Name = "fur cape";
			Weight = 1.0;
		}

		public FurCape( Serial serial ) : base( serial )
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