using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
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
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ILoginView, LoginActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            // TODO: Implement for real

            SetContentView(Resource.Layout.Activity_Login);
            await CreatePresenter(savedInstanceState);

            var resultIntent = new Intent();
            resultIntent.PutExtra(MainActivity.RecreateActivityIntentKey, true);
            SetResult(Result.Canceled, resultIntent);
        }

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            // TODO: Implement
        }

        public static void StartActivity(Activity context)
        {
            var intent = new Intent(context, typeof(LoginActivity));
            context.StartActivityForResult(intent, 1);
        }
    }
}