using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.CharacterAdapters;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ReviewAdapters;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.Adapters.StudioAdapters;
using AniDroid.Adapters.UserAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniListObject.Staff;
using AniDroid.AniListObject.User;
using AniDroid.Base;
using AniDroid.Browse;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.SearchResults;
using AniDroid.Utils;
using AniDroid.Utils.Formatting;
using AniDroid.Widgets;
using Com.Google.Android.Flexbox;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using Ninject;

namespace AniDroid.AniListObject.Media
{

    [Activity(Label = "Media")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "anilist.co", DataSchemes = new[] { "http", "https" }, DataPathPatterns = new[] { "/anime/.*", "/manga/.*" }, Label = "AniDroid")]
    public class MediaActivity : BaseAniListObjectActivity<MediaPresenter>, IMediaView
    {
        public const string MediaIdIntentKey = "MEDIA_ID";

        private int _mediaId;
        private AniList.Models.Media _media;
        private AniList.Models.Media.MediaList _mediaList;
        private AniList.Models.User.UserMediaListOptions _mediaListOptions;
        private View _mediaDetailsView;
        private bool _canEditListItem;
        private IMenuItem _editItem;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IMediaView, MediaActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                Logger.Debug("MediaActivity", $"Intent recieved with value '{dataUrl}'");
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

            Logger.Debug("MediaActivity", $"Starting activity with mediaId: {_mediaId}");

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

        public AniList.Models.Media.MediaType GetMediaType()
        {
            return _media.Type;
        }

        public void SetCanEditListItem()
        {
            _canEditListItem = true;
        }

        public void SetupMediaView(AniList.Models.Media media)
        {
            _media = media;
            _mediaList = media.MediaListEntry;

            var adapter = new FragmentlessViewPagerAdapter();
            adapter.AddView(_mediaDetailsView = CreateMediaDetailsView(media), "Details");

            if (media.Characters?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateMediaCharactersView(media.Id), "Characters");
            }

            if (media.Staff?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateMediaStaffView(media.Id), "Staff");
            }

            if (media.Relations?.Data?.Any() == true)
            {
                adapter.AddView(CreateMediaRelationsView(media.Relations.Data.ToList()), "Relations");
            }

            if (media.Studios?.Data?.Any() == true)
            {
                adapter.AddView(CreateMediaStudiosView(media.Studios.Data.ToList()), "Studios");
            }

            if (media.Tags?.Any() == true)
            {
                adapter.AddView(CreateMediaTagsView(media.Tags, media.Type), "Tags");
            }

            if (media.Rankings?.Any() == true || media.Stats?.AreStatsValid() == true)
            {
                adapter.AddView(CreateMediaUserDataView(media), "User Data");
            }

            // TODO: see if there's a better way of determining whether to display this or not
            if (Settings.IsUserAuthenticated)
            {
                adapter.AddView(CreateMediaFollowingUsersMediaListStatusView(media.Id), "Following");
            }

            if (media.Reviews?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateMediaReviewsView(media.Id), "Reviews");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        public void SetCurrentUserMediaListOptions(AniList.Models.User.UserMediaListOptions mediaListOptions)
        {
            _mediaListOptions = mediaListOptions;
        }

        public void ShowMediaListEditDialog(AniList.Models.Media.MediaList mediaList)
        {
            _editItem?.SetEnabled(true);
            EditMediaListItemDialog.Create(this, Presenter, _media, mediaList, _mediaListOptions);
        }

        public void UpdateMediaListItem(AniList.Models.Media.MediaList mediaList)
        {
            // this whole method is predicated on the fact that deletions from media lists are currently not possible throught he app
            // this logic will need to be updated once that functionality has been added

            _media.MediaListEntry = _mediaList = mediaList;

            if (_mediaDetailsView == null)
            {
                DisplaySnackbarMessage("Error occurred while updating media view", Snackbar.LengthLong);
                return;
            }

            var statusView = _mediaDetailsView.FindViewById<TextView>(Resource.Id.Media_ListStatus);
            statusView.Visibility = ViewStates.Visible;
            statusView.Text = _mediaList.Status.DisplayValue;

            var mediaListSummaryView = _mediaDetailsView.FindViewById<DataRow>(Resource.Id.Media_MediaListSummary);
            if (mediaList != null && _mediaListOptions != null)
            {
                mediaListSummaryView.Visibility = ViewStates.Visible;
                mediaListSummaryView.TextOne = $"Rating:  {mediaList.GetScoreString(_mediaListOptions.ScoreFormat)}";

                if (_media?.Type == AniList.Models.Media.MediaType.Anime)
                {
                    mediaListSummaryView.TextTwo =
                        mediaList.GetFormattedProgressString(_media.Type, _media.Episodes);
                }
                else if (_media?.Type == AniList.Models.Media.MediaType.Manga)
                {

                    mediaListSummaryView.TextTwo =
                        mediaList.GetFormattedProgressString(_media.Type, _media.Chapters);
                }
            }

            InvalidateOptionsMenu();

            if (_media.Type == AniList.Models.Media.MediaType.Anime)
            {
                var instance = MediaListFragment.GetInstance(MediaListFragment.AnimeMediaListFragmentName);

                (instance as MediaListFragment)?.UpdateMediaListItem(mediaList);
            }
            else if (_media.Type == AniList.Models.Media.MediaType.Manga)
            {
                (MediaListFragment.GetInstance(MediaListFragment.MangaMediaListFragmentName) as MediaListFragment)
                    ?.UpdateMediaListItem(mediaList);
            }
        }

        public void RemoveMediaListItem()
        {
            _media.MediaListEntry = _mediaList = null;
            Recreate();
        }

        protected override Func<Task> ToggleFavorite => () => Presenter.ToggleFavorite();

        public override bool SetupMenu(IMenu menu)
        {
            base.SetupMenu(menu);

            _editItem = menu?.FindItem(Resource.Id.Menu_AniListObject_Edit);
            _editItem?.SetIcon(_mediaList == null
                ? Resource.Drawable.svg_library_plus
                : Resource.Drawable.svg_pencil);
            _editItem?.SetVisible(_canEditListItem);

            return true;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.Menu_AniListObject_Edit)
            {
                ShowMediaListEditDialog(_mediaList);
                return true;
            }

            return base.MenuItemSelected(item);
        }

        #region Media Data

        private View CreateMediaDetailsView(AniList.Models.Media media)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_MediaDetails, null);
            retView.FindViewById<TextView>(Resource.Id.Media_Title).Text = media.Title?.UserPreferred;
            retView.FindViewById<TextView>(Resource.Id.Media_AiringStatus).Text = media.Status?.DisplayValue;
            retView.FindViewById<ExpandableText>(Resource.Id.Media_Description).TextFormatted = FromHtml(media.Description ?? "No Description");
            retView.FindViewById<ImageView>(Resource.Id.Media_TitleIcon).Click +=
                (sender, args) => MediaTitlesDialog.Create(this, media.Title, media.Synonyms);

            var formatView = retView.FindViewById<TextView>(Resource.Id.Media_Format);
            formatView.Text = (media.Format?.DisplayValue ?? "Unknown Format") +
                (media.Episodes > 1 ? $" ({media.Episodes} episodes)" : "");

            var nextAiringView = retView.FindViewById<TextView>(Resource.Id.Media_NextEpisode);
            if (media.NextAiringEpisode?.AiringAt > 0)
            {
                nextAiringView.Visibility = ViewStates.Visible;

                var airingString = !Settings.DisplayUpcomingEpisodeTimeAsCountdown
                    ? media.NextAiringEpisode.GetAiringAtDateTime().ToShortDateString()
                    : media.NextAiringEpisode.GetTimeUntilAiringTimeSpan().ToString("%d'd '%h'h '%m'm'");

                nextAiringView.Text = $"Episode {media.NextAiringEpisode?.Episode}:  {airingString}";
            }

            var listStatusView = retView.FindViewById<TextView>(Resource.Id.Media_ListStatus);
            if (media.MediaListEntry != null)
            {
                listStatusView.Visibility = ViewStates.Visible;
                listStatusView.Text = media.MediaListEntry?.Status?.DisplayValue;
            }

            LoadImage(retView.FindViewById<ImageView>(Resource.Id.Media_Image), media.CoverImage.ExtraLarge ?? media.CoverImage.Large);
            var genreContainer = retView.FindViewById<FlexboxLayout>(Resource.Id.Media_Genres);

            foreach (var genre in media.Genres)
            {
                var genreView = LayoutInflater.Inflate(Resource.Layout.Item_Category, null);
                genreView.FindViewById<TextView>(Resource.Id.Category_Text).Text = genre;
                genreView.Clickable = true;
                genreView.Click += (sender, eventArgs) => BrowseActivity.StartActivity(this, new BrowseMediaDto { Type = media.Type, IncludedGenres = new List<string> { genre }}, ObjectBrowseRequestCode);
                genreContainer.AddView(genreView);
            }

            // media list summary
            var mediaListSummaryView = retView.FindViewById<DataRow>(Resource.Id.Media_MediaListSummary);
            if (media.MediaListEntry != null && _mediaListOptions != null)
            {
                mediaListSummaryView.Visibility = ViewStates.Visible;
                mediaListSummaryView.TextOne = $"Rating:  {media.MediaListEntry.GetScoreString(_mediaListOptions.ScoreFormat)}";

                if (media.Type == AniList.Models.Media.MediaType.Anime)
                {
                    mediaListSummaryView.TextTwo =
                        media.MediaListEntry.GetFormattedProgressString(media.Type, media.Episodes);
                }
                else if (media.Type == AniList.Models.Media.MediaType.Manga)
                {

                    mediaListSummaryView.TextTwo =
                        media.MediaListEntry.GetFormattedProgressString(media.Type, media.Chapters);
                }
            }

            var dateRangeView = retView.FindViewById<DataRow>(Resource.Id.Media_DateRange);
            if (media.StartDate?.IsValid() == true || media.EndDate?.IsValid() == true)
            {
                dateRangeView.Visibility = ViewStates.Visible;
                dateRangeView.TextOne = media.GetFormattedDateRangeString();

                var startDate = media.StartDate?.GetDate();

                if (AniList.Models.Media.MediaStatus.NotYetReleased.Equals(media.Status) && startDate.HasValue &&
                    startDate.Value > DateTime.Now.Date)
                {
                    dateRangeView.ButtonVisible = true;
                    dateRangeView.ButtonClickable = true;
                    dateRangeView.ButtonClick += (sender, eventArgs) =>
                    {
                        var calIntent = new Intent(Intent.ActionEdit);
                        calIntent.SetType("vnd.android.cursor.item/event");
                        calIntent.PutExtra("beginTime", new DateTimeOffset(startDate.Value).ToUnixTimeMilliseconds());
                        calIntent.PutExtra("allDay", true);
                        calIntent.PutExtra("title", $"{media.Title?.UserPreferred} starts");
                        StartActivity(calIntent);
                    };
                }
            }

            if (media.Season != null)
            {
                dateRangeView.TextTwo = media.Season.DisplayValue + (media.StartDate?.Year > 0 ? $" {media.StartDate.Year}" : "");
                dateRangeView.Clickable = true;
                dateRangeView.Click += (sender, args) => BrowseActivity.StartActivity(this,
                    new BrowseMediaDto {Season = media.Season, SeasonYear = media.StartDate?.Year, Type = media.Type}, ObjectBrowseRequestCode);
            }

            var episodesView = retView.FindViewById<DataRow>(Resource.Id.Media_Episodes);
            if (AniList.Models.Media.MediaType.Anime.Equals(media.Type))
            {
                episodesView.TextOne = media.Episodes > 0 ? (media.Episodes > 1 ? $"{media.Episodes} episodes" : "Single episode") : "";
                episodesView.TextTwo = media.Duration > 0 ? $"{media.Duration} minutes" : "";

                if ((media.Episodes ?? 0) == 0 && (media.Duration ?? 0) == 0)
                {
                    episodesView.Visibility = ViewStates.Gone;
                }

                if (media.StreamingEpisodes?.Any() == true)
                {
                    episodesView.ButtonVisible = true;
                    episodesView.ButtonClickable = false;
                    episodesView.Clickable = true;
                    episodesView.Click += (sender, args) =>
                        MediaStreamingEpisodeListDialog.Create(this, media.StreamingEpisodes);
                }
            }
            else if (AniList.Models.Media.MediaType.Manga.Equals(media.Type))
            {
                episodesView.TextOne = media.Volumes > 0 ? $"{media.Volumes} Volumes" : "";
                episodesView.TextTwo = media.Chapters > 0 ? $"{media.Chapters} Chapters" : "";

                if ((media.Volumes ?? 0) == 0 && (media.Chapters ?? 0) == 0)
                {
                    episodesView.Visibility = ViewStates.Gone;
                }
            }

            var scoresView = retView.FindViewById(Resource.Id.Media_ScoresContainer);
            if (media.MeanScore > 30 || media.AverageScore > 30 || media.Popularity > 100)
            {
                retView.FindViewById<TextView>(Resource.Id.Media_MeanScore).Text = $"{media.MeanScore}%";
                retView.FindViewById(Resource.Id.Media_MeanScoreContainer).Visibility = media.MeanScore > 30 ? ViewStates.Visible : ViewStates.Gone;

                var avgContainer = retView.FindViewById(Resource.Id.Media_AverageScoreContainer);
                retView.FindViewById<TextView>(Resource.Id.Media_AverageScore).Text = $"{media.AverageScore}%";
                avgContainer.Visibility = media.AverageScore > 30 ? ViewStates.Visible : ViewStates.Gone;
                avgContainer.Click += (sender, args) => BrowseActivity.StartActivity(this,
                    new BrowseMediaDto
                    {
                        Type = media.Type,
                        AverageGreaterThan = media.AverageScore,
                        Sort = new List<AniList.Models.Media.MediaSort> {AniList.Models.Media.MediaSort.ScoreDesc}
                    }, ObjectBrowseRequestCode);

                var popContainer = retView.FindViewById(Resource.Id.Media_PopularityContainer);
                retView.FindViewById<TextView>(Resource.Id.Media_Popularity).Text = media.Popularity.ToString();
                popContainer.Visibility = media.Popularity > 100 ? ViewStates.Visible : ViewStates.Gone;
                popContainer.Click += (sender, args) => BrowseActivity.StartActivity(this,
                    new BrowseMediaDto
                    {
                        Type = media.Type,
                        PopularityGreaterThan = media.Popularity,
                        Sort = new List<AniList.Models.Media.MediaSort> {AniList.Models.Media.MediaSort.PopularityDesc}
                    }, ObjectBrowseRequestCode);
            }
            else
            {
                scoresView.Visibility = ViewStates.Gone;
            }

            return retView;
        }

        private View CreateMediaCharactersView(int mediaId)
        {
            var mediaCharactersEnumerable = Presenter.GetMediaCharactersEnumerable(mediaId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new CharacterEdgeRecyclerAdapter(this, mediaCharactersEnumerable, CardType,
                model => CharacterEdgeViewModel.CreateCharacterEdgeViewModel(model,
                    Resource.Drawable.ic_record_voice_over_white_24px))
            {
                ButtonIconResourceId = Resource.Drawable.ic_record_voice_over_white_24px,
                ButtonClickAction = (viewModel, position, callback) =>
                {
                    StaffListDialog.Create(this, viewModel.Model.VoiceActors);
                    callback?.Invoke();
                }
            };
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaStaffView(int mediaId)
        {
            var mediaStaffEnumerable = Presenter.GetMediaStaffEnumerable(mediaId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new StaffEdgeRecyclerAdapter(this, mediaStaffEnumerable, CardType, StaffEdgeViewModel.CreateStaffEdgeViewModel);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaRelationsView(List<AniList.Models.Media.Edge> mediaEdgeList)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaEdgeRecyclerAdapter(this,
                mediaEdgeList.Select(MediaEdgeViewModel.CreateMediaRelationViewModel).ToList(), CardType);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaStudiosView(List<AniList.Models.Studio.Edge> studioEdgeList)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new StudioEdgeRecyclerAdapter(this, studioEdgeList.Select(StudioEdgeViewModel.CreateStudioEdgeViewModel).ToList());
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaTagsView(List<AniList.Models.Media.MediaTag> tagList, AniList.Models.Media.MediaType type)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaTagsRecyclerAdapter(this, tagList, type);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaUserDataView(AniList.Models.Media media)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_NestedScrollLayout, null);
            var containerView = retView.FindViewById<LinearLayout>(Resource.Id.Scroll_Container);

            if (media.Rankings?.Any() == true)
            {
                containerView.AddView(CreateUserRankingView(media.Rankings));
            }

            if (media.Stats?.ScoreDistribution?.Count(x => x.Amount > 0) >= 3 && !AniList.Models.Media.MediaStatus.NotYetReleased.Equals(media.Status))
            {
                containerView.AddView(CreateUserScoresView(media.Stats.ScoreDistribution));
            }

            if (media.Stats?.AiringProgression?.Count >= 3)
            {
                containerView.AddView(CreateUserScoreProgressionView(media.Stats.AiringProgression.TakeLast(15)));
            }

            if (media.Stats?.StatusDistribution?.Any() == true)
            {
                containerView.AddView(CreateUserListStatusView(media.Stats.StatusDistribution));
            }

            return retView;
        }

        private View CreateMediaFollowingUsersMediaListStatusView(int mediaId)
        {
            var mediaListEnumerable = Presenter.GetMediaFollowingUsersMediaListsEnumerable(mediaId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaListRecyclerAdapter(this, CardType, mediaListEnumerable,
                MediaListViewModel.CreateUserMediaListViewModel)
            {
                ClickAction = viewModel => UserActivity.StartActivity(this, viewModel.Model?.User?.Id ?? 0)
            };
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaReviewsView(int mediaId)
        {
            var reviewsEnumarable = Presenter.GetMediaReviewsEnumerable(mediaId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new ReviewRecyclerAdapter(this, reviewsEnumarable, CardType,
                ReviewViewModel.CreateMediaReviewViewModel);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        #endregion

        #region User Data

        private View CreateUserScoresView(IEnumerable<AniList.Models.AniListObject.AniListScoreDistribution> scores)
        {
            var detailView = LayoutInflater.Inflate(Resource.Layout.View_AniListObjectDetail, null);
            var detailContainer = detailView.FindViewById<LinearLayout>(Resource.Id.AniListObjectDetail_InnerContainer);
            detailView.FindViewById<TextView>(Resource.Id.AniListObjectDetail_Name).Text = "Scores";

            var chartHeight = Resources.GetDimensionPixelSize(Resource.Dimension.Details_ChartHeight);
            var barColor = ContextCompat.GetColor(this, Resource.Color.MediaUserData_ScoreChartBar);
            var textColor = GetThemedColor(Resource.Attribute.Background_Text);

            var scoresChart = new BarChart(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, chartHeight)
            };
            var bars = scores.OrderBy(x => x.Score).Select(x => new BarEntry(x.Score, x.Amount))
                .ToList();
            var dataSet = new BarDataSet(bars, "Scores");
            dataSet.SetColor(barColor, 255);

            var data = new BarData(dataSet)
            {
                BarWidth = 9
            };

            scoresChart.Data = data;
            scoresChart.SetFitBars(true);
            scoresChart.SetDrawGridBackground(false);
            scoresChart.Description.Enabled = false;
            scoresChart.Legend.Enabled = false;
            scoresChart.AxisLeft.Enabled = false;
            scoresChart.AxisRight.Enabled = false;
            scoresChart.XAxis.SetDrawGridLines(false);
            scoresChart.XAxis.RemoveAllLimitLines();
            scoresChart.SetScaleEnabled(false);
            scoresChart.SetTouchEnabled(false);
            scoresChart.XAxis.SetLabelCount(10, false);
            scoresChart.XAxis.Granularity = 10;
            scoresChart.XAxis.Position = XAxis.XAxisPosition.Bottom;
            scoresChart.XAxis.SetDrawAxisLine(false);
            scoresChart.XAxis.ValueFormatter = new ChartUtils.AxisValueCeilingFormatter(10);

            scoresChart.XAxis.TextColor = dataSet.ValueTextColor = textColor;

            detailContainer.AddView(scoresChart);
            return detailView;
        }

        private View CreateUserScoreProgressionView(
            IEnumerable<AniList.Models.Media.MediaAiringProgression> scoreProgression)
        {
            var detailView = LayoutInflater.Inflate(Resource.Layout.View_AniListObjectDetail, null);
            var detailContainer = detailView.FindViewById<LinearLayout>(Resource.Id.AniListObjectDetail_InnerContainer);
            detailView.FindViewById<TextView>(Resource.Id.AniListObjectDetail_Name).Text = "Airing Score Progression";
            var orderedStats = scoreProgression.OrderBy(x => x.Episode).ToList();

            var chartHeight = Resources.GetDimensionPixelSize(Resource.Dimension.Details_ChartHeight);
            var textColor = GetThemedColor(Resource.Attribute.Background_Text);
            var legendMargin = Resources.GetDimensionPixelSize(Resource.Dimension.Details_MarginSmall);


            detailContainer.SetPadding(legendMargin, 0, legendMargin, 0);

            var typedColorArray = Resources.ObtainTypedArray(Resource.Array.Chart_Colors);
            var colorList = new List<int>();

            for (var i = 0; i < typedColorArray.Length(); i++)
            {
                colorList.Add(typedColorArray.GetColor(i, 0));
            }

            var scoresChart = new LineChart(this)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, chartHeight),

            };

            var data = new LineData();

            var scorePoints = orderedStats.Where(x => x.Score > 0).Select(x => new Entry(x.Episode, x.Score)).ToList();
            if (scorePoints.Any())
            {
                var scoreDataSet = new LineDataSet(scorePoints, "Score")
                {
                    AxisDependency = scoresChart.AxisLeft.GetAxisDependency(),
                    Color = colorList[0]
                };
                scoreDataSet.SetDrawCircleHole(false);
                scoreDataSet.SetCircleColor(colorList[0]);
                scoreDataSet.SetMode(LineDataSet.Mode.CubicBezier);
                scoreDataSet.ValueTextColor = textColor;

                data.AddDataSet(scoreDataSet);
            }

            var watchingPoints = orderedStats.Select(x => new Entry(x.Episode, x.Watching)).ToList();
            if (watchingPoints.Any())
            {
                var watchingDataSet = new LineDataSet(watchingPoints, "Watching")
                {
                    AxisDependency = scoresChart.AxisRight.GetAxisDependency(),
                    Color = colorList[1]
                };
                watchingDataSet.SetDrawCircleHole(false);
                watchingDataSet.SetCircleColor(colorList[1]);
                watchingDataSet.SetMode(LineDataSet.Mode.CubicBezier);
                watchingDataSet.ValueTextColor = textColor;

                data.AddDataSet(watchingDataSet);
            }

            data.SetDrawValues(false);
            scoresChart.Data = data;
            scoresChart.FitScreen();
            scoresChart.SetDrawGridBackground(false);
            scoresChart.SetTouchEnabled(false);
            scoresChart.Description.Enabled = false;
            scoresChart.XAxis.SetDrawGridLines(false);
            scoresChart.XAxis.Position = XAxis.XAxisPosition.Bottom;
            scoresChart.XAxis.ValueFormatter = new ChartUtils.AxisValueCeilingFormatter(1);
            scoresChart.XAxis.Granularity = 1;
            scoresChart.XAxis.LabelCount = Math.Min(orderedStats.Count, 12);
            scoresChart.AxisLeft.SetDrawGridLines(false);
            scoresChart.AxisLeft.Granularity = 1;
            scoresChart.AxisLeft.LabelCount = 5;
            scoresChart.AxisRight.SetDrawGridLines(false);
            scoresChart.AxisRight.Granularity = 1;

            scoresChart.XAxis.TextColor = scoresChart.AxisLeft.TextColor =
                    scoresChart.AxisRight.TextColor = scoresChart.Legend.TextColor = textColor;

            detailContainer.AddView(scoresChart);

            return detailView;
        }

        private View CreateUserListStatusView(IReadOnlyList<AniList.Models.AniListObject.AniListStatusDistribution> statusDistribution)
        {
            var detailView = LayoutInflater.Inflate(Resource.Layout.View_AniListObjectDetail, null);
            var detailContainer = detailView.FindViewById<LinearLayout>(Resource.Id.AniListObjectDetail_InnerContainer);
            detailView.FindViewById<TextView>(Resource.Id.AniListObjectDetail_Name).Text = "User Lists";
            detailContainer.Orientation = Orientation.Horizontal;

            var chartHeight = Resources.GetDimensionPixelSize(Resource.Dimension.Details_ChartHeight);
            var legendMargin = Resources.GetDimensionPixelSize(Resource.Dimension.Details_MarginSmall);

            var chartContainer = new LinearLayout(this)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, chartHeight, 1)
            };

            var legendContainer = new LinearLayout(this)
            {
                LayoutParameters =
                    new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, chartHeight, 1)
                    {
                        RightMargin = legendMargin,
                        LeftMargin = legendMargin
                    },
                Orientation = Orientation.Vertical
            };

            var typedColorArray = Resources.ObtainTypedArray(Resource.Array.Chart_Colors);
            var colorList = new List<int>();

            for (var i = 0; i < typedColorArray.Length(); i++)
            {
                colorList.Add(typedColorArray.GetColor(i, 0));
            }

            var statusChart = new PieChart(this)
            {
                LayoutParameters =
                    new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };
            var slices = statusDistribution.Select(x => new PieEntry(x.Amount, x.Status.DisplayValue) { Data = x.Status.DisplayValue }).ToList();
            var dataSet = new PieDataSet(slices, "Status")
            {
                SliceSpace = 1,
            };

            dataSet.SetDrawValues(false);
            dataSet.SetColors(colorList.ToArray(), 255);
            var data = new PieData(dataSet);

            statusChart.TransparentCircleRadius = 0;
            statusChart.HoleRadius = 0;
            statusChart.Data = data;
            statusChart.SetDrawEntryLabels(false);
            statusChart.Description.Enabled = false;
            statusChart.Legend.Enabled = false;
            statusChart.RotationEnabled = false;

            chartContainer.AddView(statusChart);

            for (var i = 0; i < statusDistribution.Count; i++)
            {
                var cell = LayoutInflater.Inflate(Resource.Layout.View_ChartLegendCell, legendContainer, false);
                var status = statusDistribution[i];
                cell.SetBackgroundColor(new Color(colorList[i % 10]));
                cell.FindViewById<TextView>(Resource.Id.ChartLegendCell_Count).Text = status.Amount.ToTruncatedString();
                cell.FindViewById<TextView>(Resource.Id.ChartLegendCell_Text).Text = status.Status.DisplayValue;
                cell.Tag = status.Status.DisplayValue;
                legendContainer.AddView(cell);
            }

            detailContainer.AddView(chartContainer);
            detailContainer.AddView(legendContainer);

            return detailView;
        }

        private View CreateUserRankingView(IEnumerable<AniList.Models.Media.MediaRank> rankings)
        {
            var detailView = LayoutInflater.Inflate(Resource.Layout.View_AniListObjectDetail, null);
            var detailContainer = detailView.FindViewById<LinearLayout>(Resource.Id.AniListObjectDetail_InnerContainer);
            detailView.FindViewById<TextView>(Resource.Id.AniListObjectDetail_Name).Text = "User Ranking";

            foreach (var ranking in rankings)
            {
                var view = LayoutInflater.Inflate(Resource.Layout.View_MediaRank, null);
                var rankingTextView = view.FindViewById<TextView>(Resource.Id.MediaRank_Text);
                var rankingIcon = view.FindViewById<ImageView>(Resource.Id.MediaRank_Image);

                rankingTextView.Text = ranking.GetFormattedRankString();
                var sortType = AniList.Models.Media.MediaSort.Id;

                if (AniList.Models.Media.MediaRankType.Rated.Equals(ranking.Type))
                {
                    sortType = AniList.Models.Media.MediaSort.ScoreDesc;
                    rankingIcon.SetImageResource(Resource.Drawable.svg_star);
                    rankingIcon.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.Favorite_Yellow)), PorterDuff.Mode.SrcIn);
                }
                else if (AniList.Models.Media.MediaRankType.Popular.Equals(ranking.Type))
                {
                    sortType = AniList.Models.Media.MediaSort.PopularityDesc;
                    rankingIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
                    rankingIcon.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.Favorite_Red)), PorterDuff.Mode.SrcIn);
                }

                view.Click += (sender, eventArgs) =>
                    BrowseActivity.StartActivity(this,
                        new BrowseMediaDto
                        {
                            Year = ranking.Year,
                            Season = ranking.Season,
                            Sort = new List<AniList.Models.Media.MediaSort> {sortType},
                            Format = ranking.Format
                        },
                        ObjectBrowseRequestCode);
                

                detailContainer.AddView(view);
            }

            return detailView;
        }

        #endregion

    }
}