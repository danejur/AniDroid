using System.Collections.Generic;
using System.Linq;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models;

namespace AniDroid.MediaList
{
    public class MediaListFilterModel
    {
        public string Title { get; set; }
        public MediaSeason Season { get; set; }
        public MediaStatus Status { get; set; } = null;
        public MediaFormat Format { get; set; }
        public int? Year { get; set; }
        public MediaSource Source { get; set; }

        public ICollection<string> IncludedGenres { get; set; }
        public ICollection<string> IncludedTags { get; set; }
        public ICollection<string> LicensedBy { get; set; }

        public bool FilteringPreviouslyActive { get; set; }

        public bool IsFilteringActive => Season != null || Status != null || Format != null || Year != null ||
                                         Source != null || IncludedGenres?.Any() == true ||
                                         IncludedTags?.Any() == true || LicensedBy?.Any() == true ||
                                         !string.IsNullOrWhiteSpace(Title);
    }
}
