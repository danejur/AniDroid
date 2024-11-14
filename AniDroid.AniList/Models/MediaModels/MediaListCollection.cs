using System.Collections.Generic;
using AniDroid.AniList.Models.UserModels;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaListCollection
    {
        public List<MediaListGroup> Lists { get; set; }
        public User User { get; set; }

    }
}