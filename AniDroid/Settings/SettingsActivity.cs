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
using AniDroid.AniList.Interfaces;
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

        public override void OnError(IAniListError error)
        {
            // TODO: should this ever matter?
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public void CreateCardTypeSettingItem(BaseRecyclerAdapter.CardType cardType)
        {
            var options = new List<string> {"Vertical", "Horizontal", "Flat Horizontal"};
            _settingsContainer.AddView(
                CreateSpinnerSettingRow("Card Display Type", "Choose how you would like to display lists in AniDroid", options, (int) cardType, (sender, args) => Presenter.SetCardType((BaseRecyclerAdapter.CardType)args.Position)));
            _settingsContainer.AddView(CreateDivider());
        }

        public void CreateAniDroidThemeSettingItem(AniDroidTheme theme)
        {
            var options = new List<string> { "AniList", "Dark" };
            _settingsContainer.AddView(
                CreateSpinnerSettingRow("AniDroid Theme", "Choose the theme you'd like to use", options, (int)theme, (sender, args) =>
                {
                    Presenter.SetTheme((AniDroidTheme) args.Position);
                    if (theme != (AniDroidTheme) args.Position)
                    {
                        Recreate();
                    }
                }));
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

        private View CreateSettingRow(string name, string description, EventHandler tapEvent)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.View_SettingItem, null);
            view.FindViewById<LinearLayout>(Resource.Id.SettingItem_Container).Click += tapEvent;
            view.FindViewById<TextView>(Resource.Id.SettingItem_Name).Text = name;

            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);
            if (!string.IsNullOrWhiteSpace(description))
                textTwoView.Text = description;
            else
                textTwoView.Visibility = ViewStates.Gone;

            return view;
        }

        private View CreateSwitchSettingRow(string name, string description, bool switchState, EventHandler<CompoundButton.CheckedChangeEventArgs> switchEvent)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Switch, null);
            view.FindViewById<TextView>(Resource.Id.SettingItem_Name).Text = name;

            var switchView = view.FindViewById<SwitchCompat>(Resource.Id.SettingItem_Switch);
            switchView.Checked = switchState;
            switchView.CheckedChange += switchEvent;

            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);
            if (!string.IsNullOrWhiteSpace(description))
                textTwoView.Text = description;
            else
                textTwoView.Visibility = ViewStates.Gone;

            return view;
        }

        private View CreateSpinnerSettingRow(string name, string description, IList<string> items, int selectedPosition, EventHandler<AdapterView.ItemSelectedEventArgs> selectedEvent)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.View_SettingItem_Spinner, null);
            view.FindViewById<TextView>(Resource.Id.SettingItem_Name).Text = name;

            var spinner = view.FindViewById<Spinner>(Resource.Id.SettingItem_Spinner);
            spinner.Adapter = new ArrayAdapter<string>(this, Resource.Layout.View_SpinnerDropDownItem, items);
            spinner.SetSelection(selectedPosition);
            spinner.ItemSelected += selectedEvent;

            var textTwoView = view.FindViewById<TextView>(Resource.Id.SettingItem_Details);
            if (!string.IsNullOrWhiteSpace(description))
                textTwoView.Text = description;
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
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
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