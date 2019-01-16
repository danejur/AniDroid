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
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using Task = System.Threading.Tasks.Task;

namespace AniDroid.Discover
{
    public class DiscoverPresenter : BaseAniDroidPresenter<IDiscoverView>, IAniListMediaListEditPresenter
    {
        public DiscoverPresenter(IDiscoverView view, IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(view, service, settings, logger)
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
                Season = Media.MediaSeason.GetFromDate(DateTime.Now),
                SeasonYear = DateTime.Now.Year,
                Type = Media.MediaType.Anime,
                Sort = new List<Media.MediaSort> { Media.MediaSort.PopularityDesc }
            }, 5));
            View.ShowTrendingResults(AniListService.BrowseMedia(new BrowseMediaDto
            {
                Sort = new List<Media.MediaSort> {Media.MediaSort.TrendingDesc}
            }, 5));
            View.ShowNewAnimeResults(AniListService.BrowseMedia(
                new BrowseMediaDto
                {
                    Type = Media.MediaType.Anime,
                    Sort = new List<Media.MediaSort> { Media.MediaSort.IdDesc }
                }, 5));
            View.ShowNewMangaResults(AniListService.BrowseMedia(
                new BrowseMediaDto
                {
                    Type = Media.MediaType.Manga,
                    Sort = new List<Media.MediaSort> { Media.MediaSort.IdDesc }
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