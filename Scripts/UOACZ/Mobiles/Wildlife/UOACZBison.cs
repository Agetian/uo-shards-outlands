using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    [CorpseName("a bison corpse")]
    public class UOACZBison : UOACZBaseWildlife
	{
		[Constructable]
		public UOACZBison() : base()
		{
            Name = "a bison";
            Body = 232;
            BaseSoundID = 0x64;
            Hue = 1843;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 600;
            Karma = 0;
		}

        public override bool AlwaysFlee { get { return false; } }

        public override string CorruptedName { get { return "a corrupted bison"; } }
        public override string CorruptedCorpseName { get { return "a corrupted bison corpse"; } }

        public override double CrudeBoneArmorDropChance { get { return .20; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();            
        }

        public override void UOACZCarve(Mobile from, Corpse corpse, Item with)
        {
            base.UOACZCarve(from, corpse, with);

            if (Corrupted)
            {
                corpse.DropItem(new UOACZCorruptedRawSteak());
                corpse.DropItem(new UOACZCorruptedRawSteak());
            }

            else
            {
                corpse.DropItem(new UOACZRawSteak());
                corpse.DropItem(new UOACZRawCutsOfMeat());

                if (Utility.RandomDouble() <= .5)
                    corpse.DropItem(new UOACZRawSteak());
            }

            if (Utility.RandomDouble() <= .1)
                corpse.DropItem(new UOACZIntestines());

            corpse.DropItem(new Leather(5));
        }
        
        public UOACZBison(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
