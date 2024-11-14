namespace AniDroid.AniList.Enums.MediaEnums
{
    public sealed class MediaTitleLanguage : AniListEnum
    {
        private MediaTitleLanguage(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaTitleLanguage Romaji { get; } = new("ROMAJI", "Romaji", 0);
        public static MediaTitleLanguage English { get; } = new("ENGLISH", "English", 1);
        public static MediaTitleLanguage Native { get; } = new("NATIVE", "Native", 2);
        public static MediaTitleLanguage RomajiStylised { get; } = new("ROMAJI_STYLISED", "Romaji Stylised", 3);
        public static MediaTitleLanguage EnglishStylised { get; } = new("ENGLISH_STYLISED", "English Stylised", 4);
        public static MediaTitleLanguage NativeStylised { get; } = new("NATIVE_STYLISED", "Native Stylised", 5);
    }
}
