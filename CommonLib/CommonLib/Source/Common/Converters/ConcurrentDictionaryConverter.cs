using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;using CommonLib.Source.Common.Extensions.Collections;

namespace CommonLib.Source.Common.Converters
{
    public static class ConcurrentDictionaryConverter
    {
        public static ConcurrentDictionary<TKey, object> ValuesToObjects<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict)
        {
            return dict.Select(kvp => new KeyValuePair<TKey, object>(kvp.Key, kvp.Value)).ToConcurrentDictionary();
        }

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> en) => new(en);

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector) where TKey : notnull => source.ToDictionary(keySelector, valueSelector).ToConcurrentDictionary();

        public static Dictionary<TKey, TValue> SafelyToDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> cdict)
        {
            var dict = new Dictionary<TKey, TValue>();
            for (var i = cdict.Count - 1; i >= 0; i--)
            {
                var kvp = cdict.ElementAtOrDefault(i);
                if (kvp.Value is not null)
                    dict[kvp.Key] = kvp.Value;
            }
            return dict;
        }
    }
}
