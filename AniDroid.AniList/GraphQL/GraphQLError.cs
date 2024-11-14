using System.Collections.Generic;
using System.Linq;

namespace AniDroid.AniList.GraphQL
{
    public class GraphQLError
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public List<GraphQLErrorLocation> Locations { get; set; }
        public ValidationErrors Validation { get; set; }

        public class GraphQLErrorLocation
        {
            public int Line { get; set; }
            public int Column { get; set; }
        }

        public class ValidationErrors
        {
            public List<string> ActivityId { get; set; }
        }

        public bool IsIdInvalidError()
        {
            return (Message == "validation") && Validation?.ActivityId?.Any(x => x.Contains("id is invalid")) == true;
        }
    }
}
