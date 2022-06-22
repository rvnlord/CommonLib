using System;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal Round(this decimal m, int precision = 0)
        {
            return Math.Round(m, precision);
        }

        public static string ToStringWithSeparator(this decimal n, string decimalSeparator)
        {
            return n.ToStringInvariant().ReplaceMany(new[] { ",", "." }, decimalSeparator);
        }

        public static decimal Clamp(this decimal value, decimal minValue, decimal maxValue, decimal? stepSize)
        {
            if (minValue < 0)
                throw new ArgumentOutOfRangeException(nameof(minValue));
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue));

            if (stepSize.HasValue)
            {
                if (stepSize < 0)
                    throw new ArgumentOutOfRangeException(nameof(stepSize));
                var mod = value % stepSize.Value;
                value -= mod;
            }

            if (maxValue > 0)
                value = Math.Min(maxValue, value);

            value = Math.Max(minValue, value);

            return value.RemoveTrailingZeroes();
        }

        public static decimal RemoveTrailingZeroes(this decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }
    }
}
