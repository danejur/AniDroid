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
using AniDroid.Adapters.Base;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Staff;
using AniDroid.Base;

namespace AniDroid.Adapters.MediaAdapters
{
    public class MediaStaffRecyclerAdapter : LazyLoadingRecyclerViewAdapter<Staff.Edge>
    {
        public MediaStaffRecyclerAdapter(BaseAniDroidActivity context, IAsyncEnumerable<IPagedData<Staff.Edge>> enumerable, CardType cardType, int verticalCardColumns = 3) : base(context, enumerable, cardType, verticalCardColumns)
        {
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.Node?.Name?.GetFormattedName(true);
            holder.DetailPrimary.Text = AniListEnum.GetDisplayValue<Staff.StaffLanguage>(item.Node?.Language);
            holder.DetailSecondary.Text = item.Role;
            Context.LoadImage(holder.Image, item.Node?.Image?.Large ?? "");

            holder.ContainerCard.SetTag(Resource.Id.Object_Position, position);
            holder.ContainerCard.Click -= RowClick;
            holder.ContainerCard.Click += RowClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Button.Visibility = ViewStates.Gone;
            return item;
        }

        private void RowClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var staffPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var staffEdge = Items[staffPos];

            StaffActivity.StartActivity(Context, staffEdge.Node.Id, BaseAniDroidActivity.ObjectBrowseRequestCode);
        }
    }
}