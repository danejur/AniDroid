using System;
using System.Collections.Generic;
using AniDroid.AniList.GraphQL;

namespace AniDroid.AniList.Interfaces
{
    public interface IAniListError
    {
        int StatusCode { get; }
        string ErrorMessage { get; }
        Exception ErrorException { get; }
        List<GraphQLError> GraphQLErrors { get; }
    }
}
