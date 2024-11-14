using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AniDroid.AniList.DataTypes;
using AniDroid.AniList.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AniDroid.AniList.Utils.Internal
{
    internal class AniListContractResolver : DefaultContractResolver
    {
        private static AniListContractResolver _instance;

        private readonly ConcurrentDictionary<Type, JsonConverter> _converterCache;

        // Manual Singleton ftw!
        public static AniListContractResolver Instance
            => _instance ?? (_instance = new AniListContractResolver());

        public readonly Dictionary<Type, Type> InterfaceConcreteMap;

        private AniListContractResolver()
        {
            _converterCache = new ConcurrentDictionary<Type, JsonConverter>();

            NamingStrategy = new CamelCaseNamingStrategy();
            InterfaceConcreteMap = new Dictionary<Type, Type>
            {
                { typeof(IList<>), typeof(List<>) },
                { typeof(ICollection<>), typeof(List<>) },
                { typeof(IPagedData<>), typeof(PagedData<>) },
            };
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType.IsSubclassOf(typeof(AniListEnum)))
            {
                return GetAniListEnumConverter(objectType);
            }

            if (objectType.IsInterface)
            {
                return GetInterfaceConverter(objectType);
            }

            return base.ResolveContractConverter(objectType);
        }

        private JsonConverter GetInterfaceConverter(Type interfaceType)
        {
            var isGeneric = interfaceType.IsGenericType;
            var refinedType = isGeneric
                ? interfaceType.GetGenericTypeDefinition()
                : interfaceType;

            if (!InterfaceConcreteMap.ContainsKey(refinedType))
            {
                return base.ResolveContractConverter(interfaceType);
            }

            var actualType = InterfaceConcreteMap[refinedType];
            var concreteGenericType = actualType.MakeGenericType(isGeneric ? interfaceType.GetGenericArguments() : new Type[0]);

            if (_converterCache.ContainsKey(concreteGenericType))
            {
                return _converterCache[concreteGenericType];
            }

            var converterType = typeof(AniListJsonConverter<>).MakeGenericType(concreteGenericType);
            return _converterCache[concreteGenericType] = Activator.CreateInstance(converterType) as JsonConverter;
        }

        private JsonConverter GetAniListEnumConverter(Type enumType)
        {
            if (_converterCache.ContainsKey(enumType))
            {
                return _converterCache[enumType];
            }

            var converterType = typeof(AniListEnumConverter<>).MakeGenericType(enumType);
            return _converterCache[enumType] = Activator.CreateInstance(converterType) as JsonConverter;
        }
    }
}
