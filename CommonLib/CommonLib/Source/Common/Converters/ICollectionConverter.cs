using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonLib.Source.Common.Converters
{
    public static class ICollectionConverter
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
    }
}
