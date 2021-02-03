using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using Google.Android.Material.Snackbar;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace AniDroid.About
{
    [Activity(Label = "About AniDroid")]
    public class AboutActivity : BaseAniDroidActivity
    {
        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public override Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_About);

            var toolbar = FindViewById<Toolbar>(Resource.Id.About_Toolbar);
            toolbar.Title = "About AniDroid";

            SetSupportActionBar(toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var versionText = FindViewById<TextView>(Resource.Id.About_AppVersion);
            versionText.Text = $"Current version: {PackageManager.GetPackageInfo(PackageName, 0).VersionName}";

            return Task.CompletedTask;
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
            throw new NotImplementedException();
        }

        public override void DisplayNotYetImplemented()
        {
            throw new NotImplementedException();
        }

        public static void StartActivity(BaseAniDroidActivity context)
        {
            var intent = new Intent(context, typeof(AboutActivity));
            context.StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return false;
        }
    }
}