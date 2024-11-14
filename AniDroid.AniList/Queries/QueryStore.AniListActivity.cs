namespace AniDroid.AniList.Queries
{
    internal partial class QueryStore
    {
        /// <summary>
        /// Parameters: (text: string, activityId: int)
        /// <para></para>
        /// Returns: AniListActivity
        /// </summary>
        public static string SaveTextActivity => @"
mutation ($text: String, $activityId: Int) {
  Data: SaveTextActivity(text: $text, id: $activityId) {
    id
    userId
    type
    replyCount
    text
    createdAt
    user {
      id
      name
      avatar {
        large
      }
    }
    likes {
      id
      name
      avatar {
        large
      }
    }
    replies {
      id
      text
      createdAt
      user {
        id
        name
        avatar {
          large
        }
      }
      likes {
        id
        name
        avatar {
          large
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (activityId: int)
        /// <para></para>
        /// Returns: DeletedResponse
        /// </summary>
        public static string DeleteActivity => @"
mutation ($activityId: Int) {
  Data: DeleteActivity(id: $activityId) {
    deleted
  }
}
";

        /// <summary>
        /// Parameters: (activityId: int, text: string)
        /// <para></para>
        /// Returns: ActivityReply
        /// </summary>
        public static string PostActivityReply => @"
mutation ($activityId: Int, $text: String) {
  Data: SaveActivityReply(activityId: $activityId, text: $text) {
    id
    userId
    activityId
    text
    createdAt
    user {
      id
      name
      avatar {
        large
      }
    }
    likes {
      id
      name
      avatar {
        large
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (id: int, text: string)
        /// <para></para>
        /// Returns: ActivityReply
        /// </summary>
        public static string SaveActivityReply => @"
mutation ($id: Int, $text: String) {
  Data: SaveActivityReply(id: $id, text: $text) {
    id
    userId
    activityId
    text
    createdAt
    user {
      id
      name
      avatar {
        large
      }
    }
    likes {
      id
      name
      avatar {
        large
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (id: int)
        /// <para></para>
        /// Returns: DeletedResponse
        /// </summary>
        public static string DeleteActivityReply => @"
mutation ($id: Int) {
  Data: DeleteActivityReply(id: $id) {
    deleted
  }
}
";

        /// <summary>
        /// Parameters: (id: int, type: LikeableType)
        /// <para></para>
        /// Returns: List of Users
        /// </summary>
        public static string ToggleLike => @"
mutation ($id: Int, $type: LikeableType) {
  Data: ToggleLike(id: $id, type: $type) {
    id
    name
    avatar {
      large
    }
  }
}
";

        /// <summary>
        /// Parameters: (isFollowing: bool?, userId: int?, page: int, count: int)
        /// <para></para>
        /// Returns: PagedData of AniListActivity
        /// </summary>
        public static string GetAniListActivity => @"
query ($isFollowing: Boolean, $userId: Int, $page: Int, $count: Int) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: activities(isFollowing: $isFollowing, sort: ID_DESC, userId: $userId) {
      ... on TextActivity {
        id
        userId
        type
        replyCount
        text
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
        likes {
          id
          name
          avatar {
            large
          }
        }
        replies {
          id
          text
          createdAt
          user {
            id
            name
            avatar {
              large
            }
          }
          likes {
            id
            name
            avatar {
              large
            }
          }
        }
      }
      ... on ListActivity {
        id
        userId
        type
        status
        progress
        replyCount
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
        media {
          id
          title {
            userPreferred
          }
          coverImage {
            large
          }
        }
        likes {
          id
          name
          avatar {
            large
          }
        }
        replies {
          id
          text
          createdAt
          user {
            id
            name
            avatar {
              large
            }
          }
          likes {
            id
            name
            avatar {
              large
            }
          }
        }
      }
      ... on MessageActivity {
        id
        type
        replyCount
        createdAt
        message
        messenger {
          id
          name
          avatar {
            large
          }
        }
        likes {
          id
          name
          avatar {
            large
          }
        }
        replies {
          id
          text
          createdAt
          user {
            id
            name
            avatar {
              large
            }
          }
          likes {
            id
            name
            avatar {
              large
            }
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Parameters: (page: int, count: int, resetNotificationCount: bool = false)
        /// <para></para>
        /// Returns: PagedData of AniListNotification
        /// </summary>
        public static string GetUserNotifications => @"
query ($page: Int, $count: Int, $resetNotificationCount: Boolean = false) {
  Data: Page(page: $page, perPage: $count) {
    pageInfo {
      total
      perPage
      currentPage
      lastPage
      hasNextPage
    }
    Data: notifications(resetNotificationCount: $resetNotificationCount) {
      ... on AiringNotification {
        id
        type
        contexts
        createdAt
        episode
        media {
          id
          title {
            userPreferred
          }
          coverImage {
            large
          }
        }
      }
      ... on FollowingNotification {
        id
        type
        context
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ActivityMessageNotification {
        id
        type
        context
        createdAt
        activityId
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ActivityMentionNotification {
        id
        type
        context
        createdAt
        activityId
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ActivityReplyNotification {
        id
        activityId
        type
        context
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ActivityLikeNotification {
        id
        type
        context
        createdAt
        activityId
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ActivityReplyLikeNotification {
        id
        type
        context
        createdAt
        activityId
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ThreadCommentMentionNotification {
        id
        type
        context
        createdAt
        commentId
        thread {
          id
          title
        }
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ThreadCommentReplyNotification {
        id
        type
        context
        createdAt
        commentId
        thread {
          id
          title
        }
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ThreadCommentSubscribedNotification {
        id
        type
        context
        createdAt
        commentId
        thread {
          id
          title
        }
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ThreadCommentLikeNotification {
        id
        type
        context
        createdAt
        commentId
        thread {
          id
          title
        }
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ThreadLikeNotification {
        id
        type
        context
        createdAt
        thread {
          id
          title
        }
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on ActivityReplySubscribedNotification {
        id
        type
        context
        createdAt
        activityId
        user {
          id
          name
          avatar {
            large
          }
        }
      }
      ... on RelatedMediaAdditionNotification {
        id
        type
        context
        createdAt
        media {
          id
          title {
            userPreferred
          }
          coverImage {
            large
          }
        }
      }
    }
  }
}
";

        /// <summary>
        /// Returns: User with UnreadNotificationCount
        /// </summary>
        public static string GetUserNotificationCount => @"
query {
  Data: Viewer {
    unreadNotificationCount
  }
}
";

        /// <summary>
        /// Parameters: (id: int)
        /// <para></para>
        /// Returns: AniListActivity
        /// </summary>
        public static string GetAniListActivityById => @"
query ($id: Int) {
  Data: Activity(id: $id) {
    ... on TextActivity {
      id
      userId
      type
      replyCount
      text
      createdAt
      user {
        id
        name
        avatar {
          large
        }
      }
      likes {
        id
        name
        avatar {
          large
        }
      }
      replies {
        id
        text
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
        likes {
          id
          name
          avatar {
            large
          }
        }
      }
    }
    ... on ListActivity {
      id
      userId
      type
      status
      progress
      replyCount
      createdAt
      user {
        id
        name
        avatar {
          large
        }
      }
      media {
        id
        title {
          userPreferred
        }
        coverImage {
          large
        }
      }
      likes {
        id
        name
        avatar {
          large
        }
      }
      replies {
        id
        text
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
        likes {
          id
          name
          avatar {
            large
          }
        }
      }
    }
    ... on MessageActivity {
      id
      type
      replyCount
      createdAt
      message
      messenger {
        id
        name
        avatar {
          large
        }
      }
      likes {
        id
        name
        avatar {
          large
        }
      }
      replies {
        id
        text
        createdAt
        user {
          id
          name
          avatar {
            large
          }
        }
        likes {
          id
          name
          avatar {
            large
          }
        }
      }
    }
  }
}
";
    }
}
