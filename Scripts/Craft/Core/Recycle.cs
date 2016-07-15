using System;
using Server;
using Server.Targeting;
using Server.Items;

using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	public enum RecycleResult
	{
		Success,
        SuccessMultiple,
		Invalid,
        InvalidEntireBackpack,
		NoSkill
	}

	public class Recycle
	{
		public Recycle()
		{
		}

        public static List<CraftSystem> GetRecyclableCraftSystems()
        {
            List<CraftSystem> m_CraftSystems = new List<CraftSystem>();

            #region CraftSystem Definitions

            if (DefAlchemy.CraftSystem.Recycle)
                m_CraftSystems.Add(DefAlchemy.CraftSystem);

            if (DefBlacksmithy.CraftSystem.Recycle)
                m_CraftSystems.Add(DefBlacksmithy.CraftSystem);

            if (DefCarpentry.CraftSystem.Recycle)
                m_CraftSystems.Add(DefCarpentry.CraftSystem);

            if (DefCartography.CraftSystem.Recycle)
                m_CraftSystems.Add(DefCartography.CraftSystem);

            if (DefCooking.CraftSystem.Recycle)
                m_CraftSystems.Add(DefCooking.CraftSystem);

            if (DefGlassblowing.CraftSystem.Recycle)
                m_CraftSystems.Add(DefGlassblowing.CraftSystem);

            if (DefInscription.CraftSystem.Recycle)
                m_CraftSystems.Add(DefInscription.CraftSystem);

            if (DefMasonry.CraftSystem.Recycle)
                m_CraftSystems.Add(DefMasonry.CraftSystem);

            if (DefTailoring.CraftSystem.Recycle)
                m_CraftSystems.Add(DefTailoring.CraftSystem);

            if (DefTinkering.CraftSystem.Recycle)
                m_CraftSystems.Add(DefTinkering.CraftSystem);

            #endregion

            return m_CraftSystems;
        }

        public static bool IsRecycleResource(Type type)
        {
            bool recycleable = false;

            if (type == typeof(IronIngot)) return true;
            if (type == typeof(Leather)) return true;
            if (type == typeof(Board)) return true;
            if (type == typeof(Cloth)) return true;

            return recycleable;
        }        
        
		public static void RecyclePrompt( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			int num = craftSystem.CanCraft( from, tool, null );

			if ( num > 0 && num != 1044267 )			
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );			

			else
			{
                CraftContext context = craftSystem.GetContext(from);

                if (context == null)
                    return;

				from.Target = new InternalTarget( craftSystem, tool );

                switch (context.RecycleOption)
                {
                    case CraftRecycleOption.RecycleSingle: 
                        from.SendMessage("Target an individual item to recycle.");
                    break;

                    case CraftRecycleOption.RecycleRegularByType:
                        from.SendMessage("Target the type of item you wish to recycle. All regular items of that type found in your pack will be recycled.");
                    break;

                    case CraftRecycleOption.RecycleExceptionalByType:
                        from.SendMessage("Target the type of item you wish to recycle. All exceptional-quality items of that type found in your pack will be recycled.");
                    break;

                    case CraftRecycleOption.RecycleMagicalByType:
                        from.SendMessage("Target the type of item you wish to recycle. All magical items of that type found in your pack will be recycled.");
                    break;

                    case CraftRecycleOption.RecycleAnyByType:
                        from.SendMessage("Target the type of item you wish to recycle. Any items of that type found in your pack will be recycled.");
                    break;

                    case CraftRecycleOption.RecycleEverything:
                        from.SendMessage(1256, "Warning! You are about to recycle ALL items of ANY type that may be recycled within in your backpack. Target yourself to proceed.");
                    break;
                }
			}
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null)
                    return;

                int num = m_CraftSystem.CanCraft(from, m_Tool, null);

                if (num > 0)
                {
                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, num));
                    return;
                }

                RecycleResult result = RecycleResult.Invalid;
                string message = "";

                CraftContext craftContext = m_CraftSystem.GetContext(from);

                if (craftContext == null)
                    return;
                
                if (craftContext.RecycleOption == CraftRecycleOption.RecycleEverything && targeted == from && from.Backpack != null)
                {
                    result = Recycle(from, from.Backpack);

                    switch (result)
                    {
                        case RecycleResult.Invalid: message = "That cannot be recycled."; break;
                        case RecycleResult.InvalidEntireBackpack: message = "No recyclable items were found in your backpack."; break;
                        case RecycleResult.NoSkill: message = "You do not know how to recycle this material."; break;
                        case RecycleResult.Success: message = "You recycle an item."; break;
                        case RecycleResult.SuccessMultiple: message = "You recycle several items."; break;
                    }

                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, message));

                    return;
                }               

                Item item = targeted as Item;

                if (item == null)
                    return;

                else
                    result = Recycle(from, item);

                switch (result)
                {                    
                    case RecycleResult.Invalid: message = "That cannot be recycled."; break;
                    case RecycleResult.InvalidEntireBackpack: message = "No recyclable items were found in your backpack."; break;
                    case RecycleResult.NoSkill: message = "You do not know how to recycle this material."; break;
                    case RecycleResult.Success: message = "You recycle an item."; break;
                    case RecycleResult.SuccessMultiple: message = "You recycle several items."; break;
                }

                from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, message));
            }

			private RecycleResult Recycle( Mobile from, Item item)
			{
                if (from == null || item == null || m_CraftSystem == null || m_Tool == null)
                    return RecycleResult.Invalid;

                if (from.Backpack == null)
                    return RecycleResult.Invalid;

                CraftContext craftContext = m_CraftSystem.GetContext(from);

                if (craftContext == null)
                    return RecycleResult.Invalid;

                bool recycleEntireBackpack = false;

                if (craftContext.RecycleOption == CraftRecycleOption.RecycleEverything && item == from.Backpack)
                    recycleEntireBackpack = true;

                List<CraftSystem> m_RecycleableCraftSystems = GetRecyclableCraftSystems();

                List<Type> m_ItemTypes = new List<Type>();

                if (recycleEntireBackpack)
                {
                    List<Item> m_BackpackItems = from.Backpack.FindItemsByType<Item>();
                    
                    foreach (Item backpackItem in m_BackpackItems)
                    {
                        if (backpackItem is BaseTool)
                            continue;

                        Type itemType = backpackItem.GetType();

                        if (!m_ItemTypes.Contains(itemType))
                            m_ItemTypes.Add(itemType);
                    }
                }

                else                
                    m_ItemTypes.Add(item.GetType());

                List<int> m_RecycleSounds = new List<int>();

                int deletedCount = 0;      

                foreach (Type itemType in m_ItemTypes)
                {
                    foreach (CraftSystem craftSystem in m_RecycleableCraftSystems)
                    {
                        CraftItem craftItem = craftSystem.CraftItems.SearchFor(itemType);

                        if (craftItem == null || craftItem.Resources.Count == 0)
                            continue;

                        Dictionary<Type, int> m_ValidRecipeResources = new Dictionary<Type, int>();

                        CraftResCol craftResourceCollection = craftItem.Resources;

                        for (int a = 0; a < craftResourceCollection.Count; a++)
                        {
                            CraftRes craftResource = craftResourceCollection.GetAt(a);

                            if (!IsRecycleResource(craftResource.ItemType))
                                continue;

                            if (!m_ValidRecipeResources.ContainsKey(craftResource.ItemType))
                                m_ValidRecipeResources.Add(craftResource.ItemType, craftResource.Amount);
                        }

                        if (m_ValidRecipeResources.Count == 0)
                            continue;

                        List<Item> m_Items = new List<Item>();
                        List<Item> m_ItemsToRecycle = new List<Item>();

                        Item[] m_MatchingItems = from.Backpack.FindItemsByType(itemType);

                        for (int a = 0; a < m_MatchingItems.Length; a++)
                        {
                            Item targetItem = m_MatchingItems[a];

                            if (craftContext.RecycleOption == CraftRecycleOption.RecycleSingle && targetItem == item)
                            {
                                m_ItemsToRecycle.Add(targetItem);
                                continue;
                            }

                            if (craftContext.RecycleOption == CraftRecycleOption.RecycleRegularByType && targetItem.Quality == Quality.Regular && !targetItem.IsMagical)
                            {
                                m_Items.Add(targetItem);
                                continue;
                            }

                            if (craftContext.RecycleOption == CraftRecycleOption.RecycleExceptionalByType && targetItem.Quality == Quality.Exceptional)
                            {
                                m_Items.Add(targetItem);
                                continue;
                            }

                            if (craftContext.RecycleOption == CraftRecycleOption.RecycleExceptionalByType && targetItem.IsMagical)
                            {
                                m_Items.Add(targetItem);
                                continue;
                            }

                            if (craftContext.RecycleOption == CraftRecycleOption.RecycleAnyByType)
                            {
                                m_ItemsToRecycle.Add(targetItem);
                                continue;
                            }

                            if (craftContext.RecycleOption == CraftRecycleOption.RecycleEverything)
                            {
                                m_ItemsToRecycle.Add(targetItem);
                                continue;
                            }
                        }

                        foreach (Item recycleItem in m_Items)
                        {
                            if (!recycleItem.Movable) continue;
                            if (recycleItem.LootType != LootType.Regular) continue;
                            if (recycleItem.PlayerClassCurrencyValue > 0) continue;
                            if (recycleItem.QuestItem) continue;
                            if (recycleItem.Nontransferable) continue;
                            if (recycleItem.DonationItem) continue;
                            if (recycleItem.DecorativeEquipment) continue;
                            if (recycleItem.TierLevel > 0 && recycleItem.Aspect != AspectEnum.None) continue;

                            m_ItemsToRecycle.Add(recycleItem);
                        }

                        if (m_ItemsToRecycle.Count == 0)
                            continue;

                        Queue m_Queue = new Queue();

                        foreach (Item recycleItem in m_ItemsToRecycle)
                        {
                            m_Queue.Enqueue(recycleItem);
                        }                 

                        while (m_Queue.Count > 0)
                        {
                            Item recycleItem = (Item)m_Queue.Dequeue();

                            bool deleteItem = false;

                            foreach (KeyValuePair<Type, int> pair in m_ValidRecipeResources)
                            {
                                Type resourceType = pair.Key;
                                int totalResourceAmount = pair.Value * recycleItem.Amount;

                                if (totalResourceAmount < 2)
                                    continue;

                                //Ingot
                                if (resourceType == typeof(IronIngot))
                                {
                                    if (!m_RecycleSounds.Contains(0x2A))
                                        m_RecycleSounds.Add(0x2A);

                                    if (!m_RecycleSounds.Contains(0x240))
                                        m_RecycleSounds.Add(0x240);

                                    if (recycleItem.Resource != CraftResource.Iron)
                                    {
                                        resourceType = CraftResources.GetCraftResourceType(recycleItem.Resource);

                                        if (resourceType == null)
                                            resourceType = typeof(IronIngot);
                                    }
                                }

                                //Leather
                                if (resourceType == typeof(Leather))
                                {
                                    if (!m_RecycleSounds.Contains(0x3E3))
                                        m_RecycleSounds.Add(0x3E3);

                                    if (recycleItem.Resource != CraftResource.RegularLeather)
                                    {
                                        resourceType = CraftResources.GetCraftResourceType(recycleItem.Resource);

                                        if (resourceType == null)
                                            resourceType = typeof(Leather);
                                    }
                                }

                                //Wood
                                if (resourceType == typeof(Board))
                                {
                                    if (!m_RecycleSounds.Contains(0x23D))
                                        m_RecycleSounds.Add(0x23D);

                                    if (recycleItem.Resource != CraftResource.RegularWood)
                                    {
                                        resourceType = CraftResources.GetCraftResourceType(recycleItem.Resource);

                                        if (resourceType == null)
                                            resourceType = typeof(Board);
                                    }
                                }

                                Item newResource = (Item)Activator.CreateInstance(resourceType);

                                if (newResource == null)
                                    continue;

                                //Cloth
                                if (resourceType == typeof(Cloth))
                                {
                                    if (!m_RecycleSounds.Contains(0x248))
                                        m_RecycleSounds.Add(0x248);

                                    newResource.Hue = recycleItem.Hue;
                                }

                                deleteItem = true;
                                deletedCount++;

                                newResource.Amount = (int)(Math.Floor((double)totalResourceAmount / 2));
                                from.AddToBackpack(newResource);
                            }

                            int arcaneEssenceValue = recycleItem.GetArcaneEssenceValue();

                            if (arcaneEssenceValue > 0)
                            {
                                ArcaneEssence arcaneEssenceItem = new ArcaneEssence(arcaneEssenceValue);
                                from.AddToBackpack(arcaneEssenceItem);
                            }

                            if (deleteItem)
                                recycleItem.Delete();
                        }
                    }
                }

                if (deletedCount > 0)
                {
                    foreach (int sound in m_RecycleSounds)
                    {
                        from.PlaySound(sound);
                    }

                    if (deletedCount > 1)
                        return RecycleResult.SuccessMultiple;

                    else
                        return RecycleResult.Success;
                }

                else
                {
                    if (recycleEntireBackpack)
                        return RecycleResult.InvalidEntireBackpack;

                    else
                        return RecycleResult.Invalid;
                }
			}
		}
	}
}