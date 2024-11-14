namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaTrend
    {
        public int MediaId { get; set; }
        public int Date { get; set; }
        public int Trending { get; set; }
        public int? AverageScore { get; set; }
        public int? Popularity { get; set; }
        public int? InProgress { get; set; }
        public bool Releasing { get; set; }
        public int? Episode { get; set; }

        public Media Media { get; set; }
    }
}
