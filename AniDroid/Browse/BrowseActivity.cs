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
using AniDroid.Utils;
using AniDroid.Utils.Interfaces;
using Newtonsoft.Json;
using Ninject;
using OneOf;

namespace AniDroid.Browse
{
    [Activity(Label = "Browse", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class BrowseActivity : BaseAniDroidActivity<BrowsePresenter>, IBrowseView
    {
        private const string BrowseDtoIntentKey = "BROWSE_DTO";

        private BaseRecyclerAdapter.RecyclerCardType _cardType;
        private Media.MediaSort _sortType;
        private MediaRecyclerAdapter _adapter;

        [InjectView(Resource.Id.Browse_CoordLayout)]
        private CoordinatorLayout _coordLayout;
        [InjectView(Resource.Id.Browse_RecyclerView)]
        private RecyclerView _recyclerView;
        [InjectView(Resource.Id.Browse_Toolbar)]
        private Toolbar _toolbar;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<IBrowseView, BrowseActivity>(this));


        public void ShowMediaSearchResults(IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> mediaEnumerable)
        {
            _adapter = new MediaRecyclerAdapter(this, mediaEnumerable, _cardType)
            {
                CreateViewModelFunc = model => new MediaViewModel(model, MediaViewModel.DetailType.Format,
                    MediaViewModel.DetailType.Genres)
            };
            _recyclerView.SetAdapter(_adapter);
        }

        public override void DisplaySnackbarMessage(string message, int length)
        {
            Snackbar.Make(_coordLayout, message, length).Show();
        }

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.Activity_Browse);
            var dto = new BrowseMediaDto();

            try
            {
                dto = AniListJsonSerializer.Default.Deserialize<BrowseMediaDto>(Intent.GetStringExtra(BrowseDtoIntentKey));
                _sortType = dto.Sort?.FirstOrDefault() ?? Media.MediaSort.Id;
            }
            catch
            {
                // ignored
            }

            var settings = Kernel.Get<IAniDroidSettings>();
            _cardType = settings.CardType;

            await CreatePresenter(savedInstanceState);
            Presenter.BrowseAniListMedia(dto);

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

        public override bool MenuItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                SetResult(Result.Ok);
                Finish();
                return true;
            }

            return false;
        }

        #endregion
    }
}