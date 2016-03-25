using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;

namespace Server.Engines.Harvest
{
    public class HarvestTarget : Target
    {
        private Item m_Tool;
        private HarvestSystem m_System;

        public HarvestTarget(Item tool, HarvestSystem system)
            : base(-1, true, TargetFlags.None)
        {
            m_Tool = tool;
            m_System = system;

            DisallowMultis = true;

            // UOAC : No LOS checks for mining
            if (tool is Pickaxe || tool is Shovel || tool is SturdyPickaxe || tool is SturdyShovel || tool is ProspectorsTool || tool is DiamondPickaxe || tool is DiamondShovel)
            {
                CheckLOS = false;
            }
            // UOAC
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_System is Mining && targeted is StaticTarget)
            {
                int itemID = ((StaticTarget)targeted).ItemID;

                // grave
                if (itemID == 0xED3 || itemID == 0xEDF || itemID == 0xEE0 || itemID == 0xEE1 || itemID == 0xEE2 || itemID == 0xEE8)
                {
                    PlayerMobile player = from as PlayerMobile;

                    if (player != null)
                    {                        
                    }
                }
            }

            if (m_System is Lumberjacking && targeted is IChopable)
            {
                ((IChopable)targeted).OnChop(from);
            }

            else if (m_System is Lumberjacking && targeted is IAxe && m_Tool is BaseAxe)
            {
                IAxe obj = (IAxe)targeted;
                Item item = (Item)targeted;

                if (!item.IsChildOf(from.Backpack) && !item.IsChildOf(from.BankBox))
                    //from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    from.SendMessage("This item must be in your backpack or bankbox to be used.");
                else if (obj.Axe(from, (BaseAxe)m_Tool))
                    from.PlaySound(0x13E);
            }

            else if (m_System is Lumberjacking && targeted is ICarvable)
                ((ICarvable)targeted).Carve(from, (Item)m_Tool);

            else if (m_System is Lumberjacking && FurnitureAttribute.Check(targeted as Item))            
                DestroyFurniture(from, (Item)targeted);            

            else if (m_System is Mining && targeted is TreasureMap)
                ((TreasureMap)targeted).OnBeginDig(from);
            else
                m_System.StartHarvesting(from, m_Tool, targeted);
        }

        private void DestroyFurniture(Mobile from, Item item)
        {
            if (from.Region is UOACZRegion && !item.Movable)
            {
                from.SendMessage("That does not seem to work here.");
                return;
            }

            if (!from.InRange(item.GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            else if (!item.IsChildOf(from.Backpack) && !item.Movable)
            {
                from.SendLocalizedMessage(500462); // You can't destroy that while it is here.
                return;
            }

            from.SendLocalizedMessage(500461); // You destroy the item.
            Effects.PlaySound(item.GetWorldLocation(), item.Map, 0x3B3);

            if (item is Container)
            {
                if (item is TrapableContainer)
                    (item as TrapableContainer).ExecuteTrap(from);

                ((Container)item).Destroy();
            }

            else
            {
                item.Delete();
            }
        }
    }
}