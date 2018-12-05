using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Login;
using AniDroid.Main;
using AniDroid.Services;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.Start
{
    [Activity(MainLauncher = true)]
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
            // TODO: add checks for data store integrity and other start-up tasks

            //Settings.ClearUserAuthentication();

            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();

            AniListNotificationService.CreateNotificationChannel(ApplicationContext);

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