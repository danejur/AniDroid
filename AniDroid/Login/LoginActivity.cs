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
        [InjectView(Resource.Id.Login_WebView)]
        private WebView _webView;
        [InjectView(Resource.Id.Login_LoggingInView)]
        private LinearLayout _loggingInView;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ILoginView, LoginActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Login);
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

        public void ShowLoginPage()
        {
            var webViewClient = new AniListLoginWebClient(Presenter);
            var authUrl = Resources.GetString(Resource.Config.AniListAuthorizeUri);
            var clientId = Resources.GetString(Resource.Config.ApiClientId);
            var redirectUri = Resources.GetString(Resource.Config.ApiRedirectUri);

            _webView.ClearCache(true);
            _webView.ClearHistory();
            _webView.Settings.JavaScriptEnabled = true;
            _webView.Settings.DomStorageEnabled = true;
            _webView.SetWebViewClient(webViewClient);
            _webView.LoadUrl(string.Format(authUrl, clientId, redirectUri));
        }

        public void OnErrorAuthorizing()
        {
            Toast.MakeText(this, "Error occurred while authorizing", ToastLength.Long).Show();
            Finish();
        }

        public void OnAuthorizing()
        {
            _webView.Visibility = ViewStates.Gone;
            _loggingInView.Visibility = ViewStates.Visible;
        }

        public void OnAuthorized()
        {
            var resultIntent = new Intent();
            resultIntent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
            SetResult(Result.Canceled, resultIntent);
            Finish();
        }

        public static void StartActivity(Activity context)
        {
            var intent = new Intent(context, typeof(LoginActivity));
            context.StartActivityForResult(intent, 1);
        }

        private class AniListLoginWebClient : WebViewClient
        {
            private readonly LoginPresenter _presenter;

            public AniListLoginWebClient(LoginPresenter presenter)
            {
                _presenter = presenter;
            }

            public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
            {
                if (!url.Contains("code="))
                {
                    return;
                }

                var query = new Uri(url).Query;
                var code = query.Trim('?').Split('&').First(x => x.Contains("code=")).Replace("code=", "");
                _presenter.AuthenticateUser(code);
            }
        }
    }
}