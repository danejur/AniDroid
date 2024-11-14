namespace AniDroid.AniList.Queries
{
    internal partial class QueryStore
    {
        /// <summary>
        /// Parameters: (queryText: string, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of Character
        /// </summary>
        public static string SearchCharacters => @"
query ($queryText: String, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: characters(search: $queryText) {
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
      isFavourite
    }
  }
}
";

        /// <summary>
        /// Parameters: (characterId: int)
        /// <para></para>
        /// Returns: Character with PageInfo for Anime and PageInfo for Manga
        /// </summary>
        public static string GetCharacterById => @"
query ($characterId: Int) {
  Data: Character(id: $characterId) {
    id
    name {
      first
      last
      native
      alternative
    }
    image {
      large
      medium
    }
    description(asHtml: true)
    isFavourite
    siteUrl
    Anime: media(perPage: 1, type: ANIME) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
      }
      edges {
        id
      }
    }
    Manga: media(perPage: 1, type: MANGA) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
      }
      edges {
        id
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (characterId: int, mediaType: MediaType, page: int, perPage: int)
        /// <para></para>
        /// Returns: Character with PagedData of Media with Staff
        /// </summary>
        public static string GetCharacterMedia => @"
query ($characterId: Int!, $mediaType: MediaType!, $page: Int, $perPage: Int) {
  Data: Character(id: $characterId) {
    media(page: $page, perPage: $perPage, type: $mediaType) {
      pageInfo {
        total
        perPage
        currentPage
        lastPage
        hasNextPage
      }
      edges {
        characterRole
        voiceActors {
          id
          name {
            first
            last
            native
          }
          image {
            large
          }
          language
          isFavourite
        }
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
      }
    }
  }
}
";
    }
}
