using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Models.UserModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using Task = System.Threading.Tasks.Task;

namespace AniDroid.Discover
{
    public class DiscoverPresenter : BaseAniDroidPresenter<IDiscoverView>, IAniListMediaListEditPresenter
    {
        public DiscoverPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public void GetDiscoverLists()
        {
            View.ShowCurrentlyAiringResults(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Season = MediaSeason.GetFromDate(DateTime.Now),
                SeasonYear = DateTime.Now.Year,
                Type = MediaType.Anime,
                Sort = new List<MediaSort> { MediaSort.PopularityDesc }
            }, 5));
            View.ShowTrendingAnimeResults(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Type = MediaType.Anime,
                Sort = new List<MediaSort> {MediaSort.TrendingDesc}
            }, 5));
            View.ShowTrendingMangaResults(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Type = MediaType.Manga,
                Sort = new List<MediaSort> { MediaSort.TrendingDesc }
            }, 5));
            View.ShowNewAnimeResults(AniListService.BrowseMedia(
                new BrowseMediaDto
                {
                    Type = MediaType.Anime,
                    Sort = new List<MediaSort> { MediaSort.IdDesc }
                }, 5));
            View.ShowNewMangaResults(AniListService.BrowseMedia(
                new BrowseMediaDto
                {
                    Type = MediaType.Manga,
                    Sort = new List<MediaSort> { MediaSort.IdDesc }
                }, 5));
        }

        public bool GetIsUserAuthenticated()
        {
            return AniDroidSettings.IsUserAuthenticated;
        }

        public User GetLoggedInUser()
        {
            return AniDroidSettings.LoggedInUser;
        }

        public async Task SaveMediaListEntry(MediaListEditDto editDto, Action onSuccess, Action onError)
        {
            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default);

            mediaUpdateResp.Switch(mediaList =>
            {
                onSuccess();
                View.DisplaySnackbarMessage("Saved", Snackbar.LengthShort);
                View.UpdateMediaListItem(mediaList);
            }).Switch(error => onError());
        }

        public async Task DeleteMediaListEntry(int mediaListId, Action onSuccess, Action onError)
        {
            var mediaDeleteResp = await AniListService.DeleteMediaListEntry(mediaListId, default);

            mediaDeleteResp.Switch((bool success) =>
            {
                onSuccess();
                View.DisplaySnackbarMessage("Deleted", Snackbar.LengthShort);
                View.RemoveMediaListItem(mediaListId);
            }).Switch(error =>
                onError());
        }
    }
}