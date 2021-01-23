using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.CurrentSeason
{
    public class CurrentSeasonPresenter : BaseAniDroidPresenter<ICurrentSeasonView>
    {
        private MediaSort _sortType;

        public CurrentSeasonPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
            var titleLanguage = settings?.LoggedInUser?.Options?.TitleLanguage ??
                             MediaTitleLanguage.English;

            if (titleLanguage.Equals(MediaTitleLanguage.Native) ||
                titleLanguage.Equals(MediaTitleLanguage.NativeStylised))
            {
                _sortType = MediaSort.TitleEnglish;
            }
            else if (titleLanguage.Equals(MediaTitleLanguage.Romaji) ||
                     titleLanguage.Equals(MediaTitleLanguage.RomajiStylised))
            {
                _sortType = MediaSort.TitleRomaji;
            }
            else
            {
                _sortType = MediaSort.TitleEnglish;
            }
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public void GetCurrentSeasonLists()
        {
            View.ShowCurrentTv(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Season = MediaSeason.Fall,
                SeasonYear = 2018,
                Type = MediaType.Anime,
                Format = MediaFormat.Tv,
                Sort = new List<MediaSort> { _sortType }
            }, 5));

        }
    }
}