using System.Collections.Generic;

namespace AniDroid.AniList.Models.UserModels
{
    public class UserStats
    {
        public int WatchedTime { get; set; }
        public int ChaptersRead { get; set; }
        public List<UserActivityHistory> ActivityHistory { get; set; }
        public List<AniListStatusDistribution> AnimeStatusDistribution { get; set; }
        public List<AniListStatusDistribution> MangaStatusDistribution { get; set; }
        public List<AniListScoreDistribution> AnimeScoreDistribution { get; set; }
        public List<AniListScoreDistribution> MangaScoreDistribution { get; set; }
    }
}