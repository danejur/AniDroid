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
            var layoutManager = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);

            var dragDropManager = new RecyclerViewDragDropManager();

            var adapter = new MediaListTabOrderRecyclerAdapter(context, mediaListTabs);


            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(dragDropManager.CreateWrappedAdapter(adapter));
            recyclerView.SetItemAnimator(new DraggableItemAnimator());

            dragDropManager.AttachRecyclerView(recyclerView);

            //var listView = view.FindViewById<Com.Woxthebox.Draglistview.DragListView>(Resource.Id.OrderedList_ListView);
            //listView.SetLayoutManager(new LinearLayoutManager(this));

            //var items = Globals.UserSettings.AnimeTabOrder.Select((x, i) => new OrderedSettingDragItemAdapter.OrderedSettingModel
            //{
            //    SettingName = x.TabName,
            //    SettingDescription = x.IsCustom ? "Custom list" : "",
            //    SettingActive = x.IsDisplayed,
            //    SettingValue = x.ListStatus,
            //    SettingObject = x,
            //    ItemId = i
            //}).ToList();

            //var adapter = new OrderedSettingDragItemAdapter(this, items) { DisplayCheckbox = true };

            //var typedValue = new TypedValue();
            //Theme.ResolveAttribute(Resource.Attribute.Background_Alternate, typedValue, true);
            //adapter.BackgroundColorResource = typedValue.ResourceId;
            //listView.SetAdapter(adapter, false);
            //listView.SetCanDragHorizontally(false);
            //listView.VerticalScrollBarEnabled = true;
            //listView.HorizontalScrollBarEnabled = false;

            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            dialog.SetView(view);
            dialog.SetCancelable(true);
            dialog.Show();

            //shownDialog.SetButton((int)DialogButtonType.Positive, "Save", (send, args) =>
            //{
            //    var settings = Globals.UserSettings;
            //    settings.AnimeTabOrder = new List<UserObjectListTab>();
            //    adapter.ItemList.ForEach(x =>
            //    {
            //        ((UserObjectListTab)x.SettingObject).IsDisplayed = x.SettingActive;
            //        settings.AnimeTabOrder.Add((UserObjectListTab)x.SettingObject);
            //    });
            //    Globals.UserSettings = settings;
            //    shownDialog.Dismiss();
            //});

            //shownDialog.Show();
        }
    }
}