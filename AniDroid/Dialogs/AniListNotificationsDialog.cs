using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Dialogs
{
    public static class AniListNotificationsDialog
    {
        public static void Create(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> enumerable, Action dataLoadedAction = null)
        {
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
            dialogView.SetBackgroundColor(Color.Transparent);
            var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var adapter =
                new AniListNotificationRecyclerAdapter(context, enumerable)
                {
                    LoadingItemBackgroundColor = Color.Transparent
                };
            adapter.DataLoaded += (sender, b) => dataLoadedAction?.Invoke();
            recycler.SetAdapter(adapter);
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme))
                .SetView(dialogView)
                .Create();
            
            dialog.Show();
        }
    }
}