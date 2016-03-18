using System;

using Server;
using Server.Multis;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x14F0, 0x14EF)]
    public abstract class BaseAddonContainerDeed : Item, ICraftable
    {
        public abstract BaseAddonContainer Addon { get; }

        private CraftResource m_Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                if (m_Resource != value)
                {
                    m_Resource = value;
                    Hue = CraftResources.GetHue(m_Resource);

                    InvalidateProperties();
                }
            }
        }

        public BaseAddonContainerDeed() : base(0x14F0)
        {
            Weight = 1.0;

            if (!Core.AOS)
                LootType = LootType.Newbied;
        }

        public BaseAddonContainerDeed(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            // version 1
            writer.Write((int)m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Resource = (CraftResource)reader.ReadInt();
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(m_Resource))
                list.Add(CraftResources.GetLocalizationNumber(m_Resource));
        }

        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                Hue = 0;

            return quality;
        }
        #endregion

        private class InternalTarget : Target
        {
            private BaseAddonContainerDeed m_Deed;

            public InternalTarget(BaseAddonContainerDeed deed) : base(-1, true, TargetFlags.None)
            {
                m_Deed = deed;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || m_Deed.Deleted)
                    return;

                if (m_Deed.IsChildOf(from.Backpack))
                {
                    BaseAddonContainer addon = m_Deed.Addon;
                    addon.Resource = m_Deed.Resource;

                    Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;

                    AddonFitResult res = addon.CouldFit(p, map, from, ref house);

                    bool isMaxSecureReached = false;
                    //Check if secure slot is already full
                    if (house != null)
                    {
                        if (!house.IsAosRules && house.SecureCount >= house.MaxSecures)
                        {
                            // The maximum number of secure items has been reached : 
                            from.SendLocalizedMessage(1008142, true, house.MaxSecures.ToString());
                            isMaxSecureReached = true;
                        }
                        else if (house.IsAosRules ? !house.CheckAosLockdowns(1) : ((house.LockDownCount + 125) >= house.MaxLockDowns))
                        {
                            from.SendLocalizedMessage(1005379); // That would exceed the maximum lock down limit for this house
                            isMaxSecureReached = true;
                        }
                        else if (house.IsAosRules && !house.CheckAosStorage(addon.TotalItems))
                        {
                            from.SendLocalizedMessage(1061839); // This action would exceed the secure storage limit of the house.
                            isMaxSecureReached = true;
                        }
                    }
                    
                    if (!isMaxSecureReached)
                    {
                        switch (res)
                        {
                            case AddonFitResult.Valid:
                                addon.MoveToWorld(new Point3D(p), map);
                                break;
                            case AddonFitResult.Blocked:
                                @from.SendLocalizedMessage(500269); // You cannot build that there.
                                break;
                            case AddonFitResult.NotInHouse:
                                @from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                                break;
                            case AddonFitResult.DoorsNotClosed:
                                @from.SendMessage("You must close all house doors before placing this.");
                                break;
                            case AddonFitResult.DoorTooClose:
                                @from.SendLocalizedMessage(500271); // You cannot build near the door.
                                break;
                            case AddonFitResult.NoWall:
                                @from.SendLocalizedMessage(500268); // This object needs to be mounted on something.
                                break;
                        }
                    }
                    

                    if (res == AddonFitResult.Valid && house != null && !isMaxSecureReached)
                    {
                        m_Deed.Delete();
                        house.Addons.Add(addon);
                        house.AddSecure(from, addon);
                    }
                    else
                    {
                        addon.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
        }
    }
}
