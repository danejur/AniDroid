using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniDroid.Torrent.NyaaSi
{
    public class NyaaSiConstants
    {
        public static class TorrentCategories
        {
            public const string AllCategories = "0_0";
            public const string Anime = "1_0";
            public const string Anime_MusicVideo = "1_1";
            public const string Anime_EnglishTranslated = "1_2";
            public const string Anime_NonEnglishTranslated = "1_3";
            public const string Anime_Raw = "1_4";
            public const string Audio = "2_0";
            public const string Audio_Lossless = "2_1";
            public const string Audio_Lossy = "2_2";
            public const string Literature = "3_0";
            public const string Literature_EnglishTranslated = "3_1";
            public const string Literature_NonEnglishTranslate = "3_2";
            public const string Literature_Raw = "3_3";
            public const string LiveAction = "4_0";
            public const string LiveAction_EnglishTranslated = "4_1";
            public const string LiveAction_PromotionalVideo = "4_2";
            public const string LiveAction_NonEnglishTranslated = "4_3";
            public const string LiveAction_Raw = "4_4";
            public const string Pictures = "5_0";
            public const string Pictures_Graphics = "5_1";
            public const string Pictures_Photos = "5_2";
            public const string Software = "6_0";
            public const string Software_Applications = "6_1";
            public const string Software_Games = "6_2";

            public static string GetDisplayCategory(string category)
            {
                return TorrentCategoryTuples.Any(x => x.Key == category)
                    ? TorrentCategoryTuples.FirstOrDefault(x => x.Key == category).Value
                    : DisplayTorrentCategories.Anime_Raw;
            }
        }

        public static class DisplayTorrentCategories
        {
            public const string AllCategories = "All Categories";
            public const string Anime = "Anime";
            public const string Anime_MusicVideo = "Anime - Anime Music Video";
            public const string Anime_EnglishTranslated = "Anime - English-translated";
            public const string Anime_NonEnglishTranslated = "Anime - Non-English-translated";
            public const string Anime_Raw = "Anime - Raw";
            public const string Audio = "Audio";
            public const string Audio_Lossless = "Audio - Lossless";
            public const string Audio_Lossy = "Audio - Lossy";
            public const string Literature = "Literature";
            public const string Literature_EnglishTranslated = "Literature - English-translated";
            public const string Literature_NonEnglishTranslate = "Literature - Non-English-translated";
            public const string Literature_Raw = "Literature - Raw";
            public const string LiveAction = "Live Action";
            public const string LiveAction_EnglishTranslated = "Live Action - English-translated";
            public const string LiveAction_PromotionalVideo = "Live Action - Idol/Promotional Video";
            public const string LiveAction_NonEnglishTranslated = "Live Action - Non-English-translated";
            public const string LiveAction_Raw = "Live Action - Raw";
            public const string Pictures = "Pictures";
            public const string Pictures_Graphics = "Pictures - Graphics";
            public const string Pictures_Photos = "Pictures - Photos";
            public const string Software = "Software";
            public const string Software_Applications = "Software - Applications";
            public const string Software_Games = "Software - Games";

            public static List<string> DisplayList { get { return TorrentCategoryTuples.Select(x => x.Value).ToList(); } }

            public static string GetCategory(string displayCategory)
            {
                return TorrentCategoryTuples.FirstOrDefault(x => x.Value == displayCategory).Key;
            }
        }

        public static class TorrentFilters
        {
            public const string NoFilter = "0";
            public const string NoRemakes = "2";
            public const string TrustedOnly = "3";

            public static string GetDisplayFilter(string filter)
            {
                return TorrentFilterTuples.FirstOrDefault(x => x.Key == filter).Value;
            }
        }

        public static class DisplayTorrentFilters
        {
            public const string NoFilter = "No filter";
            public const string NoRemakes = "No remakes";
            public const string TrustedOnly = "Trusted only";

            public static List<string> DisplayList { get { return TorrentFilterTuples.Select(x => x.Value).ToList(); } }

            public static string GetFilter(string displayFilter)
            {
                return TorrentFilterTuples.FirstOrDefault(x => x.Value == displayFilter).Key;
            }
        }

        public static readonly List<KeyValuePair<string, string>> TorrentCategoryTuples = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(TorrentCategories.AllCategories, DisplayTorrentCategories.AllCategories),
            new KeyValuePair<string, string>(TorrentCategories.Anime, DisplayTorrentCategories.Anime),
            new KeyValuePair<string, string>(TorrentCategories.Anime_MusicVideo, DisplayTorrentCategories.Anime_MusicVideo),
            new KeyValuePair<string, string>(TorrentCategories.Anime_EnglishTranslated, DisplayTorrentCategories.Anime_EnglishTranslated),
            new KeyValuePair<string, string>(TorrentCategories.Anime_NonEnglishTranslated, DisplayTorrentCategories.Anime_NonEnglishTranslated),
            new KeyValuePair<string, string>(TorrentCategories.Anime_Raw, DisplayTorrentCategories.Anime_Raw),
            new KeyValuePair<string, string>(TorrentCategories.Audio, DisplayTorrentCategories.Audio),
            new KeyValuePair<string, string>(TorrentCategories.Audio_Lossless, DisplayTorrentCategories.Audio_Lossless),
            new KeyValuePair<string, string>(TorrentCategories.Audio_Lossy, DisplayTorrentCategories.Audio_Lossy),
            new KeyValuePair<string, string>(TorrentCategories.Literature, DisplayTorrentCategories.Literature),
            new KeyValuePair<string, string>(TorrentCategories.Literature_EnglishTranslated, DisplayTorrentCategories.Literature_EnglishTranslated),
            new KeyValuePair<string, string>(TorrentCategories.Literature_NonEnglishTranslate, DisplayTorrentCategories.Literature_NonEnglishTranslate),
            new KeyValuePair<string, string>(TorrentCategories.Literature_Raw, DisplayTorrentCategories.Literature_Raw),
            new KeyValuePair<string, string>(TorrentCategories.LiveAction, DisplayTorrentCategories.LiveAction),
            new KeyValuePair<string, string>(TorrentCategories.LiveAction_EnglishTranslated, DisplayTorrentCategories.LiveAction_EnglishTranslated),
            new KeyValuePair<string, string>(TorrentCategories.LiveAction_PromotionalVideo, DisplayTorrentCategories.LiveAction_PromotionalVideo),
            new KeyValuePair<string, string>(TorrentCategories.LiveAction_NonEnglishTranslated, DisplayTorrentCategories.LiveAction_NonEnglishTranslated),
            new KeyValuePair<string, string>(TorrentCategories.LiveAction_Raw, DisplayTorrentCategories.LiveAction_Raw),
            new KeyValuePair<string, string>(TorrentCategories.Pictures, DisplayTorrentCategories.Pictures),
            new KeyValuePair<string, string>(TorrentCategories.Pictures_Graphics, DisplayTorrentCategories.Pictures_Graphics),
            new KeyValuePair<string, string>(TorrentCategories.Pictures_Photos, DisplayTorrentCategories.Pictures_Photos),
            new KeyValuePair<string, string>(TorrentCategories.Software, DisplayTorrentCategories.Software),
            new KeyValuePair<string, string>(TorrentCategories.Software_Applications, DisplayTorrentCategories.Software_Applications),
            new KeyValuePair<string, string>(TorrentCategories.Software_Games, DisplayTorrentCategories.Software_Games)
        };

        public static readonly List<KeyValuePair<string, string>> TorrentFilterTuples = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>(TorrentFilters.NoFilter, DisplayTorrentFilters.NoFilter),
            new KeyValuePair<string, string>(TorrentFilters.NoRemakes, DisplayTorrentFilters.NoRemakes),
            new KeyValuePair<string, string>(TorrentFilters.TrustedOnly, DisplayTorrentFilters.TrustedOnly),
        };

    }
}
