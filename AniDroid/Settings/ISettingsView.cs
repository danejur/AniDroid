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

namespace AniDroid.Settings
{
    public interface ISettingsView : IAniDroidView
    {
        void CreateCardTypeSettingItem(BaseRecyclerAdapter.RecyclerCardType cardType);
        void CreateAniDroidThemeSettingItem(BaseAniDroidActivity.AniDroidTheme theme);
        void CreateDisplayBannersSettingItem(bool displayBanners);
        void CreateWhatsNewSettingItem();
        
        // Auth settings
        void CreateGroupCompletedSettingItem(bool groupCompleted);
        void CreateMediaListViewTypeSettingItem(MediaListRecyclerAdapter.MediaListItemViewType viewType);
        void CreateHighlightPriorityMediaListItemsItem(bool highlightPriorityItems);
        void CreateDisplayProgressColorsItem(bool displayProgressColors);
        void CreateAnimeListTabOrderItem(List<KeyValuePair<string, bool>> animeLists);
        void CreateMangaListTabOrderItem(List<KeyValuePair<string, bool>> mangaLists);
    }
}