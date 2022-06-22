using System.Collections.Generic;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class StringRangeConverter
    {
        public static StringRange ToStringRange(this IEnumerable<string> en)
        {
            return (StringRange) en.ToPair();
        }
    }
}
