using System.Globalization;

namespace CommonLib.Source.Common.Converters
{
    public static class ByteConverter
    {
        public static string ToStringInvariant(this byte b, string format)
        {
            return b.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
