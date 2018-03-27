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
using AniDroid.Settings;
using AniDroid.Widgets;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Dialogs
{
    public static class EditMediaListItemDialog
    {
        public static void Create(BaseAniDroidActivity context, IAniListMediaListEditPresenter presenter, Media media, Media.MediaList mediaList, User.UserMediaListOptions mediaListOptions)
        {
            var dialog = new EditMediaListItemDialogFragment(presenter, media, mediaList, mediaListOptions) {Cancelable = true};

//            dialog.Show(context.SupportFragmentManager, "EditMediaDialog");

            var transaction = context.SupportFragmentManager.BeginTransaction();
            transaction.SetTransition((int)FragmentTransit.FragmentOpen);
            transaction.Add(Android.Resource.Id.Content, dialog).AddToBackStack(null).Commit();
        }

        public class EditMediaListItemDialogFragment : AppCompatDialogFragment
        {
            private const int DefaultMaxPickerValue = 9999;

            private readonly IAniListMediaListEditPresenter _presenter;
            private readonly Media _media;
            private readonly Media.MediaList _mediaList;
            private readonly User.UserMediaListOptions _mediaListOptions;
            private readonly List<string> _mediaStatusList;
            private readonly HashSet<string> _customLists;
            private new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;
            private bool _isPrivate;
            private bool _hideFromStatusLists;

            private Picker _scorePicker;
            private AppCompatSpinner _statusSpinner;
            private Picker _progressPicker;
            private Picker _progressVolumesPicker;
            private Picker _repeatPicker;
            private EditText _notesView;

            public EditMediaListItemDialogFragment(IAniListMediaListEditPresenter presenter, Media media, Media.MediaList mediaList, User.UserMediaListOptions mediaListOptions)
            {
                _presenter = presenter;
                _media = media;
                _mediaList = mediaList;
                _mediaListOptions = mediaListOptions;
                _isPrivate = mediaList?.Private ?? false;
                _hideFromStatusLists = mediaList?.HiddenFromStatusLists ?? false;
                _customLists = (mediaList?.CustomLists?.Where(x => x.Enabled).Select(x => x.Name).ToList() ??
                               new List<string>()).ToHashSet();

                _mediaStatusList = AniListEnum.GetEnumValues<Media.MediaListStatus>().OrderBy(x => x.Index)
                    .Select(x => x.DisplayValue).ToList();
            }

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
                SetupNotes(_notesView = view.FindViewById<EditText>(Resource.Id.EditMediaListItem_Notes));
                SetupCustomLists(view.FindViewById(Resource.Id.EditMediaListItem_CustomListsContainer),
                    view.FindViewById<LinearLayout>(Resource.Id.EditMediaListItem_CustomLists));

                return view;
            }

            private void SetupToolbar(Toolbar toolbar)
            {
                toolbar.Title = $"{(_mediaList == null ? "Adding" : "Editing")}: {_media.Title.UserPreferred}";
                toolbar.InflateMenu(Resource.Menu.EditMediaListItem_ActionBar);

                var privateItem = toolbar.Menu.FindItem(Resource.Id.Menu_EditMediaListItem_MarkPrivate);
                SetupIsPrivate(privateItem);

                toolbar.MenuItemClick += async (sender, args) =>
                {
                    if (args.Item.ItemId == Resource.Id.Menu_EditMediaListItem_Save)
                    {
                        await SaveMediaListItem();
                    }
                    else if (args.Item.ItemId == Resource.Id.Menu_EditMediaListItem_MarkPrivate)
                    {
                        _isPrivate = !_isPrivate;
                        SetupIsPrivate(privateItem);
                    }
                };
            }

            private void SetupIsPrivate(IMenuItem isPrivateItem)
            {
                isPrivateItem.SetIcon(_isPrivate ? Resource.Drawable.svg_eye_off : Resource.Drawable.svg_eye);
                isPrivateItem.SetTitle(_isPrivate ? "Mark as Public" : "Mark as Private");
            }

            private void SetupScore(Picker scorePicker)
            {
                if (_mediaListOptions.ScoreFormat == User.ScoreFormat.FiveStars)
                {
                    var list = new List<string> { "★", "★★", "★★★", "★★★★", "★★\n★★★" };
                    scorePicker.SetStringItems(list, (int)(_mediaList?.Score ?? 3) - 1);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.Hundred)
                {
                    scorePicker.SetMaxValue(100, 0, false, _mediaList?.Score);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.Ten)
                {
                    scorePicker.SetMaxValue(10, 0, false, _mediaList?.Score);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.TenDecimal)
                {
                    scorePicker.SetMaxValue(10, 1, false, _mediaList?.Score);
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
                        ? _media.Episodes.Value
                        : (_media?.NextAiringEpisode?.Episode > 0 ? _media.NextAiringEpisode.Episode : DefaultMaxPickerValue);

                    progressPicker.SetMaxValue(episodes, 0, false, _mediaList?.Progress);
                }
                else if (_media.Type == Media.MediaType.Manga)
                {
                    progressLabel.Text = "Chapters";
                    progressPicker.SetMaxValue(_media.Chapters ?? DefaultMaxPickerValue, 0, false, _mediaList?.Progress);
                }
            }

            private void SetupVolumeProgress(View volumeProgressContainer, Picker volumeProgressPicker)
            {
                if (_media.Type != Media.MediaType.Manga || _media.Volumes == 0)
                {
                    volumeProgressContainer.Visibility = ViewStates.Gone;
                    return;
                }

                volumeProgressContainer.Visibility = ViewStates.Visible;
                volumeProgressPicker.SetMaxValue(_media.Volumes ?? DefaultMaxPickerValue, 0, false, _mediaList?.ProgressVolumes);
            }

            private void SetupRepeat(Picker rewatchedPicker, TextView rewatchedLabel)
            {
                if (_media.Type == Media.MediaType.Manga)
                {
                    rewatchedLabel.Text = "Reread";
                }

                rewatchedPicker.SetMaxValue(DefaultMaxPickerValue, 0, false, _mediaList?.Repeat);
            }

            private void SetupNotes(TextView notesView)
            {
                notesView.Text = _mediaList?.Notes;
            }

            private void SetupCustomLists(View customListsContainer, LinearLayout customLists)
            {
                var lists = _media.Type == Media.MediaType.Anime
                    ? _mediaListOptions?.AnimeList?.CustomLists
                    : _mediaListOptions?.MangaList?.CustomLists;

                if (lists?.Any() != true)
                {
                    customListsContainer.Visibility = ViewStates.Gone;
                    return;
                }

                var hideSwitchRow =
                    SettingsActivity.CreateSwitchSettingRow(Activity, null, "Hide from status lists", _hideFromStatusLists,
                        (sender, eventArgs) => {
                            if (eventArgs.IsChecked && _customLists.Count == 0)
                            {
                                (sender as SwitchCompat).Checked = false;
                                Toast.MakeText(Activity, "Must be on at least one list!", ToastLength.Short).Show();
                                return;
                            }

                            _hideFromStatusLists = eventArgs.IsChecked;
                        });
                var hideSwitchView = hideSwitchRow.FindViewById<SwitchCompat>(Resource.Id.SettingItem_Switch);

                foreach (var list in lists)
                {
                    customLists.AddView(SettingsActivity.CreateCheckboxSettingRow(Activity, null, list, _customLists.Contains(list),
                        (sender, eventArgs) => {
                            if (eventArgs.IsChecked)
                            {
                                _customLists.Add(list);
                            }
                            else
                            {
                                _customLists.Remove(list);

                                if (_customLists.Count == 0)
                                {
                                    hideSwitchView.Checked = false;
                                }
                            }
                        }));
                }

                customLists.AddView(SettingsActivity.CreateSettingDivider(Activity));
                customLists.AddView(hideSwitchRow);
            }

            private async Task SaveMediaListItem()
            {

                var editDto = new MediaListEditDto
                {
                    MediaId = _media.Id,
                    Status = AniListEnum.GetEnum<Media.MediaListStatus>(_statusSpinner.SelectedItemPosition),
                    Score = _scorePicker.GetValue(),
                    Progress = (int?) _progressPicker.GetValue(),
                    ProgressVolumes = _media.Type == Media.MediaType.Manga ? (int?)_progressVolumesPicker.GetValue() : null,
                    Repeat = (int?)_repeatPicker.GetValue(),
                    Notes = _notesView.Text,
                    Private = _isPrivate,
                    CustomLists = _customLists.ToList(),
                    HiddenFromStatusLists = _hideFromStatusLists

                };

                if ((_mediaListOptions.ScoreFormat == User.ScoreFormat.FiveStars ||
                    _mediaListOptions.ScoreFormat == User.ScoreFormat.ThreeSmileys) && editDto.Score.HasValue)
                {
                    editDto.Score += 1;
                }

                Dismiss();
                await _presenter.SaveMediaListEntry(editDto);
            }
        }
    }
}