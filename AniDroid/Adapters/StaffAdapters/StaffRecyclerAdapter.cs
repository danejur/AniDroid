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
using AniDroid.AniList;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;

namespace AniDroid.Adapters.StaffAdapters
{
    public class StaffRecyclerAdapter : BaseRecyclerAdapter<Staff>
    {
        public StaffRecyclerAdapter(BaseAniDroidActivity context, List<Staff> items, CardType cardType) : base(context, items, cardType)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Name?.GetFormattedName(true);
            holder.DetailPrimary.Text = item.Language?.DisplayValue ?? "(Language unknown)";
            Context.LoadImage(holder.Image, item.Image?.Large);

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.Button.Visibility = ViewStates.Gone;
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var staffPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var staff = Items[staffPos];

            StaffActivity.StartActivity(Context, staff.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}