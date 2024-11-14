namespace AniDroid.AniList.Enums.ReviewEnums
{
    public sealed class ReviewRating : AniListEnum
    {
        private ReviewRating(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static ReviewRating NoVote { get; } = new("NO_VOTE", "No Vote", 0);
        public static ReviewRating UpVote { get; } = new("UP_VOTE", "Up Vote", 1);
        public static ReviewRating DownVote { get; } = new("DOWN_VOTE", "Down Vote", 2);
    }
}
