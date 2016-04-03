using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public class WeightOverloading
	{
		public static void Initialize()
		{
			EventSink.Movement += new MovementEventHandler( EventSink_Movement );
		}

		public static void FatigueOnDamage( Mobile m, int damage, double scalar)
		{
            double fatigue = fatigue = ((double)damage / 5) * ((double)m.StamMax / 100) * scalar;					

            if (fatigue > 50)
                fatigue = 50;

            int finalFatigue = (int)fatigue;

            if (finalFatigue == 0 && damage >= 1)
            {
                if (Utility.RandomDouble() < .33)
                    finalFatigue = 1;
            }

			if ( fatigue > 0 )
                m.Stam -= finalFatigue;
		}

		public const int OverloadAllowance = 4; // We can be four stones overweight without getting fatigued

		public static int GetMaxWeight( Mobile m )
		{
			return 40 + (int)(3.5 * m.Str);
		}

		public static void EventSink_Movement( MovementEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !from.Alive || from.AccessLevel > AccessLevel.Player  )
				return;
            
			int maxWeight = GetMaxWeight( from ) + OverloadAllowance;
			int overWeight = (Mobile.BodyWeight + from.TotalWeight) - maxWeight;

			if ( overWeight > 0 )
			{
				from.Stam -= GetStamLoss( from, overWeight, (e.Direction & Direction.Running) != 0 );

				if ( from.Stam == 0 )
				{
					from.SendLocalizedMessage( 500109 ); // You are too fatigued to move, because you are carrying too much weight!
					e.Blocked = true;
					return;
				}
			}
            
            if (from is PlayerMobile)
            {
                int amt = (from.Mounted ? 48 : 21); // IPY
                PlayerMobile pm = (PlayerMobile)from;

                if ((++pm.StepsTaken % amt) == 0)
                    --from.Stam;
            }
		}

		public static int GetStamLoss( Mobile from, int overWeight, bool running )
		{
			int loss = 5 + (overWeight / 25);

			if ( from.Mounted )
				loss /= 3;

			if ( running )
				loss *= 2;

			return loss;
		}

		public static bool IsOverloaded( Mobile m )
		{
			if ( !m.Player || !m.Alive || m.AccessLevel > AccessLevel.Player )
				return false;

			return ( (Mobile.BodyWeight + m.TotalWeight) > (GetMaxWeight( m ) + OverloadAllowance) );
		}
	}
}