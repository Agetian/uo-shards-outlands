using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class SkillMasteryScroll : Item
	{
		private SkillName m_Skill;
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return m_Skill; }
            set { m_Skill = value; }
        }

        public static SkillName[] Skills { get { return m_Skills; } }

		private static SkillName[] m_Skills = new SkillName[]
		{
            SkillName.Alchemy,
            SkillName.AnimalLore,
            SkillName.AnimalTaming,
            SkillName.ArmsLore,
            SkillName.Begging,
            SkillName.Blacksmith,
            SkillName.Camping,
            SkillName.Carpentry,
            SkillName.Cartography,
			SkillName.Cooking,
            SkillName.Discordance,
            SkillName.Fishing,
            SkillName.Inscribe,
            SkillName.Lockpicking,
            SkillName.Lumberjacking,
            SkillName.Mining,
            SkillName.Peacemaking,
            SkillName.Poisoning,
            SkillName.Provocation,
            SkillName.RemoveTrap,
            SkillName.SpiritSpeak,
			SkillName.Tailoring,           
            SkillName.Tinkering,
            SkillName.Veterinary,
		};    

        public static SkillName DetermineSkill()
        {            
            return m_Skills[Utility.RandomMinMax(0, m_Skills.Length - 1)];
        }

        [Constructable]
        public SkillMasteryScroll(): base(0x14F0)
        {
            Name = "skill mastery scroll";

            Hue = 2963;
            Weight = 1.0;

            m_Skill = DetermineSkill();
        }

		[Constructable]
		public SkillMasteryScroll( SkillName skill) : base( 0x14F0 )
		{
            Name = "skill mastery scroll";

			Hue = 2963;
			Weight = 1.0;

			m_Skill = skill;
		}

		public SkillMasteryScroll( Serial serial ) : base( serial )
		{
		}		
        
		public override void OnSingleClick( Mobile from )
		{
            LabelTo(from, "a skill mastery scroll");
            LabelTo(from, "(" + SkillCheck.GetSkillName(m_Skill) + ")");
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (from == null)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This must be in your backpack in order to use it.");
                return;
            }

            from.SendSound(0x055);

            from.CloseGump(typeof(SkillMasteryGump));
            from.SendGump(new SkillMasteryGump(from, SkillMasteryPageType.Scroll, this));
		}

		public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (target is PlayerMobile && ((PlayerMobile)target).Young)
            {
                from.SendMessage("Young players cannot may not acquire skill mastery scrolls.");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }   

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players may not acquire skill mastery scrolls.");
                return false;
            }

            return base.DropToItem(from, target, p);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players may not acquire skill mastery scrolls.");
                return false;
            }

            return base.OnDragLift(from);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
			writer.Write((int)m_Skill);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Skill = (SkillName)reader.ReadInt();
            }
		}
	}
}