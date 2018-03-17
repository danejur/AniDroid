using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.AniList;
using AniDroid.AniList.Models;
using AniDroid.AniList.Service;
using AniDroid.Base;
using AniDroid.SearchResults;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.AniListObject.Staff
{
    [Activity(Label = "Staff")]
    public class StaffActivity : BaseAniListObjectActivity<StaffPresenter>, IStaffView
    {
        public const string StaffIdIntentKey = "STAFF_ID";

        private int _staffId;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IStaffView, StaffActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
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

        public void SetupStaffView(AniList.Models.Staff staff)
        {
            var adapter = new FragmentlessViewPagerAdapter();
            adapter.AddView(CreateStaffDetailsView(staff), "Details");

            if (staff.Characters?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStaffCharactersView(staff.Id), "Characters");
            }

            if (staff.Anime?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStaffMediaView(staff.Id, AniList.Models.Media.MediaType.Anime), "Anime");
            }

            if (staff.Manga?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStaffMediaView(staff.Id, AniList.Models.Media.MediaType.Manga), "Manga");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        private View CreateStaffDetailsView(AniList.Models.Staff staff)
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
            var dialogRecyclerAdapter = new StaffCharactersRecyclerAdapter(this, staffCharactersEnumerable, CardType);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateStaffMediaView(int staffId, AniList.Models.Media.MediaType mediaType)
        {
            var staffMediaEnumerable = Presenter.GetStaffMediaEnumerable(staffId, mediaType, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new StaffMediaRecyclerAdapter(this, staffMediaEnumerable, CardType);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }
    }
}