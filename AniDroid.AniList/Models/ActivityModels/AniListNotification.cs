using System.Collections.Generic;
using AniDroid.AniList.Enums.ActivityEnums;
using AniDroid.AniList.Models.ForumModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.ActivityModels
{
    public class AniListNotification : AniListObject
    {
        public NotificationType Type { get; set; }
        public string Context { get; set; }
        public List<string> Contexts { get; set; }
        public int CreatedAt { get; set; }
        public int Episode { get; set; }
        public int ActivityId { get; set; }
        public int CommentId { get; set; }
        public Media Media { get; set; }
        public User User { get; set; }
        public ForumThread Thread { get; set; }

        #region Display Methods

        public string GetNotificationHtml(string accentColor)
        {
            var notificationText = "Error occurred while parsing notification.";

            if (Type.Equals(NotificationType.ActivityMessage))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> sent you a message.";
            }
            else if (Type.Equals(NotificationType.ActivityReply))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> replied to your activity.";
            }
            else if (Type.Equals(NotificationType.Following))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> started following you.";
            }
            else if (Type.Equals(NotificationType.ActivityMention))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> mentioned you in their activity.";
            }
            else if (Type.Equals(NotificationType.ThreadCommentMention))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> mentioned you, in the forum thread <b><font color='{accentColor}'>{Thread?.Title}</font></b>.";
            }
            else if (Type.Equals(NotificationType.ThreadSubscribed))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> commented in your subscribed forum thread <b><font color='{accentColor}'>{Thread?.Title}</font></b>.";
            }
            else if (Type.Equals(NotificationType.ThreadCommentReply))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> replied to your comment, in the forum thread <b><font color='{accentColor}'>{Thread?.Title}</font></b>.";
            }
            else if (Type.Equals(NotificationType.Airing))
            {
                notificationText = $"Episode <b><font color='{accentColor}'>{Episode}</font></b> of <b><font color='{accentColor}'>{Media?.Title?.UserPreferred}</font></b> aired.";
            }
            else if (Type.Equals(NotificationType.ActivityLike))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> liked your activity.";
            }
            else if (Type.Equals(NotificationType.ActivityReplyLike))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> liked your activity reply.";
            }
            else if (Type.Equals(NotificationType.ThreadLike))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> liked your forum thread, <b><font color='{accentColor}'>{Thread?.Title}</font></b>.";
            }
            else if (Type.Equals(NotificationType.ThreadCommentLike))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> liked your comment, in the forum thread <b><font color='{accentColor}'>{Thread?.Title}</font></b>.";
            }
            else if (Type.Equals(NotificationType.ActivityReplySubscribed))
            {
                notificationText = $"<b><font color='{accentColor}'>{User?.Name}</font></b> replied to an activity you previously replied to.";
            }
            else if (Type.Equals(NotificationType.RelatedMediaAddition))
            {
                notificationText = $"<b><font color='{accentColor}'>{Media?.Title?.UserPreferred}</font></b> was recently added to the site.";
            }

            return notificationText;
        }

        public string GetImageUri()
        {
            var imageUrl = User?.Avatar?.Large;

            if (Type.EqualsAny(NotificationType.Airing, NotificationType.RelatedMediaAddition))
            {
                imageUrl = Media?.CoverImage?.Large;
            }

            return imageUrl;
        }

        public NotificationActionType GetNotificationActionType()
        {
            NotificationActionType returnType = null;

            if (Type.Equals(NotificationType.ActivityMessage) ||
                Type.Equals(NotificationType.ActivityReply) ||
                Type.Equals(NotificationType.ActivityMention) ||
                Type.Equals(NotificationType.ActivityLike) ||
                Type.Equals(NotificationType.ActivityReplyLike) ||
                Type.Equals(NotificationType.ActivityReplySubscribed))
            {
                returnType = NotificationActionType.Activity;
            }
            else if (Type.Equals(NotificationType.Following))
            {
                returnType = NotificationActionType.User;
            }
            else if (Type.Equals(NotificationType.ThreadCommentMention) ||
                     Type.Equals(NotificationType.ThreadSubscribed) ||
                     Type.Equals(NotificationType.ThreadCommentReply) ||
                     Type.Equals(NotificationType.ThreadLike) ||
                     Type.Equals(NotificationType.ThreadCommentLike))
            {
                returnType = NotificationActionType.Thread;
            }
            else if (Type.EqualsAny(NotificationType.Airing, NotificationType.RelatedMediaAddition))
            {
                returnType = NotificationActionType.Media;
            }

            return returnType;
        }

        #endregion
        
    }
}
