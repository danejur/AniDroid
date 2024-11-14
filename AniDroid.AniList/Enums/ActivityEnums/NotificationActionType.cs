namespace AniDroid.AniList.Enums.ActivityEnums
{
    public sealed class NotificationActionType : AniListEnum
    {
        private NotificationActionType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static NotificationActionType Media { get; } = new NotificationActionType("MEDIA", "Media", 0);
        public static NotificationActionType User { get; } = new NotificationActionType("USER", "User", 1);
        public static NotificationActionType Thread { get; } = new NotificationActionType("THREAD", "Thread", 2);
        public static NotificationActionType Comment { get; } = new NotificationActionType("COMMENT", "Comment", 3);
        public static NotificationActionType Activity { get; } = new NotificationActionType("ACTIVITY", "Activity", 4);
    }
}