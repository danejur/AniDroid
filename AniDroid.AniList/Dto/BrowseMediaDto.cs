using System.Collections.Generic;
using AniDroid.AniList.Enums.MediaEnums;

namespace AniDroid.AniList.Dto
{
    public class BrowseMediaDto
    {
        // TODO: add rest of properties from query

        public ICollection<MediaSort> Sort { get; set; }
        public MediaType Type { get; set; }
        public MediaSeason Season { get; set; }
        public MediaStatus Status { get; set; } = null;
        public int? SeasonYear { get; set; }
        public MediaFormat Format { get; set; }
        public int? Year { get; set; }
        public string YearLike => Year.HasValue ? $"{Year}%" : null;
        public int? PopularityGreaterThan { get; set; }
        public int? AverageGreaterThan { get; set; }
        public int? EpisodesGreaterThan { get; set; }
        public int? EpisodesLessThan { get; set; }
        public MediaCountry Country { get; set; }
        public MediaSource Source { get; set; }

        public ICollection<string> IncludedGenres { get; set; }
        public ICollection<string> ExcludedGenres { get; set; }
        public ICollection<string> IncludedTags { get; set; }
        public ICollection<string> ExcludedTags { get; set; }

        public ICollection<string> LicensedBy { get; set; }
    }
}
