using System.Collections.Generic;
using AniDroid.AniList.Enums.ActivityEnums;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.ActivityModels
{
    public class AniListActivity : AniListObject
    {
        public ActivityType Type { get; set; }
        public string SiteUrl { get; set; }
        public int CreatedAt { get; set; }
        public int ReplyCount { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<ActivityReply> Replies { get; set; }
        public List<User> Likes { get; set; }

        /*--- Message Activity ---*/
        public int RecipientId { get; set; }
        public int MessengerId { get; set; }
        public string Message { get; set; }
        public User Recipient { get; set; }
        public User Messenger { get; set; }

        /*--- Text Activity ---*/
        public string Text { get; set; }

        /*--- List Activity ---*/
        public string Status { get; set; }
        public string Progress { get; set; }
        public Media Media { get; set; }
    }
}
