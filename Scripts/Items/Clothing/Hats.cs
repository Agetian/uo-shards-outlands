using System;
using Server.Engines.Craft;
using Server.Network;
using System.Collections.Generic;

using Server.Misc;
using Server.Mobiles;
namespace Server.Items
{
	public abstract class BaseHat : BaseClothing
	{
		public BaseHat( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseHat( int itemID, int hue ) : base( itemID, Layer.Helm, hue )
		{
		}

		public BaseHat( Serial serial ) : base( serial )
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

		public override int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (Quality)quality;

            if (makersMark)
                DisplayCrafter = true;           

			return base.OnCraft( quality, makersMark, from, craftSystem, typeRes, tool, craftItem, resHue );
		}

	}
    
    [Flipable( 0x2798, 0x27E3 )]
	public class Kasa : BaseHat
	{
		[Constructable]
		public Kasa() : this( 0 )
		{
            Name = "kasa";
		}

		[Constructable]
		public Kasa( int hue ) : base( 0x2798, hue )
		{
            Name = "kasa";
			Weight = 1.0;
		}

		public Kasa( Serial serial ) : base( serial )
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

	[Flipable( 0x278F, 0x27DA )]
	public class ClothNinjaHood : BaseHat
	{
		[Constructable]
		public ClothNinjaHood() : this( 0 )
		{
            Name = "cloth ninja hood";
		}

		[Constructable]
		public ClothNinjaHood( int hue ) : base( 0x278F, hue )
		{
            Name = "cloth ninja hood";
			Weight = 1.0;
		}

		public ClothNinjaHood( Serial serial ) : base( serial )
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

	[Flipable( 0x2305, 0x2306 )]
	public class FlowerGarland : BaseHat
	{
		[Constructable]
		public FlowerGarland() : this( 0 )
		{
            Name = "flower garland";
		}

		[Constructable]
		public FlowerGarland( int hue ) : base( 0x2305, hue )
		{
            Name = "flower garland";
			Weight = 1.0;
		}

		public FlowerGarland( Serial serial ) : base( serial )
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

	public class FloppyHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public FloppyHat() : this( 0 )
		{
            Name = "floppy hat";
		}

		[Constructable]
		public FloppyHat( int hue ) : base( 0x1713, hue )
		{
            Name = "floppy hat";
			Weight = 1.0;
		}

		public FloppyHat( Serial serial ) : base( serial )
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

	public class WideBrimHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public WideBrimHat() : this( 0 )
		{
            Name = "a wide-brim hat";
		}

		[Constructable]
		public WideBrimHat( int hue ) : base( 0x1714, hue )
		{
            Name = "a wide-brim hat";
			Weight = 1.0;
		}

		public WideBrimHat( Serial serial ) : base( serial )
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

	public class Cap : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public Cap() : this( 0 )
		{
            Name = "a cap";
		}

		[Constructable]
		public Cap( int hue ) : base( 0x1715, hue )
		{
            Name = "a cap";
			Weight = 1.0;
		}

		public Cap( Serial serial ) : base( serial )
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

	public class SkullCap : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public SkullCap() : this( 0 )
		{
            Name = "a skullcap";
		}

		[Constructable]
		public SkullCap( int hue ) : base( 0x1544, hue )
		{
            Name = "a skullcap";
			Weight = 1.0;
		}

		public SkullCap( Serial serial ) : base( serial )
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

	public class Bandana : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public Bandana() : this( 0 )
		{
            Name = "a bandana";
		}

		[Constructable]
		public Bandana( int hue ) : base( 0x1540, hue )
		{
            Name = "a bandana";
			Weight = 1.0;
		}

		public Bandana( Serial serial ) : base( serial )
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

	public class TallStrawHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public TallStrawHat() : this( 0 )
		{
            Name = "a tall straw hat";
		}

		[Constructable]
		public TallStrawHat( int hue ) : base( 0x1716, hue )
		{
            Name = "a tall straw hat";
			Weight = 1.0;
		}

		public TallStrawHat( Serial serial ) : base( serial )
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

	public class StrawHat : BaseHat
	{
		[Constructable]
		public StrawHat() : this( 0 )
		{
            Name = "a straw hat";
		}

		[Constructable]
		public StrawHat( int hue ) : base( 0x1717, hue )
		{
            Name = "a straw hat";
			Weight = 1.0;
		}

		public StrawHat( Serial serial ) : base( serial )
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

	public class WizardsHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public WizardsHat() : this( 0 )
		{
            Name = "a wizard's hat";
		}

		[Constructable]
		public WizardsHat( int hue ) : base( 0x1718, hue )
		{
            Name = "a wizard's hat";
			Weight = 1.0;
		}

		public WizardsHat( Serial serial ) : base( serial )
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

	public class MagicWizardsHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }
        
		public void AddStatMods( Mobile m )
		{
			if ( m == null )
				return;
		}

		public void RemoveStatMods( Mobile m )
		{
			if ( m == null )
				return;

			string modName = this.Serial.ToString();

			m.RemoveStatMod( String.Format( "[Magic Hat] -Str {0}", modName ) );
			m.RemoveStatMod( String.Format( "[Magic Hat] -Dex {0}", modName ) );
			m.RemoveStatMod( String.Format( "[Magic Hat] +Int {0}", modName ) );
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			AddStatMods( parent as Mobile );
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			RemoveStatMods( parent as Mobile );
		}

		public override int BaseStrBonus{ get{ return -5; } }
		public override int BaseDexBonus{ get{ return -5; } }
		public override int BaseIntBonus{ get{ return +5; } }

		[Constructable]
		public MagicWizardsHat() : this( 0 )
		{
            Name = "a magical wizard's hat";
		}

		[Constructable]
		public MagicWizardsHat( int hue ) : base( 0x1718, hue )
		{
            Name = "a magical wizard's hat";
			Weight = 1.0;
		}

		public MagicWizardsHat( Serial serial ) : base( serial )
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

            //-----

			AddStatMods( Parent as Mobile );
		}
	}

	public class Bonnet : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public Bonnet() : this( 0 )
		{
            Name = "a bonnet";
		}

		[Constructable]
		public Bonnet( int hue ) : base( 0x1719, hue )
		{
            Name = "a bonnet";
			Weight = 1.0;
		}

		public Bonnet( Serial serial ) : base( serial )
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

	public class FeatheredHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public FeatheredHat() : this( 0 )
		{
            Name = "a feathered hat";
		}

		[Constructable]
		public FeatheredHat( int hue ) : base( 0x171A, hue )
		{
            Name = "a feathered hat";
			Weight = 1.0;
		}

		public FeatheredHat( Serial serial ) : base( serial )
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

	public class TricorneHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public TricorneHat() : this( 0 )
		{
            Name = "a tricorne hat";
		}

		[Constructable]
		public TricorneHat( int hue ) : base( 0x171B, hue )
		{
            Name = "a tricorne hat";
			Weight = 1.0;
		}

		public TricorneHat( Serial serial ) : base( serial )
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

	public class JesterHat : BaseHat
	{
        public static int GetSBPurchaseValue() { return 1; }
        public static int GetSBSellValue() { return Item.SBDetermineSellPrice(GetSBPurchaseValue()); }

		[Constructable]
		public JesterHat() : this( 0 )
		{
            Name = "a jester's hat";
		}

		[Constructable]
		public JesterHat( int hue ) : base( 0x171C, hue )
		{
            Name = "a jester's hat";
			Weight = 1.0;
		}

		public JesterHat( Serial serial ) : base( serial )
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