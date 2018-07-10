using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.AniList;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Ninject;

namespace AniDroid.MediaList
{
    public class MediaListFragment : BaseMainActivityFragment<MediaListPresenter>, IMediaListView
    {
        public const string AnimeMediaListFragmentName = "ANIME_MEDIA_LIST_FRAGMENT";
        public const string MangaMediaListFragmentName = "MANGA_MEDIA_LIST_FRAGMENT";
        private const string MediaTypeKey = "MEDIA_TYPE";

        private Media.MediaType _type;
        private IList<MediaListRecyclerAdapter> _recyclerAdapters;
        private Media.MediaListCollection _collection;

        private static MediaListFragment _animeListFragmentInstance;
        private static MediaListFragment _mangaListFragmentInstance;

        public override bool HasMenu => true;
        public override string FragmentName {
            get {
                if (_type == Media.MediaType.Anime)
                {
                    return AnimeMediaListFragmentName;
                }

                return _type == Media.MediaType.Manga ? MangaMediaListFragmentName : "";
            }
        }

        public static BaseMainActivityFragment<MediaListPresenter> GetInstance(string fragmentName)
        {
            switch (fragmentName)
            {
                case AnimeMediaListFragmentName:
                    return _animeListFragmentInstance;
                case MangaMediaListFragmentName:
                    return _mangaListFragmentInstance;
                default:
                    return null;
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var typeString = Arguments.GetString(MediaTypeKey);
            _type = AniListEnum.GetEnum<Media.MediaType>(typeString);

            if (_type == Media.MediaType.Anime)
            {
                _animeListFragmentInstance = this;
            }
            else if (_type == Media.MediaType.Manga)
            {
                _mangaListFragmentInstance = this;
            }
        }

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IMediaListView, MediaListFragment>(this));

        public static MediaListFragment CreateMediaListFragment(Media.MediaType type)
        {
            var frag = new MediaListFragment();
            var bundle = new Bundle(6);
            bundle.PutString(MediaTypeKey, type.Value);
            frag.Arguments = bundle;

            return frag;
        }

        public override void OnError(IAniListError error)
        {
            // TODO: show error fragment here
        }

        protected override void SetInstance(BaseMainActivityFragment instance)
        {
        }

        public override void ClearState()
        {
            if (_type == Media.MediaType.Anime)
            {
                _animeListFragmentInstance = null;
            }
            else if (_type == Media.MediaType.Manga)
            {
                _mangaListFragmentInstance = null;
            }
        }

        public override View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState)
        {
            if (_collection != null)
            {
                return GetMediaListCollectionView();
            }
            
            if (_type == null)
            {
                return LayoutInflater.Inflate(Resource.Layout.View_Error, container, false);
            }

            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();
            Presenter.GetMediaLists();

            return LayoutInflater.Inflate(Resource.Layout.View_IndeterminateProgressIndicator, container, false);
        }

        public Media.MediaType GetMediaType()
        {
            return _type;
        }

        public void SetCollection(Media.MediaListCollection collection)
        {
            _collection = collection;
            RecreateFragment();
        }

        public void UpdateMediaListItem(Media.MediaList mediaList)
        {
            foreach (var adapter in _recyclerAdapters)
            {
                adapter.UpdateMediaListItem(mediaList.Media.Id, mediaList);
            }
        }

        public void ResetMediaListItem(int mediaId)
        {
            foreach (var adapter in _recyclerAdapters)
            {
                adapter.ResetMediaListItem(mediaId);
            }
        }

        public override void SetupMenu(IMenu menu)
        {
            menu.Clear();
            var inflater = new MenuInflater(Context);
            inflater.Inflate(Resource.Menu.MediaLists_ActionBar, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Menu_MediaLists_Refresh:
                    _collection = null;
                    RecreateFragment();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private View GetMediaListCollectionView()
        {
            var mediaCollectionView = LayoutInflater.Inflate(Resource.Layout.Fragment_MediaLists, null);
            var pagerAdapter = new FragmentlessViewPagerAdapter();
            _recyclerAdapters = new List<MediaListRecyclerAdapter>();

            var listOrder = GetListOrder();
            var orderedLists = _collection.Lists.Where(x => listOrder.All(y => y.Key != x.Name) || listOrder.FirstOrDefault(y => y.Key == x.Name).Value)
                    .OrderBy(x => listOrder.FindIndex(y=> y.Key == x.Name)).ToList();

            foreach (var statusList in orderedLists)
            {
                if (statusList.Entries?.Any() != true)
                {
                    continue;
                }

                var adapter = new MediaListRecyclerAdapter(Activity, statusList,
                    _collection.User.MediaListOptions, Presenter, Presenter.GetCardType(),
                    Presenter.GetMediaListItemViewType(), Presenter.GetHighlightPriorityItems(),
                    Presenter.GetDisplayProgressColors());
                _recyclerAdapters.Add(adapter);
                var listView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
                listView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView).SetAdapter(adapter);
                pagerAdapter.AddView(listView, statusList.Name);
            }

            var viewPagerView = mediaCollectionView.FindViewById<ViewPager>(Resource.Id.MediaLists_ViewPager);
            viewPagerView.Adapter = pagerAdapter;
            mediaCollectionView.FindViewById<TabLayout>(Resource.Id.MediaLists_Tabs).SetupWithViewPager(viewPagerView);

            return mediaCollectionView;
        }

        private List<KeyValuePair<string, bool>> GetListOrder()
        {
            var settings = Kernel.Get<IAniDroidSettings>();
            var retList = new List<KeyValuePair<string, bool>>();

            if (_type == Media.MediaType.Anime)
            {
                var lists = _collection.User.MediaListOptions?.AnimeList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.AnimeList.CustomLists ?? new List<string>()) ?? new List<string>();

                if (settings.AnimeListOrder?.Any() != true)
                {
                    // if we don't have the list order yet, go ahead and store it
                    settings.AnimeListOrder = lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
                }

                retList = settings.AnimeListOrder;
            }
            else if (_type == Media.MediaType.Manga)
            {
                var lists = _collection.User.MediaListOptions?.MangaList?.SectionOrder?
                                .Union(_collection.User.MediaListOptions.MangaList.CustomLists ?? new List<string>()) ?? new List<string>();

                if (settings.MangaListOrder?.Any() != true)
                {
                    settings.MangaListOrder = lists.Select(x => new KeyValuePair<string, bool>(x, true)).ToList();
                }

                retList = settings.AnimeListOrder;
            }

            return retList;
        }
    }
}