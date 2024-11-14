using System;

namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the season of the Media (e.g. Winter, Summer, etc.)
    /// </summary>
    public sealed class MediaSeason : AniListEnum
    {
        private MediaSeason(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaSeason Winter { get; } = new("WINTER", "Winter", 0);
        public static MediaSeason Spring { get; } = new("SPRING", "Spring", 1);
        public static MediaSeason Summer { get; } = new("SUMMER", "Summer", 2);
        public static MediaSeason Fall { get; } = new("FALL", "Fall", 3);

        public static MediaSeason GetFromDate(DateTime date)
        {
            return GetEnum<MediaSeason>((date.Month - 1) / 3);
        }
    }
}
