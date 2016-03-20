using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Custom;

namespace Server
{
    public class AICreatureList
    {
        private AIGroup m_AIGroup;
        private AISubgroup m_AISubgroup;
        private Type[] m_Types;

        public AIGroup Group
        {
            get { return m_AIGroup; }
            set { m_AIGroup = value; }
        }

        public AISubgroup Subgroup
        {
            get { return m_AISubgroup; }
            set { m_AISubgroup = value; }
        }

        public Type[] Types
        {
            get { return m_Types; }
            set { m_Types = value; }
        }

        public AICreatureList(AIGroup group, AISubgroup subgroup, Type[] types)
        {
            m_AIGroup = group;
            m_AISubgroup = subgroup;
            m_Types = types;
        }

        public static bool Contains(object obj)
        {
            if (m_Table == null)
                LoadTable();

            AICreatureList sp = (AICreatureList)m_Table[obj.GetType()];

            return (sp != null);
        }

        public static bool GetAI(object obj)
        {
            if (m_Table == null)
                LoadTable();

            AICreatureList sp = (AICreatureList)m_Table[obj.GetType()];
            BaseCreature creature = obj as BaseCreature;

            if (creature != null)
            {
                if (sp != null)
                {
                    creature.Group = sp.Group;
                    creature.Subgroup = sp.Subgroup;
                }

                else
                {
                    creature.Group = AIGroup.NeutralHuman;
                    creature.Subgroup = AISubgroup.None;
                }
            }

            else
            {
                return false;
            }

            return true;
        }

        private static void LoadTable()
        {
            m_Table = new Hashtable();

            for (int i = 0; i < m_AIList.Length; ++i)
            {
                AICreatureList info = m_AIList[i];
                Type[] types = info.Types;

                for (int j = 0; j < types.Length; ++j)
                    m_Table[types[j]] = info;
            }
        }

        private static Hashtable m_Table;

        private static AICreatureList[] m_AIList = new AICreatureList[]
        {
            new AICreatureList( AIGroup.None, AISubgroup.None, new Type[]
            {
            }),          

            //Melee
            new AICreatureList( AIGroup.EvilMonster, AISubgroup.None, new Type[]
            {
                typeof( EarthOreling ),
                typeof( RockOreling ),
                typeof( RockElemental ),
                typeof( Minotaur ),
                typeof( MinotaurCaptain ),
                typeof( GreaterSlime ),
                typeof( PoisonousSlime ),
                typeof( CorrosiveSlime ),
                typeof( CorruptedSoul ),
                typeof( FeralTreefellow ),
                typeof( PestilentBandage ),
                typeof( TormentedMinotaur ),
                typeof( Troglodyte ),
                typeof( Satyr ),
                typeof( BlackOrc ),
                typeof( GreaterMongbat ),
                typeof( SlimeTroll ),
                typeof( AncientSlime ),
                typeof( VoidSlime ),
                typeof( EnergyLlama ),
                typeof( HarrowerTentacles ),
                typeof( CapturedHordeMinion ),
                typeof( AntLion ),
                typeof( ArcticOgreLord ),
                typeof( BogThing ),
                typeof( EarthElemental ),
                typeof( Ettin ),
                typeof( FrostOoze ),
                typeof( FrostTroll ),
                typeof( GazerLarva ),
                typeof( Golem ),
                typeof( HeadlessOne ),
                typeof( Jwilson ),
                typeof( Juggernaut ),
                typeof( Ogre ),
                typeof( OgreLord ),
                typeof( PlagueBeast ),
                typeof( Quagmire ),
                typeof( Slime ),
                typeof( Troll ),
                typeof( AgapiteElemental ),
                typeof( BronzeElemental ),
                typeof( CopperElemental ),
                typeof( DullCopperElemental ),
                typeof( GoldenElemental ),
                typeof( ValoriteElemental ),
                typeof( VeriteElemental ),
                typeof( LuniteElemental ),
                typeof( PlagueSpawn ),
                typeof( ShadowIronElemental ),
                typeof( IceElemental ),
                typeof( ElderIceElemental ),
                typeof( MoundOfMaggots ),
                typeof( Cyclops ),
                typeof( GreaterCyclops ),
                typeof( Bogling ),
                typeof( BlackSolenInfiltratorQueen ),
                typeof( BlackSolenInfiltratorWarrior ),
                typeof( BlackSolenQueen ),
                typeof( BlackSolenWarrior ),
                typeof( BlackSolenWorker ),
                typeof( Centaur ),
                typeof( ChaosDaemon ),
                typeof( DeepSeaSerpent ),
                typeof( Drake ),
                typeof( DrakeWhelp),
                typeof( FrostSpider ),
                typeof( GiantSpider ),
                typeof( Harpy ),
                typeof( HordeMinion ),
                typeof( Kraken ),
                typeof( Lizardman ),
                typeof( Mongbat ),
                typeof( StrongMongbat ),
                typeof( Orc ),
                typeof( OrcBomber ),
                typeof( OrcBrute ),
                typeof( ArmoredTitan ),
                typeof( OrcCaptain ),
                typeof( OrcishLord ),
                typeof( FrostOrcLord ),
                typeof( FrostOrc ),
                typeof( Ratman ),
                typeof( RedSolenInfiltratorQueen ),
                typeof( RedSolenInfiltratorWarrior ),
                typeof( RedSolenQueen ),
                typeof( RedSolenWarrior ),
                typeof( RedSolenWorker ),
                typeof( SeaSerpent ),
                typeof( StoneGargoyle ),
                typeof( StoneHarpy ),
                typeof( SwampDragon ),
                typeof( ScaledSwampDragon ),
                typeof( EtherealSwampDragon ),
                typeof( SwampTentacle ),
                typeof( TerathanDrone ),
                typeof( TerathanWarrior ),
                typeof( WhippingVine ),
                typeof( Wyvern ),
                typeof( EnslavedGargoyle ),
                typeof( ExodusMinion ),
                typeof( ExodusOverseer ),
                typeof( OphidianWarrior ),
                typeof( OphidianKnight ),
                typeof( SandVortex ),
                typeof( MeerWarrior ),
                typeof( MeerCaptain ),
                typeof( JukaLord ),
                typeof( JukaWarrior ),
                typeof( GiantBlackWidow ),
                typeof( FleshGolem ),
                typeof( Gibberling ),
                typeof( Ravager ),
                typeof( Treefellow ),
                typeof( WailingBanshee ),
                typeof( Serado ),
                typeof( Yamandon ),
                typeof( Moloch ),
                typeof( RaiJu ),
                typeof( Impaler ),
                typeof( GoreFiend ),
                typeof( SkitteringHopper ),
                typeof( Kappa ),
                typeof( YomotsuElder ),
                typeof( YomotsuWarrior ),
                typeof( FleshRenderer ),
                typeof( FireBeetle ),
                typeof( FireSteed ),
                typeof( Mephitis ),
                typeof( Rikktor ),
                typeof( VorpalBunny ),
                typeof( TsukiWolf ),
                typeof( Meraktus ),
                typeof( Twaulo ),
                typeof( ServantOfSemidar ),
                typeof( DespiseOrc ),
                typeof( ForestSpider ),
                typeof( RockSpider ),
                typeof( Sphinx ),
                typeof( MysterySphinx ),
                typeof( WyvernHatchling ),
                typeof( Umberhulk ),
                typeof( Myconid ),
                typeof( MyconidTallstalk ),
                typeof( NightbarkTree ),
                typeof( Glowvines ),
                typeof( Maggot ),
                typeof( Entrail ),
                typeof( DiseasedViscera ),
                typeof( GiantRotworm ),
                typeof( RotwormLarva ),
                typeof( AncientHellhound ),
                typeof( ArmoredCharger ),
                typeof( Bloodworm ),
                typeof( GiantDemon ),
                typeof( GreaterPhoenix ),
                typeof( HugeDragon ),
                typeof( Hydra ),
                typeof( PrimalLich ),
                typeof( RisingColossus ),
                typeof( Rotworm ),
                typeof( StoneGolem ),
                typeof( Turkey ),
                typeof( CrystalElemental ),
                typeof( Phoenix ),
                typeof( OrcishExecutioner ),
                typeof( HookHorror ),
                typeof( DemonwebQueen),
                typeof( EmperorDragon),
                typeof( TheDeepOne),
                typeof( DeepTentacle),
                typeof( DeepCrab),
                typeof( AbysmalHorror),
                typeof( DemonKnight ),
                typeof( Basilisk ),
                typeof( Corpser ),
                typeof( EarthlyTendril ),
                typeof( PitTentacle ),
                typeof( TreeStalker ),
                typeof( ArborealMyconid ),
                typeof( Ent ),
                typeof( OtherworldlyDenizon ),
                typeof( DeDOSBotNetZombie),
                typeof( DeDOSBot),
                typeof( DeDOSLargeBot),
                typeof( DeDOSMassiveBot),
                typeof( DeDOSNetGremlin),
                typeof( DeDOSTunneler ),
                typeof( DeDOSTrojanHorse),
                typeof( DeDOSKynDragon),
                typeof( Lodestone ),
                typeof( RockGuar ),
                typeof( Sanguineous ),
                typeof( BloodCourser),
                typeof( BloodSlime ),                
            } ),

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MeleeMage1, new Type[]
            {
                typeof( SnowElemental ),
                typeof( Bogle ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MeleeMage2, new Type[]
            {
                typeof( AcidElemental ),
                typeof( ToxicElemental ),
                typeof( DreadSpider ),
                typeof( Efreet ),
                typeof( Dragon ),
                typeof( GrayTroll ),
                typeof( MongbatLord ),
                typeof( DragonWhelp ),
                typeof( Puddle ),
                typeof( DeepWater ),
                typeof( Gust ),
                typeof( TerathanAvenger ),
                typeof( SnowElemental ),
                typeof( ChromaticDragon ),
                typeof( DriderSentinel ),
                typeof( DriderWarrior ),
                typeof( Nightmare ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MeleeMage3, new Type[]
            {
                typeof( PoisonElemental ),
                typeof( BloodElemental ),
                typeof( Daemon ),
                typeof( FireElemental ),
                typeof( Gargoyle ),
                typeof( IceFiend ),
                typeof( WaterElemental ),
                typeof( ArcaneDaemon ),
                typeof( GargoyleDestroyer ),
                typeof( GargoyleEnforcer ),
                typeof( RuneBeetle ),                
                typeof( AncientWinterWyrm ),
                typeof( AncientWyrm ),
                typeof( KazeKemono ),
                typeof( YomotsuPriest ),
                typeof( Oni ),
                typeof( FetidEssence ),
                typeof( LadyOfTheSnow ),
                typeof( BloodTroll ),
                typeof( DriderHarbinger ),
                typeof( MindFlayer ),
                typeof( Betrayer ),
                typeof( Succubus ),
                typeof( AbyssDragon ),
                typeof( ShadowWyrm ),
                typeof( ShadowDragon ),
                typeof( ShadowDrake ),
                typeof( ElderAcidElemental ),
                typeof( AirElemental ),
                typeof( OgreMage ),
                typeof( Devourer ),
                typeof( ElderFireElemental ),
                typeof( ElderWaterElemental ),
                typeof( ElderAirElemental ),
                typeof( LavaElemental ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MeleeMage4, new Type[]
            {               
                typeof( GreaterDragon ),
                typeof( FireGargoyle ),
                typeof( SerpentineDragon ),
                typeof( TerathanMatriarch ),
                typeof( OphidianMatriarch ),
                typeof( Leviathan ),
                typeof( Balron ),
                typeof( MysteryDaemon ),
                typeof( Harrower ),
                typeof( Semidar ),
                typeof( Ilhenir ),
                typeof( FountainOfEvil ),
                typeof( PitBeast ),
                typeof( ElderBloodElemental ),
                typeof( ElderPoisonElemental ),
                typeof( IceFiendLord ),
                typeof( ArcaneDragon ),
                typeof( ArcaneDrake ),
                typeof( SilverDaemon ),
                typeof( Titan ),
                typeof( ElderToxicElemental ),
                typeof( DarkSentinel ),  
                typeof( Peradun),
                typeof( DeDOS ),
                typeof( WhiteWyrm ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MeleeMage5, new Type[]
            {
                typeof( GoldenBalron ),
                typeof( AncientRedWyrm ),
                typeof( SilverDaemonLord ),
                typeof( AncientFlame),
                typeof( ShadowOfPeradun),
                typeof( EternalFlameWyrm ),
                typeof( HydraKing),
                typeof( DaemonicOverlord ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MeleeMage6, new Type[]
            {
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Mage1, new Type[]
            {
                typeof( Gauth ),
                typeof( Imp ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Mage2, new Type[]
            {
                typeof( OrcishMage ),
                typeof( FrostOrcMage ),
                typeof( RatmanMage ),
                typeof( MeerMage ),
                typeof( Gazer ),
                typeof( Beholder ),
                typeof( WailingBanshee ),
                typeof( MysteryMongbat ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Mage3, new Type[]
            {
                typeof( OphidianMage ),
                typeof( MeerEternal ),
                typeof( JukaMage ),
                typeof( GolemController ),
                typeof( Aboleth ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Mage4, new Type[]
            {
                typeof( OphidianArchmage ),
                typeof( ElderGazer ),
                typeof( EvilOak ),
                typeof( WandererOfTheVoid ),
                typeof( DarkWisp ),
                typeof( Reaper ),
                typeof( FrostwoodReaper ),
                typeof( WildOne ),                
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Mage5, new Type[]
            {
                 typeof( MercuryGazer ),                 
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Mage6, new Type[]
            {
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.GroupHealerMage1, new Type[]
            {

            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.GroupHealerMage2, new Type[]
            {
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.GroupHealerMage3, new Type[]
            {
                typeof( CyclopsShaman ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.GroupHealerMage5, new Type[]
            {
                typeof( TreeOfLife ),
            } ) ,

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.GroupMedicMelee, new Type[]
            {
                typeof( OrcishSurjin ),
                typeof( KuoToa ),
            }),

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.MageKiller, new Type[]
            {
            }),

            new AICreatureList(AIGroup.EvilMonster, AISubgroup.Duelist, new Type[]
            {
                typeof( Deathspinner),
                typeof( CorruptSpiderling),
            }),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.Swarm, new Type[]
            {
                typeof( Locust ),
                typeof( ColossusTermite ),
            } ),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.Stealther, new Type[]
            {
                typeof( ShadowFiend ),
                typeof( DarknightCreeper ),
                typeof( ShadowKnight ),
                typeof( BloodStalker )
            } ),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.Alchemist, new Type[] {
            } ),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.MeleePotion, new Type[]
            {
                typeof( OrcishGrunt ),
                typeof( OrcishPeon ),
                typeof( OrcWarboss ),
            } ),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.Scout, new Type[]
            {
                typeof( OrcishScout ),
                typeof( GargoyleEnforcer ),
                typeof( MinotaurScout ),
            } ),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.Hunter, new Type[]
            {
                typeof( OrcishMaurk ),
            } ),

            new AICreatureList( AIGroup.EvilMonster, AISubgroup.Ranged, new Type[]
            {
                typeof( RatmanArcher ),
                typeof( Valkyrie),
                typeof( MysteryMedusa),
            } ),

            new AICreatureList(AIGroup.EvilAnimal, AISubgroup.None, new Type[]
            {
                typeof( BaseEnraged ),
                typeof( EnragedRabbit ),
                typeof( EnragedHart ),
                typeof( EnragedHind ),
                typeof( EnragedBlackBear ),
                typeof( EnragedEagle ),
                typeof( IceSerpent ),
                typeof( GiantSerpent ),
                typeof( IceSnake ),
                typeof( Snake ),
                typeof( LavaSerpent ),
                typeof( LavaSnake ),
                typeof( Scorpion ),
                typeof( ChromaticCrawler ),
                typeof( SkeletalMount ),
                typeof( SilverSerpent ),
                typeof( VampireBat ),
                typeof( BakeKitsune ),
                typeof( DeathwatchBeetle ),
                typeof( DeathwatchBeetleHatchling ),
                typeof( BurrowBeetle ),
                typeof( Cockatrice ),
                typeof( CoralSnake ),
                typeof( GiantCoralSnake ),
                typeof( GiantBat ),
                typeof( VampireBat ),
                typeof( ScorpionHatchling ),
                typeof( PlagueRat ),

                typeof( EasterBunny ),
            } ),

            new AICreatureList(AIGroup.EvilAnimal, AISubgroup.SuperPredator, new Type[]
            {
            } ),

            new AICreatureList(AIGroup.EvilAnimal, AISubgroup.Predator, new Type[]
            {
                typeof( RagingGrizzlyBear ),
                typeof( DireWolf ),
                typeof( LavaLizard ),
                typeof( IceLizard ),
                typeof( GreaterLizard ),
                typeof( PredatorHellCat ),
                typeof( FrenziedOstard ),
                typeof( HellHound ),
                typeof( HellCat ),
                typeof( Alligator ),
                typeof( Bullvore ),
                typeof( BayingHound ),
                typeof( Salamander ),
                typeof( SwampCrawler ),
                typeof( IceSkitter ),
                typeof( Skitter ),
                typeof( WhiteWolf ),
                typeof( Deepstalker ),
                typeof( Bloodcat ),
                typeof( Sabertusk ),
            } ),

            new AICreatureList(AIGroup.EvilAnimal, AISubgroup.Prey, new Type[]
            {
                typeof( GiantRat ),
            } ),

            new AICreatureList( AIGroup.NeutralAnimal, AISubgroup.None, new Type[]
            {
                typeof( UnholyFamiliar ),
                typeof( UnholySteed ),
                typeof( HolySteed ),
                typeof( HolyFamiliar ),
                typeof( Beetle ),
                typeof( Walrus ),
                typeof( Bull ),
                typeof( Bison ),
                typeof( GiantToad ),
                typeof( Dolphin ),
                typeof( DesertOstard ),
                typeof( ForestOstard ),
                typeof( Horse ),
                typeof( PackHorse ),
                typeof( PackLlama ),
                typeof( RidableLlama ),
                typeof( Ridgeback ),
                typeof( SilverSteed ),
                typeof( SavageRidgeback ),
                typeof( Unicorn ),
                typeof( LesserHiryu ),
                typeof( Hiryu ),
                typeof( Kirin ),
                typeof( BlackBear ),
                typeof( BrownBear ),
                typeof( Gorilla ),
                typeof( Gaman ),
                typeof( CuSidhe ),
                typeof( ArmoredCrab ),
                typeof( Crab ),
                typeof( Guar ),
            } ),

            new AICreatureList( AIGroup.NeutralAnimal, AISubgroup.SuperPredator, new Type[]
            {
            } ),

            new AICreatureList( AIGroup.NeutralAnimal, AISubgroup.Predator, new Type[]
            {
                typeof( Cougar ),
                typeof( GreyWolf ),
                typeof( Panther ),
                typeof( TimberWolf ),
                typeof( SnowLeopard ),
                typeof( GrizzlyBear ),
                typeof( PolarBear ),
            } ),

            new AICreatureList( AIGroup.NeutralAnimal, AISubgroup.Prey, new Type[]
            {
                typeof( Sewerrat ),
                typeof( Rat ),
                typeof( Parrot ),
                typeof( Chicken ),
                typeof( Cow ),
                typeof( Boar ),
                typeof( Goat ),
                typeof( Cat ),
                typeof( Dog ),
                typeof( GreatHart ),
                typeof( Caribou ),
                typeof( Hind ),
                typeof( Rabbit ),
                typeof( Crane ),
                typeof( Bird ),
                typeof( Pig ),
                typeof( Sheep ),
                typeof( MountainGoat ),
                typeof( Llama ),
                typeof( JackRabbit ),
                typeof( Ferret ),
                typeof( Fox ),
                typeof( BullFrog ),
            } ),

            new AICreatureList( AIGroup.GoodMonster, AISubgroup.None, new Type[]
            {
            } ),

            new AICreatureList( AIGroup.GoodMonster, AISubgroup.Mage3, new Type[]
            {
                typeof( MLDryad ),
            } ),

            new AICreatureList( AIGroup.GoodHuman, AISubgroup.None, new Type[]
            {                
            } ),

            new AICreatureList( AIGroup.GoodHuman, AISubgroup.Sailor, new Type[]
            {
                typeof( Custom.Pirates.BritainMarine ),
                typeof( Custom.Pirates.BritainSailor ),
                typeof( Custom.Pirates.BritainShipCarpenter ),
            } ),

            new AICreatureList( AIGroup.GoodHuman, AISubgroup.GroupMedicRanged, new Type[]
            {
                typeof( Custom.Pirates.BritainShipSurgeon ),
            } ),

            new AICreatureList( AIGroup.GoodHuman, AISubgroup.ShipCaptain, new Type[]
            {
                typeof( Custom.Pirates.BritainShipCaptain ),
            } ),

            new AICreatureList( AIGroup.GoodHuman, AISubgroup.BountyHunter, new Type[]
            {               
            } ),

            new AICreatureList( AIGroup.NeutralHuman, AISubgroup.None, new Type[]
            {
                typeof( BaseEscortable ),
                typeof( TalkingBaseEscortable ),
                typeof( SeekerOfAdventure ),
                typeof( Peasant ),
                typeof( Noble ),
                typeof( Messenger ),
                typeof( Merchant ),
                typeof( EscortableMage ),
                typeof( BrideGroom ),
                typeof( BaseCannonGuard ),
                typeof( DoubloonDockGuard ),
                typeof( TurnableCannonGuard ),
                typeof( Dockmaster ),
                typeof( Custom.Pirates.OceanFisherman ),

                typeof( Custom.HenchmanPirate ),
                typeof( Custom.HenchmanRaider ),
                typeof( Custom.HenchmanPirateBoatswain ),
                typeof( Custom.HenchmanPirateFirstMate ),
                typeof( Custom.HenchmanSailor ),
                typeof( Custom.HenchmanMarine ),
                typeof( Custom.HenchmanNavyBoatswain ),
                typeof( Custom.HenchmanNavyFirstMate ),
                typeof( Custom.HenchmanSquire ),
                typeof( Custom.HenchmanKnight ),
                typeof( Custom.HenchmanPaladin ),
                typeof( Custom.HenchmanCrusader ),
                typeof( Custom.HenchmanBandit ),
                typeof( Custom.HenchmanMercenary ),
                typeof( Custom.HenchmanAssassin ),
                typeof( Custom.HenchmanShadowblade ),
                typeof( Custom.HenchmanPirateCarpenter ),
                typeof( Custom.HenchmanNavyCarpenter ),                
            } ),

            new AICreatureList(AIGroup.FactionHuman, AISubgroup.None, new Type[]
            {
                typeof( ArcherGuard ),
                typeof( WarriorGuard ),
            } ) ,

            new AICreatureList(AIGroup.FactionAnimal, AISubgroup.None, new Type[]
            {
                typeof( TBWarHorse ),
                typeof( CoMWarHorse ),
                typeof( MinaxWarHorse ),
                typeof( SLWarHorse ),
            }),

            new AICreatureList( AIGroup.Summoned, AISubgroup.None, new Type[]
            {
                typeof( SummonedEarthElemental ),
            }),

            new AICreatureList( AIGroup.Undead, AISubgroup.None, new Type[]
            {
                typeof( Zombie ),
                typeof( Mummy ),
                typeof( RottingCorpse ),
                typeof( InterredGrizzle),
                typeof( UndeadKnight ),
                typeof( BoneKnight ),
                typeof( Ghoul ),
                typeof( Skeleton ),
                typeof( RestlessSoul ),
                typeof( KhaldunRevenant ),
                typeof( SpectralArmour ),
                typeof( SkeletalKnight ),
                typeof( Cursed ),
                typeof( SpectralArmour ),
                typeof( Revenant ),
                typeof( PatchworkSkeleton ),
                typeof( EtherealWarrior ),
                typeof( SkeletalDrake ),
                typeof( RisenKnight ),
                typeof( RisenHonorGuard ),
                typeof( DecayedZombie ),
                typeof( UndeadOgreLord ),
                typeof( BloodyZombie ),
                typeof( FlamingZombie ),
                typeof( CorruptReaver ),
                typeof( BoneDemon ),
                typeof( LordOfBones ),
                typeof( BloodyReaver ),
                typeof( IchibodShame ),
                typeof( GreatGobbler ),
                typeof( LesserGobbler ),
            } ),

            new AICreatureList(AIGroup.Undead, AISubgroup.MeleeMage1, new Type[]
            {
                typeof( Shade ),
                typeof( Wraith ),
                typeof( ZombieMagi ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.MeleeMage2, new Type[]
            {
                typeof( SkeletalDragon ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.MeleeMage3, new Type[]
            {
                typeof( RevenantLion ),
                typeof( CorruptWarmage ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.MeleeMage4, new Type[]
            {
                typeof( MysteryVampire ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.MeleeMage5, new Type[]
            {
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.MeleeMage6, new Type[]
            {
                typeof( TheCreeper ),
                typeof( KhaldunLichAnshu ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Mage1, new Type[]
            {
                typeof( Spectre ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Mage2, new Type[]
            {
                typeof( BoneMagi ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Mage3, new Type[]
            {
                typeof( Lich ),
                typeof( CorruptRunecaster ),
                typeof( SkeletalMage ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Mage4, new Type[]
            {
                 typeof( LichLord ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Mage5, new Type[]
            {
                typeof( AncientLich ),
                typeof( MagusLich ),
                typeof( PrimalLich ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Mage6, new Type[]
            {
                typeof( AncientNecromancer ),
                typeof( KhaldunLichAlmonjin ),
                typeof( KhaldunLichKaltivel ),
                typeof( KhaldunLichBaratoz ),
                typeof( KhaldunLichMaliel ),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.GroupHealerMeleeMage4, new Type[]
            {
                typeof(RisenNoble),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.GroupHealerMage4, new Type[]
            {
                typeof(Custom.Pirates.GhostShipNecromancer),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.GroupHealerMeleeMage5, new Type[]
            {
                typeof(RisenRoyal),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.AntiArmor, new Type[]
            {
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Scout, new Type[] {
                typeof(Creeper),
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.MageKiller, new Type[]
            {
            } ) ,

            new AICreatureList(AIGroup.Undead, AISubgroup.Assassin, new Type[] {
                typeof( HollowOne ),
            } ),

            new AICreatureList(AIGroup.Undead, AISubgroup.Sailor, new Type[] {
                typeof( Custom.Pirates.SkeletalCrewman ),
            } ),

            new AICreatureList(AIGroup.Undead, AISubgroup.ShipCaptain, new Type[] {
                typeof( Custom.Pirates.SkeletalCaptain ),
            } ),

            new AICreatureList( AIGroup.NeutralMonster, AISubgroup.None, new Type[]
            {
            } ),

            new AICreatureList( AIGroup.NeutralMonster, AISubgroup.Mage4, new Type[]
            {
                typeof( ShadowWisp ),
                typeof( Wisp ),
                typeof( Custom.HenchmanBoneMagi ),
                typeof( Custom.HenchmanLich ),
                typeof( Custom.HenchmanMummy ),
                typeof( Custom.HenchmanRottingCorpse ),
                typeof( Custom.HenchmanSkeletalKnight ),
                typeof( Custom.HenchmanSkeleton ),
                typeof( Custom.HenchmanSpectre ),
                typeof( Custom.HenchmanVampireCountess ),
                typeof( Custom.HenchmanVampireThrall ),
                typeof( Custom.HenchmanZombie ),
            } ),

            new AICreatureList( AIGroup.NeutralMonster, AISubgroup.GroupHealerMage5, new Type[]
            {
            } ),

            new AICreatureList( AIGroup.GoodMonster, AISubgroup.MeleeMage3, new Type[]
            {
                typeof( Pixie ),
            } ),

            new AICreatureList( AIGroup.GoodMonster, AISubgroup.MeleeMage4, new Type[]
            {
                typeof( Silvani ),
            } ),

            new AICreatureList( AIGroup.GoodMonster, AISubgroup.MeleeMage5, new Type[]
            {
                typeof( LordOaks ),
            } ),

            new AICreatureList(AIGroup.FactionMonster, AISubgroup.MeleeMage3, new Type[]
            {
            }),

            new AICreatureList( AIGroup.FactionHuman, AISubgroup.MeleeMage4, new Type[]
            {
            } ) ,

            new AICreatureList( AIGroup.FactionHuman, AISubgroup.MeleeMage5, new Type[]
            {
            } ) ,

            new AICreatureList( AIGroup.Summoned, AISubgroup.MeleeMage1, new Type[]
            {
                typeof( SummonedDaemon )
            }),

            new AICreatureList( AIGroup.Summoned, AISubgroup.MeleeMage2, new Type[]
            {
                typeof( SummonedAirElemental ),
                typeof( SummonedWaterElemental ),
            }),

            new AICreatureList( AIGroup.Summoned, AISubgroup.MeleeMage3, new Type[]
            {
                typeof( SummonedFireElemental ),
            }),

            new AICreatureList( AIGroup.Summoned, AISubgroup.Berserk, new Type[]
            {
                typeof( BladeSpirits ),
                typeof( EnergyVortex ),
            }),

            new AICreatureList( AIGroup.NeutralHuman, AISubgroup.WanderingHealer, new Type[]
            {
                typeof( Healer ),
                typeof( PricedHealer ),
                typeof( FortuneTeller ),
                typeof( EvilHealer ),
                typeof( EvilWanderingHealer ),
                typeof( WanderingHealer ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.None, new Type[]
            {
                typeof( Executioner ),
                typeof( Guardian ),
                typeof( KhaldunZealot ),
                typeof( Doppleganger ),
                typeof( SavageRider ),
                typeof( EliteNinja ),
                typeof( FanDancer ),
                typeof( GrimmochDrummel ),
                typeof( MorgBergen ),
                typeof( TavaraSewel ),
                typeof( Ronin ),
                typeof( Barracoon ),
                typeof( Custom.Pirates.PirateCaptain ),
                typeof( Custom.Pirates.Pirate ),
                typeof( Savage ),
                typeof( Brigand ),
                typeof( Thug ) ,
                typeof( SanguinDefender ),
                typeof( OrghereimShieldMaiden),
                typeof( OrghereimBowMaiden ),
                typeof( SanguinKnight ),
                typeof( AtlanteanWarden),

            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Sailor, new Type[]
            {
                typeof( Custom.Pirates.OceanPirate ),
                typeof( Custom.Pirates.PirateShipCarpenter ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupMedicRanged, new Type[]
            {
                typeof( Custom.Pirates.PirateSawbones ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.ShipCaptain, new Type[]
            {
                typeof( Custom.Pirates.OceanPirateCaptain ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MeleeMage1, new Type[]
            {
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MeleeMage2, new Type[]
            {
               typeof( DrowBlackguard ),
               typeof( DrowBlademaster ),
               typeof( WoodlandSprite),
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MeleeMage3, new Type[]
            {
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MeleeMage4, new Type[]
            {
               typeof( OrghereimCrone),
               typeof( AtlanteanBattleMage ),
               typeof( SanguinArchblade ),
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MeleeMage5, new Type[]
            {
               typeof( DrowKeeper ),
               typeof( Atraxis ),
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Mage1, new Type[]
            {
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Mage2, new Type[]
            {
            }),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Mage3, new Type[]
            {
                typeof( SanguinMage ),
                typeof( OrghereimSage),
                typeof( EvilMage ),
                typeof( OrcMojoka ),
                typeof( FlameElementalist ),
                typeof( EnergyElementalist ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Mage4, new Type[]
            {
                typeof( SanguinWizard ),
                typeof( KhaldunSummoner ),
                typeof( EvilMageLord ),
                typeof( LysanderGathenwale ),
                typeof( Neira ),
                typeof( ElderMojoka ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMeleeMage3, new Type[]
            {
                typeof( SanguinMender ),
                typeof( SavageShaman ),
                typeof( ElderWoodlandSprite),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMeleeMage4, new Type[]
            {
               typeof( DrowSpellsinger ),
               typeof( TombRaider ),

            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMeleeMage5, new Type[]
            {
                typeof( ElderWardr ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMage1, new Type[]
            {
                typeof( SvirfneblinIllusionist ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMage2, new Type[]
            {
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMage3, new Type[]
            {
                typeof( ElementalSeer ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMage4, new Type[]
            {
                typeof( SanguinHealer ),
                typeof( OrghereimBoneMender),
                typeof( WoodlandDruid ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMage5, new Type[]
            {
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupHealerMage6, new Type[]
            {
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.GroupMedicMelee, new Type[]
            {
                typeof( SanguinMedic ),
                typeof( OrghereimShieldMother),
                typeof( OrghereimBeastmaster),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Thief, new Type[]
            {
                typeof( SvirfneblinRogue ),                
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Hunter, new Type[]
            {
                typeof( SanguinHunter ),
                typeof( OrghereimIceCarl),
                typeof( Poacher),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Alchemist, new Type[]
            {
                typeof( SanguinAlchemist ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Assassin, new Type[]
            {
                typeof( SanguinAssassin ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Poisoner, new Type[]
            {
                typeof( Bootlegger ),
                typeof( Smuggler ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Scout, new Type[]
            {
                typeof( SanguinScout ),
                typeof( OrghereimTracker),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MeleePotion, new Type[]
            {
                typeof( SanguinConscript ),
                typeof( SanguinProtector ),
                typeof( OrghereimSwordThane),
                typeof( GraveRobber ),
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.MageKiller, new Type[]
            {
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.AntiArmor, new Type[]
            {
            } ),

            new AICreatureList(AIGroup.EvilHuman, AISubgroup.Ranged, new Type[]
            {
               typeof( Poacher ),
            } ),
        };
    }
}