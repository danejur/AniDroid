using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using AniDroid.AniList.Models;
using System.Net.Http;
using System.Text;
using AniDroid.AniList.Dto;
using AniDroid.AniList.Enums;
using AniDroid.AniList.Enums.MediaEnums;
using AniDroid.AniList.Enums.UserEnums;
using Newtonsoft.Json;
using AniDroid.AniList.Interfaces;
using AniDroid.AniList.Queries;
using Newtonsoft.Json.Linq;
using AniDroid.AniList.GraphQL;
using AniDroid.AniList.Models.ActivityModels;
using AniDroid.AniList.Models.CharacterModels;
using AniDroid.AniList.Models.ForumModels;
using AniDroid.AniList.Models.MediaModels;
using AniDroid.AniList.Models.RecommendationModels;
using AniDroid.AniList.Models.ReviewModels;
using AniDroid.AniList.Models.StaffModels;
using AniDroid.AniList.Models.StudioModels;
using AniDroid.AniList.Models.UserModels;
using AniDroid.AniList.Utils;
using AniDroid.AniList.Utils.Internal;
using OneOf;

namespace AniDroid.AniList.Service
{
    public class AniListService : IAniListService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IAniListServiceConfig Config { get; }
        public IAuthCodeResolver AuthCodeResolver { get; }

        public AniListService(IAniListServiceConfig config, IAuthCodeResolver auth, IHttpClientFactory httpClientFactory)
        {
            Config = config;
            AuthCodeResolver = auth;
            _httpClientFactory = httpClientFactory;
        }

        #region Authorization

        public async Task<OneOf<AniListAuthorizationResponse, IAniListError>> AuthenticateUser(IAniListAuthConfig config, string code, CancellationToken cToken)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsync(config.AuthTokenUri, new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", config.ClientId),
                    new KeyValuePair<string, string>("client_secret", config.ClientSecret),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", config.RedirectUri),
                    new KeyValuePair<string, string>("code", code)
                }));

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AniListAuthorizationResponse>(responseContent);
            }
            catch (Exception e)
            {
                return new AniListError(0, e.Message, e, null);
            }
        }

        #endregion

        #region Media

        public Task<OneOf<Media, IAniListError>> GetMediaById(int mediaId, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetMediaById,
                Variables = new { mediaId },
            };
            return GetResponseAsync<Media>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> SearchMedia(string queryText,
            MediaType type = null, int perPage = 20)
        {
            var arguments = new
            {
                queryText,
                type = type?.Value,
            };
            return new PagedAsyncEnumerable<Media>(perPage,
                CreateGetPageFunc<Media>(QueryStore.SearchMedia, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Media>, IAniListError>> BrowseMedia(BrowseMediaDto browseDto, int perPage)
        {
            return new PagedAsyncEnumerable<Media>(perPage,
                CreateGetPageFunc<Media>(QueryStore.BrowseMedia, browseDto),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetMediaCharacters(int mediaId, int perPage)
        {
            var arguments = new { mediaId };
            return new PagedAsyncEnumerable<CharacterEdge>(perPage,
                CreateGetPageFunc<CharacterEdge, Media>(QueryStore.GetMediaCharacters, arguments, media => media.Characters),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<StaffEdge>, IAniListError>> GetMediaStaff(int mediaId, int perPage)
        {
            var arguments = new { mediaId };
            return new PagedAsyncEnumerable<StaffEdge>(perPage,
                CreateGetPageFunc<StaffEdge, Media>(QueryStore.GetMediaStaff, arguments, media => media.Staff),
                HasNextPage);
        }

        public Task<OneOf<MediaList, IAniListError>> UpdateMediaListEntry(MediaListEditDto editDto, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.UpdateMediaList,
                Variables = editDto
            };
            return GetResponseAsync<MediaList>(query, cToken);
        }

        public async Task<OneOf<bool, IAniListError>> DeleteMediaListEntry(int mediaListId, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.DeleteMediaList,
                Variables = new { mediaListId }
            };

            return await GetResponseAsync(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> GetMediaReviews(int mediaId, int perPage)
        {
            var arguments = new { mediaId };
            return new PagedAsyncEnumerable<Review>(perPage,
                CreateGetPageFunc<Review>(QueryStore.GetMediaReviews, arguments), HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<ConnectionEdge<Recommendation>>, IAniListError>> GetMediaRecommendations(int mediaId, int perPage)
        {
            var arguments = new { mediaId };
            return new PagedAsyncEnumerable<ConnectionEdge<Recommendation>>(perPage,
                CreateGetPageFunc<ConnectionEdge<Recommendation>, Media>(QueryStore.GetMediaRecommendations, arguments, media => media.Recommendations),
                HasNextPage);
        }

        public async Task<OneOf<IList<MediaTag>, IAniListError>> GetMediaTagCollectionAsync(CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetMediaTagCollection
            };

            return await GetResponseAsync<IList<MediaTag>>(query, cToken);
        }

        public async Task<OneOf<IList<string>, IAniListError>> GetGenreCollectionAsync(CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetGenreCollection
            };

            return await GetResponseAsync<IList<string>>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<ConnectionEdge<MediaTrend>>, IAniListError>> GetMediaTrends(int mediaId, bool releasing, MediaTrendSort[] sort, int perPage)
        {
            var arguments = new { mediaId, releasing, sort, perPage };
            return new PagedAsyncEnumerable<ConnectionEdge<MediaTrend>>(perPage,
                CreateGetPageFunc<ConnectionEdge<MediaTrend>, Media>(QueryStore.GetMediaTrends, arguments, media => media.Trends),
                HasNextPage);
        }

        #endregion

        #region User

        public Task<OneOf<User, IAniListError>> GetCurrentUser(CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetCurrentUser
            };
            return GetResponseAsync<User>(query, cToken);
        }

        public Task<OneOf<User, IAniListError>> GetUser(string userName, int? userId, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetUser,
                Variables = new { userName, userId },
            };
            return GetResponseAsync<User>(query, cToken);
        }

        public Task<OneOf<MediaListCollection, IAniListError>> GetUserMediaList(int userId, MediaType type, bool groupCompleted, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetMediaListsByUserIdAndType,
                Variables = new { userId, type, groupCompleted },
            };
            return GetResponseAsync<MediaListCollection>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> SearchUsers(string queryText,
            int perPage = 20)
        {
            var arguments = new { queryText };
            return new PagedAsyncEnumerable<User>(perPage,
                CreateGetPageFunc<User>(QueryStore.SearchUsers, arguments),
                HasNextPage);
        }

        public Task<OneOf<UserFavourites, IAniListError>> ToggleFavorite(FavoriteDto favoriteDto, CancellationToken cToken)
        {
            var mutation = new GraphQLQuery
            {
                Query = QueryStore.ToggleUserFavorite,
                Variables = favoriteDto,
            };
            return GetResponseAsync<UserFavourites>(mutation, cToken);
        }

        public Task<OneOf<AniListActivity, IAniListError>> PostUserMessage(int userId, string message, CancellationToken cToken)
        {
            var mutation = new GraphQLQuery
            {
                Query = QueryStore.PostUserMessage,
                Variables = new { userId, message }
            };
            return GetResponseAsync<AniListActivity>(mutation, cToken);
        }

        public Task<OneOf<User, IAniListError>> ToggleFollowUser(int userId, CancellationToken cToken)
        {
            var mutation = new GraphQLQuery
            {
                Query = QueryStore.ToggleUserFollowing,
                Variables = new { userId }
            };
            return GetResponseAsync<User>(mutation, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> GetUserFollowers(int userId, UserSort sort,
            int perPage = 20)
        {
            var arguments = new
            {
                userId,
                sort
            };
            return new PagedAsyncEnumerable<User>(perPage,
                CreateGetPageFunc<User>(QueryStore.GetUserFollowers, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<User>, IAniListError>> GetUserFollowing(int userId, UserSort sort,
            int perPage = 20)
        {
            var arguments = new
            {
                userId,
                sort
            };
            return new PagedAsyncEnumerable<User>(perPage,
                CreateGetPageFunc<User>(QueryStore.GetUserFollowing, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaList>, IAniListError>> GetMediaFollowingUsersMediaLists(int mediaId, int perPage)
        {
            var arguments = new
            {
                mediaId
            };
            return new PagedAsyncEnumerable<MediaList>(perPage,
                CreateGetPageFunc<MediaList>(QueryStore.GetMediaFollowingUsersMediaLists, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Review>, IAniListError>> GetUserReviews(int userId, int perPage)
        {
            var arguments = new { userId };
            return new PagedAsyncEnumerable<Review>(perPage,
                CreateGetPageFunc<Review>(QueryStore.GetUserReviews, arguments), HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetUserFavoriteAnime(int userId, int perPage)
        {
            var arguments = new
            {
                userId
            };
            return new PagedAsyncEnumerable<MediaEdge>(perPage,
                CreateGetPageFunc<MediaEdge, User>(QueryStore.GetUserFavoriteAnime, arguments, x => x.Favourites.Anime),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetUserFavoriteManga(int userId, int perPage)
        {
            var arguments = new
            {
                userId
            };
            return new PagedAsyncEnumerable<MediaEdge>(perPage,
                CreateGetPageFunc<MediaEdge, User>(QueryStore.GetUserFavoriteManga, arguments, x => x.Favourites.Manga),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetUserFavoriteCharacters(int userId, int perPage)
        {
            var arguments = new
            {
                userId
            };
            return new PagedAsyncEnumerable<CharacterEdge>(perPage,
                CreateGetPageFunc<CharacterEdge, User>(QueryStore.GetUserFavoriteCharacters, arguments, x => x.Favourites.Characters),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<StaffEdge>, IAniListError>> GetUserFavoriteStaff(int userId, int perPage)
        {
            var arguments = new
            {
                userId
            };
            return new PagedAsyncEnumerable<StaffEdge>(perPage,
                CreateGetPageFunc<StaffEdge, User>(QueryStore.GetUserFavoriteStaff, arguments, x => x.Favourites.Staff),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<StudioEdge>, IAniListError>> GetUserFavoriteStudios(int userId, int perPage)
        {
            var arguments = new
            {
                userId
            };
            return new PagedAsyncEnumerable<StudioEdge>(perPage,
                CreateGetPageFunc<StudioEdge, User>(QueryStore.GetUserFavoriteStudios, arguments, x => x.Favourites.Studios),
                HasNextPage);
        }

        #endregion

        #region Activity

        public Task<OneOf<List<User>, IAniListError>> ToggleLike(int id, LikeableType type, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.ToggleLike,
                Variables = new { id, type = type.Value },
            };
            return GetResponseAsync<List<User>>(query, cToken);
        }

        public Task<OneOf<AniListActivity, IAniListError>> SaveTextActivity(string text, int? activityId, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.SaveTextActivity,
                Variables = new { text, activityId },
            };
            return GetResponseAsync<AniListActivity>(query, cToken);
        }

        public Task<OneOf<DeletedResponse, IAniListError>> DeleteActivity(int activityId, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.DeleteActivity,
                Variables = new { activityId },
            };
            return GetResponseAsync<DeletedResponse>(query, cToken);
        }

        public Task<OneOf<ActivityReply, IAniListError>> PostActivityReply(int activityId, string text, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.PostActivityReply,
                Variables = new { activityId, text },
            };
            return GetResponseAsync<ActivityReply>(query, cToken);
        }

        public Task<OneOf<AniListActivity, IAniListError>> GetAniListActivityById(int id, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetAniListActivityById,
                Variables = new { id },
            };
            return GetResponseAsync<AniListActivity>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListActivity>, IAniListError>> GetAniListActivity(AniListActivityDto activityDto, int perPage = 20)
        {
            return new PagedAsyncEnumerable<AniListActivity>(perPage,
                CreateGetPageFunc<AniListActivity>(QueryStore.GetAniListActivity, activityDto),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<AniListNotification>, IAniListError>> GetAniListNotifications(bool resetNotificationCount, int perPage = 20)
        {
            return new PagedAsyncEnumerable<AniListNotification>(perPage,
                CreateGetPageFunc<AniListNotification>(QueryStore.GetUserNotifications, new { resetNotificationCount }),
                HasNextPage);
        }

        public Task<OneOf<User, IAniListError>> GetAniListNotificationCount(CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetUserNotificationCount
            };
            return GetResponseAsync<User>(query, cToken);
        }

        public Task<OneOf<ActivityReply, IAniListError>> SaveActivityReply(int id, string text, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.SaveActivityReply,
                Variables = new { id, text },
            };
            return GetResponseAsync<ActivityReply>(query, cToken);
        }

        public Task<OneOf<DeletedResponse, IAniListError>> DeleteActivityReply(int id, CancellationToken cToken)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.DeleteActivityReply,
                Variables = new { id },
            };
            return GetResponseAsync<DeletedResponse>(query, cToken);
        }

        #endregion

        #region Character

        public Task<OneOf<Character, IAniListError>> GetCharacterById(int characterId, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetCharacterById,
                Variables = new { characterId },
            };
            return GetResponseAsync<Character>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Character>, IAniListError>> SearchCharacters(string queryText,
            int perPage = 20)
        {
            var arguments = new { queryText };
            return new PagedAsyncEnumerable<Character>(perPage,
                CreateGetPageFunc<Character>(QueryStore.SearchCharacters, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetCharacterMedia(int characterId, MediaType mediaType, int perPage = 20)
        {
            var arguments = new { characterId, mediaType = mediaType?.Value };
            return new PagedAsyncEnumerable<MediaEdge>(perPage,
                CreateGetPageFunc<MediaEdge, Character>(QueryStore.GetCharacterMedia, arguments, character => character.Media),
                HasNextPage);
        }

        #endregion

        #region Staff

        public Task<OneOf<Staff, IAniListError>> GetStaffById(int staffId, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetStaffById,
                Variables = new { staffId },
            };
            return GetResponseAsync<Staff>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Staff>, IAniListError>> SearchStaff(string queryText,
            int perPage = 20)
        {
            var arguments = new { queryText };
            return new PagedAsyncEnumerable<Staff>(perPage,
                CreateGetPageFunc<Staff>(QueryStore.SearchStaff, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<CharacterEdge>, IAniListError>> GetStaffCharacters(int staffId, int perPage = 20)
        {
            var arguments = new { staffId };
            return new PagedAsyncEnumerable<CharacterEdge>(perPage,
                CreateGetPageFunc<CharacterEdge, Staff>(QueryStore.GetStaffCharacters, arguments, staff => staff.Characters),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetStaffMedia(int staffId, MediaType mediaType, int perPage = 20)
        {
            var arguments = new { staffId, mediaType = mediaType.Value };
            return new PagedAsyncEnumerable<MediaEdge>(perPage,
                CreateGetPageFunc<MediaEdge, Staff>(QueryStore.GetStaffMedia, arguments, staff => staff.StaffMedia),
                HasNextPage);
        }


        #endregion

        #region Studio

        public Task<OneOf<Studio, IAniListError>> GetStudioById(int studioId, CancellationToken cToken = default)
        {
            var query = new GraphQLQuery
            {
                Query = QueryStore.GetStudioById,
                Variables = new { studioId },
            };
            return GetResponseAsync<Studio>(query, cToken);
        }

        public IAsyncEnumerable<OneOf<IPagedData<Studio>, IAniListError>> SearchStudios(string queryText,
            int perPage = 20)
        {
            var arguments = new { queryText };
            return new PagedAsyncEnumerable<Studio>(perPage,
                CreateGetPageFunc<Studio>(QueryStore.SearchStudios, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<MediaEdge>, IAniListError>> GetStudioMedia(int studioId, int perPage = 20)
        {
            var arguments = new { studioId };
            return new PagedAsyncEnumerable<MediaEdge>(perPage,
                CreateGetPageFunc<MediaEdge, Studio>(QueryStore.GetStudioMedia, arguments, studio => studio.Media),
                HasNextPage);
        }

        #endregion

        #region ForumThread

        public IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> SearchForumThreads(string queryText,
            int perPage = 20)
        {
            var arguments = new { queryText };
            return new PagedAsyncEnumerable<ForumThread>(perPage,
                CreateGetPageFunc<ForumThread>(QueryStore.SearchForumThreads, arguments),
                HasNextPage);
        }

        public IAsyncEnumerable<OneOf<IPagedData<ForumThread>, IAniListError>> GetMediaForumThreads(int mediaId, int perPage)
        {
            var arguments = new { mediaId };
            return new PagedAsyncEnumerable<ForumThread>(perPage,
                CreateGetPageFunc<ForumThread>(QueryStore.GetMediaForumThreads, arguments),
                HasNextPage);
        }

        #endregion

        #region Internal

        /// <summary>
        /// Creates an HttpClient using the IHttpClientFactory, configuration,  and auth resolver passed into the service's constructor.
        /// </summary>
        /// <returns></returns>
        private HttpClient CreateClient()
        {
            var retClient = _httpClientFactory.CreateClient("anidroid");
            retClient.BaseAddress = new Uri(Config.BaseUrl);

            if (AuthCodeResolver.IsAuthorized)
            {
                retClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AuthCodeResolver.AuthCode}");
            }

            return retClient;
        }

        // TODO: Document
        private async Task<OneOf<T, IAniListError>> GetResponseAsync<TResponse, T>(GraphQLQuery query, Func<TResponse, T> getObjectFunc, CancellationToken cToken) where T : class where TResponse : class
        {
            try
            {
                var resp = await CreateClient().PostAsync("",
                    new StringContent(AniListJsonSerializer.Default.Serialize(query), Encoding.UTF8,
                        "application/json"), cToken);

                if (!resp.IsSuccessStatusCode)
                {
                    return new AniListError((int)resp.StatusCode, null, null, null);
                }

                var respContent = await resp.Content.ReadAsStringAsync();
                var respData = AniListJsonSerializer.Default.Deserialize<GraphQLResponse<TResponse>>(respContent);

                return getObjectFunc(respData.Value);
            }
            catch (Exception e)
            {
                return new AniListError(0, e.Message, e, null);
            }
        }

        // TODO: Document
        private Task<OneOf<T, IAniListError>> GetResponseAsync<T>(GraphQLQuery query, CancellationToken cToken)
            where T : class
        {
            return GetResponseAsync<T, T>(query, obj => obj, cToken);
        }

        // TODO: Document
        private async Task<OneOf<bool, IAniListError>> GetResponseAsync(GraphQLQuery query, CancellationToken cToken)
        {
            try
            {
                var resp = await CreateClient().PostAsync("",
                    new StringContent(AniListJsonSerializer.Default.Serialize(query), Encoding.UTF8,
                        "application/json"), cToken);

                resp.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception e)
            {
                return new AniListError(0, e.Message, e, null);
            }
        }

        // TODO: Document
        private Func<PagingInfo, CancellationToken, Task<OneOf<IPagedData<T>, IAniListError>>> CreateGetPageFunc<T>(string queryString,
            object variables)
        {
            return CreateGetPageFunc<T, IPagedData<T>>(queryString, variables, obj => obj);
        }

        // TODO: Document
        private Func<PagingInfo, CancellationToken, Task<OneOf<IPagedData<T>, IAniListError>>> CreateGetPageFunc<T, TResponse>(string queryString,
            object variables, Func<TResponse, IPagedData<T>> getPagedDataFunc) where TResponse : class
        {
            Task<OneOf<IPagedData<T>, IAniListError>> GetPageAsync(PagingInfo info, CancellationToken ct)
            {
                var vars = JObject.FromObject(variables ?? new object(), AniListJsonSerializer.Default.Serializer);
                vars.Add("page", info.Page);
                vars.Add("count", info.PageSize);

                var query = new GraphQLQuery
                {
                    Query = queryString,
                    Variables = vars,

                };

                return GetResponseAsync(query, getPagedDataFunc, ct);
            }

            return GetPageAsync;
        }

        private static bool HasNextPage<T>(PagingInfo info, OneOf<IPagedData<T>, IAniListError> data) => data.Match((IAniListError error) => false)
            .Match(pagedData => pagedData?.PageInfo?.HasNextPage == true);

#endregion

    }
}
