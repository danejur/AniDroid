using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.CharacterAdapters;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.StaffAdapters;
using AniDroid.Adapters.StudioAdapters;
using AniDroid.Adapters.UserAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Interfaces;
using AniDroid.Base;
using AniDroid.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace AniDroid.Favorites
{
    [Activity(Label = "Favorites")]
    public class FavoritesActivity : BaseAniDroidActivity<FavoritesPresenter>, IFavoritesView
    {
        public const string UserIdIntentKey = "USER_ID";
        public const int PageLength = 25;

        [InjectView(Resource.Id.Favorites_ViewPager)]
        private ViewPager _viewPager;
        [InjectView(Resource.Id.Favorites_Toolbar)]
        private Toolbar _toolbar;
        [InjectView(Resource.Id.Favorites_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Favorites_Tabs)]
        private TabLayout _tabLayout;

        private int _userId;

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            _userId = Intent?.GetIntExtra(UserIdIntentKey, 0) ?? 0;

            await CreatePresenter(savedInstanceState);
        }

        public override void OnError(IAniListError error)
        {
        }

        public override void DisplaySnackbarMessage(string message, int length = Snackbar.LengthShort)
        {
        }

        public void SetupFavoritesView()
        {
            SetContentView(Resource.Layout.Activity_Favorites);

            _toolbar.Title = "Favorites";
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var adapter = new FragmentlessViewPagerAdapter();

            adapter.AddView(CreateFavoriteAnimeView(_userId), "Anime");
            adapter.AddView(CreateFavoriteMangaView(_userId), "Manga");
            adapter.AddView(CreateFavoriteCharactersView(_userId), "Characters");
            adapter.AddView(CreateFavoriteStaffView(_userId), "Staff");
            adapter.AddView(CreateFavoriteStudiosView(_userId), "Studios");

            _viewPager.Adapter = adapter;
            _tabLayout.SetupWithViewPager(_viewPager);
        }

        public static void StartActivity(BaseAniDroidActivity context, int userId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(FavoritesActivity));
            intent.PutExtra(UserIdIntentKey, userId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        private View CreateFavoriteAnimeView(int userId)
        {
            var favoriteAnimeEnumerable = Presenter.GetUserFavoriteAnimeEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new MediaEdgeRecyclerAdapter(this, favoriteAnimeEnumerable, CardType, MediaEdgeViewModel.CreateMediaEdgeViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateFavoriteMangaView(int userId)
        {
            var favoriteMangaEnumerable = Presenter.GetUserFavoriteMangaEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new MediaEdgeRecyclerAdapter(this, favoriteMangaEnumerable, CardType, MediaEdgeViewModel.CreateMediaEdgeViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateFavoriteCharactersView(int userId)
        {
            var favoriteCharactersEnumerable = Presenter.GetUserFavoriteCharactersEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new CharacterEdgeRecyclerAdapter(this, favoriteCharactersEnumerable, CardType, CharacterEdgeViewModel.CreateFavoriteCharacterEdgeViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateFavoriteStaffView(int userId)
        {
            var favoriteStaffEnumerable = Presenter.GetUserFavoriteStaffEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new StaffEdgeRecyclerAdapter(this, favoriteStaffEnumerable, CardType, StaffEdgeViewModel.CreateFavoriteStaffEdgeViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        private View CreateFavoriteStudiosView(int userId)
        {
            var favoriteStudiosEnumerable = Presenter.GetUserFavoriteStudiosEnumerable(userId, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var recyclerAdapter = new StudioEdgeRecyclerAdapter(this, favoriteStudiosEnumerable, BaseRecyclerAdapter.RecyclerCardType.Horizontal, StudioEdgeViewModel.CreateFavoriteStudioEdgeViewModel);
            recycler.SetAdapter(recyclerAdapter);

            return retView;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Ok);
                    Finish();
                    break;
            }

            return true;
        }
    }
}