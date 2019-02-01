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
using Android.Text;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Adapters.ViewModels
{
    public class AniListActivityReplyViewModel : AniDroidAdapterViewModel<AniListActivity.ActivityReply>
    {
        public ISpanned DetailFormatted { get; protected set; }
        public string TimestampText { get; protected set; }
        public string LikeCount { get; protected set; }
        public ColorStateList LikeIconColor { get; protected set; }

        public AniListActivityReplyViewModel(AniListActivity.ActivityReply model, Color defaultIconColor, int? userId) : base(model)
        {
            TitleText = model.User?.Name;
            DetailFormatted = BaseAniDroidActivity.FromHtml(Model.Text);
            TimestampText = Model.GetAgeString(Model.CreatedAt);
            LikeCount = (Model.Likes?.Count ?? 0).ToString();
            ImageUri = Model.User.Avatar.Large ?? Model.User.Avatar.Medium;

            if (userId.HasValue)
            {
                LikeIconColor = ColorStateList.ValueOf(
                    Model.Likes?.Any(x => x.Id == userId) == true
                        ? Color.Crimson
                        : defaultIconColor);
            }
        }

        public static AniListActivityReplyViewModel CreateViewModel(AniListActivity.ActivityReply model, Color defaultIconColor, int? userId)
        {
            return new AniListActivityReplyViewModel(model, defaultIconColor, userId);
        }
    }
}