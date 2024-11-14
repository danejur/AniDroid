namespace AniDroid.AniList.Enums.UserEnums
{
    public sealed class UserSort : AniListEnum
    {
        private UserSort(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static UserSort Id { get; } = new("ID", "Id", 0);
        public static UserSort IdDesc { get; } = new("ID_DESC", "Id (Desc)", 1);
        public static UserSort Username { get; } = new("USERNAME", "Username", 2);
        public static UserSort UsernameDesc { get; } = new("USERNAME_DESC", "Username (Desc)", 3);
        public static UserSort WatchedTime { get; } = new("WATCHED_TIME", "Watched Time", 4);
        public static UserSort WatchedTimeDesc { get; } = new("WATCHED_TIME_DESC", "Watched Time (Desc)", 5);
        public static UserSort ChaptersRead { get; } = new("CHAPTERS_READ", "Chapters Read", 6);
        public static UserSort ChaptersReadDesc { get; } = new("CHAPTERS_READ_DESC", "Chapters Read (Desc)", 7);
        public static UserSort SearchMatch { get; } = new("SEARCH_MATCH", "Search Match", 8);
    }
}