namespace AniDroid.AniList.Enums.CharacterEnums
{
    public sealed class CharacterRole : AniListEnum
    {
        private CharacterRole(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static CharacterRole Main { get; } = new("MAIN", "Main", 0);
        public static CharacterRole Supporting { get; } = new("SUPPORTING", "Supporting", 1);
        public static CharacterRole Background { get; } = new("BACKGROUND", "Background", 2);
    }
}
