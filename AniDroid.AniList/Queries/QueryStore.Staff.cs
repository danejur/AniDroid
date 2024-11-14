namespace AniDroid.AniList.Queries
{
    internal partial class QueryStore
    {
        /// <summary>
        /// Parameters: (queryText: string, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of Staff
        /// </summary>
        public static string SearchStaff => @"
query ($queryText: String, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: staff(search: $queryText) {
      id
      name {
        first
        last
        native
      }
      language
      image {
        large
      }
      isFavourite
    }
  }
}
";

        /// <summary>
        /// Parameters: (staffId: int)
        /// <para></para>
        /// Returns: Staff with PageInfo for Anime, Manga, and Characters
        /// </summary>
        public static string GetStaffById => @"
query ($staffId: Int) {
  Data: Staff(id: $staffId) {
    id
    name {
      first
      last
      native
    }
    image {
      large
    }
    description(asHtml: true)
    isFavourite
    siteUrl
    language
    Anime: staffMedia(type: ANIME) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
    }
    Manga: staffMedia(type: MANGA) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
    }
    characters {
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
        /// Parameters: (staffId: int, mediaType: MediaType, page: int, perPage: int)
        /// <para></para>
        /// Returns: Staff with PagedData of Media
        /// </summary>
        public static string GetStaffMedia => @"
query ($staffId: Int!, $mediaType: MediaType!, $page: Int, $perPage: Int) {
  Data: Staff(id: $staffId) {
    staffMedia(page: $page, type: $mediaType, perPage: $perPage) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
      edges {
        node {
          id
          title {
            userPreferred
          }
          coverImage {
            large
          }
          format
          type
        }
        staffRole
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (staffId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: Staff with PagedData of Characters
        /// </summary>
        public static string GetStaffCharacters => @"
query ($staffId: Int, $page: Int, $perPage: Int) {
  Data: Staff(id: $staffId) {
    characters(page: $page, perPage: $perPage) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
      edges {
        node {
          id
          name {
            first
            last
            native
            alternative
          }
          image {
            large
          }
          siteUrl
          isFavourite
        }
      }
    }
  }
}
";
    }
}
