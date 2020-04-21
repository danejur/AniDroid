using System.Collections.Generic;
using System.Linq;
using AniDroid.AniList.Models;

namespace AniDroid.MediaList
{
    public class MediaListFilterModel
    {
        public string Title { get; set; }
        public Media.MediaSeason Season { get; set; }
        public Media.MediaStatus Status { get; set; } = null;
        public Media.MediaFormat Format { get; set; }
        public int? Year { get; set; }
        public Media.MediaSource Source { get; set; }

        public ICollection<string> IncludedGenres { get; set; }
        public ICollection<string> IncludedTags { get; set; }
        public ICollection<string> LicensedBy { get; set; }

        public bool IsFilteringActive => Season != null || Status != null || Format != null || Year != null ||
                                         Source != null || IncludedGenres?.Any() == true ||
                                         IncludedTags?.Any() == true || LicensedBy?.Any() == true ||
                                         !string.IsNullOrWhiteSpace(Title);
    }
}
