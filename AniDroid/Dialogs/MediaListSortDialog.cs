using System;
using Android.Widget;
using AndroidX.AppCompat.App;
using AniDroid.Base;
using AniDroid.Utils.Comparers;

namespace AniDroid.Dialogs
{
    public static class MediaListSortDialog
    {
        public static void Create(BaseAniDroidActivity context,
            MediaListSortComparer.MediaListSortType currentSort, MediaListSortComparer.SortDirection currentDirection,
            Action<MediaListSortComparer.MediaListSortType, MediaListSortComparer.SortDirection> onSelectSortAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_MediaListSort, null);
            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
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
                case MediaListSortComparer.MediaListSortType.DateLastUpdated:
                    selectedSort = Resource.Id.MediaListSort_DateLastUpdated;
                    break;
                case MediaListSortComparer.MediaListSortType.Duration:
                    selectedSort = Resource.Id.MediaListSort_Duration;
                    break;
                case MediaListSortComparer.MediaListSortType.DateStarted:
                    selectedSort = Resource.Id.MediaListSort_DateStarted;
                    break;
                case MediaListSortComparer.MediaListSortType.DateCompleted:
                    selectedSort = Resource.Id.MediaListSort_DateCompleted;
                    break;
                case MediaListSortComparer.MediaListSortType.NextEpisodeDate:
                    selectedSort = Resource.Id.MediaListSort_NextEpisodeDate;
                    break;
            }

            // set current selections
            var sortRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.MediaListSort_SortRadioGroup);
            sortRadioGroup.Check(selectedSort);

            var directionRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.MediaListSort_DirectionRadioGroup);
            directionRadioGroup.Check(currentDirection == MediaListSortComparer.SortDirection.Ascending
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
                    case Resource.Id.MediaListSort_DateLastUpdated:
                        sort = MediaListSortComparer.MediaListSortType.DateLastUpdated;
                        break;
                    case Resource.Id.MediaListSort_Duration:
                        sort = MediaListSortComparer.MediaListSortType.Duration;
                        break;
                    case Resource.Id.MediaListSort_DateStarted:
                        sort = MediaListSortComparer.MediaListSortType.DateStarted;
                        break;
                    case Resource.Id.MediaListSort_DateCompleted:
                        sort = MediaListSortComparer.MediaListSortType.DateCompleted;
                        break;
                    case Resource.Id.MediaListSort_NextEpisodeDate:
                        sort = MediaListSortComparer.MediaListSortType.NextEpisodeDate;
                        break;
                }

                var direction = directionRadioGroup.CheckedRadioButtonId == Resource.Id.MediaListSort_Ascending
                    ? MediaListSortComparer.SortDirection.Ascending
                    : MediaListSortComparer.SortDirection.Descending;

                onSelectSortAction(sort, direction);
            });

            dialog.Show();
        }
    }
}