using System;
using Server.Network;
using Server.Commands;
using Server.Mobiles;
using Server.Accounting;
using Server;

namespace Server.Misc
{
	public class LoginStats
	{
		public static void Initialize()
		{
			// Register our event handler
			EventSink.Login += new LoginEventHandler( EventSink_Login );
        }
        
		private static void EventSink_Login( LoginEventArgs args )
		{
			int userCount = Server.RemoteAdmin.ServerInfo.NetStateCount();
			int itemCount = World.Items.Count;
			int mobileCount = World.Mobiles.Count;

			Mobile m = args.Mobile;

			if (m.AccessLevel >= AccessLevel.Counselor)
			{
			    m.SendMessage("Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.",
				args.Mobile.Name,
				userCount == 1 ? "is" : "are",
				userCount, userCount == 1 ? "" : "s",
				itemCount, itemCount == 1 ? "" : "s",
				mobileCount, mobileCount == 1 ? "" : "s");
			}

			else
			    m.SendMessage("Welcome {0}!", args.Mobile.Name);

			//m.SendGump( new Server.Gumps.WelcomeGump(0) );
		}
	}
}