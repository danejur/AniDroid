namespace AniDroid.AniList.Enums.RecommendationEnums
{
    public sealed class RecommendationRating : AniListEnum
    {
        private RecommendationRating(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static RecommendationRating NoRating { get; } = new("NO_RATING", "No Rating", 0);
        public static RecommendationRating RateUp { get; } = new("RATE_UP", "Rate Up", 1);
        public static RecommendationRating RateDown { get; } = new("RATE_DOWN", "Rate Down", 2);
    }
}