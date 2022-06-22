using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Source.Common.Converters
{
    public static class ConcurrentDictionaryConverter
    {
        public static ConcurrentDictionary<TKey, object> ValuesToObjects<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict)
        {
            return dict.Select(kvp => new KeyValuePair<TKey, object>(kvp.Key, kvp.Value)).ToConcurrentDictionary();
        }

        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> en) => new(en);
    }
}
