using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MoreLinq;

namespace CommonLib.Source.Common.Extensions.Collections
{
    public static class NameValueCollectionExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this NameValueCollection nvc)
        {
            if (nvc == null)
                throw new ArgumentNullException(nameof(nvc));

            return nvc.AllKeys.SelectMany(nvc.GetValues, (k, v) => new KeyValuePair<string, string>(k, v));
        }

        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.AsEnumerable().ToDictionary();
        }
    }
}
