using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Support.Design.Widget;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.AniListObject.Media
{
    public class MediaPresenter : BaseAniDroidPresenter<IMediaView>, IAniListMediaListEditPresenter
    {
        public MediaPresenter(IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var mediaId = View.GetMediaId();
            var mediaResp = AniListService.GetMediaById(mediaId, default);
            var userResp = AniListService.GetCurrentUser(default);

            if (AniDroidSettings.IsUserAuthenticated)
            {
                View.SetCanEditListItem();
            }

            await Task.WhenAll(mediaResp, userResp);

            userResp.Result.Switch(user => View.SetCurrentUserMediaListOptions(user.MediaListOptions))
                .Switch(error => {
                    if (AniDroidSettings.IsUserAuthenticated)
                    {
                        View.DisplaySnackbarMessage("Error occurred while getting user settings", Snackbar.LengthLong);
                    }
                });

            mediaResp.Result.Switch(media =>
                {
                    View.SetIsFavorite(media.IsFavourite);
                    View.SetShareText(media.Title?.UserPreferred, media.SiteUrl);
                    View.SetContentShown(!string.IsNullOrWhiteSpace(media.BannerImage));
                    View.SetupToolbar(media.Title?.UserPreferred, media.BannerImage);
                    View.SetupMediaView(media);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.Character.Edge>, IAniListError>> GetMediaCharactersEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaCharacters(mediaId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.Staff.Edge>, IAniListError>> GetMediaStaffEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaStaff(mediaId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.Media.MediaList>, IAniListError>>
            GetMediaFollowingUsersMediaListsEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaFollowingUsersMediaLists(mediaId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> GetMediaReviewsEnumerable(int mediaId,
            int perPage)
        {
            return AniListService.GetMediaReviews(mediaId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> GetMediaForumThreadsEnumerable(int mediaId,
            int perPage)
        {
            return AniListService.GetMediaForumThreads(mediaId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Recommendation.Edge>, IAniListError>>
            GetMediaRecommendationsEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaRecommendations(mediaId, perPage);
        }

        public async Task ToggleFavorite()
        {
            var mediaId = View.GetMediaId();
            var mediaType = View.GetMediaType();

            var favDto = new FavoriteDto();

            if (mediaType == AniList.Models.Media.MediaType.Anime)
            {
                favDto.AnimeId = mediaId;
            }
            else
            {
                favDto.MangaId = mediaId;
            }

            var favResp = await AniListService.ToggleFavorite(favDto,
                default(CancellationToken));

            favResp.Switch(error => View.OnError(error))
                .Switch(favorites =>
                    View.SetIsFavorite(
                        (mediaType == AniList.Models.Media.MediaType.Anime
                            ? favorites.Anime?.Nodes?.Any(x => x.Id == mediaId)
                            : favorites.Manga?.Nodes?.Any(x => x.Id == mediaId)) == true, true));
        }

        public async Task SaveMediaListEntry(MediaListEditDto editDto, Action onSuccess, Action onError)
        {
            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default(CancellationToken));

            mediaUpdateResp.Switch(mediaList =>
                {
                    onSuccess();
                    View.DisplaySnackbarMessage("Saved", Snackbar.LengthShort);
                    View.UpdateMediaListItem(mediaList);
                })
                .Switch(error =>
                {
                    onError();
                });
        }

        public async Task DeleteMediaListEntry(int mediaListId, Action onSuccess, Action onError)
        {
            var mediaDeleteResp = await AniListService.DeleteMediaListEntry(mediaListId, default(CancellationToken));

            mediaDeleteResp.Switch((bool success) =>
            {
                onSuccess();
                View.DisplaySnackbarMessage("Deleted", Snackbar.LengthShort);
                View.RemoveMediaListItem();
            }).Switch(error => onError());
        }
    }
}