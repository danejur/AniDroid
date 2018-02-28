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
using Android.Widget;
using AniDroid.Base;
using AniDroid.Utils;
using Ninject;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Settings
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : BaseAniDroidActivity<SettingsPresenter>, ISettingsView
    {
        [InjectView(Resource.Id.Settings_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Settings_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.Settings_Container)]
        private LinearLayout _settingsContainer;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<ISettingsView, SettingsActivity>(this));

        public override void OnNetworkError()
        {
            // TODO: should this ever matter?
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public static void StartActivity(Context context)
        {
            var intent = new Intent(context, typeof(SettingsActivity));
            context.StartActivity(intent);
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Settings);

            SetupToolbar();

            await CreatePresenter(savedInstanceState);
        }

        #region Toolbar

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24dp);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            return false;
        }

        #endregion
    }
}