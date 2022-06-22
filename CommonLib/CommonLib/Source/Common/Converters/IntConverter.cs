using System;
using System.Globalization;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class IntConverter
    {
        public static int? ToIntN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return Convert.ToInt32(o, CultureInfo.InvariantCulture);
            if (o.GetType().IsEnum) return (int)o;
            return int.TryParse(o.ToDoubleN()?.Round().ToStringInvariant().BeforeFirstOrWhole("."), NumberStyles.Any, CultureInfo.InvariantCulture, out var val) ? val : (int?)null;
        }

        public static int ToInt(this object o)
        {
            var intN = o.ToIntN();
            if (intN != null) return (int)intN;
            throw new ArgumentNullException(nameof(o));
        }

        public static string ToStringInvariant(this int i, string format)
        {
            return i.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
