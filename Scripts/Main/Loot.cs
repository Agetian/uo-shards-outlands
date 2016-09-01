using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

using Server.Custom.Items;

namespace Server
{
    public class Loot
    {
        public static void GenerateLoot(BaseCreature creature)
        {
            double difficulty = creature.InitialDifficulty;

            #region High-Level Creatures
            
            if (creature.IsParagon)
            {
                creature.PackItem(new ArcaneScroll());
            }

            if (creature.Rare)
            {
                creature.PackItem(new ArcaneScroll());
            }

            if (creature.IsLoHBoss())
            {
                creature.PackItem(new ArcaneScroll());

                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: creature.PackItem(new SkillMasteryOrb()); break;
                    case 2: creature.PackItem(new SkillMasteryScroll()); break;
                    case 3: creature.PackItem(new AspectCore()); break;
                }
            }

            if (creature.IsChamp())
            {
                creature.PackItem(new ArcaneScroll());

                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: creature.PackItem(new SkillMasteryOrb()); break;
                    case 2: creature.PackItem(new SkillMasteryScroll()); break;
                    case 3: creature.PackItem(new AspectCore()); break;
                }
            }

            if (creature.IsBoss())
            {
                for (int a = 0; a < 5; a++)
                    creature.PackItem(new ArcaneScroll());

                creature.PackItem(new SkillMasteryOrb());
                creature.PackItem(new SkillMasteryScroll());
                creature.PackItem(new AspectCore());

                for (int a = 0; a < 2; a++)
                {
                    if (Utility.RandomDouble() <= .5)
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: creature.PackItem(new SkillMasteryOrb()); break;
                            case 2: creature.PackItem(new SkillMasteryScroll()); break;
                            case 3: creature.PackItem(new AspectCore()); break;
                        }
                    }
                }
            }

            if (creature.IsEventBoss())
            {
            }

            #endregion
        }

        #region Weapons

        public static Type[] ArcheryWeaponTypes = new Type[]
	    {
		    typeof( Bow ),
            typeof( Crossbow ),
            typeof( HeavyCrossbow ),
	    };

        public static Type[] FencingWeaponTypes = new Type[]
	    {
		    typeof( Kryss ),
            typeof( Pitchfork ),
            typeof( ShortSpear ),
            typeof( Spear ),
            typeof( WarFork ),
	    };

        public static Type[] MacingWeaponTypes = new Type[]
	    {
		    typeof( BlackStaff ),
            typeof( Club ),
            typeof( GnarledStaff ),
            typeof( HammerPick ),
            typeof( Mace ),
            typeof( Maul ),
            typeof( QuarterStaff ),
            //typeof( ShepherdsCrook ),
            typeof( WarAxe ),
            typeof( WarHammer ),
            typeof( WarMace ),
	    };

        public static Type[] SwordpWeaponTypes = new Type[]
	    {
		    typeof( Axe ),
            typeof( Bardiche ),
            typeof( BattleAxe ),
            typeof( Broadsword ),
            typeof( Cutlass ),
            typeof( DoubleAxe ),
            typeof( ExecutionersAxe ),
            typeof( Halberd ),
            typeof( Katana ),
            typeof( LargeBattleAxe ),
            typeof( Longsword ),
            typeof( Scimitar ),
            typeof( ThinLongsword ),
            typeof( TwoHandedAxe ),
            typeof( VikingSword ),
	    };

        #endregion

        #region Armor

        public static Type[] LeatherArmorTypes = new Type[]
	    {
		    typeof( FemaleLeatherChest ),
            typeof( LeatherArms ),
            typeof( LeatherBustier ),
            typeof( LeatherCap ),
            typeof( LeatherChest ),
            typeof( LeatherGloves ),
            typeof( LeatherGorget ),
            typeof( LeatherLegs ),
            typeof( LeatherShorts ),
            typeof( LeatherSkirt ),
	    };

        public static Type[] StuddedArmorTypes = new Type[]
	    {
		    typeof( FemaleStuddedChest ),
            typeof( StuddedArms ),
            typeof( StuddedBustier ),
            typeof( StuddedCap ),
            typeof( StuddedChest ),
            typeof( StuddedGloves ),
            typeof( StuddedGorget ),
            typeof( StuddedLegs ),
	    };

        public static Type[] BoneArmorTypes = new Type[]
	    {
		    typeof( BoneArms ),
            typeof( BoneChest ),
            typeof( BoneGloves ),
            typeof( BoneGorget ),
            typeof( BoneHelm ),
            typeof( BoneLegs )
	    };

        public static Type[] RingmailArmorTypes = new Type[]
	    {
		    typeof( RingmailArms ),
            typeof( RingmailChest ),
            typeof( RingmailGloves ),
            typeof( RingmailGorget ),
            typeof( RingmailHelm ),
            typeof( RingmailLegs )
	    };

        public static Type[] ChainmailArmorTypes = new Type[]
	    {
		    typeof( ChainmailArms ),
            typeof( ChainmailChest ),
            typeof( ChainmailGloves ),
            typeof( ChainmailGorget ),
            typeof( ChainmailCoif ),
            typeof( ChainmailLegs )
	    };

        public static Type[] PlatemailArmorTypes = new Type[]
	    {
            typeof( FemalePlateChest ),
		    typeof( PlateArms ),
            typeof( PlateChest ),
            typeof( PlateGloves ),
            typeof( PlateGorget ),
            typeof( PlateHelm ),
            typeof( PlateLegs ),

            typeof( Bascinet ),
            typeof( CloseHelm ),
            typeof( NorseHelm ),
	    };

        #endregion

        #region Shields

        public static Type[] ShieldTypes = new Type[]
	    {
		    typeof( BronzeShield ),
            typeof( Buckler ),
            typeof( HeaterShield ),
            typeof( MetalKiteShield ),
            typeof( MetalShield ),
            typeof( WoodenKiteShield ),
            typeof( WoodenShield ),
	    };

        #endregion

        #region Instruments

        public static Type[] InstrumentTypes = new Type[]
	    {
		    typeof( Drums ),
            typeof( Harp ),
            typeof( Lute ),
            typeof( Tambourine ),
	    };

        #endregion

        #region Spell Scrolls

        public static Type[] SpellScrollTypes = new Type[]
		{
			typeof( ClumsyScroll ),			typeof( CreateFoodScroll ),		typeof( FeeblemindScroll ),		typeof( HealScroll ),
			typeof( MagicArrowScroll ),		typeof( NightSightScroll ),		typeof( ReactiveArmorScroll ),	typeof( WeakenScroll ),
			typeof( AgilityScroll ),		typeof( CunningScroll ),		typeof( CureScroll ),			typeof( HarmScroll ),
			typeof( MagicTrapScroll ),		typeof( MagicUnTrapScroll ),	typeof( ProtectionScroll ),		typeof( StrengthScroll ),
			typeof( BlessScroll ),			typeof( FireballScroll ),		typeof( MagicLockScroll ),		typeof( PoisonScroll ),
			typeof( TelekinisisScroll ),	typeof( TeleportScroll ),		typeof( UnlockScroll ),			typeof( WallOfStoneScroll ),
			typeof( ArchCureScroll ),		typeof( ArchProtectionScroll ),	typeof( CurseScroll ),			typeof( FireFieldScroll ),
			typeof( GreaterHealScroll ),	typeof( LightningScroll ),		typeof( ManaDrainScroll ),		typeof( RecallScroll ),
			typeof( BladeSpiritsScroll ),	typeof( DispelFieldScroll ),	typeof( IncognitoScroll ),		typeof( MagicReflectScroll ),
			typeof( MindBlastScroll ),		typeof( ParalyzeScroll ),		typeof( PoisonFieldScroll ),	typeof( SummonCreatureScroll ),
			typeof( DispelScroll ),			typeof( EnergyBoltScroll ),		typeof( ExplosionScroll ),		typeof( InvisibilityScroll ),
			typeof( MarkScroll ),			typeof( MassCurseScroll ),		typeof( ParalyzeFieldScroll ),	typeof( RevealScroll ),
			typeof( ChainLightningScroll ), typeof( EnergyFieldScroll ),	typeof( FlamestrikeScroll ),	typeof( GateTravelScroll ),
			typeof( ManaVampireScroll ),	typeof( MassDispelScroll ),		typeof( MeteorSwarmScroll ),	typeof( PolymorphScroll ),
			typeof( EarthquakeScroll ),		typeof( EnergyVortexScroll ),	typeof( ResurrectionScroll ),	typeof( SummonAirElementalScroll ),
			typeof( SummonDaemonScroll ),	typeof( SummonEarthElementalScroll ),	typeof( SummonFireElementalScroll ),	typeof( SummonWaterElementalScroll )
		};

        #endregion

        #region Reagents

        public static Type[] ReagentTypes = new Type[]
		{
			typeof( BlackPearl ),	
            typeof( Bloodmoss ),	
            typeof( Garlic ),
			typeof( Ginseng ),		
            typeof( MandrakeRoot ),		
            typeof( Nightshade ),
			typeof( SulfurousAsh ),	
            typeof( SpidersSilk )
		};

        #endregion

        #region Potions

        public static Type[] PotionTypes = new Type[]
		{ 
            typeof( LesserHealPotion ),	
			typeof( LesserCurePotion ),	
            typeof( RefreshPotion ),
			typeof( AgilityPotion ),	
            typeof( StrengthPotion ),	            
            typeof( LesserPoisonPotion ),
            typeof( LesserExplosionPotion ),
            typeof( LesserMagicResistPotion ),
		};

        #endregion

        #region Gems

        public static Type[] GemTypes = new Type[]
		{
			typeof( Amber ),	
            typeof( Amethyst ),	
            typeof( Citrine ),
			typeof( Diamond ),		
            typeof( Emerald ),		
            typeof( Ruby ),
			typeof( Sapphire ),		
            typeof( StarSapphire ),	
            typeof( Tourmaline )
		};

        #endregion

        #region Clothing

        public static Type[] ClothingTypes = new Type[]
		{
			typeof( Cloak ),				
			typeof( Bonnet ),   
            typeof( Cap ),		     
            typeof( FeatheredHat ),
			typeof( FloppyHat ),      
            typeof( JesterHat ),		
            typeof( Surcoat ),
			typeof( SkullCap ),         
            typeof( StrawHat ),	        
            typeof( TallStrawHat ),
			typeof( TricorneHat ),		
            typeof( WideBrimHat ),      
            typeof( WizardsHat ),
			typeof( BodySash ),        
            typeof( Doublet ),        
            typeof( Boots ),
			typeof( FullApron ),          
            typeof( JesterSuit ),        
            typeof( Sandals ),
			typeof( Tunic ),			
            typeof( Shoes ),			
            typeof( Shirt ),
			typeof( Kilt ),              
            typeof( Skirt ),			
            typeof( FancyShirt ),
			typeof( FancyDress ),		
            typeof( ThighBoots ),		
            typeof( LongPants ),
			typeof( PlainDress ),       
            typeof( Robe ),				
            typeof( ShortPants ),
			typeof( HalfApron )
		};

        public static Type[] HatTypes = new Type[]
		{
			typeof( SkullCap ),			
            typeof( Bandana ),	
            typeof( FloppyHat ),
			typeof( Cap ),				
            typeof( WideBrimHat ),
            typeof( StrawHat ),
			typeof( TallStrawHat ),	
            typeof( WizardsHat ),
            typeof( Bonnet ),
			typeof( FeatheredHat ),	
            typeof( TricorneHat ),
            typeof( JesterHat )
		};

        #endregion
    }    
}