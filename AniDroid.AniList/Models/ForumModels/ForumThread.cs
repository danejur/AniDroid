using System.Collections.Generic;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.ForumModels
{
    public class ForumThread : AniListObject
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int UserId { get; set; }
        public int ReplyUserId { get; set; }
        public int ReplyCommentId { get; set; }
        public int ReplyCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsLocked { get; set; }
        public bool IsSticky { get; set; }
        public bool IsSubscribed { get; set; }
        public int RepliedAt { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public User User { get; set; }
        public User ReplyUser { get; set; }
        public List<User> Likes { get; set; }
        public string SiteUrl { get; set; }
        public List<ForumThreadCategory> Categories { get; set; }
        public List<Media> MediaCategories { get; set; }
    }
}