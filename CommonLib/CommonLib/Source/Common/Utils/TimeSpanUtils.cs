using System;

namespace CommonLib.Source.Common.Utils
{
    public static class TimeSpanUtils
    {
        public static TimeSpan FromApproximateYears(int years) => TimeSpan.FromDays(years * 365.2425);
        public static TimeSpan FromApproximateMonths(int months) => TimeSpan.FromDays((double)months / 12 * 365.2425);
    }
}
