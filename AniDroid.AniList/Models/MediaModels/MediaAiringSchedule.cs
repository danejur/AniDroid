using System;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaAiringSchedule : AniListObject
    {
        public int AiringAt { get; set; }
        public int TimeUntilAiring { get; set; }
        public int Episode { get; set; }
        public int MediaId { get; set; }
        public Media Media { get; set; }

        public DateTime GetAiringAtDateTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(AiringAt).DateTime;
        }

        public TimeSpan GetTimeUntilAiringTimeSpan()
        {
            return TimeSpan.FromSeconds(TimeUntilAiring);
        }
    }
}
