using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;

namespace AniDroid.AniListObject.Studio
{
    public class StudioPresenter : BaseAniDroidPresenter<IStudioView>
    {
        public StudioPresenter(IStudioView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var studioId = View.GetStudioId();
            var studioResp = await AniListService.GetStudioById(studioId, default(CancellationToken));

            studioResp.Switch(studio =>
                {
                    View.SetIsFavorite(studio.IsFavourite);
                    View.SetShareText(studio.Name, studio.SiteUrl);
                    View.SetContentShown();
                    View.SetupToolbar(studio.Name);
                    View.SetupStudioView(studio);
                })
                .Switch(error => View.OnError(error));
        }

        public IAsyncEnumerable<IPagedData<AniList.Models.Media.Edge>> GetStudioMediaEnumerable(int studioId, int perPage)
        {
            return AniListService.GetStudioMedia(studioId, perPage);
        }
    }
}