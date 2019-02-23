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
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils.Comparers;

namespace AniDroid.Dialogs
{
    public class BrowseSortDialog
    {
        public static void Create(BaseAniDroidActivity context, Media.MediaSort currentSort,
            Action<Media.MediaSort> onSelectSortAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_BrowseSort, null);
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context,
                context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            dialog.SetView(view);
            dialog.SetTitle("Sort By");

            var selectedSort = Resource.Id.BrowseSort_Popularity;
            var selectedDirection = Resource.Id.BrowseSort_Descending;

            if (Media.MediaSort.Popularity.Equals(currentSort))
            {
                selectedSort = Resource.Id.BrowseSort_Popularity;
                selectedDirection = Resource.Id.BrowseSort_Ascending;
            }
            else if (Media.MediaSort.PopularityDesc.Equals(currentSort))
            {
                selectedSort = Resource.Id.BrowseSort_Popularity;
                selectedDirection = Resource.Id.BrowseSort_Descending;
            }
            else if (Media.MediaSort.Score.Equals(currentSort))
            {
                selectedSort = Resource.Id.BrowseSort_Score;
                selectedDirection = Resource.Id.BrowseSort_Ascending;
            }
            else if (Media.MediaSort.ScoreDesc.Equals(currentSort))
            {
                selectedSort = Resource.Id.BrowseSort_Score;
                selectedDirection = Resource.Id.BrowseSort_Descending;
            }
            else if (Media.MediaSort.StartDate.Equals(currentSort))
            {
                selectedSort = Resource.Id.BrowseSort_StartDate;
                selectedDirection = Resource.Id.BrowseSort_Ascending;
            }
            else if (Media.MediaSort.StartDateDesc.Equals(currentSort))
            {
                selectedSort = Resource.Id.BrowseSort_StartDate;
                selectedDirection = Resource.Id.BrowseSort_Descending;
            }

            // set current selections
            var sortRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.BrowseSort_SortRadioGroup);
            sortRadioGroup.Check(selectedSort);

            var directionRadioGroup = view.FindViewById<RadioGroup>(Resource.Id.BrowseSort_DirectionRadioGroup);
            directionRadioGroup.Check(selectedDirection);

            dialog.SetPositiveButton("Set", (sender, args) =>
            {
                var ascending = directionRadioGroup.CheckedRadioButtonId == Resource.Id.BrowseSort_Ascending;
                Media.MediaSort sort = null;

                switch (sortRadioGroup.CheckedRadioButtonId)
                {
                    case Resource.Id.BrowseSort_Score:
                        sort = ascending ? Media.MediaSort.Score : Media.MediaSort.ScoreDesc;
                        break;
                    case Resource.Id.BrowseSort_Popularity:
                        sort = ascending ? Media.MediaSort.Popularity : Media.MediaSort.PopularityDesc;
                        break;
                    case Resource.Id.BrowseSort_StartDate:
                        sort = ascending ? Media.MediaSort.StartDate : Media.MediaSort.StartDateDesc;
                        break;
                }

                onSelectSortAction(sort);
            });

            dialog.Show();
        }
    }
}