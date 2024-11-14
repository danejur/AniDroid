namespace AniDroid.AniList.Enums.UserEnums
{
    public sealed class ScoreFormat : AniListEnum
    {
        private ScoreFormat(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static ScoreFormat Hundred { get; } = new("POINT_100", "100", 0);
        public static ScoreFormat TenDecimal { get; } = new("POINT_10_DECIMAL", "10.0", 1);
        public static ScoreFormat Ten { get; } = new("POINT_10", "10", 2);
        public static ScoreFormat FiveStars { get; } = new("POINT_5", "Five Stars", 3);
        public static ScoreFormat ThreeSmileys { get; } = new("POINT_3", "Three Smileys", 4);
    }
}
