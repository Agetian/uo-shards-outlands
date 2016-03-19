using System;
using Server;
using Server.Network;
using Server.Accounting;
using Server.Mobiles;
using System.Text;
using Server.Gumps;

namespace Server.Misc
{
    public class Profile
    {
        public static void Initialize()
        {
            EventSink.ProfileRequest += new ProfileRequestEventHandler(EventSink_ProfileRequest);
            EventSink.ChangeProfileRequest += new ChangeProfileRequestEventHandler(EventSink_ChangeProfileRequest);
        }

        public static void EventSink_ChangeProfileRequest(ChangeProfileRequestEventArgs e)
        {
            Mobile from = e.Beholder;

            if (from.ProfileLocked)
                from.SendMessage("Your profile is locked. You may not change it.");
            else
                from.Profile = e.Text;
        }

        public static void EventSink_ProfileRequest(ProfileRequestEventArgs e)
        {
            Mobile beholder = e.Beholder;
            Mobile beheld = e.Beheld;

            if (!beheld.Player)
                return;

            if (beholder.Map != beheld.Map || !beholder.InRange(beheld, 12) || !beholder.CanSee(beheld))
                return;

            string header = FameKarmaTitles.ComputeTitle(beholder, beheld, false);

            string footer = "";

            if (beheld.ProfileLocked)
            {
                if (beholder == beheld)
                    footer = "Your profile has been locked.";
                else if (beholder.AccessLevel >= AccessLevel.Counselor)
                    footer = "This profile has been locked.";
            }

            if (footer.Length == 0 && beholder == beheld)
            {
                footer = GetAccountDuration(beheld);
                footer = String.Concat(footer, "\n", GetCharacterAge(beheld));
            }

            footer = String.Concat(footer, "\n", GetPowerHourTimer(beheld));
            footer = String.Concat(footer, "\n", GetPreviousNames(beheld));

            string body = beheld.Profile;

            if (body == null || body.Length <= 0)
                body = "";

            beholder.Send(new DisplayProfile(beholder != beheld || !beheld.ProfileLocked, beheld, header, body, footer));
        }

        private static string GetPreviousNames(Mobile m)
        {
            var pm = m as PlayerMobile;

            if (pm != null)
            {
                var prevNames = pm.PreviousNames;

                if (prevNames == null || prevNames.Count == 0)
                {
                    return "This character has no known past aliases.";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Known past aliases:");
                    foreach (var name in prevNames)
                    {
                        sb.AppendLine(name);
                    }

                    return sb.ToString();
                }

            }

            return string.Empty;
        }

        private static string GetPowerHourTimer(Mobile m)
        {
            var pm = m as PlayerMobile;

            if (pm == null)
                return "";

            if (SkillCheck.InPowerHour(pm))
            {
                var left = pm.PowerHourReset.Add(pm.PowerHourDuration).Subtract(DateTime.UtcNow);
                return String.Format("You will be boosted in your skills training for {0:0} more minutes.", left.TotalMinutes);
            }
            else
            {
                if (DateTime.UtcNow > pm.PowerHourReset.Add(SkillCheck.PowerHourResetTime))
                {
                    return "You can now trigger your powerhour to boost your skills training.";
                }
                else
                {
                    return "You will be able to benefit from boosted skills training in " + RestedStateGump.TimeSince(DateTime.UtcNow, pm.PowerHourReset.Add(SkillCheck.PowerHourResetTime));
                }
            }
        }

        public static string GetAccountDuration(Mobile m)
        {
            Account a = m.Account as Account;

            if (a == null)
                return "";

            TimeSpan ts = DateTime.UtcNow - a.Created;

            string v;

            if (Format(ts.TotalDays, "This account is {0} day{1} old.", out v))
                return v;

            if (Format(ts.TotalHours, "This account is {0} hour{1} old.", out v))
                return v;

            if (Format(ts.TotalMinutes, "This account is {0} minute{1} old.", out v))
                return v;

            if (Format(ts.TotalSeconds, "This account is {0} second{1} old.", out v))
                return v;

            return "";
        }

        public static string GetCharacterAge(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;
            if (pm == null)
                return "";

            TimeSpan ts = DateTime.UtcNow - pm.CreatedOn;

            string v;

            if (Format(ts.TotalDays, "is {0} day{1} old.", out v))
                return String.Format("{0} {1}", pm.Name, v);

            if (Format(ts.TotalHours, "is {0} hour{1} old.", out v))
                return String.Format("{0} {1}", pm.Name, v);

            if (Format(ts.TotalMinutes, "is {0} minute{1} old.", out v))
                return String.Format("{0} {1}", pm.Name, v);

            if (Format(ts.TotalSeconds, "is {0} second{1} old.", out v))
                return String.Format("{0} {1}", pm.Name, v);

            return "";
        }

        public static bool Format(double value, string format, out string op)
        {
            if (value >= 1.0)
            {
                op = String.Format(format, (int)value, (int)value != 1 ? "s" : "");
                return true;
            }

            op = null;
            return false;
        }
    }
}
