using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class ConcurrentBagExtensions
    {
        public static ConcurrentBag<T> AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> range)
        {
            foreach (var item in bag)
                bag.Add(item);
            return bag;
        }
    }
}
