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

namespace AniDroid.MediaList
{
    public class MediaListPresenter : BaseAniDroidPresenter<IMediaListView>
    {
        public MediaListPresenter(IMediaListView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public async Task GetMediaLists()
        {
            var mediaListResp = await AniListService.GetUserMediaList(AniDroidSettings.LoggedInUser?.Id ?? 0,
                View.GetMediaType(), default(CancellationToken));

            mediaListResp.Switch(error => View.OnError(error))
                .Switch(mediaLists => View.DisplayMediaLists(mediaLists));
        }
    }
}