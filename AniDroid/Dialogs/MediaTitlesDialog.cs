using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.Base;

namespace AniDroid.Dialogs
{
    public class MediaTitlesDialog
    {
        public static void Create(BaseAniDroidActivity context, MediaTitle title, ICollection<string> alternateNames)
        {
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.Dialog_MediaTitles, null);

            if (!string.IsNullOrWhiteSpace(title.Romaji))
            {
                dialogView.FindViewById(Resource.Id.MediaTitles_RomajiContainer).Visibility = ViewStates.Visible;
                dialogView.FindViewById<TextView>(Resource.Id.MediaTitles_Romaji).Text = title.Romaji;
            }

            if (!string.IsNullOrWhiteSpace(title.English))
            {
                dialogView.FindViewById(Resource.Id.MediaTitles_EnglishContainer).Visibility = ViewStates.Visible;
                dialogView.FindViewById<TextView>(Resource.Id.MediaTitles_English).Text = title.English;
            }

            if (!string.IsNullOrWhiteSpace(title.Native))
            {
                dialogView.FindViewById(Resource.Id.MediaTitles_NativeContainer).Visibility = ViewStates.Visible;
                dialogView.FindViewById<TextView>(Resource.Id.MediaTitles_Native).Text = title.Native;
            }

            if (alternateNames?.Any() == true)
            {
                dialogView.FindViewById(Resource.Id.MediaTitles_AlsoKnownAsContainer).Visibility = ViewStates.Visible;
                dialogView.FindViewById<TextView>(Resource.Id.MediaTitles_AlsoKnownAs).Text = string.Join("\n", alternateNames);
            }

            var dialog = new AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            dialog.SetView(dialogView);
            dialog.Show();
        }
    }
}