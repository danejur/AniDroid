using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.CharacterAdapters;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.Base;

namespace AniDroid.AniListObject.Staff
{
    [Activity(Label = "Staff")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "anilist.co", DataSchemes = new[] { "http", "https" }, DataPathPattern = "/staff/.*", Label = "AniDroid")]
    public class StaffActivity : BaseAniListObjectActivity<StaffPresenter>, IStaffView
    {
        public const string StaffIdIntentKey = "STAFF_ID";

        private int _staffId;

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                Logger.Debug("StaffActivity", $"Intent recieved with value '{dataUrl}'");
                var urlRegex = new Regex("anilist.co/staff/[0-9]*/?");
                var match = urlRegex.Match(dataUrl);
                var staffIdString = match.ToString().Replace("anilist.co/staff/", "").TrimEnd('/');
                SetStandaloneActivity();

                if (!int.TryParse(staffIdString, out _staffId))
                {
                    Toast.MakeText(this, "Couldn't read staff ID from URL", ToastLength.Short).Show();
                    Finish();
                }
            }
            else
            {
                _staffId = Intent.GetIntExtra(StaffIdIntentKey, 0);
            }

            Logger.Debug("StaffActivity", $"Starting activity with staffID: {_staffId}");

            await CreatePresenter(savedInstanceState);
        }

        public static void StartActivity(BaseAniDroidActivity context, int staffId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(StaffActivity));
            intent.PutExtra(StaffIdIntentKey, staffId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        public int GetStaffId()
        {
            return _staffId;
        }

        protected override Func<Task> ToggleFavorite => () => Presenter.ToggleFavorite();

        public void SetupStaffView(AniList.Models.StaffModels.Staff staff)
        {
            var adapter = new FragmentlessViewPagerAdapter();
            adapter.AddView(CreateStaffDetailsView(staff), "Details");

            if (staff.Characters?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStaffCharactersView(staff.Id), "Characters");
            }

            if (staff.Anime?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStaffMediaView(staff.Id, MediaType.Anime), "Anime");
            }

            if (staff.Manga?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStaffMediaView(staff.Id, MediaType.Manga), "Manga");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        private View CreateStaffDetailsView(AniList.Models.StaffModels.Staff staff)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_StaffDetails, null);
            var imageView = retView.FindViewById<ImageView>(Resource.Id.Staff_Image);
            var descriptionView = retView.FindViewById<TextView>(Resource.Id.Staff_Description);
            var nameView = retView.FindViewById<TextView>(Resource.Id.Staff_Name);
            var languageView = retView.FindViewById<TextView>(Resource.Id.Staff_Language);

            LoadImage(imageView, staff.Image?.Large);
            descriptionView.TextFormatted = FromHtml(staff.Description ?? "(No Description Available)");
            nameView.Text = staff.Name?.GetFormattedName(true);
            languageView.Text = staff.Language?.DisplayValue ?? "(Language Unknown)";

            return retView;
        }

        private View CreateStaffCharactersView(int staffId)
        {
            var staffCharactersEnumerable = Presenter.GetStaffCharactersEnumerable(staffId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new CharacterEdgeRecyclerAdapter(this, staffCharactersEnumerable, CardType,
                CharacterEdgeViewModel.CreateStaffCharacterEdgeViewModel);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateStaffMediaView(int staffId, MediaType mediaType)
        {
            var staffMediaEnumerable = Presenter.GetStaffMediaEnumerable(staffId, mediaType, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaEdgeRecyclerAdapter(this, staffMediaEnumerable, CardType, MediaEdgeViewModel.CreateStaffMediaViewModel);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }
    }
}