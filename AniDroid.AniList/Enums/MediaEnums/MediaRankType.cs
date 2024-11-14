namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the ranking type of the Media (e.g. Rated, Popular)
    /// </summary>
    public sealed class MediaRankType : AniListEnum
    {
        private MediaRankType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaRankType Rated { get; } = new("RATED", "Rated", 0);
        public static MediaRankType Popular { get; } = new("POPULAR", "Popular", 1);
    }
}
