namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the format of Media (e.g. Tv, Manga, etc.)
    /// </summary>
    public sealed class MediaFormat : AniListEnum
    {
        public MediaType MediaType { get; private set; }

        private MediaFormat(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaFormat Tv { get; } = new("TV", "TV", 0) { MediaType = MediaType.Anime };
        public static MediaFormat TvShort { get; } = new("TV_SHORT", "TV Short", 1) { MediaType = MediaType.Anime };
        public static MediaFormat Movie { get; } = new("MOVIE", "Movie", 2) { MediaType = MediaType.Anime };
        public static MediaFormat Special { get; } = new("SPECIAL", "Special", 3) { MediaType = MediaType.Anime };
        public static MediaFormat Ova { get; } = new("OVA", "OVA", 4) { MediaType = MediaType.Anime };
        public static MediaFormat Ona { get; } = new("ONA", "ONA", 5) { MediaType = MediaType.Anime };
        public static MediaFormat Music { get; } = new("MUSIC", "Music", 6) { MediaType = MediaType.Anime };
        public static MediaFormat Manga { get; } = new("MANGA", "Manga", 7) { MediaType = MediaType.Manga };
        public static MediaFormat Novel { get; } = new("NOVEL", "Novel", 8) { MediaType = MediaType.Manga };
        public static MediaFormat OneShot { get; } = new("ONE_SHOT", "One Shot", 9) { MediaType = MediaType.Manga };
    }
}
