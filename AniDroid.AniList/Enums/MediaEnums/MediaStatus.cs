namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the satus of the Media (e.g. Finished, Releasing, etc.)
    /// </summary>
    public sealed class MediaStatus : AniListEnum
    {
        private MediaStatus(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaStatus Finished { get; } = new("FINISHED", "Finished", 0);
        public static MediaStatus Releasing { get; } = new("RELEASING", "Releasing", 1);
        public static MediaStatus NotYetReleased { get; } = new("NOT_YET_RELEASED", "Not Yet Released", 2);
        public static MediaStatus Cancelled { get; } = new("CANCELLED", "Cancelled", 3);
        public static MediaStatus Hiatus { get; } = new("HIATUS", "Hiatus", 4);
    }
}
