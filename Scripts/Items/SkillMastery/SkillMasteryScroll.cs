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
            SkillName.Lumberjacking,
            SkillName.Mining,
            SkillName.Peacemaking,
            SkillName.Poisoning,
            SkillName.Provocation,
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

		public void Use( Mobile from, bool firstStage )
		{
			if ( Deleted )
				return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            //TEST: Create Gump

            /*
			if ( IsChildOf( from.Backpack ) )
			{
				Skill skill = from.Skills[m_Skill];

				if ( skill != null )
				{
					if ( skill.Cap >= PlayerMobile.MaxBonusSkillCap )
					{
						from.SendLocalizedMessage( 1049511, GetNameLocalized() ); // Your ~1_type~ is too high for this power scroll.
					}

					else
					{
						if ( firstStage )
						{
							from.CloseGump( typeof( StatCapScroll.InternalGump ) );
							from.CloseGump( typeof( PowerScroll.InternalGump ) );
							from.SendGump( new InternalGump( from, this ) );
						}
						else
						{
                            if (m_Value - skill.Cap > 5.0)
                            {
                                from.SendMessage("You must use scrolls of power in order.");
                                return;
                            }
							from.SendLocalizedMessage( 1049513, GetNameLocalized() ); // You feel a surge of magic as the scroll enhances your ~1_type~!

							skill.Cap = m_Value;

							Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );
							Effects.PlaySound( from.Location, from.Map, 0x243 );

							Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 6, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
							Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 4, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
							Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 6, from.Y - 4, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );

							Effects.SendTargetParticles( from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100 );

							Delete();
						}
					}
				}
			}
             * 
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
            */
		}

		public override void OnDoubleClick( Mobile from )
		{
			Use( from, true );
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