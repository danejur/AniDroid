using System;
using System.Text;

namespace AniDroid.AniList.Models
{
    public abstract class AniListObject
    {
        public int Id { get; set; }

        public DateTimeOffset GetDateTimeOffset(int sec)
        {
            return DateTimeOffset.FromUnixTimeSeconds(sec);
        }

        public string GetDurationString(long seconds, int precision = 0)
        {
            if (seconds < 60)
            {
                return $"{seconds} second{(seconds != 1 ? "s" : "")}";
            }

            if (seconds < 3600)
            {
                var ageMinutes = decimal.Round((decimal)seconds / 60, precision, MidpointRounding.AwayFromZero);
                return $"{ageMinutes} minute{(ageMinutes != 1 ? "s" : "")}";
            }

            if (seconds < 86400)
            {
                var ageHours = decimal.Round((decimal)seconds / 3600, precision, MidpointRounding.AwayFromZero);
                return $"{ageHours} hour{(ageHours != 1 ? "s" : "")}";
            }

            var ageDays = decimal.Round((decimal)seconds / 86400, precision, MidpointRounding.AwayFromZero);
            return $"{ageDays} day{(ageDays != 1 ? "s" : "")}";
        }

        public string GetExactDurationString(long seconds, bool includeSeconds = false)
        {
            var timespan = TimeSpan.FromSeconds(seconds);
            var formatString = new StringBuilder();

            if (timespan.Days > 0)
            {
                formatString.Append("%d'd '");
            }
            if (timespan.Hours > 0)
            {
                formatString.Append("%h'h '");
            }
            if (timespan.Minutes > 0)
            {
                formatString.Append("%m'm '");
            }
            if (timespan.Seconds > 0 && includeSeconds)
            {
                formatString.Append("%s's '");
            }

            return timespan.ToString(formatString.ToString()).Trim();
        }

        public string GetAgeString(long seconds)
        {
            return $"{GetDurationString(DateTimeOffset.Now.ToUnixTimeSeconds() - seconds)} ago";
        }

        public string GetFormattedDateString(long sec)
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(sec);
            return date.ToString("MMMM dd, yyyy");
        }
    }
}
