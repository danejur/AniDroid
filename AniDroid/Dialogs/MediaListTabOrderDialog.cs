using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Base;
using AniDroid.Utils.Extensions;

namespace AniDroid.Dialogs
{
    public class MediaListTabOrderDialog
    {
        public static void Create(BaseAniDroidActivity context, List<KeyValuePair<string, bool>> mediaListTabs, Action<List<KeyValuePair<string, bool>>> onDismissAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var adapter = new MediaListTabOrderRecyclerAdapter(context,
                mediaListTabs.Select(x => new BaseRecyclerAdapter.StableIdItem<KeyValuePair<string, bool>>(x))
                    .ToList());

            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(context));

            //var helper = recyclerView.AddDragAndDropSupport();

            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            dialog.SetView(view);
            dialog.SetCancelable(true);
            dialog.Show();

            dialog.DismissEvent += (sender, e) => { onDismissAction.Invoke(adapter.Items.Select(x => x.Item).ToList()); };
        }

    }
}