using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Widgets;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Dialogs
{
    public static class EditMediaListItemDialog
    {
        public static void Create(BaseAniDroidActivity context, IAniListMediaListEditPresenter presenter, Media media, Media.MediaList mediaList, User.UserMediaListOptions mediaListOptions)
        {
            new EditMediaListItemDialogFragment(presenter, media, mediaList, mediaListOptions).Show(context.SupportFragmentManager, "EditMediaDialog");

            //var transaction = context.SupportFragmentManager.BeginTransaction();
            //transaction.SetTransition((int)FragmentTransit.FragmentOpen);
            //transaction.Add(Android.Resource.Id.Content, new EditMediaListItemDialogFragment(media, mediaListOptions)).AddToBackStack(null).Commit();
        }

        public class EditMediaListItemDialogFragment : AppCompatDialogFragment
        {
            private const int DefaultMaxPickerValue = 9999;

            private readonly IAniListMediaListEditPresenter _presenter;
            private readonly Media _media;
            private readonly Media.MediaList _mediaList;
            private readonly User.UserMediaListOptions _mediaListOptions;
            private readonly List<string> _mediaStatusList;
            private new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;

            private Picker _scorePicker;
            private AppCompatSpinner _statusSpinner;
            private Picker _progressPicker;
            private Picker _progressVolumesPicker;
            private Picker _repeatPicker;

            public EditMediaListItemDialogFragment(IAniListMediaListEditPresenter presenter, Media media, Media.MediaList mediaList, User.UserMediaListOptions mediaListOptions)
            {
                _presenter = presenter;
                _media = media;
                _mediaList = mediaList;
                _mediaListOptions = mediaListOptions;

                _mediaStatusList = AniListEnum.GetEnumValues<Media.MediaListStatus>().OrderBy(x => x.Index)
                    .Select(x => x.DisplayValue).ToList();
            }

            //public override Dialog OnCreateDialog(Bundle savedInstanceState)
            //{
            //    new AlertDialog.Builder(Activity).set
            //    return new AlertDialog.Builder(Activity).SetView(OnCreateView(Activity.LayoutInflater, null, savedInstanceState))
            //        .Create();
            //}

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_EditMediaListItem, container,
                    false);

                SetupToolbar(view.FindViewById<Toolbar>(Resource.Id.EditMediaListItem_Toolbar));
                SetupScore(_scorePicker = view.FindViewById<Picker>(Resource.Id.EditMediaListItem_ScorePicker));
                SetupStatus(_statusSpinner = view.FindViewById<AppCompatSpinner>(Resource.Id.EditMediaListItem_StatusSpinner));
                SetupProgress(_progressPicker = view.FindViewById<Picker>(Resource.Id.EditMediaListItem_ProgressPicker),
                    view.FindViewById<TextView>(Resource.Id.EditMediaListItem_ProgressLabel));
                SetupVolumeProgress(view.FindViewById(Resource.Id.EditMediaListItem_VolumeProgressContainer),
                    _progressVolumesPicker = view.FindViewById<Picker>(Resource.Id.EditMediaListItem_VolumeProgressPicker));
                SetupRepeat(_repeatPicker = view.FindViewById<Picker>(Resource.Id.EditMediaListItem_RewatchedPicker),
                    view.FindViewById<TextView>(Resource.Id.EditMediaListItem_RewatchedLabel));

                return view;
            }

            private void SetupToolbar(Toolbar toolbar)
            {
                toolbar.Title = $"Editing: {_media.Title.UserPreferred}";
                toolbar.InflateMenu(Resource.Menu.EditMediaListItem_ActionBar);
                toolbar.MenuItemClick += async (sender, args) =>
                {
                    if (args.Item.ItemId == Resource.Id.Menu_EditMediaListItem_Save)
                    {
                        await SaveMediaListItem();
                    }
                };
            }

            private void SetupScore(Picker scorePicker)
            {
                if (_mediaListOptions.ScoreFormat == User.ScoreFormat.FiveStars)
                {
                    var list = new List<string> { "★", "★★", "★★★", "★★★★", "★★\n★★★" };
                    scorePicker.SetItems(list, true, (int)(_mediaList?.Score ?? 3) - 1);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.Hundred)
                {
                    scorePicker.SetItems(Enumerable.Range(1, 100).Select(x => x.ToString()).ToList(), false, (int)(_mediaList?.Score ?? 0) - 1);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.Ten)
                {
                    scorePicker.SetItems(Enumerable.Range(1, 10).Select(x => x.ToString()).ToList(), false, (int)((_mediaList?.Score ?? 0) / 10) - 1);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.TenDecimal)
                {
                    scorePicker.SetDecimalMinMax(0, 10, _mediaList?.Score ?? 0);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.ThreeSmileys)
                {
                    scorePicker.SetDrawableItems(new List<int> { Resource.Drawable.svg_sad, Resource.Drawable.svg_neutral, Resource.Drawable.svg_happy }, (int)(_mediaList?.Score ?? 2) - 1);
                }
            }

            private void SetupStatus(AppCompatSpinner statusSpinner)
            {
                statusSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, _mediaStatusList);

                if (_mediaList?.Status != null)
                { 
                    statusSpinner.SetSelection(_mediaList.Status.Index);
                }
                else if (_media.Status == Media.MediaStatus.Releasing  || _media.Status == Media.MediaStatus.Cancelled)
                {
                    statusSpinner.SetSelection(Media.MediaListStatus.Current.Index);
                }
                else if (_media.Status == Media.MediaStatus.NotYetReleased)
                {
                    statusSpinner.SetSelection(Media.MediaListStatus.Planning.Index);
                }
                else
                {
                    statusSpinner.SetSelection(Media.MediaListStatus.Completed.Index);
                }

                statusSpinner.ItemSelected += (sender, args) => {
                    if (AniListEnum.GetEnum<Media.MediaListStatus>(args.Position) == Media.MediaListStatus.Completed && _media.Episodes > 0)
                    {
                        // TODO: replace this after altering Picker
                        // _progressPicker.SetValue(_media.Episodes);
                    }
                };
            }

            private void SetupProgress(Picker progressPicker, TextView progressLabel)
            {
                if (_media.Type == Media.MediaType.Anime)
                {
                    var episodes = _media.Episodes > 0
                        ? _media.Episodes
                        : (_media?.NextAiringEpisode?.Episode > 0 ? _media.NextAiringEpisode.Episode : DefaultMaxPickerValue);

                    progressPicker.SetItems(Enumerable.Range(1, episodes).Select(x => x.ToString()).ToList(),
                        false,
                        (_mediaList?.Progress ?? 0) - 1);
                }
                else if (_media.Type == Media.MediaType.Manga)
                {
                    progressLabel.Text = "Chapters";
                    progressPicker.SetItems(Enumerable.Range(1, _media.Chapters ?? DefaultMaxPickerValue).Select(x => x.ToString()).ToList(),
                        false,
                        (_mediaList?.Progress ?? 0) - 1);
                }
            }

            private void SetupVolumeProgress(View volumeProgressContainer, Picker volumeProgressPicker)
            {
                if (_media.Type != Media.MediaType.Manga || _media.Volumes == 0)
                {
                    return;
                }

                volumeProgressContainer.Visibility = ViewStates.Visible;
                volumeProgressPicker.SetItems(
                    Enumerable.Range(1, _media.Volumes ?? DefaultMaxPickerValue).Select(x => x.ToString()).ToList(),
                    false,
                    (_mediaList?.Progress ?? 0) - 1);
            }

            private void SetupRepeat(Picker rewatchedPicker, TextView rewatchedLabel)
            {
                if (_media.Type == Media.MediaType.Manga)
                {
                    rewatchedLabel.Text = "Reread";
                }

                rewatchedPicker.SetItems(Enumerable.Range(1, DefaultMaxPickerValue).Select(x => x.ToString()).ToList(),
                    false,
                    (_mediaList?.Repeat ?? 0) - 1);
            }

            private async Task SaveMediaListItem()
            {

                var editDto = new MediaListEditDto
                {
                    MediaId = _media.Id,
                    Status = AniListEnum.GetEnum<Media.MediaListStatus>(_statusSpinner.SelectedItemPosition),
                    Score = !_scorePicker.IsDecimal
                        ? _scorePicker.GetSelectedPosition()
                        : _scorePicker.GetDecimalValue(),
                    Progress = _progressPicker.GetSelectedPosition(),
                    ProgressVolumes = _media.Type == Media.MediaType.Manga ? _progressVolumesPicker.GetSelectedPosition() : (int?)null,
                    Repeat = _repeatPicker.GetSelectedPosition()
                };

                Dismiss();
                await _presenter.SaveMediaListEntry(editDto);
            }
        }
    }
}