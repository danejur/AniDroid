namespace AniDroid.AniList.Enums.ActivityEnums
{
    public sealed class NotificationType : AniListEnum
    {
        private NotificationType(string val, string displayVal, int index) : base(val, displayVal, index) { }

        public static NotificationType ActivityMessage { get; } = new NotificationType("ACTIVITY_MESSAGE", "Activity Message", 0);
        public static NotificationType ActivityReply { get; } = new NotificationType("ACTIVITY_REPLY", "Activity Reply", 1);
        public static NotificationType Following { get; } = new NotificationType("FOLLOWING", "Following", 2);
        public static NotificationType ActivityMention { get; } = new NotificationType("ACTIVITY_MENTION", "Activity Mention", 3);
        public static NotificationType ThreadCommentMention { get; } = new NotificationType("THREAD_COMMENT_MENTION", "Thread Comment Mention", 4);
        public static NotificationType ThreadSubscribed { get; } = new NotificationType("THREAD_SUBSCRIBED", "Thread Subscribed", 5);
        public static NotificationType ThreadCommentReply { get; } = new NotificationType("THREAD_COMMENT_REPLY", "Thread Comment Reply", 6);
        public static NotificationType Airing { get; } = new NotificationType("AIRING", "Airing", 7);
        public static NotificationType ActivityLike { get; } = new NotificationType("ACTIVITY_LIKE", "ActivityLike", 8);
        public static NotificationType ActivityReplyLike { get; } = new NotificationType("ACTIVITY_REPLY_LIKE", "Activity Reply Like", 9);
        public static NotificationType ThreadLike { get; } = new NotificationType("THREAD_LIKE", "Thread Like", 10);
        public static NotificationType ThreadCommentLike { get; } = new NotificationType("THREAD_COMMENT_LIKE", "Thread Comment Like", 11);
        public static NotificationType ActivityReplySubscribed { get; } = new NotificationType("ACTIVITY_REPLY_SUBSCRIBED", "Activity Reply Subscribed", 12);
        public static NotificationType RelatedMediaAddition { get; } = new NotificationType("RELATED_MEDIA_ADDITION", "Related Media Addition", 13);
    }
}