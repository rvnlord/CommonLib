using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Source.Common.Converters
{
    public static class DictionaryConverter
    {
        public static Dictionary<TKey, object> ValuesToObjects<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return dict.Select(kvp => new KeyValuePair<TKey, object>(kvp.Key, kvp.Value)).ToDictionary();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> en) => en.ToDictionary(el => el.Key, el => el.Value);
    }
}
