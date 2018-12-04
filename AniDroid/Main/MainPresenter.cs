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
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using OneOf;

namespace AniDroid.Main
{
    public class MainPresenter : BaseAniDroidPresenter<IMainView>
    {
        public MainPresenter(IMainView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override Task Init()
        {
            // TODO: potentially update notifications here, or trigger update at least

            View.SetAuthenticatedNavigationVisibility(AniDroidSettings.IsUserAuthenticated);
            View.OnMainViewSetup();

            if (View.GetVersionCode() > AniDroidSettings.HighestVersionUsed)
            {
                View.DisplayWhatsNewDialog();
                AniDroidSettings.HighestVersionUsed = View.GetVersionCode();
            }

            return Task.CompletedTask;
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> GetNotificationsEnumerable()
        {
            return AniListService.GetAniListNotifications(true, 20);
        }

        public async Task GetUserNotificationCount()
        {
            var countResp = await AniListService.GetAniListNotificationCount(default);

            countResp.Switch((IAniListError error) => { })
                .Switch(user => View.SetNotificationCount(user.UnreadNotificationCount));
        }

        public bool GetIsUserAuthenticated()
        {
            return AniDroidSettings.IsUserAuthenticated;
        }
    }
}