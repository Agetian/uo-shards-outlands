using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Engines.Harvest
{
    public class Fishing : HarvestSystem
    {
        private static Fishing m_System;

        public static Fishing System
        {
            get
            {
                if (m_System == null)
                    m_System = new Fishing();

                return m_System;
            }
        }

        private HarvestDefinition m_Definition;
        public HarvestDefinition Definition
        {
            get { return m_Definition; }
        }

        public static int HarvestRange = 4;

        private Fishing()
        {
            HarvestResource[] res;
            HarvestVein[] veins;

            HarvestDefinition fish = new HarvestDefinition();

            fish.BankWidth = 8;
            fish.BankHeight = 8;

            fish.MinTotal = 20;
            fish.MaxTotal = 25;

            fish.MinRespawn = TimeSpan.FromMinutes(10.0);
            fish.MaxRespawn = TimeSpan.FromMinutes(20.0);

            fish.Skill = SkillName.Fishing;

            fish.Tiles = m_WaterTiles;
            fish.RangedTiles = true;

            fish.MaxRange = HarvestRange;

            fish.ConsumedPerHarvest = 1;
            fish.ConsumedPerFeluccaHarvest = 1;

            fish.EffectActions = new int[] { 12 };
            fish.EffectSounds = new int[0];
            fish.EffectCounts = new int[] { 1 };
            fish.EffectDelay = TimeSpan.Zero;
            fish.EffectSoundDelay = TimeSpan.FromSeconds(8.0);

            fish.NoResourcesMessage = 503172; // The fish don't seem to be biting here.
            fish.FailMessage = 503171; // You fish a while, but fail to catch anything.
            fish.TimedOutOfRangeMessage = 500976; // You need to be closer to the water to fish!
            fish.OutOfRangeMessage = 500976; // You need to be closer to the water to fish!
            fish.PackFullMessage = 503176; // You do not have room in your backpack for a fish.
            fish.ToolBrokeMessage = 503174; // You broke your fishing pole.

            res = new HarvestResource[]
			{
				new HarvestResource( 00.0, 00.0, 120.0, 1043297, typeof( RawFish ) ),               
			};

            veins = new HarvestVein[]
			{
				new HarvestVein( 100.0, 0.0, res[0], null ),               
			};

            fish.Resources = res;
            fish.Veins = veins;

            m_Definition = fish;
            Definitions.Add(fish);
        }

        public override void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            from.SendLocalizedMessage(500972); // You are already fishing.
        }

        private class MutateEntry
        {
            public double m_ReqSkill, m_MinSkill, m_MaxSkill;
            public bool m_DeepWater;
            public Type[] m_Types;

            public MutateEntry(double reqSkill, double minSkill, double maxSkill, bool deepWater, params Type[] types)
            {
                m_ReqSkill = reqSkill;
                m_MinSkill = minSkill;
                m_MaxSkill = maxSkill;
                m_DeepWater = deepWater;
                m_Types = types;
            }
        }

        //TEST: FIX THIS
        private static MutateEntry[] m_MutateTable = new MutateEntry[]
		{	
            new MutateEntry(  50.0,  80.0,  1000.0,  false, typeof( RawLargeFish ) ),		// 0.59%
            new MutateEntry(  80.0,  80.0,  3480.0,  true,  typeof( SpecialFishingNet ) ),	// 0.59%
            new MutateEntry(  90.0,  80.0,  3480.0,  true,  typeof( TreasureMap ) ),			// 0.59%
            new MutateEntry( 100.0,  80.0,  5200.0,  true,  typeof( MessageInABottle ) ),	// 0.39%
            new MutateEntry(   0.0, 105.0,  -420.0, false,  typeof( Boots ), typeof( Shoes ), typeof( Sandals ), typeof( ThighBoots ) ),
            new MutateEntry(   0.0, 200.0,  -200.0, false,  new Type[1]{ null } )
       };

        public override bool SpecialHarvest(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {                
            }

            return false;
        }

        public static bool CheckDock(Point3D p, Map map)
        {
            if (map == null || map == Map.Internal)
                return false;

            /*
            Sector sector = map.GetSector(p.X, p.Y);

            for (int i = 0; i < sector.Multis.Count; ++i)
            {
                BaseMulti multi = (BaseMulti)sector.Multis[i];

                if (multi is BaseGuildDock && multi.Contains(p))
                {
                    return true;
                }
            }
            */

            return false;
        }

        public override Type MutateType(Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            bool deepWater = SpecialFishingNet.FullValidation(map, loc.X, loc.Y);

            if (deepWater && CheckDock(from.Location, from.Map))
                deepWater = false;

            double skillBase = from.Skills[SkillName.Fishing].Base;
            double skillValue = from.Skills[SkillName.Fishing].Value;

            BaseShip ownerShip = BaseShip.FindShipAt(from.Location, from.Map);
            double chanceModifier = 1;
            
            for (int i = 0; i < m_MutateTable.Length; ++i)
            {
                MutateEntry entry = m_MutateTable[i];

                if (!deepWater && entry.m_DeepWater)
                    continue;

                if (skillBase >= entry.m_ReqSkill)
                {
                    double chance = (skillValue - entry.m_MinSkill) / (entry.m_MaxSkill - entry.m_MinSkill);
                    chance *= chanceModifier;

                    if (from.AccessLevel >= AccessLevel.GameMaster)
                    {
                        string typename = "null";

                        if (entry.m_Types[0] != null)
                            typename = entry.m_Types[0].Name.ToLower();

                        from.SendMessage(0x22, String.Format("{0} : {1:P2} chance", typename, chance));
                    }

                    if (chance > Utility.RandomDouble())
                        return entry.m_Types[Utility.Random(entry.m_Types.Length)];
                }
            }

            return type;
        }

        private static Map SafeMap(Map map)
        {
            if (map == null || map == Map.Internal)
                return Map.Trammel;

            return map;
        }

        public override bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            Container pack = from.Backpack;

            if (pack != null)
            {
                List<SOS> messages = pack.FindItemsByType<SOS>();

                for (int i = 0; i < messages.Count; ++i)
                {
                    SOS sos = messages[i];

                    if (from.Map == sos.TargetMap && from.InRange(sos.TargetLocation, 60) && !sos.Completed)
                        return true;
                }
            }

            return base.CheckResources(from, tool, def, map, loc, timed);
        }

        public override Item Construct(Type type, Mobile from, Item _i, HarvestDefinition _d, HarvestBank _b, HarvestResource _r)
        {
            if (type == typeof(TreasureMap))
            {
                int level = 1;

                return new TreasureMap(level, Map.Felucca);
            }

            else if (type == typeof(MessageInABottle))            
                return new MessageInABottle(Map.Felucca);            

            Container pack = from.Backpack;

            if (pack != null)
            {
                List<SOS> messages = pack.FindItemsByType<SOS>();

                for (int i = 0; i < messages.Count; ++i)
                {
                    SOS sos = messages[i];

                    if (from.Map == sos.TargetMap && from.InRange(sos.TargetLocation, 60) && !sos.Completed)
                    {
                        Item preLoot = null;

                        switch (Utility.Random(7))
                        {
                            case 0: // Body parts
                            {
                                int[] list = new int[]
								{
									0x1CDD, 0x1CE5, // arm
									0x1CE0, 0x1CE8, // torso
									0x1CE1, 0x1CE9, // head
									0x1CE2, 0x1CEC // leg
								};

                                preLoot = new ShipwreckedItem(Utility.RandomList(list));
                                break;
                            }

                            case 1: // Bone parts
                            {
                                int[] list = new int[]
								{
									0x1AE0, 0x1AE1, 0x1AE2, 0x1AE3, 0x1AE4, // skulls
									0x1B09, 0x1B0A, 0x1B0B, 0x1B0C, 0x1B0D, 0x1B0E, 0x1B0F, 0x1B10, // bone piles
									0x1B15, 0x1B16 // pelvis bones
								};

                                preLoot = new ShipwreckedItem(Utility.RandomList(list));
                                break;                            
                            }

                            case 2: // Pillows
                            {
                                preLoot = new ShipwreckedItem(Utility.Random(0x13A4, 11));
                                break;
                            }

                            case 3: // Shells
                            {
                                preLoot = new ShipwreckedItem(Utility.Random(0xFC4, 9));
                                break;
                            }

                            case 4:	//Hats
                            {
                                if (Utility.RandomBool())
                                    preLoot = new SkullCap();
                                else
                                    preLoot = new TricorneHat();

                                break;
                            }

                            case 5: // Misc
                            {
                                int[] list = new int[]
								{
									0x1EB5, // unfinished barrel
									0xA2A, // stool
									0xC1F, // broken clock									
									0x1EB1, 0x1EB2, 0x1EB3, 0x1EB4 // barrel staves
								};

                                if (Utility.Random(list.Length + 1) == 0)
                                    preLoot = new Candelabra();
                                else
                                    preLoot = new ShipwreckedItem(Utility.RandomList(list));

                                break;
                            }
                        }

                        if (preLoot != null)
                        {
                            if (preLoot is IShipwreckedItem)
                                ((IShipwreckedItem)preLoot).IsShipwreckedItem = true;

                            return preLoot;
                        }

                        LockableContainer chest;

                        if (Utility.RandomBool())
                            chest = new MetalGoldenChest();
                        else
                            chest = new WoodenChest();

                        if (sos.IsAncient)
                            chest.Hue = 0x481;

                        TreasureMapChest.Fill(chest, Math.Max(1, Math.Min(3, (sos.Level + 1))));

                        if (sos.IsAncient)
                            chest.DropItem(new FabledFishingNet());
                        else
                            chest.DropItem(new SpecialFishingNet());
                        
                        switch (Utility.Random(300))
                        {
                            case 0:
                            {
                                Item rustedchest = new PlateChest();
                                rustedchest.Hue = 2718;
                                rustedchest.Name = "a rusted platemail chest recovered from a shipwreck";

                                chest.DropItem(rustedchest);
                                break;
                            }

                            case 1:
                            {
                                Item rustedarms = new PlateArms();
                                rustedarms.Hue = 2718;
                                rustedarms.Name = "rusted platemail arms recovered from a shipwreck";

                                chest.DropItem(rustedarms);
                                break;
                            }

                            case 2:
                            {
                                Item rustedlegs = new PlateLegs();
                                rustedlegs.Hue = 2718;
                                rustedlegs.Name = "rusted platemail legguards recovered from a shipwreck";

                                chest.DropItem(rustedlegs);
                                break;
                            }

                            case 3:
                            {
                                Item rustedgloves = new PlateGloves();
                                rustedgloves.Hue = 2718;
                                rustedgloves.Name = "rusted platemail gloves recovered from a shipwreck";

                                chest.DropItem(rustedgloves);
                                break;
                            }

                            case 4:
                            {
                                Item rustedgorget = new PlateGorget();
                                rustedgorget.Hue = 2718;
                                rustedgorget.Name = "rusted platemail gorget recovered from a shipwreck";

                                chest.DropItem(rustedgorget);
                                break;
                            }

                            case 5:
                            {
                                Item rustedhelm = new PlateHelm();
                                rustedhelm.Hue = 2718;
                                rustedhelm.Name = "a rusted platemail helmet recovered from a shipwreck";

                                chest.DropItem(rustedhelm);
                                break;
                            }
                        }

                        switch (Utility.Random(400))
                        {
                            case 0:
                            {
                                Item lamp = new LampPost1();
                                lamp.Name = "Britannia Head Light";
                                lamp.Hue = 2601;

                                chest.DropItem(lamp);
                                break;
                            }

                            case 1:
                            {
                                Item lantern = new HangingLantern();
                                lantern.Name = "Fog Lamp";
                                lantern.Hue = 2601;
                                lantern.Movable = true;

                                chest.DropItem(lantern);
                                break;
                            }
                        }

                        chest.Movable = true;
                        chest.Locked = false;
                        chest.TrapType = TrapType.None;
                        chest.TrapPower = 0;
                        chest.TrapLevel = 0;

                        sos.Completed = true;
                        
                        BaseShip ownerShip = BaseShip.FindShipAt(from.Location, from.Map);

                        PlayerMobile player = from as PlayerMobile;

                        if (ownerShip != null && player != null)
                        {
                            if (ownerShip.IsFriend(player) || ownerShip.IsOwner(player) || ownerShip.IsCoOwner(player))
                            {
                                double doubloonValue = Utility.RandomMinMax(25, 50);
                                
                                int finalDoubloonAmount = (int)doubloonValue;                                

                                bool shipOwner = ownerShip.IsOwner(player);
                                bool bankDoubloonsValid = false;
                                bool holdPlacementValid = false;
                                
                                //Deposit Half In Player's Bank
                                if (Banker.DepositUniqueCurrency(player, typeof(Doubloon), finalDoubloonAmount))
                                {
                                    Doubloon doubloonPile = new Doubloon(finalDoubloonAmount);
                                    player.SendSound(doubloonPile.GetDropSound());
                                    doubloonPile.Delete();

                                    bankDoubloonsValid = true;
                                }

                                //Deposit Other Half in Ship
                                if (ownerShip.DepositDoubloons(finalDoubloonAmount))
                                {
                                    Doubloon doubloonPile = new Doubloon(finalDoubloonAmount);
                                    player.SendSound(doubloonPile.GetDropSound());
                                    doubloonPile.Delete();

                                    holdPlacementValid = true;
                                }

                                if (shipOwner)
                                {                                    
                                    player.PirateScore += finalDoubloonAmount;
                                    //ownerShip.doubloonsEarned += finalDoubloonAmount * 2;

                                    if (bankDoubloonsValid && holdPlacementValid)
                                        player.SendMessage("You've received " + (finalDoubloonAmount * 2).ToString() + " doubloons for completing a message in a bottle! They have been evenly split between your bank box and your ship's hold.");

                                    else if (bankDoubloonsValid && !holdPlacementValid)
                                        player.SendMessage("You've earned " + (finalDoubloonAmount * 2).ToString() + " doubloons, however there was not enough room to place all of them in your ship's hold.");

                                    else if (!bankDoubloonsValid && holdPlacementValid)
                                        player.SendMessage("You've earned " + (finalDoubloonAmount * 2).ToString() + " doubloons, however there was not enough room to place all of them in your bank box.");
                                }

                                else
                                {
                                    //ownerShip.doubloonsEarned += finalDoubloonAmount;
                                    player.PirateScore += finalDoubloonAmount;

                                    if (bankDoubloonsValid)
                                        player.SendMessage("You've earned " + finalDoubloonAmount.ToString() + " doubloons for completing a message in a bottle! They have been placed in your bank box.");
                                    
                                    else
                                        player.SendMessage("You've earned doubloons, but there was not enough room to place all of them in your bank box.");
                                }
                            }
                        }

                        return chest;
                    }
                }
            }

            return base.Construct(type, from, _i, _d, _b, _r);
        }

        public override void FailHarvest(Mobile from, HarvestDefinition def)
        {
            from.PlaySound(0x023);
        }

        public override bool Give(Mobile from, Item item, bool placeAtFeet)
        {
            if (item is TreasureMap || item is MessageInABottle || item is SpecialFishingNet)
            {
                BaseCreature serp;

                if (0.25 > Utility.RandomDouble())
                    serp = new DeepSeaSerpent();

                else
                    serp = new SeaSerpent();

                serp.m_WasFishedUp = true;

                int x = from.X, y = from.Y;

                Map map = from.Map;

                for (int i = 0; map != null && i < 20; ++i)
                {
                    int tx = from.X - 10 + Utility.Random(21);
                    int ty = from.Y - 10 + Utility.Random(21);

                    LandTile t = map.Tiles.GetLandTile(tx, ty);

                    if (t.Z == -5 && ((t.ID >= 0xA8 && t.ID <= 0xAB) || (t.ID >= 0x136 && t.ID <= 0x137)) && !Spells.SpellHelper.CheckMulti(new Point3D(tx, ty, -5), map))
                    {
                        x = tx;
                        y = ty;

                        break;
                    }
                }

                serp.MoveToWorld(new Point3D(x, y, -5), map);

                serp.Home = serp.Location;
                serp.RangeHome = 10;

                //TEST: FIX
                //serp.PackItem(item);

                from.SendLocalizedMessage(503170); // Uh oh! That doesn't look like a fish!

                return true;
            }

            if (item is WoodenChest || item is MetalGoldenChest)
                placeAtFeet = true;

            from.PlaySound(0x025);

            return base.Give(from, item, placeAtFeet);
        }

        public override void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
        {
           if (item is RawLargeFish)            
                from.SendLocalizedMessage(1042635); // Your fishing pole bends as you pull a big fish from the depths!            

            else if (item is WoodenChest || item is MetalGoldenChest)            
                from.SendLocalizedMessage(503175); // You pull up a heavy chest from the depths of the ocean!            

            else
            {
                int number;
                string name;

                if (item is RawFish)
                {
                    number = 1008124;
                    name = "a fish";
                }

                else if (item is BaseShoes)
                {
                    number = 1008124;
                    name = item.ItemData.Name;
                }

                else if (item is TreasureMap)
                {
                    number = 1008125;
                    name = "a sodden piece of parchment";
                }

                else if (item is MessageInABottle)
                {
                    number = 1008125;
                    name = "a bottle, with a message in it";
                }

                else if (item is SpecialFishingNet)
                {
                    number = 1008125;
                    name = "a special fishing net"; // TODO: this is just a guess--what should it really be named?
                }

                else
                {
                    number = 1043297;

                    if ((item.ItemData.Flags & TileFlag.ArticleA) != 0)
                        name = "a " + item.ItemData.Name;

                    else if ((item.ItemData.Flags & TileFlag.ArticleAn) != 0)
                        name = "an " + item.ItemData.Name;

                    else
                        name = item.ItemData.Name;
                }

                NetState ns = from.NetState;

                if (ns == null)
                    return;

                if (number == 1043297 || ns.HighSeas)
                    from.SendLocalizedMessage(number, name);

                else
                    from.SendLocalizedMessage(number, true, name);
            }
        }

        public object SearchForNearbyNode(Point3D location, Map map, int range)
        {
            object nearbyNode = null;

            return nearbyNode;
        }

        public override void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            base.OnHarvestStarted(from, tool, def, toHarvest);

            int tileID;
            Map map;
            Point3D loc;            

            if (GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {               
                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (from == null) return;
                    if (from.Deleted || !from.Alive) return;

                    Effects.PlaySound(loc, map, 0x33C);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(loc.X, loc.Y, loc.Z), map);

                    Effects.SendMovingEffect(startLocation, endLocation, 574, 5, 0, false, false, 0, 0);

                    double distance = Utility.GetDistanceToSqrt(startLocation.Location, endLocation.Location);
                    double destinationDelay = (double)distance * .12;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        Effects.PlaySound(loc, map, 0x364);

                        LandTile landTile = map.Tiles.GetLandTile(loc.X, loc.Y);

                        int[] m_BadLandTiles = new int[] { 88 };

                        bool foundBadTile = false;

                        for (int a = 0; a < m_BadLandTiles.Length; a++)
                        {
                            if (m_BadLandTiles[a] == landTile.ID)
                            {
                                foundBadTile = true;
                                break;
                            }
                        }

                        if (!foundBadTile)
                        {
                            Effects.SendLocationEffect(loc, map, 0x352D, 16, 4);

                            TimedStatic bobber = new TimedStatic(574, 6);
                            bobber.MoveToWorld(loc, map);
                        }                                   
                    });
                });
            }
        }

        public override void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
            base.OnHarvestFinished(from, tool, def, vein, bank, resource, harvested);

            from.RevealingAction();
            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.FishingCooldown * 1000);
        }

        public override object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            return this;
        }

        public override bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!base.BeginHarvesting(from, tool))
                return false;

            from.SendLocalizedMessage(500974); // What water do you want to fish in?
            return true;
        }

        public override bool CheckHarvest(Mobile from, Item tool)
        {
            if (!base.CheckHarvest(from, tool))
                return false;

            if (from.Mounted)
            {
                from.SendLocalizedMessage(500971); // You can't fish while riding!
                return false;
            }

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            CaptchaPersistance.CheckAndCreateCaptchaAccountEntry(player);

            if (!player.m_CaptchaAccountData.Attempt(player, CaptchaSourceType.Fishing))
                return false;

            return true;
        }

        public override bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (!base.CheckHarvest(from, tool, def, toHarvest))
                return false;

            if (from.Mounted)
            {
                from.SendLocalizedMessage(500971); // You can't fish while riding!
                return false;
            }

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            CaptchaPersistance.CheckAndCreateCaptchaAccountEntry(player);

            if (!player.m_CaptchaAccountData.Attempt(player, CaptchaSourceType.Fishing))
                return false;

            return true;
        }

        private static int[] m_WaterTiles = new int[]
		{
			0x00A8, 0x00AB,
			0x0136, 0x0137,
			0x5797, 0x579C,
			0x746E, 0x7485,
			0x7490, 0x74AB,
			0x74B5, 0x75D5
		};
    }
}