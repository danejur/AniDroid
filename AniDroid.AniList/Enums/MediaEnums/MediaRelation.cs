namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the relation of the Media to a related Media (e.g. Adaptation, Sequel, etc.)
    /// </summary>
    public sealed class MediaRelation : AniListEnum
    {
        private MediaRelation(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaRelation Adaptation { get; } = new("ADAPTATION", "Adaptation", 0);
        public static MediaRelation Prequel { get; } = new("PREQUEL", "Prequel", 1);
        public static MediaRelation Sequel { get; } = new("SEQUEL", "Sequel", 2);
        public static MediaRelation Parent { get; } = new("PARENT", "Parent", 3);
        public static MediaRelation SideStory { get; } = new("SIDE_STORY", "Side Story", 4);
        public static MediaRelation Character { get; } = new("CHARACTER", "Character", 5);
        public static MediaRelation Summary { get; } = new("SUMMARY", "Summary", 6);
        public static MediaRelation Alternative { get; } = new("ALTERNATIVE", "Alternative", 7);
        public static MediaRelation SpinOff { get; } = new("SPIN_OFF", "Spin-off", 8);
        public static MediaRelation Other { get; } = new("OTHER", "Other", 9);
    }
}
