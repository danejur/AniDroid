using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.AniList;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Dialogs
{
    public static class BrowseFilterDialog
    {
        public static void Create(BaseAniDroidActivity context, BrowseMediaDto dto)
        {
            var dialog =
                new BrowseFilterDialogFragment(dto)
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

            private readonly BrowseMediaDto _browseModel;

            private AppCompatSpinner _typeSpinner;
            private AppCompatSpinner _formatSpinner;
            private AppCompatSpinner _statusSpinner;
            private AppCompatSpinner _countrySpinner;
            private AppCompatSpinner _seasonSpinner;

            //private CoordinatorLayout _coordLayout;
            //private Picker _scorePicker;
            //private AppCompatSpinner _statusSpinner;
            //private Picker _progressPicker;
            //private Picker _progressVolumesPicker;
            //private Picker _repeatPicker;
            //private EditText _notesView;
            //private DatePickerTextView _startDateView;
            //private DatePickerTextView _finishDateView;

            public BrowseFilterDialogFragment(BrowseMediaDto dto)
            {
                _browseModel = dto;
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_BrowseFilterDialog, container,
                    false);

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

                SetupSeasonSpinner(
                    _seasonSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.BrowseFilterDialog_SeasonSpinner),
                    _browseModel.Season);

                return view;
            }

            private void SetupTypeSpinner(AppCompatSpinner typeSpinner, Media.MediaType selectedType)
            {
                var types = AniListEnum.GetEnumValues<Media.MediaType>();

                var displayTypes = types.Select(x => x.DisplayValue).Prepend("All").ToList();

                typeSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayTypes);

                if (selectedType != null)
                {
                    typeSpinner.SetSelection(selectedType.Index + 1);
                }
            }

            private void SetupFormatSpinner(AppCompatSpinner formatSpinner, Media.MediaType selectedType,
                Media.MediaFormat selectedFormat)
            {
                var formats = AniListEnum.GetEnumValues<Media.MediaFormat>().Where(x => x.MediaType == selectedType)
                    .ToList();

                var displayFormats = formats.Select(x => x.DisplayValue).Prepend("All").ToList();

                formatSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displayFormats);

                if (selectedFormat != null)
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

            private void SetupSeasonSpinner(AppCompatSpinner seasonSpinner, Media.MediaSeason selectedSeason)
            {
                var seasons = AniListEnum.GetEnumValues<Media.MediaSeason>();

                var displaySeasons = seasons.Select(x => x.DisplayValue).Prepend("All").ToList();

                seasonSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, displaySeasons);

                if (selectedSeason != null)
                {
                    seasonSpinner.SetSelection(seasons.FindIndex(x => x.Value == selectedSeason.Value) + 1);
                }
            }
        }
    }
}