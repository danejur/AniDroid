using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.Transitions;
using AniDroid.Adapters.General;
using AniDroid.AniList;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;
using AniDroid.MediaList;
using AniDroid.Widgets;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace AniDroid.Dialogs
{
    public static class MediaListFilterDialog
    {
        public static void Create(BaseAniDroidActivity context, IMediaListView view, MediaType type, IList<string> genres, IList<MediaTag> tags)
        {
            var dialog =
                new MediaListFilterDialogFragment(context, view, type, genres, tags)
                {
                    Cancelable = true
                };
            var transaction = context.SupportFragmentManager.BeginTransaction();
            transaction.SetTransition((int)FragmentTransit.FragmentOpen);
            transaction.Add(Android.Resource.Id.Content, dialog)
                .AddToBackStack(MediaListFilterDialogFragment.BackstackTag).Commit();
        }

        public class MediaListFilterDialogFragment : AppCompatDialogFragment
        {
            public const string BackstackTag = "MEDIALIST_FILTER_DIALOG";
            private const int MinimumYear = 1950;

            private readonly BaseAniDroidActivity _context;
            private readonly IMediaListView _mediaListView;
            private readonly MediaListFilterModel _mediaListFilterModel;
            private readonly MediaType _type;
            private readonly IList<string> _genres;
            private readonly IList<MediaTag> _tags;

            private View _view;
            private bool _pendingDismiss;
            private Toolbar _toolbar;
            private AppCompatSpinner _formatSpinner;
            private AppCompatSpinner _statusSpinner;
            private AppCompatSpinner _seasonSpinner;
            private Picker _yearPicker;
            private AppCompatSpinner _sourceSpinner;
            private AppCompatButton _genresButton;
            private AppCompatButton _tagsButton;
            private AppCompatButton _streamingOnButton;
            private EditText _titleEditText;

            private ICollection<string> _selectedGenres;
            private ICollection<string> _selectedTags;
            private ICollection<string> _selectedStreamingOn;

            public MediaListFilterDialogFragment(BaseAniDroidActivity context, IMediaListView mediaListView, MediaType type, IList<string> genres, IList<MediaTag> tags)
            {
                _context = context;
                _mediaListView = mediaListView;
                _mediaListFilterModel = mediaListView.GetMediaListFilter();
                _type = type;
                _genres = genres;
                _tags = tags;
            }

            public override void OnResume()
            {
                base.OnResume();

                if (_pendingDismiss)
                {
                    Activity.SupportFragmentManager.PopBackStack(BackstackTag, (int)PopBackStackFlags.Inclusive);
                    DismissAllowingStateLoss();
                }
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                _view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_MediaListFilterDialog, container,
                    false);

                SetupToolbar(_toolbar = _view.FindViewById<Toolbar>(Resource.Id.MediaListFilterDialog_Toolbar));

                SetupFormatSpinner(
                    _formatSpinner = _view.FindViewById<AppCompatSpinner>(Resource.Id.MediaListFilterDialog_FormatSpinner),
                    _type, _mediaListFilterModel.Format);

                SetupStatusSpinner(
                    _statusSpinner = _view.FindViewById<AppCompatSpinner>(Resource.Id.MediaListFilterDialog_StatusSpinner),
                    _mediaListFilterModel.Status);

                SetupSourceSpinner(
                    _sourceSpinner = _view.FindViewById<AppCompatSpinner>(Resource.Id.MediaListFilterDialog_SourceSpinner),
                    _mediaListFilterModel.Source);

                SetupSeasonSpinner(
                    _seasonSpinner = _view.FindViewById<AppCompatSpinner>(Resource.Id.MediaListFilterDialog_SeasonSpinner),
                    _type, _mediaListFilterModel.Season);

                SetupYearPicker(_yearPicker = _view.FindViewById<Picker>(Resource.Id.MediaListFilterDialog_YearPicker),
                    _mediaListFilterModel.Year);

                _selectedGenres = _mediaListFilterModel.IncludedGenres;
                SetupGenresButton(_genresButton =
                    _view.FindViewById<AppCompatButton>(Resource.Id.MediaListFilterDialog_GenresButton));

                _selectedTags = _mediaListFilterModel.IncludedTags;
                SetupTagsButton(_tagsButton =
                    _view.FindViewById<AppCompatButton>(Resource.Id.MediaListFilterDialog_TagsButton));

                _selectedStreamingOn = _mediaListFilterModel.LicensedBy;
                SetupStreamingOnButton(_streamingOnButton =
                    _view.FindViewById<AppCompatButton>(Resource.Id.MediaListFilterDialog_StreamingOnButton));

                SetupTitleEditText(
                    _titleEditText = _view.FindViewById<EditText>(Resource.Id.MediaListFilterDialog_Title));

                return _view;
            }

            private void SetupToolbar(Toolbar toolbar)
            {
                toolbar.Title = "Media List Filters";
                toolbar.InflateMenu(Resource.Menu.MediaListFilters_ActionBar);

                toolbar.MenuItemClick += (sender, args) => {
                    if (args.Item.ItemId == Resource.Id.Menu_MediaListFilters_Apply)
                    {
                        ApplyFilters();
                    }
                    else if (args.Item.ItemId == Resource.Id.Menu_MediaListFilters_Reset)
                    {
                        ResetFilters();
                    }
                };
            }

            private void SetupFormatSpinner(AppCompatSpinner formatSpinner, MediaType selectedType,
                MediaFormat selectedFormat)
            {
                var formats = AniListEnum.GetEnumValues<MediaFormat>()
                    .Where(x => selectedType == null || x.MediaType == selectedType)
                    .ToList();

                var displayFormats = formats.Select(x => x.DisplayValue).Prepend("All").ToList();

                formatSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayFormats);

                if (selectedFormat != null && selectedFormat.MediaType.Equals(selectedType))
                {
                    formatSpinner.SetSelection(formats.FindIndex(x => x.Value == selectedFormat.Value) + 1);
                }
            }

            private void SetupStatusSpinner(AppCompatSpinner statusSpinner, MediaStatus selectedStatus)
            {
                var statuses = AniListEnum.GetEnumValues<MediaStatus>();

                var displayStatuses = statuses.Select(x => x.DisplayValue).Prepend("All").ToList();

                statusSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayStatuses);

                if (selectedStatus != null)
                {
                    statusSpinner.SetSelection(statuses.FindIndex(x => x.Value == selectedStatus.Value) + 1);
                }
            }

            private void SetupSourceSpinner(AppCompatSpinner sourceSpinner, MediaSource selectedSource)
            {
                var sources = AniListEnum.GetEnumValues<MediaSource>();

                var displaySources = sources.Select(x => x.DisplayValue).Prepend("All").ToList();

                sourceSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displaySources);

                if (selectedSource != null)
                {
                    sourceSpinner.SetSelection(sources.FindIndex(x => x.Value == selectedSource.Value) + 1);
                }
            }

            private void SetupSeasonSpinner(AppCompatSpinner seasonSpinner, MediaType selectedType, MediaSeason selectedSeason)
            {
                var seasons = AniListEnum.GetEnumValues<MediaSeason>();

                var displaySeasons = seasons.Select(x => x.DisplayValue).Prepend("All").ToList();

                seasonSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displaySeasons);

                if (selectedSeason != null)
                {
                    seasonSpinner.SetSelection(seasons.FindIndex(x => x.Value == selectedSeason.Value) + 1);
                }

                seasonSpinner.Enabled = MediaType.Anime.Equals(selectedType);
            }

            private void SetupYearPicker(Picker yearPicker, int? year)
            {
                yearPicker.SetNumericValues(DateTime.UtcNow.Year + 2, 0, false, year, MinimumYear);
            }

            private void SetupGenresButton(AppCompatButton genresButton)
            {
                genresButton.Text = _selectedGenres?.Count > 0 ? $"{_selectedGenres.Count} Selected" : "All";

                genresButton.Click -= GenresButton_Click;
                genresButton.Click += GenresButton_Click;
            }

            private void GenresButton_Click(object sender, EventArgs e)
            {
                var dialog = new AlertDialog.Builder(_context,
                    _context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                var dialogView = _context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
                var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);

                dialog.SetView(dialogView);

                var genres = _genres.OrderBy(x => x).Select(x =>
                    new CheckBoxItemRecyclerAdapter.CheckBoxItem
                    {
                        Title = x,
                        IsChecked = _selectedGenres?.Any(y => y == x) == true
                    }).ToList();

                var adapter = new CheckBoxItemRecyclerAdapter(_context, genres);

                recycler.SetAdapter(adapter);

                dialog.Show();

                dialog.DismissEvent += (disSender, disArgs) =>
                {
                    _selectedGenres = adapter.Items.Where(x => x.IsChecked).Select(x => x.Title).ToList();
                    SetupGenresButton(_genresButton);
                };
            }

            private void SetupTagsButton(AppCompatButton tagsButton)
            {
                tagsButton.Text = _selectedTags?.Count > 0 ? $"{_selectedTags.Count} Selected" : "All";

                tagsButton.Click -= TagsButton_Click;
                tagsButton.Click += TagsButton_Click;
            }

            private void TagsButton_Click(object sender, EventArgs e)
            {
                var dialog = new AlertDialog.Builder(_context,
                    _context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                var dialogView = _context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
                var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);

                dialog.SetView(dialogView);

                var tags = _tags.OrderBy(x => x.Name).Select(x =>
                    new CheckBoxItemRecyclerAdapter.CheckBoxItem
                    {
                        Title = x.Name,
                        Description = x.Description,
                        IsChecked = _selectedTags?.Any(y => y == x.Name) == true
                    }).ToList();

                var adapter = new CheckBoxItemRecyclerAdapter(_context, tags)
                {
                    ToggleDescription = true
                };

                recycler.SetAdapter(adapter);

                dialog.Show();

                dialog.DismissEvent += (disSender, disArgs) =>
                {
                    _selectedTags = adapter.Items.Where(x => x.IsChecked).Select(x => x.Title).ToList();
                    SetupTagsButton(_tagsButton);
                };
            }

            private void SetupStreamingOnButton(AppCompatButton streamingOnButton)
            {
                streamingOnButton.Text = _selectedStreamingOn?.Count > 0 ? $"{_selectedStreamingOn.Count} Selected" : "All";

                streamingOnButton.Click -= StreamingOnButton_Click;
                streamingOnButton.Click += StreamingOnButton_Click;
            }

            private void StreamingOnButton_Click(object sender, EventArgs e)
            {
                var dialog = new AlertDialog.Builder(_context,
                    _context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                var dialogView = _context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
                var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);

                dialog.SetView(dialogView);


                var licensees = AniListEnum.GetEnumValues<MediaLicensee>().Select(x =>
                    new CheckBoxItemRecyclerAdapter.CheckBoxItem
                    {
                        Title = x.DisplayValue,
                        IsChecked = _selectedStreamingOn?.Any(y => y == x.DisplayValue) == true
                    }).ToList();

                var adapter = new CheckBoxItemRecyclerAdapter(_context, licensees);

                recycler.SetAdapter(adapter);

                dialog.Show();

                dialog.DismissEvent += (disSender, disArgs) =>
                {
                    _selectedStreamingOn = adapter.Items.Where(x => x.IsChecked).Select(x => x.Title).ToList();
                    SetupStreamingOnButton(_streamingOnButton);
                };
            }

            private void SetupTitleEditText(EditText titleEditText)
            {
                titleEditText.Text = _mediaListFilterModel.Title ?? "";
            }

            private void ApplyFilters()
            {
                _mediaListFilterModel.Format = GetSelectedFormat();
                _mediaListFilterModel.Status = GetSelectedStatus();
                _mediaListFilterModel.Source = GetSelectedSource();
                _mediaListFilterModel.Year = GetSelectedYear();
                _mediaListFilterModel.Season = GetSelectedSeason();
                _mediaListFilterModel.IncludedTags = GetSelectedTags();
                _mediaListFilterModel.IncludedGenres = GetSelectedGenres();
                _mediaListFilterModel.LicensedBy = GetSelectedStreamingOn();
                _mediaListFilterModel.Title = GetTitle();

                _mediaListView.SetMediaListFilter(_mediaListFilterModel);

                var transition = new Fade(Visibility.ModeOut);
                transition.SetDuration(300);
                ExitTransition = transition;

                (_context.GetSystemService(Context.InputMethodService) as InputMethodManager)?.HideSoftInputFromWindow(
                    _view.WindowToken, HideSoftInputFlags.None);

                // TODO: there has to be a better way to do this (crashing on this line when minimizing app during save)
                try
                {
                    Activity.SupportFragmentManager.PopBackStack(BackstackTag, (int)PopBackStackFlags.Inclusive);
                    DismissAllowingStateLoss();
                }
                catch
                {
                    _pendingDismiss = true;
                }
            }

            private void ResetFilters()
            {
                _formatSpinner.SetSelection(0, false);
                _statusSpinner.SetSelection(0, false);
                _sourceSpinner.SetSelection(0, false);
                _yearPicker.SetValue(null);
                _seasonSpinner.SetSelection(0, false);

                _selectedTags = new List<string>();
                SetupTagsButton(_tagsButton);

                _selectedGenres = new List<string>();
                SetupGenresButton(_genresButton);

                _selectedStreamingOn = new List<string>();
                SetupStreamingOnButton(_streamingOnButton);

                _titleEditText.Text = "";
            }

            private MediaFormat GetSelectedFormat()
            {
                var formats = AniListEnum.GetEnumValues<MediaFormat>()
                    .Where(x => x.MediaType == _type)
                    .ToList();

                var position = _formatSpinner.SelectedItemPosition - 1;

                return formats.ElementAtOrDefault(position);
            }

            private MediaStatus GetSelectedStatus()
            {
                var position = _statusSpinner.SelectedItemPosition - 1;
                return position >= 0 ? AniListEnum.GetEnum<MediaStatus>(position) : null;
            }

            private MediaSource GetSelectedSource()
            {
                var position = _sourceSpinner.SelectedItemPosition - 1;
                return position >= 0 ? AniListEnum.GetEnum<MediaSource>(position) : null;
            }

            private int? GetSelectedYear()
            {
                return (int?)_yearPicker.GetValue();
            }

            private MediaSeason GetSelectedSeason()
            {
                var position = _seasonSpinner.SelectedItemPosition - 1;
                return position >= 0 && MediaType.Anime.Equals(_type)
                    ? AniListEnum.GetEnum<MediaSeason>(position)
                    : null;
            }

            private ICollection<string> GetSelectedGenres()
            {
                return _selectedGenres?.Any() == true ? _selectedGenres : null;
            }

            private ICollection<string> GetSelectedTags()
            {
                return _selectedTags?.Any() == true ? _selectedTags : null;
            }

            private ICollection<string> GetSelectedStreamingOn()
            {
                return _selectedStreamingOn?.Any() == true ? _selectedStreamingOn : null;
            }

            private string GetTitle()
            {
                return _titleEditText.Text ?? "";
            }
        }
    }
}