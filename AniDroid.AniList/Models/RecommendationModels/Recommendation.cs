using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.RecommendationModels
{
    public class Recommendation : AniListObject
    {
        public int Rating { get; set; }
        public Media Media { get; set; }
        public Media MediaRecommendation { get; set; }
        public User User { get; set; }
    }
}
