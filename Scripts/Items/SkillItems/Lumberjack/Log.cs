using System;

namespace Server.Items
{
    [FlipableAttribute(0x1bdd, 0x1be0)]
    public class BaseLog : Item, ICommodity, IAxe
    {
        private CraftResource m_Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set { m_Resource = value; InvalidateProperties(); }
        }

        int ICommodity.DescriptionNumber { get { return CraftResources.IsStandard(m_Resource) ? LabelNumber : 1075062 + ((int)m_Resource - (int)CraftResource.RegularWood); } }
        bool ICommodity.IsDeedable { get { return true; } }
        
        [Constructable]
        public BaseLog(): this(1)
        {
        }

        [Constructable]
        public BaseLog(int amount): this(CraftResource.RegularWood, amount)
        {
        }

        [Constructable]
        public BaseLog(CraftResource resource): this(resource, 1)
        {
        }
        [Constructable]
        public BaseLog(CraftResource resource, int amount): base(0x1BDD)
        {
            Stackable = true;
            Weight = 2.0;
            Amount = amount;

            m_Resource = resource;
            Hue = CraftResources.GetHue(resource);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(m_Resource))
            {
                int num = CraftResources.GetLocalizationNumber(m_Resource);

                if (num > 0)
                    list.Add(num);

                else
                    list.Add(CraftResources.GetName(m_Resource));
            }
        }

        public BaseLog(Serial serial): base(serial)
        {
        }       

        public static bool UpdatingBaseLogClass;

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            BaseAxe axeFound = null;

            Item oneHand = from.FindItemOnLayer(Layer.OneHanded);
            Item twoHand = from.FindItemOnLayer(Layer.TwoHanded);

            if (oneHand is BaseAxe)
                axeFound = oneHand as BaseAxe;

            if (twoHand is BaseAxe)
                axeFound = twoHand as BaseAxe;

            if (axeFound == null && from.Backpack != null)
            {
                BaseAxe axe = from.Backpack.FindItemByType(typeof(BaseAxe)) as BaseAxe;

                if (axe != null)
                    axeFound = axe;
            }

            if (axeFound != null)
            {
                from.PlaySound(0x13E);

                if (Amount > 1)
                    from.SendMessage("You shape the logs into boards."); 

                else
                    from.SendMessage("You shape the log into a board.");

                Axe(from, axeFound);
            }

            else
                from.SendMessage("You must have an axe equipped or in your backpack to create boards.");
        }       

        public virtual bool TryCreateBoards(Mobile from, double skill, Item item)
        {
            if (Deleted || !from.CanSee(this))
                return false;

            base.ScissorHelper(from, item, 1, false);

            return true;
        }

        public virtual bool Axe(Mobile from, BaseAxe axe)
        {
            if (!TryCreateBoards(from, 0, new Board()))
                return false;

            return true;
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
            writer.Write((int)m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 1)
                UpdatingBaseLogClass = true;

            m_Resource = (CraftResource)reader.ReadInt();

            if (version == 0)
                m_Resource = CraftResource.RegularWood;
        }
    }

    public class Log : BaseLog
    {
        [Constructable]
        public Log(): this(1)
        {
            Name = "log";
        }

        [Constructable]
        public Log(int amount): base(CraftResource.RegularWood, amount)
        {
            Name = "log";
        }

        public Log(Serial serial): base(serial)
        {
            Name = "log";
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
          
            if (BaseLog.UpdatingBaseLogClass)
                return;

            int version = reader.ReadInt();
        }

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!TryCreateBoards(from, 0, new Board()))
                return false;

            return true;
        }
    }

    public class HeartwoodLog : BaseLog
    {
        [Constructable]
        public HeartwoodLog() : this(1)
        {
            Name = "heartwood log";
        }

        [Constructable]
        public HeartwoodLog(int amount): base(CraftResource.Heartwood, amount)
        {
            Name = "heartwood log";
        }

        public HeartwoodLog(Serial serial): base(serial)
        {
            Name = "heartwood log";
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 100, new HeartwoodBoard()))
                return false;

            return true;
        }
    }

    public class BloodwoodLog : BaseLog
    {
        [Constructable]
        public BloodwoodLog(): this(1)
        {
            Name = "bloodwood log";
        }

        [Constructable]
        public BloodwoodLog(int amount): base(CraftResource.Bloodwood, amount)
        {
            Name = "bloodwood log";
        }

        public BloodwoodLog(Serial serial): base(serial)
        {
            Name = "bloodwood log";
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 100, new BloodwoodBoard()))
                return false;

            return true;
        }
    }

    public class FrostwoodLog : BaseLog
    {
        [Constructable]
        public FrostwoodLog(): this(1)
        {
            Name = "frostwood log";
        }

        [Constructable]
        public FrostwoodLog(int amount): base(CraftResource.Frostwood, amount)
        {
            Name = "frostwood log";
        }

        public FrostwoodLog(Serial serial)
            : base(serial)
        {
            Name = "frostwood log";
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 100, new FrostwoodBoard()))
                return false;

            return true;
        }
    }

    public class OakLog : BaseLog
    {
        [Constructable]
        public OakLog(): this(1)
        {
            Name = "oak log";
        }

        [Constructable]
        public OakLog(int amount): base(CraftResource.OakWood, amount)
        {
            Name = "oak log";
        }

        public OakLog(Serial serial): base(serial)
        {
            Name = "oak log";
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 65, new OakBoard()))
                return false;

            return true;
        }
    }

    public class AshLog : BaseLog
    {
        [Constructable]
        public AshLog(): this(1)
        {
            Name = "ash log";
        }

        [Constructable]
        public AshLog(int amount): base(CraftResource.AshWood, amount)
        {
            Name = "ash log";
        }

        public AshLog(Serial serial): base(serial)
        {
            Name = "ash log";
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 80, new AshBoard()))
                return false;

            return true;
        }
    }

    public class YewLog : BaseLog
    {
        [Constructable]
        public YewLog(): this(1)
        {
            Name = "yew log";
        }

        [Constructable]
        public YewLog(int amount): base(CraftResource.YewWood, amount)
        {
            Name = "yew log";
        }

        public YewLog(Serial serial): base(serial)
        {
            Name = "yew log";
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 95, new YewBoard()))
                return false;

            return true;
        }
    }
}