using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;

namespace AniDroid.AniListObject.Media
{
    public class MediaPresenter : BaseAniDroidPresenter<IMediaView>
    {
        public MediaPresenter(IMediaView view, IAniListService service) : base(view, service)
        {
        }

        public override async Task Init()
        {
            var mediaId = View.GetMediaId();
            var mediaResp = await AniListService.GetMediaById(mediaId, default(CancellationToken));

            mediaResp.Switch(media =>
                {
                    View.SetIsFavorite(media.IsFavourite);
                    View.SetShareText(media.Title?.UserPreferred, media.SiteUrl);
                    View.SetContentShown();
                    View.SetupToolbar(media.Title?.UserPreferred);
                    View.SetupMediaView(media);
                })
                .Switch(error => View.OnNetworkError());
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Character.Edge>> GetMediaCharactersEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaCharacters(mediaId, perPage);
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Staff.Edge>> GetMediaStaffEnumerable(int mediaId, int perPage)
        {
            return AniListService.GetMediaStaff(mediaId, perPage);
        }
    }
}