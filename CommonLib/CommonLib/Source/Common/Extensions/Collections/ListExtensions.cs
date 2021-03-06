using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLib.Source.Common.Converters;
using Tensorflow;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ListExtensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static void RemoveBy<TSource>(this List<TSource> source, Func<TSource, bool> selector) where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            
            var src = source.ToArray();
            foreach (var entity in src)
            {
                if (selector(entity))
                    source.Remove(entity);
            }
        }

        public static void RemoveByMany<TSource, TKey>(this List<TSource> source, Func<TSource, TKey> selector, IEnumerable<TKey> matches) where TSource : class
        {
            if (matches == null)
                throw new ArgumentNullException(nameof(matches));

            foreach (var match in matches)
                source.RemoveBy(e => Equals(selector(e), match));
        }

        public static T[] IListToArray<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var array = new T[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        public static object[] IListToArray(this IList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var array = new object[list.Count];
            for (var i = 0; i < list.Count; i++)
                array[i] = list[i];
            return array;
        }

        public static List<T> ReplaceAll<T>(this List<T> list, IEnumerable<T> en)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var newList = en.ToList();
            list.RemoveAll();
            list.AddRange(newList);
            return list;
        }

        public static List<T> ReplaceAll<T>(this List<T> list, T newEl)
        {
            return list.ReplaceAll(newEl.ToEnumerable());
        }

        public static void RemoveAll<T>(this IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            while (collection.Count != 0)
                collection.RemoveAt(0);
        }

        public static int RemoveAll(this IList list, Predicate<object> match)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var list2 = list.Cast<object>().Where(current => match(current)).ToList();
            foreach (var current2 in list2)
                list.Remove(current2);
            return list2.Count;
        }

        public static void AddRange(this IList list, IEnumerable items)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (items == null)
                throw new ArgumentNullException(nameof(items));
            foreach (var current in items)
                list.Add(current);
        }

        public static void RemoveRange(this IList list, IEnumerable items)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items.Cast<object>().ToArray())
                list.RemoveAt(list.IndexOf_(item));
        }

        public static void RemoveIfExists<T>(this List<T> list, T item)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Contains(item))
                list.Remove(item);
        }

        public static List<T> RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
            return list;
        }

        public static T Extract<T>(this List<T> list, T n)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            var index = list.FindIndex(x => Equals(x, n));
            var result = list[index];
            list.RemoveAt(index);
            return result;
        }

        public static T Push<T>(this List<T> list, T n)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            list.Add(n);
            return n;
        }

        public static List<T> Replace<T>(this List<T> list, T oldEl, T newEl)
        {
            var idx = list.IndexOf_(oldEl);
            list[idx] = newEl;
            return list;
        }

        public static List<T> Unshift<T>(this List<T> list, T el)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            list.Insert(0, el);
            return list;
        }

        public static List<T> UnshiftRange<T>(this List<T> list, IEnumerable<T> en)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            list.InsertRange(0, en);
            return list;
        }

        public static List<T> AddIfNotExists<T>(this List<T> list, T el)
        {
            if (!list.Any(x => x.Equals(el)))
                list.add(el);
            return list;
        }

        public static async Task AddAsync<T>(this List<T> l, T el) => await Task.Run(() => l.Add(el));

        public static async Task AddRangeAsync<T>(this List<T> l, IEnumerable<T> range) => await Task.Run(() => l.AddRange(range));

    }
}
