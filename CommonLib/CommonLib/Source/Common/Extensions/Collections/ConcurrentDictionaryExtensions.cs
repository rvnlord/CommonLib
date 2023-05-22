using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ConcurrentDictionaryExtensions
    {
        public static IEnumerable<TValue> SafelyGetValues<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> cdict)
        {
            return cdict.SafelyToDictionary().Values;
        }

        public static ConcurrentDictionary<TKey, TValue> AddRange<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            foreach (var (key, value) in newDict)
                dict.TryAdd(key, value);

            return dict;
        }
    }
}
