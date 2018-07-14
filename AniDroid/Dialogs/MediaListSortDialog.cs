using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Base;
using AniDroid.Utils.Comparers;

namespace AniDroid.Dialogs
{
    public static class MediaListSortDialog
    {
        public static void Create(BaseAniDroidActivity context,
            MediaListSortComparer.MediaListSortType currentSort, MediaListSortComparer.MediaListSortDirection currentDirection,
            Action<MediaListSortComparer.MediaListSortType, MediaListSortComparer.MediaListSortDirection> onSelectSortAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_MediaListSort, null);
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            dialog.SetView(view);
            dialog.SetTitle("Sort Lists");

            var selectedSort = Resource.Id.MediaListSort_Default;
            switch (currentSort)
            {
                case MediaListSortComparer.MediaListSortType.Title:
                    selectedSort = Resource.Id.MediaListSort_Title;
                    break;
                case MediaListSortComparer.MediaListSortType.AverageScore:
                    selectedSort = Resource.Id.MediaListSort_AverageScore;
                    break;
                case MediaListSortComparer.MediaListSortType.Rating:
                    selectedSort = Resource.Id.MediaListSort_Rating;
                    break;
                case MediaListSortComparer.MediaListSortType.Popularity:
                    selectedSort = Resource.Id.MediaListSort_Popularity;
                    break;
                case MediaListSortComparer.MediaListSortType.DateReleased:
                    selectedSort = Resource.Id.MediaListSort_DateReleased;
                    break;
                case MediaListSortComparer.MediaListSortType.DateAdded:
                    selectedSort = Resource.Id.MediaListSort_DateAdded;
                    break;
                case MediaListSortComparer.MediaListSortType.Progress:
                    selectedSort = Resource.Id.MediaListSort_Progress;
                    break;
            }

            // set current selections
            var sortRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.MediaListSort_SortRadioGroup);
            sortRadioGroup.Check(selectedSort);

            var directionRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.MediaListSort_DirectionRadioGroup);
            directionRadioGroup.Check(currentDirection == MediaListSortComparer.MediaListSortDirection.Ascending
                ? Resource.Id.MediaListSort_Ascending
                : Resource.Id.MediaListSort_Descending);

            dialog.SetPositiveButton("Save", (sender, args) => {
                var sort = MediaListSortComparer.MediaListSortType.NoSort;
                switch (sortRadioGroup.CheckedRadioButtonId)
                {
                    case Resource.Id.MediaListSort_Title:
                        sort = MediaListSortComparer.MediaListSortType.Title;
                        break;
                    case Resource.Id.MediaListSort_AverageScore:
                        sort = MediaListSortComparer.MediaListSortType.AverageScore;
                        break;
                    case Resource.Id.MediaListSort_Rating:
                        sort = MediaListSortComparer.MediaListSortType.Rating;
                        break;
                    case Resource.Id.MediaListSort_Progress:
                        sort = MediaListSortComparer.MediaListSortType.Progress;
                        break;
                    case Resource.Id.MediaListSort_Popularity:
                        sort = MediaListSortComparer.MediaListSortType.Popularity;
                        break;
                    case Resource.Id.MediaListSort_DateReleased:
                        sort = MediaListSortComparer.MediaListSortType.DateReleased;
                        break;
                    case Resource.Id.MediaListSort_DateAdded:
                        sort = MediaListSortComparer.MediaListSortType.DateAdded;
                        break;
                }

                var direction = directionRadioGroup.CheckedRadioButtonId == Resource.Id.MediaListSort_Ascending
                    ? MediaListSortComparer.MediaListSortDirection.Ascending
                    : MediaListSortComparer.MediaListSortDirection.Descending;

                onSelectSortAction(sort, direction);
            });

            dialog.Show();
        }
    }
}