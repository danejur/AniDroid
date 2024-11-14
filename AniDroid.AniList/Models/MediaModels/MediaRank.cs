using System.Globalization;
using AniDroid.AniList.Enums.MediaEnums;

namespace AniDroid.AniList.Models.MediaModels
{
    public class MediaRank
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public MediaRankType Type { get; set; }
        public MediaFormat Format { get; set; }
        public int? Year { get; set; }
        public MediaSeason Season { get; set; }
        public bool AllTime { get; set; }
        public string Context { get; set; }

        public string GetFormattedRankString()
        {
            return $"#{Rank} {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Context)}" +
                   (Season != null ? $" {Season.DisplayValue}" : "") +
                   (Year.HasValue ? $" {Year}" : "") +
                   (Format != null ? $" ({Format.DisplayValue})" : "");
        }
    }
}