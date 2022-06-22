using System;
using CommonLib.Source.Common.Utils.TypeUtils;
using static CommonLib.Source.LibConfig;

namespace CommonLib.Source.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string MonthName(this DateTime date)
        {
            return CULTURE.DateTimeFormat.GetMonthName(date.Month);
        }

        public static DateTime Period(this DateTime date, int periodInDays)
        {
            var startDate = new DateTime();
            var myDate = new DateTime(date.Year, date.Month, date.Day);
            var diff = myDate - startDate;
            return myDate.AddDays(-(diff.TotalDays % periodInDays));
        }

        public static bool BetweenExcl(this DateTime d, DateTime greaterThan, DateTime lesserThan)
        {
            TUtils.SwapIf((gt, lt) => gt > lt, ref greaterThan, ref lesserThan);
            return d > greaterThan && d < lesserThan;
        }

        public static bool Between(this DateTime d, DateTime greaterThan, DateTime lesserThan)
        {
            TUtils.SwapIf((gt, lt) => gt > lt, ref greaterThan, ref lesserThan);
            return d >= greaterThan && d <= lesserThan;
        }

        public static DateTime SubtractDays(this DateTime dt, int days)
        {
            return dt.AddDays(-days);
        }

        public static DateTime SubtractYears(this DateTime dt, int years)
        {
            return dt.AddYears(-years);
        }

        public static DateTime RoundUp(this DateTime dt, TimeSpan ts)
        {
            return new DateTime((dt.Ticks + ts.Ticks - 1) / ts.Ticks * ts.Ticks);
        }

        public static DateTime RoundDown(this DateTime dt, TimeSpan ts)
        {
            return new DateTime(dt.Ticks / ts.Ticks * ts.Ticks);
        }
    }
}
