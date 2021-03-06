﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils.Comparers;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.Settings.MediaListSettings
{
    public class MediaListSettingsPresenter : BaseAniDroidPresenter<IMediaListSettingsView>
    {
        public MediaListSettingsPresenter(IAniListService service, IAniDroidSettings settings, IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override Task Init()
        {
            if (AniDroidSettings.AnimeListOrder?.Any() == true)
            {
                View.CreateAnimeListTabOrderItem(() => AniDroidSettings.AnimeListOrder);
            }

            if (AniDroidSettings.MangaListOrder?.Any() == true)
            {
                View.CreateMangaListTabOrderItem(() => AniDroidSettings.MangaListOrder);
            }

            View.CreateGroupCompletedSettingItem(AniDroidSettings.GroupCompletedLists);
            View.CreateMediaListViewTypeSettingItem(AniDroidSettings.MediaViewType);
            View.CreateMediaListProgressDisplayItem(AniDroidSettings.MediaListProgressDisplay);
            View.CreateAnimeListSortItem(AniDroidSettings.AnimeListSortType,
                AniDroidSettings.AnimeListSortDirection);
            View.CreateMangaListSortItem(AniDroidSettings.MangaListSortType,
                AniDroidSettings.MangaListSortDirection);
            View.CreateHighlightPriorityMediaListItemsItem(AniDroidSettings.HighlightPriorityMediaListItems);
            View.CreateUseLongClickForEpisodeAddItem(AniDroidSettings.UseLongClickForEpisodeAdd);
            View.CreateUseSwipeToRefreshOnMediaListsItem(AniDroidSettings.UseSwipeToRefreshOnMediaLists);
            View.CreateShowEpisodeAddButtonForRepeatingMediaItem(AniDroidSettings
                .ShowEpisodeAddButtonForRepeatingMedia);
            View.CreateAutoFillDateForMediaListItem(AniDroidSettings.AutoFillDateForMediaListItem);

            return Task.CompletedTask;
        }

        public void SetGroupCompleted(bool groupCompleted)
        {
            AniDroidSettings.GroupCompletedLists = groupCompleted;
        }

        public void SetMediaListViewType(MediaListRecyclerAdapter.MediaListItemViewType viewType)
        {
            AniDroidSettings.MediaViewType = viewType;
        }

        public void SetHighlightPriorityMediaListItems(bool highlightListItems)
        {
            AniDroidSettings.HighlightPriorityMediaListItems = highlightListItems;
        }

        public void SetAnimeListTabOrder(List<KeyValuePair<string, bool>> animeLists)
        {
            AniDroidSettings.AnimeListOrder = animeLists;
        }

        public void SetMangaListTabOrder(List<KeyValuePair<string, bool>> mangaLists)
        {
            AniDroidSettings.MangaListOrder = mangaLists;
        }

        public void SetAnimeListSort(MediaListSortComparer.MediaListSortType sort,
            MediaListSortComparer.SortDirection direction)
        {
            AniDroidSettings.AnimeListSortType = sort;
            AniDroidSettings.AnimeListSortDirection = direction;
        }

        public void SetMangaListSort(MediaListSortComparer.MediaListSortType sort,
            MediaListSortComparer.SortDirection direction)
        {
            AniDroidSettings.MangaListSortType = sort;
            AniDroidSettings.MangaListSortDirection = direction;
        }

        public void SetUseLongClickForEpisodeAdd(bool useLongClickForEpisodeAdd)
        {
            AniDroidSettings.UseLongClickForEpisodeAdd = useLongClickForEpisodeAdd;
        }

        public void SetMediaListProgressDisplay(
            MediaListRecyclerAdapter.MediaListProgressDisplayType mediaListProgressDisplay)
        {
            AniDroidSettings.MediaListProgressDisplay = mediaListProgressDisplay;
        }

        public void SetUseSwipeToRefreshOnMediaLists(bool useSwipeToRefreshOnMediaLists)
        {
            AniDroidSettings.UseSwipeToRefreshOnMediaLists = useSwipeToRefreshOnMediaLists;
        }

        public void SetShowEpisodeAddButtonForRepeatingMedia(bool showEpisodeAddButtonForRewatchingAnime)
        {
            AniDroidSettings.ShowEpisodeAddButtonForRepeatingMedia = showEpisodeAddButtonForRewatchingAnime;
        }
        
        public void SetAutoFillDateForMediaListItem(bool autoFillDateForNewMediaListItem)
        {
            AniDroidSettings.AutoFillDateForMediaListItem = autoFillDateForNewMediaListItem;
        }
    }
}