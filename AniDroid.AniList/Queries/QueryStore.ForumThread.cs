namespace AniDroid.AniList.Queries
{
    internal partial class QueryStore
    {
        /// <summary>
        /// Parameters: (queryText: string, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of ForumThread
        /// </summary>
        public static string SearchForumThreads => @"
query ($queryText: String, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: threads(search: $queryText) {
      id
      title
      replyCount
      siteUrl
      updatedAt
      createdAt
      likes {
        id
      }
      user {
        id
        avatar {
          large
        }
        name
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (mediaId: int!, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of ForumThread
        /// </summary>
        public static string GetMediaForumThreads => @"
query ($mediaId: Int!, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: threads(mediaCategoryId: $mediaId, sort: REPLY_COUNT_DESC) {
      id
      title
      replyCount
      siteUrl
      updatedAt
      createdAt
      likes {
        id
      }
      user {
        id
        avatar {
          large
        }
        name
      }
    }
  }
}
";
    }
}
