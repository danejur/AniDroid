using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.Utils.Interfaces
{
    public interface IAniDroidSettings
    {
        int HighestVersionUsed { get; set; }
        BaseRecyclerAdapter.RecyclerCardType CardType { get; set; }
        BaseAniDroidActivity.AniDroidTheme Theme { get; set; }
        bool DisplayBanners { get; set; }

        // Authenticated Settings

        string UserAccessCode { get; set; }
        bool IsUserAuthenticated { get; }
        void ClearUserAuthentication();
        User LoggedInUser { get; set; }
        bool ShowAllAniListActivity { get; set; }
        List<KeyValuePair<string, bool>> AnimeListOrder { get; set; }
        List<KeyValuePair<string, bool>> MangaListOrder { get; set; }
        bool GroupCompletedLists { get; set; }
        MediaListRecyclerAdapter.MediaListItemViewType MediaViewType { get; set; }
        bool HighlightPriorityMediaListItems { get; set; }
        bool DisplayMediaListItemProgressColors { get; set; }
    }
}