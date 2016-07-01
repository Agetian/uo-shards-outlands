using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class PowerScroll : Item
	{
		private SkillName m_Skill;

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
            SkillName.Provocation,
            SkillName.SpiritSpeak,
			SkillName.Tailoring,           
            SkillName.Tinkering,
            SkillName.Veterinary,
		};

        #region Skill Groups

        private static SkillName[] m_LowValueSkills = new SkillName[]
		{
            SkillName.Alchemy, 
            SkillName.Blacksmith,
            SkillName.Carpentry,
            SkillName.Cartography,
            SkillName.Cooking,
            SkillName.Fishing,
            SkillName.Lumberjacking,
            SkillName.Mining,
            SkillName.Tinkering,
            SkillName.Tailoring,
            SkillName.Veterinary, 
		};

        private static SkillName[] m_HighValueSkills = new SkillName[]
		{
            SkillName.AnimalLore,
            SkillName.AnimalTaming,
            SkillName.ArmsLore,
            SkillName.Begging,
            SkillName.Camping, 
            SkillName.Discordance, 
            SkillName.Inscribe,
            SkillName.Peacemaking,
            SkillName.Provocation,
            SkillName.SpiritSpeak,
		};

        #endregion

        public static SkillName GetPowerscrollSkillName()
        {
            SkillName skillName = SkillName.Fishing;

            double chanceResult = Utility.RandomDouble();

            if (chanceResult <= .30)            
                skillName = m_LowValueSkills[Utility.RandomMinMax(0, m_LowValueSkills.Length - 1)];  

            else
                skillName = m_HighValueSkills[Utility.RandomMinMax(0, m_HighValueSkills.Length - 1)]; 

            return skillName;
        }

        [Constructable]
        public PowerScroll(): base(0x14F0)
        {
            Hue = 2657;
            Weight = 1.0;

            m_Skill = GetPowerscrollSkillName();
        }

		[Constructable]
		public PowerScroll( SkillName skill) : base( 0x14F0 )
		{
			Hue = 2657;
			Weight = 1.0;

			m_Skill = skill;
		}

		public PowerScroll( Serial serial ) : base( serial )
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get
			{
				return m_Skill;
			}
			set
			{
				m_Skill = value;
			}
		}

		private string GetNameLocalized()
		{
			return String.Concat( "#", (1044060 + (int)m_Skill).ToString() );
		}

		private string GetName()
		{
			int index = (int)m_Skill;
			
            SkillInfo[] table = SkillInfo.Table;

			if ( index >= 0 && index < table.Length )
				return table[index].Name.ToLower();
			else
				return "???";
		}
        
		public override void OnSingleClick( Mobile from )
		{
            LabelTo(from, "a power scroll");
            LabelTo(from, "(" + SkillCheck.GetSkillName(m_Skill) + ")");
		}

		public void Use( Mobile from, bool firstStage )
		{
			if ( Deleted )
				return;

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
                from.SendMessage("Young players cannot may not acquire power scrolls.");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }   

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players may not acquire power scrolls.");
                return false;
            }

            return base.DropToItem(from, target, p);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendMessage("Young players may not acquire power scrolls.");
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

		public class InternalGump : Gump
		{
			private Mobile m_Mobile;
			private PowerScroll m_Scroll;

			public InternalGump( Mobile mobile, PowerScroll scroll ) : base( 25, 50 )
			{
				m_Mobile = mobile;
				m_Scroll = scroll;

				AddPage( 0 );

				AddBackground( 25, 10, 420, 200, 5054 );

				AddImageTiled( 33, 20, 401, 181, 2624 );
				AddAlphaRegion( 33, 20, 401, 181 );

				AddHtmlLocalized( 40, 48, 387, 100, 1049469, true, true ); /* Using a scroll increases the maximum amount of a specific skill or your maximum statistics.
																			* When used, the effect is not immediately seen without a gain of points with that skill or statistics.
																			* You can view your maximum skill values in your skills window.
																			* You can view your maximum statistic value in your statistics window.
																			*/

				AddHtmlLocalized( 125, 148, 200, 20, 1049478, 0xFFFFFF, false, false ); // Do you wish to use this scroll?

				AddButton( 100, 172, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 135, 172, 120, 20, 1046362, 0xFFFFFF, false, false ); // Yes

				AddButton( 275, 172, 4005, 4007, 0, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 310, 172, 120, 20, 1046363, 0xFFFFFF, false, false ); // No

                double value = 100; //scroll.m_Value;

				if ( value == 105.0 )
					AddHtmlLocalized( 40, 20, 260, 20, 1049635, 0xFFFFFF, false, false ); // Wonderous Scroll (105 Skill):
				else if ( value == 110.0 )
					AddHtmlLocalized( 40, 20, 260, 20, 1049636, 0xFFFFFF, false, false ); // Exalted Scroll (110 Skill):
				else if ( value == 115.0 )
					AddHtmlLocalized( 40, 20, 260, 20, 1049637, 0xFFFFFF, false, false ); // Mythical Scroll (115 Skill):
				else if ( value == 120.0 )
					AddHtmlLocalized( 40, 20, 260, 20, 1049638, 0xFFFFFF, false, false ); // Legendary Scroll (120 Skill):
				else
					AddHtml( 40, 20, 260, 20, String.Format( "<basefont color=#FFFFFF>Power Scroll ({0} Skill):</basefont>", value ), false, false );

				AddHtmlLocalized( 310, 20, 120, 20, 1044060 + (int)scroll.m_Skill, 0xFFFFFF, false, false );
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				if ( info.ButtonID == 1 )
					m_Scroll.Use( m_Mobile, false );
			}
		}
	}
}