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
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.Adapters.StudioAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.Base;
using AniDroid.SearchResults;
using AniDroid.Utils;

namespace AniDroid.AniListObject.Studio
{
    [Activity(Label = "Studio")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "anilist.co", DataSchemes = new[] { "http", "https" }, DataPathPattern = "/studio/.*", Label = "AniDroid")]
    public class StudioActivity : BaseAniListObjectActivity<StudioPresenter>, IStudioView
    {
        public const string StudioIdIntentKey = "STUDIO_ID";

        private int _studioId;

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                Logger.Debug("StudioActivity", $"Intent recieved with value '{dataUrl}'");
                var urlRegex = new Regex("anilist.co/studio/[0-9]*/?");
                var match = urlRegex.Match(dataUrl);
                var studioIdString = match.ToString().Replace("anilist.co/studio/", "").TrimEnd('/');
                SetStandaloneActivity();

                if (!int.TryParse(studioIdString, out _studioId))
                {
                    Toast.MakeText(this, "Couldn't read studio ID from URL", ToastLength.Short).Show();
                    Finish();
                }
            }
            else
            {
                _studioId = Intent.GetIntExtra(StudioIdIntentKey, 0);
            }

            Logger.Debug("StudioActivity", $"Starting activity with studioId: {_studioId}");

            await CreatePresenter(savedInstanceState);
        }

        public static void StartActivity(BaseAniDroidActivity context, int studioId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(StudioActivity));
            intent.PutExtra(StudioIdIntentKey, studioId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        public int GetStudioId()
        {
            return _studioId;
        }

        protected override Func<Task> ToggleFavorite => () => Presenter.ToggleFavorite();

        public void SetupStudioView(AniList.Models.Studio studio)
        {
            var adapter = new FragmentlessViewPagerAdapter();

            if (studio.Media?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateStudioMediaView(studio.Id), "Media");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        private View CreateStudioMediaView(int studioId)
        {
            var studioMediaEnumerable = Presenter.GetStudioMediaEnumerable(studioId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaEdgeRecyclerAdapter(this, studioMediaEnumerable, CardType, MediaEdgeViewModel.CreateStudioMediaViewModel);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

    }
}