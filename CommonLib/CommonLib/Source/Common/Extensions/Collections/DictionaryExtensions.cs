using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CommonLib.Source.LibConfig;
using static CommonLib.Source.Common.Utils.LockUtils;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryGetValue(key, out var value) ? value : default;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (defaultValueProvider == null)
                throw new ArgumentNullException(nameof(defaultValueProvider));

            return dictionary.TryGetValue(key, out var value) ? value
                : defaultValueProvider();
        }

        public static TValue VorDef<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetValueOrDefault(dictionary, key);
        }

        public static TValue VorDef<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return GetValueOrDefault(dictionary, key, defaultValue);
        }

        public static TValue VorDef<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider)
        {
            return GetValueOrDefault(dictionary, key, defaultValueProvider);
        }

        public static TValue GetValueOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            dictionary.TryGetValue(key, out var val);
            return val;
        }

        public static TValue VorN<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            return GetValueOrNull(dictionary, key);
        }

        public static KeyValuePair<TKey, TValue> KVorN<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            dictionary.TryGetValue(key, out var val);
            return new KeyValuePair<TKey, TValue>(key, val);
        }

        public static void AddIfNotNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TValue : class
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (value != null)
                dictionary.Add(key, value);
        }

        public static IDictionary<TKey, TValue> AddIfNotExist<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TValue : class
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (dictionary.VorN(key) == null)
                dictionary.Add(key, value);
            return dictionary;
        }

        public static TValue AddIfNotExistAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) where TValue : class
        {
            return dictionary.AddIfNotExist(key, value)[key];
        }

        public static void AddIfNotNullAnd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, bool? condition, TKey key, TValue value) where TValue : class
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if (value != null && condition == true)
                dictionary.Add(key, value);
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public static bool Exists<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : class
        {
            return dict.VorN(key) != null;
        }

        public static void RemoveIfExists<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) where TValue : class
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));

            if (dict.Exists(key))
                dict.Remove(key);
        }

        public static TValue VorN_Ts<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return Lock (_globalSync, nameof(_globalSync), nameof(VorN_Ts), () =>
                GetValueOrNull(dictionary, key));
        }

        public static void V_Ts<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue val)
        {
            Lock (_globalSync, nameof(_globalSync), nameof(V_Ts), () =>
               dictionary[key] = val);
        }

        public static TKey[] KeysByValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue val)
        {
            return dict.Where(x => Equals(x.Value, val)).Select(kvp => kvp.Key).ToArray();
        }

        public static TKey[] KByV<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue val) => KeysByValue(dict, val);

        public static Dictionary<TKey, TValue> ReplaceAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            dict.Clear();
            foreach (var (key, value) in newDict)
                dict[key] = value;

            return dict;
        }

        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            foreach (var (key, value) in newDict)
                dict[key] = value;

            return dict;
        }

        public static IDictionary ReplaceAll(this IDictionary dict, IDictionary newDict)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (newDict == null)
                throw new ArgumentNullException(nameof(newDict));

            dict.Clear();
            foreach (var (key, value) in (Dictionary<object, object>) newDict)
                dict[key] = value;

            return dict;
        }
    }
}
