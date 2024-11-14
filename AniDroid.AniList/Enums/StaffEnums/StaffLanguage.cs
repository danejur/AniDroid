namespace AniDroid.AniList.Enums.StaffEnums
{
    public sealed class StaffLanguage : AniListEnum
    {
        private StaffLanguage(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static StaffLanguage Japanese { get; } = new("JAPANESE", "Japanese", 0);
        public static StaffLanguage English { get; } = new("ENGLISH", "English", 1);
        public static StaffLanguage Korean { get; } = new("KOREAN", "Korean", 2);
        public static StaffLanguage Italian { get; } = new("ITALIAN", "Japanese", 3);
        public static StaffLanguage Spanish { get; } = new("SPANISH", "Spanish", 4);
        public static StaffLanguage Portuguese { get; } = new("PORTUGUESE", "Portuguese", 5);
        public static StaffLanguage French { get; } = new("FRENCH", "French", 6);
        public static StaffLanguage German { get; } = new("GERMAN", "German", 7);
        public static StaffLanguage Hebrew { get; } = new("HEBREW", "Hebrew", 8);
        public static StaffLanguage Hungarian { get; } = new("HUNGARIAN", "Hungarian", 9);
    }
}
