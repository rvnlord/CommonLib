using System;
using System.Collections.Generic;
using System.Linq;
using Truncon.Collections;

namespace CommonLib.Source.Common.Converters
{
    public static class OrderedDictionaryConverter
    {
        public static OrderedDictionary<TKey, object> ValuesToObjects<TKey, TValue>(this OrderedDictionary<TKey, TValue> dict) 
            => dict.Select(kvp => new KeyValuePair<TKey, object>(kvp.Key, kvp.Value)).ToOrderedDictionary();

        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
            => source.ToOrderedDictionary(n => n.Key, n => n.Value);

        public static OrderedDictionary<TKey, TValue> ToOrderedDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null)
                throw new ArgumentNullException(nameof(valueSelector));

            var dictionary = comparer == null
                ? new OrderedDictionary<TKey, TValue>()
                : new OrderedDictionary<TKey, TValue>(comparer);

            foreach (var item in source)
                dictionary.Add(keySelector(item), valueSelector(item));

            return dictionary;
        }
    }
}
