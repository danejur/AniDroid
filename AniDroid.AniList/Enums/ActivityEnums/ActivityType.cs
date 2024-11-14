namespace AniDroid.AniList.Enums.ActivityEnums
{
    public sealed class ActivityType : AniListEnum
    {
        private ActivityType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static ActivityType Text { get; } = new("TEXT", "Text", 0);
        public static ActivityType AnimeList { get; } = new("ANIME_LIST", "Anime List", 0);
        public static ActivityType MangaList { get; } = new("MANGA_LIST", "Manga List", 0);
        public static ActivityType Message { get; } = new("MESSAGE", "Message", 0);
        public static ActivityType MediaList { get; } = new("MEDIA_LIST", "Media List", 0);
    }
}
