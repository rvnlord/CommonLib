using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ICollectionExtensions
    {
        public static T[] IColToArray<T>(this ICollection<T> col)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));

            var array = new T[col.Count];
            col.CopyTo(array, 0);
            return array;
        }

        public static object[] IColToArray(this ICollection col)
        {
            if (col == null)
                throw new ArgumentNullException(nameof(col));

            var array = new object[col.Count];
            col.CopyTo(array, 0);
            return array;
        }

        public static int RemoveAll<T>(this ICollection<T> collection, Predicate<T> match)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var array = (from item in collection
                where match(item)
                select item).ToArray();
            var array2 = array;
            foreach (var item2 in array2)
                collection.Remove(item2);
            return array.Length;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (items == null)
                throw new ArgumentNullException(nameof(items));
            items.ForEach(collection.Add);
        }
    }
}
