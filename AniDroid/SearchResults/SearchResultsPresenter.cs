using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.Base;

namespace AniDroid.SearchResults
{
    public class SearchResultsPresenter : BaseAniDroidPresenter<ISearchResultsView>
    {
        private const int PageSize = 20;

        public SearchResultsPresenter(ISearchResultsView view, IAniListService service) : base(view, service)
        {
        }

        public void SearchAniList(string searchType, string searchTerm)
        {
            switch (searchType)
            {
                //case SearchResultsActivity.AniListSearchTypes.Anime:
                //    View.ShowAnimeSearchResults(AniListService.SearchMedia(searchTerm, Media.MediaType.Anime,
                //        PageSize));
                //    break;
                //case SearchResultsActivity.AniListSearchTypes.Manga:
                //    resultRecycler.SetAdapter(new MediaSearchRecyclerAdapter(this,
                //        AniListService.SearchMedia(searchTerm, Media.MediaType.Manga, PageSize), cardType));
                //    break;
                case SearchResultsActivity.AniListSearchTypes.Characters:
                    View.ShowCharacterSearchResults(AniListService.SearchCharacters(searchTerm, PageSize));
                    break;
                    //case SearchResultsActivity.AniListSearchTypes.Staff:
                    //    resultRecycler.SetAdapter(new StaffSearchRecyclerAdapter(this,
                    //        AniListService.SearchStaff(searchTerm, PageSize), cardType));
                    //    break;
                    //case SearchResultsActivity.AniListSearchTypes.Studios:
                    //    resultRecycler.SetAdapter(new StudioSearchRecyclerAdapter(this,
                    //        AniListService.SearchStudios(searchTerm, PageSize)));
                    //    break;
                    //case SearchResultsActivity.AniListSearchTypes.Users:
                    //    resultRecycler.SetAdapter(new UserSearchRecyclerAdapter(this,
                    //        AniListService.SearchUsers(searchTerm, PageSize), cardType));
                    //    break;
                    //case SearchResultsActivity.AniListSearchTypes.Forum:
                    //    resultRecycler.SetAdapter(new ForumThreadSearchRecyclerAdapter(this,
                    //        AniListService.SearchForumThreads(searchTerm, PageSize)));
                    //    break;
            }

        }

        public async Task<Character> ToggleCharacterFavorite(Character character)
        {
            var favResp = await AniListService.ToggleFavorite(character.Id, User.FavoriteType.Character, default(CancellationToken));
            favResp.Switch(userFavs =>
            {
                character.IsFavourite = userFavs.Characters.Nodes.Any(x => x.Id == character.Id);
                View.DisplaySnackbarMessage("Favorite toggled successfully", Snackbar.LengthShort);
            }).Switch(error =>
            {
                View.DisplaySnackbarMessage("Error occurred while toggling favorite", Snackbar.LengthShort);
            });

            return character;
        }

        public override Task Init()
        {
            // TODO: determine if these are needed for this presenter
            return Task.CompletedTask;
        }

        public override Task RestoreState(IList<string> savedState)
        {
            return Task.CompletedTask;
        }

        public override IList<string> SaveState()
        {
            return new List<string>();
        }
    }
}