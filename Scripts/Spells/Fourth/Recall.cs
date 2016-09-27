using System;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Regions;
using Server.Custom.Items;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Spells.Fourth
{
	public class RecallSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Recall", "Kal Ort Por",
				239,
				9031,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;

        private RuneTomeRuneEntry m_RunebookRuneEntry;
        private RuneTome m_RuneTome;
		
		public override TimeSpan GetCastDelay() 
		{
            return TimeSpan.FromSeconds(1.75);
		}

		public RecallSpell( Mobile caster, Item scroll ) : this( caster, scroll, null, null, null, null )
		{
		}

		public RecallSpell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book, RuneTomeRuneEntry recallRuneEntry, RuneTome runeTome ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;

            m_RunebookRuneEntry = recallRuneEntry;
            m_RuneTome = runeTome;
		}

		public override void GetCastSkills( out double min, out double max )
		{
            if (m_Book != null || m_RuneTome != null)
            {
                min = 20;
                max = 20;
            }

            else
                base.GetCastSkills(out min, out max);
		}

		public override void OnCast()
		{
            if (m_Entry != null)
                Effect( m_Entry.Location, m_Entry.Map, true );

            else if (m_RunebookRuneEntry != null)
                Effect( m_RunebookRuneEntry.m_Target, m_RunebookRuneEntry.m_TargetMap, true );

            else
				Caster.Target = new InternalTarget( this );				
		}
        
		public override bool CheckCast()
		{
			PlayerMobile pm_Caster = Caster as PlayerMobile;

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.RecallOut, Caster.Location, Caster.Map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventRecallOutResponse != "")
                    Caster.SendMessage(recallBlocker.PreventRecallOutResponse);

                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultRecallOutResponse);

                return false;
            }
                        
			else if ( Caster.Criminal )
			{
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
            
			else if ( SpellHelper.CheckCombat( Caster ) )
			{
				Caster.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
				return false;
			}            

			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}

            if (pm_Caster != null)
            {
                if (pm_Caster.RecallRestrictionExpiration > DateTime.UtcNow)
                {   
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, pm_Caster.RecallRestrictionExpiration, false, true, true, true, true);

                    pm_Caster.SendMessage("You are unable to cast this spell for another " + timeRemaining + ".");

                    return false;
                }
            }

			return SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom );
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			PlayerMobile pm = Caster as PlayerMobile;

            BaseShip ship = BaseShip.FindShipAt(loc, map);

            WarpBlockerTotem recallBlocker = WarpBlockerTotem.RecallBlockerTriggered(Caster, WarpBlockerTotem.MovementMode.RecallIn, loc, map);

            if (recallBlocker != null)
            {
                if (recallBlocker.PreventRecallInResponse != "")
                    Caster.SendMessage(recallBlocker.PreventRecallInResponse);

                else
                    Caster.SendMessage(WarpBlockerTotem.DefaultRecallInResponse);
            }

            else if (map == null || (!Core.AOS && Caster.Map != map))
                Caster.SendLocalizedMessage(1005569); // You can not recall to another facet.			

            else if (!SpellHelper.CheckTravel(Caster, map, loc, TravelCheckType.RecallTo))
                Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...            

            else if (SpellHelper.IsAnyT2A(map, loc) && pm != null)
            {
            }

            else if (Server.Misc.WeightOverloading.IsOverloaded(Caster))
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.            

            else if (map != Map.Felucca)
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.

            else if (Caster.MurderCounts >= Mobile.MurderCountsRequiredForMurderer && map != Map.Felucca)
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.            

            else if (!SpellHelper.CheckIfOK(Caster.Map, loc.X, loc.Y, loc.Z))            
                Caster.SendLocalizedMessage(501942); // That location is blocked.               

            else if ((checkMulti && SpellHelper.CheckMulti(loc, map)))     
                Caster.SendLocalizedMessage(501942); // That location is blocked.  

            else if (SpellHelper.IsSolenHiveLoc(loc))
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.            

            else if (SpellHelper.IsStarRoom(loc))
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.            

            else if (SpellHelper.IsWindLoc(loc))
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.            

            else if (ship != null && ship.Owner != Caster)
                Caster.SendLocalizedMessage(1019004); // You are not allowed to travel there.            

            else if ((m_Book != null && m_Book.CurCharges <= 0) || (m_RuneTome != null && m_RuneTome.RecallCharges <= 0))
                Caster.SendMessage("There are no recall charges left on that item.");

            else if (CheckSequence())
            {
                if (m_Book != null)
                    --m_Book.CurCharges;

                if (m_RuneTome != null)
                    --m_RuneTome.RecallCharges;

                Point3D sourceLocation = Caster.Location;
                Map sourceMap = Caster.Map;

                Point3D targetLocation = loc;
                Map targetMap = map;

                //Player Enhancement Customization: PhaseShift
                bool phaseShift = false; //PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.PhaseShift);

                if (phaseShift)
                {
                    Caster.MoveToWorld(loc, map);

                    Effects.PlaySound(sourceLocation, sourceMap, 0x1FC);
                    Effects.SendLocationEffect(sourceLocation, sourceMap, 0x3967, 30, 15, 2499, 0);

                    Effects.PlaySound(targetLocation, targetMap, 0x1FC);
                    Effects.SendLocationEffect(targetLocation, targetMap, 0x3967, 30, 15, 2499, 0);
                }

                else
                {
                    Caster.PlaySound(0x1FC);
                    Caster.MoveToWorld(loc, map);
                    Caster.PlaySound(0x1FC);
                }
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private RecallSpell m_Owner;

			public InternalTarget( RecallSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;

				owner.Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 501029 ); // Select Marked item.
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );

					else
						from.SendLocalizedMessage( 501805 ); // That rune is not yet marked.
				}

				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );

					else
						from.SendLocalizedMessage( 502354 ); // Target is not marked.
				}

                else if (o is RuneTome)
                {
                    RuneTome runeTome = o as RuneTome;

                    RuneTomeRuneEntry defaultRuneEntry = null;

                    foreach (RuneTomeRuneEntry entry in runeTome.m_RecallRuneEntries)
                    {
                        if (entry == null)
                            continue;

                        if (entry.m_IsDefaultRune)
                        {
                            defaultRuneEntry = entry;
                            break;
                        }
                    }

                    if (defaultRuneEntry == null)
                    {
                        if (runeTome.m_RecallRuneEntries.Count > 0)
                            defaultRuneEntry = runeTome.m_RecallRuneEntries[0];

                        else
                        {
                            from.SendMessage("There are no recall runes stored within this rune tome.");
                            return;
                        }
                    }

                    if (defaultRuneEntry != null)
                        m_Owner.Effect(defaultRuneEntry.m_Target, defaultRuneEntry.m_TargetMap, true);
                }

                else if (o is ShipRune)
                {
                    ShipRune rune = (ShipRune)o;
                    BaseShip m_Ship;

                    if (rune.m_Ship != null)
                    {
                        m_Ship = rune.m_Ship;

                        if (m_Ship.Deleted)
                        {
                            from.SendMessage("The ship bound to this rune no longer exists.");
                            return;
                        }

                        if (m_Ship.Owner == from)
                        {
                            m_Ship.TransferEmbarkedMobile(from);
                            m_Owner.Effect(m_Ship.GetRandomEmbarkLocation(true), m_Ship.Map, false);
                        }
                        else
                            from.SendMessage("You must be the owner of that ship to use this rune.");
                    }

                    else
                        from.SendMessage("The ship bound to this rune no longer exists.");
                }

                else if (o is HouseRaffleDeed && ((HouseRaffleDeed)o).ValidLocation())
                {
                    HouseRaffleDeed deed = (HouseRaffleDeed)o;

                    m_Owner.Effect(deed.PlotLocation, deed.PlotFacet, true);
                }

                else
                    from.Send(new MessageLocalized(from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "")); // I can not recall from that object.				
			}
			
			protected override void OnNonlocalTarget( Mobile from, object o )
			{
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}