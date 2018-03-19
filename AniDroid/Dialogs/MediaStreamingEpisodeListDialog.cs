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
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Dialogs
{
    public class MediaStreamingEpisodeListDialog
    {
        public static void Create(BaseAniDroidActivity context, List<Media.MediaStreamingEpisode> streamingEpisodes)
        {
            var dialogView = context.LayoutInflater.Inflate(Resource.Layout.View_List, null);
            dialogView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
            var dialogRecycler = dialogView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new MediaStreamingEpisodesRecyclerAdapter(context, streamingEpisodes);
            dialogRecycler.SetAdapter(recyclerAdapter);

            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context, context.GetThemedResourceId(Resource.Attribute.Dialog_Theme));
            dialog.SetView(dialogView);
            dialog.Show();
        }
    }
}