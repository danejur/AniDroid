using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.Adapters;
using AniDroid.Adapters.CharacterAdapters;
using AniDroid.Adapters.MediaAdapters;
using AniDroid.Adapters.ViewModels;
using AniDroid.AniList.Service;
using AniDroid.Base;
using AniDroid.SearchResults;
using AniDroid.Utils;
using Ninject;

namespace AniDroid.AniListObject.Character
{
    [Activity(Label = "Character")]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataHost = "anilist.co", DataSchemes = new[] { "http", "https" }, DataPathPattern = "/character/.*", Label = "AniDroid")]
    public class CharacterActivity : BaseAniListObjectActivity<CharacterPresenter>, ICharacterView
    {
        public const string CharacterIdIntentKey = "CHARACTER_ID";

        private int _characterId;

        protected override IReadOnlyKernel Kernel =>
            new StandardKernel(new ApplicationModule<ICharacterView, CharacterActivity>(this));

        public override async Task OnCreateExtended(Bundle savedInstanceState)
        {
            if (Intent.Data != null)
            {
                var dataUrl = Intent.Data.ToString();
                var urlRegex = new Regex("anilist.co/character/[0-9]*/?");
                var match = urlRegex.Match(dataUrl);
                var characterIdString = match.ToString().Replace("anilist.co/character/", "").TrimEnd('/');
                SetStandaloneActivity();

                if (!int.TryParse(characterIdString, out _characterId))
                {
                    Toast.MakeText(this, "Couldn't read character ID from URL", ToastLength.Short).Show();
                    Finish();
                }
            }
            else
            {
                _characterId = Intent.GetIntExtra(CharacterIdIntentKey, 0);
            }

            Logger.Debug("CharacterActivity", $"Starting activity with characterId: {_characterId}");

            await CreatePresenter(savedInstanceState);
        }

        public static void StartActivity(BaseAniDroidActivity context, int characterId, int? requestCode = null)
        {
            var intent = new Intent(context, typeof(CharacterActivity));
            intent.PutExtra(CharacterIdIntentKey, characterId);

            if (requestCode.HasValue)
            {
                context.StartActivityForResult(intent, requestCode.Value);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        public int GetCharacterId()
        {
            return _characterId;
        }

        public void SetupCharacterView(AniList.Models.Character character)
        {
            var adapter = new FragmentlessViewPagerAdapter();
            adapter.AddView(CreateCharacterDetailsView(character), "Details");

            if (character.Anime?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateCharacterMediaView(character.Id, AniList.Models.Media.MediaType.Anime), "Anime");
            }

            if (character.Manga?.PageInfo?.Total > 0)
            {
                adapter.AddView(CreateCharacterMediaView(character.Id, AniList.Models.Media.MediaType.Manga), "Manga");
            }

            ViewPager.OffscreenPageLimit = adapter.Count - 1;
            ViewPager.Adapter = adapter;

            TabLayout.SetupWithViewPager(ViewPager);
        }

        protected override Func<Task> ToggleFavorite => () => Presenter.ToggleFavorite();

        private View CreateCharacterDetailsView(AniList.Models.Character character)
        {
            var retView = LayoutInflater.Inflate(Resource.Layout.View_CharacterDetails, null);
            var imageView = retView.FindViewById<ImageView>(Resource.Id.Character_Image);
            var descriptionView = retView.FindViewById<TextView>(Resource.Id.Character_Description);
            var nameView = retView.FindViewById<TextView>(Resource.Id.Character_Name);
            var altNamesView = retView.FindViewById<TextView>(Resource.Id.Character_AltNames);

            LoadImage(imageView, character.Image?.Large);
            descriptionView.TextFormatted = FromHtml(character.Description ?? "(No Description Available)");
            nameView.Text = character.Name?.GetFormattedName(true);

            var altNames = character.Name?.Alternative?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            altNamesView.Text = altNames?.Any() == true
                ? $"Also known as: {string.Join(", ", altNames)}"
                : "";

            return retView;
        }

        private View CreateCharacterMediaView(int characterId, AniList.Models.Media.MediaType mediaType)
        {
            var characterAnimeEnumerable = Presenter.GetCharacterMediaEnumerable(characterId, mediaType, PageLength);
            var retView = LayoutInflater.Inflate(Resource.Layout.View_List, null);
            var recycler = retView.FindViewById<RecyclerView>(Resource.Id.List_RecyclerView);
            var dialogRecyclerAdapter = new MediaEdgeRecyclerAdapter(this, characterAnimeEnumerable, CardType, MediaEdgeViewModel.CreateCharacterMediaViewModel);
            recycler.SetAdapter(dialogRecyclerAdapter);

            return retView;
        }
    }
}