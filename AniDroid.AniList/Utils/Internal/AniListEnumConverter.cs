using System;
using Newtonsoft.Json;

namespace AniDroid.AniList.Utils.Internal
{
    internal class AniListEnumConverter<T> : JsonConverter where T : AniListEnum
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((value as AniListEnum)?.Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var strVal = reader.Value as string;
            return AniListEnum.GetEnum<T>(strVal);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(AniListEnum));
        }
    }
}
