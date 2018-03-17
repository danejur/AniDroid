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

namespace AniDroid.Login
{
    public class LoginPresenter : BaseAniDroidPresenter<ILoginView>
    {
        private readonly IAniListAuthConfig _authConfig;

        public LoginPresenter(ILoginView view, IAniListService service, IAniDroidSettings settings, IAniListAuthConfig authConfig) : base(view, service, settings)
        {
            _authConfig = authConfig;
        }

        public override async Task Init()
        {
            AniDroidSettings.ClearUserAuthentication();
            var authCode = View.GetAuthCode();

            if (string.IsNullOrWhiteSpace(authCode))
            {
                View.OnErrorAuthorizing();
                return;
            }

            var authResp = await AniListService.AuthenticateUser(_authConfig, authCode, default(CancellationToken));

            if (!authResp.IsSuccessful || string.IsNullOrWhiteSpace(authResp.Data?.AccessToken))
            {
                View.OnErrorAuthorizing();
            }
            else
            {
                AniDroidSettings.UserAccessCode = authResp.Data.AccessToken;

                var currentUser = await AniListService.GetCurrentUser(default(CancellationToken));

                currentUser.Switch((IAniListError error) =>
                {
                    AniDroidSettings.ClearUserAuthentication();
                    View.OnErrorAuthorizing();
                }).Switch(user =>
                {
                    AniDroidSettings.LoggedInUser = user;
                    View.OnAuthorized();
                });
            }
        }
    }
}