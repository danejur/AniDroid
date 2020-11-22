using System.Collections.Generic;
using System.Threading.Tasks;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.CurrentSeason
{
    public class CurrentSeasonPresenter : BaseAniDroidPresenter<ICurrentSeasonView>
    {
        private Media.MediaSort _sortType;

        public CurrentSeasonPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
            var titleLanguage = settings?.LoggedInUser?.Options?.TitleLanguage ??
                             AniList.Models.AniListObject.AniListTitleLanguage.English;

            if (titleLanguage.Equals(AniList.Models.AniListObject.AniListTitleLanguage.Native) ||
                titleLanguage.Equals(AniList.Models.AniListObject.AniListTitleLanguage.NativeStylised))
            {
                _sortType = Media.MediaSort.TitleEnglish;
            }
            else if (titleLanguage.Equals(AniList.Models.AniListObject.AniListTitleLanguage.Romaji) ||
                     titleLanguage.Equals(AniList.Models.AniListObject.AniListTitleLanguage.RomajiStylised))
            {
                _sortType = Media.MediaSort.TitleRomaji;
            }
            else
            {
                _sortType = Media.MediaSort.TitleEnglish;
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
                Season = Media.MediaSeason.Fall,
                SeasonYear = 2018,
                Type = Media.MediaType.Anime,
                Format = Media.MediaFormat.Tv,
                Sort = new List<Media.MediaSort> { _sortType }
            }, 5));

        }
    }
}