using System;
using Newtonsoft.Json;

namespace AniDroid.AniList.Utils.Internal
{
    internal class AniListJsonConverter<T> : JsonConverter
    {
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) => serializer.Deserialize<T>(reader);

        public override bool CanConvert(Type objectType) => true;
    }
}
