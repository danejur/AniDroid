using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Transitions;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.General;
using AniDroid.AniList;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Browse;
using AniDroid.Widgets;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Dialogs
{
    public static class BrowseFilterDialog
    {
        public static void Create(BaseAniDroidActivity context, IBrowsePresenter presenter)
        {
            var dialog =
                new BrowseFilterDialogFragment(context, presenter)
                {
                    Cancelable = true
                };
            var transaction = context.SupportFragmentManager.BeginTransaction();
            transaction.SetTransition((int) FragmentTransit.FragmentOpen);
            transaction.Add(Android.Resource.Id.Content, dialog)
                .AddToBackStack(BrowseFilterDialogFragment.BackstackTag).Commit();
        }

        public class BrowseFilterDialogFragment : AppCompatDialogFragment
        {
            public const string BackstackTag = "BROWSE_FILTER_DIALOG";
            private const int MinimumYear = 1950;

            private readonly BaseAniDroidActivity _context;
            private readonly IBrowsePresenter _presenter;
            private readonly BrowseMediaDto _browseModel;

            private bool _pendingDismiss;
            private Toolbar _toolbar;
            private AppCompatSpinner _typeSpinner;
            private AppCompatSpinner _formatSpinner;
            private AppCompatSpinner _statusSpinner;
            private AppCompatSpinner _countrySpinner;
            private AppCompatSpinner _seasonSpinner;
            private Picker _yearPicker;
            private AppCompatSpinner _sourceSpinner;
            private AppCompatButton _genresButton;
            private AppCompatButton _tagsButton;
            private AppCompatButton _streamingOnButton;

            private ICollection<string> _selectedGenres;
            private ICollection<string> _selectedTags;
            private ICollection<string> _selectedStreamingOn;

            public BrowseFilterDialogFragment(BaseAniDroidActivity context, IBrowsePresenter presenter)
            {
                _context = context;
                _presenter = presenter;
                _browseModel = presenter.GetBrowseDto();
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
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_BrowseFilterDialog, container,
                    false);

                SetupToolbar(_toolbar = view.FindViewById<Toolbar>(Resource.Id.BrowseFilterDialog_Toolbar));

                SetupTypeSpinner(_typeSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_TypeSpinner),
                    _browseModel.Type);

                SetupFormatSpinner(
                    _formatSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_FormatSpinner),
                    _browseModel.Type, _browseModel.Format);

                SetupStatusSpinner(
                    _statusSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_StatusSpinner),
                    _browseModel.Status);

                SetupCountrySpinner(
                    _countrySpinner =
                        view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_CountrySpinner),
                    _browseModel.Country);

                SetupSourceSpinner(
                    _sourceSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_SourceSpinner),
                    _browseModel.Source);

                SetupSeasonSpinner(
                    _seasonSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_SeasonSpinner),
                    _browseModel.Type, _browseModel.Season);

                SetupYearPicker(_yearPicker = view.FindViewById<Picker>(Resource.Id.BrowseFilterDialog_YearPicker),
                    _browseModel.Year);

                _selectedGenres = _browseModel.IncludedGenres;
                SetupGenresButton(_genresButton =
                    view.FindViewById<AppCompatButton>(Resource.Id.BrowseFilterDialog_GenresButton));

                _selectedTags = _browseModel.IncludedTags;
                SetupTagsButton(_tagsButton =
                    view.FindViewById<AppCompatButton>(Resource.Id.BrowseFilterDialog_TagsButton));

                _selectedStreamingOn = _browseModel.LicensedBy;
                SetupStreamingOnButton(_streamingOnButton =
                    view.FindViewById<AppCompatButton>(Resource.Id.BrowseFilterDialog_StreamingOnButton));

                return view;
            }

            private void SetupToolbar(Toolbar toolbar)
            {
                toolbar.Title = "Browse Filters";
                toolbar.InflateMenu(Resource.Menu.BrowseFilters_ActionBar);

                toolbar.MenuItemClick += (sender, args) => {
                    if (args.Item.ItemId == Resource.Id.Menu_BrowseFilters_Apply)
                    {
                        ApplyFilters();
                    }
                    else if (args.Item.ItemId == Resource.Id.Menu_BrowseFilters_Reset)
                    {
                        ResetFilters();
                    }
                };
            }

            private void SetupTypeSpinner(AppCompatSpinner typeSpinner, Media.MediaType selectedType)
            {
                var types = AniListEnum.GetEnumValues<Media.MediaType>();

                var displayTypes = types.Select(x => x.DisplayValue).Prepend("All").ToList();

                typeSpinner.Adapter =
                    new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayTypes);

                if (selectedType != null)
                {
                    typeSpinner.SetSelection(selectedType.Index + 1);
                }

                typeSpinner.SetTag(Resource.Id.Object_Position, typeSpinner.SelectedItemPosition);

                typeSpinner.ItemSelected += (sender, args) =>
                {
                    var previousPos = (int)typeSpinner.GetTag(Resource.Id.Object_Position);

                    if (previousPos != args.Position)
                    {
                        var position = args.Position - 1;
                        var type = position >= 0 ? AniListEnum.GetEnum<Media.MediaType>(position) : null;

                        typeSpinner.SetTag(Resource.Id.Object_Position, typeSpinner.SelectedItemPosition);

                        SetupFormatSpinner(_formatSpinner, type, null);
                    }
                };
            }

            private void SetupFormatSpinner(AppCompatSpinner formatSpinner, Media.MediaType selectedType,
                Media.MediaFormat selectedFormat)
            {
                var formats = AniListEnum.GetEnumValues<Media.MediaFormat>()
                    .Where(x => selectedType == null || x.MediaType == selectedType)
                    .ToList();

                var displayFormats = formats.Select(x => x.DisplayValue).Prepend("All").ToList();

                formatSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayFormats);

                if (selectedFormat != null && selectedFormat.MediaType.Equals(selectedType))
                {
                    formatSpinner.SetSelection(formats.FindIndex(x => x.Value == selectedFormat.Value) + 1);
                }
            }

            private void SetupStatusSpinner(AppCompatSpinner statusSpinner, Media.MediaStatus selectedStatus)
            {
                var statuses = AniListEnum.GetEnumValues<Media.MediaStatus>();

                var displayStatuses = statuses.Select(x => x.DisplayValue).Prepend("All").ToList();

                statusSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayStatuses);

                if (selectedStatus != null)
                {
                    statusSpinner.SetSelection(statuses.FindIndex(x => x.Value == selectedStatus.Value) + 1);
                }
            }

            private void SetupCountrySpinner(AppCompatSpinner countrySpinner, Media.MediaCountry selectedCountry)
            {
                var countries = AniListEnum.GetEnumValues<Media.MediaCountry>();

                var displayCountries = countries.Select(x => x.DisplayValue).ToList();

                countrySpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayCountries);

                if (selectedCountry != null)
                {
                    countrySpinner.SetSelection(countries.FindIndex(x => x.Value == selectedCountry.Value));
                }
            }

            private void SetupSourceSpinner(AppCompatSpinner sourceSpinner, Media.MediaSource selectedSource)
            {
                var sources = AniListEnum.GetEnumValues<Media.MediaSource>();

                var displaySources = sources.Select(x => x.DisplayValue).Prepend("All").ToList();

                sourceSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displaySources);

                if (selectedSource != null)
                {
                    sourceSpinner.SetSelection(sources.FindIndex(x => x.Value == selectedSource.Value) + 1);
                }
            }

            private void SetupSeasonSpinner(AppCompatSpinner seasonSpinner, Media.MediaType selectedType, Media.MediaSeason selectedSeason)
            {
                var seasons = AniListEnum.GetEnumValues<Media.MediaSeason>();

                var displaySeasons = seasons.Select(x => x.DisplayValue).Prepend("All").ToList();

                seasonSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displaySeasons);

                if (selectedSeason != null)
                {
                    seasonSpinner.SetSelection(seasons.FindIndex(x => x.Value == selectedSeason.Value) + 1);
                }

                seasonSpinner.Enabled = Media.MediaType.Anime.Equals(selectedType);
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
                var dialog = new Android.Support.V7.App.AlertDialog.Builder(_context,
                    _context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                var dialogView = _context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
                var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);

                dialog.SetView(dialogView);

                var genres = _presenter.GetGenres().OrderBy(x => x).Select(x =>
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
                var dialog = new Android.Support.V7.App.AlertDialog.Builder(_context,
                    _context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                var dialogView = _context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
                var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);

                dialog.SetView(dialogView);

                var tags = _presenter.GetMediaTags().OrderBy(x => x.Name).Select(x =>
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
                var dialog = new Android.Support.V7.App.AlertDialog.Builder(_context,
                    _context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                var dialogView = _context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
                var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);

                dialog.SetView(dialogView);


                var licensees = AniListEnum.GetEnumValues<Media.MediaLicensee>().Select(x =>
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

            private void ApplyFilters()
            {
                _browseModel.Type = GetSelectedType();
                _browseModel.Format = GetSelectedFormat(_browseModel.Type);
                _browseModel.Status = GetSelectedStatus();
                _browseModel.Country = GetSelectedCountry();
                _browseModel.Source = GetSelectedSource();
                _browseModel.Year = GetSelectedYear();
                _browseModel.Season = GetSelectedSeason(_browseModel.Type);
                _browseModel.IncludedTags = GetSelectedTags();
                _browseModel.IncludedGenres = GetSelectedGenres();
                _browseModel.LicensedBy = GetSelectedStreamingOn();

                _presenter.BrowseAniListMedia(_browseModel);

                var transition = new Fade(Visibility.ModeOut);
                transition.SetDuration(300);
                ExitTransition = transition;

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
                _typeSpinner.SetSelection(0, false);
                _formatSpinner.SetSelection(0, false);
                _statusSpinner.SetSelection(0, false);
                _countrySpinner.SetSelection(0, false);
                _sourceSpinner.SetSelection(0, false);
                _yearPicker.SetValue(null);
                _seasonSpinner.SetSelection(0, false);

                _selectedTags = new List<string>();
                SetupTagsButton(_tagsButton);

                _selectedGenres = new List<string>();
                SetupGenresButton(_genresButton);

                _selectedStreamingOn = new List<string>();
                SetupStreamingOnButton(_streamingOnButton);
            }

            private Media.MediaType GetSelectedType()
            {
                var position = _typeSpinner.SelectedItemPosition - 1;
                return position >= 0 ? AniListEnum.GetEnum<Media.MediaType>(position) : null;
            }

            private Media.MediaFormat GetSelectedFormat(Media.MediaType selectedType)
            {
                var formats = AniListEnum.GetEnumValues<Media.MediaFormat>()
                    .Where(x => selectedType == null || x.MediaType == selectedType)
                    .ToList();

                var position = _formatSpinner.SelectedItemPosition - 1;

                return formats.ElementAtOrDefault(position);
            }

            private Media.MediaStatus GetSelectedStatus()
            {
                var position = _statusSpinner.SelectedItemPosition - 1;
                return position >= 0 ? AniListEnum.GetEnum<Media.MediaStatus>(position) : null;
            }

            private Media.MediaCountry GetSelectedCountry()
            {
                return AniListEnum.GetEnum<Media.MediaCountry>(_countrySpinner.SelectedItemPosition);
            }

            private Media.MediaSource GetSelectedSource()
            {
                var position = _sourceSpinner.SelectedItemPosition - 1;
                return position >= 0 ? AniListEnum.GetEnum<Media.MediaSource>(position) : null;
            }

            private int? GetSelectedYear()
            {
                return (int?) _yearPicker.GetValue();
            }

            private Media.MediaSeason GetSelectedSeason(Media.MediaType selectedType)
            {
                var position = _seasonSpinner.SelectedItemPosition - 1;
                return position >= 0 && Media.MediaType.Anime.Equals(selectedType)
                    ? AniListEnum.GetEnum<Media.MediaSeason>(position)
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
        }
    }
}