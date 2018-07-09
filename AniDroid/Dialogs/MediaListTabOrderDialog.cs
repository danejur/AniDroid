﻿using System;
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
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Base;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Animator;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;

namespace AniDroid.Dialogs
{
    public class MediaListTabOrderDialog
    {
        public static void Create(BaseAniDroidActivity context, List<KeyValuePair<string, bool>> mediaListTabs)
        {

            var view = context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dragDropManager = new RecyclerViewDragDropManager();

            var adapter =
                dragDropManager.CreateWrappedAdapter(new MediaListTabOrderRecyclerAdapter(context,
                    mediaListTabs.Select(x => new BaseRecyclerAdapter.StableIdItem<KeyValuePair<string, bool>>(x))
                        .ToList()));
            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(context));

            //recyclerView.SetItemAnimator(new DraggableItemAnimator());

            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            dialog.SetView(view);
            dialog.SetCancelable(true);
            dialog.Show();

            dragDropManager.AttachRecyclerView(recyclerView);
        }
    }
}