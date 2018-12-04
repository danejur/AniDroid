using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.General;
using AniDroid.AniList;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Dialogs
{
    public static class MediaListFilterDialog
    {
        public static void Create(BaseAniDroidActivity context, Media.MediaType mediaType, IList<Media.MediaStatus> statusList,
            IList<Media.MediaFormat> formatList,
            Action<IList<Media.MediaStatus>, IList<Media.MediaFormat>> saveFilterAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_MediaListFilter, null);
            var formatRecycler = view.FindViewById<RecyclerView>(Resource.Id.MediaListFilter_FormatRecyclerView);
            var statusRecycler = view.FindViewById<RecyclerView>(Resource.Id.MediaListFilter_StatusRecyclerView);
            formatRecycler.SetLayoutManager(new LinearLayoutManager(context));
            statusRecycler.SetLayoutManager(new LinearLayoutManager(context));

            var formats = AniListEnum.GetEnumValues<Media.MediaFormat>().Where(x => x.MediaType == mediaType).ToList();
            var statuses = AniListEnum.GetEnumValues<Media.MediaStatus>().ToList();

            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            dialog.SetView(view);
            dialog.SetTitle("Filter Lists");
            dialog.SetPositiveButton("Save", (sender, obj) =>
            {
                var retStatuses = statuses.Where(z =>
                    (statusRecycler.GetAdapter() as CheckBoxItemRecyclerAdapter)?.Items.Where(x => x.IsChecked)
                    .Select(x => x.Id).ToList().Contains(z.Index) == true).ToList();
                var retFormats = formats.Where(z =>
                    (formatRecycler.GetAdapter() as CheckBoxItemRecyclerAdapter)?.Items.Where(x => x.IsChecked)
                    .Select(x => x.Id).ToList().Contains(z.Index) == true).ToList();

                saveFilterAction?.Invoke(retStatuses, retFormats);
            });

            dialog.SetNegativeButton("Cancel", (sender, obj) => {

            });

            dialog.SetNeutralButton("Reset", (sender, obj) =>
                {
                    saveFilterAction?.Invoke(new List<Media.MediaStatus>(), new List<Media.MediaFormat>());
                });

            formatRecycler.SetAdapter(new CheckBoxItemRecyclerAdapter(context, formats.Select(x =>
                new CheckBoxItemRecyclerAdapter.CheckBoxItem
                {
                    Title = x.DisplayValue,
                    IsChecked = formatList.Any(y => y == x),
                    Id = x.Index
                }).ToList()));

            statusRecycler.SetAdapter(new CheckBoxItemRecyclerAdapter(context, statuses.Select(x =>
                new CheckBoxItemRecyclerAdapter.CheckBoxItem
                {
                    Title = x.DisplayValue,
                    IsChecked = statusList.Any(y => y == x),
                    Id = x.Index
                }).ToList()));

            dialog.SetCancelable(true);
            dialog.Show();
        }
    }
}