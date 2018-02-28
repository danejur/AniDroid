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
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
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

        public void CreateCardTypeSettingItem(BaseRecyclerAdapter.CardType cardType)
        {
            _settingsContainer.AddView(CreateSettingRow("Card Display Type", "Choose how you would like to display lists in AniDroid", (sender, args) => DisplaySnackbarMessage("Clicked", Snackbar.LengthShort)));
            _settingsContainer.AddView(CreateDivider());
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

        #region Settings Views

        private View CreateSettingRow(string textOne, string textTwo, EventHandler tapEvent)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.View_SettingItem, null);
            view.FindViewById<LinearLayout>(Resource.Id.SettingItem_Container).Click += tapEvent;
            view.FindViewById<TextView>(Resource.Id.SettingItem_Name).Text = textOne;

            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);
            if (!string.IsNullOrWhiteSpace(textTwo))
                textTwoView.Text = textTwo;
            else
                textTwoView.Visibility = ViewStates.Gone;

            return view;
        }

        private View CreateSwitchSettingRow(string textOne, string textTwo, bool switchState, EventHandler<CompoundButton.CheckedChangeEventArgs> switchEvent)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Switch, null);
            view.FindViewById<TextView>(Resource.Id.SettingItem_Name).Text = textOne;

            var switchView = view.FindViewById<SwitchCompat>(Resource.Id.SettingItem_Switch);
            switchView.Checked = switchState;
            switchView.CheckedChange += switchEvent;

            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);
            if (!string.IsNullOrWhiteSpace(textTwo))
                textTwoView.Text = textTwo;
            else
                textTwoView.Visibility = ViewStates.Gone;

            return view;
        }

        private View CreateDivider()
        {
            var typedValue = new TypedValue();
            Theme.ResolveAttribute(Resource.Attribute.ListItem_Divider, typedValue, true);
            var dividerView = new LinearLayout(this) { LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 2) };
            dividerView.SetBackgroundResource(typedValue.ResourceId);

            return dividerView;
        }

        #endregion

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