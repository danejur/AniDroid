namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the list status of the Media (e.g. Current, Completed, etc.)
    /// </summary>
    public sealed class MediaListStatus : AniListEnum
    {
        private MediaListStatus(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaListStatus Current { get; } = new("CURRENT", "Current", 0);
        public static MediaListStatus Planning { get; } = new("PLANNING", "Planning", 1);
        public static MediaListStatus Completed { get; } = new("COMPLETED", "Completed", 2);
        public static MediaListStatus Dropped { get; } = new("DROPPED", "Dropped", 3);
        public static MediaListStatus Paused { get; } = new("PAUSED", "Paused", 4);
        public static MediaListStatus Repeating { get; } = new("REPEATING", "Repeating", 5);
    }
}
