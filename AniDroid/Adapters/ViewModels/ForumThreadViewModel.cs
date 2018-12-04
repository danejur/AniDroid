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
using AniDroid.AniList.Models;

namespace AniDroid.Adapters.ViewModels
{
    public class ForumThreadViewModel : AniDroidAdapterViewModel<ForumThread>
    {
        public ForumThreadViewModel(ForumThread model, ForumThreadDetailType primaryForumThreadDetailType, ForumThreadDetailType secondaryForumThreadDetailType) : base(model)
        {
            TitleText = Model.Title;
            DetailPrimaryText = GetDetail(primaryForumThreadDetailType);
            DetailSecondaryText = GetDetail(secondaryForumThreadDetailType);
            ImageUri = Model.User?.Avatar?.Large ?? Model?.User?.Avatar?.Medium;
        }

        public enum ForumThreadDetailType
        {
            None,
            CreatedOn,
            RepliesLikes
        }

        public static ForumThreadViewModel CreateForumThreadViewModel(ForumThread model)
        {
            return new ForumThreadViewModel(model, ForumThreadDetailType.CreatedOn, ForumThreadDetailType.RepliesLikes);
        }

        private string GetDetail(ForumThreadDetailType detailType)
        {
            string retString = null;

            if (detailType == ForumThreadDetailType.CreatedOn)
            {
                retString = $"Created {Model.GetDateTimeOffset(Model.CreatedAt):MM/dd/yyyy HH:mm:ss}";
            }
            else if (detailType == ForumThreadDetailType.RepliesLikes)
            {
                retString = $"Replies: {Model.ReplyCount}\t\tLikes: {Model.Likes?.Count ?? 0}";
            }

            return retString;
        }
    }
}