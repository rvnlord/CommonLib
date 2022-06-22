using System;
using System.Globalization;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class DecimalConverter
    {
        public static decimal? ToDecimalN(this object o)
        {
            if (o == null) return null;
            if (o is bool) return Convert.ToDecimal(o, CultureInfo.InvariantCulture);
            return decimal.TryParse(o.ToStringInvariant().ReplaceInvariant(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpvalue) ? tmpvalue : (decimal?)null;
        }

        public static decimal ToDecimal(this object o)
        {
            var decimalN = o.ToDecimalN();
            if (decimalN != null) return (decimal)decimalN;
            throw new ArgumentNullException(nameof(o));
        }
    }
}
