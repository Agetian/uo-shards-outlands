using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.SkillHandlers
{
	public class Anatomy
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Anatomy].Callback = new SkillUseCallback( OnUse );
		}        

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new Anatomy.InternalTarget();

			m.SendLocalizedMessage( 500321 ); // Whom shall I examine?

            return TimeSpan.FromSeconds(2.5);
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

				if ( from == targeted )				           
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500324); // You know yourself quite well enough already.				

				else if ( targeted is BaseVendor && ((BaseVendor)targeted).IsInvulnerable )				
					((BaseVendor)targeted).PrivateOverheadMessage( MessageType.Regular, 0x3B2, 500326, from.NetState ); // That can not be inspected.				

				else if ( targeted is Mobile )
				{
					Mobile targ = (Mobile)targeted;

					int marginOfError = Math.Max( 0, 25 - (int)(from.Skills[SkillName.Anatomy].Value / 4) );

					int str = targ.Str + Utility.RandomMinMax( -marginOfError, +marginOfError );
					int dex = targ.Dex + Utility.RandomMinMax( -marginOfError, +marginOfError );
					int stm = ((targ.Stam * 100) / Math.Max( targ.StamMax, 1 )) + Utility.RandomMinMax( -marginOfError, +marginOfError );

					int strMod = str / 10;
					int dexMod = dex / 10;
					int stmMod = stm / 10;

					if ( strMod < 0 )
                        strMod = 0;

					else if ( strMod > 10 ) 
                        strMod = 10;

					if ( dexMod < 0 )
                        dexMod = 0;

					else if ( dexMod > 10 )
                        dexMod = 10;

					if ( stmMod > 10 ) 
                        stmMod = 10;

					else if ( stmMod < 0 )
                        stmMod = 0;

                    bool gumpSuccess = false;

                    BaseCreature bc_Creature = targ as BaseCreature;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.IsHenchman)
                            gumpSuccess = true;
                    }

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.AnatomyCooldown * 1000);

					if ( from.CheckTargetSkill( SkillName.Anatomy, targ, 0, 100, 1.0))
					{
                        if (!gumpSuccess)
                        {
                            if (bc_Creature != null)
                            {
                                targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038045 + (strMod * 11) + dexMod, from.NetState); // That looks [strong] and [dexterous].

                                if (from.Skills[SkillName.Anatomy].Base >= 65.0)
                                    targ.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1038303 + stmMod, from.NetState); // That being is at [10,20,...] percent endurance.                                
                            }
                        }
                    }

					else if (!gumpSuccess)
						targ.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1042666, from.NetState ); // You can not quite get a sense of their physical characteristics.		
			
                    if (gumpSuccess)
                        from.SendGump(new AnimalLoreGump(player, bc_Creature, AnimalLoreGump.AnimalLoreGumpPage.Stats));
				}

				else if ( targeted is Item )				
					((Item)targeted).SendLocalizedMessageTo( from, 500323, "" ); // Only living things have anatomies!				
			}
		}
	}
}