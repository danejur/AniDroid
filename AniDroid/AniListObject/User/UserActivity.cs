using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.Adapters.UserAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.Utils;
using AniDroid.Widgets;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Text;
using Android.Text.Method;
using AniDroid.Adapters.ReviewAdapters;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.Utils.Formatting.Markdown;

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

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                Logger.Debug("UserActivity", $"Intent recieved with value '{dataUrl}'");
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

            Logger.Debug("UserActivity", $"Starting activity with userId: {_userId}");

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

        public void SetupUserView(AniList.Models.UserModels.User user)
        {
            var adapter = new FragmentlessViewPagerAdapter();

            adapter.AddView(CreateUserDetailsView(user), "Details");

            adapter.AddView(CreateUserActivityView(user.Id), "Activity");

            adapter.AddView(CreateUserStatsView(user), "Stats");

            adapter.AddView(CreateUserFollowingView(user.Id), "Following");

            adapter.AddView(CreateUserFollowersView(user.Id), "Followers");

            adapter.AddView(CreateUserReviewsView(user.Id), "Reviews");

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

        public void RemoveActivity(int activityPosition)
        {
            _userActivityRecyclerAdapter.RemoveItem(activityPosition);
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

        private View CreateUserDetailsView(AniList.Models.UserModels.User user)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_UserDetails, null);
            LoadImage(retView.FindViewById<ImageView>(Resource.Id.User_Image), user.Avatar.Large);
            retView.FindViewById<TextView>(Resource.Id.User_Name).Text = user.Name;

            var aboutView = retView.FindViewById<ExpandableText>(Resource.Id.User_Description);
            if (!string.IsNullOrWhiteSpace(user.About))
            {
                aboutView.TextFormatted = MarkdownTextCleaner.ConvertToSpanned(user.About);

                aboutView.ExpandTextAction = textView => {
                    var builder = new SpannableStringBuilder(MarkdownTextCleaner.ConvertToSpanned(user.About));
                    textView.MovementMethod = LinkMovementMethod.Instance;
                    textView.SetText(builder, TextView.BufferType.Spannable);
                    MarkdownSpannableFormatter.FormatMarkdownSpannable(this, textView.TextFormatted as SpannableString);
                };
            }
            else
            {
                aboutView.Visibility = ViewStates.Gone;
            }

            var followerView = retView.FindViewById<TextView>(Resource.Id.User_Donator);
            followerView.Visibility = user.DonatorTier > 0 ? ViewStates.Visible : ViewStates.Gone;

            var userAnimeView = retView.FindViewById<DataRow>(Resource.Id.User_AnimeSummary);
            var userAnimeListCount = user.Stats.AnimeStatusDistribution.Sum(x => x.Amount);
            if (userAnimeListCount > 0)
            {
                userAnimeView.Visibility = ViewStates.Visible;
                userAnimeView.TextOne = $"{userAnimeListCount} anime on lists";
                userAnimeView.TextTwo = $"{user.GetDurationString(user.Stats.WatchedTime * 60, 1)} watched";
                userAnimeView.SetButtonIcon(Resource.Drawable.svg_chevron_right);
                userAnimeView.ButtonClickable = false;
                userAnimeView.ButtonVisible = true;
                userAnimeView.Click += (sender, args) =>
                    MediaListActivity.StartActivity(this, user.Id, MediaType.Anime);
            }

            var userMangaView = retView.FindViewById<DataRow>(Resource.Id.User_MangaSummary);
            var userMangaListCount = user.Stats.MangaStatusDistribution.Sum(x => x.Amount);
            if (userMangaListCount > 0)
            {
                userMangaView.Visibility = ViewStates.Visible;
                userMangaView.TextOne = $"{userMangaListCount} manga on lists";
                userMangaView.TextTwo = $"{user.Stats.ChaptersRead} chapters read";
                userMangaView.SetButtonIcon(Resource.Drawable.svg_chevron_right);
                userMangaView.ButtonClickable = false;
                userMangaView.ButtonVisible = true;
                userMangaView.Click += (sender, args) =>
                    MediaListActivity.StartActivity(this, user.Id, MediaType.Manga);
            }

            return retView;
        }

        private View CreateUserFollowingView(int userId)
        {
            var userFollowingEnumerable = Presenter.GetUserFollowingEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new UserRecyclerAdapter(this, userFollowingEnumerable, CardType, UserViewModel.CreateUserFollowingViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateUserFollowersView(int userId)
        {
            var userFollowersEnumerable = Presenter.GetUserFollowersEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new UserRecyclerAdapter(this, userFollowersEnumerable, CardType, UserViewModel.CreateUserViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateUserReviewsView(int userId)
        {
            var userReviewsEnumerable = Presenter.GetUserReviewsEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new ReviewRecyclerAdapter(this, userReviewsEnumerable, CardType, ReviewViewModel.CreateUserReviewViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateUserStatsView(AniList.Models.UserModels.User user)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_NestedScrollLayout, null);
            var containerView = retView.FindViewById<LinearLayout>(Resource.Id.Scroll_Container);

            if (user.Stats?.AnimeStatusDistribution?.Any(x => x.Amount > 0) == true ||
                user.Stats?.MangaStatusDistribution?.Any(x => x.Amount > 0) == true)
            {
                containerView.AddView(CreateStatusDistributionView(user.Stats.AnimeStatusDistribution,
                    user.Stats.MangaStatusDistribution));
            }

            if (user.Stats?.AnimeScoreDistribution?.Any(x => x.Amount > 0) == true ||
                user.Stats?.MangaScoreDistribution?.Any(x => x.Amount > 0) == true)
            {
                containerView.AddView(CreateScoreDistributionView(user.Stats.AnimeScoreDistribution,
                    user.Stats.MangaScoreDistribution));
            }

            return retView;
        }

        private View CreateStatusDistributionView(
            IReadOnlyList<AniListStatusDistribution> animeStatusDistributions,
            IReadOnlyList<AniListStatusDistribution> mangaStatusDistributions)
        {
            var chartHeight = Resources.GetDimensionPixelSize(Resource.Dimension.Details_ChartHeight);
            var textColor = GetThemedColor(Resource.Attribute.Background_Text);
            var margin = Resources.GetDimensionPixelSize(Resource.Dimension.Details_MarginSmall);

            var detailView = LayoutInflater.Inflate(Resource.Layout.View_AniListObjectDetail, null);
            var detailContainer = detailView.FindViewById<LinearLayout>(Resource.Id.AniListObjectDetail_InnerContainer);
            detailView.FindViewById<TextView>(Resource.Id.AniListObjectDetail_Name).Text = "Status Distribution";
            detailContainer.Orientation = Orientation.Horizontal;
            detailContainer.SetPadding(margin, 0, margin, 0);

            var typedColorArray = Resources.ObtainTypedArray(Resource.Array.Chart_Colors);
            var colorList = new List<int>();

            for (var i = 0; i < typedColorArray.Length(); i++)
            {
                colorList.Add(typedColorArray.GetColor(i, 0));
            }

            var statusDistChart = new BarChart(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, chartHeight),
            };

            var data = new BarData() { BarWidth = .45F };

            var animeStatusEntries = animeStatusDistributions.Select(x => new BarEntry(x.Status.Index, x.Amount, x.Status.DisplayValue)).ToList();
            var animeDataSet = new BarDataSet(animeStatusEntries, "Anime") { Color = colorList[0] };
            data.AddDataSet(animeDataSet);

            var mangaStatusEntries = mangaStatusDistributions.Select(x => new BarEntry(x.Status.Index, x.Amount, x.Status.DisplayValue)).ToList();
            var mangaDataSet = new BarDataSet(mangaStatusEntries, "Manga") { Color = colorList[1] };
            data.AddDataSet(mangaDataSet);

            data.SetValueFormatter(new ChartUtils.IntegerValueFormatter());

            statusDistChart.Data = data;
            statusDistChart.SetTouchEnabled(false);
            statusDistChart.SetScaleEnabled(false);
            statusDistChart.GroupBars(-.5F, .12F, .04F);
            statusDistChart.AxisLeft.Enabled = false;
            statusDistChart.AxisRight.Enabled = false;
            statusDistChart.Description.Enabled = false;
            statusDistChart.XAxis.SetDrawGridLines(false);
            statusDistChart.XAxis.SetDrawAxisLine(false);
            statusDistChart.XAxis.Position = XAxis.XAxisPosition.BottomInside;
            statusDistChart.XAxis.Granularity = 1;
            statusDistChart.XAxis.LabelCount = 5;
            statusDistChart.XAxis.ValueFormatter = new ChartUtils.AxisAniListEnumFormatter<MediaListStatus>();
            statusDistChart.XAxis.TextColor = animeDataSet.ValueTextColor = mangaDataSet.ValueTextColor = statusDistChart.Legend.TextColor = textColor;

            detailContainer.AddView(statusDistChart);

            return detailView;
        }

        private View CreateScoreDistributionView(
            IReadOnlyList<AniListScoreDistribution> animeScoreDistributions,
            IReadOnlyList<AniListScoreDistribution> mangaScoreDistributions)
        {
            var chartHeight = Resources.GetDimensionPixelSize(Resource.Dimension.Details_ChartHeight);
            var textColor = GetThemedColor(Resource.Attribute.Background_Text);
            var margin = Resources.GetDimensionPixelSize(Resource.Dimension.Details_MarginSmall);

            var detailView = LayoutInflater.Inflate(Resource.Layout.View_AniListObjectDetail, null);
            var detailContainer = detailView.FindViewById<LinearLayout>(Resource.Id.AniListObjectDetail_InnerContainer);
            detailView.FindViewById<TextView>(Resource.Id.AniListObjectDetail_Name).Text = "Score Distribution";
            detailContainer.Orientation = Orientation.Horizontal;
            detailContainer.SetPadding(margin, 0, margin, 0);

            var typedColorArray = Resources.ObtainTypedArray(Resource.Array.Chart_Colors);
            var colorList = new List<int>();

            for (var i = 0; i < typedColorArray.Length(); i++)
            {
                colorList.Add(typedColorArray.GetColor(i, 0));
            }

            var scoreDistChart = new BarChart(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, chartHeight),
            };

            var data = new BarData() { BarWidth = 4.5F };

            var animeEntries = Enumerable.Range(1, 10).Select(x => new BarEntry(x * 10, animeScoreDistributions.Any(y => y.Score == (x * 10)) ? animeScoreDistributions.First(y => y.Score == (x * 10)).Amount : 0, (x * 10).ToString())).ToList();
            var animeDataSet = new BarDataSet(animeEntries, "Anime") { Color = colorList[4] };
            data.AddDataSet(animeDataSet);

            var mangaEntries = Enumerable.Range(1, 10).Select(x => new BarEntry(x * 10, mangaScoreDistributions.Any(y => y.Score == (x * 10)) ? mangaScoreDistributions.First(y => y.Score == (x * 10)).Amount : 0, (x * 10).ToString())).ToList();
            var mangaDataSet = new BarDataSet(mangaEntries, "Manga") { Color = colorList[5] };
            data.AddDataSet(mangaDataSet);

            data.SetValueFormatter(new ChartUtils.IntegerValueFormatter());

            scoreDistChart.Data = data;
            scoreDistChart.SetTouchEnabled(false);
            scoreDistChart.SetScaleEnabled(false);
            scoreDistChart.GroupBars(5F, .6F, .2F);
            scoreDistChart.AxisLeft.Enabled = false;
            scoreDistChart.AxisRight.Enabled = false;
            scoreDistChart.Description.Enabled = false;
            scoreDistChart.XAxis.SetDrawGridLines(false);
            scoreDistChart.XAxis.SetDrawAxisLine(false);
            scoreDistChart.XAxis.Position = XAxis.XAxisPosition.BottomInside;
            scoreDistChart.XAxis.Granularity = 1;
            scoreDistChart.XAxis.LabelCount = 10;
            scoreDistChart.XAxis.TextColor = animeDataSet.ValueTextColor = mangaDataSet.ValueTextColor = scoreDistChart.Legend.TextColor = textColor;

            detailContainer.AddView(scoreDistChart);

            return detailView;
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
                    AniListActivityCreateDialog.CreateNewActivity(this, (message) => Presenter?.PostUserMessage(_userId ?? 0, message));
                    return true;
            }

            return base.MenuItemSelected(item);
        }

        #endregion

    }
}