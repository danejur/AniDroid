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
        public MediaListSettingsPresenter(IMediaListSettingsView view, IAniListService service, IAniDroidSettings settings, IAniDroidLogger logger) : base(view, service, settings, logger)
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
            View.CreateAnimeListSortItem(AniDroidSettings.AnimeListSortType,
                AniDroidSettings.AnimeListSortDirection);
            View.CreateMangaListSortItem(AniDroidSettings.MangaListSortType,
                AniDroidSettings.MangaListSortDirection);
            View.CreateHighlightPriorityMediaListItemsItem(AniDroidSettings.HighlightPriorityMediaListItems);
            View.CreateDisplayProgressColorsItem(AniDroidSettings.DisplayMediaListItemProgressColors);
            View.CreateUseLongClickForEpisodeAddItem(AniDroidSettings.UseLongClickForEpisodeAdd);
            View.CreateAlwaysDisplayEpisodeProgressColorItem(AniDroidSettings.AlwaysDisplayEpisodeProgressColor);

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

        public void SetDisplayProgressColorsItem(bool displayProgressColors)
        {
            AniDroidSettings.DisplayMediaListItemProgressColors = displayProgressColors;
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
            MediaListSortComparer.MediaListSortDirection direction)
        {
            AniDroidSettings.AnimeListSortType = sort;
            AniDroidSettings.AnimeListSortDirection = direction;
        }

        public void SetMangaListSort(MediaListSortComparer.MediaListSortType sort,
            MediaListSortComparer.MediaListSortDirection direction)
        {
            AniDroidSettings.MangaListSortType = sort;
            AniDroidSettings.MangaListSortDirection = direction;
        }

        public void SetUseLongClickForEpisodeAdd(bool useLongClickForEpisodeAdd)
        {
            AniDroidSettings.UseLongClickForEpisodeAdd = useLongClickForEpisodeAdd;
        }

        public void SetAlwaysDisplayEpisodeProgressColor(bool alwaysDisplayEpisodeProgressColor)
        {
            AniDroidSettings.AlwaysDisplayEpisodeProgressColor = alwaysDisplayEpisodeProgressColor;
        }
    }
}