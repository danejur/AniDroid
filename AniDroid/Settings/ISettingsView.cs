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
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Base;
using AniDroid.Utils.Comparers;

namespace AniDroid.Settings
{
    public interface ISettingsView : IAniDroidView
    {
        void CreateCardTypeSettingItem(BaseRecyclerAdapter.RecyclerCardType cardType);
        void CreateAniDroidThemeSettingItem(BaseAniDroidActivity.AniDroidTheme theme);
        void CreateDisplayBannersSettingItem(bool displayBanners);
        void CreateWhatsNewSettingItem();
        void CreatePrivacyPolicyLinkItem();
        
        // Auth settings
        void CreateGroupCompletedSettingItem(bool groupCompleted);
        void CreateMediaListViewTypeSettingItem(MediaListRecyclerAdapter.MediaListItemViewType viewType);
        void CreateHighlightPriorityMediaListItemsItem(bool highlightPriorityItems);
        void CreateDisplayProgressColorsItem(bool displayProgressColors);
        void CreateAnimeListTabOrderItem(Func<List<KeyValuePair<string, bool>>> getAnimeLists);
        void CreateMangaListTabOrderItem(Func<List<KeyValuePair<string, bool>>> getMangaLists);
        void CreateAnimeListSortItem(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.MediaListSortDirection direction);
        void CreateMangaListSortItem(MediaListSortComparer.MediaListSortType sort, MediaListSortComparer.MediaListSortDirection direction);
        void CreateUseLongClickForEpisodeAddItem(bool useLongClickForEpisodeAdd);
        void CreateEnableNotificationServiceItem(bool enableNotificationService);
    }
}