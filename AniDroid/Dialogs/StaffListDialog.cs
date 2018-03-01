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
using AniDroid.Adapters.Base;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Dialogs
{
    public class StaffListDialog
    {
        public static void Create(BaseAniDroidActivity context, IEnumerable<Staff> staff)
        {
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
            dialogView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
            var dialogRecycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new StaffRecyclerAdapter(context, staff.ToList(), BaseRecyclerAdapter.CardType.FlatHorizontal);
            dialogRecycler.SetAdapter(recyclerAdapter);

            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            dialog.SetView(dialogView);
            dialog.Show();
        }
    }
}