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
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniList.Models.StudioModels;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;
using OneOf;

namespace AniDroid.Favorites
{
    public class FavoritesPresenter : BaseAniDroidPresenter<IFavoritesView>
    {
        public FavoritesPresenter(IAniListService service, IAniDroidSettings settings, IAniDroidLogger logger) : base(service, settings, logger)
        {
        }

        public override async Task Init()
        {
            View.SetupFavoritesView();
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetUserFavoriteAnimeEnumerable(int userId, int count)
        {
            return AniListService.GetUserFavoriteAnime(userId, 25);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetUserFavoriteMangaEnumerable(int userId, int count)
        {
            return AniListService.GetUserFavoriteManga(userId, 25);
        }

        public IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetUserFavoriteCharactersEnumerable(int userId, int count)
        {
            return AniListService.GetUserFavoriteCharacters(userId, 25);
        }

        public IAsyncEnumerable<OneOf<IPagedData<StaffEdge>, IAniListError>> GetUserFavoriteStaffEnumerable(int userId, int count)
        {
            return AniListService.GetUserFavoriteStaff(userId, 25);
        }

        public IAsyncEnumerable<OneOf<IPagedData<StudioEdge>, IAniListError>> GetUserFavoriteStudiosEnumerable(int userId, int count)
        {
            return AniListService.GetUserFavoriteStudios(userId, 25);
        }
    }
}