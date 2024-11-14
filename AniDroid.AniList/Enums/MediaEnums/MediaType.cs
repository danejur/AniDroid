namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the type of Media (e.g. Anime or Manga)
    /// </summary>
    public sealed class MediaType : AniListEnum
    {
        private MediaType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaType Anime { get; } = new("ANIME", "Anime", 0);
        public static MediaType Manga { get; } = new("MANGA", "Manga", 1);
    }
}
