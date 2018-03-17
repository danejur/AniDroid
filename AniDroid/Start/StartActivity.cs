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
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Login;
using AniDroid.Main;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Start
{
    [Activity(Label = "AniDroidNew", MainLauncher = true)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "login", DataSchemes = new[] { "anidroid" }, Label = "AniDroid")]
    public class StartActivity : BaseAniDroidActivity
    {
        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule());

        public override void OnError(IAniListError error)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public override Task OnCreateExtended(Bundle savedInstanceState)
        {
            // this implemntation taken from https://stackoverflow.com/a/38878907
            // need to make sure tht it is actually functioning as intended
            if (!IsTaskRoot)
            {
                Finish();

                return Task.CompletedTask;
            }

            if (Intent?.Action == Intent.ActionView)
            {
                var code = Intent.Data?.GetQueryParameter("code");

                if (!string.IsNullOrWhiteSpace(code))
                {
                    if (Settings.IsUserAuthenticated)
                    {
                        MainActivity.StartActivityForResult(this, 0, GetString(Resource.String.LoginLogout_AlreadyAuthenticatedMessage));
                        Finish();
                    }
                    else
                    {
                        LoginActivity.StartActivityForResult(this, 0, code);
                        Finish();
                    }
                }
                else
                {
                    MainActivity.StartActivityForResult(this, 0, GetString(Resource.String.LoginLogout_LoginErrorMessage));
                    Finish();
                }

                return Task.CompletedTask;
            }


            // TODO: add checks for data store integrity and other start-up tasks

            Settings.ClearUserAuthentication();

            MainActivity.StartActivityForResult(this, 0);
            Finish();

            return Task.CompletedTask;
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            // will never be invoked as of now
            throw new NotImplementedException();
        }

        public override void DisplayNotYetImplemented()
        {
            // will never be invoked as of now
            throw new NotImplementedException();
        }
    }
}