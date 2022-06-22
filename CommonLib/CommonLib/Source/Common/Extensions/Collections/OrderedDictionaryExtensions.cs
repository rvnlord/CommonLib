using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib.Source.Common.Converters;
using Truncon.Collections;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class OrderedDictionaryExtensions
    {
        public static TValue VorN<TKey, TValue>(this OrderedDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            dictionary.TryGetValue(key, out var val);
            return val;
        }

        public static KeyValuePair<TKey, TValue> KVorN<TKey, TValue>(this OrderedDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            dictionary.TryGetValue(key, out var val);
            return new KeyValuePair<TKey, TValue>(key, val);
        }

        public static string ToQueryString(this OrderedDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count <= 0) return "";
            var sb = new StringBuilder();
            foreach (var item in parameters)
                sb.Append($"&{item.Key.ToAddressEncoded()}={item.Value.ToString().ToAddressEncoded()}");
            return sb.ToString().Skip(1);
        }

        public static void AddIfNotNull<T, T2>(this OrderedDictionary<T, T2> parameters, T key, T2 value)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (value != null)
                parameters.Add(key, value);
        }

        public static void AddIfNotNullAnd<T, T2>(this OrderedDictionary<T, T2> parameters, bool? condition, T key, T2 value)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            if (value != null && condition == true)
                parameters.Add(key, value);
        }

        public static void RemoveIfExists<TKey, TValue>(this OrderedDictionary<TKey, TValue> dict, TKey key) where TValue : class
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));

            if (dict.ContainsKey(key))
                dict.Remove(key);
        }

        public static OrderedDictionary<TKey, TValue> OrderBy<TKey, TValue>(this OrderedDictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, TKey> selector)
        {
            if (dict == null)
                throw new NullReferenceException(nameof(dict));

            return dict.AsEnumerable().OrderBy(selector).ToOrderedDictionary();
        }

        public static OrderedDictionary<TKey, TValue> ReplaceAll<TKey, TValue>(this OrderedDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> kvps)
        {
            if (dict == null)
                throw new NullReferenceException(nameof(dict));
            if (kvps == null)
                throw new NullReferenceException(nameof(kvps));

            dict.Clear();
            foreach (var kvp in kvps)
                dict[kvp.Key] = kvp.Value;
            return dict;
        }
    }
}
