using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class UOACZFarmer : UOACZBaseCivilian
	{
        public override UOACZSystem.StockpileContributionType StockpileContributionType { get { return UOACZSystem.StockpileContributionType.Farmer; } }
        public override double StockpileContributionScalar { get { return 1.0; } }
        
        [Constructable]
		public UOACZFarmer() : base()
		{
            Title = "the farmer";

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(9, 18);

            SetSkill(SkillName.Fencing, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 3000;

            CreateRandomOutfit();

            AddItem(new Pitchfork() { Movable = false });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZFarmer(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
