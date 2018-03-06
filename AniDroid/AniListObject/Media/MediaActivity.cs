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
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;
using AniDroid.SearchResults;
using AniDroid.Utils;
using AniDroid.Utils.Formatting;
using AniDroid.Widgets;
using Com.Google.Android.Flexbox;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using Ninject;
using Calendar = Android.Icu.Util.Calendar;

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
            adapter.AddView(CreateMediaDetailsView(media), "Details");

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
                adapter.AddView(CreateMediaTagsView(media.Tags), "Tags");
            }

            if (media.Stats != null)
            {
                adapter.AddView(CreateMediaUserDataView(media), "User Data");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        private View CreateMediaDetailsView(AniList.Models.Media media)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_MediaDetails, null);
            retView.FindViewById<TextView>(Resource.Id.Media_Title).Text = media.Title?.UserPreferred;
            retView.FindViewById<TextView>(Resource.Id.Media_AiringStatus).Text = media.Status?.DisplayValue;
            retView.FindViewById<TextView>(Resource.Id.Media_Description).TextFormatted = FromHtml(media.Description ?? "No Description");

            var formatView = retView.FindViewById<TextView>(Resource.Id.Media_Format);
            formatView.Text = (media.Format?.DisplayValue ?? "Unknown Format") +
                (media.Episodes > 1 ? $" ({media.Episodes} episodes)" : "");

            LoadImage(retView.FindViewById<ImageView>(Resource.Id.Media_Image), media.CoverImage.Large);
            var genreContainer = retView.FindViewById<FlexboxLayout>(Resource.Id.Media_Genres);

            foreach (var genre in media.Genres)
            {
                var genreView = LayoutInflater.Inflate(Resource.Layout.Item_Category, null);
                genreView.FindViewById<TextView>(Resource.Id.Category_Text).Text = genre;
                genreView.Clickable = true;
                genreView.Click += (sender, eventArgs) => DisplaySnackbarMessage("Not Implemented Yet", Snackbar.LengthShort);
                genreContainer.AddView(genreView);
            }

            var dateRangeView = retView.FindViewById<AniDroidDataRow>(Resource.Id.Media_DateRange);
            dateRangeView.TextOne = media.GetFormattedDateRangeString();

            if (AniList.Models.Media.MediaStatus.NotYetReleased.Equals(media.Status))
            {
                var startDate = media.StartDate.GetDate();

                if (startDate.HasValue && startDate.Value > DateTime.Now.Date)
                {
                    dateRangeView.Clickable = true;
                    dateRangeView.Click += (sender, eventArgs) =>
                    {
                        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteCalendar) == (int)Permission.Granted)
                        {
                            var calIntent = new Intent(Intent.ActionEdit);
                            calIntent.SetType("vnd.android.cursor.item/event");
                            calIntent.PutExtra("beginTime", new DateTimeOffset(startDate.Value).ToUnixTimeMilliseconds());
                            calIntent.PutExtra("allDay", true);
                            calIntent.PutExtra("title", $"{media.Title?.UserPreferred} starts");
                            StartActivity(calIntent);
                        }
                        else
                        {
                            DisplaySnackbarMessage("Calendar permission required to add reminder", Snackbar.LengthShort);
                        }
                    };
                }
            }

            if (media.Season != null)
            {
                dateRangeView.TextTwo = media.Season.DisplayValue + (media.StartDate?.Year > 0 ? $" {media.StartDate.Year}" : "");
            }

            return retView;
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

        private View CreateMediaRelationsView(List<AniList.Models.Media.Edge> mediaEdgeList)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaRelationsRecyclerAdapter(this, mediaEdgeList, CardType);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaStudiosView(List<AniList.Models.Studio.Edge> studioEdgeList)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaStudiosRecyclerAdapter(this, studioEdgeList);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaTagsView(List<AniList.Models.Media.MediaTag> tagList)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaTagsRecyclerAdapter(this, tagList);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }

        private View CreateMediaUserDataView(AniList.Models.Media media)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_ScrollLayout, null);
            var containerView = retView.FindViewById<LinearLayout>(Resource.Id.Scroll_Container);

            if (media.Rankings?.Any() == true)
            {
                containerView.AddView(CreateUserRankingView(media.Rankings));
            }

            if (media.Stats?.ScoreDistribution?.Any() == true && !AniList.Models.Media.MediaStatus.NotYetReleased.Equals(media.Status))
            {
                containerView.AddView(CreateUserScoresView(media.Stats.ScoreDistribution));
            }

            if (media.Stats?.AiringProgression?.Count > 3)
            {
                containerView.AddView(CreateUserScoreProgressionView(media.Stats.AiringProgression.TakeLast(15)));
            }

            if (media.Stats?.StatusDistribution?.Any() == true)
            {
                containerView.AddView(CreateUserListStatusView(media.Stats.StatusDistribution));
            }

            return retView;
        }

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

                if (AniList.Models.Media.MediaRankType.Rated.Equals(ranking.Type))
                {
                    rankingIcon.SetImageResource(Resource.Drawable.ic_star_white_24px);
                    rankingIcon.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.Favorite_Yellow)), PorterDuff.Mode.SrcIn);
                }
                else if (AniList.Models.Media.MediaRankType.Popular.Equals(ranking.Type))
                {
                    rankingIcon.SetImageResource(Resource.Drawable.ic_favorite_white_24px);
                    rankingIcon.SetColorFilter(new Color(ContextCompat.GetColor(this, Resource.Color.Favorite_Red)), PorterDuff.Mode.SrcIn);
                }

                detailContainer.AddView(view);
            }

            return detailView;
        }

        #endregion

    }
}