using System;
using System.Collections.Generic;
using System.Linq;
using CommonLib.Source.Common.Utils.UtilClasses;
using Truncon.Collections;

namespace CommonLib.Source.Common.Converters
{
    public static class PairConverter
    {
        public static Pair<T, T2> ToPair<T, T2>(this KeyValuePair<T, T2> parameter)
        {
            return new Pair<T, T2>(parameter.Key, parameter.Value);
        }

        public static Pair<T, T2>[] ToPairs<T, T2>(this KeyValuePair<T, T2>[] parameters)
        {
            return parameters.Select(p => p.ToPair()).ToArray();
        }

        public static Pair<T, T2>[] ToPairs<T, T2>(this Dictionary<T, T2> parameters) => parameters.ToArray().ToPairs();
        public static Pair<T, T2>[] ToPairs<T, T2>(this OrderedDictionary<T, T2> parameters) => parameters.ToArray().ToPairs();

        public static Pair<T, T> ToPair<T>(this IEnumerable<T> en)
        {
            var arr = en.ToArray();
            if (arr.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(en), "pair should contain exactly two arguments");

            return new Pair<T, T>(arr[0], arr[1]);
        }
    }
}
