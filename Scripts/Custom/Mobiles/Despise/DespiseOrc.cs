﻿using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class DespiseOrc : BaseOrc
    {
        [Constructable]
        public DespiseOrc(): base()
        {           
            Body = Utility.RandomMinMax(400, 401);

            Name = "an orc";

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 125.1, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            switch (Utility.RandomMinMax(0, 2))
            {
                case 0:
                    SetSkill(SkillName.Swords, 90.1, 100);
                    var swords = new Item[] { new Halberd(), new Bardiche() };
                    AddItem(swords[Utility.Random(swords.Length)]);
                    break;
                case 1:
                    SetSkill(SkillName.Macing, 90.1, 100);
                    var maces = new Item[] { new WarHammer(), new QuarterStaff() };
                    AddItem(maces[Utility.Random(maces.Length)]);
                    break;
                case 2:
                    SetSkill(SkillName.Fencing, 90.1, 100);
                    var spears = new Item[] { new Spear(), new ShortSpear() };
                    AddItem(spears[Utility.Random(spears.Length)]);
                    break;
            }

            Karma = -15000;

            VirtualArmor = 50;
        }

        public DespiseOrc(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
