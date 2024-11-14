using AniDroid.AniList.Models;
using OneOf;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Enums.UserEnums;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.ForumModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.RecommendationModels;
using AniDroid.AniList.Models.ReviewModels;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniList.Models.StudioModels;
using AniDroid.AniList.Models.UserModels;
using AniDroid.AniList.Service;

namespace AniDroid.AniList.Interfaces
{
    public interface IAniListService
    {
        IAniListServiceConfig Config { get; }
        IAuthCodeResolver AuthCodeResolver { get; }

        #region Authorization

        Task<OneOf<AniListAuthorizationResponse, IAniListError>> AuthenticateUser(IAniListAuthConfig config, string code, CancellationToken cToken);

        #endregion

        #region Media

        Task<OneOf<Media, IAniListError>> GetMediaById(int mediaId, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> SearchMedia(string queryText, MediaType type, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> BrowseMedia(BrowseMediaDto browseDto, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetMediaCharacters(int mediaId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<StaffEdge>, IAniListError>> GetMediaStaff(int mediaId, int perPage);

        Task<OneOf<MediaList, IAniListError>> UpdateMediaListEntry(MediaListEditDto editDto, CancellationToken cToken);

        Task<OneOf<bool, IAniListError>> DeleteMediaListEntry(int mediaListId, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> GetMediaReviews(int mediaId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<ConnectionEdge<Recommendation>>, IAniListError>> GetMediaRecommendations(int mediaId, int perPage);

        Task<OneOf<IList<MediaTag>, IAniListError>> GetMediaTagCollectionAsync(CancellationToken cToken);

        Task<OneOf<IList<string>, IAniListError>> GetGenreCollectionAsync(CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<ConnectionEdge<MediaTrend>>, IAniListError>> GetMediaTrends(int mediaId, bool releasing, MediaTrendSort[] sort, int perPage);

        #endregion

        #region User

        Task<OneOf<User, IAniListError>> GetCurrentUser(CancellationToken cToken);

        Task<OneOf<User, IAniListError>> GetUser(string userName, int? userId, CancellationToken cToken);

        Task<OneOf<MediaListCollection, IAniListError>> GetUserMediaList(int userId, MediaType type, bool groupCompleted, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> SearchUsers(string queryText, int perPage);

        Task<OneOf<UserFavourites, IAniListError>> ToggleFavorite(FavoriteDto favoriteDto, CancellationToken cToken);

        Task<OneOf<AniListActivity, IAniListError>> PostUserMessage(int userId, string message, CancellationToken cToken);

        Task<OneOf<User, IAniListError>> ToggleFollowUser(int userId, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> GetUserFollowers(int userId, UserSort sort, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> GetUserFollowing(int userId, UserSort sort, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<MediaList>, IAniListError>> GetMediaFollowingUsersMediaLists(int mediaId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> GetUserReviews(int userId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetUserFavoriteAnime(int userId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetUserFavoriteManga(int userId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetUserFavoriteCharacters(int userId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<StaffEdge>, IAniListError>> GetUserFavoriteStaff(int userId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<StudioEdge>, IAniListError>> GetUserFavoriteStudios(int userId, int perPage);

        #endregion

        #region Activity

        /// <summary>
        /// Saves a text activity to AniList, which will show up in the user's feed.
        /// </summary>
        /// <param name="text">The text for the activity.</param>
        /// <param name="activityId">(Optional) The Id of the activity to update with new text.</param>
        /// <param name="cToken"></param>
        /// <returns></returns>
        Task<OneOf<AniListActivity, IAniListError>> SaveTextActivity(string text, int? activityId, CancellationToken cToken);

        Task<OneOf<DeletedResponse, IAniListError>> DeleteActivity(int activityId, CancellationToken cToken);

        Task<OneOf<ActivityReply, IAniListError>> PostActivityReply(int activityId, string text, CancellationToken cToken);

        Task<OneOf<List<User>, IAniListError>> ToggleLike(int id, LikeableType type, CancellationToken cToken);

        Task<OneOf<AniListActivity, IAniListError>> GetAniListActivityById(int id, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> GetAniListActivity(AniListActivityDto activityDto, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> GetAniListNotifications(bool resetNotificationCount, int perPage);

        Task<OneOf<User, IAniListError>> GetAniListNotificationCount(CancellationToken cToken);

        Task<OneOf<ActivityReply, IAniListError>> SaveActivityReply(int id, string text, CancellationToken cToken);

        Task<OneOf<DeletedResponse, IAniListError>> DeleteActivityReply(int id, CancellationToken cToken);

        #endregion

        #region Character

        Task<OneOf<Character, IAniListError>> GetCharacterById(int characterId, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<Character>, IAniListError>> SearchCharacters(string queryText, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetCharacterMedia(int characterId, MediaType mediaType, int perPage);

        #endregion

        #region Staff

        Task<OneOf<Staff, IAniListError>> GetStaffById(int staffId, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<Staff>, IAniListError>> SearchStaff(string queryText, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetStaffCharacters(int staffId, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetStaffMedia(int staffId, MediaType mediaType, int perPage);

        #endregion

        #region Studio

        Task<OneOf<Studio, IAniListError>> GetStudioById(int studioId, CancellationToken cToken);

        IAsyncEnumerable<OneOf<IPagedData<Studio>, IAniListError>> SearchStudios(string queryText, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetStudioMedia(int studioId, int perPage);

        #endregion

        #region ForumThread

        IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> SearchForumThreads(string queryText, int perPage);

        IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> GetMediaForumThreads(int mediaId, int perPage);

        #endregion

    }
}
