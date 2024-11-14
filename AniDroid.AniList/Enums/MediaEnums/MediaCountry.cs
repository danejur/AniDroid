namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the country of origin of the Media (e.g. Japan, Korea, China)
    /// </summary>
    public sealed class MediaCountry : AniListEnum
    {
        public MediaCountry(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaCountry Japan { get; } = new("JP", "Japan", 0);
        public static MediaCountry Korea { get; } = new("KR", "Korea", 1);
        public static MediaCountry China { get; } = new("CN", "China", 2);
    }
}
