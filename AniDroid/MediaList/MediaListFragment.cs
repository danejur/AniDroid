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
        private View _view;
        private Dictionary<string, BaseRecyclerAdapter<Media>> _recyclerAdapters;
        private Media.MediaListCollection _collection;

        private static BaseMainActivityFragment<MediaListPresenter> _animeListFragmentInstance;
        private static BaseMainActivityFragment<MediaListPresenter> _mangaListFragmentInstance;

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

        public new static BaseMainActivityFragment<MediaListPresenter> GetInstance(string fragmentName)
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

        public override void OnDetach()
        {
            base.OnDetach();

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

        public override View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState)
        {
            if (_collection != null)
            {
                return GetMediaListCollectionView();
            }

            var typeString = Arguments.GetString(MediaTypeKey);
            if (string.IsNullOrWhiteSpace(typeString))
            {
                return LayoutInflater.Inflate(Resource.Layout.View_Error, container, false);
            }

            _type = AniListEnum.GetEnum<Media.MediaType>(typeString);
            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();
            Presenter.GetMediaLists();

            return _view = LayoutInflater.Inflate(Resource.Layout.View_IndeterminateProgressIndicator, container, false);
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
            // TODO: update media list item
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
            _recyclerAdapters = new Dictionary<string, BaseRecyclerAdapter<Media>>();

            var listOrder = GetListOrder();
            var orderedLists = !listOrder.Any()
                ? _collection.Lists
                : _collection.Lists.Where(x => listOrder.FirstOrDefault(y => y.Key == x.Name).Value).OrderBy(x => listOrder.FindIndex(y=> y.Key == x.Name)).ToList();

            foreach (var statusList in orderedLists)
            {
                if (statusList.Entries?.Any() != true)
                {
                    continue;
                }

                var adapter = new MediaListRecyclerAdapter(Activity, statusList.Entries,
                    _collection.User.MediaListOptions, Presenter, Kernel.Get<IAniDroidSettings>().CardType);
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
                retList = settings.AnimeListOrder ?? _collection.User.MediaListOptions?.AnimeList?.SectionOrder
                              ?.Select(x => new KeyValuePair<string, bool>(x, true)).ToList() ??
                          new List<KeyValuePair<string, bool>>();
            }
            else if (_type == Media.MediaType.Manga)
            {
                retList = settings.MangaListOrder ?? _collection.User.MediaListOptions?.MangaList?.SectionOrder
                              ?.Select(x => new KeyValuePair<string, bool>(x, true)).ToList() ??
                          new List<KeyValuePair<string, bool>>();
            }

            return retList;
        }
    }
}