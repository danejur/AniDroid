namespace AniDroid.AniList.Queries
{
    internal static partial class QueryStore
    {
        /// <summary>
        /// Returns: User
        /// <para></para>
        /// (Must be authorized)
        /// </summary>
        public static string GetCurrentUser => @"
{
  Data: Viewer {
    id
    name
    bannerImage
    avatar {
      large
    }
    options {
      titleLanguage
      displayAdultContent
    }
    mediaListOptions {
      scoreFormat
      rowOrder
      useLegacyLists
      animeList {
        sectionOrder
        splitCompletedSectionByFormat
        customLists
        advancedScoring
        advancedScoringEnabled
      }
      mangaList {
        sectionOrder
        splitCompletedSectionByFormat
        customLists
        advancedScoring
        advancedScoringEnabled
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userName?: string, userId?: int)
        /// <para></para>
        /// Returns: User
        /// </summary>
        public static string GetUser => @"
query ($userId: Int, $userName: String) {
  Data: User(id: $userId, name: $userName) {
    id
    name
    about
    avatar {
      large
      medium
    }
    bannerImage
    isFollowing
    options {
      titleLanguage
      displayAdultContent
    }
    mediaListOptions {
      scoreFormat
      rowOrder
      useLegacyLists
    }
    stats {
      watchedTime
      chaptersRead
      activityHistory {
        date
        amount
        level
      }
      animeStatusDistribution {
        status
        amount
      }
      mangaStatusDistribution {
        status
        amount
      }
      animeScoreDistribution {
        score
        amount
      }
      mangaScoreDistribution {
        score
        amount
      }
    }
    donatorTier
    unreadNotificationCount
    siteUrl
    updatedAt
  }
}
";

        /// <summary>
        /// Parameters: (id: int, type: MediaType, groupCompleted: bool)
        /// <para></para>
        /// Returns: MediaListCollection with User
        /// </summary>
        public static string GetMediaListsByUserIdAndType => @"
query ($userId: Int, $type: MediaType, $groupCompleted: Boolean) {
  Data: MediaListCollection(userId: $userId, type: $type, forceSingleCompletedList: $groupCompleted) {
    user {
      id
      name
      about(asHtml: true)
      avatar {
        large
        medium
      }
      bannerImage
      isFollowing
      options {
        titleLanguage
        displayAdultContent
      }
      mediaListOptions {
        scoreFormat
        rowOrder
        useLegacyLists
        animeList {
          sectionOrder
          customLists
          advancedScoring
          splitCompletedSectionByFormat
          advancedScoringEnabled
        }
        mangaList {
          sectionOrder
          customLists
          advancedScoring
          splitCompletedSectionByFormat
          advancedScoringEnabled
        }
      }
      donatorTier
      unreadNotificationCount
      siteUrl
      updatedAt
    }
    lists {
      name
      status
      isCustomList
      isSplitCompletedList
      entries {
        id
        createdAt
        advancedScores
        status
        score
        progress
        progressVolumes
        repeat
        priority
        private
        notes
        hiddenFromStatusLists
        updatedAt
        startedAt {
          year
          month
          day
        }
        completedAt {
          year
          month
          day
        }
        customLists(asArray: true)
        media {
          id
          title {
            userPreferred
            english
            romaji
            native
          }
          coverImage {
            color
            large
          }
          status
          episodes
          chapters
          volumes
          format
          type
          source
          duration
          averageScore
          meanScore
          popularity
          season
          seasonYear
          genres
          tags {
            name
          }
          nextAiringEpisode {
            id
            airingAt
            episode
            timeUntilAiring
          }
          externalLinks {
            site
          }
          startDate {
            year
            month
            day
          }
          endDate {
            year
            month
            day
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (queryText: string, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of User
        /// </summary>
        public static string SearchUsers => @"
query ($queryText: String, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: users(search: $queryText) {
      id
      name
      avatar {
        large
      }
      isFollowing
    }
  }
}
";

        /// <summary>
        /// Parameters: (animeId: int, mangaId: int, characterId: int, staffId: int, studioId: int)
        /// <para></para>
        /// Returns: UserFavourites
        /// </summary>
        public static string ToggleUserFavorite => @"
mutation ($animeId: Int, $mangaId: Int, $characterId: Int, $staffId: Int, $studioId: Int) {
  Data: ToggleFavourite(animeId: $animeId, mangaId: $mangaId, characterId: $characterId, staffId: $staffId, studioId: $studioId) {
    anime {
      nodes {
        id
      }
    }
    manga {
      nodes {
        id
      }
    }
    characters {
      nodes {
        id
      }
    }
    staff {
      nodes {
        id
      }
    }
    studios {
      nodes {
        id
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, message: string)
        /// <para></para>
        /// Returns: MessageActivity
        /// </summary>
        public static string PostUserMessage => @"
mutation ($userId: Int, $message: String) {
  Data: SaveMessageActivity(recipientId: $userId, message: $message) {
    id
    type
    recipientId
    createdAt
    message
  }
}
";

        /// <summary>
        /// Parameters: (userId: int)
        /// <para></para>
        /// Returns: User
        /// </summary>
        public static string ToggleUserFollowing => @"
mutation ($userId: Int) {
  Data: ToggleFollow(userId: $userId) {
    id
    isFollowing
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, sort: UserSort, page?: int, count?: int)
        /// <para></para>
        /// Returns: PagedData of User
        /// </summary>
        public static string GetUserFollowing => @"
query ($userId: Int!, $sort: [UserSort], $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: following(userId: $userId, sort: $sort) {
      id
      name
      avatar {
        large
      }
      isFollowing
      donatorTier
      isBlocked
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, sort: UserSort, page?: int, count?: int)
        /// <para></para>
        /// Returns: PagedData of User
        /// </summary>
        public static string GetUserFollowers => @"
query ($userId: Int!, $sort: [UserSort], $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: followers(userId: $userId, sort: $sort) {
      id
      name
      avatar {
        large
      }
      isFollowing
      donatorTier
      isBlocked
    }
  }
}
";

        /// <summary>
        /// Parameters: (mediaId: int, page?: int, count?: int)
        /// <para></para>
        /// Returns: PagedData of MediaList
        /// </summary>
        public static string GetMediaFollowingUsersMediaLists => @"
query ($mediaId: Int!, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: mediaList(mediaId: $mediaId, isFollowing: true, sort: UPDATED_TIME_DESC) {
      id
      status
      score
      progress
      media {
        status
        volumes
        chapters
        episodes
        type
      }
      user {
        id
        name
        avatar {
          large
        }
        mediaListOptions {
          scoreFormat
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: PagedData of Reviews
        /// </summary>
        public static string GetUserReviews => @"
query ($userId: Int!, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: reviews(userId: $userId) {
      id
      summary
      rating
      ratingAmount
      score
      userRating
      media {
        id
        title {
          userPreferred
        }
        coverImage {
          large
          color
        }
      }
      user {
        id
        name
        avatar {
          large
        }
        mediaListOptions {
          scoreFormat
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: PagedData of Media
        /// </summary>
        public static string GetUserFavoriteAnime => @"
query ($userId: Int, $page: Int, $perPage: Int) {
  Data: User(id: $userId) {
    favourites {
      anime(page: $page, perPage: $perPage) {
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
            coverImage {
              color
              large
            }
            title {
              userPreferred
            }
            format
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: PagedData of Media
        /// </summary>
        public static string GetUserFavoriteManga => @"
query ($userId: Int, $page: Int, $perPage: Int) {
  Data: User(id: $userId) {
    favourites {
      manga(page: $page, perPage: $perPage) {
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
            coverImage {
              color
              large
            }
            title {
              userPreferred
            }
            format
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: PagedData of Characters
        /// </summary>
        public static string GetUserFavoriteCharacters => @"
query ($userId: Int, $page: Int, $perPage: Int) {
  Data: User(id: $userId) {
    favourites {
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
              full
              native
              alternative
            }
            image {
              large
              medium
            }
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: PagedData of Staff
        /// </summary>
        public static string GetUserFavoriteStaff => @"
query ($userId: Int, $page: Int, $perPage: Int) {
  Data: User(id: $userId) {
    favourites {
      staff(page: $page, perPage: $perPage) {
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
              full
              native
              alternative
            }
            image {
              large
              medium
            }
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (userId: int, page: int, perPage: int)
        /// <para></para>
        /// Returns: PagedData of Studios
        /// </summary>
        public static string GetUserFavoriteStudios => @"
query ($userId: Int, $page: Int, $perPage: Int) {
  Data: User(id: $userId) {
    favourites {
      studios(page: $page, perPage: $perPage) {
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
            name
            
          }
        }
      }
    }
  }
}
";
    }
}
