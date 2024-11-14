using System.IO;
using AniDroid.AniList.Utils.Internal;
using Newtonsoft.Json;

namespace AniDroid.AniList.Utils
{
    public class AniListJsonSerializer
    {
        public string Namespace { get; set; }
        public string ContentType { get; set; }
        public JsonSerializer Serializer { get; }

        public AniListJsonSerializer()
        {
            Serializer = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = AniListContractResolver.Instance,
            };
            ContentType = "application/json";
        }

        public string Serialize(object obj)
        {
            using var stringWriter = new StringWriter();
            using var jsonTextWriter = new JsonTextWriter(stringWriter);
            Serializer.Serialize(jsonTextWriter, obj);
            return stringWriter.ToString();
        }

        public T Deserialize<T>(string content)
        {
            using var stringReader = new StringReader(content);
            using var jsonTextReader = new JsonTextReader(stringReader);
            return Serializer.Deserialize<T>(jsonTextReader);
        }

        public static AniListJsonSerializer Default => new();
    }
}
