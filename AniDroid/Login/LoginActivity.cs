using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Login
{
    [Activity(Label = "Login")]
    public class LoginActivity : BaseAniDroidActivity<LoginPresenter>, ILoginView
    {
        private const string AuthCodeIntentKey = "AUTH_CODE";

        private string _authCode;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ILoginView, LoginActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Login);

            _authCode = Intent.GetStringExtra(AuthCodeIntentKey);

            await CreatePresenter(savedInstanceState);
        }

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            // TODO: Implement
        }

        public string GetAuthCode()
        {
            return _authCode;
        }

        public void OnErrorAuthorizing()
        {
            MainActivity.StartActivityForResult(this, 0, GetString(Resource.String.LoginLogout_LoginErrorMessage));
            Finish();
        }

        public void OnAuthorized()
        {
            MainActivity.StartActivityForResult(this, 0);
            Finish();
        }

        public static void RedirectToLogin(Activity context)
        {
            var authUrl = context.Resources.GetString(Resource.Config.AniListAuthorizeUri);
            var clientId = context.Resources.GetString(Resource.Config.ApiClientId);
            var redirectUri = context.Resources.GetString(Resource.Config.ApiRedirectUri);


            var browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(string.Format(authUrl, clientId, redirectUri)));
            context.StartActivityForResult(browserIntent, 0);
        }

        public static void StartActivityForResult(Activity context, int requestCode, string authCode)
        {
            var intent = new Intent(context, typeof(LoginActivity));
            intent.PutExtra(AuthCodeIntentKey, authCode);
            context.StartActivityForResult(intent, 0);
        }
    }
}