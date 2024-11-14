namespace AniDroid.AniList.Enums.UserEnums
{
    public sealed class FavoriteType : AniListEnum
    {
        private FavoriteType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static FavoriteType Anime { get; } = new("ANIME", "Anime", 0);
        public static FavoriteType Manga { get; } = new("MANGA", "Manga", 1);
        public static FavoriteType Character { get; } = new("CHARACTER", "Character", 2);
        public static FavoriteType Staff { get; } = new("STAFF", "Staff", 3);
        public static FavoriteType Studio { get; } = new("STUDIO", "Studio", 4);
    }
}