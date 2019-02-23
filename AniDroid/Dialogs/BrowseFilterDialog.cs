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
                new BrowseFilterDialogFragment(presenter)
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

            private readonly IBrowsePresenter _presenter;
            private readonly BrowseMediaDto _browseModel;

            private bool _pendingDismiss;
            private Toolbar _toolbar;
            private LinearLayout _animeSection;
            private AppCompatSpinner _typeSpinner;
            private AppCompatSpinner _formatSpinner;
            private AppCompatSpinner _statusSpinner;
            private AppCompatSpinner _countrySpinner;
            private AppCompatSpinner _seasonSpinner;
            private AppCompatSpinner _sourceSpinner;
            private Button _genresAndTagsButton;
            private Button _streamingOnButton;
            private Picker _yearFromPicker;
            private Picker _yearToPicker;
            private Picker _yearPicker;

            public BrowseFilterDialogFragment(IBrowsePresenter presenter)
            {
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

                SetupAnimeSection(
                    _animeSection = view.FindViewById<LinearLayout>(Resource.Id.BrowseDialog_AnimeSection),
                    _browseModel.Type);

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

                //SetupYearRangePickers(_yearFromPicker = view.FindViewById<Picker>(Resource.Id.BrowseFilterDialog_YearFromPicker), _yearToPicker = view.FindViewById<Picker>(Resource.Id.BrowseFilterDialog_YearToPicker), _browseModel.year);

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
                };
            }

            private void SetupAnimeSection(LinearLayout animeSection, Media.MediaType selectedType)
            {
                animeSection.Visibility =
                    Media.MediaType.Anime.Equals(selectedType) ? ViewStates.Visible : ViewStates.Gone;
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
                        SetupAnimeSection(_animeSection, type);
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

                seasonSpinner.Visibility = Media.MediaType.Anime.Equals(selectedType) ? ViewStates.Visible : ViewStates.Gone;
            }

            private void SetupYearPicker(Picker yearPicker, int? year)
            {
                yearPicker.SetNumericValues(DateTime.UtcNow.Year + 2, 0, false, year, MinimumYear);
            }

            private void SetupYearRangePickers(Picker fromYearPicker, Picker toYearPicker, int? fromYear, int? toYear)
            {
                
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
        }
    }
}