using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.AniList.Models;
using AniDroid.Adapters.Base;
using AniDroid.AniListObject.User;
using AniDroid.Base;

namespace AniDroid.Dialogs
{
    public class AniListActivityRepliesDialog
    {
        public static void Create(BaseAniDroidActivity context, AniListActivity activity, int? currentUserId, Action<int, string> replyAction, Action<int> likeAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_AniListActivityReply, null);
            var recycler = view.FindViewById<RecyclerView>(Resource.Id.AniListActivityReply_Recycler);
            var likesContainer = view.FindViewById<LinearLayout>(Resource.Id.AniListActivityReply_LikesContainer);
            var adapter = new AniListActivityRepliesRecyclerAdapter(context, activity.Replies);

            PopulateLikesContainer(context, activity, likesContainer);

            recycler.SetAdapter(adapter);

            var alert = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            alert.SetView(view);

            var a = alert.Create();

            a.SetButton((int)DialogButtonType.Negative, "Close", (send, args) => a.Dismiss());

            if (currentUserId.HasValue)
            {
                var likeButtonText = activity.Likes?.Any(x => x.Id == currentUserId) == true ? "Unlike" : "Like";
                a.SetButton((int)DialogButtonType.Neutral, likeButtonText, (send, args) => likeAction(activity.Id));

                a.SetButton((int) DialogButtonType.Positive, "Submit",
                    (send, args) => replyAction(activity.Id,
                        view.FindViewById<EditText>(Resource.Id.AniListActivityReply_Reply).Text));
            }
            else
            {
                view.FindViewById<EditText>(Resource.Id.AniListActivityReply_Reply).Visibility = ViewStates.Gone;
            }

            a.Show();
        }

        private static void PopulateLikesContainer(BaseAniDroidActivity context, AniListActivity activity, ViewGroup container)
        {
            var imageSize = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 40, context.Resources.DisplayMetrics);
            var padding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 2, context.Resources.DisplayMetrics);
            var layoutParams = new ViewGroup.LayoutParams(imageSize, imageSize);

            var profileImages = activity.Likes.Select(x =>
            {
                var image = new ImageView(context) {LayoutParameters = layoutParams};
                image.SetPadding(padding, padding, padding, padding);
                context.LoadImage(image, x.Avatar.Large);
                image.Click += (lSend, lArgs) =>
                {
                    UserActivity.StartActivity(context, x.Id);
                };
                image.LongClick += (lSend, lArgs) =>
                {
                    Toast.MakeText(context, x.Name, ToastLength.Short).Show();
                };
                return image;
            }).ToList();

            container.Visibility = activity.Likes.Any() ? ViewStates.Visible : ViewStates.Gone;
            profileImages.ForEach(container.AddView);
        }
    }
}