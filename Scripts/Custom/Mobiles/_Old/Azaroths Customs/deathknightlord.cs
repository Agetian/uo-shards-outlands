using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
	public class DeathKnightLord : BaseCreature 
	{ 
		[Constructable] 
		public DeathKnightLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 
			SpeechHue = Utility.RandomDyedHue(); 
			Title = "the death knight lord"; 
			Hue = 1;

			if ( this.Female = Utility.RandomBool() ) 
			{ 
				this.Body = 0x191; 
				this.Name = NameList.RandomName( "female" ); 
				AddItem( new Skirt( Utility.RandomRedHue() ) ); 
			} 
			else 
			{ 
				this.Body = 0x190; 
				this.Name = NameList.RandomName( "male" ); 
				AddItem( new FancyShirt( Utility.RandomRedHue() ) ); 
			} 

			SetStr( 456, 520 );
			SetDex( 100, 160 );
			SetInt( 161, 175 );

			SetHits( 475, 500 );

			SetDamage( 8, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.Anatomy, 125.0 );
			SetSkill( SkillName.Fencing, 46.0, 77.5 );
			SetSkill( SkillName.Macing, 35.0, 57.5 );
			SetSkill( SkillName.Poisoning, 60.0, 82.5 );
			SetSkill( SkillName.MagicResist, 110.5, 145.5 );
			SetSkill( SkillName.Swords, 125.0 );
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.Lumberjacking, 125.0 );

			Fame = 25000;
			Karma = -25000;

			VirtualArmor = 40;
			
			PlateArms arms = new PlateArms();
			arms.Hue = 0x966;
			arms.LootType = LootType.Regular;
			AddItem( arms );
			
			PlateLegs legs = new PlateLegs();
			legs.Hue = 0x966;
			legs.LootType = LootType.Regular;
			AddItem( legs );

			PlateGorget gorget = new PlateGorget();
			gorget.Hue = 0x966;
			gorget.LootType = LootType.Regular;
			AddItem( gorget );

			PlateChest tunic = new PlateChest();
			tunic.Hue = 0x966;
			tunic.LootType = LootType.Regular;
			AddItem( tunic );

			PlateGloves gloves = new PlateGloves();
			gloves.Hue = 0x966;
			gloves.LootType = LootType.Regular;
			AddItem( gloves );

			VikingSword weapon = new VikingSword();

			weapon.Movable = true;

			AddItem( weapon );

			Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) ); 
			hair.Hue = Utility.RandomNondyedHue(); 
			hair.Layer = Layer.Hair; 
			hair.Movable = false; 
			AddItem( hair ); 


		} 

    //public override int GoldWorth { get { return Utility.RandomMinMax(250, 400); } }

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		//public override int TreasureMapLevel{ get{ return 2; } }

		public DeathKnightLord( Serial serial ) : base( serial ) 
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