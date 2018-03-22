using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AniDroid.AniList;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Widgets;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Dialogs
{
    public static class EditMediaListItemDialog
    {
        public static void Create(BaseAniDroidActivity context, Media media, User.UserMediaListOptions mediaListOptions)
        {
            new EditMediaListItemDialogFragment(media, mediaListOptions).Show(context.SupportFragmentManager, "EditMediaDialog");

            //var transaction = context.SupportFragmentManager.BeginTransaction();
            //transaction.SetTransition((int)FragmentTransit.FragmentOpen);
            //transaction.Add(Android.Resource.Id.Content, new EditMediaListItemDialogFragment(media, mediaListOptions)).AddToBackStack(null).Commit();
        }

        public class EditMediaListItemDialogFragment : AppCompatDialogFragment
        {
            private readonly Media _media;
            private readonly User.UserMediaListOptions _mediaListOptions;
            private readonly List<string> _mediaStatusList;
            private new BaseAniDroidActivity Activity => base.Activity as BaseAniDroidActivity;


            public EditMediaListItemDialogFragment(Media media, User.UserMediaListOptions mediaListOptions)
            {
                _media = media;
                _mediaListOptions = mediaListOptions;

                _mediaStatusList = AniListEnum.GetEnumValues<Media.MediaListStatus>().OrderBy(x => x.Index)
                    .Select(x => x.DisplayValue).ToList();
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                return new AlertDialog.Builder(Activity).SetView(OnCreateView(Activity.LayoutInflater, null, savedInstanceState))
                    .Create();
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = Activity.LayoutInflater.Inflate(Resource.Layout.Fragment_EditMediaListItem, container,
                    false);

                SetupToolbar(view.FindViewById<Toolbar>(Resource.Id.EditMediaListItem_Toolbar));
                SetupScore(view.FindViewById<Picker>(Resource.Id.EditMediaListItem_ScorePicker));
                SetupStatus(view.FindViewById<Spinner>(Resource.Id.EditMediaListItem_StatusSpinner));
                SetupProgress(view.FindViewById<Picker>(Resource.Id.EditMediaListItem_ProgressPicker));

                return view;
            }

            private void SetupToolbar(Toolbar toolbar)
            {
                toolbar.Title = $"Editing: {_media.Title.UserPreferred}";
            }

            private void SetupScore(Picker scorePicker)
            {
                if (_mediaListOptions.ScoreFormat == User.ScoreFormat.FiveStars)
                {
                    var list = new List<string> { "★", "★★", "★★★", "★★★★", "★★\n★★★" };
                    scorePicker.SetItems(list, true, (int)(_media.MediaListEntry?.Score ?? 0) - 1);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.Hundred)
                {
                    scorePicker.SetItems(Enumerable.Range(1, 100).Select(x => x.ToString()).ToList(), false, (int)(_media.MediaListEntry?.Score ?? 0));
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.Ten)
                {
                    scorePicker.SetItems(Enumerable.Range(1, 10).Select(x => x.ToString()).ToList(), false, (int)((_media.MediaListEntry?.Score ?? 0) / 10));
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.TenDecimal)
                {
                    scorePicker.SetDecimalMinMax(0, 10, _media.MediaListEntry?.Score ?? 0);
                }
                else if (_mediaListOptions.ScoreFormat == User.ScoreFormat.ThreeSmileys)
                {
                    scorePicker.SetDrawableItems(new List<int> { Resource.Drawable.svg_sad, Resource.Drawable.svg_neutral, Resource.Drawable.svg_happy }, (int)(_media.MediaListEntry?.Score ?? 0) - 1);
                }
            }

            private void SetupStatus(Spinner statusSpinner)
            {
                statusSpinner.Adapter = new ArrayAdapter<string>(Activity, Resource.Layout.View_SpinnerDropDownItem, _mediaStatusList);
                statusSpinner.SetSelection(_media.MediaListEntry?.Status?.Index ?? 0);
            }

            private void SetupProgress(Picker progressPicker)
            {
                if (_media.Type == Media.MediaType.Anime)
                {
                    var episodes = _media.Episodes > 0
                        ? _media.Episodes
                        : (_media?.NextAiringEpisode?.Episode > 0 ? _media.NextAiringEpisode.Episode : 9999);

                    progressPicker.SetItems(Enumerable.Range(1, episodes).Select(x => x.ToString()).ToList(),
                        false,
                        (_media.MediaListEntry?.Progress ?? 0) - 1);
                }
                else if (_media.Type == Media.MediaType.Manga)
                {
                }
            }
        }
    }
}