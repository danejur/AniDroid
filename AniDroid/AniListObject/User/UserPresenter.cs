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

namespace AniDroid.AniListObject.User
{
    public class UserPresenter : BaseAniDroidPresenter<IUserView>
    {
        public UserPresenter(IUserView view, IAniListService service, IAniDroidSettings settings) : base(view, service, settings)
        {
        }

        public override async Task Init()
        {
            View.SetLoadingShown();
            var userId = View.GetUserId();
            var userName = View.GetUserName();

            var userResp = await AniListService.GetUser(userName, userId, default(CancellationToken));

            userResp.Switch(user =>
                {
                    View.SetShareText(user.Name, user.SiteUrl);
                    View.SetContentShown(false); // TODO: change this to switch based on banner presence
                    View.SetupToolbar(user.Name);
                    View.SetupUserView(user);
                })
                .Switch(error => View.OnError(error));
        }
    }
}