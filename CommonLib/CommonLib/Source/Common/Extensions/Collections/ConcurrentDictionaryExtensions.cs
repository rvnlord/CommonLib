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
    }
}
