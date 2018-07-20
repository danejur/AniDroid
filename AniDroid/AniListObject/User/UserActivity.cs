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
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using AniDroid.Widgets;
using Ninject;

namespace AniDroid.AniListObject.User
{
    [Activity(Label = "User")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "anilist.co", DataSchemes = new[] { "http", "https" }, DataPathPattern = @"/user/.*", Label = "AniDroid")]
    public class UserActivity : BaseAniListObjectActivity<UserPresenter>, IUserView
    {
        public const string UserIdIntentKey = "USER_ID";

        private IMenu _menu;
        private bool _canMessage;
        private bool _canFollow;
        private bool _isFollowing;
        private int? _userId;
        private string _userName;
        private AniListActivityRecyclerAdapter _userActivityRecyclerAdapter;

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

        public void SetIsFollowing(bool isFollowing, bool showNotification)
        {
            _isFollowing = isFollowing;
            _menu?.FindItem(Resource.Id.Menu_User_Follow)?.SetIcon(_isFollowing
                ? Resource.Drawable.svg_star
                : Resource.Drawable.svg_star_outline);

            if (showNotification)
            {
                DisplaySnackbarMessage(_isFollowing ? "Followed user" : "Unfollowed user");
            }
        }

        public void SetCanFollow()
        {
            _canFollow = true;
        }

        public void SetCanMessage()
        {
            _canMessage = true;
        }

        public void SetupUserView(AniList.Models.User user)
        {
            var adapter = new FragmentlessViewPagerAdapter();

            adapter.AddView(CreateUserDetailsView(user), "Details");

            adapter.AddView(CreateUserActivityView(user.Id), "Activity");

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        public void RefreshUserActivity()
        {
            _userActivityRecyclerAdapter.ResetAdapter();
        }

        public void UpdateActivity(int activityPosition, AniListActivity activity)
        {
            _userActivityRecyclerAdapter.Items[activityPosition] = activity;
            _userActivityRecyclerAdapter.NotifyItemChanged(activityPosition);
        }

        private View CreateUserActivityView(int userId)
        {
            var userActivityEnumerable = Presenter.GetUserActivityEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            _userActivityRecyclerAdapter = new AniListActivityRecyclerAdapter(this, Presenter, userActivityEnumerable, Presenter.GetCurrentUserId());
            recycler.SetAdapter(_userActivityRecyclerAdapter);

            return retView;
        }

        private View CreateUserDetailsView(AniList.Models.User user)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_UserDetails, null);
            LoadImage(retView.FindViewById<ImageView>(Resource.Id.User_Image), user.Avatar.Large);
            retView.FindViewById<TextView>(Resource.Id.User_Name).Text = user.Name;

            var userAnimeView = retView.FindViewById<DataRow>(Resource.Id.User_AnimeSummary);
            var userAnimeListCount = user.Stats.AnimeStatusDistribution.Sum(x => x.Amount);
            if (userAnimeListCount > 0)
            {
                userAnimeView.Visibility = ViewStates.Visible;
                userAnimeView.TextOne = $"{userAnimeListCount} anime on lists";
                userAnimeView.TextTwo = $"{user.Stats.WatchedTime} minutes watched";
                userAnimeView.Click += (sender, args) =>
                    MediaListActivity.StartActivity(this, user.Id, AniList.Models.Media.MediaType.Anime);
            }

            return retView;
        }

        #region Menu

        public override bool SetupMenu(IMenu menu)
        {
            menu?.Clear();
            MenuInflater.Inflate(Resource.Menu.User_ActionBar, _menu = menu);
            menu?.FindItem(Resource.Id.Menu_User_Message)?.SetVisible(_canMessage);
            menu?.FindItem(Resource.Id.Menu_User_Follow)?.SetIcon(_isFollowing
                ? Resource.Drawable.svg_star
                : Resource.Drawable.svg_star_outline);
            menu?.FindItem(Resource.Id.Menu_User_Follow)?.SetVisible(_canFollow);
            return true;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Menu_User_Share:
                    Share();
                    return true;
                case Resource.Id.Menu_User_Follow:
                    Presenter.ToggleFollowUser(_userId ?? 0);
                    return true;
                case Resource.Id.Menu_User_Message:
                    AniListActivityCreateDialog.Create(this, (message) => Presenter?.PostUserMessage(_userId ?? 0, message));
                    return true;
            }

            return base.MenuItemSelected(item);
        }

        #endregion

    }
}