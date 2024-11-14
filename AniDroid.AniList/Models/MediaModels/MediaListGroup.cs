using System.Collections.Generic;
using AniDroid.AniList.Enums.MediaEnums;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaListGroup
    {
        public string Name { get; set; }
        public bool IsCustomList { get; set; }
        public bool IsSplitCompletedList { get; set; }
        public MediaListStatus Status { get; set; }
        public List<MediaList> Entries { get; set; }
    }
}