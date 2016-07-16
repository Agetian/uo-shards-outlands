using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public abstract class BaseOuterTorso : BaseClothing
	{
		public BaseOuterTorso( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseOuterTorso( int itemID, int hue ) : base( itemID, Layer.OuterTorso, hue )
		{
		}

		public BaseOuterTorso( Serial serial ) : base( serial )
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

 	[FlipableAttribute( 0x230D, 0x230E )]
	public class GildedDress : BaseOuterTorso
	{
		[Constructable]
		public GildedDress() : this( 0 )
		{
            Name = "gilded dress";
		}

		[Constructable]
		public GildedDress( int hue ) : base( 0x230D, hue )
		{
            Name = "gilded dress";
			Weight = 2.0;
		}

		public GildedDress( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x1eff, 0x1f00 )]
	public class FancyDress : BaseOuterTorso
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public FancyDress() : this( 0 )
		{
            Name = "fancy dress";
		}

		[Constructable]
		public FancyDress( int hue ) : base( 0x1EFF, hue )
		{
            Name = "fancy dress";
			Weight = 2.0;
		}

		public FancyDress( Serial serial ) : base( serial )
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

	public class DeathRobe : Robe
	{
		public override bool DisplayLootType
		{
			get{ return false; }
		}

		[Constructable]
		public DeathRobe()
		{
            Name = "death robe";
			LootType = LootType.Newbied;			
		}

		public new bool Scissor( Mobile from, Scissors scissors )
		{
			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		public DeathRobe( Serial serial ) : base( serial )
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

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            Delete();

            return true;
        }
	}

	[Flipable]
	public class Robe : BaseOuterTorso
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public Robe() : this( 0 )
		{
            Name = "robe";
		}

		[Constructable]
		public Robe( int hue ) : base( 0x1F03, hue )
		{
            Name = "robe";
			Weight = 2.0;
		}

		public Robe( Serial serial ) : base( serial )
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

	public class MonkRobe : BaseOuterTorso
	{
		[Constructable]
		public MonkRobe() : this( 0x21E )
		{
            Name = "monk robe";
		}
		
		[Constructable]
		public MonkRobe( int hue ) : base( 0x2687, hue )
		{
            Name = "mokn robe";
			Weight = 1.0;
		}
		
		public override bool CanBeBlessed { get { return false; } }
		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		public MonkRobe( Serial serial ) : base( serial )
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

	[FlipableAttribute( 0x2683, 0x2684 )]
	public class HoodedShroudOfShadows : BaseOuterTorso
	{
		[Constructable]
		public HoodedShroudOfShadows() : this( 0x455 )
		{
            Name = "hooded shroud of shadows";
		}

		[Constructable]
		public HoodedShroudOfShadows( int hue ) : base( 0x2683, hue )
		{
            Name = "hooded shroud of shadows";
			Weight = 2.0;
		}

		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		public HoodedShroudOfShadows( Serial serial ) : base( serial )
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

	[Flipable( 0x1f01, 0x1f02 )]
	public class PlainDress : BaseOuterTorso
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public PlainDress() : this( 0 )
		{
            Name = "plain dress";
		}

		[Constructable]
		public PlainDress( int hue ) : base( 0x1F01, hue )
		{
            Name = "plain dress";
			Weight = 2.0;
		}

		public PlainDress( Serial serial ) : base( serial )
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

	[Flipable( 0x2799, 0x27E4 )]
	public class Kamishimo : BaseOuterTorso
	{
		[Constructable]
		public Kamishimo() : this( 0 )
		{
            Name = "kamishimo";
		}

		[Constructable]
		public Kamishimo( int hue ) : base( 0x2799, hue )
		{
            Name = "plain dress";
			Weight = 2.0;
		}

		public Kamishimo( Serial serial ) : base( serial )
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

	[Flipable( 0x279C, 0x27E7 )]
	public class HakamaShita : BaseOuterTorso
	{
		[Constructable]
		public HakamaShita() : this( 0 )
		{
            Name = "hakama shita";
		}

		[Constructable]
		public HakamaShita( int hue ) : base( 0x279C, hue )
		{
            Name = "hakama shita";
			Weight = 2.0;
		}

		public HakamaShita( Serial serial ) : base( serial )
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

	[Flipable( 0x2782, 0x27CD )]
	public class MaleKimono : BaseOuterTorso
	{
		[Constructable]
		public MaleKimono() : this( 0 )
		{
            Name = "male kimono";
		}

		[Constructable]
		public MaleKimono( int hue ) : base( 0x2782, hue )
		{
            Name = "male kimono";
			Weight = 2.0;
		}

		public override bool AllowFemaleWearer{ get{ return false; } }

		public MaleKimono( Serial serial ) : base( serial )
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

	[Flipable( 0x2783, 0x27CE )]
	public class FemaleKimono : BaseOuterTorso
	{
		[Constructable]
		public FemaleKimono() : this( 0 )
		{
            Name = "female kimono";
		}

		[Constructable]
		public FemaleKimono( int hue ) : base( 0x2783, hue )
		{
            Name = "female kimono";
			Weight = 2.0;
		}

		public override bool AllowMaleWearer{ get{ return false; } }

		public FemaleKimono( Serial serial ) : base( serial )
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

	[Flipable( 0x2FB9, 0x3173 )]
	public class MaleElvenRobe : BaseOuterTorso
	{
		[Constructable]
		public MaleElvenRobe() : this( 0 )
		{
            Name = "male elven robe";
		}

		[Constructable]
		public MaleElvenRobe( int hue ) : base( 0x2FB9, hue )
		{
            Name = "male elven robe";
			Weight = 2.0;
		}

		public MaleElvenRobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	[Flipable( 0x2FBA, 0x3174 )]
	public class FemaleElvenRobe : BaseOuterTorso
	{
		[Constructable]
		public FemaleElvenRobe() : this( 0 )
		{
            Name = "female elven robe";
		}

		[Constructable]
		public FemaleElvenRobe( int hue ) : base( 0x2FBA, hue )
		{
            Name = "female elven robe";
			Weight = 2.0;
		}

		public override bool AllowMaleWearer{ get{ return false; } }

		public FemaleElvenRobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	public class ResurrectRobe : BaseOuterTorso
	{
		[Constructable]
		public ResurrectRobe(): base(0x1F03)
		{
            Name = "resurrection robe";
			Weight = 2.0;
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public override TimeSpan DecayTime { get { return TimeSpan.FromSeconds(30); } }

		public ResurrectRobe(Serial serial): base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

		public override bool DropToWorld(Mobile from, Point3D p)
		{
			Delete();
			return true;
		}

		public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
		{
			Delete();
			return true;
		}

		public override bool DropToItem(Mobile from, Item target, Point3D p)
		{
			Delete();
			return true;
		}

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			return DeathMoveResult.MoveToCorpse;
		}

		public override DeathMoveResult OnInventoryDeath(Mobile parent)
		{
			return DeathMoveResult.MoveToCorpse;
		}
	}
}