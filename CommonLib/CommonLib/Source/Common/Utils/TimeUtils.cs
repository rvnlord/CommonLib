using System;
using CommonLib.Source.Common.Converters;

namespace CommonLib.Source.Common.Utils
{
    public static class TimeUtils
    {
        public static long UnixTimeStampUtcNow()
        {
            return DateTime.UtcNow.ToUnixTimestamp().ToLong();
        }
    }
}
