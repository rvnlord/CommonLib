using System;
using System.Globalization;
using System.Threading.Tasks;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Source.Common.Converters
{
    public static class TimeConverter
    {
        public static DateTime? ToDateTimeN(this object o)
        {
            return DateTime.TryParse(o?.ToString(), out var tmpvalue) ? tmpvalue : (DateTime?)null;
        }

        public static DateTime ToDateTime(this object o)
        {
            var DateTimeN = o.ToDateTimeN();
            if (DateTimeN != null) return (DateTime)DateTimeN;
            throw new ArgumentNullException(nameof(o));
        }

        public static DateTime ToDateTime(this object o, string format)
        {
            return DateTime.ParseExact(o?.ToString(), format, CultureInfo.InvariantCulture);
        }

        public static DateTime ToDMY(this DateTime dateTime)
        {
            var date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            return date;
        }

        public static DateTime? ToDMY(this DateTime? dateTimeNullable)
        {
            if (dateTimeNullable == null)
                return null;

            var date = (DateTime)dateTimeNullable;
            date = new DateTime(date.Year, date.Month, date.Day);
            return date;
        }

        public static DateTime As(this DateTime dateTime, DateTimeKind kind) => DateTime.SpecifyKind(dateTime, kind);

        public static ExtendedTime ToExtendedTime(this DateTime dt, TimeZoneKind tz = TimeZoneKind.UTC) => new(dt, tz);
        public static ExtendedTime ToExtendedTime(this long l) => new(l);

        public static ExtendedTime ToExtendedTime(this object o, string format = null, TimeZoneKind tz = TimeZoneKind.UTC)
        {
            var extTime = o.ToExtendedTimeN(format, tz);
            if (extTime == null)
                throw new InvalidCastException(nameof(o));
            return extTime;
        }

        public static ExtendedTime ToExtendedTimeN(this object o, string format = null, TimeZoneKind tz = TimeZoneKind.UTC)
        {
            if (o?.ToString() == null || o.ToString().IsNullOrWhiteSpace())
                return null;

            if (o.ToDoubleN() != null)
                return o.ToDouble().ToExtendedTime();

            var strDate = o.ToString() ?? throw new NullReferenceException();

            if (strDate.ContainsLetters() && strDate.ContainsAll(" ", "(", ")"))
            {
                //var time = new ExtendedTime(
                //    new DateTime(
                //        strDate.After("-", 2).BeforeFirst(" ").ToInt(),
                //        strDate.AfterFirst("-").BeforeFirst("-").ToInt(),
                //        strDate.AfterFirst(" ").BeforeFirst("-").ToInt(),
                //        strDate.BeforeFirst(":").ToInt(),
                //        strDate.AfterFirst(":").BeforeFirst(":").ToInt(),
                //        strDate.After(":", 2).BeforeFirst(" ").ToInt()),
                //    strDate.After(" ", 2).BeforeFirst(" (").Remove(" ").ToEnum<TimeZoneKind>());

                var tmz = strDate.After(" ", 2).BeforeFirst(" (").Remove(" ").ToEnum<TimeZoneKind>();
                var ts = strDate.Between("(", ")").ToDouble();

                return new ExtendedTime(ts, tmz);
            }

            var parsedDateTime = format != null
                ? DateTime.ParseExact(strDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
                : DateTime.Parse(strDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            return parsedDateTime.ToExtendedTime(tz);
        }

        public static UnixTimestamp ToUnixTimestamp(this DateTime dateTime) => new(dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        public static UnixTimestamp ToUnixTimestamp(this long l) => new(l);
        public static UnixTimestamp ToUnixTimestamp(this double d) => new(d);
        public static ExtendedTime ToExtendedTime(this double unixTimestamp, TimeZoneKind timeZone = TimeZoneKind.UTC) => new(unixTimestamp, timeZone);
        
        public static async Task<ExtendedTime> ToExtendedTimeAsync(this Task<DateTime> dtTask, TimeZoneKind tz = TimeZoneKind.UTC)
        {
            if (dtTask == null)
                throw new NullReferenceException(nameof(dtTask));

            var dt = await dtTask.ConfigureAwait(false);
            return dt.ToExtendedTime(tz);
        }

        public static async Task<ExtendedTime> ToExtendedTimeAsync(this ValueTask<DateTime> dtTask, TimeZoneKind tz = TimeZoneKind.UTC)
        {
            var dt = await dtTask.ConfigureAwait(false);
            return dt.ToExtendedTime(tz);
        }

        public static async Task<ExtendedTime> ToExtendedTimeNAsync(this Task<DateTime> dtTask, string format = null, TimeZoneKind tz = TimeZoneKind.UTC)
        {
            if (dtTask == null)
                throw new NullReferenceException(nameof(dtTask));

            var dt = await dtTask.ConfigureAwait(false);
            return dt.ToExtendedTimeN(format, tz);
        }

        public static async Task<ExtendedTime> ToExtendedTimeNAsync(this ValueTask<DateTime> dtTask, string format = null, TimeZoneKind tz = TimeZoneKind.UTC)
        {
            var dt = await dtTask.ConfigureAwait(false);
            return dt.ToExtendedTimeN(format, tz);
        }

        public static DateTime UnixTimeStampToDateTime(this long unix) => new DateTime(1970, 1, 1).AddSeconds(unix);
    }
}
