namespace AniDroid.AniList.Enums
{
    public sealed class LikeableType : AniListEnum
    {
        private LikeableType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static LikeableType Thread { get; } = new("THREAD", "Thread", 0);
        public static LikeableType ThreadComment { get; } = new("THREAD_COMMENT", "Thread Comment", 1);
        public static LikeableType Activity { get; } = new("ACTIVITY", "Activity", 2);
        public static LikeableType ActivityReply { get; } = new("ACTIVITY_REPLY", "Activity Reply", 3);
    }
}
