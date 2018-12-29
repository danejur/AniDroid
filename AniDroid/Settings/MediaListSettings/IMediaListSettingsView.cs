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
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Base;
using AniDroid.Utils.Comparers;

namespace AniDroid.Settings.MediaListSettings
{
    public interface IMediaListSettingsView : IAniDroidView
    {
        void CreateGroupCompletedSettingItem(bool groupCompleted);
        void CreateMediaListViewTypeSettingItem(MediaListRecyclerAdapter.MediaListItemViewType viewType);
        void CreateHighlightPriorityMediaListItemsItem(bool highlightPriorityItems);
        void CreateAnimeListTabOrderItem(Func<List<KeyValuePair<string, bool>>> getAnimeLists);
        void CreateMangaListTabOrderItem(Func<List<KeyValuePair<string, bool>>> getMangaLists);
        void CreateAnimeListSortItem(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.MediaListSortDirection direction);
        void CreateMangaListSortItem(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.MediaListSortDirection direction);
        void CreateUseLongClickForEpisodeAddItem(bool useLongClickForEpisodeAdd);
        void CreateMediaListProgressDisplayItem(MediaListRecyclerAdapter.MediaListProgressDisplayType mediaListProgressDisplay);
    }
}