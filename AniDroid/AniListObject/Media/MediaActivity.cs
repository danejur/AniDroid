using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.AniList;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;
using AniDroid.SearchResults;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.AniListObject.Media
{

    [Activity(Label = "Media")]
    public class MediaActivity : BaseAniListObjectActivity<MediaPresenter>, IMediaView
    {
        public const string MediaIdIntentKey = "MEDIA_ID";

        private int _mediaId;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IMediaView, MediaActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetLoadingShown();

            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                var urlRegex = new Regex("anilist.co/(anime|manga)/[0-9]*/?");
                var match = urlRegex.Match(dataUrl);
                var mediaIdString = match.ToString().Replace("anilist.co/anime/", "").Replace("anilist.co/manga/", "").TrimEnd('/');
                SetStandaloneActivity();

                if (!int.TryParse(mediaIdString, out _mediaId))
                {
                    Toast.MakeText(this, "Couldn't read media ID from URL", ToastLength.Short).Show();
                    Finish();
                }
            }
            else
            {
                _mediaId = Intent.GetIntExtra(MediaIdIntentKey, 0);
            }

            await CreatePresenter(savedInstanceState);
        }

        public static void StartActivity(BaseAniDroidActivity context, int mediaId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(MediaActivity));
            intent.PutExtra(MediaIdIntentKey, mediaId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        public int GetMediaId()
        {
            return _mediaId;
        }

        public void SetupMediaView(AniList.Models.Media media)
        {
            // TODO: implement toggle favorite
            //ToggleFavorite = () => ToggleFavoriteInternal(staff.Id);

            var adapter = new FragmentlessViewPagerAdapter();
            //adapter.AddView(CreateMediaDetailsView(media), "Details");

            if (media.Characters?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateMediaCharactersView(media.Id), "Characters");
            }

            if (media.Staff?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateMediaStaffView(media.Id), "Staff");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        private View CreateMediaCharactersView(int mediaId)
        {
            var mediaCharactersEnumerable = Presenter.GetMediaCharactersEnumerable(mediaId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaCharactersRecyclerAdapter(this, mediaCharactersEnumerable, CardType);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaStaffView(int mediaId)
        {
            var mediaStaffEnumerable = Presenter.GetMediaStaffEnumerable(mediaId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaStaffRecyclerAdapter(this, mediaStaffEnumerable, CardType);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }
    }
}