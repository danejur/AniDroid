namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the source of the Media (e.g. Original, Light Novel, etc.)
    /// </summary>
    public sealed class MediaSource : AniListEnum
    {
        private MediaSource(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaSource Original { get; } = new("ORIGINAL", "Original", 0);
        public static MediaSource Manga { get; } = new("MANGA", "Manga", 1);
        public static MediaSource LightNovel { get; } = new("LIGHT_NOVEL", "Light Novel", 2);
        public static MediaSource VisualNovel { get; } = new("VISUAL_NOVEL", "Visual Novel", 3);
        public static MediaSource VideoGame { get; } = new("VIDEO_GAME", "Video Game", 4);
        public static MediaSource Other { get; } = new("OTHER", "Other", 5);
    }
}
