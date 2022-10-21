using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLib.Source.Common.Converters
{
    public static class IEnumerableConverter
    {
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }

        public static async Task<T[]> ToArrayAsync<T>(this Task<IEnumerable<T>> en)
        {
            return (await (en ?? throw new NullReferenceException(nameof(en))).ConfigureAwait(false)).ToArray();
        }

        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> en)
        {
            return (await (en ?? throw new NullReferenceException(nameof(en))).ConfigureAwait(false)).ToList();
        }

        public static Tuple<T, T> ToTupleOf2<T>(this IEnumerable<T> en)
        {
            var arr = en.ToArray();
            if (arr.Length > 2)
                throw new ArgumentOutOfRangeException(nameof(arr), "Outer enumerable must contain exactly 2 elements");

            return new Tuple<T, T>(arr.Length > 0 ? arr[0] : default, arr.Length > 1 ? arr[1] : default);
        }

        public static Tuple<T, T, T> ToTupleOf3<T>(this IEnumerable<T> en)
        {
            var arr = en.ToArray();
            if (arr.Length > 3)
                throw new ArgumentOutOfRangeException(nameof(arr), "Outer enumerable must contain exactly 2 elements");

            return new Tuple<T, T, T>(arr.Length > 0 ? arr[0] : default, arr.Length > 1 ? arr[1] : default, arr.Length > 2 ? arr[2] : default);
        }

        public static KeyValuePair<T, T> ToKeyValuePair<T>(this IEnumerable<T> en)
        {
            var (k, v) = en.ToTupleOf2();
            return new KeyValuePair<T, T>(k, v);
        }

        public static KeyValuePair<T, T> ToKVP<T>(this IEnumerable<T> en) => en.ToKeyValuePair();
        
        public static List<KeyValuePair<TKey, TValue>> Flatten<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> deepKvps)
        {
            var list = new List<KeyValuePair<TKey, TValue>>();
            foreach (var (key, values) in deepKvps)
                list.AddRange(values.Select(value => new KeyValuePair<TKey, TValue>(key, value)));
            return list;
        }
    }
}
