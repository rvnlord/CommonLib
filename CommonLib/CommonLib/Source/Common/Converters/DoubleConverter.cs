using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Converters
{
    public static class DoubleConverter
    {
        public static double? ToDoubleN(this object o)
        {
            if (o is null) return null;
            if (o is bool) return Convert.ToDouble(o, CultureInfo.InvariantCulture);
            var strD = o.ToStringInvariant()?.ReplaceInvariant(",", ".");
            if (strD is null) return null;

            var isNegative = strD.StartsWithInvariant("-");
            if (isNegative || strD.StartsWithInvariant("+"))
                strD = strD.Skip(1);

            var parsedVal = double.TryParse(strD, NumberStyles.Any, CultureInfo.InvariantCulture, out var tmpvalue) ? tmpvalue : (double?)null;
            if (isNegative)
                parsedVal = -parsedVal;
            return parsedVal;
        }

        public static double ToDouble(this object o)
        {
            var doubleN = o.ToDoubleN();
            if (doubleN != null) return (double)doubleN;
            throw new ArgumentNullException(nameof(o));
        }

        public static string ToStringInvariant(this double d, string separator)
        {
            return d.ToString(new NumberFormatInfo { NumberDecimalSeparator = separator });
        }

        public static double? TryToDouble(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            str = str.Replace(',', '.');

            var ending = new[] { "rem", "em", "px", "%" }.FirstOrDefault(str.EndsWithInvariant);
            if (ending != null)
                str = str.BeforeLast(ending);

            var isParsable = double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
            if (isParsable)
                return value;
            return null;
        }

        public static double ToDouble(this string str)
        {
            var parsedD = str.TryToDouble();
            if (parsedD == null)
                throw new InvalidCastException("Nie można sparsować wartości double");
            return (double)parsedD;
        }

        public static async Task<double> ToDoubleAsync(this Task<string> s)
        {
            return (await (s ?? throw new NullReferenceException(nameof(s))).ConfigureAwait(false)).ToDouble();
        }

        public static async Task<double> ToDoubleAsync(this ValueTask<string> s)
        {
            return (await s.ConfigureAwait(false)).ToDouble();
        }
    }
}
