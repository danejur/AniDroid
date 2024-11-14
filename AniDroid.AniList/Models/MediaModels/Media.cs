using System.Collections.Generic;
using AniDroid.AniList.DataTypes;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.RecommendationModels;
using AniDroid.AniList.Models.ReviewModels;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniList.Models.StudioModels;

namespace AniDroid.AniList.Models.MediaModels
{
    public class Media : AniListObject
    {
        public int IdMal { get; set; }
        public MediaTitle Title { get; set; }
        public MediaType Type { get; set; }
        public MediaFormat Format { get; set; }
        public MediaStatus Status { get; set; }
        public string Description { get; set; }
        public FuzzyDate StartDate { get; set; }
        public FuzzyDate EndDate { get; set; }
        public MediaSeason Season { get; set; }
        public int? SeasonYear { get; set; }
        public int? Episodes { get; set; }
        public int? Duration { get; set; }
        public int? Chapters { get; set; }
        public int? Volumes { get; set; }
        public string CountryOfOrigin { get; set; }
        public bool IsLicensed { get; set; }
        public MediaSource Source { get; set; }
        public string Hashtag { get; set; }
        public MediaTrailer Trailer { get; set; }
        public int UpdatedAt { get; set; }
        public MediaCoverImage CoverImage { get; set; }
        public string BannerImage { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Synonyms { get; set; }
        public int AverageScore { get; set; }
        public int MeanScore { get; set; }
        public int Popularity { get; set; }
        public List<MediaTag> Tags { get; set; }
        public Connection<MediaEdge, Media> Relations { get; set; }
        public Connection<CharacterEdge, Character> Characters { get; set; }
        public Connection<StaffEdge, Staff> Staff { get; set; }
        public Connection<StudioEdge, Studio> Studios { get; set; }
        public Connection<ConnectionEdge<Recommendation>, Recommendation> Recommendations { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsAdult { get; set; }
        public MediaAiringSchedule NextAiringEpisode { get; set; }
        public Connection<ConnectionEdge<MediaAiringSchedule>, MediaAiringSchedule> AiringSchedule { get; set; }
        public List<MediaExternalLink> ExternalLinks { get; set; }
        public List<MediaStreaming> StreamingEpisodes { get; set; }
        public List<MediaRank> Rankings { get; set; }
        public MediaList MediaListEntry { get; set; }
        public Connection<ConnectionEdge<Review>, Review> Reviews { get; set; }
        public MediaStats Stats { get; set; }
        public string SiteUrl { get; set; }
        public bool AutoCreateForumThread { get; set; }
        public int Trending { get; set; }
        public Connection<ConnectionEdge<MediaTrend>, MediaTrend> Trends { get; set; }
        public Connection<ConnectionEdge<MediaTrend>, MediaTrend> AiringTrends { get; set; }

        public string GetFormattedDateRangeString()
        {
            var retString = "";

            if (StartDate.IsValid() && EndDate.IsValid())
            {
                retString = StartDate.Equals(EndDate) ? StartDate.GetFuzzyDateString() : $"{StartDate.GetFuzzyDateString()} to {EndDate.GetFuzzyDateString()}";
            }
            else if (StartDate.IsValid())
            {
                retString = $"{(Status == MediaStatus.NotYetReleased ? "Starts" : "Started")} {StartDate.GetFuzzyDateString()}";
            }
            else if (EndDate.IsValid())
            {
                retString = $"{(Status == MediaStatus.Finished || Status == MediaStatus.Cancelled ? "Ended" : "Ending")} {EndDate.GetFuzzyDateString()}";
            }

            return retString;
        }
    }
}
