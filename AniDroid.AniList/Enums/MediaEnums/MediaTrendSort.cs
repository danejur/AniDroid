namespace AniDroid.AniList.Enums.MediaEnums
{
    public class MediaTrendSort : AniListEnum
    {
        private MediaTrendSort(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaTrendSort Id { get; } = new("ID", "Id", 0);
        public static MediaTrendSort IdDesc { get; } = new("ID_DESC", "Id (Desc)", 1);
        public static MediaTrendSort MediaId { get; } = new("MEDIA_ID", "Media Id", 2);
        public static MediaTrendSort MediaIdDesc { get; } = new("MEDIA_ID_DESC", "Media Id (Desc)", 3);
        public static MediaTrendSort Date { get; } = new("DATE", "Date", 4);
        public static MediaTrendSort DateDesc { get; } = new("DATE_DESC", "Date (Desc)", 5);
        public static MediaTrendSort Score { get; } = new("SCORE", "Score", 6);
        public static MediaTrendSort ScoreDesc { get; } = new("SCORE_DESC", "Score (Desc)", 7);
        public static MediaTrendSort Popularity { get; } = new("POPULARITY", "Popularity", 8);
        public static MediaTrendSort PopularityDesc { get; } = new("POPULARITY_DESC", "Popularity (Desc)", 9);
        public static MediaTrendSort Trending { get; } = new("TRENDING", "Trending", 10);
        public static MediaTrendSort TrendingDesc { get; } = new("TRENDING_DESC", "Trending (Desc)", 11);
        public static MediaTrendSort Episode { get; } = new("EPISODE", "Episode", 12);
        public static MediaTrendSort EpisodeDesc { get; } = new("EPISODE_DESC", "Episode (Desc)", 13);
    }
}
