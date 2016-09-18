using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class AspectDistillation : Item
    {
        [Constructable]
        public AspectDistillation(): base(17686)
        {
            Name = "aspect distillation";

            Stackable = true;
            Weight = .1;
            Amount = 1;

            Aspect = AspectGear.GetRandomAspect();
        }

        [Constructable]
        public AspectDistillation(int amount): base(17686)
        {
            Name = "aspect distillation";

            Stackable = true;
            Weight = .1;
            Amount = amount;

            Aspect = AspectGear.GetRandomAspect();
        }

        public AspectDistillation(Serial serial): base(serial)
        {
        }

        public override void AspectChange()
        {
            Hue = AspectGear.GetAspectHue(Aspect);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Amount > 1)
                LabelTo(from, AspectGear.GetAspectName(Aspect) + " Aspect Distillation : " + Amount.ToString());

            else
                LabelTo(from, AspectGear.GetAspectName(Aspect) + " Aspect Distillation");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use a smithing, carpentry, or tailoring mould and distillations of a matching Aspect to create or upgrade Aspect weapons and armor.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   

            //Version 0
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

    #region Specific Distillations    

    public class AirDistillation : AspectDistillation
    {
        [Constructable]
        public AirDistillation(): base()
        {
            Name = "air distillation";

            Aspect = AspectEnum.Air;
        }

        [Constructable]
        public AirDistillation(int amount): base(amount)
        {
            Name = "air distillation";
            
            Amount = amount;

            Aspect = AspectEnum.Air;
        }

        public AirDistillation(Serial serial): base(serial)
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

    public class CommandDistillation : AspectDistillation
    {
        [Constructable]
        public CommandDistillation(): base()
        {
            Name = "command distillation";

            Aspect = AspectEnum.Command;
        }

        [Constructable]
        public CommandDistillation(int amount): base(amount)
        {
            Name = "command distillation";

            Amount = amount;

            Aspect = AspectEnum.Command;
        }

        public CommandDistillation(Serial serial): base(serial)
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

    public class EarthDistillation : AspectDistillation
    {
        [Constructable]
        public EarthDistillation(): base()
        {
            Name = "earth distillation";

            Aspect = AspectEnum.Earth;
        }

        [Constructable]
        public EarthDistillation(int amount): base(amount)
        {
            Name = "earth distillation";

            Amount = amount;

            Aspect = AspectEnum.Earth;
        }

        public EarthDistillation(Serial serial): base(serial)
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

    public class EldritchDistillation : AspectDistillation
    {
        [Constructable]
        public EldritchDistillation(): base()
        {
            Name = "eldritch distillation";

            Aspect = AspectEnum.Eldritch;
        }

        [Constructable]
        public EldritchDistillation(int amount): base(amount)
        {
            Name = "eldritch distillation";

            Amount = amount;

            Aspect = AspectEnum.Eldritch;
        }

        public EldritchDistillation(Serial serial): base(serial)
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

    public class FireDistillation : AspectDistillation
    {
        [Constructable]
        public FireDistillation(): base()
        {
            Name = "fire distillation";

            Aspect = AspectEnum.Fire;
        }

        [Constructable]
        public FireDistillation(int amount): base(amount)
        {
            Name = "fire distillation";

            Amount = amount;

            Aspect = AspectEnum.Fire;
        }

        public FireDistillation(Serial serial): base(serial)
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

    public class LyricDistillation : AspectDistillation
    {
        [Constructable]
        public LyricDistillation()
            : base()
        {
            Name = "lyric distillation";

            Aspect = AspectEnum.Lyric;
        }

        [Constructable]
        public LyricDistillation(int amount)
            : base(amount)
        {
            Name = "lyric distillation";

            Amount = amount;

            Aspect = AspectEnum.Lyric;
        }

        public LyricDistillation(Serial serial): base(serial)
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

    public class PoisonDistillation : AspectDistillation
    {
        [Constructable]
        public PoisonDistillation(): base()
        {
            Name = "poison distillation";

            Aspect = AspectEnum.Poison;
        }

        [Constructable]
        public PoisonDistillation(int amount): base(amount)
        {
            Name = "poison distillation";

            Amount = amount;

            Aspect = AspectEnum.Poison;
        }

        public PoisonDistillation(Serial serial): base(serial)
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

    public class ShadowDistillation : AspectDistillation
    {
        [Constructable]
        public ShadowDistillation(): base()
        {
            Name = "shadow distillation";

            Aspect = AspectEnum.Shadow;
        }

        [Constructable]
        public ShadowDistillation(int amount): base(amount)
        {
            Name = "shadow distillation";

            Amount = amount;

            Aspect = AspectEnum.Shadow;
        }

        public ShadowDistillation(Serial serial): base(serial)
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

    public class VoidDistillation : AspectDistillation
    {
        [Constructable]
        public VoidDistillation(): base()
        {
            Name = "void distillation";

            Aspect = AspectEnum.Void;
        }

        [Constructable]
        public VoidDistillation(int amount): base(amount)
        {
            Name = "void distillation";

            Amount = amount;

            Aspect = AspectEnum.Void;
        }

        public VoidDistillation(Serial serial): base(serial)
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

    public class WaterDistillation : AspectDistillation
    {
        [Constructable]
        public WaterDistillation(): base()
        {
            Name = "water distillation";

            Aspect = AspectEnum.Water;
        }

        [Constructable]
        public WaterDistillation(int amount): base(amount)
        {
            Name = "water distillation";

            Amount = amount;

            Aspect = AspectEnum.Water;
        }

        public WaterDistillation(Serial serial): base(serial)
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