using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.CustomTabs;
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
    [Activity(Label = "Login", LaunchMode = LaunchMode.SingleTask)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "login", DataSchemes = new[] { "anidroid" }, Label = "AniDroid")]
    public class LoginActivity : BaseAniDroidActivity<LoginPresenter>, ILoginView
    {
        private string _authCode;
        private bool _loginSequenceStarted;
        private bool _loginSequenceEnded;
        private CancellationTokenSource _tokenSource;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ILoginView, LoginActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Login);
            await CreatePresenter(savedInstanceState);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            if (_loginSequenceEnded)
            {
                _tokenSource = new CancellationTokenSource();
                await Presenter.Login(_tokenSource.Token);
            }
            else if (!_loginSequenceStarted)
            {
                var authUrl = Resources.GetString(Resource.Config.AniListAuthorizeUri);
                var clientId = Resources.GetString(Resource.Config.ApiClientId);
                var redirectUri = Resources.GetString(Resource.Config.ApiRedirectUri);

                var intentBuilder = new CustomTabsIntent.Builder()
                    .Build();

                _loginSequenceStarted = true;
                intentBuilder.LaunchUrl(this, Android.Net.Uri.Parse(string.Format(authUrl, clientId, redirectUri)));
            }
            else
            {
                OnLoginAborted();
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Action == Intent.ActionView)
            {
                _loginSequenceEnded = true;
            }

            _authCode = intent.Data?.GetQueryParameter("code");
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
            var intent = new Intent();
            intent.PutExtra(MainActivity.NotificationTextIntentKey,
                GetString(Resource.String.LoginLogout_LoginErrorMessage));
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void OnAuthorized()
        {
            var intent = new Intent();
            intent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void OnLoginAborted()
        {
            var intent = new Intent();
            intent.PutExtra(MainActivity.NotificationTextIntentKey,
                GetString(Resource.String.LoginLogout_LoginAbortedMessage));
            SetResult(Result.Ok, intent);
            Finish();
        }

        public override void OnBackPressed()
        {
            if (!_loginSequenceEnded)
            {
                base.OnBackPressed();
            }
            else
            {
                _tokenSource.Cancel(false);
                Settings.ClearUserAuthentication();
                OnLoginAborted();
            }
        }

        public static void StartActivity(Activity context)
        {
            var intent = new Intent(context, typeof(LoginActivity));
            context.StartActivityForResult(intent, 0);
        }
    }
}