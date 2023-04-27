using System;
using System.Globalization;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class UshortConverter
    {
        public static ushort? ToUshortN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return Convert.ToUInt16(o, CultureInfo.InvariantCulture);
            if (o.GetType().IsEnum) return (ushort)o;
            return ushort.TryParse(o.ToDoubleN()?.Round().ToStringInvariant().BeforeFirstOrWhole("."), NumberStyles.Any, CultureInfo.InvariantCulture, out var val) ? val : (ushort?)null;
        }

        public static ushort ToUshort(this object o)
        {
            var intN = o.ToUshortN();
            if (intN != null) return (ushort)intN;
            throw new ArgumentNullException(nameof(o));
        }

        public static string ToStringInvariant(this ushort u, string format)
        {
            return u.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
