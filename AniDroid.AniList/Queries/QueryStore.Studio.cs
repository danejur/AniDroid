namespace AniDroid.AniList.Queries
{
    internal partial class QueryStore
    {
        /// <summary>
        /// Parameters: (queryText: string, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of Studio
        /// </summary>
        public static string SearchStudios => @"
query ($queryText: String, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: studios(search: $queryText) {
      id
      name
      isFavourite
    }
  }
}
";

        /// <summary>
        /// Parameters: (studioId: int)
        /// <para></para>
        /// Returns: Studio
        /// </summary>
        public static string GetStudioById => @"
query ($studioId: Int) {
  Data: Studio(id: $studioId) {
    id
    name
    siteUrl
    isFavourite
    media {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (studioId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: Studio
        /// </summary>
        public static string GetStudioMedia => @"
query ($studioId: Int, $page: Int, $perPage: Int) {
  Data: Studio(id: $studioId) {
    media(page: $page, perPage: $perPage) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
      edges {
        isMainStudio
        node {
          id
          title {
            userPreferred
          }
          coverImage {
            large
          }
          type
          format
          isFavourite
        }
      }
    }
  }
}
";
    }
}
