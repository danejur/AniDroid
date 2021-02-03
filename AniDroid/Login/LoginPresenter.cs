using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.Login
{
    public class LoginPresenter : BaseAniDroidPresenter<ILoginView>
    {
        private readonly IAniListAuthConfig _authConfig;

        public LoginPresenter(IAniListService service, IAniDroidSettings settings,
            IAniListAuthConfig authConfig, IAniDroidLogger logger) : base(service, settings, logger)
        {
            _authConfig = authConfig;
        }

        public override Task Init()
        {
            return Task.CompletedTask;
        }

        public async Task Login(CancellationToken token)
        {
            AniDroidSettings.ClearUserAuthentication();
            var authCode = View.GetAuthCode();

            if (string.IsNullOrWhiteSpace(authCode))
            {
                View.OnErrorAuthorizing();
                return;
            }

            var authResp = await AniListService.AuthenticateUser(_authConfig, authCode, token);

            authResp.Switch((IAniListError error) => View.OnErrorAuthorizing())
                .Switch(async auth =>
                {
                    AniDroidSettings.UserAccessCode = auth.AccessToken;

                    var currentUser = await AniListService.GetCurrentUser(token);

                    currentUser.Switch((IAniListError error) =>
                    {
                        AniDroidSettings.ClearUserAuthentication();
                        View.OnErrorAuthorizing();
                    }).Switch(user =>
                    {
                        AniDroidSettings.LoggedInUser = user;
                        View.OnAuthorized();
                    });
                });
        }

        public string GetRedirectUrl(string authUrlTemplate)
        {
            return string.Format(authUrlTemplate, _authConfig.ClientId, _authConfig.RedirectUri);
        }
    }
}