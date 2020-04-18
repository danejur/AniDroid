using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using AniDroid.Adapters.Base;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniList.Utils;
using AniDroid.Base;
using AniDroid.Dialogs;
using AniDroid.MediaList;
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Newtonsoft.Json;
using OneOf;

namespace AniDroid.Browse
{
    [Activity(Label = "Browse", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class BrowseActivity : BaseAniDroidActivity<BrowsePresenter>, IBrowseView
    {
        private const string BrowseDtoIntentKey = "BROWSE_DTO";

        private BaseRecyclerAdapter.RecyclerCardType _cardType;
        private MediaRecyclerAdapter _adapter;

        [InjectView(Resource.Id.Browse_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Browse_RecyclerView)]
        private RecyclerView _recyclerView;
        [InjectView(Resource.Id.Browse_Toolbar)]
        private Toolbar _toolbar;

        public void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            _recyclerView.SetAdapter(_adapter = new MediaRecyclerAdapter(this, mediaEnumerable, _cardType,
                MediaViewModel.CreateMediaViewModel)
            {
                LongClickAction = (viewModel, position) =>
                {
                    if (Presenter.GetIsUserAuthenticated())
                    {
                        EditMediaListItemDialog.Create(this, Presenter, viewModel.Model,
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

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Browse);
            var browseModel = new BrowseMediaDto();

            try
            {
                browseModel = AniListJsonSerializer.Default.Deserialize<BrowseMediaDto>(Intent.GetStringExtra(BrowseDtoIntentKey)) ?? new BrowseMediaDto();
                browseModel.Sort ??= new List<Media.MediaSort>();

                if (!browseModel.Sort.Any())
                {
                    browseModel.Sort.Add(Media.MediaSort.PopularityDesc);
                }
            }
            catch
            {
                // ignored
            }
            
            await CreatePresenter(savedInstanceState);

            _cardType = Presenter.AniDroidSettings.CardType;

            Presenter.BrowseAniListMedia(browseModel);

            SetupToolbar();
        }

        public override void OnError(IAniListError error)
        {
            throw new NotImplementedException();
        }

        public static void StartActivity(BaseAniDroidActivity context, BrowseMediaDto browseDto, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(BrowseActivity));
            var dtoString = AniListJsonSerializer.Default.Serialize(browseDto);
            intent.PutExtra(BrowseDtoIntentKey, dtoString);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == ObjectBrowseRequestCode && resultCode == Result.Ok)
            {
                SetResult(Result.Ok);
                Finish();
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            _adapter.RefreshAdapter();
        }

        #region Toolbar

        private void SetupToolbar()
        {
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_arrow_back_white_24px);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool SetupMenu(IMenu menu)
        {
            menu?.Clear();
            MenuInflater.Inflate(Resource.Menu.Browse_ActionBar, menu);

            return true;
        }

        public override bool MenuItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Ok);
                    Finish();
                    break;
                case Resource.Id.Menu_Browse_Filter:
                    BrowseFilterDialog.Create(this, Presenter);
                    break;
                case Resource.Id.Menu_Browse_Sort:
                    BrowseSortDialog.Create(this, Presenter.GetBrowseDto().Sort.FirstOrDefault(), sort =>
                    {
                        var browseDto = Presenter.GetBrowseDto();
                        browseDto.Sort = new List<Media.MediaSort> { sort };
                        Presenter.BrowseAniListMedia(browseDto);
                    });
                    break;
            }

            return true;
        }

        #endregion
    }
}