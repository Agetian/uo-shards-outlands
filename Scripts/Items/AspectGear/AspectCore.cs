using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class AspectCore : Item
    {
        [Constructable]
        public AspectCore(): base(3985)
		{
            Name = "an aspect core";

            Stackable = true;
            Amount = 1;
            Weight = 1;

            int aspectCount = Enum.GetNames(typeof(AspectEnum)).Length;
            Aspect = (AspectEnum)Utility.RandomMinMax(1, aspectCount - 1);
		}

        [Constructable]
        public AspectCore(int amount): base(3985)
		{
            Name = "an aspect core";

            Stackable = true;
            Amount = amount;
            Weight = 1;

            int aspectCount = Enum.GetNames(typeof(AspectEnum)).Length;
            Aspect = (AspectEnum)Utility.RandomMinMax(1, aspectCount - 1);
		}

        public override void AspectChange()
        {
            Hue = AspectGear.GetAspectHue(Aspect);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Amount > 1)
                LabelTo(from, AspectGear.GetAspectName(Aspect).ToLower() + " core : " + Amount.ToString());

            else
                LabelTo(from, AspectGear.GetAspectName(Aspect).ToLower() + " core");

        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use this with a mould and distillations of a matching aspect to create or upgrade aspect weapons and armor.");
        }

        public AspectCore(Serial serial): base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    #region Specific Cores

    public class AirCore : AspectCore
    {
        [Constructable]
        public AirCore(): base()
        {
            Name = "air Core";

            Aspect = AspectEnum.Air;
        }

        [Constructable]
        public AirCore(int amount): base(amount)
        {
            Name = "air Core";

            Amount = amount;

            Aspect = AspectEnum.Air;
        }

        public AirCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class CommandCore : AspectCore
    {
        [Constructable]
        public CommandCore(): base()
        {
            Name = "command Core";

            Aspect = AspectEnum.Command;
        }

        [Constructable]
        public CommandCore(int amount): base(amount)
        {
            Name = "command Core";

            Amount = amount;

            Aspect = AspectEnum.Command;
        }

        public CommandCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class EarthCore : AspectCore
    {
        [Constructable]
        public EarthCore(): base()
        {
            Name = "earth Core";

            Aspect = AspectEnum.Earth;
        }

        [Constructable]
        public EarthCore(int amount): base(amount)
        {
            Name = "earth Core";

            Amount = amount;

            Aspect = AspectEnum.Earth;
        }

        public EarthCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class EldritchCore : AspectCore
    {
        [Constructable]
        public EldritchCore(): base()
        {
            Name = "eldritch Core";

            Aspect = AspectEnum.Eldritch;
        }

        [Constructable]
        public EldritchCore(int amount): base(amount)
        {
            Name = "eldritch Core";

            Amount = amount;

            Aspect = AspectEnum.Eldritch;
        }

        public EldritchCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class FireCore : AspectCore
    {
        [Constructable]
        public FireCore(): base()
        {
            Name = "fire Core";

            Aspect = AspectEnum.Fire;
        }

        [Constructable]
        public FireCore(int amount): base(amount)
        {
            Name = "fire Core";

            Amount = amount;

            Aspect = AspectEnum.Fire;
        }

        public FireCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class HarmonyCore : AspectCore
    {
        [Constructable]
        public HarmonyCore(): base()
        {
            Name = "harmony Core";

            Aspect = AspectEnum.Harmony;
        }

        [Constructable]
        public HarmonyCore(int amount): base(amount)
        {
            Name = "harmony Core";

            Amount = amount;

            Aspect = AspectEnum.Harmony;
        }

        public HarmonyCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class PoisonCore : AspectCore
    {
        [Constructable]
        public PoisonCore(): base()
        {
            Name = "poison Core";

            Aspect = AspectEnum.Poison;
        }

        [Constructable]
        public PoisonCore(int amount): base(amount)
        {
            Name = "poison Core";

            Amount = amount;

            Aspect = AspectEnum.Poison;
        }

        public PoisonCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class ShadowCore : AspectCore
    {
        [Constructable]
        public ShadowCore(): base()
        {
            Name = "shadow Core";

            Aspect = AspectEnum.Shadow;
        }

        [Constructable]
        public ShadowCore(int amount): base(amount)
        {
            Name = "shadow Core";

            Amount = amount;

            Aspect = AspectEnum.Shadow;
        }

        public ShadowCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class VoidCore : AspectCore
    {
        [Constructable]
        public VoidCore(): base()
        {
            Name = "void Core";

            Aspect = AspectEnum.Void;
        }

        [Constructable]
        public VoidCore(int amount): base(amount)
        {
            Name = "void Core";

            Amount = amount;

            Aspect = AspectEnum.Void;
        }

        public VoidCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class WaterCore : AspectCore
    {
        [Constructable]
        public WaterCore(): base()
        {
            Name = "water Core";

            Aspect = AspectEnum.Water;
        }

        [Constructable]
        public WaterCore(int amount): base(amount)
        {
            Name = "water Core";

            Amount = amount;

            Aspect = AspectEnum.Water;
        }

        public WaterCore(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    #endregion
}