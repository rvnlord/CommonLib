using System;
using System.Globalization;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class LongConverter
    {
        public static long? ToLongN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return Convert.ToInt64(o, CultureInfo.InvariantCulture);
            if (o.GetType().IsEnum) return (long)o;
            return long.TryParse(o.ToDoubleN()?.Round().ToStringInvariant(), NumberStyles.Any, CultureInfo.InvariantCulture, out var val) ? val : (long?)null;
        }

        public static long ToLong(this object o)
        {
            var longN = o.ToLongN();
            if (longN != null) return (long)longN;
            throw new ArgumentNullException(nameof(o));
        }
    }
}
