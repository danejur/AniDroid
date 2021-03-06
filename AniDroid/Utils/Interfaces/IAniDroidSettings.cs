﻿using System;
using System.Collections.Generic;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.UserModels;
using AniDroid.Base;
using AniDroid.Main;
using AniDroid.Utils.Comparers;

namespace AniDroid.Utils.Interfaces
{
    public interface IAniDroidSettings
    {
        #region Unauthenticated Settings

        int HighestVersionUsed { get; set; }
        BaseRecyclerAdapter.RecyclerCardType CardType { get; set; }
        BaseAniDroidActivity.AniDroidTheme Theme { get; set; }
        bool DisplayBanners { get; set; }
        bool DisplayUpcomingEpisodeTimeAsCountdown { get; set; }
        bool UseSwipeToRefreshHomeScreen { get; set; }
        IList<MediaTag> MediaTagCache { get; set; }
        IList<string> GenreCache { get; set; }

        #endregion

        #region Authenticated Settings

        string UserAccessCode { get; set; }
        bool IsUserAuthenticated { get; }
        void ClearUserAuthentication();
        User LoggedInUser { get; set; }
        bool ShowAllAniListActivity { get; set; }
        bool EnableNotificationService { get; set; }
        MainActivity.DefaultTab DefaultTab { get; set; }

        #endregion

        #region Media List Settings

        List<KeyValuePair<string, bool>> AnimeListOrder { get; set; }
        List<KeyValuePair<string, bool>> MangaListOrder { get; set; }
        bool GroupCompletedLists { get; set; }
        MediaListRecyclerAdapter.MediaListItemViewType MediaViewType { get; set; }
        bool HighlightPriorityMediaListItems { get; set; }
        MediaListSortComparer.MediaListSortType AnimeListSortType { get; set; }
        MediaListSortComparer.SortDirection AnimeListSortDirection { get; set; }
        MediaListSortComparer.MediaListSortType MangaListSortType { get; set; }
        MediaListSortComparer.SortDirection MangaListSortDirection { get; set; }
        bool UseLongClickForEpisodeAdd { get; set; }
        MediaListRecyclerAdapter.MediaListProgressDisplayType MediaListProgressDisplay { get; set; }
        bool UseSwipeToRefreshOnMediaLists { get; set; }
        bool ShowEpisodeAddButtonForRepeatingMedia { get; set; }
        bool AutoFillDateForMediaListItem { get; set; }
        
        #endregion

        #region Old Settings

        [Obsolete("No longer used, use MediaListProgressDisplay instead", true)]
        bool DisplayMediaListItemProgressColors { get; set; }
        [Obsolete("No longer used, use MediaListProgressDisplay instead", true)]
        bool AlwaysDisplayEpisodeProgressColor { get; set; }

        #endregion

        #region Methods

        void UpdateLoggedInUser(User user);
        void UpdateUserMediaListTabs(UserMediaListOptions mediaListOptions);

        #endregion
        
    }
}