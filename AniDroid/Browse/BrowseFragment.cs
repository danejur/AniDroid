using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Ninject;
using OneOf;

namespace AniDroid.Browse
{
    public class BrowseFragment : BaseMainActivityFragment<BrowsePresenter>, IBrowseView
    {
        public override string FragmentName => "BROWSE_FRAGMENT";

        private MediaRecyclerAdapter _adapter;
        private BaseRecyclerAdapter.RecyclerCardType _cardType;
        private static BrowseFragment _instance;

        protected override IReadOnlyKernel Kernel => new StandardKernel(new ApplicationModule<IBrowseView, BrowseFragment>(this));

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            var recycler = View.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            recycler.SetAdapter(_adapter = new MediaRecyclerAdapter(Activity, mediaEnumerable, _cardType,
                MediaViewModel.CreateMediaViewModel)
            {
                LongClickAction = viewModel =>
                {
                    if (Presenter.GetIsUserAuthenticated())
                    {
                        EditMediaListItemDialog.Create(Activity, Presenter, viewModel.Model,
                            viewModel.Model.MediaListEntry,
                            Presenter.GetAuthenticatedUser()?.MediaListOptions);
                    }
                },

            });
        }

        public void UpdateMediaListItem(Media.MediaList mediaList)
        {
            if (mediaList.Media?.Type == Media.MediaType.Anime)
            {
                var instance = MediaListFragment.GetInstance(MediaListFragment.AnimeMediaListFragmentName);

                (instance as MediaListFragment)?.UpdateMediaListItem(mediaList);
            }
            else if (mediaList.Media?.Type == Media.MediaType.Manga)
            {
                (MediaListFragment.GetInstance(MediaListFragment.MangaMediaListFragmentName) as MediaListFragment)
                    ?.UpdateMediaListItem(mediaList);
            }

            var itemPosition =
                _adapter?.Items.FindIndex(x => x.Model?.Id == mediaList.Media?.Id);

            if (itemPosition == null || mediaList.Media == null)
            {
                return;
            }

            mediaList.Media.MediaListEntry = mediaList;

            _adapter.ReplaceItem(itemPosition.Value, _adapter.CreateViewModelFunc?.Invoke(mediaList.Media));
        }

        public void RemoveMediaListItem(int mediaListId)
        {
            (MediaListFragment.GetInstance(MediaListFragment.AnimeMediaListFragmentName) as MediaListFragment)
                ?.RemoveMediaListItem(mediaListId);
            (MediaListFragment.GetInstance(MediaListFragment.MangaMediaListFragmentName) as MediaListFragment)
                ?.RemoveMediaListItem(mediaListId);

            var itemPosition =
                _adapter?.Items.FindIndex(x => x.Model?.MediaListEntry?.Id == mediaListId);

            if (itemPosition == null)
            {
                return;
            }

            var item = _adapter.Items[itemPosition.Value];
            item.Model.MediaListEntry = null;

            _adapter.ReplaceItem(itemPosition.Value, _adapter.CreateViewModelFunc?.Invoke(item.Model));
        }

        protected override void SetInstance(BaseMainActivityFragment instance)
        {
            _instance = instance as BrowseFragment;
        }

        public override void ClearState()
        {
            _instance = null;
        }

        public override View CreateMainActivityFragmentView(ViewGroup container, Bundle savedInstanceState)
        {
            CreatePresenter(savedInstanceState).GetAwaiter().GetResult();
            var settings = Kernel.Get<IAniDroidSettings>();
            _cardType = settings.CardType;

            return LayoutInflater.Inflate(Resource.Layout.View_List, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            Presenter.BrowseAniListMedia(new BrowseMediaDto());
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            _adapter.RefreshAdapter();
        }
    }
}