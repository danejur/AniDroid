using System;
using System.Collections.Generic;
using AniDroid.AniList.GraphQL;
using AniDroid.AniList.Interfaces;

namespace AniDroid.AniList.Service
{
    public class AniListError : IAniListError
    {
        public AniListError(int statusCode, string errorMessage, Exception errorException, List<GraphQLError> graphQLErrors)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            ErrorException = errorException;
            GraphQLErrors = graphQLErrors;
        }

        public int StatusCode { get; }
        public string ErrorMessage { get; }
        public Exception ErrorException { get; }
        public List<GraphQLError> GraphQLErrors { get; }
    }
}
