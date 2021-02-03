using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Widget;
using AndroidX.AppCompat.App;
using AniDroid.Base;
using AniDroid.Utils.Listeners;

namespace AniDroid.Dialogs
{
    public static class AniListActivityCreateDialog
    {
        public static void CreateNewActivity(BaseAniDroidActivity context, Func<string, Task> saveAction)
        {
            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.Dialog_AniListActivityCreate, null);
            dialog.SetView(dialogView);

            dialog.SetButton((int)DialogButtonType.Negative, "Cancel", (send, args) => dialog.Dismiss());
            dialog.SetButton((int)DialogButtonType.Positive, "Post", (send, args) => { });

            dialog.ShowEvent += (sender2, e2) =>
            {
                var activityText = dialogView.FindViewById<EditText>(Resource.Id.AniListActivityCreate_Text);
                var createButton = dialog.GetButton((int)DialogButtonType.Positive);
                createButton.SetOnClickListener(new InterceptClickListener(async () =>
                {
                    if (string.IsNullOrWhiteSpace(activityText.Text))
                    {
                        Toast.MakeText(context, "Text can't be empty!", ToastLength.Short).Show();
                        return;
                    }

                    await saveAction(activityText.Text);
                    dialog.Dismiss();
                }));
            };

            dialog.Show();
        }

        public static void CreateEditActivity(BaseAniDroidActivity context, string oldText, Func<string, Task> saveAction, Func<Task> deleteAction)
        {
            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.Dialog_AniListActivityCreate, null);
            var activityText = dialogView.FindViewById<EditText>(Resource.Id.AniListActivityCreate_Text);

            activityText.Text = oldText;

            dialog.SetView(dialogView);

            dialog.SetButton((int)DialogButtonType.Negative, "Cancel", (send, args) => dialog.Dismiss());
            dialog.SetButton((int)DialogButtonType.Positive, "Save", (send, args) => { });
            dialog.SetButton((int) DialogButtonType.Neutral, "Delete", (send, args) =>
            {
                var confirmationDialog = new AlertDialog.Builder(context,
                    context.GetThemedResourceId(Resource.Attribute.Dialog_Theme)).Create();
                confirmationDialog.SetTitle("Delete Activity");
                confirmationDialog.SetMessage("Are you sure you wish to delete this activity?");

                confirmationDialog.SetButton((int) DialogButtonType.Negative, "Cancel",
                    (cSend, cArgs) => confirmationDialog.Dismiss());
                confirmationDialog.SetButton((int) DialogButtonType.Positive, "Delete",
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
                    if (string.IsNullOrWhiteSpace(activityText.Text))
                    {
                        Toast.MakeText(context, "Text can't be empty!", ToastLength.Short).Show();
                        return;
                    }

                    await saveAction(activityText.Text);
                    dialog.Dismiss();
                }));
            };

            dialog.Show();
        }
    }
}