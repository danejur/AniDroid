using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AniDroid.AniList
{
    public abstract class AniListEnum
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, AniListEnum>> ValueDictionaries = new ConcurrentDictionary<Type, Dictionary<string, AniListEnum>>();

        protected AniListEnum(string val, string displayVal, int index)
        {
            Value = val;
            DisplayValue = displayVal;
            Index = index;
        }

        public string Value { get; }
        public string DisplayValue { get; }
        public int Index { get; }

        private static Dictionary<string, AniListEnum> GetValueDictionary<T>() where T : AniListEnum
        {
            var type = typeof(T);

            if (!ValueDictionaries.TryGetValue(type, out var dict))
            {
                dict = ValueDictionaries[type] = type.GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.PropertyType == type)
                    .Select(x => x.GetValue(x) as T)
                    .ToDictionary(x => x?.Value, y => y as AniListEnum);
            }

            return dict;
        }

        public static string GetDisplayValue<T>(string value, string defaultValue = "") where T : AniListEnum =>
            GetEnum<T>(value)?.DisplayValue ?? defaultValue;

        public static string GetDisplayValue<T>(int index, string defaultValue = "") where T : AniListEnum =>
            GetEnumValues<T>().FirstOrDefault(x => x.Index == index)?.DisplayValue ?? defaultValue;

        public static T GetEnum<T>(string value) where T : AniListEnum =>
            (GetValueDictionary<T>().TryGetValue(value, out var retEnum) ? retEnum : null) as T;

        public static T GetEnum<T>(int position) where T : AniListEnum =>
            GetEnumValues<T>().ElementAtOrDefault(position);

        public static int GetIndex<T>(string value) where T : AniListEnum =>
            GetEnum<T>(value)?.Index ?? -1;

        // TODO: this might be a good thing to cache
        public static List<T> GetEnumValues<T>() where T : AniListEnum =>
            GetValueDictionary<T>().Select(x => x.Value as T).OrderBy(x => x.Index).ToList();

        public bool Equals(AniListEnum obj) =>
            obj?.GetType() == GetType() && obj.Value == Value;

        public bool EqualsAny<T>(params T[] objs) where T : AniListEnum => objs.Any(x => x.Equals(Value));

        public bool EqualsAny<T>(IList<T> objs) where T : AniListEnum => objs.Any(x => x.Equals(Value));

        public bool Equals(string val) =>
            Value == val;

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}
