using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Main;

namespace AniDroid.Start
{
    [Activity(MainLauncher = true, LaunchMode = LaunchMode.SingleTop)]
    public class StartActivity : BaseAniDroidActivity
    {
        public override void OnError(IAniListError error)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public override Task OnCreateExtended(Bundle savedInstanceState)
        {
            // TODO: add checks for data store integrity and other start-up tasks

            //Settings.ClearUserAuthentication();

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