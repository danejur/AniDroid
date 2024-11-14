namespace AniDroid.AniList.Enums.MediaEnums
{
    /// <summary>
    /// Describes the license holder of the Media (e.g. Netflix, Amazon, etc.)
    /// </summary>
    public sealed class MediaLicensee : AniListEnum
    {
        public MediaLicensee(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static MediaLicensee Crunchyroll { get; } = new("Cruncyroll", "Crunchyroll", 0);
        public static MediaLicensee Funimation { get; } = new("Funimation", "Funimation", 1);
        public static MediaLicensee Netflix { get; } = new("Netflix", "Netflix", 2);
        public static MediaLicensee Amazon { get; } = new("Amazon", "Amazon", 3);
        public static MediaLicensee Hidive { get; } = new("Hidive", "Hidive", 4);
        public static MediaLicensee Hulu { get; } = new("Hulu", "Hulu", 5);
        public static MediaLicensee Animelab { get; } = new("Animelab", "Animelab", 6);
        public static MediaLicensee Vrv { get; } = new("VRV", "VRV", 7);
        public static MediaLicensee Viz { get; } = new("Viz", "Viz", 8);
        public static MediaLicensee MidnightPup { get; } = new("Midnight Pup", "Midnight Pup", 9);
        public static MediaLicensee TubiTv { get; } = new("Tubi TV", "Tubi TV", 10);
        public static MediaLicensee ConTv { get; } = new("CONtv", "CONtv", 11);
    }
}
