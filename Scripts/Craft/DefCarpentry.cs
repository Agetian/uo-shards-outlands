using System;
using Server.Items;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Custom.Items;
using Server.Multis;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefCarpentry : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Carpentry; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefCarpentry();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0;
        }

        private DefCarpentry(): base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!

            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            // no animation
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );

            from.PlaySound(0x23D);
        }

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (resourceType == typeof(ShipParts))
            {
                from.Backpack.ConsumeTotal(resourceType, 1);
                return false;
            }

            if (typeof(BaseHousePlans).IsAssignableFrom(resourceType))
            {
                if (from.Backpack.ConsumeTotal(resourceType, 1))
                    UnfilledHousePlans.Create(from, resourceType);

                return false;
            }

            else if
                (
                resourceType == typeof(SmallStoneTempleHousePlans) ||
                resourceType == typeof(ArbitersEstateHousePlans) ||
                resourceType == typeof(SandstoneSpaHousePlans) ||
                resourceType == typeof(MagistratesHousePlans) ||
                resourceType == typeof(BalconyHousePlans)
                )

                return false;

            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.

                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }

            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.

                else if (makersMark && quality == 2)
                {
                    //Player Enhancement Customization: Artisan
                    if (PlayerEnhancementPersistance.IsCustomizationEntryActive(from, CustomizationType.Artisan))
                        CustomizationAbilities.Artisan(from, from.Location, from.Map);

                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                }

                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.

                else
                    return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            //Fletching
            index = AddCraft(1, typeof(Kindling), "Bowcraft and Fletching", "Kindling", 0.0, 0.0, typeof(Board), 1044041, 1, 1044351);
            SetUseAllRes(index, true);

            index = AddCraft(10, typeof(Shaft), "Bowcraft and Fletching", "Shaft", 0, 25, typeof(Board), 1044041, 10, 1044351);
            SetUseAllRes(index, true);

            index = AddCraft(25, typeof(Arrow), "Bowcraft and Fletching", "Arrow", 0, 25, typeof(Shaft), 1044560, 10, 1044561);
            AddRes(index, typeof(Feather), 1044562, 10, 1044563);
            SetUseAllRes(index, true);

            index = AddCraft(25, typeof(Bolt), "Bowcraft and Fletching", "Bolt", 0, 25, typeof(Shaft), 1044560, 10, 1044561);
            AddRes(index, typeof(Feather), 1044562, 10, 1044563);
            SetUseAllRes(index, true);

            AddCraft(1, typeof(Bow), "Bowcraft and Fletching", "Bow", 35, 60, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(Crossbow), "Bowcraft and Fletching", "Crossbow", 45, 70, typeof(Board), 1044041, 12, 1044351);
            AddCraft(1, typeof(HeavyCrossbow), "Bowcraft and Fletching", "Heavy Crossbow", 55, 80, typeof(Board), 1044041, 14, 1044351);

            //Weapons and Shields
            AddCraft(1, typeof(Club), "Weapons and Shields", "Club", 25, 50, typeof(Board), 1044041, 8, 1044351);

            AddCraft(1, typeof(ShepherdsCrook), "Weapons and Shields", "Shepherds Crook", 15, 40, typeof(Board), 1044041, 8, 1044351);
            AddCraft(1, typeof(QuarterStaff), "Weapons and Shields", "Quarter Staff", 35, 60, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(GnarledStaff), "Weapons and Shields", "Gnarled Staff", 35, 60, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(BlackStaff), "Weapons and Shields", "Black Staff", 35, 60, typeof(Board), 1044041, 10, 1044351);

            AddCraft(1, typeof(WoodenShield), "Weapons and Shields", "Wooden Shield", 15, 40, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(WoodenKiteShield), "Weapons and Shields", "Wooden Kite Shield", 30, 55, typeof(Board), 1044041, 12, 1044351);

            //Instruments
            AddCraft(1, typeof(Harp), "Instruments", "Harp", 50, 75, typeof(Board), 1044041, 10, 1044351);                        
            AddCraft(1, typeof(Drums), "Instruments", "Drums", 50, 75, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(Lute), "Instruments", "Lute", 50, 75, typeof(Board), 1044041, 10, 1044351);      
            AddCraft(1, typeof(Tambourine), "Instruments", "Tambourine", 50, 75, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(BambooFlute), "Instruments", "Bamboo Flute", 85, 110, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(StandingHarp), "Instruments", "Standing Harp", 95, 120, typeof(Board), 1044041, 20, 1044351);

            // Furniture
            AddCraft(1, typeof(FootStool), "Furniture", "Foot Stool", 10, 35, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(Stool), "Furniture", "Stool", 15, 40, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(BambooChair), "Furniture", "Bamboo Chair", 20, 45, typeof(Board), 1044041, 12, 1044351);
            AddCraft(1, typeof(WoodenChair), "Furniture", "Wooden Chair", 25, 50, typeof(Board), 1044041, 12, 1044351);            
            AddCraft(1, typeof(WoodenBench), "Furniture", "Wooden Bench", 30, 55, typeof(Board), 1044041, 14, 1044351);
            AddCraft(1, typeof(Nightstand), "Furniture", "Nightstand", 35, 60, typeof(Board), 1044041, 14, 1044351);
            AddCraft(1, typeof(WoodenThrone), "Furniture", "Wooden Throne", 40, 65, typeof(Board), 1044041, 18, 1044351);
            AddCraft(1, typeof(YewWoodTable), "Furniture", "Yew Wood Table", 45, 70, typeof(Board), 1044041, 20, 1044351);
            AddCraft(1, typeof(LargeTable), "Furniture", "Large Table", 50, 75, typeof(Board), 1044041, 20, 1044351);
            AddCraft(1, typeof(CushionedWoodenChair), "Furniture", "Cushioned Wooden Chair", 55, 80, typeof(Board), 1044041, 14, 1044351);
            AddCraft(1, typeof(Throne), "Furniture", "Throne", 60, 85, typeof(Board), 1044041, 18, 1044351);
            AddCraft(1, typeof(WritingTable), "Furniture", "Writing Table", 65, 90, typeof(Board), 1044041, 20, 1044351);

            //Containers
            index = AddCraft(1, typeof(Keg), "Containers", "Keg", 75, 100, typeof(BarrelStaves), 1044288, 5, 1044253);
            AddRes(index, typeof(BarrelHoops), 1044289, 1, 1044253);
            AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);

            AddCraft(1, typeof(WoodenBox), "Containers", "Wooden Box", 5, 30, typeof(Board), 1044041, 8, 1044351);
            AddCraft(1, typeof(SmallCrate), "Containers", "Small Crate", 10, 35, typeof(Board), 1044041, 10, 1044351);
            AddCraft(1, typeof(MediumCrate), "Containers", "Medium Crate", 15, 40, typeof(Board), 1044041, 12, 1044351);
            AddCraft(1, typeof(LargeCrate), "Containers", "Large Crate", 20, 45, typeof(Board), 1044041, 14, 1044351);
            AddCraft(1, typeof(WoodenChest), "Containers", "Wooden Chest", 25, 50, typeof(Board), 1044041, 16, 1044351);
            AddCraft(1, typeof(EmptyBookcase), "Containers", "Empty Bookcase", 75, 100, typeof(Board), 1044041, 18, 1044351);
            AddCraft(1, typeof(Armoire), "Containers", "Armoire", 90, 115, typeof(Board), 1044041, 20, 1044351);
            AddCraft(1, typeof(FancyArmoire), "Containers", "Fancy Armoire", 95, 120, typeof(Board), 1044041, 20, 1044351);

            AddCraft(1, typeof(BarrelStaves), "Misc", "Barrel Staves", 15, 40, typeof(Board), 1044041, 6, 1044351);
            AddCraft(1, typeof(BarrelLid), "Misc", "Barrel Lid", 0, 25.0, typeof(Board), 1044041, 6, 1044351);
            AddCraft(1, typeof(ShortMusicStand), "Misc", "Short Music Stand", 75, 100, typeof(Board), 1044041, 12, 1044351);
            AddCraft(1, typeof(TallMusicStand), "Misc", "Tall Music Stand", 85, 110, typeof(Board), 1044041, 14, 1044351);
            AddCraft(1, typeof(Easle), "Misc", "Easle", 95, 120, typeof(Board), 1044041, 20, 1044351);

            //Add-Ons
            AddCraft(1, typeof(DartBoardSouthDeed), "Add-Ons", "Dartboard South Deed", 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(1, typeof(DartBoardEastDeed), "Add-Ons", "Dartboard East Deed", 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(1, typeof(BallotBoxDeed), "Add-Ons", "Ballot Box Deed", 47.3, 72.3, typeof(Board), 1044041, 5, 1044351);

            index = AddCraft(1, typeof(SmallBedSouthDeed), "Add-Ons", "Small Bed South Deed", 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(1, typeof(SmallBedEastDeed), "Add-Ons", "Small Bed East Deed", 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(1, typeof(LargeBedSouthDeed), "Add-Ons", "Large Bed South Deed", 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);

            index = AddCraft(1, typeof(LargeBedEastDeed), "Add-Ons", "Large Bed East Deed", 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);

            index = AddCraft(1, typeof(PentagramDeed), "Add-Ons", "Pentagram Deed", 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            index = AddCraft(1, typeof(AbbatoirDeed), "Add-Ons", "Abbatoir Deed", 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            index = AddCraft(1, typeof(TrainingDummyEastDeed), "Add-Ons", "Training Dummy East Deed", 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(1, typeof(TrainingDummySouthDeed), "Add-Ons", "Training Dummy South Deed", 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(1, typeof(PickpocketDipEastDeed), "Add-Ons", "Pickpocket Dip East Deed", 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(1, typeof(PickpocketDipSouthDeed), "Add-Ons", "Pickpocket Dip South Deed", 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(1, typeof(SmallForgeDeed), "Add-Ons", "Small Forge Deed", 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 75, 1044037);

            index = AddCraft(1, typeof(LargeForgeEastDeed), "Add-Ons", "Large Forge East Deed", 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = AddCraft(1, typeof(LargeForgeSouthDeed), "Add-Ons", "Large Forge South Deed", 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = AddCraft(1, typeof(AnvilEastDeed), "Add-Ons", "Anvil East Deed", 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            index = AddCraft(1, typeof(AnvilSouthDeed), "Add-Ons", "Anvil South Deed", 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            index = AddCraft(1, typeof(Dressform), "Add-Ons", "Dressform", 63.1, 88.1, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(1, typeof(SpinningwheelEastDeed), "Add-Ons", "Spinning Wheel East Deed", 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(1, typeof(SpinningwheelSouthDeed), "Add-Ons", "Spinning Wheel South Deed", 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(1, typeof(LoomEastDeed), "Add-Ons", "Loom East Deed", 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(1, typeof(LoomSouthDeed), "Add-Ons", "Loom South Deed", 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(1, typeof(StoneOvenEastDeed), "Add-Ons", "Stone Oven East Deed", 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);

            index = AddCraft(1, typeof(StoneOvenSouthDeed), "Add-Ons", "Stone Oven South Deed", 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);

            index = AddCraft(1, typeof(FlourMillEastDeed), "Add-Ons", "Flour Mill East Deed", 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);

            index = AddCraft(1, typeof(FlourMillSouthDeed), "Add-Ons", "Flour Mill South Deed", 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);

            AddCraft(1, typeof(WaterTroughEastDeed), "Add-Ons", "Water Trough East Deed", 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);
            AddCraft(1, typeof(WaterTroughSouthDeed), "Add-Ons", "Water Trough South Deed", 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);

            //Houses
            index = AddCraft(1, typeof(SmallStoneTempleHouseDeed), "Houses", "Small Stone Temple ", 100, 105, typeof(SmallStoneTempleHousePlans), "Paladin House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            index = AddCraft(1, typeof(ArbiterEstateDeed), "Houses", "Arbiters Estate", 100, 120, typeof(ArbitersEstateHousePlans), "Arbiters Estate House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            index = AddCraft(1, typeof(SandstoneSpaHouseDeed), "Houses", "Sandstone Spa", 100, 110, typeof(SandstoneSpaHousePlans), "Lanistas Retreat House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            index = AddCraft(1, typeof(MagistrateHouseDeed), "Houses", "Magistrates House", 100, 110, typeof(MagistratesHousePlans), "Magistrates House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            //Ship Items and Upgrades  
            index = AddCraft(1, typeof(FishingPole), "Ship Items", "Fishing Pole", 68.4, 93.4, typeof(Board), 1044041, 5, 1044351); //This is in the categor of Other during AoS
            AddSkill(index, SkillName.Tailoring, 40.0, 45.0);
            AddRes(index, typeof(Cloth), 1044286, 5, 1044287);

            index = AddCraft(1, typeof(ShipParts), "Ship Items", "Ship Parts", 50, 60, typeof(Board), "Board", 100, "You do not have the necessary boards to construct the ship parts.");
            AddRes(index, typeof(Cloth), "Cloth", 50, "You do not have the necessary cloth to construct the ship parts.");
            AddRes(index, typeof(IronIngot), "Iron Ingot", 50, "You do not have the necessary boards to construct the ship parts.");

            index = AddCraft(1, typeof(ShipRepairTool), "Ship Items", "Ship Repair Tool", 50, 75, typeof(Board), "Board", 25, "You do not have the neccesary boards to construct a ship repair tool.");
            AddRes(index, typeof(Nails), "Nails", 5, "You do not have the neccessary nails to construct a ship repair tool");
            AddRes(index, typeof(Hammer), "Hammer", 1, "You require a hammer to construct a ship repair tool");
            AddRes(index, typeof(Saw), "Saw", 1, "You require a saw to construct a ship repair tool");

            index = AddCraft(1, typeof(SmallShipDeed), "Ship Items", "Small Ship Deed", 70.0, 75.0, typeof(ShipParts), "Ship Parts", 10, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(SmallDragonShipDeed), "Ship Items", "Small Dragon Ship Deed", 70.0, 75.0, typeof(ShipParts), "Ship Parts", 11, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(MediumShipDeed), "Ship Items", "Medium Ship Deed", 80.0, 85.0, typeof(ShipParts), "Ship Parts", 12, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(MediumDragonShipDeed), "Ship Items", "Medium Dragon Ship Deed", 80.0, 85.0, typeof(ShipParts), "Ship Parts", 13, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(LargeShipDeed), "Ship Items", "Large Ship Deed", 85, 90.0, typeof(ShipParts), "Ship Parts", 16, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(LargeDragonShipDeed), "Ship Items", "Large Dragon Ship Deed ", 85.0, 90.0, typeof(ShipParts), "Ship Parts", 17, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(CarrackShipDeed), "Ship Items", "Carrack Deed ", 90.0, 95.0, typeof(ShipParts), "Ship Parts", 20, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(1, typeof(GalleonShipDeed), "Ship Items", "Galleon Deed", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 30, "You do not have the neccessary number of ship parts to construct that ship.");
            
            AddCraft(1, typeof(CarpentryMouldTier2), "Tools", "Carpentry Mould: Tier 1-2", 100, 100, typeof(Board), 1044041, 200, 1044351);
            AddCraft(1, typeof(CarpentryMouldTier4), "Tools", "Carpentry Mould: Tier 3-4", 105, 105, typeof(Board), 1044041, 225, 1044351);
            AddCraft(1, typeof(CarpentryMouldTier6), "Tools", "Carpentry Mould: Tier 5-6", 110, 110, typeof(Board), 1044041, 250, 1044351);
            AddCraft(1, typeof(CarpentryMouldTier8), "Tools", "Carpentry Mould: Tier 7-8", 115, 115, typeof(Board), 1044041, 275, 1044351);
            AddCraft(1, typeof(CarpentryMouldTier10), "Tools", "Carpentry Mould: Tier 9-10", 120, 120, typeof(Board), 1044041, 300, 1044351);
            
            /*
            index = AddCraft(1, typeof(CampingFirepit), "Misc", "Camping Firepit", 80.0, 120, typeof(Board), 1044041, 50, 1044351);
            AddRes(index, typeof(IronIngot), "Iron Ingot", 25, 1044037);
            AddRes(index, typeof(TrollFat), "Troll Fat", 1, "You do not have the neccesary crafting component needed to make this");
            */

            //Hue Plating
            /*
            AddCraft(1, typeof(RegularWoodHuePlating), "Misc", "regular wood hue plating", 70, 120, typeof(Board), "Board", 25, "You do not have the neccesary boards to construct that.");

            index = AddCraft(1, typeof(OakWoodHuePlating), "Misc", "oak wood hue plating", 70, 120, typeof(OakBoard), "oak board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Ghostweed), "ghostweed", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(AshWoodHuePlating), "Other", "ash wood hue plating", 70, 120, typeof(AshBoard), "ash board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Snakeskin), "creepervine", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(YewWoodHuePlating), "Other", "yew wood hue plating", 70, 120, typeof(YewBoard), "yew board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Quartzstone), "quartzstone", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(HeartWoodHuePlating), "Other", "heartwood hue plating", 70, 120, typeof(HeartwoodBoard), "heartwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Creepervine), "creepervine", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(BloodWoodHuePlating), "Other", "bloodwood hue plating", 70, 120, typeof(BloodwoodBoard), "bloodwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(FireEssence), "fire essence", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(FrostWoodHuePlating), "Other", "frostwood hue plating", 70, 120, typeof(FrostwoodBoard), "frostwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(BluecapMushroom), "bluecap mushroom", 5, "You do not have the neccessary crafting components to construct that.");
            */

            //-----

            Recycle = true;
            MarkOption = true;
            Repair = true;

            SetSubRes(typeof(Board), "Board");

            // Add every material you want the player to be able to choose from
            // This will override the overridable material	TODO: Verify the required skill amount
            AddSubRes(typeof(Board), "Board", 00.0, 1072652);
            AddSubRes(typeof(OakBoard), "Oak Board", 65.0, 1072652);
            AddSubRes(typeof(AshBoard), "Ash Board", 80.0, 1072652);
            AddSubRes(typeof(YewBoard), "Yew Board", 95.0, 1072652);
            AddSubRes(typeof(HeartwoodBoard), "Heartwood Board", 100, 1072652);
            AddSubRes(typeof(BloodwoodBoard), "Bloodwood Board", 100, 1072652);
            AddSubRes(typeof(FrostwoodBoard), "Frostwood Board", 100, 1072652);
        }
    }
}