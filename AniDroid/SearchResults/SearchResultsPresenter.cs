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
using AniDroid.AniList.Dto;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Models;
using AniDroid.AniListObject.Media;
using AniDroid.Base;
using AniDroid.Utils.Interfaces;
using AniDroid.Utils.Logging;

namespace AniDroid.SearchResults
{
    public class SearchResultsPresenter : BaseAniDroidPresenter<ISearchResultsView>, IAniListMediaListEditPresenter
    {
        private const int PageSize = 20;

        public SearchResultsPresenter(ISearchResultsView view, IAniListService service, IAniDroidSettings settings,
            IAniDroidLogger logger) : base(view, service, settings, logger)
        {
        }

        public void SearchAniList(string searchType, string searchTerm)
        {
            switch (searchType)
            {
                case SearchResultsActivity.AniListSearchTypes.Anime:
                    View.ShowMediaSearchResults(AniListService.SearchMedia(searchTerm, Media.MediaType.Anime,
                        PageSize));
                    break;
                case SearchResultsActivity.AniListSearchTypes.Manga:
                    View.ShowMediaSearchResults(AniListService.SearchMedia(searchTerm, Media.MediaType.Manga,
                        PageSize));
                    break;
                case SearchResultsActivity.AniListSearchTypes.Characters:
                    View.ShowCharacterSearchResults(AniListService.SearchCharacters(searchTerm, PageSize));
                    break;
                case SearchResultsActivity.AniListSearchTypes.Staff:
                    View.ShowStaffSearchResults(AniListService.SearchStaff(searchTerm, PageSize));
                    break;
                case SearchResultsActivity.AniListSearchTypes.Studios:
                    View.ShowStudioSearchResults(AniListService.SearchStudios(searchTerm, PageSize));
                    break;
                case SearchResultsActivity.AniListSearchTypes.Users:
                    View.ShowUserSearchResults(AniListService.SearchUsers(searchTerm, PageSize));
                    break;
                case SearchResultsActivity.AniListSearchTypes.Forum:
                    View.ShowForumThreadSearchResults(AniListService.SearchForumThreads(searchTerm, PageSize));
                    break;
            }
        }

        public override Task Init()
        {
            // TODO: determine if these are needed for this presenter
            return Task.CompletedTask;
        }

        public async Task SaveMediaListEntry(MediaListEditDto editDto, Action onSuccess, Action onError)
        {
            var mediaUpdateResp = await AniListService.UpdateMediaListEntry(editDto, default(CancellationToken));

            mediaUpdateResp.Switch(mediaList =>
            {
                onSuccess();
                View.DisplaySnackbarMessage("Saved", Snackbar.LengthShort);
                View.UpdateMediaListItem(mediaList);
            }).Switch(error => onError());
        }

        public async Task DeleteMediaListEntry(int mediaListId, Action onSuccess, Action onError)
        {
            var mediaDeleteResp = await AniListService.DeleteMediaListEntry(mediaListId, default(CancellationToken));

            mediaDeleteResp.Switch((bool success) =>
            {
                onSuccess();
                View.DisplaySnackbarMessage("Deleted", Snackbar.LengthShort);
                View.RemoveMediaListItem(mediaListId);
            }).Switch(error =>
                onError());
        }
    }
}