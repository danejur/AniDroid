using System.Collections.Generic;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.ActivityModels
{
    public class ActivityReply : AniListObject
    {
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string Text { get; set; }
        public int CreatedAt { get; set; }
        public User User { get; set; }
        public List<User> Likes { get; set; }
    }
}