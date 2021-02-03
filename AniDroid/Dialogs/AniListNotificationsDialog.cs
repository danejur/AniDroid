using System;
using System.Collections.Generic;
using Android.Graphics;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AniDroid.Adapters.UserAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.Base;
using OneOf;

namespace AniDroid.Dialogs
{
    public static class AniListNotificationsDialog
    {
        public static void Create(BaseAniDroidActivity context, IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> enumerable, int unreadCount, Action dataLoadedAction = null)
        {
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
            dialogView.SetBackgroundColor(Color.Transparent);
            var recycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var adapter =
                new AniListNotificationRecyclerAdapter(context, enumerable, unreadCount,
                    viewModel => AniListNotificationViewModel.CreateViewModel(viewModel, context,
                        new Color(context.GetThemedColor(Resource.Attribute.Primary))))
                {
                    LoadingItemBackgroundColor = Color.Transparent
                };
            adapter.DataLoaded += (sender, b) => dataLoadedAction?.Invoke();
            recycler.SetAdapter(adapter);
            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme))
                .SetView(dialogView)
                .Create();
            
            dialog.Show();
        }
    }
}