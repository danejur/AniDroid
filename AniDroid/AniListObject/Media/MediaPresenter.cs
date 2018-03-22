using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Support.Design.Widget;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using OneOf;

namespace AniDroid.AniListObject.Media
{
    public class MediaPresenter : BaseAniDroidPresenter<IMediaView>, IAniListMediaListEditPresenter
    {
        public MediaPresenter(IMediaView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var mediaId = View.GetMediaId();
            var mediaResp = AniListService.GetMediaById(mediaId, default(CancellationToken));
            var userResp = AniListService.GetCurrentUser(default(CancellationToken));

            await Task.WhenAll(mediaResp, userResp);

            mediaResp.Result.Switch(media =>
                {
                    View.SetIsFavorite(media.IsFavourite);
                    View.SetShareText(media.Title?.UserPreferred, media.SiteUrl);
                    View.SetContentShown(!string.IsNullOrWhiteSpace(media.BannerImage));
                    View.SetupToolbar(media.Title?.UserPreferred, media.BannerImage);
                    View.SetupMediaView(media);
                })
                .Switch(error => View.OnError(error));

            userResp.Result.Switch(user => View.SetCurrentUserMediaListOptions(user.MediaListOptions))
                .Switch(error => {
                    if (AniDroidSettings.IsUserAuthenticated)
                    {
                        View.DisplaySnackbarMessage("Error occurred while getting user settings", Snackbar.LengthLong);
                    }
                });
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.Character.Edge>, IAniListError>> GetMediaCharactersEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaCharacters(mediaId, perPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniList.Models.Staff.Edge>, IAniListError>> GetMediaStaffEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaStaff(mediaId, perPage);
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

        public async Task SaveMediaListEntry(MediaListEditDto editDto)
        {
            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default(CancellationToken));

            mediaUpdateResp.Switch(mediaList =>
                {
                    View.DisplaySnackbarMessage("Saved", Snackbar.LengthShort);
                    View.UpdateMediaListItem(mediaList);
                })
                .Switch(error =>
                {
                    View.DisplaySnackbarMessage("Error occurred while saving list entry", Snackbar.LengthLong);
                    View.ShowMediaListEditDialog(new AniList.Models.Media.MediaList
                    {
                        Score = editDto.Score ?? 0,
                        Progress = editDto.Progress,
                        ProgressVolumes = editDto.ProgressVolumes,
                        Status = editDto.Status,
                        Notes = editDto.Notes,
                        Repeat = editDto.Repeat
                    });
                });
        }
    }
}