namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the sort method used for browsing (e.g. Score, Id, etc.)
    /// </summary>
    public sealed class MediaSort : AniListEnum
    {
        private MediaSort(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaSort Id { get; } = new("ID", "Id", 0);
        public static MediaSort IdDesc { get; } = new("ID_DESC", "Id (Desc)", 1);
        public static MediaSort TitleRomaji { get; } = new("TITLE_ROMAJI", "Title - Romaji", 2);
        public static MediaSort TitleRomajiDesc { get; } = new("TITLE_ROMAJI_DESC", "Title - Romaji (Desc)", 3);
        public static MediaSort TitleEnglish { get; } = new("TITLE_ENGLISH", "Title - English", 4);
        public static MediaSort TitleEnglishDesc { get; } = new("TITLE_ENGLISH_DESC", "Title - English (Desc)", 5);
        public static MediaSort TitleNative { get; } = new("TITLE_NATIVE", "Title - Native", 6);
        public static MediaSort TitleNativeDesc { get; } = new("TITLE_NATIVE_DESC", "Title - Native (Desc)", 7);
        public static MediaSort Type { get; } = new("TYPE", "Type", 8);
        public static MediaSort TypeDesc { get; } = new("TYPE_DESC", "Type (Desc)", 9);
        public static MediaSort Format { get; } = new("FORMAT", "Format", 10);
        public static MediaSort FormatDesc { get; } = new("FORMAT_DESC", "Format (Desc)", 11);
        public static MediaSort StartDate { get; } = new("START_DATE", "Start Date", 12);
        public static MediaSort StartDateDesc { get; } = new("START_DATE_DESC", "Start Date (Desc)", 13);
        public static MediaSort EndDate { get; } = new("END_DATE", "End Date", 14);
        public static MediaSort EndDateDesc { get; } = new("END_DATE_DESC", "End Date (Desc)", 15);
        public static MediaSort Score { get; } = new("SCORE", "Score", 16);
        public static MediaSort ScoreDesc { get; } = new("SCORE_DESC", "Score (Desc)", 17);
        public static MediaSort Popularity { get; } = new("POPULARITY", "Popularity", 18);
        public static MediaSort PopularityDesc { get; } = new("POPULARITY_DESC", "Popularity (Desc)", 19);
        public static MediaSort Trending { get; } = new("TRENDING", "Trending", 20);
        public static MediaSort TrendingDesc { get; } = new("TRENDING_DESC", "Trending (Desc)", 21);
    }
}
