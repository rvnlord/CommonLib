using System;

namespace CommonLib.Source.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Multiply(this TimeSpan multiplicand, int multiplier)
        {
            return TimeSpan.FromTicks(multiplicand.Ticks * multiplier);
        }

        public static TimeSpan Multiply(this TimeSpan multiplicand, double multiplier)
        {
            return TimeSpan.FromTicks((long)(multiplicand.Ticks * multiplier));
        }

        public static TimeSpan Multiply(this TimeSpan multiplicand, TimeSpan multiplier)
        {
            return TimeSpan.FromTicks(multiplicand.Ticks * multiplier.Ticks);
        }

        public static TimeSpan Divide(this TimeSpan multiplicand, int by)
        {
            return TimeSpan.FromTicks(multiplicand.Ticks / by);
        }

        public static TimeSpan Divide(this TimeSpan multiplicand, double by)
        {
            return TimeSpan.FromTicks((long)(multiplicand.Ticks / by));
        }

        public static TimeSpan Divide(this TimeSpan multiplicand, TimeSpan by)
        {
            return TimeSpan.FromTicks(multiplicand.Ticks / by.Ticks);
        }
    }
}
