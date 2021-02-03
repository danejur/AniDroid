using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using Google.Android.Material.Snackbar;

namespace AniDroid.Browse
{
    public class BrowsePresenter : BaseAniDroidPresenter<IBrowseView>, IAniListMediaListEditPresenter, IBrowsePresenter
    {
        private BrowseMediaDto _browseDto;

        public BrowsePresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public void BrowseAniListMedia(BrowseMediaDto browseDto)
        {
            _browseDto = browseDto;
            View.ShowMediaSearchResults(AniListService.BrowseMedia(browseDto, 20));
        }

        public override Task Init()
        {
            // TODO: does this need anything?
            return Task.CompletedTask;
        }

        public bool GetIsUserAuthenticated()
        {
            return AniDroidSettings.IsUserAuthenticated;
        }

        public User GetAuthenticatedUser()
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

        public BrowseMediaDto GetBrowseDto()
        {
            return _browseDto;
        }

        public IList<MediaTag> GetMediaTags()
        {
            return AniDroidSettings.MediaTagCache;
        }

        public IList<string> GetGenres()
        {
            return AniDroidSettings.GenreCache;
        }
    }
}