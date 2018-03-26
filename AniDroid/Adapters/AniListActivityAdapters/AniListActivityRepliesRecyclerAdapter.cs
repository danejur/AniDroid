using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.User;
using AniDroid.Base;

namespace AniDroid.Adapters.AniListActivityAdapters
{
    public class AniListActivityRepliesRecyclerAdapter : BaseRecyclerAdapter<AniListActivity.ActivityReply>
    {
        private readonly Color _userNameColor;

        public AniListActivityRepliesRecyclerAdapter(BaseAniDroidActivity context, List<AniListActivity.ActivityReply> items) : base(context, items, RecyclerCardType.FlatHorizontal)
        {
            _userNameColor = new Color(Context.GetThemedColor(Resource.Attribute.Primary));
        }

        public override void BindCardViewHolder(CardItem holder, int position)
        {
            var item = Items[position];

            holder.Name.Text = item.User.Name;
            holder.DetailPrimary.TextFormatted = BaseAniDroidActivity.FromHtml(item.Text);
            Context.LoadImage(holder.Image, item.User.Avatar.Large);

            holder.Image.SetTag(Resource.Id.Object_Position, position);
            holder.Image.Click -= ImageClick;
            holder.Image.Click += ImageClick;
        }

        public override CardItem SetupCardItemViewHolder(CardItem item)
        {
            item.Name.SetTextColor(_userNameColor);
            item.DetailSecondary.Visibility = ViewStates.Gone;
            item.Container.SetBackgroundColor(Color.Transparent);
            item.ContainerCard.CardBackgroundColor = ColorStateList.ValueOf(Color.Transparent);
            return item;
        }

        private void ImageClick(object sender, EventArgs e)
        {
            var senderView = sender as View;
            var userPos = (int)senderView.GetTag(Resource.Id.Object_Position);
            var user = Items[userPos];
            UserActivity.StartActivity(Context, user.Id);
        }
    }
}