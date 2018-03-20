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
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.Base;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.AniListObject.User
{
    [Activity(Label = "User")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "anilist.co", DataSchemes = new[] { "http", "https" }, DataPathPattern = @"/user/.*", Label = "AniDroid")]
    public class UserActivity : BaseAniListObjectActivity<UserPresenter>, IUserView
    {
        public const string UserIdIntentKey = "USER_ID";

        private int? _userId;
        private string _userName;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IUserView, UserActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                var urlRegex = new Regex(@"anilist.co/user/\w+/?");
                var match = urlRegex.Match(dataUrl);
                var userName = match.ToString().Replace("anilist.co/user/", "").TrimEnd('/');
                _userId = int.TryParse(userName, out var userId) ? userId : (int?)null;
                _userName = _userId.HasValue ? null : userName;

                SetStandaloneActivity();
            }
            else
            {
                _userId = Intent.GetIntExtra(UserIdIntentKey, 0);
            }

            await CreatePresenter(savedInstanceState);
        }

        public static void StartActivity(BaseAniDroidActivity context, int userId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(UserActivity));
            intent.PutExtra(UserIdIntentKey, userId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        public int? GetUserId()
        {
            return _userId;
        }

        public string GetUserName()
        {
            return _userName;
        }

        public void SetupUserView(AniList.Models.User user)
        {
            var adapter = new FragmentlessViewPagerAdapter();
            adapter.AddView(CreateUserActivityView(user.Id), "Activity");

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        private View CreateUserActivityView(int userId)
        {
            var userActivityEnumerable = Presenter.GetUserActivityEnumrable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new AniListActivityRecyclerAdapter(this, Presenter, userActivityEnumerable, userId);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }
    }
}