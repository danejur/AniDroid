using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AniDroid.Adapters.AniListActivityAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.AniListObject;
using AniDroid.AniListObject.User;
using AniDroid.Base;
using AniDroid.Utils;
using AniDroid.Utils.Listeners;
using Google.Android.Material.TextField;

namespace AniDroid.Dialogs
{
    public class AniListActivityRepliesDialog
    {
        public static void Create(BaseAniDroidActivity context, AniListActivity activity, int activityPosition, IAniListActivityPresenter activityPresenter, int? currentUserId, Action<int, string> replyAction, Action<int> likeAction)
        {
            var view = context.LayoutInflater.Inflate(Resource.Layout.Dialog_AniListActivityReply, null);
            var recycler = view.FindViewById<RecyclerView>(Resource.Id.AniListActivityReply_Recycler);
            var likesContainer = view.FindViewById<LinearLayout>(Resource.Id.AniListActivityReply_LikesContainer);
            var adapter = new AniListActivityRepliesRecyclerAdapter(context, activityPresenter,
                activity.Replies.Select(x => AniListActivityReplyViewModel.CreateViewModel(x,
                    new Color(context.GetThemedColor(Resource.Attribute.Secondary_Dark)), currentUserId)).ToList());

            var alert = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            alert.SetView(view);

            var a = alert.Create();

            adapter.LongClickAction = (viewModel, position) =>
            {
                if (currentUserId.HasValue && viewModel.Model?.User?.Id == currentUserId)
                {
                    CreateEditReply(context, viewModel.Model.Text,
                        async text =>
                        {
                            await activityPresenter.EditActivityReplyAsync(viewModel.Model, position, text);
                            viewModel.RecreateViewModel();
                            adapter.NotifyItemChanged(position);

                            a.Dismiss();
                            await activityPresenter.UpdateActivityAsync(activity, activityPosition);
                        },
                        async () =>
                        {
                            var resp = await activityPresenter.DeleteActivityReplyAsync(viewModel.Model, position);
                            if (resp)
                            {
                                adapter.RemoveItem(position);
                            }

                            a.Dismiss();
                            await activityPresenter.UpdateActivityAsync(activity, activityPosition);
                        });
                }
            };

            PopulateLikesContainer(context, activity, likesContainer);

            recycler.SetAdapter(adapter);

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
                ImageLoader.LoadImage(image, x.Avatar.Large);
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

        private static void CreateEditReply(BaseAniDroidActivity context, string oldText, Func<string, Task> saveAction, Func<Task> deleteAction)
        {
            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.Dialog_AniListActivityCreate, null);
            var replyText = dialogView.FindViewById<EditText>(Resource.Id.AniListActivityCreate_Text);
            var replyTextLayout = dialogView.FindViewById<TextInputLayout>(Resource.Id.AniListActivityCreate_TextLayout);

            replyTextLayout.Hint = "Reply Text";

            replyText.Text = oldText;

            dialog.SetView(dialogView);

            dialog.SetButton((int)DialogButtonType.Negative, "Cancel", (send, args) => dialog.Dismiss());
            dialog.SetButton((int)DialogButtonType.Positive, "Save", (send, args) => { });
            dialog.SetButton((int)DialogButtonType.Neutral, "Delete", (send, args) =>
            {
                var confirmationDialog = new AlertDialog.Builder(context,
                    context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                confirmationDialog.SetTitle("Delete Reply");
                confirmationDialog.SetMessage("Are you sure you wish to delete this reply?");

                confirmationDialog.SetButton((int)DialogButtonType.Negative, "Cancel",
                    (cSend, cArgs) => confirmationDialog.Dismiss());
                confirmationDialog.SetButton((int)DialogButtonType.Positive, "Delete",
                    (cSend, cArgs) =>
                    {
                        confirmationDialog.Dismiss();
                        dialog.Dismiss();
                        deleteAction?.Invoke();
                    });

                confirmationDialog.Show();
            });

            dialog.ShowEvent += (sender2, e2) =>
            {
                var createButton = dialog.GetButton((int)DialogButtonType.Positive);
                createButton.SetOnClickListener(new InterceptClickListener(async () =>
                {
                    if (string.IsNullOrWhiteSpace(replyText.Text))
                    {
                        Toast.MakeText(context, "Text can't be empty!", ToastLength.Short).Show();
                        return;
                    }

                    await saveAction(replyText.Text);
                    dialog.Dismiss();
                }));
            };

            dialog.Show();
        }
    }
}